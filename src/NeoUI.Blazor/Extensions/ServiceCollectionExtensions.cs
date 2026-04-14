using Microsoft.Extensions.DependencyInjection;
using NeoUI.Blazor.Services;
using NeoUI.Blazor.Services.Grid;
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
        => AddNeoUIComponents(services, defaultTheme: null, configureLocalizer: null);

    /// <summary>
    /// Adds NeoUI.Blazor components services with app-wide theme defaults.
    /// These defaults apply only when no user preference is saved in localStorage.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="defaultTheme">A <see cref="ThemePreset"/> used as the app default theme.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNeoUIComponents(
        this IServiceCollection services,
        ThemePreset defaultTheme)
        => AddNeoUIComponents(services, defaultTheme, configureLocalizer: null);

    /// <summary>
    /// Adds NeoUI.Blazor components services to the service collection with optional localizer configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureLocalizer">
    /// An optional action to configure the <see cref="DefaultLocalizer"/> at startup.
    /// Use this to override specific string keys without subclassing.
    /// <example>
    /// <code>
    /// builder.Services.AddNeoUIComponents(localizer =>
    /// {
    ///     localizer.Set("Combobox.Placeholder", "Wählen Sie eine Option...");
    ///     localizer.Set("DataTable.Loading", "Laden...");
    /// });
    /// </code>
    /// </example>
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNeoUIComponents(
        this IServiceCollection services,
        Action<DefaultLocalizer>? configureLocalizer)
        => AddNeoUIComponents(services, defaultTheme: null, configureLocalizer);

    /// <summary>
    /// Adds NeoUI.Blazor components services with full configuration of a default theme preset and localizer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="defaultTheme">Optional <see cref="ThemePreset"/> used as the app default theme.</param>
    /// <param name="configureLocalizer">Optional action to configure the <see cref="DefaultLocalizer"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNeoUIComponents(
        this IServiceCollection services,
        ThemePreset? defaultTheme,
        Action<DefaultLocalizer>? configureLocalizer)
    {
        // Register the default theme preset so ThemeService can seed its initial state
        services.AddSingleton(defaultTheme ?? ThemePreset.Default);
        // Register localizer — consumers may replace this registration with a custom ILocalizer
        // implementation (e.g. a subclass that delegates to IStringLocalizer<T>).
        if (configureLocalizer is not null)
        {
            // Create a new DefaultLocalizer per scope so each circuit/request has its own
            // independent instance. The configureLocalizer action is applied to each instance,
            // ensuring consistent initial state without shared mutable dictionary state.
            services.AddScoped<ILocalizer>(_ =>
            {
                var localizer = new DefaultLocalizer();
                configureLocalizer(localizer);
                return localizer;
            });
        }
        else
        {
            services.AddScoped<ILocalizer, DefaultLocalizer>();
        }

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
