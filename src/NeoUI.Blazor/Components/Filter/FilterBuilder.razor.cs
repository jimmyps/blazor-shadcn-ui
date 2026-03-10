using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A declarative, composable filter builder that renders as an inline canvas toolbar.
/// Conditions are applied immediately — no Apply/Cancel steps needed.
/// </summary>
/// <typeparam name="TData">The data model type being filtered.</typeparam>
public partial class FilterBuilder<TData> : ComponentBase, IFilterBuilderContext where TData : class
{
    private readonly List<FilterFieldDefinition> _fields = new();
    private readonly List<FilterPresetDefinition> _presets = new();

    // Live condition list — the single source of truth for the filter state.
    private List<FilterCondition> _conditions = new();

    // Track the last Filters we emitted so we don't re-sync our own changes.
    private HashSet<string> _lastEmittedIds = new();

    // ── Parameters ──────────────────────────────────────────────────────────

    /// <summary>Current filter state. Use @bind-Filters for two-way binding.</summary>
    [Parameter]
    public FilterGroup? Filters { get; set; }

    /// <summary>Callback for two-way binding of the filter state.</summary>
    [Parameter]
    public EventCallback<FilterGroup> FiltersChanged { get; set; }

    /// <summary>Fires immediately whenever the active conditions change.</summary>
    [Parameter]
    public EventCallback<FilterGroup> OnFilterChange { get; set; }

    /// <summary>Child content slot for <see cref="FilterField"/> declarations.</summary>
    [Parameter]
    public RenderFragment? FilterFields { get; set; }

    /// <summary>Child content slot for <see cref="FilterPreset"/> declarations.</summary>
    [Parameter]
    public RenderFragment? FilterPresets { get; set; }

    /// <summary>Label shown on the add-filter button when no conditions are active.</summary>
    [Parameter]
    public string ButtonText { get; set; } = "Filter";

    /// <summary>Additional CSS classes for the wrapper element.</summary>
    [Parameter]
    public string? Class { get; set; }

    // ── Computed ─────────────────────────────────────────────────────────────

    private string WrapperCssClass => ClassNames.cn(Class);

    // ── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        if (Filters != null)
        {
            _conditions = Filters.Conditions.Select(CloneCondition).ToList();
            _lastEmittedIds = _conditions.Select(c => c.Id).ToHashSet();
        }
    }

    protected override void OnParametersSet()
    {
        if (Filters == null) return;

        // Only re-sync from external Filters if their condition IDs differ from what we last emitted.
        // This prevents us from overwriting local edits with our own notification.
        var externalIds = Filters.Conditions.Select(c => c.Id).ToHashSet();
        if (!externalIds.SetEquals(_lastEmittedIds))
        {
            _conditions = Filters.Conditions.Select(CloneCondition).ToList();
            _lastEmittedIds = _conditions.Select(c => c.Id).ToHashSet();
        }
    }

    // ── IFilterBuilderContext ────────────────────────────────────────────────

    /// <inheritdoc/>
    public void RegisterField(FilterFieldDefinition field)
    {
        if (!_fields.Any(f => f.Field == field.Field))
            _fields.Add(field);
    }

    /// <inheritdoc/>
    public void RegisterPreset(FilterPresetDefinition preset)
    {
        if (!_presets.Any(p => p.Name == preset.Name))
            _presets.Add(preset);
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    private void AddCondition(FilterFieldDefinition field)
    {
        var newCond = new FilterCondition
        {
            Field = field.Field,
            Operator = field.DefaultOperator ?? field.Operators.FirstOrDefault()
        };
        _conditions.Add(newCond);
        _ = NotifyFiltersChanged();
        StateHasChanged();
    }

    private void RemoveCondition(FilterCondition condition)
    {
        _conditions.Remove(condition);
        _ = NotifyFiltersChanged();
        StateHasChanged();
    }

    private async Task ClearAll()
    {
        _conditions.Clear();
        await NotifyFiltersChanged();
        StateHasChanged();
    }

    private void ApplyPreset(FilterPresetDefinition preset)
    {
        _conditions.Clear();
        _conditions.AddRange(preset.Filters.Conditions.Select(CloneCondition));
        _ = NotifyFiltersChanged();
        StateHasChanged();
    }

    private Task HandleConditionChanged(FilterCondition condition)
    {
        _ = NotifyFiltersChanged();
        return Task.CompletedTask;
    }

    private async Task NotifyFiltersChanged()
    {
        var updated = new FilterGroup
        {
            Id = Filters?.Id ?? Guid.NewGuid().ToString(),
            Logic = Filters?.Logic ?? LogicalOperator.And,
            Conditions = _conditions.Select(CloneCondition).ToList(),
            NestedGroups = Filters?.NestedGroups ?? new()
        };
        _lastEmittedIds = updated.Conditions.Select(c => c.Id).ToHashSet();

        if (FiltersChanged.HasDelegate)
            await FiltersChanged.InvokeAsync(updated);
        if (OnFilterChange.HasDelegate)
            await OnFilterChange.InvokeAsync(updated);
    }

    // ── Cloning ───────────────────────────────────────────────────────────────

    private static FilterCondition CloneCondition(FilterCondition c) => new()
    {
        Id = c.Id,
        Field = c.Field,
        Operator = c.Operator,
        Value = c.Value,
        SecondaryValue = c.SecondaryValue
    };
}
