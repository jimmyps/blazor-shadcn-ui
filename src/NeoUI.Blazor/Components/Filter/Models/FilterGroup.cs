namespace NeoUI.Blazor;

/// <summary>Represents a group of filter conditions with a logical operator.</summary>
public class FilterGroup
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public LogicalOperator Logic { get; set; } = LogicalOperator.And;
    public List<FilterCondition> Conditions { get; set; } = new();
    public List<FilterGroup> NestedGroups { get; set; } = new();
}
