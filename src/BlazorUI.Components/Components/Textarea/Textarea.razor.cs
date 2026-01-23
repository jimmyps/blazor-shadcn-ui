using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorUI.Components.Textarea;

/// <summary>
/// A textarea component that follows the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The Textarea component provides a customizable, accessible multi-line text input that
/// supports various states and features. It follows WCAG 2.1 AA standards
/// for accessibility and integrates with Blazor's data binding system.
/// </para>
/// <para>
/// Features:
/// - Multi-line text input with automatic content sizing (field-sizing-content)
/// - Form submission support via name attribute
/// - Browser autocomplete integration
/// - Read-only state support
/// - Character limit support via MaxLength and MinLength parameters
/// - Configurable rows and columns
/// - Text wrapping control (soft, hard, off)
/// - Mobile keyboard hints via inputmode
/// - Auto-focus capability
/// - Spell check control
/// - Two-way data binding with Value/ValueChanged
/// - Error state visualization via aria-invalid attribute
/// - Smooth color and shadow transitions for state changes
/// - Disabled and required states
/// - Placeholder text support
/// - Full ARIA attribute support
/// - RTL (Right-to-Left) support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Textarea @bind-Value="description" Name="description" Placeholder="Enter your description" /&gt;
///
/// &lt;Textarea Value="@comment" ValueChanged="HandleCommentChange" Name="comment" MaxLength="500" MinLength="10" Required="true" AriaInvalid="@hasError" /&gt;
/// </code>
/// </example>
public partial class Textarea : ComponentBase
{
    /// <summary>
    /// Gets or sets the current value of the textarea.
    /// </summary>
    /// <remarks>
    /// Supports two-way binding via @bind-Value syntax.
    /// </remarks>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the textarea value changes.
    /// </summary>
    /// <remarks>
    /// This event is fired on every keystroke (oninput event).
    /// Use with Value parameter for two-way binding.
    /// </remarks>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text displayed when the textarea is empty.
    /// </summary>
    /// <remarks>
    /// Provides a hint to the user about what to enter.
    /// Should not be used as a replacement for a label.
    /// </remarks>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea is disabled.
    /// </summary>
    /// <remarks>
    /// When disabled:
    /// - Textarea cannot be focused or edited
    /// - Cursor is set to not-allowed
    /// - Opacity is reduced for visual feedback
    /// </remarks>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea is required.
    /// </summary>
    /// <remarks>
    /// When true, the HTML5 required attribute is set.
    /// Works with form validation and :invalid CSS pseudo-class.
    /// </remarks>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of characters allowed in the textarea.
    /// </summary>
    /// <remarks>
    /// When set, the HTML5 maxlength attribute is applied.
    /// Browser will prevent users from entering more than this many characters.
    /// </remarks>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of characters required.
    /// </summary>
    /// <remarks>
    /// Works with form validation.
    /// Browser validates that at least this many characters are present.
    /// </remarks>
    [Parameter]
    public int? MinLength { get; set; }

