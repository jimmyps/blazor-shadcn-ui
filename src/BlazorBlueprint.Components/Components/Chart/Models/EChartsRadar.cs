using System.Text.Json.Serialization;

namespace BlazorBlueprint.Components;

internal sealed class EChartsRadarOption
{
    [JsonPropertyName("indicator")]
    public List<EChartsRadarIndicator> Indicator { get; set; } = [];

    [JsonPropertyName("shape")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Shape { get; set; }

    [JsonPropertyName("splitNumber")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SplitNumber { get; set; }

    [JsonPropertyName("axisLine")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsAxisLineOption? AxisLine { get; set; }

    [JsonPropertyName("splitLine")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsSplitLineOption? SplitLine { get; set; }

    [JsonPropertyName("splitArea")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsSplitAreaOption? SplitArea { get; set; }

    [JsonPropertyName("axisName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsAxisLabelOption? AxisName { get; set; }

    [JsonPropertyName("center")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Center { get; set; }

    [JsonPropertyName("radius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Radius { get; set; }
}

public sealed class EChartsRadarIndicator
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("max")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Max { get; set; }
}

internal sealed class EChartsSplitAreaOption
{
    [JsonPropertyName("show")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Show { get; set; }

    [JsonPropertyName("areaStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsAreaStyleOption? AreaStyle { get; set; }
}

internal sealed class EChartsPolarOption
{
    [JsonPropertyName("center")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Center { get; set; }

    [JsonPropertyName("radius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Radius { get; set; }
}

internal sealed class EChartsAngleAxisOption
{
    [JsonPropertyName("max")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Max { get; set; }

    [JsonPropertyName("startAngle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StartAngle { get; set; }

    [JsonPropertyName("show")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Show { get; set; } = true;

    [JsonPropertyName("splitLine")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsSplitLineOption? SplitLine { get; set; }
}

internal sealed class EChartsRadiusAxisOption
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Data { get; set; }

    [JsonPropertyName("show")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Show { get; set; } = true;

    [JsonPropertyName("axisLine")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsAxisLineOption? AxisLine { get; set; }

    [JsonPropertyName("axisTick")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsAxisTickOption? AxisTick { get; set; }

    [JsonPropertyName("axisLabel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsAxisLabelOption? AxisLabel { get; set; }
}
