using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Primitives.Services;

/// <summary>
/// Service for rendering content at the document body level, outside the normal DOM hierarchy.
/// Enables proper z-index stacking for overlays like dialogs, popovers, and dropdowns.
/// Portals are split into categories (Container vs Overlay) so that each category's host
/// only re-renders when its own portals change.
/// </summary>
public interface IPortalService
{
    /// <summary>
    /// Gets whether a portal host has been registered with this service.
    /// </summary>
    public bool HasHost { get; }

    /// <summary>
    /// Registers a portal host instance with the service.
    /// Called by portal host components during initialization.
    /// </summary>
    public void RegisterHost();

    /// <summary>
    /// Unregisters a portal host instance from the service.
    /// Called by portal host components during disposal.
    /// </summary>
    public void UnregisterHost();

    /// <summary>
    /// Event raised when portals in a specific category change (added, updated, or removed).
    /// Category-scoped hosts subscribe to this to avoid re-rendering when unrelated categories change.
    /// </summary>
    public event Action<PortalCategory>? OnPortalsCategoryChanged;

    /// <summary>
    /// Event raised when a specific portal has been rendered in the DOM.
    /// Used for synchronization between content components and portal hosts.
    /// </summary>
    public event Action<string>? OnPortalRendered;

    /// <summary>
    /// Notifies that a portal has been rendered in the DOM.
    /// Called by portal hosts after rendering portal content.
    /// </summary>
    /// <param name="portalId">The ID of the portal that was rendered.</param>
    public void NotifyPortalRendered(string portalId);

    /// <summary>
    /// Registers a new portal with the specified ID, content, and category.
    /// </summary>
    /// <param name="id">Unique identifier for the portal.</param>
    /// <param name="content">Content to render in the portal.</param>
    /// <param name="category">The portal category (Container or Overlay).</param>
    public void RegisterPortal(string id, RenderFragment content, PortalCategory category);

    /// <summary>
    /// Unregisters a portal by ID, removing it from rendering.
    /// </summary>
    /// <param name="id">The portal ID to remove.</param>
    public void UnregisterPortal(string id);

    /// <summary>
    /// Updates the content of an existing portal.
    /// </summary>
    /// <param name="id">The portal ID to update.</param>
    /// <param name="content">New content to render.</param>
    public void UpdatePortalContent(string id, RenderFragment content);

    /// <summary>
    /// Refreshes a portal without replacing its RenderFragment.
    /// Triggers a re-render that uses updated captured values without creating new DOM elements.
    /// </summary>
    /// <param name="id">The portal ID to refresh.</param>
    public void RefreshPortal(string id);

    /// <summary>
    /// Gets all registered portals for a specific category, ordered by insertion order.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>Ordered list of portal ID/content pairs.</returns>
    public IReadOnlyList<KeyValuePair<string, RenderFragment>> GetPortals(PortalCategory category);
}
