﻿using Umbraco.Cms.Infrastructure.Persistence.Dtos;

namespace Umbraco.Cms.Infrastructure.Migrations.Upgrade.V_15_0_0;

[Obsolete("Remove in Umbraco 18.")]
public class AddUserClientId : MigrationBase
{
    public AddUserClientId(IMigrationContext context)
        : base(context)
    {
    }

    protected override void Migrate()
        => Create.Table<User2ClientIdDto>().Do();
}