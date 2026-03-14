namespace NeoUI.Blazor;

/// <summary>Defines the type of validation rule applied to a dynamic form field.</summary>
public enum ValidationType
{
    /// <summary>Field must not be empty.</summary>
    Required,
    /// <summary>Value must meet a minimum length.</summary>
    MinLength,
    /// <summary>Value must not exceed a maximum length.</summary>
    MaxLength,
    /// <summary>Value must match a regular expression pattern.</summary>
    Pattern,
    /// <summary>Value must be a valid email address.</summary>
    Email,
    /// <summary>Numeric value must be at or above a minimum.</summary>
    Min,
    /// <summary>Numeric value must be at or below a maximum.</summary>
    Max,
    /// <summary>Custom validation via a delegate.</summary>
    Custom
}
