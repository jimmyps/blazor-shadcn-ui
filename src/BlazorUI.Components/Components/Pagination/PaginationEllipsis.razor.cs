using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Pagination;

/// <summary>
/// An ellipsis component for pagination to indicate skipped pages.
/// </summary>
public partial class PaginationEllipsis : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the pagination ellipsis.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => ClassNames.cn(
        "flex h-10 w-10 items-center justify-center",
        Class
    );
}
