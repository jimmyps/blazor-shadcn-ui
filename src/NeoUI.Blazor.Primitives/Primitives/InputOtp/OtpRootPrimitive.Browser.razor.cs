using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
namespace NeoUI.Blazor.Primitives;
public partial class OtpRootPrimitive
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    [CascadingParameter] private EditContext? BrowserEditContext { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset the browser draft.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Identifies the bound field for acceptance and validation.</summary>
    [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
    /// <summary>Canonical acceptance getter for manually wired callbacks.</summary>
    [Parameter] public Func<string?>? AcceptedValueAccessor { get; set; }
    /// <summary>Requires every slot before form submission.</summary>
    [Parameter] public bool Required { get; set; }
    private bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Inputs)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => [Value ?? _context.Value ?? DefaultValue];
    private string[] AcceptedBrowserValues() => [AcceptedValueAccessor != null ? AcceptedValueAccessor() ?? "" : ValueExpression != null ? ValueExpression.Compile().Invoke() ?? "" : Value ?? _context.Value ?? ""];
    private async Task ChangeBrowser(string[] values)
    {
        var value = values.FirstOrDefault() ?? "";
        if (value.Length > Length || value.Any(character => !IsValidChar(character))) return;
        var revision = InteractionRevision;
        if (!ValueChanged.HasDelegate) Value = value;
        await ValueChanged.InvokeAsync(value);
        if (revision != InteractionRevision) return;
        await OnValueChange.InvokeAsync(value);
        if (revision != InteractionRevision) return;
        if (BrowserEditContext != null && ValueExpression != null) BrowserEditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        if (value.Length == Length && AcceptedBrowserValues()[0] == value) await OnComplete.InvokeAsync(value);
    }
}
