using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using NeoUI.Blazor.Primitives;
using System.Linq.Expressions;
namespace NeoUI.Blazor;
public partial class TagInput
{
    [Inject] private IOptions<InteractionOptions> BrowserDefaults { get; set; } = default!;
    [CascadingParameter] private InteractionPolicy? BrowserPolicy { get; set; }
    [CascadingParameter] private EditContext? BrowserEditContext { get; set; }
    /// <summary>Overrides scoped and application interaction defaults.</summary>
    [Parameter] public InteractionMode? InteractionMode { get; set; }
    /// <summary>Increment to reset tags and the in-progress browser draft.</summary>
    [Parameter] public long InteractionRevision { get; set; }
    /// <summary>Bound field used for canonical acceptance and form notifications.</summary>
    [Parameter] public Expression<Func<IReadOnlyList<string>?>>? TagsExpression { get; set; }
    /// <summary>Canonical acceptance getter for manually wired callbacks.</summary>
    [Parameter] public Func<IReadOnlyList<string>?>? AcceptedTagsAccessor { get; set; }
    private bool BrowserInteraction => (InteractionMode ?? BrowserPolicy?.Mode ?? BrowserDefaults.Value.DefaultMode) == NeoUI.Blazor.Primitives.InteractionMode.Browser;
    private bool browserMounted;
    private string[] BrowserValues => _currentTags.ToArray();
    private string[] AcceptedBrowserValues() => (AcceptedTagsAccessor != null ? AcceptedTagsAccessor() : TagsExpression != null ? TagsExpression.Compile().Invoke() : Tags ?? _currentTags)?.ToArray() ?? [];
    private object BrowserOptions => new { triggers = (int)AddTrigger, maxTags = MaxTags, maxLength = MaxTagLength, duplicates = AllowDuplicates,
        suggestions = Suggestions?.ToArray() ?? [], search = OnSearchSuggestions != null, debounce = Math.Max(0,SuggestionDebounceMs),
        contentId = _contentId, removeLabel = Localizer["TagInput.RemoveTag", "{tag}"], badgeClass = Variant == TagInputVariant.Outline ? "inline-flex items-center gap-1 rounded-md border px-2 py-0.5 text-xs" : "inline-flex items-center gap-1 rounded-md bg-secondary text-secondary-foreground px-2 py-0.5 text-xs" };
    private async Task ChangeBrowser(string[] values)
    {
        var revision = InteractionRevision;
        var proposed = new List<string>();
        foreach (var raw in values) {
            var value = raw.Trim();
            if (string.IsNullOrEmpty(value) || value.Length > MaxTagLength || proposed.Count >= MaxTags || (!AllowDuplicates && proposed.Contains(value,StringComparer.OrdinalIgnoreCase)) || Validate?.Invoke(value) == false) { await OnTagRejected.InvokeAsync(value); }
            else proposed.Add(value);
            if (revision != InteractionRevision) return;
        }
        if (!TagsChanged.HasDelegate) { Tags = proposed; _currentTags = proposed; }
        await TagsChanged.InvokeAsync(proposed.AsReadOnly());
        if (revision != InteractionRevision) return;
        _currentTags = AcceptedBrowserValues().ToList();
        if (BrowserEditContext != null && TagsExpression != null) BrowserEditContext.NotifyFieldChanged(FieldIdentifier.Create(TagsExpression));
    }
    private async Task<string[]> SearchBrowser(string query)
    {
        _suggestionCts?.Cancel();_suggestionCts?.Dispose();_suggestionCts = new();
        try { return OnSearchSuggestions == null ? [] : (await OnSearchSuggestions(query,_suggestionCts.Token)).ToArray(); }
        catch (OperationCanceledException) { return []; }
    }
}
