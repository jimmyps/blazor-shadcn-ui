namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SelectDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _selectProps =
            [
                new("TValue",       "generic",               "-",        "The value type (string, int, enum, etc.)."),
                new("Value",        "@bind-Value / TValue?", null,       "The selected value."),
                new("Class",        "string?",               null,       "Additional CSS classes applied to the root element."),
                new("Disabled",     "bool",                  "false",    "Disables the select."),
                new("Presentation", "SelectPresentation",    "Popover",  "How the options are presented. Popover (default) or BottomSheet (mobile drawer)."),
            ];

        private const string _basicCode =
                """
                <Select TValue="string" Class="w-[280px]">
                    <SelectTrigger>
                        <SelectValue Placeholder="Select a fruit" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem Value="apple" TValue="string">Apple</SelectItem>
                        <SelectItem Value="banana" TValue="string">Banana</SelectItem>
                        <SelectItem Value="blueberry" TValue="string">Blueberry</SelectItem>
                    </SelectContent>
                </Select>
                """;

        private const string _groupedCode =
                """
                <Select TValue="string" Class="w-[280px]">
                    <SelectTrigger>
                        <SelectValue Placeholder="Select an animal" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectGroup>
                            <SelectLabel>Mammals</SelectLabel>
                            <SelectItem Value="cat" TValue="string">Cat</SelectItem>
                            <SelectItem Value="dog" TValue="string">Dog</SelectItem>
                        </SelectGroup>
                        <SelectGroup>
                            <SelectLabel>Birds</SelectLabel>
                            <SelectItem Value="eagle" TValue="string">Eagle</SelectItem>
                        </SelectGroup>
                    </SelectContent>
                </Select>
                """;

        private const string _scrollableCode =
                """
                <Select TValue="string" Class="w-[280px]">
                    <SelectTrigger>
                        <SelectValue Placeholder="Select a country" />
                    </SelectTrigger>
                    <SelectContent>
                        @foreach (var country in countries)
                        {
                            <SelectItem Value="@country" TValue="string">@country</SelectItem>
                        }
                    </SelectContent>
                </Select>
                """;

        private const string _genericCode =
                """
                <Select TValue="int" Class="w-[280px]">
                    <SelectTrigger>
                        <SelectValue Placeholder="Select a number" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem Value="1" TValue="int">One</SelectItem>
                        <SelectItem Value="2" TValue="int">Two</SelectItem>
                        <SelectItem Value="3" TValue="int">Three</SelectItem>
                    </SelectContent>
                </Select>
                """;

        private const string _bottomSheetCode =
                """
                <Select @bind-Value="selectedCity" TValue="string" Class="w-[280px]">
                    <SelectTrigger>
                        <SelectValue Placeholder="Select a city" />
                    </SelectTrigger>
                    <SelectContent Presentation="SelectPresentation.BottomSheet">
                        <SelectItem Value="nyc" Text="New York" TValue="string">New York</SelectItem>
                        <SelectItem Value="london" Text="London" TValue="string">London</SelectItem>
                        <SelectItem Value="tokyo" Text="Tokyo" TValue="string">Tokyo</SelectItem>
                        <SelectItem Value="paris" Text="Paris" TValue="string">Paris</SelectItem>
                    </SelectContent>
                </Select>
                """;
    }
}
