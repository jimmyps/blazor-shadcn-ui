namespace NeoUI.Blazor;

/// <summary>
/// Nova — compact dashboard feel. rounded-lg for controls (slightly more rounded than Vega's md),
/// ring-3 focus, tighter control heights (h-8 for inputs).
/// </summary>
internal static class NovaClasses
{
    public static readonly IReadOnlyDictionary<string, string> All =
        new Dictionary<string, string>
        {
            ["Button.Root"]           = "rounded-lg focus-visible:ring-3 focus-visible:ring-ring/50",
            ["Badge.Root"]            = "rounded-4xl",
            ["Card.Root"]             = "rounded-xl",
            ["Alert.Root"]            = "rounded-xl",
            ["Input.Root"]            = "rounded-lg px-2.5 focus-visible:ring-3",
            ["SelectTrigger.Root"]    = "rounded-lg pl-2.5 focus-visible:ring-3",
            // Phase 2 components
            ["Textarea.Root"]         = "rounded-lg px-2.5 focus-visible:ring-3 focus-visible:ring-ring/50",
            ["Checkbox.Root"]         = "rounded-[4px] focus-visible:ring-3 focus-visible:ring-ring/50",
            ["Switch.Root"]           = "focus-visible:ring-3 focus-visible:ring-ring/50",
            ["NativeSelect.Root"]     = "rounded-lg h-9 focus-visible:ring-3 focus-visible:ring-ring/50",
            ["SelectContent.Root"]    = "rounded-xl",
            ["SelectContent.Inner"]   = "p-1",
            ["SelectItem.Root"]       = "rounded-lg",
            ["Menu.CheckboxItem"]     = "rounded-lg",
            ["Menu.RadioItem"]        = "rounded-lg",
            ["Menu.SubTrigger"]       = "rounded-lg",
            ["Menu.Inner"]            = "p-1",
            ["ContextMenu.Content"]   = "rounded-xl",
            ["ContextMenu.SubContent"] = "rounded-xl",
            ["Menubar.Root"]          = "rounded-xl",
            ["Menubar.Content"]       = "rounded-xl",
            ["Menubar.SubContent"]    = "rounded-xl",
            ["Menubar.Trigger"]       = "rounded-lg px-2.5",
            ["Menubar.Item"]          = "rounded-lg",
            ["ContextMenu.Item"]      = "rounded-lg",
            ["DropdownMenu.Trigger"]  = "rounded-lg",
            ["DropdownMenu.Content"]  = "rounded-xl",
            ["DropdownMenu.SubContent"] = "rounded-xl",
            ["DropdownMenu.Item"]     = "rounded-lg",
            ["Popover.Content"]       = "rounded-xl",
            ["Dialog.Content"]        = "rounded-xl",
            ["Dialog.Close"]          = "rounded-lg",
            ["SelectionIndicator.Root"] = "rounded-lg",
            ["Tabs.List"]             = "rounded-xl",
            ["Tabs.Trigger"]          = "rounded-lg",
            ["DataView.SegmentedControl"] = "rounded-xl",
            ["Calendar.Root"]         = "rounded-lg",
            ["Calendar.Day"]          = "rounded-lg",
            ["Calendar.NavButton"]    = "rounded-lg",
            ["Calendar.Dropdown"]     = "rounded-lg",
            ["Command.Content"]       = "rounded-lg",
            ["Command.Item"]          = "rounded-lg",
            ["TagInput.Root"]         = "rounded-lg focus-within:ring-3",
            ["InputGroup.Root"]       = "rounded-lg",
            ["InputOtp.Slot"]         = "first:rounded-l-lg last:rounded-r-lg",
            ["HoverCard.Content"]     = "rounded-xl",
            ["NavigationMenu.Content"]  = "rounded-xl",
            ["NavigationMenu.Viewport"] = "rounded-xl",
            ["NavigationMenu.Trigger"]  = "rounded-lg",
            ["NavigationMenu.Link"]     = "rounded-lg",
            // Nova: thinner h-1 track, smaller round thumb (size-3)
            ["Slider.Root"]           = "h-1 [&::-webkit-slider-thumb]:h-3 [&::-webkit-slider-thumb]:w-3 [&::-moz-range-thumb]:h-3 [&::-moz-range-thumb]:w-3",
            // Composite editors
            ["FileUpload.DropZone"]   = "rounded-xl",
            ["MarkdownEditor.Root"]   = "rounded-xl",
            ["RichTextEditor.Root"]   = "rounded-xl",
            // Pagination
            ["Pagination.Link"]       = "rounded-xl",
            // ToggleGroup
            ["Toggle.Root"]           = "rounded-lg focus-visible:ring-3 focus-visible:ring-ring/50",
            ["ToggleGroup.Root"]      = "rounded-xl",
            ["ToggleGroup.Item"]      = "rounded-lg",
            // Slider — Nova: small h-3 w-3 thumb for h-1 track
            ["Slider.Handle"]         = "h-3 w-3",
            // TreeView
            ["TreeView.Root"]         = "rounded-lg",
            ["TreeView.Row"]          = "rounded-sm",
            ["TreeView.ChevronWrap"]  = "rounded-[3px]",
            // Tooltip
            ["Tooltip.Content"]       = "rounded-sm text-xs",
            // Toast
            ["Toast.Root"]            = "rounded-lg",
            // Sidebar
            ["Sidebar.MenuButton"]    = "rounded-sm",
            ["Sidebar.Trigger"]       = "rounded-sm",
            // Sheet / Drawer
            ["Sheet.Content"]         = "data-[side=left]:rounded-r-xl data-[side=right]:rounded-l-xl data-[side=top]:rounded-b-xl data-[side=bottom]:rounded-t-xl",
            ["Drawer.Content"]        = "data-[direction=left]:rounded-r-xl data-[direction=right]:rounded-l-xl data-[direction=top]:rounded-b-xl data-[direction=bottom]:rounded-t-xl",
            // RadioGroup
            ["RadioGroup.Item"]       = "focus-visible:ring-3",
            // Progress — Nova: thinner track, subtle rounding
            ["Progress.Root"]         = "h-2 rounded-sm",
        };
}
