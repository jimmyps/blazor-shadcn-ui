using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace NeoUI.Blazor;

/// <summary>
/// A textarea component optimized for use within InputGroup.
/// </summary>
/// <remarks>
/// <para>
/// InputGroupTextarea is a specialized textarea designed to work seamlessly within
/// an InputGroup container. It removes standalone styling since the parent provides
/// the visual container, border, and focus management.
/// </para>
/// <para>
/// Features:
/// - Transparent background for seamless integration
/// - No border or focus ring (parent handles these)
/// - Flexible height based on content
/// - Resize control options
/// - Automatic marking for parent detection
/// - Optional input debouncing via <see cref="DebounceDelay"/> (handled in JS, no per-keystroke round-trip)
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;InputGroup&gt;
///     &lt;InputGroupTextarea Rows="4" Placeholder="Write a comment..." /&gt;
///     &lt;InputGroupAddon Align="InputGroupAlign.BlockEnd"&gt;
///         &lt;InputGroupText&gt;0 / 280&lt;/InputGroupText&gt;
///     &lt;/InputGroupAddon&gt;
/// &lt;/InputGroup&gt;
/// </code>
/// </example>
public partial class InputGroupTextarea : ComponentBase, IAsyncDisposable
{
    protected override void OnParametersSet() => ConfigureBrowserMode();

    private ElementReference textareaReference;

    /// <summary>
    /// Reference to the shared input JavaScript module.
    /// </summary>
    private IJSObjectReference? _inputModule;

    /// <summary>
    /// DotNet object reference for JavaScript callbacks.
    /// </summary>
    private DotNetObjectReference<InputGroupTextarea>? _dotNetRef;

    /// <summary>
    /// Tracks whether the JavaScript module has been initialized.
    /// </summary>
    private bool _jsInitialized;

    /// <summary>
    /// Auto-generated ID used when the user does not provide one, so JS can reference the element.
    /// </summary>
    private string? _generatedId;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets or sets the current value of the textarea.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the textarea value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the number of visible text rows.
    /// </summary>
    /// <remarks>
    /// Default is 3 rows. The textarea can grow beyond this if resize is enabled.
    /// </remarks>
    [Parameter]
    public int Rows { get; set; } = 3;

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea is required.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets when the textarea should update its bound value.
    /// </summary>
    /// <remarks>
    /// - Input: Updates value on every input event (keystroke). Required for <see cref="DebounceDelay"/> to apply.
    /// - Change: Updates value only when the textarea loses focus.
    /// Defaults to <see cref="InputUpdateMode.Input"/> to preserve InputGroupTextarea's historical
    /// update-on-keystroke behavior.
    /// </remarks>
    [Parameter]
    public InputUpdateMode UpdateOn { get; set; } = InputUpdateMode.Input;

    /// <summary>
    /// Gets or sets the debounce delay in milliseconds before triggering value change notifications.
    /// Only applies when <see cref="UpdateOn"/> is <see cref="InputUpdateMode.Input"/>. Set to 0 for immediate updates.
    /// Default: 0 (no debounce)
    /// </summary>
    /// <remarks>
    /// Debouncing is handled inside the shared input JavaScript module, so intermediate keystrokes
    /// do not round-trip to the server (Blazor Server) or spin the renderer (WebAssembly). Only the
    /// final, debounced value is dispatched to <see cref="ValueChanged"/>.
    /// </remarks>
    [Parameter]
    public int DebounceDelay { get; set; } = 0;

    /// <summary>
    /// Gets or sets the ARIA label.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ARIA described-by attribute.
    /// </summary>
    [Parameter]
    public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea value is invalid.
    /// </summary>
    [Parameter]
    public bool? AriaInvalid { get; set; }

    /// <summary>
    /// Gets or sets additional attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the effective ID, generating a unique one when none is provided so JS can always
    /// reference the element.
    /// </summary>
    private string EffectiveId
    {
        get
        {
            if (!string.IsNullOrEmpty(Id))
                return Id;

            _generatedId ??= "input-group-textarea-" + Guid.NewGuid().ToString("N")[..6];
            return _generatedId;
        }
    }

    /// <summary>
    /// Gets the computed CSS classes for the textarea element.
    /// </summary>
    /// <remarks>
    /// Uses minimal styling since the parent InputGroup provides the visual container.
    /// Prevents resize by default for better control over layout.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base styles - minimal for group context
        "flex-1 bg-transparent px-3 py-2 text-base min-h-[60px]",
        "border-0 rounded-none", // No border or radius for seamless integration
        "placeholder:text-muted-foreground",
        "focus-visible:outline-none",
        "disabled:cursor-not-allowed disabled:opacity-50",
        "resize-none", // Prevent resize for cleaner appearance
        // Medium screens and up: smaller text
        "md:text-sm",
        // Custom classes
        Class
    );

    /// <summary>
    /// Sets up input event handling (UpdateOn mode + debounce) through the shared input JS module.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (BrowserInteraction) return;
        if (!firstRender)
            return;

        try
        {
            _inputModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NeoUI.Blazor/js/input.js");

            _dotNetRef = DotNetObjectReference.Create(this);

            // Initialize input event handling with UpdateOn mode and debounce.
            // The JS module owns the debounce timer, so intermediate keystrokes never
            // cross the circuit — only the final value invokes OnInputChanged.
            await _inputModule.InvokeVoidAsync(
                "initializeInput",
                EffectiveId,
                UpdateOn.ToString().ToLower(),
                DebounceDelay,
                _dotNetRef
            );

            _jsInitialized = true;
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or TaskCanceledException or ObjectDisposedException)
        {
            // JS module not available (e.g. circuit disconnect); the element still renders,
            // value simply won't be wired through JS event handling.
        }
    }

    /// <summary>
    /// Called from JavaScript when the textarea value changes, based on UpdateOn mode and debounce settings.
    /// </summary>
    /// <param name="value">The new textarea value.</param>
    [JSInvokable]
    public async Task OnInputChanged(string? value)
    {
        Value = value;

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }

        // Deliberately no StateHasChanged() here to avoid re-rendering (and cursor jumps) during typing.
    }

    /// <summary>
    /// Moves keyboard focus to the textarea element.
    /// </summary>
    public ValueTask FocusAsync() => textareaReference.FocusAsync();

    /// <summary>
    /// Disposes the JavaScript module, event handlers, and object references.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        try
        {
            if (_inputModule is not null)
            {
                // Dispose the module even if initializeInput threw after a successful import.
                // Only tear down JS-side state when initialization actually completed.
                if (_jsInitialized)
                {
                    await _inputModule.InvokeVoidAsync("disposeInput", EffectiveId);
                }

                await _inputModule.DisposeAsync();
            }
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or TaskCanceledException or ObjectDisposedException)
        {
            // JS runtime already gone; nothing to clean up.
        }

        _inputModule = null;
        _dotNetRef?.Dispose();
        _dotNetRef = null;
    }
}
