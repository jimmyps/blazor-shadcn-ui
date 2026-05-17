using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace NeoUI.Blazor;

/// <summary>
/// A segmented time input that provides keyboard-driven editing and an optional time picker.
/// </summary>
/// <remarks>
/// <para>
/// TimeInput renders each time part (hour, minute, second, AM/PM) as an individually focusable
/// spinbutton segment. Supports 12-hour and 24-hour formats, optional seconds display,
/// and a configurable minute step.
/// </para>
/// <para>
/// Features:
/// - Segment-based editing: hour, minute, optional second, optional AM/PM
/// - 12h / 24h mode (Use12Hour parameter, defaults to culture setting)
/// - ArrowUp/Down increment/decrement with wrap; AM/PM toggles on ArrowUp/Down
/// - Digit input with auto-advance; 'a'/'p' keys toggle AM/PM
/// - Optional clock picker popover (ShowPickerButton)
/// - MinuteStep for coarse minute selection
/// - Form integration via hidden input (Name parameter)
/// - ARIA spinbutton roles for screen reader accessibility
/// - Two-way binding via @bind-Value
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;TimeInput @bind-Value="selectedTime" /&gt;
/// &lt;TimeInput @bind-Value="selectedTime" Use12Hour="true" ShowSeconds="true" /&gt;
/// &lt;TimeInput @bind-Value="selectedTime" MinuteStep="15" /&gt;
/// </code>
/// </example>
public partial class TimeInput : ComponentBase, IAsyncDisposable
{
    private IJSObjectReference? _jsModule;
    private ElementReference _containerRef;
    private readonly List<TimeSegment> _segments = [];
    private int _activeIndex = -1;
    private string _digitBuffer = "";
    private bool _isPickerOpen;
    private bool _jsInitialized;
    private bool _isFocused;
    private CancellationTokenSource? _blurCts;

    // Picker staging values (committed on Apply)
    private int _pickerHour;
    private int _pickerMinute;
    private int _pickerSecond;
    private bool _pickerIsAm = true;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [CascadingParameter(Name = "StyleVariant")]
    private StyleVariant _styleVariant { get; set; } = StyleVariant.Default;

    /// <summary>The selected time value. Supports two-way binding via @bind-Value.</summary>
    [Parameter] public TimeOnly? Value { get; set; }

    /// <summary>Event callback invoked when the selected time changes.</summary>
    [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }

    /// <summary>Culture for localization. Defaults to CultureInfo.CurrentCulture.</summary>
    [Parameter] public CultureInfo? Culture { get; set; }

    /// <summary>
    /// Whether to use 12-hour (AM/PM) format. Defaults to true when culture uses AM/PM designators.
    /// </summary>
    [Parameter] public bool? Use12Hour { get; set; }

    /// <summary>Whether to show the seconds segment. Default false.</summary>
    [Parameter] public bool ShowSeconds { get; set; }

    /// <summary>Step interval for minutes (e.g. 15 = only 0,15,30,45 in picker). Default 1.</summary>
    [Parameter] public int MinuteStep { get; set; } = 1;

    /// <summary>Whether the input is disabled.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Whether the input is required.</summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>Whether to show the clock picker button. Default true.</summary>
    [Parameter] public bool ShowPickerButton { get; set; } = true;

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
    private bool EffectiveUse12Hour => Use12Hour ?? !string.IsNullOrEmpty(EffectiveCulture.DateTimeFormat.AMDesignator);
    private string? HiddenValue => TryBuildTime()?.ToString("HH:mm:ss");
    private bool HasAnySegmentValue => _segments.Any(s => !s.IsLiteral && s.Value is not null);
    private bool _showPlaceholder => !string.IsNullOrEmpty(Placeholder) && !_isFocused && Value is null && !HasAnySegmentValue;

