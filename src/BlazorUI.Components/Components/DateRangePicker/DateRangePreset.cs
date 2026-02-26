namespace BlazorUI.Components.DateRangePicker;

/// <summary>
/// Preset date ranges for quick selection.
/// </summary>
public enum DateRangePreset
{
    /// <summary>
    /// Today's date.
    /// </summary>
    Today,

    /// <summary>
    /// Yesterday's date.
    /// </summary>
    Yesterday,

    /// <summary>
    /// Last 7 days including today.
    /// </summary>
    Last7Days,

    /// <summary>
    /// Last 30 days including today.
    /// </summary>
    Last30Days,

    /// <summary>
    /// Current month (from 1st to last day).
    /// </summary>
    ThisMonth,

    /// <summary>
    /// Previous month (from 1st to last day).
    /// </summary>
    LastMonth,

    /// <summary>
    /// Current year (January 1 to December 31).
    /// </summary>
    ThisYear,

    /// <summary>
    /// Custom date range.
    /// </summary>
    Custom
}
