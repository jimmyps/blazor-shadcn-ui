using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Text.Json;
namespace NeoUI.Blazor.Primitives;
public partial class RadioGroupPrimitive<TValue>
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset browser selection.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Reads canonical acceptance for manual callbacks.</summary>
    [Parameter] public Func<TValue?>? AcceptedValueAccessor { get; set; }
    /// <summary>Binding expression for canonical acknowledgement.</summary>
    [Parameter] public Expression<Func<TValue>>? ValueExpression { get; set; }
    private bool BrowserInteraction => (BrowserDefaults.Value.Resolve(InteractionMode, BrowserPolicy, InteractionFamily.Inputs)) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => Value == null ? [] : [JsonSerializer.Serialize(Value)];
    private string[] AcceptedBrowserValues() { var value = AcceptedValueAccessor != null ? AcceptedValueAccessor() : ValueExpression != null ? ValueExpression.Compile().Invoke() : Value; return value == null ? [] : [JsonSerializer.Serialize(value)]; }
    private async Task ChangeBrowser(string[] values)
    {
        if (values.Length != 1) return;
        var value = JsonSerializer.Deserialize<TValue>(values[0]);
        if (value == null) return;
        if (!ValueChanged.HasDelegate) Value = value;
        await ValueChanged.InvokeAsync(value);
    }
}
