using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;
using NeoUI.BrowserTests.Host;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents(o => o.DetailedErrors = true);
builder.Services.AddNeoUIPrimitives();
if (builder.Configuration.GetValue<bool>("BrowserDefault"))
    builder.Services.ConfigureNeoUIInteractions(o => o.DefaultMode = NeoUI.Blazor.Primitives.InteractionMode.Browser);
if (builder.Configuration["FamilyPolicy"] is "conservative" or "reverse")
{
    var conservative = builder.Configuration["FamilyPolicy"] == "conservative";
    builder.Services.ConfigureNeoUIInteractions(o =>
    {
        o.DefaultMode = conservative ? NeoUI.Blazor.Primitives.InteractionMode.Browser : NeoUI.Blazor.Primitives.InteractionMode.Blazor;
        o.Floating = o.Menus = o.Disclosures = conservative ? NeoUI.Blazor.Primitives.InteractionMode.Browser : NeoUI.Blazor.Primitives.InteractionMode.Blazor;
        o.Inputs = o.Selection = conservative ? NeoUI.Blazor.Primitives.InteractionMode.Blazor : NeoUI.Blazor.Primitives.InteractionMode.Browser;
    });
}
builder.Services.AddNeoUIComponents();
var app = builder.Build();
if (builder.Configuration["TestPathBase"] is { Length: > 0 } pathBase)
{
    app.UsePathBase(pathBase);
    app.Use(async (context, next) =>
    {
        if (context.Request.PathBase != pathBase) { context.Response.StatusCode = 404; return; }
        await next();
    });
}
app.UseRouting();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
