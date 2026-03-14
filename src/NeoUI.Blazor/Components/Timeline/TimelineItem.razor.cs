using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Represents a single entry in a Timeline.
/// </summary>
/// <remarks>
/// Supports shorthand mode (Title/Time/Description params) and full declarative mode
/// (ChildContent). Custom icons, status-based styling, and optional collapsible detail
/// are all supported.
/// </remarks>
public partial class TimelineItem : ComponentBase
{
    private int _itemIndex;

    [CascadingParameter]
    public Timeline? ParentTimeline { get; set; }

    /// <summary>Gets or sets the color theme for the icon.</summary>
    [Parameter]
    public TimelineColor IconColor { get; set; } = TimelineColor.Primary;

    /// <summary>Gets or sets the current status of the item.</summary>
    [Parameter]
    public TimelineStatus Status { get; set; } = TimelineStatus.Completed;

    /// <summary>Gets or sets the color theme for the connector line.</summary>
    [Parameter]
    public TimelineColor? ConnectorColor { get; set; }

    /// <summary>Gets or sets whether to show the connector line below this item.</summary>
    [Parameter]
    public bool ShowConnector { get; set; } = true;

    /// <summary>Gets or sets the size of the icon. When null, inherits from the parent Timeline's Size.</summary>
    [Parameter]
    public TimelineSize? IconSize { get; set; }

    /// <summary>Gets or sets the icon style variant (Solid or Outline).</summary>
    [Parameter]
    public TimelineIconVariant IconVariant { get; set; } = TimelineIconVariant.Solid;

    /// <summary>Gets or sets the connector line style.</summary>
    [Parameter]
    public TimelineConnectorStyle ConnectorStyle { get; set; } = TimelineConnectorStyle.Solid;

    /// <summary>Gets or sets whether the item is in a loading state (shows pulse animation).</summary>
    [Parameter]
    public bool Loading { get; set; }

    /// <summary>Gets or sets whether the item content is collapsible.</summary>
    [Parameter]
    public bool IsCollapsible { get; set; }

    /// <summary>Gets or sets whether collapsible content is open by default.</summary>
    [Parameter]
    public bool DefaultOpen { get; set; } = true;

    /// <summary>Gets or sets the detail content shown when collapsible is expanded.</summary>
    [Parameter]
    public RenderFragment? DetailContent { get; set; }

    /// <summary>Gets or sets additional CSS classes for the connector line.</summary>
    [Parameter]
    public string? ConnectorClass { get; set; }

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Gets or sets the main content.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets custom icon content to replace the default icon.</summary>
    [Parameter]
    public RenderFragment? IconContent { get; set; }

    /// <summary>Gets or sets the time/date content for the date column.</summary>
    [Parameter]
    public RenderFragment? TimeContent { get; set; }

    /// <summary>Shorthand: title text. Auto-renders the full content tree when ChildContent is null.</summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Shorthand: time/date text.</summary>
    [Parameter]
    public string? Time { get; set; }

    /// <summary>Shorthand: description text.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool UseShorthand => ChildContent is null && Title is not null;
    private TimelineAlign Align => ParentTimeline?.Align ?? TimelineAlign.Center;
    private bool IsReversed => Align == TimelineAlign.Alternate && _itemIndex % 2 != 0;

    protected override void OnInitialized() =>
        _itemIndex = ParentTimeline?.RegisterItem() ?? 0;

    private string CssClass => ClassNames.cn("relative w-full", Class);

    private string ContentGridClass => ClassNames.cn(
        "grid gap-4 items-start",
        Align switch
        {
            TimelineAlign.Left  => "grid-cols-[auto_1fr]",
            TimelineAlign.Right => "grid-cols-[1fr_auto]",
            _                   => "grid-cols-[1fr_auto_1fr]"
        },
        Status == TimelineStatus.InProgress ? "aria-current-step" : null
    );

    private TimelineSize EffectiveIconSize => IconSize ?? ParentTimeline?.Size ?? TimelineSize.Medium;

    private string IconMinHeightClass => EffectiveIconSize switch
    {
        TimelineSize.Small => "min-h-8",
        TimelineSize.Large => "min-h-12",
        _                  => "min-h-10"
    };

    private string ContentWrapperClass    => ClassNames.cn("grid items-center", IconMinHeightClass);
    private string ContentWrapperEndClass => ClassNames.cn("grid items-center justify-items-end", IconMinHeightClass);

    private string ConnectorGapClass => EffectiveIconSize switch
    {
        TimelineSize.Small => "-mb-2",
        TimelineSize.Large => "-mb-6",
        _                  => "-mb-4"
    };

    private string ComputedConnectorClass => ClassNames.cn("flex-1 min-h-16", ConnectorGapClass, ConnectorClass);
    private string DateColumnClass => ClassNames.cn("flex flex-col justify-center", IconMinHeightClass);
}
