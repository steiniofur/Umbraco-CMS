using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Cms.Infrastructure.Persistence.Dtos;

[TableName(TableName)]
[PrimaryKey("nodeId", AutoIncrement = false)]
[ExplicitColumns]
public class ElementDto
{
    private const string TableName = Constants.DatabaseSchema.Tables.Element;

    [Column("nodeId")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    [ForeignKey(typeof(ContentDto))]
    public int NodeId { get; set; }

    [Column("published")]
    [Index(IndexTypes.NonClustered, Name = "IX_" + TableName + "_Published")]
    public bool Published { get; set; }

    [Column("edited")]
    public bool Edited { get; set; }

    [ResultColumn]
    [Reference(ReferenceType.OneToOne, ReferenceMemberName = "NodeId")]
    public ContentDto ContentDto { get; set; } = null!;
}
