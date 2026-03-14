using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>Wraps the content area (header + description) of a timeline item.</summary>
public partial class TimelineContent : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
    private string CssClass => ClassNames.cn("flex flex-col gap-1 pb-2", Class);
}
