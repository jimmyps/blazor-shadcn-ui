using BlazorUI.Components.Badge;

namespace BlazorUI.Components.Filter;

public partial class FilterBuilder<TData> : ComponentBase, IFilterBuilderContext where TData : class
{
    private bool _isOpen;
    private List<FilterFieldDefinition> _fields = new();
    private List<FilterPresetDefinition> _presets = new();
    private FilterGroup _workingFilters = new();
    
    [Parameter] public FilterGroup? Filters { get; set; }
    [Parameter] public EventCallback<FilterGroup> FiltersChanged { get; set; }
    [Parameter] public EventCallback<FilterGroup> OnFilterChange { get; set; }
    [Parameter] public RenderFragment? FilterFields { get; set; }
    [Parameter] public RenderFragment? FilterPresets { get; set; }
    [Parameter] public string ButtonText { get; set; } = "Filters";
    [Parameter] public ButtonVariant ButtonVariant { get; set; } = ButtonVariant.Outline;
    [Parameter] public ButtonSize ButtonSize { get; set; } = ButtonSize.Default;
    [Parameter] public bool ShowChips { get; set; } = true;
    [Parameter] public string? Class { get; set; }
    
    private FilterGroup CurrentFilters
    {
        get => Filters ?? new FilterGroup();
        set
        {
            Filters = value;
            _workingFilters = CloneFilterGroup(value);
        }
    }
    
    private string WrapperCssClass => ClassNames.cn(
        "space-y-2",
        Class
    );
    
    private string ButtonCssClass => ClassNames.cn(
        "gap-2"
    );
    
    protected override void OnInitialized()
    {
        _workingFilters = CloneFilterGroup(CurrentFilters);
    }
    
    protected override void OnParametersSet()
    {
        if (Filters != null)
        {
            _workingFilters = CloneFilterGroup(Filters);
        }
    }
    
    public void RegisterField(FilterFieldDefinition field)
    {
        if (!_fields.Any(f => f.Field == field.Field))
        {
            _fields.Add(field);
        }
    }
    
    public void RegisterPreset(FilterPresetDefinition preset)
    {
        if (!_presets.Any(p => p.Name == preset.Name))
        {
            _presets.Add(preset);
        }
    }
    
    private List<FilterCondition> GetActiveConditions()
    {
        return CurrentFilters.Conditions
            .Where(c => !string.IsNullOrEmpty(c.Field))
            .ToList();
    }
    
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
    
    private void RemoveCondition(FilterCondition condition)
    {
        _workingFilters.Conditions.Remove(condition);
        CurrentFilters.Conditions.Remove(condition);
        NotifyFiltersChanged();
        StateHasChanged();
    }
    
    private async Task ClearAll()
    {
        _workingFilters.Conditions.Clear();
        CurrentFilters.Conditions.Clear();
        await NotifyFiltersChanged();
        StateHasChanged();
    }
    
    private async Task Apply()
    {
        CurrentFilters.Conditions.Clear();
        CurrentFilters.Conditions.AddRange(_workingFilters.Conditions);
        
        await NotifyFiltersChanged();
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
    
    private async Task HandleConditionChanged(FilterCondition condition)
    {
        StateHasChanged();
    }
    
    private async Task NotifyFiltersChanged()
    {
        if (FiltersChanged.HasDelegate)
        {
            await FiltersChanged.InvokeAsync(CurrentFilters);
        }
        
        if (OnFilterChange.HasDelegate)
        {
            await OnFilterChange.InvokeAsync(CurrentFilters);
        }
    }
    
    private FilterGroup CloneFilterGroup(FilterGroup group)
    {
        return new FilterGroup
        {
            Id = group.Id,
            Logic = group.Logic,
            Conditions = group.Conditions.Select(CloneCondition).ToList(),
            NestedGroups = group.NestedGroups.Select(CloneFilterGroup).ToList()
        };
    }
    
    private FilterCondition CloneCondition(FilterCondition condition)
    {
        return new FilterCondition
        {
            Id = condition.Id,
            Field = condition.Field,
            Operator = condition.Operator,
            Value = condition.Value,
            SecondaryValue = condition.SecondaryValue
        };
    }
}
