
using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;
using NeoUI.Demo.Auto; using NeoUI.Demo.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddNeoUIPrimitives();
builder.Services.AddNeoUIComponents();
builder.Services.AddSingleton<MockDataService>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts(); }
app.UseHttpsRedirection(); app.UseAntiforgery();
app.UseStaticFiles(); app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode().AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NeoUI.Demo.Shared.Routes).Assembly);
app.Run();
