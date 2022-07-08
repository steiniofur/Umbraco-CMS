using Umbraco.Cms.Core.Install.Models;
using Umbraco.New.Cms.Core.Installer;

namespace Umbraco.Cms.Web.UI;

public class MyCustomInstallStep : ICustomInstallStep
{
    private readonly ILogger<MyCustomInstallStep> _logger;

    public MyCustomInstallStep(ILogger<MyCustomInstallStep> logger)
    {
        _logger = logger;
    }

    public Guid StepKey => Guid.Parse("78DC45EF-7411-4E49-9A22-03FE04BE4FBB");

    public Type ModelType => typeof(MyCustomInstallModel);

    public InstallationType InstallationTypeTarget => InstallationType.NewInstall;

    public Task ExecuteAsync(object model)
    {
        if (model is MyCustomInstallModel customModel)
        {
            _logger.LogInformation("Custom step run, with model, name: {name}", customModel.Name);
        }
        else
        {
            _logger.LogInformation("Custom install step ran, but couldn't get model :(");
        }

        return Task.CompletedTask;
    }

    public Task<bool> RequiresExecutionAsync(object model) => Task.FromResult(true);
}
