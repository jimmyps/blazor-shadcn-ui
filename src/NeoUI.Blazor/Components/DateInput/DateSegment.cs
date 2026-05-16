namespace NeoUI.Blazor;

internal sealed class DateSegment
{
    public DateSegmentKind Kind { get; init; }
    public int? Value { get; set; }
    public int Min { get; init; }
    public int Max { get; init; }
    public int MaxLength { get; init; }
    public string Placeholder { get; init; } = "";
    public string LiteralText { get; init; } = "";
    public bool IsLiteral => Kind == DateSegmentKind.Literal;
}
