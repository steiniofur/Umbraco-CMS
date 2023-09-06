namespace Umbraco.Cms.Core.Models.Elements;

//todo elements: split this into multiple files and think about naming and structure

/// <summary>
/// Tracks property values to elements in a detached state
/// From editor to full IContent/IElement mapping
/// </summary>
public class ElementPropertyValue
{
    public string Alias { get; set; }
    public object? Value { get; set; }

    public ElementPropertyValue(string alias, object? value)
    {
        Alias = alias;
        Value = value;
    }
}

/// <summary>
/// Tracks property values to elements in a detached state
/// From editor to full IContent/IElement mapping
/// </summary>
public class ElementValues
{
    public Guid ElementKey { get; set; }
    public Guid ElementTypeKey { get; set; }
    public List<ElementPropertyValue> Values { get; set; } = new List<ElementPropertyValue>();
}

public class WrappedElementsValues
{
    public IEnumerable<ElementValues> ElementValues { get; set; }
    public object? Value { get; set; }

    public WrappedElementsValues(IEnumerable<ElementValues> elementMappings, object? value)
    {
        ElementValues = elementMappings;
        Value = value;
    }
}
