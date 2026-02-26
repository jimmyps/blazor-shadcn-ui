namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class TooltipDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _tooltipAllProps =
            [
                // TooltipProvider
                new("DelayDuration",     "int",              "700",   "<strong>TooltipProvider</strong> — Duration in ms before tooltips appear."),
                new("SkipDelayDuration", "int",              "300",   "<strong>TooltipProvider</strong> — Duration before showing subsequent tooltips without delay."),
                // Tooltip
                new("Placement",         "PopoverPlacement", "Top",   "<strong>Tooltip</strong> — Tooltip position: Top, Bottom, Left, Right, and variants."),
                // TooltipTrigger
                new("AsChild",           "bool",             "false", "<strong>TooltipTrigger</strong> — When true, passes trigger behavior to the child component instead of wrapping it."),
                new("Focusable",         "bool",             "true",  "<strong>TooltipTrigger</strong> — Whether to add tabindex=\"0\" for keyboard focus."),
                new("Class",             "string?",          null,    "<strong>TooltipTrigger / TooltipContent</strong> — Additional CSS classes."),
            ];

        private const string _asChildCode =
                """
                <!-- Without AsChild: TooltipTrigger renders its own span -->
                <Tooltip>
                    <TooltipTrigger Class="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-accent cursor-pointer">
                        Hover me
                    </TooltipTrigger>
                    <TooltipContent>
                        <p>Tooltip with inline classes</p>
                    </TooltipContent>
                </Tooltip>

                <!-- With AsChild: Button receives hover/focus behavior -->
                <Tooltip>
                    <TooltipTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Icon">
                            <LucideIcon Name="info" Size="16" />
                        </Button>
                    </TooltipTrigger>
                    <TooltipContent>Additional information</TooltipContent>
                </Tooltip>
                """;

        private const string _basicCode =
                """
                <Tooltip>
                    <TooltipTrigger Class="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium hover:bg-accent cursor-pointer">
                        Hover me
                    </TooltipTrigger>
                    <TooltipContent>
                        <p>This is a tooltip</p>
                    </TooltipContent>
                </Tooltip>
                """;

        private const string _placementsCode =
                """
                <Tooltip Placement="@PopoverPlacement.Top">
                    <TooltipTrigger ...>Top</TooltipTrigger>
                    <TooltipContent>Tooltip on top</TooltipContent>
                </Tooltip>

                <Tooltip Placement="@PopoverPlacement.Bottom">
                    <TooltipTrigger ...>Bottom</TooltipTrigger>
                    <TooltipContent>Tooltip on bottom</TooltipContent>
                </Tooltip>

                <Tooltip Placement="@PopoverPlacement.Left">
                    <TooltipTrigger ...>Left</TooltipTrigger>
                    <TooltipContent>Tooltip on left</TooltipContent>
                </Tooltip>

                <Tooltip Placement="@PopoverPlacement.Right">
                    <TooltipTrigger ...>Right</TooltipTrigger>
                    <TooltipContent>Tooltip on right</TooltipContent>
                </Tooltip>
                """;

        private const string _iconTooltipsCode =
                """
                <div class="flex items-center gap-2">
                    <span>Username</span>
                    <Tooltip>
                        <TooltipTrigger>
                            <LucideIcon Name="info" Size="16" Class="cursor-help text-muted-foreground" />
                        </TooltipTrigger>
                        <TooltipContent>Choose a unique username for your account</TooltipContent>
                    </Tooltip>
                </div>
                """;

        private const string _toolbarCode =
                """
                <div class="border rounded-lg p-2 inline-flex gap-1">
                    <Tooltip>
                        <TooltipTrigger Class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-accent cursor-pointer">
                            <LucideIcon Name="bold" Size="16" />
                        </TooltipTrigger>
                        <TooltipContent>Bold (Ctrl+B)</TooltipContent>
                    </Tooltip>
                    <Tooltip>
                        <TooltipTrigger Class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-accent cursor-pointer">
                            <LucideIcon Name="italic" Size="16" />
                        </TooltipTrigger>
                        <TooltipContent>Italic (Ctrl+I)</TooltipContent>
                    </Tooltip>
                </div>
                """;

        private const string _richContentCode =
                """
                <Tooltip>
                    <TooltipTrigger Class="...">
                        <LucideIcon Name="package" Size="16" Class="mr-2" />
                        Premium Plan
                    </TooltipTrigger>
                    <TooltipContent Class="max-w-xs">
                        <div class="space-y-1">
                            <p class="font-semibold">Premium Features</p>
                            <ul class="text-xs space-y-0.5 text-muted-foreground">
                                <li>• Unlimited projects</li>
                                <li>• Priority support</li>
                                <li>• Advanced analytics</li>
                            </ul>
                        </div>
                    </TooltipContent>
                </Tooltip>
                """;
    }
}
