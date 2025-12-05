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
    /// Default alert style with neutral colors.
    /// Uses --background and --foreground CSS variables.
    /// Suitable for general information or status messages.
    /// </summary>
    Default,

    /// <summary>
    /// Destructive alert style for errors or critical warnings.
    /// Uses --destructive and --destructive-foreground CSS variables.
    /// Indicates critical status or requires immediate user attention.
    /// </summary>
    Destructive
}
