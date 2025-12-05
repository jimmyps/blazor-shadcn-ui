using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// An ellipsis component to indicate collapsed breadcrumb items.
/// </summary>
/// <remarks>
/// BreadcrumbEllipsis is used when there are too many breadcrumb items
/// to display, indicating that some items have been collapsed.
/// </remarks>
public partial class BreadcrumbEllipsis : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the ellipsis.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the span element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "flex h-9 w-9 items-center justify-center",
        Class
    );
}
