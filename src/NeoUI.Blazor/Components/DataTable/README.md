# DataTable Component

A feature-rich data table built on the `Table` primitive with automatic sorting, filtering, pagination, row selection, and full appearance customization.

## Features

- ✅ Declarative column API via `DataTableColumn` child components
- ✅ Automatic client-side sorting, filtering, and pagination
- ✅ Global search with column visibility toggle toolbar
- ✅ Row selection — Single or Multiple with checkbox + keyboard support
- ✅ Dense / comfortable layout toggle
- ✅ Configurable header background, header borders, and cell borders
- ✅ Per-part CSS class overrides (`HeaderClass`, `HeaderRowClass`, `BodyRowClass`)
- ✅ Custom cell templates, empty state, and loading state
- ✅ Custom toolbar actions
- ✅ Server-side / hybrid mode via `PreprocessData`, `OnSort`, `OnFilter`
- ✅ Accessibility — `role="grid"`, `aria-sort`, `aria-selected`, keyboard navigation

---

## Quick Start

```razor
<DataTable TData="Person" Data="@people" InitialPageSize="10">
    <Columns>
        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable Filterable />
        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Email)" Header="Email" />
        <DataTableColumn TData="Person" TValue="int"    Property="@(p => p.Age)"   Header="Age" Sortable Alignment="ColumnAlignment.Right" />
    </Columns>
</DataTable>
```

---

## Appearance Customization

### Dense Layout

`Dense="true"` (default) uses compact cell padding (`h-9 px-4` header, `py-2 px-4` body). Set to `false` for a more spacious layout (`h-12 px-4` / `p-4`).

```razor
<DataTable TData="Person" Data="@people" Dense="false">
    ...
</DataTable>
```

### Header Background

`HeaderBackground="true"` (default) applies `bg-muted/50` to the header row. Set to `false` for a flat, borderless header.

```razor
<DataTable TData="Person" Data="@people" HeaderBackground="false">
    ...
</DataTable>
```

### Header Borders & Cell Borders

Add vertical column dividers independently for the header and body:

```razor
<DataTable TData="Person" Data="@people" HeaderBorder="true" CellBorder="true">
    ...
</DataTable>
```

### Per-Part Class Overrides

Override CSS at any structural level without touching the defaults:

```razor
<DataTable TData="Person" Data="@people"
           HeaderClass="bg-primary/10"
           HeaderRowClass="text-primary"
           BodyRowClass="font-mono text-xs">
    ...
</DataTable>
```

---

## Row Selection

```razor
<DataTable TData="Person"
           Data="@people"
           SelectionMode="DataTableSelectionMode.Multiple"
           @bind-SelectedItems="@selectedPeople">
    <Columns>
        <DataTableColumn TData="Person" TValue="string" Property="@(p => p.Name)" Header="Name" Sortable />
    </Columns>
</DataTable>

@code {
    private IReadOnlyCollection<Person> selectedPeople = Array.Empty<Person>();
}
```

When the page count exceeds one, a dropdown lets users choose between selecting the current page or all filtered items.

---

## Custom Cell Templates

```razor
<DataTableColumn TData="Person" TValue="string" Property="@(p => p.Status)" Header="Status">
    <CellTemplate Context="person">
        <Badge Variant="@(person.Status == "Active" ? BadgeVariant.Default : BadgeVariant.Outline)">
            @person.Status
        </Badge>
    </CellTemplate>
</DataTableColumn>
```

---

## Custom Toolbar Actions

```razor
<DataTable TData="Person" Data="@people">
    <ToolbarActions>
        <Button Variant="ButtonVariant.Default" Size="ButtonSize.Small" OnClick="AddPerson">Add</Button>
        <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small" OnClick="Export">Export</Button>
    </ToolbarActions>
    <Columns>...</Columns>
</DataTable>
```

---

## Loading & Empty States

