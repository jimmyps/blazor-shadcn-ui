using BlazorUI.Primitives.Extensions;
using BlazorUI.Components.Extensions;
using BlazorUI.Components.Toast;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add NeoBlazorUI services
builder.Services.AddBlazorUIPrimitives();
builder.Services.AddBlazorUIComponents();

builder.Services.AddSingleton<IToastService, ToastService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();

app.MapRazorComponents<NeoUI.Web.App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NeoUI.Web.Shared.Routes).Assembly);

app.Run();
