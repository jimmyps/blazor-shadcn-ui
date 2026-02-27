namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SheetDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _sheetProps =
            [
                new("Side",         "SheetSide",           "Right", "Which side the sheet slides in from. Options: Left, Right, Top, Bottom."),
                new("Open",         "bool",                "false", "Controlled open state. Use @bind-Open for two-way binding."),
                new("OpenChanged",  "EventCallback&lt;bool&gt;","—","Callback invoked when the open state changes."),
                new("ShowClose",    "bool",                "true",  "Whether to show the default close button in the top-right corner."),
                new("Class",        "string?",             "null",  "Additional CSS classes for SheetContent (controls width/height)."),
                new("ChildContent", "RenderFragment?",     "null",  "Content of the component."),
            ];

        private const string _asChildCode = """
                <!-- Without AsChild -->
                <Sheet>
                    <SheetTrigger class="...button-styles...">Open Sheet</SheetTrigger>
                    <SheetContent><!-- content --></SheetContent>
                </Sheet>

                <!-- With AsChild -->
                <Sheet>
                    <SheetTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open Menu</Button>
                    </SheetTrigger>
                    <SheetContent Side="SheetSide.Left">
                        <SheetHeader>
                            <SheetTitle>Navigation</SheetTitle>
                        </SheetHeader>
                        <SheetFooter>
                            <SheetClose AsChild>
                                <Button Variant="ButtonVariant.Outline">Close</Button>
                            </SheetClose>
                        </SheetFooter>
                    </SheetContent>
                </Sheet>
                """;

        private const string _rightCode = """
                <Sheet>
                    <SheetTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open Sheet</Button>
                    </SheetTrigger>
                    <SheetContent>
                        <SheetHeader>
                            <SheetTitle>Edit profile</SheetTitle>
                            <SheetDescription>Make changes to your profile here.</SheetDescription>
                        </SheetHeader>
                        <!-- form fields -->
                        <SheetFooter>
                            <Button>Save changes</Button>
                        </SheetFooter>
                    </SheetContent>
                </Sheet>
                """;

        private const string _topCode = """
                <Sheet Side="SheetSide.Top">
                    <SheetTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open from Top</Button>
                    </SheetTrigger>
                    <SheetContent>
                        <SheetHeader>
                            <SheetTitle>Notifications</SheetTitle>
                        </SheetHeader>
                    </SheetContent>
                </Sheet>
                """;

        private const string _bottomCode = """
                <Sheet Side="SheetSide.Bottom">
                    <SheetTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open from Bottom</Button>
                    </SheetTrigger>
                    <SheetContent>
                        <SheetHeader>
                            <SheetTitle>Cookie Settings</SheetTitle>
                        </SheetHeader>
                        <SheetFooter>
                            <Button>Save Preferences</Button>
                        </SheetFooter>
                    </SheetContent>
                </Sheet>
                """;

        private const string _leftCode = """
                <Sheet Side="SheetSide.Left">
                    <SheetTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open from Left</Button>
                    </SheetTrigger>
                    <SheetContent>
                        <SheetHeader>
                            <SheetTitle>Navigation Menu</SheetTitle>
                        </SheetHeader>
                    </SheetContent>
                </Sheet>
                """;

        private const string _controlledCode = """
                <Sheet @bind-Open="isSheetOpen">
                    <SheetContent>
                        <SheetHeader>
                            <SheetTitle>Controlled Sheet</SheetTitle>
                        </SheetHeader>
                        <SheetFooter>
                            <Button @onclick="() => isSheetOpen = false">Close</Button>
                        </SheetFooter>
                    </SheetContent>
                </Sheet>

                @code {
                    private bool isSheetOpen = false;
                }
                """;

        private const string _noCloseCode = """
                <Sheet>
                    <SheetTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open Sheet</Button>
                    </SheetTrigger>
                    <SheetContent ShowClose="false">
                        <SheetFooter>
                            <SheetClose AsChild>
                                <Button Variant="ButtonVariant.Outline">Cancel</Button>
                            </SheetClose>
                            <SheetClose AsChild>
                                <Button>Confirm</Button>
                            </SheetClose>
                        </SheetFooter>
                    </SheetContent>
                </Sheet>
                """;

        private const string _customWidthCode = """
                <Sheet>
                    <SheetTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open Wide Sheet</Button>
                    </SheetTrigger>
                    <SheetContent Class="w-[400px] sm:w-[540px]">
                        <SheetHeader>
                            <SheetTitle>Wide Sheet</SheetTitle>
                        </SheetHeader>
                    </SheetContent>
                </Sheet>
                """;

        private const string _differentSidesCode = """
                <Sheet Side="SheetSide.Left"><!-- slides in from left --></Sheet>
                <Sheet><!-- slides in from right (default) --></Sheet>
                <Sheet Side="SheetSide.Top"><!-- slides in from top --></Sheet>
                <Sheet Side="SheetSide.Bottom"><!-- slides in from bottom --></Sheet>
                """;
    }
}
