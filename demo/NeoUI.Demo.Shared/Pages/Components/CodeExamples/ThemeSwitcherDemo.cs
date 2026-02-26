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
    }
}
