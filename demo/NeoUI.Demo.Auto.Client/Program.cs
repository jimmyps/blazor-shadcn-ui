using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NeoUI.Blazor;
using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;
using NeoUI.Demo.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add NeoUI.Blazor.Primitives services
builder.Services.AddNeoUIPrimitives();

// Add NeoUI.Blazor components services (includes DataGrid renderer and CollapsibleStateService)
builder.Services.AddNeoUIComponents();

// Add mock data service for generating demo data
builder.Services.AddSingleton<MockDataService>();

// Add toast service for notifications
builder.Services.AddSingleton<IToastService, ToastService>();

await builder.Build().RunAsync();
