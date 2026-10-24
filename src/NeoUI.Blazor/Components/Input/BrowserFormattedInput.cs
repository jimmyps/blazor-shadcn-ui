using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

// Internal adapter: one draft/IME/lifetime/form protocol, with typed acceptance owned
// by the parent and formatting owned by its browser controller.
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class BrowserFormattedInput : Input
{
    [Parameter] public object? Format { get; set; }
    [Parameter] public Func<string?, bool, Task<string?>>? AcceptDraft { get; set; }
    [Parameter] public Func<string?, string?, bool>? EquivalentValue { get; set; }
    protected override bool IsEquivalentBrowserValue(string? draft, string? accepted) => EquivalentValue?.Invoke(draft, accepted) ?? base.IsEquivalentBrowserValue(draft, accepted);
    [Parameter] public Func<bool>? DraftIsValid { get; set; }
    protected override bool BrowserDraftIsValid => DraftIsValid?.Invoke() ?? true;
    /// <summary>Reads fresh formatting metadata after canonical acceptance, before the next child render.</summary>
    [Parameter] public Func<object?>? FormatAccessor { get; set; }
    protected override object? BrowserFormat => FormatAccessor?.Invoke() ?? Format;
    protected override bool CommitOnBlur => true;
    protected override Task<string?> AcceptBrowserDraftAsync(string? value, bool final) =>
        AcceptDraft?.Invoke(value, final) ?? Task.FromResult(value);
}
