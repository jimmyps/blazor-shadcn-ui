using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace NeoUI.Blazor;

/// <summary>
/// Internal component used by <see cref="DynamicForm"/> to render a single field
/// based on its <see cref="FieldType"/>.
/// </summary>
public partial class DynamicFieldRenderer : ComponentBase
{
    /// <summary>Gets or sets the field definition.</summary>
    [Parameter, EditorRequired]
    public FormFieldDefinition Field { get; set; } = null!;

    /// <summary>Gets or sets the current value of the field.</summary>
    [Parameter]
    public object? Value { get; set; }

    /// <summary>Reads the canonical field value before a cascading render arrives.</summary>
    [Parameter] public Func<object?>? AcceptedValueAccessor { get; set; }
    private object? CurrentFieldValue => AcceptedValueAccessor != null ? AcceptedValueAccessor() : Value;
    private string? AcceptedStringValue() => (AcceptedValueAccessor != null ? AcceptedValueAccessor() : Value)?.ToString();

    /// <summary>Gets or sets the validation errors for this field.</summary>
    [Parameter]
    public IReadOnlyList<string> Errors { get; set; } = [];

    /// <summary>Gets or sets whether this field is disabled.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Callback invoked when the field value changes.</summary>
    [Parameter]
    public EventCallback<object?> OnChange { get; set; }

    // ── Identity & state ─────────────────────────────────────────────

    // Per-instance unique prefix: prevents duplicate element IDs when the same
    // field name (e.g. "email") appears in multiple DynamicForm instances on one page.
    private readonly string _uid = Guid.NewGuid().ToString("N")[..8];

    private string FieldId => $"field-{_uid}-{Field.Name}";
    private bool   HasErrors => Errors.Count > 0;

    /// <summary>
    /// True for field types where the label renders inline next to the control
    /// rather than above it (checkbox, switch).
    /// </summary>
    private bool IsInlineLabel => Field.Type is FieldType.Checkbox or FieldType.Switch;

    /// <summary>
    /// True for field types whose input component has a built-in ShowValidationError parameter.
    /// For these, the error is surfaced by the component itself and no external FieldError is needed.
    /// </summary>
    private bool HasBuiltinValidation => Field.Type is
        FieldType.Text    or FieldType.Email    or FieldType.Url      or FieldType.Phone    or
        FieldType.Password or FieldType.Number  or FieldType.Currency or FieldType.Masked   or
        FieldType.Textarea or FieldType.Color   or FieldType.File     or FieldType.Time     or
        FieldType.DateTime or FieldType.Rating;

    // ── Typed value helpers ──────────────────────────────────────────

    private string StringValue =>
        CurrentFieldValue?.ToString() ?? string.Empty;

    private bool BoolValue =>
        CurrentFieldValue is bool b ? b : bool.TryParse(CurrentFieldValue?.ToString(), out var bv) && bv;

    private double SliderValue =>
        CurrentFieldValue is double d ? d : double.TryParse(CurrentFieldValue?.ToString(), out var dv) ? dv : 0;

    private double RatingValue =>
        CurrentFieldValue is double d ? d : double.TryParse(CurrentFieldValue?.ToString(), out var dv) ? dv : 0;

    private IReadOnlyList<string>? TagsValue =>
        CurrentFieldValue is IReadOnlyList<string> tags   ? tags :
        CurrentFieldValue is IEnumerable<string>   items  ? [.. items] :
        null;

    private IReadOnlyList<string> CheckboxGroupValues =>
        CurrentFieldValue is IReadOnlyList<string> list  ? list :
        CurrentFieldValue is IEnumerable<string>   items ? [.. items] :
        [];

    private IEnumerable<string>? MultiSelectValues =>
        CurrentFieldValue is IEnumerable<string> items ? items : null;

    private DateOnly? DateValue =>
        CurrentFieldValue is DateOnly d  ? d :
        CurrentFieldValue is DateTime dt ? DateOnly.FromDateTime(dt) :
        null;

    private DateOnly? StartDateValue =>
        CurrentFieldValue is DateRangeValue drv ? drv.Start : null;

    private DateOnly? EndDateValue =>
        CurrentFieldValue is DateRangeValue drv ? drv.End : null;

