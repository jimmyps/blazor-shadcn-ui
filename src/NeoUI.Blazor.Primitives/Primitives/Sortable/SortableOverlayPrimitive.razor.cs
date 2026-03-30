using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor.Primitives;

/// <summary>
/// Drag ghost / overlay for <see cref="SortablePrimitive{TItem}"/>.
/// </summary>
/// <remarks>
/// <para>
/// Renders a fixed-position overlay element that the JavaScript sensor moves to follow the
/// pointer during drag.  When no <see cref="ChildContent"/> is provided the sensor clones the
/// dragged item's element into the overlay automatically.  When <see cref="ChildContent"/> is
/// provided it receives the active item's ID as context, allowing fully custom ghost markup.
/// </para>
/// <para>
/// Place this component anywhere inside <see cref="SortablePrimitive{TItem}"/>, typically as
/// the last child so it renders above the list in the document flow.
/// </para>
/// </remarks>
public partial class SortableOverlayPrimitive : ComponentBase
{
    /// <summary>Gets the cascading sortable context.</summary>
    [CascadingParameter]
    public SortableContext? Context { get; set; }

    /// <summary>
    /// Gets or sets optional custom render fragment for the ghost content.
    /// The context parameter is the <c>activeId</c> of the item being dragged.
    /// When <c>null</c> (default) the sensor renders a clone of the source element.
    /// </summary>
    [Parameter]
    public RenderFragment<string>? ChildContent { get; set; }

    /// <summary>Gets or sets additional HTML attributes applied to the overlay element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