    /// <summary>
    /// Gets or sets the name of the textarea for form submission.
    /// </summary>
    /// <remarks>
    /// This is critical for form submission. The name/value pair is submitted to the server.
    /// Should be unique within the form.
    /// </remarks>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the autocomplete hint for the browser.
    /// </summary>
    /// <remarks>
    /// Examples: "on", "off", "name", "street-address".
    /// Helps browsers provide appropriate autofill suggestions.
    /// </remarks>
    [Parameter]
    public string? Autocomplete { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea is read-only.
    /// </summary>
    /// <remarks>
    /// When true, the user cannot modify the value, but it's still focusable and submitted with forms.
    /// Different from Disabled - readonly textareas are still submitted with forms.
    /// </remarks>
    [Parameter]
    public bool Readonly { get; set; }

    /// <summary>
    /// Gets or sets the visible number of text rows.
    /// </summary>
    /// <remarks>
    /// Specifies the height of the textarea in rows of text.
    /// If not specified, the component uses field-sizing-content for automatic sizing.
    /// </remarks>
    [Parameter]
    public int? Rows { get; set; }

    /// <summary>
    /// Gets or sets the visible width in characters.
    /// </summary>
    /// <remarks>
    /// Specifies the width of the textarea in average character widths.
    /// Usually controlled by CSS width instead.
    /// </remarks>
    [Parameter]
    public int? Cols { get; set; }

    /// <summary>
    /// Gets or sets how text wraps when submitted in a form.
    /// </summary>
    /// <remarks>
    /// Values: "soft" (default - newlines not submitted), "hard" (newlines submitted), "off" (no wrapping).
    /// When "hard", the cols attribute must be specified.
    /// </remarks>
    [Parameter]
    public string? Wrap { get; set; }

    /// <summary>
    /// Gets or sets the input mode hint for mobile keyboards.
    /// </summary>
    /// <remarks>
    /// Examples: "none", "text", "decimal", "numeric", "tel", "search", "email", "url".
    /// Helps mobile devices show the appropriate keyboard.
    /// </remarks>
    [Parameter]
    public string? InputMode { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea should be auto-focused when the page loads.
    /// </summary>
    /// <remarks>
    /// Only one element per page should have autofocus.
    /// Improves accessibility when used appropriately.
    /// </remarks>
    [Parameter]
    public bool Autofocus { get; set; }

    /// <summary>
    /// Gets or sets whether spell checking is enabled.
    /// </summary>
    /// <remarks>
    /// Can be true, false, or null (browser default).
    /// Useful for controlling spell checking on technical content, code, etc.
    /// </remarks>
    [Parameter]
    public bool? Spellcheck { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the textarea.
    /// </summary>
    /// <remarks>
    /// Custom classes are appended after the component's base classes,
    /// allowing for style overrides and extensions.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute for the textarea element.
    /// </summary>
    /// <remarks>
    /// Used to associate the textarea with a label element via the label's 'for' attribute.
    /// This is essential for accessibility and allows clicking the label to focus the textarea.
    /// </remarks>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the textarea.
    /// </summary>
    /// <remarks>
    /// Provides an accessible name for screen readers.
    /// Use when there is no visible label element.
    /// </remarks>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element that describes the textarea.
    /// </summary>
    /// <remarks>
    /// References the id of an element containing help text or error messages.
    /// Improves screen reader experience by associating descriptive text.
    /// </remarks>
    [Parameter]
    public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea value is invalid.
    /// </summary>
    /// <remarks>
    /// When true, aria-invalid="true" is set.
    /// Should be set based on validation state.
    /// Triggers destructive color styling for error states.
    /// </remarks>
    [Parameter]
    public bool? AriaInvalid { get; set; }

    /// <summary>
    /// Gets the effective name attribute, falling back to Id if Name is not specified.
    /// </summary>
    /// <remarks>
    /// This ensures form submission works even when Name is not explicitly set.
    /// </remarks>
    private string? EffectiveName => Name ?? Id;

    /// <summary>
    /// Gets the computed CSS classes for the textarea element.
    /// </summary>
    /// <remarks>
    /// Combines shadcn/ui v4 textarea styles:
    /// - field-sizing-content for automatic content-based sizing
    /// - min-h-16 for minimum height (4rem)
    /// - Base styles (flex, rounded, border, padding, transitions)
    /// - Focus states with ring effects
    /// - aria-invalid pseudo-selector for error state styling
    /// - Dark mode support via CSS variables
    /// - Smooth transitions for color and box-shadow
    /// - Custom classes from the Class parameter
    /// Uses the cn() utility for intelligent class merging and Tailwind conflict resolution.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base textarea styles (from shadcn/ui v4)
        "flex field-sizing-content min-h-16 w-full rounded-md border border-input",
        "bg-transparent dark:bg-input/30 px-3 py-2 text-base shadow-xs",
        "placeholder:text-muted-foreground",
        // Focus states
        "outline-none focus-visible:border-ring focus-visible:ring-[2px] focus-visible:ring-ring/50",
        // Error states (aria-invalid)
        "aria-[invalid=true]:border-destructive aria-[invalid=true]:ring-destructive/20",
        "dark:aria-[invalid=true]:ring-destructive/40",
        // Disabled state
        "disabled:cursor-not-allowed disabled:opacity-50",
        // Smooth transitions
        "transition-[color,box-shadow]",
        // Responsive text sizing
        "md:text-sm",
        // Custom classes (if provided)
        Class
    );

    /// <summary>
    /// Handles the input event (fired on every keystroke).
    /// </summary>
    /// <param name="args">The change event arguments.</param>
    private async Task HandleInput(ChangeEventArgs args)
    {
        var newValue = args.Value?.ToString();
        Value = newValue;

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(newValue);
        }
    }
}
