using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Renders a vertical connector line between timeline items.
/// </summary>
/// <remarks>
/// Color adapts to item status: Completed = primary, InProgress = gradient, Pending = muted.
/// An explicit Color override takes precedence over status-based coloring.
/// </remarks>
public partial class TimelineConnector : ComponentBase
{
    /// <summary>Gets or sets the status used to determine connector styling.</summary>
    [Parameter]
    public TimelineStatus Status { get; set; } = TimelineStatus.Completed;

    /// <summary>Gets or sets an explicit color override. Overrides status-based color.</summary>
    [Parameter]
    public TimelineColor? Color { get; set; }

    /// <summary>Gets or sets the connector line style (Solid, Dashed, Dotted).</summary>
    [Parameter]
    public TimelineConnectorStyle ConnectorStyle { get; set; } = TimelineConnectorStyle.Solid;

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool IsSolid => ConnectorStyle == TimelineConnectorStyle.Solid;

    private string ColorVar
    {
        get
        {
            if (Color is not null)
                return Color switch
                {
                    TimelineColor.Primary     => "var(--primary)",
                    TimelineColor.Secondary   => "var(--secondary)",
                    TimelineColor.Muted       => "var(--muted)",
                    TimelineColor.Accent      => "var(--accent)",
                    TimelineColor.Destructive => "var(--destructive)",
                    _                         => "var(--primary)"
                };

            return Status switch
            {
                TimelineStatus.Completed  => "var(--primary)",
                TimelineStatus.InProgress => "var(--primary)",
                TimelineStatus.Pending    => "var(--muted)",
                _                         => "var(--primary)"
            };
        }
    }

    private string? InlineStyle => ConnectorStyle switch
    {
        TimelineConnectorStyle.Dashed =>
            $"background: repeating-linear-gradient(180deg, {ColorVar} 0px, {ColorVar} 4px, transparent 4px, transparent 8px)",
        TimelineConnectorStyle.Dotted =>
            $"background: repeating-linear-gradient(180deg, {ColorVar} 0px, {ColorVar} 2px, transparent 2px, transparent 5px)",
        _ => null
    };

    private string CssClass => ClassNames.cn(
        "w-0.5",
        IsSolid
            ? Color switch
            {
                TimelineColor.Primary     => "bg-primary",
                TimelineColor.Secondary   => "bg-secondary",
                TimelineColor.Muted       => "bg-muted",
                TimelineColor.Accent      => "bg-accent",
                TimelineColor.Destructive => "bg-destructive",
                _                         => Status switch
                {
                    TimelineStatus.Completed  => "bg-primary",
                    TimelineStatus.InProgress => "bg-linear-to-b from-primary to-muted",
                    TimelineStatus.Pending    => "bg-muted",
                    _                         => "bg-primary"
                }
            }
            : null,
        Class
    );
}
