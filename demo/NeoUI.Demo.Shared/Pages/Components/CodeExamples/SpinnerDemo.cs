namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SpinnerDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _spinnerProps =
            [
                new("Size",  "SpinnerSize", "Medium", "Controls the size of the spinner."),
                new("Class", "string?",     null,     "Additional CSS classes applied to the spinner element."),
            ];

        private const string _sizesCode =
                """
                <Spinner Size="SpinnerSize.Small" />
                <Spinner Size="SpinnerSize.Medium" />
                <Spinner Size="SpinnerSize.Large" />
                """;

        private const string _usageCode =
                """
                <!-- Button loading state -->
                <button disabled class="...gap-2">
                    <Spinner Size="SpinnerSize.Small" />
                    Loading...
                </button>

                <!-- Inline loading -->
                <div class="flex items-center gap-2">
                    <Spinner Size="SpinnerSize.Small" />
                    <span>Fetching latest data...</span>
                </div>

                <!-- Card loading -->
                <div class="flex flex-col items-center gap-4 py-8">
                    <Spinner />
                    <p class="text-sm text-muted-foreground">Loading content...</p>
                </div>
                """;
    }
}
