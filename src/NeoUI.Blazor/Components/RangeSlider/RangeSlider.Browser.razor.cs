using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
using System.Globalization;
namespace NeoUI.Blazor;
public partial class RangeSlider
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset both browser handles.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Canonical minimum getter for manual asynchronous callbacks.</summary>
    [Parameter] public Func<double>? AcceptedMinValueAccessor { get; set; }
    /// <summary>Canonical maximum getter for manual asynchronous callbacks.</summary>
    [Parameter] public Func<double>? AcceptedMaxValueAccessor { get; set; }
    private bool BrowserInteraction => (InteractionMode ?? BrowserPolicy?.Mode ?? BrowserDefaults.Value.DefaultMode) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private string[] BrowserValues => [MinValue.ToString(CultureInfo.InvariantCulture), MaxValue.ToString(CultureInfo.InvariantCulture)];
    private string[] AcceptedBrowserValues() => [(AcceptedMinValueAccessor?.Invoke() ?? MinValueExpression?.Compile().Invoke() ?? MinValue).ToString(CultureInfo.InvariantCulture), (AcceptedMaxValueAccessor?.Invoke() ?? MaxValueExpression?.Compile().Invoke() ?? MaxValue).ToString(CultureInfo.InvariantCulture)];
    private object BrowserOptions => new { min = Min, max = Max, step = Step, vertical = Orientation == SliderOrientation.Vertical, half = HandleHalfSizePx, updateOn = "change" };
    private async Task ChangeBrowser(string[] values)
    {
        if (values.Length != 2 || !double.TryParse(values[0],NumberStyles.Float,CultureInfo.InvariantCulture,out var min) || !double.TryParse(values[1],NumberStyles.Float,CultureInfo.InvariantCulture,out var max) || !double.IsFinite(min) || !double.IsFinite(max)) return;
        min = Math.Clamp(min,Min,Max); max = Math.Clamp(max,min,Max);
        var revision = InteractionRevision;
        if (min != MinValue) { if (!MinValueChanged.HasDelegate) MinValue = min; await MinValueChanged.InvokeAsync(min); }
        if (revision != InteractionRevision) return;
        if (max != MaxValue) { if (!MaxValueChanged.HasDelegate) MaxValue = max; await MaxValueChanged.InvokeAsync(max); }
        if (EditContext != null) { if (MinValueExpression != null) EditContext.NotifyFieldChanged(_minFieldIdentifier); if (MaxValueExpression != null) EditContext.NotifyFieldChanged(_maxFieldIdentifier); }
    }
}
