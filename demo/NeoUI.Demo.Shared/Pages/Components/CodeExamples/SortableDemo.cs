namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SortableDemo
    {
        private const string _defaultCode =
            """
            <Sortable TItem="MyItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContentPrimitive class="flex flex-col gap-2">
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id">
                            <SortableItemHandle />
                            <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                        </SortableItem>
                    }
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="rounded-lg border bg-card px-4 py-3 shadow-lg opacity-90" />
            </Sortable>
            """;

        private const string _asHandleCode =
            """
            @* AsHandle makes the entire item the drag target *@
            <Sortable TItem="MyItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContentPrimitive class="flex flex-col gap-2">
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id" AsHandle>
                            <span class="flex-1 text-sm font-medium">@item.Name</span>
                            <span class="text-xs text-muted-foreground">drag me</span>
                        </SortableItem>
                    }
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="rounded-lg border bg-card px-4 py-3 shadow-lg opacity-90" />
            </Sortable>
            """;

        private const string _horizontalCode =
            """
            <Sortable TItem="MyItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)"
                      Orientation="SortableOrientation.Horizontal">
                <SortableContentPrimitive class="flex flex-row gap-3 flex-wrap">
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id" AsHandle
                                      Class="w-24 h-24 justify-center font-medium text-sm">
                            @item.Name
                        </SortableItem>
                    }
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="rounded-lg border bg-card shadow-lg flex items-center justify-center text-sm font-medium" />
            </Sortable>
            """;

        private const string _customHandleCode =
            """
            @* Replace the grip icon with a custom handle *@
            <Sortable TItem="MyItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContentPrimitive class="flex flex-col gap-2">
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id">
                            <SortableItemHandle>
                                <LucideIcon Name="grip-horizontal" Size="16" />
                            </SortableItemHandle>
                            <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                        </SortableItem>
                    }
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="rounded-lg border bg-card px-4 py-3 shadow-lg opacity-90" />
            </Sortable>
            """;

        private const string _primitiveCode =
            """
            @* Using the unstyled primitives directly — full control over markup *@
            <SortablePrimitive TItem="MyItem"
                               Items="@items"
                               OnItemsReordered="@(r => items = r)"
                               GetItemId="@(i => i.Id)">
                <SortableContentPrimitive class="flex flex-col gap-2">
                    @foreach (var item in items)
                    {
                        <SortableItemPrimitive Value="@item.Id"
                                               class="flex items-center gap-3 rounded-lg border bg-card px-4 py-3 shadow-sm">
                            <SortableItemHandlePrimitive class="cursor-grab active:cursor-grabbing text-muted-foreground hover:text-foreground" />
                            <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                        </SortableItemPrimitive>
                    }
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="rounded-lg border bg-card px-4 py-3 shadow-lg opacity-90" />
            </SortablePrimitive>
            """;

        // ── Props tables ──────────────────────────────────────────────────────

        private const string _dataViewListCode =
            """
            @* Wrap DataView in Sortable + SortableContentPrimitive.
               SortableItem goes inside the ListTemplate — no changes to DataView needed. *@
            <Sortable TItem="CardItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContentPrimitive class="block">
                    <DataView Items="@items" ItemKey="@(i => i.Id)"
                              ShowToolbar="false" ShowPagination="false">
                        <ListTemplate Context="item">
                            <SortableItem Value="@item.Id" Class="-mx-1 rounded-none border-0 border-b last:border-b-0 shadow-none px-2">
                                <SortableItemHandle />
                                <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                                <Badge Variant="BadgeVariant.Secondary" Class="text-xs">@item.Tag</Badge>
                            </SortableItem>
                        </ListTemplate>
                    </DataView>
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="flex items-center gap-3 rounded-lg border bg-card px-4 py-3 shadow-lg opacity-90" />
            </Sortable>
            """;

        private const string _dataViewGridCode =
            """
            @* Provide both ListTemplate and GridTemplate to keep the layout toggle.
               SortableItem works identically in the grid view. *@
            <Sortable TItem="CardItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContentPrimitive class="block">
                    <DataView Items="@items" ItemKey="@(i => i.Id)"
                              ShowToolbar="false" ShowPagination="false">
                        <ListTemplate Context="item">
                            <SortableItem Value="@item.Id" Class="-mx-1 rounded-none border-0 border-b last:border-b-0 shadow-none px-2">
                                <SortableItemHandle />
                                <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                                <Badge Variant="BadgeVariant.Secondary" Class="text-xs">@item.Tag</Badge>
                            </SortableItem>
                        </ListTemplate>
                        <GridTemplate Context="item">
                            <SortableItem Value="@item.Id" AsHandle
                                          Class="flex-col items-start gap-1 h-28 rounded-lg shadow-sm">
                                <p class="text-sm font-semibold leading-tight">@item.Name</p>
                                <Badge Variant="BadgeVariant.Secondary" Class="text-xs mt-auto">@item.Tag</Badge>
                            </SortableItem>
                        </GridTemplate>
                    </DataView>
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="rounded-lg border bg-card px-4 py-3 shadow-lg opacity-90" />
            </Sortable>
            """;

        private const string _dataTableCode =
            """
            @* AdditionalRowAttributes attaches arbitrary HTML attributes to each body <tr>.
               Here we supply data-sortable-id to enable drag-and-drop row reordering. *@
            <Sortable TItem="TaskItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContentPrimitive class="block">
                    <DataTable TData="TaskItem"
                               Data="@items"
                               AdditionalRowAttributes="@(i => new Dictionary<string, object> { ["data-sortable-id"] = i.Id })"
                               ShowPagination="false"
                               ShowToolbar="false">
                        <Columns>
                            <DataTableColumn TData="TaskItem" TValue="string" Property="@(i => i.Id)" Header="" Width="40px">
                                <CellTemplate Context="row">
                                    <SortableItemHandle Class="mx-auto" />
                                </CellTemplate>
                            </DataTableColumn>
                            <DataTableColumn TData="TaskItem" TValue="string" Property="@(i => i.Name)" Header="Task" />
                            <DataTableColumn TData="TaskItem" TValue="string" Property="@(i => i.Status)" Header="Status">
                                <CellTemplate Context="row">
                                    <Badge Variant="@(row.Status == "Done" ? BadgeVariant.Default : BadgeVariant.Secondary)">@row.Status</Badge>
                                </CellTemplate>
                            </DataTableColumn>
                        </Columns>
                    </DataTable>
                </SortableContentPrimitive>
                <SortableOverlayPrimitive class="rounded border bg-card px-4 py-2 shadow-lg opacity-90 text-sm" />
            </Sortable>
            """;

        private static readonly IReadOnlyList<DemoPropRow> _sortableProps =
        [
            new("Items",            "IList&lt;TItem&gt;",                      null,        "The list of items to sort. Required."),
            new("OnItemsReordered", "EventCallback&lt;IList&lt;TItem&gt;&gt;", null,        "Callback invoked after a drag reorders items."),
            new("GetItemId",        "Func&lt;TItem, string&gt;",               null,        "Extracts a unique string ID from each item. Required."),
            new("Orientation",      "SortableOrientation",                      "Vertical",  "Drag axis: Vertical, Horizontal, or Mixed."),
            new("Class",            "string?",                                  null,        "Additional CSS classes appended to the root element."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _itemProps =
        [
            new("Value",        "string",          null,    "Unique item identifier matching GetItemId output. Required."),
            new("AsHandle",     "bool",             "false", "When true the entire item surface is the drag handle."),
            new("Class",        "string?",          null,    "Additional CSS classes appended to the item element."),
            new("ChildContent", "RenderFragment?",  null,    "Content rendered inside the item."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _handleProps =
        [
            new("ChildContent", "RenderFragment?", null, "Custom handle content. Renders a six-dot grip icon by default."),
            new("Class",        "string?",         null, "Additional CSS classes appended to the handle element."),
        ];
    }
}
