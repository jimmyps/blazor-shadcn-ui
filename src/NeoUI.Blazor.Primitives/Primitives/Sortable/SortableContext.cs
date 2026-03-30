using Microsoft.JSInterop;

namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Shared state context for <see cref="SortablePrimitive{TItem}"/> and its sub-components.
/// Distributed via Blazor's <c>CascadingValue</c> mechanism.
/// </summary>
public sealed class SortableContext
{
    /// <summary>
    /// Gets or sets the ID of the item currently being dragged, or <c>null</c> when idle.
    /// </summary>
    public string? ActiveId { get; set; }

    /// <summary>
    /// Gets a value indicating whether a drag operation is in progress.
    /// </summary>
    public bool IsDragging => ActiveId is not null;

    /// <summary>
    /// Gets or sets the drag orientation for this sortable context.
    /// </summary>
    public SortableOrientation Orientation { get; set; } = SortableOrientation.Vertical;

    /// <summary>
    /// Gets or sets the group name for cross-list drag-and-drop.
    /// Instances with the same group name can exchange items.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Gets or sets the unique instance identifier used to scope JS interop.
    /// </summary>
    internal string InstanceId { get; set; } = string.Empty;

    /// <summary>
    /// The <c>DotNetObjectReference</c> for the JS callback target.
    /// Set by <see cref="SortablePrimitive{TItem}"/>; consumed by <see cref="SortableContentPrimitive"/>.
    /// </summary>
    internal object? DotNetRef { get; set; }

    /// <summary>
    /// The JS module reference, set by <see cref="SortableContentPrimitive"/> after init.
    /// Used by <see cref="SortablePrimitive{TItem}"/> for post-render JS calls.
    /// </summary>
    internal IJSObjectReference? JsModule { get; set; }

    /// <summary>
    /// Callback invoked by <see cref="SortablePrimitive{TItem}"/> so child components can trigger
    /// a Blazor re-render (e.g., to update the overlay content after drag start).
    /// </summary>
    internal Action? NotifyStateChanged { get; set; }
}
