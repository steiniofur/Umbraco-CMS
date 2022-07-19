using NPoco;

namespace Umbraco.Cms.Persistence.SqlServer.Dtos
{
    internal class ColumnInSchemaDto
    {
        [Column("TABLE_NAME")]
        public required string TableName { get; set; }

        [Column("COLUMN_NAME")]
        public required string ColumnName { get; set; }

        [Column("ORDINAL_POSITION")]
        public int OrdinalPosition { get; set; }

        [Column("COLUMN_DEFAULT")]
        public required string ColumnDefault { get; set; }

        [Column("IS_NULLABLE")]
        public required string IsNullable { get; set; }

        [Column("DATA_TYPE")]
        public required string DataType { get; set; }
    }
}
