namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DialogDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _dialogProps =
            [
                new("Open",           "bool",                      "false",  "Controls the open state of the dialog. Use @bind-Open for two-way binding."),
                new("OpenChanged",    "EventCallback<bool>","—",      "Fired when the open state changes. Paired with Open for two-way binding."),
                new("ShowClose",      "bool",                      "true",   "On DialogContent: when false, hides the default X close button in the top-right corner."),
                new("Class",          "string?",                   "null",   "Additional CSS classes. Use max-w-* on DialogContent to customize width."),
                new("Variant",        "DialogContentVariant",      "Default","On DialogContent: style variant. Use Form for scrollable form layouts."),
                new("AsChild",        "bool",                      "false",  "On DialogTrigger/DialogClose: merges dialog behavior onto the child element."),
                new("ChildContent",   "RenderFragment?",           "null",   "Content slot for trigger, content, header, footer sub-components."),
                new("Class (Footer)", "string?",                   "null",   "On DialogFooter: additional CSS classes. Useful for spacing adjustments like mt-4."),
            ];

        private const string _simpleCode = """
                <Dialog>
                    <DialogTrigger>Open Dialog</DialogTrigger>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Are you absolutely sure?</DialogTitle>
                            <DialogDescription>
                                This action cannot be undone. This will permanently delete your account
                                and remove your data from our servers.
                            </DialogDescription>
                        </DialogHeader>
                    </DialogContent>
                </Dialog>
                """;

        private const string _footerCode = """
                <Dialog>
                    <DialogTrigger>Edit Profile</DialogTrigger>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Edit profile</DialogTitle>
                            <DialogDescription>Make changes to your profile here. Click save when you're done.</DialogDescription>
                        </DialogHeader>
                        <div class="grid gap-4 py-4">
                            <div class="grid grid-cols-4 items-center gap-4">
                                <label class="text-right text-sm font-medium">Name</label>
                                <input value="Pedro Duarte" class="col-span-3 ..." />
                            </div>
                            <div class="grid grid-cols-4 items-center gap-4">
                                <label class="text-right text-sm font-medium">Username</label>
                                <input value="@peduarte" class="col-span-3 ..." />
                            </div>
                        </div>
                        <DialogFooter>
                            <button type="submit">Save changes</button>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
                """;

        private const string _controlledCode = """
                <button @onclick="() => isOpen = true">Open Dialog</button>

                <Dialog @bind-Open="isOpen">
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Controlled Dialog</DialogTitle>
                            <DialogDescription>
                                This dialog's open state is controlled from the parent component.
                            </DialogDescription>
                        </DialogHeader>
                        <DialogFooter>
                            <button @onclick="() => isOpen = false">Cancel</button>
                            <button @onclick="HandleSave">Save</button>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
                """;

        private const string _customWidthCode = """
                <Dialog>
                    <DialogTrigger>Open Wide Dialog</DialogTrigger>
                    <DialogContent Class="max-w-3xl">
                        <DialogHeader>
                            <DialogTitle>Wide Dialog</DialogTitle>
                            <DialogDescription>
                                This dialog uses a custom max-width class to be wider than the default.
                            </DialogDescription>
                        </DialogHeader>
                    </DialogContent>
                </Dialog>
                """;

        private const string _noCloseCode = """
                <Dialog>
                    <DialogTrigger>Open Dialog</DialogTrigger>
                    <DialogContent ShowClose="false">
                        <DialogHeader>
                            <DialogTitle>Confirm Action</DialogTitle>
                            <DialogDescription>
                                This dialog doesn't have a close button in the top-right corner.
                                You must use one of the action buttons or press Escape to close.
                            </DialogDescription>
                        </DialogHeader>
                        <DialogFooter Class="mt-4">
                            <DialogClosePrimitive>Cancel</DialogClosePrimitive>
                            <DialogClosePrimitive>Confirm</DialogClosePrimitive>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
                """;

        private const string _asChildCode = """
                <Dialog>
                    <DialogTrigger AsChild>
                        <Button Variant="ButtonVariant.Destructive">
                            <LucideIcon Name="trash-2" Size="16" />
                            Delete Account
                        </Button>
                    </DialogTrigger>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Delete Account</DialogTitle>
                            <DialogDescription>
                                This action cannot be undone. This will permanently delete your account.
                            </DialogDescription>
                        </DialogHeader>
                        <DialogFooter>
                            <DialogClose AsChild>
                                <Button Variant="ButtonVariant.Outline">Cancel</Button>
                            </DialogClose>
                            <Button Variant="ButtonVariant.Destructive">Delete</Button>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
                """;

        private const string _comboboxCode = """
                <Dialog>
                    <DialogTrigger>Open Dialog with Combobox</DialogTrigger>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Select a Framework</DialogTitle>
                            <DialogDescription>Use the combobox below to search and select a framework.</DialogDescription>
                        </DialogHeader>
                        <div class="py-4">
                            <Combobox TItem="Framework"
                                      Items="frameworks"
                                      @bind-Value="selectedFramework"
                                      ValueSelector="@(f => f.Value)"
                                      DisplaySelector="@(f => f.Label)"
                                      Placeholder="Select framework..." />
                        </div>
                        <DialogFooter>
                            <DialogClose AsChild><Button Variant="ButtonVariant.Outline">Cancel</Button></DialogClose>
                            <DialogClose AsChild><Button>Confirm</Button></DialogClose>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
                """;

        private const string _advancedCode = """
                <!-- Controlled dialog with complex form -->
                <Dialog @bind-Open="showDialog">
                    <DialogContent Variant="DialogContentVariant.Form" Class="p-4 max-w-2xl max-h-[90vh] flex flex-col">
                        <DialogHeader Class="flex-shrink-0">
                            <DialogTitle>User Profile Settings</DialogTitle>
                            <DialogDescription>Update your profile information and preferences.</DialogDescription>
                        </DialogHeader>
                        <ScrollArea FillContainer="true" EnableScrollShadows="true">
                            <EditForm EditContext="editContext">
                                <DataAnnotationsValidator />
                                <FieldGroup Orientation="FieldGroupOrientation.Vertical">
                                    <Field>
                                        <FieldLabel For="name">Full Name *</FieldLabel>
                                        <FieldContent>
                                            <Input Id="name" @bind-Value="model.Name" ShowValidationError="true" />
                                        </FieldContent>
                                    </Field>
                                    <!-- more fields... -->
                                </FieldGroup>
                            </EditForm>
                        </ScrollArea>
                        <DialogFooter Class="flex-shrink-0 mt-auto pt-3 border-t">
                            <Button Variant="ButtonVariant.Outline" OnClick="Cancel">Cancel</Button>
                            <Button OnClick="Save">Save Changes</Button>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
                """;
    }
}
