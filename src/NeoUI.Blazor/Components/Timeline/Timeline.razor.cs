using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A timeline component for displaying a chronological list of events.
/// </summary>
/// <remarks>
/// <para>
/// Supports 3 size variants, multiple alignment modes, status-based styling,
/// and custom icons per item. Uses semantic HTML (ordered list) for accessibility.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Timeline&gt;
///     &lt;TimelineItem Title="Deployed" Time="Jan 2025" Status="TimelineStatus.Completed" /&gt;
///     &lt;TimelineItem Title="In Review" Time="Feb 2025" Status="TimelineStatus.InProgress" /&gt;
///     &lt;TimelineItem Title="Planned" Time="Mar 2025" Status="TimelineStatus.Pending" /&gt;
/// &lt;/Timeline&gt;
/// </code>
/// </example>
public partial class Timeline : ComponentBase
{
    private int _itemCounter;

    /// <summary>Gets or sets the size variant controlling gap spacing between items.</summary>
    [Parameter]
    public TimelineSize Size { get; set; } = TimelineSize.Medium;

    /// <summary>Gets or sets the layout alignment for timeline items.</summary>
    [Parameter]
    public TimelineAlign Align { get; set; } = TimelineAlign.Center;

    /// <summary>Gets or sets how the connector line fits between icons.</summary>
    [Parameter]
    public TimelineConnectorFit ConnectorFit { get; set; } = TimelineConnectorFit.Spaced;

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Gets or sets the child content (TimelineItem elements).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Registers a timeline item and returns its index (used for Alternate alignment).</summary>
    internal int RegisterItem() => _itemCounter++;

    protected override void OnParametersSet() => _itemCounter = 0;

    private string CssClass => ClassNames.cn(
        "flex flex-col relative",
        Size switch
        {
            TimelineSize.Small  => "gap-2",
            TimelineSize.Large  => "gap-6",
            _                   => "gap-4"
        },
        Class
    );
}
