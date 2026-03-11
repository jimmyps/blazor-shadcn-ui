namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class StatePersistenceExample
{
    private static readonly string _persistenceCode = """
        @inject IJSRuntime JS
        @using System.Text.Json

        <FilterBuilder TData="MyModel"
                       @bind-Filters="_filters"
                       OnFilterChange="HandleFilterChange">
            <FilterFields>
                <FilterField Field="Name" Label="Name" Type="FilterFieldType.Text" />
            </FilterFields>
        </FilterBuilder>

        @code {
            private FilterGroup _filters = new();

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                if (firstRender)
                {
                    var json = await JS.InvokeAsync<string?>("localStorage.getItem", "my-filters");
                    if (!string.IsNullOrEmpty(json))
                    {
                        _filters = JsonSerializer.Deserialize<FilterGroup>(json) ?? new();
                        StateHasChanged();
                    }
                }
            }

            private async Task HandleFilterChange(FilterGroup filters)
            {
                var json = JsonSerializer.Serialize(filters);
                await JS.InvokeVoidAsync("localStorage.setItem", "my-filters", json);
            }
        }
        """;
}
