namespace BlazorUI.Components.ColorPicker;

/// <summary>
/// Specifies the color format for the ColorPicker component.
/// </summary>
public enum ColorFormat
{
    /// <summary>
    /// Hexadecimal format (e.g., #FF5733 or #FF5733AA with alpha).
    /// </summary>
    Hex,

    /// <summary>
    /// RGB format (e.g., rgb(255, 87, 51) or rgba(255, 87, 51, 0.67) with alpha).
    /// </summary>
    RGB,

    /// <summary>
    /// HSL format (e.g., hsl(9, 100%, 60%) or hsla(9, 100%, 60%, 0.67) with alpha).
    /// </summary>
    HSL
}
