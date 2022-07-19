using NPoco;

namespace Umbraco.Cms.Persistence.SqlServer.Dtos
{
    internal class DefaultConstraintPerColumnDto
    {
        [Column("TABLE_NAME")]
        public required string TableName { get; set; }

        [Column("COLUMN_NAME")]
        public required string ColumnName { get; set; }

        [Column("NAME")]
        public required string Name { get; set; }

        [Column("DEFINITION")]
        public required string Definition { get; set; }
    }
}
