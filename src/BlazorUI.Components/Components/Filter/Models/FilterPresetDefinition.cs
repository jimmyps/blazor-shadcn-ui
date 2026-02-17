namespace BlazorUI.Components.Filter;

/// <summary>
/// Internal definition for a filter preset
/// </summary>
public class FilterPresetDefinition
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public FilterGroup Filters { get; set; } = new();
    public string? Description { get; set; }
}
