using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Pagination;

/// <summary>
/// A link component for pagination items.
/// </summary>
public partial class PaginationLink : ComponentBase
{
    [Parameter]
    public string? Href { get; set; }

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium ring-offset-background transition-colors",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
        "disabled:pointer-events-none disabled:opacity-50",
        "hover:bg-accent hover:text-accent-foreground h-10 px-4 py-2",
        IsActive ? "border border-input bg-background shadow-sm" : "",
        Class
    );
}
