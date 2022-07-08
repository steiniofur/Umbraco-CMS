using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.New.Cms.Core.Installer;
using Umbraco.New.Cms.Core.Models.Installer;

namespace Umbraco.Cms.ManagementApi.Binding;

public class CustomInstallStepModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        Type modelType = context.Metadata.ModelType;

        if (modelType != typeof(ICustomInstallStepModel))
        {
            return null;
        }

        CustomInstallStepCollection customInstallSteps = context.Services.GetRequiredService<CustomInstallStepCollection>();
        var customSteps = customInstallSteps.ToList();
        IEnumerable<Type> customTypes = customSteps.Select(x => x.ModelType);

        Dictionary<Type, (ModelMetadata, IModelBinder)> binders = new();

        foreach (Type type in customTypes)
        {
            ModelMetadata modelMetadata = context.MetadataProvider.GetMetadataForType(type);
            binders[type] = (modelMetadata, context.CreateBinder(modelMetadata));
        }

        return new CustomInstallStepModelBinder(binders, customInstallSteps);
    }
}
