using Microsoft.Extensions.DependencyInjection;
using NeoUI.Blazor.Primitives.Services;
namespace NeoUI.Blazor.Primitives.Extensions;

/// <summary>
/// Extension methods for registering NeoUI.Blazor.Primitives services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Configures application interaction defaults; explicit and cascading settings take precedence.</summary>
    public static IServiceCollection ConfigureNeoUIInteractions(this IServiceCollection services, Action<InteractionOptions> configure)
    {
        services.Configure(configure);
        return services;
    }

    /// <summary>
    /// Adds all NeoUI.Blazor.Primitives primitive services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNeoUIPrimitives(this IServiceCollection services)
    {
        services.AddOptions<InteractionOptions>();
        services.AddScoped<CheckboxKeyboardService>();

        // Register PortalService as scoped for user isolation in Blazor Server
        // Each user session gets its own portal registry
        services.AddScoped<IPortalService, PortalService>();

        // Register FocusManager as scoped (component-specific state)
        services.AddScoped<IFocusManager, FocusManager>();

        // Register PositioningService as scoped (component-specific state)
        services.AddScoped<IPositioningService, PositioningService>();

        // Register DropdownManagerService as scoped (ensures only one dropdown open at a time per user session)
        services.AddScoped<DropdownManagerService>();

        // Register KeyboardShortcutService as scoped (per user session for global keyboard shortcuts)
        services.AddScoped<IKeyboardShortcutService, KeyboardShortcutService>();

        return services;
    }
}
