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
