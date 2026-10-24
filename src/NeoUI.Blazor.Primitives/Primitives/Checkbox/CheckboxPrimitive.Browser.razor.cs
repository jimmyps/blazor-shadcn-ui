using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace NeoUI.Blazor.Primitives;
public partial class CheckboxPrimitive
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to authoritatively reset checked state.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Canonical acceptance getter for manually composed controls.</summary>
    [Parameter] public Func<bool>? AcceptedValueAccessor { get; set; }
    /// <summary>Binding expression for canonical acknowledgement.</summary>
    [Parameter] public Expression<Func<bool>>? CheckedExpression { get; set; }
    /// <summary>Native form field name.</summary>
    [Parameter] public string? Name { get; set; }
    /// <summary>Requires a checked value before form submission.</summary>
    [Parameter] public bool Required { get; set; }
    private bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Inputs)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => Indeterminate ? ["mixed"] : Checked ? ["checked"] : [];
    private string[] AcceptedBrowserValues() => (AcceptedValueAccessor?.Invoke() ?? CheckedExpression?.Compile().Invoke() ?? Checked) ? ["checked"] : [];
    private async Task ChangeBrowser(string[] values)
    {
        var revision = InteractionRevision;
        var next = values.Contains("checked");
        if (!CheckedChanged.HasDelegate) Checked = next;
        await CheckedChanged.InvokeAsync(next);
        if (revision != InteractionRevision) return;
        if (Indeterminate) { Indeterminate = false; await IndeterminateChanged.InvokeAsync(false); }
    }
}
