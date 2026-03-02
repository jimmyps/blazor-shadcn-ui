using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace BlazorUI.Components.RangeSlider;

/// <summary>
/// A dual-handle range slider component that follows the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The RangeSlider component provides a customizable, accessible range selector with dual handles
/// for selecting a minimum and maximum value from a range. It follows WCAG 2.1 AA standards for 
/// accessibility and integrates with Blazor's data binding system.
/// </para>
/// <para>
/// Features:
/// - Dual draggable handles for range selection
/// - Click on track to jump nearest handle
/// - Keyboard support (Arrow keys, Page Up/Down, Home/End)
/// - Touch support for mobile devices
/// - Visual feedback (hover, active, focus states)
/// - Optional value tooltips that follow handles
/// - Prevent handles from crossing
/// - Snap to step increments
/// - Optional tick marks at intervals
/// - Full ARIA slider attributes
/// - RTL (Right-to-Left) support
/// - Horizontal and vertical orientations
/// - Form integration with validation support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;RangeSlider @bind-MinValue="minPrice" @bind-MaxValue="maxPrice" 
///              Min="0" Max="1000" Step="10" ShowTooltips="true" /&gt;
///
/// &lt;RangeSlider @bind-MinValue="minAge" @bind-MaxValue="maxAge" 
///              Min="18" Max="100" ShowLabels="true" /&gt;
/// </code>
/// </example>
public partial class RangeSlider : ComponentBase, IAsyncDisposable
{
    private ElementReference _containerRef;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<RangeSlider>? _dotNetRef;
    private bool _isDragging;
    private bool _isDraggingMin;
    private EditContext? _previousEditContext;
    private FieldIdentifier _minFieldIdentifier;
    private FieldIdentifier _maxFieldIdentifier;

