using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Internal representation of a grid configuration.
/// Used by renderers to configure the grid.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class GridDefinition<TItem>
{
    /// <summary>
    /// Gets or sets the list of column definitions.
    /// </summary>
    public List<GridColumnDefinition<TItem>> Columns { get; set; } = new();

    /// <summary>
    /// Gets or sets the selection mode for the grid.
    /// </summary>
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.None;

    /// <summary>
    /// Gets or sets the paging mode for the grid.
    /// </summary>
    public GridPagingMode PagingMode { get; set; } = GridPagingMode.Client;

    /// <summary>
    /// Gets or sets the virtualization mode for the grid.
    /// </summary>
    public GridVirtualizationMode VirtualizationMode { get; set; } = GridVirtualizationMode.Auto;

    /// <summary>
    /// Gets or sets the AG Grid theme to use (Alpine, Balham, Material, Quartz).
    /// </summary>
    public GridTheme Theme { get; set; } = GridTheme.Alpine;

    /// <summary>
    /// Gets or sets the visual style modifiers for the grid (Default, Striped, Bordered, Minimal).
    /// </summary>
    public GridStyle VisualStyle { get; set; } = GridStyle.Default;

    /// <summary>
    /// Gets or sets the spacing density for the grid.
    /// </summary>
    public GridDensity Density { get; set; } = GridDensity.Comfortable;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets the initial state of the grid.
    /// </summary>
    public GridState? InitialState { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the grid state changes.
    /// </summary>
    public EventCallback<GridState> OnStateChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when server-side data is requested.
    /// </summary>
    public EventCallback<GridDataRequest<TItem>> OnDataRequest { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the selection changes.
    /// </summary>
    public EventCallback<IReadOnlyCollection<TItem>> OnSelectionChanged { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to the grid container.
    /// </summary>
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline CSS style to apply to the grid container.
    /// </summary>
    public string? InlineStyle { get; set; }

    /// <summary>
    /// Gets or sets the localization key prefix for grid text resources.
    /// </summary>
    public string? LocalizationKeyPrefix { get; set; }

    /// <summary>
    /// Gets or sets theme parameters for AG Grid's withParams API.
    /// </summary>
    public Dictionary<string, object>? ThemeParams { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the renderer.
    /// </summary>
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
