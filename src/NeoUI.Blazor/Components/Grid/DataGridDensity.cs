using System.Text.Json.Serialization;

namespace NeoUI.Blazor;

/// <summary>
/// Specifies the spacing density for grid rows and cells.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DataGridDensity
{
    /// <summary>Reduced padding for more rows.</summary>
    Compact,

    /// <summary>Standard padding.</summary>
    Medium,

    /// <summary>Increased padding for readability.</summary>
    Spacious,

    /// <summary>Standard padding. Renamed to <see cref="Medium"/> for clarity.</summary>
    [Obsolete("Use DataGridDensity.Medium instead. Comfortable has been renamed to Medium.")]
    Comfortable = Medium
}
