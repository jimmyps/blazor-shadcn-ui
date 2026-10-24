using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace NeoUI.Blazor.Primitives;
public partial class TabsPrimitive
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Omitted settings inherit the scoped or application interaction mode.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reapply a controlled UI state after reset.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    private bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Disclosures)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => new[] { Value ?? _context.State.ActiveValue }.OfType<string>().ToArray();
    private EventCallback<string[]> BrowserChanged => ValueChanged.HasDelegate || OnValueChange.HasDelegate
        ? EventCallback.Factory.Create<string[]>(this, ReportBrowserAsync) : default;
    private async Task ReportBrowserAsync(string[] values)
    {
        _context.State.ActiveValue = values.FirstOrDefault();
        var revision = InteractionRevision;
        await ValueChanged.InvokeAsync(values.FirstOrDefault());
        if (revision != InteractionRevision) return;
        await OnValueChange.InvokeAsync(values.FirstOrDefault());
    }
}
