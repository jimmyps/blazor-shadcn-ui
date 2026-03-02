namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ContextMenuDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _contextMenuProps =
            [
                new("ChildContent",  "RenderFragment?",       "null",  "ContextMenuTrigger and ContextMenuContent."),
                new("Disabled",      "bool",                  "false", "On ContextMenuItem: prevents interaction when true."),
                new("Inset",         "bool",                  "false", "On ContextMenuItem/Label: adds left padding to align with icon items."),
                new("Checked",       "bool",                  "false", "On ContextMenuCheckboxItem: the checked state. Use @bind-Checked."),
                new("Value",         "TValue",                "—",     "On ContextMenuRadioItem: the value this item represents."),
                new("Class",         "string?",               "null",  "Additional CSS classes."),
                new("OnClick",       "EventCallback",         "—",     "On ContextMenuItem: callback invoked when the item is clicked."),
            ];

        private const string _defaultCode = """
                <ContextMenu>
                    <ContextMenuTrigger Class="flex h-[150px] w-full items-center justify-center rounded-md border border-dashed">
                        Right click here
                    </ContextMenuTrigger>
                    <ContextMenuContent Class="w-64">
                        <ContextMenuItem>Back</ContextMenuItem>
                        <ContextMenuItem Disabled="true">Forward</ContextMenuItem>
                        <ContextMenuSeparator />
                        <ContextMenuItem>Save As...</ContextMenuItem>
                        <ContextMenuItem>Print...</ContextMenuItem>
                    </ContextMenuContent>
                </ContextMenu>
                """;

        private const string _iconsCode = """
                <ContextMenuContent Class="w-56">
                    <ContextMenuItem>
                        <LucideIcon Name="folder-open" Size="16" Class="mr-2" /> Open
                    </ContextMenuItem>
                    <ContextMenuItem>
                        <LucideIcon Name="copy" Size="16" Class="mr-2" /> Copy
                        <ContextMenuShortcut>⌘C</ContextMenuShortcut>
                    </ContextMenuItem>
                    <ContextMenuSeparator />
                    <ContextMenuItem Class="text-destructive focus:text-destructive">
                        <LucideIcon Name="trash-2" Size="16" Class="mr-2" /> Delete
                    </ContextMenuItem>
                </ContextMenuContent>
                """;

        private const string _groupsCode = """
                <ContextMenuContent>
                    <ContextMenuLabel>File Operations</ContextMenuLabel>
                    <ContextMenuSeparator />
                    <ContextMenuItem>New File</ContextMenuItem>
                    <ContextMenuSeparator />
                    <ContextMenuLabel>Edit Operations</ContextMenuLabel>
                    <ContextMenuSeparator />
                    <ContextMenuItem>Cut</ContextMenuItem>
                    <ContextMenuItem>Copy</ContextMenuItem>
                </ContextMenuContent>
                """;

        private const string _destructiveCode = """
                <ContextMenuContent>
                    <ContextMenuItem>Edit</ContextMenuItem>
                    <ContextMenuItem>Duplicate</ContextMenuItem>
                    <ContextMenuSeparator />
                    <ContextMenuItem Class="text-destructive focus:text-destructive">
                        Move to Trash
                    </ContextMenuItem>
                </ContextMenuContent>
                """;

        private const string _insetCode = """
                <ContextMenuContent>
                    <ContextMenuItem>
                        <LucideIcon Name="chevron-left" Size="16" Class="mr-2" /> Back
                    </ContextMenuItem>
                    <ContextMenuItem Inset="true">Forward</ContextMenuItem>
                    <ContextMenuSeparator />
                    <ContextMenuLabel Inset="true">Developer Tools</ContextMenuLabel>
                    <ContextMenuItem Inset="true">Inspect Element</ContextMenuItem>
                </ContextMenuContent>
                """;

        private const string _imageCode = """
                <ContextMenu>
                    <ContextMenuTrigger>
                        <img src="image.jpg" alt="Photo" class="rounded-md cursor-context-menu" />
                    </ContextMenuTrigger>
                    <ContextMenuContent>
                        <ContextMenuItem>Open Image in New Tab</ContextMenuItem>
                        <ContextMenuItem>Save Image As...</ContextMenuItem>
                        <ContextMenuSeparator />
                        <ContextMenuItem>Copy Image</ContextMenuItem>
                    </ContextMenuContent>
                </ContextMenu>
                """;

        private const string _checkboxCode = """
                <ContextMenuContent>
                    <ContextMenuLabel>View Options</ContextMenuLabel>
                    <ContextMenuSeparator />
                    <ContextMenuCheckboxItem @bind-Checked="showToolbar">Show Toolbar</ContextMenuCheckboxItem>
                    <ContextMenuCheckboxItem @bind-Checked="showSidebar">Show Sidebar</ContextMenuCheckboxItem>
                </ContextMenuContent>
                """;

        private const string _radioCode = """
                <ContextMenuContent>
                    <ContextMenuLabel>View Mode</ContextMenuLabel>
                    <ContextMenuSeparator />
                    <ContextMenuRadioGroup TValue="string" @bind-Value="selectedViewMode">
                        <ContextMenuRadioItem TValue="string" Value="@("grid")">Grid View</ContextMenuRadioItem>
                        <ContextMenuRadioItem TValue="string" Value="@("list")">List View</ContextMenuRadioItem>
                    </ContextMenuRadioGroup>
                </ContextMenuContent>
                """;

        private const string _submenuCode = """
                <ContextMenuContent>
                    <ContextMenuItem>Open</ContextMenuItem>
                    <ContextMenuSub>
                        <ContextMenuSubTrigger>Open With</ContextMenuSubTrigger>
                        <ContextMenuSubContent>
                            <ContextMenuItem>Browser</ContextMenuItem>
                            <ContextMenuItem>VS Code</ContextMenuItem>
                        </ContextMenuSubContent>
                    </ContextMenuSub>
                </ContextMenuContent>
                """;
    }
}
