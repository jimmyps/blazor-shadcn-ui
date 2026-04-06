namespace NeoUI.Blazor;

/// <summary>
/// Defines the position of dot indicators in a <see cref="Carousel"/> component.
/// </summary>
public enum CarouselDotsPosition
{
    /// <summary>
    /// Position is determined automatically based on orientation:
    /// Bottom for horizontal carousels, Right for vertical carousels.
    /// </summary>
    Auto,

    /// <summary>Dots are placed at the bottom center.</summary>
    Bottom,

    /// <summary>Dots are placed at the top center.</summary>
    Top,

    /// <summary>Dots are placed at the left center (stacked vertically).</summary>
    Left,

    /// <summary>Dots are placed at the right center (stacked vertically).</summary>
    Right
}
