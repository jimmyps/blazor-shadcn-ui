namespace BlazorUI.Components.Chart;

/// <summary>
/// Animation types for different chart behaviors.
/// </summary>
/// <remarks>
/// Defines the style of animation used when a chart first renders or updates.
/// Different chart types may interpret these animations differently.
/// </remarks>
public enum AnimationType
{
    /// <summary>Default animation for the chart type</summary>
    Default,
    
    /// <summary>Fade in animation</summary>
    FadeIn,
    
    /// <summary>Scale in animation (grow from center)</summary>
    ScaleIn,
    
    /// <summary>Slide in from left animation</summary>
    SlideInLeft,
    
    /// <summary>Slide in from right animation</summary>
    SlideInRight,
    
    /// <summary>Slide in from top animation</summary>
    SlideInTop,
    
    /// <summary>Slide in from bottom animation</summary>
    SlideInBottom,
    
    /// <summary>Wave animation (sequential reveal)</summary>
    Wave,
    
    /// <summary>Expand animation (bars grow from baseline)</summary>
    Expand,
    
    /// <summary>Draw animation (lines/paths draw progressively)</summary>
    Draw
}
