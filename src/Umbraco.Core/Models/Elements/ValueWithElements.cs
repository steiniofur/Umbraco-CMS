namespace Umbraco.Cms.Core.Models.Elements;

public class ValueWithElements
{
    public object? Value { get; set; }
    public List<ElementData> Elements { get; set; } = new();
}
