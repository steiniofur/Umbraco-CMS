using NUnit.Framework;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.PropertyEditors;

// Test Tags and Reference Tracking

[TestFixture]
[UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
public class BlockListPropertyEditorTests : UmbracoIntegrationTest
{
    private IDataTypeService DataTypeService => GetRequiredService<IDataTypeService>();

    private IContentTypeService ContentTypeService => GetRequiredService<IContentTypeService>();

    private IContentService ContentService => GetRequiredService<IContentService>();

    [Test]
    public void Can_Track_Block_List_References()
    {
        var blockListDataType = DataTypeBuilder.CreateSimpleBlockListEditorDataType();

        DataTypeService.Save(blockListDataType);

        var contentType =
            ContentTypeBuilder.CreateContentTypeWithDataType(blockListDataType.Id, "BlockListTest", "blockListTest");
        ContentTypeService.Save(contentType);

        var content = ContentBuilder.CreateBasicContent(contentType);

        ContentService.Save(content);

        var references = DataTypeService.GetReferences(blockListDataType.Id).ToArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, references.Length);
            var reference = references.First();
            Assert.AreEqual(contentType.PropertyTypes.First().Alias, reference.Value.First());
            Assert.AreEqual(contentType.GetUdi(), reference.Key);
        });
    }
}
