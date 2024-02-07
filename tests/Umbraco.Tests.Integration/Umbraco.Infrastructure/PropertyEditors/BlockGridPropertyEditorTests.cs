using NUnit.Framework;
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
    private IMediaTypeService MediaTypeService => GetRequiredService<IMediaTypeService>();
    private IMemberTypeService MemberTypeService => GetRequiredService<IMemberTypeService>();

    private const string DataTypeName = "BlockGridTest";
    private const string DataTypeAlias = "blockGridTest";

    // ContentTypes
    [Test]
    public void Can_Track_Block_Grid_References_Content_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateSimpleBlockGridEditorDataType();

        DataTypeService.Save(blockGridDataType);

        var contentType =
            ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias);
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

    [Test]
    public void Can_Track_Block_Grid_References_With_Multiple_References_Content_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateSimpleBlockGridEditorDataType();

        DataTypeService.Save(blockGridDataType);

        var firstContentType =
            ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias);
        ContentTypeService.Save(firstContentType);

        var secondContentType = ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, DataTypeName,
            DataTypeAlias, "blockGridDocumentTypeTestTwo", "blockGridDocumentTypeTestTwo");
        ContentTypeService.Save(secondContentType);

        var references = DataTypeService.GetReferences(blockGridDataType.Id).ToArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(2, references.Length);
            var reference = references.First();
            Assert.AreEqual(firstContentType.PropertyTypes.First().Alias, reference.Value.First());
            Assert.AreEqual(firstContentType.GetUdi(), reference.Key);
            var reference2 = references.Last();
            Assert.AreEqual(secondContentType.PropertyTypes.First().Alias, reference2.Value.First());
            Assert.AreEqual(secondContentType.GetUdi(), reference2.Key);
        });
    }

    // MediaTypes
    [Test]
    public void Can_Track_Block_Grid_References_Media_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateSimpleBlockGridEditorDataType();

        DataTypeService.Save(blockGridDataType);

        var mediaType =
            MediaTypeBuilder.CreateSimpleMediaTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias);
        MediaTypeService.Save(mediaType);

        var references = DataTypeService.GetReferences(blockGridDataType.Id).ToArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, references.Length);
            var reference = references.First();
            Assert.AreEqual(mediaType.PropertyTypes.First().Alias, reference.Value.First());
            Assert.AreEqual(mediaType.GetUdi(), reference.Key);
        });
    }

    [Test]
    public void Can_Track_Block_Grid_References_With_Multiple_References_Media_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateSimpleBlockGridEditorDataType();

        DataTypeService.Save(blockGridDataType);

        var firstMediaType =
            MediaTypeBuilder.CreateSimpleMediaTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias);
        MediaTypeService.Save(firstMediaType);

        var secondMediaType = MediaTypeBuilder.CreateSimpleMediaTypeWithDataType(blockGridDataType.Id, DataTypeName,
            DataTypeAlias, "blockGridMediaTypeTestTwo", "blockGridMediaTypeTestTwo");
        MediaTypeService.Save(secondMediaType);

        var references = DataTypeService.GetReferences(blockGridDataType.Id).ToArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(2, references.Length);
            var reference = references.First();
            Assert.AreEqual(firstMediaType.PropertyTypes.First().Alias, reference.Value.First());
            Assert.AreEqual(firstMediaType.GetUdi(), reference.Key);
            var reference2 = references.Last();
            Assert.AreEqual(secondMediaType.PropertyTypes.First().Alias, reference2.Value.First());
            Assert.AreEqual(secondMediaType.GetUdi(), reference2.Key);
        });
    }

    // MemberTypes
    [Test]
    public void Can_Track_Block_Grid_References_Member_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateSimpleBlockGridEditorDataType();

        DataTypeService.Save(blockGridDataType);

        var memberType =
            MemberTypeBuilder.CreateSimpleMemberTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias);
        MemberTypeService.Save(memberType);

        var references = DataTypeService.GetReferences(blockGridDataType.Id).ToArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(1, references.Length);
            var reference = references.First();
            Assert.AreEqual(memberType.PropertyTypes.First().Alias, reference.Value.First());
            Assert.AreEqual(memberType.GetUdi(), reference.Key);
        });
    }

    [Test]
    public void Can_Track_Block_Grid_References_With_Multiple_References_Member_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateSimpleBlockGridEditorDataType();

        DataTypeService.Save(blockGridDataType);

        var firstMemberType = MemberTypeBuilder.CreateSimpleMemberTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias);
        MemberTypeService.Save(firstMemberType);

        var secondMemberType = MemberTypeBuilder.CreateSimpleMemberTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, "blockGridMemberTypeTestTwo", "blockGridMemberTypeTestTwo");
        MemberTypeService.Save(secondMemberType);

        var references = DataTypeService.GetReferences(blockGridDataType.Id).ToArray();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(2, references.Length);
            var reference = references.First();
            Assert.AreEqual(firstMemberType.PropertyTypes.First().Alias, reference.Value.First());
            Assert.AreEqual(firstMemberType.GetUdi(), reference.Key);
            var reference2 = references.Last();
            Assert.AreEqual(secondMemberType.PropertyTypes.First().Alias, reference2.Value.First());
            Assert.AreEqual(secondMemberType.GetUdi(), reference2.Key);
        });
    }
}

