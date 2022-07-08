using System.Collections.ObjectModel;
using Umbraco.Cms.Core.Models;

namespace Umbraco.New.Cms.Core.Models.Installer;

public class InstallData
{
    public UserInstallData User { get; set; } = null!;

    public DatabaseInstallData Database { get; set; } = null!;

    public TelemetryLevel TelemetryLevel { get; set; } = TelemetryLevel.Basic;

    public ICollection<ICustomInstallStepModel> CustomModels { get; set; } = new List<ICustomInstallStepModel>();
}
