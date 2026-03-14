using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace NeoUI.Blazor;

/// <summary>Renders the title heading for a timeline item.</summary>
public partial class TimelineTitle : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string As { get; set; } = "h3";
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => ClassNames.cn(
        "font-semibold leading-none tracking-tight text-foreground", Class);

    private RenderFragment HeadingFragment => builder =>
    {
        builder.OpenElement(0, As);
        builder.AddAttribute(1, "class", CssClass);
        builder.AddMultipleAttributes(2, AdditionalAttributes);
        builder.AddContent(3, ChildContent);
        builder.CloseElement();
    };
}
