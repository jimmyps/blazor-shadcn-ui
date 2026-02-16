using BlazorUI.Components.Toast;
using BlazorUI.Components.Extensions;
using BlazorUI.Demo.Services;
using BlazorUI.Demo.Shared;
using BlazorUI.Primitives.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

#if WASM_STANDALONE
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
#endif

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add BlazorUI.Primitives services
builder.Services.AddBlazorUIPrimitives();

// Add BlazorUI.Components services (includes Grid renderer and CollapsibleStateService)
builder.Services.AddBlazorUIComponents();

// Add mock data service for generating demo data
builder.Services.AddSingleton<MockDataService>();

// Add toast service for notifications
builder.Services.AddSingleton<IToastService, ToastService>();

await builder.Build().RunAsync();
