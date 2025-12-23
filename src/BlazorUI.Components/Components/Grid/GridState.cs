using System.Text.Json.Serialization;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Represents the complete state of a grid (sorting, filtering, pagination, column state, selection).
/// Can be serialized to JSON for persistence.
/// </summary>
public class GridState
{
    /// <summary>
    /// Gets or sets the sort descriptors.
    /// </summary>
    [JsonPropertyName("sortDescriptors")]
    public List<GridSortDescriptor> SortDescriptors { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter descriptors.
    /// </summary>
    [JsonPropertyName("filterDescriptors")]
    public List<GridFilterDescriptor> FilterDescriptors { get; set; } = new();

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size (number of rows per page).
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets the column states (visibility, width, pinning, order).
    /// </summary>
    [JsonPropertyName("columnStates")]
    public List<GridColumnState> ColumnStates { get; set; } = new();

    /// <summary>
    /// Gets or sets the selected row IDs.
    /// </summary>
    [JsonPropertyName("selectedRowIds")]
    public List<object> SelectedRowIds { get; set; } = new();
}
