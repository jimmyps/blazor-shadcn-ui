namespace BlazorUI.Components.Services;

/// <summary>
/// Service for displaying dialogs, alerts, and confirmations
/// </summary>
public class DialogService
{
    private DialogHost.DialogHost? _dialogHost;
    private TaskCompletionSource<DialogResult>? _currentTask;

    /// <summary>
    /// Register the dialog host component instance
    /// </summary>
    internal void RegisterHost(DialogHost.DialogHost host)
    {
        _dialogHost = host;
    }

    /// <summary>
    /// Show a dialog with custom options and wait for user response
    /// </summary>
    public Task<DialogResult> ShowAsync(DialogOptions options)
    {
        if (_dialogHost == null)
        {
            throw new InvalidOperationException(
                "DialogHost not registered. Add <DialogHost /> to your MainLayout.razor or App.razor");
        }

        _currentTask = new TaskCompletionSource<DialogResult>();
        _dialogHost.Show(options);
        return _currentTask.Task;
    }

    /// <summary>
    /// Show a confirmation dialog and wait for user response
    /// </summary>
    public Task<DialogResult> ConfirmAsync(DialogOptions options)
    {
        return ShowAsync(options);
    }

    /// <summary>
    /// Quick delete confirmation with standard messaging
    /// </summary>
    /// <param name="itemName">Name of the item to delete</param>
    /// <param name="description">Optional custom description</param>
    /// <returns>True if user confirmed, false if cancelled</returns>
    public Task<bool> ConfirmDeleteAsync(string itemName, string? description = null)
    {
        return ShowAsync(new DialogOptions
        {
            Title = $"Delete {itemName}?",
            Message = description ?? $"Are you sure you want to delete \"{itemName}\"? This action cannot be undone.",
            Icon = "trash-2",
            Variant = DialogVariant.Destructive,
            ConfirmText = "Delete",
            CancelText = "Cancel"
        }).ContinueWith(t => t.Result == DialogResult.Confirmed);
    }

    /// <summary>
    /// Quick remove confirmation
    /// </summary>
    /// <param name="itemName">Name of the item to remove</param>
    /// <param name="description">Optional custom description</param>
    /// <returns>True if user confirmed, false if cancelled</returns>
    public Task<bool> ConfirmRemoveAsync(string itemName, string? description = null)
    {
        return ShowAsync(new DialogOptions
        {
            Title = $"Remove {itemName}?",
            Message = description ?? $"Are you sure you want to remove \"{itemName}\"?",
            Icon = "unlink",
            Variant = DialogVariant.Destructive,
            ConfirmText = "Remove",
            CancelText = "Cancel"
        }).ContinueWith(t => t.Result == DialogResult.Confirmed);
    }

    /// <summary>
    /// Show a warning confirmation dialog
    /// </summary>
    /// <returns>True if user confirmed, false if cancelled</returns>
    public Task<bool> WarnAsync(string title, string message)
    {
        return ShowAsync(new DialogOptions
        {
            Title = title,
            Message = message,
            Icon = "alert-triangle",
            Variant = DialogVariant.Warning,
            ConfirmText = "Continue",
            CancelText = "Cancel"
        }).ContinueWith(t => t.Result == DialogResult.Confirmed);
    }

    /// <summary>
    /// Show an informational alert (OK only)
    /// </summary>
    public Task AlertAsync(string title, string message, string icon = "info")
    {
        return ShowAsync(new DialogOptions
        {
            Title = title,
            Message = message,
            Icon = icon,
            Variant = DialogVariant.Info,
            Buttons = DialogButtons.OkOnly,
            ConfirmText = "OK"
        }).ContinueWith(_ => Task.CompletedTask);
    }

    /// <summary>
    /// Show a success message (OK only)
    /// </summary>
    public Task SuccessAsync(string title, string message)
    {
        return AlertAsync(title, message, icon: "check-circle");
    }

    /// <summary>
    /// Show an error message (OK only)
    /// </summary>
    public Task ErrorAsync(string title, string message)
    {
        return ShowAsync(new DialogOptions
        {
            Title = title,
            Message = message,
            Icon = "alert-circle",
            Variant = DialogVariant.Destructive,
            Buttons = DialogButtons.OkOnly,
            ConfirmText = "OK"
        }).ContinueWith(_ => Task.CompletedTask);
    }

    /// <summary>
    /// Called by the dialog host when user responds
    /// </summary>
    internal void NotifyResult(DialogResult result)
    {
        _currentTask?.SetResult(result);
        _currentTask = null;
    }
}
