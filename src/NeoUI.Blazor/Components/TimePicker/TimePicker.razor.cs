using BlazorUI.Components.Button;
using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace BlazorUI.Components.TimePicker;

/// <summary>
/// A time picker component that follows the shadcn/ui design system.
/// </summary>
public partial class TimePickerBase : ComponentBase
{
    /// <summary>
    /// Indicates whether the time picker popover is currently open.
    /// </summary>
    protected bool _isOpen;
    
    /// <summary>
    /// The hour in 12-hour format (1-12).
    /// </summary>
    protected int _hour12 = 12;
    
    /// <summary>
    /// The hour in 24-hour format (0-23).
    /// </summary>
    protected int _hour24 = 0;
    
    /// <summary>
    /// The selected minute (0-59).
    /// </summary>
    protected int _minute = 0;
    
    /// <summary>
    /// The selected second (0-59).
    /// </summary>
    protected int _second = 0;
    
    /// <summary>
    /// Indicates whether the time is in PM (true) or AM (false) for 12-hour format.
    /// </summary>
    protected bool _isPM = false;
    
    /// <summary>
    /// The previous EditContext used to detect context changes.
    /// </summary>
    protected EditContext? _previousEditContext;
    
    /// <summary>
    /// The field identifier for validation integration with EditContext.
    /// </summary>
    protected FieldIdentifier _fieldIdentifier;

    /// <summary>
    /// Gets or sets the cascading EditContext for form validation integration.
    /// Cascaded from an ancestor EditForm component.
    /// </summary>
    [CascadingParameter]
    protected EditContext? EditContext { get; set; }

    /// <summary>
    /// The selected time.
    /// </summary>
    [Parameter]
    public TimeOnly? SelectedTime { get; set; }

    /// <summary>
    /// Event callback invoked when the selected time changes.
    /// </summary>
    [Parameter]
    public EventCallback<TimeOnly?> SelectedTimeChanged { get; set; }

    /// <summary>
    /// Whether to use 12-hour format. Default is false (24-hour).
    /// </summary>
    [Parameter]
    public bool Use12Hour { get; set; } = false;

    /// <summary>
    /// Whether to show seconds selector. Default is false.
    /// </summary>
    [Parameter]
    public bool ShowSeconds { get; set; } = false;

    /// <summary>
    /// Minimum allowed time.
    /// </summary>
    [Parameter]
    public TimeOnly? MinTime { get; set; }

    /// <summary>
    /// Maximum allowed time.
    /// </summary>
    [Parameter]
    public TimeOnly? MaxTime { get; set; }

    /// <summary>
    /// Minute step interval for the dropdown.
    /// Common values: 1, 5, 10, 15, 30. Default is 1.
    /// </summary>
    [Parameter]
    public int MinuteStep { get; set; } = 1;

    /// <summary>
    /// Whether to show the clock icon. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Whether the time picker is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the time is required.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Whether to show validation error messages.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; } = true;

    /// <summary>
    /// Additional CSS classes for the button.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// HTML id attribute for the component.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Name of the component for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<TimeOnly?>>? ValueExpression { get; set; }

    /// <summary>
    /// Placeholder text when no time is selected.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Pick a time";

    /// <summary>
    /// Lifecycle method called when component parameters are set.
    /// Synchronizes internal state with the SelectedTime parameter and sets up validation.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        if (SelectedTime.HasValue)
        {
            var time = SelectedTime.Value;
            _hour24 = time.Hour;
            _minute = time.Minute;
            _second = time.Second;
            
            // Convert to 12-hour format
            if (time.Hour == 0)
            {
                _hour12 = 12;
                _isPM = false;
            }
            else if (time.Hour < 12)
            {
                _hour12 = time.Hour;
                _isPM = false;
            }
            else if (time.Hour == 12)
            {
                _hour12 = 12;
                _isPM = true;
            }
            else
            {
                _hour12 = time.Hour - 12;
                _isPM = true;
            }
        }
        else
        {
            _hour12 = 12;
            _hour24 = 0;
            _minute = 0;
            _second = 0;
            _isPM = false;
        }

