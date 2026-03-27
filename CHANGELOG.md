# Changelog

All notable changes to this project will be documented in this file.

## 2026-3-27 — Sortable drag-and-drop component

> **Release: `v3.8.0`**  
> **New feature.** Adds headless `SortablePrimitive` to `NeoUI.Blazor.Primitives` and styled `Sortable` wrapper to `NeoUI.Blazor`. No breaking changes.

---

**Design philosophy — composition over modification**

The Sortable component follows the same shadcn/ui principle that shapes the rest of NeoUI: *composability*. Drag-and-drop is layered *around* existing components rather than baked into them. `DataTable`, `DataView`, and any other list-rendering component remain unchanged — you simply wrap them with `<Sortable>`, set `Class="block"` on `<SortableContent>` so the container doesn't interfere with the inner layout, and drop a `<SortableItemHandle>` into a column `CellTemplate` or list template. No new variants, no feature flags, no re-implementation of the existing component. The handle wires itself to the nearest `SortableItem` through the primitive's context, so the drag behaviour activates exactly where the consumer places it.

This also means `Sortable` composes cleanly with any future component — the primitive only needs a `data-sortable-id` attribute on each item element to track identity, and `SortableContent` to register the droppable region.

---

### ✨ New Component — `SortablePrimitive<TItem>` (headless)

A fully headless drag-and-drop sortable primitive with pointer, touch, and keyboard support. Zero visual opinions — all styling is supplied by the consumer.

```razor
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
```

**Interaction model:**

- **Pointer/touch:** drag a handle (or the whole item when `AsHandle` is set) to reorder
- **Keyboard:** focus a handle → `Space`/`Enter` to grab → `↑`/`↓` (or `←`/`→` for horizontal) to move → `Space`/`Enter` to drop, `Escape` to cancel; arrow keys call `preventDefault` during a drag to prevent page scroll
- **Orientations:** `Vertical`, `Horizontal`, `Grid` (2-D grid reordering), `Mixed`

**Drag overlay architecture:**

`SortableOverlayPrimitive` is a fixed-position frame that JS positions over the cursor. When `ChildContent` is null the JS sensor auto-clones the source element into the overlay (`cloneNode(true)`) — the clone fills `100 × 100 %` and provides its own background/padding from the source styles. When `ChildContent` is provided the consumer renders a fully custom ghost; JS skips cloning to avoid duplicates (enforced via `data-has-child-content`). Visual effects (shadow, opacity, scale, transition) belong on the overlay frame.

**Table row clone fix:** when the dragged element is a `<tr>`, the sensor snapshots each `td`/`th` computed width before cloning and stamps it as an inline `width` on the clone cells — preserving shared table layout geometry outside its parent `<table>`.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `IList<TItem>` | — | The list of items to sort. Required. |
| `GetItemId` | `Func<TItem, string>` | — | Extracts a unique string ID from each item. Required. |
| `Orientation` | `SortableOrientation` | `Vertical` | Drag axis: `Vertical`, `Horizontal`, `Grid`, or `Mixed`. |
| `OnItemsReordered` | `EventCallback<IList<TItem>>` | — | Fired after a successful drop that changed item order. Receives the new ordered list. |
| `OnDragStart` | `EventCallback<string>` | — | Fired when a drag begins. Receives the active item ID. |
| `OnDragEnd` | `EventCallback<SortableDragEndArgs>` | — | Fired when a drag ends. Carries `ActiveId`, `OverId`, `FromIndex`, `ToIndex`, and `Moved`. |
| `OnDragCancel` | `EventCallback` | — | Fired when a drag is cancelled (Escape or pointer cancel). |

---

### ✨ New Component — `SortableContentPrimitive`

Container that registers itself with the JS sensor as the droppable region. Supply layout classes directly (`class="flex flex-col gap-2"`, `class="block"` for table/DataView wrappers, etc.).

---

### ✨ New Component — `SortableItemPrimitive`

Wrapper for each draggable item. When `AsHandle` is false (default), only a nested `SortableItemHandlePrimitive` initiates a drag; when `AsHandle` is true the entire element is the handle.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | — | Unique identifier matching `GetItemId` output. Required. |
| `AsHandle` | `bool` | `false` | When true the entire item surface is the drag handle. |

---

### ✨ New Component — `SortableItemHandlePrimitive`

Focusable grip button. Renders a six-dot braille character by default; replace via `ChildContent`. Apply `cursor-grab active:cursor-grabbing` and `focus-visible:outline-2 focus-visible:outline-primary` here.

---

### ✨ New Component — `SortableOverlayPrimitive`

Fixed-position overlay frame shown while dragging. JS sets `data-state="dragging"` on the element after positioning it, enabling Tailwind data-attribute variants (`data-[state=dragging]:scale-[1.05]`) for CSS-driven visual effects — no JS inline transforms needed. When `ChildContent` (a `RenderFragment<string>`, context = active item ID) is provided the consumer controls the ghost entirely.

---

### ✨ New Component — `Sortable<TItem>` (styled)

Pre-styled layer on top of `SortablePrimitive`. Composes `SortableContent`, `SortableItem`, `SortableItemHandle`, and `SortableOverlay` with sensible opinionated defaults so most use-cases need zero extra CSS.

The component is deliberately *open-ended*: its `ChildContent` is not a fixed slot for `SortableItem` children alone — it accepts any markup, including fully-featured NeoUI components like `DataView` or `DataTable`. DnD behaviour is activated purely by the presence of `data-sortable-id` on descendant elements, which means **no existing component needs to be modified or re-wrapped** to gain sortability.

```razor
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
```

Accepts the same `Items`, `GetItemId`, `Orientation`, `OnItemsReordered`, `OnDragStart`, `OnDragEnd`, and `OnDragCancel` parameters as `SortablePrimitive<TItem>`.

---

### ✨ New Component — `SortableContent`

Styled content container. Defaults to `flex flex-col gap-2`. Override with `Class="flex-row gap-3"` for horizontal, or `Class="block"` when wrapping a `DataView` / `DataTable`.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `flex flex-col gap-2` | CSS classes merged with the default layout. |

---

### ✨ New Component — `SortableItem`

Styled item wrapper with `flex items-center gap-3 rounded-lg border bg-card px-4 py-3 shadow-sm` defaults, placeholder opacity while dragging, and focus ring. Accepts `AsHandle` and `Class` overrides.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | — | Unique identifier matching `GetItemId` output. Required. |
| `AsHandle` | `bool` | `false` | When true the entire item surface is the drag handle. |
| `Class` | `string?` | `null` | Additional CSS classes merged with defaults. |

---

### ✨ New Component — `SortableItemHandle`

Styled grip handle button. Renders a six-dot grip icon; replace via `ChildContent`. `Class` merges additional styles.

---

### ✨ New Component — `SortableOverlay`

Styled overlay frame. Defaults: `rounded-lg shadow-lg opacity-90 transition-transform duration-150 data-[state=dragging]:scale-[1.05]`. These defaults give a floating card appearance and an animated scale-up on drag start — all driven by CSS with no JS inline styles. Override shadow, opacity, scale, or easing via `Class`; provide a `ChildContent` (context = active item ID) for a fully custom ghost.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | *(see above)* | Merged with the default overlay classes. |
| `ChildContent` | `RenderFragment<string>?` | `null` | Custom ghost. Context is the active item ID. When null the sensor auto-clones the dragged element. |

---

### 🔧 Enhancement — `DataTable<TData>`: `AdditionalRowAttributes` parameter

This is a minimal, non-invasive hook that unlocks full composability with `Sortable` and can be used in any use cases that require additional attributes on each row. The only change to `DataTable` is the addition of `AdditionalRowAttributes` (`Func<TData, Dictionary<string, object>?>?`), which lets callers stamp any HTML attributes — `data-*`, `aria-*`, or otherwise — onto each rendered `<tr>`. The common use case is supplying `data-sortable-id` for Sortable row reorder, but the API is intentionally general-purpose. Everything else — column definitions, sorting, selection, toolbar, pagination — works exactly as before.

The drag handle is placed inside any `CellTemplate` in the normal Columns API. There is no new `SortableColumn` type, no `Draggable` flag on the table, and no modified render path. The handle simply finds its nearest `SortableItem` context through the primitive's DOM-registration, and the drag engine does the rest.

```razor
<Sortable TItem="TaskItem" Items="@items" OnItemsReordered="@(r => items = r)" GetItemId="@(i => i.Id)">
    <SortableContent Class="block">
        <DataTable TData="TaskItem" Data="@items"
                   AdditionalRowAttributes="@(i => new Dictionary<string, object> { ["data-sortable-id"] = i.Id })"
                   ShowPagination="false" ShowToolbar="false">
            <Columns>
                <DataTableColumn TData="TaskItem" TValue="string" Property="@(i => i.Id)" Header="" Width="40px">
                    <CellTemplate Context="row">
                        <SortableItemHandle Class="mx-auto" />
                    </CellTemplate>
                </DataTableColumn>
                ...
            </Columns>
        </DataTable>
    </SortableContent>
    <SortableOverlay Class="rounded" />
</Sortable>
```

The same composability pattern applies to `DataView` with no `DataView` changes at all — place `<SortableItem>` directly inside `<ListTemplate>` or `<GridTemplate>` and it works.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `AdditionalRowAttributes` | `Func<TData, Dictionary<string, object>?>?` | `null` | Callback supplying extra HTML attributes for each body `<tr>`. Use it to attach `data-sortable-id` for Sortable row reorder, or any `data-*`/`aria-*` attributes your consumers need. |

---

### 📖 Demo — `SortableDemo` & `SortablePrimitiveDemo`

**`/components/sortable`** — styled component demos covering: default vertical list, full-item drag, horizontal chips, custom handle icon, `DataView` list/grid composability, `DataTable` integration, and primitive escape hatch.  
**`/primitives/sortable`** — headless primitive demos covering: vertical with handle, full-item drag, horizontal, custom overlay content (custom ghost rendering), and keyboard navigation.

Both pages include a complete **API Reference** section.

---



> **Release: `v3.7.1`**  
> **Enhancement.** Affects `SplitButton` and `SidebarPillNav` / `SidebarPillNavItem` in `NeoUI.Blazor`. No breaking changes.

---

### 🔧 Enhancement — `SplitButton`: per-segment class overrides & icon centering

Consumers can now tailor the appearance of each button segment independently without needing component-level boolean flags.

**Changes:**

- **`PrimaryClass`** (`string?`) — additional CSS merged onto the left (primary action) segment via `ClassNames.cn`. Use to add rounding, margin tweaks, etc. (e.g. `"rounded-l-full [&>span]:me-0 [&>span]:ml-1"`).
- **`DropdownClass`** (`string?`) — additional CSS merged onto the right (dropdown chevron) segment (e.g. `"rounded-r-full [&>svg]:mr-1"`).
- **`DropdownButtonSize`** — fixed: `Icon`, `IconSmall`, and `IconLarge` sizes now map to their correct counterparts instead of falling through to `Default`, which caused the right segment to be taller than the left.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `PrimaryClass` | `string?` | `null` | Extra CSS classes merged onto the primary button segment. |
| `DropdownClass` | `string?` | `null` | Extra CSS classes merged onto the dropdown chevron button segment. |

---

### 🔧 Enhancement — `SidebarPillNavItem`: `Title` parameter

New `Title` parameter separates the native browser tooltip (`title` attribute) from the accessible label (`aria-label`), which always uses `Label`.

Set `Title=""` to suppress the native tooltip when using a custom `<Tooltip>` component in consumer code.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | `Label` | Overrides the HTML `title` attribute. Set to `""` to suppress the native browser tooltip. |

---

### 🔧 Enhancement — `SidebarPillNav`: built-in `TooltipProvider` & expand button tooltip

`SidebarPillNav` now owns a `TooltipProvider` internally, so consumers can use NeoUI `<Tooltip>` components directly inside `ChildContent` and `TrailingContent` without adding a provider themselves.

The leading expand button now uses `<Tooltip>` / `<TooltipContent>` (with the localized `Sidebar.Pill.ExpandButton` string) instead of a native `title` attribute.

---

> **Release: `v3.7.0`**  
> **New feature.** Affects `Sidebar`, `SidebarTrigger`, and five new `SidebarPill*` companion components in `NeoUI.Blazor`. All additions are additive — no breaking changes.

---

### ✨ New Feature — `SidebarCollapsedMode.Pill`

A new `CollapsedMode` value transforms the sidebar into a floating pill navigation bar when collapsed. The sidebar shrinks to `w-0` via a CSS `transition-[width,opacity]`; simultaneously, a `fixed`-positioned pill bar animates down from the top. Both directions share the same `duration-500` transition so the handoff is seamless. Mobile breakpoints are unaffected — the pill nav is desktop-only.

```razor
<SidebarProvider CollapsedMode="SidebarCollapsedMode.Pill" DefaultOpen="true" HeightClass="h-screen">
    <Sidebar Collapsible="true">...</Sidebar>

    <SidebarPillNav>
        <ChildContent>
            <SidebarPillNavItem Label="Dashboard" IsActive="@(active == 0)" OnClick="@(() => active = 0)">
                <LucideIcon Name="layout-dashboard" Size="16" />
            </SidebarPillNavItem>
        </ChildContent>
        <TrailingContent>
            <SidebarPillNavItem Label="Search" OnClick="OpenSearch">
                <LucideIcon Name="search" Size="16" />
            </SidebarPillNavItem>
        </TrailingContent>
    </SidebarPillNav>

    <SidebarInset>
        <SidebarPillFade />
        <SidebarPillInset>
            ...page content...
        </SidebarPillInset>
    </SidebarInset>
</SidebarProvider>
```

**Single scroll-container architecture:** `SidebarInset` (`<main>`) is the sole `overflow-y-auto` container. `SidebarPillFade` uses `sticky top-0` inside that context so the gradient overlay correctly tracks scrolling content. `SidebarPillInset` uses `shrink-0` to prevent a secondary scroll context. No negative-margin tricks are needed.

---

### ✨ New Component — `SidebarPillNav`

The floating pill bar. Only rendered when `CollapsedMode="Pill"` and not on mobile. Contains a leading expand button, consumer icon items via `ChildContent`, and an optional `TrailingContent` slot separated by a divider.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ExpandIcon` | `string` | `"panel-left"` | Icon name for the expand/restore sidebar button. |
| `ExpandButtonClass` | `string?` | `null` | Additional CSS classes on the expand button. |
| `TrailingContent` | `RenderFragment?` | — | Optional trailing items (e.g. search, settings) after a second divider. Requires explicit `<ChildContent>` / `<TrailingContent>` tags when using both slots. |
| `Class` | `string?` | `null` | Additional CSS classes on the pill `<nav>` container. |

---

### ✨ New Component — `SidebarPillNavItem`

Icon-only circular button for use inside `<SidebarPillNav>`. Supports button mode (index-based active state) and NavLink mode (route-based active state).

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Label` | `string?` | — | Tooltip text and `aria-label`. |
| `IsActive` | `bool` | `false` | Active highlight (button mode only). |
| `Href` | `string?` | — | Renders a `<NavLink>` anchor when set; active state is route-driven. |
| `Match` | `NavLinkMatch` | `Prefix` | NavLink route-match strategy. |
| `ActiveClass` | `string?` | `null` | Overrides active-state colour classes (`bg-primary text-primary-foreground …`). |
| `InactiveClass` | `string?` | `null` | Overrides inactive-state colour classes (`text-foreground hover:bg-accent …`). |
| `Class` | `string?` | `null` | Additional CSS classes merged on top of all other classes. |

---

### ✨ New Component — `SidebarPillFade`

Sticky gradient bar that slides in at the top of `<SidebarInset>` when the pill nav is visible. Provides pill-nav clearance via real layout height (`h-0 → h-24`) — no negative-margin offset needed — while applying a `backdrop-blur-sm` + `bg-gradient-to-b` depth effect. Animates in sync with the pill nav (`duration-500`). Place as the **first** direct child of `<SidebarInset>`.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Class` | `string?` | `null` | Additional CSS classes on the fade bar. |

---

### ✨ New Component — `SidebarPillInset`

Content wrapper that transitions `padding-top` between two class sets as the pill nav appears and disappears, so the content edge stays flush with the bottom of the fade gradient.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ExpandedClass` | `string` | `"p-6 lg:p-8"` | Classes applied when the sidebar is open (pill nav hidden). |
| `CollapsedClass` | `string` | `"p-6 lg:p-8 pt-0 lg:pt-0"` | Classes applied when the pill nav is visible. |
| `Class` | `string?` | `null` | Additional CSS classes always applied. |

---

### ✨ New Component — `SidebarPillSpacer`

Auxiliary height spacer (`h-0 → h-20`, `transition-[height] duration-500`) for use as a lightweight clearance alternative to `<SidebarPillFade>` when no gradient overlay is needed.

---

### 🔧 Enhancement — `SidebarTrigger`: `Icon` parameter

New `Icon` (`string?`) overrides the context-aware default icon without requiring a full `ChildContent` replacement. Falls back to `panel-top` in pill mode and `panel-left` in all other modes when `null`.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Icon` | `string?` | `null` | Overrides the default icon. Context-aware fallback: `panel-top` (pill) / `panel-left` (other). No effect when `ChildContent` is set. |

---

### 📖 Demo — `SidebarDemo`: Pill Mode inline example

A **Pill Mode** section has been added to `/components/sidebar`, inserted between the Inset Variant and Collapsible Icons examples. The demo starts collapsed so the pill nav and gradient fade are immediately visible; clicking the expand icon in the pill restores the full sidebar. Active tab state is tracked via `_pillActive` / `_pillItems`. The **API Reference** props table on the same page has been extended to cover all new `SidebarPill*` parameters and the `SidebarTrigger` `Icon` override.

---

## 2026-3-20

> **Bug fix.** Affects `DataTable<TData>` in `NeoUI.Blazor` and `TableRow` in `NeoUI.Blazor.Primitives`. No breaking changes.

---

### 🐛 Fix — `DataTable<TData>`: keyboard focus ring invisible in Safari

Safari does not paint `box-shadow` on `<tr>` elements — the style is computed but never rendered. The previous implementation applied Tailwind's `focus:ring-2 focus:ring-ring focus:ring-inset` directly to the `<tr>` via `TableRow`, which was already the per-cell inset box-shadow approach for pinned tables but not for standard ones.

**Changes:**

- CSS focus ring rules migrated from tr-level `box-shadow` to per-cell inset `box-shadow` on `[role="cell"]` / `[role="columnheader"]` — identical to the already-working pinned-table approach, now applied universally
- Rules scoped to `[data-keyboard-nav]` on the table container so they only activate inside a `DataTable` with `EnableKeyboardNavigation="true"` and don't bleed into other components that use `role="row"`
- `data-keyboard-nav="true"` attribute added to the `DataTable` container div (conditional on `EnableKeyboardNavigation`)
- Stale `focus:ring-2 focus:ring-ring focus:ring-inset focus:z-10 relative` Tailwind classes removed from `TableRow`; only `focus:outline-none` retained as a browser-default suppressor

---

## DataTable mobile-responsive pagination

> **UI improvement.** Affects `DataTable<TData>` in `NeoUI.Blazor`. No breaking changes.

---

### 📱 Improvement — `DataTable<TData>`: mobile-responsive pagination bar

The DataTable pagination row now adapts gracefully to narrow viewports instead of overflowing or wrapping awkwardly at small screen sizes.

**Three-tier responsive layout:**

| Breakpoint | Visible controls |
|---|---|
| `< 640 px` (mobile) | Row count info · Previous / Next |
| `≥ 640 px` (sm) | + Page-size selector · First / Last |
| `≥ 1024 px` (lg) | + Page X of Y display |

Responsive visibility is applied via `hidden sm:flex` / `hidden lg:flex` Tailwind classes set directly on each pagination component's `Class` parameter — no extra wrapper `<div>` elements required.

---

## 2026-3-19 — DI-based ILocalizer localization system

> **Release: `v3.6.4`**  
> **Library change.** Affects `NeoUI.Blazor`. All changes are additive — no breaking changes to existing APIs.

---

### ✨ New Feature — `ILocalizer` / `DefaultLocalizer` localization abstraction

All UI chrome strings rendered by NeoUI components — placeholders, button labels, ARIA attributes, empty states, pagination text, and screen-reader text — are now resolved through a single DI-registered `ILocalizer` abstraction. The library ships English defaults for every key out of the box.

**New types:**

| Type | Description |
|---|---|
| `ILocalizer` | Interface with two indexer overloads: `this[string key]` and `this[string key, params object[] arguments]`. |
| `DefaultLocalizer` | Concrete implementation with all built-in English defaults. Supports runtime override via `Set(key, value)`. |

**`AddNeoUIComponents()` overload:**

```csharp
builder.Services.AddNeoUIComponents(localizer =>
{
    localizer.Set("Combobox.Placeholder", "Wählen Sie eine Option...");
    localizer.Set("DataTable.Loading", "Laden...");
});
```

`ILocalizer` is registered as **Scoped** so each Blazor Server circuit and WebAssembly session gets an independent instance. A singleton variant is also supported for static startup-key overrides.

**Components wired up:**

`Alert`, `Breadcrumb`, `Calendar`, `Carousel`, `Combobox`, `DataGrid`, `DataTable`, `DataView`, `DatePicker`, `DateRangePicker`, `Dialog`, `MultiSelect`, `NumericInput`, `Pagination` (`PaginationPrevious`, `PaginationNext`, `PaginationInfo`, `PaginationPageDisplay`, `PaginationPageSizeSelector`), `Rating`, `ResponsiveNav`, `Sheet`, `Sidebar`, `TagInput`

All string parameters on these components remain optional — existing code passing explicit strings continues to work unchanged, with the explicit value taking priority over the localizer.

**Integration patterns:**

| Pattern | How |
|---|---|
| **Option A — startup-time override** | Pass an `Action<DefaultLocalizer>` to `AddNeoUIComponents()`. |
| **Option B — `IStringLocalizer<T>` / `.resx`** | Subclass `DefaultLocalizer`, override the indexers to delegate to `IStringLocalizer<T>`, register with `AddScoped<ILocalizer, AppLocalizer>()`. |

---

### 📖 Demo — `/components/localization` page

New demo page covering the full localization system:

| Section | What it shows |
|---|---|
| **Overview** | Library-chrome vs culture-aware formatting split, with live chip examples. |
| **Live Language Preview** | `ToggleGroup` language switcher that rebuilds a `DefaultLocalizer` per selection; updates `Combobox`, `MultiSelect`, `TagInput`, `DatePicker`, and `Pagination` in real time across English, German, French, Spanish, and Japanese. |
| **Default Behavior** | Components rendered without explicit string parameters, falling back to injected `ILocalizer`. |
| **Per-Instance Override** | Explicit string parameters passed directly to components to override the localizer for a single instance. |
| **Option A — Startup-Time Override** | Code snippet showing `AddNeoUIComponents(localizer => { ... })`. |
| **Option B — Full IStringLocalizer&lt;T&gt; Integration** | Code snippet showing `DefaultLocalizer` subclass wired to `.resx` resource files. |
| **DI Lifetime: Scoped vs Singleton** | Guidance on when to use each lifetime with registration examples. |
| **Live Key Lookup** | Interactive key resolver backed by the injected `ILocalizer`. |
| **Key Reference** | Full table of all 70+ built-in localization keys and their English default values. |

---

## `ChartTooltip` `AppendToBody` + ClassNames.cn standardization

> **Internal / code quality + minor API addition to ChartTooltip.** No breaking changes. `NeoUI.Blazor` only.

---

### ✨ Feature — `ChartTooltip`: new `AppendToBody` parameter

Added `AppendToBody` (`bool?`) to `<ChartTooltip>`. When `true`, the tooltip DOM element is appended to `document.body` instead of being nested inside the chart container — preventing tooltip clipping when a chart is placed inside an `overflow:hidden` ancestor. Maps directly to ECharts' `tooltip.appendToBody` option.

---

### 🔧 Refactor — Replace `StringBuilder` CSS class building with `ClassNames.cn`

Six components migrated to the `ClassNames.cn` merge helper: `Avatar`, `AvatarImage`, `AvatarFallback`, `Checkbox`, `Label`, `Separator`. `using System.Text` removed from all six files. Stale `<remarks>` XML doc blocks removed.

---

## 2026-3-18 — Add DataTable column resizing, column reordering, row context menu, and add Chart color palette for Pie and Funnel

> **Release: `v3.6.3`**  
> **Library change.** Affects `DataTable<TData>` in `NeoUI.Blazor`. All changes are additive — no breaking changes.

---

### ✨ New Feature — `SyncWidthOnResize` — table width tracks column widths

A new `SyncWidthOnResize` parameter (default `false`) keeps the `<table>` element's total width equal to the sum of all `<col>` widths during and after resize. When disabled (default), the table retains its container width and unused space shows the container background. Enable alongside `TableContainerClass="border-0"` for a borderless table that naturally shrinks when columns are narrowed.

**New `DataTable` parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `SyncWidthOnResize` | `bool` | `false` | Keeps `<table>` width = sum of all column widths during and after resize. Pair with `TableContainerClass="border-0"`. |

---

### ✨ New Feature — `TableContainerClass` — style the inner table wrapper

A new `TableContainerClass` parameter allows passing Tailwind classes directly to the `<div>` that wraps the `<table>` (the element with `rounded-md border overflow-x-auto`). The existing `Class` parameter targets the outer container; `TableContainerClass` targets the grid's direct parent.

**New `DataTable` parameter:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `TableContainerClass` | `string?` | `null` | Additional CSS classes on the inner table container div. Use `border-0` to remove the border. |

---

### ✨ New Feature — `Striped` / `StripeClass` — zebra row striping

Two new parameters enable alternating row background colours. The stripe colour is intentionally lighter than hover (`/30` vs `/50`) so the visual hierarchy — stripe → hover → selected — is always legible.

**New `DataTable` parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Striped` | `bool` | `false` | Enables alternating row backgrounds (zebra striping). |
| `StripeClass` | `string?` | `even:bg-muted/30 even:hover:bg-muted/70` | Tailwind class for the stripe. Override to change colour or swap odd/even, e.g. `odd:bg-blue-50 dark:odd:bg-blue-950/20`. |

---

### ✨ Improvement — Smooth column reorder animation

On drop, the affected columns (dragged column + all shifted neighbours between source and target slot) fade out instantly, Blazor re-renders silently, then all affected columns fade back in at their new positions with a `500ms ease-in` opacity transition. Columns outside the affected range remain fully visible throughout.

---

### 🐛 Bug Fix — Column reorder double-invocation

`OnAfterRenderAsync` used a `null` guard to skip re-initialising JS interop, but two concurrent renders could both pass the check before either `await` resolved — registering `pointerdown` on each `<th>` twice and causing `onUp` to fire twice per drop. Fixed by setting `_reorderInitializing` / `_resizeInitializing` bool flags synchronously before the `await`, preventing the race.

---

### 🐛 Bug Fix — Column reorder incorrect order after multiple moves

After fixing double-invocation, the reorder position was still wrong after three or more moves. Root cause: JS `commitDomReorder` moved DOM nodes directly, desyncing Blazor's internal logical-element tree. Any subsequent Blazor render would diff against a stale virtual DOM and apply corrupt `insertBefore` operations. Fixed by removing `commitDomReorder` from the drop handler entirely — Blazor is now the sole owner of DOM element ordering. JS handles only visual feedback via CSS transforms; on drop, `OnColumnReordered` updates `_columns`, calls `StateHasChanged()`, and Blazor renders the correct order against an unmodified DOM.

---


### 🐛 Bug Fix — Column resize width reset in Safari

`releasePointerCapture` dispatches `lostpointercapture` synchronously, which re-entered the `onPointerUp` handler with `clientX = 0` in Safari — causing the column to snap to its minimum width on every drag. Fixed by removing all event listeners **before** calling `releasePointerCapture`, and adding a `resizeDone` guard to prevent any double-invocation.

Also fixed: `pointercancel` was registered but never removed — a minor listener leak now closed.

---

## Chart color palette for Pie and Funnel

> **Library change.** Affects `PieChart<TData>` and `FunnelChart<TData>` in `NeoUI.Blazor`. Contains one **breaking change** to the `Pie` series component.

---

### ⚠️ Breaking Change — `Pie` series: `Color` parameter removed

The `Color` parameter on `<Pie>` has been removed. It was previously documented as a legacy no-op (it was never wired into the chart builder) so real-world impact is minimal, but any usage will cause a compile error.

**Migration — replace `Color` with `Colors`:**

```razor
<!-- Before (was silently ignored) -->
<Pie DataKey="Value" NameKey="Label" Color="#e11d48" />

<!-- After — single color repeated across all slices -->
<Pie DataKey="Value" NameKey="Label" Colors="@(new[] { "#e11d48" })" />

<!-- Or provide a full palette -->
<Pie DataKey="Value" NameKey="Label" Colors="@(new[] { "#e11d48", "#0ea5e9", "#16a34a" })" />
```

---

### 🐛 Bug Fix — `PieChart` and `FunnelChart` not using the CSS variable color palette

`PieChart` and `FunnelChart` were not applying any color to their data items, causing both charts to fall back to ECharts' built-in default colors instead of the theming CSS variables (`--chart-1` … `--chart-5`) used by every other chart type (`BarChart`, `LineChart`, `AreaChart`, `ScatterChart`, `RadarChart`, `RadialBarChart`, `GaugeChart`).

Each slice / segment now receives an `itemStyle.color` entry at the data-item level, cycling through the auto palette by default.

---

### ✨ New Feature — `Pie` series: `Colors` parameter for custom slice palette

A new `Colors` parameter on the `<Pie>` series component allows passing a custom color array. Colors are distributed across slices by index and wrap automatically when there are more slices than colors.

**New `Pie` parameter:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Colors` | `string[]?` | `null` | Custom palette for slices. Cycles through the array — e.g. 3 colors for 5 slices → 0,1,2,0,1. `null` = auto CSS-variable palette. |

---

### ✨ New Feature — `Funnel` series: `Colors` parameter for custom segment palette

Same capability added to `<Funnel>`. Each segment in a funnel series is assigned a color from the array, cycling as needed.

**New `Funnel` parameter:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Colors` | `string[]?` | `null` | Custom palette for segments. Cycles through the array when there are more segments than colors. `null` = auto CSS-variable palette. |

## Add DataTable column resizing, column reordering, and row context menu

> **Library change.** Affects `DataTable<TData>` in `NeoUI.Blazor`, `ContextMenu` in `NeoUI.Blazor`, and `ContextMenuRootPrimitive` / `TableRow` in `NeoUI.Blazor.Primitives`. All changes are additive — no breaking changes to existing APIs.

---

### ✨ New Feature — `DataTable<TData>` column resizing

Columns can now be drag-resized at runtime via a handle on the right edge of each header cell. Double-clicking a resize handle auto-fits the column to its widest rendered content using an off-screen sandbox measurement strategy (single forced-reflow batch). Resizing enforces a configurable minimum width and clamps against the natural width of the header content so the sort icon is never clipped.

**New `DataTable` parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Resizable` | `bool` | `false` | Enables resize handles on all columns. Activates `table-layout: fixed` automatically. |
| `MinColumnWidth` | `int` | `80` | Minimum column width in pixels enforced during drag. |
| `OnColumnResize` | `EventCallback<(string ColumnId, string Width)>` | — | Raised once when the user releases the resize handle. |

**New `DataTableColumn` parameter:**

| Parameter | Type | Description |
|---|---|---|
| `Resizable` | `bool?` | Per-column override. `null` inherits the table-level `Resizable` setting. |

---

### ✨ New Feature — `DataTable<TData>` column reordering

Column headers can be dragged to reorder columns at runtime. The dragged column follows the cursor via `translateX` (no HTML5 ghost image). Adjacent columns shift with a 200 ms ease animation — identical to the dnd-kit `closestCenter` + `horizontalListSortingStrategy` behaviour. A 5 px movement threshold prevents accidental reorders on sort-header clicks. Pinned columns and the selection checkbox column are always excluded as both drag sources and drop targets. DOM order is committed immediately on drop with no Blazor re-render; C# state is synced via a single `JSInvokable` callback.

**New `DataTable` parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Reorderable` | `bool` | `false` | Enables drag-to-reorder on all eligible columns. |
| `OnColumnReorder` | `EventCallback<(string ColumnId, int NewIndex)>` | — | Raised when a column is dropped into a new position. Provides the column ID and new zero-based display index. |

