using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Interface for grid rendering implementations.
/// Abstracts the underlying grid technology (AG Grid, native Blazor, etc.).
/// </summary>
public interface IGridRenderer : IAsyncDisposable
{
    /// <summary>
    /// Initializes the grid renderer with the specified element and configuration.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the grid.</typeparam>
    /// <param name="element">The HTML element reference where the grid should be rendered.</param>
    /// <param name="definition">The grid configuration definition.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    Task InitializeAsync<TItem>(ElementReference element, BlazorUI.Components.Grid.GridDefinition<TItem> definition);

    /// <summary>
    /// Updates the grid data with the specified items.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the grid.</typeparam>
    /// <param name="data">The collection of items to display.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateDataAsync<TItem>(IEnumerable<TItem> data);

    /// <summary>
    /// Applies the specified state to the grid (sorting, filtering, pagination, etc.).
    /// </summary>
    /// <param name="state">The grid state to apply.</param>
    /// <returns>A task that represents the asynchronous state update operation.</returns>
    Task UpdateStateAsync(BlazorUI.Components.Grid.GridState state);

    /// <summary>
    /// Retrieves the current state of the grid.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the current grid state.</returns>
    Task<BlazorUI.Components.Grid.GridState> GetStateAsync();
}
