// Copyright (c) Umbraco.
// See LICENSE for more details.

using System.Diagnostics.CodeAnalysis;

namespace Umbraco.Cms.Core.Models.Blocks;

/// <summary>
///     Used for deserializing the block list layout
/// </summary>
public class BlockListLayoutItem : IBlockLayoutItem
{
    public required Udi ContentUdi { get; set; }

    public Udi? SettingsUdi { get; set; }

    public BlockListLayoutItem()
    { }

    [SetsRequiredMembers]
    public BlockListLayoutItem(Udi contentUdi)
        => ContentUdi = contentUdi;

    [SetsRequiredMembers]
    public BlockListLayoutItem(Udi contentUdi, Udi settingsUdi)
        : this(contentUdi)
        => SettingsUdi = settingsUdi;
}
