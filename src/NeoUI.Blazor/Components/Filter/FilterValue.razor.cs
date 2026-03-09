using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace NeoUI.Blazor.Filter;

/// <summary>
/// Renders the appropriate value input for a filter condition, driven by
/// the field's <see cref="FilterFieldDefinition.EditorType"/>.
/// </summary>
public partial class FilterValue : ComponentBase
{
    [Parameter, EditorRequired] public FilterCondition Condition { get; set; } = new();
    [Parameter] public FilterFieldDefinition? FieldDefinition { get; set; }
    [Parameter] public EventCallback<FilterCondition> ConditionChanged { get; set; }

    /// <summary>
    /// When <c>true</c>, uses borderless/transparent compact styling designed for use
    /// inside a <see cref="FilterChip"/>.
    /// </summary>
    [Parameter]
    public bool Compact { get; set; }

    // ── Computed helpers ─────────────────────────────────────────────────────

    private bool IsBetween => Condition.Operator is FilterOperator.Between or FilterOperator.NotBetween;

    private bool IsValueNotNeeded()
        => Condition.Operator is FilterOperator.IsEmpty or FilterOperator.IsNotEmpty
                               or FilterOperator.IsTrue or FilterOperator.IsFalse;

    /// <summary>Resolves the effective editor type from the field definition, defaulting via AutoInfer.</summary>
    private FilterEditorType EffectiveEditorType
    {
        get
        {
            var declared = FieldDefinition?.EditorType ?? FilterEditorType.Auto;
            if (declared != FilterEditorType.Auto)
                return declared;

            return FieldDefinition?.Type switch
            {
                FilterFieldType.Text        => FilterEditorType.Input,
                FilterFieldType.Number      => FilterEditorType.Numeric,
                FilterFieldType.Date        => FilterEditorType.Date,
                FilterFieldType.DateRange   => FilterEditorType.DateRange,
                FilterFieldType.Boolean     => FilterEditorType.Boolean,
                FilterFieldType.Select      => FilterEditorType.Select,
                FilterFieldType.MultiSelect => FilterEditorType.MultiSelect,
                FilterFieldType.Custom      => FilterEditorType.Custom,
                _                           => FilterEditorType.Input
            };
        }
    }

    /// <summary>CSS class for compact-mode or full-size input.</summary>
    private string CompactOrFullInput => Compact
        ? "border-0 shadow-none rounded-none bg-transparent focus-visible:ring-0 h-full py-0 text-sm px-2 min-w-[80px] max-w-[180px] w-auto"
        : "";

    // ── String properties ────────────────────────────────────────────────────

    private string? TextValue
    {
        get => Condition.Value?.ToString();
        set { Condition.Value = value; _ = NotifyConditionChanged(); }
    }

    private string? SecondaryTextValue
    {
        get => Condition.SecondaryValue?.ToString();
        set { Condition.SecondaryValue = value; _ = NotifyConditionChanged(); }
    }

    // ── Numeric (decimal?) properties ────────────────────────────────────────

    private decimal? NumericDecimalValue
    {
        get => ToDecimal(Condition.Value);
        set { Condition.Value = value; _ = NotifyConditionChanged(); }
    }

    private decimal? SecondaryNumericDecimalValue
    {
        get => ToDecimal(Condition.SecondaryValue);
        set { Condition.SecondaryValue = value; _ = NotifyConditionChanged(); }
    }

    // ── Currency (decimal?) properties ───────────────────────────────────────

    private decimal? CurrencyDecimalValue
    {
        get => ToDecimal(Condition.Value);
        set { Condition.Value = value; _ = NotifyConditionChanged(); }
    }

    private decimal? SecondaryCurrencyDecimalValue
    {
        get => ToDecimal(Condition.SecondaryValue);
        set { Condition.SecondaryValue = value; _ = NotifyConditionChanged(); }
    }

    private static decimal? ToDecimal(object? v)
    {
        if (v is decimal d) return d;
        if (v is double d2) return (decimal)d2;
        if (v is float f) return (decimal)f;
        if (v is int i) return i;
        if (v is long l) return l;
        if (decimal.TryParse(v?.ToString(), out var p)) return p;
        return null;
    }

    // ── Date properties ───────────────────────────────────────────────────────

    private DateOnly? DateValue
    {
        get => ToDateOnly(Condition.Value);
        set { Condition.Value = value; _ = NotifyConditionChanged(); }
    }

    private DateOnly? SecondaryDateValue
    {
        get => ToDateOnly(Condition.SecondaryValue);
        set { Condition.SecondaryValue = value; _ = NotifyConditionChanged(); }
    }

    private static DateOnly? ToDateOnly(object? v)
    {
        if (v is DateOnly d) return d;
        if (v is DateTime dt) return DateOnly.FromDateTime(dt);
        if (v is string s && DateOnly.TryParse(s, out var p)) return p;
        return null;
    }

    // ── Boolean property ──────────────────────────────────────────────────────

    private bool BoolValue
    {
        get => Condition.Value is bool b && b;
        set { Condition.Value = value; _ = NotifyConditionChanged(); }
    }

    // ── Select (single) property ──────────────────────────────────────────────

    private string? SelectValue
    {
        get => Condition.Value?.ToString();
        set { Condition.Value = value; _ = NotifyConditionChanged(); }
    }

    private async Task HandleSelectChange(ChangeEventArgs e)
    {
        Condition.Value = e.Value?.ToString();
        await NotifyConditionChanged();
    }

    // ── MultiSelect property ──────────────────────────────────────────────────

    private IEnumerable<string>? MultiSelectValues
    {
        get
        {
            if (Condition.Value is IEnumerable<string> list) return list;
            if (Condition.Value is List<string> lst) return lst;
            return Enumerable.Empty<string>();
        }
        set
        {
            Condition.Value = value?.ToList() ?? new List<string>();
            _ = NotifyConditionChanged();
        }
    }

    // ── Notification ──────────────────────────────────────────────────────────

    private async Task NotifyConditionChanged()
    {
        if (ConditionChanged.HasDelegate)
            await ConditionChanged.InvokeAsync(Condition);
    }
}
