namespace BlazorUI.Components.Toast;

/// <summary>
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
    private readonly Dictionary<string, CancellationTokenSource> _dismissTimers = new();
    private readonly Dictionary<string, DateTime> _timerStartTimes = new();
    private readonly Dictionary<string, TimeSpan> _originalDurations = new();
    private readonly Dictionary<string, bool> _isPaused = new();
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

        if (options.Duration.HasValue && options.PauseOnHover)
        {
            _ = DismissAfterDelayWithPause(options.Id, options.Duration.Value);
        }
        else if (options.Duration.HasValue)
        {
            _ = DismissAfterDelay(options.Id, options.Duration.Value);
        }
    }

    /// <inheritdoc />
    public void Show(string title, string? description = null, ToastVariant variant = ToastVariant.Default, TimeSpan? duration = null)
    {
        Show(new ToastOptions
        {
            Title = title,
            Description = description,
            Variant = variant,
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

    private async Task DismissAfterDelayWithPause(string id, TimeSpan delay)
    {
        var cts = new CancellationTokenSource();
        
        lock (_lock)
        {
            _dismissTimers[id] = cts;
            _timerStartTimes[id] = DateTime.UtcNow;
            _originalDurations[id] = delay;
            _isPaused[id] = false;
        }

        try
        {
            await Task.Delay(delay, cts.Token);
            
            // Timer completed without being cancelled
            Dismiss(id);
            
            // Clean up on successful completion
            lock (_lock)
            {
                _dismissTimers.Remove(id);
                _timerStartTimes.Remove(id);
                _originalDurations.Remove(id);
                _isPaused.Remove(id);
            }
        }
        catch (TaskCanceledException)
        {
            // Timer was cancelled (either paused or dismissed)
            // Check if it was just paused
            lock (_lock)
            {
                if (_isPaused.GetValueOrDefault(id))
                {
                    // Toast is paused, keep state for resume
                    // Don't clean up - state is needed for resume
                    return;
                }
                else
                {
                    // Toast was dismissed, clean up
                    _dismissTimers.Remove(id);
                    _timerStartTimes.Remove(id);
                    _originalDurations.Remove(id);
                    _isPaused.Remove(id);
                }
            }
        }
    }

    /// <summary>
    /// Sets the hover state for a toast (for pause-on-hover functionality).
    /// </summary>
    /// <param name="id">The toast ID.</param>
    /// <param name="isHovered">True if hovered, false otherwise.</param>
    public void SetToastHoverState(string id, bool isHovered)
    {
        lock (_lock)
        {
            if (!_dismissTimers.TryGetValue(id, out var cts))
            {
                return; // No active timer for this toast
            }

            if (isHovered)
            {
                // Pause the timer
                if (!_isPaused.GetValueOrDefault(id))
                {
                    _isPaused[id] = true;
                    
                    // Calculate elapsed time
                    var startTime = _timerStartTimes[id];
                    var elapsed = DateTime.UtcNow - startTime;
                    var originalDuration = _originalDurations[id];
                    var remaining = originalDuration - elapsed;
                    
                    // Store the remaining duration for resume
                    _originalDurations[id] = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
                    
                    // Cancel the current timer
                    cts.Cancel();
                }
            }
            else
            {
                // Resume the timer
                if (_isPaused.GetValueOrDefault(id))
                {
                    _isPaused[id] = false;
                    
                    var remaining = _originalDurations[id];
                    if (remaining > TimeSpan.Zero)
                    {
                        // Start a new timer with remaining duration
                        _ = DismissAfterDelayWithPause(id, remaining);
                    }
                    else
                    {
                        // Time's up, dismiss immediately
                        Dismiss(id);
                    }
                }
            }
        }
    }
}
