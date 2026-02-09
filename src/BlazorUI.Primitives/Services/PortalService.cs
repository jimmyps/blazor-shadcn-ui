using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Primitives.Services;

/// <summary>
/// Implementation of portal rendering service for Blazor.
/// Manages a registry of portals that can be rendered at document body level.
/// Maintains insertion order to ensure proper rendering sequence (e.g., parent before child).
/// Supports hierarchical portals where children can be appended to parent scopes.
/// </summary>
public class PortalService : IPortalService
{
    /// <summary>
    /// Wraps a portal's RenderFragment with its insertion order and category for stable sorting.
    /// </summary>
    private record PortalEntry(long Order, PortalCategory Category, RenderFragment Content);

    /// <summary>
    /// Represents a portal scope that can contain child portals.
    /// Used for hierarchical UI like dropdown submenus.
    /// </summary>
    private class PortalScope
    {
        public PortalEntry Entry { get; set; } = null!;
        public List<(string ChildId, RenderFragment Content)> Children { get; } = new();
    }

    private readonly ConcurrentDictionary<string, PortalEntry> _portals = new();
    private readonly ConcurrentDictionary<string, PortalScope> _portalScopes = new();
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
        var entry = _portals.AddOrUpdate(
            id,
            _ => new PortalEntry(Interlocked.Increment(ref _nextOrder), category, content),
            (_, existing) => existing with { Content = content, Category = category });

        // Initialize scope if it doesn't exist (in case children were added before parent)
        _portalScopes.GetOrAdd(id, _ => new PortalScope { Entry = entry });
        
        OnPortalsChanged?.Invoke();
        OnPortalsCategoryChanged?.Invoke(category);
    }

    /// <inheritdoc />
    public void UnregisterPortal(string id)
    {
        if (_portals.TryRemove(id, out var entry))
        {
            // Remove scope if it exists
            _portalScopes.TryRemove(id, out _);
            
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
    public void AppendToPortal(string parentPortalId, string childPortalId, RenderFragment content)
    {
        if (string.IsNullOrWhiteSpace(parentPortalId))
        {
            throw new ArgumentException("Parent portal ID cannot be null or whitespace.", nameof(parentPortalId));
        }

        if (string.IsNullOrWhiteSpace(childPortalId))
        {
            throw new ArgumentException("Child portal ID cannot be null or whitespace.", nameof(childPortalId));
        }

        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        // Get or create the parent scope
        var scope = _portalScopes.GetOrAdd(parentPortalId, id =>
        {
            if (!_portals.TryGetValue(id, out var entry))
            {
                throw new InvalidOperationException($"Parent portal '{id}' is not registered.");
            }
            return new PortalScope { Entry = entry };
        });

        // Add or update child in the scope
        lock (scope.Children)
        {
            var existingIndex = scope.Children.FindIndex(c => c.ChildId == childPortalId);
            if (existingIndex >= 0)
            {
                scope.Children[existingIndex] = (childPortalId, content);
            }
            else
            {
                scope.Children.Add((childPortalId, content));
            }
        }

        // Update the parent portal with composite fragment
        UpdatePortalWithComposite(parentPortalId, scope);
    }

    /// <inheritdoc />
    public void RemoveFromPortal(string parentPortalId, string childPortalId)
    {
        if (_portalScopes.TryGetValue(parentPortalId, out var scope))
        {
            lock (scope.Children)
            {
                var removed = scope.Children.RemoveAll(c => c.ChildId == childPortalId) > 0;
                if (removed)
                {
                    UpdatePortalWithComposite(parentPortalId, scope);
                }
            }
        }
    }

    /// <summary>
    /// Updates a parent portal's content to include all its children.
    /// </summary>
    private void UpdatePortalWithComposite(string parentPortalId, PortalScope scope)
    {
        var compositeFragment = CreateCompositeFragment(scope);
        _portals[parentPortalId] = scope.Entry with { Content = compositeFragment };
        
        OnPortalsChanged?.Invoke();
        OnPortalsCategoryChanged?.Invoke(scope.Entry.Category);
    }

    /// <summary>
    /// Creates a composite RenderFragment that includes parent and all children.
    /// </summary>
    private RenderFragment CreateCompositeFragment(PortalScope scope) => builder =>
    {
        // Render parent content first
        scope.Entry.Content(builder);

        // Then append all children in order
        lock (scope.Children)
        {
            foreach (var (childId, content) in scope.Children)
            {
                content(builder);
            }
        }
    };

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
