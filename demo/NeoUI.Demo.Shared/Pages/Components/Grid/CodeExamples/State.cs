namespace NeoUI.Demo.Shared.Pages.Components.Grid;

partial class State
{
    private const string _statePersistenceCode = """
private Grid<Order> gridRef = default!;

[Inject] private IJSRuntime JS { get; set; }
private DataGridState gridState = new();

protected override async Task OnInitializedAsync()
{
    var json = await JS.InvokeAsync<string>("localStorage.getItem", "gridState");
    if (!string.IsNullOrEmpty(json))
    {
        gridState = JsonSerializer.Deserialize<DataGridState>(json) ?? new();
    }
}

private async Task SaveState()
{
    // ✅ Get actual state from AG DataGrid
    gridState = await gridRef.GetStateAsync();

    var json = JsonSerializer.Serialize(gridState);
    await JS.InvokeVoidAsync("localStorage.setItem", "gridState", json);
}
""";

    private const string _stateExportCode = """
private async Task ExportState()
{
    var json = JsonSerializer.Serialize(gridState, new JsonSerializerOptions { WriteIndented = true });
    var bytes = Encoding.UTF8.GetBytes(json);
    var base64 = Convert.ToBase64String(bytes);
    await JS.InvokeVoidAsync("downloadFile", "grid-state.json", base64);
}

private async Task ImportState(ChangeEventArgs e)
{
    // Read file and deserialize to DataGridState
    gridState = JsonSerializer.Deserialize<DataGridState>(json) ?? new();
}
""";
}
