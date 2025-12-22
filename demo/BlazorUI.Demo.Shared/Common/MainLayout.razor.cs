using BlazorUI.Demo.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Demo.Shared.Common;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private CollapsibleStateService StateService { get; set; } = null!;
    
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;
    
    // Reference to the SpotlightCommandPalette component
    private SpotlightCommandPalette? _spotlightRef;
    
    // Platform-specific modifier key (⌘ for Mac, Ctrl for others)
    private string _modifierKey = "Ctrl";

    // State for each collapsible menu section
    private bool _primitivesMenuOpen;
    private bool _componentsMenuOpen;
    private bool _iconsMenuOpen;

    // State keys for localStorage
    private const string PrimitivesMenuKey = "sidebar-primitives-menu";
    private const string ComponentsMenuKey = "sidebar-components-menu";
    private const string IconsMenuKey = "sidebar-icons-menu";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load saved state from localStorage on first render
            _primitivesMenuOpen = await StateService.GetStateAsync(PrimitivesMenuKey, defaultValue: false);
            _componentsMenuOpen = await StateService.GetStateAsync(ComponentsMenuKey, defaultValue: false);
            _iconsMenuOpen = await StateService.GetStateAsync(IconsMenuKey, defaultValue: false);

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

    // Event handlers for state changes
    private async Task OnPrimitivesMenuOpenChanged(bool isOpen)
    {
        _primitivesMenuOpen = isOpen;
        await StateService.SetStateAsync(PrimitivesMenuKey, isOpen);
    }

    private async Task OnComponentsMenuOpenChanged(bool isOpen)
    {
        _componentsMenuOpen = isOpen;
        await StateService.SetStateAsync(ComponentsMenuKey, isOpen);
    }

    private async Task OnIconsMenuOpenChanged(bool isOpen)
    {
        _iconsMenuOpen = isOpen;
        await StateService.SetStateAsync(IconsMenuKey, isOpen);
    }
    
    private void OpenSpotlight()
    {
        _spotlightRef?.Open();
    }
}
