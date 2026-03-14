namespace NeoUI.Blazor;

/// <summary>
/// Defines the status of a timeline item, controlling icon and connector styling.
/// </summary>
public enum TimelineStatus
{
    /// <summary>Item has been completed. Displays with primary color.</summary>
    Completed,
    /// <summary>Item is currently in progress. Displays with a gradient connector.</summary>
    InProgress,
    /// <summary>Item is pending and has not started. Displays with muted color.</summary>
    Pending
}
