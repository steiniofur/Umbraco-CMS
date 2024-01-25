using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
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
    public void Can_Track_Block_References()
    {
        var blockGridDataType = DataTypeBuilder

        var contentType = ContentTypeBuilder.CreateTextPageContentType("myContentType");
        contentType.AllowedTemplates = Enumerable.Empty<ITemplate>();
        ContentTypeService.Save(contentType);

        var dataType = DataTypeService.GetDataType(contentType.PropertyTypes.First(propertyType => propertyType.Alias == "bodyText").DataTypeId)!;
        var editor = dataType.Editor!;
        var valueEditor = editor.GetValueEditor();

        const string markup = "<p>This is some markup</p>";

        var content = ContentBuilder.CreateTextpageContent(contentType, "My Content", -1);
        content.Properties["bodyText"]!.SetValue(markup);
        ContentService.Save(content);

        var toEditor = valueEditor.ToEditor(content.Properties["bodyText"]);
        var richTextEditorValue = toEditor as RichTextEditorValue;

        Assert.IsNotNull(richTextEditorValue);
        Assert.AreEqual(markup, richTextEditorValue.Markup);
    }
}