    // ── Lifecycle ─────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        BuildSegments();
        SyncSegmentsFromValue();
        SyncPickerFromSegments();
    }

    protected override void OnParametersSet()
    {
        if (_segments.Count == 0) return;
        SyncSegmentsFromValue();
        SyncPickerFromSegments();
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
            catch { }
        }
    }

    // ── Segment construction ──────────────────────────────────────────────

    private void BuildSegments()
    {
        _segments.Clear();
        bool use12 = EffectiveUse12Hour;

        // Hour
        _segments.Add(use12
            ? new TimeSegment { Kind = TimeSegmentKind.Hour, Min = 1, Max = 12, MaxLength = 2, Placeholder = "HH" }
            : new TimeSegment { Kind = TimeSegmentKind.Hour, Min = 0, Max = 23, MaxLength = 2, Placeholder = "HH" });

        _segments.Add(new TimeSegment { Kind = TimeSegmentKind.Literal, LiteralText = ":" });

        // Minute
        _segments.Add(new TimeSegment { Kind = TimeSegmentKind.Minute, Min = 0, Max = 59, MaxLength = 2, Placeholder = "MM" });

        // Optional seconds
        if (ShowSeconds)
        {
            _segments.Add(new TimeSegment { Kind = TimeSegmentKind.Literal, LiteralText = ":" });
            _segments.Add(new TimeSegment { Kind = TimeSegmentKind.Second, Min = 0, Max = 59, MaxLength = 2, Placeholder = "SS" });
        }

        // AM/PM
        if (use12)
        {
            _segments.Add(new TimeSegment { Kind = TimeSegmentKind.Literal, LiteralText = " " });
            _segments.Add(new TimeSegment { Kind = TimeSegmentKind.AmPm, Min = 0, Max = 1, MaxLength = 1, Placeholder = "AM" });
        }
    }

    private void SyncSegmentsFromValue()
    {
        if (Value is null)
        {
            foreach (var seg in _segments.Where(s => !s.IsLiteral))
                seg.Value = null;
            return;
        }

        var t = Value.Value;
        bool use12 = EffectiveUse12Hour;
        bool isPm = t.Hour >= 12;
        int hour12 = t.Hour % 12 == 0 ? 12 : t.Hour % 12;

        foreach (var seg in _segments)
        {
            seg.Value = seg.Kind switch
            {
                TimeSegmentKind.Hour   => use12 ? hour12 : t.Hour,
                TimeSegmentKind.Minute => t.Minute,
                TimeSegmentKind.Second => t.Second,
                TimeSegmentKind.AmPm   => isPm ? 1 : 0,
                _                      => seg.Value
            };
        }
    }

    private void SyncPickerFromSegments()
    {
        var hourSeg = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.Hour);
        var minSeg  = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.Minute);
        var secSeg  = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.Second);
        var ampSeg  = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.AmPm);

        _pickerIsAm  = ampSeg?.Value != 1;
        _pickerHour  = hourSeg?.Value ?? (EffectiveUse12Hour ? 12 : 0);
        _pickerMinute = minSeg?.Value ?? 0;
        _pickerSecond = secSeg?.Value ?? 0;
    }

    // ── Focus events ──────────────────────────────────────────────────────

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

        if (seg.IsAmPm)
        {
            if (seg.Value is null)
                SeedUnsetSegmentsFromNow(); // seed AM/PM from now; don't toggle on first press
            else
                seg.Value = seg.Value == 0 ? 1 : 0;
            return;
        }

        if (seg.Value is null)
            SeedUnsetSegmentsFromNow();

        var range = seg.Kind == TimeSegmentKind.Minute && MinuteStep > 1
            ? 60 / MinuteStep
            : seg.Max - seg.Min + 1;

        int current;
        if (seg.Kind == TimeSegmentKind.Minute && MinuteStep > 1)
        {
            var step = seg.Value!.Value / MinuteStep;
            current = seg.Min + ((step + delta + range * 100) % range) * MinuteStep;
        }
        else
        {
            current = seg.Value!.Value;
            current = seg.Min + ((current - seg.Min + delta + range * 100) % range);
        }
        seg.Value = current;
    }

    private void SeedUnsetSegmentsFromNow()
    {
        var now = DateTime.Now;
        bool use12 = EffectiveUse12Hour;
        bool isPm = now.Hour >= 12;
        int hour12 = now.Hour % 12 == 0 ? 12 : now.Hour % 12;

        foreach (var seg in _segments)
        {
            if (seg.IsLiteral || seg.Value is not null) continue;
            seg.Value = seg.Kind switch
            {
                TimeSegmentKind.Hour   => use12 ? hour12 : now.Hour,
                TimeSegmentKind.Minute => MinuteStep > 1 ? (now.Minute / MinuteStep) * MinuteStep : now.Minute,
                TimeSegmentKind.Second => now.Second,
                TimeSegmentKind.AmPm   => isPm ? 1 : 0,
                _                      => seg.Value
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
        if (seg.IsLiteral || seg.IsAmPm) return;

        _digitBuffer += digit.ToString();
        var newValue = int.Parse(_digitBuffer);

        if (newValue > seg.Max)
        {
            _digitBuffer = digit.ToString();
            newValue = digit;
        }

        seg.Value = (newValue == 0 && seg.Min > 0) ? null : newValue;

        bool advance = _digitBuffer.Length >= seg.MaxLength
            || (newValue * 10 > seg.Max && newValue >= seg.Min);

        if (advance)
            await MoveToNextSegment();
    }

    private async Task HandleAmPmKey(char key)
    {
        if (_activeIndex < 0 || _activeIndex >= _segments.Count) return;
        var seg = _segments[_activeIndex];
        if (!seg.IsAmPm) return;

        seg.Value = char.ToUpperInvariant(key) == 'A' ? 0 : 1;
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

        var activeSeg = _activeIndex >= 0 && _activeIndex < _segments.Count
            ? _segments[_activeIndex] : null;

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
                if (e.Key.Length == 1)
                {
                    if (char.IsDigit(e.Key[0]))
                    {
                        await HandleDigit(e.Key[0] - '0');
                        await NotifyValueChanged();
                    }
                    else if (activeSeg?.IsAmPm == true &&
                             (e.Key[0] is 'a' or 'A' or 'p' or 'P'))
                    {
                        await HandleAmPmKey(e.Key[0]);
                        await NotifyValueChanged();
                    }
                    else if (!e.CtrlKey && !e.MetaKey && (e.Key[0] is 'c' or 'C'))
                    {
                        SyncSegmentsFromTimeOnly(TimeOnly.FromDateTime(DateTime.Now));
                        await NotifyValueChanged();
                    }
                }
                break;
        }
    }

    // ── Picker handlers ───────────────────────────────────────────────────

    private void OnPickerHourChanged(string? val)
    {
        if (int.TryParse(val, out var h)) _pickerHour = h;
    }

    private void OnPickerMinuteChanged(string? val)
    {
        if (int.TryParse(val, out var m)) _pickerMinute = m;
    }

    private void OnPickerSecondChanged(string? val)
    {
        if (int.TryParse(val, out var s)) _pickerSecond = s;
    }

    private void OnPickerAmPmChanged(string? val)
    {
        _pickerIsAm = val == "AM";
    }

    private async Task ApplyPickerTime()
    {
        _isPickerOpen = false;

        int hour = _pickerHour;
        if (EffectiveUse12Hour)
        {
            if (!_pickerIsAm && hour < 12) hour += 12;
            else if (_pickerIsAm && hour == 12) hour = 0;
        }

        var time = new TimeOnly(hour, _pickerMinute, _pickerSecond);
        SyncSegmentsFromTimeOnly(time);
        await ValueChanged.InvokeAsync(time);
    }

    private void SyncSegmentsFromTimeOnly(TimeOnly t)
    {
        bool use12 = EffectiveUse12Hour;
        bool isPm = t.Hour >= 12;
        int hour12 = t.Hour % 12 == 0 ? 12 : t.Hour % 12;

        foreach (var seg in _segments)
        {
            seg.Value = seg.Kind switch
            {
                TimeSegmentKind.Hour   => use12 ? hour12 : t.Hour,
                TimeSegmentKind.Minute => t.Minute,
                TimeSegmentKind.Second => t.Second,
                TimeSegmentKind.AmPm   => isPm ? 1 : 0,
                _                      => seg.Value
            };
        }
    }

    // ── Value output ──────────────────────────────────────────────────────

    private async Task NotifyValueChanged()
    {
        var time = TryBuildTime();
        if (time != Value)
            await ValueChanged.InvokeAsync(time);
    }

    private TimeOnly? TryBuildTime()
    {
        var hourSeg = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.Hour);
        var minSeg  = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.Minute);
        var secSeg  = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.Second);
        var ampSeg  = _segments.FirstOrDefault(s => s.Kind == TimeSegmentKind.AmPm);

        if (hourSeg?.Value is null || minSeg?.Value is null) return null;

        int hour = hourSeg.Value.Value;

        if (EffectiveUse12Hour && ampSeg?.Value is not null)
        {
            bool isPm = ampSeg.Value == 1;
            if (isPm && hour < 12) hour += 12;
            else if (!isPm && hour == 12) hour = 0;
        }

        int sec = secSeg?.Value ?? 0;
        try { return new TimeOnly(hour, minSeg.Value.Value, sec); }
        catch { return null; }
    }

    // ── Display helpers ───────────────────────────────────────────────────

    private string GetSegmentDisplay(TimeSegment seg)
    {
        if (seg.Value is null) return seg.Placeholder;
        return seg.Value.Value.ToString("D2");
    }

    private static string GetAmPmDisplay(TimeSegment seg)
    {
        if (seg.Value is null) return seg.Placeholder; // "AM" placeholder
        return seg.Value == 0 ? "AM" : "PM";
    }

    private static string GetSegmentAriaLabel(TimeSegment seg) => seg.Kind switch
    {
        TimeSegmentKind.Hour   => "hour",
        TimeSegmentKind.Minute => "minute",
        TimeSegmentKind.Second => "second",
        _                      => ""
    };

    private string GetSegmentClass(TimeSegment seg) => ClassNames.cn(
        "tabular-nums select-none cursor-default rounded-sm px-0.5 caret-transparent",
        "focus:outline-none focus:bg-primary focus:text-primary-foreground",
        seg.Value is null ? "text-muted-foreground" : "",
        seg.IsAmPm ? "min-w-[2ch] text-center" : "min-w-[2ch] text-center"
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
