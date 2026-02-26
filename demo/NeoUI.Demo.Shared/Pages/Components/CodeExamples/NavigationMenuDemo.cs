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
                                <ul class="grid gap-3 p-2 md:w-[400px] lg:w-[500px] lg:grid-cols-[.75fr_1fr]">
                                    <li class="row-span-3">
                                        <NavigationMenuLink Href="/" Class="flex h-full w-full select-none flex-col justify-end rounded-md bg-gradient-to-b from-muted/50 to-muted p-6 no-underline outline-none focus:shadow-md">
                                            <div class="mb-2 mt-4 text-lg font-medium">NeoUI</div>
                                            <p class="text-sm leading-tight text-muted-foreground">
                                                Beautifully designed components built with Tailwind CSS.
                                            </p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/architecture">
                                            <div class="text-sm font-medium leading-none">Architecture</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">
                                                Learn about our two-tier primitives architecture.
                                            </p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/getting-started">
                                            <div class="text-sm font-medium leading-none">Installation</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">
                                                How to install and configure NeoUI.
                                            </p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/primitives">
                                            <div class="text-sm font-medium leading-none">Primitives</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">
                                                Headless primitives for building custom UIs.
                                            </p>
                                        </NavigationMenuLink>
                                    </li>
                                </ul>
                            </NavigationMenuContent>
                        </NavigationMenuItem>
                        <NavigationMenuItem Value="components">
                            <NavigationMenuTrigger>Components</NavigationMenuTrigger>
                            <NavigationMenuContent>
                                <ul class="grid w-[400px] gap-3 p-2 md:w-[500px] md:grid-cols-2 lg:w-[600px]">
                                    <li>
                                        <NavigationMenuLink Href="/components/accordion">
                                            <div class="text-sm font-medium leading-none">Accordion</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">Expandable sections for showing and hiding content.</p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/components/button">
                                            <div class="text-sm font-medium leading-none">Button</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">Interactive buttons with multiple variants.</p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/components/dialog">
                                            <div class="text-sm font-medium leading-none">Dialog</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">Modal dialogs with backdrop and focus management.</p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/components/input">
                                            <div class="text-sm font-medium leading-none">Input</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">Text inputs with validation and accessibility support.</p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/components/select">
                                            <div class="text-sm font-medium leading-none">Select</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">Dropdown selection with search and multi-select.</p>
                                        </NavigationMenuLink>
                                    </li>
                                    <li>
                                        <NavigationMenuLink Href="/components/tooltip">
                                            <div class="text-sm font-medium leading-none">Tooltip</div>
                                            <p class="line-clamp-2 text-sm leading-snug text-muted-foreground">Contextual hints displayed on hover or focus.</p>
                                        </NavigationMenuLink>
                                    </li>
                                </ul>
                            </NavigationMenuContent>
                        </NavigationMenuItem>
                        <NavigationMenuItem>
                            <NavigationMenuLink Href="/components" Class="group inline-flex h-10 w-max items-center justify-center rounded-md bg-background px-4 py-2 text-sm font-medium transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground focus:outline-none">
                                Documentation
                            </NavigationMenuLink>
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
