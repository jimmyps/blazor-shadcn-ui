namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class MultiSelectDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _multiSelectProps =
            [
                new("Items", "IEnumerable<TItem>", "—", "The list of items to display."),
                new("Values", "IEnumerable<string>?", "null", "Selected values. Use @bind-Values for two-way binding."),
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
                new("SearchQueryChanged", "EventCallback<string>", "null", "Invoked on every search keystroke. When set, bypasses the internal filter — consumer controls Items."),
                new("OnLoadMore", "EventCallback", null, "Invoked when the user scrolls near the bottom of the list. Use to append additional items."),
                new("IsLoading", "bool", "false", "Shows a spinner at the bottom of the list while the next batch is loading."),
                new("EndOfListMessage", "string?", "null", "Message shown at the bottom when all items have been loaded. Hidden when null or empty."),
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

        private const string _infiniteScrollCode = """
                <MultiSelect TItem="NumberedMsItem"
                             Items="@_msPagedItems"
                             @bind-Values="_msPagedValues"
                             ValueSelector="@(x => x.Value)"
                             DisplaySelector="@(x => x.Label)"
                             Placeholder="Select options..."
                             SearchPlaceholder="Search options..."
                             OnLoadMore="@HandleMsLoadMore"
                             IsLoading="@_msPagedLoading"
                             EndOfListMessage="@MsEndMessage"
                             PopoverWidth="w-[300px]" />

                @code {
                    private List<NumberedMsItem> _msPagedItems = new();
                    private bool _msPagedLoading;
                    private int _msPagedPage;
                    private const int PageSize = 10;
                    private const int Total = 50;

                    protected override void OnInitialized() => LoadPage();

                    private void LoadPage()
                    {
                        var start = _msPagedPage++ * PageSize;
                        for (var i = start; i < Math.Min(start + PageSize, Total); i++)
                            _msPagedItems.Add(new NumberedMsItem { Value = $"ms-item-{i+1}", Label = $"Option {i+1}" });
                    }

                    private async Task HandleMsLoadMore()
                    {
                        if (_msPagedLoading || _msPagedItems.Count >= Total) return;
                        _msPagedLoading = true;
                        await Task.Delay(600);
                        LoadPage();
                        _msPagedLoading = false;
                    }

                    private string? MsEndMessage =>
                        _msPagedItems.Count >= Total ? $"All {Total} options loaded" : null;
                }
                """;

        private const string _searchQueryChangedCode = """
                <MultiSelect TItem="PersonItem"
                             Items="@_filteredPeople"
                             @bind-Values="_selectedPeople"
                             ValueSelector="@(p => p.Value)"
                             DisplaySelector="@(p => p.Label)"
                             Placeholder="Search and select people..."
                             SearchPlaceholder="Type to search..."
                             SearchQueryChanged="@HandlePeopleSearch"
                             IsLoading="@_peopleSearching"
                             PopoverWidth="w-[320px]" />

                @code {
                    // _allPeople: pre-loaded list of all people
                    private List<PersonItem> _filteredPeople = new(_allPeople);
                    private bool _peopleSearching;

                    private async Task HandlePeopleSearch(string query)
                    {
                        _peopleSearching = true;
                        await Task.Delay(300); // simulate server round-trip
                        _filteredPeople = string.IsNullOrWhiteSpace(query)
                            ? new(_allPeople)
                            : _allPeople
                                .Where(p => p.Label.Contains(query, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                        _peopleSearching = false;
                    }
                }
                """;
    }
}
