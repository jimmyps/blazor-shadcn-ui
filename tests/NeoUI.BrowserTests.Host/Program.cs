using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;
using NeoUI.BrowserTests.Host;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents(o => o.DetailedErrors = true);
builder.Services.AddNeoUIPrimitives();
builder.Services.AddNeoUIComponents();
var app = builder.Build();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
