namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ToggleGroupDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _toggleGroupProps =
            [
                new("Type",   "ToggleGroupType",     "Single", "Single or Multiple selection mode."),
                new("Value",  "@bind-Value / string?", null,   "Selected value in Single mode."),
                new("Values", "@bind-Values / List&lt;string&gt;?", null, "Selected values in Multiple mode."),
            ];

        private const string _singleCode =
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

        private const string _multipleCode =
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

        private const string _formattingCode =
                """
                <ToggleGroup Type="ToggleGroupType.Multiple" @bind-Values="formatting">
                    <ToggleGroupItem Value="bold">
                        <LucideIcon Name="bold" Class="h-4 w-4" />
                    </ToggleGroupItem>
                    <ToggleGroupItem Value="italic">
                        <LucideIcon Name="italic" Class="h-4 w-4" />
                    </ToggleGroupItem>
                    <ToggleGroupItem Value="strikethrough">
                        <LucideIcon Name="strikethrough" Class="h-4 w-4" />
                    </ToggleGroupItem>
                </ToggleGroup>
                """;
    }
}
