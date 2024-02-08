using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Testing;
using Umbraco.Cms.Tests.Integration.Testing;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.PropertyEditors;

[TestFixture]
[UmbracoTest(Database = UmbracoTestOptions.Database.NewSchemaPerTest)]
public class BlockGridPropertyEditorTests : UmbracoIntegrationTest
{
    private IDataTypeService DataTypeService => GetRequiredService<IDataTypeService>();

    private IContentTypeService ContentTypeService => GetRequiredService<IContentTypeService>();

    private IContentService ContentService => GetRequiredService<IContentService>();

    private IMediaTypeService MediaTypeService => GetRequiredService<IMediaTypeService>();

    private IMediaService MediaService => GetRequiredService<IMediaService>();

    private IMemberTypeService MemberTypeService => GetRequiredService<IMemberTypeService>();

    private IMemberService MemberService => GetRequiredService<IMemberService>();

    private IJsonSerializer JsonSerializer => GetRequiredService<IJsonSerializer>();

    private const string DataTypeName = "BlockGridTest";
    private const string DataTypeAlias = "blockGridTest";

    // ContentTypes
    [Test]
    public void Can_Track_Block_Grid_References_Content_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateDataType(DataTypeName, Constants.PropertyEditors.Aliases.BlockGrid);
        DataTypeService.Save(blockGridDataType);

