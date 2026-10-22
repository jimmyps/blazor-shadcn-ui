using Microsoft.AspNetCore.Components;
using NeoUI.Blazor.Primitives;
using System.Linq.Expressions;
namespace NeoUI.Blazor;
public partial class ToggleGroup
{
    [Inject] private Microsoft.Extensions.Options.IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset browser state and invalidate pending acknowledgements.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    internal bool BrowserInteraction => (InteractionMode ?? BrowserPolicy?.Mode ?? BrowserDefaults.Value.DefaultMode) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    /// <summary>Canonical binding expression for single selection.</summary>
    [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
    /// <summary>Canonical binding expression for multiple selection.</summary>
    [Parameter] public Expression<Func<List<string>?>>? ValuesExpression { get; set; }
    /// <summary>Canonical selected keys for manual asynchronous callbacks.</summary>
    [Parameter] public Func<string[]>? AcceptedValuesAccessor { get; set; }
    [CascadingParameter] private Microsoft.AspNetCore.Components.Forms.EditContext? BrowserEditContext { get; set; }
    private string[] BrowserValues => Type == ToggleGroupType.Multiple ? Values?.ToArray() ?? [] : Value == null ? [] : [Value];
    private string[] AcceptedBrowserValues() => AcceptedValuesAccessor?.Invoke() ?? (Type == ToggleGroupType.Multiple
        ? (ValuesExpression?.Compile().Invoke() ?? Values)?.ToArray() ?? []
        : (ValueExpression?.Compile().Invoke() ?? Value) is { } value ? [value] : []);
    private async Task ChangeBrowser(string[] values)
    {
        if (Type == ToggleGroupType.Multiple) { if (!ValuesChanged.HasDelegate) Values = [..values]; await ValuesChanged.InvokeAsync([..values]); }
        else { if (!ValueChanged.HasDelegate) Value = values.FirstOrDefault(); await ValueChanged.InvokeAsync(values.FirstOrDefault()); }
        if (BrowserEditContext != null) {
            if (Type == ToggleGroupType.Multiple && ValuesExpression != null) BrowserEditContext.NotifyFieldChanged(Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(ValuesExpression));
            else if (ValueExpression != null) BrowserEditContext.NotifyFieldChanged(Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(ValueExpression));
        }
    }
}
