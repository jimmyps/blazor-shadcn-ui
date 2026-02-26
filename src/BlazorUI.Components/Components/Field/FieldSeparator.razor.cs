using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorUI.Components.Field;

/// <summary>
/// A visual divider for separating sections within field groups, with optional centered content.
/// </summary>
/// <remarks>
/// <para>
/// The FieldSeparator component provides a horizontal line to visually
/// separate sections of fields within a FieldGroup or FieldSet. It can optionally
/// display centered text that appears to "break" the separator line.
/// </para>
/// <para>
/// Features:
/// - Horizontal divider line
/// - Optional centered content (e.g., "Or continue with")
/// - Consistent border color from theme
/// - Proper spacing for visual separation
/// - Content sits on background to create visual break
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Simple separator without content --&gt;
/// &lt;FieldGroup&gt;
///     &lt;Field&gt;...&lt;/Field&gt;
///     &lt;FieldSeparator /&gt;
///     &lt;Field&gt;...&lt;/Field&gt;
/// &lt;/FieldGroup&gt;
///
/// &lt;!-- Separator with centered content --&gt;
/// &lt;FieldSeparator&gt;Or continue with&lt;/FieldSeparator&gt;
/// </code>
/// </example>
public partial class FieldSeparator : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the separator.
    /// </summary>
    /// <remarks>
    /// Custom classes are merged with the component's base classes,
    /// allowing for style overrides and extensions.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to display in the center of the separator.
    /// </summary>
    /// <remarks>
    /// When provided, the content appears centered on top of the separator line
    /// with background color to create a visual break in the line.
    /// Typically used for text like "Or" or "Or continue with".
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional HTML attributes to apply to the separator element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets whether the separator has content.
    /// </summary>
    private bool HasContent => ChildContent != null;

    /// <summary>
    /// Gets the computed CSS classes for the separator container element.
    /// </summary>
    /// <remarks>
    /// Creates a relative container with proper height and spacing.
    /// When content is present, the container has specific height for the content to sit properly.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base relative container
        "relative",
        // Height and spacing - different when content is present
        HasContent ? "my-2 h-5 text-sm" : "my-2 h-px",
        // Additional spacing adjustment for outline variant when content is present
        HasContent ? "group-data-[variant=outline]/field-group:-mb-2" : null,
        // Custom classes (if provided)
        Class
    );

    /// <summary>
    /// Gets the computed CSS classes for the separator line.
    /// </summary>
    /// <remarks>
    /// The line is always rendered, either as a simple horizontal line (no content)
    /// or as an absolutely positioned line behind the content (with content).
    /// </remarks>
    private string SeparatorLineCssClass => ClassNames.cn(
        "bg-border shrink-0",
        "data-[orientation=horizontal]:h-px data-[orientation=horizontal]:w-full",
        "data-[orientation=vertical]:h-full data-[orientation=vertical]:w-px",
        // When content is present, position absolutely at center
        HasContent ? "absolute inset-0 top-1/2" : null
    );

    /// <summary>
    /// Gets the computed CSS classes for the content span (when content is present).
    /// </summary>
    private string ContentCssClass => ClassNames.cn(
        "bg-background text-muted-foreground",
        "relative mx-auto block w-fit px-2"
    );
}
