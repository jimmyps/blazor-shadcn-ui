using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// An actions container component for an empty state.
/// </summary>
public partial class EmptyActions : ComponentBase
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "flex items-center justify-center gap-2",
        Class
    );
}
