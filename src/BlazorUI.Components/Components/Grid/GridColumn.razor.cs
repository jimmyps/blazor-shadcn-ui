using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Defines a column in a Grid component.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public partial class GridColumn<TItem> : ComponentBase
{
    /// <summary>
    /// Gets or sets the unique identifier for this column.
    /// If not specified, it will be generated from Field or Header.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the field name for data binding (e.g., "CustomerName").
    /// This is a string field name, NOT a lambda expression.
    /// </summary>
    [Parameter]
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    [Parameter, EditorRequired]
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the column is sortable.
    /// </summary>
    [Parameter]
    public bool Sortable { get; set; }

    /// <summary>
    /// Gets or sets whether the column is filterable.
    /// </summary>
    [Parameter]
    public bool Filterable { get; set; }

    /// <summary>
    /// Gets or sets the column width (e.g., "100px", "20%").
    /// </summary>
    [Parameter]
    public string? Width { get; set; }

    /// <summary>
    /// Gets or sets the minimum column width.
    /// </summary>
    [Parameter]
    public string? MinWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum column width.
    /// </summary>
    [Parameter]
    public string? MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets the column pinning position.
    /// </summary>
    [Parameter]
    public GridColumnPinPosition Pinned { get; set; } = GridColumnPinPosition.None;

    /// <summary>
    /// Gets or sets whether the column can be resized.
    /// </summary>
    [Parameter]
    public bool AllowResize { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column can be reordered.
    /// </summary>
    [Parameter]
    public bool AllowReorder { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column is visible.
    /// </summary>
    [Parameter]
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the cell template for rendering cell content.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? CellTemplate { get; set; }

    /// <summary>
    /// Gets or sets the header template for custom header rendering.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the filter template for custom filter UI.
    /// </summary>
    [Parameter]
    public RenderFragment? FilterTemplate { get; set; }

    /// <summary>
    /// Gets or sets the cell edit template for inline editing.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? CellEditTemplate { get; set; }

    /// <summary>
    /// Gets or sets the value selector function for extracting cell values.
    /// </summary>
    [Parameter]
    public Func<TItem, object?>? ValueSelector { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to cells in this column.
    /// </summary>
    [Parameter]
    public string? CellClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to the column header.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the format string for displaying cell values.
    /// Supports standard .NET format strings (e.g., "C" for currency, "N2" for numbers with 2 decimals, "d" for dates).
    /// Can also use composite format strings (e.g., "{0:C}", "{0:N2}").
    /// This is a simpler alternative to CellTemplate for basic formatting scenarios.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;GridColumn Field="Price" Header="Price" DataFormatString="C" /&gt;  // $1,234.56
    /// &lt;GridColumn Field="Quantity" Header="Quantity" DataFormatString="N0" /&gt;  // 1,234
    /// &lt;GridColumn Field="Date" Header="Date" DataFormatString="d" /&gt;  // 12/31/2024
    /// &lt;GridColumn Field="Percentage" Header="%" DataFormatString="P2" /&gt;  // 45.67%
    /// </code>
    /// </example>
    [Parameter]
    public string? DataFormatString { get; set; }

    [CascadingParameter]
    internal Grid<TItem>? ParentGrid { get; set; }

    protected override void OnInitialized()
    {
        if (ParentGrid == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GridColumn<TItem>)} must be placed inside a {nameof(Grid<TItem>)} component.");
        }

        ParentGrid.RegisterColumn(this);
    }

    internal GridColumnDefinition<TItem> ToDefinition()
    {
        // Generate ID from Field first, then Header as fallback
        // Use Guid suffix to ensure uniqueness if neither Field nor Header are suitable
        var generatedId = !string.IsNullOrEmpty(Field)
            ? Field
            : !string.IsNullOrEmpty(Header)
                ? $"{Header.ToLowerInvariant().Replace(" ", "-")}-{Guid.NewGuid().ToString("N")[..8]}"
                : Guid.NewGuid().ToString("N")[..8];

        // Detect field type using reflection
        Type? fieldType = null;
        string? agGridFilterType = null;

        if (!string.IsNullOrEmpty(Field))
        {
            var propertyInfo = typeof(TItem).GetProperty(
                Field,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase
            );

            if (propertyInfo != null)
            {
                fieldType = propertyInfo.PropertyType;

                // Auto-detect AG Grid filter type from .NET type
                agGridFilterType = DetectAgGridFilterType(fieldType);
            }
        }

        return new GridColumnDefinition<TItem>
        {
            Id = Id ?? generatedId,
            Field = Field,
            Header = Header,
            Sortable = Sortable,
            Filterable = Filterable,
            Width = Width,
            MinWidth = MinWidth,
            MaxWidth = MaxWidth,
            Pinned = Pinned,
            AllowResize = AllowResize,
            AllowReorder = AllowReorder,
            IsVisible = IsVisible,
            CellTemplate = CellTemplate,
            HeaderTemplate = HeaderTemplate,
            FilterTemplate = FilterTemplate,
            CellEditTemplate = CellEditTemplate,
            ValueSelector = ValueSelector,
            CellClass = CellClass,
            HeaderClass = HeaderClass,
            DataFormatString = DataFormatString,
            FieldType = fieldType,
            AgGridFilterType = agGridFilterType
        };
    }

    /// <summary>
    /// Detects the appropriate AG Grid filter type from a .NET property type.
    /// </summary>
    private static string DetectAgGridFilterType(Type type)
    {
        // Handle nullable types - get underlying type
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        // Numeric types (including enums) → agNumberColumnFilter
        if (underlyingType.IsEnum ||
            underlyingType == typeof(int) ||
            underlyingType == typeof(long) ||
            underlyingType == typeof(short) ||
            underlyingType == typeof(byte) ||
            underlyingType == typeof(decimal) ||
            underlyingType == typeof(double) ||
            underlyingType == typeof(float) ||
            underlyingType == typeof(uint) ||
            underlyingType == typeof(ulong) ||
            underlyingType == typeof(ushort) ||
            underlyingType == typeof(sbyte))
        {
            return "agNumberColumnFilter";
        }

        // Date/Time types → agDateColumnFilter
        if (underlyingType == typeof(DateTime) ||
            underlyingType == typeof(DateTimeOffset) ||
            underlyingType == typeof(DateOnly))
        {
            return "agDateColumnFilter";
        }

        // Boolean → agSetColumnFilter (for true/false selection)
        if (underlyingType == typeof(bool))
        {
            return "agSetColumnFilter";
        }

        // Default: text filter for strings and everything else
        return "agTextColumnFilter";
    }
}
