using BlazorUI.Components.Toast;
using BlazorUI.Components.Extensions;
using BlazorUI.Primitives.Extensions;
using NeoUI.Web.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#if WASM_STANDALONE
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
#endif

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorUIPrimitives();
builder.Services.AddBlazorUIComponents();

builder.Services.AddSingleton<IToastService, ToastService>();

await builder.Build().RunAsync();
