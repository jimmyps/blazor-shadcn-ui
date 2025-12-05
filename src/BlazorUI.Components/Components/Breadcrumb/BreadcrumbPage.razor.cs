using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// The current page indicator in a breadcrumb trail.
/// </summary>
/// <remarks>
/// BreadcrumbPage represents the current location in the hierarchy.
/// It is not clickable and uses aria-current="page" for accessibility.
/// </remarks>
public partial class BreadcrumbPage : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the page indicator.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the page indicator.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the span element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "font-normal text-foreground",
        Class
    );
}
