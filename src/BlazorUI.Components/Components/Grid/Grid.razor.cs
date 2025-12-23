using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorUI.Components.Grid;

/// <summary>
/// A comprehensive grid component with sorting, filtering, pagination, and selection.
/// </summary>
/// <typeparam name="TItem">The type of data items in the grid.</typeparam>
/// <remarks>
/// <para>
/// Grid provides a complete data grid solution with declarative column definitions,
/// automatic data processing, and shadcn styling. It handles common grid features
/// out-of-the-box while maintaining flexibility through templates and callbacks.
/// </para>
/// <para>
/// Features:
/// - Declarative column API via GridColumn child components
/// - Automatic sorting, filtering, and pagination (client or server-side)
/// - Row selection (single/multiple) with checkboxes
/// - Optional toolbar with global search
/// - Empty and loading state templates
/// - Full shadcn styling with hover states and transitions
/// - State persistence (export/import GridState)
/// - Accessibility support (ARIA attributes, keyboard navigation)
/// - Column pinning, resizing, and reordering
/// - Virtualization support for large datasets
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Grid TItem="Person" Data="@people" SelectionMode="GridSelectionMode.Multiple"&gt;
///     &lt;Columns&gt;
///         &lt;GridColumn Property="@(p => p.Name)" Header="Name" Sortable Filterable /&gt;
///         &lt;GridColumn Property="@(p => p.Age)" Header="Age" Sortable /&gt;
///     &lt;/Columns&gt;
/// &lt;/Grid&gt;
/// </code>
/// </example>
public partial class Grid<TItem> : ComponentBase where TItem : class
{
    /// <summary>
    /// Internal column data structure.
    /// </summary>
    internal class ColumnData
    {
        public string Id { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public Func<TItem, object?> ValueSelector { get; set; } = null!;
        public bool Sortable { get; set; }
        public bool Filterable { get; set; }
        public bool Visible { get; set; } = true;
        public string? Width { get; set; }
        public string? MinWidth { get; set; }
        public string? MaxWidth { get; set; }
        public GridColumnPinPosition Pinned { get; set; }
        public bool AllowResize { get; set; } = true;
        public bool AllowReorder { get; set; } = true;
        public RenderFragment<TItem>? CellTemplate { get; set; }
        public RenderFragment? HeaderTemplate { get; set; }
        public RenderFragment? FilterTemplate { get; set; }
        public RenderFragment<TItem>? CellEditTemplate { get; set; }
        public string? CellClass { get; set; }
        public string? HeaderClass { get; set; }
        public Func<TItem, object?> Property { get; set; } = null!;
        public string EffectiveId { get; set; } = string.Empty;
        public string EffectiveField { get; set; } = string.Empty;
    }

    private List<ColumnData> _columns = new();
    private GridState _currentState = new();
    private IEnumerable<TItem> _processedData = Array.Empty<TItem>();
    private IEnumerable<TItem> _filteredData = Array.Empty<TItem>();
    private HashSet<TItem> _selectedItems = new();
    private string _globalSearchValue = string.Empty;
    private int _totalCount = 0;
    private int _filteredCount = 0;

    /// <summary>
    /// Gets or sets the data source for the grid.
    /// For client-side mode, this should contain all data.
    /// For server-side mode, this contains the current page data.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TItem> Data { get; set; } = Array.Empty<TItem>();

    /// <summary>
    /// Gets or sets the column definitions as child content.
    /// Use GridColumn components to define columns declaratively.
    /// </summary>
    [Parameter]
    public RenderFragment? Columns { get; set; }

    /// <summary>
    /// Gets or sets the paging mode (None, Client, Server, InfiniteScroll).
    /// Default is Client.
    /// </summary>
    [Parameter]
    public GridPagingMode PagingMode { get; set; } = GridPagingMode.Client;

    /// <summary>
    /// Gets or sets the row selection mode (None, Single, Multiple).
    /// Default is None.
    /// </summary>
    [Parameter]
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.None;

    /// <summary>
    /// Gets or sets the virtualization mode (Auto, None, RowOnly, RowAndColumn).
    /// Default is Auto.
    /// </summary>
    [Parameter]
    public GridVirtualizationMode VirtualizationMode { get; set; } = GridVirtualizationMode.Auto;

    /// <summary>
    /// Gets or sets the visual theme (Default, Striped, Bordered, Minimal).
    /// Default is Default.
    /// </summary>
    [Parameter]
    public GridTheme Theme { get; set; } = GridTheme.Default;

