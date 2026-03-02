using BlazorUI.Components.Utilities;
using BlazorUI.Primitives.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorUI.Components.Button;

/// <summary>
/// A link component styled as a button that follows the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The LinkButton component provides a semantic anchor tag (&lt;a&gt;) styled to look like a button.
/// It's perfect for navigation while maintaining proper HTML semantics and accessibility.
/// Use this instead of Button when the action is navigation rather than form submission or
/// state changes.
/// </para>
/// <para>
/// Features:
/// - Semantic HTML with &lt;a&gt; element for better SEO and accessibility
/// - Same visual styles as Button component (variants, sizes)
/// - Support for external links with target attribute
/// - Full keyboard navigation (Tab, Enter)
/// - ARIA attributes for screen readers
/// - RTL (Right-to-Left) support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;LinkButton Href="/dashboard" Variant="ButtonVariant.Default"&gt;
///     Go to Dashboard
/// &lt;/LinkButton&gt;
/// 
/// &lt;LinkButton Href="https://docs.example.com" Target="_blank"&gt;
///     Documentation
/// &lt;/LinkButton&gt;
/// </code>
/// </example>
public partial class LinkButton : ComponentBase
{
    /// <summary>
    /// Gets or sets the URL that the link points to.
    /// </summary>
    /// <remarks>
    /// This is the href attribute of the anchor element.
    /// Can be a relative path (/dashboard) or absolute URL (https://example.com).
    /// </remarks>
    [Parameter, EditorRequired]
    public required string Href { get; set; }

    /// <summary>
    /// Gets or sets where to open the linked document.
    /// </summary>
    /// <remarks>
    /// Common values:
    /// - "_self" (default) - Opens in the same frame
    /// - "_blank" - Opens in a new window or tab
    /// - "_parent" - Opens in the parent frame
    /// - "_top" - Opens in the full body of the window
    /// </remarks>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets the visual style variant of the link button.
    /// </summary>
    /// <remarks>
    /// Controls the color scheme and visual appearance using CSS custom properties.
    /// Default value is <see cref="ButtonVariant.Default"/>.
    /// </remarks>
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Default;

    /// <summary>
    /// Gets or sets the size of the link button.
    /// </summary>
    /// <remarks>
    /// Controls padding, font size, and overall dimensions.
    /// Default value is <see cref="ButtonSize.Default"/>.
    /// All sizes maintain minimum touch target sizes (44x44px) for accessibility.
    /// </remarks>
    [Parameter]
    public ButtonSize Size { get; set; } = ButtonSize.Default;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the link.
    /// </summary>
    /// <remarks>
    /// Custom classes are appended after the component's base classes,
    /// allowing for style overrides and extensions.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the link is clicked.
    /// </summary>
    /// <remarks>
    /// The event handler receives a <see cref="MouseEventArgs"/> parameter with click details.
    /// This allows you to prevent navigation or add custom behavior before navigation occurs.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the link button.
    /// </summary>
    /// <remarks>
    /// Can contain text, icons, or any other Blazor markup.
    /// For icon-only links, provide an aria-label for accessibility.
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the link.
    /// </summary>
    /// <remarks>
    /// Required for icon-only links to provide accessible text for screen readers.
    /// Optional for links with text content.
    /// </remarks>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the icon to display in the link button.
    /// </summary>
    /// <remarks>
    /// Can be any RenderFragment (SVG, icon font, image).
    /// Position is controlled by <see cref="IconPosition"/>.
    /// Automatically adds RTL-aware spacing between icon and text.
    /// </remarks>
    [Parameter]
    public RenderFragment? Icon { get; set; }

    /// <summary>
    /// Gets or sets the position of the icon relative to the link text.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="IconPosition.Start"/> (before text in LTR).
    /// Automatically adapts to RTL layouts using Tailwind directional utilities.
    /// </remarks>
    [Parameter]
    public IconPosition IconPosition { get; set; } = IconPosition.Start;

    /// <summary>
    /// Gets or sets additional HTML attributes to apply to the anchor element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the trigger context from a parent trigger component when using AsChild pattern.
    /// </summary>
    /// <remarks>
    /// When a LinkButton is used as a child of a trigger component with AsChild=true,
    /// it automatically receives this context to handle trigger behavior.
    /// </remarks>
    [CascadingParameter(Name = "TriggerContext")]
    public TriggerContext? TriggerContext { get; set; }

