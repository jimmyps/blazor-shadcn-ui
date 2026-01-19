using BlazorUI.Demo;
using BlazorUI.Demo.Services;
using BlazorUI.Primitives.Extensions;
using BlazorUI.Components.Toast;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add HttpClient services for server-side components
builder.Services.AddHttpClient();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

// Serve static files (required for WebAssembly framework files)
app.UseStaticFiles();

// Map static assets (for optimized asset delivery)
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorUI.Demo.Shared.Routes).Assembly);

app.Run();
