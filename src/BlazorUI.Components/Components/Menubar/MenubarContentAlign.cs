namespace BlazorUI.Components.Menubar;

/// <summary>
/// Defines the alignment of Menubar content relative to the trigger element.
/// </summary>
/// <remarks>
/// Content alignment controls how the dropdown menu is positioned horizontally
/// in relation to the menu trigger button.
/// </remarks>
public enum MenubarContentAlign
{
    /// <summary>
    /// Aligns the menu content to the start of the trigger (left in LTR, right in RTL).
    /// </summary>
    /// <remarks>
    /// Default alignment for most menubar items.
    /// </remarks>
    Start,

    /// <summary>
    /// Centers the menu content relative to the trigger.
    /// </summary>
    /// <remarks>
    /// Useful for centered menu items or balanced layouts.
    /// </remarks>
    Center,

    /// <summary>
    /// Aligns the menu content to the end of the trigger (right in LTR, left in RTL).
    /// </summary>
    /// <remarks>
    /// Suitable for right-aligned menu items or end-positioned triggers.
    /// </remarks>
    End
}
