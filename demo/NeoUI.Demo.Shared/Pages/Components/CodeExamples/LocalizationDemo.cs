namespace NeoUI.Demo.Shared.Pages.Components;

partial class LocalizationDemo
{
    private static class LocalizationDemoCode
    {
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

        public const string PatternA =
            """
            // Program.cs — startup-time key overrides (Pattern A)
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
            // AppLocalizer.cs — full IStringLocalizer<T> integration (Pattern B)
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
