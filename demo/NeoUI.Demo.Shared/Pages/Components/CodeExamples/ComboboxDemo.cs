namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ComboboxDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _comboboxProps =
            [
                new("Items",             "IEnumerable<TItem>",    null,    "The data source for the dropdown options."),
                new("Value",             "string?",               null,    "The selected item's value. Supports two-way binding with @bind-Value."),
                new("ValueSelector",     "Func<TItem, string>",   null,    "Function to extract the string value from an item."),
                new("DisplaySelector",   "Func<TItem, string>",   null,    "Function to extract the display label from an item."),
                new("Placeholder",       "string?",               null,    "Text shown on the trigger button when nothing is selected."),
                new("SearchPlaceholder", "string?",               null,    "Placeholder text in the search input."),
                new("EmptyMessage",      "string?",               null,    "Message shown when the search returns no results."),
                new("Disabled",          "bool",                  "false", "When true, the combobox cannot be opened."),
                new("MatchTriggerWidth", "bool",                  "false", "When true, the dropdown width matches the trigger width."),
                new("PopoverWidth",      "string?",               null,    "Tailwind CSS width class for the dropdown (e.g. w-[200px])."),
                new("Class",             "string?",               null,    "Additional CSS classes for the trigger button."),
            ];

        private const string _basicCode =
                """
                <Combobox TItem="Framework"
                         Items="frameworks"
                         @bind-Value="selectedValue"
                         ValueSelector="@(f => f.Value)"
                         DisplaySelector="@(f => f.Label)"
                         Placeholder="Select framework..."
                         SearchPlaceholder="Search framework..."
                         EmptyMessage="No framework found."
                         PopoverWidth="w-[200px]" />
                """;

        private const string _languageCode =
                """
                <Combobox TItem="ProgrammingLanguage"
                         Items="languages"
                         @bind-Value="selectedLanguage"
                         ValueSelector="@(l => l.Value)"
                         DisplaySelector="@(l => l.Label)"
                         Placeholder="Select language..."
                         PopoverWidth="w-[250px]" />
                """;

        private const string _countryCode =
                """
                <Combobox TItem="Country"
                         Items="countries"
                         @bind-Value="selectedCountry"
                         ValueSelector="@(c => c.Code)"
                         DisplaySelector="@(c => c.Name)"
                         Placeholder="Select country..."
                         PopoverWidth="w-[280px]" />
                """;

        private const string _matchWidthCode =
                """
                <Combobox TItem="Framework"
                         Items="frameworks"
                         @bind-Value="selectedValue"
                         ValueSelector="@(f => f.Value)"
                         DisplaySelector="@(f => f.Label)"
                         Placeholder="Select framework..."
                         MatchTriggerWidth="true"
                         Class="w-full" />
                """;

        private const string _disabledCode =
                """
                <Combobox TItem="Framework"
                         Items="frameworks"
                         @bind-Value="selectedValue"
                         ValueSelector="@(f => f.Value)"
                         DisplaySelector="@(f => f.Label)"
                         Placeholder="Disabled combobox"
                         Disabled="true"
                         PopoverWidth="w-[200px]" />
                """;

        private const string _formCode =
                """
                <div class="space-y-2">
                    <label class="text-sm font-medium">Framework</label>
                    <Combobox TItem="Framework"
                             Items="frameworks"
                             @bind-Value="formFramework"
                             ValueSelector="@(f => f.Value)"
                             DisplaySelector="@(f => f.Label)"
                             Placeholder="Select framework..."
                             Class="w-full"
                             MatchTriggerWidth="true" />
                </div>
                """;
    }
}
