namespace BlazorUI.Primitives.Table;

/// <summary>
/// Container for all table state including sorting, pagination, and selection.
/// This class serves as the single source of truth for table configuration.
/// </summary>
/// <typeparam name="TData">The type of data items in the table.</typeparam>
public class TableState<TData> where TData : class
{
    /// <summary>
    /// Gets the sorting state for the table.
    /// </summary>
    public SortingState Sorting { get; } = new();

    /// <summary>
    /// Gets the pagination state for the table.
    /// </summary>
    public PaginationState Pagination { get; } = new();

    /// <summary>
    /// Gets the selection state for the table.
    /// </summary>
    public SelectionState<TData> Selection { get; } = new();

    /// <summary>
<<<<<<< HEAD
    /// Gets the filtering state for the table.
    /// </summary>
    public FilteringState Filtering { get; } = new();

    /// <summary>
=======
>>>>>>> pr-89
    /// Gets whether the table has any active sorting.
    /// </summary>
    public bool HasSorting => Sorting.Direction != SortDirection.None;

    /// <summary>
<<<<<<< HEAD
    /// Gets whether the table has any active filters.
    /// </summary>
    public bool HasFiltering => Filtering.HasAnyFilter;

    /// <summary>
=======
>>>>>>> pr-89
    /// Gets whether the table has any selected items.
    /// </summary>
    public bool HasSelection => Selection.HasSelection;

    /// <summary>
    /// Gets the total number of selected items.
    /// </summary>
    public int TotalSelected => Selection.SelectedCount;

    /// <summary>
    /// Gets whether pagination is enabled and active.
    /// </summary>
    public bool HasPagination => Pagination.TotalPages > 1;

    /// <summary>
    /// Resets all state to default values.
<<<<<<< HEAD
    /// Clears sorting, filtering, resets to first page, and clears selection.
=======
    /// Clears sorting, resets to first page, and clears selection.
>>>>>>> pr-89
    /// </summary>
    public void Reset()
    {
        Sorting.ClearSort();
<<<<<<< HEAD
        Filtering.ClearAllFilters();
=======
>>>>>>> pr-89
        Pagination.Reset();
        Selection.Clear();
    }

    /// <summary>
<<<<<<< HEAD
    /// Resets only the pagination state while preserving sorting, filtering, and selection.
    /// Useful when data or filters change.
=======
    /// Resets only the pagination state while preserving sorting and selection.
    /// Useful when data changes.
>>>>>>> pr-89
    /// </summary>
    public void ResetPagination()
    {
        Pagination.Reset();
    }
}
