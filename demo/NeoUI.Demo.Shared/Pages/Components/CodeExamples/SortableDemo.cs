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
                <SortableContent>
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id">
                            <SortableItemHandle />
                            <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                        </SortableItem>
                    }
                </SortableContent>
                <SortableOverlay />
            </Sortable>
            """;

        private const string _asHandleCode =
            """
            @* AsHandle makes the entire item the drag target *@
            <Sortable TItem="MyItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContent>
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id" AsHandle>
                            <span class="flex-1 text-sm font-medium">@item.Name</span>
                            <span class="text-xs text-muted-foreground">drag me</span>
                        </SortableItem>
                    }
                </SortableContent>
                <SortableOverlay />
            </Sortable>
            """;

        private const string _horizontalCode =
            """
            <Sortable TItem="MyItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)"
                      Orientation="SortableOrientation.Horizontal">
                <SortableContent Class="flex-row gap-3 flex-wrap">
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id" AsHandle
                                      Class="w-24 h-24 justify-center font-medium text-sm">
                            @item.Name
                        </SortableItem>
                    }
                </SortableContent>
                <SortableOverlay />
            </Sortable>
            """;

        private const string _customHandleCode =
            """
            @* Replace the default grip icon with a custom handle *@
            <Sortable TItem="MyItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContent>
                    @foreach (var item in items)
                    {
                        <SortableItem Value="@item.Id">
                            <SortableItemHandle>
                                <LucideIcon Name="grip-horizontal" Size="16" />
                            </SortableItemHandle>
                            <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                        </SortableItem>
                    }
                </SortableContent>
                <SortableOverlay />
            </Sortable>
            """;

        private const string _dataViewListCode =
            """
            <Sortable TItem="CardItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)">
                <SortableContent Class="block">
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
                </SortableContent>
                <SortableOverlay />
            </Sortable>
            """;

        private const string _dataViewGridCode =
            """
            @* Track active layout so Orientation matches the current view. *@
            <Sortable TItem="CardItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)"
                      Orientation="@(layout == DataViewLayout.Grid ? SortableOrientation.Grid : SortableOrientation.Vertical)">
                <SortableContent Class="block">
                    <DataView Items="@items" ItemKey="@(i => i.Id)"
                              Layout="DataViewLayout.Grid" ShowPagination="false"
                              LayoutChanged="@(l => layout = l)">
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
                </SortableContent>
                <SortableOverlay />
            </Sortable>
            """;

        private const string _dataTableCode =
            """
            @* Add Context="s" to receive a typed SortableScope<TItem>.
               Pass s.RowAttributes to any grid, table, or list — no internal attribute names needed. *@
            <Sortable TItem="TaskItem"
                      Items="@items"
                      OnItemsReordered="@(r => items = r)"
                      GetItemId="@(i => i.Id)"
                      Context="s">
                <SortableContent Class="block">
                    <DataTable TData="TaskItem"
                               Data="@items"
                               AdditionalRowAttributes="@s.RowAttributes"
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
                </SortableContent>
                <SortableOverlay Class="rounded" />
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
                <SortableOverlayPrimitive class="rounded-lg shadow-lg opacity-90 transition-transform duration-150 data-[state=dragging]:scale-[1.05]" />
            </SortablePrimitive>
            """;

        private const string _kanbanCode =
            """
            @* Each column is a Sortable with the same Group name *@
            @foreach (var col in columns)
            {
                <Sortable TItem="MyItem"
                          Items="@col.Items"
                          Group="kanban"
                          GetItemId="@(i => i.Id)"
                          OnItemsReordered="@(r => col.Items = r)"
                          OnItemTransferredOut="@(args => col.Items = col.Items.Where(i => i.Id != args.ActiveId).ToList())"
                          OnItemTransferredIn="@(args => HandleTransferIn(args, col))">
                    <SortableContent Class="flex flex-col gap-2 min-h-[80px] rounded-lg border border-dashed p-2">
                        @foreach (var item in col.Items)
                        {
                            <SortableItem @key="@item.Id" Value="@item.Id">
                                <SortableItemHandle />
                                <span class="flex-1 text-sm select-none">@item.Name</span>
                            </SortableItem>
                        }
                    </SortableContent>
                    <SortableOverlay />
                </Sortable>
            }

            @code {
                // OnItemTransferredIn fires on the TARGET before the source removes the item.
                // Look up the item while it's still in the source list, then insert into target.
                private void HandleTransferIn(SortableTransferArgs args, Column col)
                {
                    var item = allColumns.SelectMany(c => c.Items).FirstOrDefault(i => i.Id == args.ActiveId);
                    if (item is null) return;
                    var list = col.Items.ToList();
                    list.Insert(Math.Clamp(args.Index, 0, list.Count), item);
                    col.Items = list;
                }
            }
            """;

        private const string _crossListCode =
            """
            @* DataView side *@
            <Sortable TItem="MyItem" Items="@_left" Group="transfer"
                      GetItemId="@(i => i.Id)"
                      OnItemsReordered="@(r => _left = r)"
                      OnItemTransferredOut="@(args => _left = _left.Where(i => i.Id != args.ActiveId).ToList())"
                      OnItemTransferredIn="@(args => HandleLeftIn(args))">
                <SortableContent Class="block">
                    <DataView Items="@_left" ItemKey="@(i => i.Id)" ShowToolbar="false" ShowPagination="false">
                        <ListTemplate Context="item">
                            <SortableItem @key="@item.Id" Value="@item.Id">
                                <SortableItemHandle />
                                <span class="flex-1 text-sm select-none">@item.Name</span>
                            </SortableItem>
                        </ListTemplate>
                    </DataView>
                </SortableContent>
                <SortableOverlay />
            </Sortable>

            @* DataTable side — Context="s" injects data-sortable-id on each row *@
            <Sortable TItem="MyItem" Items="@_right" Group="transfer"
                      GetItemId="@(i => i.Id)"
                      OnItemsReordered="@(r => _right = r)"
                      OnItemTransferredOut="@(args => _right = _right.Where(i => i.Id != args.ActiveId).ToList())"
                      OnItemTransferredIn="@(args => HandleRightIn(args))"
                      Context="s">
                <SortableContent Class="block">
                    <DataTable TData="MyItem" Data="@_right"
                               AdditionalRowAttributes="@s.RowAttributes"
                               BodyRowClass="hover:bg-transparent"
                               ShowPagination="false" ShowToolbar="false">
                        <Columns>
                            <DataTableColumn ... Header="" Width="40px">
                                <CellTemplate Context="row"><SortableItemHandle Class="mx-auto" /></CellTemplate>
                            </DataTableColumn>
                            <DataTableColumn Property="@(i => i.Name)" Header="Name" />
                        </Columns>
                    </DataTable>
                </SortableContent>
                <SortableOverlay Class="rounded opacity-80" />
            </Sortable>
            """;


        private static readonly IReadOnlyList<DemoPropRow> _sortableProps =
        [
            new("Items",                 "IList<TItem>",                           null,       "The list of items to sort. Required."),
            new("GetItemId",             "Func<TItem, string>",                    null,       "Extracts a unique string ID from each item. Required."),
            new("Orientation",           "SortableOrientation",                    "Vertical", "Drag axis: <code>Vertical</code>, <code>Horizontal</code>, <code>Grid</code>, or <code>Mixed</code>."),
            new("Group",                 "string?",                                null,       "Group name for cross-list DnD. Instances with the same group can exchange items."),
            new("OnItemsReordered",      "EventCallback<IList<TItem>>",            null,       "Fired after a successful same-list reorder. Receives the new ordered list."),
            new("OnItemTransferredOut",  "EventCallback<SortableTransferArgs>",    null,       "Fired on the source instance when an item is transferred out. Consumer should remove the item."),
            new("OnItemTransferredIn",   "EventCallback<SortableTransferArgs>",    null,       "Fired on the target instance when an item is transferred in. Consumer should insert the item at <code>args.Index</code>."),
            new("OnCanDrop",             "Func<SortableDragQueryArgs, bool>?",     null,       "Optional drop-time guard on the target instance. Return <code>false</code> to reject the transfer."),
            new("OnDragStart",           "EventCallback<string>",                  null,       "Fired when a drag begins. Receives the active item ID."),
            new("OnDragEnd",             "EventCallback<SortableDragEndArgs>",     null,       "Fired when a same-list drag ends. Carries ActiveId, OverId, FromIndex, ToIndex, and Moved."),
            new("OnDragCancel",          "EventCallback",                          null,       "Fired when a drag is cancelled via Escape or pointer cancel."),
            new("Class",                 "string?",                                null,       "Additional CSS classes on the root element."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _contentProps =
        [
            new("Class",        "string?",         "flex flex-col gap-2", "CSS classes merged with the default flex-column layout. Override with e.g. <code>Class=\"flex-row gap-3\"</code> or <code>Class=\"block\"</code> for table/DataView wrappers."),
            new("ChildContent", "RenderFragment?", null,                  "SortableItem children to render inside the container."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _itemProps =
        [
            new("Value",        "string",          null,    "Unique item identifier matching GetItemId output. Required."),
            new("AsHandle",     "bool",             "false", "When true the entire item surface is the drag handle."),
            new("Class",        "string?",          null,    "Additional CSS classes merged with the default item styles."),
            new("ChildContent", "RenderFragment?",  null,    "Content rendered inside the item."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _handleProps =
        [
            new("ChildContent", "RenderFragment?", null, "Custom handle content. Renders a six-dot grip icon by default."),
            new("Class",        "string?",         null, "Additional CSS classes on the handle button."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _overlayProps =
        [
            new("Class",        "string?",              null, "CSS classes merged with defaults (<code>rounded-lg shadow-lg opacity-90 transition-transform duration-150 data-[state=dragging]:scale-[1.05]</code>). Override shadow, opacity, scale, or easing."),
            new("ChildContent", "RenderFragment<string>?", null, "Custom ghost content. Context is the active item ID. When null the JS sensor auto-clones the dragged element."),
        ];
    }
}
