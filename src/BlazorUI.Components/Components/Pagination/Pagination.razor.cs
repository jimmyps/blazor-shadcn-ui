using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Pagination;

/// <summary>
/// A pagination component for navigating through pages of results.
/// </summary>
/// <remarks>
/// <para>
/// The Pagination component provides navigation controls for paginated content.
/// It follows the shadcn/ui design system with accessibility support.
/// </para>
/// <para>
/// Features:
/// - Composable with PaginationContent, PaginationItem, PaginationLink
/// - Semantic HTML with nav and aria-label
/// - Support for previous/next arrows
/// - Ellipsis for long page ranges
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Pagination&gt;
///     &lt;PaginationContent&gt;
///         &lt;PaginationItem&gt;
///             &lt;PaginationPrevious Href="/page/1" /&gt;
///         &lt;/PaginationItem&gt;
///         &lt;PaginationItem&gt;
///             &lt;PaginationLink Href="/page/1"&gt;1&lt;/PaginationLink&gt;
///         &lt;/PaginationItem&gt;
///         &lt;PaginationItem&gt;
///             &lt;PaginationLink Href="/page/2" IsActive="true"&gt;2&lt;/PaginationLink&gt;
///         &lt;/PaginationItem&gt;
///         &lt;PaginationItem&gt;
///             &lt;PaginationNext Href="/page/3" /&gt;
///         &lt;/PaginationItem&gt;
///     &lt;/PaginationContent&gt;
/// &lt;/Pagination&gt;
/// </code>
/// </example>
public partial class Pagination : ComponentBase
{
    /// <summary>
    /// Gets or sets the aria-label for the pagination navigation.
    /// </summary>
    [Parameter]
    public string AriaLabel { get; set; } = "pagination";

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the pagination.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the pagination.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the pagination element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "mx-auto flex w-full justify-center",
        Class
    );
}
