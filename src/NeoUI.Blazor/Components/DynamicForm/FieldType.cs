namespace NeoUI.Blazor;

/// <summary>
/// Defines the type of a dynamic form field, determining which input component is rendered.
/// </summary>
public enum FieldType
{
    /// <summary>Single-line text input.</summary>
    Text,
    /// <summary>Email address input.</summary>
    Email,
    /// <summary>Password input with obscured characters.</summary>
    Password,
    /// <summary>URL input.</summary>
    Url,
    /// <summary>Phone number input.</summary>
    Phone,
    /// <summary>Numeric input with increment/decrement controls.</summary>
    Number,
    /// <summary>Currency input with symbol and formatting.</summary>
    Currency,
    /// <summary>Masked input with pattern-based formatting (phone, SSN, etc.).</summary>
    Masked,
    /// <summary>Multi-line text input.</summary>
    Textarea,
    /// <summary>Rich text editor with formatting toolbar.</summary>
    RichText,
    /// <summary>Styled dropdown select.</summary>
    Select,
    /// <summary>Searchable combobox with filtering.</summary>
    Combobox,
    /// <summary>Multi-value select dropdown.</summary>
    MultiSelect,
    /// <summary>Native HTML select element.</summary>
    NativeSelect,
    /// <summary>Single checkbox (boolean).</summary>
    Checkbox,
    /// <summary>Toggle switch (boolean).</summary>
    Switch,
    /// <summary>Group of checkboxes for multi-value selection.</summary>
    CheckboxGroup,
    /// <summary>Radio button group for single-value selection.</summary>
    RadioGroup,
    /// <summary>Date picker.</summary>
    Date,
    /// <summary>Date range picker with start and end dates.</summary>
    DateRange,
    /// <summary>Time picker.</summary>
    Time,
    /// <summary>Date and time combined picker.</summary>
    DateTime,
    /// <summary>Color picker.</summary>
    Color,
    /// <summary>File upload.</summary>
    File,
    /// <summary>One-time password (OTP) input.</summary>
    OTP,
    /// <summary>Single-value slider.</summary>
    Slider,
    /// <summary>Range slider with two thumbs.</summary>
    RangeSlider,
    /// <summary>Tag/chip input for managing a list of strings.</summary>
    Tags,
    /// <summary>Star/numeric rating input.</summary>
    Rating,
    /// <summary>Custom field that delegates rendering to a registered factory.</summary>
    Custom
}
