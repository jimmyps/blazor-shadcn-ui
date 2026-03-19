namespace NeoUI.Blazor;

/// <summary>
/// Context object passed to the <see cref="DataTable{TData}.RowContextMenu"/> render fragment
/// when a row is right-clicked.
/// </summary>
/// <typeparam name="TData">The data type of the table rows.</typeparam>
/// <param name="Item">The data item for the right-clicked row.</param>
/// <param name="SelectedItems">The currently selected items at the time of the right-click.</param>
/// <param name="VisibleColumns">The IDs of all currently visible columns, in display order.</param>
public record DataTableRowMenuContext<TData>(
    TData Item,
    IReadOnlyList<TData> SelectedItems,
    IReadOnlyList<string> VisibleColumns);
