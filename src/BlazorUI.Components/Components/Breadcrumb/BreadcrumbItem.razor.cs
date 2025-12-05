using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A single item in a breadcrumb trail.
/// </summary>
/// <remarks>
/// BreadcrumbItem wraps breadcrumb content (link or page) in a list item (li).
/// </remarks>
public partial class BreadcrumbItem : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the li element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "inline-flex items-center gap-1.5",
        Class
    );
}
