namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Defines the axis/orientation for a <see cref="SortablePrimitive{TItem}"/> drag-and-drop list.
/// </summary>
public enum SortableOrientation
{
    /// <summary>
    /// Items are arranged in a single vertical column.
    /// Items shift up/down as the dragged item moves.
    /// </summary>
    Vertical,

    /// <summary>
    /// Items are arranged in a single horizontal row.
    /// Items shift left/right as the dragged item moves.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Items are arranged in a CSS grid or wrapping flex layout (mixed axes).
    /// Each item shifts to exactly the snapshot position of the adjacent slot,
    /// so displacement correctly follows the 2D grid geometry.
    /// </summary>
    Grid,

    /// <summary>
    /// Items are arranged in a grid or wrapping flow (mixed axes).
    /// Only the drag overlay moves; items do not shift during drag.
    /// </summary>
    Mixed,
}
