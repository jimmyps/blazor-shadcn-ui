namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SeparatorDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _separatorProps =
            [
                new("Orientation", "SeparatorOrientation", "Horizontal", "The orientation of the separator. Options: Horizontal, Vertical."),
                new("Decorative",  "bool",                 "true",       "When true, the separator is purely decorative and hidden from screen readers. Set to false for semantic separators."),
                new("Class",       "string?",              null,         "Additional CSS classes appended to the separator element."),
            ];

        private const string _horizontalCode =
                """
                <div>
                    <h3 class="text-sm font-medium">NeoUI</h3>
                    <p class="text-sm text-muted-foreground">A Blazor UI component library</p>
                </div>
                <Separator Orientation="SeparatorOrientation.Horizontal" />
                <div>
                    <h4 class="text-sm font-medium leading-none">Components</h4>
                </div>
                """;

        private const string _verticalCode =
                """
                <div class="flex h-20 items-center space-x-4 text-sm">
                    <div>Button</div>
                    <Separator Orientation="SeparatorOrientation.Vertical" />
                    <div>Input</div>
                    <Separator Orientation="SeparatorOrientation.Vertical" />
                    <div>Badge</div>
                </div>
                """;

        private const string _ariaCode =
                """
                <!-- Decorative: hidden from screen readers -->
                <Separator Decorative="true" />

                <!-- Semantic: announced to screen readers -->
                <Separator Decorative="false" />
                """;

        private const string _stylingCode =
                """
                <Separator Class="my-4" />
                <Separator Class="my-4 h-[2px]" />
                <Separator Class="my-4 bg-primary" />
                <Separator Class="my-4 border-t border-dashed border-border h-0" />
                """;

        private const string _listCode =
                """
                <div class="border rounded-lg">
                    <div class="p-4"><h3 class="font-medium">Profile Settings</h3></div>
                    <Separator />
                    <div class="p-4"><h3 class="font-medium">Preferences</h3></div>
                    <Separator />
                    <div class="p-4"><h3 class="font-medium">Notifications</h3></div>
                    <Separator />
                    <div class="p-4"><h3 class="font-medium">Security</h3></div>
                </div>
                """;
    }
}
