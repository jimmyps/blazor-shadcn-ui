namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class NavigationMenuDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _navMenuProps =
            [
                new("Value",       "string?",         null, "The controlled value of the active menu item."),
                new("Class",       "string?",         null, "Additional CSS classes appended to the root element."),
                new("ChildContent","RenderFragment?", null, "Nest NavigationMenuList here."),
            ];

        private const string _defaultCode =
                """
                <NavigationMenu>
                    <NavigationMenuList>
                        <NavigationMenuItem Value="getting-started">
                            <NavigationMenuTrigger>Getting Started</NavigationMenuTrigger>
                            <NavigationMenuContent>
                                <ul class="grid gap-3 p-4 w-[400px]">
                                    <li>
                                        <NavigationMenuLink Href="/getting-started">
                                            <div class="text-sm font-medium">Installation</div>
                                            <p class="text-sm text-muted-foreground">How to install NeoUI.</p>
                                        </NavigationMenuLink>
                                    </li>
                                </ul>
                            </NavigationMenuContent>
                        </NavigationMenuItem>
                        <NavigationMenuItem>
                            <NavigationMenuLink Href="/components">Documentation</NavigationMenuLink>
                        </NavigationMenuItem>
                    </NavigationMenuList>
                </NavigationMenu>
                """;

        private const string _simpleCode =
                """
                <NavigationMenu>
                    <NavigationMenuList>
                        <NavigationMenuItem>
                            <NavigationMenuLink Href="/">Home</NavigationMenuLink>
                        </NavigationMenuItem>
                        <NavigationMenuItem>
                            <NavigationMenuLink Href="/components">Components</NavigationMenuLink>
                        </NavigationMenuItem>
                    </NavigationMenuList>
                </NavigationMenu>
                """;
    }
}
