using BlazorUI.Components.Common;
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
    // Key for storing first invalid input ID in EditContext.Properties
    private static readonly object _firstInvalidInputIdKey = new();
    
    private IJSObjectReference? _inputModule;
    private DotNetObjectReference<Input>? _dotNetRef;
    private bool _jsInitialized = false;
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
    /// Gets or sets when the input should update its bound value.
    /// </summary>
    /// <remarks>
    /// - Input: Updates value immediately on every keystroke (default)
    /// - Change: Updates value only when input loses focus
    /// </remarks>
    [Parameter]
    public InputUpdateMode UpdateOn { get; set; } = InputUpdateMode.Change;

    /// <summary>
    /// Gets or sets the debounce delay in milliseconds for Input mode.
    /// Only applies when UpdateOn=Input. Set to 0 for immediate updates.
    /// Default: 0 (no debounce)
    /// </summary>
    [Parameter]
    public int DebounceDelay { get; set; } = 0;

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
                // Import the input module
                _inputModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/input.js");

                // Create DotNetObjectReference for callbacks
                _dotNetRef = DotNetObjectReference.Create(this);

                // Initialize input event handling with UpdateOn mode and debounce
                await _inputModule.InvokeVoidAsync(
                    "initializeInput",
                    Id,
                    UpdateOn.ToString().ToLower(),
                    DebounceDelay,
                    _dotNetRef
                );

                _jsInitialized = true;

                // Initialize validation if ShowValidationError is enabled
                if (ShowValidationError && EditContext != null && ValueExpression != null)
                {
                    _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
                    await _inputModule.InvokeVoidAsync("initializeValidation", Id, UpdateOn.ToString().ToLower());
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error initializing input JS: {ex.Message}");
            }
        }

        // Apply validation errors after render
        if (ShowValidationError && _inputModule != null)
        {
            await UpdateValidationDisplayAsync();
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        // Reset first invalid input tracking for this EditContext on new validation cycle
        if (EditContext != null)
        {
            EditContext.Properties.Remove(_firstInvalidInputIdKey);
        }
        _hasShownTooltip = false;

        InvokeAsync(async () =>
        {
            await UpdateValidationDisplayAsync();
            StateHasChanged(); // Re-render to update aria-invalid attribute
        });
    }

    private async Task UpdateValidationDisplayAsync()
    {
        if (EditContext == null || _inputModule == null || string.IsNullOrEmpty(Id))
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
                    // Determine if this is the first invalid input for this EditContext
                    // Using EditContext.Properties for per-form state storage
                    string? firstInvalidId = null;
                    if (EditContext.Properties.TryGetValue(_firstInvalidInputIdKey, out var value))
                    {
                        firstInvalidId = value as string;
                    }
                    var isFirstInvalid = firstInvalidId == null;
                    
                    if (isFirstInvalid)
                    {
                        EditContext.Properties[_firstInvalidInputIdKey] = Id;
                    }

                    if (isFirstInvalid && !_hasShownTooltip)
                    {
                        // Only the FIRST invalid input shows tooltip and gets focus
                        await _inputModule.InvokeVoidAsync(
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
                        await _inputModule.InvokeVoidAsync(
                            "setValidationErrorSilent",
                            Id,
                            errorMessage
                        );
                    }
                }
                else
                {
                    // Clear validation error
                    await _inputModule.InvokeVoidAsync(
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

    /// <summary>
    /// Called from JavaScript when input value changes.
    /// This is invoked based on UpdateOn mode and debounce settings.
    /// </summary>
    [JSInvokable]
    public async Task OnInputChanged(string? value)
    {
        // Update local state
        Value = value;

        // Notify parent component
        await ValueChanged.InvokeAsync(value);

        // Trigger EditContext validation if applicable
        if (EditContext != null && ValueExpression != null)
        {
            _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
            EditContext.NotifyFieldChanged(_fieldIdentifier);
            await UpdateValidationState();
        }

        // CRITICAL: Don't call StateHasChanged() here to avoid re-render during typing!
    }

    private async Task UpdateValidationState()
    {
        if (ShowValidationError && EditContext != null && _inputModule != null)
        {
            await UpdateValidationDisplayAsync();
        }
    }

    private void DetachValidationStateChangedListener()
    {
        if (_previousEditContext != null)
        {
            _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            DetachValidationStateChangedListener();

            if (_jsInitialized && _inputModule != null)
            {
                // Dispose input event handling
                await _inputModule.InvokeVoidAsync("disposeInput", Id);

                // Dispose validation tracking
                if (!string.IsNullOrEmpty(Id))
                {
                    await _inputModule.InvokeVoidAsync("disposeValidation", Id);
                }
                
                await _inputModule.DisposeAsync();
            }

            _dotNetRef?.Dispose();
        }
        catch (JSDisconnectedException)
        {
            // The JS runtime is already disposed
        }
        catch
        {
            // Ignore disposal errors
        }
    }
}
