namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class RefreshDemo
{
    private const string _refreshCode = """
<Grid @ref="_gridRef" Items="@products" IdField="ProductId">
    <Columns>
        <DataGridColumn Field="Name" Header="Product" />
        <DataGridColumn Field="Price" Header="Price" DataFormatString="{0:C}" />
        <DataGridColumn Field="Stock" Header="Stock" />
    </Columns>
</Grid>

<Button OnClick="IncreasePrices">Increase Prices</Button>

@code {
    private Grid<Product> _gridRef = default!;
    private List<Product> products = new();

    protected override void OnInitialized()
    {
        products = GenerateProducts(20);
    }

    private async Task IncreasePrices()
    {
        foreach (var product in products)
        {
            product.Price *= 1.1m;
        }
        await _gridRef.RefreshAsync();
    }

    private async Task ManualRefresh()
    {
        await _gridRef.RefreshAsync();
    }
}
""";
}
