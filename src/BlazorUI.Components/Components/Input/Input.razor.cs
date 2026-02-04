using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace BlazorUI.Components.Input;

/// <summary>
/// An input component that follows the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The Input component provides a customizable, accessible form input that supports
/// multiple input types and states. It follows WCAG 2.1 AA standards
/// for accessibility and integrates with Blazor's data binding system.
/// </para>
/// <para>
/// Features:
/// - Multiple input types (text, email, password, number, tel, url, file, search, date, time)
/// - Form submission support via name attribute
/// - Browser autocomplete integration
/// - Read-only state support
/// - Input validation (required, pattern, min, max, minlength, maxlength)
/// - Number input controls (min, max, step)
/// - Mobile keyboard hints via inputmode
/// - Auto-focus capability
/// - Spell check control
/// - File input styling with custom pseudo-selectors
/// - Error state visualization via aria-invalid attribute
/// - Smooth color transitions for state changes
/// - Disabled and required states
/// - Placeholder text support
/// - Two-way data binding with Value/ValueChanged
/// - Full ARIA attribute support
/// - RTL (Right-to-Left) support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Input Type="InputType.Text" @bind-Value="userName" Name="username" Placeholder="Enter your name" /&gt;
///
/// &lt;Input Type="InputType.Email" Value="@email" ValueChanged="HandleEmailChange" Name="email" Required="true" Autocomplete="email" AriaInvalid="@hasError" /&gt;
///
/// &lt;Input Type="InputType.Number" @bind-Value="age" Name="age" Min="0" Max="120" Step="1" /&gt;
/// </code>
/// </example>
public partial class Input : ComponentBase, IAsyncDisposable
{
    private static string? _firstInvalidInputId = null;
    
    private IJSObjectReference? _validationModule;
    private EditContext? _previousEditContext;
    private FieldIdentifier _fieldIdentifier;
    private string? _currentErrorMessage;
    private bool _hasShownTooltip = false;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets the cascaded EditContext from an EditForm.
    /// </summary>
    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    /// <summary>
    /// Gets or sets the type of input.
    /// </summary>
    /// <remarks>
    /// Determines the HTML input type attribute.
    /// Default value is <see cref="InputType.Text"/>.
    /// </remarks>
    [Parameter]
    public InputType Type { get; set; } = InputType.Text;

    /// <summary>
    /// Gets or sets the current value of the input.
    /// </summary>
    /// <remarks>
    /// Supports two-way binding via @bind-Value syntax.
    /// </remarks>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the input value changes.
    /// </summary>
    /// <remarks>
    /// This event is fired on every keystroke (oninput event).
    /// Use with Value parameter for two-way binding.
    /// </remarks>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text displayed when the input is empty.
    /// </summary>
    /// <remarks>
    /// Provides a hint to the user about what to enter.
    /// Should not be used as a replacement for a label.
    /// </remarks>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets whether the input is disabled.
    /// </summary>
    /// <remarks>
    /// When disabled:
    /// - Input cannot be focused or edited
    /// - Cursor is set to not-allowed
    /// - Opacity is reduced for visual feedback
    /// </remarks>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the input is required.
    /// </summary>
    /// <remarks>
    /// When true, the HTML5 required attribute is set.
    /// Works with form validation and :invalid CSS pseudo-class.
    /// </remarks>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the name of the input for form submission.
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
    /// Examples: "email", "username", "current-password", "new-password", "name", "tel", "off".
    /// Helps browsers provide appropriate autofill suggestions.
    /// </remarks>
    [Parameter]
    public string? Autocomplete { get; set; }

