using System.Net.Http.Json;

namespace NeoUI.Blazor.Services.Grid;

/// <summary>
/// Base class for HTTP-based DataGrid server-side data providers.
/// Serialises DataGridDataRequest to a JSON POST body and deserialises DataGridDataResponse.
/// Override ConfigureRequestAsync to inject auth headers, tenant IDs, query params, etc.
/// Override MapResponseAsync to handle non-standard response shapes.
/// </summary>
/// <typeparam name="TItem">The type of items in the grid.</typeparam>
public abstract class HttpDataGridProvider<TItem> : IDataGridServerDataProvider<TItem>
{
    /// <summary>Gets the HttpClient used to send requests.</summary>
    protected readonly HttpClient HttpClient;

    /// <summary>Gets the URL of the endpoint to POST requests to.</summary>
    protected readonly string EndpointUrl;

    /// <summary>
    /// Initializes a new instance of <see cref="HttpDataGridProvider{TItem}"/>.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for sending requests.</param>
    /// <param name="endpointUrl">The URL of the endpoint to POST requests to.</param>
    protected HttpDataGridProvider(HttpClient httpClient, string endpointUrl)
    {
        HttpClient = httpClient;
        EndpointUrl = endpointUrl;
    }

    /// <inheritdoc/>
    public async Task<DataGridDataResponse<TItem>> GetDataAsync(
        DataGridDataRequest<TItem> request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, EndpointUrl)
        {
            Content = JsonContent.Create(request)
        };

        await ConfigureRequestAsync(httpRequest, request, cancellationToken);

        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await MapResponseAsync(response, cancellationToken);
    }

    /// <summary>
    /// Override to customise the HTTP request before it is sent.
    /// Add authentication headers, tenant IDs, custom query parameters, etc.
    /// </summary>
    /// <param name="request">The HTTP request message to customise.</param>
    /// <param name="dataRequest">The DataGrid data request for context.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    protected virtual Task ConfigureRequestAsync(
        HttpRequestMessage request,
        DataGridDataRequest<TItem> dataRequest,
        CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Override to map a non-standard HTTP response body to DataGridDataResponse.
    /// Default implementation expects the response to deserialise directly to DataGridDataResponse{TItem}.
    /// </summary>
    /// <param name="response">The HTTP response message to read.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    protected virtual async Task<DataGridDataResponse<TItem>> MapResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var result = await response.Content.ReadFromJsonAsync<DataGridDataResponse<TItem>>(
            cancellationToken: cancellationToken);
        return result ?? new DataGridDataResponse<TItem>();
    }
}
