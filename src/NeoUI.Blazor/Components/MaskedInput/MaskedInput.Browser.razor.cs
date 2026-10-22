using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
using System.Globalization;

namespace NeoUI.Blazor;

public partial class MaskedInput
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    [CascadingParameter] private PopoverContext? BrowserPopover { get; set; }
    /// <summary>Omitted settings inherit the enclosing floating, scoped or application policy.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to replace the browser draft, including same-value resets.</summary>
    [Parameter] public long ValueRevision { get; set; }
    /// <summary>Canonical value getter for manual asynchronous compositions; binding uses ValueExpression.</summary>
    [Parameter] public Func<string?>? AcceptedValueAccessor { get; set; }
    private bool? browserMode;
    private BrowserFormattedInput? browserInput;
    private bool BrowserInteraction => browserMode == true;
    private void ConfigureBrowserMode()
    {
        var mode = ComposedInteraction.IsBrowser(InteractionMode, BrowserPolicy, BrowserDefaults.Value, BrowserPopover);
        if (browserMode.HasValue && browserMode != mode) throw new InvalidOperationException("InteractionMode cannot change after initialization. Recreate the component with @key.");
        browserMode = mode;
    }
    /// <summary>Synchronizes and validates the final editable draft.</summary>
    public Task<InputFlushResult> FlushAsync(int timeoutMilliseconds = 30000) => browserInput?.FlushAsync(timeoutMilliseconds)
        ?? Task.FromResult(new InputFlushResult { Status = BrowserInteraction ? InputFlushStatus.NotReady : InputFlushStatus.NotSupported });
    private object BrowserFormat => new { kind = "mask", mask = Mask, maskChar = MaskChar.ToString(), showMask = ShowMask, required = Required };
    private bool EquivalentBrowserValue(string? draft, string? accepted) => ExtractRawValue(draft ?? string.Empty) == ExtractRawValue(accepted ?? string.Empty);
    private async Task<string?> AcceptBrowserDraft(string? text, bool final)
    {
        var raw = ExtractRawValue(text ?? string.Empty);
        var proposed = string.IsNullOrEmpty(raw) ? null : raw;
        var revision = ValueRevision;
        if (!ValueChanged.HasDelegate) Value = proposed;
        else if (proposed != Value) await ValueChanged.InvokeAsync(proposed);
        if (revision != ValueRevision) return CurrentDisplayValue;
        Value = AcceptedValueAccessor != null ? AcceptedValueAccessor() : ValueExpression != null ? ValueExpression.Compile().Invoke() : Value;
        if (_validationBehavior != null) await _validationBehavior.NotifyFieldChangedAsync();
        else if (EditContext != null && ValueExpression != null) EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        return (Value ?? string.Empty) == raw ? text : string.IsNullOrEmpty(Value) ? string.Empty : ApplyMask(Value);
    }
}
