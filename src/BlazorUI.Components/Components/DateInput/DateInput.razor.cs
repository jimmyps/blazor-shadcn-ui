using BlazorUI.Components.Common;
using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Globalization;
using System.Linq.Expressions;

namespace BlazorUI.Components.DateInput;

/// <summary>
/// A date input component with masked text input and optional calendar picker.
/// Follows the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The DateInput component provides structured date input with automatic masking, formatting,
/// and optional calendar picker for selection. It integrates with Blazor's EditForm for validation.
/// </para>
/// <para>
/// Features:
/// - Real-time masked input based on date format
/// - Culture-aware date parsing and formatting
/// - Optional calendar picker (ShowPicker parameter)
/// - Format constants via DateFormats class
/// - Custom format string support
/// - Min/Max date constraints
/// - Custom date disable predicate
/// - Form validation integration
/// - UpdateOn modes (Input vs Change)
/// - Full ARIA attribute support
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;DateInput @bind-Value="birthDate" Format="@DateFormats.US" /&gt;
/// &lt;DateInput @bind-Value="startDate" Format="@DateFormats.ISO" ShowPicker="false" /&gt;
/// &lt;DateInput @bind-Value="eventDate" Culture="new CultureInfo("fr-FR")" /&gt;
/// </code>
/// </example>
public partial class DateInput : ComponentBase, IAsyncDisposable
{
    private static readonly object _firstInvalidInputIdKey = new();
    
    private IJSObjectReference? _inputModule;
    private DotNetObjectReference<DateInput>? _dotNetRef;
    private bool _jsInitialized = false;
    private IJSObjectReference? _validationModule;
    private EditContext? _previousEditContext;
    private FieldIdentifier _fieldIdentifier;
    private string? _currentErrorMessage;
    private bool _hasShownTooltip = false;
    private string? _generatedId;
    private string? _inputValue;
    private DateOnly? _calendarValue;
    private bool _pickerOpen = false;
    private bool _isInitialized = false;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets the cascaded EditContext from an EditForm.
    /// </summary>
    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    #region Value Binding

    /// <summary>
    /// Gets or sets the current date value.
    /// </summary>
    [Parameter]
    public DateOnly? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the date value changes.
    /// </summary>
    [Parameter]
    public EventCallback<DateOnly?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<DateOnly?>>? ValueExpression { get; set; }

    #endregion

    #region Format & Culture

