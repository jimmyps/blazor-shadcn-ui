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
    private readonly Func<string, string, string, string, Task<bool>> _onTransferIn;
    private readonly Func<string, string, string, string, Task> _onTransferOut;

    internal SortableJsCallbackTarget(
        Func<string, Task> onDragStart,
        Func<string, string, Task> onDragEnd,
        Func<Task> onDragCancel,
        Func<string, string, string, string, Task<bool>> onTransferIn,
        Func<string, string, string, string, Task> onTransferOut)
    {
        _onDragStart   = onDragStart;
        _onDragEnd     = onDragEnd;
        _onDragCancel  = onDragCancel;
        _onTransferIn  = onTransferIn;
        _onTransferOut = onTransferOut;
    }

    /// <summary>Called by JS when a drag operation begins.</summary>
    [JSInvokable]
    public Task OnDragStart(string activeId) => _onDragStart(activeId);

    /// <summary>Called by JS when a same-list drag operation ends.</summary>
    [JSInvokable]
    public Task OnDragEnd(string activeId, string overId) => _onDragEnd(activeId, overId);

    /// <summary>Called by JS when a drag operation is cancelled (Escape key or pointer cancel).</summary>
    [JSInvokable]
    public Task OnDragCancel() => _onDragCancel();

    /// <summary>
    /// Called by JS on the TARGET instance during a cross-list drop.
    /// Returns <c>true</c> if the transfer is accepted; <c>false</c> to reject.
    /// </summary>
    [JSInvokable]
    public Task<bool> OnTransferIn(string activeId, string overId, string sourceInstanceId, string targetInstanceId)
        => _onTransferIn(activeId, overId, sourceInstanceId, targetInstanceId);

    /// <summary>
    /// Called by JS on the SOURCE instance after the target has accepted the transfer.
    /// </summary>
    [JSInvokable]
    public Task OnTransferOut(string activeId, string overId, string sourceInstanceId, string targetInstanceId)
        => _onTransferOut(activeId, overId, sourceInstanceId, targetInstanceId);
}
