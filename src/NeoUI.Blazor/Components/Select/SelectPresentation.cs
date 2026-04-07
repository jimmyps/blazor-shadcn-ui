namespace NeoUI.Blazor;

/// <summary>
/// Defines how a <see cref="Select{TValue}"/> content is presented to the user.
/// </summary>
public enum SelectPresentation
{
    /// <summary>
    /// Default floating popover anchored below the trigger element.
    /// </summary>
    Popover,

    /// <summary>
    /// Full-width bottom sheet that slides up from the bottom of the viewport.
    /// Internally reuses the <see cref="Drawer"/> component.
    /// Recommended for mobile-first interfaces.
    /// </summary>
    BottomSheet
}
