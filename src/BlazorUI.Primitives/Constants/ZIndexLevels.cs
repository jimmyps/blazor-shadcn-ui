namespace BlazorUI.Primitives.Constants;

/// <summary>
/// Defines the z-index hierarchy for portal-based components.
/// Ensures proper stacking of nested floating elements (e.g., Combobox inside Dialog).
/// </summary>
public static class ZIndexLevels
{
    /// <summary>
    /// Dialog overlay/backdrop - darkens the background.
    /// </summary>
    public const int DialogOverlay = 50;

    /// <summary>
    /// Dialog content - the main dialog box.
    /// </summary>
    public const int DialogContent = 55;

    /// <summary>
    /// Popover, Select, DropdownMenu, ContextMenu content - dropdowns and menus.
    /// Should appear above dialogs when nested.
    /// </summary>
    public const int PopoverContent = 60;

    /// <summary>
    /// Tooltip content - always on top.
    /// </summary>
    public const int TooltipContent = 70;

    /// <summary>
    /// Gets the recommended z-index for a given portal type.
    /// </summary>
    /// <param name="portalType">The type of portal (dialog, popover, tooltip)</param>
    /// <returns>The appropriate z-index value</returns>
    public static int GetZIndex(PortalType portalType)
    {
        return portalType switch
        {
            PortalType.DialogOverlay => DialogOverlay,
            PortalType.DialogContent => DialogContent,
            PortalType.PopoverContent => PopoverContent,
            PortalType.TooltipContent => TooltipContent,
            _ => PopoverContent // Default to popover level
        };
    }
}

/// <summary>
/// Enum representing the different types of portals in the system.
/// </summary>
public enum PortalType
{
    /// <summary>
    /// Dialog overlay/backdrop
    /// </summary>
    DialogOverlay,

    /// <summary>
    /// Dialog content container
    /// </summary>
    DialogContent,

    /// <summary>
    /// Popover, Select, DropdownMenu, ContextMenu content
    /// </summary>
    PopoverContent,

    /// <summary>
    /// Tooltip content
    /// </summary>
    TooltipContent
}
