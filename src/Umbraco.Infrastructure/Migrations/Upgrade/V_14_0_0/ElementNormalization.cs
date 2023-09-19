using Umbraco.Cms.Infrastructure.Persistence.Dtos;

namespace Umbraco.Cms.Infrastructure.Migrations.Upgrade.V_14_0_0;

public class ElementNormalization : MigrationBase
{
    public ElementNormalization(IMigrationContext context) : base(context)
    {
    }

    protected override void Migrate()
    {
        Create.Table<ElementDto>().Do();
        Create.Table<PropertyDataElementDto>().Do();
    }
}
