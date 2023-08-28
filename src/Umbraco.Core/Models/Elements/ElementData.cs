// Copyright (c) Umbraco.
// See LICENSE for more details.

namespace Umbraco.Cms.Core.Models.Elements;

/// <summary>
///     Represents a single block's data in raw form
/// </summary>
public class ElementData
{
    public Guid ContentTypeKey { get; set; }

    public string ContentTypeAlias { get; set; } = string.Empty;

    public Udi? Udi { get; set; }

    public Guid Key => Udi is not null ? ((GuidUdi)Udi).Guid : throw new InvalidOperationException("No Udi assigned");

    public IDictionary<string, ElementPropertyValue> PropertyValues { get; set; } =
        new Dictionary<string, ElementPropertyValue>();

    public class ElementPropertyValue
    {
        public ElementPropertyValue(object? value, IPropertyType propertyType)
        {
            Value = value;
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
        }

        public object? Value { get; }

        public IPropertyType PropertyType { get; }
    }
}
