using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Factory for creating grid renderer instances based on configuration.
/// </summary>
public class GridRendererFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GridRendererFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    public GridRendererFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a grid renderer instance of the specified type.
    /// </summary>
    /// <typeparam name="TRenderer">The type of renderer to create.</typeparam>
    /// <returns>An instance of the requested renderer.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the renderer type cannot be created.</exception>
    public TRenderer CreateRenderer<TRenderer>() where TRenderer : IGridRenderer
    {
        var renderer = _serviceProvider.GetService<TRenderer>();
        if (renderer == null)
        {
            throw new InvalidOperationException(
                $"Unable to create renderer of type {typeof(TRenderer).Name}. " +
                $"Ensure the renderer is registered in the DI container.");
        }
        return renderer;
    }

    /// <summary>
    /// Creates the default grid renderer (AgGridRenderer).
    /// </summary>
    /// <returns>An instance of the default grid renderer.</returns>
    public IGridRenderer CreateDefaultRenderer()
    {
        var jsRuntime = _serviceProvider.GetRequiredService<IJSRuntime>();
        return new AgGridRenderer(jsRuntime);
    }
}
