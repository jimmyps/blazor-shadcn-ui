namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class HoverCardDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _hoverCardProps =
            [
                new("OpenDelay",    "int",             "700",  "Delay in milliseconds before the card opens on hover."),
                new("CloseDelay",   "int",             "300",  "Delay in milliseconds before the card closes."),
                new("Side",         "string?",         null,   "The preferred side to show the card. Options: top, right, bottom, left."),
                new("Align",        "string?",         null,   "Alignment relative to the trigger. Options: start, center, end."),
                new("ChildContent", "RenderFragment?", null,   "Nest HoverCardTrigger and HoverCardContent here."),
            ];

        private const string _defaultCode =
                """
                <HoverCard>
                    <HoverCardTrigger Class="underline">@nextjs</HoverCardTrigger>
                    <HoverCardContent>
                        <h4 class="text-sm font-semibold">@nextjs</h4>
                        <p class="text-sm text-muted-foreground">The React Framework by @vercel.</p>
                    </HoverCardContent>
                </HoverCard>
                """;

        private const string _asChildCode =
                """
                <!-- Without AsChild: HoverCardTrigger wraps content in its own element -->
                <HoverCard>
                    <HoverCardTrigger Class="underline cursor-pointer">@username</HoverCardTrigger>
                    <HoverCardContent><p class="text-sm">User profile info</p></HoverCardContent>
                </HoverCard>

                <!-- With AsChild: trigger behavior applied to the child element -->
                <HoverCard>
                    <HoverCardTrigger AsChild>
                        <Button Variant="ButtonVariant.Link">@johndoe</Button>
                    </HoverCardTrigger>
                    <HoverCardContent><p class="text-sm">Profile card</p></HoverCardContent>
                </HoverCard>
                """;

        private const string _userProfileCode =
                """
                <HoverCard>
                    <HoverCardTrigger Class="font-semibold text-primary hover:underline cursor-pointer">
                        @johndoe
                    </HoverCardTrigger>
                    <HoverCardContent>
                        <h4 class="text-sm font-semibold">John Doe</h4>
                        <p class="text-sm text-muted-foreground">Full-stack developer.</p>
                    </HoverCardContent>
                </HoverCard>
                """;

        private const string _definitionCode =
                """
                <HoverCard OpenDelay="200">
                    <HoverCardTrigger Class="border-b border-dashed border-primary cursor-help">Floating UI</HoverCardTrigger>
                    <HoverCardContent>
                        <h4 class="text-sm font-semibold">Floating UI</h4>
                        <p class="text-sm text-muted-foreground">Smart positioning library with collision detection.</p>
                    </HoverCardContent>
                </HoverCard>
                """;

        private const string _productCode =
                """
                <HoverCard OpenDelay="300">
                    <HoverCardTrigger Class="inline-flex items-center justify-center rounded-md border h-10 px-4 py-2">
                        View Product
                    </HoverCardTrigger>
                    <HoverCardContent>
                        <div class="h-32 w-full rounded-md bg-gradient-to-br from-orange-400 to-rose-400"></div>
                        <h4 class="text-sm font-semibold mt-2">Premium Headphones</h4>
                        <span class="text-lg font-bold">$299.99</span>
                    </HoverCardContent>
                </HoverCard>
                """;
    }
}
