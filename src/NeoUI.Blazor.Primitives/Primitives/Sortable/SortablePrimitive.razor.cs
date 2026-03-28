using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Root context component for the headless Sortable drag-and-drop primitive.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="SortablePrimitive{TItem}"/> is the root of a composable, headless drag-and-drop
/// sorting system. It manages state, wires up the JavaScript interop layer, and propagates
/// context to all child primitives via Blazor's <c>CascadingValue</c>.
/// </para>
/// <para>
/// This component is generic over <typeparamref name="TItem"/> so the caller can use any item type
/// without boxing. The drag-and-drop identifier is always a <c>string</c> produced by
/// <see cref="GetItemId"/>, keeping the JS interop layer type-free.
/// </para>
/// <para>
/// Compound component pattern:
/// <list type="bullet">
/// <item><see cref="SortablePrimitive{TItem}"/> — root / context provider</item>
/// <item><see cref="SortableContentPrimitive"/> — droppable container (initialises JS)</item>
/// <item><see cref="SortableItemPrimitive"/> — draggable item</item>
/// <item><see cref="SortableItemHandlePrimitive"/> — optional grip handle</item>
/// <item><see cref="SortableOverlayPrimitive"/> — drag ghost / portal overlay</item>
/// </list>
/// </para>
/// <para>
/// All pointer/pixel-level drag work runs inside <c>sortable.js</c> for 60 fps performance.
/// JS calls back into C# via <c>[JSInvokable]</c> only on drag-start and drag-end.
/// Blazor/C# handles array reordering and accessibility markup.
/// </para>
/// </remarks>
/// <example>
/// Vertical list:
/// <code>
/// &lt;SortablePrimitive TItem="MyItem" Items="@items" OnItemsReordered="@(r => items = r)"
///                    GetItemId="@(i => i.Id)"&gt;
///     &lt;SortableContentPrimitive class="flex flex-col gap-2"&gt;
///         @foreach (var item in items)
///         {
///             &lt;SortableItemPrimitive Value="@item.Id"&gt;
///                 &lt;SortableItemHandlePrimitive /&gt;
///                 @item.Name
///             &lt;/SortableItemPrimitive&gt;
///         }
///     &lt;/SortableContentPrimitive&gt;
///     &lt;SortableOverlayPrimitive /&gt;
/// &lt;/SortablePrimitive&gt;
/// </code>
/// </example>
/// <typeparam name="TItem">The type of item in the sortable list.</typeparam>
public partial class SortablePrimitive<TItem> : ComponentBase, IAsyncDisposable
{
    private readonly SortableContext _context = new();
    private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];
    private DotNetObjectReference<SortableJsCallbackTarget>? _dotNetRef;
    private SortableScope<TItem> _scope = default!;
    private bool _disposed;

    // ── Parameters ────────────────────────────────────────────────────

    /// <summary>
    /// Gets or sets the collection of items to sort.
    /// </summary>
    [Parameter, EditorRequired]
    public IList<TItem> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the callback invoked after a drag operation reorders items.
    /// The argument is the new ordered list.
    /// </summary>
    [Parameter]
    public EventCallback<IList<TItem>> OnItemsReordered { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a drag operation starts.
    /// The argument is the dragged item's identifier.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnDragStart { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a drag operation ends (whether or not the item moved).
    /// </summary>
    [Parameter]
    public EventCallback<SortableDragEndArgs> OnDragEnd { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when a drag operation is cancelled (e.g. Escape key).
    /// </summary>
    [Parameter]
    public EventCallback OnDragCancel { get; set; }

    /// <summary>
    /// Gets or sets a function that extracts a unique string identifier from an item.
    /// </summary>
    [Parameter, EditorRequired]
    public Func<TItem, string> GetItemId { get; set; } = _ => string.Empty;

    /// <summary>
    /// Gets or sets the drag orientation. Defaults to <see cref="SortableOrientation.Vertical"/>.
    /// </summary>
    [Parameter]
    public SortableOrientation Orientation { get; set; } = SortableOrientation.Vertical;

    /// <summary>
    /// Gets or sets the child content rendered inside this component.
    /// When a <c>Context</c> parameter is supplied the child fragment receives a
    /// <see cref="SortableScope{TItem}"/> with helpers such as <see cref="SortableScope{TItem}.RowAttributes"/>.
    /// </summary>
    [Parameter]
    public RenderFragment<SortableScope<TItem>>? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional HTML attributes applied to the root wrapper element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    // ── Lifecycle ────────────────────────────────────────────────────

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        var target = new SortableJsCallbackTarget(
            onDragStart: HandleDragStartAsync,
            onDragEnd: HandleDragEndAsync,
            onDragCancel: HandleDragCancelAsync);

        _dotNetRef = DotNetObjectReference.Create(target);

        _context.InstanceId = _instanceId;
        _context.Orientation = Orientation;
        _context.DotNetRef = _dotNetRef;
        _context.NotifyStateChanged = () => InvokeAsync(StateHasChanged);

        _scope = new SortableScope<TItem>(GetItemId, _context);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        _context.Orientation = Orientation;
        _scope = new SortableScope<TItem>(GetItemId, _context);
    }

    // ── JS Callbacks ─────────────────────────────────────────────────

    /// <summary>Invoked by JS when a drag operation starts.</summary>
    private Task HandleDragStartAsync(string activeId)
    {
        _context.ActiveId = activeId;
        return InvokeAsync(async () =>
        {
            if (OnDragStart.HasDelegate)
                await OnDragStart.InvokeAsync(activeId);
            StateHasChanged();
        });
    }

    /// <summary>Invoked by JS when a drag operation ends with a valid drop target.</summary>
    private async Task HandleDragEndAsync(string activeId, string overId)
    {
        _context.ActiveId = null;

        var fromIndex = Items.ToList().FindIndex(i => GetItemId(i) == activeId);
        var toIndex   = Items.ToList().FindIndex(i => GetItemId(i) == overId);
        var moved     = activeId != overId && fromIndex >= 0 && toIndex >= 0 && fromIndex != toIndex;

        await InvokeAsync(async () =>
        {
            if (moved)
            {
                var reordered = Reorder(Items, activeId, overId);
                if (OnItemsReordered.HasDelegate)
                    await OnItemsReordered.InvokeAsync(reordered);
            }

            if (OnDragEnd.HasDelegate)
                await OnDragEnd.InvokeAsync(new SortableDragEndArgs(activeId, overId, fromIndex, toIndex, moved));

            StateHasChanged();
        });
    }

    /// <summary>Invoked by JS when a drag operation is cancelled.</summary>
    private Task HandleDragCancelAsync()
    {
        _context.ActiveId = null;
        return InvokeAsync(async () =>
        {
            if (OnDragCancel.HasDelegate)
                await OnDragCancel.InvokeAsync();
            StateHasChanged();
        });
    }

    // ── Helpers ───────────────────────────────────────────────────────

    private IList<TItem> Reorder(IList<TItem> source, string activeId, string overId)
    {
        var list = source.ToList();
        var fromIndex = list.FindIndex(i => GetItemId(i) == activeId);
        var toIndex   = list.FindIndex(i => GetItemId(i) == overId);

        if (fromIndex < 0 || toIndex < 0 || fromIndex == toIndex)
            return list;

        var item = list[fromIndex];
        list.RemoveAt(fromIndex);
        list.Insert(toIndex, item);
        return list;
    }

    // ── Disposal ─────────────────────────────────────────────────────

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;
        _disposed = true;
        _dotNetRef?.Dispose();
        return ValueTask.CompletedTask;
    }
}
