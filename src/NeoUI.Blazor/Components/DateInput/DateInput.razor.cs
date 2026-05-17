using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace NeoUI.Blazor;

/// <summary>
/// A segmented date input that provides keyboard-driven editing and an optional calendar picker.
/// </summary>
/// <remarks>
/// <para>
/// DateInput renders each date part (month, day, year) as an individually focusable spinbutton
/// segment. Users can Tab between segments, use ArrowUp/Down to increment or decrement values,
/// type digits to enter values directly, and optionally open a calendar popover.
/// </para>
/// <para>
/// Features:
/// - Segment-based editing: month, day, year individually navigable
/// - Culture-aware format derived from CultureInfo.DateTimeFormat.ShortDatePattern
/// - ArrowUp/Down increment/decrement with wrap
/// - Digit input with auto-advance
/// - Optional calendar picker (ShowPickerButton)
/// - MinDate / MaxDate constraints propagated to calendar
/// - Form integration via hidden input (Name parameter)
/// - ARIA spinbutton roles for screen reader accessibility
/// - Two-way binding via @bind-Value
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;DateInput @bind-Value="selectedDate" /&gt;
/// &lt;DateInput @bind-Value="selectedDate" ShowPickerButton="false" /&gt;
/// &lt;DateInput @bind-Value="selectedDate" MinDate="DateOnly.Today" /&gt;
/// </code>
/// </example>
public partial class DateInput : ComponentBase, IAsyncDisposable
{
    private IJSObjectReference? _jsModule;
    private ElementReference _containerRef;
    private readonly List<DateSegment> _segments = [];
    private int _activeIndex = -1;
    private string _digitBuffer = "";
    private bool _isPickerOpen;
    private DateOnly? _pickerDate;
    private bool _jsInitialized;
    private bool _isFocused;
    private CancellationTokenSource? _blurCts;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [CascadingParameter(Name = "StyleVariant")]
    private StyleVariant _styleVariant { get; set; } = StyleVariant.Default;

    /// <summary>The selected date value. Supports two-way binding via @bind-Value.</summary>
    [Parameter] public DateOnly? Value { get; set; }

    /// <summary>Event callback invoked when the selected date changes.</summary>
    [Parameter] public EventCallback<DateOnly?> ValueChanged { get; set; }

    /// <summary>Culture for format and localization. Defaults to CultureInfo.CurrentCulture.</summary>
    [Parameter] public CultureInfo? Culture { get; set; }

    /// <summary>Whether the input is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Whether the input is required.</summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Whether to show the calendar picker button. Default true.</summary>
    [Parameter] public bool ShowPickerButton { get; set; } = true;

    /// <summary>Minimum selectable date (passed to calendar).</summary>
    [Parameter] public DateOnly? MinDate { get; set; }

    /// <summary>Maximum selectable date (passed to calendar).</summary>
    [Parameter] public DateOnly? MaxDate { get; set; }

    /// <summary>Function to determine if a specific date should be disabled in the calendar.</summary>
    [Parameter] public Func<DateOnly, bool>? IsDateDisabled { get; set; }

    /// <summary>Calendar caption layout. Default: Label.</summary>
    [Parameter] public CalendarCaptionLayout CaptionLayout { get; set; } = CalendarCaptionLayout.Label;

    /// <summary>HTML id for the container element.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Name for the hidden form input (enables form submission).</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Additional CSS classes for the container.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>ARIA label for the group container.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    /// <summary>ARIA describedby for the group container.</summary>
    [Parameter] public string? AriaDescribedBy { get; set; }

    /// <summary>ARIA invalid state. When true applies destructive border styling.</summary>
    [Parameter] public bool? AriaInvalid { get; set; }

    /// <summary>Placeholder text shown when the value is empty and the input is not focused.</summary>
    [Parameter] public string? Placeholder { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private CultureInfo EffectiveCulture => Culture ?? CultureInfo.CurrentCulture;
    private string? HiddenValue => TryBuildDate()?.ToString("yyyy-MM-dd");
    private bool HasAnySegmentValue => _segments.Any(s => !s.IsLiteral && s.Value is not null);
    private bool _showPlaceholder => !string.IsNullOrEmpty(Placeholder) && !_isFocused && Value is null && !HasAnySegmentValue;

    // ── Lifecycle ─────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        BuildSegments();
        SyncSegmentsFromValue();
        _pickerDate = Value;
    }

