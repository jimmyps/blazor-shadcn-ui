namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DialogServiceDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _dialogServiceProps =
            [
                new("ConfirmDeleteAsync", "Task<bool>",        null, "Shows a destructive delete confirmation with item name and optional message."),
                new("ConfirmRemoveAsync", "Task<bool>",        null, "Shows a remove confirmation dialog."),
                new("WarnAsync",          "Task<bool>",        null, "Shows a warning confirmation dialog."),
                new("AlertAsync",         "Task",              null, "Shows an informational alert with an OK button."),
                new("SuccessAsync",       "Task",              null, "Shows a success notification dialog."),
                new("ErrorAsync",         "Task",              null, "Shows an error dialog."),
                new("ShowAsync",          "Task<DialogResult>",null, "Shows a fully customizable dialog using DialogOptions."),
            ];

        private const string _quickActionsCode =
                """
                @inject DialogService Dialog

                // Delete confirmation
                if (await Dialog.ConfirmDeleteAsync("User Account"))
                {
                    await DeleteUser();
                }

                // Success notification
                await Dialog.SuccessAsync("Saved", "Your changes have been saved.");

                // Error notification
                await Dialog.ErrorAsync("Error", "Something went wrong.");
                """;

        private const string _customDialogsCode =
                """
                var result = await Dialog.ShowAsync(new DialogOptions
                {
                    Title = "Save Changes?",
                    Message = "Do you want to save your changes?",
                    Icon = "save",
                    Variant = DialogVariant.Warning,
                    Buttons = DialogButtons.YesNoCancel,
                    ConfirmText = "Save",
                    CancelText = "Don't Save",
                    AlternateText = "Cancel"
                });

                if (result == DialogResult.Confirmed)
                {
                    await SaveChanges();
                }
                """;

        private const string _architectureCode =
                """
                <!-- In MainLayout.razor or App.razor -->
                <DialogHost />

                <!-- Your app content -->
                @Body
                """;
    }
}