    /// <summary>
    /// Gets or sets whether the input is read-only.
    /// </summary>
    /// <remarks>
    /// When true, the user cannot modify the value, but it's still focusable and submitted with forms.
    /// Different from Disabled - readonly inputs are still submitted with forms.
    /// </remarks>
    [Parameter]
    public bool Readonly { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of characters allowed.
    /// </summary>
    /// <remarks>
    /// When set, the browser will prevent users from entering more characters.
    /// Applies to text, email, password, tel, url, and search types.
    /// </remarks>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of characters required.
    /// </summary>
    /// <remarks>
    /// Works with form validation.
    /// Applies to text, email, password, tel, url, and search types.
    /// </remarks>
    [Parameter]
    public int? MinLength { get; set; }

    /// <summary>
    /// Gets or sets the minimum value for number, date, or time inputs.
    /// </summary>
    /// <remarks>
    /// Applies to number, date, time inputs.
    /// Works with form validation and :invalid pseudo-class.
    /// </remarks>
    [Parameter]
    public string? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for number, date, or time inputs.
    /// </summary>
    /// <remarks>
    /// Applies to number, date, time inputs.
    /// Works with form validation and :invalid pseudo-class.
    /// </remarks>
    [Parameter]
    public string? Max { get; set; }

    /// <summary>
    /// Gets or sets the step interval for number inputs.
    /// </summary>
    /// <remarks>
    /// Defines the granularity of values (e.g., "0.01" for currency, "1" for integers).
    /// Applies to number, date, time inputs.
    /// </remarks>
    [Parameter]
    public string? Step { get; set; }

    /// <summary>
    /// Gets or sets the regex pattern for validation.
    /// </summary>
    /// <remarks>
    /// Validates input against the specified regular expression.
    /// Works with form validation and :invalid pseudo-class.
    /// </remarks>
    [Parameter]
    public string? Pattern { get; set; }

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
    /// Gets or sets whether the input should be auto-focused when the page loads.
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
    /// Useful for controlling spell checking on email addresses, usernames, etc.
    /// </remarks>
    [Parameter]
    public bool? Spellcheck { get; set; }


    /// <summary>
    /// Gets or sets additional CSS classes to apply to the input.
    /// </summary>
    /// <remarks>
    /// Custom classes are appended after the component's base classes,
    /// allowing for style overrides and extensions.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute for the input element.
    /// </summary>
    /// <remarks>
    /// Used to associate the input with a label element via the label's 'for' attribute.
    /// This is essential for accessibility and allows clicking the label to focus the input.
    /// </remarks>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the input.
    /// </summary>
    /// <remarks>
    /// Provides an accessible name for screen readers.
    /// Use when there is no visible label element.
    /// </remarks>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element that describes the input.
    /// </summary>
    /// <remarks>
    /// References the id of an element containing help text or error messages.
    /// Improves screen reader experience by associating descriptive text.
    /// </remarks>
    [Parameter]
    public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets whether the input value is invalid.
    /// </summary>
    /// <remarks>
    /// When true, aria-invalid="true" is set.
    /// Should be set based on validation state.
    /// </remarks>
    [Parameter]
    public bool? AriaInvalid { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically show validation errors from EditContext.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When true and used within an EditForm, automatically:
    /// - Displays validation errors in a native browser tooltip
    /// - Focuses the first invalid input
    /// - Sets AriaInvalid to true for error styling (red border/ring)
    /// </para>
    /// <para>
    /// Only the FIRST invalid input will show the tooltip and receive focus.
    /// All invalid inputs will get the destructive border/ring styling via aria-invalid.
    /// </para>
    /// <para>
    /// Requires ValueExpression to be set (automatically set when using @bind-Value).
    /// Best used together with ValidationMessage for persistent error display.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// &lt;Input Id="email"
    ///        @bind-Value="model.Email"
    ///        ShowValidationError="true" /&gt;
    /// &lt;ValidationMessage For="@(() => model.Email)" /&gt;
    /// </code>
    /// </example>
    [Parameter]
    public bool ShowValidationError { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    /// <remarks>
    /// This is automatically set when using @bind-Value syntax.
    /// Required for ShowValidationError to work with EditContext validation.
    /// </remarks>
    [Parameter]
    public Expression<Func<string?>>? ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets additional attributes to be applied to the input element.
    /// </summary>
    /// <remarks>
    /// Captures any HTML attributes not explicitly defined as parameters.
    /// This allows for maximum flexibility while maintaining type safety for common attributes.
    /// Examples: data-* attributes, form, list, size, title, tabindex, etc.
    /// </remarks>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the input element.
    /// </summary>
    /// <remarks>
    /// Combines:
    /// - Base input styles (flex, rounded, border, transitions, focus states)
    /// - File input pseudo-selector styling for better file input appearance
    /// - aria-invalid pseudo-selector for error state styling with destructive colors
    /// - Smooth color transitions for state changes
    /// - Disabled and required state styles
    /// - Placeholder text styles
    /// - RTL and dark mode adjustments
    /// - Custom classes from the Class parameter
    /// Uses the cn() utility for intelligent class merging and Tailwind conflict resolution.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base input styles (from shadcn/ui)
        "flex h-8 w-full rounded-md border border-input bg-background px-2 py-1 text-base shadow-xs",
        "file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground",
        "placeholder:text-muted-foreground",
        "outline-none focus-visible:border-ring focus-visible:ring-[2px] focus-visible:ring-ring/50",
        "disabled:cursor-not-allowed disabled:opacity-50",
        // aria-invalid state styling (destructive error colors)
        "aria-[invalid=true]:border-destructive aria-[invalid=true]:ring-destructive",
        "aria-[invalid=true]:focus-visible:ring-destructive/30",
        // Smooth transitions for state changes
        "transition-[color,box-shadow]",
        // Medium screens and up: smaller text
        "md:text-sm",
        // Custom classes (if provided)
        Class
    );

    /// <summary>
    /// Gets the effective name attribute, falling back to Id if Name is not specified.
    /// </summary>
    /// <remarks>
    /// This ensures form submission works even when Name is not explicitly set.
    /// </remarks>
    private string? EffectiveName => Name ?? Id;

    /// <summary>
    /// Gets the HTML input type attribute value.
    /// </summary>
    private string HtmlType => Type switch
    {
        InputType.Text => "text",
        InputType.Email => "email",
        InputType.Password => "password",
        InputType.Number => "number",
        InputType.Tel => "tel",
        InputType.Url => "url",
        InputType.Search => "search",
        InputType.Date => "date",
        InputType.Time => "time",
        InputType.File => "file",
        _ => "text"
    };

    /// <summary>
    /// Gets the effective AriaInvalid value.
    /// When ShowValidationError is true, this is automatically set based on validation state.
    /// Otherwise, uses the manually set AriaInvalid parameter.
    /// </summary>
    private bool? EffectiveAriaInvalid => ShowValidationError && EditContext != null
        ? !string.IsNullOrEmpty(_currentErrorMessage)
        : AriaInvalid;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Set up field identifier for validation
        if (ShowValidationError && ValueExpression != null)
        {
            _fieldIdentifier = FieldIdentifier.Create(ValueExpression);

            // Subscribe to EditContext if it changed
            if (EditContext != _previousEditContext)
            {
                DetachValidationStateChangedListener();
                EditContext?.OnValidationStateChanged += OnValidationStateChanged;
                _previousEditContext = EditContext;
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                if (ShowValidationError)
                {
                    _validationModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/NeoBlazorUI.Components/js/input-validation.js");
                }
            }
            catch (JSException)
            {
                // JS module not available, validation will still work via HTML5
            }
        }

        // Apply validation errors after render
        if (ShowValidationError && _validationModule != null)
        {
            await UpdateValidationDisplayAsync();
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        // Reset first invalid input tracking on new validation cycle
        _firstInvalidInputId = null;
        _hasShownTooltip = false;

        InvokeAsync(async () =>
        {
            await UpdateValidationDisplayAsync();
            StateHasChanged(); // Re-render to update aria-invalid attribute
        });
    }

    private async Task UpdateValidationDisplayAsync()
    {
        if (EditContext == null || _validationModule == null || string.IsNullOrEmpty(Id))
            return;

        try
        {
            // Get validation messages for this field
            var messages = EditContext.GetValidationMessages(_fieldIdentifier).ToList();
            var errorMessage = messages.FirstOrDefault();

            // Only update if the error message changed
            if (errorMessage != _currentErrorMessage)
            {
                _currentErrorMessage = errorMessage;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    // Determine if this is the first invalid input
                    var isFirstInvalid = _firstInvalidInputId == null;
                    
                    if (isFirstInvalid)
                    {
                        _firstInvalidInputId = Id;
                    }

                    if (isFirstInvalid && !_hasShownTooltip)
                    {
                        // Only the FIRST invalid input shows tooltip and gets focus
                        await _validationModule.InvokeVoidAsync(
                            "setValidationError",
                            Id,
                            errorMessage
                        );
                        _hasShownTooltip = true;
                    }
                    else
                    {
                        // Other invalid inputs just get the custom validity set
                        // (no tooltip, no focus)
                        await _validationModule.InvokeVoidAsync(
                            "setValidationErrorSilent",
                            Id,
                            errorMessage
                        );
                    }
                }
                else
                {
                    // Clear validation error
                    await _validationModule.InvokeVoidAsync(
                        "clearValidationError",
                        Id
                    );
                    
                    _hasShownTooltip = false;
                }
            }
        }
        catch (JSException)
        {
            // Ignore JS errors
        }
    }

    private void DetachValidationStateChangedListener()
    {
        if (_previousEditContext != null)
        {
            _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }

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

        // Notify EditContext of field change to trigger validation
        if (ShowValidationError && EditContext != null && ValueExpression != null)
        {
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    /// <summary>
    /// Handles the change event (fired when input loses focus).
    /// </summary>
    /// <param name="args">The change event arguments.</param>
    private async Task HandleChange(ChangeEventArgs args)
    {
        // Change event is already handled by HandleInput for immediate updates
        // This is here for compatibility and potential future use
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        DetachValidationStateChangedListener();

        if (_validationModule is not null)
        {
            try
            {
                await _validationModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // The JS runtime is already disposed
            }
        }
    }
}
