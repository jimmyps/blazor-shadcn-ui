using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// An icon component for an empty state.
/// </summary>
public partial class EmptyIcon : ComponentBase
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-muted",
        "[&>svg]:h-10 [&>svg]:w-10 [&>svg]:text-muted-foreground",
        Class
    );
}
