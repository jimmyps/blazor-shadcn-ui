using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;
using NeoUI.BrowserTests.Host;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents(o => o.DetailedErrors = true);
builder.Services.AddNeoUIPrimitives();
if (builder.Configuration.GetValue<bool>("BrowserDefault"))
    builder.Services.ConfigureNeoUIInteractions(o => o.DefaultMode = NeoUI.Blazor.Primitives.InteractionMode.Browser);
builder.Services.AddNeoUIComponents();
var app = builder.Build();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
