namespace NeoUI.Blazor;

/// <summary>
/// Holds the start and end dates for a <see cref="FieldType.DateRange"/> field.
/// </summary>
public record DateRangeValue(DateOnly? Start, DateOnly? End);
