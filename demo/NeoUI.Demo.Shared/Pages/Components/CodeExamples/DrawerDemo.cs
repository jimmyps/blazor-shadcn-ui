namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DrawerDemo
    {
        private bool _snapDrawerOpen;
        private int _snapIndex;

        private static readonly IReadOnlyList<DemoPropRow> _drawerProps =
            [
                new("Direction",    "DrawerDirection", "Bottom", "Top, Right, Bottom, or Left."),
                new("Open",         "@bind-Open / bool?", null,  "Controlled open state."),
                new("DefaultOpen",  "bool",            "false",  "Initial open state."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _drawerContentProps =
            [
                new("SnapPoints",   "float[]?",  null,    "Fraction-of-viewport heights (e.g. 0.35, 0.65, 1.0) to snap to. Bottom drawers only."),
                new("SnapIndex",    "int",        "0",     "Currently active snap point index. Supports two-way binding."),
                new("ShowHandle",   "bool",       "true",  "Show the drag handle at the top of the drawer."),
            ];

        private const string _basicCode =
                """
                <Drawer>
                    <DrawerTrigger>
                        <Button Variant="ButtonVariant.Outline">Open Drawer</Button>
                    </DrawerTrigger>
                    <DrawerContent>
                        <DrawerHeader>
                            <DrawerTitle>Edit Profile</DrawerTitle>
                            <DrawerDescription>Make changes to your profile here.</DrawerDescription>
                        </DrawerHeader>
                        <div class="p-4">
                            <p class="text-sm text-muted-foreground">Drawer content goes here...</p>
                        </div>
                        <DrawerFooter>
                            <Button>Save changes</Button>
                            <DrawerClose>
                                <Button Variant="ButtonVariant.Outline">Cancel</Button>
                            </DrawerClose>
                        </DrawerFooter>
                    </DrawerContent>
                </Drawer>
                """;

        private const string _directionsCode =
                """
                <Drawer Direction="DrawerDirection.Top">
                    <DrawerTrigger><Button Variant="ButtonVariant.Outline">Top</Button></DrawerTrigger>
                    <DrawerContent ShowHandle="false">...</DrawerContent>
                </Drawer>

                <Drawer Direction="DrawerDirection.Right">
                    <DrawerTrigger><Button Variant="ButtonVariant.Outline">Right</Button></DrawerTrigger>
                    <DrawerContent ShowHandle="false">...</DrawerContent>
                </Drawer>

                <Drawer Direction="DrawerDirection.Bottom">
                    <DrawerTrigger><Button Variant="ButtonVariant.Outline">Bottom</Button></DrawerTrigger>
                    <DrawerContent>...</DrawerContent>
                </Drawer>

                <Drawer Direction="DrawerDirection.Left">
                    <DrawerTrigger><Button Variant="ButtonVariant.Outline">Left</Button></DrawerTrigger>
                    <DrawerContent ShowHandle="false">...</DrawerContent>
                </Drawer>
                """;

        private const string _snapPointsCode =
                """
                <Drawer>
                    <DrawerTrigger>
                        <Button Variant="ButtonVariant.Outline">Open Snap Drawer</Button>
                    </DrawerTrigger>
                    <DrawerContent SnapPoints="@(new[] { 0.35f, 0.65f, 1.0f })" @bind-SnapIndex="_snapIndex">
                        <DrawerHeader>
                            <DrawerTitle>Snap Points Demo</DrawerTitle>
                            <DrawerDescription>Drag the handle or use the buttons to snap to different heights.</DrawerDescription>
                        </DrawerHeader>
                        <div class="p-4 space-y-3">
                            <p class="text-sm text-muted-foreground">Current snap: @(_snapIndex + 1) of 3</p>
                            <div class="flex gap-2">
                                <Button Size="ButtonSize.Small" Variant="ButtonVariant.Outline" OnClick="@(() => _snapIndex = 0)">35%</Button>
                                <Button Size="ButtonSize.Small" Variant="ButtonVariant.Outline" OnClick="@(() => _snapIndex = 1)">65%</Button>
                                <Button Size="ButtonSize.Small" Variant="ButtonVariant.Outline" OnClick="@(() => _snapIndex = 2)">100%</Button>
                            </div>
                        </div>
                    </DrawerContent>
                </Drawer>
                """;
    }
}
