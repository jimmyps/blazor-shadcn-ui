using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor.Filter;

/// <summary>
/// Renders the appropriate value input control for a filter condition based on the field type and operator.
/// </summary>
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
            Condition.Value = new List<string>();
            return (List<string>)Condition.Value;
        }
        set
        {
            Condition.Value = value;
            _ = NotifyConditionChanged();
        }
    }

    private string? TextValue
    {
        get => Condition.Value?.ToString();
        set
        {
            Condition.Value = value;
            _ = NotifyConditionChanged();
        }
    }

    private string? NumberValue
    {
        get => Condition.Value?.ToString();
        set
        {
            if (decimal.TryParse(value, out var d))
                Condition.Value = d;
            else
                Condition.Value = value;
            _ = NotifyConditionChanged();
        }
    }

    private string? SecondaryNumberValue
    {
        get => Condition.SecondaryValue?.ToString();
        set
        {
            if (decimal.TryParse(value, out var d))
                Condition.SecondaryValue = d;
            else
                Condition.SecondaryValue = value;
            _ = NotifyConditionChanged();
        }
    }

    private DateOnly? DateValue
    {
        get
        {
            if (Condition.Value is DateOnly date) return date;
            if (Condition.Value is DateTime dt) return DateOnly.FromDateTime(dt);
            if (Condition.Value is string s && DateOnly.TryParse(s, out var parsed)) return parsed;
            return null;
        }
        set
        {
            Condition.Value = value;
            _ = NotifyConditionChanged();
        }
    }

    private DateOnly? SecondaryDateValue
    {
        get
        {
            if (Condition.SecondaryValue is DateOnly date) return date;
            if (Condition.SecondaryValue is DateTime dt) return DateOnly.FromDateTime(dt);
            if (Condition.SecondaryValue is string s && DateOnly.TryParse(s, out var parsed)) return parsed;
            return null;
        }
        set
        {
            Condition.SecondaryValue = value;
            _ = NotifyConditionChanged();
        }
    }

    private bool BoolValue
    {
        get => Condition.Value is bool b && b;
        set
        {
            Condition.Value = value;
            _ = NotifyConditionChanged();
        }
    }

    private string? SelectValue
    {
        get => Condition.Value?.ToString();
        set
        {
            Condition.Value = value;
            _ = NotifyConditionChanged();
        }
    }

    private bool IsValueNotNeeded()
        => Condition.Operator is FilterOperator.IsEmpty or FilterOperator.IsNotEmpty
                               or FilterOperator.IsTrue or FilterOperator.IsFalse;

    private void ToggleMultiSelectOption(string value)
    {
        var currentList = MultiSelectValue;
        if (currentList.Contains(value))
            currentList.Remove(value);
        else
            currentList.Add(value);
        MultiSelectValue = new List<string>(currentList);
    }

    private string GetMultiSelectButtonClass(bool isSelected) => ClassNames.cn(
        "inline-flex items-center justify-center rounded-md px-3 py-1.5 text-sm font-medium",
        "transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
        "disabled:pointer-events-none disabled:opacity-50",
        isSelected
            ? "bg-primary text-primary-foreground hover:bg-primary/90"
            : "bg-muted text-muted-foreground hover:bg-muted/80"
    );

    private async Task NotifyConditionChanged()
    {
        if (ConditionChanged.HasDelegate)
            await ConditionChanged.InvokeAsync(Condition);
    }
}
