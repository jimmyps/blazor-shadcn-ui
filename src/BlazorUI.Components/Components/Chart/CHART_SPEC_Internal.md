## Internal Architecture: Single DTO (1:1 ECharts Option)

### Goal
Internally, charts MUST produce a single C# DTO that mirrors the ECharts `option` JSON structure **1:1**, and then serialize it directly to JS with no additional “manual mapping layer” between intermediate `*Config` objects.

- The public API remains the declarative Blazor components (roots, primitives, series).
- The internal output is an `EChartsOption` DTO graph.
- JS receives the serialized `option` object, performs runtime CSS variable resolution, then calls `echartsInstance.setOption(option, ...)`.

### Rationale
- Avoids proliferation of `*Config` classes.
- Minimizes translation/mapping bugs between C# and JS.
- Keeps JS minimal and focused (lifecycle + CSS var resolution + setOption).
- Makes debugging easy by logging the exact ECharts `option` payload.

### Type Rules (zero friction)
When defining the internal DTO types:
- If ECharts expects a **string**, we use **string** (e.g., `"axis"`, `"item"`, `"horizontal"`, `"category"`).
- If ECharts expects a **number**, use **number** (`int`/`double`) (e.g., grid paddings).
- Avoid enums/converters for these internal DTO fields. Use strings as ECharts does.
- For polymorphic ECharts values that vary by chart type (e.g., `series.data`, `series.radius`, `itemStyle.borderRadius`, `areaStyle.color`), use `object` only where necessary.

### Formatter policy
Formatter fields in the internal DTO are **string-only**:
- `tooltip.formatter`: `string?`
- `axisLabel.formatter`: `string?`

Formatter strings are forwarded 1:1 to ECharts. Strings may be either:
1. **ECharts template strings**: e.g., `"{b}: {c}"`, `"{value}%"`
2. **JavaScript formatter functions expressed as strings**: e.g., `"function(params) { return params.value + '%'; }"`

The JS layer is responsible for interpreting function-strings and converting them to actual JavaScript functions before passing to ECharts.

### Legend position policy (MVP)
Legend placement fields are **string-only** in MVP:
- `legend.left`: `string?`
- `legend.top`: `string?`

Numeric margins (e.g. `MarginTop = 4`) must serialize as strings (e.g. `"4"`).

---

## Proposed Internal DTO Model: `EChartsOption` (MVP Subset)

> This model is the internal wire DTO used to serialize the ECharts `option` payload.
> It is intentionally MVP-scoped to support features in this chart spec.

