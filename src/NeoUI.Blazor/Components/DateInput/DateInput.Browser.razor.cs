using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
using System.Linq.Expressions;
namespace NeoUI.Blazor;
public partial class DateInput
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    [CascadingParameter] private EditContext? BrowserEditContext { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset all browser segments.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Identifies the model field for canonical acceptance and form validation.</summary>
    [Parameter] public Expression<Func<DateOnly?>>? ValueExpression { get; set; }
    /// <summary>Canonical value getter for manually wired callbacks.</summary>
    [Parameter] public Func<DateOnly?>? AcceptedValueAccessor { get; set; }
    private BrowserStateHost? browserHost;
    private bool BrowserInteraction => (InteractionMode ?? BrowserPolicy?.Mode ?? BrowserDefaults.Value.DefaultMode) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => _segments.Where(segment => !segment.IsLiteral).Select(segment => segment.Value?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "").ToArray();
    private string[]? browserAccepted;
    private string[] AcceptedBrowserValues() => browserAccepted ?? BrowserValues;
    private object BrowserOptions => new { required = Required, twoDigitYearMax = EffectiveCulture.Calendar.TwoDigitYearMax, segments = _segments.Where(segment => !segment.IsLiteral).Select(segment => new {
        kind = segment.Kind.ToString().ToLowerInvariant(), min = segment.Min, max = segment.Max, length = segment.MaxLength, placeholder = segment.Placeholder, step = 1
    }).ToArray() };
    private async Task ChangeBrowser(string[] values)
    {
        var segments = _segments.Where(segment => !segment.IsLiteral).ToArray();
        if (values.Length != segments.Length) return;
        browserAccepted = values;
        var complete = values.All(value => !string.IsNullOrEmpty(value));
        var empty = values.All(string.IsNullOrEmpty);
        if (!complete && !empty) return;
        for (var index = 0; index < segments.Length; index++) {
            if (string.IsNullOrEmpty(values[index])) segments[index].Value = null;
            else if (int.TryParse(values[index],out var number) && number >= segments[index].Min && number <= segments[index].Max) segments[index].Value = number;
            else return;
        }
        var proposed = empty ? (DateOnly?)null : TryBuildDate();
        if (!empty && proposed == null) return;
        var revision = InteractionRevision;
        if (!ValueChanged.HasDelegate) Value = proposed;
        await ValueChanged.InvokeAsync(proposed);
        if (revision != InteractionRevision) return;
        Value = AcceptedValueAccessor != null ? AcceptedValueAccessor() : ValueExpression != null ? ValueExpression.Compile().Invoke() : Value;
        SyncSegmentsFromValue();browserAccepted = BrowserValues;
        if (BrowserEditContext != null && ValueExpression != null) BrowserEditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
    }
}
