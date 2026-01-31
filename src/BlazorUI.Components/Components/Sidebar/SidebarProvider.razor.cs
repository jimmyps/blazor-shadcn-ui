using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Sidebar;

public partial class SidebarProvider
{
    private SidebarContext Context { get; set; } = new();
    private IJSObjectReference? _module;
    private DotNetObjectReference<SidebarProvider>? _dotNetRef;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IHttpContextAccessor? HttpContextAccessor { get; set; }

    protected override void OnInitialized()
    {
        bool initialOpen = DefaultOpen;
        
        // Try to read cookie server-side during prerendering
        if (HttpContextAccessor?.HttpContext != null && !string.IsNullOrEmpty(CookieKey))
        {
            var cookieValue = HttpContextAccessor.HttpContext.Request.Cookies[CookieKey];
            if (bool.TryParse(cookieValue, out var savedOpen))
            {
                initialOpen = savedOpen;
            }
        }
        
        // Initialize context immediately for SSR - enables expand/collapse to work during prerendering
        Context.Initialize(open: initialOpen, variant: Variant, side: Side, staticRendering: StaticRendering);
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

                // Try to restore state from cookie if persistence is enabled
                if (!string.IsNullOrEmpty(CookieKey))
                {
                    var savedOpen = await _module.InvokeAsync<bool?>("getSidebarState", CookieKey);
                    if (savedOpen.HasValue)
                    {
                        Context.SetOpen(savedOpen.Value);
                    }
                }

                // Set up mobile detection, keyboard shortcuts, and optional static rendering support
                await _module.InvokeVoidAsync("initializeSidebar", _dotNetRef, CookieKey, StaticRendering);

                // Subscribe to state changes for persistence
                Context.StateChanged += OnStateChanged;

                StateHasChanged();
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

        GC.SuppressFinalize(this);
    }
}
