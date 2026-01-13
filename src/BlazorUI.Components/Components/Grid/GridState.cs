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
    
    // Row grouping
    
    /// <summary>
    /// Gets or sets the list of columns used for row grouping.
    /// </summary>
    [JsonPropertyName("rowGroupColumns")]
    public List<string> RowGroupColumns { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the list of expanded row group IDs.
    /// </summary>
    [JsonPropertyName("expandedRowGroups")]
    public List<object> ExpandedRowGroups { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the list of rows pinned to the top.
    /// </summary>
    [JsonPropertyName("pinnedTopRows")]
    public List<object> PinnedTopRows { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the list of rows pinned to the bottom.
    /// </summary>
    [JsonPropertyName("pinnedBottomRows")]
    public List<object> PinnedBottomRows { get; set; } = new();
    
    // Focus and selection
    
    /// <summary>
    /// Gets or sets the currently focused cell.
    /// </summary>
    [JsonPropertyName("focusedCell")]
    public FocusedCell? FocusedCell { get; set; }
    
    /// <summary>
    /// Gets or sets the cell range selection (for range selection mode).
    /// </summary>
    [JsonPropertyName("cellRangeSelection")]
    public List<CellRange> CellRangeSelection { get; set; } = new();
    
    // Advanced filtering
    
    /// <summary>
    /// Gets or sets the advanced filter model (for AG Grid's advanced filter).
    /// </summary>
    [JsonPropertyName("advancedFilterModel")]
    public object? AdvancedFilterModel { get; set; }
    
    // Sidebar
    
    /// <summary>
    /// Gets or sets the sidebar state.
    /// </summary>
    [JsonPropertyName("sideBar")]
    public SideBarState? SideBar { get; set; }
    
    // Pivot
    
    /// <summary>
    /// Gets or sets whether pivot mode is enabled.
    /// </summary>
    [JsonPropertyName("pivotMode")]
    public bool PivotMode { get; set; }
    
    /// <summary>
    /// Gets or sets the list of columns used for pivoting.
    /// </summary>
    [JsonPropertyName("pivotColumns")]
    public List<string> PivotColumns { get; set; } = new();
    
    // Scroll position
    
    /// <summary>
    /// Gets or sets the scroll position.
    /// </summary>
    [JsonPropertyName("scroll")]
    public ScrollPosition? Scroll { get; set; }
    
    // Version for migration
    
    /// <summary>
    /// Gets or sets the state version for migration support.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

/// <summary>
/// Represents a focused cell in the grid.
/// </summary>
public class FocusedCell
{
    /// <summary>
    /// Gets or sets the row index of the focused cell.
    /// </summary>
    [JsonPropertyName("rowIndex")]
    public int RowIndex { get; set; }
    
    /// <summary>
    /// Gets or sets the column ID of the focused cell.
    /// </summary>
    [JsonPropertyName("columnId")]
    public string ColumnId { get; set; } = string.Empty;
}

/// <summary>
/// Represents a cell range selection.
/// </summary>
public class CellRange
{
    /// <summary>
    /// Gets or sets the starting row index.
    /// </summary>
    [JsonPropertyName("startRow")]
    public int StartRow { get; set; }
    
    /// <summary>
    /// Gets or sets the ending row index.
    /// </summary>
    [JsonPropertyName("endRow")]
    public int EndRow { get; set; }
    
    /// <summary>
    /// Gets or sets the list of column IDs in the range.
    /// </summary>
    [JsonPropertyName("columns")]
    public List<string> Columns { get; set; } = new();
}

/// <summary>
/// Represents the sidebar state.
/// </summary>
public class SideBarState
{
    /// <summary>
    /// Gets or sets whether the sidebar is visible.
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; }
    
    /// <summary>
    /// Gets or sets the active panel ID.
    /// </summary>
    [JsonPropertyName("activePanel")]
    public string? ActivePanel { get; set; }
}

/// <summary>
/// Represents the scroll position.
/// </summary>
public class ScrollPosition
{
    /// <summary>
    /// Gets or sets the vertical scroll position in pixels.
    /// </summary>
    [JsonPropertyName("top")]
    public int Top { get; set; }
    
    /// <summary>
    /// Gets or sets the horizontal scroll position in pixels.
    /// </summary>
    [JsonPropertyName("left")]
    public int Left { get; set; }
}
