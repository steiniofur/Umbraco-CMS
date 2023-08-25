using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Cms.Infrastructure.Persistence.Dtos;

/// <summary>
/// Which elements are being used in which property
/// </summary>
public class PropertyDataElementDto
{
    public const string TableName = Constants.DatabaseSchema.Tables.PropertyDataElement;

    [Column("propertyDataId")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    [ForeignKey(typeof(PropertyDataDto))]
    public int PropertyDataId { get; set; }

    [Column("elementId")]
    [PrimaryKeyColumn(AutoIncrement = false)]
    [ForeignKey(typeof(ElementDto))]
    public int ElementId { get; set; }
}
