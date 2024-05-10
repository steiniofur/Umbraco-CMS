using System.Diagnostics.CodeAnalysis;

namespace Umbraco.Cms.Core.Models.Blocks;

public class BlockGridLayoutAreaItem
{
    public required Guid Key { get; set; }

    public BlockGridLayoutItem[] Items { get; set; } = [];

    public BlockGridLayoutAreaItem()
    { }

    [SetsRequiredMembers]
    public BlockGridLayoutAreaItem(Guid key)
        => Key = key;
}
