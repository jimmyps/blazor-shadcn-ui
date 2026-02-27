namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DrawerDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _drawerProps =
            [
                new("Direction",    "DrawerDirection", "Bottom", "Top, Right, Bottom, or Left."),
                new("Open",         "@bind-Open / bool?", null,  "Controlled open state."),
                new("DefaultOpen",  "bool",            "false",  "Initial open state."),
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
    }
}
