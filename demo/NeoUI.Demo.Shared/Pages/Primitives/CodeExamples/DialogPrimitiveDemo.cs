namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class DialogPrimitiveDemo
    {
        private const string _asChildCode =
        """
        <DialogPrimitive>
            <DialogTriggerPrimitive AsChild>
                <Button Variant="ButtonVariant.Outline">Open DialogPrimitive</Button>
            </DialogTriggerPrimitive>

            <DialogPortal>
                <DialogOverlay class="fixed inset-0 bg-black/50 z-50" />
                <DialogContentPrimitive class="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-background p-6 rounded-lg border max-w-md z-50">
                    <DialogTitlePrimitive>With AsChild</DialogTitlePrimitive>
                    <DialogDescriptionPrimitive>
                        Button component handles the trigger behavior.
                    </DialogDescriptionPrimitive>
                    <DialogClosePrimitive AsChild>
                        <Button Variant="ButtonVariant.Secondary">Close</Button>
                    </DialogClosePrimitive>
                </DialogContentPrimitive>
            </DialogPortal>
        </DialogPrimitive>
        """;

        private const string _basicCode =
        """
        <DialogPrimitive>
            <DialogTriggerPrimitive>Open DialogPrimitive</DialogTriggerPrimitive>
            <DialogContentPrimitive>
                <DialogTitlePrimitive>Dialog Title</DialogTitlePrimitive>
                <DialogClosePrimitive>Close</DialogClosePrimitive>
            </DialogContentPrimitive>
        </DialogPrimitive>
        """;

        private const string _controlledCode =
        """
        <DialogPrimitive @bind-Open="isControlledOpen">
            <DialogPortal>
                <DialogOverlay class="fixed inset-0 bg-black/50 z-50" />
                <DialogContentPrimitive class="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-background p-6 rounded-lg border max-w-md z-50">
                    <DialogTitlePrimitive>Controlled DialogPrimitive</DialogTitlePrimitive>
                    <DialogDescriptionPrimitive>
                        This dialog's state is controlled by the parent component.
                    </DialogDescriptionPrimitive>
                    <DialogClosePrimitive>Close</DialogClosePrimitive>
                </DialogContentPrimitive>
            </DialogPortal>
        </DialogPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Enter / Space</Kbd>
        <Kbd>Escape</Kbd>
        <Kbd>Tab</Kbd>
        """;

        private const string _modalCode =
        """
        <DialogPrimitive Modal="true">
            <DialogTriggerPrimitive>Open Modal DialogPrimitive</DialogTriggerPrimitive>
            <DialogPortal>
                <DialogOverlay class="fixed inset-0 bg-black/50 z-50" />
                <DialogContentPrimitive class="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-background p-6 rounded-lg border max-w-md z-50">
                    <DialogTitlePrimitive>Modal DialogPrimitive</DialogTitlePrimitive>
                    <DialogDescriptionPrimitive>
                        Try clicking the overlay or pressing Escape to close this dialog.
                    </DialogDescriptionPrimitive>
                    <DialogClosePrimitive>Close</DialogClosePrimitive>
                </DialogContentPrimitive>
            </DialogPortal>
        </DialogPrimitive>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _dialogProps =
    [
        new("Open", "bool?", null, "Controls whether the dialog is open (controlled mode)."),
        new("OpenChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the open state changes."),
        new("DefaultOpen", "bool", "false", "Initial open state (uncontrolled mode)."),
        new("Modal", "bool", "true", "Whether the dialog is modal."),
    ];
    }
}
