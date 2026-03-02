using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// An ellipsis component for breadcrumb items to indicate collapsed items.
/// </summary>
public partial class BreadcrumbEllipsis : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb ellipsis.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the breadcrumb ellipsis element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "flex h-9 w-9 items-center justify-center",
        Class
    );
}
