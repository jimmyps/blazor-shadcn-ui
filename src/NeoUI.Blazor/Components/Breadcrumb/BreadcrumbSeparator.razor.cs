using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A separator component for breadcrumb items.
/// </summary>
public partial class BreadcrumbSeparator : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the breadcrumb separator.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets custom content for the separator. If not provided, a default chevron icon is used.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the breadcrumb separator element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "[&>svg]:size-3.5",
        Class
    );
}
