namespace NeoUI.Blazor;

/// <summary>
/// Maia — soft, rounded, friendly. Pill-shaped buttons (rounded-4xl), generous card radius,
/// slight translucent input background (bg-input/30), ring-[3px] focus rings.
/// </summary>
internal static class MaiaClasses
{
    public static readonly IReadOnlyDictionary<string, string> All =
        new Dictionary<string, string>
        {
            ["Button.Root"]           = "rounded-4xl focus-visible:ring-[3px] focus-visible:ring-ring/50",
            ["Badge.Root"]            = "rounded-4xl",
            ["Card.Root"]             = "rounded-2xl",
            ["Alert.Root"]            = "rounded-2xl",
            ["Input.Root"]            = "rounded-4xl h-9 px-3 bg-input/30 focus-visible:ring-[3px]",
            ["SelectTrigger.Root"]    = "rounded-4xl px-3 bg-input/30 focus-visible:ring-[3px]",
            // Phase 2 components
            ["Textarea.Root"]         = "rounded-xl bg-input/30 px-3 py-3 focus-visible:ring-[3px]",
            ["Checkbox.Root"]         = "rounded-[6px] focus-visible:ring-[3px] focus-visible:ring-ring/50",
            ["Switch.Root"]           = "focus-visible:ring-[3px] focus-visible:ring-ring/50",
            ["NativeSelect.Root"]     = "rounded-4xl h-9 pl-3 bg-input/30 focus-visible:ring-[3px]",
            ["SelectContent.Root"]    = "rounded-2xl shadow-2xl",
            ["SelectContent.Inner"]   = "p-1.5",
            ["SelectItem.Root"]       = "rounded-xl",
            ["Menu.CheckboxItem"]     = "rounded-xl",
            ["Menu.RadioItem"]        = "rounded-xl",
            ["Menu.SubTrigger"]       = "rounded-xl",
            ["Menu.Inner"]            = "p-1.5",
            ["ContextMenu.Content"]   = "rounded-2xl shadow-2xl",
            ["ContextMenu.SubContent"] = "rounded-2xl shadow-2xl",
            ["Menubar.Root"]          = "rounded-4xl",
            ["Menubar.Content"]       = "rounded-2xl shadow-2xl",
            ["Menubar.SubContent"]    = "rounded-2xl shadow-2xl",
            ["Menubar.Trigger"]       = "rounded-4xl px-3",
            ["Menubar.Item"]          = "rounded-xl",
            ["ContextMenu.Item"]      = "rounded-xl",
            ["DropdownMenu.Trigger"]  = "rounded-4xl",
            ["DropdownMenu.Content"]  = "rounded-2xl shadow-2xl",
            ["DropdownMenu.SubContent"] = "rounded-2xl shadow-2xl",
            ["DropdownMenu.Item"]     = "rounded-xl",
            ["Popover.Content"]       = "rounded-2xl shadow-2xl",
            ["Dialog.Content"]        = "rounded-4xl",
            ["Dialog.Close"]          = "rounded-2xl",
            ["SelectionIndicator.Root"] = "rounded-xl",
            ["Tabs.List"]             = "rounded-4xl",
            ["Tabs.Trigger"]          = "rounded-xl",
            ["DataView.SegmentedControl"] = "rounded-4xl",
            ["Calendar.Root"]         = "rounded-xl",
            ["Calendar.Day"]          = "rounded-xl",
            ["Calendar.NavButton"]    = "rounded-xl",
            ["Calendar.Dropdown"]     = "rounded-4xl bg-input/30",
            ["Command.Content"]       = "rounded-xl",
            ["Command.Item"]          = "rounded-xl",
            ["TagInput.Root"]         = "rounded-4xl bg-input/30 border-transparent focus-within:ring-[3px]",
            ["InputGroup.Root"]       = "rounded-4xl bg-input/30 border-transparent",
            ["InputOtp.Slot"]         = "first:rounded-l-4xl last:rounded-r-4xl",
            ["HoverCard.Content"]     = "rounded-2xl shadow-2xl",
            // Maia: thicker h-3 track, larger rounded-4xl thumb (size-5)
            ["Slider.Root"]           = "h-3 [&::-webkit-slider-thumb]:h-5 [&::-webkit-slider-thumb]:w-8 [&::-webkit-slider-thumb]:rounded-4xl [&::-moz-range-thumb]:h-5 [&::-moz-range-thumb]:w-8 [&::-moz-range-thumb]:rounded-4xl",
            // Composite editors
            ["FileUpload.DropZone"]   = "rounded-2xl",
            ["MarkdownEditor.Root"]   = "rounded-2xl",
            ["RichTextEditor.Root"]   = "rounded-2xl",
            // Pagination
            ["Pagination.Link"]       = "rounded-xl",
            // ToggleGroup
            ["Toggle.Root"]           = "rounded-4xl focus-visible:ring-[3px] focus-visible:ring-ring/50",
            ["ToggleGroup.Root"]      = "rounded-2xl",
            ["ToggleGroup.Item"]      = "rounded-xl",
            // Slider — Maia: large h-5 w-5 thumb for h-3 track
            ["Slider.Handle"]         = "h-5 w-5",
            // TreeView
            ["TreeView.Root"]         = "rounded-2xl",
            ["TreeView.Row"]          = "rounded-xl",
            ["TreeView.ChevronWrap"]  = "rounded-lg",
            // Tooltip
            ["Tooltip.Content"]       = "rounded-2xl px-4 py-2",
            // Toast
            ["Toast.Root"]            = "rounded-2xl",
            // Sidebar
            ["Sidebar.MenuButton"]    = "rounded-xl",
            ["Sidebar.Trigger"]       = "rounded-xl",
            // Sheet / Drawer — Maia: friendly rounded corners on interior edge
            ["Sheet.Content"]         = "data-[side=left]:rounded-r-2xl data-[side=right]:rounded-l-2xl data-[side=top]:rounded-b-2xl data-[side=bottom]:rounded-t-2xl",
            ["Drawer.Content"]        = "data-[direction=left]:rounded-r-2xl data-[direction=right]:rounded-l-2xl data-[direction=top]:rounded-b-2xl data-[direction=bottom]:rounded-t-2xl",
            // RadioGroup
            ["RadioGroup.Item"]       = "focus-visible:ring-[3px] focus-visible:ring-ring/50",
            // Progress — Maia: slightly thinner track
            ["Progress.Root"]         = "h-3",
        };
}
