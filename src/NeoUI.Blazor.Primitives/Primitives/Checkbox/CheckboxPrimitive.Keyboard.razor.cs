using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace NeoUI.Blazor.Primitives;

public partial class CheckboxPrimitive : IAsyncDisposable
{
    [Inject] private IJSRuntime KeyboardJS { get; set; } = default!;
    [Inject] private ILogger<CheckboxPrimitive> KeyboardLogger { get; set; } = default!;
    private ElementReference keyboardElement;
    private IJSObjectReference? keyboardModule;
    private bool keyboardDisposed;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || keyboardDisposed) return;
        try
        {
            var module = await KeyboardJS.InvokeAsync<IJSObjectReference>("import", "./_content/NeoUI.Blazor.Primitives/js/primitives/checkbox.js");
            if (keyboardDisposed) { await module.DisposeAsync(); return; }
            keyboardModule = module;
            await module.InvokeVoidAsync("initialize", keyboardElement);
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException or ObjectDisposedException or OperationCanceledException)
        {
            if (!keyboardDisposed) KeyboardLogger.LogWarning(ex, "Failed to initialize checkbox keyboard behavior.");
        }
    }

    /// <summary>Releases the checkbox's local keyboard adapter.</summary>
    public async ValueTask DisposeAsync()
    {
        keyboardDisposed = true;
        GC.SuppressFinalize(this);
        if (keyboardModule is null) return;
        try { await keyboardModule.InvokeVoidAsync("dispose", keyboardElement); }
        catch (Exception ex) when (ex is JSException or InvalidOperationException or ObjectDisposedException or OperationCanceledException) { }
        try { await keyboardModule.DisposeAsync(); }
        catch (Exception ex) when (ex is JSException or InvalidOperationException or ObjectDisposedException or OperationCanceledException) { }
        keyboardModule = null;
    }
}
