namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class RangeSliderDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _rangeSliderProps =
            [
                new("MinValue",     "double",            "0",          "The lower bound value. Use @bind-MinValue for two-way binding."),
                new("MaxValue",     "double",            "100",        "The upper bound value. Use @bind-MaxValue for two-way binding."),
                new("Min",          "double",            "0",          "The minimum allowed value."),
                new("Max",          "double",            "100",        "The maximum allowed value."),
                new("Step",         "double",            "1",          "The increment step between values."),
                new("ShowTooltips", "bool",              "true",       "Whether to show value tooltips on hover."),
                new("ShowLabels",   "bool",              "false",      "Whether to show min/max labels."),
                new("ShowTicks",    "bool",              "false",      "Whether to show tick marks."),
                new("TickInterval", "double",            "25",         "Interval between tick marks."),
                new("Orientation",  "SliderOrientation", "Horizontal", "Slider orientation: Horizontal or Vertical."),
                new("Disabled",     "bool",              "false",      "Whether the slider is disabled."),
                new("AriaLabel",    "string?",           "null",       "ARIA label for accessibility."),
            ];

        private const string _basicCode =
                """
                <RangeSlider Min="0" Max="100" />
                """;

        private const string _priceRangeCode =
                """
                <RangeSlider Min="0"
                             Max="1000"
                             Step="10"
                             ShowTooltips="true"
                             ShowLabels="true" />
                """;

        private const string _ageRangeCode =
                """
                <RangeSlider Min="18"
                             Max="100"
                             Step="1"
                             ShowTooltips="true"
                             ShowLabels="true"
                             AriaLabel="Age range selector" />
                """;

        private const string _tickMarksCode =
                """
                <RangeSlider Min="0"
                             Max="100"
                             Step="5"
                             ShowTicks="true"
                             TickInterval="25"
                             ShowTooltips="true"
                             ShowLabels="true" />
                """;

        private const string _noTooltipsCode =
                """
                <RangeSlider ShowTooltips="false"
                             ShowLabels="true" />
                """;

        private const string _noLabelsCode =
                """
                <RangeSlider ShowLabels="false"
                             ShowTooltips="true" />
                """;

        private const string _decimalStepsCode =
                """
                <RangeSlider Min="0"
                             Max="10"
                             Step="0.5"
                             ShowTooltips="true"
                             ShowLabels="true" />
                """;

        private const string _verticalCode =
                """
                <RangeSlider Orientation="SliderOrientation.Vertical"
                             ShowTooltips="true"
                             ShowLabels="true" />
                """;

        private const string _disabledCode =
                """
                <RangeSlider Disabled="true"
                             ShowTooltips="true"
                             ShowLabels="true" />
                """;

        private const string _formIntegrationCode =
                """
                <EditForm Model="@formModel" OnValidSubmit="HandleValidSubmit">
                    <DataAnnotationsValidator />
                    <div class="space-y-4">
                        <div class="space-y-2">
                            <Label>Budget Range</Label>
                            <RangeSlider Min="0"
                                         Max="1000"
                                         Step="50"
                                         ShowTooltips="true"
                                         ShowLabels="true" />
                            <ValidationMessage For="@(() => formModel.BudgetMin)" />
                            <ValidationMessage For="@(() => formModel.BudgetMax)" />
                        </div>
                        <Button Type="ButtonType.Submit">Submit</Button>
                    </div>
                </EditForm>
                """;

        private const string _keyboardCode =
                """
                <RangeSlider ShowTooltips="true"
                             ShowLabels="true" />
                """;
    }
}