**New `DataTableColumn` parameter:**

| Parameter | Type | Description |
|---|---|---|
| `Reorderable` | `bool?` | Per-column override. `null` inherits the table-level `Reorderable` setting. Pinned columns are always excluded regardless of this value. |

---

### ✨ New Feature — `DataTable<TData>` row context menu

Right-clicking any data row opens a context menu scoped to that row. The `RowContextMenu` render fragment receives a `DataTableRowMenuContext<TData>` with the row's item, the current selection, and the IDs of all visible columns. Any `ContextMenuItem`, `ContextMenuSeparator`, or sub-menu components are valid children. The menu repositions correctly when right-clicking different rows in quick succession without toggling the open state.

**New `DataTable` parameter:**

| Parameter | Type | Description |
|---|---|---|
| `RowContextMenu` | `RenderFragment<DataTableRowMenuContext<TData>>?` | Template for the context menu. Receives row and selection context. |

**New type — `DataTableRowMenuContext<TData>`:**

| Member | Type | Description |
|---|---|---|
| `Item` | `TData` | The data item for the right-clicked row. |
| `SelectedItems` | `IReadOnlyList<TData>` | Currently selected items at the time of the right-click. |
| `VisibleColumns` | `IReadOnlyList<string>` | IDs of all currently visible columns, in display order. |

---

### ✨ Enhancement — `ContextMenu` / `ContextMenuRootPrimitive` programmatic positioning

`ContextMenu` and its underlying `ContextMenuRootPrimitive` now support fully programmatic open-and-position. This is the mechanism used by the new `DataTable` row context menu to open at the pointer location without a `ContextMenuTrigger`.

**New parameters on `ContextMenu` and `ContextMenuRootPrimitive`:**

| Parameter | Type | Description |
|---|---|---|
| `X` | `double` | Viewport X coordinate for programmatic positioning. |
| `Y` | `double` | Viewport Y coordinate for programmatic positioning. |

**New method on `ContextMenu` and `ContextMenuRootPrimitive`:**

| Method | Description |
|---|---|
| `OpenAt(double x, double y)` | Opens (or repositions) the context menu at the specified viewport coordinates without a parameter round-trip. |

---

### 📖 Demo — Column resize, reorder, and row context menu sections

Two new sections added to the `/components/datatable` demo page:

| Section | What it shows |
|---|---|
| **Column Resize & Reorder** | Employee directory with `Resizable` and `Reorderable` enabled. Drag handles on every header, double-click to auto-fit, and live `OnColumnResize` / `OnColumnReorder` event readout below the table. |
| **Row Context Menu** | Same employee dataset with a `RowContextMenu` that exposes per-row actions (View Profile, Send Email, separator, Remove) and displays the selected count when multiple rows are selected. |

---

## 2026-3-17 — DataTable column pinning and hierarchical tree rows

> **Release: `v3.6.2`**  
> **Library change.** Affects `DataTable<TData>` in `NeoUI.Blazor` and `TableRow` in `NeoUI.Blazor.Primitives`. All changes are additive — no breaking changes to existing APIs.

---

### ✨ New Feature — `DataTable<TData>` column pinning

Columns can now be pinned to the left or right edge of the table, staying fixed while the rest of the table scrolls horizontally. Pinned columns render with a frosted-glass treatment — semi-transparent background with backdrop blur — so scrolled content bleeds through subtly while keeping cell content fully readable. Separator borders mark the boundary between pinned and scrollable regions, and all interaction states (hover, selected, keyboard focus) render correctly across both pinned and non-pinned cells.

**New `DataGridColumn` parameter:**

| Parameter | Type | Description |
|---|---|---|
| `Pinned` | `ColumnPinnedSide` | Pins the column. `Left` anchors to the left edge; `Right` to the right; `None` (default) leaves it unpinned. |

**New enum:** `ColumnPinnedSide` — `None`, `Left`, `Right`

---

### ✨ New Feature — `DataTable<TData>` hierarchical / self-referencing tree rows

`DataTable` can now display hierarchical data from a self-referencing model. Rows expand and collapse inline, with optional lazy loading of children on demand via an async delegate. Search automatically expands ancestor nodes to reveal matching descendants, and pagination is hidden in tree mode.

**New `DataTable` parameters:**

| Parameter | Type | Description |
|---|---|---|
| `ChildrenProperty` | `Func<TData, IEnumerable<TData>?>?` | Returns child items for a row. Activates tree mode when set. |
| `LoadChildrenAsync` | `Func<TData, Task<IEnumerable<TData>>>?` | Lazy-loads children on first expand. Used when children are not pre-populated. |
| `HasChildrenField` | `Func<TData, bool>?` | Determines whether an expand toggle is shown before children are loaded. |
| `ValueField` | `Func<TData, object>?` | Unique key per row, used to track expanded state. |
| `ExpandedValues` | `IEnumerable<object>?` | Externally controlled set of expanded row keys. |
| `ExpandedValuesChanged` | `EventCallback<IEnumerable<object>>` | Notifies when expanded state changes. |

---

### ✨ Enhancement — `DataTable<TData>` keyboard navigation

Arrow-key row navigation (`↑` / `↓`) and `Enter` / `Space` selection are now fully operational. Navigation is independent of `SelectionMode` — arrow keys move focus in all modes. Interactive elements inside cells (buttons, inputs, checkboxes) receive their own keyboard events without interfering with row-level handling.

---

### 📖 Demo — Column pinning and tree rows sections

Two new sections added to the `/components/datatable` demo page:

| Section | What it shows |
|---|---|
| **Column Pinning** | Employee directory with 8 columns — Name pinned left, Actions pinned right. Demonstrates frosted-glass effect, separator borders, and full hover/selected/focus states while scrolling. |
| **Tree / Hierarchical Rows (Chart of Accounts)** | 13-column accounting hierarchy with lazy-loaded children. Account Name pinned left, Actions pinned right — combines tree rows and column pinning in one demo. Expand/collapse, search with auto-expand, and async child loading. |

---

## 2026-3-16 — DataTable virtualization, adaptive DataView grid, and sort API update

> **Release: `v3.6.1`**  
> **Library change.** Affects `DataTable<TData>` and `DataView<TItem>` in `NeoUI.Blazor`. Introduces two virtualisation modes and a new adaptive grid parameter. Contains one **breaking change** to `DataTableRequest`.

---

### ⚠️ Breaking Change — `DataTableRequest` sort fields replaced by `SortDescriptors`

`DataTableRequest.SortColumn` and `DataTableRequest.SortDirection` have been removed. Sort state is now expressed as `SortDescriptors` (`IReadOnlyList<SortDescriptor>`), matching the multi-column sort model used internally.

**Migration — update your `ServerData` callback:**

```csharp
// Before
ServerData="@(async req => {
    var col = req.SortColumn;
    var dir = req.SortDirection;
    ...
})"

// After
ServerData="@(async req => {
    var sort = req.SortDescriptors.FirstOrDefault();
    var col  = sort?.Column;
    var dir  = sort?.Direction;
    ...
})"
```

---

### ✨ Enhancement — `DataTable<TData>` server-side virtualisation (infinite scrolling)

A new `ItemsProvider` parameter wires `DataTable` directly to Blazor's `<Virtualize>` component for demand-driven, server-side infinite scroll. Set `ItemsProvider` instead of `ServerData` and the table fetches rows on demand as the user scrolls — no explicit pagination required.

The delegate receives a `DataTableVirtualRequest` with full sort, search, and cancellation context:

```csharp
ItemsProvider="@(async req => {
    var page = await _api.GetPageAsync(
        req.StartIndex, req.Count,
        req.SortDescriptors, req.SearchText,
        req.CancellationToken);
    return new(page.Data, page.TotalCount);
})"
```

**New parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ItemsProvider` | `DataTableVirtualProvider<TData>?` | `null` | Activates server-side virtualised mode. Set instead of `ServerData`. |
| `ItemHeight` | `float` | `40` | Approximate row height in pixels (`ItemSize` hint passed to the virtualizer). Must match your actual row height. |
| `Height` | `string` | `"400px"` | CSS height of the scrollable table container. Required when either virtualisation mode is active. |
| `VirtualizeOverscanCount` | `int` | `3` | Extra rows rendered beyond the visible viewport to reduce blank-row flicker during fast scrolling. |

**New public types:**

| Type | Description |
|---|---|
| `DataTableVirtualProvider<TData>` | Delegate type for the `ItemsProvider` parameter. |
| `DataTableVirtualRequest` | Request context: `StartIndex`, `Count`, `SortDescriptors`, `SearchText`, `CancellationToken`. |
| `SortDescriptor` | Single-column sort descriptor: `Column` and `Direction`. |

---

### ✨ Enhancement — `DataTable<TData>` client-side DOM windowing

`Virtualize="true"` enables Blazor `<Virtualize>` for large in-memory datasets. All rows stay in memory; only the visible nodes are rendered, reducing DOM size and scroll jank for collections of ≈500–20 000 rows where full pagination is undesirable. Requires `ItemHeight` and `Height` to be set so the virtualizer can compute scroll geometry.

---

### ✨ Enhancement — `DataView<TItem>` adaptive auto-fill grid columns

A new `GridColumnMinWidth` parameter enables CSS `repeat(auto-fill, minmax(…, 1fr))` column layout. The browser computes the column count automatically based on the available container width — no manual breakpoint math required. Accepts any CSS length (`"160px"`, `"10rem"`) or a bare Tailwind spacing key (`"40"`). Overrides `GridColumns` when set.

---

### 📖 Demo — `DataTable<TData>` virtualisation sections

Two new sections added to the `/components/datatable` demo page:

| Section | What it shows |
|---|---|
| **Infinite Scroll (Server-Side)** | `ItemsProvider` delegate; rows fetched on demand via `<Virtualize>`; receives `StartIndex`, `Count`, `SortDescriptors`, and `SearchText` — no page/page-size math needed. Sorting and global search trigger an automatic virtualizer refresh. |
| **Virtualize (Client-Side)** | `Virtualize=true` with a large in-memory dataset. All data stays in memory; only visible DOM rows are rendered. Search and sort still run client-side. Demonstrates the `ItemHeight` and `Height` requirements. |

---

### 📖 Demo — `DataView<TItem>` adaptive grid section

One new section added to the `/components/data-view` demo page:

| Section | What it shows |
|---|---|
| **Infinite GridView Columns** | `GridColumnMinWidth="160px"` — the browser computes the column count automatically from container width via `repeat(auto-fill, minmax(…, 1fr))`. Resize the viewport to see the column count adapt live with no JavaScript or breakpoint logic required. |

---

### 🐛 Bug Fixes

#### `DataTable` — empty-state template shown in virtualised modes

The empty-state template appeared incorrectly when either virtualisation mode was active because `_processedData` is intentionally unpopulated in those paths. The guard now bypasses the empty check entirely when virtualisation is enabled.

#### `DataTable` — invalid HTML from `<Virtualize>` spacer elements

Blazor's `<Virtualize>` was emitting `<div>` spacer elements inside `<tbody>`, producing invalid HTML. The spacer element is now explicitly set to `<tr>` via `SpacerElement="tr"`.

#### `DataTable` — `colspan="0"` on placeholder rows when all columns hidden

`VisibleColumnCount` was not guarded against zero, producing `colspan="0"` on loading and empty-state rows when all columns were toggled off. The count is now clamped to a minimum of `1`.

#### `DataTable` — selection state broken in client-side `Virtualize` mode

`IsAllSelected` and `IsSomeSelected` read from `_processedData`, which is bypassed in client `Virtualize` mode. `_processedData` and `_filteredData` are now kept in sync with `_virtualizeItems` so selection state correctly reflects the full dataset.

#### `DataView` — focus-ring flicker on the listbox container

`focus:outline-none` suppressed the browser outline only while the container itself had focus, leaving a default black ring visible in some focus states. Replaced with unconditional `outline-none`.

#### `DataView` — duplicate `@oninput` handler on search input

A duplicate `@oninput` event handler was attached to the search `<input>`, causing the search callback to fire twice per keystroke. The redundant handler has been removed.

---

### Changed

- `DataTableRequest.SortDescriptors` replaces `SortColumn` / `SortDirection` — see Breaking Change above.
- `DataView` item focus indicator now uses `outline-ring` consistently for both focused and unfocused states, improving visual consistency and accessibility.
- `DataTable` demo — inactive status badge changed from `BadgeVariant.Destructive` to `BadgeVariant.Secondary`.
- `DataTable` demo — Server-Side Data section reordered to appear before Global Search & Column Visibility for a more logical progression.
- `NeoUI.Blazor.Primitives` dependency bumped from `3.4.0` → `3.6.0`.
- `grid-auto-fill-*` CSS utility migrated from the Tailwind v3 `matchUtilities` plugin to a native Tailwind v4 `@utility` block in both the component library and demo CSS inputs.

---

## 2026-3-14 — Ten new components, four new chart types, DataTable server-side data, and chart reliability fixes

> **Release: `v3.6.0`**  
> Affects `NeoUI.Blazor`. Component count rises from **85+** to **100+**. All changes are additive — no breaking changes to existing APIs.

---

### ✨ New Components (6)

#### `Timeline` — `/components/timeline`

A fully-composable chronological event display. Supports three statuses (`Pending`, `Active`, `Completed`), custom icon content, dashed/solid/none connector styles, collapsible items, and alternating (`Left`/`Right`/`Alternate`) alignment.

**Sub-components:** `Timeline`, `TimelineItem`, `TimelineHeader`, `TimelineTitle`, `TimelineDescription`, `TimelineTime`, `TimelineIcon`, `TimelineConnector`, `TimelineContent`, `TimelineEmpty`

**Enums:** `TimelineAlign`, `TimelineColor`, `TimelineSize`, `TimelineStatus`, `TimelineIconVariant`, `TimelineConnectorStyle`, `TimelineConnectorFit`

**Demo sections:**

| Section | What it shows |
|---|---|
| **Basic** | Shorthand `Title`/`Time`/`Description` params — no child slots. Status drives circle colour automatically. |
| **With Custom Icons** | `IconContent` slot with arbitrary Blazor content; `ChildContent` for the item body. |
| **Alignment** | `Left` and `Right` icon placement; `ShowConnector=false` on the last item. |
| **Alternating Layout** | `TimelineAlign.Alternate` distributes even items left, odd items right. |
| **Icon Variants** | `Solid` (filled circle) vs `Outline` (hollow ring) for event-type differentiation. |
| **Sizes** | `Small`, `Default`, and `Large` controlling icon diameter and item gap. |
| **Connector Styles** | `Solid`, `Dashed`, and `Dotted` connectors for optional or uncertain steps. |
| **Connector Spacing** | Per-item `ConnectorClass` to control height for variable-height card content. |
| **Collapsible Items** | `IsCollapsible` wraps the item in a `Collapsible`; `DetailContent` is the expandable body. |
| **Connected Fit** | `ConnectorFit.Connected` runs the line flush against the icon for compact process flows. |
| **Large Collapsible Content** | Rich expandable sections — ideal for changelogs and release notes. |
| **Empty State** | `TimelineEmpty` with optional custom slot content. |

---

#### `TreeView` — `/components/tree-view`

Hierarchical data display with expand/collapse, single and multi-select, tri-state checkbox propagation, and optional drag-and-drop node reordering. Driven by a generic `TItem` with lambda-based child/value/text/icon resolution — no schema definition required.

**Sub-components:** `TreeView<TItem>`, `TreeItem<TItem>`, `TreeItemNode<TItem>`

**Enums:** `TreeSelectionMode`, `CheckStateKind`

**Demo sections:**

| Section | What it shows |
|---|---|
| **Basic** | Nested file tree; click to select, selected path reported below. |
| **Checkboxes** | `CheckedValues` binding; check, empty, and indeterminate states per node. |
| **Feature Installer** | `PropagateChecks=true` — checking a group selects all children; partial groups show indeterminate. |
| **Multi-Select** | `SelectionMode.Multiple` with Ctrl/Cmd click. |
| **Flat Data with ParentField** | Build the hierarchy from a flat list via `ValueField` + `ParentField`. |
| **Expand All & Show Lines** | `DefaultExpandAll` and `ShowLines` connecting guides. |
| **Search / Filter** | `SearchText` binding; parent nodes kept visible when a descendant matches, matched text highlighted. |
| **Loading & Error States** | `LoadingNodes` / `ErrorNodes` for per-node async feedback; `OnRetryLoad` callback. |
| **Load Children Async** | `LoadChildrenAsync` delegate — tree manages loading state, errors, and caching automatically. |

---

#### `DataView` — `/components/data-view`

Switchable list/grid layout container for data collections. Provides declarative column definitions that automatically wire the search input, sort dropdown, and keyboard navigation. Includes built-in pagination, single/multi/none selection modes, infinite scroll, client-side virtualization, and a flexible `ItemTemplate` slot for completely custom item rendering.

**Sub-components:** `DataView<TItem>`, `DataViewColumn<TItem>`, `DataViewListTemplate`, `DataViewGridTemplate`

**Enums:** `DataViewLayout`, `DataViewSelectionMode`, `DataViewCheckVariant`

**Demo sections:**

| Section | What it shows |
|---|---|
| **Basic** | Searchable user list; `DataViewColumn` fields auto-wire search input and sort dropdown. |
| **List / Grid Switcher** | Both `ListTemplate` and `GridTemplate` provided — layout toggle appears automatically in the toolbar. |
| **Single Selection** | Click to select/deselect; ↑↓ to navigate, Space/Enter to toggle. |
| **Multiple Selection** | Toggle selection; count badge and Clear button wired via `ToolbarActions` slot. |
| **Check Variant** | `CircleCheck` (always reserves space) vs `Check` (hidden when unselected) vs `None`. |
| **Grouping** | `GroupBy` to bucket items by field; `GroupHeaderTemplate` for custom group headers. |
| **Infinite Scroll (Server-Side)** | `OnLoadMore` callback; new pages appended as the user scrolls. |
| **Virtualize (Client-Side)** | `Virtualize=true` for large in-memory collections with minimal DOM nodes. |
| **Search & Sort** | Multiple `DataViewColumn` with `Filterable` and `Sortable` flags. |
| **Toolbar with Custom Actions** | `ToolbarActions` slot (Export, Add) rendered alongside search/sort and layout toggle. |
| **Pagination** | `PageSize` and `PageSizes` for built-in paging footer. |
| **Loading State** | Skeleton shown while data is being fetched. |
| **Empty State** | Custom content when item list is empty. |

---

#### `DynamicForm` — `/components/dynamic-form`

Schema-driven form renderer. Pass a `FormSchema` (list of `FormFieldDefinition` records) and the component renders the appropriate NeoUI input for each field type — `Text`, `Email`, `Password`, `Number`, `Date`, `DateRange`, `Select`, `MultiSelect`, `Checkbox`, `Switch`, `Slider`, `SliderRange`, `Textarea`, `Rating`, `ColorPicker`, `TimePicker`, `FileUpload`, `InputOtp`, `MaskedInput`, `CurrencyInput`, `RichText`, and `MarkdownEditor`. Handles validation rules and emits `@bind-Values` and `OnFieldChanged`.

**Components:** `DynamicForm`, `FormSection`, `DynamicFieldRenderer`

**Types:** `FormSchema`, `FormFieldChangedEventArgs`, `DateRangeValue`, `SliderRangeValue`

**Enums:** `FieldType` (24 variants), `FormLayout`, `ValidationType`

**Demo sections:**

| Section | What it shows |
|---|---|
| **Basic Contact Form** | Flat schema in a single column — name, email, phone, message. |
| **Two-Column Layout** | `FormLayout.TwoColumn` for wider forms. |
| **Sections** | `FormSection` grouping with collapsible support. |
| **Field Types Showcase** | All 24 `FieldType` values rendered in a single live form. |
| **Validation** | Required, min/max length, email, and custom validator with a summary panel. |
| **Conditional Visibility** | `VisibleWhen` expressions — fields appear or hide based on other field values. |

---

#### `TagInput` — `/components/tag-input`

An input field that manages a list of string tags/chips. Configurable add-triggers (`Enter`, `Comma`, `Tab`, `Space`, `Blur`, or any combination), async suggestion callbacks, duplicate prevention, max-tag limit, and `Backspace` removal. Supports `Outlined` and `Ghost` visual variants.

**Types:** `TagInput`, `TagInputTrigger` (flags enum), `TagInputVariant`

**Demo sections:**

| Section | What it shows |
|---|---|
| **Basic** | Enter or comma to add; Backspace removes last tag. |
| **With Suggestions** | `OnSearchSuggestions` async callback filters a list as the user types. |
| **Static Suggestions** | `Suggestions` parameter with a fixed list. |
| **Max Tags & Duplicates** | `MaxTags` limit and `AllowDuplicates` toggle. |
| **Trigger Keys** | `AddTrigger` flags enum — configure exactly which keys submit a tag. |
| **Variants** | `Default`, `Outlined`, and `Ghost` tag styles. |
| **Clearable** | `Clearable=true` shows a clear-all button when tags are present. |
| **Disabled** | All interaction blocked. |

---

#### `SplitButton` — `/components/split-button`

Pairs a prominent primary-action button with a chevron-triggered dropdown for secondary actions. Supports the same `Variant` and `Size` tokens as `Button`, a separator between primary and trigger, and composable `SplitButtonItem` children inside the dropdown.

**Sub-components:** `SplitButton`, `SplitButtonItem`, `SplitButtonSeparator`

**Demo sections:**

| Section | What it shows |
|---|---|
| **Basic** | Primary action on left, dropdown arrow on right. |
| **Variants** | All `ButtonVariant` tokens applied to a split button. |
| **Sizes** | `Small`, `Default`, and `Large`. |
| **With Icon** | `Icon` slot prepending a Lucide icon to the primary label. |
| **Disabled** | Both segments disabled when `Disabled=true`. |

---

### ✨ New Chart Types (4) + New Chart Demo (1)

All four new chart types follow the same composable, Recharts-inspired declarative API as the existing chart components. Additionally, a new demo page was added for `RadialBarChart`, which already existed as a component.

#### `CandlestickChart` + `Candlestick` series — `/components/chart/candlestick`

OHLC / candlestick chart for financial and time-series price data. The `Candlestick` series accepts `OpenKey`, `HighKey`, `LowKey`, `CloseKey`, and `DateKey` data-key parameters, plus separate `BullishColor`/`BearishColor` styling.

| Demo Section | What it shows |
|---|---|
| **Basic** | Simple OHLC chart with rising/falling colours. |

#### `FunnelChart` + `Funnel` series — `/components/chart/funnel`

Pipeline and conversion funnel (or pyramid in ascending sort). The `Funnel` series exposes `DataKey`, `NameKey`, `Sort` (`Ascending`/`Descending`/`None`), `Align` (`Left`/`Center`/`Right`), `Gap`, `MinSize`, `MaxSize`, `Top`, and `Bottom` layout parameters.

| Demo Section | What it shows |
|---|---|
| **Sales Pipeline** | Standard top-down funnel with descending values and a legend. |
| **Conversion Rates** | Percentage-based funnel with a custom tooltip formatter. |
| **Pyramid (Ascending)** | Inverted funnel / pyramid using `FunnelSort.Ascending`. |

#### `GaugeChart` + `Gauge` series — `/components/chart/gauge`

Circular gauge and speedometer charts, configurable as a classic semi-circle KPI gauge or a full-circle progress ring. When multiple `Gauge` series are declared in one `GaugeChart`, the component automatically tiles them horizontally — center positions and a scaled radius (`min(150/N, 75)%`) are computed per-series so adjacent arcs never overlap.

Key parameters: `StartAngle`, `EndAngle`, `SplitNumber`, `ShowPointer`, `ShowSplitLine`, `ShowAxisLabel`, `ShowProgress`, `ProgressWidth`, `AxisLineWidth`, `DetailFontSize`, `DetailOffsetY`, `ShowTitle`, `Fill`.

| Demo Section | What it shows |
|---|---|
| **Single KPI Gauge** | Classic semi-circle gauge for a single metric value. |
| **Multiple Gauges** | Three gauges (CPU/Memory/Disk) auto-tiled side-by-side in one chart. |
| **Progress Ring** | Full-circle gauge (`StartAngle=90`, `EndAngle=-270`) styled as a clean progress ring with `ShowPointer=false` and `SplitNumber=0`. |

#### `HeatmapChart` + `Heatmap` series + `VisualMap` primitive — `/components/chart/heatmap`

Intensity grid charts for activity calendars and correlation matrices. Uses `XKey`/`YKey`/`ValueKey` data-key parameters on the `Heatmap` series. The `VisualMap` primitive controls color-stop ranges, show/hide legend, orientation, and min/max scaling.

| Demo Section | What it shows |
|---|---|
| **Activity Calendar** | Hour-by-day activity heatmap, similar to a GitHub contribution graph. |
| **Correlation Matrix** | Pearson correlation between numeric features with a diverging color scale. |

#### `RadialBarChart` — new demo page `/components/chart/radial-bar` _(component pre-existing)_

A demo page was added for the pre-existing `RadialBarChart` component.

| Demo Section | What it shows |
|---|---|
| **Basic** | Single series across categories in a circular layout. |
| **Multi-Series** | Several series plotted side-by-side for direct comparison. |
| **Stacked** | Stacked series showing composition and total at a glance. |

---

### ✨ Enhancement — `DataTable` Server-Side Data

Added a lightweight server-side data callback to `DataTable<TData>`. Wire up `ServerData` to any async delegate and the table handles paging, sorting, and search automatically:

```csharp
ServerData="@(async req => new DataTableResult<Order> { Items = pagedOrders, TotalCount = total })"
```

**New types:**

| Type | Description |
|---|---|
| `DataTableRequest` | Page, PageSize, SortColumn, SortDirection, SearchText |
| `DataTableResult<TData>` | Items (current page), TotalCount (all pages) |

---

### 🐛 Bug Fixes

#### `ChartContainer` — width class override
`ContainerStyle` previously emitted `width:99%` as an inline style, which silently overrode any CSS class providing an explicit width (e.g. `Class="w-[200px]"` on progress rings). The inline width has been removed; width is now controlled entirely by the CSS classes, which restores correct sizing for all fixed-width gauge containers.

#### `GaugeChart` — progress arc ignored `Fill` color
`EChartsGaugeProgress.color` is not a valid ECharts property and was silently ignored, causing the progress arc to use ECharts' default palette color instead of the component's `Fill`. A series-level `itemStyle.color` is now set from `resolvedColor` so the arc respects the `Fill` parameter.

#### `FunnelChart` — legend overlap with funnel body
The funnel series `Top` was hardcoded to `"10"`, meaning the funnel body started immediately below the chart top edge and overlapped any top-positioned `Legend`. Fixed by:
1. Adding explicit `Top` and `Bottom` parameters to `Funnel.razor` (default `null`).
2. Using `funnel.Top ?? "10"` in `FunnelChart.BuildSeries` — no more source-level legend-detection heuristics.
3. Updating the Sales Pipeline demo to pass `Top="60"` only on the section that has a legend.

#### `GaugeChart` — multiple gauges rendered stacked at center
When two or more `Gauge` series were declared in a single `GaugeChart`, all series defaulted to `center: ['50%','50%']` and `radius: '75%'`, stacking every gauge on top of the others. `BuildSeries` now computes per-series `Center` (`[(i+0.5)/N*100%, 55%]`) and a scaled `Radius` (`min(⌊150/N⌋, 75)%`) whenever `total > 1`.

---

## 2026-3-12 – DataGrid — New Blazor-native ServerSide Row Model

> **Library change.** Affects `DataGrid<TItem>` in `NeoUI.Blazor`. Introduces `IDataGridServerDataProvider<TItem>` and `HttpDataGridProvider<TItem>` in `NeoUI.Blazor`, and updates the DataGrid JavaScript in `NeoUI.Blazor`. `DataGridDensity.Comfortable` has been renamed to `DataGridDensity.Medium`; `Comfortable` is retained as an `[Obsolete]` alias for backward compatibility.

---

### ✨ New Feature — `DataGridRowModelType.BlazorServerSide`

Introduces a Blazor-native server-side row model for the `DataGrid<TItem>` component that delivers full server-side paging, sorting, and filtering **without requiring an AG Grid Enterprise license**.

`BlazorServerSide` uses AG Grid's free `clientSide` row model under the hood, but all data loading is orchestrated from C# via a **single JS→C# interop call** that returns data directly. No Enterprise `ServerSideRowModel` module required (~100KB+ bundle savings).

---

### New APIs

#### `DataGridRowModelType.BlazorServerSide` _(enum value)_

A new row model type that enables license-free, single-round-trip server-side data handling.

#### `DataGrid<TItem>` — new parameters

| Parameter | Type | Description |
|---|---|---|
| `ServerDataProvider` | `IDataGridServerDataProvider<TItem>?` | Plug in any external data source. Takes priority over `OnServerDataRequest`. |
| `TotalServerRowCount` | `int` | Two-way bindable total row count, automatically updated after each fetch. |
| `TotalServerRowCountChanged` | `EventCallback<int>` | Callback for `@bind-TotalServerRowCount` support. |
| `FillWidth` | `bool` | Makes the grid fill all available horizontal space responsively. |
| `Flex` | `int?` | Sets AG Grid flex sizing on columns. |
| `AutoSizeColumns` | `bool` | Automatically sizes columns to fit content on load. |
| `IdField` | `string` | **Required for `BlazorServerSide`** — used to match rows for cross-page selection re-sync. |

#### `DataGrid<TItem>` — new virtual method

```csharp
protected virtual Task<DataGridDataResponse<TItem>> OnFetchServerDataAsync(DataGridDataRequest<TItem> request)
```

Override in a subclass to provide data without using `OnServerDataRequest` or `ServerDataProvider` (code-behind style).

#### `DataGrid<TItem>` — new API method

```csharp
public Task AutoSizeColumnsAsync()
```

Programmatically trigger column auto-sizing after load.

#### `IDataGridServerDataProvider<TItem>` _(new interface)_

An abstraction for wiring any async data source (REST API, GraphQL, in-memory, etc.) to the grid.

```csharp
Task<DataGridDataResponse<TItem>> GetDataAsync(DataGridDataRequest<TItem> request, CancellationToken ct = default);
```

#### `HttpDataGridProvider<TItem>` _(new abstract base class)_

A ready-to-subclass HTTP provider that POSTs a `DataGridDataRequest` as JSON and deserialises the response. Override the following hooks for customisation:

- `ConfigureRequestAsync` — inject auth headers, tenant IDs, query parameters, etc.
- `MapResponseAsync` — handle non-standard response envelopes.

---

### Data fetch priority

When `RowModelType` is `BlazorServerSide`, the grid resolves the data source in this order:

1. `ServerDataProvider` (if set)
2. `OnServerDataRequest` (if set)
3. `OnFetchServerDataAsync` (virtual method override)

---

### ✨ New Feature — Cross-Page Row Selection Persistence

Row selections now survive page navigation in `BlazorServerSide` mode. Select rows on any page, navigate away and back — selections are preserved.

**How it works:**

- `IdField` is the key. The grid uses it to match incoming rows against the set of previously selected IDs.
- When navigating to a new page, the currently selected IDs are tracked in Blazor and rows matching those IDs on the new page are automatically re-checked.
- Applying a sort or filter resets to page 1 but **preserves existing selections** for any rows that remain visible after filtering.
- `@bind-SelectedItems` always reflects the full cross-page selection set in Blazor, not just the current page.
- The JS renderer uses `idField` for consistent row ID mapping and prevents selection loss on page transitions.
- Initialization guards prevent concurrent setup races.

---

### ✨ Enhancement — Column Sizing, Density & Loading Polish

Several quality-of-life improvements, all additive with no breaking changes:

| Change | Detail |
|---|---|
| `FillWidth` parameter | Responsive full-width column stretching |
| `Flex` parameter | Per-column AG Grid flex sizing |
| `AutoSizeColumns` parameter | Auto-fit columns to content on initialisation |
| `AutoSizeColumnsAsync()` API | Programmatic auto-size trigger |
| Compact density | Row height updated to 36px for visual consistency |
| `Comfortable` → `Medium` | Density preset renamed for clarity |
| Loading/init spinners | Improved visual states during grid setup and data fetching |
| JS: `autoSizeAndFill` | New JS implementation backing the above sizing features |
| Grid ready notification | JS now notifies Blazor when the grid is fully initialised |

---

### 📖 Demo Page — `/components/datagrid/server-side`

A new dedicated demo page accessible at `/components/datagrid/server-side`, with **five live interactive sections**:

| Section | What it demonstrates |
|---|---|
| **A — Blazor Method Call** (`OnServerDataRequest`) | Simplest pattern; async `Func` with in-memory sort, filter, page. Shows a live request counter and "last request" debug line. |
| **B — Custom `ServerDataProvider`** | `IDataGridServerDataProvider<TItem>` / `InMemoryProductProvider` implementation. Includes a callout explaining how to subclass `HttpDataGridProvider<TItem>`. |
| **C — Simulated Latency** | 800 ms `Task.Delay` to demonstrate the loading overlay behaviour. |
| **D — Row Selection Across Pages** | Multi-select with `@bind-SelectedItems`. Shows selected order badges (first 10 + overflow count), Clear button, and an explanation callout for how cross-page selection works. |
| **E — FilterBuilder Integration** | `FilterBuilder` toolbar replaces all column-level filters — no column has `Filterable="true"`. `OnFilterChange` stores the `FilterGroup` and calls `RefreshAsync()`; the handler applies it via the built-in `ApplyFilter()` LINQ extension. Includes two presets (High-Value Pending, Delivered) and a callout explaining the pattern. |

The DataGrid sub-page is registered as `datagrid/server-side` with the description _"Server-side paging, sorting, and filtering — no Enterprise license required, single JS↔C# round trip"_, appearing correctly in the sidebar nav and command palette.

---

### 🐛 Fix — `DataGrid` initializing-state race condition (empty rows on multi-grid pages)

When multiple `DataGrid<TItem>` instances initialized simultaneously on the same page, a parent `StateHasChanged()` triggered by one grid's `TotalServerRowCountChanged` could re-render sibling grids while they had `_isInitializing = true`. The render condition `!_initialized && (_isInitializing || !_columnsRegistered)` would swap in the spinner, **removing the `<div @ref="_gridContainer">` from the DOM** mid-initialization. AG Grid then initialized into a detached element (invisible), and a fresh empty `div` was placed in the DOM after initialization completed — permanently empty rows despite correct pagination counts.

**Fix:** Removed `_isInitializing` from the render condition. The spinner now only renders until columns register (one render cycle). Once `_columnsRegistered = true`, the grid container `div` is permanently present in the DOM regardless of initialization state. `_isInitializing` is retained as a C# re-entrancy guard in `OnAfterRenderAsync`. AG Grid's own loading overlay covers the fetch window.

---

## 2026-3-11 — DataTable: Appearance API, Density, Bug Fixes & Docs

> **Enhancement.** Introduces a full appearance-customisation API to `DataTable<TData>` (`Dense`, `HeaderBackground`, `HeaderBorder`, `CellBorder`, `ColumnsVisibility`, per-part CSS class overrides), fixes toolbar UX issues, de-bolds pagination labels, expands the API reference, and adds a `README.md` for the component. No breaking changes.

---

### ✨ Feat: `DataTable<TData>` — appearance and layout parameters

Seven new `[Parameter]` properties for fine-grained visual control:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Dense` | `bool` | `true` | Compact cell padding (`h-9 / py-2 px-4`). `false` uses spacious defaults (`h-12 / p-4`). |
| `HeaderBackground` | `bool` | `true` | Applies `bg-muted/50` to the header row. |
| `HeaderBorder` | `bool` | `false` | Vertical `divide-x divide-border` dividers between header cells. |
| `CellBorder` | `bool` | `false` | Vertical `divide-x divide-border` dividers between body cells. |
| `ColumnsVisibility` | `bool` | `true` | Show/hide the "Columns" toggle button in the toolbar. |
| `HeaderClass` | `string?` | `null` | Extra CSS classes on `<thead>`. |
| `HeaderRowClass` | `string?` | `null` | Extra CSS classes on the header `<tr>`. |
| `BodyRowClass` | `string?` | `null` | Extra CSS classes on each body `<tr>`. |

