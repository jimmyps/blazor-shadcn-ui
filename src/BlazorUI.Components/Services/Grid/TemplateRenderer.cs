using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Encodings.Web;

namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Service for rendering Blazor RenderFragment templates to HTML strings.
/// Used for AG Grid cell template rendering.
/// </summary>
public class TemplateRenderer
{
    private readonly HtmlRenderer _htmlRenderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateRenderer"/> class.
    /// </summary>
    /// <param name="htmlRenderer">The HTML renderer service.</param>
    public TemplateRenderer(HtmlRenderer htmlRenderer)
    {
        _htmlRenderer = htmlRenderer;
    }

    /// <summary>
    /// Renders a RenderFragment to an HTML string.
    /// </summary>
    /// <typeparam name="TItem">The type of the template context.</typeparam>
    /// <param name="template">The template to render.</param>
    /// <param name="context">The context object for the template.</param>
    /// <returns>The rendered HTML string.</returns>
    public async Task<string> RenderTemplateAsync<TItem>(RenderFragment<TItem>? template, TItem context)
    {
        if (template == null)
        {
            return string.Empty;
        }

        try
        {
            var htmlContent = await _htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await _htmlRenderer.RenderComponentAsync<TemplateHost<TItem>>(
                    ParameterView.FromDictionary(new Dictionary<string, object?>
                    {
                        { nameof(TemplateHost<TItem>.Template), template },
                        { nameof(TemplateHost<TItem>.Context), context }
                    }));

                return output.ToHtmlString();
            });

            return htmlContent;
        }
        catch (Exception ex)
        {
            // Log error and return empty string to avoid breaking the grid
            Console.Error.WriteLine($"Error rendering template: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Renders a non-generic RenderFragment to an HTML string.
    /// </summary>
    /// <param name="template">The template to render.</param>
    /// <returns>The rendered HTML string.</returns>
    public async Task<string> RenderTemplateAsync(RenderFragment? template)
    {
        if (template == null)
        {
            return string.Empty;
        }

        try
        {
            var htmlContent = await _htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await _htmlRenderer.RenderComponentAsync<SimpleTemplateHost>(
                    ParameterView.FromDictionary(new Dictionary<string, object?>
                    {
                        { nameof(SimpleTemplateHost.Template), template }
                    }));

                return output.ToHtmlString();
            });

            return htmlContent;
        }
        catch (Exception ex)
        {
            // Log error and return empty string to avoid breaking the grid
            Console.Error.WriteLine($"Error rendering template: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Helper component to host a RenderFragment with context.
    /// </summary>
    private class TemplateHost<TItem> : ComponentBase
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
    /// Helper component to host a simple RenderFragment.
    /// </summary>
    private class SimpleTemplateHost : ComponentBase
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
}
