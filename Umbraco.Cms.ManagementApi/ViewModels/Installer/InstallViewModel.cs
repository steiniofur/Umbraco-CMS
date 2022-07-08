using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models;
using Umbraco.New.Cms.Core.Models.Installer;

namespace Umbraco.Cms.ManagementApi.ViewModels.Installer;

public class InstallViewModel
{
    [DataMember(Name = "user")]
    [Required]
    public UserInstallViewModel User { get; init; } = null!;

    [DataMember(Name = "database")]
    [Required]
    public DatabaseInstallViewModel Database { get; init; } = null!;

    [DataMember(Name = "telemetryLevel")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TelemetryLevel TelemetryLevel { get; init; } = TelemetryLevel.Basic;

    [DataMember(Name = "customModels")]
    public Dictionary<string, object> CustomModels { get; set; } = new();

    [IgnoreDataMember]
    public ICollection<ICustomInstallStepModel> BoundCustomModels { get; set; } = new List<ICustomInstallStepModel>();
}
