using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A component representing the current page in a breadcrumb.
/// </summary>
public partial class BreadcrumbPage : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb page.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the breadcrumb page.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the breadcrumb page element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "font-normal text-foreground",
        Class
    );
}
