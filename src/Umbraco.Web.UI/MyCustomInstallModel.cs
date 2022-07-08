using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Umbraco.Cms.ManagementApi.Binding;
using Umbraco.New.Cms.Core.Installer;
using Umbraco.New.Cms.Core.Models.Installer;

namespace Umbraco.Cms.Web.UI;

[DataContract(Name ="myCustomInstallModel")]
public class MyCustomInstallModel : ICustomInstallStepModel
{
    [DataMember(Name = "name")]
    [Required]
    [MinLength(10)]
    public string Name { get; set; } = null!;

    [DataMember(Name = "stepKey")]
        public Guid StepKey => Guid.Parse("78DC45EF-7411-4E49-9A22-03FE04BE4FBB");
}
