using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.PublishedCache;
using Umbraco.Cms.Infrastructure.PublishedCache.DataSource;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Builders.Extensions;
using Umbraco.Cms.Tests.Common.Published;
using Umbraco.Cms.Tests.UnitTests.TestHelpers;

namespace Umbraco.Cms.Tests.UnitTests.Umbraco.Core.Routing;

[TestFixture]
public abstract class UrlRoutingTestBase : PublishedSnapshotServiceTestBase
{
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        PopulateCache();
    }

    // Sets up the mock domain service
    protected override ServiceContext CreateServiceContext(IContentType[] contentTypes, IMediaType[] mediaTypes,
        IDataType[] dataTypes)
    {
        var serviceContext = base.CreateServiceContext(contentTypes, mediaTypes, dataTypes);

        // setup mock domain service
        var domainService = Mock.Get(serviceContext.DomainService);
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

        return serviceContext;
    }

    protected virtual void PopulateCache()
    {
        var dataTypes = GetDefaultDataTypes().Select(dt => dt as IDataType).ToArray();
        var propertyDataTypes = new Dictionary<string, IDataType>
        {
            // we only have one data type for this test which will be resolved with string empty.
            [string.Empty] = dataTypes[0],
        };
        IContentType emptyContentType = new ContentType(ShortStringHelper, -1);
        emptyContentType.Alias = "alias";

        var kits = new[]
        {
            BuildKit(1001, "Home", "1001", "-1,1001", ["this/is/my/alias", "anotheralias"], propertyDataTypes, emptyContentType),
            BuildKit(10011, "Sub1", "1001-1", "-1,1001,10011", ["page2/alias", "2ndpagealias", "en/flux", "endanger"], propertyDataTypes, emptyContentType),
            BuildKit(100111, "Sub2", "1001-1-1", "-1,1001,10011,100111", ["only/one/alias", "entropy", "bar/foo", "en/bar/nil"], propertyDataTypes, emptyContentType),
            BuildKit(100112, "Sub 3", "1001-1-2", "-1,1001,10011,100112", null, propertyDataTypes, emptyContentType),
            BuildKit(1001121, "Sub 3", "1001-1-2-1", "-1,1001,10011,100112,1001121", null, propertyDataTypes, emptyContentType),
            BuildKit(1001122, "Sub 3", "1001-1-2-2", "-1,1001,10011,100112,1001122", null, propertyDataTypes, emptyContentType),
            BuildKit(10012, "Sub 2", "1001-2", "-1,1001,10012", ["alias42"], propertyDataTypes, emptyContentType),
            BuildKit(100121, "Sub2", "1001-2-1", "-1,1001,10012,100121", ["alias43"], propertyDataTypes, emptyContentType),
            BuildKit(100122, "Sub 3", "1001-2-2", "-1,1001,10012,100122", null, propertyDataTypes, emptyContentType),
            BuildKit(1001221, "Sub 3", "1001-2-2-1", "-1,1001,10012,100122,1001221", null, propertyDataTypes, emptyContentType),
            BuildKit(1001222, "Sub 3", "1001-2-2-2", "-1,1001,10012,100122,1001222", null, propertyDataTypes, emptyContentType),
            BuildKit(10013, "Sub 2", "1001-3", "-1,1001,10013", ["alias42"], propertyDataTypes, emptyContentType),
            BuildKit(1002, "Test", "1002", "-1,1002", null, propertyDataTypes, emptyContentType),
            BuildKit(1003, "Home", "1003", "-1,1003", null, propertyDataTypes, emptyContentType),
            BuildKit(10031, "Sub1", "1003-1", "-1,1003,10031", null, propertyDataTypes, emptyContentType),
            BuildKit(100311, "Sub2", "1003-1-1", "-1,1003,10031,100311", null, propertyDataTypes, emptyContentType),
            BuildKit(100312, "Sub 3", "1003-1-2", "-1,1003,10031,100312", null, propertyDataTypes, emptyContentType),
            BuildKit(1003121, "Sub 3", "1003-1-2-1", "-1,1003,10031,100312,1003121", null, propertyDataTypes, emptyContentType),
            BuildKit(1003122, "Sub 3", "1003-1-2-2", "-1,1003,10031,100312,1003122", null, propertyDataTypes, emptyContentType),
            BuildKit(10032, "Sub 2", "1003-2", "-1,1003,10032", null, propertyDataTypes, emptyContentType),
            BuildKit(100321, "Sub2", "1003-2-1", "-1,1003,10032,100321", null, propertyDataTypes, emptyContentType),
            BuildKit(100322, "Sub 3", "1003-2-2", "-1,1003,10032,100322", null, propertyDataTypes, emptyContentType),
            BuildKit(1003221, "Sub 3", "1003-2-2-1", "-1,1003,10032,100322,1003221", null, propertyDataTypes, emptyContentType),
            BuildKit(1003222, "Sub 3", "1003-2-2-2", "-1,1003,10032,100322,1003222", null, propertyDataTypes, emptyContentType),
            BuildKit(10033, "Sub 2", "1003-3", "-1,1003,10033", null, propertyDataTypes, emptyContentType),
        };

        InitializedCache(kits, [emptyContentType], dataTypes);
    }

    protected ContentNodeKit BuildKit(
        int id,
        string name,
        string urlName,
        string path,
        string[]? urlAliases,
        Dictionary<string, IDataType> propertyDataTypes,
        IContentType contentType)
    {
        var contentDataBuilder = new ContentDataBuilder()
            .WithName(name)
            .WithUrlSegment(urlName);

        if (urlAliases is not null)
        {
            contentDataBuilder.WithProperties(new Dictionary<string, PropertyData[]>
            {
                { "umbracoUrlAlias", [new PropertyData { Culture = string.Empty, Segment = string.Empty, Value = string.Join(',', urlAliases) }] },
            });
        }

        var homeData = contentDataBuilder.Build(ShortStringHelper, propertyDataTypes, contentType, contentType.Alias);

        return ContentNodeKitBuilder.CreateWithContent(
            contentType.Id,
            id,
            path,
            level: path.Split(',').Length - 1,
            draftData: homeData,
            publishedData: homeData);
    }

    public const int LangDeId = 333;
    public const int LangEngId = 334;
    public const int LangFrId = 335;
    public const int LangCzId = 336;
    public const int LangNlId = 337;
    public const int LangDkId = 338;
}
