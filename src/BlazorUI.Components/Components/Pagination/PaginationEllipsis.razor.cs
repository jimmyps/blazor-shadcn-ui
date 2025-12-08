using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Pagination;

/// <summary>
/// An ellipsis component for pagination to indicate skipped pages.
/// </summary>
public partial class PaginationEllipsis : ComponentBase
{
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => ClassNames.cn(
        "flex h-9 w-9 items-center justify-center",
        Class
    );
}
