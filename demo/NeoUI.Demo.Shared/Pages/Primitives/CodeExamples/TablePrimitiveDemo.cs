namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class TablePrimitiveDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _tableProps =
    [
        new("Data", "IEnumerable&lt;TData&gt;", null, "The data source for the table."),
            new("State", "TableState&lt;TData&gt;?", null, "Table state for controlled mode. Supports two-way binding with <code class=\"text-xs bg-muted px-1 rounded\">@bind-State</code>."),
            new("SelectionMode", "SelectionMode", "None", "Selection mode: None, Single, or Multiple."),
            new("EnableKeyboardNavigation", "bool", "true", "Enables row keyboard navigation (Arrow keys, Enter/Space)."),
            new("ManualPagination", "bool", "false", "When true, table will not automatically set total item count."),
            new("OnSelectionChange", "EventCallback&lt;IReadOnlyCollection&lt;TData&gt;&gt;", null, "Callback invoked when selection changes."),
            new("OnSortChange", "EventCallback&lt;(string, SortDirection)&gt;", null, "Callback invoked when sorting changes."),
            new("TableRow.Item", "TData?", null, "The data item for a row. Required for selection behavior."),
            new("TableHeaderCell.ColumnId", "string?", null, "Column identifier used for sortable headers."),
        ];

        private const string _basicTableCode =
        """
        <Table TData="Person" Data="@people">
            <TableHeader>
                <TableRow TData="Person">
                    <TableHeaderCell TData="Person">Name</TableHeaderCell>
                    <TableHeaderCell TData="Person">Email</TableHeaderCell>
                    <TableHeaderCell TData="Person">Age</TableHeaderCell>
                    <TableHeaderCell TData="Person">Department</TableHeaderCell>
                </TableRow>
            </TableHeader>
            <TableBody>
                @foreach (var person in people)
                {
                    <TableRow TData="Person">
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                        <TableCell>@person.Age</TableCell>
                        <TableCell>@person.Department</TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>
        """;

        private const string _rowActionsCode =
            """
        <Table TData="Person" Data="@people">
            <TableBody>
                @foreach (var person in people)
                {
                    <TableRow TData="Person">
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                        <TableCell>@person.Department</TableCell>
                        <TableCell>
                            <DropdownMenuPrimitive>
                                <DropdownMenuTriggerPrimitive>
                                    <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon">
                                        <LucideIcon Name="ellipsis" Size="16" />
                                    </Button>
                                </DropdownMenuTriggerPrimitive>
                                <DropdownMenuContentPrimitive Align="PopoverAlign.End">
                                    <DropdownMenuItemPrimitive OnClick="@(() => HandleViewDetails(person))">View details</DropdownMenuItemPrimitive>
                                    <DropdownMenuItemPrimitive OnClick="@(() => HandleEdit(person))">Edit</DropdownMenuItemPrimitive>
                                    <DropdownMenuItemPrimitive OnClick="@(() => HandleDuplicate(person))">Duplicate</DropdownMenuItemPrimitive>
                                    <DropdownMenuItemPrimitive OnClick="@(() => HandleDelete(person))">Delete</DropdownMenuItemPrimitive>
                                </DropdownMenuContentPrimitive>
                            </DropdownMenuPrimitive>
                        </TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>
        """;

        private const string _sortableTableCode =
            """
        <Table TData="Person" Data="@GetSortedPeople()" @bind-State="@sortableTableState">
            <TableHeader>
                <TableRow TData="Person">
                    <TableHeaderCell TData="Person" ColumnId="name">Name</TableHeaderCell>
                    <TableHeaderCell TData="Person" ColumnId="email">Email</TableHeaderCell>
                    <TableHeaderCell TData="Person" ColumnId="age">Age</TableHeaderCell>
                    <TableHeaderCell TData="Person" ColumnId="department">Department</TableHeaderCell>
                </TableRow>
            </TableHeader>
            <TableBody>
                @foreach (var person in GetSortedPeople())
                {
                    <TableRow TData="Person">
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                        <TableCell>@person.Age</TableCell>
                        <TableCell>@person.Department</TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>
        """;

        private const string _paginatedTableCode =
            """
        <Table TData="Person" Data="@largePeopleDataset" @bind-State="@paginatedTableState"
               OnPageChange="@HandlePaginationChange"
               OnPageSizeChange="@HandlePaginationChange">
            <TableBody>
                @foreach (var person in GetPaginatedData())
                {
                    <TableRow TData="Person">
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                        <TableCell>@person.Age</TableCell>
                        <TableCell>@person.Department</TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>

        <TablePagination TData="Person" State="@paginatedTableState.Pagination"
                         OnPageChange="@HandlePaginationChange"
                         OnPageSizeChange="@HandlePaginationChange" />
        """;

        private const string _selectableTableCode =
            """
        <Table TData="Person" Data="@people" @bind-State="@selectableTableState"
               SelectionMode="SelectionMode.Multiple"
               EnableKeyboardNavigation="@enableKeyboardNavigation"
               OnSelectionChange="@HandleSelectionChange">
            <TableBody>
                @foreach (var person in people)
                {
                    var isSelected = selectableTableState.Selection.IsSelected(person);
                    <TableRow TData="Person" Item="@person">
                        <TableCell>
                            <input type="checkbox" checked="@isSelected" @onchange="@(() => ToggleRowSelection(person))" />
                        </TableCell>
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>
        """;

        private const string _combinedTableCode =
            """
        <Table TData="Person" Data="@GetSortedCombinedData()" @bind-State="@combinedTableState"
               SelectionMode="SelectionMode.Multiple"
               OnPageChange="@HandleCombinedPaginationChange"
               OnPageSizeChange="@HandleCombinedPaginationChange"
               OnSelectionChange="@HandleCombinedSelectionChange">
            <TableHeader>
                <TableRow TData="Person">
                    <TableHeaderCell TData="Person">Name + filter + sort</TableHeaderCell>
                    <TableHeaderCell TData="Person">Email + filter + sort</TableHeaderCell>
                    <TableHeaderCell TData="Person">Age + filter + sort</TableHeaderCell>
                    <TableHeaderCell TData="Person">Department + filter + sort</TableHeaderCell>
                </TableRow>
            </TableHeader>
            <TableBody>
                @foreach (var person in GetCombinedPageData())
                {
                    <TableRow TData="Person" Item="@person">
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                        <TableCell>@person.Age</TableCell>
                        <TableCell>@person.Department</TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>
        """;

        private const string _columnFilteringCode =
            """
        <Table TData="Person" Data="@GetFilteredPeople()" @bind-State="@filteredTableState"
               OnPageChange="@HandleFilteredPaginationChange"
               OnPageSizeChange="@HandleFilteredPaginationChange">
            <TableHeader>
                <TableRow TData="Person">
                    <TableHeaderCell TData="Person">Name (filter popover)</TableHeaderCell>
                    <TableHeaderCell TData="Person">Email (filter popover)</TableHeaderCell>
                    <TableHeaderCell TData="Person">Department (filter popover)</TableHeaderCell>
                </TableRow>
            </TableHeader>
            <TableBody>
                @foreach (var person in GetFilteredPageData())
                {
                    <TableRow TData="Person">
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                        <TableCell>@person.Department</TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>
        """;

        private const string _globalSearchCode =
            """
        <input type="text" @bind="globalSearchFilter" @bind:event="oninput" placeholder="Search across all columns..." />

        <Table TData="Person" Data="@GetGloballyFilteredPeople()" @bind-State="@globalSearchTableState"
               OnPageChange="@HandleGlobalSearchPaginationChange"
               OnPageSizeChange="@HandleGlobalSearchPaginationChange">
            <TableBody>
                @foreach (var person in GetGlobalSearchPageData())
                {
                    <TableRow TData="Person">
                        <TableCell>@person.Name</TableCell>
                        <TableCell>@person.Email</TableCell>
                        <TableCell>@person.Age</TableCell>
                        <TableCell>@person.Department</TableCell>
                    </TableRow>
                }
            </TableBody>
        </Table>
        """;
    }
}
