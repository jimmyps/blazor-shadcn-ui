namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class HoverCardPrimitiveDemo
    {
        private const string _asChildCode =
        """
        <HoverCardPrimitive>
            <HoverCardTriggerPrimitive AsChild>
                <Button Variant="ButtonVariant.Link">@@johndoe</Button>
            </HoverCardTriggerPrimitive>
            <HoverCardContentPrimitive class="bg-popover text-popover-foreground border border-border rounded-lg p-4 shadow-lg w-[250px]">
                <div class="flex gap-3">
                    <div class="w-10 h-10 rounded-full bg-gradient-to-br from-purple-500 to-purple-700"></div>
                    <div>
                        <div class="font-semibold">John Doe</div>
                        <p class="text-xs text-muted-foreground">Software Developer</p>
                    </div>
                </div>
            </HoverCardContentPrimitive>
        </HoverCardPrimitive>
        """;

        private const string _basicCode =
        """
        <HoverCardPrimitive>
            <HoverCardTriggerPrimitive>Hover me</HoverCardTriggerPrimitive>
            <HoverCardContentPrimitive>Hover Card Content</HoverCardContentPrimitive>
        </HoverCardPrimitive>
        """;

        private const string _delayCode =
        """
        <HoverCardPrimitive OpenDelay="200" CloseDelay="100">
            <HoverCardTriggerPrimitive>Quick (200ms)</HoverCardTriggerPrimitive>
            <HoverCardContentPrimitive>Opens quickly with 200ms delay.</HoverCardContentPrimitive>
        </HoverCardPrimitive>

        <HoverCardPrimitive OpenDelay="1000" CloseDelay="500">
            <HoverCardTriggerPrimitive>Slow (1000ms)</HoverCardTriggerPrimitive>
            <HoverCardContentPrimitive>Opens slowly with 1000ms delay.</HoverCardContentPrimitive>
        </HoverCardPrimitive>
        """;

        private const string _positionCode =
        """
        <HoverCardContentPrimitive Side="@PopoverSide.Top">Top</HoverCardContentPrimitive>
        <HoverCardContentPrimitive Side="@PopoverSide.Right">Right</HoverCardContentPrimitive>
        """;
    }
}
