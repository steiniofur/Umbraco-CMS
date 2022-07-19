using NPoco;

namespace Umbraco.Cms.Persistence.SqlServer.Dtos
{
    internal class ConstraintPerColumnDto
    {
        [Column("TABLE_NAME")]
        public required string TableName { get; set; }

        [Column("COLUMN_NAME")]
        public required string ColumnName { get; set; }

        [Column("CONSTRAINT_NAME")]
        public required string ConstraintName { get; set; }
    }
}
