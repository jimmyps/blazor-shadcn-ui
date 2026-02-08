using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Primitives.Services;

/// <summary>
/// Implementation of portal rendering service for Blazor.
/// Manages a registry of portals that can be rendered at document body level.
/// Maintains insertion order to ensure proper rendering sequence (e.g., parent before child).
/// </summary>
public class PortalService : IPortalService
{
    /// <summary>
    /// Wraps a portal's RenderFragment with its insertion order and category for stable sorting.
    /// </summary>
    private record PortalEntry(long Order, PortalCategory Category, RenderFragment Content);

    private readonly ConcurrentDictionary<string, PortalEntry> _portals = new();
    private long _nextOrder = 0;

    /// <inheritdoc />
    public event Action? OnPortalsChanged;

    /// <inheritdoc />
    public event Action<string>? OnPortalRendered;

    /// <inheritdoc />
    public event Action<PortalCategory>? OnPortalsCategoryChanged;

    /// <inheritdoc />
    public void NotifyPortalRendered(string portalId)
    {
        OnPortalRendered?.Invoke(portalId);
    }

    /// <inheritdoc />
    public void RegisterPortal(string id, PortalCategory category, RenderFragment content)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Portal ID cannot be null or whitespace.", nameof(id));
        }

        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        // Preserve order for existing portals, assign new order for new portals
        _portals.AddOrUpdate(
            id,
            _ => new PortalEntry(Interlocked.Increment(ref _nextOrder), category, content),
            (_, existing) => existing with { Content = content, Category = category });
        
        OnPortalsChanged?.Invoke();
        OnPortalsCategoryChanged?.Invoke(category);
    }

    /// <inheritdoc />
    public void UnregisterPortal(string id)
    {
        if (_portals.TryRemove(id, out var entry))
        {
            OnPortalsChanged?.Invoke();
            OnPortalsCategoryChanged?.Invoke(entry.Category);
        }
    }

    /// <inheritdoc />
    public void UpdatePortalContent(string id, RenderFragment content)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (!_portals.TryGetValue(id, out var existing))
        {
            throw new InvalidOperationException($"Portal with ID '{id}' is not registered.");
        }

        // Update content while preserving insertion order and category
        _portals[id] = existing with { Content = content };

        OnPortalsChanged?.Invoke();
        OnPortalsCategoryChanged?.Invoke(existing.Category);
    }

    /// <inheritdoc />
    public void RefreshPortal(string id)
    {
        if (_portals.TryGetValue(id, out var entry))
        {
            // Notify PortalHost to re-render WITHOUT replacing the RenderFragment
            // This allows the existing fragment to pick up new captured values
            // without creating new DOM elements (which would break ElementReference)
            OnPortalsChanged?.Invoke();
            OnPortalsCategoryChanged?.Invoke(entry.Category);
        }
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, RenderFragment> GetPortals()
    {
        // Return portals sorted by insertion order
        return _portals
            .OrderBy(kvp => kvp.Value.Order)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Content);
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, RenderFragment> GetPortals(PortalCategory category)
    {
        // Return portals for specific category, sorted by insertion order
        return _portals
            .Where(kvp => kvp.Value.Category == category)
            .OrderBy(kvp => kvp.Value.Order)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Content);
    }

    /// <inheritdoc />
    public PortalCategory? GetPortalCategory(string id)
    {
        if (_portals.TryGetValue(id, out var entry))
        {
            return entry.Category;
        }
        return null;
    }
}
