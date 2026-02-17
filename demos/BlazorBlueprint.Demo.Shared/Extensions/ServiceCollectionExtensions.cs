using Microsoft.Extensions.DependencyInjection;
using BlazorBlueprint.Demo.Services;
using BlazorBlueprint.Components;

namespace BlazorBlueprint.Demo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorBlueprintDemo(this IServiceCollection services)
    {
        // Add all BlazorBlueprint services (Primitives + Components)
        services.AddBlazorBlueprintComponents();

        // Add theme service for dark mode management
        services.AddScoped<ThemeService>();

        // Add collapsible state service for menu state persistence
        services.AddScoped<CollapsibleStateService>();

        // Add mock data service for generating demo data
        services.AddSingleton<MockDataService>();

        return services;
    }
}
