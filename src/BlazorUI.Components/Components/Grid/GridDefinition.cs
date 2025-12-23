using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Internal representation of complete grid configuration.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
internal class GridDefinition<TItem>
{
    public List<GridColumnDefinition<TItem>> Columns { get; set; } = new();
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.None;
    public GridPagingMode PagingMode { get; set; } = GridPagingMode.Client;
    public GridVirtualizationMode VirtualizationMode { get; set; } = GridVirtualizationMode.Auto;
    public GridTheme Theme { get; set; } = GridTheme.Default;
    public GridDensity Density { get; set; } = GridDensity.Comfortable;
    public int PageSize { get; set; } = 25;
    public GridState? InitialState { get; set; }
    
    // Callbacks
    public EventCallback<GridState> OnStateChanged { get; set; }
    public EventCallback<GridDataRequest<TItem>> OnDataRequest { get; set; }
    public EventCallback<IReadOnlyCollection<TItem>> OnSelectionChanged { get; set; }
    
    // CSS
    public string? Class { get; set; }
    public string? Style { get; set; }
    
    // Localization
    public string? LocalizationKeyPrefix { get; set; }
    
    // Metadata for renderer
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
