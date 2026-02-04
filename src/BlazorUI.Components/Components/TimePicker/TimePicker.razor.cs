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
    protected bool _isOpen;
    protected int _hour12 = 12;
    protected int _hour24 = 0;
    protected int _minute = 0;
    protected int _second = 0;
    protected bool _isPM = false;
    protected EditContext? _previousEditContext;
    protected FieldIdentifier _fieldIdentifier;

    [CascadingParameter]
    protected EditContext? EditContext { get; set; }

    /// <summary>
    /// The selected time.
    /// </summary>
    [Parameter]
    public TimeSpan? Time { get; set; }

    /// <summary>
    /// Event callback invoked when the selected time changes.
    /// </summary>
    [Parameter]
    public EventCallback<TimeSpan?> TimeChanged { get; set; }

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
    public TimeSpan? MinTime { get; set; }

    /// <summary>
    /// Maximum allowed time.
    /// </summary>
    [Parameter]
    public TimeSpan? MaxTime { get; set; }

    /// <summary>
    /// Step interval in minutes. Default is 1.
    /// </summary>
    [Parameter]
    public int Step { get; set; } = 1;

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
    public Expression<Func<TimeSpan?>>? ValueExpression { get; set; }

    /// <summary>
    /// Placeholder text when no time is selected.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Pick a time";

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        if (Time.HasValue)
        {
            var time = Time.Value;
            _hour24 = time.Hours;
            _minute = time.Minutes;
            _second = time.Seconds;
            
            // Convert to 12-hour format
            if (time.Hours == 0)
            {
                _hour12 = 12;
                _isPM = false;
            }
            else if (time.Hours < 12)
            {
                _hour12 = time.Hours;
                _isPM = false;
            }
            else if (time.Hours == 12)
            {
                _hour12 = 12;
                _isPM = true;
            }
            else
            {
                _hour12 = time.Hours - 12;
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

        var newTime = new TimeSpan(hour, _minute, ShowSeconds ? _second : 0);

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
        await TimeChanged.InvokeAsync(newTime);

        // Notify EditContext if available
        if (EditContext != null && ValueExpression != null)
        {
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    protected void OnPeriodChanged(ChangeEventArgs e)
    {
        _isPM = e.Value?.ToString() == "PM";
    }

    protected string GetDisplayText()
    {
        if (!Time.HasValue)
            return Placeholder;

        var time = Time.Value;
        
        if (Use12Hour)
        {
            var format = ShowSeconds ? "h\\:mm\\:ss tt" : "h\\:mm tt";
            var hour = time.Hours % 12;
            if (hour == 0) hour = 12;
            var period = time.Hours >= 12 ? "PM" : "AM";
            return ShowSeconds 
                ? $"{hour}:{time.Minutes:D2}:{time.Seconds:D2} {period}"
                : $"{hour}:{time.Minutes:D2} {period}";
        }
        
        var format24 = ShowSeconds ? "hh\\:mm\\:ss" : "hh\\:mm";
        return ShowSeconds 
            ? $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}"
            : $"{time.Hours:D2}:{time.Minutes:D2}";
    }

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
        
        if (!Time.HasValue)
            classes.Add("text-muted-foreground");

        if (!string.IsNullOrEmpty(Class))
            classes.Add(Class);

        return ClassNames.cn(classes.ToArray());
    }

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

    protected string GetPopoverContentClass()
    {
        return "w-auto p-4";
    }

    protected string GetSelectClass()
    {
        return ClassNames.cn(
            "flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm",
            "ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
            "disabled:cursor-not-allowed disabled:opacity-50"
        );
    }
}
