using Microsoft.AspNetCore.Components.Web.Virtualization;
using NeoUI.Blazor.Primitives;

namespace NeoUI.Blazor;

/// <summary>
/// Describes a single sort criterion applied to a <see cref="DataTable{TData}"/> column.
/// A list of these is included in both <see cref="DataTableRequest"/> and
/// <see cref="DataTableVirtualRequest"/> so that multi-column sorting can be
/// supported in the future without a breaking API change.
/// </summary>
public record SortDescriptor(
    /// <summary>The column ID to sort by.</summary>
    string ColumnId,
    /// <summary>The sort direction for this column.</summary>
    SortDirection Direction
);

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

    /// <summary>
    /// Active sort descriptors in priority order. Currently contains at most one entry;
    /// designed to accommodate multi-column sorting in the future.
    /// </summary>
    public IReadOnlyList<SortDescriptor> SortDescriptors { get; init; } = [];

    /// <summary>Global search text entered by the user, or <c>null</c> when empty.</summary>
    public string? SearchText { get; init; }
}

/// <summary>
/// Describes an offset-based data request used by <see cref="DataTable{TData}"/>
/// when <c>ItemsProvider</c> is set for virtualized infinite-scroll rendering.
/// Unlike <see cref="DataTableRequest"/>, it uses <see cref="StartIndex"/> and
/// <see cref="Count"/> rather than page/page-size, and carries a
/// <see cref="CancellationToken"/> so in-flight fetches can be cancelled when the
/// user scrolls quickly.
/// </summary>
public record DataTableVirtualRequest(
    /// <summary>Zero-based index of the first row to fetch.</summary>
    int StartIndex,
    /// <summary>Maximum number of rows to return.</summary>
    int Count,
    /// <summary>
    /// Active sort descriptors in priority order. Currently contains at most one entry;
    /// designed to accommodate multi-column sorting in the future.
    /// </summary>
    IReadOnlyList<SortDescriptor> SortDescriptors,
    /// <summary>Global search text entered by the user, or <c>null</c> when empty.</summary>
    string? SearchText,
    /// <summary>Token that is cancelled when the virtualizer no longer needs this slice.</summary>
    CancellationToken CancellationToken
);

/// <summary>
/// Delegate used by <see cref="DataTable{TData}"/> when <c>ItemsProvider</c> is set.
/// Receives a <see cref="DataTableVirtualRequest"/> with offset, count, sort, and
/// search context and must return an <see cref="ItemsProviderResult{TData}"/> containing
/// the slice and the total matching-item count.
/// </summary>
public delegate ValueTask<ItemsProviderResult<TData>>
    DataTableVirtualProvider<TData>(DataTableVirtualRequest request);

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
