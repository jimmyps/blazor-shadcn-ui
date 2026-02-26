namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class BadgeDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _badgeProps =
            [
                new("Variant",      "BadgeVariant",    "Default",  "Visual style variant. Options: Default, Secondary, Destructive, Outline."),
                new("Class",        "string?",          null,       "Additional CSS classes to apply to the badge element."),
                new("ChildContent", "RenderFragment?",  null,       "The content to display inside the badge (text, icons, numbers, etc.)."),
            ];

        private const string _variantsCode =
                """
                <Badge Variant="BadgeVariant.Default">Default</Badge>
                <Badge Variant="BadgeVariant.Secondary">Secondary</Badge>
                <Badge Variant="BadgeVariant.Destructive">Destructive</Badge>
                <Badge Variant="BadgeVariant.Outline">Outline</Badge>
                """;

        private const string _usageCode =
                """
                <Badge Variant="BadgeVariant.Default">Active</Badge>
                <Badge Variant="BadgeVariant.Secondary">Pending</Badge>
                <Badge Variant="BadgeVariant.Destructive">Error</Badge>
                <Badge Variant="BadgeVariant.Outline">Draft</Badge>
                """;

        private const string _inListsCode =
                """
                <div class="flex items-center justify-between p-4">
                    <div>
                        <h4 class="text-sm font-medium">Project Alpha</h4>
                        <p class="text-sm text-muted-foreground">Active development</p>
                    </div>
                    <Badge Variant="BadgeVariant.Default">In Progress</Badge>
                </div>
                """;

        private const string _withIconsCode =
                """
                <Badge Variant="BadgeVariant.Default">
                    <LucideIcon Name="check" Size="12" Class="mr-1" />
                    Verified
                </Badge>
                <Badge Variant="BadgeVariant.Destructive">
                    <LucideIcon Name="x" Size="12" Class="mr-1" />
                    Closed
                </Badge>
                """;

        private const string _customStylingCode =
                """
                <Badge Variant="BadgeVariant.Default" Class="text-lg px-4 py-1">Large Badge</Badge>
                <Badge Variant="BadgeVariant.Secondary" Class="rounded-sm">Square Corners</Badge>
                <Badge Variant="BadgeVariant.Outline" Class="border-2 border-blue-500 text-blue-500">Custom Color</Badge>
                """;
    }
}
