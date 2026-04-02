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
                new("CurrentStyleVariant", "StyleVariant", "—", "Currently active visual style variant."),
                new("CurrentRadiusPreset", "RadiusPreset", "—", "Currently active border radius preset."),
                new("CurrentFontPreset", "FontPreset", "—", "Currently active font preset."),
                new("IsDarkMode", "bool", "—", "Whether dark mode is currently active."),
                new("OnThemeChanged", "event Action", "—", "Fired whenever the theme changes."),
                new("InitializeAsync()", "Task", "—", "Reads persisted preferences from localStorage."),
                new("SetBaseColorAsync(BaseColor)", "Task", "—", "Changes and persists the base color scheme."),
                new("SetPrimaryColorAsync(PrimaryColor)", "Task", "—", "Changes and persists the primary accent color."),
                new("SetStyleVariantAsync(StyleVariant)", "Task", "—", "Changes and persists the visual style variant."),
                new("SetRadiusPresetAsync(RadiusPreset)", "Task", "—", "Changes and persists the border radius preset."),
                new("SetFontPresetAsync(FontPreset)", "Task", "—", "Changes and persists the font preset."),
                new("SetThemeAsync(bool)", "Task", "—", "Toggles dark mode and persists the preference."),
                new("ApplyPresetAsync(NeoThemePreset)", "Task", "—", "Applies a named preset across all theme dimensions."),
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

        private const string _presetCode = """
                // Apply a built-in named preset
                await ThemeService.ApplyPresetAsync(NeoThemePreset.Luma);
                await ThemeService.ApplyPresetAsync(NeoThemePreset.Nova);

                // Build a custom preset
                var myPreset = new NeoThemePreset(
                    Name:         "Corporate",
                    BaseColor:    BaseColor.Slate,
                    PrimaryColor: PrimaryColor.Blue,
                    StyleVariant: StyleVariant.Nova,
                    RadiusPreset: RadiusPreset.Small,
                    FontPreset:   FontPreset.Inter,
                    IsDarkMode:   false);

                await ThemeService.ApplyPresetAsync(myPreset);
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
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/luma.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/mist.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/mauve.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/taupe.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/olive.css" />
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

        private const string _styleVariantsCode =
            """
            <!-- Visual style variants (include only those you need) -->
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/vega.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/nova.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/maia.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/lyra.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/mira.css" />
            """;

        private const string _radiusPresetsCode =
            """
            <!-- Radius presets (include only those you need) -->
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/none.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/small.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/large.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/full.css" />
            """;

        private const string _fontPresetsCode =
            """
            <!-- Font presets (add matching Google Fonts / Bunny Fonts <link> tags above) -->
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/fonts/inter.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/fonts/geist.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/fonts/calsans.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/fonts/dmsans.css" />
            <link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/fonts/plusjakarta.css" />
            """;

        private const string _themeJsCode =
            """
            <!-- Theme JS — must come before Blazor boots to prevent FOUC -->
            <script src="_content/NeoUI.Blazor/js/theme.js"></script>
            """;

        private const string _serviceRegistrationCode =
            """
            // Program.cs
            builder.Services.AddNeoUIPrimitives();
            builder.Services.AddNeoUIComponents(); // registers ThemeService
            """;
    }
}

