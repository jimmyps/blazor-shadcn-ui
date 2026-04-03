namespace NeoUI.Blazor;

/// <summary>
/// Defines the colour variant for a <see cref="NotificationBadge"/> chip.
/// </summary>
public enum NotificationBadgeVariant
{
    /// <summary>Red — for errors, alerts, or unread messages. Default.</summary>
    Destructive,

    /// <summary>Primary brand colour — for general counts (cart, downloads).</summary>
    Primary,

    /// <summary>Green — for success counts (completed tasks, earnings).</summary>
    Success,
}
