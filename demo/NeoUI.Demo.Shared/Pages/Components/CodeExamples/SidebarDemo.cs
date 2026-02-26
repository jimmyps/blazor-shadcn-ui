namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SidebarDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _sidebarProps =
            [
                new("DefaultOpen",   "bool",             "true",          "On SidebarProvider: initial open state of the sidebar."),
                new("Variant",       "SidebarVariant",   "Sidebar",       "On SidebarProvider: visual variant. Options: Sidebar, Floating, Inset."),
                new("Side",          "SidebarSide",      "Left",          "On SidebarProvider: side to render. Options: Left, Right."),
                new("CookieKey",     "string",           "sidebar:state", "On SidebarProvider: cookie key for state persistence."),
                new("HeightClass",   "string?",          "null",          "On SidebarProvider: CSS height class (e.g. h-full for contained demos)."),
                new("Collapsible",   "bool",             "true",          "On Sidebar: when true, collapses to icon-only mode; when false, hides completely."),
                new("Tooltip",       "string?",          "null",          "On SidebarMenuButton: tooltip shown when sidebar is collapsed."),
                new("IsActive",      "bool",             "false",         "On SidebarMenuButton: marks the item as the active/current page."),
                new("Href",          "string?",          "null",          "On SidebarMenuButton: renders as NavLink with the given href."),
                new("Match",         "NavLinkMatch",     "Prefix",        "On SidebarMenuButton: controls how the href is matched for active state."),
            ];

        private const string _basicCode = """
                <SidebarProvider CookieKey="sidebar:demo" HeightClass="h-full">
                    <div class="flex h-full w-full">
                        <Sidebar Collapsible="true">
                            <SidebarHeader>
                                <SidebarHeaderContent>
                                    <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
                                        <LucideIcon Name="command" Size="16" />
                                    </div>
                                    <SidebarHeaderInfo>
                                        <span class="truncate font-semibold">Acme Inc</span>
                                        <span class="truncate text-xs text-muted-foreground">Enterprise</span>
                                    </SidebarHeaderInfo>
                                </SidebarHeaderContent>
                            </SidebarHeader>
                            <SidebarContent>
                                <SidebarMenu>
                                    <SidebarMenuItem>
                                        <SidebarMenuButton Tooltip="Home">
                                            <LucideIcon Name="house" Size="16" />
                                            <span>Home</span>
                                        </SidebarMenuButton>
                                    </SidebarMenuItem>
                                    <SidebarMenuItem>
                                        <SidebarMenuButton Tooltip="Inbox" IsActive="true">
                                            <LucideIcon Name="inbox" Size="16" />
                                            <span>Inbox</span>
                                        </SidebarMenuButton>
                                    </SidebarMenuItem>
                                </SidebarMenu>
                            </SidebarContent>
                            <SidebarFooter>
                                <SidebarMenu>
                                    <SidebarMenuItem>
                                        <SidebarMenuButton Tooltip="User Profile">
                                            <LucideIcon Name="user" Size="16" />
                                            <span>John Doe</span>
                                        </SidebarMenuButton>
                                    </SidebarMenuItem>
                                </SidebarMenu>
                            </SidebarFooter>
                        </Sidebar>
                        <SidebarInset>
                            <header class="flex h-16 shrink-0 items-center gap-2 border-b px-4">
                                <SidebarTrigger />
                                <h1 class="text-lg font-semibold">Dashboard</h1>
                            </header>
                        </SidebarInset>
                    </div>
                </SidebarProvider>
                """;

        private const string _navigationCode = """
                <SidebarMenuButton Href="/" Match="NavLinkMatch.All" Tooltip="Home">
                    <LucideIcon Name="house" Size="16" />
                    <span>Home</span>
                </SidebarMenuButton>

                <SidebarMenuButton Href="/components" Match="NavLinkMatch.Prefix" Tooltip="Components">
                    <LucideIcon Name="layers" Size="16" />
                    <span>Components</span>
                </SidebarMenuButton>
                """;

        private const string _groupsCode = """
                <SidebarContent>
                    <SidebarGroup>
                        <SidebarGroupLabel>Platform</SidebarGroupLabel>
                        <SidebarGroupContent>
                            <SidebarMenu>
                                <SidebarMenuItem>
                                    <SidebarMenuButton Tooltip="Dashboard">
                                        <LucideIcon Name="layout-dashboard" Size="16" />
                                        <span>Dashboard</span>
                                    </SidebarMenuButton>
                                </SidebarMenuItem>
                            </SidebarMenu>
                        </SidebarGroupContent>
                    </SidebarGroup>
                    <SidebarSeparator />
                    <SidebarGroup>
                        <SidebarGroupLabel>Settings</SidebarGroupLabel>
                        <SidebarGroupContent>
                            <SidebarMenu>
                                <SidebarMenuItem>
                                    <SidebarMenuButton Tooltip="Profile">
                                        <LucideIcon Name="user" Size="16" />
                                        <span>Profile</span>
                                    </SidebarMenuButton>
                                </SidebarMenuItem>
                            </SidebarMenu>
                        </SidebarGroupContent>
                    </SidebarGroup>
                </SidebarContent>
                """;

        private const string _submenusCode = """
                <SidebarMenuItem>
                    <SidebarMenuButton Tooltip="Team">
                        <LucideIcon Name="users" Size="16" />
                        <span>Team</span>
                    </SidebarMenuButton>
                    <SidebarMenuSub>
                        <SidebarMenuSubItem>
                            <SidebarMenuSubButton><span>Engineering</span></SidebarMenuSubButton>
                        </SidebarMenuSubItem>
                        <SidebarMenuSubItem>
                            <SidebarMenuSubButton><span>Design</span></SidebarMenuSubButton>
                        </SidebarMenuSubItem>
                    </SidebarMenuSub>
                </SidebarMenuItem>
                """;

        private const string _floatingCode = """
                <SidebarProvider Variant="SidebarVariant.Floating" CookieKey="sidebar:floating" HeightClass="h-full">
                    <div class="flex h-full w-full relative">
                        <Sidebar Collapsible="true">
                            <!-- sidebar content -->
                        </Sidebar>
                        <SidebarInset>
                            <!-- main content -->
                        </SidebarInset>
                    </div>
                </SidebarProvider>
                """;

        private const string _insetCode = """
                <SidebarProvider Variant="SidebarVariant.Inset" CookieKey="sidebar:inset" HeightClass="h-full">
                    <div class="flex h-full w-full">
                        <Sidebar Collapsible="true">
                            <!-- sidebar content -->
                        </Sidebar>
                        <SidebarInset>
                            <!-- main content -->
                        </SidebarInset>
                    </div>
                </SidebarProvider>
                """;

        private const string _collapsibleIconsCode = """
                <!-- DefaultOpen="false" starts the sidebar collapsed to icon-only mode -->
                <SidebarProvider DefaultOpen="false" HeightClass="h-full">
                    <div class="flex h-full w-full">
                        <Sidebar Collapsible="true">
                            <SidebarContent>
                                <SidebarMenu>
                                    <SidebarMenuItem>
                                        <SidebarMenuButton Tooltip="Dashboard">
                                            <LucideIcon Name="layout-dashboard" Size="16" />
                                            <span>Dashboard</span>
                                        </SidebarMenuButton>
                                    </SidebarMenuItem>
                                </SidebarMenu>
                            </SidebarContent>
                        </Sidebar>
                        <SidebarInset>
                            <header class="flex h-16 items-center gap-2 border-b px-4">
                                <SidebarTrigger />
                            </header>
                        </SidebarInset>
                    </div>
                </SidebarProvider>
                """;

        private const string _treeViewCode = """
                <SidebarMenuItem>
                    <SidebarMenuButton Tooltip="src">
                        <LucideIcon Name="folder" Size="16" />
                        <span>src</span>
                    </SidebarMenuButton>
                    <SidebarMenuSub>
                        <SidebarMenuSubItem>
                            <SidebarMenuSubButton>
                                <LucideIcon Name="folder" Size="16" />
                                <span>components</span>
                            </SidebarMenuSubButton>
                        </SidebarMenuSubItem>
                        <SidebarMenuSubItem>
                            <SidebarMenuSubButton>
                                <LucideIcon Name="file-code" Size="16" />
                                <span>index.tsx</span>
                            </SidebarMenuSubButton>
                        </SidebarMenuSubItem>
                    </SidebarMenuSub>
                </SidebarMenuItem>
                """;

        private const string _stickyHeaderCode = """
                <SidebarProvider HeightClass="h-full">
                    <div class="flex h-full w-full flex-col">
                        <!-- Site Header -->
                        <header class="flex h-14 shrink-0 items-center gap-4 border-b bg-background px-6">
                            <span class="font-semibold">Company Name</span>
                        </header>
                        <!-- Sidebar and Content -->
                        <div class="flex flex-1 overflow-hidden">
                            <Sidebar Collapsible="true">
                                <SidebarContent><!-- menu items --></SidebarContent>
                            </Sidebar>
                            <div class="flex flex-1 flex-col">
                                <div class="flex h-16 items-center gap-2 border-b px-4">
                                    <SidebarTrigger />
                                </div>
                            </div>
                        </div>
                    </div>
                </SidebarProvider>
                """;

        private const string _headerDropdownCode = """
                <Sidebar Collapsible="true">
                    <SidebarHeader>
                        <ResponsiveHeaderDropdown @bind-SelectedVersion="selectedVersion" />
                    </SidebarHeader>
                    <SidebarContent>
                        <!-- menu groups -->
                    </SidebarContent>
                </Sidebar>
                """;

        private const string _collapsibleSubmenusCode = """
                <SidebarMenuItem>
                    <Collapsible>
                        <SidebarMenuButton Tooltip="Models">
                            <LucideIcon Name="box" Size="16" />
                            <span>Models</span>
                            <SidebarMenuChevron>
                                <LucideIcon Name="chevron-right" Size="16" />
                            </SidebarMenuChevron>
                        </SidebarMenuButton>
                        <CollapsibleContent>
                            <SidebarMenuSub>
                                <SidebarMenuSubItem>
                                    <SidebarMenuSubButton><span>Genesis</span></SidebarMenuSubButton>
                                </SidebarMenuSubItem>
                                <SidebarMenuSubItem>
                                    <SidebarMenuSubButton><span>Explorer</span></SidebarMenuSubButton>
                                </SidebarMenuSubItem>
                            </SidebarMenuSub>
                        </CollapsibleContent>
                    </Collapsible>
                </SidebarMenuItem>
                """;

        private const string _nestedMenuCode = """
                <!-- Level 1 collapsible -->
                <SidebarMenuItem>
                    <Collapsible>
                        <SidebarMenuButton Tooltip="Projects">
                            <LucideIcon Name="folder" Size="16" />
                            <span>Projects</span>
                            <SidebarMenuChevron><LucideIcon Name="chevron-right" Size="16" /></SidebarMenuChevron>
                        </SidebarMenuButton>
                        <CollapsibleContent>
                            <SidebarMenuSub>
                                <SidebarMenuSubItem>
                                    <SidebarMenuSubButton><span>Active Projects</span></SidebarMenuSubButton>
                                </SidebarMenuSubItem>
                                <!-- Level 2 collapsible -->
                                <SidebarMenuSubItem>
                                    <Collapsible>
                                        <SidebarMenuSubButton>
                                            <span>Project Settings</span>
                                            <SidebarMenuChevron><LucideIcon Name="chevron-right" Size="16" /></SidebarMenuChevron>
                                        </SidebarMenuSubButton>
                                        <CollapsibleContent>
                                            <SidebarMenuSub>
                                                <SidebarMenuSubItem>
                                                    <SidebarMenuSubButton><span>General Settings</span></SidebarMenuSubButton>
                                                </SidebarMenuSubItem>
                                                <SidebarMenuSubItem>
                                                    <SidebarMenuSubButton><span>Access Control</span></SidebarMenuSubButton>
                                                </SidebarMenuSubItem>
                                            </SidebarMenuSub>
                                        </CollapsibleContent>
                                    </Collapsible>
                                </SidebarMenuSubItem>
                            </SidebarMenuSub>
                        </CollapsibleContent>
                    </Collapsible>
                </SidebarMenuItem>
                """;
    }
}
