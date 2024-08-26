using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;

namespace Umbraco.Cms.Tests.UnitTests.Umbraco.Core.Routing;

[TestFixture]
public class ContentFinderByUrlWithDomainsTests : UrlRoutingTestBase
{
    private void SetDomains3()
    {
        var domainService = Mock.Get(DomainService);

        domainService.Setup(service => service.GetAll(It.IsAny<bool>()))
            .Returns((bool incWildcards) => new[]
            {
                new UmbracoDomain("domain1.com/")
                {
                    Id = 1, LanguageId = LangDeId, RootContentId = 1001,
                    LanguageIsoCode = "de-DE",
                },
            });
    }

    private void SetDomains4()
    {
        var domainService = Mock.Get(DomainService);

        domainService.Setup(service => service.GetAll(It.IsAny<bool>()))
            .Returns((bool incWildcards) => new[]
            {
                new UmbracoDomain("domain1.com/")
                {
                    Id = 1, LanguageId = LangEngId, RootContentId = 1001, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("domain1.com/en")
                {
                    Id = 2, LanguageId = LangEngId, RootContentId = 10011, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("domain1.com/fr")
                {
                    Id = 3, LanguageId = LangFrId, RootContentId = 10012, LanguageIsoCode = "fr-FR",
                },
                new UmbracoDomain("http://domain3.com/")
                {
                    Id = 4, LanguageId = LangEngId, RootContentId = 1003, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("http://domain3.com/en")
                {
                    Id = 5, LanguageId = LangEngId, RootContentId = 10031, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("http://domain3.com/fr")
                {
                    Id = 6, LanguageId = LangFrId, RootContentId = 10032, LanguageIsoCode = "fr-FR",
                },
            });
    }

    [TestCase("http://domain1.com/", 1001)]
    [TestCase("http://domain1.com/1001-1", 10011)]
    [TestCase("http://domain1.com/1001-2/1001-2-1", 100121)]
    public async Task Lookup_SingleDomain(string url, int expectedId)
    {
        SetDomains3();

        GlobalSettings.HideTopLevelNodeFromPath = true;

        var umbracoContextAccessor = GetUmbracoContextAccessor(url);
        var publishedRouter = CreatePublishedRouter(umbracoContextAccessor);
        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
        var frequest = await publishedRouter.CreateRequestAsync(umbracoContext.CleanedUmbracoUrl);

        // must lookup domain else lookup by URL fails
        publishedRouter.FindAndSetDomain(frequest);

        var lookup = new ContentFinderByUrl(Mock.Of<ILogger<ContentFinderByUrl>>(), umbracoContextAccessor);
        var result = await lookup.TryFindContent(frequest);
        Assert.IsTrue(result);
        Assert.AreEqual(expectedId, frequest.PublishedContent.Id);
    }

    [TestCase("http://domain1.com/", 1001, "en-US")]
    [TestCase("http://domain1.com/en", 10011, "en-US")]
    [TestCase("http://domain1.com/en/1001-1-1", 100111, "en-US")]
    [TestCase("http://domain1.com/fr", 10012, "fr-FR")]
    [TestCase("http://domain1.com/fr/1001-2-1", 100121, "fr-FR")]
    [TestCase("http://domain1.com/1001-3", 10013, "en-US")]
    [TestCase("http://domain2.com/1002", 1002, "")]
    [TestCase("http://domain3.com/", 1003, "en-US")]
    [TestCase("http://domain3.com/en", 10031, "en-US")]
    [TestCase("http://domain3.com/en/1003-1-1", 100311, "en-US")]
    [TestCase("http://domain3.com/fr", 10032, "fr-FR")]
    [TestCase("http://domain3.com/fr/1003-2-1", 100321, "fr-FR")]
    [TestCase("http://domain3.com/1003-3", 10033, "en-US")]
    [TestCase("https://domain1.com/", 1001, "en-US")]
    [TestCase("https://domain3.com/", 1001, "")] // because domain3 is explicitely set on http
    public async Task Lookup_NestedDomains(string url, int expectedId, string expectedCulture)
    {
        SetDomains4();

        // defaults depend on test environment
        expectedCulture ??= Thread.CurrentThread.CurrentUICulture.Name;

        GlobalSettings.HideTopLevelNodeFromPath = true;

        var umbracoContextAccessor = GetUmbracoContextAccessor(url);
        var publishedRouter = CreatePublishedRouter(umbracoContextAccessor);
        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
        var frequest = await publishedRouter.CreateRequestAsync(umbracoContext.CleanedUmbracoUrl);

        // must lookup domain else lookup by URL fails
        publishedRouter.FindAndSetDomain(frequest);
        Assert.AreEqual(expectedCulture, frequest.Culture);

        var lookup = new ContentFinderByUrl(Mock.Of<ILogger<ContentFinderByUrl>>(), umbracoContextAccessor);
        var result = await lookup.TryFindContent(frequest);
        Assert.IsTrue(result);
        Assert.AreEqual(expectedId, frequest.PublishedContent.Id);
    }
}
