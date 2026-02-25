using NeoUI.Blazor.Services;
using NeoUI.Blazor.Services.Grid;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using NeoUI.Blazor.Primitives;
using NeoUI.Blazor.Primitives.Services;
namespace NeoUI.Blazor.Extensions;

/// <summary>
/// Extension methods for registering NeoUI.Blazor components services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NeoUI.Blazor components services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNeoUIComponents(this IServiceCollection services)
    {
        // Register template renderer for DataGrid cell templates
        // This enables rendering of Blazor components (like Badge) inside AG DataGrid cells
        services.AddScoped<IDataGridDataGridTemplateRenderer, DataGridTemplateRenderer>();
        
        // Register generic grid renderer as TRANSIENT
        // Each DataGrid<TItem> component gets its own renderer instance with its own template dictionary
        // This prevents template conflicts when multiple grids are on the same page
        services.AddTransient(typeof(IDataGridRenderer<>), typeof(AgDataGridRenderer<>));

        // Register DialogService for programmatic dialogs
        services.AddScoped<DialogService>();
      
        // Register CollapsibleStateService for sidebar collapsible menu state persistence
        services.AddScoped<CollapsibleStateService>();
        
        // Register ThemeService for theme management
        services.AddScoped<ThemeService>();

        // Register ToastService
        services.AddSingleton<IToastService, ToastService>();

        return services;
    }
}
