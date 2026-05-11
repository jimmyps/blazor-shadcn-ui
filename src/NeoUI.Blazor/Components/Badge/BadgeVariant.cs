namespace NeoUI.Blazor;

/// <summary>
/// Defines the visual style variant for a Badge component.
/// </summary>
/// <remarks>
/// Badge variants follow the shadcn/ui design system and use CSS custom properties
/// for theming. Each variant conveys different meaning or urgency levels.
/// </remarks>
public enum BadgeVariant
{
    /// <summary>
    /// Default primary badge style with solid background.
    /// Uses --primary and --primary-foreground CSS variables.
    /// Suitable for highlighting important items or new content.
    /// </summary>
    Default,

    /// <summary>
    /// Secondary badge style with muted background.
    /// Uses --secondary and --secondary-foreground CSS variables.
    /// For alternative or less prominent labels.
    /// </summary>
    Secondary,

    /// <summary>
    /// Destructive badge style for warnings or errors.
    /// Uses --destructive and --destructive-foreground CSS variables.
    /// Indicates critical status or requires user attention.
    /// </summary>
    Destructive,

    /// <summary>
    /// Outlined badge style with transparent background and border.
    /// Uses --foreground CSS variable for text.
    /// Minimal style for subtle categorization or tags.
    /// </summary>
    Outline,

    /// <summary>
    /// Muted badge style with neutral gray background.
    /// Uses --muted and --muted-foreground CSS variables.
    /// Suitable for subtle or inactive labels.
    /// </summary>
    Muted,

    /// <summary>
    /// Success badge style with green background.
    /// Uses --alert-success and --alert-success-foreground CSS variables.
    /// Indicates successful operations or positive status.
    /// </summary>
    Success,

    /// <summary>
    /// Info badge style with blue background.
    /// Uses --alert-info and --alert-info-foreground CSS variables.
    /// For informational labels or educational context.
    /// </summary>
    Info,

    /// <summary>
    /// Warning badge style with amber/orange background.
    /// Uses --alert-warning and --alert-warning-foreground CSS variables.
    /// Indicates caution or potential issues.
    /// </summary>
    Warning,

    /// <summary>
    /// Danger badge style with red background.
    /// Uses --alert-danger and --alert-danger-foreground CSS variables.
    /// Indicates critical errors or destructive actions.
    /// </summary>
    Danger
}
