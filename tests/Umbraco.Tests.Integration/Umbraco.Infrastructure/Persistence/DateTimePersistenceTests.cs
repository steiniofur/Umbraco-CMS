using NUnit.Framework;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Integration.TestServerTest;

namespace Umbraco.Cms.Tests.Integration.Umbraco.Infrastructure.Persistence;

[TestFixture]
public class DateTimePersistenceTests : UmbracoTestServerTestBase
{
    private IContentService _contentService;
    private IContentTypeService _contentTypeService;
    private IUmbracoContextAccessor _contextAccessor;

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        _contentService = GetRequiredService<IContentService>();
        _contentTypeService = GetRequiredService<IContentTypeService>();
        _contextAccessor = GetRequiredService<IUmbracoContextAccessor>();
        PrepareUrl("/");
    }

    [Test]
    public void Content_CreateDate_Kind_Should_Equal_After_Publishing_From_ContentService()
    {
        var date = DateTime.Now;

        var createdItem = CreateBasicContentItem();
        var fetchedItem = _contentService.GetById(createdItem.Id);

        // assert
        // since it can take a little bit of time (usually milliseconds) between creating the item and persisting + retrieving,
        // we check against 5 minutes as that should not be caused by timezone differences
        Assert.Multiple(() =>
        {
            Assert.LessOrEqual(
                fetchedItem!.CreateDate.ToUniversalTime() - date.ToUniversalTime(),
                TimeSpan.FromMinutes(5));
            Assert.LessOrEqual(
                fetchedItem!.UpdateDate.ToUniversalTime() - date.ToUniversalTime(),
                TimeSpan.FromMinutes(5));
        });
    }

    [Test]
    public void Content_CreateDate_Kind_Should_Equal_After_Publishing_From_PublishedContentCache()
    {
        var date = DateTime.Now;
        var publishedItem = CreateAndPublishBasicContentItem();
        Assert.NotNull(publishedItem, "Guard: PublishedCache is not in the correct state");
        var contentCache = _contextAccessor.GetRequiredUmbracoContext().Content;
        Assert.NotNull(publishedItem, "Guard: Invalid umbraco context content cache");
        var cachedPublishedItem = contentCache!.GetById(publishedItem.Id);

        // assert
        // since it can take a little bit of time (usually milliseconds) between creating the item and persisting + retrieving,
        // we check against 5 minutes as that should not be caused by timezone differences
        Assert.Multiple(() =>
        {
            Assert.LessOrEqual(
                cachedPublishedItem!.CreateDate.ToUniversalTime() - date.ToUniversalTime(),
                TimeSpan.FromMinutes(5));
            Assert.LessOrEqual(
                cachedPublishedItem!.UpdateDate.ToUniversalTime() - date.ToUniversalTime(),
                TimeSpan.FromMinutes(5));
        });
    }

    private IContent CreateBasicContentItem()
    {
        // arrange
        var contentType = ContentTypeBuilder.CreateBasicContentType("dateTestCT");
        _contentTypeService.Save(contentType);

        var content = ContentBuilder.CreateBasicContent(contentType);

        // act
        _contentService.Save(content);

        return content;
    }

    private IContent CreateAndPublishBasicContentItem()
    {
        var content = CreateBasicContentItem();
        var publishResult = _contentService.Publish(content, ["*"]);
        Assert.NotNull(publishResult.Content, "Guard: Publishing failed");
        return _contentService.GetById(publishResult.Content!.Id)!;
    }
}
