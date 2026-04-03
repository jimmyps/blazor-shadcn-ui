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
                                           class="flex items-center gap-3 rounded-lg border bg-card px-4 py-3 shadow-sm">
                        <SortableItemHandlePrimitive class="cursor-grab active:cursor-grabbing text-muted-foreground hover:text-foreground" />
                        <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded-lg shadow-lg opacity-90 transition-transform duration-150 data-[state=dragging]:scale-[1.05]" />
        </SortablePrimitive>
        """;

        private const string _verticalAsHandleCode =
        """
        @* AsHandle makes the entire item surface the drag target *@
        <SortablePrimitive TItem="MyItem"
                           Items="@items"
                           OnItemsReordered="@(r => items = r)"
                           GetItemId="@(i => i.Id)">
            <SortableContentPrimitive class="flex flex-col gap-2">
                @foreach (var item in items)
                {
                    <SortableItemPrimitive Value="@item.Id" AsHandle
                                           class="flex items-center gap-3 rounded-lg border bg-card px-4 py-3 shadow-sm cursor-grab active:cursor-grabbing select-none">
                        <span class="flex-1 text-sm font-medium">@item.Name</span>
                        <span class="text-xs text-muted-foreground">drag me</span>
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded-lg shadow-lg opacity-90 transition-transform duration-150 data-[state=dragging]:scale-[1.05]" />
        </SortablePrimitive>
        """;

        private const string _horizontalCode =
        """
        <SortablePrimitive TItem="MyItem"
                           Items="@items"
                           OnItemsReordered="@(r => items = r)"
                           GetItemId="@(i => i.Id)"
                           Orientation="SortableOrientation.Horizontal">
            <SortableContentPrimitive class="flex flex-row gap-3 flex-wrap">
                @foreach (var item in items)
                {
                    <SortableItemPrimitive Value="@item.Id" AsHandle
                                           class="flex items-center justify-center w-20 h-20 rounded-lg border bg-card shadow-sm cursor-grab active:cursor-grabbing select-none text-sm font-medium">
                        @item.Name
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded-lg shadow-lg opacity-90 transition-transform duration-150 data-[state=dragging]:scale-[1.05]" />
        </SortablePrimitive>
        """;

        private const string _customOverlayCode =
        """
        @* Provide a Context render fragment for a fully custom drag ghost *@
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
            <SortableOverlayPrimitive Context="activeId">
                @{
                    var active = items.FirstOrDefault(x => x.Id == activeId);
                }
                @if (active is not null)
                {
                    <div class="flex items-center gap-3 rounded-lg border-2 border-primary bg-card px-4 py-3 shadow-xl">
                        <span class="text-primary">⠿</span>
                        <span class="flex-1 text-sm font-semibold text-primary">@active.Name</span>
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
                                           class="flex items-center gap-3 rounded-lg border bg-card px-4 py-3 shadow-sm">
                        <SortableItemHandlePrimitive class="cursor-grab active:cursor-grabbing text-muted-foreground hover:text-foreground focus-visible:outline-2 focus-visible:outline-primary" />
                        <span class="flex-1 text-sm font-medium select-none">@item.Name</span>
                    </SortableItemPrimitive>
                }
            </SortableContentPrimitive>
            <SortableOverlayPrimitive class="rounded-lg shadow-lg opacity-90 transition-transform duration-150 data-[state=dragging]:scale-[1.05]" />
        </SortablePrimitive>
        """;

        // ── Props tables ───────────────────────────────────────────────────────

        private static readonly IReadOnlyList<DemoPropRow> _sortableProps =
        [
            new("Items",            "IList&lt;TItem&gt;",                       null,       "The list of items to sort. Required."),
            new("GetItemId",        "Func&lt;TItem, string&gt;",                null,       "Extracts a unique string ID from each item. Required."),
            new("Orientation",      "SortableOrientation",                       "Vertical", "Drag axis: <code>Vertical</code>, <code>Horizontal</code>, <code>Grid</code>, or <code>Mixed</code>."),
            new("OnItemsReordered", "EventCallback&lt;IList&lt;TItem&gt;&gt;",  null,       "Fired after a successful drop that changed the item order. Receives the new ordered list."),
            new("OnDragStart",      "EventCallback&lt;string&gt;",              null,       "Fired when a drag begins. Receives the active item ID."),
            new("OnDragEnd",        "EventCallback&lt;SortableDragEndArgs&gt;", null,       "Fired when a drag ends. Carries ActiveId, OverId, FromIndex, ToIndex, and Moved."),
            new("OnDragCancel",     "EventCallback",                             null,       "Fired when a drag is cancelled via Escape or pointer cancel."),
            new("ChildContent",     "RenderFragment?",                           null,       "Child content: SortableContentPrimitive, SortableOverlayPrimitive, etc."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _contentProps =
        [
            new("class",        "string?",         null, "HTML class attribute — apply flex layout, gap, and direction here."),
            new("ChildContent", "RenderFragment?", null, "SortableItemPrimitive children to sort."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _itemProps =
        [
            new("Value",        "string",          null,    "Unique identifier for this item. Must match GetItemId output. Required."),
            new("AsHandle",     "bool",             "false", "When true the entire item surface acts as the drag handle."),
            new("class",        "string?",          null,    "HTML class attribute — apply background, border, padding, and layout here."),
            new("ChildContent", "RenderFragment?",  null,    "Content rendered inside the item wrapper."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _handleProps =
        [
            new("class",        "string?",         null, "HTML class attribute — apply cursor, color, and focus styles here."),
            new("ChildContent", "RenderFragment?", null, "Custom handle content. Renders a six-dot grip icon by default."),
        ];

        private static readonly IReadOnlyList<DemoPropRow> _overlayProps =
        [
            new("class",        "string?",                        null, "HTML class attribute — apply shadow, opacity, and transition here."),
            new("ChildContent", "RenderFragment&lt;string&gt;?",  null, "Custom ghost content. Context is the active item ID. When null the sensor auto-clones the dragged element."),
        ];
    }
}
