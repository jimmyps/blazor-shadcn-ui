namespace NeoUI.Blazor;

/// <summary>
/// Defines the schema for a dynamic form — its fields, sections, title, and layout.
/// </summary>
public sealed class FormSchema
{
    /// <summary>Gets or sets the form title shown at the top.</summary>
    public string? Title { get; set; }

    /// <summary>Gets or sets a description shown below the title.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets form sections. When populated, <see cref="Fields"/> is ignored.
    /// </summary>
    public List<FormSectionDefinition> Sections { get; set; } = [];

    /// <summary>Gets or sets a flat list of fields (no section grouping).</summary>
    public List<FormFieldDefinition> Fields { get; set; } = [];

    /// <summary>Gets or sets the default column count. Overrides <see cref="DynamicForm.Columns"/>.</summary>
    public int? Columns { get; set; }
}

/// <summary>
/// Defines a section within a dynamic form schema that groups related fields.
/// </summary>
public sealed class FormSectionDefinition
{
    /// <summary>Gets or sets the section heading text.</summary>
    public string? Title { get; set; }

    /// <summary>Gets or sets a description shown below the heading.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the fields within this section.</summary>
    public List<FormFieldDefinition> Fields { get; set; } = [];

    /// <summary>Gets or sets an override for the column count within this section.</summary>
    public int? Columns { get; set; }

    /// <summary>Gets or sets whether this section is collapsible.</summary>
    public bool Collapsible { get; set; }

    /// <summary>Gets or sets whether collapsible sections start expanded (default true).</summary>
    public bool DefaultExpanded { get; set; } = true;

    /// <summary>
    /// Gets or sets an expression to control section visibility.
    /// Evaluated against current form values. Example: <c>"Country == 'US'"</c>.
    /// </summary>
    public string? VisibleWhen { get; set; }
}

/// <summary>
/// Defines a single field within a dynamic form schema.
/// </summary>
public sealed class FormFieldDefinition
{
    /// <summary>Gets or sets the field name (used as the key in values dictionary).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the label text.</summary>
    public string? Label { get; set; }

    /// <summary>Gets or sets the helper text shown below the field.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the placeholder text.</summary>
    public string? Placeholder { get; set; }

    /// <summary>Gets or sets the field type determining which component is rendered.</summary>
    public FieldType Type { get; set; } = FieldType.Text;

    /// <summary>Gets or sets whether this field is required.</summary>
    public bool Required { get; set; }

    /// <summary>Gets or sets whether this field is disabled.</summary>
    public bool Disabled { get; set; }

    /// <summary>Gets or sets whether this field is read-only.</summary>
    public bool ReadOnly { get; set; }

    /// <summary>Gets or sets the default value applied when the field is not in the values dictionary.</summary>
    public object? DefaultValue { get; set; }

    /// <summary>Gets or sets the display order (lower values appear first).</summary>
    public int? Order { get; set; }

    /// <summary>Gets or sets the number of grid columns this field spans.</summary>
    public int ColSpan { get; set; } = 1;

    /// <summary>Gets or sets an expression to control field visibility.</summary>
    public string? VisibleWhen { get; set; }

    /// <summary>Gets or sets the options for Select/Combobox/MultiSelect/RadioGroup/CheckboxGroup fields.</summary>
    public List<FormSelectOption>? Options { get; set; }

    /// <summary>Gets or sets the validation rules for this field.</summary>
    public List<FieldValidation>? Validations { get; set; }

    /// <summary>Gets or sets additional type-specific metadata.</summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// A simple label/value option used for Select-style fields in a dynamic form.
/// </summary>
public sealed class FormSelectOption
{
    /// <summary>Gets or sets the display label.</summary>
    public string Label { get; set; } = string.Empty;
    /// <summary>Gets or sets the stored value.</summary>
    public string Value { get; set; } = string.Empty;
    /// <summary>Gets or sets whether this option is disabled.</summary>
    public bool Disabled { get; set; }
}

/// <summary>
/// A validation rule applied to a dynamic form field.
/// </summary>
public sealed class FieldValidation
{
    /// <summary>Gets or sets the validation type.</summary>
    public ValidationType Type { get; set; }

    /// <summary>Gets or sets the error message shown when validation fails.</summary>
    public string? Message { get; set; }

    /// <summary>Gets or sets the numeric/string value associated with the rule (e.g. min length, max value).</summary>
    public object? Value { get; set; }

    /// <summary>Gets or sets a regex pattern string (used when Type is <see cref="ValidationType.Pattern"/>).</summary>
    public string? Pattern { get; set; }

    /// <summary>Gets or sets a custom validator delegate (used when Type is <see cref="ValidationType.Custom"/>).</summary>
    public Func<object?, string?>? Validator { get; set; }
}
