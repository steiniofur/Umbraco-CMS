using NPoco;

namespace Umbraco.Cms.Persistence.SqlServer.Dtos
{
    internal class ConstraintPerTableDto
    {
        [Column("TABLE_NAME")]
        public required string TableName { get; set; }

        [Column("CONSTRAINT_NAME")]
        public required string ConstraintName { get; set; }
    }
}
