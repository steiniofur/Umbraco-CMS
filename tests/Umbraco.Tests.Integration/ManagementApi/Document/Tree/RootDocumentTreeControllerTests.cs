using System.Linq.Expressions;
using System.Net;
using Umbraco.Cms.Api.Management.Controllers.Document.Tree;

namespace Umbraco.Cms.Tests.Integration.ManagementApi.Document.Tree;

public class RootDocumentTreeControllerTests : ManagementApiUserGroupTestBase<RootDocumentTreeController>
{
    protected override Expression<Func<RootDocumentTreeController, object>> MethodSelector =>
        x => x.Root(0, 100, null, null);

    protected override UserGroupAssertionModel AdminUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.OK
    };

    protected override UserGroupAssertionModel EditorUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.OK
    };

    protected override UserGroupAssertionModel SensitiveDataUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Forbidden,
    };

    protected override UserGroupAssertionModel TranslatorUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Forbidden
    };

    protected override UserGroupAssertionModel WriterUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.OK
    };

    protected override UserGroupAssertionModel UnauthorizedUserGroupAssertionModel => new()
    {
        ExpectedStatusCode = HttpStatusCode.Unauthorized
    };
}
