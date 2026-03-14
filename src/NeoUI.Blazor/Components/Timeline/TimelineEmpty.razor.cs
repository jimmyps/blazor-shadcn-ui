using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Fallback component displayed when a <see cref="Timeline"/> has no items.
/// </summary>
public partial class TimelineEmpty : ComponentBase
{
    /// <summary>Gets or sets custom empty state content. Falls back to a default message when null.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => ClassNames.cn(
        "flex flex-col items-center justify-center p-8 text-center",
        Class
    );
}
