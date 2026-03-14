namespace NeoUI.Demo.Shared.Pages.Components;

partial class TimelineDemo
{
    private static readonly IReadOnlyList<DemoPropRow> _timelineProps =
    [
        new("Size",         "TimelineSize",         "Medium",  "Small, Medium, or Large — controls gap between items."),
        new("Align",        "TimelineAlign",        "Center",  "Center (date|icon|content), Left, Right, or Alternate."),
        new("ConnectorFit", "TimelineConnectorFit", "Spaced",  "Spaced = ring gap around icon; Connected = line touches icon."),
        new("Class",        "string?",              "null",    "Additional CSS classes."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _timelineItemProps =
    [
        new("Status",         "TimelineStatus",        "Completed","Completed, InProgress, or Pending."),
        new("ConnectorStyle", "TimelineConnectorStyle","Solid",    "Solid, Dashed, or Dotted for the connector below this item."),
        new("IsCollapsible",  "bool",                  "false",    "Whether the description content can be toggled open/closed."),
        new("DefaultOpen",    "bool",                  "true",     "Initial open state when IsCollapsible is true."),
        new("Title",          "string?",               "null",     "Shorthand — renders full content tree when ChildContent is null."),
        new("Time",           "string?",               "null",     "Shorthand time/date text."),
        new("Description",    "string?",               "null",     "Shorthand description text."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _timelineIconProps =
    [
        new("Color",       "TimelineColor",       "Primary", "Primary, Secondary, Muted, Accent, or Destructive."),
        new("Variant",     "TimelineIconVariant", "Solid",   "Solid (filled) or Outline."),
        new("Loading",     "bool",                "false",   "Pulse animation for in-flight state."),
        new("ChildContent","RenderFragment?",     "null",    "Custom icon content rendered inside the circle."),
    ];

    private static readonly IReadOnlyList<DemoPropRow> _timelineEmptyProps =
    [
        new("ChildContent", "RenderFragment?", "null", "Custom empty message. Falls back to \"No timeline items to display.\""),
        new("Class",        "string?",         "null", "Additional CSS classes."),
    ];

    private static class TimelineDemoCode
    {
        public const string Basic =
            """
            <Timeline Size="TimelineSize.Small">
                <TimelineItem Title="Repository created"  Time="Jan 2, 2024"  Status="TimelineStatus.Completed"
                              Description="Initialised with README, licence, and CI workflow." />
                <TimelineItem Title="First pull request"  Time="Jan 9, 2024"  Status="TimelineStatus.Completed"
                              Description="Core authentication module merged after review." />
                <TimelineItem Title="Beta deployment"     Time="Feb 14, 2024" Status="TimelineStatus.InProgress"
                              Description="Staging environment live — gathering early feedback." />
                <TimelineItem Title="Public launch"       Time="Planned"      Status="TimelineStatus.Pending"
                              Description="Target: end of Q1." ShowConnector="false" />
            </Timeline>
            """;

        public const string CustomIcons =
            """
            <Timeline>
                <TimelineItem Status="TimelineStatus.Completed">
                    <IconContent>
                        <TimelineIcon Color="TimelineColor.Primary">
                            <LucideIcon Name="rocket" Size="16" />
                        </TimelineIcon>
                    </IconContent>
                    <ChildContent>
                        <TimelineContent>
                            <TimelineHeader>
                                <TimelineTitle>v1.0 released</TimelineTitle>
                                <TimelineTime>Mar 1</TimelineTime>
                            </TimelineHeader>
                            <TimelineDescription>Initial public release shipped to production.</TimelineDescription>
                        </TimelineContent>
                    </ChildContent>
                </TimelineItem>
                <TimelineItem Status="TimelineStatus.InProgress">
                    <IconContent>
                        <TimelineIcon Color="TimelineColor.Accent">
                            <LucideIcon Name="chart-line" Size="16" />
                        </TimelineIcon>
                    </IconContent>
                    <ChildContent>
                        <TimelineContent>
                            <TimelineHeader>
                                <TimelineTitle>50 k users</TimelineTitle>
                                <TimelineTime>Apr 22</TimelineTime>
                            </TimelineHeader>
                            <TimelineDescription>Monthly active users crossing the 50 000 mark.</TimelineDescription>
                        </TimelineContent>
                    </ChildContent>
                </TimelineItem>
            </Timeline>
            """;

        public const string Alignment =
            """
            @* Left *@
            <Timeline Align="TimelineAlign.Left">
                <TimelineItem Status="TimelineStatus.Completed">
                    <IconContent><TimelineIcon Color="TimelineColor.Primary"><LucideIcon Name="pencil-ruler" Size="14" /></TimelineIcon></IconContent>
                    <ChildContent><TimelineContent><TimelineTitle>Design</TimelineTitle><TimelineDescription>Wireframes approved.</TimelineDescription></TimelineContent></ChildContent>
                </TimelineItem>
                <TimelineItem Status="TimelineStatus.Pending" ShowConnector="false">
                    <IconContent><TimelineIcon Color="TimelineColor.Muted"><LucideIcon Name="rocket" Size="14" /></TimelineIcon></IconContent>
                    <ChildContent><TimelineContent><TimelineTitle>Launch</TimelineTitle><TimelineDescription>Scheduled for next sprint.</TimelineDescription></TimelineContent></ChildContent>
                </TimelineItem>
            </Timeline>

            @* Right — same structure with Align="TimelineAlign.Right" *@
            """;

        public const string Alternate =
            """
            <Timeline Align="TimelineAlign.Alternate">
                <TimelineItem Status="TimelineStatus.Completed">
                    <IconContent><TimelineIcon Color="TimelineColor.Primary"><LucideIcon Name="flag" Size="16" /></TimelineIcon></IconContent>
                    <ChildContent>
                        <TimelineContent>
                            <TimelineHeader><TimelineTitle>Project kick-off</TimelineTitle><TimelineTime>Week 1</TimelineTime></TimelineHeader>
                            <TimelineDescription>Team onboarded and milestones agreed.</TimelineDescription>
                        </TimelineContent>
                    </ChildContent>
                </TimelineItem>
                <TimelineItem Status="TimelineStatus.Pending" ShowConnector="false">
                    <IconContent><TimelineIcon Color="TimelineColor.Muted"><LucideIcon Name="package-check" Size="16" /></TimelineIcon></IconContent>
                    <ChildContent>
                        <TimelineContent>
                            <TimelineHeader><TimelineTitle>Release</TimelineTitle><TimelineTime>Week 7</TimelineTime></TimelineHeader>
                            <TimelineDescription>Planned ship date.</TimelineDescription>
                        </TimelineContent>
                    </ChildContent>
                </TimelineItem>
            </Timeline>
            """;

        public const string IconVariants =
            """
            @* Solid (default) *@
            <TimelineItem Status="TimelineStatus.Completed">
                <IconContent>
                    <TimelineIcon Color="TimelineColor.Primary" Variant="TimelineIconVariant.Solid">
                        <LucideIcon Name="check" Size="14" />
                    </TimelineIcon>
                </IconContent>
                <ChildContent><TimelineContent><TimelineTitle>Deployed</TimelineTitle></TimelineContent></ChildContent>
            </TimelineItem>

            @* Outline *@
            <TimelineItem Status="TimelineStatus.Completed">
                <IconContent>
                    <TimelineIcon Color="TimelineColor.Primary" Variant="TimelineIconVariant.Outline">
                        <LucideIcon Name="check" Size="14" />
                    </TimelineIcon>
                </IconContent>
                <ChildContent><TimelineContent><TimelineTitle>Deployed</TimelineTitle></TimelineContent></ChildContent>
            </TimelineItem>
            """;

        public const string Sizes =
            """
            <Timeline Size="TimelineSize.Small"  Align="TimelineAlign.Left"> ... </Timeline>
            <Timeline Size="TimelineSize.Medium" Align="TimelineAlign.Left"> ... </Timeline>
            <Timeline Size="TimelineSize.Large"  Align="TimelineAlign.Left">
                <TimelineItem Status="TimelineStatus.Completed">
                    <IconContent>
                        <TimelineIcon Size="TimelineSize.Large" Color="TimelineColor.Primary">
                            <LucideIcon Name="package" Size="18" />
                        </TimelineIcon>
                    </IconContent>
                    <ChildContent><TimelineContent><TimelineTitle>Shipped</TimelineTitle></TimelineContent></ChildContent>
                </TimelineItem>
            </Timeline>
            """;

        public const string ConnectorStyles =
            """
            <Timeline Align="TimelineAlign.Left">
                <TimelineItem Title="Booked"    Status="TimelineStatus.Completed" ConnectorStyle="TimelineConnectorStyle.Dashed" />
                <TimelineItem Title="Confirmed" Status="TimelineStatus.Completed" ConnectorStyle="TimelineConnectorStyle.Dashed" />
                <TimelineItem Title="Pending"   Status="TimelineStatus.Pending"   ConnectorStyle="TimelineConnectorStyle.Dashed" ShowConnector="false" />
            </Timeline>
            """;

        public const string ConnectorSpacing =
            """
            <Timeline Align="TimelineAlign.Left">
                <TimelineItem Title="Event A" Status="TimelineStatus.Completed" ConnectorClass="min-h-12" />
                <TimelineItem Title="Event B" Status="TimelineStatus.Completed" ConnectorClass="min-h-12" />
                <TimelineItem Title="Event C" Status="TimelineStatus.Pending"   ShowConnector="false" />
            </Timeline>
            """;

        public const string ConnectedFit =
            """
            <Timeline ConnectorFit="TimelineConnectorFit.Connected" Align="TimelineAlign.Left">
                <TimelineItem Status="TimelineStatus.Completed">
                    <IconContent><TimelineIcon Color="TimelineColor.Primary"><LucideIcon Name="shopping-cart" Size="14" /></TimelineIcon></IconContent>
                    <ChildContent>
                        <TimelineContent>
                            <TimelineHeader><TimelineTitle>Order placed</TimelineTitle><TimelineTime>09:00</TimelineTime></TimelineHeader>
                        </TimelineContent>
                    </ChildContent>
                </TimelineItem>
                <TimelineItem Status="TimelineStatus.Pending" ShowConnector="false">
                    <IconContent><TimelineIcon Color="TimelineColor.Muted"><LucideIcon Name="truck" Size="14" /></TimelineIcon></IconContent>
                    <ChildContent>
                        <TimelineContent>
                            <TimelineHeader><TimelineTitle>Dispatched</TimelineTitle><TimelineTime>—</TimelineTime></TimelineHeader>
                        </TimelineContent>
                    </ChildContent>
                </TimelineItem>
            </Timeline>
            """;

        public const string Collapsible =
            """
            <Timeline>
                <TimelineItem Status="TimelineStatus.Completed" IsCollapsible="true" DefaultOpen="false">
                    <IconContent>
                        <TimelineIcon Color="TimelineColor.Primary">
                            <LucideIcon Name="git-commit-horizontal" Size="16" />
                        </TimelineIcon>
                    </IconContent>
                    <ChildContent>
                        @* Always-visible trigger *@
                        <TimelineContent>
                            <TimelineHeader>
                                <TimelineTitle>feat(auth): add OAuth2 provider — abc1234</TimelineTitle>
                                <TimelineTime>2 days ago</TimelineTime>
                            </TimelineHeader>
                        </TimelineContent>
                    </ChildContent>
                    <DetailContent>
                        @* Expandable body *@
                        <TimelineContent Class="mt-1">
                            <TimelineDescription>Added Google and GitHub OAuth2 sign-in with PKCE flow.</TimelineDescription>
                        </TimelineContent>
                    </DetailContent>
                </TimelineItem>
            </Timeline>
            """;

        public const string CollapsibleDetail =
            """
            <Timeline Align="TimelineAlign.Left">
                <TimelineItem Status="TimelineStatus.Completed" IsCollapsible="true">
                    <IconContent>
                        <TimelineIcon Color="TimelineColor.Primary">
                            <LucideIcon Name="tag" Size="16" />
                        </TimelineIcon>
                    </IconContent>
                    <ChildContent>
                        <TimelineContent>
                            <TimelineHeader>
                                <TimelineTitle>v2.4.0 — Component library refresh</TimelineTitle>
                                <TimelineTime>May 10, 2024</TimelineTime>
                            </TimelineHeader>
                            <TimelineDescription>Major visual overhaul across 18 components.</TimelineDescription>
                        </TimelineContent>
                    </ChildContent>
                    <DetailContent>
                        <TimelineContent Class="mt-2 border rounded-md p-4 text-sm space-y-2 bg-muted/30">
                            <p class="font-semibold">What's new</p>
                            <ul class="list-disc list-inside space-y-1 text-muted-foreground">
                                <li>DataView component with virtualisation and selection</li>
                                <li>SplitButton with accessible keyboard handling</li>
                            </ul>
                        </TimelineContent>
                    </DetailContent>
                </TimelineItem>
            </Timeline>
            """;

        public const string EmptyState =
            """
            <Timeline>
                <TimelineEmpty />
            </Timeline>

            <Timeline>
                <TimelineEmpty>No activity yet — events will appear here as they occur.</TimelineEmpty>
            </Timeline>
            """;
    }
}
