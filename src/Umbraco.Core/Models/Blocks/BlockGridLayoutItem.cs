// Copyright (c) Umbraco.
// See LICENSE for more details.

using System.Diagnostics.CodeAnalysis;

namespace Umbraco.Cms.Core.Models.Blocks;

/// <summary>
/// Used for deserializing the block grid layout
/// </summary>
public class BlockGridLayoutItem : IBlockLayoutItem
{
    public required Udi ContentUdi { get; set; }

    public Udi? SettingsUdi { get; set; }

    public int? ColumnSpan { get; set; }

    public int? RowSpan { get; set; }

    public BlockGridLayoutAreaItem[] Areas { get; set; } = [];

    public BlockGridLayoutItem()
    { }

    [SetsRequiredMembers]
    public BlockGridLayoutItem(Udi contentUdi)
        => ContentUdi = contentUdi;

    [SetsRequiredMembers]
    public BlockGridLayoutItem(Udi contentUdi, Udi settingsUdi)
        : this(contentUdi)
        => SettingsUdi = settingsUdi;
}
