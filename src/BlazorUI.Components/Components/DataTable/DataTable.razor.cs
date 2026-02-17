using BlazorUI.Components.Utilities;
using BlazorUI.Primitives.Table;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.DataTable;

/// <summary>
/// A styled data table component that wraps the Table Primitive with automatic sorting,
/// filtering, pagination, and row selection capabilities.
/// </summary>
/// <typeparam name="TData">The type of data items in the table.</typeparam>
/// <remarks>
/// <para>
/// DataTable provides a complete table solution with declarative column definitions,
/// automatic data processing, and shadcn styling. It handles common table features
/// out-of-the-box while maintaining flexibility through templates and callbacks.
/// </para>
/// <para>
/// Features:
/// - Declarative column API via DataTableColumn child components
/// - Automatic sorting, filtering, and pagination (hybrid mode with overrides)
/// - Row selection (single/multiple) with checkboxes
/// - Optional toolbar with global search and column visibility toggle
/// - Empty and loading state templates
/// - Full shadcn styling with hover states and transitions
/// - Accessibility support (ARIA attributes, keyboard navigation)
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;DataTable TData="Person" Data="@people" SelectionMode="DataTableSelectionMode.Multiple"&gt;
///     &lt;Columns&gt;
///         &lt;DataTableColumn Property="@(p => p.Name)" Header="Name" Sortable Filterable /&gt;
///         &lt;DataTableColumn Property="@(p => p.Age)" Header="Age" Sortable /&gt;
///     &lt;/Columns&gt;
/// &lt;/DataTable&gt;
/// </code>
/// </example>
public partial class DataTable<TData> : ComponentBase where TData : class
{
    /// <summary>
    /// Public class for storing column data without component parameters.
    /// This avoids BL0005 warnings when creating column instances programmatically.
    /// </summary>
    public class ColumnData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the column.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the header text displayed for this column.
        /// </summary>
        public string Header { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property accessor function that retrieves the column value from a data item.
        /// </summary>
        public Func<TData, object> Property { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether the column can be sorted.
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// Gets or sets whether the column is included in global search filtering.
        /// </summary>
        public bool Filterable { get; set; }

        /// <summary>
        /// Gets or sets whether the column is currently visible.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the width CSS value for the column (e.g., "200px", "20%").
        /// </summary>
        public string? Width { get; set; }

        /// <summary>
        /// Gets or sets the minimum width CSS value for the column.
        /// </summary>
        public string? MinWidth { get; set; }

        /// <summary>
        /// Gets or sets the maximum width CSS value for the column.
        /// </summary>
        public string? MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the custom render template for cells in this column.
        /// </summary>
        public RenderFragment<TData>? CellTemplate { get; set; }

        /// <summary>
        /// Gets or sets additional CSS classes to apply to cells in this column.
        /// </summary>
        public string? CellClass { get; set; }

        /// <summary>
        /// Gets or sets additional CSS classes to apply to the column header.
        /// </summary>
        public string? HeaderClass { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment for the column content.
        /// </summary>
        public ColumnAlignment Alignment { get; set; } = ColumnAlignment.Left;
    }

    /// <summary>
    /// Stores the list of registered column definitions.
    /// </summary>
    private List<ColumnData> _columns = new();

    /// <summary>
    /// Maintains the table state including sorting, filtering, pagination, and selection.
    /// </summary>
    private TableState<TData> _tableState = new();

    /// <summary>
    /// Contains the final processed data after filtering, sorting, and pagination.
    /// </summary>
    private IEnumerable<TData> _processedData = Array.Empty<TData>();

    /// <summary>
    /// Contains the data after filtering but before pagination.
    /// </summary>
    private IEnumerable<TData> _filteredData = Array.Empty<TData>();

    /// <summary>
    /// Stores the current global search filter value.
    /// </summary>
    private string _globalSearchValue = string.Empty;

    /// <summary>
    /// Version counter for column visibility changes to track render dependencies.
    /// </summary>
    private int _columnsVersion = 0;

    /// <summary>
    /// Tracks whether the select-all dropdown is currently open.
    /// </summary>
    private bool _selectAllDropdownOpen = false;

    /// <summary>
    /// Cached reference to the last data collection for ShouldRender optimization.
    /// </summary>
    private IEnumerable<TData>? _lastData;

    /// <summary>
    /// Cached selection mode for ShouldRender optimization.
    /// </summary>
    private DataTableSelectionMode _lastSelectionMode;

    /// <summary>
    /// Cached loading state for ShouldRender optimization.
    /// </summary>
    private bool _lastIsLoading;

    /// <summary>
    /// Cached columns version for ShouldRender optimization.
    /// </summary>
    private int _lastColumnsVersion;

    /// <summary>
    /// Cached global search value for ShouldRender optimization.
    /// </summary>
    private string _lastGlobalSearchValue = string.Empty;

    /// <summary>
    /// Version counter for selection changes to track render dependencies.
    /// </summary>
    private int _selectionVersion = 0;

    /// <summary>
    /// Cached selection version for ShouldRender optimization.
    /// </summary>
    private int _lastSelectionVersion = 0;

    /// <summary>
    /// Cached selected items collection for parameter change detection.
    /// </summary>
    private IReadOnlyCollection<TData>? _lastSelectedItems;

    /// <summary>
    /// Version counter for pagination changes to track render dependencies.
    /// </summary>
    private int _paginationVersion = 0;

    /// <summary>
    /// Cached pagination version for ShouldRender optimization.
    /// </summary>
    private int _lastPaginationVersion = 0;

    /// <summary>
    /// Gets or sets the data source for the table.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TData> Data { get; set; } = Array.Empty<TData>();

    /// <summary>
    /// Gets or sets the column definitions as child content.
    /// Use DataTableColumn components to define columns declaratively.
    /// </summary>
    [Parameter]
    public RenderFragment? Columns { get; set; }

    /// <summary>
    /// Gets or sets the row selection mode.
    /// Default is None (no selection).
    /// </summary>
    [Parameter]
    public DataTableSelectionMode SelectionMode { get; set; } = DataTableSelectionMode.None;

    /// <summary>
    /// Gets or sets whether to show the toolbar with global search and column visibility.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show pagination controls.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool ShowPagination { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the table is in a loading state.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets whether keyboard navigation is enabled for table rows.
    /// When true, rows can be navigated with arrow keys and selected with Enter/Space.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool EnableKeyboardNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets the available page size options.
    /// Default is [5, 10, 20, 50, 100].
    /// </summary>
    [Parameter]
    public int[] PageSizes { get; set; } = { 5, 10, 20, 50, 100 };

    /// <summary>
    /// Gets or sets the initial page size.
    /// Default is 5.
    /// </summary>
    [Parameter]
    public int InitialPageSize { get; set; } = 5;

    /// <summary>
    /// Gets or sets custom toolbar actions (buttons, etc.).
    /// </summary>
    [Parameter]
    public RenderFragment? ToolbarActions { get; set; }

    /// <summary>
    /// Gets or sets a custom template for the empty state.
    /// If null, displays default "No results found" message.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets a custom template for the loading state.
    /// If null, displays default "Loading..." message.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the container div.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the table.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the selected items.
    /// Use @bind-SelectedItems for two-way binding.
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<TData> SelectedItems { get; set; } = Array.Empty<TData>();

    /// <summary>
    /// Event callback invoked when the selected items change due to user interaction.
    /// Used for two-way binding with @bind-SelectedItems.
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<TData>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Event callback invoked when the user changes the sort column or direction.
    /// Provides the column ID and sort direction for implementing custom sorting logic.
    /// </summary>
    [Parameter]
    public EventCallback<(string ColumnId, SortDirection Direction)> OnSort { get; set; }

    /// <summary>
    /// Event callback invoked when the user changes the global search filter value.
    /// Provides the search text for implementing custom filtering logic.
    /// </summary>
    [Parameter]
    public EventCallback<string?> OnFilter { get; set; }

    /// <summary>
    /// Gets or sets a custom function to preprocess data before filtering, sorting, and pagination.
    /// Use this to transform data, apply server-side operations, or fetch additional details.
    /// </summary>
    [Parameter]
    public Func<IEnumerable<TData>, Task<IEnumerable<TData>>>? PreprocessData { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the outer container element.
    /// </summary>
    private string ContainerCssClass => ClassNames.cn(
        "w-full space-y-4",
        Class
    );

    /// <summary>
    /// Gets the computed CSS classes for the table container element.
    /// </summary>
    private string TableContainerCssClass => ClassNames.cn(
        "rounded-md border"
    );

    /// <summary>
    /// Gets the computed CSS classes for the table element.
    /// </summary>
    private string TableCssClass => ClassNames.cn(
        "w-full caption-bottom text-sm"
    );

    /// <summary>
    /// Initializes the component and sets up default pagination and selection state.
    /// </summary>
    protected override void OnInitialized()
    {
        _tableState.Pagination.PageSize = InitialPageSize;
        _tableState.Pagination.CurrentPage = 1;
        // Set selection mode on the state so Select/Deselect methods work correctly
        _tableState.Selection.Mode = GetPrimitiveSelectionMode();
    }

    /// <summary>
    /// Synchronizes parameters with internal state and processes the data.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        // Keep selection mode in sync with parameter
        _tableState.Selection.Mode = GetPrimitiveSelectionMode();

        // Sync SelectedItems parameter to internal state if changed externally
        // Skip if SelectedItems is the same reference as our internal collection (shouldn't happen with the copy we make, but defensive)
        if (!ReferenceEquals(SelectedItems, _lastSelectedItems) &&
            !ReferenceEquals(SelectedItems, _tableState.Selection.SelectedItems))
        {
            _tableState.Selection.Clear();
            foreach (var item in SelectedItems)
            {
                _tableState.Selection.Select(item);
            }
            _lastSelectedItems = SelectedItems;
            _selectionVersion++;
        }

        await ProcessDataAsync();
    }

    /// <summary>
    /// Registers a column with the data table during component initialization.
    /// Called internally by DataTableColumn components when they are initialized.
    /// </summary>
    /// <typeparam name="TValue">The type of the column's property value.</typeparam>
    /// <param name="column">The column component to register.</param>
    internal void RegisterColumn<TValue>(DataTableColumn<TData, TValue> column) where TValue : notnull
    {
        // Create internal column data structure (avoids BL0005 component parameter warnings)
        var columnData = new ColumnData
        {
            Id = column.Id ?? column.Header.ToLowerInvariant().Replace(" ", "-"),
            Header = column.Header,
            Property = item =>
            {
                var value = column.Property(item);
                return value ?? throw new InvalidOperationException($"Column '{column.Header}' returned null for a non-nullable type.");
            },
            Sortable = column.Sortable,
            Filterable = column.Filterable,
            Visible = column.Visible,
            Width = column.Width,
            MinWidth = column.MinWidth,
            MaxWidth = column.MaxWidth,
            CellTemplate = column.CellTemplate,
            CellClass = column.CellClass,
            HeaderClass = column.HeaderClass,
            Alignment = column.Alignment
        };

        _columns.Add(columnData);
    }

    /// <summary>
    /// Processes the data through the complete pipeline: preprocessing, filtering, sorting, and pagination.
    /// Updates both the filtered and final processed data collections.
    /// </summary>
    private async Task ProcessDataAsync()
    {
        var data = Data ?? Array.Empty<TData>();

        // 1. Preprocess (if custom function provided)
        if (PreprocessData != null)
        {
            data = await PreprocessData(data);
        }

        // 2. Apply filtering (column filters + global search)
        _filteredData = ApplyFiltering(data);

        // 3. Apply sorting
        var sortedData = ApplySorting(_filteredData);

        // 4. Update pagination total items BEFORE pagination
        _tableState.Pagination.TotalItems = sortedData.Count();

        // 5. Apply pagination
        _processedData = sortedData
            .Skip(_tableState.Pagination.StartIndex)
            .Take(_tableState.Pagination.PageSize)
            .ToList();
    }

    /// <summary>
    /// Applies global search filtering across all filterable columns.
    /// Returns the filtered data collection.
    /// </summary>
    /// <param name="data">The data to filter.</param>
    /// <returns>The filtered data matching the global search criteria.</returns>
    private IEnumerable<TData> ApplyFiltering(IEnumerable<TData> data)
    {
        if (string.IsNullOrWhiteSpace(_globalSearchValue))
        {
            return data;
        }

        // Cache search value to avoid repeated property access in closure
        var searchValue = _globalSearchValue;

        // Pre-filter to only filterable columns to reduce iterations
        var filterableColumns = _columns.Where(c => c.Filterable).ToList();
        if (filterableColumns.Count == 0)
        {
            filterableColumns = _columns; // Fall back to all columns if none marked filterable
        }

        return data.Where(item => MatchesSearch(item, searchValue, filterableColumns));
    }

    /// <summary>
    /// Checks if a data item matches the search value in any of the specified columns.
    /// Uses case-insensitive string comparison.
    /// </summary>
    /// <param name="item">The data item to check.</param>
    /// <param name="searchValue">The search text to match against.</param>
    /// <param name="columns">The columns to search within.</param>
    /// <returns>True if the item matches the search value in any column; otherwise, false.</returns>
    private static bool MatchesSearch(TData item, string searchValue, List<ColumnData> columns)
    {
        foreach (var column in columns)
        {
            try
            {
                var value = column.Property(item);
                if (value == null) continue;

                var stringValue = value.ToString();
                if (!string.IsNullOrEmpty(stringValue) &&
                    stringValue.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
                // Skip columns that cause errors during property access
            }
        }
        return false;
    }

    /// <summary>
    /// Applies sorting to the data based on the current sort column and direction.
    /// Returns the data unchanged if no sorting is active.
    /// </summary>
    /// <param name="data">The data to sort.</param>
    /// <returns>The sorted data collection.</returns>
    private IEnumerable<TData> ApplySorting(IEnumerable<TData> data)
    {
        if (_tableState.Sorting.Direction == SortDirection.None)
            return data;

        var column = _columns.FirstOrDefault(c => c.Id == _tableState.Sorting.SortedColumn);
        if (column == null)
            return data;

        var sorted = _tableState.Sorting.Direction == SortDirection.Ascending
            ? data.OrderBy(item => column.Property(item))
            : data.OrderByDescending(item => column.Property(item));

        return sorted;
    }

    /// <summary>
    /// Handles sort change events triggered by column header clicks.
    /// Invokes the OnSort callback and reprocesses the data.
    /// </summary>
    /// <param name="sortInfo">The column ID and sort direction.</param>
    private async Task HandleSortChange((string ColumnId, SortDirection Direction) sortInfo)
    {
        // Invoke custom callback if provided
        if (OnSort.HasDelegate)
        {
            await OnSort.InvokeAsync(sortInfo);
        }

        // Automatic sorting will happen in ProcessDataAsync via state binding
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Handles changes to the global search input.
    /// Resets pagination to the first page and reprocesses the data.
    /// </summary>
    /// <param name="value">The new search value.</param>
    private async Task HandleGlobalSearchChanged(string value)
    {
        _globalSearchValue = value;

        // Invoke custom callback if provided
        if (OnFilter.HasDelegate)
        {
            await OnFilter.InvokeAsync(_globalSearchValue);
        }

        // Reset to first page when filtering
        _tableState.Pagination.CurrentPage = 1;

        await ProcessDataAsync();
        // StateHasChanged() not needed - Blazor auto-renders after async event handlers
    }

    /// <summary>
    /// Handles column visibility toggle events from the column visibility menu.
    /// Updates the column's visibility state and triggers a re-render.
    /// </summary>
    /// <param name="columnId">The ID of the column to toggle.</param>
    /// <param name="visible">The new visibility state.</param>
    private void HandleColumnVisibilityChanged(string columnId, bool visible)
    {
        var column = _columns.FirstOrDefault(c => c.Id == columnId);
        if (column != null)
        {
            column.Visible = visible;

            // Increment version to signal change without list recreation
            _columnsVersion++;

            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles selection changes from the underlying table primitive.
    /// Invokes the SelectedItemsChanged callback with a defensive copy of the selection.
    /// </summary>
    /// <param name="selectedItems">The collection of currently selected items.</param>
    private async Task HandleSelectionChange(IReadOnlyCollection<TData> selectedItems)
    {
        if (SelectedItemsChanged.HasDelegate)
        {
            // Pass a copy to avoid reference aliasing - if parent stores the reference
            // and we later call Clear(), it would clear the parent's collection too
            await SelectedItemsChanged.InvokeAsync(selectedItems.ToList().AsReadOnly());
        }
    }

    /// <summary>
    /// Determines whether to show the select-all dropdown menu.
    /// Returns true when there are more items in the dataset than currently visible on the page.
    /// </summary>
    /// <returns>True if the select-all dropdown should be shown; otherwise, false.</returns>
    private bool ShouldShowSelectAllPrompt()
    {
        return _tableState.Pagination.TotalItems > _processedData.Count();
    }

    /// <summary>
    /// Gets the total count of items after filtering but before pagination.
    /// This represents all items matching the current filter criteria.
    /// </summary>
    /// <returns>The total filtered item count.</returns>
    private int GetTotalFilteredItemCount()
    {
        return _filteredData.Count();
    }

    /// <summary>
    /// Opens the select-all dropdown menu that allows choosing between selecting current page or all items.
    /// </summary>
    private void OpenSelectAllDropdown()
    {
        _selectAllDropdownOpen = true;
        StateHasChanged();
    }

    /// <summary>
    /// Handles the select-all checkbox state change.
    /// Opens the dropdown menu if multiple pages exist, otherwise selects all items on the current page.
    /// </summary>
    /// <param name="isChecked">True if the checkbox was checked; false if unchecked.</param>
    private async Task HandleSelectAllChanged(bool isChecked)
    {
        if (!isChecked)
        {
            await HandleClearSelection();
            return;
        }

        if (ShouldShowSelectAllPrompt())
        {
            _selectAllDropdownOpen = true;
            StateHasChanged();
            return;
        }

        await HandleSelectAllOnCurrentPage();
    }

    /// <summary>
    /// Selects all items visible on the current page and closes the select-all dropdown.
    /// </summary>
    private async Task HandleSelectAllOnCurrentPage()
    {
        foreach (var item in _processedData)
        {
            _tableState.Selection.Select(item);
        }
        _selectAllDropdownOpen = false;
        _selectionVersion++;
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    /// <summary>
    /// Selects all items in the entire filtered dataset across all pages and closes the select-all dropdown.
    /// </summary>
    private async Task HandleSelectAllItems()
    {
        foreach (var item in _filteredData)
        {
            _tableState.Selection.Select(item);
        }
        _selectAllDropdownOpen = false;
        _selectionVersion++;
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    /// <summary>
    /// Clears all selected items and closes the select-all dropdown.
    /// </summary>
    private async Task HandleClearSelection()
    {
        _tableState.Selection.Clear();
        _selectAllDropdownOpen = false;
        _selectionVersion++;
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    /// <summary>
    /// Handles individual row checkbox state changes.
    /// Selects or deselects the item based on the checkbox state.
    /// </summary>
    /// <param name="item">The data item associated with the row.</param>
    /// <param name="isChecked">True if the checkbox was checked; false if unchecked.</param>
    private async Task HandleRowSelectionChanged(TData item, bool isChecked)
    {
        if (isChecked)
        {
            _tableState.Selection.Select(item);
        }
        else
        {
            _tableState.Selection.Deselect(item);
        }

        _selectionVersion++;  // Track selection change for ShouldRender
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    /// <summary>
    /// Determines if all items on the current page are selected.
    /// Returns false if there are no items on the page.
    /// </summary>
    /// <returns>True if all current page items are selected; otherwise, false.</returns>
    private bool IsAllSelected()
    {
        if (!_processedData.Any())
            return false;

        return _processedData.All(item => _tableState.Selection.IsSelected(item));
    }

    /// <summary>
    /// Determines if some (but not all) items on the current page are selected.
    /// Used to set the indeterminate state of the select-all checkbox.
    /// </summary>
    /// <returns>True if some but not all items are selected; otherwise, false.</returns>
    private bool IsSomeSelected()
    {
        if (!_processedData.Any())
            return false;

        var selectedCount = _processedData.Count(item => _tableState.Selection.IsSelected(item));
        return selectedCount > 0 && selectedCount < _processedData.Count();
    }

    /// <summary>
    /// Builds the inline style attribute for column width constraints.
    /// Returns null if no width constraints are specified.
    /// </summary>
    /// <param name="column">The column to get width styles for.</param>
    /// <returns>A CSS style string or null if no width is specified.</returns>
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
    /// Converts the DataTable's selection mode to the underlying table primitive's selection mode enum.
    /// </summary>
    /// <returns>The corresponding primitive selection mode.</returns>
    private SelectionMode GetPrimitiveSelectionMode()
    {
        return SelectionMode switch
        {
            DataTableSelectionMode.None => Primitives.Table.SelectionMode.None,
            DataTableSelectionMode.Single => Primitives.Table.SelectionMode.Single,
            DataTableSelectionMode.Multiple => Primitives.Table.SelectionMode.Multiple,
            _ => Primitives.Table.SelectionMode.None
        };
    }

    /// <summary>
    /// Handles page navigation events from the pagination component.
    /// Reprocesses the data to display the new page.
    /// </summary>
    /// <param name="newPage">The new page number (1-based index).</param>
    private async Task HandlePageChanged(int newPage)
    {
        _paginationVersion++;  // Track pagination change for ShouldRender
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Handles page size selection events from the pagination component.
    /// Reprocesses the data with the new page size.
    /// </summary>
    /// <param name="newPageSize">The new page size.</param>
    private async Task HandlePageSizeChanged(int newPageSize)
    {
        _paginationVersion++;  // Track pagination change for ShouldRender
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Optimizes rendering by detecting which state has changed.
    /// Prevents unnecessary re-renders when no relevant state has been modified.
    /// </summary>
    /// <returns>True if the component should re-render; otherwise, false.</returns>
    protected override bool ShouldRender()
    {
        var dataChanged = !ReferenceEquals(_lastData, Data);
        var selectionModeChanged = _lastSelectionMode != SelectionMode;
        var loadingChanged = _lastIsLoading != IsLoading;
        var columnsChanged = _lastColumnsVersion != _columnsVersion;
        var searchChanged = _lastGlobalSearchValue != _globalSearchValue;
        var selectionChanged = _lastSelectionVersion != _selectionVersion;
        var paginationChanged = _lastPaginationVersion != _paginationVersion;

        if (dataChanged || selectionModeChanged || loadingChanged || columnsChanged || searchChanged || selectionChanged || paginationChanged)
        {
            _lastData = Data;
            _lastSelectionMode = SelectionMode;
            _lastIsLoading = IsLoading;
            _lastColumnsVersion = _columnsVersion;
            _lastGlobalSearchValue = _globalSearchValue;
            _lastSelectionVersion = _selectionVersion;
            _lastPaginationVersion = _paginationVersion;
            return true;
        }

        return false;
    }
}
