namespace BlazorUI.Components.Filter;

public partial class FilterValue : ComponentBase
{
    [Parameter, EditorRequired] public FilterCondition Condition { get; set; } = new();
    [Parameter] public FilterFieldDefinition? FieldDefinition { get; set; }
    [Parameter] public EventCallback<FilterCondition> ConditionChanged { get; set; }
    
    private List<string> MultiSelectValue
    {
        get
        {
            if (Condition.Value is List<string> list)
                return list;
            return new List<string>();
        }
        set
        {
            Condition.Value = value;
            NotifyConditionChanged();
        }
    }
    
    private string? TextValue
    {
        get => Condition.Value?.ToString();
        set
        {
            Condition.Value = value;
            NotifyConditionChanged();
        }
    }
    
    private string? NumberValue
    {
        get => Condition.Value?.ToString();
        set
        {
            if (decimal.TryParse(value, out var decimalValue))
            {
                Condition.Value = decimalValue;
            }
            else
            {
                Condition.Value = value;
            }
            NotifyConditionChanged();
        }
    }
    
    private string? SecondaryNumberValue
    {
        get => Condition.SecondaryValue?.ToString();
        set
        {
            if (decimal.TryParse(value, out var decimalValue))
            {
                Condition.SecondaryValue = decimalValue;
            }
            else
            {
                Condition.SecondaryValue = value;
            }
            NotifyConditionChanged();
        }
    }
    
    private DateOnly? DateValue
    {
        get
        {
            if (Condition.Value is DateOnly date)
                return date;
            if (Condition.Value is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);
            if (Condition.Value is string str && DateOnly.TryParse(str, out var parsed))
                return parsed;
            return null;
        }
        set
        {
            Condition.Value = value;
            NotifyConditionChanged();
        }
    }
    
    private DateOnly? SecondaryDateValue
    {
        get
        {
            if (Condition.SecondaryValue is DateOnly date)
                return date;
            if (Condition.SecondaryValue is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);
            if (Condition.SecondaryValue is string str && DateOnly.TryParse(str, out var parsed))
                return parsed;
            return null;
        }
        set
        {
            Condition.SecondaryValue = value;
            NotifyConditionChanged();
        }
    }
    
    private bool BoolValue
    {
        get => Condition.Value is bool b && b;
        set
        {
            Condition.Value = value;
            NotifyConditionChanged();
        }
    }
    
    private string? SelectValue
    {
        get => Condition.Value?.ToString();
        set
        {
            Condition.Value = value;
            NotifyConditionChanged();
        }
    }
    
    private bool IsValueNotNeeded()
    {
        return Condition.Operator switch
        {
            FilterOperator.IsEmpty => true,
            FilterOperator.IsNotEmpty => true,
            FilterOperator.IsTrue => true,
            FilterOperator.IsFalse => true,
            _ => false
        };
    }
    
    private void ToggleMultiSelectOption(string value)
    {
        var currentList = MultiSelectValue;
        if (currentList.Contains(value))
        {
            currentList.Remove(value);
        }
        else
        {
            currentList.Add(value);
        }
        MultiSelectValue = new List<string>(currentList);
    }
    
    private string GetMultiSelectButtonClass(bool isSelected)
    {
        return ClassNames.cn(
            "inline-flex items-center justify-center rounded-md px-3 py-1.5 text-sm font-medium",
            "transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
            "disabled:pointer-events-none disabled:opacity-50",
            isSelected
                ? "bg-primary text-primary-foreground hover:bg-primary/90"
                : "bg-muted text-muted-foreground hover:bg-muted/80"
        );
    }
    
    private async Task NotifyConditionChanged()
    {
        if (ConditionChanged.HasDelegate)
        {
            await ConditionChanged.InvokeAsync(Condition);
        }
    }
}
