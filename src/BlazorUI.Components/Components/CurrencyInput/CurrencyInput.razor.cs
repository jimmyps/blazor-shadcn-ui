using BlazorUI.Components.Common;
using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;
using System.Linq.Expressions;

namespace BlazorUI.Components.CurrencyInput;

/// <summary>
/// A currency input component that follows the shadcn/ui design system.
/// </summary>
/// <typeparam name="TValue">The type of numeric value (decimal or double).</typeparam>
/// <remarks>
/// <para>
/// The CurrencyInput component provides a customizable, accessible currency input that supports
/// locale-aware formatting. It follows WCAG 2.1 AA standards for accessibility and integrates 
/// with Blazor's data binding system.
/// </para>
/// <para>
/// Features:
/// - Generic type support for decimal and double
/// - Currency symbol formatting based on culture
/// - Culture-aware decimal separator handling
/// - Form submission support via name attribute
/// - Input validation (required, min, max)
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
/// &lt;CurrencyInput TValue="decimal" @bind-Value="price" Currency="USD" /&gt;
///
/// &lt;CurrencyInput TValue="decimal" @bind-Value="amount" Currency="EUR" Culture="de-DE" /&gt;
///
/// &lt;CurrencyInput TValue="double" @bind-Value="cost" Currency="GBP" ShowCurrencySymbol="false" /&gt;
/// </code>
/// </example>
public partial class CurrencyInput<TValue> : ComponentBase, IAsyncDisposable
{
    private static string? _firstInvalidInputId = null;
    
    // Cache for currency definitions to avoid repeated lookups
    private CurrencyDefinition? _cachedCurrency;
    private string? _cachedCurrencyCode;
    
    private IJSObjectReference? _validationModule;
    private EditContext? _previousEditContext;
    private FieldIdentifier _fieldIdentifier;
    private string? _currentErrorMessage;
    private bool _hasShownTooltip = false;
    private ElementReference _inputElement;
    private string? _editingValue;
    private bool _isFocused;

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
    /// Gets or sets the currency code (e.g., USD, EUR, GBP, JPY).
    /// </summary>
    /// <remarks>
    /// Defaults to "USD". Used to determine the currency symbol and formatting.
    /// </remarks>
    [Parameter]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the culture code for formatting (e.g., en-US, de-DE, fr-FR).
    /// </summary>
    /// <remarks>
    /// When null, uses the current culture. Determines decimal separators and number formatting.
    /// </remarks>
    [Parameter]
    public string? Culture { get; set; }

    /// <summary>
    /// Gets or sets whether to show the currency symbol.
    /// </summary>
    /// <remarks>
    /// When true, displays the currency symbol (e.g., $, €, £) in the formatted value.
    /// Defaults to true.
    /// </remarks>
    [Parameter]
    public bool ShowCurrencySymbol { get; set; } = true;

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
    /// Gets or sets the minimum value for validation.
    /// </summary>
    /// <remarks>
    /// When set, values less than this minimum will be clamped to this value.
    /// Can be specified as a string (e.g., "10", "0.01").
    /// </remarks>
    [Parameter]
    public string? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for validation.
    /// </summary>
    /// <remarks>
    /// When set, values greater than this maximum will be clamped to this value.
    /// Can be specified as a string (e.g., "1000", "999.99").
    /// </remarks>
    [Parameter]
    public string? Max { get; set; }

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
    /// &lt;CurrencyInput TValue="decimal" Id="price"
    ///        @bind-Value="model.Price"
    ///        ShowValidationError="true" /&gt;
    /// &lt;ValidationMessage For="@(() => model.Price)" /&gt;
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
    private bool? EffectiveAriaInvalid => ShowValidationError && EditContext != null
        ? !string.IsNullOrEmpty(_currentErrorMessage)
        : AriaInvalid;

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
    /// Gets the currency definition for the current currency code.
    /// </summary>
    private CurrencyDefinition CurrencyDefinition
    {
        get
        {
            // Use cached value if currency code hasn't changed
            if (_cachedCurrency != null && _cachedCurrencyCode == Currency)
            {
                return _cachedCurrency;
            }

            _cachedCurrency = CurrencyCatalog.GetCurrency(Currency);
            _cachedCurrencyCode = Currency;
            return _cachedCurrency;
        }
    }

