namespace NeoUI.Demo.Shared.Pages.Components;

partial class LocalizationDemo
{
    private static class LocalizationDemoCode
    {
        public const string LivePreview =
            """
            @* Build a DefaultLocalizer for the selected language, then pass strings
               as explicit parameters — they override ILocalizer on a per-instance basis. *@

            <ToggleGroup Type="ToggleGroupType.Single" Value="@_lang" ValueChanged="OnLangChanged">
                <ToggleGroupItem Value="en">🇺🇸 English</ToggleGroupItem>
                <ToggleGroupItem Value="de">🇩🇪 Deutsch</ToggleGroupItem>
                <ToggleGroupItem Value="fr">🇫🇷 Français</ToggleGroupItem>
                <ToggleGroupItem Value="es">🇪🇸 Español</ToggleGroupItem>
                <ToggleGroupItem Value="ja">🇯🇵 日本語</ToggleGroupItem>
            </ToggleGroup>

            <Combobox TItem="string" Items="@items" ValueSelector="@(s => s)" DisplaySelector="@(s => s)"
                      Placeholder="@_loc["Combobox.Placeholder"]"
                      SearchPlaceholder="@_loc["Combobox.SearchPlaceholder"]"
                      EmptyMessage="@_loc["Combobox.EmptyMessage"]" />

            <MultiSelect TItem="string" Items="@items" ValueSelector="@(s => s)" DisplaySelector="@(s => s)"
                         @bind-Values="_selected"
                         Placeholder="@_loc["MultiSelect.Placeholder"]"
                         SearchPlaceholder="@_loc["MultiSelect.SearchPlaceholder"]"
                         EmptyMessage="@_loc["MultiSelect.EmptyMessage"]"
                         SelectAllLabel="@_loc["MultiSelect.SelectAll"]"
                         ClearLabel="@_loc["MultiSelect.Clear"]"
                         CloseLabel="@_loc["MultiSelect.Close"]" />

            <Pagination>
                <PaginationContent>
                    <PaginationItem><PaginationPrevious Href="#" Text="@_loc["Pagination.Previous"]" /></PaginationItem>
                    <PaginationItem><PaginationLink Href="#" IsActive="true">1</PaginationLink></PaginationItem>
                    <PaginationItem><PaginationNext Href="#" Text="@_loc["Pagination.Next"]" /></PaginationItem>
                </PaginationContent>
            </Pagination>

            @* Culture-aware components (Calendar, DatePicker, DateRangePicker, NumericInput)
               use CultureInfo.CurrentCulture automatically — no ILocalizer keys needed. *@

            @code {
                private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
                {
                    ["en"] = new() { ["Combobox.Placeholder"] = "Select an option...", ... },
                    ["de"] = new() { ["Combobox.Placeholder"] = "Option auswählen...",  ... },
                    // ... other languages
                };

                private string _lang = "en";
                private ILocalizer _loc = BuildLocalizer("en");

                private static ILocalizer BuildLocalizer(string lang)
                {
                    var loc = new DefaultLocalizer();
                    foreach (var (key, val) in _translations[lang])
                        loc.Set(key, val);
                    return loc;
                }

                private void OnLangChanged(string? lang)
                {
                    _lang = lang ?? "en";
                    _loc = BuildLocalizer(_lang);
                }
            }
            """;

        public const string DefaultBehavior =
            """
            @* No explicit string params — all strings come from ILocalizer *@
            <Combobox TItem="string"
                      Items="@frameworks"
                      ValueSelector="@(s => s)"
                      DisplaySelector="@(s => s)"
                      PopoverWidth="w-[260px]" />

            <TagInput @bind-Tags="_tags" />

            <Pagination>
                <PaginationContent>
                    <PaginationItem><PaginationPrevious Href="#" /></PaginationItem>
                    <PaginationItem><PaginationLink Href="#" IsActive="true">1</PaginationLink></PaginationItem>
                    <PaginationItem><PaginationNext Href="#" /></PaginationItem>
                </PaginationContent>
            </Pagination>
            """;

        public const string PerInstanceOverride =
            """
            @* Explicit string params take precedence over the localizer *@
            <Combobox TItem="string"
                      Items="@frameworks"
                      ValueSelector="@(s => s)"
                      DisplaySelector="@(s => s)"
                      Placeholder="Framework auswählen..."
                      SearchPlaceholder="Suchen..."
                      EmptyMessage="Keine Ergebnisse gefunden."
                      PopoverWidth="w-[260px]" />

            <Pagination>
                <PaginationContent>
                    <PaginationItem><PaginationPrevious Href="#" Text="Zurück" /></PaginationItem>
                    <PaginationItem><PaginationLink Href="#" IsActive="true">1</PaginationLink></PaginationItem>
                    <PaginationItem><PaginationNext Href="#" Text="Weiter" /></PaginationItem>
                </PaginationContent>
            </Pagination>
            """;

        public const string ScopedVsSingleton =
            """
            // Default: Scoped — one instance per circuit (Blazor Server) or session (WebAssembly).
            // Required for Option B so each user's CultureInfo is resolved independently.
            builder.Services.AddNeoUIComponents();
            builder.Services.AddScoped<ILocalizer, AppLocalizer>(); // overrides the default Scoped registration

            // Singleton — one shared instance for the entire app lifetime.
            // Safe when all users share the same language (Option A or a fixed custom localizer).
            // Must be registered AFTER AddNeoUIComponents() to override the default Scoped registration.
            builder.Services.AddNeoUIComponents();
            builder.Services.AddSingleton<ILocalizer>(_ =>
            {
                var loc = new DefaultLocalizer();
                loc.Set("Combobox.Placeholder", "Wählen Sie eine Option...");
                loc.Set("Pagination.Previous",  "Zurück");
                loc.Set("Pagination.Next",      "Weiter");
                return loc;
            });
            """;

        public const string PatternA =
            """
            // Program.cs — startup-time key overrides (Option A)
            builder.Services.AddNeoUIComponents(localizer =>
            {
                localizer.Set("Combobox.Placeholder",      "Wählen Sie eine Option...");
                localizer.Set("Combobox.SearchPlaceholder", "Suchen...");
                localizer.Set("Combobox.EmptyMessage",      "Keine Ergebnisse gefunden.");
                localizer.Set("DataTable.Loading",          "Laden...");
                localizer.Set("Pagination.Previous",        "Zurück");
                localizer.Set("Pagination.Next",            "Weiter");
            });
            """;

        public const string PatternB =
            """
            // AppLocalizer.cs — full IStringLocalizer<T> integration (Option B)
            public class AppLocalizer(IStringLocalizer<SharedResources> loc) : DefaultLocalizer
            {
                public override string this[string key] =>
                    loc[key] is { ResourceNotFound: false } found ? found.Value : base[key];

                public override string this[string key, params object[] arguments] =>
                    loc[key, arguments] is { ResourceNotFound: false } found
                        ? found.Value
                        : base[key, arguments];
            }

            // Program.cs — register after AddNeoUIComponents() to override the default scoped registration
            builder.Services.AddNeoUIComponents();
            builder.Services.AddScoped<ILocalizer, AppLocalizer>();
            """;

        public const string LiveLookup =
            """
            @inject ILocalizer Localizer

            <Input @bind-Value="_key" Placeholder="e.g. Combobox.Placeholder" />
            <Button OnClick="@(() => _result = Localizer[_key])">Resolve</Button>

            @if (_result is not null)
            {
                <p>@_key → @_result</p>
            }
            """;
    }
}
