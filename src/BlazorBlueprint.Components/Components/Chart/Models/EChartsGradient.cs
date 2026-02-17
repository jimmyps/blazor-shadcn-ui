using System.Text.Json.Serialization;

namespace BlazorBlueprint.Components;

internal sealed class EChartsLinearGradient
{
    [JsonPropertyName("type")]
    public string Type { get; } = "linear";

    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("x2")]
    public double X2 { get; set; }

    [JsonPropertyName("y2")]
    public double Y2 { get; set; }

    [JsonPropertyName("colorStops")]
    public List<EChartsColorStop> ColorStops { get; set; } = [];

    [JsonPropertyName("global")]
    public bool Global { get; set; }
}

internal sealed class EChartsColorStop
{
    [JsonPropertyName("offset")]
    public double Offset { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;
}

internal sealed class EChartsTitleOption
{
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    [JsonPropertyName("subtext")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Subtext { get; set; }

    [JsonPropertyName("left")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Left { get; set; } = "center";

    [JsonPropertyName("top")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Top { get; set; }

    [JsonPropertyName("textAlign")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TextAlign { get; set; }

    [JsonPropertyName("textVerticalAlign")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TextVerticalAlign { get; set; }

    [JsonPropertyName("textStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsTextStyleOption? TextStyle { get; set; }

    [JsonPropertyName("subtextStyle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EChartsTextStyleOption? SubtextStyle { get; set; }

    [JsonPropertyName("itemGap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ItemGap { get; set; }
}
