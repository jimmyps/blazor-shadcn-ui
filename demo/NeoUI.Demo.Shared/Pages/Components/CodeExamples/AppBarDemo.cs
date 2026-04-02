namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class AppBarDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _props =
        [
            new("Title",        "string?",          null,       "Title text centred in the bar. Ignored when TitleContent is provided."),
            new("TitleContent", "RenderFragment?",  null,       "Custom title fragment; takes precedence over Title."),
            new("OnBack",       "EventCallback",    null,       "Callback for the back button. Button only renders when this is provided."),
            new("BackLabel",    "string",           "\"Go back\"", "Accessible aria-label for the back button."),
            new("RightContent", "RenderFragment?",  null,       "Right-side slot for icon buttons, avatar, share icon, etc."),
            new("Transparent",  "bool",             "false",    "Removes background and bottom border — use over hero images."),
            new("ShowBorder",   "bool",             "true",     "Shows the bottom border. Automatically hidden when Transparent is true."),
            new("Class",        "string?",          null,       "Additional CSS classes on the header element."),
        ];

        private const string _basicCode =
            """
            <AppBar Title="My App" />
            """;

        private const string _backCode =
            """
            <AppBar Title="Order Details" OnBack="@GoBack" />

            @code {
                private void GoBack() => NavigationManager.NavigateBack();
            }
            """;

        private const string _rightCode =
            """
            <AppBar Title="Messages" OnBack="@GoBack">
                <RightContent>
                    <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon" Class="h-9 w-9">
                        <LucideIcon Name="search" Class="h-4 w-4" />
                    </Button>
                    <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon" Class="h-9 w-9">
                        <LucideIcon Name="ellipsis-vertical" Class="h-4 w-4" />
                    </Button>
                </RightContent>
            </AppBar>
            """;

        private const string _transparentCode =
            """
            <div class="relative bg-gradient-to-br from-violet-500 to-indigo-500">
                <AppBar Title="Profile"
                        Transparent="true"
                        OnBack="@GoBack"
                        Class="text-white [&_button]:text-white [&_button]:hover:bg-white/20" />
                @* hero content *@
            </div>
            """;

        private const string _customTitleCode =
            """
            <AppBar OnBack="@GoBack">
                <TitleContent>
                    <div class="flex flex-col items-center leading-tight">
                        <span class="text-sm font-semibold">Jimmy Petrus</span>
                        <span class="text-xs text-muted-foreground">Online</span>
                    </div>
                </TitleContent>
            </AppBar>
            """;
    }
}
