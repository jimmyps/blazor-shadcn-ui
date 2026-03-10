namespace NeoUI.Blazor;

/// <summary>Represents a single filter condition.</summary>
public class FilterCondition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Field { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; }
    public object? Value { get; set; }
    public object? SecondaryValue { get; set; }
}
