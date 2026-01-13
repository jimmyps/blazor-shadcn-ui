using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Encodings.Web;

namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Default implementation of ITemplateRenderer using Blazor's HtmlRenderer.
/// Renders RenderFragment templates to HTML strings for use in JavaScript grid libraries.
/// </summary>
public class TemplateRenderer : ITemplateRenderer
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateRenderer"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving dependencies.</param>
    public TemplateRenderer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public async Task<string> RenderToStringAsync<TItem>(RenderFragment<TItem> template, TItem context)
    {
        if (template == null)
        {
            Console.WriteLine("[TemplateRenderer] Template is null");
            return string.Empty;
        }

        if (context == null)
        {
            Console.WriteLine("[TemplateRenderer] Context is null");
            return string.Empty;
        }

        Console.WriteLine($"[TemplateRenderer] Rendering template for context type: {context.GetType().Name}");

        await using var htmlRenderer = new HtmlRenderer(_serviceProvider, NullLoggerFactory.Instance);

        var parameters = ParameterView.Empty;
        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                var output = await htmlRenderer.RenderComponentAsync<TemplateWrapper<TItem>>(
                    ParameterView.FromDictionary(new Dictionary<string, object?>
                    {
                        [nameof(TemplateWrapper<TItem>.Template)] = template,
                        [nameof(TemplateWrapper<TItem>.Context)] = context
                    }));

                var htmlString = output.ToHtmlString();
                Console.WriteLine($"[TemplateRenderer] Rendered HTML length: {htmlString.Length}");
                if (htmlString.Length > 0 && htmlString.Length < 500)
                {
                    Console.WriteLine($"[TemplateRenderer] Rendered HTML: {htmlString}");
                }
                else if (htmlString.Length >= 500)
                {
                    Console.WriteLine($"[TemplateRenderer] Rendered HTML (truncated): {htmlString.Substring(0, 500)}...");
                }
                else
                {
                    Console.WriteLine("[TemplateRenderer] WARNING: Rendered HTML is empty!");
                }
                
                return htmlString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TemplateRenderer] Error during rendering: {ex.Message}");
                Console.WriteLine($"[TemplateRenderer] Stack trace: {ex.StackTrace}");
                throw;
            }
        });

        return html;
    }

    /// <inheritdoc/>
    public async Task<string> RenderToStringAsync(RenderFragment template)
    {
        if (template == null)
        {
            return string.Empty;
        }

        await using var htmlRenderer = new HtmlRenderer(_serviceProvider, NullLoggerFactory.Instance);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync<SimpleTemplateWrapper>(
                ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    [nameof(SimpleTemplateWrapper.Template)] = template
                }));

            return output.ToHtmlString();
        });

        return html;
    }

    /// <summary>
    /// Wrapper component that renders a template with context.
    /// Internal helper for rendering templates to HTML.
    /// </summary>
    private class TemplateWrapper<TItem> : ComponentBase
    {
        [Parameter]
        public RenderFragment<TItem>? Template { get; set; }

        [Parameter]
        public TItem? Context { get; set; }

        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            if (Template != null && Context != null)
            {
                builder.AddContent(0, Template(Context));
            }
        }
    }

    /// <summary>
    /// Wrapper component that renders a template without context.
    /// Internal helper for rendering templates to HTML.
    /// </summary>
    private class SimpleTemplateWrapper : ComponentBase
    {
        [Parameter]
        public RenderFragment? Template { get; set; }

        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            if (Template != null)
            {
                builder.AddContent(0, Template);
            }
        }
    }

    /// <summary>
    /// Null logger factory for HtmlRenderer (no logging needed for template rendering).
    /// </summary>
    private class NullLoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
    {
        public static readonly NullLoggerFactory Instance = new();

        public void AddProvider(Microsoft.Extensions.Logging.ILoggerProvider provider) { }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) =>
            Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

        public void Dispose() { }
    }
}
