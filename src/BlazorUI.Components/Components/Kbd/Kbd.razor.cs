using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Kbd;

/// <summary>
/// A keyboard key badge component for displaying keyboard shortcuts.
/// </summary>
/// <remarks>
/// <para>
/// The Kbd component provides a styled way to display keyboard keys or shortcuts.
/// It follows the shadcn/ui design system.
/// </para>
/// <para>
/// Features:
/// - Semantic HTML with kbd element
/// - Styled to look like keyboard keys
/// - Accessible keyboard shortcut display
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Kbd&gt;Ctrl&lt;/Kbd&gt; + &lt;Kbd&gt;S&lt;/Kbd&gt;
///
/// &lt;Kbd&gt;⌘ K&lt;/Kbd&gt;
/// </code>
/// </example>
public partial class Kbd : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes to apply to the kbd element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the kbd element.
    /// </summary>
    /// <remarks>
    /// Typically contains a single key name (e.g., "Ctrl", "Shift", "Enter")
    /// or a key symbol (e.g., "⌘", "⌥", "⇧").
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the computed CSS classes for the kbd element.
    /// </summary>
    private string CssClass => ClassNames.cn(
        "pointer-events-none inline-flex h-5 select-none items-center gap-1 rounded border bg-muted px-1.5 font-mono text-[10px] font-medium text-muted-foreground opacity-100",
        Class
    );
}
