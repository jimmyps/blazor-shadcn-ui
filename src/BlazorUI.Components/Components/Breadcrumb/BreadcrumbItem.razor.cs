using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A list item component for a breadcrumb.
/// </summary>
public partial class BreadcrumbItem : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the breadcrumb item.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the breadcrumb item element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "inline-flex items-center gap-1.5",
        Class
    );
}
