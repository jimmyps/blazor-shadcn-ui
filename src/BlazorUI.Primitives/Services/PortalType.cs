namespace BlazorUI.Primitives.Services;

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
