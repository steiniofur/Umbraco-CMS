using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.PropertyEditors;

// Test Tags and Reference Tracking

[TestFixture]
[UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
public class BlockGridPropertyEditorTests : UmbracoIntegrationTest
{
    private IDataTypeService DataTypeService => GetRequiredService<IDataTypeService>();

    private IContentTypeService ContentTypeService => GetRequiredService<IContentTypeService>();

    private IContentService ContentService => GetRequiredService<IContentService>();

    [Test]
    public void Can_Track_Block_Grid_References()
    {
        var blockGridDataType = DataTypeBuilder.CreateSimpleBlockGridEditorDataType();

        DataTypeService.Save(blockGridDataType);

        var contentType =
            ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, "BlockGridTest", "blockGridTest");
        ContentTypeService.Save(contentType);

        var content = ContentBuilder.CreateBasicContent(contentType);

        ContentService.Save(content);

        var references = DataTypeService.GetReferences(blockGridDataType.Id).ToArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, references.Length);
            var reference = references.First();
            Assert.AreEqual(contentType.PropertyTypes.First().Alias, reference.Value.First());
            Assert.AreEqual(contentType.GetUdi(), reference.Key);
        });
    }
}
