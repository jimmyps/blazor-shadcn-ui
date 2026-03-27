using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace NeoUI.Blazor.Services.Grid;

/// <summary>
/// Default implementation of IDataGridDataGridTemplateRenderer using Blazor's HtmlRenderer.
/// Renders RenderFragment templates to HTML strings for use in JavaScript grid libraries.
/// </summary>
public class DataGridTemplateRenderer : IDataGridDataGridTemplateRenderer
{
    private static readonly Action<ILogger, Exception?> LogTemplateNull =
        LoggerMessage.Define(LogLevel.Warning, new EventId(1, nameof(LogTemplateNull)),
            "[DataGridTemplateRenderer] Template is null");

    private static readonly Action<ILogger, Exception?> LogContextNull =
        LoggerMessage.Define(LogLevel.Warning, new EventId(2, nameof(LogContextNull)),
            "[DataGridTemplateRenderer] Context is null");

    private static readonly Action<ILogger, Exception?> LogHtmlEmpty =
        LoggerMessage.Define(LogLevel.Warning, new EventId(3, nameof(LogHtmlEmpty)),
            "[DataGridTemplateRenderer] WARNING: Rendered HTML is empty!");

    private static readonly Action<ILogger, string, Exception?> LogRenderFailed =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(4, nameof(LogRenderFailed)),
            "[DataGridTemplateRenderer] Error during rendering: {Message}");

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataGridTemplateRenderer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridTemplateRenderer"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving dependencies.</param>
    /// <param name="logger">The logger instance.</param>
    public DataGridTemplateRenderer(IServiceProvider serviceProvider, ILogger<DataGridTemplateRenderer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<string> RenderToStringAsync<TItem>(RenderFragment<TItem> template, TItem context)
    {
        if (template == null)
        {
            LogTemplateNull(_logger, null);
            return string.Empty;
        }

        if (context == null)
        {
            LogContextNull(_logger, null);
            return string.Empty;
        }

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
                if (htmlString.Length == 0)
                {
                    LogHtmlEmpty(_logger, null);
                }

                return htmlString;
            }
            catch (Exception ex)
            {
                LogRenderFailed(_logger, ex.Message, ex);
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
