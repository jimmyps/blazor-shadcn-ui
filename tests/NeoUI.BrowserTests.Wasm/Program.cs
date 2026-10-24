using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;
using NeoUI.BrowserTests.Host;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Cases>("#app");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddNeoUIPrimitives();
builder.Services.ConfigureNeoUIInteractions(o => o.DefaultMode = NeoUI.Blazor.Primitives.InteractionMode.Browser);
builder.Services.AddNeoUIComponents();
await builder.Build().RunAsync();
