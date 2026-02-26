namespace BlazorUI.Components.Toast;

/// <summary>
/// Options for displaying a toast notification.
/// </summary>
public class ToastOptions
{
    /// <summary>
    /// The unique identifier for the toast.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The title of the toast.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The description/content of the toast.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The variant/style of the toast.
    /// </summary>
    public ToastVariant Variant { get; set; } = ToastVariant.Default;

    /// <summary>
    /// How long the toast should be visible. Null means indefinite.
    /// </summary>
    public TimeSpan? Duration { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The size of the toast.
    /// Default is ToastSize.Default.
    /// </summary>
    public ToastSize Size { get; set; } = ToastSize.Default;

    /// <summary>
    /// Position override for this specific toast.
    /// If null, uses the ToastProvider's default position.
    /// </summary>
    public ToastPosition? Position { get; set; }

    /// <summary>
    /// Whether to pause auto-dismiss when hovering over the toast.
    /// Default is true.
    /// </summary>
    public bool PauseOnHover { get; set; } = true;

    /// <summary>
    /// Whether to show the variant icon.
    /// Default is true (auto-shows Success ✓, Error ✗, Warning ⚠, Info ℹ).
    /// </summary>
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Action button label.
    /// </summary>
    public string? ActionLabel { get; set; }

    /// <summary>
    /// Callback when action button is clicked.
    /// </summary>
    public Action? OnAction { get; set; }

    /// <summary>
    /// Callback when the toast is dismissed.
    /// </summary>
    public Action? OnDismiss { get; set; }

    /// <summary>
    /// Creates a default toast options instance.
    /// </summary>
    public ToastOptions()
    {
    }

    /// <summary>
    /// Creates a toast with title and description.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    public ToastOptions(string title, string? description = null)
    {
        Title = title;
        Description = description;
    }

    /// <summary>
    /// Creates a toast with title, description, and variant.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="variant">The toast variant.</param>
    public ToastOptions(string title, string? description, ToastVariant variant)
    {
        Title = title;
        Description = description;
        Variant = variant;
    }

    /// <summary>
    /// Creates a compact toast (useful for dialogs and tight spaces).
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="variant">The toast variant.</param>
    /// <param name="position">Optional position override.</param>
    /// <returns>A toast options configured for compact display.</returns>
    public static ToastOptions Compact(
        string title, 
        string? description = null, 
        ToastVariant variant = ToastVariant.Default,
        ToastPosition? position = null)
    {
        return new ToastOptions
        {
            Title = title,
            Description = description,
            Variant = variant,
            Size = ToastSize.Compact,
            Position = position
        };
    }

    /// <summary>
    /// Creates a success toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    /// <returns>A success toast options.</returns>
    public static ToastOptions Success(string title, string? description = null)
    {
        return new ToastOptions(title, description, ToastVariant.Success);
    }

    /// <summary>
    /// Creates an error toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    /// <returns>An error toast options.</returns>
    public static ToastOptions Error(string title, string? description = null)
    {
        return new ToastOptions(title, description, ToastVariant.Destructive);
    }

    /// <summary>
    /// Creates a warning toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    /// <returns>A warning toast options.</returns>
    public static ToastOptions Warning(string title, string? description = null)
    {
        return new ToastOptions(title, description, ToastVariant.Warning);
    }

    /// <summary>
    /// Creates an info toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    /// <returns>An info toast options.</returns>
    public static ToastOptions Info(string title, string? description = null)
    {
        return new ToastOptions(title, description, ToastVariant.Info);
    }
}

/// <summary>
/// Variant/style of a toast notification.
/// </summary>
public enum ToastVariant
{
    /// <summary>
    /// Default/neutral toast.
    /// </summary>
    Default,

    /// <summary>
    /// Success toast (green).
    /// </summary>
    Success,

    /// <summary>
    /// Warning toast (yellow/orange).
    /// </summary>
    Warning,

    /// <summary>
    /// Error/destructive toast (red).
    /// </summary>
    Destructive,

    /// <summary>
    /// Info toast (blue).
    /// </summary>
    Info
}

/// <summary>
/// Size of a toast notification.
/// </summary>
public enum ToastSize
{
    /// <summary>
    /// Default size - standard padding and text size.
    /// </summary>
    Default,

    /// <summary>
    /// Compact size - reduced padding for denser UI.
    /// </summary>
    Compact
}

/// <summary>
/// Position of the toast viewport.
/// </summary>
public enum ToastPosition
{
    /// <summary>
    /// Top left corner.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Top center.
    /// </summary>
    TopCenter,

    /// <summary>
    /// Top right corner.
    /// </summary>
    TopRight,

    /// <summary>
    /// Bottom left corner.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Bottom center.
    /// </summary>
    BottomCenter,

    /// <summary>
    /// Bottom right corner.
    /// </summary>
    BottomRight
}
