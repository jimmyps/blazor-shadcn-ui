using Microsoft.AspNetCore.Components;

namespace BlazorUI.Primitives.Services;

/// <summary>
/// Service for rendering content at the document body level, outside the normal DOM hierarchy.
/// Enables proper z-index stacking for overlays like dialogs, popovers, and dropdowns.
/// </summary>
public interface IPortalService
{
    /// <summary>
    /// Event raised when the portal registry changes (portal added, updated, or removed).
    /// </summary>
    event Action? OnPortalsChanged;

    /// <summary>
    /// Event raised when a specific portal has been rendered in the DOM.
    /// Used for synchronization between content components and PortalHost.
    /// </summary>
    event Action<string>? OnPortalRendered;

    /// <summary>
    /// Event raised when portals in a specific category change.
    /// Used by category-specific portal hosts to optimize re-rendering.
    /// </summary>
    event Action<PortalCategory>? OnPortalsCategoryChanged;

    /// <summary>
    /// Notifies that a portal has been rendered in the DOM.
    /// Called by PortalHost after rendering portal content.
    /// </summary>
    /// <param name="portalId">The ID of the portal that was rendered.</param>
    void NotifyPortalRendered(string portalId);

    /// <summary>
    /// Registers a new portal with the specified ID, category, and content.
    /// </summary>
    /// <param name="id">Unique identifier for the portal.</param>
    /// <param name="category">The category of the portal (Container or Overlay).</param>
    /// <param name="content">Content to render in the portal.</param>
    void RegisterPortal(string id, PortalCategory category, RenderFragment content);

    /// <summary>
    /// Unregisters a portal by ID, removing it from rendering.
    /// </summary>
    /// <param name="id">The portal ID to remove.</param>
    void UnregisterPortal(string id);

    /// <summary>
    /// Updates the content of an existing portal.
    /// </summary>
    /// <param name="id">The portal ID to update.</param>
    /// <param name="content">New content to render.</param>
    void UpdatePortalContent(string id, RenderFragment content);

    /// <summary>
    /// Refreshes a portal without replacing its RenderFragment.
    /// Triggers a re-render that uses updated captured values without creating new DOM elements.
    /// </summary>
    /// <param name="id">The portal ID to refresh.</param>
    void RefreshPortal(string id);

    /// <summary>
    /// Appends a child portal to a parent portal's scope.
    /// The child content will be rendered within the parent's portal, not as a separate portal.
    /// This prevents cascading re-renders and improves performance for hierarchical UI like submenus.
    /// </summary>
    /// <param name="parentPortalId">The parent portal ID to append to.</param>
    /// <param name="childPortalId">Unique identifier for the child content.</param>
    /// <param name="content">Content to append to the parent portal.</param>
    void AppendToPortal(string parentPortalId, string childPortalId, RenderFragment content);

    /// <summary>
    /// Removes a child from a parent portal's scope.
    /// </summary>
    /// <param name="parentPortalId">The parent portal ID to remove from.</param>
    /// <param name="childPortalId">The child ID to remove.</param>
    void RemoveFromPortal(string parentPortalId, string childPortalId);

    /// <summary>
    /// Gets all registered portals.
    /// </summary>
    /// <returns>Dictionary of portal IDs to their render fragments.</returns>
    IReadOnlyDictionary<string, RenderFragment> GetPortals();

    /// <summary>
    /// Gets all registered portals for a specific category.
    /// </summary>
    /// <param name="category">The portal category to filter by.</param>
    /// <returns>Dictionary of portal IDs to their render fragments for the specified category.</returns>
    IReadOnlyDictionary<string, RenderFragment> GetPortals(PortalCategory category);

    /// <summary>
    /// Gets the category of a specific portal.
    /// </summary>
    /// <param name="id">The portal ID.</param>
    /// <returns>The portal category, or null if the portal is not registered.</returns>
    PortalCategory? GetPortalCategory(string id);

    /// <summary>
    /// Gets the depth/nesting level of a portal based on registration order.
    /// Used for calculating z-index in nested dialogs.
    /// </summary>
    /// <param name="id">The portal ID.</param>
    /// <returns>The zero-based depth (0 for first portal, 1 for second, etc.), or -1 if not registered.</returns>
    int GetPortalDepth(string id);
}
