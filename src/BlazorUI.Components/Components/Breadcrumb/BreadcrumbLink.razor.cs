using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A link component for a breadcrumb item.
/// </summary>
public partial class BreadcrumbLink : ComponentBase
{
    /// <summary>
    /// Gets or sets the href attribute for the link.
    /// </summary>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb link.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the breadcrumb link.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the breadcrumb link element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "transition-colors hover:text-foreground",
        Class
    );
}
