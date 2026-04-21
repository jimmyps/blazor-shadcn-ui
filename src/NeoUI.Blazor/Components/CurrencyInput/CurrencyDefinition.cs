namespace NeoUI.Blazor;

/// <summary>
/// Defines the properties of a currency.
/// </summary>
public class CurrencyDefinition
{
    /// <summary>
    /// Gets or sets the ISO 4217 currency code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    public required string Symbol { get; init; }

    /// <summary>
    /// Gets or sets the number of decimal places.
    /// </summary>
    public required int DecimalPlaces { get; init; }

    /// <summary>
    /// Gets or sets the culture name for formatting.
    /// </summary>
    public required string CultureName { get; init; }

    /// <summary>
    /// Gets or sets the currency name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets whether the symbol appears before the amount.
    /// </summary>
    public bool SymbolBefore { get; init; } = true;

    /// <summary>
    /// Gets or sets whether a space is placed between the symbol and the amount.
    /// For example, IDR formats as "Rp 100.000" (space), while USD formats as "$1.00" (no space).
    /// </summary>
    public bool SpaceAfterSymbol { get; init; } = false;
}
