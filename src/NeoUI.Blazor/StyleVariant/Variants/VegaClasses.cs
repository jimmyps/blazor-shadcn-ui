namespace NeoUI.Blazor;

/// <summary>
/// Vega — clean, neutral, familiar. Closest to the NeoUI default.
/// Minimal overrides; base component classes are the primary truth here.
/// </summary>
internal static class VegaClasses
{
    public static readonly IReadOnlyDictionary<string, string> All =
        new Dictionary<string, string>
        {
            // Vega is the closest to NeoUI's default. Key differences vs NeoUI base:
            // - Cards use rounded-xl (vs our rounded-lg)
            // - Focus rings are ring-3 with /50 opacity (vs ring-2 solid)
            // - Inputs are slightly taller (h-9 vs h-8)
            ["Button.Root"]           = "focus-visible:ring-3 focus-visible:ring-ring/50",
            ["Toggle.Root"]           = "focus-visible:ring-3 focus-visible:ring-ring/50",
            ["Badge.Root"]            = "rounded-4xl",
            ["Card.Root"]             = "rounded-xl",
            ["Input.Root"]            = "h-9 px-2.5 focus-visible:ring-3",
            ["SelectTrigger.Root"]    = "pl-2.5 focus-visible:ring-3",
            // Phase 2 components
            ["Textarea.Root"]         = "rounded-md px-2.5 focus-visible:ring-3 focus-visible:ring-ring/50",
            ["Checkbox.Root"]         = "rounded-[4px] focus-visible:ring-3 focus-visible:ring-ring/50",
            ["Switch.Root"]           = "focus-visible:ring-3 focus-visible:ring-ring/50",
            ["NativeSelect.Root"]     = "rounded-md h-9 focus-visible:ring-3 focus-visible:ring-ring/50",
            ["SelectContent.Root"]    = "rounded-md",
            ["DropdownMenu.Content"]  = "rounded-md",
            ["Popover.Content"]       = "rounded-md",
            ["Dialog.Content"]        = "rounded-xl",
            ["Tabs.List"]             = "rounded-lg",
            ["Tabs.Trigger"]          = "rounded-md",
        };
}
