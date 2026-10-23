using Microsoft.AspNetCore.Components;
using NeoUI.Blazor.Primitives;
using System.Linq.Expressions;
namespace NeoUI.Blazor;
public partial class Toggle
{
    [Inject] private Microsoft.Extensions.Options.IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset browser state and invalidate pending acknowledgements.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    internal bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Inputs)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    /// <summary>Binding expression for canonical acceptance and field notifications.</summary>
    [Parameter] public Expression<Func<bool>>? PressedExpression { get; set; }
    /// <summary>Canonical value getter for manual callbacks.</summary>
    [Parameter] public Func<bool>? AcceptedValueAccessor { get; set; }
    [CascadingParameter] private Microsoft.AspNetCore.Components.Forms.EditContext? BrowserEditContext { get; set; }
    private bool AcceptedBrowserValue() => AcceptedValueAccessor?.Invoke() ?? PressedExpression?.Compile().Invoke() ?? Pressed;
    private string[] BrowserValues => Pressed ? ["checked"] : [];
    private string[] AcceptedBrowserValues() => AcceptedBrowserValue() ? ["checked"] : [];
    private async Task ChangeBrowser(string[] values)
    {
        if (!PressedChanged.HasDelegate) Pressed = values.Contains("checked");
        var revision = InteractionRevision;
        await PressedChanged.InvokeAsync(values.Contains("checked"));
        if (revision != InteractionRevision) return;
        Pressed = AcceptedBrowserValue();
        if (BrowserEditContext != null && PressedExpression != null) BrowserEditContext.NotifyFieldChanged(Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(PressedExpression));
    }
}
