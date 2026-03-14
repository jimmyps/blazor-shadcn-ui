using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>Renders a semantic time element for displaying dates in a timeline item.</summary>
public partial class TimelineTime : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
    private string CssClass => ClassNames.cn(
        "text-sm font-medium tracking-tight text-muted-foreground", Class);
}
