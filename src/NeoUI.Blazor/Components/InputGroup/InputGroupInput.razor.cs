using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace NeoUI.Blazor;

/// <summary>
/// An input component optimized for use within InputGroup.
/// </summary>
/// <remarks>
/// <para>
/// InputGroupInput is a specialized version of the Input component designed to work
/// seamlessly within an InputGroup container. It removes the standalone input styling
/// (border, background, focus ring) since those are provided by the parent InputGroup.
/// </para>
/// <para>
/// Features:
/// - Transparent background for seamless integration
/// - No border or focus ring (parent handles these)
/// - Flexible width to fill available space
/// - Full parameter compatibility with standard Input
/// - Automatic marking for parent detection (data-slot attribute)
/// - Optional input debouncing via <see cref="DebounceDelay"/> (handled in JS, no per-keystroke round-trip)
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;InputGroup&gt;
///     &lt;InputGroupInput Type="InputType.Email" Placeholder="Enter email" /&gt;
/// &lt;/InputGroup&gt;
/// </code>
/// </example>
public partial class InputGroupInput : ComponentBase, IAsyncDisposable
{
    protected override void OnParametersSet() => ConfigureBrowserMode();

    private ElementReference _inputRef;

    /// <summary>
    /// Reference to the shared input JavaScript module.
    /// </summary>
    private IJSObjectReference? _inputModule;

    /// <summary>
    /// DotNet object reference for JavaScript callbacks.
    /// </summary>
    private DotNetObjectReference<InputGroupInput>? _dotNetRef;

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
    /// Gets or sets the type of input.
    /// </summary>
    [Parameter]
    public InputType Type { get; set; } = InputType.Text;

    /// <summary>
    /// Gets or sets the current value of the input.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the input value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets whether the input is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the input is required.
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
    /// Gets or sets when the input should update its bound value.
    /// </summary>
    /// <remarks>
    /// - Input: Updates value on every input event (keystroke). Required for <see cref="DebounceDelay"/> to apply.
    /// - Change: Updates value only when the input loses focus.
    /// Defaults to <see cref="InputUpdateMode.Input"/> to preserve InputGroupInput's historical
    /// update-on-keystroke behavior (this differs from the standalone Input default of Change).
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
    /// Gets or sets whether the input value is invalid.
    /// </summary>
    [Parameter]
    public bool? AriaInvalid { get; set; }

    /// <summary>
    /// Gets or sets additional attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Callback invoked after first render with the input's ElementReference.
    /// Use this for JS interop operations like focusing or event setup.
    /// </summary>
    [Parameter]
    public Action<ElementReference>? OnInputRef { get; set; }

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

            _generatedId ??= "input-group-input-" + Guid.NewGuid().ToString("N")[..6];
            return _generatedId;
        }
    }

    /// <summary>
    /// Gets the computed CSS classes for the input element.
    /// </summary>
    /// <remarks>
    /// Uses minimal styling since the parent InputGroup provides the visual container.
    /// Focuses on text rendering, placeholder, and disabled state only.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base styles - minimal for group context
        "flex-1 bg-transparent px-3 py-2 text-base",
        "border-0 rounded-none", // No border or radius for seamless integration
        "placeholder:text-muted-foreground",
        "focus-visible:outline-none",
        "disabled:cursor-not-allowed disabled:opacity-50",
        // File input styling
        "file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground",
        // Medium screens and up: smaller text
        "md:text-sm",
        // Custom classes
        Class
    );

    /// <summary>
    /// Gets the HTML input type attribute value.
    /// </summary>
    private string HtmlType => Type switch
    {
        InputType.Text => "text",
        InputType.Email => "email",
        InputType.Password => "password",
        InputType.Number => "number",
        InputType.Tel => "tel",
        InputType.Url => "url",
        InputType.Search => "search",
        InputType.Date => "date",
        InputType.Time => "time",
        InputType.File => "file",
        _ => "text"
    };

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

        OnInputRef?.Invoke(_inputRef);
    }

    /// <summary>
    /// Called from JavaScript when the input value changes, based on UpdateOn mode and debounce settings.
    /// </summary>
    /// <param name="value">The new input value.</param>
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
    /// Moves keyboard focus to the input element.
    /// </summary>
    public ValueTask FocusAsync() => _inputRef.FocusAsync();

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
