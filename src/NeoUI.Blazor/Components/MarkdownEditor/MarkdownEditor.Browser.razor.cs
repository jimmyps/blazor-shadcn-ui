using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
using System.Linq.Expressions;

namespace NeoUI.Blazor;
public partial class MarkdownEditor
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    [CascadingParameter] private PopoverContext? BrowserPopover { get; set; }
    /// <summary>Overrides the scoped or application interaction policy.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset the browser draft and editing history.</summary>
    [Parameter] public long ValueRevision { get; set; }
    /// <summary>Identifies the bound model for canonical acceptance and form notifications.</summary>
    [Parameter] public Expression<Func<string?>>? ValueExpression { get; set; }
    /// <summary>Canonical value getter for manually wired asynchronous callbacks.</summary>
    [Parameter] public Func<string?>? AcceptedValueAccessor { get; set; }
    /// <summary>Typing debounce; blur and form submission flush the latest draft.</summary>
    [Parameter] public int DebounceDelay { get; set; }
    /// <summary>Prevents editing while retaining form participation.</summary>
    [Parameter] public bool Readonly { get; set; }
    private bool? browserMode;
    private BrowserTextareaInput? browserInput;
    private readonly string browserId = $"markdown-{Guid.NewGuid():N}";
    private string BrowserId => Id ?? browserId;
    private bool BrowserInteraction => browserMode == true;
    private object BrowserFormat => new { kind = "markdown", revision = ValueRevision };
    private string? BrowserActionTarget => BrowserInteraction ? BrowserId : null;
    protected override void OnParametersSet()
    {
        var mode = ComposedInteraction.IsBrowser(InteractionMode, BrowserPolicy, BrowserDefaults.Value, BrowserPopover);
        if (browserMode.HasValue && browserMode != mode) throw new InvalidOperationException("InteractionMode cannot change after initialization. Recreate the component with @key.");
        browserMode = mode;
    }
    private async Task AcceptMarkdownDraft(string? value)
    {
        if (!ValueChanged.HasDelegate) Value = value;
        await ValueChanged.InvokeAsync(value);
        Value = AcceptedValueAccessor != null ? AcceptedValueAccessor() : ValueExpression != null ? ValueExpression.Compile().Invoke() : Value;
    }
    /// <summary>Synchronizes the latest markdown draft before custom application work.</summary>
    public Task<InputFlushResult> FlushAsync(int timeoutMilliseconds = 30000) => browserInput?.FlushAsync(timeoutMilliseconds)
        ?? Task.FromResult(new InputFlushResult { Status = BrowserInteraction ? InputFlushStatus.NotReady : InputFlushStatus.NotSupported });
}
