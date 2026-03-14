using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>Header area of a timeline item, typically contains TimelineTitle and TimelineTime.</summary>
public partial class TimelineHeader : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
    private string CssClass => ClassNames.cn("flex items-center gap-2", Class);
}
