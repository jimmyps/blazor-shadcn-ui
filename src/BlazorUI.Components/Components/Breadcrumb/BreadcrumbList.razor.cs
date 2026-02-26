using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// An ordered list component for breadcrumb items.
/// </summary>
public partial class BreadcrumbList : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb list.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the breadcrumb list.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the breadcrumb list element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "flex flex-wrap items-center gap-1.5 break-words text-sm text-muted-foreground sm:gap-2.5",
        Class
    );
}
