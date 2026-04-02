using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// An icon sub-component for use inside a <see cref="Badge"/>.
/// Renders a properly-sized icon span that aligns with the badge text.
/// </summary>
/// <example>
/// <code>
/// &lt;Badge&gt;
///     &lt;BadgeIcon&gt;&lt;LucideIcon Name="check" /&gt;&lt;/BadgeIcon&gt;
///     Verified
/// &lt;/Badge&gt;
/// </code>
/// </example>
public partial class BadgeIcon : ComponentBase
{
    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Gets or sets the icon content (typically a LucideIcon or SVG).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "-ml-0.5 mr-1 size-3.5 [&_svg]:size-full",
        Class
    );
}
