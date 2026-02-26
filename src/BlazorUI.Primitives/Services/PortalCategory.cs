namespace BlazorUI.Primitives.Services;

/// <summary>
/// Categories of portals for separate rendering hosts.
/// </summary>
public enum PortalCategory
{
    /// <summary>
    /// Container portals with backdrop overlays.
    /// Examples: Dialog, Sheet, AlertDialog, Drawer.
    /// Z-index range: 40-50.
    /// </summary>
    Container,
    
    /// <summary>
    /// Floating overlay portals using FloatingPortal infrastructure.
    /// Examples: Popover, Tooltip, HoverCard, Select, Combobox,
    ///           DropdownMenu, ContextMenu, Menubar.
    /// Z-index range: 60-70.
    /// </summary>
    Overlay
}
