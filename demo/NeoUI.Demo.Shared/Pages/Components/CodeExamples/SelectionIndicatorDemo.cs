namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SelectionIndicatorDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _props =
        [
            new("Selector", "string",  "[data-state=active]", "CSS selector used to find the active element within the parent container."),
            new("Class",    "string?", null,                  "Additional CSS classes. Override to change variant, e.g. underline: \"bottom-0 top-auto h-0.5 rounded-none bg-foreground\"."),
        ];

        private const string _tabsCode =
            """
            @*
              Tip: extract the repeated Class string as a local const:
              private const string _triggerClass =
                  "relative z-10 data-[state=active]:bg-transparent data-[state=active]:shadow-none";
            *@

            <Tabs DefaultValue="overview">
                <TabsList>
                    <TabsTrigger Value="overview"
                                 Class="relative z-10 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
                        Overview
                    </TabsTrigger>
                    <TabsTrigger Value="analytics"
                                 Class="relative z-10 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
                        Analytics
                    </TabsTrigger>
                    <TabsTrigger Value="reports"
                                 Class="relative z-10 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
                        Reports
                    </TabsTrigger>
                    <TabsTrigger Value="settings"
                                 Class="relative z-10 data-[state=active]:bg-transparent data-[state=active]:shadow-none">
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
                <ToggleGroupItem Value="left"    Class="relative z-10 bg-transparent"><LucideIcon Name="align-left"    Class="h-4 w-4" /></ToggleGroupItem>
                <ToggleGroupItem Value="center"  Class="relative z-10 bg-transparent"><LucideIcon Name="align-center"  Class="h-4 w-4" /></ToggleGroupItem>
                <ToggleGroupItem Value="right"   Class="relative z-10 bg-transparent"><LucideIcon Name="align-right"   Class="h-4 w-4" /></ToggleGroupItem>
                <ToggleGroupItem Value="justify" Class="relative z-10 bg-transparent"><LucideIcon Name="align-justify" Class="h-4 w-4" /></ToggleGroupItem>
                <SelectionIndicator Selector="[aria-checked=true]" />
            </ToggleGroup>

            @code {
                private string? _alignment = "center";
            }
            """;

        private const string _navCode =
            """
            <div class="inline-flex items-stretch border-b border-border">
                @foreach (var item in _items)
                {
                    <button class="relative z-10 px-4 py-2 text-sm font-medium transition-colors
                                   @(_active == item ? "text-foreground" : "text-muted-foreground hover:text-foreground")"
                            data-active="@(_active == item ? "true" : null)"
                            @onclick="@(() => _active = item)">
                        @item
                    </button>
                }
                <SelectionIndicator
                    Selector="[data-active=true]"
                    Class="bottom-0 top-auto h-0.5 rounded-none bg-foreground" />
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
                    <button class="relative z-10 px-3 py-1 text-xs font-medium rounded-md transition-colors
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
