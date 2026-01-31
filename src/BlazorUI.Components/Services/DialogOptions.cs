namespace BlazorUI.Components.Services;

/// <summary>
/// Options for customizing dialog appearance and behavior
/// </summary>
public class DialogOptions
{
    /// <summary>
    /// Dialog title
    /// </summary>
    public string Title { get; set; } = "Confirm";

    /// <summary>
    /// Dialog message/description
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Lucide icon name (e.g., "trash-2", "alert-triangle")
    /// </summary>
    public string Icon { get; set; } = "alert-circle";

    /// <summary>
    /// Visual variant of the dialog
    /// </summary>
    public DialogVariant Variant { get; set; } = DialogVariant.Default;

    /// <summary>
    /// Button configuration
    /// </summary>
    public DialogButtons Buttons { get; set; } = DialogButtons.OkCancel;

    /// <summary>
    /// Text for the confirm/primary button
    /// </summary>
    public string ConfirmText { get; set; } = "Confirm";

    /// <summary>
    /// Text for the cancel/secondary button
    /// </summary>
    public string CancelText { get; set; } = "Cancel";

    /// <summary>
    /// Text for the third/alternate button (when using YesNoCancel)
    /// </summary>
    public string? AlternateText { get; set; }
}
