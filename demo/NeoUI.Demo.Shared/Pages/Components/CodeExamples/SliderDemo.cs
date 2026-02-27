namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SliderDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _sliderProps =
            [
                new("Value",    "@bind-Value / double", "0",     "Current slider value."),
                new("Min",      "double",               "0",     "Minimum value."),
                new("Max",      "double",               "100",   "Maximum value."),
                new("Step",     "double",               "1",     "Step increment."),
                new("Disabled", "bool",                 "false", "Disables the slider."),
                new("Class",    "string?",              null,    "Additional CSS classes."),
            ];

        private const string _basicCode =
                """
                <Slider Value="50" />
                """;

        private const string _rangeCode =
                """
                <Slider Value="100" Min="0" Max="200" />
                """;

        private const string _stepCode =
                """
                <Slider Value="50" Min="0" Max="100" Step="5" />
                """;

        private const string _disabledCode =
                """
                <Slider Value="50" Disabled="true" />
                """;

        private const string _usageCode =
                """
                <!-- Volume control -->
                <div class="flex items-center gap-4">
                    <LucideIcon Name="volume-2" Class="h-5 w-5" />
                    <Slider @bind-Value="volumeValue" Min="0" Max="100" Class="flex-1" />
                    <span class="text-sm">@volumeValue%</span>
                </div>

                <!-- Price filter -->
                <Slider @bind-Value="priceValue" Min="0" Max="1000" Step="10" />
                <p class="text-sm text-muted-foreground">Maximum price: $@priceValue</p>
                """;
    }
}
