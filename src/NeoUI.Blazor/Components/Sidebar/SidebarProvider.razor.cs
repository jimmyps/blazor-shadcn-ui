using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;

namespace NeoUI.Blazor;

/// <summary>
/// Provides context and state management for sidebar components with responsive behavior and persistence.
/// </summary>
public partial class SidebarProvider : ComponentBase
{
    [Inject] private Microsoft.Extensions.Options.IOptions<NeoUI.Blazor.Primitives.InteractionOptions> InteractionDefaults { get; set; } = default!;
    [CascadingParameter] private NeoUI.Blazor.Primitives.InteractionPolicy? InteractionPolicy { get; set; }
    private SidebarContext Context { get; set; } = new();
    private IJSObjectReference? _module, _browserController;
    private readonly SemaphoreSlim _lifecycleGate = new(1, 1);
    private bool _disposed, _browserSetup, _legacySetup;
    private DotNetObjectReference<SidebarProvider>? _dotNetRef;
    private PersistingComponentStateSubscription _persistingSubscription;
    private IHttpContextAccessor? _httpContextAccessor;

    /// <summary>
    /// Gets or sets the JavaScript runtime for interop operations.
    /// </summary>
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets or sets the service provider for dependency resolution.
    /// </summary>
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    /// <summary>
    /// Gets or sets the persistent component state for preserving sidebar state across prerendering.
    /// </summary>
    [Inject]
    private PersistentComponentState PersistentState { get; set; } = default!;

    /// <inheritdoc/>
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
        Context.Initialize(open: initialOpen, variant: Variant, side: Side, staticRendering: StaticRendering, collapsedMode: CollapsedMode);
        
        // Register a callback to persist the state during prerendering
        _persistingSubscription = PersistentState.RegisterOnPersisting(PersistState);
    }

    /// <summary>
    /// Persists the current sidebar state for restoration after prerendering.
    /// </summary>
    private Task PersistState()
    {
        // Persist the current sidebar open state for client-side restoration
        PersistentState.PersistAsJson("SidebarOpen", Context.Open);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        Context.InteractionMode = InteractionDefaults.Value.Resolve(InteractionMode, InteractionPolicy, NeoUI.Blazor.Primitives.InteractionFamily.Menus);
        Context.InteractionRevision = InteractionRevision;
        // Update context when parameters change
        Context.SetVariant(Variant);
        Context.SetSide(Side);
        Context.SetCollapsedMode(CollapsedMode);
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await _lifecycleGate.WaitAsync();
        try
        {
            if (_disposed) return;
            _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/NeoUI.Blazor/js/sidebar.js");
            if (_disposed) return;
            _dotNetRef = DotNetObjectReference.Create(this);
            _browserSetup = Context.InteractionMode == NeoUI.Blazor.Primitives.InteractionMode.Browser;
            if (_browserSetup)
                _browserController = await _module.InvokeAsync<IJSObjectReference>("initializeBrowserSidebar", _dotNetRef, Context.BrowserId, StaticRendering);
            else
            {
                _legacySetup = true;
                await _module.InvokeVoidAsync("initializeSidebar", _dotNetRef, CookieKey, StaticRendering);
            }
            if (!_disposed) Context.StateChanged += OnStateChanged;
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or TaskCanceledException) { }
        catch (ObjectDisposedException) when (_disposed) { }
        finally { _lifecycleGate.Release(); }
    }

    /// <summary>
    /// Handles sidebar state changes and persists them to cookies.
    /// </summary>
    private async void OnStateChanged(object? sender, EventArgs e)
    {
        if (_disposed) return;
        // Persist sidebar state to cookie when it changes
        if (_module != null && !string.IsNullOrEmpty(CookieKey))
        {
            try
            {
                await _module.InvokeVoidAsync("saveSidebarState", CookieKey, Context.Open);
            }
            catch (Exception ex) when (ex is JSException or JSDisconnectedException or ObjectDisposedException)
            {
                // Ignore persistence errors
            }
        }

        // Notify UI of state change
        if (!_disposed) await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Called from JavaScript when mobile state changes.
    /// </summary>
    [JSInvokable]
    public void OnMobileChange(bool isMobile)
    {
        if (!isMobile && Context.InteractionMode == NeoUI.Blazor.Primitives.InteractionMode.Browser)
            Context.SetOpenMobile(false);
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

    /// <inheritdoc/>
    public void Dispose()
    {
        _persistingSubscription.Dispose();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        Context.StateChanged -= OnStateChanged;
        await _lifecycleGate.WaitAsync();
        try
        {
            if (_browserController != null)
            {
                await _browserController.InvokeVoidAsync("dispose");
                await _browserController.DisposeAsync();
            }
            else if (_legacySetup && _module != null) await _module.InvokeVoidAsync("cleanup");
            if (_module != null) await _module.DisposeAsync();
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or TaskCanceledException or ObjectDisposedException) { }
        finally
        {
            _dotNetRef?.Dispose();
            _persistingSubscription.Dispose();
            _lifecycleGate.Release();
        }
        GC.SuppressFinalize(this);
    }
}
