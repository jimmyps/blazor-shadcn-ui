using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Internal representation of a grid column configuration.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
internal class GridColumnDefinition<TItem>
{
    public string Id { get; set; } = string.Empty;
    public string? Field { get; set; }
    public string Header { get; set; } = string.Empty;
    public bool Sortable { get; set; }
    public bool Filterable { get; set; }
    public string? Width { get; set; }
    public string? MinWidth { get; set; }
    public string? MaxWidth { get; set; }
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;
    public bool AllowResize { get; set; } = true;
    public bool AllowReorder { get; set; } = true;
    public bool IsVisible { get; set; } = true;
    
    // Templates
    public RenderFragment<TItem>? CellTemplate { get; set; }
    public RenderFragment? HeaderTemplate { get; set; }
    public RenderFragment? FilterTemplate { get; set; }
    public RenderFragment<TItem>? CellEditTemplate { get; set; }
    
    // Value extraction
    public Func<TItem, object?>? ValueSelector { get; set; }
    
    // CSS classes
    public string? CellClass { get; set; }
    public string? HeaderClass { get; set; }
    
    // Metadata for renderer
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
