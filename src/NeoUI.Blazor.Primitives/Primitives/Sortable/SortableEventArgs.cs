namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Arguments passed to the <c>OnDragEnd</c> callback of <see cref="SortablePrimitive{TItem}"/>.
/// </summary>
/// <param name="ActiveId">The unique identifier of the item that was dragged.</param>
/// <param name="OverId">The identifier of the item the dragged item was dropped over.</param>
/// <param name="FromIndex">Zero-based index of the dragged item before the drag.</param>
/// <param name="ToIndex">Zero-based index of the dragged item after the drag.</param>
/// <param name="Moved">
/// <c>true</c> when the item actually changed position;
/// <c>false</c> when it was dropped back on its original slot.
/// </param>
public record SortableDragEndArgs(
    string ActiveId,
    string OverId,
    int    FromIndex,
    int    ToIndex,
    bool   Moved);

/// <summary>
/// Arguments passed to transfer callbacks when an item moves between two
/// <see cref="SortablePrimitive{TItem}"/> instances in the same group.
/// </summary>
/// <param name="ActiveId">The identifier of the item being transferred.</param>
/// <param name="OverId">The identifier of the nearest item in the target container.</param>
/// <param name="Index">
/// For <c>OnItemTransferredOut</c>: the zero-based index of the item in the source list.<br/>
/// For <c>OnItemTransferredIn</c>: the zero-based index where the item should be inserted.
/// </param>
/// <param name="SourceInstanceId">Opaque identifier of the source <see cref="SortablePrimitive{TItem}"/>.</param>
/// <param name="TargetInstanceId">Opaque identifier of the target <see cref="SortablePrimitive{TItem}"/>.</param>
public record SortableTransferArgs(
    string ActiveId,
    string OverId,
    int    Index,
    string SourceInstanceId,
    string TargetInstanceId);

/// <summary>
/// Arguments passed to the <c>OnCanDrop</c> predicate to allow or reject a cross-list transfer.
/// </summary>
/// <param name="ActiveId">The identifier of the item being dragged.</param>
/// <param name="SourceInstanceId">Opaque identifier of the source <see cref="SortablePrimitive{TItem}"/>.</param>
/// <param name="TargetInstanceId">Opaque identifier of the target <see cref="SortablePrimitive{TItem}"/>.</param>
public record SortableDragQueryArgs(
    string ActiveId,
    string SourceInstanceId,
    string TargetInstanceId);
