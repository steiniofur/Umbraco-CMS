using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Core.DistributedLocking.Exceptions;
using Umbraco.Cms.Core.Exceptions;
using Umbraco.Extensions;

namespace Umbraco.Cms.Persistence.Sqlite.Services;

/// <summary>
/// An InProcess implementation of <see cref="IDistributedLockingMechanism"/>.
/// <remarks>
/// <para>
/// This is in effect a non-distributed locking mechanism, as it only works within the current process.
/// </para>
/// <para>
/// It is intended to be used with SQLite, or any setup that doesn't support load balancing or multiple instances, and therefore does not actually require a distributed lock.
/// </para>
/// </remarks>
/// </summary>
public class InProcessDistributedLockingMechanism : IDistributedLockingMechanism
{
    private readonly ILogger<InProcessDistributedLockingMechanism> _logger;
    private ConnectionStrings _connectionStrings;
    private GlobalSettings _globalSettings;
    private ConcurrentDictionary<int, ReaderWriterLockSlim> _locks = new();

    public InProcessDistributedLockingMechanism(
        IOptionsMonitor<ConnectionStrings> connectionStrings,
        IOptionsMonitor<GlobalSettings> globalSettings,
        ILogger<InProcessDistributedLockingMechanism> logger)
    {
        _logger = logger;
        _connectionStrings = connectionStrings.CurrentValue;
        _globalSettings = globalSettings.CurrentValue;
        connectionStrings.OnChange(x => _connectionStrings = x);
        globalSettings.OnChange(x => _globalSettings = x);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="InProcessDistributedLockingMechanism"/> class.
    /// We have to dispose of the locks when the mechanism destroyed.
    /// </summary>
    ~InProcessDistributedLockingMechanism()
    {
        foreach (ReaderWriterLockSlim readerWriterLock in _locks.Values)
        {
            readerWriterLock.Dispose();
        }
    }

    // We only want to use this when using SQLite, in this case there is no need for an actual DistributedLock
    public bool Enabled => _connectionStrings.IsConnectionStringConfigured() &&
                           string.Equals(_connectionStrings.ProviderName, Constants.ProviderName, StringComparison.InvariantCultureIgnoreCase);

    public IDistributedLock ReadLock(int lockId, TimeSpan? obtainLockTimeout = null)
    {
        ReaderWriterLockSlim readerWriterLock = _locks.GetOrAdd(lockId, _ => new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion));

        obtainLockTimeout ??= _globalSettings.DistributedLockingReadLockDefaultTimeout;
        return new InProcessDistributedLock(this, lockId, DistributedLockType.ReadLock, readerWriterLock, obtainLockTimeout.Value);
    }

    public IDistributedLock WriteLock(int lockId, TimeSpan? obtainLockTimeout = null)
    {
        ReaderWriterLockSlim readerWriterLock = _locks.GetOrAdd(lockId, _ => new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion));

        obtainLockTimeout ??= _globalSettings.DistributedLockingReadLockDefaultTimeout;
        return new InProcessDistributedLock(this, lockId, DistributedLockType.WriteLock, readerWriterLock, obtainLockTimeout.Value);
    }

    private class InProcessDistributedLock : IDistributedLock
    {
        private readonly InProcessDistributedLockingMechanism _parent;
        private readonly ReaderWriterLockSlim _readerWriterLock;
        private readonly TimeSpan _timeout;

        public int LockId { get; }

        public DistributedLockType LockType { get; }

        public InProcessDistributedLock(
            InProcessDistributedLockingMechanism parent,
            int lockId,
            DistributedLockType lockType,
            ReaderWriterLockSlim readerWriterLock,
            TimeSpan timeout)
        {
            _parent = parent;
            _readerWriterLock = readerWriterLock;
            _timeout = timeout;
            LockId = lockId;
            LockType = lockType;

            if (_parent._logger.IsEnabled(LogLevel.Debug))
            {
                _parent._logger.LogDebug("Thread: {ThreadId} Requesting {lockType} for id {id}", Environment.CurrentManagedThreadId, LockType, LockId);
            }

            switch (lockType)
            {
                case DistributedLockType.ReadLock:
                    ObtainReadLock();
                    break;
                case DistributedLockType.WriteLock:
                    ObtainWriteLock();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lockType), lockType, @"Unsupported lockType");
            }

            if (_parent._logger.IsEnabled(LogLevel.Debug))
            {
                _parent._logger.LogDebug("Thread: {Threadid} Acquired {lockType} for id {id}", Environment.CurrentManagedThreadId, LockType, LockId);
            }
        }

        private void ObtainReadLock()
        {
            if (_readerWriterLock.TryEnterReadLock(_timeout))
            {
                return;
            }

            if (_parent._logger.IsEnabled(LogLevel.Debug))
            {
                _parent._logger.LogDebug("{lockType} for id {id} timed out", LockType, LockId);
            }

            throw new DistributedReadLockTimeoutException(LockId);
        }

        private void ObtainWriteLock()
        {
            if (_readerWriterLock.TryEnterWriteLock(_timeout))
            {
                return;
            }

            if (_parent._logger.IsEnabled(LogLevel.Debug))
            {
                _parent._logger.LogDebug("{lockType} for id {id} timed out", LockType, LockId);
            }

            throw new DistributedWriteLockTimeoutException(LockId);
        }

        public void Dispose()
        {
            // Note we don't want to actually dispose the lock here. We want to keep the lock around for other threads
            // The LockingMechanism will dispose of the locks when it is destroyed.
            switch (LockType)
            {
                case DistributedLockType.ReadLock:
                    if (_parent._logger.IsEnabled(LogLevel.Debug))
                    {
                        _parent._logger.LogDebug("Thread: {ThreadId} Releasing {lockType} for id {id}", Environment.CurrentManagedThreadId, LockType, LockId);
                    }

                    _readerWriterLock.ExitReadLock();

                    break;
                case DistributedLockType.WriteLock:
                    if (_parent._logger.IsEnabled(LogLevel.Debug))
                    {
                        _parent._logger.LogDebug("Thread: {ThreadId} Releasing {lockType} for id {id}", Environment.CurrentManagedThreadId, LockType, LockId);
                    }

                    _readerWriterLock.ExitWriteLock();
                    break;
                default:
                    // This should very much never happen
                    throw new PanicException("Unsupported lockType in lock dispose, unable to release lock");
            }
        }
    }
}
