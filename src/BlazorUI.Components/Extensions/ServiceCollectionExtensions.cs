using Microsoft.Extensions.DependencyInjection;
using BlazorUI.Components.Services.Grid;

namespace BlazorUI.Components.Extensions;

/// <summary>
/// Extension methods for registering BlazorUI.Components services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds BlazorUI.Components services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBlazorUIComponents(this IServiceCollection services)
    {
        // Register generic grid renderer (AG Grid implementation)
        // Use open generic registration so DI can resolve IGridRenderer<TItem> for any TItem
        services.AddScoped(typeof(IGridRenderer<>), typeof(AgGridRenderer<>));
        
        // Note: ITemplateRenderer is optional and can be registered by the application
        // if cell template rendering is needed

        return services;
    }
}
