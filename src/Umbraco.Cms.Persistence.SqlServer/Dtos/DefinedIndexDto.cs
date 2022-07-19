using NPoco;

namespace Umbraco.Cms.Persistence.SqlServer.Dtos
{
    internal class DefinedIndexDto
    {

        [Column("TABLE_NAME")]
        public required string TableName { get; set; }

        [Column("INDEX_NAME")]
        public required string IndexName { get; set; }

        [Column("COLUMN_NAME")]
        public required string ColumnName { get; set; }

        [Column("UNIQUE")]
        public short Unique { get; set; }
    }
}
