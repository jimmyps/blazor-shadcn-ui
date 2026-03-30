# NeoUI.Blazor.Primitives

Headless, unstyled Blazor primitive components with full accessibility support. Build your own component library using these composable primitives.

## ✨ Features

- **🎭 Headless & Unstyled**: Complete control over styling - primitives provide behavior, accessibility, and state management without imposing any visual design
- **♿ Accessibility First**: Built-in ARIA attributes, keyboard navigation, and screen reader support following WCAG 2.1 AA standards
- **🧩 Composition-Based**: Flexible component composition patterns for building complex UIs
- **🎯 .NET 10 Ready**: Built for the latest .NET platform with full WebAssembly and Server support
- **🔒 Type-Safe**: Full C# type safety with IntelliSense support
- **🔄 State Management**: Built-in controlled and uncontrolled state patterns
- **⌨️ Keyboard Navigation**: Full keyboard support for all interactive components
- **📦 Zero Dependencies**: No CSS or JavaScript required - bring your own styling

## 📦 Installation

```bash
dotnet add package NeoUI.Blazor.Primitives
```

## 🚀 Quick Start

**[Try the Live Demo](https://demos.neoui.io)** - Explore all primitives and styled components with interactive examples and full documentation.


## 📚 Available Primitives

- **Accordion**: Collapsible content sections with single or multiple item expansion
- **Checkbox**: Binary selection control with indeterminate state
- **Collapsible**: Expandable content area with trigger control
- **Dialog**: Modal dialogs with backdrop and focus management
- **Dropdown Menu**: Context menus with items, separators, and keyboard shortcuts
- **Hover Card**: Rich preview cards on hover with delay control
- **Label**: Accessible labels for form controls with automatic association
- **Popover**: Floating panels for additional content with positioning
- **Radio Group**: Mutually exclusive options with keyboard navigation
- **Select**: Dropdown selection with groups and virtualization support
- **Sheet**: Side panels that slide in from viewport edges
- **Sortable**: Headless drag-and-drop sortable list with pointer, touch, and keyboard support. Supports cross-list transfer between grouped containers.
- **Switch**: Toggle control for on/off states
- **Table**: Data table with header, body, rows, cells, and pagination
- **Tabs**: Tabbed interface with keyboard navigation
- **Tooltip**: Brief informational popups with hover/focus triggers

## 🎨 Pre-Styled Components (100+)

Built on top of these primitives, **[NeoUI.Blazor](https://www.nuget.org/packages/NeoUI.Blazor)** provides production-ready components with beautiful shadcn/ui design:

### Form & Input Components
Button, Button Group, Checkbox, Combobox, Date Picker, Field, Input, Input Group, Input OTP, Label, Multi Select, Native Select, Radio Group, Select, Slider, Switch, Textarea, Time Picker

### Data Display Components
Avatar, Badge, Card, Data Table, Empty, Grid, Item, Kbd, Progress, Separator, Skeleton, Spinner, Typography

### Navigation Components
Accordion, Breadcrumb, Command, Context Menu, Dropdown Menu, Menubar, Navigation Menu, Pagination, Sidebar, Tabs

### Overlay Components
Alert Dialog, Dialog, Hover Card, Popover, Sheet, Toast, Tooltip

### Feedback Components
Alert

### Layout & Display Components
Aspect Ratio, Carousel, Collapsible, Resizable, Scroll Area

### Advanced Components
Chart (12 types), Grid, Markdown Editor, Motion (20+ animation presets), Rich Text Editor, Toggle, Toggle Group

### Icon Libraries
Lucide Icons (1,640+), Heroicons (1,288), Feather Icons (286)

**Want beautiful defaults?** Check out the **[Components README](../NeoUI.Blazor/README.md)** for full documentation.





## 📖 Primitive API Reference


### Accordion

```razor
<AccordionPrimitive Type="AccordionType.Single" Collapsible="true" DefaultValue="item-1">
    <AccordionItemPrimitive Value="item-1">
        <AccordionTriggerPrimitive>Section 1</AccordionTriggerPrimitive>
        <AccordionContentPrimitive>Content 1</AccordionContentPrimitive>
    </AccordionItemPrimitive>
</AccordionPrimitive>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Type` | `AccordionType` | `Single` | `Single` (one item open), `Multiple` (many items open) |
| `Collapsible` | `bool` | `false` | When `Single`, allows closing all items |

### Tabs

```razor
<TabsPrimitive
    DefaultValue="tab1"
    Orientation="TabsOrientation.Horizontal"
    ActivationMode="TabsActivationMode.Automatic">
    <TabsListPrimitive>
        <TabsTriggerPrimitive Value="tab1">Tab 1</TabsTriggerPrimitive>
    </TabsListPrimitive>
    <TabsContentPrimitive Value="tab1">Content</TabsContentPrimitive>
</TabsPrimitive>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Orientation` | `TabsOrientation` | `Horizontal` | `Horizontal`, `Vertical` |
| `ActivationMode` | `TabsActivationMode` | `Automatic` | `Automatic` (on focus), `Manual` (on click) |

### Sheet

```razor
<SheetPrimitive>
    <SheetTriggerPrimitive>Open</SheetTriggerPrimitive>
    <SheetPortal>
        <SheetOverlay />
        <SheetContentPrimitive Side="SheetSide.Right">
            <SheetTitlePrimitive>Title</SheetTitlePrimitive>
            <SheetDescriptionPrimitive>Description</SheetDescriptionPrimitive>
            <SheetClosePrimitive>Close</SheetClosePrimitive>
        </SheetContentPrimitive>
    </SheetPortal>
</SheetPrimitive>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Side` | `SheetSide` | `Right` | `Top`, `Right`, `Bottom`, `Left` |

### Popover

```razor
<PopoverPrimitive>
    <PopoverTriggerPrimitive>Open</PopoverTriggerPrimitive>
    <PopoverContentPrimitive Side="PopoverSide.Bottom" Align="PopoverAlign.Center">
        Content here
    </PopoverContentPrimitive>
</PopoverPrimitive>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Side` | `PopoverSide` | `Bottom` | `Top`, `Right`, `Bottom`, `Left` |
| `Align` | `PopoverAlign` | `Center` | `Start`, `Center`, `End` |
| `CloseOnEscape` | `bool` | `true` | Close when Escape key pressed |
| `CloseOnClickOutside` | `bool` | `true` | Close when clicking outside |

### Tooltip

```razor
<TooltipPrimitive DelayDuration="400" HideDelay="0">
    <TooltipTriggerPrimitive>Hover me</TooltipTriggerPrimitive>
    <TooltipContentPrimitive>Tooltip text</TooltipContentPrimitive>
</TooltipPrimitive>
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `DelayDuration` | `int` | `400` | Milliseconds before showing |
| `HideDelay` | `int` | `0` | Milliseconds before hiding |

### HoverCard

```razor
<HoverCardPrimitive OpenDelay="700" CloseDelay="300">
    <HoverCardTriggerPrimitive>Hover for preview</HoverCardTriggerPrimitive>
    <HoverCardContentPrimitive>Rich preview content</HoverCardContentPrimitive>
</HoverCardPrimitive>
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `OpenDelay` | `int` | `700` | Milliseconds before showing |
| `CloseDelay` | `int` | `300` | Milliseconds before hiding |

### Checkbox

```razor
<CheckboxPrimitive
    @bind-Checked="isChecked"
    Indeterminate="@isIndeterminate"
    IndeterminateChanged="HandleIndeterminateChange" />
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Checked` | `bool` | `false` | Checked state |
| `Indeterminate` | `bool` | `false` | Shows partial/mixed state |

### Select

```razor
<SelectPrimitive TValue="string" @bind-Value="selected" @bind-Open="isOpen">
    <SelectTriggerPrimitive>
        Choose an option...
    </SelectTriggerPrimitive>
    <SelectContentPrimitive>
        <SelectItemPrimitive Value="@("a")">Option A</SelectItemPrimitive>
        <SelectItemPrimitive Value="@("b")">Option B</SelectItemPrimitive>
    </SelectContentPrimitive>
</SelectPrimitive>
```

Select is generic (`TValue`). Supports both value and open state binding.

### Table

```razor
<Table TData="Person">
    <TableHeader>
        <TableRow>
            <TableHeaderCell>Name</TableHeaderCell>
            <TableHeaderCell>Email</TableHeaderCell>
        </TableRow>
    </TableHeader>
    <TableBody>
        @foreach (var person in people)
        {
            <TableRow>
                <TableCell>@person.Name</TableCell>
                <TableCell>@person.Email</TableCell>
            </TableRow>
        }
    </TableBody>
</Table>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `SelectionMode` | `SelectionMode` | `None` | `None`, `Single`, `Multiple` |
| `SortDirection` | `SortDirection` | `None` | `None`, `Ascending`, `Descending` |

### Sortable

```razor
<SortablePrimitive TItem="MyItem"
                   Items="@items"
                   OnItemsReordered="@(r => items = r)"
                   GetItemId="@(i => i.Id)">
    <SortableContentPrimitive class="flex flex-col gap-2">
        @foreach (var item in items)
        {
            <SortableItemPrimitive @key="@item.Id" Value="@item.Id"
                                   class="flex items-center gap-3 rounded border px-4 py-3">
                <SortableItemHandlePrimitive class="cursor-grab" />
                <span>@item.Name</span>
            </SortableItemPrimitive>
        }
    </SortableContentPrimitive>
    <SortableOverlayPrimitive class="rounded border shadow-lg px-4 py-3" />
</SortablePrimitive>
```

**Cross-list transfer** — share a `Group` name across multiple `SortablePrimitive` instances. Consumer handles all state mutations via transfer events.

```razor
<SortablePrimitive TItem="MyItem" Items="@colA" Group="board"
                   GetItemId="@(i => i.Id)"
                   OnItemsReordered="@(r => colA = r)"
                   OnItemTransferredOut="@(a => colA = colA.Where(i => i.Id != a.ActiveId).ToList())"
                   OnItemTransferredIn="@(a => { var item = FindItem(a.ActiveId); colA.Insert(a.Index, item); })">
    ...
</SortablePrimitive>
```

**`SortablePrimitive<TItem>` parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IList<TItem>` | required | Source list |
| `GetItemId` | `Func<TItem, string>` | required | Extracts unique string ID from each item |
| `Orientation` | `SortableOrientation` | `Vertical` | `Vertical`, `Horizontal`, `Grid`, `Mixed` |
| `Group` | `string?` | `null` | Shared group name for cross-list DnD. Instances with the same group accept drops from each other. |
| `OnItemsReordered` | `EventCallback<IList<TItem>>` | — | Fired after a same-list reorder. Receives the reordered list. |
| `OnItemTransferredIn` | `EventCallback<SortableTransferArgs>` | — | Fired on the **target** when an item arrives from a peer. Consumer inserts the item at `args.Index`. |
| `OnItemTransferredOut` | `EventCallback<SortableTransferArgs>` | — | Fired on the **source** after the target accepts. Consumer removes `args.ActiveId` from its list. |
| `OnCanDrop` | `Func<SortableDragQueryArgs, bool>?` | `null` | Drop-time guard. Return `false` to reject; transfer events are not fired. |
| `OnDragStart` | `EventCallback<string>` | — | Fired when a drag begins. Receives the active item ID. |
| `OnDragEnd` | `EventCallback<SortableDragEndArgs>` | — | Fired when a same-list drag ends. |
| `OnDragCancel` | `EventCallback` | — | Fired on Escape or pointer cancel. |

**`SortableScope<TItem>`** — exposed via `Context="s"` on `<SortablePrimitive>` or `<Sortable>`.

| Member | Type | Description |
|--------|------|-------------|
| `RowAttributes` | `Func<TItem, Dictionary<string, object>>` | Stamps `data-sortable-id` on each row. Pass to `AdditionalRowAttributes` on any table or grid. |
| `ActiveId` | `string?` | ID of the item currently being dragged, or `null`. |
| `IsDragging` | `bool` | `true` while a drag is in progress. |
| `IsItemDragging(TItem)` | `bool` | `true` when the given item is the active drag item. |

**`SortableItemPrimitive` parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | required | Item ID matching `GetItemId` output. Add `@key="@item.Id"` on the element — required for cross-list. |
| `AsHandle` | `bool` | `false` | Makes the entire item draggable (no separate handle needed) |

**`SortableOverlayPrimitive` parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment<string>?` | `null` | Custom ghost. Context is the active item ID. Defaults to a JS clone of the source element. |

### MultiSelect

> **Note**: `MultiSelect` is a fully styled component with no separate primitive — use `<MultiSelect>` directly from `NeoUI.Blazor`.


## 🚀 Usage Example

```razor
@using NeoUI.Blazor.Primitives

<DialogPrimitive>
    <DialogTriggerPrimitive class="my-custom-button-class">
        Open Dialog
    </DialogTriggerPrimitive>
    <DialogPortal>
        <DialogOverlay class="my-overlay-styles" />
        <DialogContentPrimitive class="my-custom-dialog-styles">
            <DialogTitlePrimitive class="my-title-styles">
                Custom Styled Dialog
            </DialogTitlePrimitive>
            <DialogDescriptionPrimitive class="my-description-styles">
                This is a fully customizable dialog.
            </DialogDescriptionPrimitive>
            <p class="my-content-styles">Your content here</p>
            <DialogClosePrimitive class="my-close-button">Close</DialogClosePrimitive>
        </DialogContentPrimitive>
    </DialogPortal>
</DialogPrimitive>
```



## 🔄 Controlled vs Uncontrolled


All stateful primitives support both controlled and uncontrolled modes:

### Uncontrolled (Component manages its own state)

```razor
<DialogPrimitive>
    <DialogTriggerPrimitive>Open</DialogTriggerPrimitive>
    <DialogPortal>
        <DialogOverlay />
        <DialogContentPrimitive>
            <!-- Component handles open/close internally -->
        </DialogContentPrimitive>
    </DialogPortal>
</DialogPrimitive>
```

### Controlled (Parent component manages state)

```razor
<DialogPrimitive @bind-Open="isDialogOpen">
    <DialogTriggerPrimitive>Open</DialogTriggerPrimitive>
    <DialogPortal>
        <DialogOverlay />
        <DialogContentPrimitive>
            <button @onclick="() => isDialogOpen = false">Close</button>
        </DialogContentPrimitive>
    </DialogPortal>
</DialogPrimitive>

@code {
    private bool isDialogOpen = false;
}
```


## 🏗️ Design Philosophy


NeoUI.Blazor.Primitives follows the "headless component" pattern popularized by Radix UI and Headless UI:

1. **Separation of Concerns**: Primitives handle behavior and accessibility; you handle the design
2. **Composability**: Build complex components by composing simple primitives
3. **No Style Opinions**: Zero CSS included - bring your own design system
4. **Accessibility by Default**: ARIA attributes and keyboard navigation built-in


## 🎯 When to Use

**Use NeoUI.Blazor.Primitives when:**
- Building a custom design system from scratch
- Need complete control over component styling
- Want to match a specific brand or design language
- Integrating with existing CSS frameworks or design tokens

**Consider [NeoUI.Blazor](https://www.nuget.org/packages/NeoUI.Blazor) when:**
- Want beautiful defaults with shadcn/ui design
- Prefer zero-configuration setup with pre-built CSS
- Need to ship quickly without custom styling



## 📖 Documentation

For full documentation, examples, and API reference, visit:
- **[Live Demo](https://demos.neoui.io)** - Interactive examples and documentation
- [GitHub Repository](https://github.com/jimmyps/blazor-shadcn-ui)



## 📄 License

MIT License - see LICENSE file for details

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 🙏 Acknowledgments

- Initial Blazor primitives inspiration by [Mathew Taylor](https://github.com/blazorui-net/ui)
- Design patterns inspired by [Radix UI](https://www.radix-ui.com/) and [Headless UI](https://headlessui.com/)
- Built with ❤️ for the Blazor community

## 📊 Version Information

- **Current Version**: 3.8.2
- **Target Framework**: .NET 10
- **Package ID**: NeoUI.Blazor.Primitives
- **Assembly Name**: NeoUI.Blazor.Primitives

---


