namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class TooltipPrimitiveDemo
    {
        private const string _alignCode =
        """
        <TooltipContentPrimitive Side="@PopoverSide.Top" Align="@PopoverAlign.Start">Aligned to start</TooltipContentPrimitive>
        <TooltipContentPrimitive Side="@PopoverSide.Top" Align="@PopoverAlign.Center">Centered (default)</TooltipContentPrimitive>
        <TooltipContentPrimitive Side="@PopoverSide.Top" Align="@PopoverAlign.End">Aligned to end</TooltipContentPrimitive>
        """;

        private const string _asChildCode =
        """
        <!-- Without AsChild -->
        <TooltipPrimitive>
            <TooltipTriggerPrimitive>Hover me</TooltipTriggerPrimitive>
            <TooltipContentPrimitive>Without AsChild tooltip</TooltipContentPrimitive>
        </TooltipPrimitive>

        <!-- With AsChild -->
        <TooltipPrimitive>
            <TooltipTriggerPrimitive AsChild>
                <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Icon">
                    <LucideIcon Name="info" Size="16" />
                </Button>
            </TooltipTriggerPrimitive>
            <TooltipContentPrimitive>With AsChild tooltip</TooltipContentPrimitive>
        </TooltipPrimitive>
        """;

        private const string _basicCode =
        """
        <TooltipPrimitive>
            <TooltipTriggerPrimitive>Hover me</TooltipTriggerPrimitive>
            <TooltipContentPrimitive>This is a tooltip</TooltipContentPrimitive>
        </TooltipPrimitive>
        """;

        private const string _controlledCode =
        """
        <TooltipPrimitive @bind-Open="isOpen">
            <TooltipTriggerPrimitive>Controlled TooltipPrimitive</TooltipTriggerPrimitive>
            <TooltipContentPrimitive>This tooltip is controlled externally</TooltipContentPrimitive>
        </TooltipPrimitive>
        """;

        private const string _delayCode =
        """
        <TooltipPrimitive DelayDuration="0">
            <TooltipTriggerPrimitive>Instant (0ms)</TooltipTriggerPrimitive>
            <TooltipContentPrimitive>Shows immediately</TooltipContentPrimitive>
        </TooltipPrimitive>

        <TooltipPrimitive DelayDuration="700">
            <TooltipTriggerPrimitive>Default (700ms)</TooltipTriggerPrimitive>
            <TooltipContentPrimitive>Shows after 700ms</TooltipContentPrimitive>
        </TooltipPrimitive>

        <TooltipPrimitive DelayDuration="1500">
            <TooltipTriggerPrimitive>Slow (1500ms)</TooltipTriggerPrimitive>
            <TooltipContentPrimitive>Shows after 1.5 seconds</TooltipContentPrimitive>
        </TooltipPrimitive>
        """;

        private const string _positioningCode =
        """
        <TooltipContentPrimitive Side="@PopoverSide.Top">Top positioned</TooltipContentPrimitive>
        <TooltipContentPrimitive Side="@PopoverSide.Bottom">Bottom positioned</TooltipContentPrimitive>
        <TooltipContentPrimitive Side="@PopoverSide.Left">Left positioned</TooltipContentPrimitive>
        <TooltipContentPrimitive Side="@PopoverSide.Right">Right positioned</TooltipContentPrimitive>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _tooltipProps =
    [
        new("Open", "bool?", null, "Controls whether the tooltip is open (controlled mode)."),
        new("OpenChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when open state changes."),
        new("DelayDuration", "int", "700", "Delay in milliseconds before showing the tooltip."),
        new("Side", "PopoverSide", "Top", "Preferred side for tooltip placement."),
        new("Align", "PopoverAlign", "Center", "Alignment relative to the trigger."),
    ];
    }
}