    /// <summary>
    /// Gets or sets the date format string for input and display.
    /// Uses .NET date format patterns. If null, defaults to culture's short date pattern.
    /// See <see cref="DateFormats"/> for common format constants.
    /// </summary>
    [Parameter]
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the culture for date formatting and parsing.
    /// When null, uses CultureInfo.CurrentCulture.
    /// Works in conjunction with Format parameter for culture-aware formatting.
    /// </summary>
    [Parameter]
    public CultureInfo? Culture { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text displayed when the input is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    #endregion

    #region Picker Configuration

    /// <summary>
    /// Gets or sets whether to show the calendar picker button.
    /// When true, displays a calendar button that opens a popover with date picker.
    /// When false, input is text-only (for data entry workflows).
    /// Default: true
    /// </summary>
    [Parameter]
    public bool ShowPicker { get; set; } = true;

    #endregion

    #region Validation & Constraints

    /// <summary>
    /// Minimum selectable date.
    /// </summary>
    [Parameter]
    public DateOnly? MinDate { get; set; }

    /// <summary>
    /// Maximum selectable date.
    /// </summary>
    [Parameter]
    public DateOnly? MaxDate { get; set; }

    /// <summary>
    /// Function to determine if a specific date should be disabled.
    /// </summary>
    [Parameter]
    public Func<DateOnly, bool>? IsDateDisabled { get; set; }

    /// <summary>
    /// Minimum years offset from today for date selection.
    /// Negative values = past years, positive = future years, 0 = today.
    /// Example: -80 means 80 years ago.
    /// When set, this parameter takes precedence over MinDate, and MinDate is completely ignored.
    /// Useful for scenarios like Date of Birth where you want "5-80 years ago" without manual date calculation.
    /// </summary>
    [Parameter]
    public int? MinYearsFromToday { get; set; }

    /// <summary>
    /// Maximum years offset from today for date selection.
    /// Negative values = past years, positive = future years, 0 = today.
    /// Example: -5 means 5 years ago.
    /// When set, this parameter takes precedence over MaxDate, and MaxDate is completely ignored.
    /// Useful for scenarios like appointments where you want "today to 2 years from now" without manual date calculation.
    /// </summary>
    [Parameter]
    public int? MaxYearsFromToday { get; set; }

    #endregion

    #region Update Behavior

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

    #endregion

    #region Standard Input Properties

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
    /// Gets or sets whether the input is read-only.
    /// </summary>
    [Parameter]
    public bool Readonly { get; set; }

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
    /// Gets or sets the name of the input for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    #endregion

    #region Accessibility

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

    #endregion

    #region Form Validation

    /// <summary>
    /// Gets or sets whether to automatically show validation errors from EditContext.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; } = true;

    #endregion

    #region Additional Attributes

    /// <summary>
    /// Gets or sets additional attributes to be applied to the input element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    #endregion

    #region Private Properties

    private bool? EffectiveAriaInvalid => ShowValidationError && EditContext != null
        ? !string.IsNullOrEmpty(_currentErrorMessage)
        : AriaInvalid;

    /// <summary>
    /// Gets the effective ID, generating a unique ID if none is provided.
    /// </summary>
    private string EffectiveId
    {
        get
        {
            if (!string.IsNullOrEmpty(Id))
                return Id;

            if (_generatedId == null)
            {
                _generatedId = "date-input-" + Guid.NewGuid().ToString("N")[..6];
            }

            return _generatedId;
        }
    }

    private string CssClass => ClassNames.cn(
        "flex h-8 w-full rounded-md border border-input bg-background px-2 py-1 text-base shadow-xs",
        "placeholder:text-muted-foreground",
        "outline-none focus-visible:border-ring focus-visible:ring-[2px] focus-visible:ring-ring/50",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "aria-[invalid=true]:border-destructive aria-[invalid=true]:ring-destructive",
        "aria-[invalid=true]:focus-visible:ring-destructive/30",
        "transition-[color,box-shadow]",
        "md:text-sm",
        ShowPicker ? "pr-10" : null,
        Class
    );

    /// <summary>
    /// Gets the effective minimum date, calculated from MinYearsFromToday if set, otherwise MinDate.
    /// </summary>
    private DateOnly? EffectiveMinDate
    {
        get
        {
            if (MinYearsFromToday.HasValue)
            {
                return DateOnly.FromDateTime(DateTime.Today.AddYears(MinYearsFromToday.Value));
            }
            return MinDate;
        }
    }

    /// <summary>
    /// Gets the effective maximum date, calculated from MaxYearsFromToday if set, otherwise MaxDate.
    /// </summary>
    private DateOnly? EffectiveMaxDate
    {
        get
        {
            if (MaxYearsFromToday.HasValue)
            {
                return DateOnly.FromDateTime(DateTime.Today.AddYears(MaxYearsFromToday.Value));
            }
            return MaxDate;
        }
    }


    #endregion

    #region Helper Methods

    private CultureInfo GetEffectiveCulture() => Culture ?? CultureInfo.CurrentCulture;

    private string GetMaskPattern(string? format)
    {
        if (string.IsNullOrEmpty(format))
        {
            // Use culture's short date pattern
            format = GetEffectiveCulture().DateTimeFormat.ShortDatePattern;
        }
        
        // Convert .NET date format to mask pattern
        return format
            .Replace("yyyy", "0000")
            .Replace("yy", "00")
            .Replace("MM", "00")
            .Replace("dd", "00")
            .Replace("M", "0")
            .Replace("d", "0")
            .Replace("MMM", "AAA")    // Abbreviated month (3 letters)
            .Replace("MMMM", "AAAA"); // Full month name (flexible length)
    }

    private bool TryParseDate(string? input, out DateOnly date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(input)) return false;
        
        var culture = GetEffectiveCulture();
        var format = Format ?? culture.DateTimeFormat.ShortDatePattern;
        
        // Remove mask characters
        var cleaned = input.Replace("_", "").Trim();
        
        // Try exact format first
        if (DateOnly.TryParseExact(cleaned, format, culture, DateTimeStyles.None, out date))
            return true;
        
        // Fallback to culture-aware parsing
        return DateOnly.TryParse(cleaned, culture, DateTimeStyles.None, out date);
    }

    private string GetEffectivePlaceholder()
    {
        if (!string.IsNullOrEmpty(Placeholder))
            return Placeholder;
        
        var format = Format ?? GetEffectiveCulture().DateTimeFormat.ShortDatePattern;
        
        // Convert format to placeholder (yyyy -> YYYY, MM -> MM, etc.)
        return format
            .Replace("yyyy", "YYYY")
            .Replace("yy", "YY")
            .Replace("MM", "MM")
            .Replace("dd", "DD")
            .Replace("M", "M")
            .Replace("d", "D")
            .Replace("MMM", "MMM")
            .Replace("MMMM", "MMMM");
    }

