using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddNeoUIPrimitives();
builder.Services.AddNeoUIComponents();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<NeoUI.Test.App>()
    .AddInteractiveServerRenderMode();

app.Run();
