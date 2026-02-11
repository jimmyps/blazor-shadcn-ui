namespace BlazorUI.Components.DateInput;

/// <summary>
/// Common date format patterns for DateInput component.
/// </summary>
public static class DateFormats
{
    /// <summary>ISO 8601 format: 2024-03-15. Mask: "0000-00-00"</summary>
    public const string ISO = "yyyy-MM-dd";

    /// <summary>US format: 03/15/2024. Mask: "00/00/0000"</summary>
    public const string US = "MM/dd/yyyy";

    /// <summary>European format: 15/03/2024. Mask: "00/00/0000"</summary>
    public const string EU = "dd/MM/yyyy";

    /// <summary>Short format: 03/15/24. Mask: "00/00/00"</summary>
    public const string Short = "MM/dd/yy";

    /// <summary>Long format: March 15, 2024. Mask: Dynamic</summary>
    public const string Long = "MMMM dd, yyyy";
    
    /// <summary>Date with abbreviated month: 15-Mar-2024. Mask: "00-AAA-0000"</summary>
    public const string Abbreviated = "dd-MMM-yyyy";
}
