namespace BlazorUI.Components.Filter;

/// <summary>
/// Represents a group of filter conditions with logical operator
/// </summary>
public class FilterGroup
{
    /// <summary>
    /// Unique identifier for this group
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Logical operator to combine conditions (AND/OR)
    /// </summary>
    public LogicalOperator Logic { get; set; } = LogicalOperator.And;
    
    /// <summary>
    /// List of conditions in this group
    /// </summary>
    public List<FilterCondition> Conditions { get; set; } = new();
    
    /// <summary>
    /// Nested filter groups for complex logic
    /// </summary>
    public List<FilterGroup> NestedGroups { get; set; } = new();
}
