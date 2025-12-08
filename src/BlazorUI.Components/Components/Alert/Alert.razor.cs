using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Alert;

/// <summary>
/// An alert component that displays important messages to users.
/// </summary>
/// <remarks>
/// <para>
/// The Alert component provides a way to display status messages, notifications,
/// or important information. It follows the shadcn/ui design system with semantic variants.
/// </para>
/// <para>
/// Features:
/// - 2 visual variants (Default, Destructive)
/// - Semantic HTML with role="alert" for accessibility
/// - Composable with AlertTitle and AlertDescription
/// - Support for icons via child content
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Alert Variant="AlertVariant.Default"&gt;
///     &lt;AlertTitle&gt;Information&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;Your session will expire in 5 minutes.&lt;/AlertDescription&gt;
/// &lt;/Alert&gt;
///
/// &lt;Alert Variant="AlertVariant.Destructive"&gt;
///     &lt;AlertTitle&gt;Error&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;Unable to save your changes.&lt;/AlertDescription&gt;
/// &lt;/Alert&gt;
/// </code>
/// </example>
public partial class Alert : ComponentBase
{
    /// <summary>
    /// Gets or sets the visual style variant of the alert.
    /// </summary>
    /// <remarks>
    /// Controls the color scheme and visual appearance using CSS custom properties.
    /// Default value is <see cref="AlertVariant.Default"/>.
    /// </remarks>
    [Parameter]
    public AlertVariant Variant { get; set; } = AlertVariant.Default;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the alert.
    /// </summary>
    /// <remarks>
    /// Custom classes are appended after the component's base classes,
    /// allowing for style overrides and extensions.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the alert.
    /// </summary>
    /// <remarks>
    /// Typically contains AlertTitle, AlertDescription, and optionally an icon.
    /// For accessibility, ensure meaningful content is provided.
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the alert element.
    /// </summary>
    /// <remarks>
    /// Combines:
    /// - Base alert styles (relative positioning, rounded, border, padding)
    /// - Variant-specific classes (colors, backgrounds)
    /// - Custom classes from the Class parameter
    /// Uses the cn() utility for intelligent class merging and Tailwind conflict resolution.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base alert styles (from shadcn/ui)
        "relative w-full rounded-lg border p-4",
        "[&>svg~*]:pl-7 [&>svg+div]:translate-y-[-3px] [&>svg]:absolute [&>svg]:left-4 [&>svg]:top-4 [&>svg]:text-foreground",
        // Variant-specific styles
        Variant switch
        {
            AlertVariant.Default => "bg-background text-foreground",
            AlertVariant.Destructive => "border-destructive/50 text-destructive dark:border-destructive [&>svg]:text-destructive",
            _ => "bg-background text-foreground"
        },
        // Custom classes (if provided)
        Class
    );
}
