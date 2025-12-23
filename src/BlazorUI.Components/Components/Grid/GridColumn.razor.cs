using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Defines a column in a Grid with declarative syntax.
/// </summary>
/// <typeparam name="TItem">The type of data items in the grid.</typeparam>
/// <typeparam name="TValue">The type of the column's value.</typeparam>
/// <remarks>
/// <para>
/// GridColumn provides a declarative way to define grid columns using Razor syntax.
/// Each column specifies how to extract data (Property), display headers (Header), and
/// optionally render custom cell content (CellTemplate).
/// </para>
/// <para>
/// Features:
/// - Type-safe data access via Property parameter (Func&lt;TItem, TValue&gt;)
/// - Sortable and Filterable flags for automatic behavior
/// - Custom cell rendering via CellTemplate
/// - Column visibility toggle support
/// - Width configuration (Width, MinWidth, MaxWidth)
/// - Column pinning support (Left, Right, None)
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;GridColumn TItem="Person" TValue="string"
///             Property="@(p => p.Name)"
///             Header="Full Name"
///             Sortable="true"
///             Filterable="true" /&gt;
/// </code>
/// </example>
public partial class GridColumn<TItem, TValue> : ComponentBase where TItem : class where TValue : notnull
{
    /// <summary>
    /// Gets or sets the unique identifier for this column.
    /// If not provided, it will be auto-generated from the Header or Field.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the field name for this column (used for state persistence).
    /// If not provided, uses Id or Header.
    /// </summary>
    [Parameter]
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets the header text displayed for this column.
    /// </summary>
    [Parameter, EditorRequired]
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the function that extracts the column value from a data item.
    /// This enables type-safe data access.
    /// </summary>
    [Parameter, EditorRequired]
    public Func<TItem, TValue> Property { get; set; } = null!;

    /// <summary>
    /// Gets or sets whether this column can be sorted.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool Sortable { get; set; }

    /// <summary>
    /// Gets or sets whether this column can be filtered.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool Filterable { get; set; }

    /// <summary>
    /// Gets or sets whether this column is currently visible.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the width of the column (e.g., "200px", "20%", "auto").
    /// Null means the column will size automatically.
    /// </summary>
    [Parameter]
    public string? Width { get; set; }

    /// <summary>
    /// Gets or sets the minimum width of the column (e.g., "100px").
    /// Useful for responsive layouts.
    /// </summary>
    [Parameter]
    public string? MinWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of the column (e.g., "400px").
    /// Useful for preventing excessively wide columns.
    /// </summary>
    [Parameter]
    public string? MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets the column pinning position.
    /// Default is None.
    /// </summary>
    [Parameter]
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;

    /// <summary>
    /// Gets or sets whether this column can be resized by the user.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool AllowResize { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this column can be reordered by the user.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool AllowReorder { get; set; } = true;

    /// <summary>
    /// Gets or sets a custom template for rendering cell values.
    /// If null, the value is rendered using ToString().
    /// </summary>
    /// <remarks>
    /// The context parameter provides the data item (TItem) for the row.
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;GridColumn Property="@(p => p.Status)" Header="Status"&gt;
    ///     &lt;CellTemplate Context="person"&gt;
    ///         &lt;Badge Variant="@(person.Status == "Active" ? BadgeVariant.Default : BadgeVariant.Destructive)"&gt;
    ///             @person.Status
    ///         &lt;/Badge&gt;
    ///     &lt;/CellTemplate&gt;
    /// &lt;/GridColumn&gt;
    /// </code>
    /// </example>
    [Parameter]
    public RenderFragment<TItem>? CellTemplate { get; set; }

    /// <summary>
    /// Gets or sets a custom template for rendering the column header.
    /// If null, the Header text is displayed.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets a custom template for rendering the filter UI.
    /// If null, a default text input is used for filterable columns.
    /// </summary>
    [Parameter]
    public RenderFragment? FilterTemplate { get; set; }

    /// <summary>
    /// Gets or sets a custom template for rendering cell values in edit mode.
    /// If null, editing is not supported for this column.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? CellEditTemplate { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to cells in this column.
    /// </summary>
    [Parameter]
    public string? CellClass { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the header cell.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the parent Grid component.
    /// Automatically set via cascading parameter.
    /// </summary>
    [CascadingParameter]
    internal Grid<TItem>? ParentGrid { get; set; }

    /// <summary>
    /// Gets the effective column ID (uses Id if provided, Field if provided, otherwise generates from Header).
    /// </summary>
    internal string EffectiveId => Id ?? Field ?? Header.ToLowerInvariant().Replace(" ", "-");

    /// <summary>
    /// Gets the effective field name (uses Field if provided, otherwise uses EffectiveId).
    /// </summary>
    internal string EffectiveField => Field ?? EffectiveId;

    protected override void OnInitialized()
    {
        if (ParentGrid == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GridColumn<TItem, TValue>)} must be placed inside a {nameof(Grid<TItem>)} component.");
        }

        // Register this column with the parent grid
        ParentGrid.RegisterColumn(this);
    }
}
