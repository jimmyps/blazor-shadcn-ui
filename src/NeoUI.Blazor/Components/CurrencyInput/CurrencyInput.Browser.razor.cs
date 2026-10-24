using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
using System.Globalization;

namespace NeoUI.Blazor;

public partial class CurrencyInput<TValue>
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    [CascadingParameter] private PopoverContext? BrowserPopover { get; set; }
    /// <summary>Omitted settings inherit the enclosing floating, scoped or application policy.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to replace the browser draft, including same-value resets.</summary>
    [Parameter] public long ValueRevision { get; set; }
    /// <summary>Canonical value getter for manual asynchronous compositions; binding uses ValueExpression.</summary>
    [Parameter] public Func<TValue?>? AcceptedValueAccessor { get; set; }
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
    private bool browserDraftValid = true;
    private object BrowserFormat => new { kind = "currency", editValue = GetEditingValue(GetCultureInfo()), symbol = CurrencyDefinition.Symbol,
        group = GetCultureInfo().NumberFormat.NumberGroupSeparator, @decimal = GetCultureInfo().NumberFormat.NumberDecimalSeparator };
    private string? BrowserValue => CurrentValueAsString;
    private bool ParseBrowser(string? text, out TValue? value)
    {
        var culture = GetCultureInfo();
        text = text?.Replace(CurrencyDefinition.Symbol, "").Replace(culture.NumberFormat.CurrencySymbol, "").Trim();
        return BrowserNumericConversion.TryParse(text, culture, out value);
    }
    private TValue? FinalBrowserValue(TValue? value) => BrowserNumericConversion.Clamp(value, Min, Max);
    private bool EquivalentBrowserValue(string? draft, string? accepted) => ParseBrowser(draft, out var left)
        && (accepted == BrowserValue ? EqualityComparer<TValue?>.Default.Equals(left, Value)
            : ParseBrowser(accepted, out var right) && EqualityComparer<TValue?>.Default.Equals(left, right));
    private async Task<string?> AcceptBrowserDraft(string? text, bool final)
    {
        browserDraftValid = ParseBrowser(text, out var proposed);
        if (!browserDraftValid) return text;
        if (final) proposed = FinalBrowserValue(proposed);
        var revision = ValueRevision;
        if (!ValueChanged.HasDelegate) Value = proposed;
        else if (!EqualityComparer<TValue?>.Default.Equals(proposed, Value)) await ValueChanged.InvokeAsync(proposed);
        if (revision != ValueRevision) return BrowserValue;
        Value = AcceptedValueAccessor != null ? AcceptedValueAccessor() : ValueExpression != null ? ValueExpression.Compile().Invoke() : Value;
        if (_validationBehavior != null) await _validationBehavior.NotifyFieldChangedAsync();
        else if (EditContext != null && ValueExpression != null) EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        if (!final && EqualityComparer<TValue?>.Default.Equals(proposed, Value)) return text;
        return BrowserValue;
    }
}
