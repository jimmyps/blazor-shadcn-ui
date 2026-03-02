namespace BlazorUI.Components.Drawer;

/// <summary>
/// Defines the direction from which a Drawer component slides into view.
/// </summary>
/// <remarks>
/// The drawer direction determines the edge of the screen from which the drawer
/// will slide in and the animation direction.
/// </remarks>
public enum DrawerDirection
{
    /// <summary>
    /// Drawer slides in from the top edge of the screen.
    /// </summary>
    /// <remarks>
    /// Suitable for notifications, alerts, or top-level actions.
    /// </remarks>
    Top,

    /// <summary>
    /// Drawer slides in from the bottom edge of the screen.
    /// </summary>
    /// <remarks>
    /// Common on mobile interfaces for action sheets and forms.
    /// </remarks>
    Bottom,

    /// <summary>
    /// Drawer slides in from the left edge of the screen.
    /// </summary>
    /// <remarks>
    /// Typical for navigation menus in LTR layouts.
    /// </remarks>
    Left,

    /// <summary>
    /// Drawer slides in from the right edge of the screen.
    /// </summary>
    /// <remarks>
    /// Often used for settings panels, filters, or secondary navigation.
    /// </remarks>
    Right
}
