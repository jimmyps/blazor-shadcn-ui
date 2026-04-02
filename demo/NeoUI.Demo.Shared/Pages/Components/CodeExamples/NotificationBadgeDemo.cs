namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class NotificationBadgeDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _props =
        [
            new("Count",        "int",                          "0",             "Count to display. Hidden when zero unless ShowZero is true."),
            new("Max",          "int",                          "99",            "Counts above this threshold show as &quot;99+&quot;."),
            new("ShowZero",     "bool",                         "false",         "Show the badge even when Count is zero."),
            new("Dot",          "bool",                         "false",         "Render a small dot indicator instead of a numeric count."),
            new("Variant",      "NotificationBadgeVariant",     "Destructive",   "Badge colour: Destructive (red), Primary (brand), or Success (green)."),
            new("BadgeClass",   "string?",                      null,            "Additional CSS classes on the badge chip itself."),
            new("ChildContent", "RenderFragment?",              null,            "The element to wrap (icon button, avatar, etc.)."),
        ];

        private const string _basicCode =
            """
            <NotificationBadge Count="3">
                <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon">
                    <LucideIcon Name="bell" Class="h-5 w-5" />
                </Button>
            </NotificationBadge>
            """;

        private const string _variantsCode =
            """
            <NotificationBadge Count="5" Variant="NotificationBadgeVariant.Destructive">...</NotificationBadge>
            <NotificationBadge Count="5" Variant="NotificationBadgeVariant.Primary">...</NotificationBadge>
            <NotificationBadge Count="5" Variant="NotificationBadgeVariant.Success">...</NotificationBadge>
            """;

        private const string _dotCode =
            """
            <NotificationBadge Dot="true">
                <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon">
                    <LucideIcon Name="bell" Class="h-5 w-5" />
                </Button>
            </NotificationBadge>
            """;

        private const string _maxCode =
            """
            @* Default Max=99: count 100 shows as "99+" *@
            <NotificationBadge Count="100">...</NotificationBadge>

            @* Override Max: count 100 with Max=9 shows as "9+" *@
            <NotificationBadge Count="100" Max="9">...</NotificationBadge>
            """;

        private const string _showZeroCode =
            """
            @* Hidden by default *@
            <NotificationBadge Count="0">...</NotificationBadge>

            @* Visible with ShowZero *@
            <NotificationBadge Count="0" ShowZero="true">...</NotificationBadge>
            """;
    }
}
