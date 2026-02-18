using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Services;

/// <summary>
/// Service for persisting collapsible menu state using JavaScript-based localStorage and cookie management.
/// This service delegates all storage operations to a JavaScript module that handles both localStorage
/// and cookie persistence. For SSR scenarios, components should use IHttpContextAccessor to read cookies
/// server-side before falling back to this service's client-side methods.
/// </summary>
public class CollapsibleStateService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private const string StoragePrefix = "blazorui:collapsible:";
    
    /// <summary>
    /// Cookie expiration period in days. Cookies will persist for 365 days.
    /// </summary>
    private const int CookieExpirationDays = 365;

    /// <summary>
    /// Gets the storage key prefix used for collapsible state storage.
    /// </summary>
    public static string KeyPrefix => StoragePrefix;
    
    private IJSObjectReference? _collapsibleModule;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollapsibleStateService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for interop operations.</param>
    public CollapsibleStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Ensures the JavaScript module is loaded.
    /// </summary>
    /// <returns>The loaded JavaScript module reference.</returns>
    private async Task<IJSObjectReference> EnsureModuleAsync()
    {
        if (_collapsibleModule == null)
        {
            _collapsibleModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NeoBlazorUI.Components/js/collapsible-state.js");
        }
        return _collapsibleModule;
    }

    /// <summary>
    /// Get the saved state for a collapsible menu from client-side storage.
    /// This method calls JavaScript to read from localStorage/cookies and is only available during client-side rendering.
    /// </summary>
    /// <param name="key">Unique identifier for the collapsible (e.g., "sidebar-primitives-menu")</param>
    /// <param name="defaultValue">Default value if no state is saved</param>
    /// <returns>True if the menu should be open, false otherwise</returns>
    public async Task<bool> GetStateAsync(string key, bool defaultValue = false)
    {
        try
        {
            var module = await EnsureModuleAsync();
            var savedState = await module.InvokeAsync<bool?>("getCollapsibleState", key);
            return savedState ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Save the state for a collapsible menu.
    /// Writes to both localStorage (for client-side persistence) and cookies (for SSR) via JavaScript.
    /// </summary>
    /// <param name="key">Unique identifier for the collapsible</param>
    /// <param name="isOpen">Whether the menu is open</param>
    public async Task SetStateAsync(string key, bool isOpen)
    {
        try
        {
            var module = await EnsureModuleAsync();
            await module.InvokeVoidAsync("setCollapsibleState", key, isOpen);
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>
    /// Clear the saved state for a collapsible menu.
    /// </summary>
    /// <param name="key">Unique identifier for the collapsible</param>
    public async Task ClearStateAsync(string key)
    {
        try
        {
            var module = await EnsureModuleAsync();
            await module.InvokeVoidAsync("clearCollapsibleState", key);
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>
    /// Clear all saved collapsible states.
    /// </summary>
    public async Task ClearAllStatesAsync()
    {
        try
        {
            var module = await EnsureModuleAsync();
            await module.InvokeVoidAsync("clearAllCollapsibleStates");
        }
        catch
        {
            // Silently fail
        }
    }

    /// <summary>
    /// Disposes the service and its JavaScript module.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_collapsibleModule != null)
        {
            try
            {
                await _collapsibleModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, ignore
            }
        }
        
        GC.SuppressFinalize(this);
    }
}
