using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>
/// A schema-driven form that renders fields, validates input, and reports changes.
/// </summary>
/// <remarks>
/// <para>
/// DynamicForm accepts a <see cref="FormSchema"/> and a values dictionary,
/// then renders appropriate inputs for each field definition. It handles
/// validation, submit, and field-change callbacks.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;DynamicForm Schema="@_schema" @bind-Values="_values" OnValidSubmit="Save" /&gt;
/// </code>
/// </example>
public partial class DynamicForm : ComponentBase
{
    private readonly Dictionary<string, List<string>> _fieldErrors = new();
    private bool _submitting;
    private bool _submitted;

    // ── Parameters ────────────────────────────────────────────────────

    /// <summary>Gets or sets the form schema.</summary>
    [Parameter]
    public FormSchema? Schema { get; set; }

    /// <summary>Gets or sets the current field values dictionary.</summary>
    [Parameter]
    public Dictionary<string, object?> Values { get; set; } = new();

    /// <summary>Callback invoked when Values changes.</summary>
    [Parameter]
    public EventCallback<Dictionary<string, object?>> ValuesChanged { get; set; }

    /// <summary>Callback invoked when validation passes and the form is submitted.</summary>
    [Parameter]
    public EventCallback<Dictionary<string, object?>> OnValidSubmit { get; set; }

    /// <summary>Callback invoked on submit even when validation fails.</summary>
    [Parameter]
    public EventCallback<Dictionary<string, object?>> OnSubmit { get; set; }

    /// <summary>Callback invoked when a field value changes.</summary>
    [Parameter]
    public EventCallback<FormFieldChangedEventArgs> OnFieldChanged { get; set; }

    /// <summary>Gets or sets the number of columns in the field grid (when schema does not override).</summary>
    [Parameter]
    public int Columns { get; set; } = (int)FormLayout.SingleColumn;

    /// <summary>Gets or sets whether to show the submit button.</summary>
    [Parameter]
    public bool ShowSubmitButton { get; set; } = true;

    /// <summary>Gets or sets the submit button label.</summary>
    [Parameter]
    public string SubmitText { get; set; } = "Submit";

    /// <summary>Gets or sets additional CSS classes for the submit button.</summary>
    [Parameter]
    public string? SubmitButtonClass { get; set; }

    /// <summary>Gets or sets whether to show a validation summary above the form.</summary>
    [Parameter]
    public bool ShowValidationSummary { get; set; } = true;

    /// <summary>Gets or sets whether all inputs are disabled.</summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>Gets or sets custom field renderers keyed by field name or type name.</summary>
    [Parameter]
    public Dictionary<string, Func<FormFieldDefinition, Dictionary<string, object?>, RenderFragment>>? CustomRenderers { get; set; }

    /// <summary>Gets or sets additional CSS classes for the form element.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Gets or sets additional form-level content (appended before submit button).</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Captures any additional HTML attributes.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    // ── Computed ───────────────────────────────────────────────────────

    private bool HasErrors     => _submitted && _fieldErrors.Values.Any(e => e.Count > 0);
    private IEnumerable<string> AllErrors => _fieldErrors.SelectMany(kv => kv.Value);

    private string FormCssClass => ClassNames.cn("space-y-6", Class);

    // ── Submit ─────────────────────────────────────────────────────────

    private async Task HandleSubmit()
    {
        _submitted = true;
        _submitting = true;
        StateHasChanged();

        try
        {
            ValidateAll();
            await OnSubmit.InvokeAsync(Values);
            if (!HasErrors)
                await OnValidSubmit.InvokeAsync(Values);
        }
        finally
        {
            _submitting = false;
            StateHasChanged();
        }
    }

    // ── Field change ───────────────────────────────────────────────────

    internal async Task HandleFieldChange(string fieldName, object? newValue)
    {
        var old = Values.TryGetValue(fieldName, out var prev) ? prev : null;
        Values[fieldName] = newValue;
        await ValuesChanged.InvokeAsync(Values);
        await OnFieldChanged.InvokeAsync(new FormFieldChangedEventArgs(fieldName, newValue, old));
        if (_submitted) ValidateField(fieldName);
        StateHasChanged();
    }

