namespace NeoUI.Blazor.Primitives;

/// <summary>Determines who handles transient floating UI interactions.</summary>
public enum InteractionMode
{
    /// <summary>Blazor handles interactions using the current renderer.</summary>
    Blazor,
    /// <summary>The browser handles opening, dismissal, positioning and navigation locally.</summary>
    Browser
}
