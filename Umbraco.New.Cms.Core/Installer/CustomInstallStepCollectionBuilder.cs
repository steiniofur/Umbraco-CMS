using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;

namespace Umbraco.New.Cms.Core.Installer;

public class CustomInstallStepCollectionBuilder : OrderedCollectionBuilderBase<CustomInstallStepCollectionBuilder, CustomInstallStepCollection, ICustomInstallStep>
{
    protected override CustomInstallStepCollectionBuilder This => this;

    protected override ServiceLifetime CollectionLifetime => ServiceLifetime.Transient;
}