        // Set up field identifier for validation
        if (EditContext != null && ValueExpression != null)
        {
            if (EditContext != _previousEditContext)
            {
                _previousEditContext = EditContext;
                _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
            }
        }
    }

    /// <summary>
    /// Applies the currently selected time values, validates against min/max constraints,
    /// closes the popover, and notifies of the time change.
    /// </summary>
    protected async Task ApplyTime()
    {
        int hour;
        
        if (Use12Hour)
        {
            // Convert 12-hour to 24-hour
            hour = (_hour12 == 12) ? (_isPM ? 12 : 0) : (_isPM ? _hour12 + 12 : _hour12);
        }
        else
        {
            hour = _hour24;
        }

        var newTime = new TimeOnly(hour, _minute, ShowSeconds ? _second : 0);

        // Validate against MinTime/MaxTime
        if (MinTime.HasValue && newTime < MinTime.Value)
        {
            newTime = MinTime.Value;
        }
        if (MaxTime.HasValue && newTime > MaxTime.Value)
        {
            newTime = MaxTime.Value;
        }

        _isOpen = false;
        await SelectedTimeChanged.InvokeAsync(newTime);

        // Notify EditContext if available
        if (EditContext != null && ValueExpression != null)
        {
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    /// <summary>
    /// Handles changes to the AM/PM period selector from a change event.
    /// </summary>
    /// <param name="e">The change event arguments containing the selected period.</param>
    protected void OnPeriodChanged(ChangeEventArgs e)
    {
        _isPM = e.Value?.ToString() == "PM";
    }

    /// <summary>
    /// Handles changes to the AM/PM period selector from a select component.
    /// </summary>
    /// <param name="value">The selected period value ("AM" or "PM").</param>
    protected void OnPeriodChangedFromSelect(string? value)
    {
        _isPM = value == "PM";
    }

    /// <summary>
    /// Gets the formatted display text for the trigger button.
    /// Returns the placeholder if no time is selected, otherwise formats the time
    /// according to the 12/24-hour format and ShowSeconds settings.
    /// </summary>
    /// <returns>The formatted time string or placeholder text.</returns>
    protected string GetDisplayText()
    {
        if (!SelectedTime.HasValue)
            return Placeholder;

        var time = SelectedTime.Value;
        
        if (Use12Hour)
        {
            var hour = time.Hour % 12;
            if (hour == 0) hour = 12;
            var period = time.Hour >= 12 ? "PM" : "AM";
            return ShowSeconds 
                ? $"{hour}:{time.Minute:D2}:{time.Second:D2} {period}"
                : $"{hour}:{time.Minute:D2} {period}";
        }
        
        return ShowSeconds 
            ? $"{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}"
            : $"{time.Hour:D2}:{time.Minute:D2}";
    }

    /// <summary>
    /// Gets the CSS class names for the trigger button.
    /// Includes base styling, disabled state, placeholder styling, and custom classes.
    /// </summary>
    /// <returns>The combined CSS class string.</returns>
    protected string GetButtonClass()
    {
        var classes = new List<string> 
        { 
            "w-[280px] justify-start text-left font-normal",
            "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-colors",
            "focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring",
            "disabled:pointer-events-none disabled:opacity-50",
            "border border-input bg-background shadow-sm hover:bg-accent hover:text-accent-foreground",
            "h-9 px-4 py-2"
        };
        
        if (!SelectedTime.HasValue)
            classes.Add("text-muted-foreground");

        if (!string.IsNullOrEmpty(Class))
            classes.Add(Class);

        return ClassNames.cn(classes.ToArray());
    }

    /// <summary>
    /// Gets the HTML attributes for the trigger button.
    /// Includes disabled state, id, name, and aria-label attributes.
    /// </summary>
    /// <returns>A dictionary of HTML attributes.</returns>
    protected Dictionary<string, object> GetTriggerAttributes()
    {
        var attrs = new Dictionary<string, object>();
        
        if (Disabled)
        {
            attrs.Add("disabled", true);
        }

        if (!string.IsNullOrEmpty(Id))
        {
            attrs.Add("id", Id);
        }

        if (!string.IsNullOrEmpty(Name))
        {
            attrs.Add("name", Name);
        }
        
        attrs.Add("aria-label", "Select time");

        return attrs;
    }

    /// <summary>
    /// Gets the CSS class names for the popover content container.
    /// </summary>
    /// <returns>The CSS class string for the popover content.</returns>
    protected string GetPopoverContentClass()
    {
        return "w-auto p-4";
    }
}
