using Umbraco.Cms.Core;

namespace Umbraco.Cms.Api.Management.ViewModels.Tree;

public class ScriptTreeItemResponseModel : FileSystemTreeItemPresentationModel
{
    public string Type => Constants.UdiEntityType.Script;
}