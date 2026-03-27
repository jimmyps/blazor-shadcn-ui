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
    /// <summary>
    /// Gets or sets the accessible label for the handle button.
    /// When <c>null</c> (the default), no <c>aria-label</c> attribute is emitted.
    /// </summary>
    /// <remarks>
    /// Provide a localized string via this parameter. When using the styled
    /// <c>SortableItemHandle</c> wrapper from <c>NeoUI.Blazor</c>, this is automatically
    /// populated from the DI <c>ILocalizer</c> (<c>Sortable.DragHandle</c> key).
    /// </remarks>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>Gets or sets optional custom content for the handle (replaces the default grip icon).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional HTML attributes applied to the handle element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
