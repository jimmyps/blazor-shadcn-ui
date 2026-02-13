namespace BlazorUI.Components.Dialog;

/// <summary>
/// Defines the visual variants for DialogContent styling.
/// </summary>
public enum DialogContentVariant
{
    /// <summary>
    /// Default dialog appearance with standard background.
    /// Uses bg-background for a simple, neutral look.
    /// </summary>
    Default,

    /// <summary>
    /// Form-optimized dialog appearance with card background.
    /// Uses bg-card and includes scroll shadow customization for form layouts.
    /// Ideal for dialogs containing forms, settings, or data entry interfaces.
    /// </summary>
    Form
}
