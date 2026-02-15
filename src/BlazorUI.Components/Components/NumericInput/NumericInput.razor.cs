using BlazorUI.Components.Common;
using BlazorUI.Components.Utilities;
using BlazorUI.Components.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;
using System.Linq.Expressions;

namespace BlazorUI.Components.NumericInput;

/// <summary>
/// A numeric input component that follows the shadcn/ui design system.
/// </summary>
/// <typeparam name="TValue">The type of numeric value (int, decimal, double, or float).</typeparam>
/// <remarks>
/// <para>
/// The NumericInput component provides a customizable, accessible numeric input that supports
/// multiple numeric types. It follows WCAG 2.1 AA standards for accessibility and integrates 
/// with Blazor's data binding system.
/// </para>
/// <para>
/// Features:
/// - Generic type support for int, decimal, double, and float
/// - Form submission support via name attribute
/// - Input validation (required, min, max, step)
/// - Number input controls (min, max, step)
/// - Mobile keyboard hints via inputmode
/// - Auto-focus capability
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
/// &lt;NumericInput TValue="int" @bind-Value="quantity" Name="quantity" Placeholder="Enter quantity" /&gt;
///
/// &lt;NumericInput TValue="decimal" @bind-Value="price" Name="price" Min="0" Max="1000" Step="0.01" /&gt;
///
/// &lt;NumericInput TValue="double" @bind-Value="percentage" Name="percentage" Min="0" Max="100" Step="0.1" /&gt;
/// </code>
/// </example>
public partial class NumericInput<TValue> : ComponentBase, IAsyncDisposable
{
    private IJSObjectReference? _inputModule;
    private InputValidationBehavior? _validationBehavior;
    private IJSObjectReference? _cursorModule;
    private DotNetObjectReference<NumericInput<TValue>>? _dotNetRef;
    private bool _jsInitialized = false;
    private string? _generatedId;

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
    /// Gets or sets the current value of the input.
    /// </summary>
    /// <remarks>
    /// Supports two-way binding via @bind-Value syntax.
    /// </remarks>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the input value changes.
    /// </summary>
    /// <remarks>
    /// This event is fired on every keystroke (oninput event).
    /// Use with Value parameter for two-way binding.
    /// </remarks>
    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

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
    /// When set, prevents users from entering more characters via JavaScript enforcement.
    /// Works on type="number" inputs (unlike the maxlength HTML attribute which is ignored on number inputs).
    /// </remarks>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the minimum value for number inputs.
    /// </summary>
    /// <remarks>
    /// Works with form validation and :invalid pseudo-class.
    /// </remarks>
    [Parameter]
    public string? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for number inputs.
    /// </summary>
    /// <remarks>
    /// Works with form validation and :invalid pseudo-class.
    /// </remarks>
    [Parameter]
    public string? Max { get; set; }

    /// <summary>
    /// Gets or sets the step interval for number inputs.
    /// </summary>
    /// <remarks>
    /// Defines the granularity of values (e.g., "0.01" for currency, "1" for integers).
    /// </remarks>
    [Parameter]
    public string? Step { get; set; }

    /// <summary>
    /// Gets or sets the input mode hint for mobile keyboards.
    /// </summary>
    /// <remarks>
    /// Examples: "decimal", "numeric".
    /// Helps mobile devices show the appropriate keyboard.
    /// Default is "decimal" for numeric inputs.
    /// </remarks>
    [Parameter]
    public string? InputMode { get; set; } = "decimal";

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
    /// &lt;NumericInput TValue="int" Id="age"
    ///        @bind-Value="model.Age"
    ///        ShowValidationError="true" /&gt;
    /// &lt;ValidationMessage For="@(() => model.Age)" /&gt;
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
    public Expression<Func<TValue?>>? ValueExpression { get; set; }

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
    /// Gets whether the input is in an invalid state (for validation).
    /// </summary>
    /// <remarks>
    /// When ShowValidationError is true, this is automatically set based on validation state.
    /// Otherwise, uses the manually set AriaInvalid parameter.
    /// </remarks>
    private bool? EffectiveAriaInvalid => 
        _validationBehavior?.EffectiveAriaInvalid ?? AriaInvalid;

