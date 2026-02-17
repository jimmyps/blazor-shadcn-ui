using Microsoft.Extensions.DependencyInjection;
using BlazorBlueprint.Primitives.Extensions;

namespace BlazorBlueprint.Components;

/// <summary>
/// Extension methods for registering BlazorBlueprint.Components services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all BlazorBlueprint services (Primitives + Components) to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBlazorBlueprintComponents(this IServiceCollection services)
    {
        // Register all primitive services (portal, focus, positioning, dropdown manager, keyboard shortcuts)
        services.AddBlazorBlueprintPrimitives();

        // Register ToastService as scoped for user isolation in Blazor Server
        // Each user session gets its own toast notification state
        services.AddScoped<ToastService>();

        // Register DialogService as scoped for programmatic confirm dialogs
        services.AddScoped<DialogService>();

        return services;
    }
}
