using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NeoUI.Blazor.Primitives;
using NeoUI.Blazor.Primitives.Extensions;

var services = new ServiceCollection().AddLogging();
services.AddNeoUIPrimitives();
services.AddSingleton<IJSRuntime, NoInterop>();
await using var provider = services.BuildServiceProvider();
await using var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());
await renderer.Dispatcher.InvokeAsync(async () => {
    var cycle = new CyclicValue();
    RenderFragment child = b => { b.OpenComponent<RadioGroupItemPrimitive<CyclicValue>>(0); b.AddAttribute(1,"Value",cycle); b.CloseComponent(); };
    var radio = await renderer.RenderComponentAsync<RadioGroupPrimitive<CyclicValue>>(ParameterView.FromDictionary(new Dictionary<string,object?> { ["Value"] = cycle, ["ChildContent"] = child }));
    if (!radio.ToHtmlString().Contains("radiogroup")) throw new Exception("Default radio did not render.");
    var host = await renderer.RenderComponentAsync<BrowserStateHost>(ParameterView.FromDictionary(new Dictionary<string,object?> { ["Enabled"] = false, ["Values"] = new[]{"one"}, ["FieldOptions"] = cycle, ["ChildContent"] = (RenderFragment)(b=>b.AddContent(0,"Legacy content")) }));
    foreach (var type in new[]{typeof(CheckboxPrimitive),typeof(SwitchPrimitive)}) {
        var rendered = await renderer.RenderComponentAsync(type, ParameterView.FromDictionary(new Dictionary<string,object?> { ["Name"] = "legacy-field", ["Required"] = true }));
        var html = rendered.ToHtmlString();
        if (!html.Contains("name=\"legacy-field\"") || !html.Contains("required")) throw new Exception("Legacy Name/Required attributes were lost.");
    }
    if (host.ToHtmlString() != "Legacy content") throw new Exception("Disabled host must remain a transparent render wrapper.");
});
Console.WriteLine("Passed: default RadioGroup cyclic TValue and disabled shared host with JSON reflection disabled.");
public sealed class CyclicValue { public CyclicValue Self => this; }
public sealed class NoInterop : IJSRuntime {
    public ValueTask<T> InvokeAsync<T>(string identifier, object?[]? args) => throw new Exception("Static rendering must not invoke JavaScript.");
    public ValueTask<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<T>(identifier,args);
}