    /// <summary>
    /// Gets the computed CSS classes for the input element.
    /// </summary>
    /// <remarks>
    /// Combines:
    /// - Base input styles (flex, rounded, border, transitions, focus states)
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
    /// Gets the effective ID, generating a unique ID if none is provided.
    /// </summary>
    /// <remarks>
    /// This ensures JavaScript can always reference the element, even when Id is not explicitly set.
    /// The generated ID follows the pattern: numeric-input-{6-character-guid}.
    /// </remarks>
    private string EffectiveId
    {
        get
        {
            if (!string.IsNullOrEmpty(Id))
                return Id;

            if (_generatedId == null)
            {
                // Generate a unique 6-character ID using GUID
                _generatedId = "numeric-input-" + Guid.NewGuid().ToString("N")[..6];
            }

            return _generatedId;
        }
    }

    /// <summary>
    /// Gets the current value as a string for binding to the input element.
    /// </summary>
    private string? CurrentValueAsString
    {
        get
        {
            if (Value == null)
                return null;

            // Use invariant culture for consistent number formatting
            return Convert.ToString(Value, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// Called from JavaScript when input value changes.
    /// This is invoked based on UpdateOn mode and debounce settings.
    /// </summary>
    [JSInvokable]
    public async Task OnInputChanged(string? value)
    {
        var parsedValue = TryParseValue(value);
        var originalValue = parsedValue;
        
        // Clamp to min/max ONLY when UpdateOn=Change (on blur)
        // For UpdateOn=Input, clamping happens separately on blur via ValidateAndClamp
        if (UpdateOn == InputUpdateMode.Change)
        {
            parsedValue = ClampToRange(parsedValue);
            
            // If value was clamped, update the DOM input element via JS
            if (!EqualityComparer<TValue>.Default.Equals(originalValue, parsedValue) && _inputModule != null)
            {
                var clampedString = Convert.ToString(parsedValue, CultureInfo.InvariantCulture);
                await _inputModule.InvokeVoidAsync("updateValue", EffectiveId, clampedString);
            }
        }
        
        // Update local state
        Value = parsedValue;

        // Notify parent component
        await ValueChanged.InvokeAsync(parsedValue);

        // Trigger EditContext validation if applicable
        if (_validationBehavior != null)
        {
            await _validationBehavior.NotifyFieldChangedAsync();
        }
    }

    /// <summary>
    /// Called from JavaScript on blur when UpdateOn=Input to validate and clamp value.
    /// </summary>
    [JSInvokable]
    public async Task ValidateAndClamp()
    {
        if (Value == null)
            return;

        var clampedValue = ClampToRange(Value);
        
        // Only update if value was clamped
        if (!EqualityComparer<TValue>.Default.Equals(Value, clampedValue))
        {
            Value = clampedValue;
            await ValueChanged.InvokeAsync(clampedValue);
            
            if (_validationBehavior != null)
            {
                await _validationBehavior.NotifyFieldChangedAsync();
            }
            
            // Update DOM input element with clamped value via JS
            if (_inputModule != null)
            {
                var clampedString = Convert.ToString(clampedValue, CultureInfo.InvariantCulture);
                await _inputModule.InvokeVoidAsync("updateValue", EffectiveId, clampedString);
            }
        }
    }

    /// <summary>
    /// Clamps the value to the Min/Max range if specified.
    /// </summary>
    private TValue? ClampToRange(TValue? value)
    {
        if (value == null)
            return value;

        var targetType = typeof(TValue);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Parse Min and Max values
        var minValue = TryParseValue(Min);
        var maxValue = TryParseValue(Max);

        if (minValue == null && maxValue == null)
            return value; // No clamping needed

        // Clamp based on type
        if (underlyingType == typeof(int))
        {
            var intValue = (int)(object)value;
            if (minValue != null && intValue < (int)(object)minValue)
                return minValue;
            if (maxValue != null && intValue > (int)(object)maxValue)
                return maxValue;
        }
        else if (underlyingType == typeof(decimal))
        {
            var decValue = (decimal)(object)value;
            if (minValue != null && decValue < (decimal)(object)minValue)
                return minValue;
            if (maxValue != null && decValue > (decimal)(object)maxValue)
                return maxValue;
        }
        else if (underlyingType == typeof(double))
        {
            var dblValue = (double)(object)value;
            if (minValue != null && dblValue < (double)(object)minValue)
                return minValue;
            if (maxValue != null && dblValue > (double)(object)maxValue)
                return maxValue;
        }
        else if (underlyingType == typeof(float))
        {
            var fltValue = (float)(object)value;
            if (minValue != null && fltValue < (float)(object)minValue)
                return minValue;
            if (maxValue != null && fltValue > (float)(object)maxValue)
                return maxValue;
        }
        else if (underlyingType == typeof(long))
        {
            var longValue = (long)(object)value;
            if (minValue != null && longValue < (long)(object)minValue)
                return minValue;
            if (maxValue != null && longValue > (long)(object)maxValue)
                return maxValue;
        }
        else if (underlyingType == typeof(short))
        {
            var shortValue = (short)(object)value;
            if (minValue != null && shortValue < (short)(object)minValue)
                return minValue;
            if (maxValue != null && shortValue > (short)(object)maxValue)
                return maxValue;
        }

        return value;
    }

    /// <summary>
    /// Tries to parse the string value to the target numeric type.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <returns>The parsed value, or null if parsing fails.</returns>
    private TValue? TryParseValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var targetType = typeof(TValue);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            if (underlyingType == typeof(int))
            {
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                    return (TValue)(object)result;
            }
            else if (underlyingType == typeof(decimal))
            {
                if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                    return (TValue)(object)result;
            }
            else if (underlyingType == typeof(double))
            {
                if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return (TValue)(object)result;
            }
            else if (underlyingType == typeof(float))
            {
                if (float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
                    return (TValue)(object)result;
            }
            else if (underlyingType == typeof(long))
            {
                if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                    return (TValue)(object)result;
            }
            else if (underlyingType == typeof(short))
            {
                if (short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                    return (TValue)(object)result;
            }
        }
        catch (OverflowException)
        {
            // Value is too large or too small for the target type
            // Return default value
        }
        catch (FormatException)
        {
            // Value is not in a valid format
            // Return default value
        }

        return default;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        if (ShowValidationError)
        {
            _validationBehavior = new InputValidationBehavior(
                owner: this,
                getEffectiveId: () => EffectiveId,
                getEditContext: () => EditContext,
                shouldShowValidation: () => ShowValidationError,
                getJsModule: () => _inputModule
            );
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_validationBehavior != null)
        {
            _validationBehavior.OnParametersSet(ValueExpression);
            
            // Subscribe to EditContext validation state changes
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged -= OnValidationStateChanged;
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
            }
        }
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        if (_validationBehavior == null) return;
        
        InvokeAsync(async () =>
        {
            var shouldRender = await _validationBehavior.HandleValidationStateChangedAsync();
            if (shouldRender)
            {
                StateHasChanged();
            }
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Import the input module for event handling
                _inputModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/input.js");

                // Create DotNetObjectReference for callbacks
                _dotNetRef = DotNetObjectReference.Create(this);

                // Determine if blur validation is needed (UpdateOn=Input + Min/Max)
                bool needsBlurValidation = UpdateOn == InputUpdateMode.Input && (Min != null || Max != null);

                // Initialize input event handling with UpdateOn mode, debounce, and blur validation flag
                await _inputModule.InvokeVoidAsync(
                    "initializeInput",
                    EffectiveId,
                    UpdateOn.ToString().ToLower(),
                    DebounceDelay,
                    _dotNetRef,
                    needsBlurValidation  // Pass blur validation flag
                );

                // Initialize MaxLength enforcement if specified
                if (MaxLength.HasValue)
                {
                    await _inputModule.InvokeVoidAsync(
                        "enforceMaxLength",
                        EffectiveId,
                        MaxLength.Value
                    );
                }

                _jsInitialized = true;

                // Initialize validation if ShowValidationError is enabled
                if (ShowValidationError && EditContext != null && ValueExpression != null)
                {
                    await _inputModule.InvokeVoidAsync("initializeValidation", EffectiveId, UpdateOn.ToString().ToLower());
                }
                
                // Load cursor position module (still needed for programmatic updates)
                _cursorModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/cursor-position.js");

                // Apply initial validation state after first render
                if (ShowValidationError && _validationBehavior != null)
                {
                    await _validationBehavior.UpdateValidationDisplayAsync();
                }
            }
            catch (JSException ex)
            {
                Console.Error.WriteLine($"Error initializing numeric input JS: {ex.Message}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            // Unsubscribe from EditContext event
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged -= OnValidationStateChanged;
            }
            
            if (_validationBehavior != null)
            {
                await _validationBehavior.DisposeAsync();
            }

            if (_jsInitialized && _inputModule != null)
            {
                // Dispose input event handling (now includes blur validation)
                await _inputModule.InvokeVoidAsync("disposeInput", EffectiveId);
                await _inputModule.InvokeVoidAsync("disposeValidation", EffectiveId);
                
                // Dispose maxlength enforcement if it was initialized
                if (MaxLength.HasValue)
                {
                    await _inputModule.InvokeVoidAsync("disposeMaxLength", EffectiveId);
                }
                
                await _inputModule.DisposeAsync();
            }

            if (_cursorModule != null)
            {
                await _cursorModule.DisposeAsync();
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
