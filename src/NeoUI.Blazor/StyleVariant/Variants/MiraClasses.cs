namespace NeoUI.Blazor;

/// <summary>
/// Mira — ultra-compact for data-dense interfaces. xs/relaxed text, h-7 inputs,
/// soft bg-input/20 on form fields, subtle ring-ring/30 focus (30% opacity).
/// </summary>
internal static class MiraClasses
{
    public static readonly IReadOnlyDictionary<string, string> All =
        new Dictionary<string, string>
        {
            ["Button.Root"]           = "text-xs/relaxed focus-visible:ring-ring/30",
            ["Toggle.Root"]           = "text-xs/relaxed focus-visible:ring-ring/30",
            ["Badge.Root"]            = "text-[0.625rem]",
            ["Alert.Root"]            = "text-xs",
            ["Input.Root"]            = "h-7 bg-input/20 text-xs focus-visible:ring-ring/30",
            ["SelectTrigger.Root"]    = "h-7 text-xs focus-visible:ring-ring/30",
            // Phase 2 components — Mira: compact/dense, ring-ring/30, bg-input/20 on form fields
            ["Textarea.Root"]         = "rounded-md bg-input/20 px-2 text-xs/relaxed focus-visible:ring-2 focus-visible:ring-ring/30",
            ["Checkbox.Root"]         = "rounded-[4px] focus-visible:ring-2 focus-visible:ring-ring/30",
            ["Switch.Root"]           = "focus-visible:ring-2 focus-visible:ring-ring/30",
            ["NativeSelect.Root"]     = "rounded-md h-7 bg-input/20 px-2 text-xs/relaxed focus-visible:ring-2 focus-visible:ring-ring/30",
            ["SelectContent.Root"]    = "rounded-lg",
            ["SelectItem.Root"]       = "rounded-md",
            ["DropdownMenu.Content"]  = "rounded-lg",
            ["DropdownMenu.Item"]     = "rounded-md",
            ["Popover.Content"]       = "rounded-lg",
            ["Dialog.Content"]        = "rounded-xl",
            ["Tabs.List"]             = "rounded-lg",
            ["DataView.SegmentedControl"] = "rounded-lg",
            ["Tabs.Trigger"]          = "rounded-md",
            // Tooltip
            ["Tooltip.Content"]       = "rounded-sm text-xs px-2 py-1",
            // Toast
            ["Toast.Root"]            = "rounded-lg text-xs",
        };
}
