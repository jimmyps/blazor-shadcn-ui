using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Displays fallback content when an AvatarImage fails to load or is unavailable.
/// </summary>
/// <remarks>
/// <para>
/// AvatarFallback provides graceful degradation for avatar images. When an image
/// cannot be loaded or is not provided, this component displays alternative content
/// such as user initials, an icon, or custom markup.
/// </para>
/// <para>
/// Features:
/// - Automatic centering of content
/// - Consistent background color from design system
/// - Supports text, icons, or custom content
/// - Accessible and semantic
/// - Matches parent Avatar size automatically
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Avatar&gt;
///     &lt;AvatarImage Source="@userImageUrl" Alt="@userName" /&gt;
///     &lt;AvatarFallback&gt;@GetInitials(userName)&lt;/AvatarFallback&gt;
/// &lt;/Avatar&gt;
///
/// &lt;Avatar&gt;
///     &lt;AvatarFallback&gt;
///         &lt;LucideIcon Name="user" Size="16" /&gt;
///     &lt;/AvatarFallback&gt;
/// &lt;/Avatar&gt;
/// </code>
/// </example>
public partial class AvatarFallback : ComponentBase
{
    /// <summary>
    /// Gets or sets the content to render as fallback.
    /// </summary>
    /// <remarks>
    /// Typically contains:
    /// - User initials (e.g., "JD" for John Doe)
    /// - An icon component (e.g., LucideIcon with "user")
    /// - Custom markup or components
    /// Content is automatically centered within the avatar.
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the fallback container.
    /// </summary>
    /// <remarks>
    /// Custom classes are appended after the component's base classes,
    /// allowing for style overrides such as custom background colors.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => ClassNames.cn(
        "flex h-full w-full items-center justify-center rounded-full",
        "bg-muted text-muted-foreground font-medium",
        Class);
}
