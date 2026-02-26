using Microsoft.AspNetCore.Components;
using BlazorUI.Components.Grid;

namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Interface for grid rendering implementations.
/// Abstracts the underlying grid technology (AG Grid, native Blazor, etc.).
/// Non-generic base interface for common operations and DI registration.
/// </summary>
public interface IGridRenderer : IAsyncDisposable
{
    /// <summary>
    /// Applies the specified state to the grid (sorting, filtering, pagination, etc.).
    /// </summary>
    /// <param name="state">The grid state to apply.</param>
    /// <returns>A task that represents the asynchronous state update operation.</returns>
    Task UpdateStateAsync(GridState state);
    
    /// <summary>
    /// Retrieves the current state of the grid.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the current grid state.</returns>
    Task<GridState> GetStateAsync();
}

/// <summary>
/// Generic grid renderer interface providing type-safe operations.
/// Extends the base interface with generic initialization and data operations.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public interface IGridRenderer<TItem> : IGridRenderer
{
    /// <summary>
    /// Initializes the grid renderer with the specified element and configuration.
    /// </summary>
    /// <param name="element">The HTML element reference where the grid should be rendered.</param>
    /// <param name="definition">The grid configuration definition.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    Task InitializeAsync(ElementReference element, GridDefinition<TItem> definition);
    
    /// <summary>
    /// Updates the grid data with the specified items.
    /// </summary>
    /// <param name="data">The collection of items to display.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateDataAsync(IEnumerable<TItem> data);
    
    /// <summary>
    /// Applies a transaction of batched changes (add, remove, update) to the grid.
    /// This is more efficient than UpdateDataAsync for incremental changes.
    /// </summary>
    /// <param name="transaction">The transaction containing the changes to apply.</param>
    /// <returns>A task that represents the asynchronous transaction operation.</returns>
    Task ApplyTransactionAsync(GridTransaction<TItem> transaction);
    
    /// <summary>
    /// Updates the grid theme at runtime without recreating the grid.
    /// Preserves grid state (scroll position, selection, filters).
    /// </summary>
    /// <param name="theme">The new theme to apply.</param>
    /// <param name="themeParams">Optional theme parameters to customize the theme.</param>
    /// <returns>A task that represents the asynchronous theme update operation.</returns>
    Task UpdateThemeAsync(GridTheme theme, Dictionary<string, object>? themeParams);

    /// <summary>
    /// Refreshes the server-side cache, causing AG Grid to re-fetch data.
    /// Only applicable for server-side row models.
    /// </summary>
    /// <returns>A task that represents the asynchronous refresh operation.</returns>
    Task RefreshServerSideCacheAsync();
}
