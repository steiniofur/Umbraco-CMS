using Umbraco.Cms.Core.Install.Models;

namespace Umbraco.New.Cms.Core.Installer;

public interface ICustomInstallStep
{
    public Guid StepKey { get; }

    public Type ModelType { get; }

    InstallationType InstallationTypeTarget { get; }

    Task ExecuteAsync(object model);

    Task<bool> RequiresExecutionAsync(object model);
}