**Internal computed helpers added to `DataTable.razor.cs`:**

- `HeaderCellPaddingClass` — `h-9 px-4` (Dense) or `h-12 px-4`
- `BodyCellPaddingClass` — `py-2 px-4` (Dense) or `p-4`
- `ComputedHeaderRowClass` — merges `border-b`, optional `bg-muted/50`, optional `divide-x divide-border`, and `HeaderRowClass`
- `GetBodyRowClass(bool isSelected)` — merges selection state, optional `divide-x divide-border`, and `BodyRowClass`

All new parameters are tracked in `ShouldRender` to prevent stale renders when style props change at runtime.

---

### 🐛 Fix: `DataTableToolbar` — column visibility label click unresponsive

Clicking the column label text in the column visibility popover had no effect; only clicking directly on the checkbox worked.

**Root cause:** The `FieldLabel For=` → `Checkbox Id=` association relied on native `<label for>` / `<input id>` linkage, which the `Checkbox` component does not surface on its underlying input.

**Fix:** Row `<div>` now owns the `@onclick` handler (`OnColumnVisibilityChanged?.Invoke(column.Id, !column.Visible)`). `Checkbox` receives `Class="pointer-events-none"` so it displays state only. `FieldLabel` replaced with a plain `<span>`. The `checkboxId` local variable removed.

---

### 🐛 Fix: `DataTableToolbar` — excessive gap between toolbar and table

`py-4` on the toolbar div stacked with `space-y-4` on the container, producing a ~32 px visual gap. Changed `py-4` → `pt-4` (removes bottom padding); `space-y-4` now provides the sole 16 px separation.

---

### 🐛 Fix: `DataTableToolbar` — Columns button icon/text spacing too wide

Reduced `me-2` → `me-1.5` on the clipboard SVG inside the Columns button for tighter icon-to-label spacing at `ButtonSize.Small`.

---

### 🐛 Fix: `PaginationPageDisplay` / `PaginationPageSizeSelector` — bold pagination labels

"Page X of Y" (`PaginationPageDisplay`) and "Rows per page" (`PaginationPageSizeSelector`) were rendered in `font-medium`. Changed both to `text-muted-foreground` (no bold) to match the already-muted "Showing X–Y of Z" style from `PaginationInfo`.

---

### 📄 Docs: `DataTable` — new `README.md` and expanded API reference

- Created `src/NeoUI.Blazor/Components/DataTable/README.md` — covers Quick Start, Appearance Customisation, Row Selection, Custom Cell Templates, Toolbar Actions, Loading/Empty states, Server-Side mode, and full API tables for `DataTable<TData>` and `DataTableColumn<TData, TValue>`.
- `DataTableDemo.cs` `_dataTableProps`: expanded from 10 → 25 rows (all new style params + previously undocumented `Columns`, `ToolbarActions`, `EmptyTemplate`, `LoadingTemplate`, `AriaLabel`, `OnSort`, `OnFilter`, `PreprocessData`).
- `DataTableDemo.cs` `_dataTableColumnProps`: expanded from 6 → 13 rows (added `Id`, `Filterable`, `Width`, `MinWidth`, `MaxWidth`, `CellClass`, `HeaderClass`).

---

### 🎨 Demo: `DataTableDemo` — interactive style controls in Basic Table section

Replaced the static Basic Table `DemoBlock` with an interactive variant featuring five `Switch` toggles (Dense layout, Header background, Header borders, Cell borders, Columns button) so users can explore all appearance options live without code changes.

---

## FilterBuilder: New Component

> **New component.** Adds `FilterBuilder<TData>` and all supporting sub-components, models, enumerations, extension methods, and 7 demo pages to `NeoUI.Blazor` and `NeoUI.Demo.Shared`. No breaking changes to existing APIs.

---

### 🔑 Key Features

