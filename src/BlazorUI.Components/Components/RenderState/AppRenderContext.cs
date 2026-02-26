namespace BlazorUI.Components.RenderState;

/// <summary>
/// Provides runtime information about the app's render lifecycle state.
/// Tracks interactive state, hydration status, and navigation mode.
/// </summary>
public class AppRenderContext
{
    /// <summary>
    /// Indicates whether the app is interactive (hydration finished, JS + DOM are safe).
    /// False during SSR or prerender only.
    /// </summary>
    public bool IsInteractive { get; private set; }

    /// <summary>
    /// Indicates whether the app is in the first interactive frame (hydration phase).
    /// Use this to avoid animations or DOM measurements during hydration.
    /// </summary>
    public bool IsHydrating { get; private set; } = true;

    /// <summary>
    /// Indicates whether the current navigation is enhanced (client-side SPA navigation)
    /// versus a full reload. This value is false until an enhanced navigation occurs.
    /// </summary>
    public bool IsEnhancedNavigation { get; private set; }

    /// <summary>
    /// Event raised when any of the state properties change.
    /// Subscribe to this to react to lifecycle transitions.
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Marks the app as interactive. This is called once when the first interactive render occurs.
    /// </summary>
    internal void MarkInteractive()
    {
        if (IsInteractive) return;
        IsInteractive = true;
        Changed?.Invoke();
    }

    /// <summary>
    /// Marks hydration as complete. This is called after the DOM has settled post-hydration.
    /// </summary>
    internal void MarkHydrationComplete()
    {
        if (!IsHydrating) return;
        IsHydrating = false;
        Changed?.Invoke();
    }

    /// <summary>
    /// Sets whether the current navigation is enhanced (SPA-style).
    /// </summary>
    internal void SetEnhancedNavigation(bool value)
    {
        if (IsEnhancedNavigation == value) return;
        IsEnhancedNavigation = value;
        Changed?.Invoke();
    }
}
