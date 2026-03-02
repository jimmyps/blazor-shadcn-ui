namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ItemDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _itemProps =
            [
                new("Variant", "ItemVariant", "Default", "Visual style: Default, Outline, Muted."),
                new("Size", "ItemSize", "Default", "Size: Default, Sm."),
                new("AsChild", "string?", "null", "Renders the item as the given HTML element (e.g. \"a\", \"button\")."),
                new("Href", "string?", "null", "URL for link rendering (when AsChild=\"a\")."),
                new("Class", "string?", "null", "Additional CSS classes."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _subComponentProps =
            [
                new("ItemContent — Class", "string?", "null", "Additional CSS classes."),
                new("ItemMedia — Variant", "ItemMediaVariant", "Default", "Visual style for the media area: Default, Icon, Image."),
                new("ItemMedia — Class", "string?", "null", "Additional CSS classes."),
                new("ItemActions — Class", "string?", "null", "Additional CSS classes."),
                new("ItemHeader — Class", "string?", "null", "Additional CSS classes."),
                new("ItemFooter — Class", "string?", "null", "Additional CSS classes."),
            ];

        private const string _basicCode = """
                <Item Variant="ItemVariant.Outline">
                    <ItemContent>
                        <ItemTitle>Basic Item</ItemTitle>
                        <ItemDescription>A simple item with title and description.</ItemDescription>
                    </ItemContent>
                    <ItemActions>
                        <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">Action</Button>
                    </ItemActions>
                </Item>
                """;

        private const string _notificationCode = """
                <ItemGroup>
                    <Item>
                        <ItemMedia Variant="ItemMediaVariant.Icon">
                            <LucideIcon Name="circle-check" Size="18" Class="text-green-600" />
                        </ItemMedia>
                        <ItemContent>
                            <ItemTitle>Successfully saved</ItemTitle>
                            <ItemDescription>Your changes have been saved to the database.</ItemDescription>
                        </ItemContent>
                        <ItemActions>
                            <span class="text-xs text-muted-foreground">2m ago</span>
                        </ItemActions>
                    </Item>
                    <ItemSeparator />
                    <!-- more items... -->
                </ItemGroup>
                """;

        private const string _settingsCode = """
                <ItemGroup>
                    <Item Variant="ItemVariant.Outline" Size="ItemSize.Sm" AsChild="a" Href="#profile">
                        <ItemMedia>
                            <LucideIcon Name="user" Size="18" />
                        </ItemMedia>
                        <ItemContent>
                            <ItemTitle>Profile Settings</ItemTitle>
                        </ItemContent>
                        <ItemActions>
                            <LucideIcon Name="chevron-right" Size="16" />
                        </ItemActions>
                    </Item>
                    <!-- more items... -->
                </ItemGroup>
                """;

        private const string _userProfileCode = """
                <ItemGroup>
                    <Item Variant="ItemVariant.Outline">
                        <ItemMedia Variant="ItemMediaVariant.Image">
                            <Avatar><AvatarFallback>JD</AvatarFallback></Avatar>
                        </ItemMedia>
                        <ItemContent>
                            <ItemTitle>John Doe <Badge>Admin</Badge></ItemTitle>
                            <ItemDescription>john.doe@example.com</ItemDescription>
                        </ItemContent>
                        <ItemActions>
                            <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">View Profile</Button>
                        </ItemActions>
                    </Item>
                    <!-- more items... -->
                </ItemGroup>
                """;

        private const string _variantsCode = """
                <Item Variant="ItemVariant.Default">
                    <ItemContent>
                        <ItemTitle>Default Variant</ItemTitle>
                        <ItemDescription>Hover to see the subtle background effect.</ItemDescription>
                    </ItemContent>
                </Item>
                <Item Variant="ItemVariant.Outline">
                    <ItemContent>
                        <ItemTitle>Outline Variant</ItemTitle>
                        <ItemDescription>Has a border and background on hover.</ItemDescription>
                    </ItemContent>
                </Item>
                <Item Variant="ItemVariant.Muted">
                    <ItemContent>
                        <ItemTitle>Muted Variant</ItemTitle>
                        <ItemDescription>Subtle background color with darker hover state.</ItemDescription>
                    </ItemContent>
                </Item>
                """;

        private const string _sizesCode = """
                <Item Variant="ItemVariant.Outline" Size="ItemSize.Default">
                    <ItemContent>
                        <ItemTitle>Default Size</ItemTitle>
                        <ItemDescription>Standard padding for most use cases.</ItemDescription>
                    </ItemContent>
                </Item>
                <Item Variant="ItemVariant.Outline" Size="ItemSize.Sm">
                    <ItemContent>
                        <ItemTitle>Small Size</ItemTitle>
                        <ItemDescription>Compact padding for dense layouts.</ItemDescription>
                    </ItemContent>
                </Item>
                """;

        private const string _asChildCode = """
                <!-- As anchor tag -->
                <Item Variant="ItemVariant.Outline" AsChild="a" Href="https://github.com" target="_blank">
                    <ItemMedia><LucideIcon Name="github" Size="18" /></ItemMedia>
                    <ItemContent>
                        <ItemTitle>View on GitHub</ItemTitle>
                    </ItemContent>
                    <ItemActions><LucideIcon Name="external-link" Size="16" /></ItemActions>
                </Item>

                <!-- As button -->
                <Item Variant="ItemVariant.Outline" AsChild="button">
                    <ItemMedia><LucideIcon Name="star" Size="18" /></ItemMedia>
                    <ItemContent>
                        <ItemTitle>Mark as Favorite</ItemTitle>
                    </ItemContent>
                </Item>
                """;

        private const string _complexLayoutCode = """
                <Item Variant="ItemVariant.Outline" Class="flex-wrap">
                    <ItemMedia Variant="ItemMediaVariant.Image">
                        <Avatar><AvatarFallback>TR</AvatarFallback></Avatar>
                    </ItemMedia>
                    <ItemContent>
                        <ItemTitle>Task Report</ItemTitle>
                        <ItemDescription>Monthly summary of completed tasks and projects.</ItemDescription>
                    </ItemContent>
                    <ItemActions>
                        <Badge>New</Badge>
                    </ItemActions>
                    <ItemFooter Class="mt-3">
                        <div class="flex gap-2">
                            <Button Variant="ButtonVariant.Default" Size="ButtonSize.Small">Download PDF</Button>
                            <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">Share</Button>
                        </div>
                        <span class="text-xs text-muted-foreground">Generated today</span>
                    </ItemFooter>
                </Item>
                """;
    }
}
