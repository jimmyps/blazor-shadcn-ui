namespace NeoUI.Blazor;

/// <summary>
/// Luma — luminous, glassy, pill-shaped. rounded-4xl buttons with bg-clip-padding trick,
/// rounded-3xl inputs with translucent bg-input/50 and transparent border,
/// ring-ring/30 (30% opacity) focus rings for a softer glow effect.
/// </summary>
internal static class LumaClasses
{
    public static readonly IReadOnlyDictionary<string, string> All =
        new Dictionary<string, string>
        {
            ["Button.Root"]           = "rounded-4xl border border-transparent bg-clip-padding focus-visible:ring-3 focus-visible:ring-ring/30",
            ["Badge.Root"]            = "rounded-3xl",
            ["Card.Root"]             = "rounded-4xl shadow-md",
            ["Alert.Root"]            = "rounded-2xl",
            ["Input.Root"]            = "rounded-3xl h-9 px-3 bg-input/50 border-transparent focus-visible:ring-3 focus-visible:ring-ring/30",
            ["SelectTrigger.Root"]    = "rounded-3xl px-3 bg-input/50 border-transparent focus-visible:ring-3 focus-visible:ring-ring/30",
            // Phase 2 components — Luma: glassy/pill, bg-input/50, border-transparent, ring-ring/30
            ["Textarea.Root"]         = "rounded-2xl px-3 py-3 bg-input/50 border-transparent focus-visible:ring-3 focus-visible:ring-ring/30",
            ["Checkbox.Root"]         = "rounded-[5px] data-[state=unchecked]:bg-input/90 data-[state=unchecked]:border-transparent focus-visible:ring-3 focus-visible:ring-ring/30",
            ["Switch.Root"]           = "focus-visible:ring-3 focus-visible:ring-ring/30",
            // Switch size-specific overrides: shadcn Luma (border-2 same as base, w-11 medium same as base)
            // Only height and thumb shape differ. Travel = (track_inner - thumb_width) where inner = w - 4px (border-2)
            // calc(100%-8px) in shadcn = thumb_width - 8px
            ["Switch.Root.Small"]     = "h-4 w-7",
            ["Switch.Root.Medium"]    = "h-5",
            ["Switch.Root.Large"]     = "h-6 w-14",
            // Thumb: pill-shaped (wider than tall), calc(100%-8px) travel = thumb_width - 8px
            ["Switch.Thumb.Small"]    = "h-3 w-4 data-[state=checked]:translate-x-[8px]",
            ["Switch.Thumb.Medium"]   = "h-4 w-6 data-[state=checked]:translate-x-4",
            ["Switch.Thumb.Large"]    = "h-5 w-8 data-[state=checked]:translate-x-[20px]",
            ["NativeSelect.Root"]     = "rounded-3xl h-9 pl-3 bg-input/50 border-transparent focus-visible:ring-3 focus-visible:ring-ring/30",
            ["SelectContent.Root"]    = "rounded-3xl shadow-2xl",
            ["SelectContent.Inner"]   = "p-1.5",
            ["SelectItem.Root"]       = "rounded-2xl px-3 py-2",
            ["Menu.CheckboxItem"]     = "rounded-2xl",
            ["Menu.RadioItem"]        = "rounded-2xl",
            ["DropdownMenu.Trigger"]  = "rounded-4xl",
            ["DropdownMenu.Content"]  = "rounded-3xl shadow-2xl",
            ["DropdownMenu.SubContent"] = "rounded-3xl shadow-2xl",
            ["DropdownMenu.Item"]     = "rounded-2xl px-3 py-2",
            ["Menu.Inner"]            = "p-1.5",
            ["Menu.SubTrigger"]       = "rounded-2xl px-3 py-2",
            ["Menubar.Root"]          = "rounded-full",
            ["Menubar.Trigger"]       = "rounded-2xl px-3",
            ["ContextMenu.Content"]   = "rounded-3xl shadow-2xl",
            ["ContextMenu.SubContent"] = "rounded-3xl shadow-2xl",
            ["Menubar.Content"]       = "rounded-3xl shadow-2xl",
            ["Menubar.SubContent"]    = "rounded-3xl shadow-2xl",
            ["Menubar.Item"]          = "rounded-2xl px-3 py-2",
            ["ContextMenu.Item"]      = "rounded-2xl px-3 py-2",
            ["Popover.Content"]       = "rounded-2xl shadow-2xl",
            ["Dialog.Content"]        = "rounded-4xl",
            ["Dialog.Close"]          = "rounded-2xl",
            ["Tabs.List"]             = "rounded-3xl",
            ["Tabs.Trigger"]          = "rounded-2xl",
            ["DataView.SegmentedControl"] = "rounded-3xl",
            ["SelectionIndicator.Root"] = "rounded-2xl",
            ["Calendar.Root"]         = "rounded-2xl",
            ["Calendar.Day"]          = "rounded-full",
            ["Calendar.NavButton"]    = "rounded-full",
            ["Calendar.Dropdown"]     = "rounded-3xl bg-input/50 border-transparent",
            ["Command.Content"]       = "rounded-2xl",
            ["Command.Item"]          = "rounded-2xl px-3 py-2",
            ["TagInput.Root"]         = "rounded-3xl bg-input/50 border-transparent focus-within:ring-3 focus-within:ring-ring/30",
            ["InputGroup.Root"]       = "rounded-3xl bg-input/50 border-transparent",
            ["InputOtp.Slot"]         = "first:rounded-l-3xl last:rounded-r-3xl",
            ["HoverCard.Content"]     = "rounded-2xl shadow-2xl",
            ["NavigationMenu.Content"]  = "rounded-3xl shadow-2xl",
            ["NavigationMenu.Viewport"] = "rounded-3xl shadow-2xl",
            ["NavigationMenu.Trigger"]  = "rounded-4xl",
            ["NavigationMenu.Link"]     = "rounded-2xl px-3 py-2",
            // Composite editors
            ["FileUpload.DropZone"]   = "rounded-4xl",
            ["MarkdownEditor.Root"]   = "rounded-3xl",
            ["RichTextEditor.Root"]   = "rounded-3xl",
            // Pagination
            ["Pagination.Link"]       = "rounded-full",
            // ToggleGroup
            ["Toggle.Root"]           = "rounded-4xl focus-visible:ring-3 focus-visible:ring-ring/30",
            ["ToggleGroup.Root"]      = "rounded-4xl",
            ["ToggleGroup.Item"]      = "rounded-3xl",
            // Slider — Luma: pill thumb (w-6), white bg, shadow-md, ring
            ["Slider.Root"]           = "[&::-webkit-slider-thumb]:w-6 [&::-webkit-slider-thumb]:shadow-md [&::-webkit-slider-thumb]:border-0 [&::-webkit-slider-thumb]:ring-1 [&::-webkit-slider-thumb]:ring-black/10 [&:not(:disabled)::-webkit-slider-thumb:hover]:ring-4 [&:not(:disabled)::-webkit-slider-thumb:hover]:ring-ring/30 [&:not(:disabled):focus-visible::-webkit-slider-thumb]:ring-4 [&:not(:disabled):focus-visible::-webkit-slider-thumb]:ring-ring/30 [&::-moz-range-thumb]:w-6 [&::-moz-range-thumb]:shadow-md [&::-moz-range-thumb]:border-0",
            ["Slider.Handle"]         = "data-[orientation=horizontal]:w-6 data-[orientation=vertical]:h-6 shadow-md border-0 ring-1 ring-black/10 not-aria-disabled:hover:ring-4 not-aria-disabled:hover:ring-ring/30 not-aria-disabled:focus-visible:ring-4 not-aria-disabled:focus-visible:ring-ring/30",
            // TreeView
            ["TreeView.Root"]         = "rounded-2xl",
            ["TreeView.Row"]          = "rounded-xl",
            ["TreeView.ChevronWrap"]  = "rounded-lg",
            // Tooltip
            ["Tooltip.Content"]       = "rounded-2xl px-3.5 py-2 shadow-lg",
            // Toast
            ["Toast.Root"]            = "rounded-2xl shadow-lg",
            // Sidebar
            ["Sidebar.MenuButton"]    = "rounded-xl",
            ["Sidebar.Trigger"]       = "rounded-xl",
            // Sheet / Drawer — Luma: round only the interior edge using data-side/data-direction
            ["Sheet.Content"]         = "data-[side=left]:rounded-r-2xl data-[side=right]:rounded-l-2xl data-[side=top]:rounded-b-2xl data-[side=bottom]:rounded-t-2xl",
            ["Drawer.Content"]        = "data-[direction=left]:rounded-r-2xl data-[direction=right]:rounded-l-2xl data-[direction=top]:rounded-b-2xl data-[direction=bottom]:rounded-t-2xl",
            // RadioGroup
            ["RadioGroup.Item"]       = "focus-visible:ring-3 focus-visible:ring-ring/30",
        };
}
