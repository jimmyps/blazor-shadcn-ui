using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// An empty state component for displaying when no data is available.
/// </summary>
/// <remarks>
/// <para>
/// The Empty component provides a consistent way to display empty states
/// in tables, lists, or other data displays. It follows the shadcn/ui design system.
/// </para>
/// <para>
/// Features:
/// - Composable with icon, title, description, and actions
/// - Centered layout with appropriate spacing
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Empty&gt;
///     &lt;EmptyIcon&gt;
///         &lt;!-- Icon content --&gt;
///     &lt;/EmptyIcon&gt;
///     &lt;EmptyTitle&gt;No items found&lt;/EmptyTitle&gt;
///     &lt;EmptyDescription&gt;There are no items to display.&lt;/EmptyDescription&gt;
///     &lt;EmptyActions&gt;
///         &lt;Button&gt;Add Item&lt;/Button&gt;
///     &lt;/EmptyActions&gt;
/// &lt;/Empty&gt;
/// </code>
/// </example>
public partial class Empty : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the empty state.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the empty state.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the empty state element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "flex min-h-[400px] flex-col items-center justify-center rounded-lg border border-dashed p-8 text-center",
        Class
    );
}
