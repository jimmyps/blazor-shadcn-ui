using NeoUI.Blazor.Primitives;

namespace NeoUI.Blazor;

/// <summary>
/// Describes a server-side data request for <see cref="DataTable{TData}"/> when
/// <c>ServerData</c> is set. Contains all context needed for the server to return
/// the correct page of sorted, filtered data.
/// </summary>
public record DataTableRequest
{
    /// <summary>Current 1-based page number.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Number of rows per page.</summary>
    public int PageSize { get; init; } = 10;

    /// <summary>Column ID to sort by, or <c>null</c> when no sort is active.</summary>
    public string? SortColumn { get; init; }

    /// <summary>Sort direction. <see cref="SortDirection.None"/> when unsorted.</summary>
    public SortDirection SortDirection { get; init; } = SortDirection.None;

    /// <summary>Global search text entered by the user, or <c>null</c> when empty.</summary>
    public string? SearchText { get; init; }
}

/// <summary>
/// The response returned by the <c>ServerData</c> callback on <see cref="DataTable{TData}"/>.
/// </summary>
/// <typeparam name="TData">The type of data items in the table.</typeparam>
public record DataTableResult<TData>
{
    /// <summary>The page of items to render.</summary>
    public IEnumerable<TData> Items { get; init; } = [];

    /// <summary>
    /// Total number of items matching the current filter (across all pages).
    /// Used to drive the pagination footer.
    /// </summary>
    public int TotalCount { get; init; }
}
