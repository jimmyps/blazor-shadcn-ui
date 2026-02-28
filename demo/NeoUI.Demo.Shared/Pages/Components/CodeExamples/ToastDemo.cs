namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ToastDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _toastProps =
            [
                new("Title",        "string",       "—",      "Toast title text."),
                new("Description",  "string?",      "null",   "Additional detail text shown below the title."),
                new("Variant",      "ToastVariant", "Default","Visual style. Options: Default, Success, Warning, Error, Info."),
                new("Size",         "ToastSize",    "Default","Toast size. Options: Default, Compact."),
                new("Duration",     "TimeSpan",     "5s",     "How long the toast stays visible before auto-dismissing."),
                new("ActionLabel",  "string?",      "null",   "Text for the optional action button."),
                new("OnAction",     "Action?",      "null",   "Callback invoked when the action button is clicked."),
                new("ShowIcon",     "bool",         "true",   "Whether to show the variant icon."),
                new("PauseOnHover", "bool",         "true",   "Whether to pause auto-dismiss when hovering over the toast."),
            ];

        private const string _defaultCode = """
                @inject IToastService ToastService

                <Button @onclick="ShowToast">Show Toast</Button>

                @code {
                    private void ShowToast()
                    {
                        ToastService.Show("Notification", "This is a default toast message.");
                    }
                }
                """;

        private const string _actionCode = """
                private void ShowToastWithAction()
                {
                    ToastService.Show(new ToastOptions
                    {
                        Title = "Event scheduled",
                        Description = "Your meeting has been scheduled for tomorrow at 10 AM.",
                        ActionLabel = "Undo",
                        OnAction = () => ToastService.Info("Undone", "The event has been cancelled."),
                        Duration = TimeSpan.FromSeconds(8)
                    });
                }
                """;

        private const string _durationCode = """
                // Short: 2 seconds
                ToastService.Show("Quick message", "Disappears in 2 seconds.", ToastVariant.Default, TimeSpan.FromSeconds(2));

                // Long: 10 seconds
                ToastService.Show("Long message", "Stays for 10 seconds.", ToastVariant.Default, TimeSpan.FromSeconds(10));
                """;

        private const string _multipleCode = """
                ToastService.Info("First", "This is the first toast.");
                ToastService.Success("Second", "This is the second toast.");
                ToastService.Warning("Third", "This is the third toast.");
                """;

        private const string _dismissAllCode = """
                ToastService.DismissAll();
                """;

        private const string _variantsCode = """
                ToastService.Show("Default", "Standard notification.");
                ToastService.Success("Success!", "Your changes have been saved.");
                ToastService.Error("Error", "Something went wrong.");
                ToastService.Warning("Warning", "This action cannot be undone.");
                ToastService.Info("Info", "You can customize the toast duration.");
                """;

        private const string _sizesCode = """
                // Default size
                ToastService.Show(new ToastOptions
                {
                    Title = "Default Size",
                    Variant = ToastVariant.Success,
                    Size = ToastSize.Default
                });

                // Compact size
                ToastService.Show(new ToastOptions
                {
                    Title = "Compact Size",
                    Variant = ToastVariant.Info,
                    Size = ToastSize.Compact
                });
                """;

        private const string _iconsCode = """
                // With icon (default)
                ToastService.Show(new ToastOptions { Title = "With Icon", Variant = ToastVariant.Success, ShowIcon = true });

                // Without icon
                ToastService.Show(new ToastOptions { Title = "Without Icon", Variant = ToastVariant.Warning, ShowIcon = false });
                """;

        private const string _pauseCode = """
                // Pauses on hover (default)
                ToastService.Show(new ToastOptions
                {
                    Title = "Pause on Hover",
                    Variant = ToastVariant.Info,
                    PauseOnHover = true,
                    Duration = TimeSpan.FromSeconds(5)
                });

                // Always counts down
                ToastService.Show(new ToastOptions
                {
                    Title = "No Pause",
                    Variant = ToastVariant.Warning,
                    PauseOnHover = false,
                    Duration = TimeSpan.FromSeconds(5)
                });
                """;

        private const string _customCode = """
                ToastService.Show(new ToastOptions
                {
                    Title = "Fully Customized",
                    Description = "Compact size, custom icon, pause on hover, and action button.",
                    Variant = ToastVariant.Success,
                    Size = ToastSize.Compact,
                    ShowIcon = true,
                    PauseOnHover = true,
                    ActionLabel = "View Details",
                    OnAction = () => ToastService.Info("Details", "Action button clicked!"),
                    Duration = TimeSpan.FromSeconds(8)
                });
                """;
    }
}
