namespace BlazorBlueprint.Components;

/// <summary>
/// Service for showing programmatic confirm dialogs.
/// Register as scoped in DI for Blazor Server user isolation.
/// </summary>
public class DialogService
{
    private readonly List<ConfirmDialogData> dialogs = new();

    /// <summary>
    /// Event fired when the dialog collection changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Gets the current list of pending dialogs.
    /// </summary>
    public IReadOnlyList<ConfirmDialogData> Dialogs => dialogs.AsReadOnly();

    /// <summary>
    /// Shows a confirm dialog and returns true if confirmed, false if cancelled.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="description">The dialog description/message.</param>
    /// <param name="options">Optional customization options for button labels and variant.</param>
    /// <returns>True if the user clicked confirm, false if cancelled.</returns>
    public Task<bool> Confirm(string title, string? description = null, ConfirmDialogOptions? options = null)
    {
        var data = new ConfirmDialogData
        {
            Title = title,
            Description = description,
            Options = options ?? new ConfirmDialogOptions()
        };

        dialogs.Add(data);
        OnChange?.Invoke();

        return data.Tcs.Task;
    }

    /// <summary>
    /// Resolves a dialog with the given result and removes it from the list.
    /// Called by DialogProvider when the user clicks confirm or cancel.
    /// </summary>
    /// <param name="id">The dialog ID to resolve.</param>
    /// <param name="result">True for confirm, false for cancel.</param>
    internal void Resolve(string id, bool result)
    {
        var dialog = dialogs.FirstOrDefault(d => d.Id == id);
        if (dialog != null)
        {
            dialogs.Remove(dialog);
            dialog.Tcs.TrySetResult(result);
            OnChange?.Invoke();
        }
    }

    /// <summary>
    /// Cancels all pending dialogs, resolving each with false.
    /// </summary>
    internal void CancelAll()
    {
        foreach (var dialog in dialogs.ToList())
        {
            dialog.Tcs.TrySetResult(false);
        }

        dialogs.Clear();
        OnChange?.Invoke();
    }
}
