namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DataTableDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _dataTableProps =
            [
                new("Data", "IEnumerable<TData>", "—", "The data source for the table."),
                new("SelectionMode", "DataTableSelectionMode", "None", "Row selection mode: None, Single, Multiple."),
                new("SelectedItems", "IReadOnlyCollection<TData>", "—", "Currently selected items. Use @bind-SelectedItems for two-way binding."),
                new("EnableKeyboardNavigation", "bool", "true", "Enables arrow key navigation and Enter/Space to select rows."),
                new("ShowToolbar", "bool", "true", "Whether to show the toolbar (search + column visibility)."),
                new("ShowPagination", "bool", "true", "Whether to show pagination controls."),
                new("IsLoading", "bool", "false", "Whether the table is in loading state."),
                new("InitialPageSize", "int", "5", "The initial number of rows per page."),
                new("PageSizes", "int[]", "[5,10,20,50,100]", "Available page size options."),
                new("Class", "string?", "null", "Additional CSS classes."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _dataTableColumnProps =
            [
                new("Property", "Func<TData, TValue>", "—", "Expression to get the column value."),
                new("Header", "string", "—", "Column header text."),
                new("Sortable", "bool", "false", "Whether the column is sortable."),
                new("Alignment", "ColumnAlignment", "Left", "Cell alignment: Left, Center, Right."),
                new("CellTemplate", "RenderFragment<TData>?", "null", "Custom cell render template."),
                new("Visible", "bool", "true", "Whether the column is visible."),
            ];

        private const string _basicTableCode = """
                <DataTable TData="Person" Data="people" InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Id)" Header="ID" Sortable Alignment="ColumnAlignment.Right" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" Sortable />
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Age)" Header="Age" Sortable Alignment="ColumnAlignment.Right" />
                    </Columns>
                </DataTable>
                """;

        private const string _rowSelectionCode = """
                <DataTable TData="Person"
                           Data="people"
                           SelectionMode="DataTableSelectionMode.Multiple"
                           EnableKeyboardNavigation="true"
                           InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;

        private const string _customCellCode = """
                <DataTable TData="Person" Data="people" ShowPagination="false">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Status)" Header="Status" Sortable>
                            <CellTemplate Context="person">
                                <Badge Variant="@(person.Status == "Active" ? BadgeVariant.Default : BadgeVariant.Outline)">
                                    @person.Status
                                </Badge>
                            </CellTemplate>
                        </DataTableColumn>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Actions">
                            <CellTemplate Context="person">
                                <div class="flex gap-2">
                                    <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">View</Button>
                                    <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Small">Edit</Button>
                                </div>
                            </CellTemplate>
                        </DataTableColumn>
                    </Columns>
                </DataTable>
                """;

        private const string _searchCode = """
                <DataTable TData="Person" Data="people" InitialPageSize="10">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Id)" Header="ID" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Age)" Header="Age" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;

        private const string _emptyStateCode = """
                <DataTable TData="Person" Data="emptyPeople" InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                    </Columns>
                </DataTable>
                """;

        private const string _loadingCode = """
                <DataTable TData="Person" Data="people" IsLoading="true" InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;

        private const string _withoutToolbarCode = """
                <DataTable TData="Person" Data="people" ShowToolbar="false" InitialPageSize="5">
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="int" Property="@(p => p.Age)" Header="Age" Sortable />
                    </Columns>
                </DataTable>
                """;

        private const string _customToolbarCode = """
                <DataTable TData="Person" Data="people" InitialPageSize="5">
                    <ToolbarActions>
                        <Button Variant="ButtonVariant.Default" Size="ButtonSize.Small">Add Person</Button>
                        <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">Export</Button>
                    </ToolbarActions>
                    <Columns>
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
                        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Role)" Header="Role" />
                    </Columns>
                </DataTable>
                """;
    }
}
