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
    /// Muted alert style with neutral gray accent.
    /// Uses --muted and --foreground CSS variables.
    /// Suitable for subtle informational messages.
    /// </summary>
    Muted,

    /// <summary>
    /// Destructive alert style for errors and critical warnings.
    /// Uses --destructive and --destructive-foreground CSS variables.
    /// Indicates errors, failures, or requires immediate user attention.
    /// </summary>
    Destructive,

    /// <summary>
    /// Success alert style with green accent.
    /// Uses --alert-success CSS variables.
    /// Indicates successful operations or positive status.
    /// </summary>
    Success,

    /// <summary>
    /// Info alert style with blue accent.
    /// Uses --alert-info CSS variables.
    /// For informational or educational messages.
    /// </summary>
    Info,

    /// <summary>
    /// Warning alert style with amber/orange accent.
    /// Uses --alert-warning CSS variables.
    /// Indicates caution or potential issues.
    /// </summary>
    Warning,

    /// <summary>
    /// Danger alert style with red accent.
    /// Uses --alert-danger CSS variables.
    /// Indicates errors, critical warnings, or destructive actions.
    /// </summary>
    Danger
}
