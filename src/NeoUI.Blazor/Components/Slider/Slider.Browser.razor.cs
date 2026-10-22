using Microsoft.AspNetCore.Components;
using NeoUI.Blazor.Primitives;
using System.Linq.Expressions;
using System.Globalization;
namespace NeoUI.Blazor;
public partial class Slider
{
    [Inject] private Microsoft.Extensions.Options.IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset browser state and invalidate pending acknowledgements.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    internal bool BrowserInteraction => (InteractionMode ?? BrowserPolicy?.Mode ?? BrowserDefaults.Value.DefaultMode) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    /// <summary>Identifies the bound value for acceptance and form validation.</summary>
    [Parameter] public Expression<Func<double>>? ValueExpression { get; set; }
    /// <summary>Reads canonical acceptance for manual callbacks.</summary>
    [Parameter] public Func<double>? AcceptedValueAccessor { get; set; }
    [CascadingParameter] private Microsoft.AspNetCore.Components.Forms.EditContext? BrowserEditContext { get; set; }
    private string[] BrowserValues => [Value.ToString(CultureInfo.InvariantCulture)];
    private string[] AcceptedBrowserValues() => [(AcceptedValueAccessor?.Invoke() ?? ValueExpression?.Compile().Invoke() ?? Value).ToString(CultureInfo.InvariantCulture)];
    private object BrowserOptions => new { min = Min, max = Max, step = Step, updateOn = "input" };
    private async Task ChangeBrowser(string[] values)
    {
        if (!double.TryParse(values.FirstOrDefault(),NumberStyles.Float,CultureInfo.InvariantCulture,out var value) || !double.IsFinite(value)) return;
        value = Math.Clamp(value,Min,Max);
        if (!ValueChanged.HasDelegate) Value = value;
        await ValueChanged.InvokeAsync(value);
        if (BrowserEditContext != null && ValueExpression != null) BrowserEditContext.NotifyFieldChanged(Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(ValueExpression));
    }
}
