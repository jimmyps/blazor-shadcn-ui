namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ProgressDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _progressProps =
            [
                new("Value",   "double?", null,  "Current progress value between 0 and Max."),
                new("Max",     "double",  "100", "Maximum value of the progress bar."),
                new("Class",   "string?", null,  "Additional CSS classes appended to the root element."),
            ];

        private const string _basicCode =
                """
                <Progress Value="60" />
                """;

        private const string _valuesCode =
                """
                <Progress Value="0" />
                <Progress Value="25" />
                <Progress Value="50" />
                <Progress Value="75" />
                <Progress Value="100" />
                """;

        private const string _interactiveCode =
                """
                <Progress Value="@progressValue" />

                @code {
                    private double progressValue = 60;
                }
                """;

        private const string _usageCode =
                """
                <!-- File upload -->
                <Progress Value="60" />

                <!-- Profile completion (3 of 5 steps = 60%) -->
                <Progress Value="60" />
                """;
    }
}
