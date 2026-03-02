using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// An icon component for an empty state.
/// </summary>
public partial class EmptyIcon : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the empty icon.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the child content to render inside the empty icon.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-muted",
        "[&>svg]:h-10 [&>svg]:w-10 [&>svg]:text-muted-foreground",
        Class
    );
}
