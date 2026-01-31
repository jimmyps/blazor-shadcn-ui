using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorUI.Components.Sidebar;

public partial class SidebarProvider : IDisposable
{
    private SidebarContext Context { get; set; } = new();
    private IJSObjectReference? _module;
    private DotNetObjectReference<SidebarProvider>? _dotNetRef;
    private PersistingComponentStateSubscription _persistingSubscription;
    private IHttpContextAccessor? _httpContextAccessor;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private PersistentComponentState PersistentState { get; set; } = default!;

    protected override void OnInitialized()
    {
        // Try to get IHttpContextAccessor - it's only available in Server/SSR, not in WebAssembly
        _httpContextAccessor = ServiceProvider.GetService<IHttpContextAccessor>();
        
        bool initialOpen = DefaultOpen;
        
        // Try to restore state from persistent component state (client-side after prerender)
        if (PersistentState.TryTakeFromJson<bool>("SidebarOpen", out var persistedOpen))
        {
            initialOpen = persistedOpen;
        }
        // Otherwise try to read cookie server-side during prerendering (only available in Server/SSR)
        else if (_httpContextAccessor?.HttpContext != null && !string.IsNullOrEmpty(CookieKey))
        {
            var cookies = _httpContextAccessor.HttpContext.Request.Cookies;
            
            // URL-encode the cookie key to match how JavaScript sets it (e.g., "sidebar:state" -> "sidebar%3Astate")
            var encodedCookieKey = Uri.EscapeDataString(CookieKey);
            
            // Try both the original and encoded key names
            string? cookieValue = null;
            if (cookies.TryGetValue(encodedCookieKey, out var encodedValue))
            {
                cookieValue = Uri.UnescapeDataString(encodedValue);
            }
            else if (cookies.TryGetValue(CookieKey, out var plainValue))
            {
                cookieValue = plainValue;
            }
            
            if (cookieValue != null && bool.TryParse(cookieValue, out var savedOpen))
            {
                initialOpen = savedOpen;
            }
        }
        
        // Initialize context immediately for SSR - enables expand/collapse to work during prerendering
        Context.Initialize(open: initialOpen, variant: Variant, side: Side, staticRendering: StaticRendering);
        
        // Register a callback to persist the state during prerendering
        _persistingSubscription = PersistentState.RegisterOnPersisting(PersistState);
    }

    private Task PersistState()
    {
        // Persist the current sidebar open state for client-side restoration
        PersistentState.PersistAsJson("SidebarOpen", Context.Open);
        return Task.CompletedTask;
    }

    protected override void OnParametersSet()
    {
        // Update context when parameters change
        Context.SetVariant(Variant);
        Context.SetSide(Side);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Load the sidebar JavaScript module
                _module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/sidebar.js");

                // Create a reference to this component for JS callbacks
                _dotNetRef = DotNetObjectReference.Create(this);

                // Set up mobile detection, keyboard shortcuts, and optional static rendering support
                await _module.InvokeVoidAsync("initializeSidebar", _dotNetRef, CookieKey, StaticRendering);

                // Subscribe to state changes for persistence
                Context.StateChanged += OnStateChanged;
            }
            catch (JSException)
            {
                // JS module not available, continue without JS enhancements
            }
        }
    }

    private async void OnStateChanged(object? sender, EventArgs e)
    {
        // Persist sidebar state to cookie when it changes
        if (_module != null && !string.IsNullOrEmpty(CookieKey))
        {
            try
            {
                await _module.InvokeVoidAsync("saveSidebarState", CookieKey, Context.Open);
            }
            catch (JSException)
            {
                // Ignore persistence errors
            }
        }

        // Notify UI of state change
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Called from JavaScript when mobile state changes.
    /// </summary>
    [JSInvokable]
    public void OnMobileChange(bool isMobile)
    {
        Context.SetIsMobile(isMobile);
    }

    /// <summary>
    /// Called from JavaScript when keyboard shortcut (Ctrl/Cmd + B) is pressed.
    /// </summary>
    [JSInvokable]
    public void OnToggleShortcut()
    {
        Context.ToggleSidebar();
        StateHasChanged();
    }

    /// <summary>
    /// Toggle sidebar - called from JS interop when StaticRendering is enabled.
    /// </summary>
    [JSInvokable]
    public void ToggleSidebar()
    {
        Context.ToggleSidebar();
        StateHasChanged();
    }

    public void Dispose()
    {
        _persistingSubscription.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (Context != null)
        {
            Context.StateChanged -= OnStateChanged;
        }

        if (_module != null)
        {
            try
            {
                await _module.InvokeVoidAsync("cleanup");
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected, ignore
            }
        }

        _dotNetRef?.Dispose();
        _persistingSubscription.Dispose();

        GC.SuppressFinalize(this);
    }
}
