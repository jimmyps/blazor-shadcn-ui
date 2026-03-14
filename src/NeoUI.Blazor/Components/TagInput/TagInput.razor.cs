using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace NeoUI.Blazor;

/// <summary>
/// A tag/chip input component for managing a list of string values.
/// </summary>
/// <remarks>
/// <para>
/// Supports configurable trigger keys, duplicate prevention, validation, static and async
/// suggestions with debounce, custom tag templates, keyboard navigation, and clipboard paste
/// splitting on delimiters.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;TagInput @bind-Tags="_tags"
///            Placeholder="Add skill…"
///            AddTrigger="TagInputTrigger.Enter | TagInputTrigger.Comma"
///            Clearable="true" /&gt;
/// </code>
/// </example>
public partial class TagInput : ComponentBase, IAsyncDisposable
{
    // ── State ──────────────────────────────────────────────────────────
    private List<string> _currentTags = new();
    private string _inputText = string.Empty;
    private List<string> _filteredSuggestions = new();
    private bool _suggestionsOpen;
    private int _suggestionIndex = -1;
    private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];
    private string _portalId => $"tag-input-portal-{_instanceId}";
    private string _containerId => $"tag-input-container-{_instanceId}";
    private string _contentId  => $"tag-input-content-{_instanceId}";
    private ElementReference _containerRef;
    private ElementReference _inputRef;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<TagInput>? _dotNetRef;
    private bool _jsInitialized;
    private bool _disposed;
    private CancellationTokenSource? _suggestionCts;
    private CancellationTokenSource? _blurCts;

    // ── ShouldRender tracking ──────────────────────────────────────────
    private IReadOnlyList<string>? _lastTags;
    private string _lastInputText = string.Empty;
    private bool _lastSuggestionsOpen;
    private int _lastSuggestionIndex = -1;
    private bool _lastDisabled;
    private int _lastTagsCount = -1;

    // ── Remove handler cache ───────────────────────────────────────────
    private readonly Dictionary<string, Func<Task>> _removeHandlerCache = new();

    // ── Parameters ────────────────────────────────────────────────────

    /// <summary>Gets or sets the list of tags. Use @bind-Tags for two-way binding.</summary>
    [Parameter]
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>Callback invoked when the tag list changes.</summary>
    [Parameter]
    public EventCallback<IReadOnlyList<string>?> TagsChanged { get; set; }

    /// <summary>Placeholder text shown when no tags are present.</summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>Maximum number of tags allowed.</summary>
    [Parameter]
    public int MaxTags { get; set; } = int.MaxValue;

    /// <summary>Maximum character length for a single tag.</summary>
    [Parameter]
    public int MaxTagLength { get; set; } = 50;

    /// <summary>Whether duplicate tag values are allowed (case-insensitive).</summary>
    [Parameter]
    public bool AllowDuplicates { get; set; }

    /// <summary>Which keys trigger tag creation. Combine flags with bitwise OR.</summary>
    [Parameter]
    public TagInputTrigger AddTrigger { get; set; } = TagInputTrigger.Enter | TagInputTrigger.Comma;

    /// <summary>Optional validation function. Return false to reject a tag.</summary>
    [Parameter]
    public Func<string, bool>? Validate { get; set; }

    /// <summary>Callback invoked when a tag is rejected (duplicate, validation failure, limit).</summary>
    [Parameter]
    public EventCallback<string> OnTagRejected { get; set; }

    /// <summary>Static list of suggestions shown as the user types.</summary>
    [Parameter]
    public IEnumerable<string>? Suggestions { get; set; }

    /// <summary>Async function to load suggestions based on the search query.</summary>
    [Parameter]
    public Func<string, CancellationToken, Task<IEnumerable<string>>>? OnSearchSuggestions { get; set; }

    /// <summary>Debounce interval in milliseconds for suggestion search.</summary>
    [Parameter]
    public int SuggestionDebounceMs { get; set; } = 300;

    /// <summary>Visual variant for rendered tags.</summary>
    [Parameter]
    public TagInputVariant Variant { get; set; } = TagInputVariant.Default;

    /// <summary>Whether to show a clear-all button when tags are present.</summary>
    [Parameter]
    public bool Clearable { get; set; }

    /// <summary>Whether the component is disabled.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Optional custom template for rendering each tag.</summary>
    [Parameter]
    public RenderFragment<string>? TagTemplate { get; set; }

    /// <summary>ARIA label for the input container.</summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    // ── Computed ───────────────────────────────────────────────────────

    private bool HasSuggestions => Suggestions is not null || OnSearchSuggestions is not null;
    private string? EffectivePlaceholder => _currentTags.Count == 0 ? (Placeholder ?? "Add tag…") : null;

    private string? ActiveDescendantId =>
        _suggestionsOpen && _suggestionIndex >= 0
            ? $"{_instanceId}-suggestion-{_suggestionIndex}"
            : null;

    private BadgeVariant TagBadgeVariant => Variant switch
    {
        TagInputVariant.Outline   => BadgeVariant.Outline,
        TagInputVariant.Secondary => BadgeVariant.Secondary,
        _                         => BadgeVariant.Secondary
    };

    // ── Lifecycle ──────────────────────────────────────────────────────

    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_lastTags, Tags))
        {
            _currentTags = Tags?.ToList() ?? new List<string>();
            _removeHandlerCache.Clear();
            _lastTags = Tags;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        try
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NeoUI.Blazor/js/tag-input.js");
            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsModule.InvokeVoidAsync("initialize",
                _containerRef, _inputRef, _dotNetRef, _instanceId,
                new { triggers = (int)AddTrigger });
            _jsInitialized = true;
        }
        catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException or InvalidOperationException)
        {
            // Expected during prerendering or circuit disconnect
            _ = ex;
        }
    }

    protected override bool ShouldRender()
    {
        var changed =
            _lastInputText != _inputText ||
            _lastSuggestionsOpen != _suggestionsOpen ||
            _lastSuggestionIndex != _suggestionIndex ||
            _lastDisabled != Disabled ||
            _lastTagsCount != _currentTags.Count;

        if (changed)
        {
            _lastInputText = _inputText;
            _lastSuggestionsOpen = _suggestionsOpen;
            _lastSuggestionIndex = _suggestionIndex;
            _lastDisabled = Disabled;
            _lastTagsCount = _currentTags.Count;
        }
        return changed;
    }

    // ── JS-invokable callbacks ─────────────────────────────────────────

    [JSInvokable]
    public async Task JsTriggerAdd()
    {
        if (_disposed || Disabled) return;
        if (_suggestionsOpen && _suggestionIndex >= 0 && _suggestionIndex < _filteredSuggestions.Count)
            await TryAddTag(_filteredSuggestions[_suggestionIndex]);
        else
            await TryAddTag(_inputText);
        StateHasChanged();
    }

    [JSInvokable]
    public async Task JsBackspace()
    {
        if (_disposed || Disabled || _currentTags.Count == 0) return;
        var last = _currentTags[^1];
        _currentTags.RemoveAt(_currentTags.Count - 1);
        _removeHandlerCache.Remove(last);
        await TagsChanged.InvokeAsync(_currentTags.AsReadOnly());
        StateHasChanged();
    }

    [JSInvokable]
    public void JsSuggestionNext()
    {
        if (!_suggestionsOpen) return;
        _suggestionIndex = Math.Min(_suggestionIndex + 1, _filteredSuggestions.Count - 1);
        StateHasChanged();
    }

    [JSInvokable]
    public void JsSuggestionPrev()
    {
        if (!_suggestionsOpen) return;
        _suggestionIndex = Math.Max(_suggestionIndex - 1, 0);
        StateHasChanged();
    }

    [JSInvokable]
    public void JsSuggestionClose()
    {
        _suggestionsOpen = false;
        _suggestionIndex = -1;
        StateHasChanged();
    }

    [JSInvokable]
    public async Task JsPasteText(string text)
    {
        if (_disposed || Disabled || string.IsNullOrWhiteSpace(text)) return;
        var delimiters = new List<char>();
        if ((AddTrigger & TagInputTrigger.Comma)     != 0) delimiters.Add(',');
        if ((AddTrigger & TagInputTrigger.Semicolon) != 0) delimiters.Add(';');
        if ((AddTrigger & TagInputTrigger.Space)     != 0) delimiters.Add(' ');
        var parts = text.Split(delimiters.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts) await TryAddTag(part.Trim());
        StateHasChanged();
    }

    // ── Event handlers ─────────────────────────────────────────────────

    private async Task HandleInput(ChangeEventArgs e)
    {
        _inputText = e.Value?.ToString() ?? string.Empty;
        await SearchSuggestionsAsync(_inputText);
    }

    private void HandleFocus(FocusEventArgs _)
    {
        _blurCts?.Cancel();
        _blurCts = null;
    }

    private async Task HandleBlur(FocusEventArgs _)
    {
        _blurCts = new CancellationTokenSource();
        var token = _blurCts.Token;
        try
        {
            await Task.Delay(150, token);
            if (!token.IsCancellationRequested)
            {
                _suggestionsOpen = false;
                _suggestionIndex = -1;
                StateHasChanged();
            }
        }
        catch (OperationCanceledException) { }
    }

    private async Task ClearAll()
    {
        _currentTags.Clear();
        _removeHandlerCache.Clear();
        await TagsChanged.InvokeAsync(null);
        StateHasChanged();
    }

    // ── Helpers ────────────────────────────────────────────────────────

    private Func<Task> GetRemoveHandler(string tag)
    {
        if (!_removeHandlerCache.TryGetValue(tag, out var handler))
        {
            handler = async () =>
            {
                _currentTags.Remove(tag);
                _removeHandlerCache.Remove(tag);
                await TagsChanged.InvokeAsync(_currentTags.AsReadOnly());
                StateHasChanged();
            };
            _removeHandlerCache[tag] = handler;
        }
        return handler;
    }

    private async Task TryAddTag(string raw)
    {
        var value = raw.Trim();
        if (string.IsNullOrEmpty(value)) return;
        if (value.Length > MaxTagLength) { await OnTagRejected.InvokeAsync(value); return; }
        if (_currentTags.Count >= MaxTags) { await OnTagRejected.InvokeAsync(value); return; }
        if (!AllowDuplicates && _currentTags.Contains(value, StringComparer.OrdinalIgnoreCase))
        { await OnTagRejected.InvokeAsync(value); return; }
        if (Validate is not null && !Validate(value)) { await OnTagRejected.InvokeAsync(value); return; }

        _currentTags.Add(value);
        _inputText = string.Empty;
        _suggestionsOpen = false;
        _suggestionIndex = -1;
        _filteredSuggestions.Clear();
        await TagsChanged.InvokeAsync(_currentTags.AsReadOnly());

        try
        {
            if (_jsModule is not null && _jsInitialized)
                await _jsModule.InvokeVoidAsync("focusInput", _instanceId);
        }
        catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException) { }
    }

    private async Task SearchSuggestionsAsync(string query)
    {
        _suggestionCts?.Cancel();
        _suggestionCts?.Dispose();
        _suggestionCts = new CancellationTokenSource();
        var token = _suggestionCts.Token;

        if (string.IsNullOrWhiteSpace(query)) { _suggestionsOpen = false; return; }

        try
        {
            await Task.Delay(SuggestionDebounceMs, token);
            IEnumerable<string>? results = null;
            if (OnSearchSuggestions is not null)
                results = await OnSearchSuggestions(query, token);
            else if (Suggestions is not null)
                results = Suggestions.Where(s => s.Contains(query, StringComparison.OrdinalIgnoreCase));

            if (results is null || token.IsCancellationRequested) return;

            _filteredSuggestions = results
                .Where(s => AllowDuplicates || !_currentTags.Contains(s, StringComparer.OrdinalIgnoreCase))
                .ToList();
            _suggestionsOpen = _filteredSuggestions.Count > 0;
            _suggestionIndex = -1;
            StateHasChanged();
        }
        catch (OperationCanceledException) { }
    }

    private async Task SelectSuggestion(string suggestion)
    {
        _blurCts?.Cancel();
        await TryAddTag(suggestion);
        StateHasChanged();
        try
        {
            if (_jsModule is not null && _jsInitialized)
                await _jsModule.InvokeVoidAsync("focusInput", _instanceId);
        }
        catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException) { _ = ex; }
    }

    // ── CSS ────────────────────────────────────────────────────────────

    private string ContainerCssClass => ClassNames.cn(
        "flex flex-wrap items-center gap-1.5 min-h-9 w-full rounded-md",
        "border border-input px-3 py-1.5 text-sm",
        Variant == TagInputVariant.Secondary ? "bg-secondary/50" : "bg-background",
        "ring-offset-background transition-[color,box-shadow]",
        "focus-within:outline-none focus-within:ring-2 focus-within:ring-ring/50",
        _suggestionsOpen ? "ring-2 ring-ring/50" : null,
        Disabled ? "opacity-50 cursor-not-allowed" : null,
        Class
    );

    private static string InputCssClass =>
        "flex-1 min-w-[120px] bg-transparent outline-none placeholder:text-muted-foreground disabled:cursor-not-allowed text-sm";

    private static string SuggestionItemCssClass(bool isActive) => ClassNames.cn(
        "flex cursor-pointer select-none items-center rounded-sm px-2 py-1.5 text-sm",
        "hover:bg-accent hover:text-accent-foreground",
        isActive ? "bg-accent text-accent-foreground" : null
    );

    // ── IAsyncDisposable ───────────────────────────────────────────────

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
        _suggestionCts?.Cancel();
        _suggestionCts?.Dispose();
        _blurCts?.Cancel();
        _blurCts?.Dispose();

        if (_jsModule is not null && _jsInitialized)
        {
            try
            {
                await _jsModule.InvokeVoidAsync("dispose", _instanceId);
                await _jsModule.DisposeAsync();
            }
            catch (Exception ex) when (ex is JSDisconnectedException or TaskCanceledException or ObjectDisposedException or InvalidOperationException) { _ = ex; }
            _jsModule = null;
        }
        _dotNetRef?.Dispose();
        _dotNetRef = null;
    }
}
