using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>Internal definition for a filter field extracted from FilterField child components.</summary>
public class FilterFieldDefinition
{
    public string Field { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public FilterFieldType Type { get; set; }
    public FilterEditorType EditorType { get; set; } = FilterEditorType.Auto;
    public List<FilterOperator> Operators { get; set; } = new();
    public FilterOperator? DefaultOperator { get; set; }
    public List<SelectOption>? Options { get; set; }
    public RenderFragment<FilterCondition>? CustomControl { get; set; }
    public string? Placeholder { get; set; }
    public object? Min { get; set; }
    public object? Max { get; set; }
    public object? Step { get; set; }
}
