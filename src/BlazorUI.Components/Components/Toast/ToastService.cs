namespace BlazorUI.Components.Toast;

/// <summary>
<<<<<<< HEAD
/// Service interface for displaying toast notifications.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Event raised when toasts are updated.
    /// </summary>
    event Action? OnChange;

    /// <summary>
    /// Gets the current list of active toasts.
    /// </summary>
    IReadOnlyList<ToastOptions> Toasts { get; }

    /// <summary>
    /// Shows a toast with the specified options.
    /// </summary>
    /// <param name="options">The toast options.</param>
    void Show(ToastOptions options);

    /// <summary>
    /// Shows a simple toast with title and optional description.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="variant">The toast variant.</param>
    /// <param name="duration">How long to show the toast.</param>
    void Show(string title, string? description = null, ToastVariant variant = ToastVariant.Default, TimeSpan? duration = null);

    /// <summary>
    /// Shows a success toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    void Success(string title, string? description = null);

    /// <summary>
    /// Shows an error toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    void Error(string title, string? description = null);

    /// <summary>
    /// Shows a warning toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    void Warning(string title, string? description = null);

    /// <summary>
    /// Shows an info toast.
    /// </summary>
    /// <param name="title">The toast title.</param>
    /// <param name="description">Optional description.</param>
    void Info(string title, string? description = null);

    /// <summary>
    /// Dismisses a specific toast by ID.
    /// </summary>
    /// <param name="id">The toast ID.</param>
    void Dismiss(string id);

    /// <summary>
    /// Dismisses all toasts.
    /// </summary>
    void DismissAll();
}

/// <summary>
/// Default implementation of the toast service.
/// </summary>
public class ToastService : IToastService
{
    private readonly List<ToastOptions> _toasts = new();
    private readonly object _lock = new();

    /// <inheritdoc />
    public event Action? OnChange;

    /// <inheritdoc />
    public IReadOnlyList<ToastOptions> Toasts
    {
        get
        {
            lock (_lock)
            {
                return _toasts.ToList().AsReadOnly();
            }
        }
    }

    /// <inheritdoc />
    public void Show(ToastOptions options)
    {
        lock (_lock)
        {
            _toasts.Add(options);
        }
        OnChange?.Invoke();

        if (options.Duration.HasValue)
        {
            _ = DismissAfterDelay(options.Id, options.Duration.Value);
        }
    }

    /// <inheritdoc />
    public void Show(string title, string? description = null, ToastVariant variant = ToastVariant.Default, TimeSpan? duration = null)
    {
        Show(new ToastOptions
=======
/// Service for managing toast notifications.
/// Register as a singleton or scoped service in DI.
/// </summary>
public class ToastService
{
    private readonly List<ToastData> _toasts = new();

    /// <summary>
    /// Event fired when the toast collection changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Gets the current list of toasts.
    /// </summary>
    public IReadOnlyList<ToastData> Toasts => _toasts.AsReadOnly();

    /// <summary>
    /// Shows a toast notification.
    /// </summary>
    /// <param name="toast">The toast data to display.</param>
    public void Show(ToastData toast)
    {
        _toasts.Add(toast);
        OnChange?.Invoke();
    }

    /// <summary>
    /// Shows a simple toast with a message.
    /// </summary>
    /// <param name="description">The message to display.</param>
    /// <param name="title">Optional title.</param>
    /// <param name="variant">The visual variant.</param>
    /// <param name="duration">Duration in milliseconds (default 5000).</param>
    public void Show(string description, string? title = null, ToastVariant variant = ToastVariant.Default, int duration = 5000)
    {
        Show(new ToastData
>>>>>>> 8835bfed9859e4bf8349954ac05f732fe9ffddcf
        {
            Title = title,
            Description = description,
            Variant = variant,
<<<<<<< HEAD
            Duration = duration ?? TimeSpan.FromSeconds(5)
        });
    }

    /// <inheritdoc />
    public void Success(string title, string? description = null)
    {
        Show(title, description, ToastVariant.Success);
    }

    /// <inheritdoc />
    public void Error(string title, string? description = null)
    {
        Show(title, description, ToastVariant.Destructive);
    }

    /// <inheritdoc />
    public void Warning(string title, string? description = null)
    {
        Show(title, description, ToastVariant.Warning);
    }

    /// <inheritdoc />
    public void Info(string title, string? description = null)
    {
        Show(title, description, ToastVariant.Info);
    }

    /// <inheritdoc />
    public void Dismiss(string id)
    {
        ToastOptions? dismissed = null;
        lock (_lock)
        {
            var toast = _toasts.FirstOrDefault(t => t.Id == id);
            if (toast != null)
            {
                _toasts.Remove(toast);
                dismissed = toast;
            }
        }
        dismissed?.OnDismiss?.Invoke();
        OnChange?.Invoke();
    }

    /// <inheritdoc />
    public void DismissAll()
    {
        List<ToastOptions> dismissed;
        lock (_lock)
        {
            dismissed = new List<ToastOptions>(_toasts);
            _toasts.Clear();
        }
        foreach (var toast in dismissed)
        {
            toast.OnDismiss?.Invoke();
        }
        OnChange?.Invoke();
    }

    private async Task DismissAfterDelay(string id, TimeSpan delay)
    {
        await Task.Delay(delay);
        Dismiss(id);
    }
=======
            Duration = duration
        });
    }

    /// <summary>
    /// Shows a success toast (default variant).
    /// </summary>
    /// <param name="description">The message to display.</param>
    /// <param name="title">Optional title.</param>
    public void Success(string description, string? title = null)
    {
        Show(description, title, ToastVariant.Default);
    }

    /// <summary>
    /// Shows an error toast (destructive variant).
    /// </summary>
    /// <param name="description">The message to display.</param>
    /// <param name="title">Optional title.</param>
    public void Error(string description, string? title = null)
    {
        Show(description, title, ToastVariant.Destructive);
    }

    /// <summary>
    /// Dismisses a specific toast by ID.
    /// </summary>
    /// <param name="id">The toast ID to dismiss.</param>
    public void Dismiss(string id)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == id);
        if (toast != null)
        {
            _toasts.Remove(toast);
            OnChange?.Invoke();
        }
    }

    /// <summary>
    /// Dismisses all toasts.
    /// </summary>
    public void DismissAll()
    {
        _toasts.Clear();
        OnChange?.Invoke();
    }
>>>>>>> 8835bfed9859e4bf8349954ac05f732fe9ffddcf
}
