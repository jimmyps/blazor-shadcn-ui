using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Internal representation of a grid configuration.
/// Used by renderers to configure the grid.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class DataGridDefinition<TItem>
{
    /// <summary>
    /// Gets or sets the list of column definitions.
    /// </summary>
    public List<DataGridColumnDefinition<TItem>> Columns { get; set; } = new();

    /// <summary>
    /// Gets or sets the selection mode for the grid.
    /// </summary>
    public DataGridSelectionMode SelectionMode { get; set; } = DataGridSelectionMode.None;

    /// <summary>
    /// Gets or sets the paging mode for the grid.
    /// </summary>
    public DataGridPagingMode PagingMode { get; set; } = DataGridPagingMode.Client;

    /// <summary>
    /// Gets or sets the virtualization mode for the grid.
    /// </summary>
    public DataGridVirtualizationMode VirtualizationMode { get; set; } = DataGridVirtualizationMode.Auto;

    /// <summary>
    /// Gets or sets the AG DataGrid theme to use (Alpine, Balham, Material, Quartz).
    /// </summary>
    public DataGridTheme Theme { get; set; } = DataGridTheme.Alpine;

    /// <summary>
    /// Gets or sets the visual style modifiers for the grid (Default, Striped, Bordered, Minimal).
    /// </summary>
    public DataGridStyle VisualStyle { get; set; } = DataGridStyle.Default;

    /// <summary>
    /// Gets or sets the spacing density for the grid.
    /// </summary>
    public DataGridDensity Density { get; set; } = DataGridDensity.Comfortable;

    /// <summary>
    /// Gets or sets whether to suppress the header menus (filter/column menu).
    /// When true, columns will not show the menu icon even if filterable/sortable.
    /// This is useful for controlled filtering scenarios where you provide external filter UI.
    /// Default is false.
    /// </summary>
    public bool SuppressHeaderMenus { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets the property name to use as the unique row identifier.
    /// This is critical for row selection persistence across data updates.
    /// Common values: "Id" (C# convention), "id" (JavaScript convention), "_id" (MongoDB).
    /// If not specified, defaults to "Id".
    /// </summary>
    public string IdField { get; set; } = "Id";

    /// <summary>
    /// Gets or sets the current state of the grid.
    /// </summary>
    public DataGridState? State { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the grid state changes.
    /// </summary>
    public EventCallback<DataGridState> OnStateChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when server-side data is requested.
    /// </summary>
    public EventCallback<DataGridDataRequest<TItem>> OnDataRequest { get; set; }
    
    /// <summary>
    /// Gets or sets the row model type for the grid (ClientSide, ServerSide, Infinite).
    /// </summary>
    public string? RowModelType { get; set; }
    
    /// <summary>
    /// Gets or sets the server-side data request handler.
    /// This is a Func that returns data based on the request parameters.
    /// Used for ServerSide and Infinite row models.
    /// </summary>
    public Func<DataGridDataRequest<TItem>, Task<DataGridDataResponse<TItem>>>? ServerDataRequestHandler { get; set; }

    /// <summary>
    /// Gets or sets the BlazorServerSide data fetch handler.
    /// Called by AgDataGridRenderer when OnStateChangedAndFetchData is invoked from JS.
    /// Receives current grid state, fetches data, returns { items, totalCount }.
    /// </summary>
    public Func<DataGridState, Task<DataGridDataResponse<TItem>>>? BlazorServerSideFetchHandler { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the selection changes.
    /// </summary>
    public EventCallback<IReadOnlyCollection<TItem>> OnSelectionChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback for two-way binding of selected items.
    /// This enables @bind-SelectedItems support.
    /// </summary>
    public EventCallback<IReadOnlyCollection<TItem>> SelectedItemsChanged { get; set; }

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
    
    /// <summary>
    /// Gets or sets a callback to resolve item IDs back to original instances.
    /// Used by renderers to convert deserialized items (from JSON) to original references.
    /// This enables natural C# collection operations like Remove(item) to work correctly.
    /// </summary>
    /// <remarks>
    /// When AG Grid (or other renderers) serialize items to JSON and deserialize them back,
    /// they create NEW instances. This callback allows the renderer to get the ORIGINAL
    /// instances from the Grid's Items collection by matching on ID values.
    /// </remarks>
    public Func<IEnumerable<object>, IEnumerable<TItem>>? ResolveItemsByIds { get; set; }

    /// <summary>
    /// Gets or sets a callback that returns the string IDs of the currently selected items.
    /// Used in BlazorServerSide mode so the renderer can re-apply selection after each page
    /// navigation (the previous page's row nodes are replaced but their IDs are preserved).
    /// </summary>
    public Func<IReadOnlyCollection<string>>? GetSelectedIdsForRestore { get; set; }
}
