using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Primitives.Services;

/// <summary>
/// Implementation of portal rendering service for Blazor.
/// Manages a registry of portals that can be rendered at document body level.
/// </summary>
public class PortalService : IPortalService, IDisposable
{
    private readonly ConcurrentDictionary<string, RenderFragment> _portals = new();

    // Debounce support so multiple portal changes in quick succession
    // only trigger a single OnPortalsChanged notification.
    private readonly object _debounceLock = new();
    private Timer? _debounceTimer;
    private bool _pendingNotification;

    // Small debounce window (ms) for batching; tweak if needed.
    private const int DebounceDelayMs = 100;

    /// <inheritdoc />
    public event Action? OnPortalsChanged;

    /// <inheritdoc />
    public void RegisterPortal(string id, RenderFragment content)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Portal ID cannot be null or whitespace.", nameof(id));
        }

        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        _portals[id] = content;
        SchedulePortalsChanged();
    }

    /// <inheritdoc />
    public void UnregisterPortal(string id)
    {
        if (_portals.TryRemove(id, out _))
        {
            SchedulePortalsChanged();
        }
    }

    /// <inheritdoc />
    public void UpdatePortalContent(string id, RenderFragment content)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (!_portals.TryUpdate(id, content, _portals.GetValueOrDefault(id)!))
        {
            throw new InvalidOperationException($"Portal with ID '{id}' is not registered.");
        }

        SchedulePortalsChanged();
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, RenderFragment> GetPortals()
    {
        return _portals;
    }

    private void SchedulePortalsChanged()
    {
        lock (_debounceLock)
        {
            _pendingNotification = true;

            if (_debounceTimer == null)
            {
                _debounceTimer = new Timer(_ =>
                {
                    bool shouldRaise;
                    lock (_debounceLock)
                    {
                        shouldRaise = _pendingNotification;
                        _pendingNotification = false;
                    }

                    if (shouldRaise)
                    {
                        OnPortalsChanged?.Invoke();
                    }

                }, null, Timeout.Infinite, Timeout.Infinite);
            }

            // Restart timer for the debounce window
            _debounceTimer.Change(DebounceDelayMs, Timeout.Infinite);
        }
    }

    public void Dispose()
    {
        _debounceTimer?.Dispose();
    }
}
