namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class BottomNavDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _navProps =
        [
            new("ActiveTab",        "string?",          null,               "Value of the currently active tab."),
            new("ActiveTabChanged", "EventCallback&lt;string?&gt;", null,  "Fires when the active tab changes. Use @bind-ActiveTab for two-way binding."),
            new("ChildContent",     "RenderFragment?",  null,               "BottomNavItem children."),
            new("Fixed",            "bool",             "true",             "Pins the nav to the viewport bottom. Set false when scoped inside a container."),
            new("AriaLabel",        "string",           "\"Main navigation\"", "Accessible label for the nav landmark."),
            new("Class",            "string?",          null,               "Additional CSS classes."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _itemProps =
        [
            new("Value",          "string",           "(required)",  "Unique identifier matched against BottomNav.ActiveTab."),
            new("Icon",           "string?",          null,          "Lucide icon name (e.g. &quot;house&quot;). Ignored when IconContent is provided."),
            new("IconContent",    "RenderFragment?",  null,          "Custom icon fragment — use for SVG, avatars, or styled icons."),
            new("Label",          "string?",          null,          "Text label displayed below the icon."),
            new("BadgeCount",     "int",              "0",           "Notification badge count. Hidden when zero unless ShowZeroBadge is true."),
            new("MaxBadgeCount",  "int",              "99",          "Counts above this show as &quot;99+&quot;."),
            new("ShowZeroBadge",  "bool",             "false",       "Show badge even when BadgeCount is zero."),
            new("OnClick",        "EventCallback",    null,          "Additional click handler invoked alongside the tab switch."),
            new("Class",          "string?",          null,          "Additional CSS classes on the button element."),
        ];

        private const string _basicCode =
            """
            <BottomNav @bind-ActiveTab="@_tab">
                <BottomNavItem Value="home"    Icon="house"   Label="Home" />
                <BottomNavItem Value="search"  Icon="search"  Label="Search" />
                <BottomNavItem Value="inbox"   Icon="inbox"   Label="Inbox" />
                <BottomNavItem Value="profile" Icon="user"    Label="Profile" />
            </BottomNav>

            @code {
                private string _tab = "home";
            }
            """;

        private const string _badgeCode =
            """
            <BottomNav @bind-ActiveTab="@_tab">
                <BottomNavItem Value="home"     Icon="house"          Label="Home" />
                <BottomNavItem Value="messages" Icon="message-circle" Label="Messages" BadgeCount="5" />
                <BottomNavItem Value="activity" Icon="bell"           Label="Activity" BadgeCount="124" MaxBadgeCount="99" />
                <BottomNavItem Value="profile"  Icon="user"           Label="Profile" />
            </BottomNav>
            """;

        private const string _customIconCode =
            """
            <BottomNav @bind-ActiveTab="@_tab">
                <BottomNavItem Value="home" Label="Home">
                    <IconContent><LucideIcon Name="house" Class="h-5 w-5" /></IconContent>
                </BottomNavItem>
                <BottomNavItem Value="create" Label="Create">
                    <IconContent>
                        <div class="bg-primary rounded-full p-1">
                            <LucideIcon Name="plus" Class="h-4 w-4 text-primary-foreground" />
                        </div>
                    </IconContent>
                </BottomNavItem>
                <BottomNavItem Value="profile" Label="You">
                    <IconContent>
                        <Avatar Class="h-6 w-6">
                            <AvatarFallback Class="text-xs">JP</AvatarFallback>
                        </Avatar>
                    </IconContent>
                </BottomNavItem>
            </BottomNav>
            """;
    }
}
