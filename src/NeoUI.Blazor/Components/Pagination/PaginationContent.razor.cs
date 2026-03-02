using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Pagination;

/// <summary>
/// A container component for pagination items.
/// </summary>
public partial class PaginationContent : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the pagination content container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the child content to render inside the pagination content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "flex flex-row items-center gap-1",
        Class
    );
}
