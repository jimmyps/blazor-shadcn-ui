using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Services.Grid;

/// <summary>
/// Service for rendering RenderFragment templates to HTML strings.
/// Used by grid renderers to convert Blazor templates into HTML for JavaScript grid libraries.
/// </summary>
public interface ITemplateRenderer
{
    /// <summary>
    /// Renders a RenderFragment to an HTML string.
    /// </summary>
    /// <typeparam name="TItem">The type of the context item.</typeparam>
    /// <param name="template">The template to render.</param>
    /// <param name="context">The context item to pass to the template.</param>
    /// <returns>The rendered HTML string.</returns>
    Task<string> RenderToStringAsync<TItem>(RenderFragment<TItem> template, TItem context);

    /// <summary>
    /// Renders a RenderFragment to an HTML string (no context).
    /// </summary>
    /// <param name="template">The template to render.</param>
    /// <returns>The rendered HTML string.</returns>
    Task<string> RenderToStringAsync(RenderFragment template);
}