    /// <summary>
    /// Gets or sets the JavaScript runtime for interop operations.
    /// </summary>
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets the cascaded EditContext from an EditForm.
    /// </summary>
    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    /// <summary>
    /// Gets or sets the lower bound of the selected range.
    /// </summary>
    [Parameter]
    public double MinValue { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the minimum value changes.
    /// </summary>
    [Parameter]
    public EventCallback<double> MinValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the upper bound of the selected range.
    /// </summary>
    [Parameter]
    public double MaxValue { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the maximum value changes.
    /// </summary>
    [Parameter]
    public EventCallback<double> MaxValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    [Parameter]
    public double Min { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    [Parameter]
    public double Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets the step increment.
    /// </summary>
    [Parameter]
    public double Step { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether to show min/max labels.
    /// </summary>
    [Parameter]
    public bool ShowLabels { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show value tooltips on handles.
    /// </summary>
    [Parameter]
    public bool ShowTooltips { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show tick marks.
    /// </summary>
    [Parameter]
    public bool ShowTicks { get; set; }

    /// <summary>
    /// Gets or sets the interval for tick marks.
    /// </summary>
    [Parameter]
    public double? TickInterval { get; set; }

    /// <summary>
    /// Gets or sets whether the slider is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the slider.
    /// </summary>
    [Parameter]
    public SliderOrientation Orientation { get; set; } = SliderOrientation.Horizontal;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute for the component.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the component for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the component.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound minimum value.
    /// </summary>
    [Parameter]
    public Expression<Func<double>>? MinValueExpression { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound maximum value.
    /// </summary>
    [Parameter]
    public Expression<Func<double>>? MaxValueExpression { get; set; }

    /// <summary>
    /// Gets or sets additional attributes to be applied to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string ContainerCssClass => ClassNames.cn(
        "relative w-full",
        Orientation == SliderOrientation.Vertical ? "h-64 flex flex-col" : "",
        Class
    );

    /// <summary>
    /// Gets the CSS class for the label container.
    /// </summary>
    private string LabelCssClass => ClassNames.cn(
        "flex justify-between mb-2",
        Orientation == SliderOrientation.Vertical ? "flex-col-reverse gap-2" : ""
    );

    /// <summary>
    /// Gets the CSS class for the slider container.
    /// </summary>
    private string SliderContainerCssClass => ClassNames.cn(
        "relative flex items-center",
        Orientation == SliderOrientation.Horizontal ? "w-full h-5" : "h-full w-5 flex-col",
        Disabled ? "cursor-not-allowed" : "cursor-pointer"
    );

    /// <summary>
    /// Gets the CSS class for the slider track.
    /// </summary>
    private string TrackCssClass => ClassNames.cn(
        "absolute bg-secondary rounded-full",
        Orientation == SliderOrientation.Horizontal 
            ? "w-full h-2 left-0" 
            : "h-full w-2 bottom-0"
    );

    /// <summary>
    /// Gets the CSS class for the selected range indicator.
    /// </summary>
    private string RangeCssClass => ClassNames.cn(
        "absolute bg-primary rounded-full transition-all",
        Orientation == SliderOrientation.Horizontal 
            ? "h-2" 
            : "w-2"
    );

    /// <summary>
    /// Gets the CSS class for the draggable handle.
    /// </summary>
    private string HandleCssClass => ClassNames.cn(
        "absolute z-10 h-5 w-5 rounded-full border-2 border-primary bg-background",
        "ring-offset-background transition-colors",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
        !Disabled ? "hover:bg-accent cursor-grab active:cursor-grabbing" : "cursor-not-allowed opacity-50"
    );

    /// <summary>
    /// Gets the CSS class for the value tooltip.
    /// </summary>
    private string TooltipCssClass => ClassNames.cn(
        "absolute px-2 py-1 text-xs font-medium text-primary-foreground bg-primary rounded",
        "pointer-events-none whitespace-nowrap",
        Orientation == SliderOrientation.Horizontal 
            ? "-top-8 left-1/2 -translate-x-1/2" 
            : "-right-10 top-1/2 -translate-y-1/2"
    );

    /// <summary>
    /// Gets the CSS class for the tick marks container.
    /// </summary>
    private string TickContainerCssClass => ClassNames.cn(
        "absolute",
        Orientation == SliderOrientation.Horizontal 
            ? "w-full h-2 left-0" 
            : "h-full w-2 bottom-0"
    );

    /// <summary>
    /// Gets the CSS class for individual tick marks.
    /// </summary>
    private string TickCssClass => ClassNames.cn(
        "absolute bg-border",
        Orientation == SliderOrientation.Horizontal 
            ? "w-px h-2" 
            : "h-px w-2"
    );

    /// <summary>
    /// Gets the inline style for the range indicator positioning and size.
    /// </summary>
    private string RangeStyle
    {
        get
        {
            var minPercent = GetPercentage(MinValue);
            var maxPercent = GetPercentage(MaxValue);

            if (Orientation == SliderOrientation.Horizontal)
            {
                return $"left: {minPercent}%; width: {maxPercent - minPercent}%;";
            }
            else
            {
                return $"bottom: {minPercent}%; height: {maxPercent - minPercent}%;";
            }
        }
    }

    /// <summary>
    /// Gets the inline style for the minimum value handle positioning.
    /// </summary>
    private string MinHandleStyle
    {
        get
        {
            var percent = GetPercentage(MinValue);
            if (Orientation == SliderOrientation.Horizontal)
            {
                return $"left: calc({percent}% - 10px);";
            }
            else
            {
                return $"bottom: calc({percent}% - 10px);";
            }
        }
    }

    /// <summary>
    /// Gets the inline style for the maximum value handle positioning.
    /// </summary>
    private string MaxHandleStyle
    {
        get
        {
            var percent = GetPercentage(MaxValue);
            if (Orientation == SliderOrientation.Horizontal)
            {
                return $"left: calc({percent}% - 10px);";
            }
            else
            {
                return $"bottom: calc({percent}% - 10px);";
            }
        }
    }

    /// <summary>
    /// Calculates the percentage position of a value within the min-max range.
    /// </summary>
    private double GetPercentage(double value)
    {
        if (Max <= Min) return 0;
        return ((value - Min) / (Max - Min)) * 100;
    }

    /// <summary>
    /// Gets the inline style for positioning a tick mark at the given percentage.
    /// </summary>
    private string GetTickStyle(double percent)
    {
        if (Orientation == SliderOrientation.Horizontal)
        {
            return $"left: {percent}%;";
        }
        else
        {
            return $"bottom: {percent}%;";
        }
    }

    /// <summary>
    /// Snaps a value to the nearest step increment.
    /// </summary>
    private double SnapToStep(double value)
    {
        var steps = Math.Round((value - Min) / Step);
        return Math.Max(Min, Math.Min(Max, Min + steps * Step));
    }

    /// <summary>
    /// Updates the minimum value ensuring it doesn't exceed the maximum value.
    /// </summary>
    private async Task UpdateMinValue(double newValue)
    {
        if (Disabled) return;

        newValue = SnapToStep(newValue);
        
        // Prevent handles from crossing
        if (newValue > MaxValue)
        {
            newValue = MaxValue;
        }

        if (MinValue != newValue)
        {
            MinValue = newValue;
            await MinValueChanged.InvokeAsync(MinValue);

            if (EditContext != null && MinValueExpression != null)
            {
                EditContext.NotifyFieldChanged(_minFieldIdentifier);
            }

            StateHasChanged();
        }
    }

    /// <summary>
    /// Updates the maximum value ensuring it doesn't go below the minimum value.
    /// </summary>
    private async Task UpdateMaxValue(double newValue)
    {
        if (Disabled) return;

        newValue = SnapToStep(newValue);
        
        // Prevent handles from crossing
        if (newValue < MinValue)
        {
            newValue = MinValue;
        }

        if (MaxValue != newValue)
        {
            MaxValue = newValue;
            await MaxValueChanged.InvokeAsync(MaxValue);

            if (EditContext != null && MaxValueExpression != null)
            {
                EditContext.NotifyFieldChanged(_maxFieldIdentifier);
            }

            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles mouse down event on the minimum value handle.
    /// </summary>
    private void OnMinHandleMouseDown(MouseEventArgs e)
    {
        if (Disabled) return;
        _isDragging = true;
        _isDraggingMin = true;
    }

    /// <summary>
    /// Handles mouse down event on the maximum value handle.
    /// </summary>
    private void OnMaxHandleMouseDown(MouseEventArgs e)
    {
        if (Disabled) return;
        _isDragging = true;
        _isDraggingMin = false;
    }

    /// <summary>
    /// Handles touch start event on the minimum value handle.
    /// </summary>
    private void OnMinHandleTouchStart(TouchEventArgs e)
    {
        if (Disabled) return;
        _isDragging = true;
        _isDraggingMin = true;
    }

    /// <summary>
    /// Handles touch start event on the maximum value handle.
    /// </summary>
    private void OnMaxHandleTouchStart(TouchEventArgs e)
    {
        if (Disabled) return;
        _isDragging = true;
        _isDraggingMin = false;
    }

    /// <summary>
    /// Handles keyboard navigation on slider handles.
    /// </summary>
    private async Task OnHandleKeyDown(KeyboardEventArgs e, bool isMin)
    {
        if (Disabled) return;

        var currentValue = isMin ? MinValue : MaxValue;
        var newValue = currentValue;

        switch (e.Key)
        {
            case "ArrowRight":
            case "ArrowUp":
                newValue = currentValue + Step;
                break;
            case "ArrowLeft":
            case "ArrowDown":
                newValue = currentValue - Step;
                break;
            case "PageUp":
                newValue = currentValue + (Step * 10);
                break;
            case "PageDown":
                newValue = currentValue - (Step * 10);
                break;
            case "Home":
                newValue = Min;
                break;
            case "End":
                newValue = Max;
                break;
            default:
                return;
        }

        if (isMin)
        {
            await UpdateMinValue(newValue);
        }
        else
        {
            await UpdateMaxValue(newValue);
        }
    }

    /// <summary>
    /// Called from JavaScript when a handle is being dragged.
    /// </summary>
    [JSInvokable]
    public async Task OnDragMove(double clientX, double clientY)
    {
        if (!_isDragging || Disabled) return;

        var value = await CalculateValueFromPosition(clientX, clientY);

        if (_isDraggingMin)
        {
            await UpdateMinValue(value);
        }
        else
        {
            await UpdateMaxValue(value);
        }
    }

    /// <summary>
    /// Called from JavaScript when dragging ends.
    /// </summary>
    [JSInvokable]
    public void OnDragEnd()
    {
        _isDragging = false;
    }

    /// <summary>
    /// Called from JavaScript when the track is clicked to move the nearest handle.
    /// </summary>
    [JSInvokable]
    public async Task OnTrackClick(double clientX, double clientY)
    {
        if (Disabled) return;

        var value = await CalculateValueFromPosition(clientX, clientY);
        
        // Move the nearest handle
        var distanceToMin = Math.Abs(value - MinValue);
        var distanceToMax = Math.Abs(value - MaxValue);

        if (distanceToMin < distanceToMax)
        {
            await UpdateMinValue(value);
        }
        else
        {
            await UpdateMaxValue(value);
        }
    }

    /// <summary>
    /// Calculates the slider value from a mouse/touch position.
    /// </summary>
    private async Task<double> CalculateValueFromPosition(double clientX, double clientY)
    {
        if (_jsModule == null) return MinValue;

        try
        {
            var result = await _jsModule.InvokeAsync<double>("calculateValue", 
                _containerRef, clientX, clientY, Orientation == SliderOrientation.Vertical, Min, Max);
            return result;
        }
        catch (JSException)
        {
            return MinValue;
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Validate and correct values
        MinValue = Math.Max(Min, Math.Min(MaxValue, MinValue));
        MaxValue = Math.Max(MinValue, Math.Min(Max, MaxValue));

        // Set up field identifiers for validation
        if (EditContext != null)
        {
            if (MinValueExpression != null)
            {
                _minFieldIdentifier = FieldIdentifier.Create(MinValueExpression);
            }
            if (MaxValueExpression != null)
            {
                _maxFieldIdentifier = FieldIdentifier.Create(MaxValueExpression);
            }

            // Subscribe to EditContext if it changed
            if (EditContext != _previousEditContext)
            {
                DetachValidationStateChangedListener();
                EditContext.OnValidationStateChanged += OnValidationStateChanged;
                _previousEditContext = EditContext;
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                _dotNetRef = DotNetObjectReference.Create(this);
                _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/range-slider.js");
                
                await _jsModule.InvokeVoidAsync("initialize", _containerRef, _dotNetRef);
            }
            catch (JSException)
            {
                // JS module not available, component will still work with reduced functionality
            }
        }
    }

    /// <summary>
    /// Handles validation state changes from the EditContext.
    /// </summary>
    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Detaches the validation state change listener from the EditContext.
    /// </summary>
    private void DetachValidationStateChangedListener()
    {
        if (_previousEditContext != null)
        {
            _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        DetachValidationStateChangedListener();

        if (_jsModule != null)
        {
            try
            {
                await _jsModule.InvokeVoidAsync("dispose", _containerRef);
                await _jsModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore - this happens during hot reload or when navigating away
            }
        }

        _dotNetRef?.Dispose();
    }
}
