using Microsoft.AspNetCore.Components;
using NeoUI.Blazor.Primitives;
namespace NeoUI.Blazor;
public partial class RadioGroup<TValue>
{
    [Inject] private Microsoft.Extensions.Options.IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    private bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Inputs)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset browser selection.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Canonical value getter for manual asynchronous callbacks.</summary>
    [Parameter] public Func<TValue?>? AcceptedValueAccessor { get; set; }
    private TValue? AcceptedBrowserValue() => AcceptedValueAccessor != null ? AcceptedValueAccessor() : ValueExpression != null ? ValueExpression.Compile().Invoke() : Value;
}
