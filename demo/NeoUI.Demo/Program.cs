using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;
using NeoUI.Demo;
using NeoUI.Demo.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add HttpClient services for server-side components
builder.Services.AddHttpClient();

// Add HttpContextAccessor for SSR scenarios (e.g., reading cookies on server)
builder.Services.AddHttpContextAccessor();

// Add NeoUI.Blazor.Primitives services
builder.Services.AddNeoUIPrimitives();

// Add NeoUI.Blazor components services (includes DataGrid renderer and CollapsibleStateService)
builder.Services.AddNeoUIComponents();

// Add mock data service for generating demo data
builder.Services.AddSingleton<MockDataService>();

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
    .AddAdditionalAssemblies(typeof(NeoUI.Demo.Shared.Routes).Assembly);

app.Run();
