namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Typed context object exposed to <see cref="SortablePrimitive{TItem}"/> child content
/// via the Blazor <c>Context</c> parameter.
/// </summary>
/// <remarks>
/// Consumers can opt in by adding <c>Context="s"</c> to their <c>&lt;Sortable&gt;</c> or
/// <c>&lt;SortablePrimitive&gt;</c> tag. The scope provides pre-built helpers so consumers never
/// need to know about internal implementation details such as the <c>data-sortable-id</c>
/// attribute name used by the JavaScript layer.
/// </remarks>
/// <example>
/// Wire any list, table, or grid component without knowing the internal attribute contract:
/// <code>
/// &lt;Sortable TItem="TaskItem" Items="@_items" GetItemId="@(i => i.Id)" Context="s"&gt;
///     &lt;SortableContent&gt;
///         &lt;DataTable AdditionalRowAttributes="@s.RowAttributes" ... /&gt;
///     &lt;/SortableContent&gt;
/// &lt;/Sortable&gt;
/// </code>
/// </example>
/// <typeparam name="TItem">The item type of the parent <see cref="SortablePrimitive{TItem}"/>.</typeparam>
public sealed class SortableScope<TItem>
{
    private readonly Func<TItem, string> _getItemId;
    private readonly SortableContext    _context;

    internal SortableScope(Func<TItem, string> getItemId, SortableContext context)
    {
        _getItemId = getItemId;
        _context   = context;

        RowAttributes = item => new Dictionary<string, object>
        {
            ["data-sortable-id"] = _getItemId(item)
        };
    }

    /// <summary>
    /// A pre-built delegate that returns the HTML attributes required to make a row or item
    /// participatein drag-and-drop sorting. Pass this directly to any component that accepts
    /// a per-row/per-item attribute factory — e.g. <c>AdditionalRowAttributes</c> on
    /// <c>DataTable</c>.
    /// </summary>
    public Func<TItem, Dictionary<string, object>> RowAttributes { get; }

    /// <summary>
    /// Gets the identifier of the item currently being dragged, or <c>null</c> when idle.
    /// </summary>
    public string? ActiveId => _context.ActiveId;

    /// <summary>
    /// Gets a value indicating whether a drag operation is in progress.
    /// </summary>
    public bool IsDragging => _context.IsDragging;

    /// <summary>
    /// Returns <c>true</c> when <paramref name="item"/> is the item currently being dragged.
    /// </summary>
    public bool IsItemDragging(TItem item) =>
        _context.ActiveId is not null && _context.ActiveId == _getItemId(item);
}
