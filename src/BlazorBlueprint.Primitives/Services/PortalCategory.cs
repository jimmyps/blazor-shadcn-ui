namespace BlazorBlueprint.Primitives;

/// <summary>
/// Defines the category of portal content, used to separate rendering
/// responsibilities between different portal host components.
/// </summary>
public enum PortalCategory
{
    /// <summary>
    /// Floating positioned UI elements: Popover, Select, Dropdown, Tooltip, HoverCard.
    /// These are lightweight and frequently toggled.
    /// </summary>
    Overlay,

    /// <summary>
    /// Modal overlay components: Dialog, Sheet, AlertDialog.
    /// These are heavier and less frequently toggled.
    /// </summary>
    Container
}
