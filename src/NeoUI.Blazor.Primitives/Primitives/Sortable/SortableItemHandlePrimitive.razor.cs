using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor.Primitives;

/// <summary>
/// An optional grip handle for a <see cref="SortableItemPrimitive"/>.
/// </summary>
/// <remarks>
/// <para>
/// When this component is rendered inside a <see cref="SortableItemPrimitive"/> that does
/// <em>not</em> have <c>AsHandle="true"</c>, drag operations are constrained to start only
/// from this element.  The JavaScript sensor detects <c>data-sortable-handle</c> on the event
/// target and activates dragging accordingly.
/// </para>
/// <para>
/// A default grip icon (six-dot grid) is rendered when no <see cref="ChildContent"/> is provided.
/// </para>
/// </remarks>
public partial class SortableItemHandlePrimitive : ComponentBase
{
    /// <summary>Gets or sets optional custom content for the handle (replaces the default grip icon).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional HTML attributes applied to the handle element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