- **Generic, type-safe API** — `FilterBuilder<TData where TData : class>` binds directly to your model type; two-way `@bind-Filters` binding via `FilterGroup` with an `OnFilterChange` callback
- **Inline canvas UI** — no modal, no popover, no Apply/Cancel step; conditions render as chip rows directly in the page and changes apply instantly
- **Declarative child-content API** — fields and presets are declared as child components (`<FilterField>`, `<FilterPreset>`) inside named `RenderFragment` slots (`FilterFields`, `FilterPresets`); no code-behind configuration required
- **Segmented filter chips** — each active condition renders as `[icon Label] | [operator ▾] | [value input] | [×]`; operator uses the NeoUI `Select` component (auto-width, borderless)
- **`FilterEditorType`** — decouples the value widget from the field's data type; a `Number` field can render a `Currency`, `Masked`, or custom editor without changing the field definition
- **`FilterPresetsVariant`** — `Dropdown` (default, DropdownMenu button) or `Tabs` (Stripe-style horizontal tab bar with an implicit "All" tab; selecting a tab replaces active conditions with the preset's conditions)
- **12 editor types** — `Auto`, `Input`, `Numeric`, `Currency`, `Masked`, `Date`, `DateRange`, `Boolean`, `Select`, `MultiSelect`, `Combobox`, `Custom` (`RenderFragment<FilterCustomContext>`)
- **8 field types** — `Text`, `Number`, `Date`, `DateRange`, `Boolean`, `Select`, `MultiSelect`, `Custom`
- **19 filter operators** — full coverage across text, numeric/date, collection, and boolean domains
- **`FilterGroup` nesting** — conditions and nested groups combined with `And` / `Or` logical operators; composable at any depth
- **LINQ extensions** — `ApplyFilters<T>()` on `IEnumerable<T>` and `IQueryable<T>`; all string comparisons use `OrdinalIgnoreCase`
- **7 demo pages** — basic, all-types, presets (Tabs), custom editor (Rating), nested groups, persistence (localStorage), and a feature overview with Quick Start

---

### ✨ Feat: `FilterBuilder<TData>` — inline canvas filter UI with declarative child-content API

Adds a fully standalone, generic filter builder component (`@typeparam TData where TData : class`) implemented as a `.razor` + `.razor.cs` partial pair. The UI is an **inline block canvas** — no Popover, no Apply/Cancel step; all changes apply instantly.

The canvas renders a `[≡ Filter]` field-picker button, one chip row per active condition, and a `[✕ Clear]` button pinned to the top-right (visible only when at least one condition is active). The root component accepts `Filters` / `FiltersChanged` / `OnFilterChange` for two-way binding, named `RenderFragment` parameters `FilterFields` and `FilterPresets` for declarative child configuration, a `ButtonText` parameter, and a `PresetsVariant` parameter.

Each active condition renders as a segmented pill:

```
[icon Label] | [operator Select ▾] | [value input] | [×]
```

The operator selector uses the NeoUI `Select` component (auto-width, no border, background-only hover/active states). The value input widget switches on `EditorType` and supports all editor variants (see `FilterEditorType` below). Input controls retain their border; top/bottom clipping is avoided by removing `overflow-hidden` from the chip container. All controls are `w-auto` — no forced full-width.

**Sub-components added:**

| File | Role |
|------|------|
| `IFilterBuilderContext.cs` | Cascading context interface; exposes `RegisterField` / `RegisterPreset` to child components |
| `FilterChip.razor` | Interactive segmented chip for a single active condition |
| `FilterConditionRow.razor` | Standalone row: field label + operator selector + value input + remove button |
| `FilterValue.razor` / `.cs` | Typed value input; switches on all 8 `FilterFieldType` values; `Compact` mode for in-chip use |
| `FilterField.razor` | Registers a field definition via `IFilterBuilderContext`; supports `Icon` and `EditorType` |
| `FilterPreset.razor` | Registers a preset definition via `IFilterBuilderContext` |

---

### ✨ Feat: Core models and enumerations (`NeoUI.Blazor`)

**New enumerations:**

- **`FilterOperator`** — `Equals`, `NotEquals`, `Contains`, `NotContains`, `StartsWith`, `EndsWith`, `IsEmpty`, `IsNotEmpty`, `GreaterThan`, `LessThan`, `GreaterThanOrEqual`, `LessThanOrEqual`, `Between`, `NotBetween`, `IsAnyOf`, `IsNoneOf`, `IsAllOf`, `IsTrue`, `IsFalse`
- **`FilterFieldType`** — `Text`, `Number`, `Date`, `DateRange`, `Boolean`, `Select`, `MultiSelect`, `Custom`
- **`LogicalOperator`** — `And`, `Or`
- **`FilterEditorType`** — `Auto`, `Input`, `Numeric`, `Currency`, `Masked`, `Date`, `DateRange`, `Boolean`, `Select`, `MultiSelect`, `Custom`. Decouples the value widget from the field's data type (e.g. a `Number` field can render a `Currency` editor).
- **`FilterPresetsVariant`** — `Dropdown` (default; renders a Presets button backed by `DropdownMenu`) or `Tabs` (renders a Stripe-style horizontal tab bar with an implicit "All" tab; selecting a tab replaces the active conditions with that preset's conditions).

**New model classes:**

- `FilterCondition` — `Id`, `Field`, `Operator`, `Value`, `SecondaryValue`
- `FilterGroup` — `Id`, `Logic`, `Conditions`, `NestedGroups`
- `SelectOption` — `record(Value, Label)`
- `FilterFieldDefinition` — internal field descriptor extracted from `FilterField` children
- `FilterPresetDefinition` — internal preset descriptor extracted from `FilterPreset` children

---

### ✨ Feat: `FilterExtensions` — LINQ helpers for applying a `FilterGroup`

Adds `ApplyFilters<T>()` extension methods on both `IEnumerable<T>` and `IQueryable<T>`. All string comparisons (`Contains`, `NotContains`, `StartsWith`, `EndsWith`) use `StringComparison.OrdinalIgnoreCase`.

---

### ✨ Feat: Demo pages — 7 pages under `/components/filter`

| Route | Content |
|-------|---------|
| `/components/filter` | Feature overview, keyboard-accessible example navigation cards, Quick Start snippet |
| `/components/filter/basic` | Product catalog with cards; `FilterEditorType.Currency` for Price |
| `/components/filter/all-types` | Employee directory + `DataTable` (toolbar off); all 8 field types + `EditorType` reference table |
| `/components/filter/presets` | Order management + `DataTable` (toolbar off) + 3 presets rendered as `PresetsVariant="Tabs"` |
| `/components/filter/custom` | Star-rating custom control using the NeoUI `Rating` component |
| `/components/filter/nested` | Code-only reference for nested `FilterGroup` + LINQ |
| `/components/filter/persistence` | `localStorage` persistence pattern snippet |

The component registry entry uses slug `filter` with 6 sub-page entries marked `IsSubPage: true`.

---

## 2026-3-10 – Select, MultiSelect & DataTable: Component-Level Refinements

> **Library changes.** Affects `SelectTrigger`, `SelectItem`, `MultiSelect`, and `DataTable` in `NeoUI.Blazor`. No breaking changes to public APIs.

---

### ✨ Feature: `SelectTrigger` — new `Borderless` parameter

Added a `Borderless` boolean parameter to `SelectTrigger`. When `true`, the component's border, box-shadow, and all focus/open-state ring styles are suppressed — leaving only background-hover feedback as the only visual cue. Intended for in-context placements where the host element (e.g. a `FilterChip`) already owns the visible border, and a redundant inner ring would look disconnected.

---

### 🐛 Fix: `SelectItem` — check-mark icon overlapping item label text

The selected-state check icon is absolutely positioned at the right edge (`inset-y-0 right-0 pr-3`). The item label `<span>` previously carried no matching right-padding reservation, so the icon could overlay the trailing characters of long labels. The label span now uses `pr-10` to always clear the icon's footprint regardless of label length.

---

### ✨ Feature: `MultiSelect` — tooltip on the `+N more` overflow indicator

The `+N more` badge shown when selected items exceed `MaxDisplayTags` now renders a `Tooltip` on hover that lists the full display names of the hidden items. The implementation uses the existing `TooltipProvider / Tooltip / TooltipTrigger / TooltipContent` component stack for visual and behavioural consistency. `TooltipTrigger`'s wrapper renders with `class="contents"` (`display: contents`), keeping it layout-transparent inside the flex tag row. A `GetDisplayText(string value)` lookup helper was added to `MultiSelect.razor.cs` to resolve stored value strings back to human-readable labels for the tooltip body.

---

## 2026-3-9 — Sidebar & Trigger: Three Bug Fixes

> **Library change.** Affects `Sidebar`, `SidebarMenuButton`, `Button`, and `LinkButton` in `NeoUI.Blazor`, and `DropdownMenuContentPrimitive` in `NeoUI.Blazor.Primitives`. No breaking changes to public APIs.

---

### 🐛 Fix: `SidebarMenuButton` / `Button` / `LinkButton` — stale `ElementReference` when used as `AsChild` trigger

`Button`, `LinkButton`, and `SidebarMenuButton` only called `TriggerContext.SetTriggerElement` on `firstRender`. When the render tree structure changed on a subsequent render — in particular, `SidebarMenuButton` adds or removes a `<TooltipPrimitive>` wrapper depending on `ShouldShowTooltip()` (which toggles with the sidebar collapsed/expanded state) — Blazor recreated the underlying `<button>` element with a new `ElementReference` ID. The stale ID was never re-registered, so `_asChildTriggerRef` in the trigger primitive (and therefore `Context.State.TriggerElement` / `FloatingPortal.AnchorElement`) pointed to a DOM element that no longer existed. C# saw `HasValue = true` (non-empty ID string) but JS `__internalId` lookup returned `null`, causing the floating content to mis-position or fail to appear entirely.

Fixed by removing the `firstRender &&` guard in `OnAfterRender` for all three components. `SetTriggerElement` already has an ID-equality guard in the context layer, so calling it on every render is a no-op when the element is stable and only fires a state update when the ID genuinely changes. This matches the pattern already used by `TooltipTriggerPrimitive`.

---

### 🐛 Fix: `Sidebar` — `SidebarRail` causes horizontal scrollbar

The `<aside>` element had `overflow-y-auto` and `h-screen` in its layout classes. `overflow-y-auto` creates a block formatting context; the `SidebarRail` button is `position: absolute` at `right: -1rem` (straddling the sidebar edge), so it overflowed the BFC horizontally, producing a horizontal scrollbar on the `<aside>`. The `h-screen` also forced 100vh height even inside constrained containers such as the 580px dashboard shell.

`SidebarContent` already carries `overflow-auto flex-1 min-h-0` — the correct place for sidebar scroll containment. Removed `overflow-y-auto` and `h-screen` from the default variant layout classes (replaced with `min-h-full`), and removed `overflow-y-auto` from the Floating and Inset variants as well. Aligns with upstream's layout class pattern.

---

### ✨ Feat: `DropdownMenuContentPrimitive` — `ForceMount` default changed to `true`

Changed the default value of `ForceMount` from `false` to `true` in `DropdownMenuContentPrimitive`, matching `SelectContentPrimitive`. With `ForceMount=false` (previous default), the portal was mounted fresh on every open: `MountPortalAsync` added ~32ms of overhead (PortalHost render cycle + `Task.Yield`), during which the CSS entrance animation had already started on the freshly-inserted element. By the time the element became visible, the animation was ~35–45% complete, producing a visible mid-animation pop-in. With `ForceMount=true`, the portal is pre-mounted at startup and `SetupAsync` runs immediately on open (same render cycle as the `data-state` transition), reducing the animation lead-time to ~20ms and eliminating the choppy first-show.

---

## 2026-3-6 — Performance: CascadingValue IsFixed alignment with upstream

> **Library change.** Affects 9 component and primitive files across `NeoUI.Blazor` and `NeoUI.Blazor.Primitives`. No breaking changes to public APIs.

---

### ♻️ Perf: Fix `IsFixed="false"` on stable `this` cascades in content and sub-content primitives

Audited all `CascadingValue` usages across the library and identified 9 instances where `Value="this"` (or `Value="@this"`) was incorrectly marked `IsFixed="false"`. Component instance references (`this`) are structurally stable for the lifetime of the component and are not replaced on re-render. Marking them `false` caused Blazor to maintain unnecessary subscriber lists and perform cascade propagation checks on every render cycle for no benefit.

All 9 occurrences have been corrected to `IsFixed="true"`, aligning with upstream's established convention. The remaining 35 context-object cascades (`@_context`, `Context`, `@SubContext`, etc.) are intentionally left as `IsFixed="false"` to match upstream's conservative approach for mutable state objects.

**Affected files:**

- `Combobox.razor`
- `Motion.razor`
- `ResizablePanelGroup.razor`
- `ContextMenuContentPrimitive.razor`
- `ContextMenuSubContentPrimitive.razor`
- `DropdownMenuContentPrimitive.razor`
- `DropdownMenuSubContentPrimitive.razor`
- `MenubarContentPrimitive.razor`
- `MenubarSubContentPrimitive.razor`

---

## 2026-3-4 – Sidebar & Positioning: Bug Fixes

> **Library change.** Affects `SidebarMenuButton`

---

### ✨ Feature: `SidebarMenuButton` now works as an `AsChild` target for trigger components

`SidebarMenuButton` previously ignored `TriggerContext`, which meant using it as `<DropdownMenuTrigger AsChild>` (or any other trigger primitive with `AsChild`) silently failed — the dropdown never opened, no anchor element was registered for positioning, and aria attributes were absent.

`SidebarMenuButton` now fully participates in the `AsChild` / `TriggerContext` pattern, matching the behaviour already present in `Button`:

- **`[CascadingParameter(Name = "TriggerContext")]`** — receives context cascaded by `DropdownMenuTriggerPrimitive`, `PopoverTriggerPrimitive`, etc.
- **`_buttonRef` + `OnAfterRender` → `SetTriggerElement`** — registers the rendered element with the positioning service so the floating overlay can anchor correctly.
- **`HandleClick`** — invokes `TriggerContext.Toggle()` when a trigger context is present; falls through to `CollapsibleContext` otherwise, preserving existing sub-menu expand/collapse behaviour.
- **`HandleKeyDown` → `TriggerContext.OnKeyDown`** — delegates arrow-key / Enter handling to the trigger context, enabling keyboard navigation into the opened overlay.
- **`id`, `aria-haspopup`, `aria-expanded`, `aria-controls`** — wired from `TriggerContext` on the `<button>` element for full accessibility compliance.
- **`data-state`** — now evaluates `TriggerContext?.IsOpen` first (falling back to `CollapsibleContext`), so `data-[state=open]:*` Tailwind selectors fire correctly while the overlay is open — enabling the standard UX pattern of the trigger holding its active/hover background while its overlay is visible.

---

### 🐛 Fix: `SidebarMenuButton` — `data-active` emitted as C# boolean instead of `"true"/"false"` string

The `<button>` render path was emitting `data-active="@effectiveActive"`, which Blazor serialises as `True` / `False` (capitalised). CSS attribute selectors such as `data-[active=true]:bg-sidebar-accent` never matched, so the active state was visually absent on non-anchor buttons. Fixed to `@(effectiveActive ? "true" : "false")` to match the lowercase string expected by both the component's own class logic and consumer-side selectors.

---

### 🐛 Fix: `SidebarMenuButton` — `data-size` attribute missing from rendered elements

`data-size` was declared in the component API and used by `SidebarMenuBadge`'s peer selectors (`peer-data-[size=sm]/menu-button:top-1` etc.) but was never actually emitted on either the `<button>` or `<NavLink>` elements. Peer selectors targeting size never fired. Both render paths now emit `data-size="@Size.ToValue()"`.

---

### 🐛 Fix: `SidebarMenuButton` — size variant classes keyed on wrong strings

The `GetClasses()` size switch matched on `"small"` and `"large"`, but `SidebarMenuButtonSize.ToValue()` returns `"sm"` and `"lg"`. The `sm` and `lg` size variants therefore never applied. Switch cases corrected to `"sm"` and `"lg"`.

---

### 🐛 Fix: `positioning.js` — overflow detection used wrong coordinate space on scrolled pages

The `preventOverflow` middleware was computing `exceedsBottom / exceedsTop / exceedsRight / exceedsLeft` against the pre-existing `adjustedX / adjustedY` variables (document-space coordinates) rather than the viewport-relative `vpX / vpY` values that had been derived immediately above. On any page with a non-zero scroll offset the overflow checks were incorrect, causing floating elements to be mis-positioned or needlessly constrained. All four boundary checks and the subsequent correction assignments are now consistently performed in viewport coordinates, with a final conversion back to document space (`vpX + scrollX`, `vpY + scrollY`) before returning.

---

### 🐛 Fix: `positioning.js` — wrong CSS `position` on floating element caused coordinate-space mismatch

Floating UI's `getOffsetParent()` inspects the floating element's *current* `CSS position` to resolve its containing block. The element is initialised off-screen as `position: fixed` by `GetInitialStyle()` in Blazor. When the requested strategy is `absolute`, `getOffsetParent()` was reading `fixed` and returning `null` / `window` instead of the true offset parent, so coordinates were computed in the wrong space. The call to `lib.computePosition()` is now wrapped with a temporary `position` override that sets the element's style to match the requested strategy, then restores the original value in a `finally` block. Because JS is single-threaded and no layout/paint occurs between the set and restore within a single synchronous execution sequence, there are no visual side-effects.

---

### ♻️ Refactor: `SidebarDemo` — remove redundant outer wrapper from demo blocks

Multiple `DemoBlock` sections in `SidebarDemo.razor` contained an extra `<div class="border rounded-lg p-6 bg-background">` wrapping the inner demo container. The inner container already carries its own border, rounded corners, and background, making the outer wrapper redundant and adding unwanted padding. Removed from all affected sections.

---

## 2026-3-3 – NavigationMenu: Hover Sensitivity & Trigger Interaction Fixes

> **Primitive change.** Affects `NavigationMenuTriggerPrimitive` and `NavigationMenuContext` in `NeoUI.Blazor.Primitives`, and `NavigationMenuTrigger` in `NeoUI.Blazor`. No breaking changes to public APIs.

---

### 🐛 Feature: Open delay on hover to prevent accidental menu activation

`NavigationMenuContext` previously opened the menu immediately on `mouseenter`. A cursor simply passing across the nav bar was enough to pop open a panel. A new `ScheduleOpen(string value)` method (mirroring the existing `ScheduleClose`) now applies a configurable delay before activating an item:

- If **no menu is currently open**, activation is deferred by `OpenDelay` ms (default `200`).
- If a menu **is already open** (user deliberately moving between triggers), the switch happens **immediately** to keep navigation responsive.
- `CancelOpenTimer()` is called on `mouseleave` so a quick cursor pass-through never opens anything.

A new `OpenDelay` property on `NavigationMenuContext` (default `200`) is wired to the existing `DelayDuration` parameter on `NavigationMenuRoot` — this parameter was previously defined but never applied.

---

### 🐛 Fix: Active trigger no longer closes its own menu on click

`HandleClick` in `NavigationMenuTriggerPrimitive` previously toggled the menu closed when clicking an already-open trigger, making it easy to accidentally dismiss the panel. The handler is now a no-op when `IsActive` is true — clicks only open, never close.

---

## 2026-3-1 – Chart System: Opt-In Primitives Model (#140)

> **Library change.** Affects `NeoUI.Blazor` chart components and all chart demo pages. No breaking changes to the public API — all previous charts continue to render; the defaults are now explicit rather than implicit.

---

### ✨ Feature: Opt-In Chart Primitives

Chart primitive components (`<Grid />`, `<XAxis />`, `<YAxis />`, `<ChartTooltip />`, `<Legend />`) are now **opt-in** rather than always-present. Nothing is rendered into the ECharts option unless the corresponding primitive is added as a child of the chart root. This gives consumers full, declarative control over which chart elements appear and how they are styled.

Previously, chart roots (e.g. `BarChart`) unconditionally built and emitted `grid`, `xAxis`, `yAxis`, `tooltip`, and `legend` objects using internal defaults. Now:

- `Grid = BuildGrid()` is always emitted (so ECharts uses the chart's `Padding` / `ContainLabel` settings rather than ECharts' large 60 px defaults), but **split-line visibility is still gated on `<Grid />`** via `BuildSplitLine`.
- `Legend`, `Tooltip` — only emitted when `<Legend />` / `<ChartTooltip />` is present.
- `XAxis`, `YAxis` — always emitted for best compatibility (required by ECharts for cartesian layouts), but when the primitive is absent the **visual sub-components only** are hidden (`axisLine.show: false`, `axisTick.show: false`, `axisLabel.show: false`). The axis is kept alive as a coordinate reference so that `splitLine` (grid lines from `<Grid />`) can still render independently.

---

### ♻️ Refactor: Demo chart pages updated to opt-in model

All chart demo pages (`BarChartExamples`, `LineChartExamples`, `AreaChartExamples`, `PieChartExamples`, `RadarChartExamples`, `ScatterChartExamples`) have been updated to explicitly declare the primitives they need. Shared demo defaults extracted into `ChartDefaults.cs`.

---

## 2026-2-28 – Demo Solution Polish: Blazor UX Improvements & Project Cleanup

> **Demo-only change.** No library component APIs were modified. All changes are contained within the `demo/` projects.

---

### 🏗️ Infrastructure: Switch Tailwind CSS Build to npm (`NeoUI.Demo.Shared` & `NeoUI.Blazor`)

Replaced the standalone `tailwindcss.exe` binary approach with an npm-based workflow in both the demo shared project and the components library. This fixes a production 404 for `app.css` caused by the gitignored output file not being present at `dotnet build` time in CI, and is faster and more portable than the downloaded binary.

**Root cause of the `app.css` 404:**  
`**/wwwroot/css/app.css` was gitignored, so a fresh CI checkout had no file on disk. The Blazor static web asset manifest needed the file to exist before `dotnet build` started; the previous `BeforeTargets="BeforeBuild"` MSBuild target generated it too late in the pipeline.

**Fix — generate CSS before `dotnet build`:**

- Added `package.json` to both `demo/NeoUI.Demo.Shared/` and `src/NeoUI.Blazor/` declaring `@tailwindcss/cli` as a dev dependency with `build:css` and `watch:css` npm scripts.
- Added `NpmInstall` MSBuild target (`BeforeTargets="BeforeBuild"`) that auto-runs `npm install` when `node_modules/` is absent, so developers never need to run it manually.
- Updated `BuildTailwindCSS` / `BuildNeoUICSS` MSBuild targets to call `npm run build:css` via `DependsOnTargets="NpmInstall"` instead of invoking `tailwindcss.exe` directly.
- Removed the `OS==Windows_NT` restriction from the components target — npm works cross-platform.
- Updated all three CI/CD deployment workflows (`main`, `dev/staging`, `dev/preview`) to replace the "Download Tailwind CLI" PowerShell step with `actions/setup-node@v4` → `npm install` → `npm run build:css` (run from `demo/NeoUI.Demo.Shared/`) **before** `dotnet build`, ensuring `app.css` is present when MSBuild evaluates the project.
- The NuGet publish workflow is unchanged — `components.css` remains pre-committed to the repo and CI skips the build target via `'$(CI)' != 'true'`.

---

### 🏗️ Infrastructure: Demo Project Restructure for v3 Release

The demo solution has been reorganised into dedicated, purpose-named host projects so each render mode can be started directly without configuration switching:

| Project | Render Mode | HTTPS Port |
|---|---|---|
| `NeoUI.Demo.Auto` | Interactive Auto (Server + WASM) | 7172 |
| `NeoUI.Demo.Auto.Client` | WASM satellite for Auto | — |
| `NeoUI.Demo.Server` | Interactive Server only | 7173 |
| `NeoUI.Demo.Wasm` | Standalone WebAssembly | 7174 |

- `NeoUI.Demo` and `NeoUI.Demo.Client` have been renamed to `NeoUI.Demo.Auto` and `NeoUI.Demo.Auto.Client` respectively.
- `NeoUI.Demo.Server` and `NeoUI.Demo.Wasm` are new projects created from scratch.
- All shared `wwwroot` assets (`css/`, `images/`, `styles/`, `favicon.png`) have been moved from the old Client project into `NeoUI.Demo.Shared/wwwroot` with `StaticWebAssetBasePath="."` so every host project picks them up at root path automatically.
- The Tailwind CSS build target has been consolidated into `NeoUI.Demo.Shared.csproj`.

---

### 🏗️ Infrastructure: `NeoUI.Demo.Auto.Client` Workaround Removed

With `NeoUI.Demo.Wasm` now a dedicated standalone project, the temporary workaround in `NeoUI.Demo.Auto.Client` is no longer needed:

- Removed the `RemoveIndexHtmlFromStaticAssets` MSBuild target that excluded `wwwroot/index.html` from static assets when the client project was used as a satellite WASM project
- Removed the `ResolveStaticWebAssetsInputsDependsOn` override
- Deleted `NeoUI.Demo.Auto.Client/wwwroot/index.html` — the dedicated `NeoUI.Demo.Wasm` project owns its own `index.html`

---

### 🏗️ Infrastructure: Tailwind Config Restored to Correct Location

The `tailwind.config.js` for the demo was restored to its correct location at `demo/NeoUI.Demo.Shared/tailwind.config.js` (project root), matching the `@config "../../tailwind.config.js"` reference in `wwwroot/css/app-input.css`:

- Content paths corrected: `../../src/NeoUI.Blazor/**/*`, `../../src/NeoUI.Blazor.Primitives/**/*`, and all icon libraries resolve correctly from the Shared project root
- Removed a stale `../NeoUI.Demo.Shared/**/*` entry that pointed to a non-existent path

---

### ✨ Feature: Polished `blazor-error-ui` Across All Host Projects

The default plain-text Blazor error UI strip has been replaced with a polished themed bottom-bar notification in all three host projects:

- **`NeoUI.Demo.Auto/App.razor`** — themed bottom bar in `<body>`
- **`NeoUI.Demo.Server/App.razor`** — same treatment
- **`NeoUI.Demo.Wasm/wwwroot/index.html`** — same treatment (inline styles, no Tailwind dependency so it renders even if Blazor itself crashes)

Design: fixed bottom bar with a destructive icon badge, error message, a styled Reload link, and a dismiss button — all using CSS variables (`--card`, `--border`, `--destructive`, `--foreground`, etc.) from the loaded theme.

---

### ✨ Feature: Reconnect Modal for Auto & Server

A polished `ReconnectModal` component (using the native `<dialog>` element) has been added to `NeoUI.Demo.Auto` and `NeoUI.Demo.Server`. It replaces the browser default reconnect UI with a fully themed modal:

| State | UI |
|---|---|
| Reconnecting (first attempt) | Pulsing animation + "Reconnecting…" message |
| Retrying | Live countdown via `components-seconds-to-next-attempt` |
| Failed | Destructive icon + Retry Connection button |
| Paused | Pause icon + Resume Session button |
| Resume Failed | Destructive icon + prompt to reload |

- `ReconnectModal.razor` — pure Tailwind markup, scoped `<style>` block for state visibility management and `@keyframes` animations, driven entirely by CSS classes Blazor sets on `#components-reconnect-modal`
- `ReconnectModal.razor.js` — ES module handling `components-reconnect-state-changed` events, `Blazor.reconnect()` / `Blazor.resumeCircuit()` retry logic, and visibility-change auto-retry

> Note: the reconnect modal is Server/Auto-specific (SignalR circuit reconnection). `NeoUI.Demo.Wasm` intentionally does not include it.

---

### ✨ Feature: `Error.razor` Page for `NeoUI.Demo.Wasm`

A polished `/Error` page has been added to `NeoUI.Demo.Wasm`, adapted from the business template:

- Uses NeoUI components: `Card`, `Alert` (destructive variant), `Button`
- Shows Request ID block (sourced from `Activity.Current?.Id`) when available
- "Go Home" and "Reload Page" action buttons
- **Adapted for WASM**: removed `[CascadingParameter] HttpContext?` (not available in WebAssembly) and removed the server-specific "Development Mode / ASPNETCORE_ENVIRONMENT" guidance card

---

## 2026-2-27 – Demo Site Revamp: Data-Driven Registry, `Demo*` Infrastructure & Full Page Conversion

> **Demo-only change.** No library component APIs were modified. All changes are contained within `demo/NeoUI.Demo.Shared`.

---

### ✨ Feature: Component Registry

A single, centralized `ComponentRegistry` now drives all demo-site navigation surfaces — eliminating scattered hardcoded lists.

- Registry entries carry: slug, title, description, icon name, category/tags, and tier (`Component` vs `Primitive`)
- Stable ordering used for **prev/next** navigation across all ~80 demo pages
- Helper methods: find entry by route/slug, get prev/next entry, enumerate grouped items
- All three navigation surfaces now read from the registry:
  - **Left sidebar** menu
  - **`/components` index page** (card grid)
  - **Spotlight Command Palette** (global search)

---

### ✨ Feature: `Demo*` Infrastructure Components

A suite of reusable, consistently-named demo-scaffolding components added to `demo/NeoUI.Demo.Shared`:

| Component | Purpose |
|---|---|
| `DemoPageHeader` | Page title + description banner |
| `DemoSection` | Titled section with optional description |
| `DemoBlock` | Live preview / code tab switcher |
| `DemoCopyButton` | One-click copy for code snippets |
| `DemoPropsTable` | API reference table (`DemoPropRow` rows) |
| `DemoPageNav` | Zero-config prev/next navigation (derives context from registry) |

Breadcrumb navigation is also wired into the top bar in `MainLayout`, showing the active page context and prev/next buttons when browsing any demo page.

---

### ✨ Feature: All Demo Pages Converted to `Demo*` Pattern

All **~80 component and primitive demo pages** have been converted from ad-hoc `<section>` / `<h2>` markup to the standardised `Demo*` structure, following the `AlertDemo.razor` reference implementation:

- `DemoPageHeader` replaces the old inline title/description
- Each example group is wrapped in `DemoSection` + `DemoBlock` (live preview + syntax-highlighted code tab)
- API reference table (`DemoPropsTable`) added to every page
- All related code snippets and demo-viewer related codes are placed in CodeExamples subfolder, providing clean examples in the razor file.
- `DemoPageNav` appended to every page (prev/next, zero-config)
- Cosmetic label improvements applied where appropriate (e.g. `"DatePicker Component"` → `"Date Picker"`, `"Basic Examples"` → `"Basic"`)

---

## 2026-2-24 – RC Namespace Flattening & Brand Rename: `BlazorUI.*` → `NeoUI.*`

> **Breaking change.** Every package ID, namespace, filename, CSS class, JS token, localStorage key, and CI/CD reference has been updated to the new `NeoUI.*` scheme. See migration notes below.

---

### 📦 Package IDs renamed

| Old | New |
|---|---|
| `NeoBlazorUI.Components` | `NeoUI.Blazor` |
| `NeoBlazorUI.Primitives` | `NeoUI.Blazor.Primitives` |
| `NeoBlazorUI.Icons.Lucide` | `NeoUI.Icons.Lucide` |
| `NeoBlazorUI.Icons.Heroicons` | `NeoUI.Icons.Heroicons` |
| `NeoBlazorUI.Icons.Feather` | `NeoUI.Icons.Feather` |

---

### 📁 Filesystem renames (`git mv`)

| Old path | New path |
|---|---|
| `BlazorUI.sln` | `NeoUI.Blazor.sln` |
| `src/BlazorUI.Components/` | `src/NeoUI.Blazor/` |
| `src/BlazorUI.Primitives/` | `src/NeoUI.Blazor.Primitives/` |
| `src/BlazorUI.Icons.Lucide/` | `src/NeoUI.Icons.Lucide/` |
| `src/BlazorUI.Icons.Heroicons/` | `src/NeoUI.Icons.Heroicons/` |
| `src/BlazorUI.Icons.Feather/` | `src/NeoUI.Icons.Feather/` |
| `demo/BlazorUI.Demo/` | `demo/NeoUI.Demo/` |
| `demo/BlazorUI.Demo.Shared/` | `demo/NeoUI.Demo.Shared/` |
| `demo/BlazorUI.Demo.Client/` | `demo/NeoUI.Demo.Client/` |
| `wwwroot/blazorui.css` | `wwwroot/components.css` |
| `wwwroot/css/blazorui-input.css` | `wwwroot/css/components-input.css` |

---

### 🗂️ Namespace flattening

All per-component sub-namespaces have been collapsed to a single top-level import. Consumers no longer need per-component `@using` lines.

**Before:**
```razor
@using NeoUI.Blazor.Button
@using NeoUI.Blazor.Dialog
@using NeoUI.Blazor.Sidebar
@using NeoUI.Blazor.Collapsible
```

**After:**
```razor
@using NeoUI.Blazor
@using NeoUI.Blazor.Primitives   @* if using primitive components directly *@
```

| Layer | Namespace | Notes |
|---|---|---|
| Styled components | `NeoUI.Blazor` | All component, enum, and service types |
| Primitive components | `NeoUI.Blazor.Primitives` | See Primitive suffix convention below |
| Primitive services | `NeoUI.Blazor.Primitives.Services` | `DropdownManagerService`, `KeyboardShortcutService`, etc. |
| Chart components & types | `NeoUI.Blazor.Charts` | Scoped separately to avoid IntelliSense pollution for non-chart users; matches Syncfusion/Infragistics convention |

`Utilities` and `Contexts` sub-namespaces have been merged into their parent (`NeoUI.Blazor` and `NeoUI.Blazor.Primitives` respectively). `Services` is retained.

---

### 🏷️ Primitive component naming convention

To disambiguate styled components from same-named primitives when both are used in the same file, **all primitive `.razor` components that have a styled counterpart now carry a `Primitive` suffix**:

```razor
@* Styled component — short name *@
<Accordion>...</Accordion>

@* Primitive component — Primitive suffix *@
<AccordionPrimitive>...</AccordionPrimitive>
```

**Infrastructure primitives** with no styled counterpart use no suffix (same as `FloatingPortal`):

| No-suffix primitives |
|---|
| `FloatingPortal` |
| `Table`, `TableBody`, `TableHeader`, `TableHeaderCell`, `TableCell`, `TableRow`, `TablePagination` |
| `DialogPortal`, `DialogOverlay` |
| `SheetPortal`, `SheetOverlay` |
| `NavigationMenuRoot` |

Support types (context records, enums, services) keep their original names with no suffix in all cases.

---

### 🔀 Component renames to eliminate ambiguity

| Old name | New name | Reason |
|---|---|---|
| `Grid<TItem>` | `DataGrid<TItem>` | Clash with `NeoUI.Blazor.Charts.Grid` (chart grid lines) |
| `GridColumn` | `DataGridColumn` | Same — all `Grid*` types renamed to `DataGrid*` |
| `GridDefinition` | `DataGridDefinition` | |
| `Chart.Tooltip` | `ChartTooltip` | Clash with styled `Tooltip` component |
| `Command` | `CommandContent` | Razor RZ1042 void-element clash for `<command>` |

---

### 🌐 Runtime token renames

| Before | After |
|---|---|
| `blazorui:visible` / `blazorui:hidden` (JS events) | `neoui:visible` / `neoui:hidden` |
| `blazorui-portal-root` (DOM id) | `neoui-portal-root` |
| `.blazorui-portal` (CSS class) | `.neoui-portal` |
| `blazorui:collapsible:` (localStorage prefix) | `neoui:collapsible:` |
| `_content/NeoBlazorUI.Components/` | `_content/NeoUI.Blazor/` |
| `_content/NeoBlazorUI.Primitives/` | `_content/NeoUI.Blazor.Primitives/` |
| `blazorui.css` | `components.css` |
| `blazorui-input.css` | `components-input.css` |
| `BuildBlazorUICSS` (MSBuild target) | `BuildNeoUICSS` |

---

### ⚙️ CI/CD & tooling

- `nuget-publish.yml` — `case` block updated to new `csproj` paths and `NeoUI.*` package IDs.
- Three Azure deploy workflows (`BlazorUIDemo20251223130817.yml`, `*-preview.yml`, `*-staging.yml`) — `AZURE_WEBAPP_PACKAGE_PATH` and `WORKING_DIRECTORY` updated to `demo\NeoUI.Demo`.
- `scripts/release-components.sh` — all variable names, grep/sed patterns, NuGet API URLs, display strings, and git commit messages updated.
- `demo/NeoUI.Demo.Client/tailwind.config.js` — content globs updated to scan `src/NeoUI.Blazor/**`, `src/NeoUI.Blazor.Primitives/**`, and each icon project folder.
- Icon codegen scripts (`GenerateIconData.ps1`, `generate-icon-data.js`) — generated namespace output lines updated.

---

### 📖 Migration guide

1. **Update NuGet references** — swap old package IDs in your `.csproj` files (see Package IDs table above).
2. **Update `@using` directives** — replace all per-component `@using BlazorUI.Components.*` with `@using NeoUI.Blazor`. Add `@using NeoUI.Blazor.Primitives` if using primitive components directly.
3. **Update static asset references** — change `_content/NeoBlazorUI.Components/blazorui.css` to `_content/NeoUI.Blazor/components.css` in `App.razor` / `index.html`.
4. **Update `Program.cs`** — rename `AddBlazorUIComponents()` / `AddNeoBlazorUIPrimitives()` → `AddNeoUIComponents()` / `AddNeoUIPrimitives()` (check your `ServiceCollectionExtensions`).
5. **Rename component tags** — `<Grid>` → `<DataGrid>`, `<GridColumn>` → `<DataGridColumn>`, `<Command>` → `<CommandContent>`, chart `<Tooltip>` → `<ChartTooltip>`.
6. **localStorage** — `blazorui:collapsible:*` keys are now `neoui:collapsible:*`. Existing persisted state will be ignored on first load (treated as missing); no data loss beyond preference reset.

---

## 2026-2-23 - UI/X Improvements on All Menu Components and New ThemeSwitcher demo page

### 🐛 Menu Hover Sensitivity — Grace Period & Debounce

**Affected:** `MenubarItem`, `MenubarSubTrigger`, `DropdownMenuItem`, `DropdownMenuSubTrigger`, `ContextMenuItem`, `ContextMenuSubTrigger`

Hovering quickly over menu items or briefly leaving a sub-trigger while navigating toward its open
submenu panel caused the submenu to close immediately — matching no platform UX convention
(Windows and macOS both use a ~200–300 ms grace period before closing).

#### Changes

**Regular items** (`MenubarItem`, `DropdownMenuItem`, `ContextMenuItem`)

- Added `CancellationTokenSource _hoverCts` per item.
- `HandleMouseEnter` now waits **250 ms** before calling `CloseActiveSubMenu()`. If the mouse
  leaves the item within that window, `HandleMouseLeave` cancels the token and the close is
  abandoned — the open submenu keeps showing.
- Added `@onmouseleave` / `HandleMouseLeave` (was missing entirely on `DropdownMenuItem` and
  `ContextMenuItem`).
- `_hoverCts` is disposed in `Dispose()`.

**Sub-triggers** (`MenubarSubTrigger`, `DropdownMenuSubTrigger`, `ContextMenuSubTrigger`)

- Added `CancellationTokenSource _hoverCts` per component.
- Added `isAlreadyActive` guard at the top of `HandleMouseEnter`: if this submenu is already open
  and registered as the active submenu, the handler returns immediately — prevents the
  close-then-300 ms-reopen flicker that occurred when the mouse briefly left and re-entered the
  same trigger.
- Sibling submenu is now **closed immediately** (before the delay) when a new sub-trigger is
  entered, eliminating the visual conflict where two triggers appeared selected simultaneously and
  preventing the open sibling's panel from overlapping the new trigger and firing spurious
  `mouseleave` events.
- The **300 ms open delay** is preserved to prevent flash-opens on quick pass-through. If
  `HandleMouseLeave` fires before the delay, the token is cancelled and neither the close nor the
  open occurs — the original submenu remains visible.
- `_hoverCts` cancelled and disposed in `Dispose()`.

---

### 🐛 Bug Fix: Menu Open Auto-Focus — Container vs. First Item

**Affected:** `MenubarContent`, `MenubarSubContent`, `ContextMenuContent`, `ContextMenuSubContent`, `DropdownMenuSubContent`, `menu-keyboard.js`

All menu content containers passed `initialFocus: "first"` to `menu-keyboard.js`, which caused the
first enabled item to receive focus the moment the menu opened. This is non-standard for menus
(Windows/macOS highlight nothing until the user presses a key or moves the mouse).

#### Changes

- **`menu-keyboard.js`** — added new `initialFocus = "container"` branch: calls
  `focusWithDoubleRaf(container)` to focus the `[tabindex="-1"]` container div (enabling keyboard
  event reception) without moving focus to any item. The existing `"first"` / `"last"` paths are
  untouched so `DropdownMenuContent` behaviour is unchanged.
- `MenubarContent` — changed `initialFocus` from `"first"` to `"container"`.
- `MenubarSubContent` — changed `initialFocus` from `"first"` to `"container"`.
- `ContextMenuContent` — changed `initialFocus` from `"first"` to `"container"`.
- `ContextMenuSubContent` — changed `initialFocus` from `"first"` to `"container"`.
- `DropdownMenuSubContent` — changed `initialFocus` from `"first"` to `"container"`.

`ArrowDown` with nothing focused still navigates to the first item (`currentIndex === -1 → 0`),
preserving full keyboard accessibility.

---

### ✨ Feature: `TooltipContent` — `Strategy` Parameter

**Affected:** `NeoUI.Blazor.Primitives.Tooltip.TooltipContent`, `NeoUI.Blazor.Tooltip.TooltipContent`

The `FloatingPortal` inside `TooltipContent` previously had `Strategy` hardcoded to
`PositioningStrategy.Absolute`. Tooltips inside transformed or `overflow: hidden` containers
(e.g. sidebars) were clipped or mis-positioned.

- Added `[Parameter] PositioningStrategy Strategy` (default `Absolute`) to the **primitive**
  `TooltipContent` and wired it to `FloatingPortal`.
- Added the same parameter to the **component** `TooltipContent` wrapper, passed through to the
  primitive.

---

### ✨ Feature: `ThemeSwitcher` — New Parameters

**Affected:** `ThemeSwitcher`

| Parameter | Type | Default | Description |
|---|---|---|---|
| `TriggerClass` | `string?` | — | Extra CSS classes merged onto the trigger `Button` |
| `PopoverContentClass` | `string?` | — | Extra CSS classes merged onto the `PopoverContent` panel |
| `Align` | `PopoverAlign` | `End` | Alignment of the popover panel relative to the trigger |

- `Strategy` is now also forwarded to `TooltipContent` so the tooltip escapes stacking contexts
  when `Strategy = Fixed`.

---

### 📖 Demo: ThemeSwitcher Page

New demo page at `/components/theme-switcher` covering:

- Live `ThemeSwitcher` demo with reactive current-theme readout (base color + primary color)
- Standalone `DarkModeToggle` demo
- Usage snippets (minimal, layout placement)
- Customisation examples (`TriggerClass`, `PopoverContentClass`)
- `Strategy` / fixed-positioning guidance
- `ThemeSwitcher` parameter API table
- `ThemeService` member reference table with injectable usage example
- **App.razor Setup** section (core stylesheet, base color CSS, primary color CSS, theme JS +
  `initialize()`, `Program.cs` service registration)
- `Alert Variant="Warning"` production tip to trim unused theme CSS files

Added to sidebar navigation, component index grid, and Spotlight search palette.
Added "What's New" callout card to the home page.

---

### 🐛 Bug Fix: Chart Series Always 0 in Layout-Embedded Charts

**Affected:** All chart series and primitives — `Pie`, `Bar`, `Line`, `Area`, `Scatter`, `Radar`,
`RadialBar`, `Tooltip`, `Legend`, `Grid`, `XAxis`, `YAxis`, `PolarGrid`, `RadarGrid`,
`CenterLabel`, `AxisLabel`, `LabelList`, `LabelLine`, `Fill`

Charts in complex scenarios (e.g. `ChartTile`) always rendered with zero series while the
same chart on a standalone page worked correctly.

#### Root Cause

All series and chart primitives used `[CascadingParameter] private dynamic? Parent` to receive
their parent chart. Without a type or name constraint, Blazor matched the parameter to the
**nearest** ancestor `CascadingValue` — which in the layout context was `SidebarContext` from
`SidebarProvider`, not the chart root. Dynamic dispatch then called `RegisterPie` (etc.) on the
wrong type, threw a `RuntimeBinderException`, and the original empty `catch { }` silently
discarded it. `_pies` was never populated → Series count = 0.

On standalone pages there were no competing cascading values above the chart, so the correct
parent was matched by accident.

A secondary cause was that all chart-level `Register*` methods were `internal`. The C# DLR
`RuntimeBinder` does not resolve `internal` members through `dynamic` dispatch even within the
same assembly, compounding the failure.

#### Fix — Interface-Based Cascading (no more `dynamic`)

Introduced `ChartInterfaces.cs` with a typed interface hierarchy:

```
IChartParent              ← Tooltip, Legend
  IXAxisParent            ← XAxis; also RadarChart, RadialBarChart
    ICartesianChartParent ← Grid, YAxis
      IBarSeriesParent    ← Bar  (BarChart + ComposedChart)
      ILineSeriesParent   ← Line (LineChart + ComposedChart)
      IAreaSeriesParent   ← Area (AreaChart + ComposedChart)
      IScatterSeriesParent← Scatter (ScatterChart + ComposedChart)
  IPieChartParent         ← Pie
  IRadarChartParent       ← Radar, RadarGrid
  IRadialBarChartParent   ← RadialBar, PolarGrid, CenterLabel

IAxisParent               ← AxisLabel  (XAxis, YAxis implement this)
IFillParent               ← Fill       (Line, Area implement this)
SeriesBase (type)         ← LabelList  (all series inherit this)
Pie (type)                ← LabelLine  (Pie-specific)
```

- All 8 chart root components declare `@implements I*Parent` and use an unnamed
  `CascadingValue Value="this" IsFixed="true"`. Blazor's type-based `IsAssignableFrom` matching
  now discriminates chart parents precisely — `SidebarContext` and any other unrelated cascading
  value can never satisfy these interfaces.
- `ComposedChart` naturally implements all four `I*SeriesParent` interfaces, preserving full
  composed-chart support without dual cascading parameters or any other workaround.
- All `dynamic?` cascading parameters replaced with the appropriate interface or concrete type.
  All `try { } catch { }` blocks removed — misuse is now a compile-time error rather than a
  silent runtime no-op.
- All chart `Register*` methods changed from `internal` to `public` (required for interface
  implementation and correct as component registration protocol).

---

## 2026-2-21

### ⚡ Performance: Replaced C# `@onkeydown` Handlers with `menu-keyboard.js` in All Menu Content Containers

**Motivation:** Every `DropdownMenu`, `Menubar`, and `ContextMenu` content container handled
keyboard navigation (ArrowDown/Up, Home/End, Enter/Space, Escape, ArrowLeft/Right) via
Blazor `@onkeydown` — a full C# SignalR round-trip per keystroke. Each individual item
(`MenuItem`, `CheckboxItem`, `RadioItem`, `SubTrigger`) also duplicated Enter/Space handlers.
This caused:
- One Blazor round-trip per keydown event even for simple focus movement
- Navigation `await` latency visible at ≥ 100 ms on Interactive Server
- Duplicated keydown logic scattered across 15+ components
- `FocusElementAsync` JS interop helper duplicated in every item component

**What Changed:**

A new `menu-keyboard.js` ES module centralises all menu keyboard behaviour in JavaScript.
C# is called back only for **state-changing events** (Escape closes, ArrowLeft closes submenu,
ArrowRight/Left switches Menubar menus). Item focus movement and Enter/Space activation never
leave the browser.

---

### 🆕 New JS Module: `menu-keyboard.js`

`src/BlazorUI.Primitives/wwwroot/js/primitives/menu-keyboard.js`

Supports three modes attached to a `role="menu"` container:

| Mode | Used by | Extra keys |
|---|---|---|
| `vertical` | `DropdownMenuContent`, `ContextMenuContent` | — |
| `menubar` | `MenubarContent` | ArrowRight/Left → `JsOnNextMenu` / `JsOnPreviousMenu` |
| `submenu` | `*SubContent` (all three families) | ArrowLeft → `JsOnCloseSubMenu` |

Options: `loop` (wrap navigation), `initialFocus` (`"first"` focuses first enabled item on open).

JS calls back to C# only via:
- `JsOnEscapeKey()` — close the menu/root
- `JsOnCloseSubMenu()` — close submenu + restore focus to trigger
- `JsOnNextMenu()` / `JsOnPreviousMenu()` — switch Menubar menu (menubar mode only)

---

### 🔧 Components Changed

#### DropdownMenu

| Component | Change |
|---|---|
| `DropdownMenuContent` | Replaced `keyboard-nav.js` + C# `HandleKeyDown` with `menu-keyboard.js` "vertical" mode; added `[JSInvokable] JsOnEscapeKey` |
| `DropdownMenuSubContent` | Replaced `keyboard-nav.js` + C# `HandleKeyDown` with `menu-keyboard.js` "submenu" mode; added `JsOnEscapeKey`, `JsOnCloseSubMenu` |
| `DropdownMenuItem` | Removed `@onkeydown`, `@inject IJSRuntime`, `FocusElementAsync` helper; added `Href`/`Target` params (renders `<a>` when set) and `MergedAttributes` (merges `Context.ItemClass`) |
| `DropdownMenuCheckboxItem` | Removed `@onkeydown` / `HandleKeyDown` |
| `DropdownMenuRadioItem` | Removed `@onkeydown` / `HandleKeyDown` |
| `DropdownMenuSubTrigger` | Removed `@onkeydown` / `HandleKeyDown`; removed `@inject IJSRuntime` / `FocusElementAsync` |
| `DropdownMenuContext.cs` | Added `ItemClass` property (cascaded from `DropdownMenu.ItemClass`) |
| `DropdownMenu.razor` | Added `ItemClass` parameter; synced to context in `OnParametersSet` |

#### Menubar

| Component | Change |
|---|---|
| `MenubarContent` | Replaced `keyboard-nav.js` + C# `HandleKeyDown` / navigation methods with `menu-keyboard.js` "menubar" mode; added `JsOnEscapeKey`, `JsOnNextMenu`, `JsOnPreviousMenu`; simplified `FocusContainerAsync` |
| `MenubarSubContent` | Replaced `keyboard-nav.js` + C# `HandleKeyDown` with `menu-keyboard.js` "submenu" mode; added `JsOnEscapeKey`, `JsOnCloseSubMenu` |
| `MenubarItem` | Removed `@onkeydown`, `@inject IJSRuntime`, `FocusElementAsync` helper |
| `MenubarCheckboxItem` | Removed `@onkeydown` / `HandleKeyDown` |
| `MenubarRadioItem` | Removed `@onkeydown` / `HandleKeyDown` |
| `MenubarSubTrigger` | Removed `@onkeydown` / `HandleKeyDown` |

#### ContextMenu

| Component | Change |
|---|---|
| `ContextMenuContent` | Replaced `keyboard-nav.js` + C# `HandleKeyDown` + inline navigation methods with `menu-keyboard.js` "vertical" mode; added `JsOnEscapeKey` |
| `ContextMenuSubContent` | Replaced C# `HandleKeyDown` + `FocusNextItem`/`FocusPreviousItem` helpers with `menu-keyboard.js` "submenu" mode; added `JsOnEscapeKey`, `JsOnCloseSubMenu` |
| `ContextMenuItem` | Removed `@onkeydown`, `@inject IJSRuntime`, `FocusElementAsync` helper |
| `ContextMenuCheckboxItem` | Removed `@onkeydown` / `HandleKeyDown` |
| `ContextMenuRadioItem` | Removed `@onkeydown` / `HandleKeyDown` |
| `ContextMenuSubTrigger` | Removed `@onkeydown` / `HandleKeyDown` |

---

### 🐛 Bug Fix: Submenu Close Chain Broken for Keyboard-Opened Submenus

**Affected:** `DropdownMenuSubTrigger`, `MenubarSubTrigger`, `ContextMenuSubTrigger`

`HandleMouseEnter` registered `SubContext` as `ActiveSubMenu` on the parent context, but
`HandleClick` (called by both mouse click and keyboard Enter/Space/ArrowRight via JS `click()`)
did not. This meant that when a submenu was opened via keyboard, `Context.Close()` /
`MenuContext.CloseMenu()` could not recurse into the submenu — only the root menu closed while
the submenu panel remained visible.

**Fix:** `HandleClick` now registers `SubContext` as `ActiveSubMenu` on the parent context
(matching what `HandleMouseEnter` already did) before calling `SubContext.Open()`.

---

### ⚡ Performance: `PopoverContent` Escape Key Migrated to `dialog.js`

`PopoverContent` previously handled Escape via `@onkeydown` (one Blazor round-trip per keydown
inside the popover). It now reuses `dialog.js` (`initializeKeyboardHandler` /
`disposeKeyboardHandler`) — the same module `DialogContent` uses — which attaches a single
capture-phase JS listener and calls back to C# only when Escape is actually pressed.

Added `[JSInvokable] HandleEscapeKey()` matching the Dialog pattern. The `@onkeydown` binding
and C# `HandleKeyDown` method have been removed.

### 🐛 Bug Fix: `PopoverContent` Container Not Auto-Focused on Open

`PopoverContent` was missing the `data-autofocus` attribute on its content `<div>`. As a result:
- `portal.js`'s `blazorui:visible` auto-focus listener never moved focus into the popover
- The new `dialog.js` Escape handler (capture-phase, requires focus inside the element) therefore
  never fired when Escape was pressed

**Fix:** Added `data-autofocus` to the content div, consistent with `DropdownMenuContent`.

---

### ⌨️ Feature: DataTable Keyboard Navigation for Row Selection and Column Sorting

Keyboard navigation is now fully functional in the `Table` primitive for both row selection and
header-triggered sorting.

#### `TableRow` — Row selection keyboard nav

| Key | Behaviour |
|---|---|
| `ArrowDown` | Move focus to the next selectable row |
| `ArrowUp` | Move focus to the previous selectable row |
| `Enter` / `Space` | Toggle selection of the focused row |

- Rows receive `tabindex="0"` only when `EnableKeyboardNavigation` is true on the `TableContext`
- A visible focus ring (`focus:ring-2 focus:ring-ring focus:ring-inset`) is automatically added
  to keyboard-navigable rows via `ComputedClass`
- Focus movement is handled by a new `table-row-nav.js` JS module (`moveFocusToNextRow` /
  `moveFocusToPreviousRow`) using DOM sibling traversal — no C# round-trip required per move
- A capture-phase JS listener (`preventSpaceKeyScroll`) prevents Space and Arrow keys from
  scrolling the page while a row is focused; attached once on first render via `OnAfterRenderAsync`

#### `TableHeaderCell` — Sortable header keyboard nav

| Key | Behaviour |
|---|---|
| `Enter` / `Space` | Toggle sort direction for the column |

- Sortable header cells (those with a `ColumnId`) receive `tabindex="0"`; non-sortable cells
  remain at `tabindex="-1"` and are skipped in tab order
- `aria-sort` attribute reflects the current sort direction (`ascending`, `descending`, `none`)
  for correct screen-reader announcement

#### New JS module: `table-row-nav.js`

`src/BlazorUI.Primitives/wwwroot/js/primitives/table-row-nav.js`

| Export | Purpose |
|---|---|
| `preventSpaceKeyScroll(element)` | Capture-phase handler; prevents Space / ArrowUp / ArrowDown scroll; returns `{ dispose }` cleanup |
| `moveFocusToNextRow(element)` | Advances focus to the next sibling row with `tabindex`, manages `tabindex` roving |
| `moveFocusToPreviousRow(element)` | Same as above, backwards |

---

## 2026-02-20 - Force-Mount Overlay Architecture for All FloatingPortal Consumers

### 🏗️ Major Refactoring and Improvements to FloatingPortal

Several improvements were applied to `FloatingPortal.razor` alongside the force-mount work. These harden the lifecycle and eliminate
a class of WASM-specific rendering artefacts.

#### 1. JS-Delegated AutoUpdate Lifecycle

`showFloating` and `hideFloating` in `positioning.js` now own the full AutoUpdate lifecycle:

- **On hide (`hideFloating`):** AutoUpdate is disposed *synchronously before* the `requestAnimationFrame`
  callback. This stops wasted position computations while the portal is invisible and matches the
  upstream dispose-on-hide intent.
- **On show (`showFloating`):** AutoUpdate is re-created *after* recomputing the current position.
  JS handles the full sequence: dispose old → `computePosition` → set up new `autoUpdate` → show via
  `requestAnimationFrame`. A single C# `await ShowFloatingAsync(...)` call drives this entire chain.

The C# side no longer holds an `_positioningCleanup` handle in the ForceMount re-open path —
JS owns AutoUpdate exclusively via `element._autoUpdateCleanupId` stored on the DOM element itself.

```
Before (ForceMount re-open):   C# disposes old handle → JS computes → JS recreates AutoUpdate
After  (ForceMount re-open):   Single ShowFloatingAsync → JS owns full lifecycle internally
```

#### 2. `ForceMount` Lifecycle Guard: `_isUpdatingVisibility` + `_previousIsOpen` Edge Detection

Two flags cooperate to prevent re-entrant visibility updates during async transitions (critical
in WASM where `await` yields to the browser event loop):

- **`_previousIsOpen`** — set to the new value *before* the first `await`, so any re-render
  triggered by the async operation sees a stable snapshot and does not re-enter the branch.
- **`_isUpdatingVisibility`** — set to `true` for the duration of the async update; a concurrent
  `OnAfterRenderAsync` call that arrives while a transition is in-flight skips silently.

This eliminates the flicker/duplication class of bugs that occurred in Interactive Server when
Blazor's re-render cycle raced with the JS `requestAnimationFrame` callback.

#### 3. Separated ForceMount and Standard Lifecycle Paths

`OnAfterRenderAsync` is now split into two dedicated methods with a single dispatch:

- **`HandleForceMountLifecycleAsync()`** — portal registers once on first mount and stays
  registered indefinitely. Open/close transitions call `ShowFloatingAsync` / `HideAsync`
  without touching portal registration. The `_isPositioned` flag persists across visibility
  cycles so Blazor's virtual DOM diff never rewrites the `style` attribute after JS has taken
  ownership of position and visibility.

- **`HandleStandardLifecycleAsync()`** — portal mounts on open and fully unregisters on close
  via `CleanupAsync`. The original, backward-compatible path preserved for `ForceMount=false`
  consumers (e.g. `TooltipContent`, `HoverCardContent`) where DOM economy outweighs remount cost.

Each path is independently readable with no conditional branches bleeding across the two modes.
Adding behaviour to one lifecycle cannot accidentally affect the other.

---

### ⚡ Performance: `FloatingPortal.ForceMount` Now Defaults to `true`

`ForceMount` was previously opt-in (`false` by default, first introduced 2026-02-11). It is now
`true` by default across the entire library. Every floating portal stays registered in the DOM and
is hidden/shown exclusively via JS — eliminating remount overhead and enabling CSS exit animations
by default for all consumers without any parameter changes.

**Migration:** No action required. If a specific consumer should still unmount on close (e.g. to
keep the DOM lean when many instances coexist), pass `ForceMount="false"` explicitly. Both
`TooltipContent` and `HoverCardContent` already do this.

---

### ⚡ Performance: Eliminated Outer `@if` Gates on All Floating Overlays

**Motivation:** Every `FloatingPortal` consumer had an outer `@if (Context.IsOpen)` guard that destroyed
and recreated the entire component subtree on each open/close cycle — directly defeating `ForceMount`,
which was already the default on `FloatingPortal`. This caused:
- Full portal unmount/remount, losing all cached JS handles and DOM state
- Re-execution of `MountPortalAsync`, JS module imports, and `SetupAsync` on every open
- Visible re-mount latency, especially under Interactive Server (SignalR round-trips)

**What Changed:**

The outer `@if` gate was removed from **9 components**. `FloatingPortal` is now always rendered;
open/close state is communicated via `IsOpen`, and `ForceMount` handles visibility via JS
(`requestAnimationFrame` + `data-state` transitions) without any Blazor re-mount overhead.

#### Components changed to always-mounted (`ForceMount=true`, default)

| Component | File |
|---|---|
| `DropdownMenuContent` | `Primitives/DropdownMenu/DropdownMenuContent.razor` |
| `DropdownMenuSubContent` | `Primitives/DropdownMenu/DropdownMenuSubContent.razor` |
| `PopoverContent` | `Primitives/Popover/PopoverContent.razor` |
| `SelectContent` | `Primitives/Select/SelectContent.razor` |

#### Components changed to standard lifecycle (`ForceMount=false`, explicit)

| Component | File | Reason |
|---|---|---|
| `TooltipContent` | `Primitives/Tooltip/TooltipContent.razor` | Can have 30–100+ instances per page; hover delay absorbs mount cost |
| `HoverCardContent` | `Primitives/HoverCard/HoverCardContent.razor` | Same multiplicity concern; richer content makes N hidden portals expensive |
| `ContextMenuContent` | `Primitives/ContextMenu/ContextMenuContent.razor` | Not fit by design |
| `ContextMenuSubContent` | `Primitives/ContextMenu/ContextMenuSubContent.razor` | Not fit by design |
| `MenubarSubContent` | `Primitives/Menubar/MenubarSubContent.razor` | Not fit by design |

#### Already correct (no change needed)

- `MenubarContent` — was already unconditionally rendered

**Specific changes per component:**

- **All 9 components:** `IsOpen="true"` → `IsOpen="@Context.IsOpen"` (or equivalent context property)
- **`ContextMenuContent`:** Overlay `<div>` kept in its own narrow `@if (Context.IsOpen)` — it is a
  full-screen fixed hit-test layer and should not persist in the DOM when the menu is closed
- **`SelectContent`:** Removed redundant `ForceMount="@ForceMount"` pass-through (FloatingPortal
  already defaults to `true`); fixed `AnchorElement` to null-safe `_context?.State.TriggerElement`
  since the outer gate previously guaranteed `_context` was non-null
- **`TooltipContent` / `HoverCardContent`:** Added explicit `ForceMount="false"` — component instance
  stays alive (preserving `_portalId` and event subscriptions) but the portal DOM node is
  created/destroyed per open cycle, keeping the DOM lean when many instances coexist

**Benefits:**
- ✅ **Zero re-mount overhead** on reopen for force-mounted components — JS toggles visibility directly
- ✅ **JS handles stay warm** — `_keyboardNavCleanup`, `_clickOutsideCleanup`, etc. survive close/reopen cycles
- ✅ **CSS exit animations** — `data-state` transitions work correctly because the DOM node is never destroyed mid-animation
- ✅ **WASM-friendly** — single `ShowFloatingAsync` call replaces full portal setup on reopen
- ✅ **Lean DOM** for hover-triggered components — Tooltip and HoverCard nodes only exist when open

---

## 2026-02-18 - Theme System Migration & Complete XML Documentation

### 🎨 Theme System Migration to Component Library

**Migrated the entire theme system to NeoUI.Blazor for zero-configuration, reusable theming:**

**What Changed:**
- **ThemeService** - Moved from demo project to `src/BlazorUI.Components/Services/Theming/ThemeService.cs`
  - Now part of the component library for easy consumption
  - Zero configuration required - just add the service and components
  - Manages theme state, dark/light mode, base colors (5 options), and primary colors (17 options)
  - Includes LocalStorage persistence and SSR-safe initialization

- **Theme Components** - Moved to `src/BlazorUI.Components/Components/Theme/`
  - **ThemeSwitcher** - Popover-based theme configuration panel with live preview
  - **DarkModeToggle** - Switch component for dark/light mode switching

- **Theme JavaScript** - Moved to `src/BlazorUI.Components/wwwroot/js/theme.js`
  - CSP-compliant theme application and dark mode detection
  - Available via `_content/NeoUI.Blazor/js/theme.js`

- **Theme CSS Files** - Moved to `src/BlazorUI.Components/wwwroot/css/themes/`
  - All 5 base color themes (Zinc, Slate, Gray, Neutral, Stone)
  - All 17 primary color themes (Red, Rose, Orange, Amber, Yellow, Lime, Green, Emerald, Teal, Cyan, Sky, Blue, Indigo, Violet, Purple, Fuchsia, Pink)
  - Available via `_content/NeoUI.Blazor/css/themes/...`

**Benefits:**
- ✅ **Zero Configuration** - Just reference the component library
- ✅ **Reusable** - Theme system available to all consuming applications
- ✅ **Easy Integration** - Add `<ThemeSwitcher />` anywhere in your app
- ✅ **Complete Theming** - 85 theme combinations (5 base × 17 primary colors)
- ✅ **Production Ready** - All theme assets served from `_content/` path

**Migration Path:**
```razor
<!-- Reference theme CSS from component library -->
<link href="_content/NeoUI.Blazor/css/themes/base/zinc.css" rel="stylesheet" />
<link href="_content/NeoUI.Blazor/css/themes/primary/blue.css" rel="stylesheet" />

<!-- Include theme JavaScript -->
<script src="_content/NeoUI.Blazor/js/theme.js"></script>
```

---

### 📚 Complete XML Documentation - 0 Warnings

**Added comprehensive XML documentation to all public members across the entire component library:**

**Documentation Coverage:**
- ✅ **Zero Build Warnings** - Complete XML documentation for all public/protected members
- ✅ **250+ Documented Members** - Comprehensive documentation across all components
- ✅ **IntelliSense Support** - Full IntelliSense descriptions for all APIs
- ✅ **Parameter Documentation** - Clear descriptions for all component parameters
- ✅ **Method Documentation** - Detailed explanations of public methods and callbacks

**Components Documented:**
- Input Components (Input, Textarea, NumericInput, MaskedInput, CurrencyInput, ColorPicker, FileUpload)
- Layout Components (Sidebar, SidebarProvider, RangeSlider, RichTextEditor, MultiSelect)
- Data Components (DataTable, Grid, Pagination)
- Display Components (Chart, Empty, MarkdownEditor)
- Navigation Components (ResponsiveNavProvider)
- Services (CollapsibleStateService, ThemeService)
- Primitives (MotionPresetBase, ClassNames)
- Validation (InputValidationBehavior)

**Documentation Quality:**
- Clear descriptions of purpose and functionality
- Parameter documentation with type and usage information
- Return value descriptions for methods
- Remarks for complex behavior and edge cases
- Examples where applicable

**Developer Experience:**
- 🚀 Enhanced IntelliSense in Visual Studio and VS Code
- 📖 Better API discoverability
- 🎯 Reduced learning curve for new developers
- ✨ Professional-grade documentation standards

---

## 2026-02-15 - Multi-Color Theme System with Dynamic Theming

### 🎨 Features - Dynamic Theme System

**Implemented a comprehensive theme customization system with live theme switching:**

**Key Features:**
- **Base Color Selection** - 5 base color options (Zinc, Slate, Gray, Neutral, Stone) for foundational UI colors
- **Primary Color Selection** - 17 primary color options (Red, Rose, Orange, Amber, Yellow, Lime, Green, Emerald, Teal, Cyan, Sky, Blue, Indigo, Violet, Purple, Fuchsia, Pink)
- **Dark/Light Mode Toggle** - Seamless switching between dark and light themes
- **Live Theme Preview** - Real-time theme changes without page reload
- **LocalStorage Persistence** - Theme preferences saved and restored across sessions
- **CSP-Compliant** - Uses named JavaScript functions instead of eval for Content Security Policy compliance

---

### 🏗️ Implementation Details

**1. ThemeService (demo\BlazorUI.Demo.Shared\Services\ThemeService.cs):**
```csharp
// Core service managing theme state
- BaseColor enum: Zinc, Slate, Gray, Neutral, Stone
- PrimaryColor enum: 17 color options
- Methods: ToggleThemeAsync(), SetBaseColorAsync(), SetPrimaryColorAsync()
- Event-driven updates: OnThemeChanged event
- LocalStorage integration for persistence
- SSR-safe initialization
```

**2. Theme UI Components:**

**ThemeSwitcher (demo\BlazorUI.Demo.Shared\Common\ThemeSwitcher.razor):**
- Popover-based theme configuration panel
- Visual color pickers with preview swatches
- DataGrid layout for base colors (5 options) and primary colors (17 options)
- Selected state indicators with ring highlights
- Integrated DarkModeToggle
- Tooltip showing current theme selection (e.g., "Zinc / Blue")

**DarkModeToggle (demo\BlazorUI.Demo.Shared\Common\DarkModeToggle.razor):**
- Switch component with sun/moon icons
- Real-time dark/light mode switching
- Visual feedback of current mode

**3. JavaScript Integration (demo\BlazorUI.Demo.Client\wwwroot\js\theme.js):**
```javascript
window.theme = {
    apply: function(config) { /* Applies theme classes dynamically */ },
    isDark: function() { /* Checks current theme mode */ }
}
```

**4. CSS Theme System (demo\BlazorUI.Demo.Client\wwwroot\styles\theme.css):**
- OKLCH color space for perceptually uniform colors
- Comprehensive CSS custom properties for all theme variables
- Separate light/dark mode definitions
- Support for dynamic base and primary color classes

---

### 🎯 User Experience

**Theme Switcher Features:**
- **Visual Color Representation** - Each base color displays with its distinctive hue (500 shade)
- **Clear Selection State** - Selected colors highlighted with accent background and ring indicator
- **Organized Layout** - Separate sections for Base Color and Theme Color
- **Accessible** - Proper ARIA labels and keyboard navigation support
- **Tooltip Feedback** - Hover over theme switcher to see current selection

**Persistence:**
- Theme preferences stored in localStorage
- Automatic restoration on page reload
- Syncs with system prefers-color-scheme on first visit

**Benefits:**
- ✅ Complete theme customization without code changes
- ✅ Live preview of all theme combinations
- ✅ Persistent user preferences
- ✅ SSR-compatible initialization
- ✅ CSP-compliant implementation
- ✅ Accessible and keyboard-navigable UI
- ✅ 85 total theme combinations (5 base × 17 primary colors)

---

## 2026-02-14 - Toast Component Enhancements & Input Validation Refactoring

### 🎨 Toast Component - Granular Customization & UX Improvements

**Added comprehensive customization options to Toast component with size control, auto icons, pause-on-hover, and per-toast positioning.**

**Key Features:**

1. **ToastSize Enum - Default & Compact Sizes:**
   - `ToastSize.Default` - Standard padding (px-4 py-3 pr-8) for regular notifications
   - `ToastSize.Compact` - Reduced padding (px-3 py-2 pr-6) for dialogs and dense UI
   - Per-toast size control via `ToastOptions.Size` property

2. **Auto Variant Icons using LucideIcon:**
   - ✅ Success - `check` icon (green)
   - ✗ Error/Destructive - `x` icon (red)
   - ⚠ Warning - `alert-triangle` icon (yellow)
   - ℹ Info - `info` icon (blue)
   - Icons enabled by default, can be toggled via `ShowIcon` property
   - Replaced inline SVG with LucideIcon for consistency

3. **Pause on Hover Functionality:**
   - Timer pauses when user hovers over toast
   - Accurately tracks elapsed time before pause
   - Resumes with correct remaining duration on mouse leave
   - Stable timer management with proper state tracking
   - Enabled by default, can be disabled via `PauseOnHover` property

4. **Per-Toast Position Override:**
   - Added `Position` property to `ToastOptions`
   - Toasts grouped by position and rendered in separate viewports
   - Falls back to `ToastProvider`'s default position if not specified
   - Supports all 6 positions: TopLeft, TopCenter, TopRight, BottomLeft, BottomCenter, BottomRight

5. **Convenient Constructors & Factory Methods:**
   - Traditional constructors: `new ToastOptions(title, description)`, `new ToastOptions(title, description, variant)`
   - Static factory methods:
     - `ToastOptions.Success(title, description)`
     - `ToastOptions.Error(title, description)`
     - `ToastOptions.Warning(title, description)`
     - `ToastOptions.Info(title, description)`
     - `ToastOptions.Compact(title, description, variant, position)` - Pre-configured for dialogs
   - Cleaner, more readable API

**Demo Updates:**
- **ToastDemo** - Added examples for Size, Icons, Pause-on-Hover, Position override
- **DialogDemo** - All 12 toast notifications now use `ToastOptions.Compact()` with `BottomCenter` position
- Enhanced usage examples showing factory methods and advanced customization

**Benefits:**
- ✅ **Granular control** - Size, position, icons, pause behavior all customizable per-toast
- ✅ **Better UX** - Pause-on-hover prevents accidental dismissal while reading
- ✅ **Dialog-friendly** - Compact size and bottom-center positioning perfect for dialog notifications
- ✅ **Developer-friendly API** - Fluent factory methods reduce boilerplate
- ✅ **Consistent design** - LucideIcon integration matches component library standards
- ✅ **Production-ready** - Stable timer management with accurate pause/resume

---

## 2026-02-13 - Input Validation Architecture Refactoring & Dialog Performance Optimization

### 🏗️ Architecture - Input Validation Behavior Centralization

**Refactored all input components to use a centralized validation behavior pattern, eliminating ~410 lines of duplicated code and improving consistency.**

**Key Changes:**

1. **New InputValidationBehavior Class (Shared Validation Logic):**
   - Created `src/BlazorUI.Components/Validation/InputValidationBehavior.cs` (212 lines)
   - Encapsulates all validation logic: error display, focus management, first-invalid tracking
   - Reusable across all input components via composition pattern
   - Handles EditContext integration, validation state changes, and ARIA attributes
   - Provides clean API: `OnParametersSet()`, `UpdateValidationDisplayAsync()`, `HandleValidationStateChangedAsync()`, `NotifyFieldChangedAsync()`
   - Eliminates ~410 lines of duplicated validation code across 5 components (48% code reduction)

2. **Refactored Input Components (Using InputValidationBehavior):**
   - **Input component** - Complete refactoring (reference implementation)
   - **Textarea component** - Added validation support with behavior pattern
   - **CurrencyInput, MaskedInput and NumericInput component** - Refactored validation logic

3. **Validation UX Improvements:**
   - Added `shouldFocus` parameter (3rd param) to `setValidationError()` JS function
   - **Smart focus behavior:** Only steal focus when explicitly requested (form submit validation)
   - **Natural tab navigation:** Validation errors show without disrupting user's tab flow
   - **Better UX:** Error tooltip appears without forcing focus back to invalid field during natural navigation
   - `input.js` updated to accept optional `shouldFocus` parameter (default: true for backward compatibility)

4. **Dialog Performance Optimization:**
   - **Critical Fix:** Moved `@onkeydown` event handling from C# to JavaScript in DialogContent
   - Created `src/BlazorUI.Primitives/wwwroot/js/primitives/dialog.js` keyboard handler
   - **Performance gain:** Zero C# roundtrips for regular typing (only Escape key triggers C# callback)
   - **Before:** Every keystroke in dialog caused C# interop → parent re-render → all inputs re-render
   - **After:** JavaScript intercepts all keydown events, only calls C# for Escape key
   - Eliminates massive performance bottleneck when typing in dialog forms

5. **Bug Fixes:**
   - **Textarea component:** Fixed `aria-invalid` attribute using `AriaInvalid` instead of `EffectiveAriaInvalid`
   - Validation errors now correctly display red border styling on Textarea
   - All refactored components verified to use `EffectiveAriaInvalid` in markup

6. **FloatingPortal Stability & Rendering Efficiency Improvements:**
   - **Re-entrant Call Prevention:** Added `_isUpdatingVisibility` guard flag to prevent infinite loops during ForceMount async visibility updates (WebAssembly)
   - **Smart State Tracking:** Updates `_previousIsOpen` and guard flag BEFORE async operations to prevent race conditions
   - **Efficient Refresh Pattern:** `RefreshPortal()` triggers re-render WITHOUT replacing RenderFragment (preserves captured values and ElementReference)
   - **Conditional Updates:** Only updates visibility when `IsOpen` actually changes and not already updating
   - **Result:** Eliminated flickering, prevented portal content duplication, and fixed timing-sensitive focus issues

7. **TailwindMerge Arbitrary Value Support Enhancement:**
   - **Support Modifiers Grouping:** TailwindMerge now correctly groups modifiers (hover:, focus:, data-state=) with arbitrary values
   - **Enhanced Regex Patterns:** Updated color regexes to support arbitrary values with commas and spaces

8. **Toast Animation Perfection (JavaScript-Delegated with RAF):**
   - **Problem Solved:** Blazor re-renders were interfering with CSS transitions causing "jumpy" animations
   - **JavaScript-First Approach:** Created `toast-animation.js` module to handle animation state entirely in JS
   - **Benefits:**
     - ✅ **Smooth entrance animations** - No flicker or jump during initial render
     - ✅ **Blazor-agnostic timing** - RAF ensures proper paint cycles regardless of C# timing
     - ✅ **Graceful fallback** - Toast still visible if JS fails (no animation, but functional)
     - ✅ **Zero re-render interference** - JS manages `data-state`, Blazor manages content

**Code Metrics:**
- **Removed:** ~410 lines of duplicated validation code (across 5 components)
- **Added:** ~337 lines total (212 shared behavior class + 50 JS dialog handler + 75 component overhead)
- **Net reduction:** ~73 lines
- **Duplicated validation logic:** 410 lines → 212 shared lines (48% reduction)
- **Consistency:** 100% - All input components now follow identical validation pattern

**Technical Details:**

**Per Component Changes:**
- Removed manual validation fields: `_firstInvalidInputIdKey`, `_validationModule`, `_previousEditContext`, `_fieldIdentifier`, `_currentErrorMessage`, `_hasShownTooltip`
- Replaced with single field: `private InputValidationBehavior? _validationBehavior;`
- Standardized lifecycle methods: `OnInitialized()`, `OnParametersSet()`, `OnValidationStateChanged()`, `OnAfterRenderAsync()`, `DisposeAsync()`
- All components use `EffectiveAriaInvalid` property delegating to behavior
- Consistent EditContext subscription/unsubscription pattern

**Component-Specific Preservation:**
- **NumericInput:** Preserved `ValidateAndClamp()`, blur validation, and MaxLength enforcement
- **MaskedInput:** Preserved masking logic, `OnValueChanged()` callback, and fallback handling
- **CurrencyInput:** Preserved currency formatting, focus/blur handlers, and culture handling

**Benefits:**
- ✅ **Single source of truth** for validation logic
- ✅ **Consistent behavior** across all input components
- ✅ **Easier maintenance** - validation fixes apply to all components
- ✅ **Better testability** - validation logic isolated in behavior class
- ✅ **Performance** - Dialog typing no longer triggers re-renders
- ✅ **Better UX** - Smart focus behavior during validation
- ✅ **Stable Portals** - No flickering, no duplicate content, no race conditions
- ✅ **Full Tailwind Support** - Arbitrary values with complex CSS functions work correctly
- ✅ **Smooth Toast Animations** - Professional entrance animations without jumps or flicker

## 2026-02-11 - Nested Dialog, Variant and Enhanced Portal Management

### ✨ Feature - Dialog Variants & Nested Dialog Support

**Added DialogContentVariant system and improved portal z-index management for better nested dialog behavior.**

**Key Changes:**

1. **Dialog Variant System:**
   - Added `DialogContentVariant` enum with `Default` and `Form` variants
   - Added `Variant` parameter to `DialogContent` component for flexible background styling
   - **Default variant:** Uses `bg-background` for simple, neutral dialogs (maintains original behavior)
   - **Form variant:** Uses `bg-card` with `--scroll-shadow-bg:var(--card)` CSS variable
   - Form variant ensures `ScrollArea` scroll shadows match dialog background color
   - Backward compatible - Default is the default variant value

2. **Enhanced Portal System (Primitives):**
   - Enhanced `IPortalService` with portal type tracking and z-index management
   - Extracted `ZIndexLevels` to dedicated service class in `Services` namespace
   - Implemented automatic z-index layering: Dialogs (100) > Menus (70) > Floating (50)
   - Improved portal registration with unique IDs and automatic lifecycle management
   - Better disposal and cleanup to prevent memory leaks

3. **Nested Dialog Support:**
   - Proper z-index management ensures nested dialogs appear above parent dialogs
   - Portal type tracking provides predictable visual layering
   - Fixed stacking issues when multiple dialogs are open simultaneously

4. **Portal-Based Component Updates:**
   - Updated `DialogPortal` with enhanced portal management including automatic refresh on state changes
   - Updated `FloatingPortal` with portal type support
   - Updated all menu/popover components to use new portal service API:
     - `PopoverContent`
     - `DropdownMenuContent`, `DropdownMenuSubContent`
     - `ContextMenuContent`, `ContextMenuSubContent`
     - `MenubarContent`, `MenubarSubContent`
     - `SelectContent`
     - `TooltipContent`
     - `AlertDialogContent`

**Breaking Changes:**
- **Moved ZIndexLevels** from `NeoUI.Blazor.Primitives.Constants` to `NeoUI.Blazor.Primitives.Services`
  - **Migration**: Update `using` statements from `NeoUI.Blazor.Primitives.Constants` to `NeoUI.Blazor.Primitives.Services`

## 2026-02-11 - UI Consistency Improvements for Select, Combobox, MultiSelect, and RadioGroup

### 🎨 UI/UX Improvements

**Enhanced visual consistency across form components and improved RadioGroup check variant behavior.**

**Key Changes:**

1. **RadioGroup Check Variant - Clean Unselected State:**
   - RadioGroupItem with Check variant now shows nothing when unselected (previously showed a circle)
   - Selected state displays a checkmark icon as expected
   - Creates cleaner, more modern UI for card-style radio selections
   - Matches design patterns from shadcn/ui and modern UI libraries

2. **Form Component Font Weight Consistency:**
   - Removed `font-medium` from MultiSelect trigger CSS
   - Removed `font-medium` from Combobox trigger CSS
   - All form controls (Select, MultiSelect, Combobox, Input, etc.) now have consistent font weight
   - Creates more uniform appearance across all form elements

3. **Select Hover Behavior Fix:**
   - Added `[&[aria-expanded=true]]:hover:bg-background` to SelectTrigger
   - Disables hover styling when Select popup is open
   - Matches correct behavior of MultiSelect and Combobox
   - Prevents confusing visual feedback during interaction

4. **DateRangePicker File Organization:**
   - Moved DateRangePicker from DatePicker folder to dedicated DateRangePicker folder
   - Removed duplicate DateRangePicker.razor from DatePicker directory
   - Better component organization and clearer separation of concerns

5. **ScrollArea Component Enhancements:**
   - Added new FillContainer setting to support usage in form featuring automatic height calculation by excluding header and footer area
   - Enhanced scroll-area.js for better dynamic content handling

6. **Combobox Component Updates:**
   - Refined Combobox.razor template structure
   - Font weight consistency applied (font-medium removed from trigger)

**Component Changes:**
- `src/BlazorUI.Components/Components/RadioGroup/RadioGroupItem.razor` - Check variant unselected state
- `src/BlazorUI.Components/Components/RadioGroup/RadioGroupItem.razor.cs` - Check variant logic
- `src/BlazorUI.Components/Components/MultiSelect/MultiSelect.razor.cs` - Removed font-medium
- `src/BlazorUI.Components/Components/Combobox/Combobox.razor.cs` - Removed font-medium
- `src/BlazorUI.Components/Components/Combobox/Combobox.razor` - Template refinements
- `src/BlazorUI.Components/Components/Select/SelectTrigger.razor` - Hover behavior when open
- `src/BlazorUI.Components/Components/DatePicker/DatePicker.razor` - Minor updates
- `src/BlazorUI.Components/Components/DateRangePicker/DateRangePicker.razor` - File reorganization
- `src/BlazorUI.Components/Components/ScrollArea/ScrollArea.razor` - Enhanced behavior
- `src/BlazorUI.Components/wwwroot/js/scroll-area.js` - Dynamic content improvements
- `src/BlazorUI.Components/wwwroot/blazorui.css` - Scrolling styles


## 2026-02-11 - Added ForceMount to FloatingPortal, Improved Select Component Reliability

### ✨ Feature - ForceMount for Select Components

**Implemented ForceMount pattern to keep portal content mounted when closed, eliminating the need for DisplayTextSelector and improving performance.**

Select component now sets ForceMount to true by default. You can disable the behavior by setting SelectContent's ForceMount property to false.

**Key Benefits:**
- ✅ **No more DisplayTextSelector needed** - Items register immediately, DisplayText resolves automatically
- ✅ **Better performance** - No re-mounting/unmounting on each open/close
- ✅ **Smoother animations** - Content stays in DOM, only visibility toggles
- ✅ **Persistent handlers** - Click-outside and keyboard navigation handlers stay active
- ✅ **Cleaner API** - Less configuration required for common use cases
- ✅ **Reliable Select in modals** - DisplayText and value management work consistently in Dialog/AlertDialog contexts

**Major Infrastructure Improvements:**

1. **FloatingPortal ForceMount Support:**
   - Added `ForceMount` parameter to keep portal mounted when closed (opt-in, default: false)
   - Portal stays registered with PortalService, content remains in DOM but hidden
   - On open: `showFloating()` toggles `data-state="open"` and visibility styles
   - On close: `hideFloating()` toggles `data-state="closed"` and hides content
   - Features that continue to work:
     - ✅ CSS animations triggered by `data-state` transitions
     - ✅ Event handlers persist (click-outside, keyboard navigation)
     - ✅ Item registration survives across open/close cycles
     - ✅ Positioning auto-update continues to track reference element
   - SelectContent uses ForceMount by default (ForceMount="true")
   - Eliminates re-mounting overhead and enables DisplayText resolution without DisplayTextSelector

2. **Select Value/DisplayText Reliability:**
   - Fixed DisplayText resolution across standalone and modal contexts
   - Items now register on mount (with ForceMount), DisplayText available immediately
   - Eliminated race conditions between item registration and value display
   - ClearItems() on close ensures dynamic item updates work correctly
   - No more stale parameter values - automatic restoration from context in OnParametersSet
   - Proper focus management on every open (scroll + keyboard navigation)

**Implementation:**

1. **ForceMount Parameter:**
   - Added to `FloatingPortal` (default: false, opt-in)
   - Added to `SelectContent` (default: true, enabled by default for Selects)
   - Portal stays registered when closed, content hidden via CSS

2. **Visibility Management:**
   - Added `showFloating()` and `hideFloating()` to positioning.js
   - Toggles `data-state` attribute for CSS animations
   - Uses `requestAnimationFrame` for smooth transitions
   - Added `ShowFloatingAsync()` and `HideFloatingAsync()` to IPositioningService

3. **Focus Management:**
   - Keyboard handlers setup once, persist across opens
   - `focusContent()` called on every open for proper keyboard navigation
   - Scroll position restored to selected item on each open

4. **Item Registration:**
   - Items mount immediately with ForceMount
   - Register with SelectContext on mount
   - DisplayText resolved from registered items automatically
   - Items cleared on close, re-register on next open (supports dynamic items)

5. **State Tracking:**
   - `_previousIsOpen` flag prevents redundant visibility updates
   - `_isKeyboardSetup` and `_isClickOutsideSetup` prevent duplicate handlers
   - Cleanup only on component disposal (not on close)

6. **Select in Modals:**
   - Fixed parameter restoration when Select used in Dialog/AlertDialog
   - OnParametersSet ensures controlled value stays in sync with context
   - DisplayText remains stable across dialog open/close cycles
   - No more flickering or stale values when dialog reopens

**Breaking Changes:**
- None - ForceMount is opt-in for FloatingPortal, opt-out for Select

**Migration:**
- Remove `DisplayTextSelector` from Select components (no longer needed)
- All demo pages updated to remove DisplayTextSelector
- Backward compatible - DisplayTextSelector still works as fallback

**Files Changed:**
- `src/BlazorUI.Primitives/Primitives/Floating/FloatingPortal.razor` - ForceMount implementation
- `src/BlazorUI.Primitives/Primitives/Select/SelectContent.razor` - ForceMount enabled by default
- `src/BlazorUI.Primitives/Primitives/Select/SelectContext.cs` - Item registration with ClearItems
- `src/BlazorUI.Primitives/Primitives/Select/Select.razor` - OnParametersSet value restoration for modals
- `src/BlazorUI.Primitives/Primitives/Select/SelectValue.razor` - Direct DisplayText reading (removed caching)
- `src/BlazorUI.Primitives/Services/IPositioningService.cs` - Show/Hide methods
- `src/BlazorUI.Primitives/Services/PositioningService.cs` - Show/Hide implementation
- `src/BlazorUI.Primitives/wwwroot/js/primitives/positioning.js` - showFloating/hideFloating with data-state toggle
- `src/BlazorUI.Primitives/wwwroot/js/primitives/select.js` - focusContent for repeated opens
- Demo pages: Removed DisplayTextSelector from SelectPrimitiveDemo, AreaChart, BarChart, LineChart, PieChart examples

---

## 2026-02-09 - Two-Layer Portal Architecture: Categorized Hosts + Hierarchical Scopes

### 🏗️ Architecture - Two-Layer Portal System

**Implemented a two-layer portal architecture combining categorized hosts with hierarchical scopes:**

**Layer 1: Categorized Portal Hosts** (Type-Based Separation)
- Portals categorized by type: `Container` (Dialog, Sheet) vs `Overlay` (Dropdown, Tooltip)
- Separate host components prevent render cascades across portal types
- Each host only re-renders when portals in its category change
- ~90% reduction in cross-category re-renders

**Layer 2: Hierarchical Portal Scopes** (Parent-Child Relationships)
- Within each category, portals can form parent-child hierarchies
- Children append to parent's scope instead of creating new portals
- Resolves infinite render loops in nested menus
- ~83% reduction in re-renders for multi-level menus

**Combined Result:**
- ~92% reduction in total unnecessary re-renders
- ~60% reduction in DOM portal elements
- Zero infinite loops with nested components
- Clear separation of concerns

---

### 🏗️ Layer 1: Categorized Portal Hosts (Phase 1-3)

**Problem:**
- Single PortalHost re-rendered ALL portals when any portal changed
- Dialog opening triggered re-render of unrelated Dropdown menus
- Tooltips re-rendered when Sheets opened
- No isolation between different portal types

**Solution:**
- Created `PortalCategory` enum: `Container` vs `Overlay`
- Implemented category-specific portal hosts:
  - `ContainerPortalHost` - Renders Dialog, Sheet, AlertDialog, Drawer (z-index 40-50)
  - `OverlayPortalHost` - Renders Dropdown, Tooltip, Popover, Select (z-index 60-70)
- Category-based event system: `OnPortalsCategoryChanged`
- Each host subscribes only to its category's changes

**Implementation:**

1. **PortalCategory Enum:**
```csharp
public enum PortalCategory
{
    Container,  // Dialog, Sheet, AlertDialog, Drawer
    Overlay     // Popover, Tooltip, Select, Dropdown, etc.
}
```

2. **CategoryPortalHost Component:**
```razor
<CategoryPortalHost Category="PortalCategory.Overlay" />
```
- Generic base component for category-specific rendering
- Only re-renders when portals in its category change

3. **Specialized Portal Hosts:**
```razor
<!-- In root layout -->
<ContainerPortalHost />  <!-- For full-screen containers -->
<OverlayPortalHost />    <!-- For floating overlays -->
```

**Benefits:**
- ✅ Blazing-fast performance with minimal re-renders
- ✅ Container changes don't affect Overlay portals (and vice versa)
- ✅ Each host manages independent render cycle
- ✅ Clear architectural separation
- ✅ ~90% reduction in cross-category re-renders

---

### 🏗️ Layer 2: Hierarchical Portal Scopes

**Resolved infinite render loops with parent-child portal relationships:**

**Problem:**
- Nested menus (dropdown submenus, menubar submenus, context menu submenus) created separate portals
- Each child portal creation triggered parent re-render
- Multi-level nesting caused cascading re-renders → infinite loops

**Solution:**
- Implemented hierarchical portal system where children append to parent's scope
- Single portal per menu hierarchy instead of one portal per submenu
- ~83% reduction in re-renders for multi-level menus
- ~67% reduction in DOM portal elements

**Implementation:**

1. **Enhanced PortalService:**
   - Added `AppendToPortal(parentPortalId, childPortalId, content)` - Appends child to parent scope
   - Added `RemoveFromPortal(parentPortalId, childPortalId)` - Removes child from parent
   - Implemented `PortalScope` class to track parent-child relationships
   - Creates composite RenderFragments (parent + all children in order)

2. **Updated FloatingPortal:**
   - Added `ParentPortalId` parameter (defaults to null for backward compatibility)
   - Smart registration: appends to parent if `ParentPortalId` set, creates new portal otherwise
   - Proper cleanup: removes from parent scope or unregisters based on hierarchy

3. **Migrated Components to Hierarchical System:**
   - **DropdownMenuSubContent** - Now uses FloatingPortal + hierarchical portals
   - **MenubarSubContent** - Now uses FloatingPortal + hierarchical portals
   - **ContextMenuSubContent** - Migrated from legacy positioning to FloatingPortal + hierarchical portals

**Each submenu component implements:**
```csharp
private string? GetParentPortalId()
{
    // Nested submenu: use parent submenu's portal
    if (ParentSubContext != null)
        return $"[component]-submenu-{ParentSubContext.GetHashCode()}";
    
    // Direct child of root: use root menu's portal
    return RootContext != null ? $"[component]-portal-{RootContext.Id}" : null;
}
```

**Benefits:**
- ✅ Eliminates infinite loops and cascading re-renders
- ✅ Single portal render per menu hierarchy for utmost efficiency
- ✅ Natural DOM hierarchy (children inside parent scope)
- ✅ No z-index stacking issues
- ✅ Easier focus management within single portal
- ✅ Better memory efficiency
- ✅ Backward compatible (existing root portals work unchanged)

**Files Changed:**
- `IPortalService.cs` - Added hierarchical portal methods
- `PortalService.cs` - Implemented PortalScope system
- `FloatingPortal.razor` - Added ParentPortalId parameter
- `DropdownMenuSubContent.razor` - Uses hierarchical portals
- `MenubarSubContent.razor` - Uses hierarchical portals
- `ContextMenuSubContent.razor` - Migrated to FloatingPortal + hierarchical portals

**Documentation:**
- `docs/HIERARCHICAL_PORTALS.md` - Complete architecture documentation

---

## 2026-02-08 - Two-Layer Portal Architecture: Categorized Hosts + Hierarchical Scopes

### 🎯 Core Features

#### **NumericInput Component Enhancements**

**Added Min/Max validation with automatic value clamping:**

**New Features:**
- Added `MaxLength` parameter for character limit enforcement on number inputs
- Implemented automatic Min/Max value clamping on blur for both UpdateOn modes
- Added type-safe clamping support for all numeric types (int, decimal, double, float, long, short)
- DOM value now updates immediately when clamping occurs (no stale values)

**Technical Details:**
- `ClampToRange()`: Type-safe method that clamps values to Min/Max bounds
- `ValidateAndClamp()`: JS-invokable method for UpdateOn=Input mode blur validation
- Calls `updateValue()` JS function to update DOM when value is clamped
- Works seamlessly with ColorPicker RGB inputs (0-255 range enforcement)

**Behavior:**
- **UpdateOn=Change**: Clamps on blur via `OnInputChanged()`
- **UpdateOn=Input**: Defers clamping to blur via `ValidateAndClamp()` to avoid interrupting typing
- **MaxLength**: Enforced in real-time during typing (blocks 4th character, etc.)

---

### 🔧 JavaScript Infrastructure Improvements (input.js)

**Critical bug fixes and architectural improvements:**

**1. Fixed Race Condition - Dual Blur Handlers**
- **Problem**: UpdateOn=Input + Min/Max created two competing blur handlers
- **Solution**: Merged into single unified handler with `enableBlurValidation` flag
- **Impact**: Eliminated race conditions, guaranteed execution order

**2. Fixed Event Propagation - MaxLength Enforcement**
- **Problem**: `stopPropagation()` blocked other input handlers (validation, etc.)
- **Solution**: Use capture phase + dispatch new event after truncation
- **Impact**: MaxLength now runs first, other handlers see truncated value

**3. Fixed Memory Leak - Debounce Timer**
- **Problem**: Timer could invoke disposed DotNetObjectReference
- **Solution**: Check state exists before invoking in timer callback
- **Impact**: No more errors after rapid disposal

**4. Added Consistent Error Handling**
- **New**: `safeInvoke()` helper for all JS interop calls
- **Handles**: Disposed objects gracefully, logs real errors, ignores disposal errors
- **Impact**: Robust error handling across all input components

**5. Fixed Validation Cleanup**
- **Problem**: `customValidity` persisted after disposal
- **Solution**: Clear `setCustomValidity('')` in `disposeValidation()`
- **Impact**: Clean disposal, no validation state leaks

**6. Removed Duplicate Functions**
- Removed separate `initializeBlurValidation()` / `disposeBlurValidation()`
- Merged blur validation into main `initializeInput()` flow
- Cleaner API, less code duplication

---

### 🏗️ Architecture - MenubarContent Refactoring

**Migrated to FloatingPortal for positioning infrastructure:**

**Changes:**
- Replaced manual positioning logic with `FloatingPortal` component
- Re-established cascading values (MenubarContext, MenubarMenuContext) inside portal
- Removed ~150 lines of duplicate positioning code
- Now consistent with PopoverContent, DropdownMenuContent, SelectContent

**Benefits:**
- ✅ Centralized positioning logic (one source of truth)
- ✅ Automatic portal rendering to document body
- ✅ No z-index stacking issues
- ✅ Easier maintenance (positioning bugs fixed in one place)
- ✅ Follows established component pattern

---

### 🚀 Impact Summary

**Stability:**
- ✅ Fixed 3 critical bugs (race condition, event blocking, memory leak)
- ✅ Added comprehensive error handling via `safeInvoke()`
- ✅ Eliminated code duplication (150+ lines removed)

**Functionality:**
- ✅ NumericInput now enforces Min/Max/MaxLength correctly
- ✅ Works across all UpdateOn modes
- ✅ Immediate DOM feedback on clamping

**Compatibility:**
- ✅ No breaking changes to existing components
- ✅ Input, CurrencyInput, MaskedInput unaffected
- ✅ Backward compatible JS API (optional parameters)

**Code Quality:**
- ✅ Consistent patterns across floating components
- ✅ Type-safe numeric clamping
- ✅ Production-ready error handling

---

### 🎨 Code Quality Improvements

#### **Standardized CSS Class Merging Across All Components**

**Migrated all components to use `ClassNames.cn()` for CSS class merging:**

**Changes:**
- Updated 14 component files to replace non-standard class merging patterns
- Added `@using NeoUI.Blazor.Utilities` directive to all affected components
- Grouped CSS classes by intent/purpose following Input component pattern
- Removed legacy patterns: `StringBuilder`, `List<string>`, manual string concatenation

**Components Updated:**
1. **ContextMenu** (3 files):
   - ContextMenuSubContent.razor
   - ContextMenuContent.razor
   - ContextMenuLabel.razor

2. **Toast** (2 files):
   - ToastClose.razor
   - ToastAction.razor

3. **Accordion** (2 files):
   - Accordion.razor
   - AccordionItem.razor

4. **NavigationMenu** (2 files):
   - NavigationMenuIndicator.razor
   - NavigationMenuList.razor

5. **Other Components** (5 files):
   - CarouselItem.razor
   - Skeleton.razor.cs
   - TooltipContent.razor
   - DialogDescription.razor
   - AspectRatio.razor

**Old Patterns Replaced:**
```csharp
// ❌ String interpolation with Trim()
$"{Class}".Trim()
$"border-b {Class}".Trim()

// ❌ Ternary operators with concatenation
string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}"

// ❌ StringBuilder for building classes
var builder = new StringBuilder();
builder.Append("animate-pulse bg-muted ");
builder.Append(Class);
return builder.ToString().Trim();

// ❌ List with string.Join
var classes = new List<string> { "text-sm", "text-muted-foreground" };
if (!string.IsNullOrWhiteSpace(Class)) classes.Add(Class);
return string.Join(" ", classes);
```

**NeoBlazorUI Standard Pattern:**
```csharp
// ✅ ClassNames.cn() with grouped classes by intent
ClassNames.cn(
    // Base styles
    "px-2 py-1.5 text-sm font-semibold text-foreground",
    // Interaction states
    "hover:text-foreground focus:opacity-100",
    // Custom classes
    Class
)
```

**Benefits:**
- ✅ Consistent code style across entire codebase
- ✅ Automatic null/empty string handling
- ✅ Tailwind CSS conflict resolution via TailwindMerge
- ✅ Better readability with intent-based grouping
- ✅ Easier maintenance and debugging
- ✅ Reduced code duplication

**Pattern Details:**
- Classes grouped by purpose: "Base styles", "Animation states", "Interaction states", etc.
- Comments indicate intent of each group
- Related classes on same line for better readability
- Follows established pattern from Input component

---


## 2026-02-07 - Navigation Enhancement, Component Styling Improvements & Demo Standardization

### 🎨 UI/UX Enhancements & Demo Improvements

**Status:** ✅ Complete, Production Ready  
**Impact:** Enhanced navigation structure, improved component styling consistency, and standardized demo documentation across DataGrid examples.

---

### 🎯 1. Enhanced Navigation Structure

#### **Chart and Grid Root-Level Navigation**

**Added dedicated navigation sections for Chart and DataGrid components:**

**Changes:**
- Added Chart section with collapsible submenu in MainLayout
  - Area Chart, Bar Chart, Line Chart, Pie Chart, Scatter Chart, Radar Chart, Composed Chart
- Added DataGrid section with collapsible submenu in MainLayout
  - Basic, Templating, Selection, Transactions, Sorting & Filtering, State, Server-Side, Advanced, Theming
- Replaced all custom SVG icons with LucideIcon components for consistency
- Fixed Chart and DataGrid routing - updated hrefs from hash fragments to proper routes

**Benefits:**
- ✅ Chart and DataGrid components get extra visibility with dedicated navigation
- ✅ Direct access to specific chart types and grid features
- ✅ Consistent icon system throughout navigation
- ✅ Proper Blazor routing with bookmarkable URLs
- ✅ Better SEO with individual page routes

---

### 🎯 2. Component Styling Enhancements

#### **SelectTrigger and MultiSelect Styling Improvements**

**Enhanced focus, outline, and transitions to match native select behavior:**

**SelectTrigger Improvements:**
- Added smooth 200ms color transitions (`transition-colors`)
- Enhanced hover states (`hover:bg-accent hover:text-accent-foreground`)
- Improved focus states with both `focus:` and `focus-visible:` for better accessibility
- Added open state ring indicator (`data-[state=open]:ring-2`)
- Disabled state protection (`disabled:hover:bg-background`)

**MultiSelect Improvements:**
- Applied same styling enhancements as SelectTrigger
- Added cache tracking for open state to optimize performance
- Dynamic ring styling when dropdown is expanded
- Adjusted popover max height

**Benefits:**
- ✅ Native-like user experience with smooth transitions
- ✅ Better accessibility with proper focus indicators
- ✅ Visual feedback for open/closed states
- ✅ Consistent styling across select components

---

### 🎯 3. CommandInput AutoFocus Enhancement

#### **JavaScript-Powered Auto-Focus Implementation**

**Enhanced CommandInput to properly handle auto-focus via JavaScript:**

**Changes:**
- Added `autoFocus` parameter to `initializeCommandInput` JavaScript function
- Implemented `focusCommandInput` utility function
- Updated `FocusAsync()` method to use JavaScript module
- Proper focus handling for initial render and dynamic focus changes

**Benefits:**
- ✅ Reliable auto-focus on component initialization
- ✅ Programmatic focus control via `FocusAsync()`
- ✅ Works correctly in popovers and dialogs
- ✅ Uses `requestAnimationFrame` for DOM readiness

---

### 🎯 4. Chart Demo Improvements

#### **DisplayTextSelector Pattern Implementation**

**Implemented data-driven select pattern in PieChartExamples:**

**Changes:**
- Added `monthDescriptions` dictionary for month display text
- Implemented `DisplayTextSelector` for proper value-to-text mapping
- Refactored description text to use dictionary lookup with `TryGetValue`
- Dynamic `SelectItem` generation using `@foreach`

**Files Changed:**
```
demo/BlazorUI.Demo.Shared/Pages/Components/Charts/PieChartExamples.razor
```

**Benefits:**
- ✅ Maintainable - Easy to add more months
- ✅ Type-safe dictionary lookup
- ✅ Consistent with AreaChartExamples pattern
- ✅ Clean, DRY code

---

### 🎯 5. Grid Demo Documentation Standardization

#### **Standardized Information Boxes with Alert Component**

**Replaced all custom info boxes with standardized Alert components across DataGrid demos:**

**Alert Variants Used:**
- `Info` - Instructional/informational content (default for most boxes)
- `Warning` - Important notices (AG Grid Enterprise license)
- `Muted` - Subtle information (TMDb API key input)

---

### 📊 Summary

**Components Enhanced:** 4 (SelectTrigger, MultiSelect, CommandInput, PieChartExamples)  
**Navigation Improvements:** Chart and DataGrid root-level sections with 16 new navigation links  
**Demo Pages Standardized:** 3 DataGrid demo pages with 11 Alert components  
**Icons Standardized:** All navigation icons now use LucideIcon  
**Routing Fixed:** 13 chart/grid pages with proper @page directives  

**Impact:**
- Enhanced user experience with smoother transitions and better visual feedback
- Improved navigation structure for easier component discovery
- Consistent, professional documentation across DataGrid examples
- Better accessibility and maintainability

---



## 2026-02-06 - Input Component UpdateOn Behavior, EffectiveId Pattern & Performance Optimization

### 🚀 Performance Improvements & Architecture Enhancements

**Status:** ✅ Complete, Production Ready  
**Impact:** Major improvements to all input components with better performance, UX, and reliability. Auto-generated IDs eliminate null reference issues.

---

### 🎯 1. Input Components UpdateOn Default Behavior Change

#### **Enhanced Default Performance Mode**

**Changed default behavior for all input components:**

**Components Updated:**
- `Input` - Text/email/password/number inputs
- `CurrencyInput` - Locale-aware currency formatting
- `MaskedInput` - Pattern-based masked input
- `NumericInput` - Type-safe numeric input
- `Textarea` - Multi-line text input

**What Changed:**
- **Default `UpdateOn` mode** changed from `Input` → `Change`
- **Updates on blur** instead of every keystroke
- **JavaScript-side validation tooltip management** for optimal performance

**Key Benefits:**
1. **Better Typing UX**
   - No interruptions while typing
   - Validation tooltips cleared automatically during input (when `UpdateOn="Change"`)
   - Tooltips only show after user completes input (on blur)

2. **Better Performance**
   - Fewer C# ↔ JS interop calls (critical for WebAssembly)
   - Reduced re-renders in parent components
   - Value updates once on blur instead of every keystroke

3. **Blazor WebAssembly Optimized**
   - Minimizes costly interop overhead
   - Client-side interactivity remains responsive
   - Ideal for WASM deployment scenarios

**Files Changed:**
```
src/BlazorUI.Components/Components/Input/Input.razor.cs
src/BlazorUI.Components/Components/CurrencyInput/CurrencyInput.razor.cs
src/BlazorUI.Components/Components/MaskedInput/MaskedInput.razor.cs
src/BlazorUI.Components/Components/NumericInput/NumericInput.razor.cs
src/BlazorUI.Components/Components/Textarea/Textarea.razor.cs
```

**Breaking Change:** ⚠️ Minor
- Previous default: `UpdateOn="Input"` (immediate updates)
- New default: `UpdateOn="Change"` (update on blur)
- **Migration:** Explicitly set `UpdateOn="Input"` if you need real-time updates

**Example:**
```razor
<!-- Default behavior (recommended) -->
<Input @bind-Value="username" />
<!-- Updates on blur -->

<!-- Real-time updates (when needed) -->
<Input @bind-Value="searchQuery" UpdateOn="InputUpdateMode.Input" />
<!-- Updates on every keystroke -->
```

---

### 🆔 2. EffectiveId Pattern - Auto-Generated IDs

#### **Eliminated Null ID Issues with Smart ID Generation**

**Problem Solved:**
- Previously, components required explicit `Id` parameter for JavaScript functionality
- Null IDs caused JavaScript interop failures
- `ElementReference` approach had timing issues

**Solution Implemented:**
All input components now use the **EffectiveId pattern**:
- Auto-generates unique IDs if not provided: `{component-type}-{6-chars}`
- Falls back to user-provided `Id` if specified
- Ensures JavaScript can always reference elements reliably

**Components Updated:**
1. **Input** → Generates `input-a3f7c2` (example)
2. **CurrencyInput** → Generates `currency-input-b8d4e1`
3. **MaskedInput** → Generates `masked-input-2c8f96`
4. **NumericInput** → Generates `numeric-input-9e5a3b`
5. **Textarea** → Generates `textarea-f1c9d7`

**Technical Implementation:**
```csharp
private string? _generatedId;

private string EffectiveId
{
    get
    {
        if (!string.IsNullOrEmpty(Id))
            return Id;

        if (_generatedId == null)
        {
            _generatedId = "input-" + Guid.NewGuid().ToString("N")[..6];
        }

        return _generatedId;
    }
}
```

**Benefits:**
- ✅ No null reference exceptions in JavaScript
- ✅ Works without explicit `Id` parameter
- ✅ Labels can still associate via `for` attribute
- ✅ Consistent behavior across all input components
- ✅ Backward compatible (user IDs take precedence)

**Files Changed:**
```
src/BlazorUI.Components/Components/Input/Input.razor.cs
src/BlazorUI.Components/Components/Input/Input.razor
src/BlazorUI.Components/Components/CurrencyInput/CurrencyInput.razor.cs
src/BlazorUI.Components/Components/CurrencyInput/CurrencyInput.razor
src/BlazorUI.Components/Components/MaskedInput/MaskedInput.razor.cs
src/BlazorUI.Components/Components/MaskedInput/MaskedInput.razor
src/BlazorUI.Components/Components/NumericInput/NumericInput.razor.cs
src/BlazorUI.Components/Components/NumericInput/NumericInput.razor
src/BlazorUI.Components/Components/Textarea/Textarea.razor.cs
src/BlazorUI.Components/Components/Textarea/Textarea.razor
```

---

### 🗂️ 3. JavaScript Module Consolidation

#### **Unified Validation in input.js**

**Changed:**
- Removed separate `input-validation.js` file
- Consolidated all validation functions into `input.js`
- Components now reuse the same module instance

**Before:**
```javascript
// Two separate imports
_inputModule = await import("input.js");
_validationModule = await import("input-validation.js");
```

**After:**
```javascript
// Single import, reused for validation
_inputModule = await import("input.js");
_validationModule = _inputModule; // Reuse same module
```

**Benefits:**
- ✅ Fewer HTTP requests (better performance)
- ✅ Reduced JavaScript bundle size
- ✅ Simpler architecture
- ✅ Consistent API across all components

**Files Changed:**
```
src/BlazorUI.Components/wwwroot/js/input.js (consolidated)
src/BlazorUI.Components/Components/CurrencyInput/CurrencyInput.razor.cs
src/BlazorUI.Components/Components/MaskedInput/MaskedInput.razor.cs
src/BlazorUI.Components/Components/NumericInput/NumericInput.razor.cs
```

---

### ⚡ 4. JavaScript-First Event Architecture - Blazing-Fast Performance

#### **Eliminated Blazor Event Handler Overhead**

**Major Architectural Shift:**
All input components now handle `oninput` and `onchange` events **entirely in JavaScript** instead of Blazor's `@oninput` and `@onchange` directives.

**Problem with Blazor Event Handlers:**
```razor
<!-- Old approach: Every keystroke triggers C# interop -->
<input @oninput="HandleInput" @onchange="HandleChange" />
```
- Every keystroke → SignalR/WebSocket call → C# method → StateHasChanged → Re-render
- **Blazing-fast in WebAssembly** (no network), **sluggish in Server mode** (network round-trip)
- **Auto mode** suffered from the worst-case scenario (initial Server render lag)

**New JavaScript-First Approach:**
```razor
<!-- New approach: JS handles events, calls C# only when needed -->
<input id="@EffectiveId" value="@Value" />
```
```javascript
// JavaScript manages all DOM events
element.addEventListener('input', (e) => {
    if (updateOn === 'input' && debounceDelay > 0) {
        // Debounced call to C#
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => 
            dotNetRef.invokeMethodAsync('OnInputChanged', e.target.value),
            debounceDelay
        );
    } else if (updateOn === 'input') {
        // Immediate call to C#
        dotNetRef.invokeMethodAsync('OnInputChanged', e.target.value);
    }
    // updateOn === 'change': Do nothing, wait for blur
});

element.addEventListener('change', (e) => {
    if (updateOn === 'change') {
        dotNetRef.invokeMethodAsync('OnInputChanged', e.target.value);
    }
});
```

**Performance Benefits:**

**1. Blazor Server Mode:**
- **Before:** Every keystroke = SignalR message (~50-200ms latency)
- **After:** Typing handled locally in browser, single update on blur
- **Result:** Instant visual feedback, 20x faster perceived performance

**2. Blazor WebAssembly Mode:**
- **Before:** Already fast (no network), but still C# → JS → C# overhead
- **After:** Even faster with direct JavaScript event handling
- **Result:** Native-like responsiveness

**3. Blazor Auto Mode:**
- **Before:** Suffered from Server mode lag during initial render
- **After:** JavaScript code works the same regardless of interactivity mode
- **Result:** Consistent blazing-fast performance everywhere

**4. UpdateOn="Change" (Default):**
- **Before:** 20 keystrokes = 20 C# calls + 20 re-renders
- **After:** 20 keystrokes = 0 C# calls, 1 call on blur
- **Result:** ~95% reduction in network traffic (Server) and CPU usage (all modes)

**5. UpdateOn="Input" with Debouncing:**
- **Before:** Not possible to debounce in C# (too late in the pipeline)
- **After:** JavaScript-side debouncing prevents rapid-fire C# calls
- **Result:** Real-time updates without overwhelming the server

**Technical Implementation:**

```csharp
// C# just provides the DotNetObjectReference
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        _inputModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoUI.Blazor/js/input.js");
        
        _dotNetRef = DotNetObjectReference.Create(this);
        
        // JavaScript takes over all event handling
        await _inputModule.InvokeVoidAsync(
            "initializeInput",
            EffectiveId,
            UpdateOn.ToString().ToLower(),
            DebounceDelay,
            _dotNetRef
        );
    }
}

// JavaScript calls this when value actually needs to update
[JSInvokable]
public async Task OnInputChanged(string? value)
{
    Value = value;
    await ValueChanged.InvokeAsync(value);
    // Only trigger EditContext validation when needed
}
```

**Components Using JavaScript-First Architecture:**
- ✅ **Input** - All input types (text, email, password, etc.)
- ✅ **CurrencyInput** - Locale-aware currency formatting
- ✅ **MaskedInput** - Pattern-based input masks
- ✅ **NumericInput** - Type-safe numeric input
- ✅ **Textarea** - Multi-line text input

**Why This Matters:**

**Interactivity Mode Comparison:**
```
Typing 20 characters in Blazor Server:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Before: 20 network calls (1000-4000ms total latency)
After:  1 network call on blur (50-200ms latency)
Result: Feels instant instead of laggy
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

**Benefits:**
- ✅ **Blazing-fast performance** in all interactivity modes (Server, WASM, Auto)
- ✅ **Zero network overhead** during typing (Server mode)
- ✅ **Native-like responsiveness** regardless of connection quality
- ✅ **Battery efficient** on mobile devices (fewer JavaScript ↔ C# calls)
- ✅ **Scalable** - server handles fewer requests
- ✅ **Future-proof** - architecture works great for Blazor United scenarios

**Files Changed:**
```
src/BlazorUI.Components/wwwroot/js/input.js (event handling logic)
src/BlazorUI.Components/Components/Input/Input.razor (removed @oninput/@onchange)
src/BlazorUI.Components/Components/CurrencyInput/CurrencyInput.razor
src/BlazorUI.Components/Components/MaskedInput/MaskedInput.razor
src/BlazorUI.Components/Components/NumericInput/NumericInput.razor
src/BlazorUI.Components/Components/Textarea/Textarea.razor
```

---

### 🎨 5. MaskedInput Blur Event Fix

**Fixed Issue:** `onblur` event not triggering when `UpdateOn="Change"`

**Root Cause:**
- `lastRawValue` was being updated during typing
- On blur, `currentRaw == lastRawValue`, so no Blazor callback

**Solution:**
- Only update `lastRawValue` when actually notifying Blazor
- For `UpdateOn="Change"`: Don't update during typing, update on blur
- For `UpdateOn="Input"`: Update immediately on every change

**Files Changed:**
```javascript
src/BlazorUI.Components/wwwroot/js/masked-input.js
```

---

### 📖 6. Documentation Enhancements

**Added comprehensive UpdateOn behavior documentation to demo pages:**

**Demo Pages Updated:**
- `InputDemo.razor` - General text input examples
- `CurrencyInputDemo.razor` - Currency-specific guidance  
- `MaskedInputDemo.razor` - Masked input patterns
- `NumericInputDemo.razor` - Numeric validation examples

**Each demo now includes:**
- ✅ Prominent info alert explaining `UpdateOn` behavior
- ✅ Collapsible "Read more" section with:
  - Benefits of `UpdateOn="Change"` (default)
  - Comparison of both modes
  - Context-specific use case examples
  - WebAssembly performance notes
- ✅ Lucide icons for visual consistency
- ✅ Smooth chevron rotation animation on expand/collapse

**Example Info Alert Structure:**
```razor
<Alert Variant="AlertVariant.Info">
    <AlertTitle>Optimized for Performance & Best Typing Experience</AlertTitle>
    <AlertDescription>
        By default uses UpdateOn="Change" for better performance...
        <Collapsible>
            <CollapsibleTrigger>Read more ˅</CollapsibleTrigger>
            <CollapsibleContent>
                <!-- Detailed benefits, modes, tips -->
                💡 Tip: Use UpdateOn="Input" only when you need real-time 
                updates, or if you're targeting WebAssembly mode where 
                interactivity will be fully handled in client-side.
            </CollapsibleContent>
        </Collapsible>
    </AlertDescription>
</Alert>
```

**Files Changed:**
```
demo/BlazorUI.Demo.Shared/Pages/Components/InputDemo.razor
demo/BlazorUI.Demo.Shared/Pages/Components/CurrencyInputDemo.razor
demo/BlazorUI.Demo.Shared/Pages/Components/MaskedInputDemo.razor
demo/BlazorUI.Demo.Shared/Pages/Components/NumericInputDemo.razor
```

---

### 🎮 7. CommandInput Performance Optimization

#### **Integrated Input Component with JS-First Keyboard Navigation**

**Problem:**
CommandInput was re-implementing input handling logic:
- Plain `<input>` with `@oninput` (Blazor event handler)
- Manual debouncing with `System.Threading.Timer`
- `@onkeydown` causing Input component re-renders
- Duplicate code vs Input component

**Solution:**
Replaced plain input with our optimized Input component + JavaScript keyboard interception:

```razor
<!-- Before: Plain input with Blazor events -->
<input @oninput="HandleInput" @onkeydown="HandleKeyDown" />

<!-- After: Input component with JS navigation -->
<Input @bind-Value="searchQuery" 
       UpdateOn="InputUpdateMode.Input"
       DebounceDelay="SearchInterval"
       AdditionalAttributes="@InputAttributes" />
```

**JavaScript Keyboard Interception:**
```javascript
// input.js - New command input navigation
export function initializeCommandInput(elementId, dotNetRef) {
    const navigationKeys = ['ArrowDown', 'ArrowUp', 'Home', 'End', 'Enter'];
    
    element.addEventListener('keydown', (e) => {
        if (navigationKeys.includes(e.key)) {
            e.preventDefault(); // No scroll, no cursor move
            dotNetRef.invokeMethodAsync('HandleNavigationKey', e.key);
        }
        // All other keys: Input component handles normally
    }, { capture: true }); // Intercept before Input sees it
}
```

**Key Optimizations:**

1. **Zero Input Re-Renders**
   - JavaScript intercepts navigation keys before Input component
   - No parameter changes during keyboard navigation
   - Input component never re-renders from arrow key presses

2. **Zero C# Calls for Typing**
   - Regular keys (a-z, 0-9, etc.) → handled entirely in JavaScript
   - Only navigation keys → call C# for list navigation
   - Result: Blazing fast typing performance

3. **Reuses All Input Improvements**
   - ✅ JavaScript-first event handling
   - ✅ EffectiveId pattern (auto-generated IDs)
   - ✅ JavaScript debouncing (no more `System.Threading.Timer`)
   - ✅ Consistent architecture across all inputs

4. **Cleaner Code**
   - Removed ~30 lines of manual timer management
   - Removed cached dictionary pattern (no longer needed)
   - Single responsibility: CommandInput = wrapper, Input = editing

**Performance Impact:**
```
User types "search query" (12 characters):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Before: 12 @oninput C# calls + timer management
After:  JavaScript-only (zero C# calls during typing)
Result: Instant response regardless of interactivity mode
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

User presses ArrowDown (5 times):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Before: 5 @onkeydown C# calls + Input re-renders
After:  5 JS-intercepted calls (no Input re-renders)
Result: Smooth navigation without visual interruption
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

**Input Styling for Command Palette:**
```css
/* Transparent input with no visual chrome */
!border-0              /* No border */
!shadow-none           /* No shadow */
bg-transparent         /* Transparent background */
focus-visible:!ring-0  /* No focus ring */
/* Border-b comes from parent wrapper div */
```

**Benefits:**
- ✅ **Blazing-fast typing** - JavaScript-only event handling
- ✅ **Smooth navigation** - Zero Input re-renders during arrow key presses
- ✅ **Better architecture** - Reuses Input component instead of reimplementing
- ✅ **Less code** - Removed timer management and manual event handling
- ✅ **Consistent** - Same patterns as Input/Textarea/CurrencyInput/etc.

**Files Changed:**
```javascript
src/BlazorUI.Components/wwwroot/js/input.js
  - Added initializeCommandInput()
  - Added disposeCommandInput()
  - Command input keyboard state tracking

src/BlazorUI.Components/Components/Command/CommandInput.razor
  - Replaced <input> with <Input> component
  - Added JavaScript keyboard navigation integration
  - Removed manual timer and cached dictionary
  - IAsyncDisposable for proper cleanup
```

---

### 🛠️ Technical Implementation Details

#### **JavaScript-Side Validation Management**

**Enhanced `input.js` Module:**
```javascript
// Initialize with UpdateOn mode
export function initializeValidation(elementId, updateOn = 'input') {
    // Auto-clear tooltip on first keystroke (Change mode)
    // Prevents tooltip interference while typing
}

// Functions consolidated from input-validation.js:
- initializeValidation(elementId, updateOn)
- disposeValidation(elementId)  
- setValidationError(elementId, message)
- setValidationErrorSilent(elementId, message)
- clearValidationError(elementId)
```

**Flow for `UpdateOn="Change"`:**
1. User has validation error → tooltip shown
2. User starts typing → **JS auto-clears tooltip** (no C# call)
3. User tabs out → C# validates → new error/success shown

**Performance Impact:**
- Zero C# ↔ JS calls during typing (Change mode)
- Single event listener per input (efficient)
- Proper cleanup on dispose

---

### 📝 Code Quality & Testing

**Improvements:**
- ✅ Consistent behavior across all input components
- ✅ Optimized for WebAssembly deployment
- ✅ Better separation of concerns (JS handles UI, C# handles logic)
- ✅ Comprehensive inline documentation
- ✅ User-friendly demo documentation

**Test Scenarios Validated:**
1. ✅ `UpdateOn="Change"` - validates on blur
2. ✅ `UpdateOn="Input"` - validates on every keystroke  
3. ✅ Validation tooltip auto-clears during typing (Change mode)
4. ✅ MaskedInput blur event triggers correctly
5. ✅ Auto-generated IDs work without explicit Id parameter
6. ✅ No memory leaks (proper event listener cleanup)
7. ✅ WebAssembly performance (minimal interop)

---

### 🎯 Migration Guide

**For Existing Code:**

**1. UpdateOn Behavior:**
If you relied on immediate updates (old default behavior):
```razor
<!-- Before (implicit default) -->
<Input @bind-Value="searchTerm" />

<!-- After (explicit for real-time) -->
<Input @bind-Value="searchTerm" UpdateOn="InputUpdateMode.Input" />
```

**2. ID Parameter:**
No migration needed! Components auto-generate IDs when not specified.
```razor
<!-- Works without Id (auto-generates: input-a3f7c2) -->
<Input @bind-Value="username" />

<!-- Still works with explicit Id -->
<Input Id="my-input" @bind-Value="username" />
```

**Recommended for most cases (new default):**
```razor
<Input @bind-Value="email" />
<!-- Updates on blur - better UX and performance -->
```

---

### 📊 Performance Metrics

**WebAssembly Interop Reduction:**
- **Before (UpdateOn=Input):** N interop calls per input (N = keystrokes)
- **After (UpdateOn=Change):** 1 interop call per input (on blur)
- **Savings:** ~95% reduction in interop overhead for typical form filling

**Example Scenario:**
- User types 20 characters in a field
- **Old behavior:** 20 C# ↔ JS calls + 20 re-renders
- **New behavior:** 1 C# ↔ JS call + 1 re-render
- **Result:** Smoother typing, lower CPU usage, better battery life

- ### 🐛 Bug Fixes

**Status:** ✅ Complete, Production Ready  
**Impact:** Critical positioning fix and improved navigation UX

---

### 🔧 8. Floating UI Position Revert Fix (PR #114)

**Fixed Issue:** Floating UI positioned elements would revert to original position in Blazor Server interactive mode

**Root Cause:**
- Custom viewport constraint middleware was interfering with Floating UI's built-in positioning
- Position calculations were being overridden after Floating UI completed its work
- Issue only occurred in interactive Server mode due to timing of Blazor re-renders

**Solution:**
- Renamed middleware from `viewportConstraint` to `blazorViewportConstraint` to follow Floating UI naming conventions
- Refactored to use proper Floating UI middleware pattern
- Ensures compatibility with Floating UI's flip and shift middleware
- Position now persists correctly across all render modes

**Benefits:**
- ✅ Popovers, dropdowns, and tooltips stay in correct position
- ✅ Works reliably in Server, WebAssembly, and Auto modes
- ✅ No visual "jumping" or position reversion
- ✅ Proper integration with Floating UI architecture

**Files Changed:**
```
src/BlazorUI.Primitives/wwwroot/js/primitives/positioning.js
```

---

### 🎯 9. SidebarInset Scroll Reset on Navigation (PR #115)

**Added Feature:** Automatic scroll position reset when navigating between pages

**Problem:**
- When navigating from a scrolled page to a new page, scroll position persisted
- Users would land mid-page instead of at the top
- Common issue in SPA applications with independent scrolling areas

**Solution:**
- Added `ResetScrollOnNavigation` parameter to SidebarInset component (default: false)
- Integrates with Blazor's NavigationManager to detect route changes
- Automatically scrolls to top on navigation when enabled
- JavaScript helper function for smooth, reliable scroll reset

**Implementation:**
```razor
<!-- Enable scroll reset in MainLayout -->
<SidebarInset ResetScrollOnNavigation="true">
    @Body
</SidebarInset>
```

```csharp
// SidebarInset.razor.cs
[Parameter]
public bool ResetScrollOnNavigation { get; set; }

protected override void OnInitialized()
{
    if (ResetScrollOnNavigation && NavigationManager != null)
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }
}

private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
{
    await ResetScrollPositionAsync();
}
```

**Benefits:**
- ✅ Better UX - users always start at top of new pages
- ✅ Opt-in feature - doesn't affect existing layouts
- ✅ Works with all Sidebar variants (default, floating, inset)
- ✅ Smooth scroll behavior via JavaScript
- ✅ Proper cleanup (unsubscribes on dispose)

**Files Changed:**
```
src/BlazorUI.Components/Components/Sidebar/SidebarInset.razor
src/BlazorUI.Components/Components/Sidebar/SidebarInset.razor.cs
src/BlazorUI.Components/wwwroot/js/sidebar.js
demo/BlazorUI.Demo.Shared/Common/MainLayout.razor
```

---

### 📝 Related Improvements

**SidebarInset Component Enhancements** (from earlier commits):
- Independent scrolling for sidebar and content areas
- ScrollArea integration with auto-hide scrollbars
- Dynamic content height updates via ResizeObserver
- Production-ready scrolling behavior matching modern apps

**ScrollArea Enhancements:**
- Auto-hide behavior - scrollbar only visible when content overflows
- `data-state` attribute (visible/hidden) based on overflow detection
- ScrollBar hides completely when no overflow
- Responsive to dynamic content changes (collapsible menus, dynamic lists)

---

## 2026-02-05 - Input Components & Positioning Enhancements
src/BlazorUI.Components/Components/MaskedInput/MaskedInput.razor.cs (JS)
src/BlazorUI.Components/Components/NumericInput/NumericInput.razor.cs
src/BlazorUI.Components/wwwroot/js/input.js (new, renamed from input-validation.js)
src/BlazorUI.Components/wwwroot/js/masked-input.js
```

**Breaking Change:** ⚠️ Minor
- Previous default: `UpdateOn="Input"` (immediate updates)
- New default: `UpdateOn="Change"` (update on blur)
- **Migration:** Explicitly set `UpdateOn="Input"` if you need real-time updates

**Example:**
```razor
<!-- Default behavior (recommended) -->
<Input @bind-Value="username" />
<!-- Updates on blur -->

<!-- Real-time updates (when needed) -->
<Input @bind-Value="searchQuery" UpdateOn="InputUpdateMode.Input" />
<!-- Updates on every keystroke -->
```

---

### 🎨 MaskedInput Blur Event Fix

**Fixed Issue:** `onblur` event not triggering when `UpdateOn="Change"`

**Root Cause:**
- `lastRawValue` was being updated during typing
- On blur, `currentRaw == lastRawValue`, so no Blazor callback

**Solution:**
- Only update `lastRawValue` when actually notifying Blazor
- For `UpdateOn="Change"`: Don't update during typing, update on blur
- For `UpdateOn="Input"`: Update immediately on every change

**Files Changed:**
```javascript
src/BlazorUI.Components/wwwroot/js/masked-input.js
```

---

### 📖 Documentation Enhancements

**Added comprehensive UpdateOn behavior documentation:**

**Demo Pages Updated:**
- `InputDemo.razor` - Info alert with collapsible details
- `CurrencyInputDemo.razor` - Performance optimization tips
- `MaskedInputDemo.razor` - Masked input specific guidance
- `NumericInputDemo.razor` - Numeric validation examples

**Each demo includes:**
- ✅ Prominent info alert explaining `UpdateOn` behavior
- ✅ Collapsible "Read more" section with:
  - Benefits of `UpdateOn="Change"` (default)
  - Comparison of both modes
  - Context-specific use case examples
  - WebAssembly performance notes
- ✅ Lucide icons for visual consistency
- ✅ Smooth chevron rotation animation

**Example Info Alert Structure:**
```razor
<Alert Variant="AlertVariant.Info">
    <AlertTitle>Optimized for Performance & Best Typing Experience</AlertTitle>
    <AlertDescription>
        By default uses UpdateOn="Change" for better performance...
        <Collapsible>
            <CollapsibleTrigger>Read more ˅</CollapsibleTrigger>
            <CollapsibleContent>
                <!-- Detailed benefits, modes, tips -->
            </CollapsibleContent>
        </Collapsible>
    </AlertDescription>
</Alert>
```

---

### 🛠️ Technical Implementation

#### **JavaScript-Side Validation Management**

**New: `input.js` Module**
- Renamed from `input-validation.js` for broader scope
- Manages validation tooltips AND UpdateOn behavior
- Auto-clears tooltips on input when `UpdateOn="Change"`

**Key Functions:**
```javascript
// Initialize with UpdateOn mode
initializeValidation(elementId, updateOn = 'input')

// Auto-clear tooltip on first keystroke (Change mode)
// Prevents tooltip interference while typing

// Cleanup
disposeValidation(elementId)
```

**Flow for `UpdateOn="Change"`:**
1. User has validation error → tooltip shown
2. User starts typing → **JS auto-clears tooltip** (no C# call)
3. User tabs out → C# validates → new error/success shown

**Performance Impact:**
- Zero C# ↔ JS calls during typing (Change mode)
- Single event listener per input (efficient)
- Proper cleanup on dispose

---

### 📝 Code Quality

**Improvements:**
- ✅ Consistent behavior across all input components
- ✅ Optimized for WebAssembly deployment
- ✅ Better separation of concerns (JS handles UI, C# handles logic)
- ✅ Comprehensive inline documentation
- ✅ User-friendly demo documentation

**Test Scenarios Validated:**
1. ✅ `UpdateOn="Change"` - validates on blur
2. ✅ `UpdateOn="Input"` - validates on every keystroke  
3. ✅ Validation tooltip auto-clears during typing (Change mode)
4. ✅ MaskedInput blur event triggers correctly
5. ✅ No memory leaks (proper event listener cleanup)
6. ✅ WebAssembly performance (minimal interop)

---

### 🎯 Migration Guide

**For Existing Code:**

If you relied on immediate updates (old default behavior):
```razor
<!-- Before (implicit default) -->
<Input @bind-Value="searchTerm" />

<!-- After (explicit for real-time) -->
<Input @bind-Value="searchTerm" UpdateOn="InputUpdateMode.Input" />
```

**Recommended for most cases (new default):**
```razor
<Input @bind-Value="email" />
<!-- Updates on blur - better UX and performance -->
```

---

## 2026-02-05 - Input Components & Positioning Enhancements

### 🎯 New Components & Major Enhancements

**Status:** ✅ Complete, Production Ready  
**Impact:** 8 new components (TimePicker, DateRangePicker, plus 6 specialized inputs) and industry-standard popover positioning that prevents viewport clipping.

#### 📊 Session Statistics
- **8 new components** (TimePicker, DateRangePicker, ColorPicker, CurrencyInput, Drawer, MaskedInput, NumericInput, RangeSlider, Rating)
- **3 components enhanced** (NativeSelect styling, Popover positioning, DataTableToolbar accessibility)
- **2 new enums** (TimePeriod, DateRangePreset)
- **1 JavaScript enhancement** (popover viewport boundary detection)
- **100% accessibility** maintained with ARIA labels and keyboard navigation
- **9 components added** to all navigation indexes (Components Index, Spotlight Search, MainLayout sidebar)

---

### 🆕 New Components

#### **1. TimePicker Component**
**Location:** `src\BlazorUI.Components\Components\TimePicker\`

**Description:** Time selection component with hour/minute dropdowns and optional AM/PM toggle.

**Features:**
- **Native selects** for hour and minute (uses NativeSelect component)
- **12-hour or 24-hour format** with `Use24HourFormat` parameter
- **Flexible binding:**
  - `@bind-Value` (TimeOnly?) - two-way binding
  - `@bind-Hour`, `@bind-Minute`, `@bind-Period` - individual component binding
- **Customizable:**
  - Size variants (Small, Default, Large)
  - Hour/minute step increments
  - Placeholder text
  - Disabled state
  - CSS classes

**Example Usage:**
```razor
<!-- Simple binding -->
<TimePicker @bind-Value="meetingTime" />

<!-- 24-hour format -->
<TimePicker @bind-Value="departureTime" Use24HourFormat="true" />

<!-- In a form field -->
<Field>
    <FieldLabel>Appointment Time</FieldLabel>
    <TimePicker @bind-Value="appointmentTime" Size="NativeSelectSize.Large" />
    <FieldDescription>Select your preferred time slot</FieldDescription>
</Field>
```

**Files Added:**
- `TimePicker.razor` - Main component
- `TimePicker.razor.cs` - Component logic with time conversion
- `TimePeriod.cs` - AM/PM enum
- `TimePickerDemo.razor` - Comprehensive demo page

---

#### **2. DateRangePicker Component**
**Location:** `src\BlazorUI.Components\Components\DateRangePicker\`

**Description:** Date range selection with preset date ranges and two-calendar view.

**Features:**
- **Two side-by-side calendars** for intuitive range selection
- **Preset date ranges** (Today, Yesterday, Last 7 Days, Last 30 Days, This Month, Last Month, This Year)
- **Show/Apply button system** for confirmed selections
- **Clear button** to reset selection
- **Day counter** showing selected range
- **Calendar synchronization** - calendars stay one month apart

**Example Usage:**
```razor
<!-- With presets and confirmation buttons -->
<DateRangePicker @bind-StartDate="start" 
                 @bind-EndDate="end"
                 ShowPresets="true"
                 ShowButtons="true" />

<!-- Auto-close on selection -->
<DateRangePicker @bind-StartDate="checkIn"
                 @bind-EndDate="checkOut"
                 ShowButtons="false"
                 StartDateLabel="Check-in"
                 EndDateLabel="Check-out" />
```

**Files Added:**
- `DateRangePicker.razor` - Main component (complete rewrite)
- `DateRangePreset.cs` - Preset types enum
- `DateRangePickerDemo.razor` - Updated demo

---

#### **3. Color Picker Component**
**Location:** `src\BlazorUI.Components\Components\ColorPicker\`

**Description:** Color selection with hex, RGB, and HSL support.

**Features:**
- Multiple color format support
- Visual color picker interface
- Real-time color preview

**Files Added:**
- ColorPicker component files

---

#### **4. Currency Input Component**
**Location:** `src\BlazorUI.Components\Components\CurrencyInput\`

**Description:** Formatted currency input with locale support.

**Features:**
- Automatic currency formatting
- Locale-aware display
- Decimal precision control

**Files Added:**
- CurrencyInput component files

---

#### **5. Drawer Component**
**Location:** `src\BlazorUI.Components\Components\Drawer\`

**Description:** Slide-out panel with gesture controls and backdrop.

**Features:**
- Touch gesture support
- Multiple slide directions
- Backdrop overlay

**Files Added:**
- Drawer component files

---

#### **6. Masked Input Component**
**Location:** `src\BlazorUI.Components\Components\MaskedInput\`

**Description:** Text input with customizable format masks (phone, date, etc.).

**Features:**
- Predefined and custom masks
- Format enforcement
- Input validation

**Files Added:**
- MaskedInput component files

---

#### **7. Numeric Input Component**
**Location:** `src\BlazorUI.Components\Components\NumericInput\`

**Description:** Number input with increment/decrement buttons and formatting.

**Features:**
- Spinner buttons
- Min/max constraints
- Step increments
- Number formatting

**Files Added:**
- NumericInput component files

---

#### **8. Range Slider Component**
**Location:** `src\BlazorUI.Components\Components\RangeSlider\`

**Description:** Dual-handle slider for selecting value ranges.

**Features:**
- Two handles for range selection
- Visual range highlight
- Min/max constraints
- Step increments

**Files Added:**
- RangeSlider component files

---

#### **9. Rating Component**
**Location:** `src\BlazorUI.Components\Components\Rating\`

**Description:** Star rating input with half-star precision and readonly mode.

**Features:**
- Interactive star rating
- Half-star support
- Readonly display mode
- Customizable star count

**Files Added:**
- Rating component files

---

### 🔄 Enhanced Components

#### **NativeSelect - Enhanced Styling**
**Location:** `src\BlazorUI.Components\Components\NativeSelect\`

**Changes:**

**a) Fixed Focus Styles**
```css
/* Before: Focus showed ring but was hard to see */
focus:ring-2 focus:ring-ring

/* After: Better visibility with subtle border change */
focus-visible:border-ring focus-visible:ring-[2px] focus-visible:ring-ring/50
```

**b) Added Chevron-Down Icon**
```razor
<!-- Before: No dropdown indicator -->
<select>...</select>

<!-- After: Lucide icon positioned on right -->
<div class="relative">
    <select>...</select>
    <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-2">
        <LucideIcon Name="chevron-down" 
                    Class="h-4 w-4 text-muted-foreground @(Disabled ? "opacity-50" : "")" />
    </div>
</div>
```

**Features:**
- Icon matches disabled state (50% opacity when disabled)
- Positioned absolutely to not interfere with select
- Uses Lucide icon for consistency with rest of library
- `appearance-none` on select removes native arrow

**Files Modified:**
- `NativeSelect.razor` - Added wrapper div and Lucide icon

---

#### **DataTableToolbar - Accessibility Enhancement**
**Location:** `src\BlazorUI.Components\Components\DataTable\DataTableToolbar.razor`

**Changes:**

**Improved Column Toggle UI**
```razor
<!-- Before: Manual click handling on span -->
<div>
    <Checkbox Checked="@column.Visible" 
              CheckedChanged="@((bool isChecked) => OnColumnVisibilityChanged?.Invoke(column.Id, isChecked))" />
    <span @onclick="@(() => OnColumnVisibilityChanged?.Invoke(column.Id, !column.Visible))">
        @column.Header
    </span>
</div>

<!-- After: Proper label-checkbox association -->
<div>
    <Checkbox Id="@checkboxId" 
              Checked="@column.Visible"
              CheckedChanged="@((bool isChecked) => OnColumnVisibilityChanged?.Invoke(column.Id, isChecked))" />
    <FieldLabel For="@checkboxId" Class="text-sm flex-1 cursor-pointer">
        @column.Header
    </FieldLabel>
</div>
```

**Features:**
- Proper HTML `<label for="id">` association with checkboxes
- Unique IDs generated per column (`column-toggle-{column.Id}`)
- Automatic checkbox toggling via native browser behavior
- Better screen reader support
- Full-width labels with `flex-1` class
- Removed redundant click handlers

**Files Modified:**
- `DataTableToolbar.razor` - Replaced span with FieldLabel, added checkbox IDs

---

### 🎨 Popover Positioning - Viewport Boundary Detection

**Problem:** Large popovers (like DateRangePicker with presets) were getting clipped when positioned near viewport edges.

**Solution:** Enhanced JavaScript positioning with post-processing viewport constraints.

**Implementation:**
```javascript
// positioning.js - applyPosition() enhancement

// AFTER Floating UI positioning (flip + shift have done their work)
const rect = floating.getBoundingClientRect();
const exceedsBottom = rect.bottom > viewportHeight - padding;

if (exceedsBottom) {
    if (rect.height > maxHeight) {
        // Last resort: Add scrollbar
        floating.style.maxHeight = `${maxHeight}px`;
        floating.style.overflowY = 'auto';
    } else {
        // Content fits! Just reposition it
        const newTop = viewportHeight - rect.height - padding;
        floating.style.top = `${Math.max(padding, newTop)}px`;
    }
}
```

**Positioning Flow:**
1. **Floating UI middleware** runs first:
   - `offset` - Add spacing (8px default)
   - `flip` - Try opposite side if not enough space
   - `shift` - Shift along axis to maximize space
   
2. **Post-processing** runs after:
   - Check if positioned element exceeds viewport
   - If content fits: Reposition to make fully visible
   - If content too large: Add scrollbar (last resort)

**Before:**
```
Viewport Edge
─────────────────
[Trigger Button]
┌─────────────┐
│  Popover    │ ← Clipped!
│  Content    │
══════════════════ ← Bottom clips content
```

**After:**
```
Viewport Edge
─────────────────
[Trigger Button] ← May overlap
┌─────────────┐
│  Popover    │ ← Repositioned
│  Content    │ ← Fully visible!
└─────────────┘
══════════════════ ← Fits within padding
```

**Benefits:**
- ✅ Popovers never clipped by viewport
- ✅ Repositioning prioritized over scrollbars
- ✅ Industry-standard behavior (matches popular UI libraries)
- ✅ Works with all popover-based components (DateRangePicker, Select, DropdownMenu, etc.)

**Files Modified:**
- `positioning.js` - Added viewport boundary check in `applyPosition()`

---

### 📝 Documentation Updates

**Index Pages Updated:**
- `Components/Index.razor` - Added 9 new components (ColorPicker, CurrencyInput, DateRangePicker, Drawer, FileUpload, MaskedInput, NumericInput, RangeSlider, Rating)
- `SpotlightCommandPalette.razor` - Added all 9 components to search
- `MainLayout.razor` - Added all 9 components to sidebar navigation

**Demo Pages Added/Updated:**
- `TimePickerDemo.razor` - Comprehensive TimePicker examples
- `DateRangePickerDemo.razor` - Updated with new preset and button features
- `NativeSelectDemo.razor` - Updated to show improved styling

**README Updates:**
- Main `README.md` - Updated component count to 85+, added all new components to lists
- `src\BlazorUI.Components\README.md` - Updated to 85+ components with full descriptions
- `SESSION_SUMMARY.md` - Complete session documentation with all changes

---

### 🎯 API Changes

#### **TimePicker (New)**
```razor
<TimePicker @bind-Value="time"
            Use24HourFormat="bool"
            HourStep="int"
            MinuteStep="int"
            Size="NativeSelectSize"
            Placeholder="string?"
            Disabled="bool"
            Class="string?" />

<!-- Or bind individual components -->
<TimePicker @bind-Hour="hour"
            @bind-Minute="minute"
            @bind-Period="period" />
```

#### **DateRangePicker (Enhanced)**
```razor
<DateRangePicker @bind-StartDate="start"
                 @bind-EndDate="end"
                 ShowPresets="bool"           <!-- NEW: Preset sidebar -->
                 ShowButtons="bool"            <!-- NEW: Clear/Apply buttons (default: true) -->
                 StartDateLabel="string?"      <!-- Existing -->
                 EndDateLabel="string?"        <!-- Existing -->
                 MinDate="DateOnly?"
                 MaxDate="DateOnly?"
                 CaptionLayout="CalendarCaptionLayout"
                 ButtonVariant="ButtonVariant"
                 ButtonSize="ButtonSize" />
```

**Breaking Change:** Namespace changed from `NeoUI.Blazor.DatePicker` to `NeoUI.Blazor.DateRangePicker`
```razor
@* Before *@
@using NeoUI.Blazor.DatePicker

@* After *@
@using NeoUI.Blazor.DateRangePicker
```

---

### ✅ Testing Summary

**Components Tested:**
- ✅ TimePicker - 12/24 hour formats, all size variants, form integration
- ✅ DateRangePicker - Presets, Show/Apply buttons, date constraints
- ✅ NativeSelect - Focus styles, disabled state, chevron icon
- ✅ Popover - Viewport clipping prevention at all edges

**Browsers Tested:**
- ✅ Chrome/Edge (Chromium)
- ✅ Firefox
- ✅ Safari (WebKit)

**Accessibility:**
- ✅ Keyboard navigation (Tab, Arrow keys)
- ✅ Screen readers (ARIA labels)
- ✅ Focus indicators
- ✅ Disabled states

---

## 2026-02-04 - Menu System Overhaul & Portal Infrastructure Improvements

### 🎯 Major Menu System Enhancements

**Status:** ✅ Complete, Production Ready  
**Impact:** All menu components (DropdownMenu, Menubar, ContextMenu) now have reliable focus management, proper z-index stacking, and seamless keyboard navigation.

#### 📊 Session Statistics
- **14 files modified** across menu primitives and portal infrastructure
- **3 menu systems enhanced** (DropdownMenu, Menubar, ContextMenu) with consistent patterns
- **JavaScript focus module** (`focusElement`) added for reliable focus timing
- **Depth-based z-index** implemented for nested submenus
- **Portal insertion order** maintained for proper rendering sequence
- **100% keyboard navigation** working across all menu types

---

### 🏗️ Architecture Improvements

#### **1. FloatingPortal - Cascading Parameter Chain Fix (CRITICAL)**
**Problem:** FloatingPortal renders content outside normal DOM hierarchy (via `PortalHost`), breaking Blazor's cascading parameter chain. Nested menu items couldn't access root `MenuContext`.

**Solution:** Re-cascade root context through portal content
```razor
<!-- Before: Context lost through portal -->
<CascadingValue Value="@this">
    @ChildContent  <!-- DropdownMenuItem can't find DropdownMenuContext -->
</CascadingValue>

<!-- After: Explicitly re-cascade through portal -->
<CascadingValue Value="MenuContext">  <!-- ✅ Re-cascade root context -->
    <CascadingValue Value="@this">
        <CascadingValue Value="@SubContext" Name="ParentSubContext">
            @ChildContent  <!-- ✅ DropdownMenuItem can now access all contexts -->
        </CascadingValue>
    </CascadingValue>
</CascadingValue>
```

**Impact:** Fixed `InvalidOperationException: DropdownMenuItem must be used within a DropdownMenu component` errors in nested submenus.

**Files Modified:**
- `DropdownMenuSubContent.razor`
- `MenubarSubContent.razor`

---

#### **2. JavaScript Keyboard Navigation for All Menus**
**Problem:** C# keyboard navigation had focus timing issues, required manual item tracking, and didn't prevent scroll on arrow keys.

**Solution:** Migrated to JavaScript `keyboard-nav.js` module for all menu components
- **Double `requestAnimationFrame`** ensures elements are focusable before focusing
- **Automatic DOM order detection** - no manual item list tracking needed
- **Prevents default scroll behavior** for arrow keys, Home, End, PageUp, PageDown
- **Lazy-loaded modules** - only loaded when needed

**New JavaScript Function:**
```javascript
export function focusElement(element) {
    return new Promise((resolve) => {
        // Double RAF ensures element is fully rendered and focusable
        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                element.focus();
                resolve(document.activeElement === element);
            });
        });
    });
}
```

**C# Integration:**
```csharp
// Submenu opens → reliable focus
private async Task HandleFloatingReady()
{
    await SetupKeyboardNavAsync();
    var focused = await _keyboardNavModule.InvokeAsync<bool>("focusElement", _contentRef);
}

