using NeoUI.Blazor;
namespace NeoUI.Blazor.Services.Grid;

/// <summary>
/// Service for exporting grid data to various formats.
/// </summary>
public interface IDataGridExportService
{
    /// <summary>
    /// Exports grid data to CSV format.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the grid.</typeparam>
    /// <param name="definition">The grid definition containing column configuration.</param>
    /// <param name="data">The data to export.</param>
    /// <returns>A byte array containing the CSV file content (UTF-8 encoded).</returns>
    Task<byte[]> ExportToCsvAsync<TItem>(DataGridDefinition<TItem> definition, IEnumerable<TItem> data);

    /// <summary>
    /// Exports grid data to Excel format.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the grid.</typeparam>
    /// <param name="definition">The grid definition containing column configuration.</param>
    /// <param name="data">The data to export.</param>
    /// <returns>A byte array containing the Excel file content.</returns>
    Task<byte[]> ExportToExcelAsync<TItem>(DataGridDefinition<TItem> definition, IEnumerable<TItem> data);
}
