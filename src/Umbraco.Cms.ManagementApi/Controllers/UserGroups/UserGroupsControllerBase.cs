using Microsoft.AspNetCore.Mvc;
using Umbraco.New.Cms.Web.Common.Routing;

namespace Umbraco.Cms.ManagementApi.Controllers.UserGroups;

[ApiController]
[VersionedApiBackOfficeRoute("user-groups")]
[ApiExplorerSettings(GroupName = "User Groups")]
[ApiVersion("1.0")]
public abstract class UserGroupsControllerBase : ManagementApiControllerBase
{
}
