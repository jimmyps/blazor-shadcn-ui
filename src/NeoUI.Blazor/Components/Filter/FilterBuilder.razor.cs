using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor.Filter;

/// <summary>
/// A flexible and declarative filter builder component.
/// </summary>
/// <typeparam name="TData">The data model type being filtered.</typeparam>
public partial class FilterBuilder<TData> : ComponentBase, IFilterBuilderContext where TData : class
{
    private bool _isOpen;
    private readonly List<FilterFieldDefinition> _fields = new();
    private readonly List<FilterPresetDefinition> _presets = new();
    private FilterGroup _workingFilters = new();

    // ── Parameters ──────────────────────────────────────────────────────────

    /// <summary>Current filter state. Use @bind-Filters for two-way binding.</summary>
    [Parameter]
    public FilterGroup? Filters { get; set; }

    /// <summary>Callback fired when the filter state changes (two-way binding).</summary>
    [Parameter]
    public EventCallback<FilterGroup> FiltersChanged { get; set; }

    /// <summary>Callback fired immediately when Apply Filters is clicked.</summary>
    [Parameter]
    public EventCallback<FilterGroup> OnFilterChange { get; set; }

    /// <summary>Child content for FilterField declarations.</summary>
    [Parameter]
    public RenderFragment? FilterFields { get; set; }

    /// <summary>Child content for FilterPreset declarations.</summary>
    [Parameter]
    public RenderFragment? FilterPresets { get; set; }

    /// <summary>Label shown on the trigger button.</summary>
    [Parameter]
    public string ButtonText { get; set; } = "Filters";

    /// <summary>Visual variant of the trigger button.</summary>
    [Parameter]
    public ButtonVariant ButtonVariant { get; set; } = ButtonVariant.Outline;

    /// <summary>Size of the trigger button.</summary>
    [Parameter]
    public ButtonSize ButtonSize { get; set; } = ButtonSize.Default;

    /// <summary>When true, active filter chips are shown above the trigger button.</summary>
    [Parameter]
    public bool ShowChips { get; set; } = true;

    /// <summary>Additional CSS classes for the wrapper element.</summary>
    [Parameter]
    public string? Class { get; set; }

    // ── Computed props ───────────────────────────────────────────────────────

    private FilterGroup CurrentFilters => Filters ?? new FilterGroup();

    private string WrapperCssClass => ClassNames.cn("space-y-2", Class);

    private string ButtonCssClass => ClassNames.cn("gap-2");

    // ── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        _workingFilters = CloneFilterGroup(CurrentFilters);
    }

    protected override void OnParametersSet()
    {
        if (Filters != null)
            _workingFilters = CloneFilterGroup(Filters);
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

    // ── Internal helpers ─────────────────────────────────────────────────────

    private List<FilterCondition> GetActiveConditions()
        => CurrentFilters.Conditions.Where(c => !string.IsNullOrEmpty(c.Field)).ToList();

    private void AddCondition()
    {
        var newCondition = new FilterCondition();
        if (_fields.Any())
        {
            var firstField = _fields.First();
            newCondition.Field = firstField.Field;
            newCondition.Operator = firstField.DefaultOperator ?? firstField.Operators.FirstOrDefault();
        }
        _workingFilters.Conditions.Add(newCondition);
        StateHasChanged();
    }

    private void RemoveWorkingCondition(FilterCondition condition)
    {
        _workingFilters.Conditions.Remove(condition);
        StateHasChanged();
    }

    private void RemoveCondition(FilterCondition condition)
    {
        var activeList = new List<FilterCondition>(CurrentFilters.Conditions);
        activeList.RemoveAll(c => c.Id == condition.Id);
        var updated = new FilterGroup
        {
            Id = CurrentFilters.Id,
            Logic = CurrentFilters.Logic,
            Conditions = activeList,
            NestedGroups = CurrentFilters.NestedGroups
        };
        _ = NotifyFiltersChanged(updated);
        StateHasChanged();
    }

    private async Task ClearAll()
    {
        _workingFilters.Conditions.Clear();
        var updated = new FilterGroup { Id = CurrentFilters.Id, Logic = CurrentFilters.Logic };
        await NotifyFiltersChanged(updated);
        StateHasChanged();
    }

    private async Task Apply()
    {
        var updated = new FilterGroup
        {
            Id = CurrentFilters.Id,
            Logic = CurrentFilters.Logic,
            Conditions = _workingFilters.Conditions.Select(CloneCondition).ToList(),
            NestedGroups = CurrentFilters.NestedGroups
        };
        await NotifyFiltersChanged(updated);
        _isOpen = false;
        StateHasChanged();
    }

    private void Cancel()
    {
        _workingFilters = CloneFilterGroup(CurrentFilters);
        _isOpen = false;
        StateHasChanged();
    }

    private void ApplyPreset(FilterPresetDefinition preset)
    {
        _workingFilters.Conditions.Clear();
        _workingFilters.Conditions.AddRange(preset.Filters.Conditions.Select(CloneCondition));
        StateHasChanged();
    }

    private Task HandleConditionChanged(FilterCondition condition)
    {
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task NotifyFiltersChanged(FilterGroup updated)
    {
        if (FiltersChanged.HasDelegate)
            await FiltersChanged.InvokeAsync(updated);
        if (OnFilterChange.HasDelegate)
            await OnFilterChange.InvokeAsync(updated);
    }

    // ── Cloning ──────────────────────────────────────────────────────────────

    private static FilterGroup CloneFilterGroup(FilterGroup group) => new()
    {
        Id = group.Id,
        Logic = group.Logic,
        Conditions = group.Conditions.Select(CloneCondition).ToList(),
        NestedGroups = group.NestedGroups.Select(CloneFilterGroup).ToList()
    };

    private static FilterCondition CloneCondition(FilterCondition condition) => new()
    {
        Id = condition.Id,
        Field = condition.Field,
        Operator = condition.Operator,
        Value = condition.Value,
        SecondaryValue = condition.SecondaryValue
    };
}
