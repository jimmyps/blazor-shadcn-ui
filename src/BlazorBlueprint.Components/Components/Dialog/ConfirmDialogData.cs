namespace BlazorBlueprint.Components;

/// <summary>
/// Represents a pending confirm dialog managed by <see cref="DialogService"/>.
/// </summary>
public class ConfirmDialogData
{
    /// <summary>
    /// Unique identifier for the dialog instance.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The dialog title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The dialog description/message.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Customization options for the dialog.
    /// </summary>
    public ConfirmDialogOptions Options { get; set; } = new();

    /// <summary>
    /// The TaskCompletionSource that resolves when the user responds.
    /// </summary>
    internal TaskCompletionSource<bool> Tcs { get; set; } = new();
}
