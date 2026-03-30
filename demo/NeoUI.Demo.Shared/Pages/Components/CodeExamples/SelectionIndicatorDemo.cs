namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SelectionIndicatorDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _props =
        [
            new("Selector", "string",  "[data-state=active]", "CSS selector used to find the active element within the parent container."),
            new("Hover",    "bool",    "false",               "When true, the indicator also slides to the hovered item and snaps back on mouse leave."),
            new("Class",    "string?", null,                  "Additional CSS classes. Use --si-height for underlines. Use data-[si-hover]: variants to style the hover state differently from the selected state."),
        ];

        private const string _tabsCode =
            """
            @*
              Tip: extract the repeated Class string as a local const:
              private const string _triggerClass =
                  "relative z-1 data-[state=active]:bg-transparent data-[state=active]:shadow-none";
            *@

            <Tabs DefaultValue="overview">
                <TabsList>
                    <TabsTrigger Value="overview"
                                 Class="relative z-1 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
                        Overview
                    </TabsTrigger>
                    <TabsTrigger Value="analytics"
                                 Class="relative z-1 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
                        Analytics
                    </TabsTrigger>
                    <TabsTrigger Value="reports"
                                 Class="relative z-1 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
                        Reports
                    </TabsTrigger>
                    <TabsTrigger Value="settings"
                                 Class="relative z-1 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
                        Settings
                    </TabsTrigger>
                    <SelectionIndicator />
                </TabsList>
                <TabsContent Value="overview">Overview</TabsContent>
                <TabsContent Value="analytics">Analytics</TabsContent>
                <TabsContent Value="reports">Reports</TabsContent>
                <TabsContent Value="settings">Settings</TabsContent>
            </Tabs>
            """;

        private const string _segmentedCode =
            """
            <ToggleGroup Type="ToggleGroupType.Single"
                         @bind-Value="@_alignment"
                         Class="bg-muted p-1 rounded-md gap-0">
                <ToggleGroupItem Value="left"    Class="relative z-1 bg-transparent"><LucideIcon Name="align-left"    Class="h-4 w-4" /></ToggleGroupItem>
                <ToggleGroupItem Value="center"  Class="relative z-1 bg-transparent"><LucideIcon Name="align-center"  Class="h-4 w-4" /></ToggleGroupItem>
                <ToggleGroupItem Value="right"   Class="relative z-1 bg-transparent"><LucideIcon Name="align-right"   Class="h-4 w-4" /></ToggleGroupItem>
                <ToggleGroupItem Value="justify" Class="relative z-1 bg-transparent"><LucideIcon Name="align-justify" Class="h-4 w-4" /></ToggleGroupItem>
                <SelectionIndicator Selector="[aria-checked=true]" />
            </ToggleGroup>

            @code {
                private string? _alignment = "center";
            }
            """;

        private const string _dropdownCode =
            """
            <DropdownMenu>
                <DropdownMenuTrigger AsChild="true">
                    <Button Variant="ButtonVariant.Outline">View as @_view</Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent Class="w-40">
                    <DropdownMenuLabel>View As</DropdownMenuLabel>
                    <DropdownMenuSeparator />
                    <DropdownMenuRadioGroup @bind-Value="@_view">
                        @* hover:bg-transparent lets the SelectionIndicator be the sole hover visual *@
                        <DropdownMenuRadioItem Value="List"    CloseOnSelect="false" Class="relative z-1 hover:bg-transparent focus:bg-transparent">List</DropdownMenuRadioItem>
                        <DropdownMenuRadioItem Value="Grid"    CloseOnSelect="false" Class="relative z-1 hover:bg-transparent focus:bg-transparent">Grid</DropdownMenuRadioItem>
                        <DropdownMenuRadioItem Value="Columns" CloseOnSelect="false" Class="relative z-1 hover:bg-transparent focus:bg-transparent">Columns</DropdownMenuRadioItem>
                        <SelectionIndicator Selector="[data-state=checked]"
                                            Hover="true"
                                            Class="inset-x-1 bg-accent data-[si-hover]:bg-accent/50 rounded-sm shadow-none" />
                    </DropdownMenuRadioGroup>
                </DropdownMenuContent>
            </DropdownMenu>

            @code {
                private string _view = "List";
            }
            """;

        private const string _paginationCode =
            """
            @* Suppress PaginationLink's own active background so the indicator shows through *@

            <div class="inline-flex items-center bg-muted p-1 rounded-lg gap-0">
                @for (var i = 1; i <= 5; i++)
                {
                    var page = i;
                    <PaginationLink IsActive="@(_page == page)"
                                    OnClick="@(() => _page = page)"
                                    Class="relative z-1 border-transparent bg-transparent hover:bg-transparent w-9 h-9">
                        @page
                    </PaginationLink>
                }
                <SelectionIndicator Selector="[aria-current=page]" />
            </div>

            @code {
                private int _page = 1;
            }
            """;


        private const string _navCode =
            """
            <div class="inline-flex items-stretch border-b border-border">
                @foreach (var item in _items)
                {
                    <button class="relative z-1 px-4 py-2 text-sm font-medium transition-colors
                                   @(_active == item ? "text-foreground" : "text-muted-foreground hover:text-foreground")"
                            data-active="@(_active == item ? "true" : null)"
                            @onclick="@(() => _active = item)">
                        @item
                    </button>
                }
                <SelectionIndicator
                    Selector="[data-active=true]"
                    Class="rounded-none bg-foreground [--si-height:2px]" />
            </div>

            @code {
                private static readonly string[] _items = ["Overview", "Changelog", "Examples", "API"];
                private string _active = "Overview";
            }
            """;

        private const string _customCode =
            """
            @* Works with any HTML container — no NeoUI component required.
               Just set data-active="true" on the active element. *@

            <div class="inline-flex bg-muted rounded-lg p-1">
                @foreach (var period in new[] { "1D", "7D", "1M", "3M", "1Y", "All" })
                {
                    <button class="relative z-1 px-3 py-1 text-xs font-medium rounded-md transition-colors
                                   @(_period == period ? "text-foreground" : "text-muted-foreground hover:text-foreground")"
                            data-active="@(_period == period ? "true" : null)"
                            @onclick="@(() => _period = period)">
                        @period
                    </button>
                }
                <SelectionIndicator Selector="[data-active=true]" />
            </div>

            @code {
                private string _period = "1M";
            }
            """;
    }
}
