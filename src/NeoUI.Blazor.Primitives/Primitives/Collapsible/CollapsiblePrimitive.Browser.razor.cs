using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace NeoUI.Blazor.Primitives;
public partial class CollapsiblePrimitive
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Omitted settings inherit the scoped or application interaction mode.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reapply a controlled UI state after reset.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    private bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Disclosures)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => Open ? ["open"] : [];
    private EventCallback<string[]> BrowserChanged => OpenChanged.HasDelegate
        ? EventCallback.Factory.Create<string[]>(this, ReportBrowserAsync) : default;
    private async Task ReportBrowserAsync(string[] values)
    {
        context.Open = values.Contains("open");
        await OpenChanged.InvokeAsync(context.Open);
    }
}