// Arrow navigation → JS handles it
case "ArrowDown":
    await _keyboardNavModule.InvokeVoidAsync("navigateNext", _contentRef, true);
    break;
```

**Files Modified:**
- `keyboard-nav.js` - Added `focusElement()` function
- `DropdownMenuSubContent.razor` - Integrated JS keyboard nav
- `MenubarSubContent.razor` - Integrated JS keyboard nav
- `ContextMenuContent.razor` - Updated `FocusContainerAsync` to use JS focus

**Benefits:**
- ✅ No more focus timing issues
- ✅ No manual item list tracking (35+ lines of code removed per component)
- ✅ Consistent behavior across all menu types
- ✅ Prevents page scroll when navigating menus

---

#### **3. Depth-Based Z-Index for Nested Submenus**
**Problem:** All submenus used same z-index (60), causing nested submenus to render beneath their parents.

**Solution:** Calculate z-index as `ZIndexLevels.PopoverContent + depth`
```csharp
// MenubarSubContext.cs / DropdownMenuSubContext.cs
public int Depth { get; set; } = 0;  // ✅ Track nesting level

// MenubarSub.razor / DropdownMenuSub.razor
_context.Depth = ParentSubContext != null ? ParentSubContext.Depth + 1 : 0;

// MenubarSubContent.razor / DropdownMenuSubContent.razor
private int EffectiveZIndex => ZIndex ?? (ZIndexLevels.PopoverContent + SubContext.Depth);
```

**Z-Index Hierarchy:**
```
Root menu content:      z-index 60 (depth 0)
First submenu:          z-index 60 (depth 0)
Nested submenu:         z-index 61 (depth 1)
Double-nested submenu:  z-index 62 (depth 2)
```

**Files Modified:**
- `DropdownMenuSubContext.cs` - Added `Depth` property
- `DropdownMenuSub.razor` - Depth tracking from parent
- `DropdownMenuSubContent.razor` - `EffectiveZIndex` calculation
- `MenubarSubContext.cs` - Added `Depth` property
- `MenubarSub.razor` - Depth tracking from parent
- `MenubarSubContent.razor` - `EffectiveZIndex` calculation

**Benefits:**
- ✅ Nested submenus always render above parents
- ✅ Unlimited nesting depth supported
- ✅ Automatic calculation (no manual z-index management)
- ✅ Can still override via `ZIndex` parameter if needed

---

#### **4. FocusElementAsync Helper Pattern**
**Problem:** Every component duplicated focus logic with timing issues and no consistent fallback strategy.

**Solution:** Standardized `FocusElementAsync()` helper method across all menu items and triggers
```csharp
private async Task<bool> FocusElementAsync(ElementReference element, string elementName = "element")
{
    // Lazy-load JS module on first focus
    if (!_keyboardNavModuleLoaded)
    {
        _keyboardNavModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoBlazorUI.Primitives/js/primitives/keyboard-nav.js");
        _keyboardNavModuleLoaded = true;
    }

    // Try JS focus with double RAF timing
    if (_keyboardNavModule != null)
    {
        var focused = await _keyboardNavModule.InvokeAsync<bool>("focusElement", element);
        if (focused) return true;
    }

    // Fallback to C# focus
    await element.FocusAsync();
    return true;
}
```

**Files Modified:**
- `DropdownMenuItem.razor` - Added helper + focus restoration on hover
- `DropdownMenuSubTrigger.razor` - Added helper with lazy loading
- `MenubarItem.razor` - Added helper + focus restoration on hover
- `ContextMenuItem.razor` - Added helper + focus restoration on hover

**Benefits:**
- ✅ DRY principle - one method for all focus needs
- ✅ Automatic JS/C# fallback strategy
- ✅ Descriptive logging for debugging
- ✅ Lazy module loading (performance optimization)

---

#### **5. Focus Restoration on Hover**
**Problem:** When hovering from submenu back to parent menu item, keyboard navigation stopped working because focus was lost.

**Solution:** Restore focus to parent menu container when closing submenu via hover
```csharp
private async void HandleMouseEnter(MouseEventArgs args)
{
    bool hadActiveSubmenu = ParentSubContext?.ActiveSubMenu != null;
    ParentSubContext?.CloseActiveSubMenu();
    
    // ✅ Restore focus to enable continued keyboard navigation
    if (hadActiveSubmenu && SubContentContext != null)
    {
        await SubContentContext.FocusContainerAsync();
    }
}
```

**Files Modified:**
- `DropdownMenuItem.razor` - Focus restoration on hover
- `DropdownMenuContent.razor` - Added `FocusContainerAsync()` method
- `MenubarItem.razor` - Focus restoration on hover
- `MenubarContent.razor` - Updated `FocusContainerAsync()` to use JS focus
- `ContextMenuItem.razor` - Focus restoration on hover
- `ContextMenuContent.razor` - Updated `FocusContainerAsync()` to use JS focus

**User Experience:**
```
1. User hovers submenu → Opens, receives focus ✅
2. User presses ArrowDown → Navigates within submenu ✅
3. User hovers parent item → Submenu closes, parent receives focus ✅
4. User presses ArrowDown → Navigates parent menu ✅
```

---

#### **6. Portal Insertion Order Maintained (Lock-Free)**
**Problem:** `ConcurrentDictionary` doesn't maintain insertion order, causing Dialog to sometimes render after its nested Combobox (wrong z-index order).

**Solution:** Wrap content with order tracking using immutable record
```csharp
private record PortalEntry(long Order, RenderFragment Content);
private readonly ConcurrentDictionary<string, PortalEntry> _portals = new();
private long _nextOrder = 0;

