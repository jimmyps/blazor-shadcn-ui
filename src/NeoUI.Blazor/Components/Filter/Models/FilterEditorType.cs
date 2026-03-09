namespace NeoUI.Blazor.Filter;

/// <summary>
/// Specifies the input widget used for the filter value editor.
/// When <see cref="Auto"/>, the editor is inferred from the field's <see cref="FilterFieldType"/>.
/// </summary>
public enum FilterEditorType
{
    /// <summary>Automatically inferred from the field's <see cref="FilterFieldType"/>.</summary>
    Auto,

    /// <summary>Standard text <see cref="NeoUI.Blazor.Input"/> (default for Text fields).</summary>
    Input,

    /// <summary><see cref="NeoUI.Blazor.NumericInput{TValue}"/> for decimal/integer values.</summary>
    Numeric,

    /// <summary><see cref="NeoUI.Blazor.CurrencyInput{TValue}"/> for formatted monetary values.</summary>
    Currency,

    /// <summary><see cref="NeoUI.Blazor.MaskedInput"/> for pattern-masked text inputs.</summary>
    Masked,

    /// <summary><see cref="NeoUI.Blazor.DatePicker"/> for a single date value.</summary>
    Date,

    /// <summary><see cref="NeoUI.Blazor.DateRangePicker"/> for start/end date pairs.</summary>
    DateRange,

    /// <summary>Simple True/False toggle for boolean fields.</summary>
    Boolean,

    /// <summary>Single-selection <c>&lt;select&gt;</c> element. Options come from <see cref="FilterFieldDefinition.Options"/>.</summary>
    Select,

    /// <summary><see cref="NeoUI.Blazor.MultiSelect{TItem}"/> for multiple-value selection. Options come from <see cref="FilterFieldDefinition.Options"/>.</summary>
    MultiSelect,

    /// <summary>Custom <see cref="Microsoft.AspNetCore.Components.RenderFragment{FilterCondition}"/> provided via <c>ChildContent</c> on <see cref="FilterField"/>.</summary>
    Custom
}
