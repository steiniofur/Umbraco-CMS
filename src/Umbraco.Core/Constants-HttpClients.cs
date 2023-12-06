namespace Umbraco.Cms.Core;

/// <summary>
/// Defines constants.
/// </summary>
public static partial class Constants
{
    /// <summary>
    /// Defines constants for named HTTP clients.
    /// </summary>
    public static class HttpClients
    {
        /// <summary>
        /// Name of the configured HTTP client which ignores certificate errors.
        /// </summary>
        /// <remarks>
        /// This HTTP client is used by the Keep-Alive background job.
        /// </remarks>
        public const string IgnoreCertificateErrors = "Umbraco:HttpClients:IgnoreCertificateErrors";
    }
}