public void RegisterPortal(string id, RenderFragment content)
{
    _portals.AddOrUpdate(
        id,
        _ => new PortalEntry(Interlocked.Increment(ref _nextOrder), content),  // New: assign order
        (_, existing) => existing with { Content = content });  // Existing: preserve order
}

public IReadOnlyDictionary<string, RenderFragment> GetPortals()
{
    return _portals
        .OrderBy(kvp => kvp.Value.Order)  // ✅ Sort by insertion order
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Content);
}
```

**Files Modified:**
- `PortalService.cs` - Added `PortalEntry` record and order tracking

**Benefits:**
- ✅ No locks - uses `ConcurrentDictionary` + `Interlocked.Increment`
- ✅ Thread-safe atomic operations
- ✅ Immutable record pattern with `with` expressions
- ✅ Guaranteed rendering order (parent before child)
- ✅ Clean, elegant solution

---

### Added

**JavaScript Functions:**
- `focusElement(element)` - Reliable focus with double requestAnimationFrame timing in `keyboard-nav.js`

**Component Methods:**
- `FocusElementAsync()` - Standardized helper in all menu items and triggers
- `FocusContainerAsync()` - JS focus for all menu content components

**Data Structures:**
- `PortalEntry` record - Wraps insertion order + content for stable portal sorting

**Properties:**
- `Depth` - Submenu nesting level tracking in `DropdownMenuSubContext` and `MenubarSubContext`
- `EffectiveZIndex` - Calculated z-index based on depth in submenu content components

---

### Changed

**Keyboard Navigation:**
- **Before:** C# manual tracking with `List<IMenuItem>`, `_focusedIndex`, complex `FocusNextItem()`/`FocusPreviousItem()` methods
- **After:** JavaScript automatic DOM order detection, ~80 lines of code removed per component

**Focus Management:**
- **Before:** Direct `ElementReference.FocusAsync()` calls with timing issues
- **After:** `FocusElementAsync()` helper with double RAF + JS/C# fallback

**Z-Index Calculation:**
- **Before:** `ZIndex = ZIndexLevels.PopoverContent` (constant 60 for all submenus)
- **After:** `EffectiveZIndex = ZIndexLevels.PopoverContent + SubContext.Depth` (dynamic based on nesting)

**Portal Rendering:**
- **Before:** `ConcurrentDictionary` with random iteration order
- **After:** Order-tracked dictionary with stable insertion sequence

**Cascading Parameters:**
- **Before:** Cascading broken through FloatingPortal
- **After:** Explicitly re-cascade root context through portal content

---

### Fixed

**Critical Bugs:**
- ✅ **Cascading Parameter Chain:** Fixed `InvalidOperationException` in nested submenus by re-cascading `MenuContext` through FloatingPortal
- ✅ **Focus Timing Issues:** Submenu focus now works reliably on open via JavaScript double requestAnimationFrame
- ✅ **Focus Loss on Hover:** Keyboard navigation continues working when hovering from submenu to parent item
- ✅ **Nested Submenu Z-Index:** Nested submenus now render above their parents via depth-based calculation
- ✅ **Portal Rendering Order:** Portals now render in insertion order (parent before child) instead of random order

**Keyboard Navigation:**
- ✅ Arrow keys no longer cause page scroll in menus
- ✅ Focus restoration works when closing submenus via ArrowLeft
- ✅ All menu types (DropdownMenu, Menubar, ContextMenu) have consistent keyboard behavior
- ✅ Home/End keys navigate to first/last items

---

### Developer Experience

**Simplified Code:**
- Removed ~80 lines of manual focus tracking per submenu component
- Eliminated complex `_focusedIndex` state management
- No more `GetEnabledItemIndex()`, `FocusNextItem()`, `FocusPreviousItem()` helpers
- Single `FocusElementAsync()` method handles all focus needs

**Better Patterns:**
- Lock-free concurrent data structures with immutable records
- Lazy-loaded JavaScript modules (performance optimization)
- Explicit context re-cascading through portals (clear intent)
- Depth-based z-index calculation (automatic, no manual management)

**Maintainability:**
- Consistent focus pattern across all menu components
- JavaScript keyboard nav module shared by all menus
- Single source of truth for insertion order (PortalService)
- Clear separation of concerns (JS handles DOM, C# handles state)

---

### Breaking Changes

**None** - All changes are internal improvements with backward-compatible APIs.

---

### Tested & Validated

**Components Fully Tested:**
- ✅ DropdownMenu (Sub, SubTrigger, SubContent, MenuItem, MenuContent)
- ✅ Menubar (Sub, SubTrigger, SubContent, MenuItem, MenuContent)
- ✅ ContextMenu (MenuItem, MenuContent)
- ✅ FloatingPortal - Cascading parameter chain
- ✅ PortalService - Insertion order maintenance

**Scenarios Validated:**
- ✅ Nested submenus (2-3 levels deep) with proper z-index stacking
- ✅ Keyboard navigation: Arrow keys, Home, End, Enter, Escape, ArrowLeft/Right
- ✅ Focus timing: Submenu opens → receives focus immediately
- ✅ Focus restoration: Hover from submenu to parent → keyboard nav continues
- ✅ Portal rendering: Dialog always renders before nested Combobox
- ✅ Cascading parameters: Nested menu items access root context correctly

**User-Facing Features Validated:**
- ✅ Smooth keyboard navigation without page scroll
- ✅ Nested submenus render above parents (no z-index issues)
- ✅ Focus visible and working throughout menu interactions
- ✅ No "flash" or timing delays when opening submenus
- ✅ Hover + keyboard navigation work seamlessly together

---

## 2026-02-03 - Upstream Merge Complete + Critical Fixes

### 🎉 Major Upstream Merge: blazorui-net/BlazorUI (upstream/feb2)

**Merge Commit:** `8835bfed9859e4bf8349954ac05f732fe9ffddcf`  
**Status:** ✅ Complete, Fully Tested & Production Ready

#### 📊 Merge Statistics
- **167 files modified** across components, primitives, demos, and infrastructure
- **14 components enhanced** with new features and improved architecture
- **15 components kept** from our fork (superior implementations)
- **4 primitives refactored** to use modern Floating UI architecture
- **2 critical bugs fixed** (z-index conflicts, infinite loops)
- **515 lines removed** (35% code reduction in primitives)
- **100% tests passed** - all components validated including nested portal scenarios

#### 🏗️ Architecture Improvements

**Z-Index Hierarchy System (NEW)**
- Created centralized `ZIndexLevels` constants class for consistent layering
- Proper z-index hierarchy: DialogOverlay (40) < DialogContent (50) < PopoverContent (60) < TooltipContent (70)
- Fixed 10 components with incorrect z-index defaults (4 primitives + 6 components)
- Updated JavaScript (portal.js, positioning.js) to use consistent z-index variable
- Result: Nested portals work correctly (Select inside Dialog, Dropdown inside Dialog, etc.)

**FloatingPortal Infinite Loop Prevention (NEW)**
- Implemented lock-free rate limiting with `ConcurrentDictionary` + `ConcurrentQueue`
- Per-PortalId tracking: 3 refresh attempts within 100ms triggers loop detection
- Thread-safe, high-performance solution with automatic recovery
- Tested with 3+ levels of portal nesting
- Result: No more browser freezes, smooth nested portal rendering

**Floating UI Migration (35% Code Reduction)**
- Refactored DropdownMenu, HoverCard, Popover, Tooltip primitives to use declarative `FloatingPortal` component
- Eliminated manual lifecycle management, positioning service injections, and boilerplate code
- Centralized portal/positioning logic for better maintainability
- Modern industry-standard Floating UI library integration

**Select Component Modernization**
- Migrated from Combobox dependency to direct FloatingPortal integration
- Better separation of concerns and simplified architecture
- Improved DisplayText lifecycle with OnAfterRender synchronization
- Restored smooth animations (fade, zoom, directional slides)

**Command Component Enhancement**
- Self-contained architecture with internal CommandContext state management
- Removed Combobox dependency for smaller bundle size
- Virtualization support for large datasets (1500+ items tested)
- Preserved SearchInterval debouncing feature (our custom optimization)

### Added

**Infrastructure:**
- `ZIndexLevels` constants class for centralized z-index management
- Rate limiting algorithm in FloatingPortal for infinite loop prevention
- Documentation: SESSION_SUMMARY_ZINDEX_FIXES.md, FLOATING_PORTAL_GUARD_FIX.md

**Merged Components from Upstream:**
- **NativeSelect** - Native HTML `<select>` with shadcn/ui styling, generic TValue support, size variants, form attributes
- **CommandVirtualizedGroup** - Efficient virtualization for large datasets with lazy loading
- **Alert variants** - Added Muted, Success, Info, Warning, Danger (total 7 variants)
- **Alert features** - Icon parameter (RenderFragment), AccentBorder parameter

**New Features:**
- AlertDialog: CloseOnClickOutside parameter, AsChild composition pattern
- Command: Custom FilterFunction, controlled search state (@bind-SearchQuery), global Disabled state, CloseOnSelect control
- RichTextEditor: EditorRange, SelectionChangeEventArgs, TextChangeEventArgs, ToolbarPreset enum
- Select: DisplayTextSelector for immediate text resolution (no flicker)

**Demo Enhancements:**
- Global command search (SpotlightCommandPalette) with Ctrl+K/Cmd+K
- Virtualized icon search (1500+ icons from Lucide, Heroicons, Feather)
- Chart examples with DRY dictionary-based time range selectors
- Comprehensive Alert and AlertDialog demos
- Kbd component demo updated to use ChildContent instead of non-existent Keys parameter

### Changed - Upstream Merge (Post-Merge Refactor & Fixes)

**Z-Index Fixes:**
- PopoverContent, DropdownMenuContent, MenubarContent, ContextMenuContent: Changed default from 50 to `ZIndexLevels.PopoverContent` (60)
- Component layer (6 files): CSS now uses `ZIndex` property instead of hardcoded constant (respects custom overrides)
- JavaScript portal.js: Removed hardcoded `z-index: 9999` from portal container (children manage their own z-index)
- JavaScript positioning.js: Centralized z-index with `floatingZIndex = 60` variable (consistent with C# constants)

**FloatingPortal Enhancements:**
- Replaced guard flag approach with time-based rate limiting (per-PortalId tracking)
- Lock-free implementation using concurrent collections
- Automatic recovery (old timestamps age out naturally)
- Supports unlimited nesting depth without performance degradation

**Architecture Refactoring:**
- Select primitive: FloatingPortal direct integration (removed Combobox dependency)
- DropdownMenu, HoverCard, Popover, Tooltip: Floating UI declarative architecture
- Command: Self-contained with CommandContext (removed Combobox dependency)
- FloatingPortal: Added data-state and data-side attributes for animations

**Performance Optimizations:**
- Command virtualization handles thousands of items smoothly
- SearchInterval debouncing preserved (300ms delay, our feature)
- Efficient filtering with minimal re-renders
- Lazy loading in virtualized groups
- Lock-free rate limiting for FloatingPortal (no contention)

**Code Quality:**
- Chart Examples (Area, Bar, Line): Refactored to dictionary-based pattern with DisplayTextSelector (DRY principle)
- Removed 515 lines of redundant primitive code
- Better separation of concerns across all refactored components
- Centralized state management in Command and Select
- TailwindMerge: Enhanced regex to support arbitrary values with commas and spaces (e.g., `transition-[color, box-shadow]`)

**Accessibility Improvements:**
- Command: Fixed root ARIA role from `listbox` to `group` (semantic correctness)
- AlertDialog: Added `role="alertdialog"` attribute
- Dialog primitive wrappers for AlertDialogTitle and AlertDialogDescription
- Full keyboard navigation in Command (Arrow keys, Home, End, Enter, Escape)

**Component Enhancements:**
- Alert: 7 variants total (Default, Muted, Destructive, Success, Info, Warning, Danger)
- AlertDialog: Standard dismissal behavior (click-outside disabled by default, Escape enabled)
- Sidebar: Improved context subscription/unsubscription pattern (prevents memory leaks)
- RichTextEditor: Refactored JS initialization into InitializeJsAsync method

### Fixed

**Critical Bugs (NEW):**
- ✅ **Z-Index Conflicts:** Nested portals (Select/Dropdown inside Dialog) now render correctly at proper z-index layer
- ✅ **Infinite Loops:** FloatingPortal no longer causes browser freezes with nested portals (Dialog → Select, Dialog → Dropdown → Submenu, etc.)
- ✅ **JavaScript Z-Index Override:** Portal container no longer overrides component z-index with hardcoded `z-9999`
- ✅ **Z-Index Inconsistency:** JavaScript z-index now matches C# constants (both use 60 for PopoverContent)

**Select Component:**
- ✅ Animations restored (fade, zoom, directional slides) by adding data-state/data-side attributes
- ✅ DisplayText lifecycle fixed - no longer shows value instead of text on initial render
- ✅ Transform origin properly set on first child element for correct zoom animations
- ✅ DisplayTextSelector provides immediate text (zero flicker)

**Command Component:**
- ✅ HasVisibleItems() now checks virtualized groups (fixes incorrect "No results" display)
- ✅ SpotlightCommandPalette empty state correctly respects icon search results
- ✅ ARIA structure semantically correct (group → combobox + listbox)

**Chart Examples:**
- ✅ Time range selectors use clean dictionary pattern (eliminated ternary chains)
- ✅ Easy to extend (just add one line to dictionary for new time ranges)
- ✅ Type-safe and maintainable

**Primitives:**
- ✅ FloatingPortal transform origin fix for all floating components
- ✅ Better element readiness handling in positioning.js
- ✅ Auto-CSS injection and CDN fallback support

### Developer Experience

**Improved APIs:**
- Command: Intuitive `OnValueChange` (vs `OnSelect`) aligns with Blazor conventions
- Select: DisplayTextSelector for immediate display text resolution
- Chart examples: Single dictionary instead of multiple helper methods

**Better Maintainability:**
- 35% less primitive code to maintain
- Centralized positioning logic (fix bugs in one place)
- Dictionary-based patterns for easy configuration
- Declarative FloatingPortal vs imperative positioning setup

**Enhanced Features:**
- Command virtualization for large datasets
- Custom filtering functions
- Controlled search state
- Comprehensive keyboard navigation

### Breaking Changes (Minor)

⚠️ **Command Component:**
- `OnSelect` → `OnValueChange` (parameter renamed, same method signature)
- **Migration:** Simple find/replace `OnSelect=` with `OnValueChange=`

ℹ️ **Behavior Changes:**
- Alert: `Default` variant now uses `bg-background` (use `Muted` for old subtle styling)
- AlertDialog: Click-outside dismissal disabled by default (set `CloseOnClickOutside="true"` to enable)

### Tested & Validated

**Components Fully Tested:**
- ✅ Select (Primitive + Component) - Animations, DisplayText lifecycle, FloatingPortal integration
- ✅ Command (All subcomponents) - ARIA roles, virtualized groups, empty state, keyboard nav, SearchInterval
- ✅ SpotlightCommandPalette - Empty state logic, icon search (1500+ icons), navigation items
- ✅ Chart Examples (Area, Bar, Line) - DisplayTextSelector pattern, time range filtering
- ✅ FloatingPortal - Transform origin, data-state/data-side attributes
- ✅ Alert & AlertDialog - All 7 variants, dismissal behavior, accessibility
- ✅ DropdownMenu, HoverCard, Popover, Tooltip - Floating UI refactor

**User-Facing Features Validated:**
- ✅ Smooth Select dropdown animations from all directions (top, bottom, left, right)
- ✅ Zero-flicker display text in Select components
- ✅ Command palette correctly shows "No results" only when truly no matches
- ✅ Chart time range selectors with clean dictionary-based code
- ✅ All keyboard navigation working (Arrow keys, Home, End, Enter, Escape)
- ✅ Search debouncing preserves performance (300ms SearchInterval)
- ✅ Virtualized groups handle 1500+ items smoothly

### Components Kept (Our Superior Implementations)

**14 Components Retained:**
- **Pagination** (21 files vs 16) - PageSizeSelector, First/Last, Info display, Context, Size variants
- **Toast** (13 files vs 10) - 6 positions, 5 variants, structured data model
- **Toggle + ToggleGroup** (7 files vs 4) - Exclusive selection, toolbar patterns
- **Menubar** (18 files vs 16) - Interface pattern, alignment control
- **Slider, TimePicker, ScrollArea, Resizable** - Orientation, format, enhanced config, direction control
- **NavigationMenu, Progress, Kbd, Empty, Spinner, DropdownMenu** - Feature parity or better

### Build & Performance

- ✅ Solution builds successfully with no errors
- ✅ All animations working smoothly
- ✅ Performance validated (virtualization handles 1500+ items)
- ✅ Keyboard navigation responsive
- ✅ No breaking changes for end users (only minor API rename)

### Production Ready

All merged components, refactors, and fixes are production-ready and thoroughly tested. The merge brings modern architecture, better performance, reduced code complexity, and enhanced developer experience while maintaining backward compatibility.

---

## 2026-01-31

### Added
- DialogService for programmatic dialogs with async/await API
- LinkButton component for semantic navigation
- Sidebar improvements for production use cases (independent scrolling, state persistence)
- PersistentComponentState for zero-flicker state handover
- ScrollArea auto-hide behavior and enhancements
- Component navigation updates (LinkButton and DialogService to sidebar, index, spotlight)

### Changed
- SidebarProvider improvements (IHttpContextAccessor optional, IServiceProvider injection)
- SidebarCollapsibleGroup refactored for WebAssembly compatibility
- Sidebar component layout updates (h-screen, overflow-y-auto)
- ScrollArea enhancements (auto-hide, data-state attribute, dynamic content updates)
- State management refactoring (collapsible-state.js module, cookie operations moved to JavaScript)
- GlobalSuppressions updates

### Fixed
- DialogHost binding to use proper AlertDialog
- WebAssembly compatibility (IHttpContextAccessor dependencies, state persistence)
- Cookie persistence (URL-encoding, Secure flag, backward compatibility)
- Animation flickering during render mode transitions
- ScrollArea dynamic content height updates

### Removed
- Unused storage-helpers.js file
- JavaScript files no longer used

## 2026-01-29

### Added
- Column alignment support for DataTable component

## 2026-01-28

### Added
- Comprehensive LLM-friendly component reference documentation
- Descriptions and usage examples for all 62 components

### Fixed
- Typography section formatting
- Escaped @ symbols in HoverCard example
- HoverCard example simplified for clarity

## 2026-01-27

### Added
- Blazor EditForm and EditContext integration for automatic validation error display and focus

### Changed
- Documentation improvements and formatting fixes

## 2026-01-26

### Added
- LLM-friendly component documentation
- All 176 missing child components to comprehensive documentation

## 2026-01-24

### Added
- Content support to FieldSeparator component for centered content

### Changed
- Component version bumped to 1.0.15

## 2026-01-23

### Added
- Comprehensive form attributes to Input, Textarea, Checkbox, Switch, and RadioGroup components
- Name and Required attributes to RadioGroup in primitives
- Common input properties to input-related controls

### Changed
- Input-related visual styles updated to match latest shadcn
- Tailwind re-enabled in build process
- Dependencies version updates
- Component version bumped to 1.0.14
- Primitives version bumped to 1.0.14
- Version bumped to 1.0.12

## 2026-01-22

### Added
- SSR support in SidebarProvider and related components

## 2026-01-20

### Changed
- Assembly signature updated with all dependencies
- Fixed primitives reference in Components
- NuGet metadata updates for Components project
- README updates

## 2026-01-19

### Changed
- Target framework changed to .NET 10
- Package metadata finalized for v1 publishing
- Package name updates
- NuGet metadata updates

## 2026-01-18

### Added
- Grid NotifyItemsChanged pattern and Server-Side Row Model implementation

### Changed
- Tailwind CSS build enabled in project configuration

## 2026-01-17

### Added
- Server-side row model (SSRM) using AG Grid Enterprise module
- SSRM demo showcasing server-side sorting, filtering, and paging

### Changed
- DataGridImportMap refactored to simplify developer experience in App.razor
- Selection sample updated to use new TrackableObservableCollection

## 2026-01-14

### Added
- Comprehensive movie database demo with TMDb API integration
- Server-side row model with DataGridRowModelType and demo pages
- NotifyItemsChanged pattern with TrackableObservableCollection
- Grid Component Milestones 1-4 with complete demo pages
- Complete AG DataGrid state management with @bind-State and hash-based mutation detection

### Changed
- DataGrid added to indexes for production ready
- All DataGrid demos finalized with working functions

### Fixed
- Code review fixes for hash properties, firstRender check, and unused filterable property
- Type safety improvements

## 2026-01-13

### Added
- @bind-State with hash-based mutation detection for natural state management
- InitialState application and programmatic state updates for controlled sort/filter demos
- GetStateAsync to DataGrid component
- Three-level AG DataGrid theme customization with Shadcn integration

### Changed
- DataGridState and DataDataGridColumnState expanded with complete AG DataGrid properties
- All DataGrid demos finalized (minus sorting and states)
- Proper marshalling between JS and C# for row selections
- Comprehensive Transactions and Refresh API for granular controls

## 2026-01-12

### Fixed
- Case-sensitive IdField lookup in grid component
- ObservableCollection updates in grid
- Runtime theme switching support

## 2026-01-10

### Fixed
- Case-insensitive property lookup in SyncSelectionToGrid
- Multi-select demo changed to use ObservableCollection
- Runtime theme update support with UpdateThemeAsync

## 2026-01-09

### Changed
- Major revamp and refactor of data refresh infrastructure
- Added observability and partial update support

## 2026-01-07

### Fixed
- Default values for shadcn theme working properly
- Removed redundant defaults from JavaScript

## 2026-01-06

### Added
- Template rendering framework supporting cell templates and header templates
- GridAction for easy JS interop from Blazor components inside templates
- DataFormatString in DataGridColumn for easy data formatting

### Changed
- Templating demo finalized with DataFormatString implementation

## 2026-01-04

### Added
- Theming API support with shadcn design tokens
- ES module support for AG Grid

### Changed
- Switched from CDN to ES modules for AG Grid
- Theme application using native withParams API

### Fixed
- AG DataGrid theming regression
- Theme parameter handling

### Removed
- Manual CSS/JS loading infrastructure (200+ lines)

## 2026-01-03

### Added
- Comprehensive theme customization documentation
- Shadcn theme integration with dynamic CSS variable reading
- Support for any color format in CSS variables (hsl, oklch, rgba)

### Changed
- Refactored to use AG Grid's native withParams API
- Color values changed from hsl(var(--token)) to var(--token)
- Updated opacity colors to use color-mix()

### Fixed
- CSS variable handling improvements
- Parent validation improvements

## 2025-12-25

### Added
- Automatic navigation and active state detection in Sidebar component

## 2025-12-24

### Added
- DataGrid component with 20 comprehensive demo examples
- DataGrid demo tabs following charting pattern
- SearchInterval in CommandInput for faster query performance

### Changed
- Component index updates
- ContextMenu updated to use latest positioning pattern
- Dialog-based component animations polished with Tailwind config refactor
- Command demo examples updated

### Fixed
- Checkbox state sync regression after upstream merge
- Popover opacity conflicting with animation

## 2025-12-23

### Added
- Grid Component Milestones 1-4 (Core Object Model, Components, Renderer Abstraction, AG Grid Renderer)
- DataGrid architecture documentation (GRID_DEMOS_V1.md, GRID_VS_DATATABLE.md)
- CI/CD setup for GitHub and Azure App Service

### Changed
- .NET 10 migration completed
- README updated with latest architectural changes
- All components list added to sidebar, index, and spotlight search

### Fixed
- Missing using directives in DataGrid component
- BuildDataGridDefinition timing issues
- Page number calculation in Grid
- CSV formula injection mitigation added

## 2025-12-22

### Added
- Global search feature with Spotlight command palette
- Platform-specific keyboard shortcuts (Cmd/Ctrl+K)

### Changed
- Build targets for Tailwind re-enabled
- Command palette visual finalized

## 2025-12-21

### Fixed
- Command OnSelect not working
- Spotlight command palette for global search finalized

## 2025-12-20

### Added
- Motion component to global search

## 2025-12-19

### Added
- Declarative Chart API implementation
- Theme-aware charts with automatic refresh on theme change
- Legend text color support

### Changed
- All chart implementations rebuilt to production quality

## 2025-12-18

### Added
- Theme-aware implementation for all charts
- Automatic chart refresh on theme change (JavaScript-only approach)

### Changed
- ScatterChart, RadarChart, and ComposedChart examples rebuilt to production quality
- RadarChart code comments improved for clarity

### Fixed
- RadarChart to use DataKey for extracting series values
- Bubble chart and dataset descriptions

## 2025-12-17

### Added
- Animation parameters to ChartBase
- Full Emphasis support for Pie charts
- Chart enhancements for Pie, Scatter, Radar, and RadialBar charts

### Changed
- All chart builders now map animation properties
- Chart defaults finalized for best developer experience
- Resizer enhanced with observer

### Fixed
- Pie, Scatter, Radar, and RadialBar chart rendering issues

## 2025-12-16

### Added
- Animation system for declarative chart API
- Fill/LinearGradient/Stop with complete ECharts gradient mapping
- 6 production-ready features: YAxis position, DataGrid styling, Axis min/max, Tooltip styling, Symbol customization, Series opacity

### Fixed
- ECharts renderer: v6 download, improved color detection, single-flight loading

## 2025-12-15

### Added
- Comprehensive sample data for AreaChart examples
- Updated shadcn demos with production-ready features (Area, Bar, Line, Pie charts)

### Changed
- All chart demos replaced with new declarative API versions
- Div containers used instead of canvas for ECharts SVG rendering mode

## 2025-12-12

### Added
- Tooltip component improvements

## 2025-12-11

### Fixed
- Code style and formatting inconsistencies in the codebase

## 2025-12-10

### Changed
- Minor UI styling and layout adjustments across existing components

## 2025-12-09

### Changed
- Internal component refactoring and minor bug fixes

## 2025-12-08

### Changed
- Minor accessibility, spacing, and theme consistency updates across existing UI components

## 2025-12-07

### Added
- HeightAnimation documentation and usage examples to CommandDemo
- CSS variables to blazorui-input.css

### Changed
- Visual styles with auto dynamic list height

### Fixed
- Animation conflicts in Spotlight demo with !animate-none

## 2025-12-06

### Added
- Calendar base component with keyboard navigation
- MultiSelect component
- Smooth expand/collapse animations for Accordion and Collapsible
- Smooth height animation for command palette filtering
- Spotlight-style command palette with global keyboard shortcut support

### Changed
- Primitives package reference updated to 1.1.0
- Calendar component improvements: centered caption, dropdown mode, styled border/shadow
- IsFocused documentation simplified

### Fixed
- Focus ring persisting after clicking outside calendar
- Double focus ring issue with @key on tbody
- Dialog positioning with fixed top position
- Keyboard focus lost after month changes
- Calendar rendering, focus ring visibility, intrinsic width

## 2025-12-05

### Added
- Viewport boundary detection for context menu positioning
- Transparent overlay and scroll lock to ContextMenu
- Recursive submenu closing support for nested submenus
- Icon examples section to menu demo pages

### Changed
- Navigation refactored to follow DropdownMenu pattern
- ContextMenu refactored to follow DropdownMenu/Menubar pattern
- Calendar component code cleanup

### Fixed
- Submenu overflow issue and checkbox/radio item padding
- Submenu UX: prevent close on hover, update visual state, Escape closes all menus
- Keyboard navigation in submenus and focus restoration
- Hover highlighting and keyboard scroll in ContextMenu
- Disabled menu item style in DropdownMenuItem
- SubTrigger registration with parent submenu for proper keyboard navigation
- Overlay z-index to prevent negative values

## 2025-12-04

### Added
- Input OTP component with error state styling
- Menubar component
- CheckboxItem, RadioGroup/RadioItem, and Submenu components to DropdownMenu, Menubar, and ContextMenu
- Error state styling (AriaInvalid) to Input OTP component
- Animation utilities to tailwind config
- Hover-to-switch behavior for menubar triggers

### Changed
- Demo pages updated to showcase new menu features
- Animation utilities moved from demo to component library tailwind config

### Fixed
- Menubar dropdown positioning and keyboard scroll behavior
- Keyboard navigation and alphabet input support
- Focus UX: no editing middle slots, only show active state when focused
- CSS animations for dropdown menus by not setting opacity inline
- Text alignment (centered) and separator styling
- Input behaviors during backspace

### Removed
- eval() usage for security (replaced with makeVisible: true)

## 2025-12-03

### Added
- Initial Menubar and Input OTP component skeletons

---

**Note:** This changelog is based on git commit history. For a complete view of all commits, [visit the repository's commit history](https://github.com/jimmyps/blazor-shadcn-ui/commits/main).

