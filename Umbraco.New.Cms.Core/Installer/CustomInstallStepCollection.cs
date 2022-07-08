using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Install.Models;

namespace Umbraco.New.Cms.Core.Installer;

public class CustomInstallStepCollection : BuilderCollectionBase<ICustomInstallStep>
{
    public CustomInstallStepCollection(Func<IEnumerable<ICustomInstallStep>> items) : base(items)
    {
    }

    public IEnumerable<ICustomInstallStep> GetInstallSteps()
        => this.Where(x => x.InstallationTypeTarget.HasFlag(InstallationType.NewInstall));

    public IEnumerable<ICustomInstallStep> GetUpgradeSteps()
        => this.Where(x => x.InstallationTypeTarget.HasFlag(InstallationType.Upgrade));
}
