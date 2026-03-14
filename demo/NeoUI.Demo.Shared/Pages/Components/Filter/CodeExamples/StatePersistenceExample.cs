namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class StatePersistenceExample
{
    private static readonly string _persistenceCode = """
        @inject IJSRuntime JS
        @using System.Text.Json
        @using Microsoft.JSInterop

        <FilterBuilder TData="Order"
                       @bind-Filters="_filters"
                       OnFilterChange="HandleFilterChange">
            <FilterFields>
                <FilterField Field="OrderNumber" Label="Order #" Type="FilterFieldType.Text" />
                <FilterField Field="Status" Label="Status" Type="FilterFieldType.Select" Options="@statusOptions" />
                <FilterField Field="Amount" Label="Amount" Type="FilterFieldType.Number"
                             EditorType="FilterEditorType.Currency" Min="0" />
            </FilterFields>
        </FilterBuilder>

        @code {
            private const string StorageKey = "neoui.demo.filter.state";
            private FilterGroup _filters = new();

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                if (!firstRender)
                {
                    return;
                }

                var json = await JS.InvokeAsync<string?>("localStorage.getItem", StorageKey);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return;
                }

                var restored = JsonSerializer.Deserialize<FilterGroup>(json);
                _filters = restored is null ? new() : NormalizeFilters(restored);
                StateHasChanged();
            }

            private async Task HandleFilterChange(FilterGroup filters)
            {
                _filters = filters;
                var json = JsonSerializer.Serialize(filters);
                await JS.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            }

            private static FilterGroup NormalizeFilters(FilterGroup filters)
            {
                // Convert JsonElement values back into editor-friendly types
                // such as decimal for Amount and string for text/select fields.
                return filters;
            }
        }
        """;
}
