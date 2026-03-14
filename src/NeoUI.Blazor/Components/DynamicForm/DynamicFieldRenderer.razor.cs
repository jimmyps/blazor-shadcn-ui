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

    private string FieldId => $"field-{Field.Name}";
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
        Value?.ToString() ?? string.Empty;

    private bool BoolValue =>
        Value is bool b ? b : bool.TryParse(Value?.ToString(), out var bv) && bv;

    private double SliderValue =>
        Value is double d ? d : double.TryParse(Value?.ToString(), out var dv) ? dv : 0;

    private double RatingValue =>
        Value is double d ? d : double.TryParse(Value?.ToString(), out var dv) ? dv : 0;

    private IReadOnlyList<string>? TagsValue =>
        Value is IReadOnlyList<string> tags   ? tags :
        Value is IEnumerable<string>   items  ? [.. items] :
        null;

    private IReadOnlyList<string> CheckboxGroupValues =>
        Value is IReadOnlyList<string> list  ? list :
        Value is IEnumerable<string>   items ? [.. items] :
        [];

    private IEnumerable<string>? MultiSelectValues =>
        Value is IEnumerable<string> items ? items : null;

    private DateOnly? DateValue =>
        Value is DateOnly d  ? d :
        Value is DateTime dt ? DateOnly.FromDateTime(dt) :
        null;

    private DateOnly? StartDateValue =>
        Value is DateRangeValue drv ? drv.Start : null;

    private DateOnly? EndDateValue =>
        Value is DateRangeValue drv ? drv.End : null;

    private TimeOnly? TimeValue =>
        Value is TimeOnly t  ? t :
        Value is DateTime dt ? TimeOnly.FromDateTime(dt) :
        null;

    // DateTime field — date part
    private DateOnly? DateTimeDate =>
        Value is DateTime dt ? DateOnly.FromDateTime(dt) : null;

    // DateTime field — time part
    private TimeOnly? DateTimeTime =>
        Value is DateTime dt ? TimeOnly.FromDateTime(dt) : null;

    private double? NumericDoubleValue =>
        Value is double  d  ? d :
        Value is decimal dc ? (double)dc :
        Value is int     i  ? (double)i :
        double.TryParse(Value?.ToString(), out var dv) ? dv : null;

    private decimal? DecimalValue =>
        Value is decimal dc ? dc :
        Value is double  d  ? (decimal)d :
        Value is int     i  ? (decimal)i :
        decimal.TryParse(Value?.ToString(), out var v) ? v : null;

    private double RangeMinValue =>
        Value is SliderRangeValue r ? r.Min : 0;

    private double RangeMaxValue =>
        Value is SliderRangeValue r ? r.Max : 100;

    private List<IBrowserFile>? FilesValue =>
        Value is List<IBrowserFile> files ? files : null;

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
