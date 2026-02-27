namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class AlertDialogDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _alertDialogProps =
            [
                new("Open",                "bool",             "false", "Controls whether the dialog is open."),
                new("CloseOnClickOutside", "bool",             "false", "When true, clicking outside the dialog closes it."),
                new("CloseOnEscape",       "bool",             "true",  "When true, pressing Escape closes the dialog."),
                new("ChildContent",        "RenderFragment?",  null,    "Content of the alert dialog. Nest AlertDialogTrigger and AlertDialogContent here."),
            ];

        private const string _basicCode =
                """
                <AlertDialog>
                    <AlertDialogTrigger>
                        <Button Variant="ButtonVariant.Outline">Show Dialog</Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                        <AlertDialogHeader>
                            <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
                            <AlertDialogDescription>
                                This action cannot be undone.
                            </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                            <AlertDialogCancel><Button Variant="ButtonVariant.Outline">Cancel</Button></AlertDialogCancel>
                            <AlertDialogAction><Button Variant="ButtonVariant.Destructive">Continue</Button></AlertDialogAction>
                        </AlertDialogFooter>
                    </AlertDialogContent>
                </AlertDialog>
                """;

        private const string _confirmCode =
                """
                <AlertDialog>
                    <AlertDialogTrigger>
                        <Button>Save Changes</Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                        <AlertDialogHeader>
                            <AlertDialogTitle>Save your changes?</AlertDialogTitle>
                            <AlertDialogDescription>Your changes will be saved to your profile.</AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                            <AlertDialogCancel><Button Variant="ButtonVariant.Outline">Cancel</Button></AlertDialogCancel>
                            <AlertDialogAction><Button>Save</Button></AlertDialogAction>
                        </AlertDialogFooter>
                    </AlertDialogContent>
                </AlertDialog>
                """;

        private const string _warningCode =
                """
                <AlertDialog>
                    <AlertDialogTrigger>
                        <Button Variant="ButtonVariant.Destructive">Delete All Data</Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                        <AlertDialogHeader>
                            <AlertDialogTitle>Delete all data?</AlertDialogTitle>
                            <AlertDialogDescription>This action cannot be undone.</AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                            <AlertDialogCancel><Button Variant="ButtonVariant.Outline">Cancel</Button></AlertDialogCancel>
                            <AlertDialogAction><Button Variant="ButtonVariant.Destructive">Delete Everything</Button></AlertDialogAction>
                        </AlertDialogFooter>
                    </AlertDialogContent>
                </AlertDialog>
                """;

        private const string _informationCode =
                """
                <AlertDialog>
                    <AlertDialogTrigger>
                        <Button Variant="ButtonVariant.Secondary">View Terms</Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                        <AlertDialogHeader>
                            <AlertDialogTitle>Terms and Conditions</AlertDialogTitle>
                            <AlertDialogDescription>
                                By continuing, you agree to our Terms of Service and Privacy Policy.
                            </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                            <AlertDialogAction>
                                <Button>I Understand</Button>
                            </AlertDialogAction>
                        </AlertDialogFooter>
                    </AlertDialogContent>
                </AlertDialog>
                """;

        private const string _controlledCode =
                """
                <Button OnClick="@(() => isOpen = true)">Open Dialog</Button>

                <AlertDialog @bind-Open="isOpen">
                    <AlertDialogContent>
                        <AlertDialogHeader>
                            <AlertDialogTitle>Controlled Dialog</AlertDialogTitle>
                            <AlertDialogDescription>Open state is controlled by the parent.</AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                            <AlertDialogCancel><Button Variant="ButtonVariant.Outline">Close</Button></AlertDialogCancel>
                        </AlertDialogFooter>
                    </AlertDialogContent>
                </AlertDialog>

                @code {
                    private bool isOpen;
                }
                """;
    }
}
