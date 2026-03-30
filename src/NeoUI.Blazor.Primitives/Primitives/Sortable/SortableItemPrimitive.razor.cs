using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor.Primitives;

/// <summary>
/// A draggable item inside a <see cref="SortableContentPrimitive"/>.
/// </summary>
/// <remarks>
/// <para>
/// Renders a wrapper element with <c>data-sortable-id</c> so the JavaScript sensor can
/// identify it during pointer tracking.  By default, drag is initiated from a
/// <see cref="SortableItemHandlePrimitive"/> child.  Set <see cref="AsHandle"/> to
/// <c>true</c> to make the entire item draggable (no separate handle needed).
/// </para>
/// </remarks>
public partial class SortableItemPrimitive : ComponentBase
{
    /// <summary>
    /// Gets or sets the unique identifier of this item. Must match the value returned by
    /// <c>GetItemId</c> on the parent <see cref="SortablePrimitive{TItem}"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the entire item acts as the drag handle.
    /// When <c>false</c> (default), drag is only initiated via a <see cref="SortableItemHandlePrimitive"/>.
    /// </summary>
    [Parameter]
    public bool AsHandle { get; set; }

    /// <summary>Gets or sets the child content rendered inside the item wrapper.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional HTML attributes applied to the item element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
