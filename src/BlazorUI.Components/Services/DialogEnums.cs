namespace BlazorUI.Components.Services;

/// <summary>
/// Visual variant of the dialog
/// </summary>
public enum DialogVariant
{
    /// <summary>
    /// Default/neutral style
    /// </summary>
    Default,

    /// <summary>
    /// Destructive action (red, delete, remove)
    /// </summary>
    Destructive,

    /// <summary>
    /// Warning (yellow/orange)
    /// </summary>
    Warning,

    /// <summary>
    /// Informational (blue)
    /// </summary>
    Info,

    /// <summary>
    /// Success (green)
    /// </summary>
    Success
}

/// <summary>
/// Button configuration for dialogs
/// </summary>
public enum DialogButtons
{
    /// <summary>
    /// OK and Cancel buttons (default)
    /// </summary>
    OkCancel,

    /// <summary>
    /// Yes and No buttons
    /// </summary>
    YesNo,

    /// <summary>
    /// Yes, No, and Cancel buttons (3-way choice)
    /// </summary>
    YesNoCancel,

    /// <summary>
    /// OK button only (for alerts)
    /// </summary>
    OkOnly
}

/// <summary>
/// User's response to a dialog
/// </summary>
public enum DialogResult
{
    /// <summary>
    /// User confirmed (OK, Yes)
    /// </summary>
    Confirmed,

    /// <summary>
    /// User cancelled (Cancel, No)
    /// </summary>
    Cancelled,

    /// <summary>
    /// User selected third/alternate option (e.g., "Don't Save")
    /// </summary>
    Alternate
}