    /// <summary>
    /// Reference to the anchor element for positioning support when used with AsChild.
    /// </summary>
    private ElementReference _linkRef;

    /// <summary>
    /// Gets the computed CSS classes for the link button element.
    /// </summary>
    /// <remarks>
    /// Combines:
    /// - Base button styles (positioning, transitions, focus states)
    /// - Variant-specific classes (colors, backgrounds, borders)
    /// - Size-specific classes (padding, font size, height)
    /// - Custom classes from the Class parameter
    /// Uses the cn() utility for intelligent class merging and Tailwind conflict resolution.
    /// Note: Uses 'no-underline' to prevent default link underline except for Link variant.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base button styles (from shadcn/ui)
        "inline-flex items-center justify-center gap-2 rounded-md text-sm font-medium",
        "transition-colors focus-visible:outline-none focus-visible:ring-2",
        "focus-visible:ring-ring focus-visible:ring-offset-2",
        "no-underline", // Override default link underline (except for Link variant)
        // Variant-specific styles
        Variant switch
        {
            ButtonVariant.Default => "bg-primary text-primary-foreground hover:bg-primary/90",
            ButtonVariant.Destructive => "bg-destructive text-destructive-foreground hover:bg-destructive/90",
            ButtonVariant.Outline => "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
            ButtonVariant.Secondary => "bg-secondary text-secondary-foreground hover:bg-secondary/80",
            ButtonVariant.Ghost => "hover:bg-accent hover:text-accent-foreground",
            ButtonVariant.Link => "text-primary underline-offset-4 hover:underline",
            _ => "bg-primary text-primary-foreground hover:bg-primary/90"
        },
        // Size-specific styles
        Size switch
        {
            ButtonSize.Small => "h-9 rounded-md px-3 text-xs",
            ButtonSize.Default => "h-10 px-4 py-2",
            ButtonSize.Large => "h-11 rounded-md px-8",
            ButtonSize.Icon => "h-10 w-10",
            ButtonSize.IconSmall => "h-9 w-9",
            ButtonSize.IconLarge => "h-11 w-11",
            _ => "h-10 px-4 py-2"
        },
        // Custom classes (if provided)
        Class
    );

    /// <summary>
    /// Handles the link click event.
    /// </summary>
    /// <param name="args">The mouse event arguments.</param>
    /// <remarks>
    /// This method is invoked when the link is clicked.
    /// If a TriggerContext is present (from AsChild pattern), it invokes Toggle or Close.
    /// It also triggers the OnClick callback if one is registered.
    /// The browser's default navigation behavior occurs unless preventDefault() is called in OnClick.
    /// </remarks>
    private async Task HandleClick(MouseEventArgs args)
    {
        // Handle trigger context behavior (from AsChild pattern)
        if (TriggerContext != null)
        {
            // For close buttons (no Toggle, only Close)
            if (TriggerContext.Toggle == null && TriggerContext.Close != null)
            {
                TriggerContext.Close.Invoke();
            }
            // For trigger buttons (have Toggle)
            else if (TriggerContext.Toggle != null)
            {
                TriggerContext.Toggle.Invoke();
            }
        }

        // Invoke the OnClick callback
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }
    }

    /// <summary>
    /// Handles mouse enter events for hover-triggered components (Tooltip, HoverCard).
    /// </summary>
    private void HandleMouseEnter()
    {
        TriggerContext?.OnMouseEnter?.Invoke();
    }

    /// <summary>
    /// Handles mouse leave events for hover-triggered components.
    /// </summary>
    private void HandleMouseLeave()
    {
        TriggerContext?.OnMouseLeave?.Invoke();
    }

    /// <summary>
    /// Handles focus events for focus-triggered components.
    /// </summary>
    private void HandleFocus()
    {
        TriggerContext?.OnFocus?.Invoke();
    }

    /// <summary>
    /// Handles blur events for focus-triggered components.
    /// </summary>
    private void HandleBlur()
    {
        TriggerContext?.OnBlur?.Invoke();
    }

    /// <summary>
    /// Registers the link element reference with the trigger context for positioning.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && TriggerContext?.SetTriggerElement != null)
        {
            TriggerContext.SetTriggerElement(_linkRef);
        }
    }
}
