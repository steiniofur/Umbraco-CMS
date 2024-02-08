using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Serialization;

namespace Umbraco.Cms.Core.PropertyEditors;

public static class BlockPropertyEditorHelper
{

    public static string SerializeBlocks(BlockValue blockValue, IJsonSerializer jsonSerializer) => jsonSerializer.Serialize(blockValue);
}
