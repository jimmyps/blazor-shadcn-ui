namespace NeoUI.Blazor;

/// <summary>
/// Mira — ultra-compact for data-dense interfaces. xs/relaxed text, h-7 inputs,
/// soft bg-input/20 on form fields, subtle ring-ring/30 focus (30% opacity).
/// Rounding uses standard md/lg scale — compact but not boxy.
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
            ["SelectContent.Inner"]   = "p-1",
            ["SelectItem.Root"]       = "rounded-md",
            ["Menu.CheckboxItem"]     = "rounded-md",
            ["Menu.RadioItem"]        = "rounded-md",
            ["Menu.SubTrigger"]       = "rounded-md",
            ["Menu.Inner"]            = "p-1",
            ["DropdownMenu.Trigger"]  = "rounded-md",
            ["DropdownMenu.Content"]  = "rounded-lg",
            ["DropdownMenu.SubContent"] = "rounded-lg",
            ["DropdownMenu.Item"]     = "rounded-md",
            ["Menubar.Trigger"]       = "rounded-md text-xs",
            ["Menubar.Content"]       = "rounded-lg",
            ["Menubar.SubContent"]    = "rounded-lg",
            ["Menubar.Item"]          = "rounded-md",
            ["ContextMenu.Content"]   = "rounded-lg",
            ["ContextMenu.SubContent"] = "rounded-lg",
            ["ContextMenu.Item"]      = "rounded-md",
            ["Popover.Content"]       = "rounded-lg",
            ["Dialog.Content"]        = "rounded-xl",
            ["Dialog.Close"]          = "rounded-md",
            ["SelectionIndicator.Root"] = "rounded-md",
            ["Tabs.List"]             = "rounded-lg",
            ["Tabs.Trigger"]          = "rounded-md",
            ["DataView.SegmentedControl"] = "rounded-lg",
            ["Calendar.Root"]         = "rounded-lg",
            ["Calendar.Day"]          = "rounded-md",
            ["Calendar.NavButton"]    = "rounded-md",
            ["Calendar.Dropdown"]     = "rounded-md bg-input/20 text-xs",
            ["Command.Content"]       = "rounded-lg",
            ["Command.Item"]          = "rounded-md text-xs",
            ["TagInput.Root"]         = "h-7 bg-input/20 text-xs",
            ["InputGroup.Root"]       = "rounded-md",
            ["InputOtp.Slot"]         = "first:rounded-l-md last:rounded-r-md",
            ["HoverCard.Content"]     = "rounded-lg",
            // Composite editors
            ["FileUpload.DropZone"]   = "rounded-xl",
            ["MarkdownEditor.Root"]   = "rounded-md",
            ["RichTextEditor.Root"]   = "rounded-md",
            // Pagination
            ["Pagination.Link"]       = "rounded-md",
            // ToggleGroup
            ["ToggleGroup.Root"]      = "rounded-md",
            ["ToggleGroup.Item"]      = "rounded-sm",
            // Slider — Mira: compact h-1 track, small h-3 w-3 thumb (same as Nova/Lyra)
            ["Slider.Root"]           = "h-1 [&::-webkit-slider-thumb]:h-3 [&::-webkit-slider-thumb]:w-3 [&::-moz-range-thumb]:h-3 [&::-moz-range-thumb]:w-3",
            ["Slider.Handle"]         = "h-3 w-3",
            // TreeView
            ["TreeView.Root"]         = "rounded-md",
            ["TreeView.Row"]          = "rounded-sm",
            ["TreeView.ChevronWrap"]  = "rounded-sm",
            // Tooltip
            ["Tooltip.Content"]       = "rounded-sm text-xs px-2 py-1",
            // Toast
            ["Toast.Root"]            = "rounded-lg text-xs",
            // Sidebar
            ["Sidebar.MenuButton"]    = "rounded-md",
            ["Sidebar.Trigger"]       = "rounded-md",
            // Sheet / Drawer — Mira: standard compact rounding
            ["Sheet.Content"]         = "data-[side=left]:rounded-r-lg data-[side=right]:rounded-l-lg data-[side=top]:rounded-b-lg data-[side=bottom]:rounded-t-lg",
            ["Drawer.Content"]        = "data-[direction=left]:rounded-r-lg data-[direction=right]:rounded-l-lg data-[direction=top]:rounded-b-lg data-[direction=bottom]:rounded-t-lg",
            // RadioGroup — Mira: compact focus ring
            ["RadioGroup.Item"]       = "focus-visible:ring-2 focus-visible:ring-ring/30",
            // Progress — Mira: compact height
            ["Progress.Root"]         = "h-2",
        };
}
