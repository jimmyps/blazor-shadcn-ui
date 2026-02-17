namespace BlazorBlueprint.Components;

/// <summary>
/// Defines the visual style variant for a Toast component.
/// </summary>
public enum ToastVariant
{
    /// <summary>
    /// Default toast style for general messages. No automatic icon.
    /// </summary>
    Default,

    /// <summary>
    /// Success toast style with green accent and circle-check icon.
    /// </summary>
    Success,

    /// <summary>
    /// Informational toast style with blue accent and info icon.
    /// </summary>
    Info,

    /// <summary>
    /// Warning toast style with amber accent and triangle-alert icon.
    /// </summary>
    Warning,

    /// <summary>
    /// Destructive toast style for errors with red accent and circle-x icon.
    /// </summary>
    Destructive
}