        var contentType = ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias);
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
        var blockGridDataType = DataTypeBuilder.CreateDataType(DataTypeName, Constants.PropertyEditors.Aliases.BlockGrid);
        DataTypeService.Save(blockGridDataType);

        var firstContentType = ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
        ContentTypeService.Save(firstContentType);

        var secondContentType = ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid, "blockGridDocumentTypeTestTwo", "blockGridDocumentTypeTestTwo");
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

    [Test]
    public void Can_Track_Tags_References_In_Block_Grid_References_Content_Types()
    {
        var tagDataType = DataTypeBuilder.CreateDataType("TagsDataType", Constants.PropertyEditors.Aliases.Tags);
        DataTypeService.Save(tagDataType);

        var elementType = ContentTypeBuilder.CreateContentTypeWithDataType(tagDataType.Id, "TagsDataType", "tagsDataType", Constants.PropertyEditors.Aliases.Tags, "elementName", "ElementName", true);
        ContentTypeService.Save(elementType);

        var blockGridDataType = DataTypeBuilder.CreateBlockGridEditorDataTypeWithElement(elementType.Key, JsonSerializer);
        DataTypeService.Save(blockGridDataType);

        var contentType = ContentTypeBuilder.CreateContentTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
        contentType.AllowedAsRoot = true;
        ContentTypeService.Save(contentType);

        var elementId = Guid.NewGuid();
        var propertyValue = BlockPropertyEditorHelper.SerializeBlocks(
            JsonSerializer.Deserialize<BlockValue>($$"""
                                                     {
                                                     	"layout": {
                                                     		"Umbraco.BlockGrid": [{
                                                     				"contentUdi": "umb://element/{{elementId:N}}"
                                                     			}
                                                     		]
                                                     	},
                                                     	"contentData": [{
                                                     			"contentTypeKey": "{{elementType.Key:B}}",
                                                     			"udi": "umb://element/{{elementId:N}}",
                                                     			"tagsDataType": "['Tag Test', 'Tag Testing', 'Tag Tested']"
                                                     		}
                                                     	],
                                                     	"settingsData": []
                                                     }
                                                     """),
            JsonSerializer);
        var content = ContentBuilder.CreateBasicContent(contentType);
        content.Properties[0]?.SetValue(propertyValue);
        ContentService.Save(content);

        var dataType = DataTypeService.GetDataType(contentType.PropertyTypes.First(propertyType => propertyType.Alias == DataTypeAlias).DataTypeId)!;
        var editor = dataType.Editor!;
        var valueEditor = (BlockValuePropertyValueEditorBase)editor.GetValueEditor();

        var tags = valueEditor.GetTags(content.GetValue(DataTypeAlias), null, null).ToArray();
        Assert.Multiple(() =>
        {
            Assert.AreEqual(3, tags.Length);
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Test"));
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Testing"));
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Tested"));
        });
    }

    // MediaTypes
    [Test]
    public void Can_Track_Block_Grid_References_Media_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateDataType(DataTypeName, Constants.PropertyEditors.Aliases.BlockGrid);
        DataTypeService.Save(blockGridDataType);

        var mediaType = MediaTypeBuilder.CreateMediaTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
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
        var blockGridDataType = DataTypeBuilder.CreateDataType(DataTypeName, Constants.PropertyEditors.Aliases.BlockGrid);
        DataTypeService.Save(blockGridDataType);

        var firstMediaType = MediaTypeBuilder.CreateMediaTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
        MediaTypeService.Save(firstMediaType);

        var secondMediaType = MediaTypeBuilder.CreateMediaTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid, "blockGridMediaTypeTestTwo", "blockGridMediaTypeTestTwo");
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

    [Test]
    public void Can_Track_Tags_References_In_Block_Grid_References_Media_Types()
    {
        var tagDataType = DataTypeBuilder.CreateDataType("TagsDataType", Constants.PropertyEditors.Aliases.Tags);
        DataTypeService.Save(tagDataType);

        var elementType = ContentTypeBuilder.CreateContentTypeWithDataType(tagDataType.Id, "TagsDataType", "tagsDataType", Constants.PropertyEditors.Aliases.Tags, "elementName", "ElementName", true);
        ContentTypeService.Save(elementType);

        var blockGridDataType = DataTypeBuilder.CreateBlockGridEditorDataTypeWithElement(elementType.Key, JsonSerializer);
        DataTypeService.Save(blockGridDataType);

        var mediaType = MediaTypeBuilder.CreateMediaTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
        mediaType.AllowedAsRoot = true;
        MediaTypeService.Save(mediaType);

        var elementId = Guid.NewGuid();
        var propertyValue = BlockPropertyEditorHelper.SerializeBlocks(
            JsonSerializer.Deserialize<BlockValue>($$"""
                                                     {
                                                     	"layout": {
                                                     		"Umbraco.BlockGrid": [{
                                                     				"contentUdi": "umb://element/{{elementId:N}}"
                                                     			}
                                                     		]
                                                     	},
                                                     	"contentData": [{
                                                     			"contentTypeKey": "{{elementType.Key:B}}",
                                                     			"udi": "umb://element/{{elementId:N}}",
                                                     			"tagsDataType": "['Tag Test', 'Tag Testing', 'Tag Tested']"
                                                     		}
                                                     	],
                                                     	"settingsData": []
                                                     }
                                                     """),
            JsonSerializer);
        var media = MediaBuilder.CreateMedia(mediaType);
        media.Properties[0]?.SetValue(propertyValue);
        MediaService.Save(media);

        var dataType = DataTypeService.GetDataType(mediaType.PropertyTypes.First(propertyType => propertyType.Alias == DataTypeAlias).DataTypeId)!;
        var editor = dataType.Editor!;
        var valueEditor = (BlockValuePropertyValueEditorBase)editor.GetValueEditor();

        var tags = valueEditor.GetTags(media.GetValue(DataTypeAlias), null, null).ToArray();
        Assert.Multiple(() =>
        {
            Assert.AreEqual(3, tags.Length);
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Test"));
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Testing"));
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Tested"));
        });
    }

    // MemberTypes
    [Test]
    public void Can_Track_Block_Grid_References_Member_Types()
    {
        var blockGridDataType = DataTypeBuilder.CreateDataType(DataTypeName, Constants.PropertyEditors.Aliases.BlockGrid);
        DataTypeService.Save(blockGridDataType);

        var memberType = MemberTypeBuilder.CreateMemberTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
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
        var blockGridDataType = DataTypeBuilder.CreateDataType(DataTypeName, Constants.PropertyEditors.Aliases.BlockGrid);
        DataTypeService.Save(blockGridDataType);

        var firstMemberType = MemberTypeBuilder.CreateMemberTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
        MemberTypeService.Save(firstMemberType);

        var secondMemberType = MemberTypeBuilder.CreateMemberTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid, "blockGridMemberTypeTestTwo", "blockGridMemberTypeTestTwo");
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

    [Test]
    public void Can_Track_Tags_References_In_Block_Grid_References_Member_Types()
    {
        var tagDataType = DataTypeBuilder.CreateDataType("TagsDataType", Constants.PropertyEditors.Aliases.Tags);
        DataTypeService.Save(tagDataType);

        var elementType = ContentTypeBuilder.CreateContentTypeWithDataType(tagDataType.Id, "TagsDataType", "tagsDataType", Constants.PropertyEditors.Aliases.Tags, "elementName", "ElementName", true);
        ContentTypeService.Save(elementType);

        var blockGridDataType = DataTypeBuilder.CreateBlockGridEditorDataTypeWithElement(elementType.Key, JsonSerializer);
        DataTypeService.Save(blockGridDataType);

        var memberType = MemberTypeBuilder.CreateMemberTypeWithDataType(blockGridDataType.Id, DataTypeName, DataTypeAlias, Constants.PropertyEditors.Aliases.BlockGrid);
        MemberTypeService.Save(memberType);

        var elementId = Guid.NewGuid();
        var propertyValue = BlockPropertyEditorHelper.SerializeBlocks(
            JsonSerializer.Deserialize<BlockValue>($$"""
                                                     {
                                                     	"layout": {
                                                     		"Umbraco.BlockGrid": [{
                                                     				"contentUdi": "umb://element/{{elementId:N}}"
                                                     			}
                                                     		]
                                                     	},
                                                     	"contentData": [{
                                                     			"contentTypeKey": "{{elementType.Key:B}}",
                                                     			"udi": "umb://element/{{elementId:N}}",
                                                     			"tagsDataType": "['Tag Test', 'Tag Testing', 'Tag Tested']"
                                                     		}
                                                     	],
                                                     	"settingsData": []
                                                     }
                                                     """),
            JsonSerializer);
        var member = MemberBuilder.CreateMemberWithDataType(memberType);
        member.Properties[0]?.SetValue(propertyValue);
        MemberService.Save(member);

        var dataType = DataTypeService.GetDataType(memberType.PropertyTypes.First(propertyType => propertyType.Alias == DataTypeAlias).DataTypeId)!;
        var editor = dataType.Editor!;
        var valueEditor = (BlockValuePropertyValueEditorBase)editor.GetValueEditor();

        var tags = valueEditor.GetTags(member.GetValue(DataTypeAlias), null, null).ToArray();
        Assert.Multiple(() =>
        {
            Assert.AreEqual(3, tags.Length);
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Test"));
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Testing"));
            Assert.IsNotNull(tags.Single(tag => tag.Text == "Tag Tested"));
        });
    }
}
