using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Core.Helpers;

public class PropertyValueManipulationHelper
{
    public delegate IContentTypeComposition? GetContentTypeFromKeyDelegate(Guid key);

    public GetContentTypeFromKeyDelegate? GetContentTypeFromKeyMethod { get; set; } //todo elements nullable or not?
}
