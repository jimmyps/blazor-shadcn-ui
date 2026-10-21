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

    internal void Sync(InteractionMode mode, bool? open, bool defaultOpen, long commandRevision)
    {
        if (initialized && mode != Mode)
            throw new InvalidOperationException("InteractionMode cannot change after initialization. Recreate the component with @key.");
        if (!initialized || (!Reporting && (open != parameterOpen || revision != commandRevision)))
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
    internal Func<string, bool, Task>? Notify { get; set; }
    internal Func<string, Task<string?>>? Commit { get; set; }
    internal DotNetObjectReference<BrowserFloatingEvents> Reference { get; }
    internal BrowserFloatingEvents() => Reference = DotNetObjectReference.Create(this);

    [JSInvokable]
    public Task NotifyAsync(string name, bool open) => Notify?.Invoke(name, open) ?? Task.CompletedTask;

    [JSInvokable]
    public Task<string?> CommitAsync(string itemId) => Commit?.Invoke(itemId) ?? Task.FromResult<string?>(null);

    public void Dispose() => Reference.Dispose();
}
