using BlazorUI.Primitives.Contexts;

namespace BlazorUI.Primitives.NavigationMenu;

/// <summary>
/// State for the NavigationMenu primitive context.
/// </summary>
public class NavigationMenuState
{
    /// <summary>
    /// Gets or sets the currently active/open item value.
    /// Empty string means no item is active.
    /// </summary>
    public string ActiveValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the previously active item value for motion direction.
    /// </summary>
    public string PreviousActiveValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the orientation of the navigation menu.
    /// </summary>
    public NavigationMenuOrientation Orientation { get; set; } = NavigationMenuOrientation.Horizontal;
}

/// <summary>
/// Orientation of the navigation menu.
/// </summary>
public enum NavigationMenuOrientation
{
    /// <summary>
    /// Horizontal navigation menu.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Vertical navigation menu.
    /// </summary>
    Vertical
}

/// <summary>
/// Context for NavigationMenu primitive component and its children.
/// Manages active item state and provides IDs for ARIA attributes.
/// </summary>
public class NavigationMenuContext : PrimitiveContextWithEvents<NavigationMenuState>, IDisposable
{
    private readonly List<string> _items = new();
    private readonly object _lock = new();
    private System.Threading.Timer? _closeTimer;
    private bool _disposed;

    /// <summary>
    /// Delay in milliseconds before closing menu after mouse leave.
    /// Default is 200ms.
    /// </summary>
    public int CloseDelay { get; set; } = 200;

    /// <summary>
    /// Initializes a new instance of the NavigationMenuContext.
    /// </summary>
    public NavigationMenuContext() : base(new NavigationMenuState(), "nav-menu")
    {
    }

    /// <summary>
    /// Gets the ID for a specific navigation item trigger.
    /// </summary>
    public string GetTriggerId(string value) => GetScopedId($"trigger-{value}");

    /// <summary>
    /// Gets the ID for a specific navigation item content.
    /// </summary>
    public string GetContentId(string value) => GetScopedId($"content-{value}");

    /// <summary>
    /// Gets the currently active value.
    /// </summary>
    public string ActiveValue => State.ActiveValue;

    /// <summary>
    /// Gets the orientation of the navigation menu.
    /// </summary>
    public NavigationMenuOrientation Orientation => State.Orientation;

    /// <summary>
    /// Gets the registered items.
    /// </summary>
    public IReadOnlyList<string> Items
    {
        get
        {
            lock (_lock)
            {
                return _items.ToList().AsReadOnly();
            }
        }
    }

    /// <summary>
    /// Registers a navigation item.
    /// </summary>
    /// <param name="value">The item value.</param>
    public void RegisterItem(string value)
    {
        lock (_lock)
        {
            if (!_items.Contains(value))
            {
                _items.Add(value);
            }
        }
    }

    /// <summary>
    /// Unregisters a navigation item.
    /// </summary>
    /// <param name="value">The item value.</param>
    public void UnregisterItem(string value)
    {
        lock (_lock)
        {
            _items.Remove(value);
        }
    }

    /// <summary>
    /// Checks if the specified item is currently active.
    /// </summary>
    /// <param name="value">The item value to check.</param>
    public bool IsItemActive(string value)
    {
        return State.ActiveValue == value;
    }

    /// <summary>
    /// Sets the active item and cancels any pending close.
    /// </summary>
    /// <param name="value">The value of the item to activate.</param>
    public void SetActiveItem(string value)
    {
        CancelCloseTimer();
        
        UpdateState(state =>
        {
            // Only track previous if switching between items (not from closed state)
            if (!string.IsNullOrEmpty(state.ActiveValue))
            {
                state.PreviousActiveValue = state.ActiveValue;
            }
            state.ActiveValue = value;
        });
    }

    /// <summary>
    /// Clears the active item (closes all menus).
    /// </summary>
    public void ClearActiveItem()
    {
        CancelCloseTimer();
        
        UpdateState(state =>
        {
            // Don't update PreviousActiveValue - keep it for motion direction
            state.ActiveValue = string.Empty;
        });
    }

    /// <summary>
    /// Schedules closing the menu after the delay.
    /// Call this on mouse leave from trigger or content.
    /// The close will be cancelled if SetActiveItem is called before the delay expires.
    /// </summary>
    public void ScheduleClose()
    {
        CancelCloseTimer();

        var currentValue = State.ActiveValue;
        
        _closeTimer = new System.Threading.Timer(_ =>
        {
            if (!_disposed)
            {
                // only close if still on the same active item
                if (State.ActiveValue == currentValue)
                {
                    ClearActiveItem();

                    // clear previous active since menu is fully closed
                    State.PreviousActiveValue = string.Empty;
                }
            }
        }, null, CloseDelay, System.Threading.Timeout.Infinite);
    }

    /// <summary>
    /// Cancels any pending close operation.
    /// </summary>
    public void CancelCloseTimer()
    {
        _closeTimer?.Dispose();
        _closeTimer = null;
    }

    /// <summary>
    /// Gets the motion direction for an item based on its position relative to previous item.
    /// Returns empty string if no previous item (for initial opening animation).
    /// </summary>
    public string GetMotionDirection(string itemValue)
    {
        lock (_lock)
        {
            // No previous active item = first time opening, use default animation (not motion)
            if (string.IsNullOrEmpty(State.PreviousActiveValue))
                return "";

            // Same item re-opening, no directional motion needed
            if (State.PreviousActiveValue == itemValue)
                return "";

            var currentIndex = _items.IndexOf(itemValue);
            var previousIndex = _items.IndexOf(State.PreviousActiveValue);

            // Items not found in registered list
            if (currentIndex == -1 || previousIndex == -1)
                return "";

            return currentIndex > previousIndex ? "from-end" : "from-start";
        }
    }

    /// <summary>
    /// Navigates to the next item.
    /// </summary>
    public void NavigateNext()
    {
        string? nextItem = null;
        lock (_lock)
        {
            if (_items.Count == 0) return;

            var currentIndex = string.IsNullOrEmpty(State.ActiveValue) 
                ? -1 
                : _items.IndexOf(State.ActiveValue);
            
            var nextIndex = (currentIndex + 1) % _items.Count;
            nextItem = _items[nextIndex];
        }
        
        if (nextItem != null)
        {
            SetActiveItem(nextItem);
        }
    }

    /// <summary>
    /// Navigates to the previous item.
    /// </summary>
    public void NavigatePrevious()
    {
        string? prevItem = null;
        lock (_lock)
        {
            if (_items.Count == 0) return;

            var currentIndex = string.IsNullOrEmpty(State.ActiveValue) 
                ? 0 
                : _items.IndexOf(State.ActiveValue);
            
            var prevIndex = currentIndex <= 0 ? _items.Count - 1 : currentIndex - 1;
            prevItem = _items[prevIndex];
        }
        
        if (prevItem != null)
        {
            SetActiveItem(prevItem);
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="NavigationMenuContext"/>.
    /// </summary>
    /// <remarks>
    /// Disposes the close timer and marks the context as disposed to prevent further operations.
    /// </remarks>
    public void Dispose()
    {
        _disposed = true;
        _closeTimer?.Dispose();
    }
}

/// <summary>
/// Context for an individual navigation menu item.
/// </summary>
public class NavigationMenuItemContext
{
    /// <summary>
    /// The unique value for this item.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// The ID for the trigger element.
    /// </summary>
    public string TriggerId { get; set; } = string.Empty;

    /// <summary>
    /// The ID for the content element.
    /// </summary>
    public string ContentId { get; set; } = string.Empty;
}
