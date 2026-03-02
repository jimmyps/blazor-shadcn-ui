using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.ResponsiveNav;

/// <summary>
/// Provider component for responsive navigation functionality.
/// </summary>
public partial class ResponsiveNavProvider : ComponentBase
{
    private ResponsiveNavContext Context { get; set; } = new();
    private IJSObjectReference? _module;
    private DotNetObjectReference<ResponsiveNavProvider>? _dotNetRef;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Initializes the responsive navigation provider after rendering.
    /// </summary>
    /// <param name="firstRender">Whether this is the first render.</param>
    /// <returns>A task representing the asynchronous operation.</returns>

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                // Load the responsive nav JavaScript module
                _module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/responsive-nav.js");

                // Create a reference to this component for JS callbacks
                _dotNetRef = DotNetObjectReference.Create(this);

                // Initialize mobile detection
                await _module.InvokeVoidAsync("initialize", _dotNetRef);

                // Subscribe to state changes
                Context.StateChanged += OnStateChanged;

                StateHasChanged();
            }
            catch (JSException)
            {
                // JS module not available, continue without JS features
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// Handles state changes from the responsive navigation context.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private async void OnStateChanged(object? sender, EventArgs e)
    {
        // Notify UI of state change
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Called from JavaScript when mobile state changes.
    /// </summary>
    /// <param name="isMobile">Whether the viewport is in mobile mode.</param>
    [JSInvokable]
    public void OnMobileChange(bool isMobile)
    {
        Context.SetIsMobile(isMobile);
    }

    /// <summary>
    /// Disposes the responsive navigation provider and releases resources.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
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
