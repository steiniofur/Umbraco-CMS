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
public class DomainsAndCulturesTests : UrlRoutingTestBase
{
    private void SetDomains1()
    {
        var domainService = Mock.Get(DomainService);

        domainService.Setup(service => service.GetAll(It.IsAny<bool>()))
            .Returns((bool incWildcards) => new[]
            {
                new UmbracoDomain("domain1.com/")
                {
                    Id = 1, LanguageId = LangDeId, RootContentId = 1001, LanguageIsoCode = "de-DE",
                },
                new UmbracoDomain("domain1.com/en")
                {
                    Id = 2, LanguageId = LangEngId, RootContentId = 10011, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("domain1.com/fr")
                {
                    Id = 3, LanguageId = LangFrId, RootContentId = 10012, LanguageIsoCode = "fr-FR",
                },
            });
    }

    private void SetDomains2()
    {
        var domainService = Mock.Get(DomainService);

        domainService.Setup(service => service.GetAll(It.IsAny<bool>()))
            .Returns((bool incWildcards) => new[]
            {
                new UmbracoDomain("domain1.com/")
                {
                    Id = 1, LanguageId = LangDeId, RootContentId = 1001, LanguageIsoCode = "de-DE",
                },
                new UmbracoDomain("domain1.com/en")
                {
                    Id = 2, LanguageId = LangEngId, RootContentId = 10011, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("domain1.com/fr")
                {
                    Id = 3, LanguageId = LangFrId, RootContentId = 10012, LanguageIsoCode = "fr-FR",
                },
                new UmbracoDomain("*1001")
                {
                    Id = 4, LanguageId = LangDeId, RootContentId = 1001, LanguageIsoCode = "de-DE",
                },
                new UmbracoDomain("*10011")
                {
                    Id = 5, LanguageId = LangCzId, RootContentId = 10011, LanguageIsoCode = "cs-CZ",
                },
                new UmbracoDomain("*100112")
                {
                    Id = 6, LanguageId = LangNlId, RootContentId = 100112, LanguageIsoCode = "nl-NL",
                },
                new UmbracoDomain("*1001122")
                {
                    Id = 7, LanguageId = LangDkId, RootContentId = 1001122, LanguageIsoCode = "da-DK",
                },
                new UmbracoDomain("*10012")
                {
                    Id = 8, LanguageId = LangNlId, RootContentId = 10012, LanguageIsoCode = "nl-NL",
                },
                new UmbracoDomain("*10031")
                {
                    Id = 9, LanguageId = LangNlId, RootContentId = 10031, LanguageIsoCode = "nl-NL",
                },
            });
    }

    // domains such as "/en" are natively supported, and when instanciating
    // DomainAndUri for them, the host will come from the current request
    private void SetDomains3()
    {
        var domainService = Mock.Get(DomainService);

        domainService.Setup(service => service.GetAll(It.IsAny<bool>()))
            .Returns((bool incWildcards) => new[]
            {
                new UmbracoDomain("/en")
                {
                    Id = 1, LanguageId = LangEngId, RootContentId = 10011, LanguageIsoCode = "en-US",
                },
                new UmbracoDomain("/fr")
                {
                    Id = 2, LanguageId = LangFrId, RootContentId = 10012, LanguageIsoCode = "fr-FR",
                },
            });
    }

    #region Cases

    [TestCase("http://domain1.com/", "de-DE", 1001)]
    [TestCase("http://domain1.com/1001-1", "de-DE", 10011)]
    [TestCase("http://domain1.com/1001-1/1001-1-1", "de-DE", 100111)]
    [TestCase("http://domain1.com/en", "en-US", 10011)]
    [TestCase("http://domain1.com/en/1001-1-1", "en-US", 100111)]
    [TestCase("http://domain1.com/fr", "fr-FR", 10012)]
    [TestCase("http://domain1.com/fr/1001-2-1", "fr-FR", 100121)]

    #endregion

    public async Task DomainAndCulture(string inputUrl, string expectedCulture, int expectedNode)
    {
        SetDomains1();

        GlobalSettings.HideTopLevelNodeFromPath = false;

        var umbracoContextAccessor = GetUmbracoContextAccessor(inputUrl);
        var publishedRouter = CreatePublishedRouter(umbracoContextAccessor);
        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
        var frequest = await publishedRouter.CreateRequestAsync(umbracoContext.CleanedUmbracoUrl);

        // lookup domain
        publishedRouter.FindAndSetDomain(frequest);

        Assert.AreEqual(expectedCulture, frequest.Culture);

        var finder = new ContentFinderByUrl(Mock.Of<ILogger<ContentFinderByUrl>>(), umbracoContextAccessor);
        var result = await finder.TryFindContent(frequest);

        Assert.IsTrue(result);
        Assert.AreEqual(frequest.PublishedContent.Id, expectedNode);
    }

    #region Cases

    [TestCase("http://domain1.com/", "de-DE", 1001)] // domain takes over local wildcard at 1001
    [TestCase("http://domain1.com/1001-1", "cs-CZ", 10011)] // wildcard on 10011 applies
    [TestCase("http://domain1.com/1001-1/1001-1-1", "cs-CZ", 100111)] // wildcard on 10011 applies
    [TestCase("http://domain1.com/1001-1/1001-1-2", "nl-NL", 100112)] // wildcard on 100112 applies
    [TestCase("http://domain1.com/1001-1/1001-1-2/1001-1-2-1", "nl-NL", 1001121)] // wildcard on 100112 applies
    [TestCase("http://domain1.com/1001-1/1001-1-2/1001-1-2-2", "da-DK", 1001122)] // wildcard on 1001122 applies
    [TestCase("http://domain1.com/1001-2", "nl-NL", 10012)] // wildcard on 10012 applies
    [TestCase("http://domain1.com/1001-2/1001-2-1", "nl-NL", 100121)] // wildcard on 10012 applies
    [TestCase("http://domain1.com/en", "en-US", 10011)] // domain takes over local wildcard at 10011
    [TestCase("http://domain1.com/en/1001-1-1", "en-US", 100111)] // domain takes over local wildcard at 10011
    [TestCase("http://domain1.com/en/1001-1-2", "nl-NL", 100112)] // wildcard on 100112 applies
    [TestCase("http://domain1.com/en/1001-1-2/1001-1-2-1", "nl-NL", 1001121)] // wildcard on 100112 applies
    [TestCase("http://domain1.com/en/1001-1-2/1001-1-2-2", "da-DK", 1001122)] // wildcard on 1001122 applies
    [TestCase("http://domain1.com/fr", "fr-FR", 10012)] // domain takes over local wildcard at 10012
    [TestCase("http://domain1.com/fr/1001-2-1", "fr-FR", 100121)] // domain takes over local wildcard at 10012
    [TestCase("/1003", "", 1003)] // default culture (no domain)
    [TestCase("/1003/1003-1", "nl-NL", 10031)] // wildcard on 10031 applies
    [TestCase("/1003/1003-1/1003-1-1", "nl-NL", 100311)] // wildcard on 10031 applies

    #endregion

    public async Task DomainAndCultureWithWildcards(string inputUrl, string expectedCulture, int expectedNode)
    {
        SetDomains2();

        // defaults depend on test environment
        expectedCulture ??= Thread.CurrentThread.CurrentUICulture.Name;

        GlobalSettings.HideTopLevelNodeFromPath = false;

        var umbracoContextAccessor = GetUmbracoContextAccessor(inputUrl);
        var publishedRouter = CreatePublishedRouter(umbracoContextAccessor);
        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
        var frequest = await publishedRouter.CreateRequestAsync(umbracoContext.CleanedUmbracoUrl);

        // lookup domain
        publishedRouter.FindAndSetDomain(frequest);

        // find document
        var finder = new ContentFinderByUrl(Mock.Of<ILogger<ContentFinderByUrl>>(), umbracoContextAccessor);
        var result = await finder.TryFindContent(frequest);

        // apply wildcard domain
        publishedRouter.HandleWildcardDomains(frequest);

        Assert.IsTrue(result);
        Assert.AreEqual(expectedCulture, frequest.Culture);
        Assert.AreEqual(frequest.PublishedContent.Id, expectedNode);
    }

    #region Cases

    [TestCase("http://domain1.com/en", "en-US", 10011)]
    [TestCase("http://domain1.com/en/1001-1-1", "en-US", 100111)]
    [TestCase("http://domain1.com/fr", "fr-FR", 10012)]
    [TestCase("http://domain1.com/fr/1001-2-1", "fr-FR", 100121)]

    #endregion

    public async Task DomainGeneric(string inputUrl, string expectedCulture, int expectedNode)
    {
        SetDomains3();

        GlobalSettings.HideTopLevelNodeFromPath = false;

        var umbracoContextAccessor = GetUmbracoContextAccessor(inputUrl);
        var publishedRouter = CreatePublishedRouter(umbracoContextAccessor);
        var umbracoContext = umbracoContextAccessor.GetRequiredUmbracoContext();
        var frequest = await publishedRouter.CreateRequestAsync(umbracoContext.CleanedUmbracoUrl);

        // lookup domain
        publishedRouter.FindAndSetDomain(frequest);
        Assert.IsNotNull(frequest.Domain);

        Assert.AreEqual(expectedCulture, frequest.Culture);

        var finder = new ContentFinderByUrl(Mock.Of<ILogger<ContentFinderByUrl>>(), umbracoContextAccessor);
        var result = await finder.TryFindContent(frequest);

        Assert.IsTrue(result);
        Assert.AreEqual(frequest.PublishedContent.Id, expectedNode);
    }
}
