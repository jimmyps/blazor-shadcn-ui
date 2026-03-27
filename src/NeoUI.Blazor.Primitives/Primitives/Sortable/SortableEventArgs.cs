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
