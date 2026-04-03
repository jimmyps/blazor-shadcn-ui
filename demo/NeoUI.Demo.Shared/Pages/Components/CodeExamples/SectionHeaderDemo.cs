namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SectionHeaderDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _props =
        [
            new("Title",         "string?",          null,           "Section title text. Ignored when TitleContent is provided."),
            new("TitleContent",  "RenderFragment?",  null,           "Custom title fragment — takes precedence over Title."),
            new("OnViewAll",     "EventCallback",    null,           "Callback for the &quot;view all&quot; button. Button only renders when provided."),
            new("ViewAllText",   "string?",          null,           "Text shown next to the chevron. Omit for icon-only button."),
            new("ViewAllLabel",  "string",           "\"View all\"", "Accessible aria-label for the &quot;view all&quot; button."),
            new("ShowSeparator", "bool",             "true",         "Show a separator line below the title row."),
            new("Class",         "string?",          null,           "Additional CSS classes on the outer wrapper."),
        ];

        private const string _basicCode =
            """
            <SectionHeader Title="Recent Orders" />
            """;

        private const string _viewAllCode =
            """
            <SectionHeader Title="Featured Products"
                           OnViewAll="@ViewAll"
                           ViewAllText="See all" />

            @code {
                private void ViewAll() => NavigationManager.NavigateTo("/products");
            }
            """;

        private const string _chevronOnlyCode =
            """
            @* Omit ViewAllText for chevron-only button *@
            <SectionHeader Title="Trending Now" OnViewAll="@ViewAll" />
            """;

        private const string _noSepCode =
            """
            <SectionHeader Title="Popular" OnViewAll="@ViewAll" ViewAllText="All" ShowSeparator="false" />
            """;

        private const string _customTitleCode =
            """
            <SectionHeader OnViewAll="@ViewAll" ViewAllText="All">
                <TitleContent>
                    <div class="flex items-center gap-2">
                        <LucideIcon Name="flame" Class="h-4 w-4 text-orange-500" />
                        <span>Hot Deals</span>
                        <Badge Variant="BadgeVariant.Destructive" Class="text-[10px] px-1.5 py-0">NEW</Badge>
                    </div>
                </TitleContent>
            </SectionHeader>
            """;

        private const string _contextCode =
            """
            <SectionHeader Title="Continue Shopping" OnViewAll="@ViewAll" ViewAllText="See all" />
            @* product list *@

            <SectionHeader Title="Recommended For You" OnViewAll="@ViewAll" />
            @* recommendations *@

            <SectionHeader Title="Categories" ShowSeparator="false" />
            @* category chips *@
            """;
    }
}