```csharp
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorUI.Components.Chart;

public sealed class EChartsOption
{
    [JsonPropertyName("grid")]
    public EChartsGrid? Grid { get; set; }

    // MVP: single axis object (not array)
    [JsonPropertyName("xAxis")]
    public EChartsAxis? XAxis { get; set; }

    [JsonPropertyName("yAxis")]
    public EChartsAxis? YAxis { get; set; }

    [JsonPropertyName("legend")]
    public EChartsLegend? Legend { get; set; }

    [JsonPropertyName("tooltip")]
    public EChartsTooltip? Tooltip { get; set; }

    [JsonPropertyName("radar")]
    public EChartsRadar? Radar { get; set; }

    [JsonPropertyName("series")]
    public List<EChartsSeries>? Series { get; set; }

    [JsonPropertyName("animation")]
    public bool? Animation { get; set; }

    [JsonPropertyName("animationDuration")]
    public int? AnimationDuration { get; set; }

    [JsonPropertyName("animationEasing")]
    public string? AnimationEasing { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class EChartsGrid
{
    // ECharts grid paddings accept numbers.
    [JsonPropertyName("top")]
    public int? Top { get; set; }

    [JsonPropertyName("right")]
    public int? Right { get; set; }

    [JsonPropertyName("bottom")]
    public int? Bottom { get; set; }

    [JsonPropertyName("left")]
    public int? Left { get; set; }

    [JsonPropertyName("containLabel")]
    public bool? ContainLabel { get; set; }
}

public sealed class EChartsAxis
{
    // "category" | "value" | "time" | "log"
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    // Labels for category axis
    [JsonPropertyName("data")]
    public List<string>? Data { get; set; }

    [JsonPropertyName("axisLine")]
    public EChartsAxisLine? AxisLine { get; set; }

    [JsonPropertyName("axisTick")]
    public EChartsAxisTick? AxisTick { get; set; }

    [JsonPropertyName("axisLabel")]
    public EChartsAxisLabel? AxisLabel { get; set; }

    [JsonPropertyName("splitLine")]
    public EChartsSplitLine? SplitLine { get; set; }
}

public sealed class EChartsAxisLine
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

public sealed class EChartsAxisTick
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

public sealed class EChartsAxisLabel
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    // MVP: string-only formatter (templates), no function formatters.
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
}

public sealed class EChartsSplitLine
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }
}

public sealed class EChartsLegend
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    // "horizontal" | "vertical"
    [JsonPropertyName("orient")]
    public string? Orient { get; set; }

    // MVP: string-only placement fields.
    [JsonPropertyName("left")]
    public string? Left { get; set; } // "left" | "center" | "right" | "10" | "10%"

    [JsonPropertyName("top")]
    public string? Top { get; set; }  // "top" | "middle" | "bottom" | "4" | "10%"

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}

public sealed class EChartsTooltip
{
    [JsonPropertyName("show")]
    public bool? Show { get; set; }

    // "axis" | "item"
    [JsonPropertyName("trigger")]
    public string? Trigger { get; set; }

    [JsonPropertyName("axisPointer")]
    public EChartsAxisPointer? AxisPointer { get; set; }

    // MVP: string-only formatter (templates), no function formatters.
    [JsonPropertyName("formatter")]
    public string? Formatter { get; set; }
}

public sealed class EChartsAxisPointer
{
    // "line" | "shadow" | "cross" | "none"
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public sealed class EChartsRadar
{
    [JsonPropertyName("indicator")]
    public List<EChartsRadarIndicator>? Indicator { get; set; }
}

public sealed class EChartsRadarIndicator
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("max")]
    public double? Max { get; set; }
}

public sealed class EChartsSeries
{
    // "line" | "bar" | "pie" | "scatter" | "radar"
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    // Polymorphic by chart type.
    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("itemStyle")]
    public EChartsItemStyle? ItemStyle { get; set; }

    [JsonPropertyName("lineStyle")]
    public EChartsLineStyle? LineStyle { get; set; }

    [JsonPropertyName("areaStyle")]
    public EChartsAreaStyle? AreaStyle { get; set; }

    // Line-specific
    [JsonPropertyName("smooth")]
    public bool? Smooth { get; set; }

    [JsonPropertyName("showSymbol")]
    public bool? ShowSymbol { get; set; }

    // Stack (bar/area)
    [JsonPropertyName("stack")]
    public string? Stack { get; set; }

    // Pie-specific: "70%" or ["50%","70%"]
    [JsonPropertyName("radius")]
    public object? Radius { get; set; }

    // Emphasis
    [JsonPropertyName("emphasis")]
    public EChartsEmphasis? Emphasis { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public sealed class EChartsEmphasis
{
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }

    // "self" etc.
    [JsonPropertyName("focus")]
    public string? Focus { get; set; }
}

public sealed class EChartsItemStyle
{
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    // bar radius: number or [tl,tr,br,bl]
    [JsonPropertyName("borderRadius")]
    public object? BorderRadius { get; set; }
}

public sealed class EChartsLineStyle
{
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    // "solid" | "dashed" | "dotted"
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }
}

public sealed class EChartsAreaStyle
{
    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }

    // string color or gradient object
    [JsonPropertyName("color")]
    public object? Color { get; set; }
}

public sealed class EChartsLinearGradient
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "linear";

    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("x2")]
    public double X2 { get; set; }

    [JsonPropertyName("y2")]
    public double Y2 { get; set; }

    [JsonPropertyName("colorStops")]
    public List<EChartsColorStop> ColorStops { get; set; } = new();
}

public sealed class EChartsColorStop
{
    [JsonPropertyName("offset")]
    public double Offset { get; set; } // 0..1

    [JsonPropertyName("color")]
    public string Color { get; set; } = "";
}
```