using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Pagination;

/// <summary>
/// A list item component for pagination.
/// </summary>
public partial class PaginationItem : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the pagination item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the child content to render inside the pagination item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(Class);
}
