namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class AlertDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _alertProps =
            [
                new("Variant",      "AlertVariant",    "Default",   "Visual style variant. Options: Default, Muted, Destructive, Success, Info, Warning, Danger."),
                new("AccentBorder", "bool",             "false",     "When <code class=\"text-xs bg-muted px-1 rounded\">true</code>, displays a thick left border in the variant's accent color with a subtle tinted background."),
                new("Icon",         "RenderFragment?",  null,        "Optional icon slot rendered to the left of the content."),
                new("ChildContent", "RenderFragment?",  null,        "Alert body. Nest <code class=\"text-xs bg-muted px-1 rounded\">AlertTitle</code> and <code class=\"text-xs bg-muted px-1 rounded\">AlertDescription</code> here."),
                new("Class",        "string?",          null,        "Additional CSS classes appended to the root element."),
            ];

        private const string _basicCode =
                """
                <Alert>
                    <Icon><LucideIcon Name="terminal" Class="h-4 w-4" /></Icon>
                    <ChildContent>
                        <AlertTitle>Heads up!</AlertTitle>
                        <AlertDescription>You can add components to your app using the CLI.</AlertDescription>
                    </ChildContent>
                </Alert>
                """;

        private const string _variantsCode =
                """
                <Alert>
                    <Icon><LucideIcon Name="terminal" Class="h-4 w-4" /></Icon>
                    <ChildContent>
                        <AlertTitle>Default</AlertTitle>
                        <AlertDescription>A standard alert for general informational messages.</AlertDescription>
                    </ChildContent>
                </Alert>

                <Alert Variant="AlertVariant.Destructive">
                    <Icon><LucideIcon Name="octagon-alert" Class="h-4 w-4" /></Icon>
                    <ChildContent>
                        <AlertTitle>Destructive</AlertTitle>
                        <AlertDescription>Error message following shadcn/ui design system conventions.</AlertDescription>
                    </ChildContent>
                </Alert>

                <Alert Variant="AlertVariant.Success">
                    <Icon><LucideIcon Name="circle-check" Class="h-4 w-4" /></Icon>
                    <ChildContent>
                        <AlertTitle>Success</AlertTitle>
                        <AlertDescription>Your changes have been saved successfully.</AlertDescription>
                    </ChildContent>
                </Alert>
                """;

        private const string _accentBorderCode =
                """
                <Alert Variant="AlertVariant.Success" AccentBorder="true">
                    <Icon><LucideIcon Name="circle-check" Class="h-4 w-4" /></Icon>
                    <ChildContent>
                        <AlertTitle>Success with Accent</AlertTitle>
                        <AlertDescription>The accent border makes this success message more prominent.</AlertDescription>
                    </ChildContent>
                </Alert>

                <Alert Variant="AlertVariant.Warning" AccentBorder="true">
                    <Icon><LucideIcon Name="triangle-alert" Class="h-4 w-4" /></Icon>
                    <ChildContent>
                        <AlertTitle>Important Note</AlertTitle>
                        <AlertDescription>Each component has its own namespace.</AlertDescription>
                    </ChildContent>
                </Alert>
                """;

        private const string _noIconCode =
                """
                <Alert>
                    <AlertTitle>Heads up!</AlertTitle>
                    <AlertDescription>You can add components to your app using the CLI.</AlertDescription>
                </Alert>

                <Alert Variant="AlertVariant.Warning" AccentBorder="true">
                    <AlertTitle>Note</AlertTitle>
                    <AlertDescription>Remember to save your work before leaving the page.</AlertDescription>
                </Alert>

                <Alert Variant="AlertVariant.Destructive">
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>Your session has expired. Please log in again.</AlertDescription>
                </Alert>
                """;
    }
}