    /// <summary>
    /// Gets the culture info to use for formatting.
    /// </summary>
    private CultureInfo GetCultureInfo()
    {
        // If Culture parameter is explicitly set, use it
        if (!string.IsNullOrEmpty(Culture))
        {
            try
            {
                return CultureInfo.GetCultureInfo(Culture);
            }
            catch (CultureNotFoundException)
            {
                // Fall back to currency's default culture if specified culture is invalid
            }
        }
        
        // Use the currency's default culture
        try
        {
            return CultureInfo.GetCultureInfo(CurrencyDefinition.CultureName);
        }
        catch (CultureNotFoundException)
        {
            // Final fallback to current culture
            return CultureInfo.CurrentCulture;
        }
    }

    /// <summary>
    /// Gets the current value as a formatted string for display.
    /// </summary>
    private string? CurrentValueAsString
    {
        get
        {
            // When focused, show the raw editing value
            if (_isFocused && _editingValue != null)
                return _editingValue;

            if (Value == null)
                return null;

            try
            {
                var numericValue = Convert.ToDecimal(Value);
                var currencyDef = CurrencyDefinition;
                
                if (ShowCurrencySymbol)
                {
                    // Format with currency symbol and correct decimal places
                    var cultureInfo = GetCultureInfo();
                    var numberFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
                    
                    // Override with our currency definition values
                    numberFormat.CurrencySymbol = currencyDef.Symbol;
                    numberFormat.CurrencyDecimalDigits = currencyDef.DecimalPlaces;
                    
                    // Handle symbol positioning (for currencies like VND that have symbol after)
                    if (!currencyDef.SymbolBefore)
                    {
                        numberFormat.CurrencyPositivePattern = 1; // n $
                        numberFormat.CurrencyNegativePattern = 1; // -n $
                    }
                    
                    return numericValue.ToString("C", numberFormat);
                }
                else
                {
                    // Format without symbol, using currency's decimal places
                    var formatString = $"F{currencyDef.DecimalPlaces}";
                    return numericValue.ToString(formatString, GetCultureInfo());
                }
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Handles the input event (fired on every keystroke).
    /// </summary>
    private async Task HandleInput(ChangeEventArgs args)
    {
        var stringValue = args.Value?.ToString();
        
        // Store raw input value while editing
        _editingValue = stringValue;
        
        // Only update value if UpdateOn is set to Input
        if (UpdateOn == InputUpdateMode.Input)
        {
            var parsedValue = TryParseValue(stringValue);
            Value = parsedValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(parsedValue);
            }

            // Notify EditContext of field change to trigger validation
            if (ShowValidationError && EditContext != null && ValueExpression != null)
            {
                EditContext.NotifyFieldChanged(_fieldIdentifier);
            }
        }
    }

    /// <summary>
    /// Handles the change event (fired when input loses focus).
    /// </summary>
    private async Task HandleChange(ChangeEventArgs args)
    {
        var stringValue = args.Value?.ToString();
        
        // Update value on blur if UpdateOn is set to Change
        if (UpdateOn == InputUpdateMode.Change)
        {
            // Use the editing value if available (more reliable than args.Value)
            var valueToUse = !string.IsNullOrEmpty(_editingValue) ? _editingValue : stringValue;
            var parsedValue = TryParseValue(valueToUse);
            Value = parsedValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(parsedValue);
            }

            // Notify EditContext of field change to trigger validation
            if (ShowValidationError && EditContext != null && ValueExpression != null)
            {
                EditContext.NotifyFieldChanged(_fieldIdentifier);
            }
        }
    }

    /// <summary>
    /// Handles the focus event.
    /// </summary>
    private void HandleFocus()
    {
        _isFocused = true;
        
        // Initialize editing value with current unformatted value (full precision)
        if (Value != null)
        {
            try
            {
                var numericValue = Convert.ToDecimal(Value);
                // Use "G" format to preserve all significant digits (e.g., 100.2378 stays as 100.2378)
                _editingValue = numericValue.ToString("G", CultureInfo.InvariantCulture);
            }
            catch
            {
                _editingValue = null;
            }
        }
        else
        {
            _editingValue = null;
        }
    }

    /// <summary>
    /// Handles the blur event.
    /// </summary>
    private void HandleBlur()
    {
        // Clear editing state to restore formatted display
        // This is critical because @onchange doesn't fire if the value didn't change,
        // but we still need to restore the formatted value on blur
        _isFocused = false;
        _editingValue = null;
        
        // Force re-render to show formatted value
        StateHasChanged();
    }

    /// <summary>
    /// Tries to parse the string value to the target numeric type, removing currency symbols and formatting.
    /// </summary>
    private TValue? TryParseValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var cultureInfo = GetCultureInfo();
        var currencyDef = CurrencyDefinition;
        var targetType = typeof(TValue);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Remove currency symbols and group separators
        var cleanValue = value;
        
        // Remove the currency symbol
        cleanValue = cleanValue.Replace(currencyDef.Symbol, "");
        
        // Remove culture-specific currency symbol (fallback)
        cleanValue = cleanValue.Replace(cultureInfo.NumberFormat.CurrencySymbol, "");
        
        // Remove group separators (thousands separators)
        cleanValue = cleanValue.Replace(cultureInfo.NumberFormat.CurrencyGroupSeparator, "")
            .Replace(cultureInfo.NumberFormat.NumberGroupSeparator, "")
            .Trim();

        try
        {
            if (underlyingType == typeof(decimal))
            {
                if (decimal.TryParse(cleanValue, NumberStyles.Number, cultureInfo, out var result))
                {
                    // Apply min/max validation - parse from string
                    if (!string.IsNullOrEmpty(Min) && decimal.TryParse(Min, NumberStyles.Number, CultureInfo.InvariantCulture, out var minValue))
                    {
                        if (result < minValue)
                            result = minValue;
                    }
                    
                    if (!string.IsNullOrEmpty(Max) && decimal.TryParse(Max, NumberStyles.Number, CultureInfo.InvariantCulture, out var maxValue))
                    {
                        if (result > maxValue)
                            result = maxValue;
                    }
                    
                    return (TValue)(object)result;
                }
            }
            else if (underlyingType == typeof(double))
            {
                if (double.TryParse(cleanValue, NumberStyles.Number, cultureInfo, out var result))
                {
                    // Apply min/max validation - parse from string
                    if (!string.IsNullOrEmpty(Min) && double.TryParse(Min, NumberStyles.Number, CultureInfo.InvariantCulture, out var minValue))
                    {
                        if (result < minValue)
                            result = minValue;
                    }
                    
                    if (!string.IsNullOrEmpty(Max) && double.TryParse(Max, NumberStyles.Number, CultureInfo.InvariantCulture, out var maxValue))
                    {
                        if (result > maxValue)
                            result = maxValue;
                    }
                    
                    return (TValue)(object)result;
                }
            }
        }
        catch (OverflowException)
        {
            // Value is too large or too small for the target type
        }
        catch (FormatException)
        {
            // Value is not in a valid format
        }

        return default;
    }

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

                    // Show tooltip and focus only for the first invalid input
                    if (isFirstInvalid && !_hasShownTooltip)
                    {
                        await _validationModule.InvokeVoidAsync("showValidationError", Id, errorMessage);
                        _hasShownTooltip = true;
                    }
                }
                else
                {
                    // Clear validation error
                    await _validationModule.InvokeVoidAsync("clearValidationError", Id);
                    
                    // Reset first invalid tracking if this was the first invalid input
                    if (_firstInvalidInputId == Id)
                    {
                        _firstInvalidInputId = null;
                    }
                }
            }
        }
        catch (JSException)
        {
            // Ignore JS errors, validation will still work via aria-invalid
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
        DetachValidationStateChangedListener();
        
        if (_validationModule != null)
        {
            try
            {
                await _validationModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore - this happens during hot reload or when navigating away
            }
        }
    }
}
