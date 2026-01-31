using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Services;

/// <summary>
/// Service for persisting collapsible menu state in both localStorage and cookies.
/// Supports server-side rendering by reading from cookies during SSR.
/// </summary>
public class CollapsibleStateService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private const string StoragePrefix = "blazorui:collapsible:";
    
    /// <summary>
    /// Cookie expiration period in days. Cookies will persist for 365 days.
    /// </summary>
    private const int CookieExpirationDays = 365;

    /// <summary>
    /// Gets the storage key prefix used for collapsible state storage.
    /// </summary>
    public static string KeyPrefix => StoragePrefix;
    
    private IJSObjectReference? _storageModule;

    public CollapsibleStateService(IJSRuntime jsRuntime, IHttpContextAccessor? httpContextAccessor = null)
    {
        _jsRuntime = jsRuntime;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Get the saved state for a collapsible menu.
    /// Tries to read from cookies during SSR, falls back to localStorage during client-side rendering.
    /// </summary>
    /// <param name="key">Unique identifier for the collapsible (e.g., "sidebar-primitives-menu")</param>
    /// <param name="defaultValue">Default value if no state is saved</param>
    /// <returns>True if the menu should be open, false otherwise</returns>
    public async Task<bool> GetStateAsync(string key, bool defaultValue = false)
    {
        try
        {
            var storageKey = StoragePrefix + key;
            
            // Try to read from cookie first (available during SSR)
            if (_httpContextAccessor?.HttpContext != null)
            {
                var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[storageKey];
                if (bool.TryParse(cookieValue, out var cookieResult))
                {
                    return cookieResult;
                }
            }
            
            // Fallback to localStorage (client-side only)
            var value = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", storageKey);

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return bool.TryParse(value, out var result) && result;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Save the state for a collapsible menu.
    /// Writes to both localStorage (for client-side persistence) and cookies (for SSR).
    /// </summary>
    /// <param name="key">Unique identifier for the collapsible</param>
    /// <param name="isOpen">Whether the menu is open</param>
    public async Task SetStateAsync(string key, bool isOpen)
    {
        try
        {
            var storageKey = StoragePrefix + key;
            
            // Save to localStorage for client-side persistence
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", storageKey, isOpen.ToString());

            // Also save to cookie for server-side reads
            if (_httpContextAccessor?.HttpContext != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(CookieExpirationDays),
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Secure = true,
                    HttpOnly = false // Must be false to allow JavaScript access via localStorage
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append(
                    storageKey,
                    isOpen.ToString(),
                    cookieOptions
                );
            }
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
            var storageKey = StoragePrefix + key;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", storageKey);
            
            // Also remove cookie if available
            if (_httpContextAccessor?.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(storageKey);
            }
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
            // Load the storage helper module if not already loaded
            if (_storageModule == null)
            {
                _storageModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/storage-helpers.js");
            }

            // Get all localStorage keys that match our prefix using the JS helper
            var allKeys = await _storageModule.InvokeAsync<string[]>("getLocalStorageKeysByPrefix", StoragePrefix);

            // Remove each matching key
            foreach (var key in allKeys)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            }
        }
        catch
        {
            // Silently fail
        }
    }
}
