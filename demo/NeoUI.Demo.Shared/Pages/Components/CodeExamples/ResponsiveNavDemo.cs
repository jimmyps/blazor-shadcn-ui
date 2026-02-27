namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ResponsiveNavDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _triggerProps =
            [
                new("Class", "string?", null, "Additional CSS classes for the hamburger trigger button."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _contentProps =
            [
                new("Side",            "SheetSide",       "Left",  "The side from which the mobile sheet slides in."),
                new("ShowClose",       "bool",            "true",  "Whether to show the close button in the sheet."),
                new("ContentClass",    "string?",         null,    "Additional CSS classes for the sheet content area."),
                new("CloseOnNavigate", "bool",            "true",  "Whether to close the mobile menu when navigation occurs."),
                new("Header",          "RenderFragment?", null,    "Optional header content rendered above the main content."),
                new("Footer",          "RenderFragment?", null,    "Optional footer content rendered below the main content."),
                new("ChildContent",    "RenderFragment?", null,    "The main navigation content inside the sheet."),
            ];

        private const string _basicCode =
            """
            <ResponsiveNavProvider>
                <header class="flex items-center justify-between p-4 border-b bg-background">
                    <div class="flex items-center gap-4">
                        <a href="/" class="font-bold text-lg">NeoUI</a>
                        <!-- Mobile trigger - hidden on desktop -->
                        <ResponsiveNavTrigger />
                    </div>

                    <!-- Desktop navigation - hidden on mobile -->
                    <NavigationMenu Class="hidden md:flex">
                        <NavigationMenuList>
                            <NavigationMenuItem>
                                <NavigationMenuLink Href="/docs">Documentation</NavigationMenuLink>
                            </NavigationMenuItem>
                            <NavigationMenuItem Value="components">
                                <NavigationMenuTrigger>Components</NavigationMenuTrigger>
                                <NavigationMenuContent>
                                    <div class="grid gap-3 p-4 w-[400px]">
                                        <div class="p-3 rounded-lg hover:bg-accent">
                                            <a href="/components/button" class="block font-medium mb-1">Button</a>
                                            <p class="text-sm text-muted-foreground">Interactive buttons with variants.</p>
                                        </div>
                                        <div class="p-3 rounded-lg hover:bg-accent">
                                            <a href="/components/dialog" class="block font-medium mb-1">Dialog</a>
                                            <p class="text-sm text-muted-foreground">Modal dialogs for interactions.</p>
                                        </div>
                                    </div>
                                </NavigationMenuContent>
                            </NavigationMenuItem>
                            <NavigationMenuItem>
                                <NavigationMenuLink Href="/primitives">Primitives</NavigationMenuLink>
                            </NavigationMenuItem>
                        </NavigationMenuList>
                    </NavigationMenu>

                    <div class="hidden md:block">
                        <a href="/login" class="text-sm text-muted-foreground hover:text-foreground">Sign In</a>
                    </div>
                </header>

                <ResponsiveNavContent>
                    <nav class="flex flex-col space-y-4">
                        <a href="/docs" class="text-lg font-medium hover:text-primary">Documentation</a>
                        <div class="space-y-2">
                            <span class="text-sm font-medium text-muted-foreground">Components</span>
                            <div class="pl-4 space-y-2 border-l">
                                <a href="/components/button" class="block text-sm hover:text-primary">Button</a>
                                <a href="/components/dialog" class="block text-sm hover:text-primary">Dialog</a>
                            </div>
                        </div>
                        <a href="/primitives" class="text-lg font-medium hover:text-primary">Primitives</a>
                        <Separator />
                        <a href="/login" class="text-sm text-muted-foreground hover:text-primary">Sign In</a>
                    </nav>
                </ResponsiveNavContent>
            </ResponsiveNavProvider>
            """;

        private const string _headerFooterCode =
            """
            <ResponsiveNavProvider>
                <header class="flex items-center justify-between p-4 border-b bg-background">
                    <div class="flex items-center gap-4">
                        <LucideIcon Name="rocket" Size="24" Class="text-primary" />
                        <span class="font-bold">Acme Inc</span>
                        <ResponsiveNavTrigger />
                    </div>

                    <NavigationMenu Class="hidden md:flex">
                        <NavigationMenuList>
                            <NavigationMenuItem>
                                <NavigationMenuLink Href="#">Products</NavigationMenuLink>
                            </NavigationMenuItem>
                            <NavigationMenuItem>
                                <NavigationMenuLink Href="#">Solutions</NavigationMenuLink>
                            </NavigationMenuItem>
                            <NavigationMenuItem>
                                <NavigationMenuLink Href="#">Pricing</NavigationMenuLink>
                            </NavigationMenuItem>
                            <NavigationMenuItem>
                                <NavigationMenuLink Href="#">About</NavigationMenuLink>
                            </NavigationMenuItem>
                        </NavigationMenuList>
                    </NavigationMenu>
                </header>

                <ResponsiveNavContent>
                    <Header>
                        <div class="flex items-center gap-3">
                            <LucideIcon Name="rocket" Size="24" Class="text-primary" />
                            <span class="font-bold text-lg">Acme Inc</span>
                        </div>
                    </Header>
                    <ChildContent>
                        <nav class="flex flex-col space-y-4">
                            <a href="#" class="text-lg font-medium hover:text-primary">Products</a>
                            <a href="#" class="text-lg font-medium hover:text-primary">Solutions</a>
                            <a href="#" class="text-lg font-medium hover:text-primary">Pricing</a>
                            <a href="#" class="text-lg font-medium hover:text-primary">About</a>
                        </nav>
                    </ChildContent>
                    <Footer>
                        <div class="flex flex-col gap-2">
                            <a href="#" class="text-sm text-muted-foreground hover:text-primary">Contact Us</a>
                            <a href="#" class="text-sm text-muted-foreground hover:text-primary">Support</a>
                        </div>
                    </Footer>
                </ResponsiveNavContent>
            </ResponsiveNavProvider>
            """;
    }
}
