using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace NeoUI.Blazor.Primitives;
public partial class AccordionPrimitive
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Omitted settings inherit the scoped or application interaction mode.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reapply a controlled UI state after reset.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    private bool BrowserInteraction => (InteractionMode ?? BrowserPolicy?.Mode ?? BrowserDefaults.Value.DefaultMode) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => (Value ?? _context.State.OpenValues).ToArray();
    private EventCallback<string[]> BrowserChanged => ValueChanged.HasDelegate || OnValueChange.HasDelegate
        ? EventCallback.Factory.Create<string[]>(this, ReportBrowserAsync) : default;
    private async Task ReportBrowserAsync(string[] values)
    {
        _context.State.OpenValues = new HashSet<string>(values);
        var revision = InteractionRevision;
        await ValueChanged.InvokeAsync(new HashSet<string>(values));
        if (revision != InteractionRevision) return;
        await OnValueChange.InvokeAsync(new HashSet<string>(values));
    }
}