    /// <summary>
    /// Gets or sets the row density (Comfortable, Compact, Spacious).
    /// Default is Comfortable.
    /// </summary>
    [Parameter]
    public GridDensity Density { get; set; } = GridDensity.Comfortable;

    /// <summary>
    /// Gets or sets whether to show the toolbar.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show global search in the toolbar.
    /// Default is true when ShowToolbar is true.
    /// </summary>
    [Parameter]
    public bool ShowGlobalSearch { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show pagination controls.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool ShowPagination { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the grid is in a loading state.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets the available page size options.
    /// Default is [10, 25, 50, 100].
    /// </summary>
    [Parameter]
    public int[] PageSizes { get; set; } = { 10, 25, 50, 100 };

    /// <summary>
    /// Gets or sets custom toolbar content (buttons, etc.).
    /// </summary>
    [Parameter]
    public RenderFragment? ToolbarContent { get; set; }

    /// <summary>
    /// Gets or sets a custom template for the empty state.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets a custom template for the loading state.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets additional inline styles for the container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the grid.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the grid state for state binding.
    /// Use @bind-State for two-way binding.
    /// </summary>
    [Parameter]
    public GridState? State { get; set; }

    /// <summary>
    /// Event callback invoked when the state changes.
    /// </summary>
    [Parameter]
    public EventCallback<GridState> StateChanged { get; set; }

    /// <summary>
    /// Event callback invoked when selection changes.
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<TItem>> OnSelectionChanged { get; set; }

    /// <summary>
    /// Event callback invoked when a server-side data request is needed.
    /// Only used when PagingMode is Server.
    /// </summary>
    [Parameter]
    public EventCallback<GridDataRequest<TItem>> OnDataRequest { get; set; }

    /// <summary>
    /// Gets or sets the total count for server-side paging.
    /// Required when PagingMode is Server.
    /// </summary>
    [Parameter]
    public int? TotalCount { get; set; }

    /// <summary>
    /// Gets or sets a function to get a unique key for each item.
    /// Used for tracking selected items.
    /// Default uses object reference equality.
    /// </summary>
    [Parameter]
    public Func<TItem, object>? ItemKeySelector { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the container.
    /// </summary>
    private string ContainerCssClass => ClassNames.cn(
        "w-full space-y-4",
        Class
    );

    /// <summary>
    /// Gets the computed CSS classes for the table container.
    /// </summary>
    private string TableContainerCssClass => ClassNames.cn(
        "rounded-md border",
        Theme == GridTheme.Bordered ? "border-2" : ""
    );

    /// <summary>
    /// Gets the computed CSS classes for the table.
    /// </summary>
    private string TableCssClass => ClassNames.cn(
        "w-full caption-bottom text-sm",
        Density == GridDensity.Compact ? "text-xs" : "",
        Density == GridDensity.Spacious ? "text-base" : ""
    );

    /// <summary>
    /// Gets the computed CSS classes for header rows.
    /// </summary>
    private string HeaderRowCssClass => ClassNames.cn(
        "border-b transition-colors hover:bg-muted/50"
    );

    protected override void OnInitialized()
    {
        // Initialize state if not provided
        if (State != null)
        {
            _currentState = State;
        }
        else
        {
            _currentState.PageSize = PageSizes.FirstOrDefault(25);
            _currentState.PageNumber = 1;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        // Sync external state if provided
        if (State != null && State != _currentState)
        {
            _currentState = State;
        }

        await ProcessDataAsync();
    }

    /// <summary>
    /// Registers a column with the grid.
    /// </summary>
    internal void RegisterColumn<TValue>(GridColumn<TItem, TValue> column) where TValue : notnull
    {
        var columnData = new ColumnData
        {
            Id = column.EffectiveId,
            Field = column.EffectiveField,
            EffectiveId = column.EffectiveId,
            EffectiveField = column.EffectiveField,
            Header = column.Header,
            ValueSelector = item => column.Property(item),
            Property = item => column.Property(item),
            Sortable = column.Sortable,
            Filterable = column.Filterable,
            Visible = column.Visible,
            Width = column.Width,
            MinWidth = column.MinWidth,
            MaxWidth = column.MaxWidth,
            Pinned = column.Pinned,
            AllowResize = column.AllowResize,
            AllowReorder = column.AllowReorder,
            CellTemplate = column.CellTemplate,
            HeaderTemplate = column.HeaderTemplate,
            FilterTemplate = column.FilterTemplate,
            CellEditTemplate = column.CellEditTemplate,
            CellClass = column.CellClass,
            HeaderClass = column.HeaderClass
        };

        _columns.Add(columnData);
    }

    /// <summary>
    /// Processes the data through filtering, sorting, and pagination.
    /// </summary>
    private async Task ProcessDataAsync()
    {
        if (PagingMode == GridPagingMode.Server)
        {
            // Server-side mode: request data from server
            await RequestServerDataAsync();
            _processedData = Data ?? Array.Empty<TItem>();
            _totalCount = TotalCount ?? 0;
        }
        else
        {
            // Client-side mode: process data locally
            var data = Data ?? Array.Empty<TItem>();
            _totalCount = data.Count();

            // Apply filtering
            _filteredData = ApplyFiltering(data);
            _filteredCount = _filteredData.Count();

            // Apply sorting
            var sortedData = ApplySorting(_filteredData);

            // Apply pagination
            if (PagingMode == GridPagingMode.Client)
            {
                var skip = (_currentState.PageNumber - 1) * _currentState.PageSize;
                _processedData = sortedData.Skip(skip).Take(_currentState.PageSize).ToList();
            }
            else if (PagingMode == GridPagingMode.None)
            {
                _processedData = sortedData.ToList();
            }
        }

        // Notify state changed
        if (StateChanged.HasDelegate)
        {
            await StateChanged.InvokeAsync(_currentState);
        }
    }

    /// <summary>
    /// Requests data from the server for server-side paging.
    /// </summary>
    private async Task RequestServerDataAsync()
    {
        if (!OnDataRequest.HasDelegate)
        {
            throw new InvalidOperationException("OnDataRequest callback must be provided when using server-side paging.");
        }

        var request = new GridDataRequest<TItem>
        {
            StartIndex = (_currentState.PageNumber - 1) * _currentState.PageSize,
            Count = _currentState.PageSize,
            PageNumber = _currentState.PageNumber,
            PageSize = _currentState.PageSize,
            SortDescriptors = _currentState.SortDescriptors,
            FilterDescriptors = _currentState.FilterDescriptors
        };

        await OnDataRequest.InvokeAsync(request);
    }

    /// <summary>
    /// Applies filtering to the data.
    /// </summary>
    private IEnumerable<TItem> ApplyFiltering(IEnumerable<TItem> data)
    {
        var filtered = data;

        // Apply column filters
        foreach (var filterDescriptor in _currentState.FilterDescriptors)
        {
            var column = _columns.FirstOrDefault(c => c.EffectiveField == filterDescriptor.Field);
            if (column != null && filterDescriptor.Value != null)
            {
                filtered = filtered.Where(item =>
                {
                    var value = column.ValueSelector(item);
                    return ApplyFilterOperator(value, filterDescriptor.Value, filterDescriptor.Operator, filterDescriptor.CaseSensitive);
                });
            }
        }

        // Apply global search
        if (!string.IsNullOrWhiteSpace(_globalSearchValue))
        {
            filtered = filtered.Where(item =>
            {
                return _columns.Where(c => c.Filterable).Any(column =>
                {
                    var value = column.ValueSelector(item);
                    var stringValue = value?.ToString();
                    return !string.IsNullOrEmpty(stringValue) &&
                           stringValue.Contains(_globalSearchValue, StringComparison.OrdinalIgnoreCase);
                });
            });
        }

        return filtered;
    }

    /// <summary>
    /// Applies a filter operator to a value.
    /// </summary>
    private bool ApplyFilterOperator(object? value, object filterValue, GridFilterOperator op, bool caseSensitive)
    {
        var stringValue = value?.ToString() ?? "";
        var filterString = filterValue?.ToString() ?? "";

        var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        return op switch
        {
            GridFilterOperator.Equals => stringValue.Equals(filterString, comparison),
            GridFilterOperator.NotEquals => !stringValue.Equals(filterString, comparison),
            GridFilterOperator.Contains => stringValue.Contains(filterString, comparison),
            GridFilterOperator.NotContains => !stringValue.Contains(filterString, comparison),
            GridFilterOperator.StartsWith => stringValue.StartsWith(filterString, comparison),
            GridFilterOperator.EndsWith => stringValue.EndsWith(filterString, comparison),
            GridFilterOperator.IsEmpty => string.IsNullOrEmpty(stringValue),
            GridFilterOperator.IsNotEmpty => !string.IsNullOrEmpty(stringValue),
            GridFilterOperator.LessThan => CompareValues(value, filterValue) < 0,
            GridFilterOperator.LessThanOrEqual => CompareValues(value, filterValue) <= 0,
            GridFilterOperator.GreaterThan => CompareValues(value, filterValue) > 0,
            GridFilterOperator.GreaterThanOrEqual => CompareValues(value, filterValue) >= 0,
            _ => false
        };
    }

    /// <summary>
    /// Compares two values for numeric/date comparisons.
    /// </summary>
    private int CompareValues(object? a, object? b)
    {
        if (a is IComparable comparableA && b != null)
        {
            return comparableA.CompareTo(b);
        }
        return 0;
    }

    /// <summary>
    /// Applies sorting to the data.
    /// </summary>
    private IEnumerable<TItem> ApplySorting(IEnumerable<TItem> data)
    {
        if (!_currentState.SortDescriptors.Any())
            return data;

        IOrderedEnumerable<TItem>? ordered = null;

        foreach (var sortDescriptor in _currentState.SortDescriptors.OrderBy(s => s.Order))
        {
            var column = _columns.FirstOrDefault(c => c.EffectiveField == sortDescriptor.Field);
            if (column == null) continue;

            if (ordered == null)
            {
                ordered = sortDescriptor.Direction == GridSortDirection.Ascending
                    ? data.OrderBy(item => column.ValueSelector(item))
                    : data.OrderByDescending(item => column.ValueSelector(item));
            }
            else
            {
                ordered = sortDescriptor.Direction == GridSortDirection.Ascending
                    ? ordered.ThenBy(item => column.ValueSelector(item))
                    : ordered.ThenByDescending(item => column.ValueSelector(item));
            }
        }

        return ordered ?? data;
    }

    /// <summary>
    /// Handles column sort click.
    /// </summary>
    private async Task HandleColumnSort(string field)
    {
        var existingSort = _currentState.SortDescriptors.FirstOrDefault(s => s.Field == field);

        if (existingSort != null)
        {
            // Toggle direction
            if (existingSort.Direction == GridSortDirection.Ascending)
            {
                existingSort.Direction = GridSortDirection.Descending;
            }
            else
            {
                _currentState.SortDescriptors.Remove(existingSort);
            }
        }
        else
        {
            // Add new sort (clear others for single-column sort)
            _currentState.SortDescriptors.Clear();
            _currentState.SortDescriptors.Add(new GridSortDescriptor
            {
                Field = field,
                Direction = GridSortDirection.Ascending,
                Order = 0
            });
        }

        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Handles global search changes.
    /// </summary>
    private async Task HandleGlobalSearchChanged(string value)
    {
        _globalSearchValue = value;
        _currentState.PageNumber = 1; // Reset to first page
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Handles select all checkbox.
    /// </summary>
    private async Task HandleSelectAllChanged(bool isChecked)
    {
        if (isChecked)
        {
            foreach (var item in _processedData)
            {
                _selectedItems.Add(item);
            }
        }
        else
        {
            _selectedItems.Clear();
        }

        await NotifySelectionChanged();
        StateHasChanged();
    }

    /// <summary>
    /// Handles row selection change.
    /// </summary>
    private async Task HandleRowSelectionChanged(TItem item, bool isChecked)
    {
        if (isChecked)
        {
            if (SelectionMode == GridSelectionMode.Single)
            {
                _selectedItems.Clear();
            }
            _selectedItems.Add(item);
        }
        else
        {
            _selectedItems.Remove(item);
        }

        await NotifySelectionChanged();
        StateHasChanged();
    }

    /// <summary>
    /// Handles row click.
    /// </summary>
    private async Task HandleRowClick(TItem item)
    {
        if (SelectionMode == GridSelectionMode.Single)
        {
            _selectedItems.Clear();
            _selectedItems.Add(item);
            await NotifySelectionChanged();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Notifies selection changed.
    /// </summary>
    private async Task NotifySelectionChanged()
    {
        if (OnSelectionChanged.HasDelegate)
        {
            await OnSelectionChanged.InvokeAsync(_selectedItems.ToList().AsReadOnly());
        }
    }

    /// <summary>
    /// Checks if all rows are selected.
    /// </summary>
    private bool IsAllSelected()
    {
        if (!_processedData.Any()) return false;
        return _processedData.All(item => _selectedItems.Contains(item));
    }

    /// <summary>
    /// Checks if an item is selected.
    /// </summary>
    private bool IsItemSelected(TItem item)
    {
        return _selectedItems.Contains(item);
    }

    /// <summary>
    /// Gets selected items.
    /// </summary>
    private IReadOnlyCollection<TItem> GetSelectedItems()
    {
        return _selectedItems.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets visible columns in order.
    /// </summary>
    private IEnumerable<ColumnData> GetVisibleColumns()
    {
        return _columns.Where(c => c.Visible).OrderBy(c => c.Pinned == GridColumnPinPosition.Left ? 0 : c.Pinned == GridColumnPinPosition.Right ? 2 : 1);
    }

    /// <summary>
    /// Gets header cell CSS class.
    /// </summary>
    private string GetHeaderCellClass(ColumnData column)
    {
        return ClassNames.cn(
            "h-12 px-4 text-left align-middle font-medium text-muted-foreground",
            column.Sortable ? "cursor-pointer select-none" : "",
            column.HeaderClass
        );
    }

    /// <summary>
    /// Gets cell CSS class.
    /// </summary>
    private string GetCellClass(ColumnData column)
    {
        var padding = Density switch
        {
            GridDensity.Compact => "p-2",
            GridDensity.Spacious => "p-6",
            _ => "p-4"
        };

        return ClassNames.cn(
            padding,
            "align-middle",
            column.CellClass
        );
    }

    /// <summary>
    /// Gets row CSS class.
    /// </summary>
    private string GetRowClass(bool isSelected)
    {
        var baseClass = "border-b transition-colors hover:bg-muted/50";
        
        if (Theme == GridTheme.Striped)
        {
            baseClass += " even:bg-muted/20";
        }

        if (isSelected)
        {
            baseClass += " bg-muted";
        }

        if (SelectionMode == GridSelectionMode.Single)
        {
            baseClass += " cursor-pointer";
        }

        return ClassNames.cn(baseClass);
    }

    /// <summary>
    /// Gets column width style.
    /// </summary>
    private string? GetColumnWidthStyle(ColumnData column)
    {
        var styles = new List<string>();

        if (!string.IsNullOrWhiteSpace(column.Width))
            styles.Add($"width: {column.Width}");
        if (!string.IsNullOrWhiteSpace(column.MinWidth))
            styles.Add($"min-width: {column.MinWidth}");
        if (!string.IsNullOrWhiteSpace(column.MaxWidth))
            styles.Add($"max-width: {column.MaxWidth}");

        return styles.Any() ? string.Join("; ", styles) : null;
    }

    /// <summary>
    /// Gets total page count.
    /// </summary>
    private int GetTotalPages()
    {
        if (PagingMode == GridPagingMode.None) return 1;
        
        var count = PagingMode == GridPagingMode.Server ? (_totalCount) : _filteredCount;
        return (int)Math.Ceiling((double)count / _currentState.PageSize);
    }

    /// <summary>
    /// Handles page size change.
    /// </summary>
    private async Task HandlePageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var newSize))
        {
            _currentState.PageSize = newSize;
            _currentState.PageNumber = 1;
            await ProcessDataAsync();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles first page.
    /// </summary>
    private async Task HandleFirstPage()
    {
        _currentState.PageNumber = 1;
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Handles previous page.
    /// </summary>
    private async Task HandlePreviousPage()
    {
        if (_currentState.PageNumber > 1)
        {
            _currentState.PageNumber--;
            await ProcessDataAsync();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles next page.
    /// </summary>
    private async Task HandleNextPage()
    {
        if (_currentState.PageNumber < GetTotalPages())
        {
            _currentState.PageNumber++;
            await ProcessDataAsync();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles last page.
    /// </summary>
    private async Task HandleLastPage()
    {
        _currentState.PageNumber = GetTotalPages();
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Exports the current grid state as JSON.
    /// </summary>
    public string ExportState()
    {
        return JsonSerializer.Serialize(_currentState);
    }

    /// <summary>
    /// Imports grid state from JSON.
    /// </summary>
    public async Task ImportState(string json)
    {
        var state = JsonSerializer.Deserialize<GridState>(json);
        if (state != null)
        {
            _currentState = state;
            await ProcessDataAsync();
            StateHasChanged();
        }
    }
}
