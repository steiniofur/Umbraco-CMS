using Microsoft.AspNetCore.Mvc.ModelBinding;
using Umbraco.New.Cms.Core.Installer;
using Umbraco.New.Cms.Core.Models.Installer;

namespace Umbraco.Cms.ManagementApi.Binding;

public class CustomInstallStepModelBinder : IModelBinder
{
    private readonly IDictionary<Type, (ModelMetadata, IModelBinder)> _binders;
    private readonly CustomInstallStepCollection _customInstallSteps;

    public CustomInstallStepModelBinder(
        IDictionary<Type, (ModelMetadata, IModelBinder)> binders,
        CustomInstallStepCollection customInstallSteps)
    {
        _binders = binders;
        _customInstallSteps = customInstallSteps;
    }

    // public CustomInstallStepModelBinder()
    // {
    // }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var modelKindName =
            ModelNames.CreatePropertyModelName(bindingContext.ModelName, nameof(ICustomInstallStepModel.StepKey));
        var modelTypeValue = bindingContext.ValueProvider.GetValue(modelKindName).FirstValue;
        // var type = _customInstallSteps.Where(x => x.StepKey )
    }
}
