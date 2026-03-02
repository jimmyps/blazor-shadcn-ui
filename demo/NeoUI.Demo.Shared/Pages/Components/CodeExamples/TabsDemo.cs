namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class TabsDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _tabsProps =
            [
                new("DefaultValue", "string?",         null,  "The value of the tab that should be active by default (uncontrolled)."),
                new("Value",        "string?",         null,  "The controlled value of the active tab."),
                new("Class",        "string?",         null,  "Additional CSS classes appended to the root element."),
                new("ChildContent", "RenderFragment?", null,  "Nest TabsList and TabsContent here."),
            ];

        private const string _defaultCode =
                """
                <Tabs DefaultValue="account">
                    <TabsList Class="grid w-full grid-cols-2">
                        <TabsTrigger Value="account">Account</TabsTrigger>
                        <TabsTrigger Value="password">Password</TabsTrigger>
                    </TabsList>
                    <TabsContent Value="account">
                        <p class="text-sm text-muted-foreground">Account settings go here.</p>
                    </TabsContent>
                    <TabsContent Value="password">
                        <p class="text-sm text-muted-foreground">Password settings go here.</p>
                    </TabsContent>
                </Tabs>
                """;

        private const string _multipleCode =
                """
                <Tabs DefaultValue="overview">
                    <TabsList>
                        <TabsTrigger Value="overview">Overview</TabsTrigger>
                        <TabsTrigger Value="analytics">Analytics</TabsTrigger>
                        <TabsTrigger Value="reports">Reports</TabsTrigger>
                    </TabsList>
                    <TabsContent Value="overview">Overview content</TabsContent>
                    <TabsContent Value="analytics">Analytics content</TabsContent>
                    <TabsContent Value="reports">Reports content</TabsContent>
                </Tabs>
                """;

        private const string _controlledCode =
                """
                <Tabs @bind-Value="activeTab">
                    <TabsList Class="grid w-full grid-cols-3">
                        <TabsTrigger Value="morning">Morning</TabsTrigger>
                        <TabsTrigger Value="afternoon">Afternoon</TabsTrigger>
                        <TabsTrigger Value="evening">Evening</TabsTrigger>
                    </TabsList>
                    <TabsContent Value="morning">Morning content</TabsContent>
                    <TabsContent Value="afternoon">Afternoon content</TabsContent>
                    <TabsContent Value="evening">Evening content</TabsContent>
                </Tabs>

                @code {
                    private string? activeTab = "morning";
                }
                """;
    }
}
