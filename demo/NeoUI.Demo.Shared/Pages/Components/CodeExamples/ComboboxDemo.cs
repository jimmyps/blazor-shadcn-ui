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
                new("OnLoadMore",        "EventCallback",         null,    "Invoked when the user scrolls near the bottom of the list. Use to load additional items."),
                new("IsLoading",         "bool",                  "false", "Shows a spinner at the bottom of the list while the next batch is loading."),
                new("EndOfListMessage",  "string?",               null,    "Message shown at the bottom when all items have been loaded. Hidden when null or empty."),
                new("SearchQueryChanged","EventCallback<string>", null,    "Invoked on every search keystroke. When set, bypasses the built-in text filter — the consumer controls Items."),
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

        private const string _infiniteScrollCode =
                """
                <Combobox TItem="NumberedItem"
                         Items="@_pagedItems"
                         @bind-Value="_pagedSelection"
                         ValueSelector="@(x => x.Value)"
                         DisplaySelector="@(x => x.Label)"
                         Placeholder="Select an option..."
                         SearchPlaceholder="Search options..."
                         OnLoadMore="@HandleLoadMore"
                         IsLoading="@_isLoading"
                         EndOfListMessage="@_endOfListMessage"
                         PopoverWidth="w-[240px]" />

                @code {
                    private List<NumberedItem> _pagedItems = new();
                    private bool _isLoading;
                    private int _page;
                    private const int PageSize = 10;
                    private const int Total = 50;

                    protected override void OnInitialized() => LoadPage();

                    private void LoadPage()
                    {
                        var start = _page++ * PageSize;
                        for (var i = start; i < Math.Min(start + PageSize, Total); i++)
                            _pagedItems.Add(new NumberedItem { Value = $"item-{i+1}", Label = $"Option {i+1}" });
                    }

                    private async Task HandleLoadMore()
                    {
                        if (_isLoading || _pagedItems.Count >= Total) return;
                        _isLoading = true;
                        await Task.Delay(600); // simulate network
                        LoadPage();
                        _isLoading = false;
                    }

                    private string? _endOfListMessage =>
                        _pagedItems.Count >= Total ? $"All {Total} options loaded" : null;
                }
                """;

        private const string _asyncFilterCode = """
                <Combobox TItem="Framework"
                         Items="@_asyncFilteredFrameworks"
                         @bind-Value="_asyncSelectedFramework"
                         ValueSelector="@(f => f.Value)"
                         DisplaySelector="@(f => f.Label)"
                         Placeholder="Select framework..."
                         SearchPlaceholder="Type to search..."
                         SearchQueryChanged="@HandleFrameworkSearch"
                         IsLoading="@_frameworkSearching"
                         PopoverWidth="w-[240px]" />

                @code {
                    // _allFrameworks: pre-loaded list of all frameworks
                    private List<Framework> _asyncFilteredFrameworks = new(_allFrameworks);
                    private bool _frameworkSearching;

                    private async Task HandleFrameworkSearch(string query)
                    {
                        _frameworkSearching = true;
                        await Task.Delay(300); // simulate server round-trip
                        _asyncFilteredFrameworks = string.IsNullOrWhiteSpace(query)
                            ? new(_allFrameworks)
                            : _allFrameworks
                                .Where(f => f.Label.Contains(query, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                        _frameworkSearching = false;
                    }
                }
                """;
    }
}
