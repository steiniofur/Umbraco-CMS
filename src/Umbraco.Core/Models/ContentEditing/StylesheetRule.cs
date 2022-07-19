using System.Runtime.Serialization;

namespace Umbraco.Cms.Core.Models.ContentEditing
{
    [DataContract(Name = "stylesheetRule", Namespace = "")]
    public class StylesheetRule
    {
        [DataMember(Name = "name")]
        public required string Name { get; set; }

        [DataMember(Name = "selector")]
        public required string Selector { get; set; }

        [DataMember(Name = "styles")]
        public required string Styles { get; set; }
    }
}
