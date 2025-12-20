using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorUI.Primitives.Extensions;
using BlazorUI.Demo.Services;
using BlazorUI.Components.Toast;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<BlazorUI.Demo.Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add BlazorUI.Primitives services
builder.Services.AddBlazorUIPrimitives();

// Add theme service for dark mode management (scoped because it depends on IJSRuntime)
builder.Services.AddScoped<ThemeService>();

// Add collapsible state service for menu state persistence
builder.Services.AddScoped<CollapsibleStateService>();

// Add mock data service for generating demo data
builder.Services.AddSingleton<MockDataService>();

// Add toast service for notifications
builder.Services.AddSingleton<IToastService, ToastService>();

// Add keyboard shortcut service for global shortcuts
builder.Services.AddScoped<KeyboardShortcutService>();

await builder.Build().RunAsync();
