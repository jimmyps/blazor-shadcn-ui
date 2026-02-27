namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class TabsPrimitiveDemo
    {
        private const string _basicCode =
        """
        <TabsPrimitive DefaultValue="tab1">
            <TabsListPrimitive>
                <TabsTriggerPrimitive Value="tab1">Tab 1</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="tab2">Tab 2</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="tab3">Tab 3</TabsTriggerPrimitive>
            </TabsListPrimitive>
            <TabsContentPrimitive Value="tab1">Tab 1 Content</TabsContentPrimitive>
            <TabsContentPrimitive Value="tab2">Tab 2 Content</TabsContentPrimitive>
            <TabsContentPrimitive Value="tab3">Tab 3 Content</TabsContentPrimitive>
        </TabsPrimitive>
        """;

        private const string _controlledCode =
        """
        <TabsPrimitive @bind-Value="controlledValue">
            <TabsListPrimitive>
                <TabsTriggerPrimitive Value="account">Account</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="password">Password</TabsTriggerPrimitive>
            </TabsListPrimitive>
            <TabsContentPrimitive Value="account">Account Settings</TabsContentPrimitive>
            <TabsContentPrimitive Value="password">Password Settings</TabsContentPrimitive>
        </TabsPrimitive>
        """;

        private const string _disabledCode =
        """
        <TabsPrimitive DefaultValue="enabled1">
            <TabsListPrimitive>
                <TabsTriggerPrimitive Value="enabled1">Enabled 1</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="disabled" Disabled="true">Disabled</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="enabled2">Enabled 2</TabsTriggerPrimitive>
            </TabsListPrimitive>
        </TabsPrimitive>
        """;

        private const string _manualCode =
        """
        <TabsPrimitive DefaultValue="overview" ActivationMode="TabsActivationMode.Manual">
            <TabsListPrimitive>
                <TabsTriggerPrimitive Value="overview">Overview</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="analytics">Analytics</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="reports">Reports</TabsTriggerPrimitive>
            </TabsListPrimitive>
        </TabsPrimitive>
        """;

        private const string _verticalCode =
        """
        <TabsPrimitive DefaultValue="general" Orientation="TabsOrientation.Vertical">
            <TabsListPrimitive>
                <TabsTriggerPrimitive Value="general">General</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="security">Security</TabsTriggerPrimitive>
                <TabsTriggerPrimitive Value="notifications">Notifications</TabsTriggerPrimitive>
            </TabsListPrimitive>
        </TabsPrimitive>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _tabsProps =
    [
        new("Value", "string?", null, "Active tab value (controlled mode)."),
        new("ValueChanged", "EventCallback&lt;string&gt;", null, "Callback invoked when active tab changes."),
        new("DefaultValue", "string?", null, "Initial active tab (uncontrolled mode)."),
        new("Orientation", "TabsOrientation", "Horizontal", "Layout orientation for keyboard navigation semantics."),
        new("ActivationMode", "TabsActivationMode", "Automatic", "Whether focus auto-activates tabs or requires Enter/Space."),
        new("Disabled", "bool", "false", "Disables an individual tab trigger."),
    ];
    }
}
