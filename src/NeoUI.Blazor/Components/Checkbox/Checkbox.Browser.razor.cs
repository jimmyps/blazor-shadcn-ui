using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
namespace NeoUI.Blazor;
public partial class Checkbox
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset browser checked state.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Canonical acceptance getter for manually wired callbacks.</summary>
    [Parameter] public Func<bool>? AcceptedValueAccessor { get; set; }
    private bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Inputs)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private async Task HandleIndeterminateChanged(bool value)
    {
        if (BrowserInteraction) Indeterminate = value;
        await IndeterminateChanged.InvokeAsync(value);
    }
    private bool AcceptedBrowserValue() => AcceptedValueAccessor?.Invoke() ?? CheckedExpression?.Compile().Invoke() ?? Checked;
}
