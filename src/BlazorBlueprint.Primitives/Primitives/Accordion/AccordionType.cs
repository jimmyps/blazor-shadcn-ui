using System.Diagnostics.CodeAnalysis;

namespace BlazorBlueprint.Primitives;

/// <summary>
/// Type of accordion behavior.
/// </summary>
public enum AccordionType
{
    /// <summary>
    /// Only one item can be open at a time.
    /// </summary>
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Single is a domain term for selection mode")]
    Single,

    /// <summary>
    /// Multiple items can be open simultaneously.
    /// </summary>
    Multiple
}
