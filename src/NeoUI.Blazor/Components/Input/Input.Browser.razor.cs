using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using NeoUI.Blazor.Primitives;
using System.Text.Json;

namespace NeoUI.Blazor;

public partial class Input
{
    [Inject] private IOptions<InteractionOptions> InteractionDefaults { get; set; } = default!;
    [CascadingParameter] protected BrowserInputScope? InputScope { get; set; }
    [CascadingParameter] private InteractionPolicy? InteractionPolicy { get; set; }
    [CascadingParameter] private PopoverContext? PopoverContext { get; set; }

    /// <summary>Omitted values inherit the enclosing floating control, scoped or application interaction policy.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }

    /// <summary>Change to replace the browser draft with Value, including unchanged resets. This invalidates queued edits.</summary>
    [Parameter] public long ValueRevision { get; set; }

    /// <summary>Reads the canonical value after an awaited callback without depending on a child render. For manual compositions; standard binding uses ValueExpression.</summary>
    [Parameter] public Func<string?>? AcceptedValueAccessor { get; set; }

    private bool? _browserInteraction;
    private string? _browserInitialValue;
    private long _browserSequence;
    protected virtual bool BrowserDraftIsValid => true;
    protected virtual object? BrowserFormat => null;
    protected virtual bool CommitOnBlur => false;
    internal IJSObjectReference? ValidationModule => _inputModule;
    private bool BrowserInteraction => _browserInteraction == true;
    protected string? RenderedValue => BrowserInteraction ? _browserInitialValue : Value;
    protected string? BrowserInputConfiguration => BrowserInteraction ? JsonSerializer.Serialize(new
    {
        value = Value ?? string.Empty,
        revision = ValueRevision,
        updateOn = UpdateOn.ToString().ToLowerInvariant(),
        debounce = Math.Max(0, DebounceDelay),
        @readonly = Readonly,
        format = BrowserFormat, commitOnBlur = CommitOnBlur,
        server = !OperatingSystem.IsBrowser()
    }) : null;

    private void ConfigureBrowserInput()
    {
        var browser = ComposedInteraction.IsBrowser(InteractionMode, InteractionPolicy, InteractionDefaults.Value, PopoverContext);
        if (_browserInteraction.HasValue && _browserInteraction != browser)
            throw new InvalidOperationException("InteractionMode cannot change after initialization. Recreate the input with @key.");
        if (!_browserInteraction.HasValue) _browserInitialValue = Value;
        _browserInteraction = browser;
    }

    private long _flushRequestId;
    private bool _browserInputDisposed;
    private readonly Dictionary<long, TaskCompletionSource<InputFlushResult>> _flushRequests = new();

    [JSInvokable]
    public void CompleteInputFlush(long id, InputFlushResult result)
    {
        if (_flushRequests.TryGetValue(id, out var completion)) completion.TrySetResult(result);
    }

    private void DisposeBrowserFlushes()
    {
        _browserInputDisposed = true;
        foreach (var completion in _flushRequests.Values) completion.TrySetResult(new() { Status = InputFlushStatus.Disposed });
        _flushRequests.Clear();
    }

    /// <summary>Synchronizes this browser input, bypassing debounce and awaiting the latest acknowledgement.</summary>
    public async Task<InputFlushResult> FlushAsync(int timeoutMilliseconds = 30000)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeoutMilliseconds);
        if (_browserInputDisposed) return new() { Status = InputFlushStatus.Disposed, FieldId = EffectiveId };
        if (!BrowserInteraction) return new() { Status = InputFlushStatus.NotSupported, FieldId = EffectiveId };
        if (!_jsInitialized) return new() { Status = InputFlushStatus.NotReady, FieldId = EffectiveId };
        var id = ++_flushRequestId;
        var completion = new TaskCompletionSource<InputFlushResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        _flushRequests.Add(id, completion);
        IJSObjectReference? lease = null, module = null;
        try
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromMilliseconds((long)timeoutMilliseconds + 1000));
            module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", deadline.Token, "./_content/NeoUI.Blazor/js/browser-input.js");
            if (_browserInputDisposed) return new() { Status = InputFlushStatus.Disposed, FieldId = EffectiveId };
            lease = await module.InvokeAsync<IJSObjectReference>("beginInput", deadline.Token, EffectiveId, timeoutMilliseconds);
            await lease.InvokeVoidAsync("watch", deadline.Token, _dotNetRef, id, nameof(CompleteInputFlush), !OperatingSystem.IsBrowser());
            return await completion.Task.WaitAsync(deadline.Token);
        }
        catch (ObjectDisposedException)
        { return new() { Status = InputFlushStatus.Disposed }; }
        catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException)
        { return new() { Status = InputFlushStatus.Uncertain, FieldId = EffectiveId }; }
        finally
        {
            _flushRequests.Remove(id);
            try
            {
                using var releaseDeadline = new CancellationTokenSource(1000);
                if (lease != null) { await lease.InvokeVoidAsync("release", releaseDeadline.Token); await lease.DisposeAsync(); }
                if (module != null) await module.DisposeAsync();
            }
            catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException) { }
        }
    }

    // Composed owners can accept a value before their next render reaches this
    // child. An acknowledgement must not depend on renderer backpressure.
    internal void AcceptBrowserValue(string? value, long revision)
    {
        if (BrowserInteraction && revision == ValueRevision) Value = value;
    }

    /// <summary>Processes a versioned edit out of band. UI drafts never wait for this callback.</summary>
    [JSInvokable]
    public Task<object> OnBrowserInputChanged(string? value, long sequence, long revision) => ProcessBrowserEditAsync(value, sequence, revision, false);

    /// <summary>Processes a final blur or form snapshot through the same versioned queue.</summary>
    [JSInvokable]
    public Task<object> OnBrowserInputCommitted(string? value, long sequence, long revision) => ProcessBrowserEditAsync(value, sequence, revision, true);

    protected virtual async Task<string?> AcceptBrowserDraftAsync(string? value, bool final)
    {
        if (!ValueChanged.HasDelegate) Value = value;
        await ValueChanged.InvokeAsync(value);
        if (AcceptedValueAccessor != null) return AcceptedValueAccessor();
        if (ValueExpression != null) return ValueExpression.Compile().Invoke();
        return Value;
    }

    protected virtual bool IsEquivalentBrowserValue(string? draft, string? accepted) => (draft ?? string.Empty) == (accepted ?? string.Empty);

    private async Task<object> ProcessBrowserEditAsync(string? value, long sequence, long revision, bool final)
    {
        if (!BrowserInteraction || _browserInputDisposed || revision != ValueRevision || sequence <= _browserSequence || Disabled || Readonly)
            return new { value = Value ?? string.Empty, sequence, revision, ignored = true };
        _browserSequence = sequence;
        var accepted = await AcceptBrowserDraftAsync(value, final);
        if (_browserInputDisposed || revision != ValueRevision)
            return new { value = Value ?? string.Empty, sequence, revision, ignored = true };
        Value = accepted;
        if (_validationBehavior != null) await _validationBehavior.NotifyFieldChangedAsync();
        else if (EditContext != null && ValueExpression != null) EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        return new { value = Value ?? string.Empty, sequence, revision, ignored = false, equivalent = IsEquivalentBrowserValue(value, Value), invalid = !BrowserDraftIsValid, format = BrowserFormat };
    }
}
