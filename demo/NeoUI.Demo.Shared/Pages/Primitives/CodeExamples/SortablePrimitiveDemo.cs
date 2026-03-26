namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class SortablePrimitiveDemo
    {
        private const string _verticalHandleCode =
        """
        <SortablePrimitive TItem="MyItem"
                           Items="@items"
                           OnItemsReordered="@(r => items = r)"
                           GetItemId="@(i => i.Id)">
            <SortableContentPrimitive class="flex flex-col gap-2">
                @foreach (var item in items)
                {
                    <SortableItemPrimitive Value="@item.Id"
                                           class="flex items-center gap-3 rounded border bg-card px-4 py-3">
                        <SortableItemHandlePrimitive class="cursor-grab text-muted-foreground" />
                        <span class="flex-1 text-sm">@item.Name</span>
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded border bg-card px-4 py-3 shadow-lg" />
        </SortablePrimitive>
        """;

        private const string _verticalAsHandleCode =
        """
        <SortablePrimitive TItem="MyItem"
                           Items="@items"
                           OnItemsReordered="@(r => items = r)"
                           GetItemId="@(i => i.Id)">
            <SortableContentPrimitive class="flex flex-col gap-2">
                @foreach (var item in items)
                {
                    <SortableItemPrimitive Value="@item.Id" AsHandle
                                           class="flex items-center gap-3 rounded border bg-card px-4 py-3 cursor-grab">
                        <span class="text-sm">@item.Name</span>
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded border bg-primary/10 px-4 py-3 shadow-lg" />
        </SortablePrimitive>
        """;

        private const string _horizontalCode =
        """
        <SortablePrimitive TItem="MyItem"
                           Items="@items"
                           OnItemsReordered="@(r => items = r)"
                           GetItemId="@(i => i.Id)"
                           Orientation="SortableOrientation.Horizontal">
            <SortableContentPrimitive class="flex flex-row gap-3">
                @foreach (var item in items)
                {
                    <SortableItemPrimitive Value="@item.Id" AsHandle
                                           class="flex items-center justify-center w-20 h-20 rounded border bg-card cursor-grab text-sm">
                        @item.Name
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded border bg-primary/10 shadow-lg flex items-center justify-center text-sm" />
        </SortablePrimitive>
        """;

        private const string _customOverlayCode =
        """
        <SortablePrimitive TItem="MyItem"
                           Items="@items"
                           OnItemsReordered="@(r => items = r)"
                           GetItemId="@(i => i.Id)">
            <SortableContentPrimitive class="flex flex-col gap-2">
                @foreach (var item in items)
                {
                    <SortableItemPrimitive Value="@item.Id"
                                           class="flex items-center gap-3 rounded border bg-card px-4 py-3">
                        <SortableItemHandlePrimitive class="cursor-grab text-muted-foreground" />
                        <span class="text-sm">@item.Name</span>
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive Context="activeId">
                @{
                    var active = items.FirstOrDefault(x => x.Id == activeId);
                }
                @if (active is not null)
                {
                    <div class="flex items-center gap-3 rounded border-2 border-primary bg-card px-4 py-3 shadow-xl">
                        <span class="text-sm font-semibold text-primary">@active.Name</span>
                    </div>
                }
            </SortableOverlayPrimitive>
        </SortablePrimitive>
        """;

        private const string _keyboardCode =
        """
        @* Focus a handle, then:
           Space / Enter  → grab / drop
           ↑ ↓            → move
           Escape         → cancel *@
        <SortablePrimitive TItem="MyItem"
                           Items="@items"
                           OnItemsReordered="@(r => items = r)"
                           GetItemId="@(i => i.Id)">
            <SortableContentPrimitive class="flex flex-col gap-2">
                @foreach (var item in items)
                {
                    <SortableItemPrimitive Value="@item.Id"
                                           class="flex items-center gap-3 rounded border bg-card px-4 py-3">
                        <SortableItemHandlePrimitive class="cursor-grab text-muted-foreground focus-visible:outline-2 focus-visible:outline-primary" />
                        <span class="text-sm">@item.Name</span>
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded border bg-card px-4 py-3 shadow-lg" />
        </SortablePrimitive>
        """;

        // ── Props tables ───────────────────────────────────────────────────────

        private static readonly IReadOnlyList<DemoPropRow> _sortableProps =
        [
            new("Items",             "IList&lt;TItem&gt;",                   null,       "The list of items to sort. Required."),
            new("OnItemsReordered",  "EventCallback&lt;IList&lt;TItem&gt;&gt;", null,    "Callback invoked after a drag reorders items. Receives the new ordered list."),
            new("GetItemId",         "Func&lt;TItem, string&gt;",            null,       "Function that extracts a unique string ID from each item. Required."),
            new("Orientation",       "SortableOrientation",                   "Vertical", "Drag axis: <code>Vertical</code>, <code>Horizontal</code>, or <code>Mixed</code>."),
            new("ChildContent",      "RenderFragment?",                       null,       "Child content: SortableContentPrimitive, SortableOverlayPrimitive, etc."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _contentProps =
        [
            new("ChildContent", "RenderFragment?", null, "SortableItemPrimitive children to sort."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _itemProps =
        [
            new("Value",        "string",           null,    "Unique identifier for this item. Must match GetItemId output. Required."),
            new("AsHandle",     "bool",              "false", "When true the entire item surface acts as the drag handle."),
            new("ChildContent", "RenderFragment?",   null,    "Content rendered inside the item wrapper."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _handleProps =
        [
            new("ChildContent", "RenderFragment?", null, "Custom handle content. Renders a six-dot grip icon by default."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _overlayProps =
        [
            new("ChildContent", "RenderFragment&lt;string&gt;?", null,
                "Custom ghost content. The context parameter is the active item ID. When null the sensor clones the source element."),
        ];
    }
}
