namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class PopoverPrimitiveDemo
    {
        private const string _asChildCode =
        """
        <PopoverPrimitive>
            <PopoverTriggerPrimitive AsChild>
                <Button Variant="ButtonVariant.Outline">Open PopoverPrimitive</Button>
            </PopoverTriggerPrimitive>
            <PopoverContentPrimitive>
                <p>With AsChild content</p>
            </PopoverContentPrimitive>
        </PopoverPrimitive>
        """;

        private const string _basicCode =
        """
        <PopoverPrimitive>
            <PopoverTriggerPrimitive>Open PopoverPrimitive</PopoverTriggerPrimitive>
            <PopoverContentPrimitive>
                <p>This is a headless popover primitive.</p>
            </PopoverContentPrimitive>
        </PopoverPrimitive>
        """;

        private const string _controlledCode =
        """
        <PopoverPrimitive @bind-Open="isOpen">
            <PopoverTriggerPrimitive>@(isOpen ? "Close" : "Open") Controlled</PopoverTriggerPrimitive>
            <PopoverContentPrimitive>
                <p>Controlled by parent state: @isOpen</p>
            </PopoverContentPrimitive>
        </PopoverPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Click / Enter</Kbd>
        <Kbd>Escape</Kbd>
        <Kbd>Tab</Kbd>
        """;

        private const string _positioningCode =
        """
        <PopoverContentPrimitive Side="@PopoverSide.Top">Top</PopoverContentPrimitive>
        <PopoverContentPrimitive Side="@PopoverSide.Right">Right</PopoverContentPrimitive>
        <PopoverContentPrimitive Side="@PopoverSide.Left">Left</PopoverContentPrimitive>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _popoverProps =
    [
        new("Open", "bool?", null, "Controls whether the popover is open (controlled mode)."),
        new("OpenChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the open state changes."),
        new("Side", "PopoverSide", "Bottom", "Which side of the trigger to position the content."),
        new("Align", "PopoverAlign", "Center", "How to align the content relative to the trigger."),
    ];
    }
}
