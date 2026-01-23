# NeoBlazorUI.Primitives

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
dotnet add package NeoBlazorUI.Primitives
```

## 🚀 Quick Start

**[Try the Live Demo](https://blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net)** - Explore all primitives and styled components with interactive examples and full documentation.


## 📚 Available Primitives

- **Accordion**: Collapsible content sections with single or multiple item expansion
- **Checkbox**: Binary selection control with indeterminate state
- **Collapsible**: Expandable content area with trigger control
- **Combobox**: Autocomplete input with searchable dropdown
- **Dialog**: Modal dialogs with backdrop and focus management
- **Dropdown Menu**: Context menus with items, separators, and keyboard shortcuts
- **Hover Card**: Rich preview cards on hover with delay control
- **Label**: Accessible labels for form controls with automatic association
- **MultiSelect**: Multi-select dropdown with tag management and select-all support
- **Popover**: Floating panels for additional content with positioning
- **Radio Group**: Mutually exclusive options with keyboard navigation
- **Select**: Dropdown selection with groups and virtualization support
- **Sheet**: Side panels that slide in from viewport edges
- **Switch**: Toggle control for on/off states
- **Table**: Data table with header, body, rows, cells, and pagination
- **Tabs**: Tabbed interface with keyboard navigation
- **Tooltip**: Brief informational popups with hover/focus triggers

## 🎨 Pre-Styled Components (75+)

Built on top of these primitives, **[NeoBlazorUI.Components](https://www.nuget.org/packages/NeoBlazorUI.Components)** provides production-ready components with beautiful shadcn/ui design:

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
Chart (8 types), Grid, Markdown Editor, Motion (20+ animation presets), Rich Text Editor, Toggle, Toggle Group

### Icon Libraries
Lucide Icons (1,640+), Heroicons (1,288), Feather Icons (286)

**Want beautiful defaults?** Check out the **[Components README](../BlazorUI.Components/README.md)** for full documentation.





## 📖 Primitive API Reference


### Accordion

```razor
<Accordion Type="AccordionType.Single" Collapsible="true" DefaultValue="item-1">
    <AccordionItem Value="item-1">
        <AccordionTrigger>Section 1</AccordionTrigger>
        <AccordionContent>Content 1</AccordionContent>
    </AccordionItem>
</Accordion>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Type` | `AccordionType` | `Single` | `Single` (one item open), `Multiple` (many items open) |
| `Collapsible` | `bool` | `false` | When `Single`, allows closing all items |

### Tabs

```razor
<Tabs
    DefaultValue="tab1"
    Orientation="TabsOrientation.Horizontal"
    ActivationMode="TabsActivationMode.Automatic">
    <TabsList>
        <TabsTrigger Value="tab1">Tab 1</TabsTrigger>
    </TabsList>
    <TabsContent Value="tab1">Content</TabsContent>
</Tabs>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Orientation` | `TabsOrientation` | `Horizontal` | `Horizontal`, `Vertical` |
| `ActivationMode` | `TabsActivationMode` | `Automatic` | `Automatic` (on focus), `Manual` (on click) |

### Sheet

```razor
<Sheet>
    <SheetTrigger>Open</SheetTrigger>
    <SheetPortal>
        <SheetOverlay />
        <SheetContent Side="SheetSide.Right">
            <SheetTitle>Title</SheetTitle>
            <SheetDescription>Description</SheetDescription>
            <SheetClose>Close</SheetClose>
        </SheetContent>
    </SheetPortal>
</Sheet>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Side` | `SheetSide` | `Right` | `Top`, `Right`, `Bottom`, `Left` |

### Popover

```razor
<Popover>
    <PopoverTrigger>Open</PopoverTrigger>
    <PopoverContent Side="PopoverSide.Bottom" Align="PopoverAlign.Center">
        Content here
    </PopoverContent>
</Popover>
```

| Parameter | Type | Default | Values |
|-----------|------|---------|--------|
| `Side` | `PopoverSide` | `Bottom` | `Top`, `Right`, `Bottom`, `Left` |
| `Align` | `PopoverAlign` | `Center` | `Start`, `Center`, `End` |
| `CloseOnEscape` | `bool` | `true` | Close when Escape key pressed |
| `CloseOnClickOutside` | `bool` | `true` | Close when clicking outside |

### Tooltip

```razor
<Tooltip DelayDuration="400" HideDelay="0">
    <TooltipTrigger>Hover me</TooltipTrigger>
    <TooltipContent>Tooltip text</TooltipContent>
