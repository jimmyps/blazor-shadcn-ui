namespace NeoUI.Blazor;

/// <summary>
/// Provides localized strings for NeoUI components.
/// </summary>
/// <remarks>
/// <para>
/// Components resolve all UI chrome strings (placeholders, button labels, aria-labels,
/// empty states) through this interface using dot-notation keys
/// (e.g., <c>"Combobox.Placeholder"</c>, <c>"DataTable.Loading"</c>).
/// </para>
/// <para>
/// The library registers <see cref="DefaultLocalizer"/> automatically via
/// <c>AddNeoUIComponents()</c>. To customize strings, either configure the default
/// localizer at startup or replace the registration with a custom implementation.
/// </para>
/// <example>
/// <code>
/// // Pattern A: Configure defaults at startup
/// builder.Services.AddNeoUIComponents(localizer =>
/// {
///     localizer.Set("Combobox.Placeholder", "Wählen Sie eine Option...");
///     localizer.Set("DataTable.Loading", "Laden...");
/// });
///
/// // Pattern B: Full IStringLocalizer integration
/// public class AppLocalizer(IStringLocalizer&lt;SharedResources&gt; loc) : DefaultLocalizer
/// {
///     public override string this[string key] =>
///         loc[key] is { ResourceNotFound: false } found ? found.Value : base[key];
///
///     public override string this[string key, params object[] arguments] =>
///         loc[key, arguments] is { ResourceNotFound: false } found ? found.Value : base[key, arguments];
/// }
///
/// builder.Services.AddNeoUIComponents();
/// builder.Services.AddScoped&lt;ILocalizer, AppLocalizer&gt;();
/// </code>
/// </example>
/// </remarks>
public interface ILocalizer
{
    /// <summary>
    /// Gets the localized string for the specified key.
    /// </summary>
    /// <param name="key">The localization key in dot notation (e.g., <c>"DataTable.Loading"</c>).</param>
    /// <returns>The localized string, or the key itself if not found.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Gets the localized string for the specified key, formatted with the provided arguments.
    /// </summary>
    /// <param name="key">The localization key in dot notation (e.g., <c>"Pagination.ShowingFormat"</c>).</param>
    /// <param name="arguments">The format arguments to substitute into the string.</param>
    /// <returns>The formatted localized string, or the key itself if not found.</returns>
    string this[string key, params object[] arguments] { get; }
}
