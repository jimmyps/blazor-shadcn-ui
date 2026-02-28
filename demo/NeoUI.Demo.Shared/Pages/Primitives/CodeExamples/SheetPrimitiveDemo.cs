namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class SheetPrimitiveDemo
    {
        private const string _asChildCode =
        """
        <SheetPrimitive>
            <SheetTriggerPrimitive AsChild>
                <Button Variant="ButtonVariant.Outline">Open SheetPrimitive</Button>
            </SheetTriggerPrimitive>
        </SheetPrimitive>
        """;

        private const string _basicCode =
        """
        <SheetPrimitive>
            <SheetTriggerPrimitive>Open SheetPrimitive</SheetTriggerPrimitive>

            <SheetPortal>
                <SheetOverlay class="fixed inset-0 bg-black/50 z-50" />
                <SheetContentPrimitive class="fixed right-0 top-0 z-50 h-full w-3/4 sm:max-w-sm bg-background p-6 border-l">
                    <SheetTitlePrimitive>Sheet Title</SheetTitlePrimitive>
                    <SheetDescriptionPrimitive>This is a basic sheet using the unstyled primitive components.</SheetDescriptionPrimitive>
                    <SheetClosePrimitive>Close</SheetClosePrimitive>
                </SheetContentPrimitive>
            </SheetPortal>
        </SheetPrimitive>
        """;

        private const string _controlledCode =
        """
        <SheetPrimitive @bind-Open="isControlledOpen">
            <SheetPortal>
                <SheetOverlay class="fixed inset-0 bg-black/50 z-50" />
                <SheetContentPrimitive class="fixed right-0 top-0 z-50 h-full w-3/4 sm:max-w-sm bg-background p-6 border-l">
                    <SheetTitlePrimitive>Controlled SheetPrimitive</SheetTitlePrimitive>
                    <SheetClosePrimitive>Close</SheetClosePrimitive>
                </SheetContentPrimitive>
            </SheetPortal>
        </SheetPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Enter / Space</Kbd>
        <Kbd>Escape</Kbd>
        <Kbd>Tab</Kbd>
        """;

        private const string _sidesCode =
        """
        <SheetPrimitive Side="SheetSide.Top">
            <SheetTriggerPrimitive>Top</SheetTriggerPrimitive>
        </SheetPrimitive>
        <SheetPrimitive Side="SheetSide.Right">
            <SheetTriggerPrimitive>Right</SheetTriggerPrimitive>
        </SheetPrimitive>
        <SheetPrimitive Side="SheetSide.Bottom">
            <SheetTriggerPrimitive>Bottom</SheetTriggerPrimitive>
        </SheetPrimitive>
        <SheetPrimitive Side="SheetSide.Left">
            <SheetTriggerPrimitive>Left</SheetTriggerPrimitive>
        </SheetPrimitive>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _sheetProps =
    [
        new("Open", "bool?", null, "Controls whether the sheet is open (controlled mode)."),
        new("OpenChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the open state changes."),
        new("DefaultOpen", "bool", "false", "Initial open state (uncontrolled mode)."),
        new("Side", "SheetSide", "SheetSide.Right", "The side from which the sheet slides in."),
        new("Modal", "bool", "true", "Whether the sheet is modal."),
    ];
    }
}