</Tooltip>
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `DelayDuration` | `int` | `400` | Milliseconds before showing |
| `HideDelay` | `int` | `0` | Milliseconds before hiding |

### HoverCard

```razor
<HoverCard OpenDelay="700" CloseDelay="300">
    <HoverCardTrigger>Hover for preview</HoverCardTrigger>
    <HoverCardContent>Rich preview content</HoverCardContent>
</HoverCard>
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `OpenDelay` | `int` | `700` | Milliseconds before showing |
| `CloseDelay` | `int` | `300` | Milliseconds before hiding |

### Checkbox

```razor
<Checkbox
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
<Select TValue="string" @bind-Value="selected" @bind-Open="isOpen">
    <SelectTrigger>
        <SelectValue Placeholder="Choose..." />
    </SelectTrigger>
    <SelectContent>
        <SelectItem Value="@("a")">Option A</SelectItem>
        <SelectItem Value="@("b")">Option B</SelectItem>
    </SelectContent>
</Select>
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

### MultiSelect

```razor
<MultiSelect TItem="string" @bind-SelectedItems="selected" Items="options">
    <MultiSelectTrigger>
        <MultiSelectInput Placeholder="Select items..." />
    </MultiSelectTrigger>
    <MultiSelectContent>
        <MultiSelectSelectAll>Select All</MultiSelectSelectAll>
        @foreach (var item in options)
        {
            <MultiSelectItem Value="item">@item</MultiSelectItem>
        }
    </MultiSelectContent>
</MultiSelect>
```

| Parameter | Type | Description |
|-----------|------|-------------|
| `SelectAllState` | `SelectAllState` | `None`, `Indeterminate`, `All` |


## 🚀 Usage Example

```razor
@using BlazorUI.Primitives.Dialog

<DialogPrimitive>
    <DialogTrigger class="my-custom-button-class">
        Open Dialog
    </DialogTrigger>
    <DialogPortal>
        <DialogOverlay class="my-overlay-styles" />
        <DialogContent class="my-custom-dialog-styles">
            <DialogTitle class="my-title-styles">
                Custom Styled Dialog
            </DialogTitle>
            <DialogDescription class="my-description-styles">
                This is a fully customizable dialog.
            </DialogDescription>
            <p class="my-content-styles">Your content here</p>
            <DialogClose class="my-close-button">Close</DialogClose>
        </DialogContent>
    </DialogPortal>
</DialogPrimitive>
```



## 🔄 Controlled vs Uncontrolled


All stateful primitives support both controlled and uncontrolled modes:

### Uncontrolled (Component manages its own state)

```razor
<DialogPrimitive>
    <DialogTrigger>Open</DialogTrigger>
    <DialogContent>
        <!-- Component handles open/close internally -->
    </DialogContent>
</DialogPrimitive>
```

### Controlled (Parent component manages state)

```razor
<DialogPrimitive @bind-Open="isDialogOpen">
    <DialogTrigger>Open</DialogTrigger>
    <DialogContent>
        <button @onclick="() => isDialogOpen = false">Close</button>
    </DialogContent>
</DialogPrimitive>

@code {
    private bool isDialogOpen = false;
}
```


## 🏗️ Design Philosophy


BlazorUI.Primitives follows the "headless component" pattern popularized by Radix UI and Headless UI:

1. **Separation of Concerns**: Primitives handle behavior and accessibility; you handle the design
2. **Composability**: Build complex components by composing simple primitives
3. **No Style Opinions**: Zero CSS included - bring your own design system
4. **Accessibility by Default**: ARIA attributes and keyboard navigation built-in


## 🎯 When to Use

**Use NeoBlazorUI.Primitives when:**
- Building a custom design system from scratch
- Need complete control over component styling
- Want to match a specific brand or design language
- Integrating with existing CSS frameworks or design tokens

**Consider [NeoBlazorUI.Components](https://www.nuget.org/packages/NeoBlazorUI.Components) when:**
- Want beautiful defaults with shadcn/ui design
- Prefer zero-configuration setup with pre-built CSS
- Need to ship quickly without custom styling



## 📖 Documentation

For full documentation, examples, and API reference, visit:
- **[Live Demo](https://blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net)** - Interactive examples and documentation
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

- **Current Version**: 1.0.12
- **Target Framework**: .NET 10
- **Package ID**: NeoBlazorUI.Primitives
- **Assembly Name**: NeoBlazorUI.Primitives

---


