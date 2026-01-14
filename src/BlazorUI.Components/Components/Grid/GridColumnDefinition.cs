using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Internal representation of a grid column configuration.
/// Used by renderers to configure grid columns.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public class GridColumnDefinition<TItem>
{
    /// <summary>
    /// Gets or sets the unique identifier for this column.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field name for data binding (e.g., "CustomerName").
    /// This is a string field name, NOT a lambda expression.
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the column is sortable.
    /// </summary>
    public bool Sortable { get; set; }

    /// <summary>
    /// Gets or sets whether the column is filterable.
    /// </summary>
    public bool Filterable { get; set; }

    /// <summary>
    /// Gets or sets the column width (e.g., "100px", "20%").
    /// </summary>
    public string? Width { get; set; }

    /// <summary>
    /// Gets or sets the minimum column width.
    /// </summary>
    public string? MinWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum column width.
    /// </summary>
    public string? MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets the column pinning position.
    /// </summary>
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;

    /// <summary>
    /// Gets or sets whether the column can be resized.
    /// </summary>
    public bool AllowResize { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column can be reordered.
    /// </summary>
    public bool AllowReorder { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the cell template for rendering cell content.
    /// </summary>
    public RenderFragment<TItem>? CellTemplate { get; set; }

    /// <summary>
    /// Gets or sets the header template for custom header rendering.
    /// </summary>
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the filter template for custom filter UI.
    /// </summary>
    public RenderFragment? FilterTemplate { get; set; }

    /// <summary>
    /// Gets or sets the cell edit template for inline editing.
    /// </summary>
    public RenderFragment<TItem>? CellEditTemplate { get; set; }

    /// <summary>
    /// Gets or sets the value selector function for extracting cell values.
    /// </summary>
    public Func<TItem, object?>? ValueSelector { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to cells in this column.
    /// </summary>
    public string? CellClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to the column header.
    /// </summary>
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the format string for displaying cell values.
    /// Supports standard .NET format strings (e.g., "C", "N2", "d", "P2").
    /// </summary>
    public string? DataFormatString { get; set; }

    /// <summary>
    /// Gets or sets the detected .NET type of this column's field.
    /// Used by renderers to automatically configure appropriate filters and editors.
    /// </summary>
    public Type? FieldType { get; set; }

    /// <summary>
    /// Gets or sets the AG Grid filter type to use for this column.
    /// Auto-detected from FieldType if not explicitly set.
    /// Possible values: agTextColumnFilter, agNumberColumnFilter, agDateColumnFilter, agSetColumnFilter.
    /// </summary>
    public string? AgGridFilterType { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the renderer.
    /// </summary>
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
