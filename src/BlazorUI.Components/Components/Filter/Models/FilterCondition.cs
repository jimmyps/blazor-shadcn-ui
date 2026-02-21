namespace BlazorUI.Components.Filter;

/// <summary>
/// Represents a single filter condition
/// </summary>
public class FilterCondition
{
    /// <summary>
    /// Unique identifier for this condition
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// The field name to filter on (property name from TData)
    /// </summary>
    public string Field { get; set; } = string.Empty;
    
    /// <summary>
    /// The operator to apply
    /// </summary>
    public FilterOperator Operator { get; set; }
    
    /// <summary>
    /// The value to compare against
    /// </summary>
    public object? Value { get; set; }
    
    /// <summary>
    /// Secondary value for operators like "Between"
    /// </summary>
    public object? SecondaryValue { get; set; }
}
