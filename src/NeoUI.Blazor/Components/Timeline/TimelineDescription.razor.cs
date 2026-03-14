using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>Renders the description text of a timeline item.</summary>
public partial class TimelineDescription : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
    private string CssClass => ClassNames.cn("text-sm text-muted-foreground", Class);
}
