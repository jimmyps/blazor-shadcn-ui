using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Chart.Series;

/// <summary>
/// Base class for all chart series components providing common properties and animation support.
/// </summary>
/// <typeparam name="TData">The type of data items in the chart.</typeparam>
public abstract class SeriesBase<TData> : ComponentBase
{
    // ===== CORE IDENTIFICATION =====
    
    /// <summary>
    /// Gets or sets the property name to extract data values from.
    /// </summary>
    [Parameter, EditorRequired]
    public string DataKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the display name for this series (shown in legend/tooltip).
    /// </summary>
    [Parameter]
    public string? Name { get; set; }
    
    // ===== VISUAL STYLING =====
    
    /// <summary>
    /// Gets or sets the fill color (supports CSS variables like "var(--chart-1)").
    /// </summary>
    [Parameter]
    public string? Fill { get; set; }
    
    /// <summary>
    /// Gets or sets the stroke/border color.
    /// </summary>
    [Parameter]
    public string? Stroke { get; set; }
    
    /// <summary>
    /// Gets or sets the stroke/border width in pixels.
    /// </summary>
    [Parameter]
    public int? StrokeWidth { get; set; }
    
    // ===== LABEL SUPPORT =====
    
    /// <summary>
    /// Gets or sets whether to show data labels on the series.
    /// </summary>
    [Parameter]
    public bool ShowLabel { get; set; }
    
    /// <summary>
    /// Gets or sets the position of data labels.
    /// </summary>
    [Parameter]
    public LabelPosition? LabelPosition { get; set; }
    
    /// <summary>
    /// Gets or sets the formatter for data labels (template string or JS function).
    /// </summary>
    [Parameter]
    public string? LabelFormatter { get; set; }
    
    // ===== ANIMATION PROPERTIES =====
    
    /// <summary>
    /// Gets or sets whether to enable animation for this series. Null = use chart default.
    /// </summary>
    [Parameter]
    public bool? EnableAnimation { get; set; }
    
    /// <summary>
    /// Gets or sets the animation duration in milliseconds. Null = use chart default.
    /// </summary>
    [Parameter]
    public int? AnimationDuration { get; set; }
    
    /// <summary>
    /// Gets or sets the animation easing function. Null = use chart default.
    /// </summary>
    [Parameter]
    public AnimationEasing? AnimationEasing { get; set; }
    
    /// <summary>
    /// Gets or sets the animation delay in milliseconds. Null = use chart default.
    /// </summary>
    [Parameter]
    public int? AnimationDelay { get; set; }
    
    /// <summary>
    /// Gets or sets the animation duration for data updates. Null = use chart default.
    /// </summary>
    [Parameter]
    public int? AnimationDurationUpdate { get; set; }
    
    /// <summary>
    /// Gets or sets the animation easing for data updates. Null = use chart default.
    /// </summary>
    [Parameter]
    public AnimationEasing? AnimationEasingUpdate { get; set; }
    
    // ===== CHILD CONTENT =====
    
    /// <summary>
    /// Gets or sets the child content (for LabelList components).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    // ===== LABEL REGISTRATION =====
    
    /// <summary>
    /// Gets the registered LabelList component for this series.
    /// </summary>
    protected LabelList? LabelList { get; private set; }
    
    /// <summary>
    /// Registers a LabelList component with this series.
    /// </summary>
    public void RegisterLabelList(LabelList labelList)
    {
        LabelList = labelList;
    }
}