    private TimeOnly? TimeValue =>
        CurrentFieldValue is TimeOnly t  ? t :
        CurrentFieldValue is DateTime dt ? TimeOnly.FromDateTime(dt) :
        null;

    // DateTime field — date part
    private DateOnly? DateTimeDate =>
        CurrentFieldValue is DateTime dt ? DateOnly.FromDateTime(dt) : null;

    // DateTime field — time part
    private TimeOnly? DateTimeTime =>
        CurrentFieldValue is DateTime dt ? TimeOnly.FromDateTime(dt) : null;

    private double? NumericDoubleValue =>
        CurrentFieldValue is double  d  ? d :
        CurrentFieldValue is decimal dc ? (double)dc :
        CurrentFieldValue is int     i  ? (double)i :
        double.TryParse(CurrentFieldValue?.ToString(), out var dv) ? dv : null;

    private decimal? DecimalValue =>
        CurrentFieldValue is decimal dc ? dc :
        CurrentFieldValue is double  d  ? (decimal)d :
        CurrentFieldValue is int     i  ? (decimal)i :
        decimal.TryParse(CurrentFieldValue?.ToString(), out var v) ? v : null;

    private double RangeMinValue =>
        CurrentFieldValue is SliderRangeValue r ? r.Min : 0;

    private double RangeMaxValue =>
        CurrentFieldValue is SliderRangeValue r ? r.Max : 100;

    private List<IBrowserFile>? FilesValue =>
        CurrentFieldValue is List<IBrowserFile> files ? files : null;

    private InputType HtmlInputType => Field.Type switch
    {
        FieldType.Email => InputType.Email,
        FieldType.Url   => InputType.Url,
        FieldType.Phone => InputType.Tel,
        _               => InputType.Text
    };

    // ── Change handlers ──────────────────────────────────────────────

    private Task HandleStringChange(string? v)              => OnChange.InvokeAsync(v);
    private Task HandleBoolChange(bool v)                   => OnChange.InvokeAsync((object?)v);
    private Task HandleSliderChange(double v)               => OnChange.InvokeAsync((object?)v);
    private Task HandleTagsChange(IReadOnlyList<string>? v) => OnChange.InvokeAsync((object?)v);
    private Task HandleNumericChange(double? v)             => OnChange.InvokeAsync((object?)v);
    private Task HandleDecimalChange(decimal? v)            => OnChange.InvokeAsync((object?)v);
    private Task HandleDateChange(DateOnly? d)              => OnChange.InvokeAsync((object?)d);
    private Task HandleTimeChange(TimeOnly? t)              => OnChange.InvokeAsync((object?)t);
    private Task HandleFilesChange(List<IBrowserFile> f)    => OnChange.InvokeAsync((object?)f);

    private Task HandleStartDateChange(DateOnly? d) =>
        OnChange.InvokeAsync(new DateRangeValue(d, EndDateValue));

    private Task HandleEndDateChange(DateOnly? d) =>
        OnChange.InvokeAsync(new DateRangeValue(StartDateValue, d));

    private Task HandleDateTimePartChange(DateOnly? d)
    {
        var time   = DateTimeTime ?? TimeOnly.MinValue;
        var result = d is null ? (DateTime?)null : d.Value.ToDateTime(time);
        return OnChange.InvokeAsync((object?)result);
    }

    private Task HandleTimePartChange(TimeOnly? t)
    {
        var date   = DateTimeDate ?? DateOnly.FromDateTime(DateTime.Today);
        var result = t is null ? (DateTime?)null : date.ToDateTime(t.Value);
        return OnChange.InvokeAsync((object?)result);
    }

    private Task HandleRangeMinChange(double v) =>
        OnChange.InvokeAsync(new SliderRangeValue(v, RangeMaxValue));

    private Task HandleRangeMaxChange(double v) =>
        OnChange.InvokeAsync(new SliderRangeValue(RangeMinValue, v));

    private Task HandleMultiSelectChange(IEnumerable<string>? v) =>
        OnChange.InvokeAsync((object?)v?.ToList());

    private Task HandleCheckboxGroupChange(string optionValue, bool isChecked)
    {
        var current = CheckboxGroupValues.ToList();
        if (isChecked) { if (!current.Contains(optionValue)) current.Add(optionValue); }
        else           { current.Remove(optionValue); }
        return OnChange.InvokeAsync((object?)current.AsReadOnly());
    }
}
