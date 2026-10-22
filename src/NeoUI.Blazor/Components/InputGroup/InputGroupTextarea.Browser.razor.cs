using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
using System.Linq.Expressions;

namespace NeoUI.Blazor;

public partial class InputGroupTextarea
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    [CascadingParameter] private PopoverContext? BrowserPopover { get; set; }
    /// <summary>Omitted settings inherit the floating, scoped or application policy.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to replace the current browser draft, including same-value resets.</summary>
    [Parameter] public long ValueRevision { get; set; }
    /// <summary>Reads canonical acceptance for manually wired callbacks without waiting for rendering.</summary>
    [Parameter] public Func<string?>? AcceptedValueAccessor { get; set; }
    private Dictionary<string, object> BrowserAttributes => new(AdditionalAttributes ?? new()) { ["data-slot"] = "input-group-control" };
    private bool? browserMode;
    private Input? browserInput;
    private bool BrowserInteraction => browserMode == true;
    private void ConfigureBrowserMode()
    {
        var mode = ComposedInteraction.IsBrowser(InteractionMode, BrowserPolicy, BrowserDefaults.Value, BrowserPopover);
        if (browserMode.HasValue && browserMode != mode)
            throw new InvalidOperationException("InteractionMode cannot change after initialization. Recreate the component with @key.");
        browserMode = mode;
    }
    /// <summary>Synchronizes the latest browser draft before custom application work.</summary>
    public Task<InputFlushResult> FlushAsync(int timeoutMilliseconds = 30000) => browserInput?.FlushAsync(timeoutMilliseconds)
        ?? Task.FromResult(new InputFlushResult { Status = BrowserInteraction ? InputFlushStatus.NotReady : InputFlushStatus.NotSupported });
    /// <summary>Identifies the model field for acceptance and EditContext notifications.</summary>
    [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
    /// <summary>Prevents editing while retaining focus and form participation.</summary>
    [Parameter] public bool Readonly { get; set; }
}
