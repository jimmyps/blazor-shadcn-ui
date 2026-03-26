using Microsoft.JSInterop;

namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Non-generic JS interop callback target for <see cref="SortablePrimitive{TItem}"/>.
/// Wraps generic callbacks so a single <see cref="DotNetObjectReference{T}"/> can be
/// passed to the JavaScript <c>sortable.js</c> module.
/// </summary>
internal sealed class SortableJsCallbackTarget
{
    private readonly Func<string, Task> _onDragStart;
    private readonly Func<string, string, Task> _onDragEnd;
    private readonly Func<Task> _onDragCancel;

    internal SortableJsCallbackTarget(
        Func<string, Task> onDragStart,
        Func<string, string, Task> onDragEnd,
        Func<Task> onDragCancel)
    {
        _onDragStart = onDragStart;
        _onDragEnd = onDragEnd;
        _onDragCancel = onDragCancel;
    }

    /// <summary>Called by JS when a drag operation begins.</summary>
    [JSInvokable]
    public Task OnDragStart(string activeId) => _onDragStart(activeId);

    /// <summary>Called by JS when a drag operation ends (pointer released or keyboard confirmed).</summary>
    [JSInvokable]
    public Task OnDragEnd(string activeId, string overId) => _onDragEnd(activeId, overId);

    /// <summary>Called by JS when a drag operation is cancelled (Escape key or pointer cancel).</summary>
    [JSInvokable]
    public Task OnDragCancel() => _onDragCancel();
}
