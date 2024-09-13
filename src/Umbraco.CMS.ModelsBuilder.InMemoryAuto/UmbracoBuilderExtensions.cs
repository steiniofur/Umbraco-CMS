using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Umbraco.CMS.ModelsBuilder.InMemoryAuto;

public static class UmbracoBuilderExtensions
{
    public static IUmbracoBuilder AddInMemoryModelsRazorEngine(this IUmbracoBuilder builder)
    {
        // We should only add/replace these services when models builder is InMemory, otherwise we'll cause issues.
        // Since these services expect the ModelsMode to be InMemoryAuto
        if (builder.Config.GetModelsMode() is ModelsMode.InMemoryAuto)
        {
            builder.Services.AddSingleton<UmbracoRazorReferenceManager>();
            builder.Services.AddSingleton<CompilationOptionsProvider>();
            builder.Services.AddSingleton<IViewCompilerProvider, UmbracoViewCompilerProvider>();
            builder.Services.AddSingleton<RuntimeCompilationCacheBuster>();
            builder.Services.AddSingleton<InMemoryAssemblyLoadContextManager>();

            builder.Services.AddSingleton<InMemoryModelFactory>();
            // Register the factory as IPublishedModelFactory
            builder.Services.AddSingleton<IPublishedModelFactory, InMemoryModelFactory>();

            builder.RuntimeModeValidators()
                .Add<InMemoryModelsBuilderModeValidator>();

            return builder;
        }

        // This is what the community MB would replace, all of the above services are fine to be registered
        builder.Services.AddSingleton<IPublishedModelFactory>(factory => factory.CreateDefaultPublishedModelFactory());

        return builder;
    }
}
