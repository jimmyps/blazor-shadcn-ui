namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class MultiSelectDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _multiSelectProps =
            [
                new("Items", "IEnumerable<TItem>", "—", "The list of items to display."),
                new("Values", "IEnumerable<TValue>?", "null", "Selected values. Use @bind-Values for two-way binding."),
                new("ValueSelector", "Func<TItem, TValue>", "—", "Function to get the value from an item."),
                new("DisplaySelector", "Func<TItem, string>", "—", "Function to get the display label from an item."),
                new("Placeholder", "string?", "null", "Placeholder text when nothing is selected."),
                new("SearchPlaceholder", "string?", "null", "Placeholder text in the search input."),
                new("EmptyMessage", "string?", "null", "Message shown when no items match the search."),
                new("ShowSelectAll", "bool", "false", "Whether to show a \"Select All\" option."),
                new("SelectAllLabel", "string", "\"Select All\"", "Label for the select-all option."),
                new("MaxDisplayTags", "int", "3", "Maximum number of tags to show before \"+N more\"."),
                new("AutoClose", "bool", "true", "Whether the dropdown closes when clicking outside."),
                new("MatchTriggerWidth", "bool", "false", "Whether the dropdown matches the trigger's width."),
                new("PopoverWidth", "string?", "null", "CSS width class for the dropdown (e.g. \"w-[300px]\")."),
                new("Disabled", "bool", "false", "Whether the component is disabled."),
                new("Class", "string?", "null", "Additional CSS classes for the trigger element."),
            ];

        private const string _basicCode = """
                <MultiSelect TItem="Language"
                             Items="languages"
                             Values="selectedLanguages"
                             ValueSelector="@(l => l.Value)"
                             DisplaySelector="@(l => l.Label)"
                             Placeholder="Select languages..."
                             SearchPlaceholder="Search languages..."
                             EmptyMessage="No language found."
                             PopoverWidth="w-[300px]" />
                """;

        private const string _techStackCode = """
                <MultiSelect TItem="Technology"
                             Items="technologies"
                             Values="selectedTechnologies"
                             ValueSelector="@(t => t.Value)"
                             DisplaySelector="@(t => t.Label)"
                             Placeholder="Select technologies..."
                             ShowSelectAll="true"
                             SelectAllLabel="Select All"
                             MaxDisplayTags="3"
                             PopoverWidth="w-[320px]" />
                """;

        private const string _countryCode = """
                <MultiSelect TItem="Country"
                             Items="countries"
                             Values="selectedCountries"
                             ValueSelector="@(c => c.Code)"
                             DisplaySelector="@(c => c.Name)"
                             Placeholder="Select countries..."
                             MaxDisplayTags="2"
                             ShowSelectAll="true"
                             PopoverWidth="w-[280px]" />
                """;

        private const string _withoutSelectAllCode = """
                <MultiSelect TItem="Framework"
                             Items="frameworks"
                             Values="selectedFrameworks"
                             ValueSelector="@(f => f.Value)"
                             DisplaySelector="@(f => f.Label)"
                             Placeholder="Select frameworks..."
                             ShowSelectAll="false"
                             PopoverWidth="w-[250px]" />
                """;

        private const string _autoCloseCode = """
                <MultiSelect TItem="Framework"
                             Items="frameworks"
                             Values="autoCloseFrameworks"
                             ValueSelector="@(f => f.Value)"
                             DisplaySelector="@(f => f.Label)"
                             Placeholder="Select frameworks..."
                             AutoClose="false"
                             PopoverWidth="w-[280px]" />
                """;

        private const string _matchWidthCode = """
                <div class="w-72">
                    <MultiSelect TItem="Language"
                                 Items="languages"
                                 Values="matchWidthLanguages"
                                 ValueSelector="@(l => l.Value)"
                                 DisplaySelector="@(l => l.Label)"
                                 Placeholder="Select languages..."
                                 MatchTriggerWidth="true"
                                 Class="w-full" />
                </div>
                """;

        private const string _disabledCode = """
                <MultiSelect TItem="Language"
                             Items="languages"
                             Values="disabledValues"
                             ValueSelector="@(l => l.Value)"
                             DisplaySelector="@(l => l.Label)"
                             Placeholder="Disabled multiselect"
                             Disabled="true"
                             PopoverWidth="w-[300px]" />
                """;

        private const string _formCode = """
                <div class="space-y-4 border rounded-lg p-6">
                    <div class="space-y-2">
                        <label class="text-sm font-medium">Skills</label>
                        <MultiSelect TItem="Language"
                                     Items="languages"
                                     Values="formSkills"
                                     ValueSelector="@(l => l.Value)"
                                     DisplaySelector="@(l => l.Label)"
                                     Placeholder="Select your skills..."
                                     ShowSelectAll="true"
                                     MatchTriggerWidth="true" />
                    </div>
                    <div class="space-y-2">
                        <label class="text-sm font-medium">Preferred Frameworks</label>
                        <MultiSelect TItem="Framework"
                                     Items="frameworks"
                                     Values="formFrameworks"
                                     ValueSelector="@(f => f.Value)"
                                     DisplaySelector="@(f => f.Label)"
                                     Placeholder="Select frameworks..."
                                     MaxDisplayTags="2"
                                     MatchTriggerWidth="true" />
                    </div>
                    <Button Variant="ButtonVariant.Default">Submit</Button>
                </div>
                """;
    }
}
