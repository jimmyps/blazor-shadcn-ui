using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Alert;

/// <summary>
/// An alert component that displays a callout for important information or feedback.
/// </summary>
/// <remarks>
/// <para>
/// The Alert component provides a prominent way to display important messages,
/// warnings, or status updates. It follows the shadcn/ui design system with
/// multiple visual variants for different contexts.
/// </para>
/// <para>
/// Features:
/// - 2 visual variants (Default, Destructive)
/// - Semantic HTML with role="alert" for accessibility
/// - Support for icons, titles, and descriptions
/// - RTL (Right-to-Left) support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Alert&gt;
///     &lt;AlertTitle&gt;Heads up!&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;You can add components and dependencies to your app using the cli.&lt;/AlertDescription&gt;
/// &lt;/Alert&gt;
///
/// &lt;Alert Variant="AlertVariant.Destructive"&gt;
///     &lt;AlertTitle&gt;Error&lt;/AlertTitle&gt;
///     &lt;AlertDescription&gt;Your session has expired. Please log in again.&lt;/AlertDescription&gt;
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
    /// Typically contains AlertTitle and AlertDescription components,
    /// and optionally an icon. For accessibility, ensure meaningful content.
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the alert element.
    /// </summary>
    /// <remarks>
    /// Combines:
    /// - Base alert styles (relative, rounded-lg, border, padding)
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
            AlertVariant.Destructive => "border-destructive/50 text-destructive dark:border-destructive [&>svg]:text-destructive",
            _ => "bg-background text-foreground"
        },
        // Custom classes (if provided)
        Class
    );
}
