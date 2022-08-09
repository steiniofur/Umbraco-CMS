using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.DependencyInjection;
using Umbraco.Cms.Web.Common.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
///     Umbraco specific extensions for the <see cref="IHostBuilder" /> interface.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    ///     Configures an existing <see cref="IHostBuilder" /> with defaults for an Umbraco application.
    /// </summary>
    public static IHostBuilder ConfigureUmbracoDefaults(this IHostBuilder builder)
    {
#if DEBUG
        builder.ConfigureAppConfiguration(config
            => config.AddJsonFile(
                "appsettings.Local.json",
                true,
                true));

#endif
        // Get runtime mode from RuntimeHostConfigurationOption
        builder.ConfigureHostOptions((context, options) =>
        {
            if (string.IsNullOrEmpty(context.Configuration[Constants.Configuration.ConfigRuntimeMode]))
            {
                var runtimeMode = AppContext.GetData("Umbraco.Cms.RuntimeMode")?.ToString();
                if (bool.TryParse(runtimeMode, out var useRuntimeMode) && useRuntimeMode)
                {
                    var mode = context.HostingEnvironment.EnvironmentName switch
                    {
                        "Production" => "Production",
                        "Staging" => "Production",
                        _ => "Development"
                    };

                    context.Configuration[Constants.Configuration.ConfigRuntimeMode] = mode;
                }
            }
        });

        builder.ConfigureLogging(x => x.ClearProviders());

        return new UmbracoHostBuilderDecorator(builder, OnHostBuilt);
    }

    // Runs before any IHostedService starts (including generic web host).
    private static void OnHostBuilt(IHost host) =>
        StaticServiceProvider.Instance = host.Services;
}
