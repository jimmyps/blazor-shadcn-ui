namespace BlazorBlueprint.Components;

/// <summary>
/// Options for customizing a confirm dialog shown via <see cref="DialogService"/>.
/// </summary>
public class ConfirmDialogOptions
{
    /// <summary>
    /// The label for the confirm/action button. Default: "Continue".
    /// </summary>
    public string ConfirmText { get; set; } = "Continue";

    /// <summary>
    /// The label for the cancel button. Default: "Cancel".
    /// </summary>
    public string CancelText { get; set; } = "Cancel";

    /// <summary>
    /// Whether the confirm button should use the destructive variant.
    /// Default: false (uses primary/default variant).
    /// </summary>
    public bool Destructive { get; set; }
}
