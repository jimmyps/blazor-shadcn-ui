using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Filter;

/// <summary>
/// Internal definition for a filter field extracted from FilterField components
/// </summary>
public class FilterFieldDefinition
{
    public string Field { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public FilterFieldType Type { get; set; }
    public List<FilterOperator> Operators { get; set; } = new();
    public FilterOperator? DefaultOperator { get; set; }
    public List<SelectOption>? Options { get; set; }
    public RenderFragment<FilterCondition>? CustomControl { get; set; }
    public string? Placeholder { get; set; }
    public object? Min { get; set; }
    public object? Max { get; set; }
    public object? Step { get; set; }
}
