namespace BlazorUI.Components.Alert;

/// <summary>
/// Defines the visual style variant for an Alert component.
/// </summary>
/// <remarks>
/// Alert variants follow the shadcn/ui design system and use CSS custom properties
/// for theming. Each variant conveys different meaning or urgency levels.
/// </remarks>
public enum AlertVariant
{
    /// <summary>
    /// Default informational alert style.
    /// Uses standard --foreground and --background CSS variables.
    /// Suitable for general notifications and informational messages.
    /// </summary>
    Default,

    /// <summary>
    /// Destructive alert style for errors and critical warnings.
    /// Uses --destructive and --destructive-foreground CSS variables.
    /// Indicates errors, failures, or requires immediate user attention.
    /// </summary>
    Destructive
}
