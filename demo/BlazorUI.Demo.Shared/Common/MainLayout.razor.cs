using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Demo.Shared.Common;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;
    
    // Reference to the SpotlightCommandPalette component
    private SpotlightCommandPalette? _spotlightRef;
    
    // Platform-specific modifier key (⌘ for Mac, Ctrl for others)
    private string _modifierKey = "Ctrl";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Detect platform-specific modifier key
            try
            {
                var module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./js/keyboard-shortcuts.js");
                _modifierKey = await module.InvokeAsync<string>("getModifierKey");
            }
            catch
            {
                // Fallback to Ctrl if detection fails
                _modifierKey = "Ctrl";
            }

            // Trigger re-render with loaded state
            StateHasChanged();
        }
    }
    
    private void OpenSpotlight()
    {
        _spotlightRef?.Open();
    }
}
