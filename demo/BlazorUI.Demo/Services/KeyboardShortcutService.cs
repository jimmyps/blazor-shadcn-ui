using Microsoft.JSInterop;

namespace BlazorUI.Demo.Services;

/// <summary>
/// Service for managing global keyboard shortcuts.
/// </summary>
public class KeyboardShortcutService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<KeyboardShortcutService>? _dotNetReference;
    private readonly Dictionary<string, Func<Task>> _handlers = new();
    private IJSObjectReference? _module;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardShortcutService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JS runtime.</param>
    public KeyboardShortcutService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initializes the keyboard shortcut service.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_module != null) return;

        _dotNetReference = DotNetObjectReference.Create(this);
        _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./js/keyboard-shortcuts.js");

        await _module.InvokeVoidAsync("initialize", _dotNetReference);
    }

    /// <summary>
    /// Registers a keyboard shortcut handler.
    /// </summary>
    /// <param name="shortcut">The keyboard shortcut key (e.g., "k" for Cmd+K/Ctrl+K).</param>
    /// <param name="handler">The handler to execute when the shortcut is pressed.</param>
    public void RegisterShortcut(string shortcut, Func<Task> handler)
    {
        _handlers[shortcut] = handler;
    }

    /// <summary>
    /// Unregisters a keyboard shortcut handler.
    /// </summary>
    /// <param name="shortcut">The keyboard shortcut key to unregister.</param>
    public void UnregisterShortcut(string shortcut)
    {
        _handlers.Remove(shortcut);
    }

    /// <summary>
    /// Called from JavaScript when a shortcut is pressed.
    /// </summary>
    /// <param name="shortcut">The shortcut that was pressed.</param>
    [JSInvokable]
    public async Task OnShortcutPressed(string shortcut)
    {
        if (_handlers.TryGetValue(shortcut, out var handler))
        {
            await handler.Invoke();
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.InvokeVoidAsync("cleanup");
            await _module.DisposeAsync();
        }

        _dotNetReference?.Dispose();
    }
}
