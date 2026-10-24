namespace NeoUI.Blazor.Primitives.Services;

// Shared only inside a category host's rendered descendants. Prevent refresh
// feedback while that host is already delivering their current parameters.
internal sealed class PortalRenderContext(PortalCategory category)
{
    internal PortalCategory Category { get; set; } = category;
    internal bool IsRendering { get; set; }
}
