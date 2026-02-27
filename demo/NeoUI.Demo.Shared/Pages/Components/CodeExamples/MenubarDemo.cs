namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class MenubarDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _menubarProps =
            [
                new("ChildContent", "RenderFragment?",  "null",  "MenubarMenu elements."),
                new("Class",        "string?",          "null",  "Additional CSS classes for the root Menubar element."),
                new("Disabled",     "bool",             "false", "On MenubarItem: prevents interaction when true."),
                new("Checked",      "bool",             "false", "On MenubarCheckboxItem: the checked state. Use @bind-Checked."),
                new("Value",        "TValue",           "—",     "On MenubarRadioItem: the value this item represents."),
            ];

        private const string _basicCode = """
                <Menubar>
                    <MenubarMenu>
                        <MenubarTrigger>File</MenubarTrigger>
                        <MenubarContent>
                            <MenubarItem OnClick="HandleNewTab">
                                New Tab <MenubarShortcut>⌘T</MenubarShortcut>
                            </MenubarItem>
                            <MenubarSeparator />
                            <MenubarItem OnClick="HandlePrint">
                                Print... <MenubarShortcut>⌘P</MenubarShortcut>
                            </MenubarItem>
                        </MenubarContent>
                    </MenubarMenu>
                </Menubar>
                """;

        private const string _keyboardCode = """
                <Menubar>
                    <MenubarMenu>
                        <MenubarTrigger>First Menu</MenubarTrigger>
                        <MenubarContent>
                            <MenubarItem>Item 1</MenubarItem>
                            <MenubarItem>Item 2</MenubarItem>
                        </MenubarContent>
                    </MenubarMenu>
                    <MenubarMenu>
                        <MenubarTrigger>Second Menu</MenubarTrigger>
                        <MenubarContent>
                            <MenubarItem>Option A</MenubarItem>
                            <MenubarItem>Option B</MenubarItem>
                        </MenubarContent>
                    </MenubarMenu>
                </Menubar>
                """;

        private const string _iconsCode = """
                <MenubarMenu>
                    <MenubarTrigger>
                        <LucideIcon Name="file" Size="16" Class="mr-2" /> File
                    </MenubarTrigger>
                    <MenubarContent>
                        <MenubarItem>
                            <LucideIcon Name="file-plus" Size="16" Class="mr-2" /> New File
                            <MenubarShortcut>⌘N</MenubarShortcut>
                        </MenubarItem>
                    </MenubarContent>
                </MenubarMenu>
                """;

        private const string _disabledCode = """
                <MenubarItem OnClick="HandleNew">New</MenubarItem>
                <MenubarItem Disabled="true">Save (Disabled)</MenubarItem>
                """;

        private const string _checkboxRadioCode = """
                <!-- Checkbox items -->
                <MenubarCheckboxItem @bind-Checked="showToolbar">Show Toolbar</MenubarCheckboxItem>

                <!-- Radio group -->
                <MenubarRadioGroup TValue="string" @bind-Value="zoomLevel">
                    <MenubarRadioItem TValue="string" Value="@("100%")">100%</MenubarRadioItem>
                    <MenubarRadioItem TValue="string" Value="@("150%")">150%</MenubarRadioItem>
                </MenubarRadioGroup>
                """;

        private const string _submenuCode = """
                <MenubarSub>
                    <MenubarSubTrigger>Shape</MenubarSubTrigger>
                    <MenubarSubContent>
                        <MenubarItem>Rectangle</MenubarItem>
                        <MenubarItem>Circle</MenubarItem>
                        <MenubarSeparator />
                        <MenubarSub>
                            <MenubarSubTrigger>Arrows</MenubarSubTrigger>
                            <MenubarSubContent>
                                <MenubarItem>Right Arrow</MenubarItem>
                                <MenubarItem>Left Arrow</MenubarItem>
                            </MenubarSubContent>
                        </MenubarSub>
                    </MenubarSubContent>
                </MenubarSub>
                """;
    }
}
