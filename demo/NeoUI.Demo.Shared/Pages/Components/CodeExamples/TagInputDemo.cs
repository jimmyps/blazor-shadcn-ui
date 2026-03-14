namespace NeoUI.Demo.Shared.Pages.Components;

partial class TagInputDemo
{
    private static readonly IReadOnlyList<string> _frozenTags = ["blazor", "dotnet", "csharp"];

    private static readonly string[] _allTech =
    [
        "Blazor", "C#", ".NET", "ASP.NET", "Razor", "MAUI", "Azure", "SignalR",
        "Entity Framework", "Minimal APIs", "gRPC", "TypeScript", "React", "Vue",
        "Tailwind CSS", "Docker", "Kubernetes"
    ];

    private static readonly string[] _countries =
    [
        "Australia", "Austria", "Belgium", "Brazil", "Canada", "China", "Denmark",
        "Finland", "France", "Germany", "India", "Ireland", "Italy", "Japan",
        "Netherlands", "New Zealand", "Norway", "Poland", "Portugal", "Spain",
        "Sweden", "Switzerland", "United Kingdom", "United States"
    ];

    private static Task<IEnumerable<string>> FilterTech(string query, CancellationToken ct) =>
        Task.FromResult(_allTech.Where(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)));

    private static readonly IReadOnlyList<DemoPropRow> _props =
    [
        new("Tags",                 "IReadOnlyList<string>?",                                "null",          "Current tag list (use @bind-Tags for two-way binding)."),
        new("TagsChanged",          "EventCallback<IReadOnlyList<string>?>",                 "",              "Fires when tags change."),
        new("Placeholder",          "string?",                                               "null",          "Input placeholder text."),
        new("AddTrigger",           "TagInputTrigger",                                       "Enter | Comma", "Flag combination of keys that submit a tag."),
        new("MaxTags",              "int",                                                   "int.MaxValue",  "Maximum allowed tag count."),
        new("MaxTagLength",         "int",                                                   "50",            "Maximum characters per tag."),
        new("AllowDuplicates",      "bool",                                                  "false",         "Whether the same value can be added more than once."),
        new("Suggestions",          "IEnumerable<string>?",                                  "null",          "Static suggestion list shown while typing."),
        new("OnSearchSuggestions",  "Func<string,CancellationToken,Task<IEnumerable<string>>>?", "null",      "Async callback for dynamic suggestions."),
        new("SuggestionDebounceMs", "int",                                                   "300",           "Debounce delay in ms for OnSearchSuggestions."),
        new("Variant",              "TagInputVariant",                                       "Default",       "Tag appearance: Default, Outline, Secondary."),
        new("Clearable",            "bool",                                                  "false",         "Shows a clear-all button when tags are present."),
        new("Disabled",             "bool",                                                  "false",         "Disables all interaction."),
        new("TagTemplate",          "RenderFragment<string>?",                               "null",          "Custom render template for each tag chip."),
        new("Validate",             "Func<string, bool>?",                                   "null",          "Custom validation predicate — returning false rejects the tag."),
        new("OnTagRejected",        "EventCallback<string>",                                 "",              "Fires when a tag fails validation or is a duplicate."),
    ];

    private static class TagInputDemoCode
    {
        public const string Basic =
            """
            <TagInput @bind-Tags="_tags" Placeholder="Add a tag…" />
            """;

        public const string Suggestions =
            """
            <TagInput @bind-Tags="_tags"
                      Placeholder="Add technology…"
                      OnSearchSuggestions="@FilterTech" />

            @code {
                private Task<IEnumerable<string>> FilterTech(string q, CancellationToken ct)
                    => Task.FromResult(_allTech.Where(t => t.Contains(q, StringComparison.OrdinalIgnoreCase)));
            }
            """;

        public const string StaticSuggestions =
            """
            <TagInput @bind-Tags="_tags"
                      Placeholder="Add a country…"
                      Suggestions="@_countries" />
            """;

        public const string Constraints =
            """
            <TagInput @bind-Tags="_tags"
                      Placeholder="Up to 4 tags…"
                      MaxTags="4"
                      AllowDuplicates="false" />
            """;

        public const string Triggers =
            """
            <TagInput @bind-Tags="_tags"
                      Placeholder="Enter or Tab to add…"
                      AddTrigger="TagInputTrigger.Enter | TagInputTrigger.Tab" />
            """;

        public const string Variants =
            """
            <TagInput @bind-Tags="_tags1" Placeholder="Default…"    Variant="TagInputVariant.Default"   />
            <TagInput @bind-Tags="_tags2" Placeholder="Outline…"    Variant="TagInputVariant.Outline"   />
            <TagInput @bind-Tags="_tags3" Placeholder="Secondary…"  Variant="TagInputVariant.Secondary" />
            """;

        public const string Clearable =
            """
            <TagInput @bind-Tags="_tags" Placeholder="Add tags then clear all…" Clearable="true" />
            """;

        public const string Disabled =
            """
            <TagInput Tags="@_frozenTags" Disabled="true" Placeholder="Disabled" />
            """;
    }
}
