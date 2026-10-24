using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace NeoUI.Blazor.Primitives.Services;

// Scoped to a circuit/app. Every checkbox shares the same pending/completed import;
// the module installs one document listener and returns no JS object reference.
internal sealed class CheckboxKeyboardService(IJSRuntime js, ILogger<CheckboxKeyboardService> logger)
{
    private readonly object gate = new();
    private Task? initialization;
    public Task EnsureInitializedAsync()
    {
        lock (gate) return initialization ??= InitializeAsync();
    }
    private async Task InitializeAsync()
    {
        try { await js.InvokeVoidAsync("import", "./_content/NeoUI.Blazor.Primitives/js/primitives/checkbox.js"); }
        catch (Exception ex) when (ex is JSDisconnectedException or OperationCanceledException or ObjectDisposedException) { }
        catch (Exception ex) when (ex is JSException or InvalidOperationException)
        { logger.LogWarning(ex, "Failed to initialize shared checkbox keyboard behavior."); }
    }
}
