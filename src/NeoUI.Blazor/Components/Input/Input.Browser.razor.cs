using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using NeoUI.Blazor.Primitives;
using System.Text.Json;

namespace NeoUI.Blazor;

public partial class Input
{
    [Inject] private IOptions<InteractionOptions> InteractionDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? InteractionPolicy { get; set; }
    [CascadingParameter] private PopoverContext? PopoverContext { get; set; }

    /// <summary>Omitted values inherit the enclosing floating control, scoped or application interaction policy.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }

    /// <summary>Change to replace the browser draft with Value, including unchanged resets. This invalidates queued edits.</summary>
    [Parameter] public long ValueRevision { get; set; }

    private bool? _browserInteraction;
    private string? _browserInitialValue;
    private long _browserSequence;
    private bool BrowserInteraction => _browserInteraction == true;
    private string? RenderedValue => BrowserInteraction ? _browserInitialValue : Value;
    private string? BrowserInputConfiguration => BrowserInteraction ? JsonSerializer.Serialize(new
    {
        value = Value ?? string.Empty,
        revision = ValueRevision,
        updateOn = UpdateOn.ToString().ToLowerInvariant(),
        debounce = Math.Max(0, DebounceDelay)
    }) : null;

    private void ConfigureBrowserInput()
    {
        var browser = ComposedInteraction.IsBrowser(InteractionMode, InteractionPolicy, InteractionDefaults.Value, PopoverContext);
        if (_browserInteraction.HasValue && _browserInteraction != browser)
            throw new InvalidOperationException("InteractionMode cannot change after initialization. Recreate the input with @key.");
        if (!_browserInteraction.HasValue) _browserInitialValue = Value;
        _browserInteraction = browser;
    }

    /// <summary>Processes a versioned edit out of band. UI drafts never wait for this callback.</summary>
    [JSInvokable]
    public async Task<object> OnBrowserInputChanged(string? value, long sequence, long revision)
    {
        if (!BrowserInteraction || revision != ValueRevision || sequence <= _browserSequence || Disabled || Readonly)
            return new { value = Value ?? string.Empty, sequence, revision, ignored = true };
        _browserSequence = sequence;
        // Keep the accepted parameter intact: a rejecting parent may retain the same
        // render-tree parameter and Blazor then does not set it again.
        if (!ValueChanged.HasDelegate) Value = value;
        await ValueChanged.InvokeAsync(value);
        if (revision != ValueRevision)
            return new { value = Value ?? string.Empty, sequence, revision, ignored = true };
        if (_validationBehavior != null) await _validationBehavior.NotifyFieldChangedAsync();
        return new { value = Value ?? string.Empty, sequence, revision, ignored = false };
    }
}
