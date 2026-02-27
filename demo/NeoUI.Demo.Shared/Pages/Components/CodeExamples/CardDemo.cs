namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class CardDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _cardProps =
            [
                new("Class",        "string?",          null, "Additional CSS classes for the Card root element."),
                new("ChildContent", "RenderFragment?",  null, "Card body. Compose with CardHeader, CardContent, and CardFooter sub-components."),
            ];

        private const string _basicCode =
                """
                <Card>
                    <CardHeader>
                        <CardTitle>Card Title</CardTitle>
                        <CardDescription>Card description goes here.</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <p class="text-sm">This is the main content area of the card.</p>
                    </CardContent>
                    <CardFooter>
                        <Button Size="ButtonSize.Small">Action</Button>
                    </CardFooter>
                </Card>
                """;

        private const string _loginFormCode =
                """
                <Card>
                    <CardHeader>
                        <CardTitle>Login to your account</CardTitle>
                        <CardDescription>Enter your email below to login</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <!-- form fields -->
                    </CardContent>
                    <CardFooter>
                        <Button Class="w-full">Login</Button>
                    </CardFooter>
                </Card>
                """;

        private const string _gridCode =
                """
                <div class="grid gap-4 md:grid-cols-3">
                    <Card>
                        <CardHeader>
                            <CardTitle>Total Revenue</CardTitle>
                            <CardDescription>January - December 2024</CardDescription>
                        </CardHeader>
                        <CardContent>
                            <div class="text-2xl font-bold">$45,231.89</div>
                        </CardContent>
                    </Card>
                    <!-- more cards... -->
                </div>
                """;

        private const string _actionsCode =
                """
                <Card>
                    <CardHeader>
                        <CardTitle>Create Project</CardTitle>
                        <CardDescription>Deploy your new project in one-click.</CardDescription>
                    </CardHeader>
                    <CardContent>...</CardContent>
                    <CardFooter Class="gap-2">
                        <Button Variant="ButtonVariant.Outline">Cancel</Button>
                        <Button>Deploy</Button>
                    </CardFooter>
                </Card>
                """;

        private const string _contentOnlyCode =
                """
                <Card>
                    <CardHeader>
                        <CardTitle>Notifications</CardTitle>
                        <CardDescription>You have 3 unread messages.</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <!-- notification items -->
                    </CardContent>
                </Card>
                """;

        private const string _customCode =
                """
                <Card Class="border-2 border-primary">
                    <CardHeader>
                        <CardTitle>Featured Card</CardTitle>
                    </CardHeader>
                </Card>

                <Card Class="shadow-lg">
                    <CardHeader>
                        <CardTitle>Enhanced Shadow</CardTitle>
                    </CardHeader>
                </Card>
                """;
    }
}
