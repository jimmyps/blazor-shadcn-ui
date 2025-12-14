using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

/// <summary>
/// Tooltip trigger type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TooltipTrigger
{
    /// <summary>Trigger on item (e.g., pie slice, scatter point)</summary>
    [JsonPropertyName("item")]
    Item,
    
    /// <summary>Trigger on axis (e.g., line/bar charts)</summary>
    [JsonPropertyName("axis")]
    Axis
}

/// <summary>
/// Axis pointer type for tooltips.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AxisPointerType
{
    /// <summary>No axis pointer (default)</summary>
    [JsonPropertyName("none")]
    None,
    
    /// <summary>Line axis pointer</summary>
    [JsonPropertyName("line")]
    Line,
    
    /// <summary>Shadow axis pointer</summary>
    [JsonPropertyName("shadow")]
    Shadow,
    
    /// <summary>Cross axis pointer</summary>
    [JsonPropertyName("cross")]
    Cross
}

/// <summary>
/// Legend icon shape.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LegendIcon
{
    /// <summary>Circle icon (default)</summary>
    [JsonPropertyName("circle")]
    Circle,
    
    /// <summary>Rectangle icon</summary>
    [JsonPropertyName("rect")]
    Rect,
    
    /// <summary>Rounded rectangle icon</summary>
    [JsonPropertyName("roundRect")]
    RoundRect,
    
    /// <summary>Triangle icon</summary>
    [JsonPropertyName("triangle")]
    Triangle,
    
    /// <summary>Diamond icon</summary>
    [JsonPropertyName("diamond")]
    Diamond,
    
    /// <summary>Pin icon</summary>
    [JsonPropertyName("pin")]
    Pin,
    
    /// <summary>Arrow icon</summary>
    [JsonPropertyName("arrow")]
    Arrow,
    
    /// <summary>No icon</summary>
    [JsonPropertyName("none")]
    None
}

/// <summary>
/// Orientation for legend or other components.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Orient
{
    /// <summary>Horizontal orientation</summary>
    [JsonPropertyName("horizontal")]
    Horizontal,
    
    /// <summary>Vertical orientation</summary>
    [JsonPropertyName("vertical")]
    Vertical
}

/// <summary>
/// Axis type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AxisType
{
    /// <summary>Category axis (for discrete data)</summary>
    [JsonPropertyName("category")]
    Category,
    
    /// <summary>Value axis (for continuous numerical data)</summary>
    [JsonPropertyName("value")]
    Value,
    
    /// <summary>Time axis (for time-series data)</summary>
    [JsonPropertyName("time")]
    Time,
    
    /// <summary>Logarithmic axis</summary>
    [JsonPropertyName("log")]
    Log
}

/// <summary>
/// Emphasis focus behavior.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmphasisFocus
{
    /// <summary>Focus on self (default - disabled)</summary>
    [JsonPropertyName("self")]
    Self,
    
    /// <summary>Focus on series</summary>
    [JsonPropertyName("series")]
    Series,
    
    /// <summary>No emphasis</summary>
    [JsonPropertyName("none")]
    None
}
