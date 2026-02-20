using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorUI.Primitives.Services;

/// <summary>
/// Implementation of positioning service using Floating UI library.
/// </summary>
public class PositioningService : IPositioningService, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly SemaphoreSlim _moduleLock = new(1, 1);
    private IJSObjectReference? _module;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositioningService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for invoking positioning functions.</param>
    public PositioningService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> GetModuleAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PositioningService));
        }

        await _moduleLock.WaitAsync();
        try
        {
            if (_module == null)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Primitives/js/primitives/positioning.js");
            }
            return _module;
        }
        finally
        {
            _moduleLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<PositionResult> ComputePositionAsync(
        ElementReference reference,
        ElementReference floating,
        PositioningOptions? options = null)
    {
        var module = await GetModuleAsync();
        options ??= new PositioningOptions();

        var jsOptions = new
        {
            placement = options.Placement,
            offset = options.Offset,
            flip = options.Flip,
            shift = options.Shift,
            padding = options.Padding,
            strategy = options.Strategy,
            matchReferenceWidth = options.MatchReferenceWidth
        };

        var result = await module.InvokeAsync<JsonElement>(
            "computePosition", reference, floating, jsOptions);

        return new PositionResult
        {
            X = result.GetProperty("x").GetDouble(),
            Y = result.GetProperty("y").GetDouble(),
            Placement = result.GetProperty("placement").GetString() ?? options.Placement,
            TransformOrigin = result.TryGetProperty("transformOrigin", out var origin)
                ? origin.GetString()
                : null,
            Strategy = result.TryGetProperty("strategy", out var strategy)
                ? strategy.GetString() ?? options.Strategy
                : options.Strategy
        };
    }

    /// <inheritdoc />
    public async Task ApplyPositionAsync(ElementReference floating, PositionResult position, bool makeVisible = false)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("applyPosition", floating, position, makeVisible);
    }

    /// <inheritdoc />
    public async Task<IAsyncDisposable> AutoUpdateAsync(
        ElementReference reference,
        ElementReference floating,
        PositioningOptions? options = null)
    {
        var module = await GetModuleAsync();
        options ??= new PositioningOptions();

        var jsOptions = new
        {
            placement = options.Placement,
            offset = options.Offset,
            flip = options.Flip,
            shift = options.Shift,
            padding = options.Padding,
            strategy = options.Strategy,
            matchReferenceWidth = options.MatchReferenceWidth
        };

        var cleanup = await module.InvokeAsync<IJSObjectReference>(
            "autoUpdate", reference, floating, jsOptions);

        return new AutoUpdateHandle(cleanup);
    }

    /// <inheritdoc />
    public async Task<PositionResult> ApplyCoordinatePositionAsync(
        ElementReference floating,
        double x,
        double y,
        int padding = 8,
        bool makeVisible = true)
    {
        var module = await GetModuleAsync();

        var jsOptions = new
        {
            padding,
            makeVisible
        };

        var result = await module.InvokeAsync<JsonElement>(
            "applyCoordinatePosition", floating, x, y, jsOptions);

        return new PositionResult
        {
            X = result.GetProperty("x").GetDouble(),
            Y = result.GetProperty("y").GetDouble(),
            TransformOrigin = result.TryGetProperty("transformOrigin", out var origin)
                ? origin.GetString()
                : "top left",
            Placement = "bottom", // Not applicable for coordinate positioning
            Strategy = "fixed"
        };
    }

    /// <inheritdoc />
    public async Task ShowFloatingAsync(ElementReference floating, ElementReference? reference = null, PositioningOptions? options = null)
    {
        var module = await GetModuleAsync();

        if (reference.HasValue && options != null)
        {
            // Pass anchor and options to JS for full setup (dispose old + compute + autoUpdate + show)
            var jsOptions = new
            {
                placement = options.Placement,
                offset = options.Offset,
                flip = options.Flip,
                shift = options.Shift,
                padding = options.Padding,
                strategy = options.Strategy,
                matchReferenceWidth = options.MatchReferenceWidth
            };
            await module.InvokeVoidAsync("showFloating", floating, reference.Value, jsOptions);
        }
        else
        {
            // Simple show without repositioning
            await module.InvokeVoidAsync("showFloating", floating);
        }
    }

    /// <summary>
    /// Hides a floating element while keeping it in the DOM.
    /// </summary>
    public async Task HideFloatingAsync(ElementReference floating)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("hideFloating", floating);
    }

    /// <summary>
    /// Disposes the positioning service, releasing JavaScript module resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_module != null)
        {
            await _module.DisposeAsync();
        }

        _moduleLock.Dispose();
    }

    private class AutoUpdateHandle : IAsyncDisposable
    {
        private readonly IJSObjectReference _cleanup;

        public AutoUpdateHandle(IJSObjectReference cleanup)
        {
            _cleanup = cleanup;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _cleanup.InvokeVoidAsync("apply");
            }
            catch
            {
                // Cleanup may already be disposed
            }
        }
    }
}
