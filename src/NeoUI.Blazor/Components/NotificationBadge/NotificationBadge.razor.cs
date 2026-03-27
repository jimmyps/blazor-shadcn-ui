using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// Wraps any element and overlays a small notification count badge in its top-right corner.
/// </summary>
/// <remarks>
/// <para>
/// NotificationBadge is a positioning wrapper — it renders its <see cref="ChildContent"/>
/// inside a <c>relative</c> span and places an absolutely-positioned count chip in the
/// top-right corner. It intentionally does not replace <see cref="Badge"/>, which is an
/// inline label component. Use NotificationBadge when you need a count indicator on top of
/// an icon, avatar, or button.
/// </para>
/// <para>
/// Common uses:
/// - Cart icon with item count
/// - Notification bell with unread count
/// - Sidebar menu item with pending actions
/// - Avatar with online/offline dot indicator
/// </para>
/// <para>
/// Compatible with .NET MAUI Blazor Hybrid apps.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Cart icon with count --&gt;
/// &lt;NotificationBadge Count="@cartCount"&gt;
///     &lt;Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon"&gt;
///         &lt;LucideIcon Name="shopping-cart" Size="20" /&gt;
///     &lt;/Button&gt;
/// &lt;/NotificationBadge&gt;
///
/// &lt;!-- Dot-only indicator (no number) --&gt;
/// &lt;NotificationBadge Count="1" Dot="true"&gt;
///     &lt;Avatar&gt;&lt;AvatarFallback&gt;JD&lt;/AvatarFallback&gt;&lt;/Avatar&gt;
/// &lt;/NotificationBadge&gt;
///
/// &lt;!-- Custom colour --&gt;
/// &lt;NotificationBadge Count="3" Variant="NotificationBadgeVariant.Primary"&gt;
///     &lt;LucideIcon Name="bell" Size="20" /&gt;
/// &lt;/NotificationBadge&gt;
/// </code>
/// </example>
public partial class NotificationBadge : ComponentBase
{
    /// <summary>
    /// Gets or sets the count to display on the badge.
    /// Badge is hidden when zero unless <see cref="ShowZero"/> is true.
    /// </summary>
    [Parameter]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the maximum number shown before truncating to "+".
    /// Defaults to 99 — counts above 99 display as "99+".
    /// </summary>
    [Parameter]
    public int Max { get; set; } = 99;

    /// <summary>
    /// Gets or sets whether the badge is shown when <see cref="Count"/> is zero.
    /// Defaults to false.
    /// </summary>
    [Parameter]
    public bool ShowZero { get; set; }

    /// <summary>
    /// Gets or sets whether to render a small dot indicator instead of the numeric count.
    /// Useful for "has unread" signals where the exact count is not important.
    /// Defaults to false.
    /// </summary>
    [Parameter]
    public bool Dot { get; set; }

    /// <summary>
    /// Gets or sets the colour variant of the badge.
    /// Defaults to <see cref="NotificationBadgeVariant.Destructive"/>.
    /// </summary>
    [Parameter]
    public NotificationBadgeVariant Variant { get; set; } = NotificationBadgeVariant.Destructive;

    /// <summary>
    /// Gets or sets additional CSS classes on the badge chip.
    /// </summary>
    [Parameter]
    public string? BadgeClass { get; set; }

    /// <summary>
    /// Gets or sets the element to be wrapped. Typically an icon button or avatar.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private int DisplayableCount => Math.Max(0, Count);

    private bool IsVisible => DisplayableCount > 0 || ShowZero;

    private string DisplayCount => DisplayableCount > Max ? $"{Max}+" : DisplayableCount.ToString();

    private string AriaLabel => Dot
        ? "Has notifications"
        : $"{DisplayCount} notifications";

    private string BadgeCssClass => ClassNames.cn(
        "absolute -top-1 -right-1",
        "flex items-center justify-center",
        "rounded-full font-bold leading-none",
        Dot
            ? "size-2.5"
            : "min-w-[16px] h-4 px-1 text-[9px]",
        Variant switch
        {
            NotificationBadgeVariant.Primary
                => "bg-primary text-primary-foreground",
            NotificationBadgeVariant.Success
                => "bg-green-500 text-white",
            _   // Destructive (default)
                => "bg-destructive text-destructive-foreground",
        },
        BadgeClass
    );
}
