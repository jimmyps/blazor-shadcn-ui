using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A single tab item inside a <see cref="BottomNav"/>.
/// </summary>
/// <remarks>
/// Displays an icon (via <see cref="Icon"/> name or <see cref="IconContent"/>
/// render fragment) and an optional text label. Highlights when its
/// <see cref="Value"/> matches the parent <see cref="BottomNav.ActiveTab"/>.
/// </remarks>
/// <example>
/// <code>
/// &lt;!-- Icon name shorthand (uses LucideIcon) --&gt;
/// &lt;BottomNavItem Value="home" Icon="house" Label="Home" /&gt;
///
/// &lt;!-- Custom icon render fragment --&gt;
/// &lt;BottomNavItem Value="cart" Label="Cart" BadgeCount="@cartCount"&gt;
///     &lt;IconContent&gt;
///         &lt;LucideIcon Name="shopping-cart" Size="24" /&gt;
///     &lt;/IconContent&gt;
/// &lt;/BottomNavItem&gt;
/// </code>
/// </example>
public partial class BottomNavItem : ComponentBase
{
    [CascadingParameter]
    private BottomNav? Parent { get; set; }

    /// <summary>
    /// Gets or sets the unique value identifying this tab.
    /// Matched against <see cref="BottomNav.ActiveTab"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public string Value { get; set; } = default!;

    /// <summary>
    /// Gets or sets the Lucide icon name to display (e.g. "house", "search").
    /// Ignored when <see cref="IconContent"/> is provided.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets a custom icon render fragment. Takes precedence over <see cref="Icon"/>.
    /// </summary>
    [Parameter]
    public RenderFragment? IconContent { get; set; }

    /// <summary>
    /// Gets or sets the label text displayed beneath the icon.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the notification badge count.
    /// Badge is hidden when zero unless <see cref="ShowZeroBadge"/> is true.
    /// </summary>
    [Parameter]
    public int BadgeCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum count to display before showing "+".
    /// Defaults to 99 (shows "99+" for values above 99).
    /// </summary>
    [Parameter]
    public int MaxBadgeCount { get; set; } = 99;

    /// <summary>
    /// Gets or sets whether the badge is shown even when <see cref="BadgeCount"/> is zero.
    /// </summary>
    [Parameter]
    public bool ShowZeroBadge { get; set; }

    /// <summary>
    /// Gets or sets an additional click handler invoked alongside the tab switch.
    /// </summary>
    [Parameter]
    public EventCallback OnClick { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes on the button element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private bool IsActive => Parent?.IsActive(Value) ?? false;

    private string CssClass => ClassNames.cn(
        "flex flex-1 flex-col items-center justify-center gap-0.5",
        "min-w-0 py-2 px-1 transition-colors",
        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-inset",
        IsActive
            ? "text-primary"
            : "text-muted-foreground hover:text-foreground",
        Class
    );

    private string BadgeCssClass => ClassNames.cn(
        "absolute -top-1 -right-1.5",
        "flex items-center justify-center",
        "min-w-[16px] h-4 px-1 rounded-full",
        "bg-destructive text-destructive-foreground",
        "text-[9px] font-bold leading-none"
    );

    private async Task HandleClick()
    {
        if (Parent is not null)
            await Parent.SetActiveTabAsync(Value);

        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync();
    }
}
