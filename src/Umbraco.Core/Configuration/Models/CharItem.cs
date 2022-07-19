using Umbraco.Cms.Core.Configuration.UmbracoSettings;

namespace Umbraco.Cms.Core.Configuration.Models
{
    public class CharItem : IChar
    {
        /// <summary>
        /// The character to replace
        /// </summary>
        public required string Char { get; set; }

        /// <summary>
        /// The replacement character
        /// </summary>
        public required string Replacement { get; set; }
    }
}
