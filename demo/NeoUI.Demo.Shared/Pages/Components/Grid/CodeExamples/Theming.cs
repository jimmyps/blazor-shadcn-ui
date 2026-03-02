namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class Theming
{
    private const string _presetsCode = """
<!-- Compact + Striped -->
<DataGrid TItem="Product" Items="@products"
          Density="DataGridDensity.Compact"
          VisualStyle="DataGridStyle.Striped"
          Height="250px">
    <Columns>
        <DataGridColumn TItem="Product" Field="Name" Header="Product" />
        <DataGridColumn TItem="Product" Field="Category" Header="Category" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Price)" Header="Price" />
    </Columns>
</DataGrid>

<!-- Spacious + Bordered -->
<DataGrid TItem="Product" Items="@products"
          Density="DataGridDensity.Spacious"
          VisualStyle="DataGridStyle.Bordered"
          Height="250px">
    <Columns>
        <DataGridColumn TItem="Product" Field="Name" Header="Product" />
        <DataGridColumn TItem="Product" Field="Category" Header="Category" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Price)" Header="Price" />
    </Columns>
</DataGrid>
""";

    private const string _fineGrainedCode = """
<!-- Custom Accent Color -->
<DataGrid TItem="Product" Items="@products"
          Density="DataGridDensity.Comfortable"
          Height="250px">
    <Columns>
        <DataGridThemeParameters
            AccentColor="#dc2626"
            RowHeight="36"
            RowHoverColor="#dc262633" />
        <DataGridColumn TItem="Product" Field="Name" Header="Product" />
        <DataGridColumn TItem="Product" Field="Category" Header="Category" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Price)" Header="Price" />
    </Columns>
</DataGrid>

<!-- Custom Typography -->
<DataGrid TItem="Product" Items="@products"
          Density="DataGridDensity.Comfortable"
          Height="250px">
    <Columns>
        <DataGridThemeParameters
            FontSize="16"
            BorderRadius="8"
            HeaderFontWeight="700" />
        <DataGridColumn TItem="Product" Field="Name" Header="Product" />
        <DataGridColumn TItem="Product" Field="Category" Header="Category" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Price)" Header="Price" />
    </Columns>
</DataGrid>
""";

    private const string _themeComparisonCode = """
<DataGrid TItem="Product" Items="@products"
          Theme="@selectedTheme"
          Density="DataGridDensity.Comfortable"
          Height="300px">
    <Columns>
        <DataGridColumn TItem="Product" Field="Name" Header="Product" />
        <DataGridColumn TItem="Product" Field="Category" Header="Category" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Price)" Header="Price" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Stock)" Header="Stock" />
    </Columns>
</DataGrid>

@code {
    private DataGridTheme selectedTheme = DataGridTheme.Shadcn;
}
""";

    private const string _precedenceCode = """
<!-- Standard Compact — default 28px row height -->
<DataGrid TItem="Product" Items="@products"
          Density="DataGridDensity.Compact"
          Height="250px">
    <Columns>
        <DataGridColumn TItem="Product" Field="Name" Header="Product" />
        <DataGridColumn TItem="Product" Field="Category" Header="Category" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Price)" Header="Price" />
    </Columns>
</DataGrid>

<!-- Compact with RowHeight overridden to 50px -->
<DataGrid TItem="Product" Items="@products"
          Density="DataGridDensity.Compact"
          Height="250px">
    <Columns>
        <DataGridThemeParameters RowHeight="50" />
        <DataGridColumn TItem="Product" Field="Name" Header="Product" />
        <DataGridColumn TItem="Product" Field="Category" Header="Category" />
        <DataGridColumn TItem="Product" Field="@nameof(Product.Price)" Header="Price" />
    </Columns>
</DataGrid>
""";
}