    internal object? GetFieldValue(FormFieldDefinition field)
    {
        if (Values.TryGetValue(field.Name, out var v)) return v;
        return field.DefaultValue;
    }

    internal IReadOnlyList<string> GetFieldErrors(string fieldName) =>
        _fieldErrors.TryGetValue(fieldName, out var e) ? e : [];

    // ── Visibility ─────────────────────────────────────────────────────

    internal bool IsFieldVisible(FormFieldDefinition field)
    {
        if (string.IsNullOrEmpty(field.VisibleWhen)) return true;
        return EvaluateCondition(field.VisibleWhen);
    }

    internal bool IsSectionVisible(FormSectionDefinition section)
    {
        if (string.IsNullOrEmpty(section.VisibleWhen)) return true;
        return EvaluateCondition(section.VisibleWhen);
    }

    // ── Validation ─────────────────────────────────────────────────────

    private void ValidateAll()
    {
        _fieldErrors.Clear();
        var fields = Schema?.Sections.SelectMany(s => s.Fields).Concat(Schema.Fields)
                  ?? Schema?.Fields
                  ?? Enumerable.Empty<FormFieldDefinition>();
        foreach (var f in fields) ValidateField(f);
    }

    private void ValidateField(string fieldName)
    {
        var field = FindField(fieldName);
        if (field is not null) ValidateField(field);
    }

    private void ValidateField(FormFieldDefinition field)
    {
        var errors = new List<string>();
        var value  = GetFieldValue(field);
        var str    = value?.ToString() ?? string.Empty;

        if (field.Required && string.IsNullOrWhiteSpace(str))
            errors.Add(string.IsNullOrEmpty(field.Label) ? $"{field.Name} is required." : $"{field.Label} is required.");

        foreach (var v in field.Validations ?? [])
        {
            var message = v.Message ?? string.Empty;
            switch (v.Type)
            {
                case ValidationType.MinLength when v.Value is int min && str.Length < min:
                    errors.Add(string.IsNullOrEmpty(message) ? $"Minimum {min} characters required." : message); break;
                case ValidationType.MaxLength when v.Value is int max && str.Length > max:
                    errors.Add(string.IsNullOrEmpty(message) ? $"Maximum {max} characters allowed." : message); break;
                case ValidationType.Email when !string.IsNullOrEmpty(str) && !str.Contains('@'):
                    errors.Add(string.IsNullOrEmpty(message) ? "Enter a valid email address." : message); break;
                case ValidationType.Pattern when !string.IsNullOrEmpty(v.Pattern) && !string.IsNullOrEmpty(str):
                    if (!System.Text.RegularExpressions.Regex.IsMatch(str, v.Pattern))
                        errors.Add(string.IsNullOrEmpty(message) ? "Value does not match the required pattern." : message);
                    break;
                case ValidationType.Min when v.Value is double minD && double.TryParse(str, out var dv) && dv < minD:
                    errors.Add(string.IsNullOrEmpty(message) ? $"Minimum value is {minD}." : message); break;
                case ValidationType.Max when v.Value is double maxD && double.TryParse(str, out var dv2) && dv2 > maxD:
                    errors.Add(string.IsNullOrEmpty(message) ? $"Maximum value is {maxD}." : message); break;
                case ValidationType.Custom when v.Validator is not null:
                    var customError = v.Validator(value);
                    if (!string.IsNullOrEmpty(customError)) errors.Add(customError);
                    break;
            }
        }

        _fieldErrors[field.Name] = errors;
    }

    // ── Helpers ────────────────────────────────────────────────────────

    private FormFieldDefinition? FindField(string name)
    {
        if (Schema is null) return null;
        return Schema.Fields.FirstOrDefault(f => f.Name == name)
            ?? Schema.Sections.SelectMany(s => s.Fields).FirstOrDefault(f => f.Name == name);
    }

    private bool EvaluateCondition(string condition)
    {
        // Simple "FieldName == 'Value'" evaluator
        var match = System.Text.RegularExpressions.Regex.Match(condition.Trim(), @"^(\w+)\s*==\s*'([^']*)'$");
        if (!match.Success) return true;
        var fieldName = match.Groups[1].Value;
        var expected  = match.Groups[2].Value;
        return Values.TryGetValue(fieldName, out var v) && v?.ToString() == expected;
    }
}

