// Copyright (c) Umbraco.
// See LICENSE for more details.

using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Persistence.Sqlite.Services;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.Persistence;

[TestFixture]
[UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
public class UnitOfWorkTests : UmbracoIntegrationTest
{
    [Test]
    public void ReadLockExisting()
    {
        var provider = ScopeProvider;
        using (var scope = provider.CreateScope())
        {
            scope.EagerReadLock(Constants.Locks.Servers);
            scope.Complete();
        }
    }

    [Test]
    public void WriteLockExisting()
    {
        var provider = ScopeProvider;
        using (var scope = provider.CreateScope())
        {
            scope.EagerWriteLock(Constants.Locks.Servers);
            scope.Complete();
        }
    }
}
