using Microsoft.JSInterop;

namespace NeoUI.Blazor.Primitives;

// Parameter commands are versioned separately from browser events. An ambient parent render
// must never replay the initial closed state over a menu the user has just opened.
internal sealed class BrowserFloatingState
{
    internal InteractionMode Mode { get; private set; }
    internal bool Enabled => Mode == InteractionMode.Browser;
    internal bool Open { get; private set; }
    internal long OpenVersion { get; private set; }
    internal long ValueVersion { get; set; }
    internal bool Reporting { get; private set; }
    internal Func<bool, Task>? OpenChanged { get; set; }
    private bool initialized;
    private long revision;
    private bool? parameterOpen;
    private readonly HashSet<bool> pendingOpenEchoes = [];

    internal void Sync(InteractionMode mode, bool? open, bool defaultOpen, long commandRevision)
    {
        if (initialized && mode != Mode)
            throw new InvalidOperationException("InteractionMode cannot change after initialization. Recreate the component with @key.");
        var reset = initialized && revision != commandRevision;
        var changed = open != parameterOpen;
        // EventCallback schedules its parent's render after it returns. Reporting alone
        // cannot suppress those late binding echoes; remember the reported values too.
        var echo = open.HasValue && pendingOpenEchoes.Remove(open.Value);
        if (!initialized || reset || (!Reporting && changed && !echo))
        {
            Open = open ?? defaultOpen;
            OpenVersion++;
        }
        if (initialized && revision != commandRevision) ValueVersion++;
        parameterOpen = open;
        revision = commandRevision;
        Mode = mode;
        initialized = true;
    }

    internal async Task ReportOpenAsync(bool open)
    {
        if (OpenChanged != null) pendingOpenEchoes.Add(open);
        Reporting = true;
        try
        {
            if (OpenChanged != null) await OpenChanged(open);
        }
        finally { Reporting = false; }
    }
}

// One optional bridge per floating layer; no .NET calls are needed for UI mechanics.
internal sealed class BrowserFloatingEvents : IDisposable
{
    internal bool IsDisposed { get; private set; }
    internal Action<double, double>? ContextPosition { get; set; }
    [JSInvokable] public void NotifyContextAsync(double x, double y) => ContextPosition?.Invoke(x, y);
    internal Func<string, bool, Task>? Notify { get; set; }
    internal Func<string, string?, Task<string?>>? CommitMenu { get; set; }
    internal Func<string, Task<string?>>? Commit { get; set; }
    internal DotNetObjectReference<BrowserFloatingEvents> Reference { get; }
    internal BrowserFloatingEvents() => Reference = DotNetObjectReference.Create(this);

    [JSInvokable]
    public Task NotifyAsync(string name, bool open) => Notify?.Invoke(name, open) ?? Task.CompletedTask;

    [JSInvokable]
    public Task<string?> CommitAsync(string itemId) => Commit?.Invoke(itemId) ?? Task.FromResult<string?>(null);

    [JSInvokable]
    public Task<string?> CommitMenuAsync(string key, string? proposed) => CommitMenu?.Invoke(key, proposed) ?? Task.FromResult<string?>(null);

    // DotNetObjectReference.Value itself throws after disposal. Only the known owner
    // bridge is inspected; unrelated interop failures must still propagate.
    internal static bool IsReferenceDisposed(object? bridge)
    {
        if (bridge is not DotNetObjectReference<BrowserFloatingEvents> reference) return false;
        try { return reference.Value.IsDisposed; }
        catch (ObjectDisposedException) { return true; }
    }

    public void Dispose()
    {
        IsDisposed = true;
        Reference.Dispose();
    }
}
