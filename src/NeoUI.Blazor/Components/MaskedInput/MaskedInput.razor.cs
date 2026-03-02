using BlazorUI.Components.Common;
using BlazorUI.Components.Utilities;
using BlazorUI.Components.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace BlazorUI.Components.MaskedInput;

/// <summary>
/// A masked input component that follows the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The MaskedInput component provides structured input with automatic masking and formatting.
/// It follows WCAG 2.1 AA standards for accessibility and integrates with Blazor's data binding system.
/// </para>
/// <para>
/// Features:
/// - Real-time masking as user types
/// - Auto-skip literal characters
/// - Cursor position management
/// - Backspace/Delete support
/// - Paste support with automatic formatting
/// - Form validation integration
/// - Common preset masks (phone, SSN, credit card, etc.)
/// - Full ARIA attribute support
/// - RTL and dark mode compatible
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;MaskedInput @bind-Value="phoneNumber" Mask="(000) 000-0000" Placeholder="Enter phone number" /&gt;
/// &lt;MaskedInput @bind-Value="ssn" Mask="@MaskedInput.Masks.SSN" ShowMask="true" /&gt;
/// &lt;MaskedInput @bind-Value="creditCard" Mask="@MaskedInput.Masks.CreditCard" /&gt;
/// </code>
/// </example>
public partial class MaskedInput : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// JavaScript module for input event handling and validation.
    /// </summary>
    private IJSObjectReference? _inputModule;
    
    /// <summary>
    /// DotNet object reference for JavaScript callbacks.
    /// </summary>
    private DotNetObjectReference<MaskedInput>? _dotNetRef;
    
    /// <summary>
    /// Tracks whether JavaScript input module has been initialized.
    /// </summary>
    private bool _jsInitialized = false;
    
    /// <summary>
    /// Validation behavior handler for EditContext integration.
    /// </summary>
    private InputValidationBehavior? _validationBehavior;
    
    /// <summary>
    /// JavaScript module for mask formatting and cursor management.
    /// </summary>
    private IJSObjectReference? _maskModule;
    
    /// <summary>
    /// Auto-generated ID when user doesn't provide one.
    /// </summary>
    private string? _generatedId;
    
    /// <summary>
    /// Tracks the last raw (unmasked) value to prevent duplicate updates.
    /// </summary>
    private string? _lastRawValue;
    
    /// <summary>
    /// DotNet object reference for mask-specific JavaScript callbacks.
    /// </summary>
    private DotNetObjectReference<MaskedInput>? _maskDotNetRef;
    
    /// <summary>
    /// Tracks whether mask module has been initialized.
    /// </summary>
    private bool _isInitialized = false;

    /// <summary>
    /// JavaScript runtime for invoking JavaScript functions.
    /// </summary>
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
    /// - Input: Updates value immediately on every keystroke
    /// - Change: Updates value only when input loses focus (default)
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
    /// Gets or sets the current value of the input (unmasked).
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the input value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the mask pattern.
    /// </summary>
    /// <remarks>
    /// Pattern characters:
    /// - '0' = Digit (0-9)
    /// - '9' = Digit or space
    /// - 'A' = Letter (a-Z)
    /// - 'a' = Letter or space
    /// - '*' = Alphanumeric
    /// - Any other character is a literal
    /// </remarks>
    [Parameter]
    public string Mask { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the placeholder character for mask positions.
    /// </summary>
    [Parameter]
    public char MaskChar { get; set; } = '_';

    /// <summary>
    /// Gets or sets whether to show the mask when input is empty.
    /// </summary>
    [Parameter]
    public bool ShowMask { get; set; } = true;

    /// <summary>
    /// Gets or sets the placeholder text displayed when the input is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets whether the input is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the input is required.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the name of the input for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets whether the input is read-only.
    /// </summary>
    [Parameter]
    public bool Readonly { get; set; }

    /// <summary>
    /// Gets or sets the input mode hint for mobile keyboards.
    /// </summary>
    [Parameter]
    public string? InputMode { get; set; } = "text";

    /// <summary>
    /// Gets or sets whether the input should be auto-focused when the page loads.
    /// </summary>
    [Parameter]
    public bool Autofocus { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the input.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute for the input element.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the input.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element that describes the input.
    /// </summary>
    [Parameter]
    public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets whether the input value is invalid.
    /// </summary>
    [Parameter]
    public bool? AriaInvalid { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically show validation errors from EditContext.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<string?>>? ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets additional attributes to be applied to the input element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Common mask patterns for convenience.
    /// </summary>
    public static class Masks
    {
        /// <summary>US Phone number: (000) 000-0000</summary>
        public const string PhoneUS = "(000) 000-0000";
        
        /// <summary>International Phone: +0 (000) 000-0000</summary>
        public const string PhoneInternational = "+0 (000) 000-0000";
        
        /// <summary>Social Security Number: 000-00-0000</summary>
        public const string SSN = "000-00-0000";
        
        /// <summary>Credit Card: 0000 0000 0000 0000</summary>
        public const string CreditCard = "0000 0000 0000 0000";
        
        /// <summary>Date: 00/00/0000</summary>
        public const string Date = "00/00/0000";
        
        /// <summary>Time: 00:00</summary>
        public const string Time = "00:00";
        
        /// <summary>ZIP Code: 00000</summary>
        public const string ZIP = "00000";
        
        /// <summary>ZIP+4: 00000-0000</summary>
        public const string ZIP4 = "00000-0000";
    }

    /// <summary>
    /// Gets the effective AriaInvalid value from validation behavior or parameter.
    /// </summary>
    private bool? EffectiveAriaInvalid => 
        _validationBehavior?.EffectiveAriaInvalid ?? AriaInvalid;

    /// <summary>
    /// Gets the effective ID, generating a unique ID if none is provided.
    /// </summary>
    /// <remarks>
    /// This ensures JavaScript can always reference the element, even when Id is not explicitly set.
    /// The generated ID follows the pattern: masked-input-{6-character-guid}.
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
                _generatedId = "masked-input-" + Guid.NewGuid().ToString("N")[..6];
            }

            return _generatedId;
        }
    }

    /// <summary>
    /// Gets the computed CSS classes for the input element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "flex h-8 w-full rounded-md border border-input bg-background px-2 py-1 text-base shadow-xs",
        "placeholder:text-muted-foreground",
        "outline-none focus-visible:border-ring focus-visible:ring-[2px] focus-visible:ring-ring/50",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "aria-[invalid=true]:border-destructive aria-[invalid=true]:ring-destructive",
        "aria-[invalid=true]:focus-visible:ring-destructive/30",
        "transition-[color,box-shadow]",
        "md:text-sm",
        Class
    );

    /// <summary>
    /// Gets the effective name attribute, falling back to Id if Name is not specified.
    /// </summary>
    private string? EffectiveName => Name ?? Id;

    /// <summary>
    /// Gets the effective placeholder, hidden when ShowMask is enabled.
    /// </summary>
    private string? EffectivePlaceholder => !ShowMask || string.IsNullOrEmpty(Mask) ? Placeholder : null;

    /// <summary>
    /// Gets the current masked display value shown to the user.
    /// </summary>
    private string CurrentDisplayValue
    {
        get
        {
            if (string.IsNullOrEmpty(Mask))
                return Value ?? string.Empty;

            if (string.IsNullOrEmpty(Value) && ShowMask)
                return GenerateEmptyMask();

            return ApplyMask(Value ?? string.Empty);
        }
    }

    /// <summary>
    /// Generates an empty mask display with placeholder characters.
    /// </summary>
    private string GenerateEmptyMask()
    {
        var result = new char[Mask.Length];
        for (int i = 0; i < Mask.Length; i++)
        {
            result[i] = IsLiteral(Mask[i]) ? Mask[i] : MaskChar;
        }
        return new string(result);
    }

    /// <summary>
    /// Determines if a mask character is a literal (not a placeholder).
    /// </summary>
    private bool IsLiteral(char maskChar)
    {
        return maskChar != '0' && maskChar != '9' && maskChar != 'A' && maskChar != 'a' && maskChar != '*';
    }

    /// <summary>
    /// Checks if an input character matches the mask pattern at the current position.
    /// </summary>
    private bool MatchesMaskChar(char input, char maskChar)
    {
        return maskChar switch
        {
            '0' => char.IsDigit(input),
            '9' => char.IsDigit(input) || input == ' ',
            'A' => char.IsLetter(input),
            'a' => char.IsLetter(input) || input == ' ',
            '*' => char.IsLetterOrDigit(input),
            _ => input == maskChar // Literal character
        };
    }

    /// <summary>
    /// Applies the mask pattern to raw input value.
    /// </summary>
    private string ApplyMask(string rawValue)
    {
        if (string.IsNullOrEmpty(Mask))
            return rawValue;

        var result = new List<char>();
        int rawIndex = 0;

        for (int maskIndex = 0; maskIndex < Mask.Length; maskIndex++)
        {
            char maskChar = Mask[maskIndex];

            if (IsLiteral(maskChar))
            {
                result.Add(maskChar);
                // Skip matching literal in raw value
                if (rawIndex < rawValue.Length && rawValue[rawIndex] == maskChar)
                    rawIndex++;
            }
            else
            {
                if (rawIndex < rawValue.Length)
                {
                    char rawChar = rawValue[rawIndex];
                    if (MatchesMaskChar(rawChar, maskChar))
                    {
                        result.Add(rawChar);
                        rawIndex++;
                    }
                    else
                    {
                        // Invalid character, try to skip it
                        rawIndex++;
                        maskIndex--; // Retry current mask position
                    }
                }
                else if (ShowMask)
                {
                    result.Add(MaskChar);
                }
            }
        }

        return new string(result.ToArray());
    }

    /// <summary>
    /// Extracts the raw (unmasked) value from a masked input string.
    /// </summary>
    private string ExtractRawValue(string maskedValue)
    {
        if (string.IsNullOrEmpty(Mask))
            return maskedValue;

        var result = new List<char>();
        int maskIndex = 0;

        foreach (char c in maskedValue)
        {
            if (maskIndex >= Mask.Length)
                break;

            char maskChar = Mask[maskIndex];

            if (IsLiteral(maskChar))
            {
                if (c == maskChar)
                    maskIndex++;
                // Skip literal characters
            }
            else
            {
                if (c != MaskChar && MatchesMaskChar(c, maskChar))
                {
                    result.Add(c);
                }
                maskIndex++;
            }
        }

        return new string(result.ToArray());
    }

    /// <summary>
    /// Handles input events when JavaScript is unavailable (fallback).
    /// </summary>
    private async Task HandleInput(ChangeEventArgs args)
    {
        // When JS module is loaded, it handles all the masking
        // This is a fallback for when JS is not available
        if (_jsInitialized)
        {
            return; // JS handles everything
        }

        if (UpdateOn == InputUpdateMode.Input)
        {
            // Fallback: Basic masking without advanced features
            var maskedValue = args.Value?.ToString() ?? string.Empty;
            var rawValue = ExtractRawValue(maskedValue);

            if (rawValue != _lastRawValue)
            {
                _lastRawValue = rawValue;
                Value = string.IsNullOrEmpty(rawValue) ? null : rawValue;

                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }

                if (_validationBehavior != null)
                {
                    await _validationBehavior.NotifyFieldChangedAsync();
                }
            }
        }
    }

    /// <summary>
    /// Handles change events when JavaScript is unavailable (fallback).
    /// </summary>
    private async Task HandleChange(ChangeEventArgs args)
    {
        if (_jsInitialized)
        {
            return; // JS handles everything
        }

        if (UpdateOn == InputUpdateMode.Change)
        {
            var maskedValue = args.Value?.ToString() ?? string.Empty;
            var rawValue = ExtractRawValue(maskedValue);

            _lastRawValue = rawValue;
            Value = string.IsNullOrEmpty(rawValue) ? null : rawValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (_validationBehavior != null)
            {
                await _validationBehavior.NotifyFieldChangedAsync();
            }
        }
    }

    /// <summary>
    /// Called from JavaScript when input value changes.
    /// This is invoked based on UpdateOn mode and debounce settings.
    /// </summary>
    [JSInvokable]
    public async Task OnInputChanged(string? value)
    {
        // For MaskedInput, value is already the raw extracted value from the mask
        if (value != _lastRawValue)
        {
            _lastRawValue = value;
            
            // Update local state
            Value = string.IsNullOrEmpty(value) ? null : value;

            // Notify parent component
            await ValueChanged.InvokeAsync(Value);

            // Trigger EditContext validation if applicable
            if (_validationBehavior != null)
            {
                await _validationBehavior.NotifyFieldChangedAsync();
            }
        }
    }

    /// <summary>
    /// Calculates the new cursor position, skipping literal characters in the mask.
    /// </summary>
    private int CalculateNewCursorPosition(string maskedValue, int currentPos)
    {
        // Skip over literal characters
        while (currentPos < maskedValue.Length && currentPos < Mask.Length && 
               IsLiteral(Mask[currentPos]) && maskedValue[currentPos] == Mask[currentPos])
        {
            currentPos++;
        }
        return currentPos;
    }

    /// <summary>
    /// Handles keyboard events for navigation and editing.
    /// </summary>
    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        // Allow navigation and editing keys
        if (args.Key == "Backspace" || args.Key == "Delete" || 
            args.Key == "ArrowLeft" || args.Key == "ArrowRight" ||
            args.Key == "Home" || args.Key == "End" || args.Key == "Tab")
        {
            return;
        }

        // Let the default input handling work
        await Task.CompletedTask;
    }

    /// <summary>
    /// Handles paste events, delegated to oninput event handler.
    /// </summary>
    private async Task HandlePaste(ClipboardEventArgs args)
    {
        // Paste will be handled by oninput event
        await Task.CompletedTask;
    }

    /// <summary>
    /// Initializes the component and sets up validation behavior if enabled.
    /// </summary>
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

    /// <summary>
    /// Updates validation behavior when parameters change and subscribes to EditContext events.
    /// </summary>
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

    /// <summary>
    /// Handles EditContext validation state changes and updates the component display.
    /// </summary>
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

    /// <summary>
    /// Initializes JavaScript modules after the component first renders.
    /// Sets up input event handling, mask formatting, and validation.
    /// </summary>
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

                // Initialize input event handling with UpdateOn mode and debounce
                // Use EffectiveId which always has a value (user-provided or generated)
                await _inputModule.InvokeVoidAsync(
                    "initializeInput",
                    EffectiveId,
                    UpdateOn.ToString().ToLower(),
                    DebounceDelay,
                    _dotNetRef
                );

                _jsInitialized = true;

                // Initialize validation if ShowValidationError is enabled
                if (ShowValidationError && EditContext != null && ValueExpression != null)
                {
                    await _inputModule.InvokeVoidAsync("initializeValidation", EffectiveId, UpdateOn.ToString().ToLower());
                }

                // Load mask module
                _maskModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/masked-input.js");

                // Initialize masked input with JavaScript
                if (_maskModule != null && !string.IsNullOrEmpty(Mask))
                {
                    _maskDotNetRef = DotNetObjectReference.Create(this);
                    
                    // Pass UpdateOn setting to JavaScript ('input' or 'change')
                    var updateOnMode = UpdateOn == InputUpdateMode.Input ? "input" : "change";
                    
                    await _maskModule.InvokeVoidAsync("initializeMaskedInput", 
                        EffectiveId, Mask, MaskChar, _maskDotNetRef, updateOnMode);
                    _isInitialized = true;
                }

                // Apply initial validation state after first render
                if (ShowValidationError && _validationBehavior != null)
                {
                    await _validationBehavior.UpdateValidationDisplayAsync();
                }
            }
            catch (JSException)
            {
                // JS modules not available, component will work without advanced features
            }
        }
    }

    /// <summary>
    /// Called from JavaScript when the masked value changes.
    /// Receives the raw (unmasked) value and updates the component state.
    /// </summary>
    [JSInvokable]
    public async Task OnValueChanged(string? rawValue)
    {
        if (rawValue != _lastRawValue)
        {
            _lastRawValue = rawValue;
            Value = string.IsNullOrEmpty(rawValue) ? null : rawValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            if (_validationBehavior != null)
            {
                await _validationBehavior.NotifyFieldChangedAsync();
            }

            StateHasChanged();
        }
    }

    /// <summary>
    /// Disposes JavaScript modules, event handlers, and object references.
    /// </summary>
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
                // Dispose input event handling and validation tracking
                await _inputModule.InvokeVoidAsync("disposeInput", EffectiveId);
                await _inputModule.InvokeVoidAsync("disposeValidation", EffectiveId);
                
                await _inputModule.DisposeAsync();
            }

            _dotNetRef?.Dispose();
        }
        catch (JSDisconnectedException)
        {
            // Ignore - this happens during hot reload or when navigating away
        }
        
        // Clean up masked input JS
        if (_maskModule != null && _isInitialized)
        {
            try
            {
                await _maskModule.InvokeVoidAsync("disposeMaskedInput", EffectiveId);
            }
            catch (JSException)
            {
                // Ignore
            }
        }

        _maskDotNetRef?.Dispose();
        
        if (_maskModule != null)
        {
            try
            {
                await _maskModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore
            }
        }
    }
}
