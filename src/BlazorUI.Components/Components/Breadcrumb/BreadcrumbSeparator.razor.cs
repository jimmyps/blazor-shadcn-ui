using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Breadcrumb;

/// <summary>
/// A separator between breadcrumb items.
/// </summary>
/// <remarks>
/// BreadcrumbSeparator displays a visual separator (default: chevron-right icon)
/// between breadcrumb items. You can provide custom content via ChildContent.
/// </remarks>
public partial class BreadcrumbSeparator : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the separator.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets custom separator content. If not provided, a chevron-right icon is used.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the separator element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "[&>svg]:size-3.5",
        Class
    );
}
