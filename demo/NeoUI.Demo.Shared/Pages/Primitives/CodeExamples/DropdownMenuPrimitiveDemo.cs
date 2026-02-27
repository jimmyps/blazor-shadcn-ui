namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class DropdownMenuPrimitiveDemo
    {
        private const string _asChildCode =
        """
        <!-- Without AsChild -->
        <DropdownMenuPrimitive>
            <DropdownMenuTriggerPrimitive class="px-3 py-2 border rounded text-sm cursor-pointer hover:bg-accent">
                Open Menu
            </DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive class="min-w-48 p-1 border rounded-md bg-popover shadow-md">
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Item 1</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Item 2</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>

        <!-- With AsChild -->
        <DropdownMenuPrimitive>
            <DropdownMenuTriggerPrimitive AsChild>
                <Button Variant="ButtonVariant.Outline">
                    Open Menu
                    <LucideIcon Name="chevron-down" Size="16" Class="ml-2" />
                </Button>
            </DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive class="min-w-48 p-1 border rounded-md bg-popover shadow-md">
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Item 1</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Item 2</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>
        """;

        private const string _basicCode =
        """
        <DropdownMenuPrimitive>
            <DropdownMenuTriggerPrimitive class="px-3 py-2 border rounded text-sm cursor-pointer hover:bg-accent">
                Open Menu
            </DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive class="min-w-48 p-1 border rounded-md bg-popover shadow-md">
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Profile</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Settings</DropdownMenuItemPrimitive>
                <Separator class="my-1" />
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Logout</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>
        """;

        private const string _controlledCode =
        """
        <DropdownMenuPrimitive @bind-Open="isOpen">
            <DropdownMenuTriggerPrimitive>Controlled Menu</DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive>
                <DropdownMenuItemPrimitive OnClick="@(() => HandleItemClick(\"Item 1\"))">Item 1</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive OnClick="@(() => HandleItemClick(\"Item 2\"))">Item 2</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive OnClick="@(() => HandleItemClick(\"Item 3\"))">Item 3</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>
        """;

        private const string _keyboardNavCode =
        """
        <DropdownMenuPrimitive>
            <DropdownMenuTriggerPrimitive class="px-3 py-2 border rounded text-sm cursor-pointer hover:bg-accent">
                Keyboard Test Menu
            </DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive class="min-w-48 p-1 border rounded-md bg-popover shadow-md">
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">First Item</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Second Item</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive class="px-2 py-1.5 rounded opacity-50 cursor-not-allowed" Disabled="true">Disabled Item</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Third Item</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive class="px-2 py-1.5 cursor-pointer rounded hover:bg-accent">Last Item</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>
        """;

        private const string _positioningCode =
        """
        <DropdownMenuPrimitive>
            <DropdownMenuTriggerPrimitive>Bottom Start</DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive Side="@PopoverSide.Bottom" Align="@PopoverAlign.Start">
                <DropdownMenuItemPrimitive>Item 1</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive>Item 2</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>

        <DropdownMenuPrimitive>
            <DropdownMenuTriggerPrimitive>Top Center</DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive Side="@PopoverSide.Top" Align="@PopoverAlign.Center">
                <DropdownMenuItemPrimitive>Item 1</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive>Item 2</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>

        <DropdownMenuPrimitive>
            <DropdownMenuTriggerPrimitive>Right End</DropdownMenuTriggerPrimitive>
            <DropdownMenuContentPrimitive Side="@PopoverSide.Right" Align="@PopoverAlign.End">
                <DropdownMenuItemPrimitive>Item 1</DropdownMenuItemPrimitive>
                <DropdownMenuItemPrimitive>Item 2</DropdownMenuItemPrimitive>
            </DropdownMenuContentPrimitive>
        </DropdownMenuPrimitive>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _dropdownMenuProps =
    [
        new("Open", "bool?", null, "Controls whether the menu is open (controlled mode)."),
        new("OpenChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the open state changes."),
        new("Side", "PopoverSide", "Bottom", "Which side of the trigger to position the content."),
        new("Align", "PopoverAlign", "Start", "How to align the content relative to the trigger."),
        new("Disabled", "bool", "false", "Whether a menu item is disabled."),
    ];
    }
}
