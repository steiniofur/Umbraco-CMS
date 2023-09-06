using Umbraco.Cms.Core.Helpers;
using Umbraco.Cms.Core.Models.Elements;
using Umbraco.Cms.Core.Models.Entities;

namespace Umbraco.Cms.Core.Models;

public interface IProperty : IEntity, IRememberBeingDirty
{
    ValueStorageType ValueStorageType { get; }

    /// <summary>
    ///     Returns the PropertyType, which this Property is based on
    /// </summary>
    IPropertyType PropertyType { get; }

    /// <summary>
    ///     Gets the list of values.
    /// </summary>
    IReadOnlyCollection<IPropertyValue> Values { get; set; }

    /// <summary>
    ///     Gets the list of elements used in this property's values.
    /// </summary>
    public IReadOnlyCollection<IElement> Elements { get; }

    /// <summary>
    ///     Returns the Alias of the PropertyType, which this Property is based on
    /// </summary>
    string Alias { get; }

    int PropertyTypeId { get; }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    object? GetValue(string? culture = null, string? segment = null, bool published = false);

    /// <summary>
    ///     Sets a value.
    /// </summary>
    void SetValue(object? value, string? culture = null, string? segment = null);

    void PublishValues(string? culture = "*", string segment = "*");

    void UnpublishValues(string? culture = "*", string segment = "*");

    void SetLocalElements(IEnumerable<ElementValues> elementValues, string? culture, string? segment,
        PropertyValueManipulationHelper? propertyValueManipulationHelper = null);
}
