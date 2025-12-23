using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Represents the complete state of a grid, including sorting, filtering, pagination, and column configuration.
/// This state is JSON-serializable for persistence across sessions.
/// </summary>
public class GridState
{
    /// <summary>
    /// Gets or sets the list of sort descriptors applied to the grid.
    /// </summary>
    [JsonPropertyName("sortDescriptors")]
    public List<GridSortDescriptor> SortDescriptors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of filter descriptors applied to the grid.
    /// </summary>
    [JsonPropertyName("filterDescriptors")]
    public List<GridFilterDescriptor> FilterDescriptors { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets the list of column states (visibility, width, pinning, order).
    /// </summary>
    [JsonPropertyName("columnStates")]
    public List<GridColumnState> ColumnStates { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of selected row IDs.
    /// </summary>
    [JsonPropertyName("selectedRowIds")]
    public List<object> SelectedRowIds { get; set; } = new();
}
