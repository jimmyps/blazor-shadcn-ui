namespace NeoUI.Blazor;

/// <summary>
/// Specifies the direction of a screen transition animation.
/// </summary>
public enum ScreenTransitionDirection
{
    /// <summary>No animation — content appears instantly.</summary>
    None,

    /// <summary>Cross-fade. Use when switching between sibling tabs.</summary>
    Tab,

    /// <summary>Slide in from the right. Use for push/drill-down navigation.</summary>
    Push,

    /// <summary>Slide in from the left. Use for back/pop navigation.</summary>
    Pop
}
