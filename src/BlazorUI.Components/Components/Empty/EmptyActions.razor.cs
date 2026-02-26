using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// An actions container component for an empty state.
/// </summary>
public partial class EmptyActions : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the empty actions container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the child content to render inside the empty actions.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "flex items-center justify-center gap-2",
        Class
    );
}