    protected override void OnParametersSet()
    {
        if (_segments.Count == 0) return; // OnInitialized hasn't run yet
        SyncSegmentsFromValue();
        _pickerDate = Value;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_jsInitialized)
        {
            try
            {
                _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoUI.Blazor/js/date-time-input.js");
                await _jsModule.InvokeVoidAsync("initialize", _containerRef);
                _jsInitialized = true;
            }
            catch { /* Graceful degradation during prerendering */ }
        }
    }

    // ── Segment construction ─────────────────────────────────────────────

    private void BuildSegments()
    {
        _segments.Clear();
        // Parse culture short date pattern, e.g. "M/d/yyyy", "dd/MM/yyyy", "yyyy-MM-dd"
        var fmt = EffectiveCulture.DateTimeFormat.ShortDatePattern;
        int i = 0;
        while (i < fmt.Length)
        {
            char c = fmt[i];
            if (c == '\'')
            {
                // Quoted literal
                int j = i + 1;
                while (j < fmt.Length && fmt[j] != '\'') j++;
                _segments.Add(new DateSegment { Kind = DateSegmentKind.Literal, LiteralText = fmt[(i + 1)..j] });
                i = j + 1;
            }
            else if (c is 'M' or 'd' or 'y')
            {
                int j = i;
                while (j < fmt.Length && fmt[j] == c) j++;
                var tokenLen = j - i;
                i = j;
                _segments.Add(c switch
                {
                    'M' => new DateSegment { Kind = DateSegmentKind.Month, Min = 1, Max = 12, MaxLength = 2, Placeholder = "MM" },
                    'd' => new DateSegment { Kind = DateSegmentKind.Day, Min = 1, Max = 31, MaxLength = 2, Placeholder = "DD" },
                    _ => tokenLen >= 4
                        ? new DateSegment { Kind = DateSegmentKind.Year, Min = 1, Max = 9999, MaxLength = 4, Placeholder = "YYYY" }
                        : new DateSegment { Kind = DateSegmentKind.Year, Min = 0, Max = 99, MaxLength = 2, Placeholder = "YY" }
                });
            }
            else
            {
                _segments.Add(new DateSegment { Kind = DateSegmentKind.Literal, LiteralText = c.ToString() });
                i++;
            }
        }
    }

    private void SyncSegmentsFromValue()
    {
        foreach (var seg in _segments)
        {
            if (seg.IsLiteral) continue;
            seg.Value = Value is null ? null : seg.Kind switch
            {
                DateSegmentKind.Month => Value.Value.Month,
                DateSegmentKind.Day   => Value.Value.Day,
                DateSegmentKind.Year  => seg.MaxLength >= 4 ? Value.Value.Year : Value.Value.Year % 100,
                _                     => seg.Value
            };
        }
    }

    // ── Focus events ─────────────────────────────────────────────────────

    private void OnSegmentFocus(int index)
    {
        _activeIndex = index;
        _digitBuffer = "";
    }

    private void OnSegmentBlur() => _digitBuffer = "";

    // ── Segment navigation ────────────────────────────────────────────────

    private async Task MoveToPrevSegment()
    {
        if (_jsModule is null) return;
        _digitBuffer = "";
        await _jsModule.InvokeVoidAsync("focusPrevSegment", _containerRef);
    }

    private async Task MoveToNextSegment()
    {
        if (_jsModule is null) return;
        _digitBuffer = "";
        await _jsModule.InvokeVoidAsync("focusNextSegment", _containerRef);
    }

    // ── Value manipulation ────────────────────────────────────────────────

    private void Increment(int delta)
    {
        if (_activeIndex < 0 || _activeIndex >= _segments.Count) return;
        var seg = _segments[_activeIndex];
        if (seg.IsLiteral) return;

        if (seg.Value is null)
            SeedUnsetSegmentsFromToday();

        var range = seg.Max - seg.Min + 1;
        var current = seg.Value!.Value;
        seg.Value = seg.Min + ((current - seg.Min + delta + range * 100) % range);
    }

    private void SeedUnsetSegmentsFromToday()
    {
        var today = DateTime.Today;
        foreach (var seg in _segments)
        {
            if (seg.IsLiteral || seg.Value is not null) continue;
            seg.Value = seg.Kind switch
            {
                DateSegmentKind.Month => today.Month,
                DateSegmentKind.Day   => today.Day,
                DateSegmentKind.Year  => seg.MaxLength >= 4 ? today.Year : today.Year % 100,
                _                     => seg.Value
            };
        }
    }

    private void SetToToday()
    {
        var today = DateTime.Today;
        foreach (var seg in _segments)
        {
            if (seg.IsLiteral) continue;
            seg.Value = seg.Kind switch
            {
                DateSegmentKind.Month => today.Month,
                DateSegmentKind.Day   => today.Day,
                DateSegmentKind.Year  => seg.MaxLength >= 4 ? today.Year : today.Year % 100,
                _                     => seg.Value
            };
        }
    }

    private void ClearActiveSegment()
    {
        _digitBuffer = "";
        if (_activeIndex < 0 || _activeIndex >= _segments.Count) return;
        var seg = _segments[_activeIndex];
        if (!seg.IsLiteral) seg.Value = null;
    }

    private async Task HandleDigit(int digit)
    {
        if (_activeIndex < 0 || _activeIndex >= _segments.Count) return;
        var seg = _segments[_activeIndex];
        if (seg.IsLiteral) return;

        _digitBuffer += digit.ToString();
        var newValue = int.Parse(_digitBuffer);

        if (newValue > seg.Max)
        {
            // Overflow — restart buffer with this digit
            _digitBuffer = digit.ToString();
            newValue = digit;
        }

        // For segments with min=1 (month/day), keep null while only "0" buffered
        seg.Value = (newValue == 0 && seg.Min >= 1) ? null : newValue;

        // Auto-advance when buffer is full, or no valid 2-digit continuation exists
        bool advance = _digitBuffer.Length >= seg.MaxLength
            || (newValue * 10 > seg.Max && newValue >= seg.Min);

        if (advance)
            await MoveToNextSegment();
    }

    // ── Container focus tracking ──────────────────────────────────────────

    private void OnContainerFocusIn()
    {
        _blurCts?.Cancel();
        _blurCts = null;
        _isFocused = true;
    }

    private async Task OnContainerFocusOut()
    {
        _blurCts?.Cancel();
        var cts = new CancellationTokenSource();
        _blurCts = cts;
        try
        {
            await Task.Delay(50, cts.Token);
            _isFocused = false;
            StateHasChanged();
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (_blurCts == cts) _blurCts = null;
            cts.Dispose();
        }
    }

    private async Task FocusFirstSegment()
    {
        if (_jsModule is null) return;
        await _jsModule.InvokeVoidAsync("focusFirstSegment", _containerRef);
    }

    // ── Keyboard handler ──────────────────────────────────────────────────

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (Disabled) return;

        switch (e.Key)
        {
            case "ArrowLeft":
                await MoveToPrevSegment();
                break;
            case "ArrowRight":
                await MoveToNextSegment();
                break;
            case "ArrowUp":
                Increment(1);
                await NotifyValueChanged();
                break;
            case "ArrowDown":
                Increment(-1);
                await NotifyValueChanged();
                break;
            case "Backspace":
            case "Delete":
                ClearActiveSegment();
                await NotifyValueChanged();
                break;
            default:
                if (e.Key.Length == 1 && char.IsDigit(e.Key[0]))
                {
                    await HandleDigit(e.Key[0] - '0');
                    await NotifyValueChanged();
                }
                else if (!e.CtrlKey && !e.MetaKey && (e.Key == "c" || e.Key == "C"))
                {
                    SetToToday();
                    await NotifyValueChanged();
                }
                break;
        }
    }

    // ── Picker ────────────────────────────────────────────────────────────

    private async Task OnPickerDateSelected(DateOnly? date)
    {
        if (date is null) return;
        _isPickerOpen = false;
        _pickerDate = date;
        foreach (var seg in _segments)
        {
            if (seg.IsLiteral) continue;
            seg.Value = seg.Kind switch
            {
                DateSegmentKind.Month => date.Value.Month,
                DateSegmentKind.Day   => date.Value.Day,
                DateSegmentKind.Year  => seg.MaxLength >= 4 ? date.Value.Year : date.Value.Year % 100,
                _                     => seg.Value
            };
        }
        await NotifyValueChanged();
    }

    // ── Value output ─────────────────────────────────────────────────────

    private async Task NotifyValueChanged()
    {
        var date = TryBuildDate();
        if (date != Value)
            await ValueChanged.InvokeAsync(date);
    }

    private DateOnly? TryBuildDate()
    {
        var yearSeg = _segments.FirstOrDefault(s => s.Kind == DateSegmentKind.Year);
        var month   = _segments.FirstOrDefault(s => s.Kind == DateSegmentKind.Month)?.Value;
        var day     = _segments.FirstOrDefault(s => s.Kind == DateSegmentKind.Day)?.Value;
        var year    = yearSeg?.Value;

        if (yearSeg is null || month is null || day is null || year is null || year < 1) return null;

        var fullYear = yearSeg.MaxLength < 4
            ? EffectiveCulture.Calendar.ToFourDigitYear(year.Value)
            : year.Value;

        try
        {
            var maxDay = DateTime.DaysInMonth(fullYear, month.Value);
            return new DateOnly(fullYear, month.Value, Math.Min(day.Value, maxDay));
        }
        catch { return null; }
    }

    // ── Display helpers ───────────────────────────────────────────────────

    private string GetSegmentDisplay(DateSegment seg)
    {
        if (seg.Value is null) return seg.Placeholder;
        return seg.Kind switch
        {
            DateSegmentKind.Month => seg.Value.Value.ToString("D2"),
            DateSegmentKind.Day   => seg.Value.Value.ToString("D2"),
            DateSegmentKind.Year  => seg.MaxLength >= 4
                ? seg.Value.Value.ToString("D4")
                : seg.Value.Value.ToString("D2"),
            _ => seg.Value.ToString()!
        };
    }

    private static string GetSegmentAriaLabel(DateSegment seg) => seg.Kind switch
    {
        DateSegmentKind.Month => "month",
        DateSegmentKind.Day   => "day",
        DateSegmentKind.Year  => "year",
        _                     => ""
    };

    private string GetSegmentClass(DateSegment seg) => ClassNames.cn(
        "tabular-nums select-none cursor-default rounded-sm px-0.5 caret-transparent",
        "focus:outline-none focus:bg-primary focus:text-primary-foreground",
        seg.Value is null ? "text-muted-foreground" : "",
        seg.Kind == DateSegmentKind.Year ? "min-w-[4ch] text-center" : "min-w-[2ch] text-center"
    );

    private string ContainerClass => ClassNames.cn(
        "flex h-8 w-full items-center rounded-md border border-input bg-background px-2 py-1 text-base shadow-xs",
        "md:text-sm",
        "transition-colors focus-within:border-ring focus-within:ring-[2px] focus-within:ring-ring/50",
        Disabled ? "cursor-not-allowed opacity-50 pointer-events-none" : "",
        AriaInvalid == true
            ? "border-destructive focus-within:ring-destructive/50 focus-within:border-destructive"
            : "",
        _styleVariant.GetClasses("Input.Root"),
        Class
    );

    public async ValueTask DisposeAsync()
    {
        _blurCts?.Cancel();
        _blurCts?.Dispose();
        if (_jsModule is not null)
        {
            try
            {
                await _jsModule.InvokeVoidAsync("dispose", _containerRef);
                await _jsModule.DisposeAsync();
            }
            catch { }
        }
    }
}
