using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using BlazorUI.Components.Services.Grid;
using BlazorUI.Components.Services;

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
        // Register template renderer for Grid cell templates
        // This enables rendering of Blazor components (like Badge) inside AG Grid cells
        services.AddScoped<ITemplateRenderer, TemplateRenderer>();
        
        // Register generic grid renderer as TRANSIENT
        // Each Grid<TItem> component gets its own renderer instance with its own template dictionary
        // This prevents template conflicts when multiple grids are on the same page
        services.AddTransient(typeof(IGridRenderer<>), typeof(AgGridRenderer<>));

        // Register DialogService for programmatic dialogs
        services.AddScoped<DialogService>();
      
        // Register CollapsibleStateService for sidebar collapsible menu state persistence
        services.AddScoped<CollapsibleStateService>();
        
        // Note: IHttpContextAccessor should be registered by the consuming application if SSR cookie 
        // reading is needed. Most Blazor apps that use AddRazorComponents() will have this registered automatically.

        return services;
    }
}
