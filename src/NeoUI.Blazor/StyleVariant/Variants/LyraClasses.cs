namespace NeoUI.Blazor;

/// <summary>
/// Lyra — boxy, sharp, developer-tool aesthetic. Zero radius everywhere, xs text throughout,
/// ring-1 minimal focus, compact sizing. Suits mono fonts and dense data UIs.
/// </summary>
internal static class LyraClasses
{
    public static readonly IReadOnlyDictionary<string, string> All =
        new Dictionary<string, string>
        {
            ["Button.Root"]           = "rounded-none text-xs focus-visible:ring-1",
            ["Badge.Root"]            = "rounded-none",
            ["Card.Root"]             = "rounded-none",
            ["Alert.Root"]            = "rounded-none text-xs",
            ["Input.Root"]            = "rounded-none px-2.5 text-xs focus-visible:ring-1",
            ["SelectTrigger.Root"]    = "rounded-none pl-2.5 text-xs focus-visible:ring-1",
            // Phase 2 components — Lyra: sharp/boxy, rounded-none everywhere, ring-1
            ["Textarea.Root"]         = "rounded-none px-2.5 text-xs focus-visible:ring-1",
            ["Checkbox.Root"]         = "rounded-none focus-visible:ring-1",
            ["Switch.Root"]           = "focus-visible:ring-1",
            ["NativeSelect.Root"]     = "rounded-none h-8 text-xs focus-visible:ring-1",
            ["SelectContent.Root"]    = "rounded-none",
            ["SelectItem.Root"]       = "rounded-none",
            ["Menu.CheckboxItem"]     = "rounded-none",
            ["Menu.RadioItem"]        = "rounded-none",
            ["ContextMenu.Content"]   = "rounded-none",
            ["ContextMenu.SubContent"] = "rounded-none",
            ["Menubar.Content"]       = "rounded-none",
            ["Menubar.SubContent"]    = "rounded-none",
            ["Menubar.Item"]          = "rounded-none",
            ["ContextMenu.Item"]      = "rounded-none",
            ["DropdownMenu.Trigger"]  = "rounded-none",
            ["DropdownMenu.Content"]  = "rounded-none",
            ["DropdownMenu.SubContent"] = "rounded-none",
            ["DropdownMenu.Item"]     = "rounded-none",
            ["Popover.Content"]       = "rounded-none",
            ["Dialog.Content"]        = "rounded-none",
            ["Dialog.Close"]          = "rounded-none",
            ["SelectionIndicator.Root"] = "rounded-none",
            ["Tabs.List"]             = "rounded-none",
            ["Tabs.Trigger"]          = "rounded-none",
            ["DataView.SegmentedControl"] = "rounded-none",
            ["Calendar.Root"]         = "rounded-none",
            ["Calendar.Day"]          = "rounded-none",
            ["Calendar.NavButton"]    = "rounded-none",
            ["Calendar.Dropdown"]     = "rounded-none",
            ["Command.Content"]       = "rounded-none",
            ["Command.Item"]          = "rounded-none",
            ["TagInput.Root"]         = "rounded-none",
            ["InputGroup.Root"]       = "rounded-none",
            ["InputOtp.Slot"]         = "first:rounded-l-none last:rounded-r-none",
            ["HoverCard.Content"]     = "rounded-none",
            // Lyra: boxy h-1 track, small square thumb (size-3 rounded-none)
            ["Slider.Root"]           = "rounded-none h-1 [&::-webkit-slider-thumb]:h-3 [&::-webkit-slider-thumb]:w-3 [&::-webkit-slider-thumb]:rounded-none [&::-moz-range-thumb]:h-3 [&::-moz-range-thumb]:w-3 [&::-moz-range-thumb]:rounded-none",
            // Composite editors
            ["FileUpload.DropZone"]   = "rounded-none",
            ["MarkdownEditor.Root"]   = "rounded-none",
            ["RichTextEditor.Root"]   = "rounded-none",
            // Pagination
            ["Pagination.Link"]       = "rounded-none",
            // ToggleGroup
            ["Toggle.Root"]           = "rounded-none text-xs focus-visible:ring-1",
            ["ToggleGroup.Root"]      = "rounded-none",
            ["ToggleGroup.Item"]      = "rounded-none",
            // Slider — Lyra: square handle
            ["Slider.Handle"]         = "rounded-none",
            // TreeView
            ["TreeView.Root"]         = "rounded-none",
            ["TreeView.Row"]          = "rounded-none",
            ["TreeView.ChevronWrap"]  = "rounded-none",
            // Tooltip
            ["Tooltip.Content"]       = "rounded-none",
            // Toast
            ["Toast.Root"]            = "rounded-none",
            // Sidebar
            ["Sidebar.MenuButton"]    = "rounded-none",
            ["Sidebar.Trigger"]       = "rounded-none",
        };
}
