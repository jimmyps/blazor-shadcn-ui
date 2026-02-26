using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// A title component for an empty state.
/// </summary>
public partial class EmptyTitle : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the empty title.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the child content to render inside the empty title.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "mt-4 text-lg font-semibold",
        Class
    );
}