    #endregion

    #region Event Handlers

    private async Task OnInputValueChanged(string? value)
    {
        _inputValue = value;
        
        // Try to parse the input value
        if (TryParseDate(value, out var parsedDate))
        {
            if (Value != parsedDate)
            {
                Value = parsedDate;
                _calendarValue = parsedDate;
                await ValueChanged.InvokeAsync(Value);
                
                if (ShowValidationError && EditContext != null && ValueExpression != null)
                {
                    EditContext.NotifyFieldChanged(_fieldIdentifier);
                }
            }
        }
        else if (string.IsNullOrWhiteSpace(value))
        {
            // Clear the value
            if (Value != null)
            {
                Value = null;
                _calendarValue = null;
                await ValueChanged.InvokeAsync(Value);
                
                if (ShowValidationError && EditContext != null && ValueExpression != null)
                {
                    EditContext.NotifyFieldChanged(_fieldIdentifier);
                }
            }
        }
    }

    private async Task OnCalendarValueChanged(DateOnly? date)
    {
        _calendarValue = date;
        Value = date;
        
        // Update input value
        if (date.HasValue)
        {
            var culture = GetEffectiveCulture();
            var format = Format ?? culture.DateTimeFormat.ShortDatePattern;
            _inputValue = date.Value.ToString(format, culture);
        }
        else
        {
            _inputValue = null;
        }
        
        // Close picker
        _pickerOpen = false;
        
        await ValueChanged.InvokeAsync(Value);
        
        if (ShowValidationError && EditContext != null && ValueExpression != null)
        {
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    #endregion

    #region Lifecycle Methods

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Update input value when Value changes externally
        if (Value.HasValue)
        {
            var culture = GetEffectiveCulture();
            var format = Format ?? culture.DateTimeFormat.ShortDatePattern;
            var formattedValue = Value.Value.ToString(format, culture);
            
            if (_inputValue != formattedValue)
            {
                _inputValue = formattedValue;
            }
            
            if (_calendarValue != Value)
            {
                _calendarValue = Value;
            }
        }
        else
        {
            if (_inputValue != null)
            {
                _inputValue = null;
            }
            if (_calendarValue != null)
            {
                _calendarValue = null;
            }
        }

        if (ShowValidationError && ValueExpression != null)
        {
            _fieldIdentifier = FieldIdentifier.Create(ValueExpression);

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
                // Import the input module for event handling
                _inputModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/input.js");

                // Create DotNetObjectReference for callbacks
                _dotNetRef = DotNetObjectReference.Create(this);

                _jsInitialized = true;

                if (ShowValidationError)
                {
                    _validationModule = _inputModule;
                }

                _isInitialized = true;
            }
            catch (JSException)
            {
                // JS modules not available, component will work without advanced features
            }
        }

        if (ShowValidationError && _validationModule != null)
        {
            await UpdateValidationDisplayAsync();
        }
    }

    #endregion

    #region Validation

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
            StateHasChanged();
        });
    }

    private async Task UpdateValidationDisplayAsync()
    {
        if (EditContext == null || _validationModule == null)
            return;

        try
        {
            var messages = EditContext.GetValidationMessages(_fieldIdentifier).ToList();
            var errorMessage = messages.FirstOrDefault();

            if (errorMessage != _currentErrorMessage)
            {
                _currentErrorMessage = errorMessage;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    // Determine if this is the first invalid input for this EditContext
                    string? firstInvalidId = null;
                    if (EditContext.Properties.TryGetValue(_firstInvalidInputIdKey, out var value))
                    {
                        firstInvalidId = value as string;
                    }
                    var isFirstInvalid = firstInvalidId == null;
                    
                    if (isFirstInvalid)
                    {
                        EditContext.Properties[_firstInvalidInputIdKey] = EffectiveId;
                    }

                    if (isFirstInvalid && !_hasShownTooltip)
                    {
                        await _validationModule.InvokeVoidAsync("showValidationError", EffectiveId, errorMessage);
                        _hasShownTooltip = true;
                    }
                }
                else
                {
                    await _validationModule.InvokeVoidAsync("clearValidationError", EffectiveId);
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

    #endregion

    #region Dispose

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsInitialized && _inputModule != null)
            {
                await _inputModule.DisposeAsync();
            }

            _dotNetRef?.Dispose();
        }
        catch (JSDisconnectedException)
        {
            // Ignore - this happens during hot reload or when navigating away
        }

        DetachValidationStateChangedListener();
    }

    #endregion
}
