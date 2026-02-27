namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ThemeSwitcherDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _themeSwitcherProps =
            [
                new("TriggerClass", "string?", "null", "Additional CSS classes merged onto the trigger Button."),
                new("PopoverContentClass", "string?", "null", "Additional CSS classes merged onto the PopoverContent panel."),
                new("Align", "PopoverAlign", "End", "Popover alignment relative to trigger."),
                new("Strategy", "PositioningStrategy", "Fixed", "Use Fixed inside transformed/overflow-hidden containers; Absolute otherwise."),
                new("ZIndex", "int", "ZIndexLevels.PopoverContent", "Z-index for the popover panel."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _themeServiceProps =
            [
                new("CurrentBaseColor", "BaseColor", "—", "Currently active base color scheme."),
                new("CurrentPrimaryColor", "PrimaryColor", "—", "Currently active primary accent color."),
                new("IsDarkMode", "bool", "—", "Whether dark mode is currently active."),
                new("OnThemeChanged", "event Action", "—", "Fired whenever the theme changes."),
                new("InitializeAsync()", "Task", "—", "Reads persisted preferences from localStorage."),
                new("SetBaseColorAsync(BaseColor)", "Task", "—", "Changes and persists the base color scheme."),
                new("SetPrimaryColorAsync(PrimaryColor)", "Task", "—", "Changes and persists the primary accent color."),
                new("SetThemeAsync(bool)", "Task", "—", "Toggles dark mode and persists the preference."),
            ];

        private const string _demoCode = """
                <ThemeSwitcher Align="PopoverAlign.Start" />
                <DarkModeToggle />
                """;

        private const string _darkModeToggleCode = """
                <DarkModeToggle />
                """;

        private const string _customisationCode = """
                <!-- Extra ring on trigger -->
                <ThemeSwitcher TriggerClass="rounded-full ring-2 ring-primary ring-offset-2 ring-offset-background" />

                <!-- Narrower popover panel -->
                <ThemeSwitcher PopoverContentClass="!w-72" />
                """;

        private const string _coreStylesheetCode =
            """<link href="_content/NeoUI.Blazor/components.css" rel="stylesheet" />""";

        private const string _baseColorsCode =
            """
            <!-- Base color themes (include only those you need) -->
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/zinc.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/slate.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/gray.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/neutral.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/stone.css" />
            """;

        private const string _primaryColorsCode =
            """
            <!-- Primary color themes (include only those you need) -->
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/red.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/rose.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/orange.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/amber.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/yellow.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/lime.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/green.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/emerald.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/teal.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/cyan.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/sky.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/blue.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/indigo.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/violet.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/purple.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/fuchsia.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/pink.css" />
            """;

        private const string _themeJsCode =
            """
            <!-- Theme JS — must come before Blazor boots to prevent FOUC -->
            <script src="_content/NeoUI.Blazor/js/theme.js"></script>
            <script>
                window.theme.initialize();
            </script>
            """;

        private const string _serviceRegistrationCode =
            """
            // Program.cs
            builder.Services.AddNeoUIPrimitives();
            builder.Services.AddNeoUIComponents(); // registers ThemeService
            """;
    }
}