```razor
<DataTable TData="Person" Data="@people" IsLoading="@isLoading">
    <LoadingTemplate>
        <div class="p-8 text-center text-muted-foreground">Fetching data…</div>
    </LoadingTemplate>
    <EmptyTemplate>
        <div class="p-8 text-center text-muted-foreground">No records found.</div>
    </EmptyTemplate>
    <Columns>...</Columns>
</DataTable>
```

---

## Server-Side / Hybrid Mode

Use `PreprocessData` for async transformations (e.g. server fetch after initial client render), and `OnSort` / `OnFilter` for fully server-driven tables:

```razor
<DataTable TData="Order"
           Data="@orders"
           PreprocessData="@EnrichOrders"
           OnSort="@HandleSort"
           OnFilter="@HandleFilter">
    <Columns>...</Columns>
</DataTable>

@code {
    private async Task<IEnumerable<Order>> EnrichOrders(IEnumerable<Order> data)
    {
        // e.g. attach related entities
        return data;
    }

    private async Task HandleSort((string ColumnId, SortDirection Direction) sort) { }
    private async Task HandleFilter(string? query) { }
}
```

---

## Component API

### DataTable\<TData\>

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Data` | `IEnumerable<TData>` | **required** | The data source for the table. |
| `Columns` | `RenderFragment?` | `null` | Slot for `DataTableColumn` child components. |
| `SelectionMode` | `DataTableSelectionMode` | `None` | Row selection mode: `None`, `Single`, `Multiple`. |
| `SelectedItems` | `IReadOnlyCollection<TData>` | `[]` | Currently selected items. Use `@bind-SelectedItems`. |
| `EnableKeyboardNavigation` | `bool` | `true` | Arrow key row navigation and Enter/Space selection. |
| `ShowToolbar` | `bool` | `true` | Show/hide the search and column visibility toolbar. |
| `ShowPagination` | `bool` | `true` | Show/hide pagination controls. |
| `IsLoading` | `bool` | `false` | Replaces table content with a loading indicator. |
| `InitialPageSize` | `int` | `5` | Starting rows-per-page value. |
| `PageSizes` | `int[]` | `[5,10,20,50,100]` | Options shown in the page-size selector. |
| `Dense` | `bool` | `true` | Compact padding (`h-9`/`py-2 px-4`). `false` = spacious (`h-12`/`p-4`). |
| `HeaderBackground` | `bool` | `true` | Applies `bg-muted/50` to the header row. |
| `HeaderBorder` | `bool` | `false` | Vertical dividers between header cells. |
| `CellBorder` | `bool` | `false` | Vertical dividers between body cells. |
| `HeaderClass` | `string?` | `null` | Extra CSS classes on `<thead>`. |
| `HeaderRowClass` | `string?` | `null` | Extra CSS classes on the header `<tr>`. |
| `BodyRowClass` | `string?` | `null` | Extra CSS classes on each body `<tr>`. |
| `ToolbarActions` | `RenderFragment?` | `null` | Custom buttons rendered inside the toolbar. |
| `EmptyTemplate` | `RenderFragment?` | `null` | Content shown when the table has no rows. |
| `LoadingTemplate` | `RenderFragment?` | `null` | Content shown when `IsLoading` is `true`. |
| `AriaLabel` | `string?` | `null` | ARIA label applied to the `<table>` element. |
| `Class` | `string?` | `null` | Extra CSS classes on the outer container `<div>`. |
| `OnSort` | `EventCallback<(string, SortDirection)>` | — | Fires when the user changes the sort column or direction. |
| `OnFilter` | `EventCallback<string?>` | — | Fires when the global search value changes. |
| `PreprocessData` | `Func<IEnumerable<TData>, Task<IEnumerable<TData>>>?` | `null` | Async hook to transform data before filtering and sorting. |
| `ServerData` | `Func<DataTableState, Task<TableData<TData>>>?` | `null` | Server-side data callback for fully server-driven paging/sorting/filtering. |
| `Striped` | `bool` | `false` | Zebra-striped rows. |
| `StripeClass` | `string?` | `null` | Custom CSS class applied to odd rows when `Striped` is true. |
| `RowContextMenu` | `RenderFragment<TData>?` | `null` | Right-click context menu content; receives the row item as context. |
| `Resizable` | `bool` | `false` | Allow column resizing by dragging header borders. |
| `MinColumnWidth` | `int` | `50` | Minimum column width in pixels when resizing. |
| `OnColumnResize` | `EventCallback<ColumnResizeEventArgs<TData>>` | — | Fires after a column is resized. |
| `Reorderable` | `bool` | `false` | Allow drag-and-drop column reordering. |
| `OnColumnReorder` | `EventCallback<ColumnReorderEventArgs<TData>>` | — | Fires after columns are reordered. |
| `ColumnsVisibility` | `bool` | `false` | Show column visibility toggle button in the toolbar. |
| `Virtualize` | `bool` | `false` | Enable virtual scrolling for large datasets. |
| `Height` | `string?` | `null` | Container height (required when `Virtualize` is true, e.g. `"600px"`). |
| `ItemHeight` | `float` | `40` | Row height in pixels used by the virtual scroll engine. |
| `VirtualizeOverscanCount` | `int` | `3` | Extra rows rendered outside the visible viewport for smooth scrolling. |
| `ItemsProvider` | `GridItemsProvider<TData>?` | `null` | Virtualized data provider callback for server-driven virtual scrolling. |
| `ChildrenProperty` | `string?` | `null` | Property name on data items whose value contains child rows (enables tree mode). |
| `LoadChildrenAsync` | `Func<TData, Task<IEnumerable<TData>>>?` | `null` | Async callback to lazily load child rows. |
| `HasChildrenField` | `string?` | `null` | Boolean property name indicating whether an item has children (shows expand arrow). |
| `ExpandedValues` | `IReadOnlySet<TKey>?` | `null` | Controlled set of expanded row keys (tree mode). |
| `ExpandedValuesChanged` | `EventCallback<IReadOnlySet<TKey>>` | — | Fires when the set of expanded rows changes (tree mode). |
| `ValueField` | `Expression<Func<TData, TKey>>?` | `null` | Key field selector used to identify rows in tree mode. |
| `SyncWidthOnResize` | `bool` | `false` | Re-synchronize column widths when the browser window is resized. |
| `TableContainerClass` | `string?` | `null` | Extra CSS classes on the scrollable table container `<div>`. |
| `ColumnSizing` | `TableColumnSizing` | `Auto` | Column width algorithm: `Auto` (content-driven) or `Fixed` (equal distribution). |

> **Localization**: All UI strings (pagination labels, empty/loading states, ARIA attributes) use `ILocalizer` for localization — override via `DefaultLocalizer` keys or a custom `ILocalizer` implementation. See [Localization docs](../../README.md#localization).

### DataTableColumn\<TData, TValue\>

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Property` | `Func<TData, TValue>` | **required** | Expression that returns the column value from a row. |
| `Header` | `string` | **required** | Column header label text. |
| `Id` | `string?` | `null` | Unique column ID. Auto-generated from `Header` when omitted. |
| `Sortable` | `bool` | `false` | Whether clicking the header sorts by this column. |
| `Filterable` | `bool` | `false` | Whether this column is included in global search. |
| `Alignment` | `ColumnAlignment` | `Left` | Cell alignment: `Left`, `Center`, `Right`. |
| `CellTemplate` | `RenderFragment<TData>?` | `null` | Custom cell render template. Receives the row item as context. |
| `Visible` | `bool` | `true` | Column visibility. Can be toggled via the column visibility menu. |
| `Width` | `string?` | `null` | Fixed column width (e.g. `"200px"`, `"20%"`). |
| `MinWidth` | `string?` | `null` | Minimum column width CSS value. |
| `MaxWidth` | `string?` | `null` | Maximum column width CSS value. |
| `CellClass` | `string?` | `null` | Extra CSS classes on every body cell in this column. |
| `HeaderClass` | `string?` | `null` | Extra CSS classes on this column's header cell. |
