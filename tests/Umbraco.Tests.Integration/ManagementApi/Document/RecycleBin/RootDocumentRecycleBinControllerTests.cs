using System.Linq.Expressions;
using System.Net;
using Umbraco.Cms.Api.Management.Controllers.Document.RecycleBin;

namespace Umbraco.Cms.Tests.Integration.ManagementApi.Document.RecycleBin;

public class RootDocumentRecycleBinControllerTests : ManagementApiUserGroupTestBase<RootDocumentRecycleBinController>
{
    protected override Expression<Func<RootDocumentRecycleBinController, object>> MethodSelector =>
        x => x.Root( 0, 100);

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
