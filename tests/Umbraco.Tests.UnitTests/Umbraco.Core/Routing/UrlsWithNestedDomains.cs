using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Tests.Common;
using Umbraco.Extensions;

namespace Umbraco.Cms.Tests.UnitTests.Umbraco.Core.Routing;

[TestFixture]
public class UrlsWithNestedDomains : UrlRoutingTestBase
{
    // in the case of nested domains more than 1 URL may resolve to a document
    // but only one route can be cached - the 'canonical' route ie the route
    // using the closest domain to the node - here we test that if we request
    // a non-canonical route, it is not cached / the cache is not polluted
    [Test]
    public async Task DoNotPolluteCache()
    {
        var requestHandlerSettings = new RequestHandlerSettings { AddTrailingSlash = true };
        GlobalSettings.HideTopLevelNodeFromPath = false;

        SetDomains1();

        const string url = "http://domain1.com/1001-1/1001-1-1";

        // get the nice URL for 100111
        var umbracoContextAccessor = GetUmbracoContextAccessor(url);
        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();

        var urlProvider = new DefaultUrlProvider(
            Mock.Of<IOptionsMonitor<RequestHandlerSettings>>(x => x.CurrentValue == requestHandlerSettings),
            Mock.Of<ILogger<DefaultUrlProvider>>(),
            new SiteDomainMapper(),
            umbracoContextAccessor,
            new UriUtility(Mock.Of<IHostingEnvironment>()),
            Mock.Of<ILocalizationService>());
        var publishedUrlProvider = GetPublishedUrlProvider(umbracoContext, urlProvider);

        var absUrl = publishedUrlProvider.GetUrl(100111, UrlMode.Absolute);
        Assert.AreEqual("http://domain2.com/1001-1-1/", absUrl);

        const string cacheKeyPrefix = "NuCache.ContentCache.RouteByContent";

        // check that the proper route has been cached
        var cache = (FastDictionaryAppCache)umbracoContext.PublishedSnapshot.ElementsCache;

        var cacheKey = $"{cacheKeyPrefix}[P:100111]";
        Assert.AreEqual("10011/1001-1-1", cache.Get(cacheKey));

        // route a rogue URL
        var publishedRouter = CreatePublishedRouter(umbracoContextAccessor);
        var frequest = await publishedRouter.CreateRequestAsync(umbracoContext.CleanedUmbracoUrl);

        publishedRouter.FindAndSetDomain(frequest);
        Assert.IsTrue(frequest.HasDomain());

        // check that it's been routed
        var lookup = new ContentFinderByUrl(Mock.Of<ILogger<ContentFinderByUrl>>(), umbracoContextAccessor);
        var result = await lookup.TryFindContent(frequest);
        Assert.IsTrue(result);
        Assert.AreEqual(100111, frequest.PublishedContent.Id);

        // has the cache been polluted?
        Assert.AreEqual("10011/1001-1-1", cache.Get(cacheKey)); // no

        // what's the nice URL now?
        Assert.AreEqual("http://domain2.com/1001-1-1/", publishedUrlProvider.GetUrl(100111)); // good
    }

    private void SetDomains1()
    {
        var domainService = Mock.Get(DomainService);

        domainService.Setup(service => service.GetAll(It.IsAny<bool>()))
            .Returns((bool incWildcards) => new[]
            {
                new UmbracoDomain("http://domain1.com/")
                {
                    Id = 1, LanguageId = LangEngId, RootContentId = 1001, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("http://domain2.com/")
                {
                    Id = 2, LanguageId = LangEngId, RootContentId = 10011, LanguageIsoCode = "en-US",
                },
            });
    }

    private IPublishedUrlProvider GetPublishedUrlProvider(IUmbracoContext umbracoContext, DefaultUrlProvider urlProvider)
    {
        var webRoutingSettings = new WebRoutingSettings();
        return new UrlProvider(
            new TestUmbracoContextAccessor(umbracoContext),
            Options.Create(webRoutingSettings),
            new UrlProviderCollection(() => new[] { urlProvider }),
            new MediaUrlProviderCollection(() => Enumerable.Empty<IMediaUrlProvider>()),
            Mock.Of<IVariationContextAccessor>());
    }
}
