namespace BlazorUI.Components.Pagination;

/// <summary>
/// Defines the size variants for Pagination link components.
/// </summary>
/// <remarks>
/// Pagination link sizes control the padding, height, and overall dimensions
/// of pagination navigation buttons.
/// </remarks>
public enum PaginationLinkSize
{
    /// <summary>
    /// Default pagination link size with text and icon support.
    /// </summary>
    /// <remarks>
    /// Standard size suitable for most pagination layouts with page numbers.
    /// </remarks>
    Default,

    /// <summary>
    /// Compact icon-only pagination link size.
    /// </summary>
    /// <remarks>
    /// Used for icon-only navigation buttons (previous/next arrows) in space-constrained layouts.
    /// </remarks>
    Icon
}
