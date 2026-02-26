namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ButtonDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _buttonProps =
            [
                new("Variant",      "ButtonVariant",   "Default", "Visual style: Default, Destructive, Outline, Secondary, Ghost, Link."),
                new("Size",         "ButtonSize",       "Default", "Size: Small, Default, Large, Icon, IconSmall, IconLarge."),
                new("Type",         "ButtonType",       "Button",  "HTML button type: Button, Submit, Reset."),
                new("Disabled",     "bool",             "false",   "Whether the button is non-interactive."),
                new("IconPosition", "IconPosition?",    null,      "Where to place the icon: Start or End."),
                new("AriaLabel",    "string?",          null,      "Accessible label for icon-only buttons."),
                new("Class",        "string?",          null,      "Additional CSS classes."),
                new("OnClick",      "EventCallback",    "—",       "Click event callback."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _linkButtonProps =
            [
                new("Href",         "string",           "—",       "The URL to navigate to."),
                new("Target",       "string?",          null,      "Link target, e.g. \"_blank\" for new tab."),
                new("Variant",      "ButtonVariant",    "Default", "Same variants as Button."),
                new("Size",         "ButtonSize",       "Default", "Same sizes as Button."),
                new("IconPosition", "IconPosition?",    null,      "Where to place the icon."),
                new("Class",        "string?",          null,      "Additional CSS classes."),
            ];

        private const string _variantsCode =
                """
                <Button Variant="ButtonVariant.Default">Default</Button>
                <Button Variant="ButtonVariant.Destructive">Destructive</Button>
                <Button Variant="ButtonVariant.Outline">Outline</Button>
                <Button Variant="ButtonVariant.Secondary">Secondary</Button>
                <Button Variant="ButtonVariant.Ghost">Ghost</Button>
                <Button Variant="ButtonVariant.Link">Link</Button>
                """;

        private const string _sizesCode =
                """
                <Button Size="ButtonSize.Small">Small</Button>
                <Button Size="ButtonSize.Default">Default</Button>
                <Button Size="ButtonSize.Large">Large</Button>
                <Button Size="ButtonSize.Icon" AriaLabel="Icon button">
                    <LucideIcon Name="arrow-right" Size="16" />
                </Button>
                <Button Size="ButtonSize.IconSmall" AriaLabel="Small icon button">
                    <LucideIcon Name="minus" Size="14" />
                </Button>
                <Button Size="ButtonSize.IconLarge" AriaLabel="Large icon button">
                    <LucideIcon Name="arrow-right" Size="18" />
                </Button>
                """;

        private const string _iconsCode =
                """
                <Button IconPosition="IconPosition.Start">
                    <Icon><LucideIcon Name="arrow-left" Size="16" /></Icon>
                    <ChildContent>Back</ChildContent>
                </Button>
                <Button IconPosition="IconPosition.Start" Variant="ButtonVariant.Outline">
                    <Icon><LucideIcon Name="search" Size="16" /></Icon>
                    <ChildContent>Search</ChildContent>
                </Button>

                <Button IconPosition="IconPosition.End">
                    <Icon><LucideIcon Name="arrow-right" Size="16" /></Icon>
                    <ChildContent>Next</ChildContent>
                </Button>
                <Button IconPosition="IconPosition.End" Variant="ButtonVariant.Secondary">
                    <Icon><LucideIcon Name="external-link" Size="16" /></Icon>
                    <ChildContent>External Link</ChildContent>
                </Button>
                """;

        private const string _rtlCode =
                """
                <div dir="rtl" class="p-6 bg-card rounded-lg border">
                    <div class="flex flex-wrap gap-4">
                        <Button IconPosition="IconPosition.Start">
                            <Icon><LucideIcon Name="arrow-left" Size="16" /></Icon>
                            <ChildContent>Back (Start)</ChildContent>
                        </Button>
                        <Button IconPosition="IconPosition.End">
                            <Icon><LucideIcon Name="arrow-right" Size="16" /></Icon>
                            <ChildContent>Next (End)</ChildContent>
                        </Button>
                    </div>
                </div>
                """;

        private const string _disabledCode =
                """
                <Button Disabled="true">Default Disabled</Button>
                <Button Variant="ButtonVariant.Destructive" Disabled="true">Destructive Disabled</Button>
                <Button Variant="ButtonVariant.Outline" Disabled="true">Outline Disabled</Button>
                <Button Variant="ButtonVariant.Secondary" Disabled="true">Secondary Disabled</Button>
                <Button Variant="ButtonVariant.Ghost" Disabled="true">Ghost Disabled</Button>
                <Button Variant="ButtonVariant.Link" Disabled="true">Link Disabled</Button>
                """;

        private const string _interactiveCode =
                """
                <Button OnClick="HandleClick">Click Me</Button>
                <span class="text-sm">@clickMessage</span>

                <Button OnClick="IncrementCounter">Clicked @counter times</Button>
                <Button Variant="ButtonVariant.Outline" OnClick="ResetCounter">Reset</Button>
                """;

        private const string _typesCode =
                """
                <Button Type="ButtonType.Button">Type: Button</Button>
                <Button Type="ButtonType.Submit">Type: Submit</Button>
                <Button Type="ButtonType.Reset" Variant="ButtonVariant.Secondary">Type: Reset</Button>
                """;

        private const string _linkButtonCode =
                """
                <LinkButton Href="/" Variant="ButtonVariant.Default">Home (Default)</LinkButton>
                <LinkButton Href="/" Variant="ButtonVariant.Outline">Home (Outline)</LinkButton>
                <LinkButton Href="/" Variant="ButtonVariant.Secondary">Home (Secondary)</LinkButton>

                <LinkButton Href="/components" IconPosition="IconPosition.Start">
                    <Icon><LucideIcon Name="arrow-left" Size="16" /></Icon>
                    <ChildContent>Back to Components</ChildContent>
                </LinkButton>
                <LinkButton Href="https://neoui.io" Target="_blank" Variant="ButtonVariant.Outline" IconPosition="IconPosition.End">
                    <Icon><LucideIcon Name="external-link" Size="16" /></Icon>
                    <ChildContent>NeoUI Website</ChildContent>
                </LinkButton>

                <LinkButton Href="https://blazor.net" Target="_blank">Blazor Docs</LinkButton>
                """;

        private const string _comparisonCode =
                """
                <!-- Actions that do not navigate -->
                <Button Variant="ButtonVariant.Default">Submit Form</Button>

                <!-- Navigation and links -->
                <LinkButton Href="/architecture" Variant="ButtonVariant.Default">View Architecture</LinkButton>
                """;

        private const string _customClassCode =
                """
                <Button Class="w-full">Full Width Button</Button>
                <Button Class="shadow-lg">With Shadow</Button>
                <Button Variant="ButtonVariant.Outline" Class="border-2 border-blue-500">Custom Border</Button>
                """;

        private const string _darkModeCode =
                """
                <Button Variant="ButtonVariant.Default">Default</Button>
                <Button Variant="ButtonVariant.Destructive">Destructive</Button>
                <Button Variant="ButtonVariant.Outline">Outline</Button>
                <Button Variant="ButtonVariant.Secondary">Secondary</Button>
                <Button Variant="ButtonVariant.Ghost">Ghost</Button>
                <Button Variant="ButtonVariant.Link">Link</Button>
                """;
    }
}
