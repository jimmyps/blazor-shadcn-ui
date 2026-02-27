namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ToggleDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _toggleProps =
            [
                new("Pressed",  "@bind-Pressed / bool", "false",           "Whether the toggle is pressed."),
                new("Variant",  "ToggleVariant",         "Default",         "Default or Outline visual style."),
                new("Size",     "ToggleSize",             "Default",         "Small, Default, or Large."),
                new("Disabled", "bool",                   "false",           "Disables the toggle."),
            ];

        private const string _basicCode =
                """
                <Toggle>
                    <LucideIcon Name="bold" Class="h-4 w-4" />
                </Toggle>
                """;

        private const string _variantsCode =
                """
                <Toggle Variant="ToggleVariant.Default">Default</Toggle>
                <Toggle Variant="ToggleVariant.Outline">Outline</Toggle>
                """;

        private const string _sizesCode =
                """
                <Toggle Size="ToggleSize.Small">Small</Toggle>
                <Toggle Size="ToggleSize.Default">Default</Toggle>
                <Toggle Size="ToggleSize.Large">Large</Toggle>
                """;

        private const string _singleGroupCode =
                """
                <ToggleGroup Type="ToggleGroupType.Single" @bind-Value="alignment">
                    <ToggleGroupItem Value="left">
                        <LucideIcon Name="align-left" Class="h-4 w-4" />
                    </ToggleGroupItem>
                    <ToggleGroupItem Value="center">
                        <LucideIcon Name="align-center" Class="h-4 w-4" />
                    </ToggleGroupItem>
                    <ToggleGroupItem Value="right">
                        <LucideIcon Name="align-right" Class="h-4 w-4" />
                    </ToggleGroupItem>
                </ToggleGroup>
                """;

        private const string _multipleGroupCode =
                """
                <ToggleGroup Type="ToggleGroupType.Multiple" @bind-Values="styles">
                    <ToggleGroupItem Value="bold">
                        <LucideIcon Name="bold" Class="h-4 w-4" />
                    </ToggleGroupItem>
                    <ToggleGroupItem Value="italic">
                        <LucideIcon Name="italic" Class="h-4 w-4" />
                    </ToggleGroupItem>
                    <ToggleGroupItem Value="underline">
                        <LucideIcon Name="underline" Class="h-4 w-4" />
                    </ToggleGroupItem>
                </ToggleGroup>
                """;
    }
}
