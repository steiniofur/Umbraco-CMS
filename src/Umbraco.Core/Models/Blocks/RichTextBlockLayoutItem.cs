// Copyright (c) Umbraco.
// See LICENSE for more details.

using System.Diagnostics.CodeAnalysis;

namespace Umbraco.Cms.Core.Models.Blocks;

/// <summary>
///     Used for deserializing the rich text block layouts
/// </summary>
public class RichTextBlockLayoutItem : IBlockLayoutItem
{
    public required Udi ContentUdi { get; set; }

    public Udi? SettingsUdi { get; set; }

    public RichTextBlockLayoutItem()
    { }

    [SetsRequiredMembers]
    public RichTextBlockLayoutItem(Udi contentUdi)
        => ContentUdi = contentUdi;

    [SetsRequiredMembers]
    public RichTextBlockLayoutItem(Udi contentUdi, Udi settingsUdi)
        : this(contentUdi)
        => SettingsUdi = settingsUdi;
}
