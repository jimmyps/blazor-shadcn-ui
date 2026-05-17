namespace NeoUI.Blazor;

internal sealed class TimeSegment
{
    public TimeSegmentKind Kind { get; init; }
    public int? Value { get; set; }     // For AmPm: 0=AM, 1=PM, null=unset
    public int Min { get; init; }
    public int Max { get; init; }
    public int MaxLength { get; init; }
    public string Placeholder { get; init; } = "";
    public string LiteralText { get; init; } = "";
    public bool IsLiteral => Kind == TimeSegmentKind.Literal;
    public bool IsAmPm => Kind == TimeSegmentKind.AmPm;
}
