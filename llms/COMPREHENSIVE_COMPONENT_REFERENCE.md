# Comprehensive Component Reference

**NeoBlazorUI** - A comprehensive UI component library for Blazor inspired by shadcn/ui

Version: 1.0.15  
Target Framework: .NET 8.0+  
Last Updated: 2026-01-26

---

## 📋 Table of Contents

### Styled Components (62)
- [Accordion](#accordion)
- [Alert](#alert)
- [AlertDialog](#alertdialog)
- [AspectRatio](#aspectratio)
- [Avatar](#avatar)
- [Badge](#badge)
- [Breadcrumb](#breadcrumb)
- [Button](#button)
- [ButtonGroup](#buttongroup)
- [Calendar](#calendar)
- [Card](#card)
- [Carousel](#carousel)
- [Chart](#chart)
- [Checkbox](#checkbox)
- [Collapsible](#collapsible)
- [Combobox](#combobox)
- [Command](#command)
- [ContextMenu](#contextmenu)
- [DataTable](#datatable)
- [DatePicker](#datepicker)
- [Dialog](#dialog)
- [DropdownMenu](#dropdownmenu)
- [Empty](#empty)
- [Field](#field)
- [Grid](#grid)
- [HeightAnimation](#heightanimation)
- [HoverCard](#hovercard)
- [Input](#input)
- [InputGroup](#inputgroup)
- [InputOtp](#inputotp)
- [Item](#item)
- [Kbd](#kbd)
- [Label](#label)
- [MarkdownEditor](#markdowneditor)
- [Menubar](#menubar)
- [Motion](#motion)
- [MultiSelect](#multiselect)
- [NativeSelect](#nativeselect)
- [NavigationMenu](#navigationmenu)
- [Pagination](#pagination)
- [Popover](#popover)
- [Progress](#progress)
- [RadioGroup](#radiogroup)
- [Resizable](#resizable)
- [RichTextEditor](#richtexteditor)
- [ScrollArea](#scrollarea)
- [Select](#select)
- [Separator](#separator)
- [Sheet](#sheet)
- [Sidebar](#sidebar)
- [Skeleton](#skeleton)
- [Slider](#slider)
- [Spinner](#spinner)
- [Switch](#switch)
- [Tabs](#tabs)
- [Textarea](#textarea)
- [TimePicker](#timepicker)
- [Toast](#toast)
- [Toggle](#toggle)
- [ToggleGroup](#togglegroup)
- [Tooltip](#tooltip)
- [Typography](#typography)

### Primitives (21)
- [Accordion](#accordion-primitive)
- [Checkbox](#checkbox-primitive)
- [Collapsible](#collapsible-primitive)
- [Combobox](#combobox-primitive)
- [ContextMenu](#contextmenu-primitive)
- [Dialog](#dialog-primitive)
- [DropdownMenu](#dropdownmenu-primitive)
- [HoverCard](#hovercard-primitive)
- [InputOtp](#inputotp-primitive)
- [Label](#label-primitive)
- [Menubar](#menubar-primitive)
- [MultiSelect](#multiselect-primitive)
- [NavigationMenu](#navigationmenu-primitive)
- [Popover](#popover-primitive)
- [RadioGroup](#radiogroup-primitive)
- [Select](#select-primitive)
- [Sheet](#sheet-primitive)
- [Switch](#switch-primitive)
- [Table](#table-primitive)
- [Tabs](#tabs-primitive)
- [Tooltip](#tooltip-primitive)

### Enums Reference (75)
- [AccordionType](#accordiontype-enum)
- [AlertVariant](#alertvariant-enum)
- [AnimationEasing](#animationeasing-enum)
- [AnimationType](#animationtype-enum)
- [AvatarSize](#avatarsize-enum)
- [AxisScale](#axisscale-enum)
- [BadgeVariant](#badgevariant-enum)
- [BarLayout](#barlayout-enum)
- [ButtonSize](#buttonsize-enum)
- [ButtonType](#buttontype-enum)
- [ButtonVariant](#buttonvariant-enum)
- [CalendarCaptionLayout](#calendarcaptionlayout-enum)
- [CarouselOrientation](#carouselorientation-enum)
- [ChartEngine](#chartengine-enum)
- [ChartType](#charttype-enum)
- [DataTableSelectionMode](#datatableselectionmode-enum)
- [Focus](#focus-enum)
- [GradientDirection](#gradientdirection-enum)
- [GridColumnPinPosition](#gridcolumnpinposition-enum)
- [GridDensity](#griddensity-enum)
- [GridFilterOperator](#gridfilteroperator-enum)
- [GridPagingMode](#gridpagingmode-enum)
- [GridRowModelType](#gridrowmodeltype-enum)
- [GridSelectionMode](#gridselectionmode-enum)
- [GridSortDirection](#gridsortdirection-enum)
- [GridStyle](#gridstyle-enum)
- [GridTheme](#gridtheme-enum)
- [GridVirtualizationMode](#gridvirtualizationmode-enum)
- [IconPosition](#iconposition-enum)
- [ImageFormat](#imageformat-enum)
- [InputGroupAlign](#inputgroupalign-enum)
- [InputType](#inputtype-enum)
- [InterpolationType](#interpolationtype-enum)
- [ItemMediaVariant](#itemmediavariant-enum)
- [ItemSize](#itemsize-enum)
- [ItemVariant](#itemvariant-enum)
- [LabelPosition](#labelposition-enum)
- [LegendAlign](#legendalign-enum)
- [LegendIcon](#legendicon-enum)
- [LegendLayout](#legendlayout-enum)
- [LegendVerticalAlign](#legendverticalalign-enum)
- [LineStyleType](#linestyletype-enum)
- [NavigationMenuOrientation](#navigationmenuorientation-enum)
- [Orientation](#orientation-enum)
- [PolarGridType](#polargridtype-enum)
- [RadarShape](#radarshape-enum)
- [ResizableDirection](#resizabledirection-enum)
- [ScrollAreaType](#scrollareatype-enum)
- [SelectAllState](#selectallstate-enum)
- [SelectionMode](#selectionmode-enum)
- [SeparatorOrientation](#separatororientation-enum)
- [SheetSide](#sheetside-enum)
- [SidebarGroupLabelElement](#sidebargrouplabelelement-enum)
- [SidebarMenuButtonElement](#sidebarmenubuttonelement-enum)
- [SidebarMenuButtonSize](#sidebarmenubuttonsize-enum)
- [SidebarMenuButtonVariant](#sidebarmenubuttonvariant-enum)
- [SidebarMenuSubButtonSize](#sidebarmenusubbuttonsize-enum)
- [SidebarSide](#sidebarside-enum)
- [SidebarVariant](#sidebarvariant-enum)
- [SkeletonShape](#skeletonshape-enum)
- [SortDirection](#sortdirection-enum)
- [SpinnerSize](#spinnersize-enum)
- [StackOffset](#stackoffset-enum)
- [SymbolShape](#symbolshape-enum)
- [TabsActivationMode](#tabsactivationmode-enum)
- [TabsOrientation](#tabsorientation-enum)
- [ToastPosition](#toastposition-enum)
- [ToastVariant](#toastvariant-enum)
- [ToggleGroupType](#togglegrouptype-enum)
- [ToggleSize](#togglesize-enum)
- [ToggleVariant](#togglevariant-enum)
- [TooltipCursor](#tooltipcursor-enum)
- [TooltipMode](#tooltipmode-enum)
- [TypographyVariant](#typographyvariant-enum)
- [YAxisPosition](#yaxisposition-enum)

---

## 🎨 Styled Components

Pre-styled components with shadcn/ui design, built on top of primitives.

### Accordion

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Accordion`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Accordion
```

**Basic Usage:**
```razor
<Accordion>
    <AccordionItem Value="item-1">
        <AccordionTrigger>Is it accessible?</AccordionTrigger>
        <AccordionContent>
            Yes. It adheres to WCAG 2.1 AA standards.
        </AccordionContent>
    </AccordionItem>

    <AccordionItem Value="item-2">
        <AccordionTrigger>Is it styled?</AccordionTrigger>
        <AccordionContent>
            Yes. It comes with default shadcn/ui styles.
        </AccordionContent>
    </AccordionItem>
</Accordion>
```

---

### Alert

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Alert`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Alert
```

**Components & Parameters:**

#### `Alert`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Variant` | `AlertVariant` | `AlertVariant.Default` |  | Gets or sets the visual style variant of the alert. <remarks> Controls the color scheme and visual appearance using CSS custom properties. Default value is <see cref="AlertVariant.Default"/>. </rem... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the alert. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the alert. <remarks> Typically contains AlertTitle, AlertDescription, and optionally an icon. For accessibility, ensure meaningful content is provided... |

#### `AlertDescription`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the alert description. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the alert description. |

#### `AlertTitle`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the alert title. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the alert title. |

**Related Enums:**

- [`AlertVariant`](#alertvariant-enum)

---

### AlertDialog

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.AlertDialog`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.AlertDialog
```

---

### AspectRatio

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.AspectRatio`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.AspectRatio
```

---

### Avatar

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Avatar`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Avatar
```

**Components & Parameters:**

#### `Avatar`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to render inside the avatar. <remarks> Typically contains AvatarImage and AvatarFallback components. The first successfully loaded content will be displayed. </remarks> |
| `Size` | `AvatarSize` | `AvatarSize.Default` |  | Gets or sets the size variant of the avatar. <remarks> Controls the dimensions and font-size of the avatar. Default value is <see cref="AvatarSize.Default"/>. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the avatar container. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |

#### `AvatarFallback`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to render as fallback. <remarks> Typically contains: - User initials (e.g., "JD" for John Doe) - An icon component (e.g., LucideIcon with "user") - Custom markup or compone... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the fallback container. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides such as custom backgro... |

#### `AvatarImage`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Source` | `string?` | `` |  | Gets or sets the URL of the image to display. <remarks> Should be a valid image URL. If the image fails to load, the component will hide and defer to AvatarFallback. </remarks> |
| `Alt` | `string?` | `` |  | Gets or sets the alternative text for the image. <remarks> Essential for accessibility. Screen readers use this to describe the image to visually impaired users. Should describe who the avatar repr... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the image. <remarks> Custom classes are appended after the component's base classes. </remarks> |

**Basic Usage:**
```razor
<Avatar>
    <AvatarImage Src="/avatar.jpg" Alt="User Avatar" />
    <AvatarFallback>JD</AvatarFallback>
</Avatar>
```

**Related Enums:**

- [`AvatarSize`](#avatarsize-enum)

---

### Badge

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Badge`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Badge
```

**Components & Parameters:**

#### `Badge`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Variant` | `BadgeVariant` | `BadgeVariant.Default` |  | Gets or sets the visual style variant of the badge. <remarks> Controls the color scheme and visual appearance using CSS custom properties. Default value is <see cref="BadgeVariant.Default"/>. </rem... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the badge. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the badge. <remarks> Typically contains short text (1-2 words) or a small number. For accessibility, ensure the content is meaningful. </remarks> |

**Basic Usage:**
```razor
<Badge>Default</Badge>
<Badge Variant="BadgeVariant.Secondary">Secondary</Badge>
<Badge Variant="BadgeVariant.Destructive">Destructive</Badge>
<Badge Variant="BadgeVariant.Outline">Outline</Badge>
```

**Related Enums:**

- [`BadgeVariant`](#badgevariant-enum)

---

### Breadcrumb

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Breadcrumb`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Breadcrumb
```

**Components & Parameters:**

#### `Breadcrumb`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `AriaLabel` | `string` | `"breadcrumb"` |  | Gets or sets the aria-label for the breadcrumb navigation. <remarks> Provides accessible label for screen readers. Default value is "breadcrumb". </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the breadcrumb. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the breadcrumb. |

#### `BreadcrumbEllipsis`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the breadcrumb ellipsis. |

#### `BreadcrumbItem`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the breadcrumb item. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the breadcrumb item. |

#### `BreadcrumbLink`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Href` | `string?` | `` |  | Gets or sets the href attribute for the link. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the breadcrumb link. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the breadcrumb link. |

#### `BreadcrumbList`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the breadcrumb list. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the breadcrumb list. |

#### `BreadcrumbPage`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the breadcrumb page. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the breadcrumb page. |

#### `BreadcrumbSeparator`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the breadcrumb separator. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets custom content for the separator. If not provided, a default chevron icon is used. |

---

### Button

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Button`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Button
```

**Components & Parameters:**

#### `Button`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Variant` | `ButtonVariant` | `ButtonVariant.Default` |  | Gets or sets the visual style variant of the button. <remarks> Controls the color scheme and visual appearance using CSS custom properties. Default value is <see cref="ButtonVariant.Default"/>. </r... |
| `Size` | `ButtonSize` | `ButtonSize.Default` |  | Gets or sets the size of the button. <remarks> Controls padding, font size, and overall dimensions. Default value is <see cref="ButtonSize.Default"/>. All sizes maintain minimum touch target sizes ... |
| `Type` | `ButtonType` | `ButtonType.Button` |  | Gets or sets the HTML button type attribute. <remarks> Controls form submission behavior when button is inside a form. Default value is <see cref="ButtonType.Button"/> to prevent accidental form su... |
| `Disabled` | `bool` | `` |  | Gets or sets whether the button is disabled. <remarks> When disabled: - Button cannot be clicked or focused - Opacity is reduced (via disabled:opacity-50 Tailwind class) - Pointer events are disabl... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the button. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `OnClick` | `EventCallback<MouseEventArgs>` | `` |  | Gets or sets the callback invoked when the button is clicked. <remarks> The event handler receives a <see cref="MouseEventArgs"/> parameter with click details. If the button is disabled, this callb... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the button. <remarks> Can contain text, icons, or any other Blazor markup. For icon-only buttons, use <see cref="ButtonSize.Icon"/> and provide an ari... |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the button. <remarks> Required for icon-only buttons to provide accessible text for screen readers. Optional for buttons with text content. </remarks> |
| `Icon` | `RenderFragment?` | `` |  | Gets or sets the icon to display in the button. <remarks> Can be any RenderFragment (SVG, icon font, image). Position is controlled by <see cref="IconPosition"/>. Automatically adds RTL-aware spaci... |
| `IconPosition` | `IconPosition` | `IconPosition.Start` |  | Gets or sets the position of the icon relative to the button text. <remarks> Default value is <see cref="IconPosition.Start"/> (before text in LTR). Automatically adapts to RTL layouts using Tailwi... |

**Basic Usage:**
```razor
<Button>Click me</Button>
```

**Related Enums:**

- [`ButtonVariant`](#buttonvariant-enum)
- [`IconPosition`](#iconposition-enum)
- [`ButtonSize`](#buttonsize-enum)
- [`ButtonType`](#buttontype-enum)

---

### ButtonGroup

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.ButtonGroup`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.ButtonGroup
```

**Components & Parameters:**

#### `ButtonGroup`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Orientation` | `ButtonGroupOrientation` | `ButtonGroupOrientation.Horizontal` |  | Gets or sets the orientation of the button group. <remarks> Controls whether buttons are arranged horizontally (default) or vertically. Default value is <see cref="ButtonGroupOrientation.Horizontal... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the button group. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the button group. <remarks> Provides an accessible name for the group when role="group" is used. Important for screen reader users to understand the group's purpose.... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the button group. <remarks> Typically contains Button components, but can also contain nested ButtonGroup components for creating complex layouts. </r... |

#### `ButtonGroupSeparator`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Orientation` | `SeparatorOrientation` | `SeparatorOrientation.Vertical` |  | Gets or sets the orientation of the separator. <remarks> Should match the parent ButtonGroup's orientation. Default value is <see cref="SeparatorOrientation.Vertical"/> (for horizontal button group... |
| `Decorative` | `bool` | `true` |  | Gets or sets whether the separator is purely decorative. <remarks> When true (default), the separator is treated as decorative and hidden from assistive technologies. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the separator. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |

#### `ButtonGroupText`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the text container. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the text container. <remarks> Can contain text, icons, or any other markup. Icons will automatically be sized to match the button group's icon size. <... |

**Basic Usage:**
```razor
<ButtonGroup>
    <Button Variant="ButtonVariant.Outline">Left</Button>
    <Button Variant="ButtonVariant.Outline">Center</Button>
    <Button Variant="ButtonVariant.Outline">Right</Button>
</ButtonGroup>
```

---

### Calendar

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Calendar`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Calendar
```

**Related Enums:**

- [`CalendarCaptionLayout`](#calendarcaptionlayout-enum)

---

### Card

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Card`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Card
```

**Components & Parameters:**

#### `Card`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the card. <remarks> Typically contains CardHeader, CardContent, and CardFooter components. Can contain any Blazor markup. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the card. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |

#### `CardAction`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered in the card action area. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the card action area. |

#### `CardContent`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the card content area. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the card content. |

#### `CardDescription`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered as the card description. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the card description. |

#### `CardFooter`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the card footer. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the card footer. |

#### `CardHeader`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the card header. <remarks> Typically contains CardTitle, CardDescription, and optionally CardAction. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the card header. |

#### `CardTitle`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered as the card title. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the card title. |

**Basic Usage:**
```razor
<Card>
    <CardHeader>
        <CardTitle>Card Title</CardTitle>
        <CardDescription>Card description goes here</CardDescription>
    </CardHeader>
    <CardContent>
        Main content
    </CardContent>
    <CardFooter>
        Footer content
    </CardFooter>
</Card>
```

---

### Carousel

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Carousel`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Carousel
```

**Related Enums:**

- [`CarouselOrientation`](#carouselorientation-enum)

---

### Chart

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Chart`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Chart
```

**Related Enums:**

- [`ImageFormat`](#imageformat-enum)
- [`ChartEngine`](#chartengine-enum)
- [`AxisScale`](#axisscale-enum)
- [`TooltipMode`](#tooltipmode-enum)
- [`TooltipCursor`](#tooltipcursor-enum)
- [`LegendLayout`](#legendlayout-enum)
- [`LegendAlign`](#legendalign-enum)
- [`LegendVerticalAlign`](#legendverticalalign-enum)
- [`LegendIcon`](#legendicon-enum)
- [`Focus`](#focus-enum)
- [`BarLayout`](#barlayout-enum)
- [`GradientDirection`](#gradientdirection-enum)
- [`LabelPosition`](#labelposition-enum)
- [`InterpolationType`](#interpolationtype-enum)
- [`StackOffset`](#stackoffset-enum)
- [`PolarGridType`](#polargridtype-enum)
- [`YAxisPosition`](#yaxisposition-enum)
- [`LineStyleType`](#linestyletype-enum)
- [`SymbolShape`](#symbolshape-enum)
- [`RadarShape`](#radarshape-enum)
- [`AnimationType`](#animationtype-enum)
- [`AnimationEasing`](#animationeasing-enum)
- [`ChartType`](#charttype-enum)

---

### Checkbox

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Checkbox`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Checkbox
```

**Components & Parameters:**

#### `Checkbox`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Checked` | `bool` | `` |  | Gets or sets whether the checkbox is checked. <remarks> This property supports two-way binding using the @bind-Checked directive. Changes to this property trigger the CheckedChanged event callback.... |
| `CheckedChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback invoked when the checked state changes. <remarks> This event callback enables two-way binding with @bind-Checked. It is invoked whenever the user toggles the checkbox stat... |
| `Indeterminate` | `bool` | `` |  | Gets or sets whether the checkbox is in an indeterminate state. <remarks> The indeterminate state is typically used for "select all" checkboxes when only some child items are selected. When indeter... |
| `IndeterminateChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback invoked when the indeterminate state changes. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the checkbox is disabled. <remarks> When disabled: - Checkbox cannot be clicked or focused - Opacity is reduced - Pointer events are disabled - aria-disabled attribute is set t... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the checkbox. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the checkbox. <remarks> Provides accessible text for screen readers when the checkbox doesn't have associated label text. </remarks> |
| `Id` | `string?` | `` |  | Gets or sets the ID attribute for the checkbox element. <remarks> Used for associating the checkbox with label elements via htmlFor attribute. </remarks> |
| `Name` | `string?` | `` |  | Gets or sets the name of the checkbox for form submission. <remarks> This is critical for form submission. The name/value pair is submitted to the server. If not specified, falls back to the Id val... |
| `Required` | `bool` | `` |  | Gets or sets whether the checkbox is required. <remarks> When true, the checkbox must be checked for form submission. Works with form validation. </remarks> |
| `CheckedExpression` | `Expression<Func<bool>>?` | `` |  | Gets or sets an expression that identifies the bound value. <remarks> Used for form validation integration. When provided, the checkbox registers with the EditContext and participates in form valid... |

**Basic Usage:**
```razor
<Checkbox @bind-Checked="isAccepted" />

@code {
    private bool isAccepted = false;
}
```

---

### Collapsible

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Collapsible`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Collapsible
```

**Components & Parameters:**

#### `Collapsible`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Open` | `bool` | `` |  | Gets or sets a value indicating whether the collapsible is currently expanded. <value> <c>true</c> if the collapsible is open (content visible); otherwise, <c>false</c>. Default is <c>false</c>. </... |
| `OpenChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback invoked when the open state changes. <value> An <see cref="EventCallback{Boolean}"/> that receives the new open state, or <c>null</c> if no callback is provided. </value> |
| `Disabled` | `bool` | `` |  | Gets or sets a value indicating whether the collapsible is disabled. <value> <c>true</c> if the collapsible is disabled and cannot be toggled; otherwise, <c>false</c>. Default is <c>false</c>. </va... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the container element. <value> A string containing one or more CSS class names, or <c>null</c>. </value> <remarks> Use this parameter to customize th... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content to be rendered inside the collapsible container. <value> A <see cref="RenderFragment"/> containing the child components, or <c>null</c>. </value> <remarks> Typically ... |

#### `CollapsibleContent`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the content container element. <value> A string containing one or more CSS class names, or <c>null</c>. </value> <remarks> Use this parameter to styl... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered when the collapsible is expanded. <value> A <see cref="RenderFragment"/> containing the collapsible content, or <c>null</c>. </value> |

#### `CollapsibleTrigger`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the trigger button element. <value> A string containing one or more CSS class names, or <c>null</c>. </value> <remarks> Common Tailwind utilities for... |
| `AsChild` | `bool` | `false` |  | When true, the trigger does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the trigger. <value> A <see cref="RenderFragment"/> containing the trigger content, or <c>null</c>. </value> |

**Basic Usage:**
```razor
<Collapsible>
    <CollapsibleTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">
            Toggle
        </Button>
    </CollapsibleTrigger>
    <CollapsibleContent>
        <div class="p-4">
            This content can be collapsed
        </div>
    </CollapsibleContent>
</Collapsible>
```

---

### Combobox

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Combobox`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Combobox
```

**Components & Parameters:**

#### `Combobox`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `string?` | `` |  | Gets or sets the currently selected value. |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback that is invoked when the selected value changes. |
| `Placeholder` | `string` | `"Select an option..."` |  | Gets or sets the placeholder text shown in the button when no item is selected. |
| `SearchPlaceholder` | `string` | `"Search..."` |  | Gets or sets the placeholder text shown in the search input. |
| `EmptyMessage` | `string` | `"No results found."` |  | Gets or sets the message displayed when no items match the search. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the combobox container. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the combobox is disabled. |
| `PopoverWidth` | `string` | `"w-[200px]"` |  | Gets or sets the width of the popover content. <remarks> Defaults to "w-[200px]". Can be overridden with Tailwind classes. </remarks> |

**Basic Usage:**
```razor
<Combobox @bind-Value="selectedValue" Items="@items">
    <ComboboxTrigger>
        <ComboboxValue Placeholder="Select item..." />
    </ComboboxTrigger>
    <ComboboxContent>
        <ComboboxInput Placeholder="Search..." />
        <ComboboxList>
            <ComboboxEmpty>No results found.</ComboboxEmpty>
            @foreach (var item in items)
            {
                <ComboboxItem Value="@item.Value">
                    @item.Label
                </ComboboxItem>
            }
        </ComboboxList>
    </ComboboxContent>
</Combobox>

@code {
    private string selectedValue = "";
    private List<SelectItem> items = new()
    {
        new("apple", "Apple"),
        new("banana", "Banana"),
        new("orange", "Orange")
    };

    record SelectItem(string Value, string Label);
}
```

---

### Command

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Command`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Command
```

**Basic Usage:**
```razor
<Command>
    <CommandInput Placeholder="Type a command..." />
    <CommandList>
        <CommandEmpty>No results found.</CommandEmpty>
        <CommandGroup Heading="Suggestions">
            <CommandItem OnSelect="@(() => Navigate("/"))">
                <LucideIcon Name="house" Size="16" />
                <span>Dashboard</span>
            </CommandItem>
            <CommandItem OnSelect="@(() => Navigate("/settings"))">
                <LucideIcon Name="settings" Size="16" />
                <span>Settings</span>
            </CommandItem>
        </CommandGroup>
    </CommandList>
</Command>

@code {
    private void Navigate(string url)
    {
        NavigationManager.NavigateTo(url);
    }
}
```

---

### ContextMenu

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.ContextMenu`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.ContextMenu
```

---

### DataTable

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.DataTable`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.DataTable
```

**Components & Parameters:**

#### `DataTable`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Columns` | `RenderFragment?` | `` |  | Gets or sets the column definitions as child content. Use DataTableColumn components to define columns declaratively. |
| `SelectionMode` | `DataTableSelectionMode` | `DataTableSelectionMode.None` |  | Gets or sets the row selection mode. Default is None (no selection). |
| `ShowToolbar` | `bool` | `true` |  | Gets or sets whether to show the toolbar with global search and column visibility. Default is true. |
| `ShowPagination` | `bool` | `true` |  | Gets or sets whether to show pagination controls. Default is true. |
| `IsLoading` | `bool` | `` |  | Gets or sets whether the table is in a loading state. Default is false. |
| `PageSizes` | `int[]` | `{ 10, 20, 50, 100 }` |  | Gets or sets the available page size options. Default is [10, 20, 50, 100]. |
| `InitialPageSize` | `int` | `10` |  | Gets or sets the initial page size. Default is 10. |
| `ToolbarActions` | `RenderFragment?` | `` |  | Gets or sets custom toolbar actions (buttons, etc.). |
| `EmptyTemplate` | `RenderFragment?` | `` |  | Gets or sets a custom template for the empty state. If null, displays default "No results found" message. |
| `LoadingTemplate` | `RenderFragment?` | `` |  | Gets or sets a custom template for the loading state. If null, displays default "Loading..." message. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes for the container div. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the table. |
| `SelectedItems` | `IReadOnlyCollection<TData>` | `Array.Empty<TData>()` |  | Gets or sets the selected items. Use @bind-SelectedItems for two-way binding. |
| `SelectedItemsChanged` | `EventCallback<IReadOnlyCollection<TData>>` | `` |  | Event callback invoked when the selected items change. |
| `OnSort` | `EventCallback<(string ColumnId, SortDirection Direction)>` | `` |  | Event callback invoked when sorting changes. Use for custom sorting logic (hybrid mode). |

#### `DataTableColumn`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Id` | `string?` | `` |  | Gets or sets the unique identifier for this column. If not provided, it will be auto-generated from the Header. |
| `Sortable` | `bool` | `` |  | Gets or sets whether this column can be sorted. Default is false. |
| `Filterable` | `bool` | `` |  | Gets or sets whether this column can be filtered. Default is false. |
| `Visible` | `bool` | `true` |  | Gets or sets whether this column is currently visible. Default is true. |
| `Width` | `string?` | `` |  | Gets or sets the width of the column (e.g., "200px", "20%", "auto"). Null means the column will size automatically. |
| `MinWidth` | `string?` | `` |  | Gets or sets the minimum width of the column (e.g., "100px"). Useful for responsive layouts. |
| `MaxWidth` | `string?` | `` |  | Gets or sets the maximum width of the column (e.g., "400px"). Useful for preventing excessively wide columns. |
| `CellTemplate` | `RenderFragment<TData>?` | `` |  | Gets or sets a custom template for rendering cell values. If null, the value is rendered using ToString(). <remarks> The context parameter provides the data item (TData) for the row. </remarks> <ex... |
| `CellClass` | `string?` | `` |  | Gets or sets additional CSS classes to apply to cells in this column. |
| `HeaderClass` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the header cell. |

**Basic Usage:**
```razor
<DataTable Data="@users" @bind-State="tableState">
    <DataTableColumn TData="User" Property="u => u.Name" Title="Name" Sortable="true" />
    <DataTableColumn TData="User" Property="u => u.Email" Title="Email" />
    <DataTableColumn TData="User" Property="u => u.Role" Title="Role" />
</DataTable>

<TablePagination State="@tableState" />

@code {
    private List<User> users = new();
    private TableState tableState = new();

    record User(string Name, string Email, string Role);
}
```

**Related Enums:**

- [`DataTableSelectionMode`](#datatableselectionmode-enum)

---

### DatePicker

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.DatePicker`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.DatePicker
```

---

### Dialog

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Dialog`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Dialog
```

**Basic Usage:**
```razor
<Dialog>
    <DialogTrigger>Open Dialog</DialogTrigger>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Dialog Title</DialogTitle>
            <DialogDescription>Dialog description</DialogDescription>
        </DialogHeader>
        <p>Dialog content goes here</p>
        <DialogFooter>
            <DialogClose>Cancel</DialogClose>
            <Button>Confirm</Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

---

### DropdownMenu

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.DropdownMenu`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.DropdownMenu
```

**Basic Usage:**
```razor
<DropdownMenu>
    <DropdownMenuTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">
            Open Menu
        </Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent>
        <DropdownMenuItem>
            <LucideIcon Name="edit" Size="16" />
            Edit
        </DropdownMenuItem>
        <DropdownMenuItem>
            <LucideIcon Name="copy" Size="16" />
            Duplicate
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem>
            <LucideIcon Name="trash" Size="16" />
            Delete
        </DropdownMenuItem>
    </DropdownMenuContent>
</DropdownMenu>
```

---

### Empty

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Empty`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Empty
```

**Components & Parameters:**

#### `Empty`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the empty state. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the empty state. |

#### `EmptyActions`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

#### `EmptyDescription`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

#### `EmptyIcon`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

#### `EmptyTitle`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

---

### Field

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Field`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Field
```

**Components & Parameters:**

#### `Field`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Orientation` | `FieldOrientation` | `FieldOrientation.Vertical` |  | Gets or sets the orientation of the field layout. <remarks> Controls the layout direction and behavior: - Vertical: Stacks label above control (default, mobile-first) - Horizontal: Places label bes... |
| `IsInvalid` | `bool` | `` |  | Gets or sets whether the field is in an invalid/error state. <remarks> When true, applies error styling via the data-invalid attribute. This enables conditional styling for validation errors. Defau... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the field. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the field. <remarks> Typically contains FieldLabel, FieldContent, FieldDescription, and FieldError components for a complete form field. </remarks> |

#### `FieldContent`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the field content container. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </re... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the field content container. <remarks> Typically contains an input control, followed by optional FieldDescription and FieldError components. </remarks> |

#### `FieldDescription`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Id` | `string?` | `` |  | Gets or sets the ID for aria-describedby association. <remarks> When set, this ID should be referenced by the associated input's aria-describedby attribute for proper accessibility. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the description. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the description. <remarks> Typically contains helpful text explaining the purpose or format of the associated form field. </remarks> |

#### `FieldError`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Errors` | `IEnumerable<string>?` | `` |  | Gets or sets the array of error messages to display. <remarks> When provided, each error is rendered as a bulleted list item. If both Errors and ChildContent are provided, Errors takes precedence. ... |
| `Id` | `string?` | `` |  | Gets or sets the ID for aria-describedby association. <remarks> When set, this ID should be referenced by the associated input's aria-describedby attribute for proper accessibility. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the error container. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets custom error content to be rendered. <remarks> Used when you need custom error rendering beyond simple text messages. If both Errors and ChildContent are provided, Errors takes precede... |

#### `FieldGroup`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Orientation` | `FieldGroupOrientation` | `FieldGroupOrientation.Vertical` |  | Gets or sets the orientation of the field group layout. <remarks> Controls the layout direction: - Vertical: Stacks fields vertically (default) - Horizontal: Places fields horizontally with wrap su... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the field group. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the field group. <remarks> Typically contains multiple Field components. </remarks> |

#### `FieldLabel`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `For` | `string?` | `` |  | Gets or sets the ID of the form control this label is associated with. <remarks> Should match the ID of the input element for proper accessibility. Enables clicking the label to focus the associate... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the label. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the label. <remarks> Typically contains text describing the associated form control. Can also include required indicators, tooltips, etc. </remarks> |

#### `FieldLegend`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Variant` | `FieldLegendVariant` | `FieldLegendVariant.Legend` |  | Gets or sets the variant of the legend element. <remarks> Controls the HTML element used: - Legend: Renders a semantic legend element (use with FieldSet) - Label: Renders a div with role="group" (u... |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label when using Label variant. <remarks> Provides an accessible name for the group when variant is Label. Only applicable when Variant is FieldLegendVariant.Label. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the legend. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the legend. <remarks> Typically contains text describing the grouped form fields. </remarks> |

#### `FieldSeparator`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the separator. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to display in the center of the separator. <remarks> When provided, the content appears centered on top of the separator line with background color to create a visual break... |

#### `FieldSet`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the fieldset. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the fieldset. <remarks> Typically contains a FieldLegend followed by multiple Field components for grouped form controls. </remarks> |

#### `FieldTitle`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the title. <remarks> Custom classes are merged with the component's base classes, allowing for style overrides and extensions. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the title. <remarks> Typically contains text describing the field or group of controls. </remarks> |

**Basic Usage:**
```razor
<Field>
    <FieldLabel>Email</FieldLabel>
    <FieldContent>
        <Input Type="InputType.Email" @bind-Value="email" />
    </FieldContent>
    <FieldDescription>We'll never share your email.</FieldDescription>
</Field>

@code {
    private string email = "";
}
```

---

### Grid

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Grid`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Grid
```

**Components & Parameters:**

#### `Grid`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ActionHost` | `object?` | `` |  | Gets or sets the host component that contains grid action methods. Set this to 'this' to enable auto-discovery of methods marked with [GridAction]. |
| `Items` | `IEnumerable<TItem>` | `Array.Empty<TItem>()` |  | Gets or sets the collection of items to display in the grid. |
| `SelectionMode` | `GridSelectionMode` | `GridSelectionMode.None` |  | Gets or sets the selection mode for the grid. |
| `PagingMode` | `GridPagingMode` | `GridPagingMode.None` |  | Gets or sets the paging mode for the grid. |
| `PageSize` | `int` | `25` |  | Gets or sets the number of items per page. Must be greater than 0. Default is 25. |
| `IdField` | `string` | `"Id"` |  | Gets or sets the property name to use as the unique row identifier. This is critical for row selection persistence across data updates and pagination. Examples: "Id" (C# convention), "ProductId", "... |
| `VirtualizationMode` | `GridVirtualizationMode` | `GridVirtualizationMode.Auto` |  | Gets or sets the virtualization mode for the grid. |
| `Theme` | `GridTheme` | `GridTheme.Shadcn` |  | Gets or sets the AG Grid theme to use (Shadcn, Alpine, Balham, Material, Quartz). Default is Shadcn. |
| `VisualStyle` | `GridStyle` | `GridStyle.Default` |  | Gets or sets the visual style modifiers for the grid (Default, Striped, Bordered, Minimal). These modifiers work with any AG Grid theme. |
| `Density` | `GridDensity` | `GridDensity.Comfortable` |  | Gets or sets the spacing density for the grid. |
| `SuppressHeaderMenus` | `bool` | `false` |  | Gets or sets whether to suppress the header menus (filter/column menu). When true, columns will not show the menu icon even if filterable/sortable. This is useful for controlled filtering scenarios... |
| `State` | `GridState?` | `` |  | Gets or sets the current state of the grid. Supports two-way binding via @bind-State for automatic state synchronization. |
| `StateChanged` | `EventCallback<GridState>` | `` |  | Gets or sets the callback invoked when the grid state changes. Used for two-way binding support (@bind-State). |
| `IsLoading` | `bool` | `` |  | Gets or sets whether the grid is in a loading state. |
| `Columns` | `RenderFragment?` | `` |  | Gets or sets the child content containing GridColumn definitions. |
| `LoadingTemplate` | `RenderFragment?` | `` |  | Gets or sets the template to display while the grid is loading. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the grid container. |
| `Height` | `string?` | `` |  | Gets or sets the height of the grid. Can be a fixed value (e.g., "500px") or a percentage (e.g., "100%"). If not specified, defaults to "300px". |
| `Width` | `string?` | `` |  | Gets or sets the width of the grid. Can be a fixed value (e.g., "800px") or a percentage (e.g., "100%"). Defaults to "100%". |
| `InlineStyle` | `string?` | `` |  | Gets or sets inline styles to apply to the grid container. |
| `LocalizationKeyPrefix` | `string?` | `` |  | Gets or sets the localization key prefix for grid text resources. |
| `OnStateChanged` | `EventCallback<GridState>` | `` |  | Gets or sets the callback invoked when the grid state changes. |
| `RowModelType` | `GridRowModelType` | `GridRowModelType.ClientSide` |  | Gets or sets the row model type for the grid. Default is ClientSide. Use ServerSide for server-side data fetching with sorting/filtering/pagination. |
| `OnDataRequest` | `EventCallback<GridDataRequest<TItem>>` | `` |  | Gets or sets the callback invoked when server-side data is requested (legacy EventCallback version). For new code, use OnServerDataRequest (Func) instead. |
| `OnSelectionChanged` | `EventCallback<IReadOnlyCollection<TItem>>` | `` |  | Gets or sets the callback invoked when the selection changes. |
| `SelectedItems` | `IReadOnlyCollection<TItem>` | `Array.Empty<TItem>()` |  | Gets or sets the selected items in the grid. |
| `SelectedItemsChanged` | `EventCallback<IReadOnlyCollection<TItem>>` | `` |  | Gets or sets the callback invoked when the selected items change (for two-way binding). |

#### `GridColumn`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Id` | `string?` | `` |  | Gets or sets the unique identifier for this column. If not specified, it will be generated from Field or Header. |
| `Field` | `string?` | `` |  | Gets or sets the field name for data binding (e.g., "CustomerName"). This is a string field name, NOT a lambda expression. |
| `Sortable` | `bool` | `` |  | Gets or sets whether the column is sortable. |
| `Filterable` | `bool` | `` |  | Gets or sets whether the column is filterable. |
| `Width` | `string?` | `` |  | Gets or sets the column width (e.g., "100px", "20%"). |
| `MinWidth` | `string?` | `` |  | Gets or sets the minimum column width. |
| `MaxWidth` | `string?` | `` |  | Gets or sets the maximum column width. |
| `Pinned` | `GridColumnPinPosition` | `GridColumnPinPosition.None` |  | Gets or sets the column pinning position. |
| `AllowResize` | `bool` | `true` |  | Gets or sets whether the column can be resized. |
| `AllowReorder` | `bool` | `true` |  | Gets or sets whether the column can be reordered. |
| `IsVisible` | `bool` | `true` |  | Gets or sets whether the column is visible. |
| `CellTemplate` | `RenderFragment<TItem>?` | `` |  | Gets or sets the cell template for rendering cell content. |
| `HeaderTemplate` | `RenderFragment?` | `` |  | Gets or sets the header template for custom header rendering. |
| `FilterTemplate` | `RenderFragment?` | `` |  | Gets or sets the filter template for custom filter UI. |
| `CellEditTemplate` | `RenderFragment<TItem>?` | `` |  | Gets or sets the cell edit template for inline editing. |
| `CellClass` | `string?` | `` |  | Gets or sets the CSS class to apply to cells in this column. |
| `HeaderClass` | `string?` | `` |  | Gets or sets the CSS class to apply to the column header. |
| `DataFormatString` | `string?` | `` |  | Gets or sets the format string for displaying cell values. Supports standard .NET format strings (e.g., "C" for currency, "N2" for numbers with 2 decimals, "d" for dates). Can also use composite fo... |

#### `GridThemeParameters`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Spacing` | `int?` | `` |  | Gets or sets the base spacing unit in pixels. Controls padding and margins throughout the grid. Default varies by density: Compact=3, Comfortable=4, Spacious=6 <example>4</example> |
| `RowHeight` | `int?` | `` |  | Gets or sets the row height in pixels. Default varies by density: Compact=28, Comfortable=42, Spacious=56 <example>42</example> |
| `HeaderHeight` | `int?` | `` |  | Gets or sets the header height in pixels. Default varies by density: Compact=32, Comfortable=48, Spacious=64 <example>48</example> |
| `IconSize` | `int?` | `` |  | Gets or sets the size of icons in pixels. Default varies by density: Compact=14, Comfortable=16, Spacious=20 <example>16</example> |
| `InputHeight` | `int?` | `` |  | Gets or sets the height of input elements (filters, editors) in pixels. Default varies by density: Compact=28, Comfortable=32, Spacious=40 <example>32</example> |
| `ToggleButtonWidth` | `int?` | `` |  | Gets or sets the width of toggle buttons in pixels. <example>28</example> |
| `ToggleButtonHeight` | `int?` | `` |  | Gets or sets the height of toggle buttons in pixels. <example>28</example> |
| `AccentColor` | `string?` | `` |  | Gets or sets the primary accent color used for active states, selections, and focus indicators. Accepts CSS color values (hex, rgb, hsl, oklch, etc.) or CSS variables. Default for Shadcn theme: var... |
| `BackgroundColor` | `string?` | `` |  | Gets or sets the background color for cells and the grid body. Default for Shadcn theme: var(--background) <example>"#ffffff" or "var(--background)"</example> |
| `ForegroundColor` | `string?` | `` |  | Gets or sets the default text color for grid content. Default for Shadcn theme: var(--foreground) <example>"#000000" or "var(--foreground)"</example> |
| `BorderColor` | `string?` | `` |  | Gets or sets the color of borders and dividing lines. Default for Shadcn theme: var(--border) <example>"#e5e7eb" or "var(--border)"</example> |
| `HeaderBackgroundColor` | `string?` | `` |  | Gets or sets the background color for column headers. Default for Shadcn theme: var(--muted) <example>"#f9fafb" or "var(--muted)"</example> |
| `HeaderForegroundColor` | `string?` | `` |  | Gets or sets the text color for column headers. Default for Shadcn theme: var(--foreground) <example>"#000000" or "var(--foreground)"</example> |
| `RowHoverColor` | `string?` | `` |  | Gets or sets the background color when hovering over a row. Default for Shadcn theme: color-mix(in srgb, var(--accent) 10%, transparent) <example>"rgba(37, 99, 235, 0.1)" or "color-mix(in srgb, var... |
| `OddRowBackgroundColor` | `string?` | `` |  | Gets or sets the background color for odd rows (used with Striped style). Default for Shadcn theme with Striped style: color-mix(in srgb, var(--muted) 30%, transparent) <example>"#f9fafb" or "color... |
| `SelectedRowBackgroundColor` | `string?` | `` |  | Gets or sets the background color for selected rows. Default for Shadcn theme: color-mix(in srgb, var(--primary) 20%, transparent) <example>"rgba(37, 99, 235, 0.2)" or "color-mix(in srgb, var(--pri... |
| `RangeSelectionBorderColor` | `string?` | `` |  | Gets or sets the border color for range selections. <example>"#2563eb"</example> |
| `CellTextColor` | `string?` | `` |  | Gets or sets the text color for cell content. <example>"#000000"</example> |
| `InvalidColor` | `string?` | `` |  | Gets or sets the color used to indicate validation errors. Default for Shadcn theme: var(--destructive) <example>"#dc2626" or "var(--destructive)"</example> |
| `ModalOverlayBackgroundColor` | `string?` | `` |  | Gets or sets the background color for modal overlays. <example>"rgba(0, 0, 0, 0.5)"</example> |
| `ChromeBackgroundColor` | `string?` | `` |  | Gets or sets the background color for UI chrome elements (panels, toolbars). <example>"#f9fafb"</example> |
| `TooltipBackgroundColor` | `string?` | `` |  | Gets or sets the background color for tooltips. Default for Shadcn theme: var(--popover) <example>"#ffffff" or "var(--popover)"</example> |
| `TooltipTextColor` | `string?` | `` |  | Gets or sets the text color for tooltips. Default for Shadcn theme: var(--popover-foreground) <example>"#000000" or "var(--popover-foreground)"</example> |
| `FontFamily` | `string?` | `` |  | Gets or sets the font family for the grid. Default for Shadcn theme: var(--font-sans) <example>"Inter, system-ui, sans-serif" or "var(--font-sans)"</example> |
| `FontSize` | `int?` | `` |  | Gets or sets the base font size in pixels. Default varies by density: Compact=12, Comfortable=14, Spacious=16 <example>14</example> |
| `HeaderFontSize` | `int?` | `` |  | Gets or sets the font size for column headers in pixels. <example>14</example> |
| `HeaderFontWeight` | `object?` | `` |  | Gets or sets the font weight for column headers. <example>600 or "bold"</example> |
| `Borders` | `bool?` | `` |  | Gets or sets whether to show borders between cells and rows. Default varies by style: Default=true, Striped=true, Bordered=true, Minimal=false <example>true</example> |
| `BorderRadius` | `int?` | `` |  | Gets or sets the border radius in pixels for UI elements. Default for Shadcn theme: 4 <example>4</example> |
| `WrapperBorder` | `bool?` | `` |  | Gets or sets whether to show a border around the entire grid wrapper. Default varies by style: Default=false, Striped=false, Bordered=true, Minimal=false <example>false</example> |
| `WrapperBorderRadius` | `int?` | `` |  | Gets or sets the border radius in pixels for the grid wrapper. <example>8</example> |

**Related Enums:**

- [`GridDensity`](#griddensity-enum)
- [`GridTheme`](#gridtheme-enum)
- [`GridStyle`](#gridstyle-enum)
- [`GridRowModelType`](#gridrowmodeltype-enum)
- [`GridSortDirection`](#gridsortdirection-enum)
- [`GridFilterOperator`](#gridfilteroperator-enum)
- [`GridPagingMode`](#gridpagingmode-enum)
- [`GridSelectionMode`](#gridselectionmode-enum)
- [`GridColumnPinPosition`](#gridcolumnpinposition-enum)
- [`GridVirtualizationMode`](#gridvirtualizationmode-enum)

---

### HeightAnimation

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.HeightAnimation`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.HeightAnimation
```

---

### HoverCard

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.HoverCard`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.HoverCard
```

**Basic Usage:**
```razor
<HoverCard>
    <HoverCardTrigger AsChild>
        <a href="#" class="underline">@@username</a>
    </HoverCardTrigger>
    <HoverCardContent>
        <div class="flex gap-4">
            <Avatar>
                <AvatarImage Src="/avatar.jpg" />
                <AvatarFallback>JD</AvatarFallback>
            </Avatar>
            <div>
                <h4 class="text-sm font-semibold">John Doe</h4>
                <p class="text-sm text-muted-foreground">@@johndoe</p>
                <p class="text-xs mt-2">Software developer and open source contributor</p>
            </div>
        </div>
    </HoverCardContent>
</HoverCard>
```

---

### Input

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Input`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Input
```

**Components & Parameters:**

#### `Input`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Type` | `InputType` | `InputType.Text` |  | Gets or sets the type of input. <remarks> Determines the HTML input type attribute. Default value is <see cref="InputType.Text"/>. </remarks> |
| `Value` | `string?` | `` |  | Gets or sets the current value of the input. <remarks> Supports two-way binding via @bind-Value syntax. </remarks> |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback invoked when the input value changes. <remarks> This event is fired on every keystroke (oninput event). Use with Value parameter for two-way binding. </remarks> |
| `Placeholder` | `string?` | `` |  | Gets or sets the placeholder text displayed when the input is empty. <remarks> Provides a hint to the user about what to enter. Should not be used as a replacement for a label. </remarks> |
| `Disabled` | `bool` | `` |  | Gets or sets whether the input is disabled. <remarks> When disabled: - Input cannot be focused or edited - Cursor is set to not-allowed - Opacity is reduced for visual feedback </remarks> |
| `Required` | `bool` | `` |  | Gets or sets whether the input is required. <remarks> When true, the HTML5 required attribute is set. Works with form validation and :invalid CSS pseudo-class. </remarks> |
| `Name` | `string?` | `` |  | Gets or sets the name of the input for form submission. <remarks> This is critical for form submission. The name/value pair is submitted to the server. Should be unique within the form. </remarks> |
| `Autocomplete` | `string?` | `` |  | Gets or sets the autocomplete hint for the browser. <remarks> Examples: "email", "username", "current-password", "new-password", "name", "tel", "off". Helps browsers provide appropriate autofill su... |
| `Readonly` | `bool` | `` |  | Gets or sets whether the input is read-only. <remarks> When true, the user cannot modify the value, but it's still focusable and submitted with forms. Different from Disabled - readonly inputs are ... |
| `MaxLength` | `int?` | `` |  | Gets or sets the maximum number of characters allowed. <remarks> When set, the browser will prevent users from entering more characters. Applies to text, email, password, tel, url, and search types... |
| `MinLength` | `int?` | `` |  | Gets or sets the minimum number of characters required. <remarks> Works with form validation. Applies to text, email, password, tel, url, and search types. </remarks> |
| `Min` | `string?` | `` |  | Gets or sets the minimum value for number, date, or time inputs. <remarks> Applies to number, date, time inputs. Works with form validation and :invalid pseudo-class. </remarks> |
| `Max` | `string?` | `` |  | Gets or sets the maximum value for number, date, or time inputs. <remarks> Applies to number, date, time inputs. Works with form validation and :invalid pseudo-class. </remarks> |
| `Step` | `string?` | `` |  | Gets or sets the step interval for number inputs. <remarks> Defines the granularity of values (e.g., "0.01" for currency, "1" for integers). Applies to number, date, time inputs. </remarks> |
| `Pattern` | `string?` | `` |  | Gets or sets the regex pattern for validation. <remarks> Validates input against the specified regular expression. Works with form validation and :invalid pseudo-class. </remarks> |
| `InputMode` | `string?` | `` |  | Gets or sets the input mode hint for mobile keyboards. <remarks> Examples: "none", "text", "decimal", "numeric", "tel", "search", "email", "url". Helps mobile devices show the appropriate keyboard.... |
| `Autofocus` | `bool` | `` |  | Gets or sets whether the input should be auto-focused when the page loads. <remarks> Only one element per page should have autofocus. Improves accessibility when used appropriately. </remarks> |
| `Spellcheck` | `bool?` | `` |  | Gets or sets whether spell checking is enabled. <remarks> Can be true, false, or null (browser default). Useful for controlling spell checking on email addresses, usernames, etc. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the input. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `Id` | `string?` | `` |  | Gets or sets the HTML id attribute for the input element. <remarks> Used to associate the input with a label element via the label's 'for' attribute. This is essential for accessibility and allows ... |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the input. <remarks> Provides an accessible name for screen readers. Use when there is no visible label element. </remarks> |
| `AriaDescribedBy` | `string?` | `` |  | Gets or sets the ID of the element that describes the input. <remarks> References the id of an element containing help text or error messages. Improves screen reader experience by associating descr... |
| `AriaInvalid` | `bool?` | `` |  | Gets or sets whether the input value is invalid. <remarks> When true, aria-invalid="true" is set. Should be set based on validation state. </remarks> |

**Basic Usage:**
```razor
<Input @bind-Value="username" Placeholder="Enter your username" />

@code {
    private string username = "";
}
```

**Related Enums:**

- [`InputType`](#inputtype-enum)

---

### InputGroup

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.InputGroup`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.InputGroup
```

**Components & Parameters:**

#### `InputGroup`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content to be rendered inside the input group. <remarks> Typically contains InputGroupInput/InputGroupTextarea and InputGroupAddon components. The order of child components a... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the input group container. <remarks> Custom classes are merged with the component's base classes using TailwindMerge, allowing for intelligent confli... |

#### `InputGroupAddon`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Align` | `InputGroupAlign` | `InputGroupAlign.InlineStart` |  | Gets or sets the alignment position of the addon. <remarks> Determines where the addon content appears relative to the input: - InlineStart: Left side (or right in RTL) - InlineEnd: Right side (or ... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content to be rendered inside the addon. <remarks> Can contain icons, buttons, text, or any other content. The component automatically adjusts padding based on content type. ... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the addon container. |

#### `InputGroupButton`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Type` | `ButtonType` | `ButtonType.Button` |  | Gets or sets the button type (submit, button, reset). |
| `Variant` | `ButtonVariant` | `ButtonVariant.Default` |  | Gets or sets the button visual variant. |
| `Size` | `ButtonSize` | `ButtonSize.Small` |  | Gets or sets the button size. <remarks> Default is Small for better proportions within input groups. </remarks> |
| `Disabled` | `bool` | `` |  | Gets or sets whether the button is disabled. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for accessibility. |
| `OnClick` | `EventCallback` | `` |  | Gets or sets the click event handler. |
| `Icon` | `RenderFragment?` | `` |  | Gets or sets the icon to display in the button. |
| `IconPosition` | `IconPosition` | `IconPosition.Start` |  | Gets or sets the position of the icon relative to button text. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content (button text). |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes. |

#### `InputGroupInput`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Type` | `InputType` | `InputType.Text` |  | Gets or sets the type of input. |
| `Value` | `string?` | `` |  | Gets or sets the current value of the input. |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback invoked when the input value changes. |
| `Placeholder` | `string?` | `` |  | Gets or sets the placeholder text. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the input is disabled. |
| `Required` | `bool` | `` |  | Gets or sets whether the input is required. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes. |
| `Id` | `string?` | `` |  | Gets or sets the HTML id attribute. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label. |
| `AriaDescribedBy` | `string?` | `` |  | Gets or sets the ARIA described-by attribute. |
| `AriaInvalid` | `bool?` | `` |  | Gets or sets whether the input value is invalid. |

#### `InputGroupText`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content (text or icons). |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes. |

#### `InputGroupTextarea`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `string?` | `` |  | Gets or sets the current value of the textarea. |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback invoked when the textarea value changes. |
| `Rows` | `int` | `3` |  | Gets or sets the number of visible text rows. <remarks> Default is 3 rows. The textarea can grow beyond this if resize is enabled. </remarks> |
| `Placeholder` | `string?` | `` |  | Gets or sets the placeholder text. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the textarea is disabled. |
| `Required` | `bool` | `` |  | Gets or sets whether the textarea is required. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes. |
| `Id` | `string?` | `` |  | Gets or sets the HTML id attribute. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label. |
| `AriaDescribedBy` | `string?` | `` |  | Gets or sets the ARIA described-by attribute. |
| `AriaInvalid` | `bool?` | `` |  | Gets or sets whether the textarea value is invalid. |

**Basic Usage:**
```razor
@using BlazorUI.Components.InputGroup

<InputGroup>
    <InputGroupAddon Align="InputGroupAlign.InlineStart">
        <LucideIcon Name="search" Size="16" />
    </InputGroupAddon>
    <InputGroupInput Placeholder="Search..." />
</InputGroup>
```

**Related Enums:**

- [`InputGroupAlign`](#inputgroupalign-enum)

---

### InputOtp

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.InputOtp`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.InputOtp
```

---

### Item

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Item`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Item
```

**Components & Parameters:**

#### `Item`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Variant` | `ItemVariant` | `ItemVariant.Default` |  | Gets or sets the visual style variant of the item. |
| `Size` | `ItemSize` | `ItemSize.Default` |  | Gets or sets the size of the item. |
| `AsChild` | `string?` | `` |  | Gets or sets the element type to render as (e.g., "a", "button"). When set, the component renders as that element instead of a div. |
| `Href` | `string?` | `` |  | Gets or sets the href attribute when rendering as an anchor. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the item. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the item. |
| `DataSlot` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |
| `DataSlot` | `string?` | `` |  |  |
| `href` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |
| `DataSlot` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

#### `ItemActions`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the actions container. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the actions container. |

#### `ItemContent`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the content container. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the container. |

#### `ItemDescription`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the description. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered as the description. |

#### `ItemFooter`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the footer. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered in the footer. |

#### `ItemGroup`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the container. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the group. |

#### `ItemHeader`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the header. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered in the header. |

#### `ItemMedia`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Variant` | `ItemMediaVariant` | `ItemMediaVariant.Default` |  | Gets or sets the visual style variant of the media. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the media container. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the media container. |

#### `ItemSeparator`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the separator. |

#### `ItemTitle`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the title. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered as the title. |

**Basic Usage:**
```razor
<Item>
    <ItemMedia>
        <Avatar>
            <AvatarFallback>JD</AvatarFallback>
        </Avatar>
    </ItemMedia>
    <ItemContent>
        <ItemTitle>John Doe</ItemTitle>
        <ItemDescription>john@example.com</ItemDescription>
    </ItemContent>
    <ItemAction>
        <Button Size="ButtonSize.Icon" Variant="ButtonVariant.Ghost">
            <LucideIcon Name="more-horizontal" Size="16" />
        </Button>
    </ItemAction>
</Item>
```

**Related Enums:**

- [`ItemMediaVariant`](#itemmediavariant-enum)
- [`ItemVariant`](#itemvariant-enum)
- [`ItemSize`](#itemsize-enum)

---

### Kbd

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Kbd`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Kbd
```

**Components & Parameters:**

#### `Kbd`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the kbd element. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the kbd element. <remarks> Typically contains a single key name (e.g., "Ctrl", "Shift", "Enter") or a key symbol (e.g., "⌘", "⌥", "⇧"). </remarks> |

---

### Label

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Label`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Label
```

**Components & Parameters:**

#### `Label`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `For` | `string?` | `` |  | Gets or sets the ID of the form element this label is associated with. <value> A string containing the ID of the target form control, or <c>null</c>. </value> <remarks> This parameter maps to the H... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the label element. <value> A string containing one or more CSS class names, or <c>null</c>. </value> <remarks> Use this parameter to customize the la... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the label element. <value> A <see cref="RenderFragment"/> containing the label's content, or <c>null</c>. </value> <remarks> Typically contains the la... |

**Basic Usage:**
```razor
<Label For="email">Email Address</Label>
<Input Id="email" Type="InputType.Email" />
```

---

### MarkdownEditor

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.MarkdownEditor`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.MarkdownEditor
```

**Components & Parameters:**

#### `MarkdownEditor`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `string?` | `` |  | Gets or sets the markdown content value. |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback invoked when the value changes. |
| `Placeholder` | `string?` | `` |  | Gets or sets the placeholder text displayed when the editor is empty. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the editor is disabled. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the editor container. |
| `Id` | `string?` | `` |  | Gets or sets the HTML id attribute for the textarea element. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the textarea. |
| `AriaDescribedBy` | `string?` | `` |  | Gets or sets the ID of the element that describes the textarea. |
| `AriaInvalid` | `bool?` | `` |  | Gets or sets whether the textarea value is invalid. |

**Basic Usage:**
```razor
@using BlazorUI.Components.MarkdownEditor

<MarkdownEditor @bind-Value="markdown" Placeholder="Write something..." />

@code {
    private string? markdown;
}
```

---

### Menubar

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Menubar`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Menubar
```

---

### Motion

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Motion`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Motion
```

---

### MultiSelect

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.MultiSelect`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.MultiSelect
```

**Components & Parameters:**

#### `MultiSelect`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Values` | `IEnumerable<string>?` | `` |  | Gets or sets the currently selected values. |
| `ValuesChanged` | `EventCallback<IEnumerable<string>?>` | `` |  | Gets or sets the callback that is invoked when the selected values change. |
| `Placeholder` | `string` | `"Select items..."` |  | Gets or sets the placeholder text shown when no items are selected. |
| `SearchPlaceholder` | `string` | `"Search..."` |  | Gets or sets the placeholder text shown in the search input. |
| `EmptyMessage` | `string` | `"No results found."` |  | Gets or sets the message displayed when no items match the search. |
| `SelectAllLabel` | `string` | `"Select All"` |  | Gets or sets the label for the Select All option. |
| `ShowSelectAll` | `bool` | `true` |  | Gets or sets whether to show the Select All option. |
| `ClearLabel` | `string` | `"Clear"` |  | Gets or sets the label for the Clear button. |
| `CloseLabel` | `string` | `"Close"` |  | Gets or sets the label for the Close button. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the multiselect container. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the multiselect is disabled. |
| `MaxDisplayTags` | `int` | `3` |  | Gets or sets the maximum number of tags to display before showing "+N more". |
| `PopoverWidth` | `string` | `"w-[300px]"` |  | Gets or sets the width of the popover content. |
| `ValuesExpression` | `Expression<Func<IEnumerable<string>?>>?` | `` |  | Gets or sets an expression that identifies the bound values. Used for form validation integration. |

---

### NativeSelect

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.NativeSelect`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.NativeSelect
```

**Components & Parameters:**

#### `NativeSelect`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `string?` | `` |  | Gets or sets the selected value. |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback that is invoked when the value changes. |
| `Id` | `string?` | `` |  | Gets or sets the id attribute for the select element. |
| `Name` | `string?` | `` |  | Gets or sets the name attribute for the select element. |
| `Placeholder` | `string?` | `` |  | Gets or sets the placeholder text when no option is selected. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the select is disabled. |
| `Required` | `bool` | `` |  | Gets or sets whether the select is required. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the select. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the select (options). |

---

### NavigationMenu

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.NavigationMenu`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.NavigationMenu
```

---

### Pagination

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Pagination`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Pagination
```

**Components & Parameters:**

#### `Pagination`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `AriaLabel` | `string` | `"pagination"` |  | Gets or sets the aria-label for the pagination navigation. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the pagination. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the pagination. |

#### `PaginationContent`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

#### `PaginationEllipsis`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  |  |

#### `PaginationItem`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Class` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

#### `PaginationLink`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Href` | `string?` | `` |  |  |
| `IsActive` | `bool` | `` |  |  |
| `Class` | `string?` | `` |  |  |
| `ChildContent` | `RenderFragment?` | `` |  |  |

#### `PaginationNext`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Href` | `string?` | `` |  |  |
| `Class` | `string?` | `` |  |  |

#### `PaginationPrevious`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Href` | `string?` | `` |  |  |
| `Class` | `string?` | `` |  |  |

---

### Popover

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Popover`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Popover
```

**Basic Usage:**
```razor
<Popover>
    <PopoverTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">Open Popover</Button>
    </PopoverTrigger>
    <PopoverContent>
        <p>Popover content goes here</p>
    </PopoverContent>
</Popover>
```

---

### Progress

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Progress`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Progress
```

**Components & Parameters:**

#### `Progress`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `double` | `` |  | Gets or sets the current progress value. |
| `Max` | `double` | `100` |  | Gets or sets the maximum value for the progress bar. <remarks> Default value is 100. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the progress bar. |

---

### RadioGroup

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.RadioGroup`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.RadioGroup
```

**Components & Parameters:**

#### `RadioGroup`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `TValue` | `default!` |  | Gets or sets the currently selected value. <remarks> This property supports two-way binding using the @bind-Value directive. Changes to this property trigger the ValueChanged event callback. </rema... |
| `ValueChanged` | `EventCallback<TValue>` | `` |  | Gets or sets the callback invoked when the selected value changes. <remarks> This event callback enables two-way binding with @bind-Value. It is invoked whenever a radio button is selected. </remarks> |
| `Disabled` | `bool` | `` |  | Gets or sets whether the entire radio group is disabled. <remarks> When disabled, all radio items in the group cannot be selected. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the radio group container. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the radio group. <remarks> Provides accessible text for screen readers to describe the purpose of the radio group. </remarks> |
| `Name` | `string?` | `` |  | Gets or sets the name for all radio items in this group. <remarks> This name is shared by all radio items in the group, making them mutually exclusive. Critical for form submission - the selected v... |
| `Required` | `bool` | `` |  | Gets or sets whether a selection is required in this radio group. <remarks> When true, the user must select one of the radio items for form submission. Works with form validation. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the radio group. <remarks> Should contain RadioGroupItem components. </remarks> |
| `ValueExpression` | `Expression<Func<TValue>>?` | `` |  | Gets or sets an expression that identifies the bound value. <remarks> Used for form validation integration. When provided, the radio group registers with the EditContext and participates in form va... |

#### `RadioGroupItem`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Disabled` | `bool` | `` |  | Gets or sets whether this individual radio item is disabled. <remarks> When disabled, the item cannot be selected and appears with reduced opacity. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the radio item. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the radio item. <remarks> Provides accessible text for screen readers when the radio item doesn't have an associated label element. </remarks> |
| `Id` | `string?` | `` |  | Gets or sets the ID attribute for the radio item element. <remarks> Used for associating the radio item with label elements via htmlFor attribute. </remarks> |

**Basic Usage:**
```razor
<RadioGroup @bind-Value="selectedOption">
    <RadioGroupItem Value="option1" Id="opt1" />
    <RadioGroupItem Value="option2" Id="opt2" />
    <RadioGroupItem Value="option3" Id="opt3" />
</RadioGroup>

@code {
    private string selectedOption = "option1";
}
```

---

### Resizable

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Resizable`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Resizable
```

**Related Enums:**

- [`ResizableDirection`](#resizabledirection-enum)

---

### RichTextEditor

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.RichTextEditor`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.RichTextEditor
```

**Components & Parameters:**

#### `RichTextEditor`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `string?` | `` |  | Gets or sets the HTML content value. |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback invoked when the value changes. |
| `Placeholder` | `string?` | `` |  | Gets or sets the placeholder text displayed when the editor is empty. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the editor is disabled. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the editor container. |
| `Id` | `string?` | `` |  | Gets or sets the HTML id attribute for the contenteditable element. |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the editor. |
| `AriaDescribedBy` | `string?` | `` |  | Gets or sets the ID of the element that describes the editor. |
| `AriaInvalid` | `bool?` | `` |  | Gets or sets whether the editor value is invalid. |
| `MinHeight` | `string` | `"150px"` |  | Gets or sets the minimum height of the editor content area. |
| `MaxHeight` | `string?` | `` |  | Gets or sets the maximum height of the editor content area. When content exceeds this height, a scrollbar appears. |
| `Height` | `string?` | `` |  | Gets or sets a fixed height for the editor content area. When set, the editor will not auto-expand and will show scrollbar when content overflows. Takes precedence over MinHeight/MaxHeight when set. |

**Basic Usage:**
```razor
@using BlazorUI.Components.RichTextEditor

<RichTextEditor @bind-Value="content" Placeholder="Start typing..." />

@code {
    private string? content;
}
```

---

### ScrollArea

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.ScrollArea`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.ScrollArea
```

**Related Enums:**

- [`ScrollAreaType`](#scrollareatype-enum)
- [`Orientation`](#orientation-enum)

---

### Select

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Select`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Select
```

**Basic Usage:**
```razor
<Select @bind-Value="selectedFruit">
    <SelectTrigger>
        <SelectValue Placeholder="Select a fruit..." />
    </SelectTrigger>
    <SelectContent>
        <SelectItem Value="apple">Apple</SelectItem>
        <SelectItem Value="banana">Banana</SelectItem>
        <SelectItem Value="orange">Orange</SelectItem>
    </SelectContent>
</Select>

@code {
    private string selectedFruit = "";
}
```

---

### Separator

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Separator`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Separator
```

**Components & Parameters:**

#### `Separator`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Orientation` | `SeparatorOrientation` | `SeparatorOrientation.Horizontal` |  | Gets or sets the orientation of the separator. <remarks> Determines whether the separator is displayed horizontally or vertically. Default value is <see cref="SeparatorOrientation.Horizontal"/>. </... |
| `Decorative` | `bool` | `true` |  | Gets or sets whether the separator is purely decorative. <remarks> <para> When true (default), the separator is treated as decorative (role="none") and hidden from assistive technologies. </para> <... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the separator. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |

**Basic Usage:**
```razor
<Separator />
```

**Related Enums:**

- [`SeparatorOrientation`](#separatororientation-enum)

---

### Sheet

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Sheet`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Sheet
```

**Basic Usage:**
```razor
<Sheet>
    <SheetTrigger AsChild>
        <Button>Open Sheet</Button>
    </SheetTrigger>
    <SheetContent>
        <SheetHeader>
            <SheetTitle>Sheet Title</SheetTitle>
            <SheetDescription>Sheet description</SheetDescription>
        </SheetHeader>
        <p>Sheet content</p>
        <SheetFooter>
            <SheetClose AsChild>
                <Button>Close</Button>
            </SheetClose>
        </SheetFooter>
    </SheetContent>
</Sheet>
```

---

### Sidebar

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Sidebar`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Sidebar
```

**Components & Parameters:**

#### `Sidebar`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | The content to render inside the sidebar. |
| `Class` | `string?` | `` |  | Additional CSS classes to apply to the sidebar. |
| `Collapsible` | `bool` | `true` |  | Collapsible behavior: icon-only when collapsed, full width when expanded. Default is true. |
| `AutoDetectActive` | `bool` | `false` |  | Whether menu items should automatically detect their active state based on current URL. When enabled, all SidebarMenuButton and SidebarMenuSubButton components will automatically highlight based on... |

**Basic Usage:**
```razor
<SidebarProvider>
    <Sidebar>
        <SidebarHeader>
            <SidebarHeaderContent>App Name</SidebarHeaderContent>
        </SidebarHeader>

        <SidebarContent>
            <SidebarMenu>
                <SidebarMenuItem>
                    <SidebarMenuButton Href="/" Match="NavLinkMatch.All">
                        <LucideIcon Name="house" Size="16" />
                        <span>Home</span>
                    </SidebarMenuButton>
                </SidebarMenuItem>

                <SidebarMenuItem>
                    <SidebarMenuButton Href="/settings" Match="NavLinkMatch.All">
                        <LucideIcon Name="settings" Size="16" />
                        <span>Settings</span>
                    </SidebarMenuButton>
                </SidebarMenuItem>
            </SidebarMenu>
        </SidebarContent>
    </Sidebar>

    <SidebarInset>
        <main>
            @Body
        </main>
    </SidebarInset>
</SidebarProvider>
```

**Related Enums:**

- [`SidebarMenuButtonElement`](#sidebarmenubuttonelement-enum)
- [`SidebarMenuButtonVariant`](#sidebarmenubuttonvariant-enum)
- [`SidebarMenuSubButtonSize`](#sidebarmenusubbuttonsize-enum)
- [`SidebarMenuButtonSize`](#sidebarmenubuttonsize-enum)
- [`SidebarGroupLabelElement`](#sidebargrouplabelelement-enum)
- [`SidebarVariant`](#sidebarvariant-enum)
- [`SidebarSide`](#sidebarside-enum)

---

### Skeleton

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Skeleton`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Skeleton
```

**Components & Parameters:**

#### `Skeleton`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Shape` | `SkeletonShape` | `SkeletonShape.Rectangular` |  | Gets or sets the shape variant of the skeleton. <value> A <see cref="SkeletonShape"/> value. Default is <see cref="SkeletonShape.Rectangular"/>. </value> <remarks> <list type="bullet"> <item><see c... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the skeleton element. <value> A string containing one or more CSS class names, or <c>null</c>. </value> <remarks> Use this parameter to customize the... |

**Basic Usage:**
```razor
<Skeleton Class="h-12 w-12 rounded-full" />
<Skeleton Class="h-4 w-[250px]" />
<Skeleton Class="h-4 w-[200px]" />
```

**Related Enums:**

- [`SkeletonShape`](#skeletonshape-enum)

---

### Slider

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Slider`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Slider
```

**Components & Parameters:**

#### `Slider`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `double` | `` |  | Gets or sets the current value of the slider. |
| `ValueChanged` | `EventCallback<double>` | `` |  | Gets or sets the callback that is invoked when the value changes. |
| `Min` | `double` | `0` |  | Gets or sets the minimum value. |
| `Max` | `double` | `100` |  | Gets or sets the maximum value. |
| `Step` | `double` | `1` |  | Gets or sets the step increment. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the slider is disabled. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the slider. |

---

### Spinner

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Spinner`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Spinner
```

**Components & Parameters:**

#### `Spinner`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Size` | `SpinnerSize` | `SpinnerSize.Medium` |  | Gets or sets the size of the spinner. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the spinner. |

**Related Enums:**

- [`SpinnerSize`](#spinnersize-enum)

---

### Switch

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Switch`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Switch
```

**Components & Parameters:**

#### `Switch`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Checked` | `bool` | `` |  | Gets or sets whether the switch is checked (on). <remarks> This property supports two-way binding using the @bind-Checked directive. Changes to this property trigger the CheckedChanged event callba... |
| `CheckedChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback invoked when the checked state changes. <remarks> This event callback enables two-way binding with @bind-Checked. It is invoked whenever the user toggles the switch state.... |
| `Disabled` | `bool` | `` |  | Gets or sets whether the switch is disabled. <remarks> When disabled: - Switch cannot be clicked or focused - Opacity is reduced - Pointer events are disabled - aria-disabled attribute is set to tr... |
| `Size` | `SwitchSize` | `SwitchSize.Medium` |  | Gets or sets the size variant of the switch. <remarks> Available sizes: - Small: Compact switch for dense layouts - Medium: Default size (recommended) - Large: Prominent switch for primary actions ... |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the switch. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the switch. <remarks> Provides accessible text for screen readers when the switch doesn't have associated label text. </remarks> |
| `Id` | `string?` | `` |  | Gets or sets the ID attribute for the switch element. <remarks> Used for associating the switch with label elements via htmlFor attribute. </remarks> |
| `Name` | `string?` | `` |  | Gets or sets the name of the switch for form submission. <remarks> This is critical for form submission. The name/value pair is submitted to the server. If not specified, falls back to the Id value... |
| `Required` | `bool` | `` |  | Gets or sets whether the switch is required. <remarks> When true, the switch must be checked for form submission. Works with form validation. </remarks> |
| `CheckedExpression` | `Expression<Func<bool>>?` | `` |  | Gets or sets an expression that identifies the bound value. <remarks> Used for form validation integration. When provided, the switch registers with the EditContext and participates in form validat... |

**Basic Usage:**
```razor
<Switch @bind-Checked="isEnabled" />

@code {
    private bool isEnabled = false;
}
```

---

### Tabs

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Tabs`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Tabs
```

**Basic Usage:**
```razor
<Tabs DefaultValue="tab1">
    <TabsList>
        <TabsTrigger Value="tab1">Tab 1</TabsTrigger>
        <TabsTrigger Value="tab2">Tab 2</TabsTrigger>
        <TabsTrigger Value="tab3">Tab 3</TabsTrigger>
    </TabsList>

    <TabsContent Value="tab1">
        <p>Content for Tab 1</p>
    </TabsContent>

    <TabsContent Value="tab2">
        <p>Content for Tab 2</p>
    </TabsContent>

    <TabsContent Value="tab3">
        <p>Content for Tab 3</p>
    </TabsContent>
</Tabs>
```

---

### Textarea

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Textarea`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Textarea
```

**Components & Parameters:**

#### `Textarea`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `string?` | `` |  | Gets or sets the current value of the textarea. <remarks> Supports two-way binding via @bind-Value syntax. </remarks> |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback invoked when the textarea value changes. <remarks> This event is fired on every keystroke (oninput event). Use with Value parameter for two-way binding. </remarks> |
| `Placeholder` | `string?` | `` |  | Gets or sets the placeholder text displayed when the textarea is empty. <remarks> Provides a hint to the user about what to enter. Should not be used as a replacement for a label. </remarks> |
| `Disabled` | `bool` | `` |  | Gets or sets whether the textarea is disabled. <remarks> When disabled: - Textarea cannot be focused or edited - Cursor is set to not-allowed - Opacity is reduced for visual feedback </remarks> |
| `Required` | `bool` | `` |  | Gets or sets whether the textarea is required. <remarks> When true, the HTML5 required attribute is set. Works with form validation and :invalid CSS pseudo-class. </remarks> |
| `MaxLength` | `int?` | `` |  | Gets or sets the maximum number of characters allowed in the textarea. <remarks> When set, the HTML5 maxlength attribute is applied. Browser will prevent users from entering more than this many cha... |
| `MinLength` | `int?` | `` |  | Gets or sets the minimum number of characters required. <remarks> Works with form validation. Browser validates that at least this many characters are present. </remarks> |
| `Name` | `string?` | `` |  | Gets or sets the name of the textarea for form submission. <remarks> This is critical for form submission. The name/value pair is submitted to the server. Should be unique within the form. </remarks> |
| `Autocomplete` | `string?` | `` |  | Gets or sets the autocomplete hint for the browser. <remarks> Examples: "on", "off", "name", "street-address". Helps browsers provide appropriate autofill suggestions. </remarks> |
| `Readonly` | `bool` | `` |  | Gets or sets whether the textarea is read-only. <remarks> When true, the user cannot modify the value, but it's still focusable and submitted with forms. Different from Disabled - readonly textarea... |
| `Rows` | `int?` | `` |  | Gets or sets the visible number of text rows. <remarks> Specifies the height of the textarea in rows of text. If not specified, the component uses field-sizing-content for automatic sizing. </remarks> |
| `Cols` | `int?` | `` |  | Gets or sets the visible width in characters. <remarks> Specifies the width of the textarea in average character widths. Usually controlled by CSS width instead. </remarks> |
| `Wrap` | `string?` | `` |  | Gets or sets how text wraps when submitted in a form. <remarks> Values: "soft" (default - newlines not submitted), "hard" (newlines submitted), "off" (no wrapping). When "hard", the cols attribute ... |
| `InputMode` | `string?` | `` |  | Gets or sets the input mode hint for mobile keyboards. <remarks> Examples: "none", "text", "decimal", "numeric", "tel", "search", "email", "url". Helps mobile devices show the appropriate keyboard.... |
| `Autofocus` | `bool` | `` |  | Gets or sets whether the textarea should be auto-focused when the page loads. <remarks> Only one element per page should have autofocus. Improves accessibility when used appropriately. </remarks> |
| `Spellcheck` | `bool?` | `` |  | Gets or sets whether spell checking is enabled. <remarks> Can be true, false, or null (browser default). Useful for controlling spell checking on technical content, code, etc. </remarks> |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the textarea. <remarks> Custom classes are appended after the component's base classes, allowing for style overrides and extensions. </remarks> |
| `Id` | `string?` | `` |  | Gets or sets the HTML id attribute for the textarea element. <remarks> Used to associate the textarea with a label element via the label's 'for' attribute. This is essential for accessibility and a... |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the textarea. <remarks> Provides an accessible name for screen readers. Use when there is no visible label element. </remarks> |
| `AriaDescribedBy` | `string?` | `` |  | Gets or sets the ID of the element that describes the textarea. <remarks> References the id of an element containing help text or error messages. Improves screen reader experience by associating de... |
| `AriaInvalid` | `bool?` | `` |  | Gets or sets whether the textarea value is invalid. <remarks> When true, aria-invalid="true" is set. Should be set based on validation state. Triggers destructive color styling for error states. </... |

**Basic Usage:**
```razor
<Textarea @bind-Value="description" Placeholder="Enter description..." />

@code {
    private string description = "";
}
```

---

### TimePicker

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.TimePicker`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.TimePicker
```

---

### Toast

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Toast`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Toast
```

**Related Enums:**

- [`ToastVariant`](#toastvariant-enum)
- [`ToastPosition`](#toastposition-enum)

---

### Toggle

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Toggle`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Toggle
```

**Components & Parameters:**

#### `Toggle`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Pressed` | `bool` | `` |  | Gets or sets whether the toggle is pressed. |
| `PressedChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback that is invoked when the pressed state changes. |
| `Variant` | `ToggleVariant` | `ToggleVariant.Default` |  | Gets or sets the visual variant of the toggle. |
| `Size` | `ToggleSize` | `ToggleSize.Default` |  | Gets or sets the size of the toggle. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the toggle is disabled. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the toggle. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the toggle. |

**Related Enums:**

- [`ToggleVariant`](#togglevariant-enum)
- [`ToggleSize`](#togglesize-enum)

---

### ToggleGroup

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.ToggleGroup`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.ToggleGroup
```

**Components & Parameters:**

#### `ToggleGroup`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Type` | `ToggleGroupType` | `ToggleGroupType.Single` |  | Gets or sets the type of toggle group (single or multiple selection). |
| `Value` | `string?` | `` |  | Gets or sets the currently selected value (for single selection mode). |
| `ValueChanged` | `EventCallback<string?>` | `` |  | Gets or sets the callback that is invoked when the value changes. |
| `Values` | `List<string>?` | `` |  | Gets or sets the currently selected values (for multiple selection mode). |
| `ValuesChanged` | `EventCallback<List<string>?>` | `` |  | Gets or sets the callback that is invoked when the values change. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the toggle group is disabled. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the toggle group. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the toggle group. |

#### `ToggleGroupItem`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `string` | `string.Empty` |  | Gets or sets the value of this toggle item. |
| `Disabled` | `bool` | `` |  | Gets or sets whether this item is disabled. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the toggle item. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the toggle item. |

**Related Enums:**

- [`ToggleGroupType`](#togglegrouptype-enum)

---

### Tooltip

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Tooltip`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Tooltip
```

**Components & Parameters:**

#### `TooltipProvider`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `DelayDuration` | `int` | `700` |  | Gets or sets the duration in milliseconds to wait before showing a tooltip. <value> The delay duration in milliseconds. Default is 700ms. </value> <remarks> <para> This delay applies to all child <... |
| `SkipDelayDuration` | `int` | `300` |  | Gets or sets the duration in milliseconds during which the delay is skipped for subsequent tooltips. <value> The skip delay duration in milliseconds. Default is 300ms. </value> <remarks> <para> Aft... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content to be rendered within the provider. <value> A <see cref="RenderFragment"/> containing child components, or <c>null</c>. </value> <remarks> All child <see cref="Toolti... |

**Basic Usage:**
```razor
<Tooltip>
    <TooltipTrigger AsChild>
        <Button Size="ButtonSize.Icon">
            <LucideIcon Name="help-circle" Size="16" />
        </Button>
    </TooltipTrigger>
    <TooltipContent>
        <p>Help information</p>
    </TooltipContent>
</Tooltip>
```

---

### Typography

**Package:** `NeoBlazorUI.Components`  
**Namespace:** `BlazorUI.Components.Typography`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Components
```

**Import:**
```razor
@using BlazorUI.Components.Typography
```

**Components & Parameters:**

#### `Typography`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Variant` | `TypographyVariant` | `TypographyVariant.P` |  | Gets or sets the typography variant. |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the typography element. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the typography element. |

**Related Enums:**

- [`TypographyVariant`](#typographyvariant-enum)

---


---

## 🔧 Primitives

Headless, unstyled components with complete accessibility implementation.

### Accordion (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Accordion`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Accordion
```

**Basic Usage:**
```razor
<Accordion>
    <AccordionItem Value="item-1">
        <AccordionTrigger>Is it accessible?</AccordionTrigger>
        <AccordionContent>
            Yes. It adheres to WCAG 2.1 AA standards.
        </AccordionContent>
    </AccordionItem>

    <AccordionItem Value="item-2">
        <AccordionTrigger>Is it styled?</AccordionTrigger>
        <AccordionContent>
            Yes. It comes with default shadcn/ui styles.
        </AccordionContent>
    </AccordionItem>
</Accordion>
```

**Related Enums:**

- [`AccordionType`](#accordiontype-enum)

---

### Checkbox (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Checkbox`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Checkbox
```

**Components & Parameters:**

#### `Checkbox`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Checked` | `bool` | `` |  | Gets or sets whether the checkbox is checked. <remarks> This property supports two-way binding using the @bind-Checked directive. Changes to this property trigger the CheckedChanged event callback.... |
| `CheckedChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback invoked when the checked state changes. <remarks> This event callback enables two-way binding with @bind-Checked. It is invoked whenever the user toggles the checkbox stat... |
| `Indeterminate` | `bool` | `` |  | Gets or sets whether the checkbox is in an indeterminate state. <remarks> The indeterminate state is typically used for "select all" checkboxes when only some child items are selected. When indeter... |
| `IndeterminateChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback invoked when the indeterminate state changes. |
| `Disabled` | `bool` | `` |  | Gets or sets whether the checkbox is disabled. <remarks> When disabled: - Checkbox cannot be clicked or focused - aria-disabled attribute is set to true - Keyboard events are ignored </remarks> |
| `Id` | `string?` | `` |  | Gets or sets the ID attribute for the checkbox element. <remarks> Used for associating the checkbox with label elements via htmlFor attribute. </remarks> |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the checkbox. <remarks> Provides accessible text for screen readers when the checkbox doesn't have associated label text. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the checkbox. <value> A <see cref="RenderFragment"/> containing the checkbox content (e.g., checkmark icon), or <c>null</c>. </value> |

**Basic Usage:**
```razor
<Checkbox @bind-Checked="isAccepted" />

@code {
    private bool isAccepted = false;
}
```

---

### Collapsible (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Collapsible`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Collapsible
```

**Components & Parameters:**

#### `Collapsible`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Open` | `bool` | `` |  | Gets or sets a value indicating whether the collapsible is currently expanded. <value> <c>true</c> if the collapsible is open (content visible); otherwise, <c>false</c>. Default is <c>false</c>. </... |
| `OpenChanged` | `EventCallback<bool>` | `` |  | Gets or sets the callback invoked when the open state changes. <value> An <see cref="EventCallback{Boolean}"/> that receives the new open state. </value> |
| `Disabled` | `bool` | `` |  | Gets or sets a value indicating whether the collapsible is disabled. <value> <c>true</c> if the collapsible is disabled and cannot be toggled; otherwise, <c>false</c>. Default is <c>false</c>. </va... |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content to be rendered inside the collapsible container. <value> A <see cref="RenderFragment"/> containing the child components. </value> |

#### `CollapsibleContent`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered when the collapsible is expanded. <value> A <see cref="RenderFragment"/> containing the collapsible content, or <c>null</c>. </value> |
| `ForceMount` | `bool` | `false` |  | Whether to force mount the content even when the collapsible is closed. When true, content remains mounted (useful for CSS animations when styled). When false (default), content is unmounted when c... |

#### `CollapsibleTrigger`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the trigger. <value> A <see cref="RenderFragment"/> containing the trigger content, or <c>null</c>. </value> |
| `AsChild` | `bool` | `false` |  | When true, the trigger does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. The child component must consume TriggerContext and apply ... |

**Basic Usage:**
```razor
<Collapsible>
    <CollapsibleTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">
            Toggle
        </Button>
    </CollapsibleTrigger>
    <CollapsibleContent>
        <div class="p-4">
            This content can be collapsed
        </div>
    </CollapsibleContent>
</Collapsible>
```

---

### Combobox (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Combobox`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Combobox
```

**Basic Usage:**
```razor
<Combobox @bind-Value="selectedValue" Items="@items">
    <ComboboxTrigger>
        <ComboboxValue Placeholder="Select item..." />
    </ComboboxTrigger>
    <ComboboxContent>
        <ComboboxInput Placeholder="Search..." />
        <ComboboxList>
            <ComboboxEmpty>No results found.</ComboboxEmpty>
            @foreach (var item in items)
            {
                <ComboboxItem Value="@item.Value">
                    @item.Label
                </ComboboxItem>
            }
        </ComboboxList>
    </ComboboxContent>
</Combobox>

@code {
    private string selectedValue = "";
    private List<SelectItem> items = new()
    {
        new("apple", "Apple"),
        new("banana", "Banana"),
        new("orange", "Orange")
    };

    record SelectItem(string Value, string Label);
}
```

---

### ContextMenu (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.ContextMenu`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.ContextMenu
```

---

### Dialog (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Dialog`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Dialog
```

**Basic Usage:**
```razor
<Dialog>
    <DialogTrigger>Open Dialog</DialogTrigger>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Dialog Title</DialogTitle>
            <DialogDescription>Dialog description</DialogDescription>
        </DialogHeader>
        <p>Dialog content goes here</p>
        <DialogFooter>
            <DialogClose>Cancel</DialogClose>
            <Button>Confirm</Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

---

### DropdownMenu (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.DropdownMenu`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.DropdownMenu
```

**Basic Usage:**
```razor
<DropdownMenu>
    <DropdownMenuTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">
            Open Menu
        </Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent>
        <DropdownMenuItem>
            <LucideIcon Name="edit" Size="16" />
            Edit
        </DropdownMenuItem>
        <DropdownMenuItem>
            <LucideIcon Name="copy" Size="16" />
            Duplicate
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem>
            <LucideIcon Name="trash" Size="16" />
            Delete
        </DropdownMenuItem>
    </DropdownMenuContent>
</DropdownMenu>
```

---

### HoverCard (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.HoverCard`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.HoverCard
```

**Basic Usage:**
```razor
<HoverCard>
    <HoverCardTrigger AsChild>
        <a href="#" class="underline">@@username</a>
    </HoverCardTrigger>
    <HoverCardContent>
        <div class="flex gap-4">
            <Avatar>
                <AvatarImage Src="/avatar.jpg" />
                <AvatarFallback>JD</AvatarFallback>
            </Avatar>
            <div>
                <h4 class="text-sm font-semibold">John Doe</h4>
                <p class="text-sm text-muted-foreground">@@johndoe</p>
                <p class="text-xs mt-2">Software developer and open source contributor</p>
            </div>
        </div>
    </HoverCardContent>
</HoverCard>
```

---

### InputOtp (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.InputOtp`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.InputOtp
```

---

### Label (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Label`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Label
```

**Basic Usage:**
```razor
<Label For="email">Email Address</Label>
<Input Id="email" Type="InputType.Email" />
```

---

### Menubar (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Menubar`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Menubar
```

---

### MultiSelect (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.MultiSelect`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.MultiSelect
```

**Related Enums:**

- [`SelectAllState`](#selectallstate-enum)

---

### NavigationMenu (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.NavigationMenu`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.NavigationMenu
```

**Related Enums:**

- [`NavigationMenuOrientation`](#navigationmenuorientation-enum)

---

### Popover (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Popover`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Popover
```

**Basic Usage:**
```razor
<Popover>
    <PopoverTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">Open Popover</Button>
    </PopoverTrigger>
    <PopoverContent>
        <p>Popover content goes here</p>
    </PopoverContent>
</Popover>
```

---

### RadioGroup (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.RadioGroup`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.RadioGroup
```

**Components & Parameters:**

#### `RadioGroup`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Value` | `TValue?` | `` |  | Gets or sets the currently selected value. <remarks> This property supports two-way binding using the @bind-Value directive. Changes to this property trigger the ValueChanged event callback. </rema... |
| `ValueChanged` | `EventCallback<TValue>` | `` |  | Gets or sets the callback invoked when the selected value changes. <remarks> This event callback enables two-way binding with @bind-Value. It is invoked whenever a radio button is selected. </remarks> |
| `Disabled` | `bool` | `` |  | Gets or sets whether the entire radio group is disabled. <remarks> When disabled, all radio items in the group cannot be selected. </remarks> |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the radio group. <remarks> Provides accessible text for screen readers to describe the purpose of the radio group. </remarks> |
| `Name` | `string?` | `` |  | Gets or sets the name for all radio items in this group. <remarks> This name is shared by all radio items in the group for form submission. </remarks> |
| `Required` | `bool` | `` |  | Gets or sets whether a selection is required in this radio group. <remarks> When true, one of the radio items must be selected for form submission. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the radio group. <remarks> Should contain RadioGroupItem components. </remarks> |

#### `RadioGroupItem`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `Disabled` | `bool` | `` |  | Gets or sets whether this individual radio item is disabled. <remarks> When disabled, the item cannot be selected and appears with reduced opacity. </remarks> |
| `Id` | `string?` | `` |  | Gets or sets the ID attribute for the radio item element. <remarks> Used for associating the radio item with label elements via htmlFor attribute. Auto-generated if not provided. </remarks> |
| `AriaLabel` | `string?` | `` |  | Gets or sets the ARIA label for the radio item. <remarks> Provides accessible text for screen readers when the radio item doesn't have an associated label element. </remarks> |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the content to be rendered inside the radio item. |

**Basic Usage:**
```razor
<RadioGroup @bind-Value="selectedOption">
    <RadioGroupItem Value="option1" Id="opt1" />
    <RadioGroupItem Value="option2" Id="opt2" />
    <RadioGroupItem Value="option3" Id="opt3" />
</RadioGroup>

@code {
    private string selectedOption = "option1";
}
```

---

### Select (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Select`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Select
```

**Basic Usage:**
```razor
<Select @bind-Value="selectedFruit">
    <SelectTrigger>
        <SelectValue Placeholder="Select a fruit..." />
    </SelectTrigger>
    <SelectContent>
        <SelectItem Value="apple">Apple</SelectItem>
        <SelectItem Value="banana">Banana</SelectItem>
        <SelectItem Value="orange">Orange</SelectItem>
    </SelectContent>
</Select>

@code {
    private string selectedFruit = "";
}
```

---

### Sheet (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Sheet`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Sheet
```

**Basic Usage:**
```razor
<Sheet>
    <SheetTrigger AsChild>
        <Button>Open Sheet</Button>
    </SheetTrigger>
    <SheetContent>
        <SheetHeader>
            <SheetTitle>Sheet Title</SheetTitle>
            <SheetDescription>Sheet description</SheetDescription>
        </SheetHeader>
        <p>Sheet content</p>
        <SheetFooter>
            <SheetClose AsChild>
                <Button>Close</Button>
            </SheetClose>
        </SheetFooter>
    </SheetContent>
</Sheet>
```

**Related Enums:**

- [`SheetSide`](#sheetside-enum)

---

### Switch (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Switch`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Switch
```

**Basic Usage:**
```razor
<Switch @bind-Checked="isEnabled" />

@code {
    private bool isEnabled = false;
}
```

---

### Table (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Table`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Table
```

**Components & Parameters:**

#### `Table`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `State` | `TableState<TData>?` | `` |  | The table state (controlled mode). When provided, the table uses external state management. |
| `StateChanged` | `EventCallback<TableState<TData>>` | `` |  | Event callback invoked when the table state changes (for controlled mode). Use with @bind-State for two-way binding. |
| `SelectionMode` | `SelectionMode` | `SelectionMode.None` |  | The selection mode for the table. |
| `OnSortChange` | `EventCallback<(string ColumnId, SortDirection Direction)>` | `` |  | Event callback invoked when sorting changes. |
| `OnRowSelect` | `EventCallback<TData>` | `` |  | Event callback invoked when a row is selected. |
| `OnPageChange` | `EventCallback<int>` | `` |  | Event callback invoked when the current page changes. |
| `OnPageSizeChange` | `EventCallback<int>` | `` |  | Event callback invoked when the page size changes. |
| `OnSelectionChange` | `EventCallback<IReadOnlyCollection<TData>>` | `` |  | Event callback invoked when the selection changes. |
| `ChildContent` | `RenderFragment?` | `` |  | Child content for the table (TableHeader, TableBody, etc.). |
| `AriaLabel` | `string?` | `` |  | ARIA label for the table. |
| `Class` | `string?` | `` |  | Additional CSS classes to apply to the table element. |
| `ManualPagination` | `bool` | `false` |  | When true, the table will not automatically set TotalItems based on data count. Use this when the parent component handles pagination and passes pre-paginated data. |

#### `TablePagination`

| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| `State` | `PaginationState` | `default!` |  | Gets or sets the pagination state to control and display. |
| `OnPageChange` | `EventCallback<int>` | `` |  | Gets or sets the callback invoked when the page changes. |
| `OnPageSizeChange` | `EventCallback<int>` | `` |  | Gets or sets the callback invoked when the page size changes. |
| `PageSizeOptions` | `int[]` | `[10, 25, 50, 100]` |  | Gets or sets the available page sizes for the selector. Default: [10, 25, 50, 100] |
| `ShowPageSizeSelector` | `bool` | `true` |  | Gets or sets whether to show the page size selector. Default: true |
| `ShowPageInfo` | `bool` | `true` |  | Gets or sets whether to show page info (e.g., "1-10 of 100 items"). Default: true |
| `PaginationTemplate` | `RenderFragment<PaginationContext>?` | `` |  | Gets or sets the custom pagination template. Provides full control over pagination markup. |
| `ChildContent` | `RenderFragment?` | `` |  | Gets or sets the child content (used when PaginationTemplate is not provided). |
| `Class` | `string?` | `` |  | Gets or sets additional CSS classes to apply to the container. |

**Related Enums:**

- [`SelectionMode`](#selectionmode-enum)
- [`SortDirection`](#sortdirection-enum)

---

### Tabs (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Tabs`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Tabs
```

**Basic Usage:**
```razor
<Tabs DefaultValue="tab1">
    <TabsList>
        <TabsTrigger Value="tab1">Tab 1</TabsTrigger>
        <TabsTrigger Value="tab2">Tab 2</TabsTrigger>
        <TabsTrigger Value="tab3">Tab 3</TabsTrigger>
    </TabsList>

    <TabsContent Value="tab1">
        <p>Content for Tab 1</p>
    </TabsContent>

    <TabsContent Value="tab2">
        <p>Content for Tab 2</p>
    </TabsContent>

    <TabsContent Value="tab3">
        <p>Content for Tab 3</p>
    </TabsContent>
</Tabs>
```

**Related Enums:**

- [`TabsOrientation`](#tabsorientation-enum)
- [`TabsActivationMode`](#tabsactivationmode-enum)

---

### Tooltip (Primitive)

**Package:** `NeoBlazorUI.Primitives`  
**Namespace:** `BlazorUI.Primitives.Tooltip`

**Installation:**
```bash
dotnet add package NeoBlazorUI.Primitives
```

**Import:**
```razor
@using BlazorUI.Primitives.Tooltip
```

**Basic Usage:**
```razor
<Tooltip>
    <TooltipTrigger AsChild>
        <Button Size="ButtonSize.Icon">
            <LucideIcon Name="help-circle" Size="16" />
        </Button>
    </TooltipTrigger>
    <TooltipContent>
        <p>Help information</p>
    </TooltipContent>
</Tooltip>
```

---


---

## 📚 Enums Reference

### AccordionType Enum

**Used by:** Accordion

| Value | Description |
|-------|-------------|
| `Single` | Only one item can be open at a time. |
| `Multiple` | Multiple items can be open simultaneously. |

### AlertVariant Enum

**Used by:** Alert

| Value | Description |
|-------|-------------|
| `Default` | Default informational alert style. Uses standard --foreground and --background CSS variables. Suitable for general notifications and informational messages. |
| `Destructive` | Destructive alert style for errors and critical warnings. Uses --destructive and --destructive-foreground CSS variables. Indicates errors, failures, or requires immediate user attention. |

### AnimationEasing Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Linear` | Linear animation with constant speed |
| `QuadIn` | Ease in using quadratic function |
| `QuadOut` | Ease out using quadratic function |
| `QuadInOut` | Ease in and out using quadratic function |
| `CubicIn` | Ease in using cubic function |
| `CubicOut` | Ease out using cubic function (default) |
| `CubicInOut` | Ease in and out using cubic function |
| `QuartIn` | Ease in using quartic function |
| `QuartOut` | Ease out using quartic function |
| `QuartInOut` | Ease in and out using quartic function |
| `QuintIn` | Ease in using quintic function |
| `QuintOut` | Ease out using quintic function |
| `QuintInOut` | Ease in and out using quintic function |
| `SineIn` | Ease in using sinusoidal function |
| `SineOut` | Ease out using sinusoidal function |
| `SineInOut` | Ease in and out using sinusoidal function |
| `ExpoIn` | Ease in using exponential function |
| `ExpoOut` | Ease out using exponential function |
| `ExpoInOut` | Ease in and out using exponential function |
| `CircIn` | Ease in using circular function |
| `CircOut` | Ease out using circular function |
| `CircInOut` | Ease in and out using circular function |
| `ElasticIn` | Elastic ease in (spring effect) |
| `ElasticOut` | Elastic ease out (spring effect) |
| `ElasticInOut` | Elastic ease in and out (spring effect) |
| `BackIn` | Ease in using back function (overshoots) |
| `BackOut` | Ease out using back function (overshoots) |
| `BackInOut` | Ease in and out using back function (overshoots) |
| `BounceIn` | Bounce ease in |
| `BounceOut` | Bounce ease out |
| `BounceInOut` | Bounce ease in and out |

### AnimationType Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Default` | Default animation for the chart type |
| `FadeIn` | Fade in animation |
| `ScaleIn` | Scale in animation (grow from center) |
| `SlideInLeft` | Slide in from left animation |
| `SlideInRight` | Slide in from right animation |
| `SlideInTop` | Slide in from top animation |
| `SlideInBottom` | Slide in from bottom animation |
| `Wave` | Wave animation (sequential reveal) |
| `Expand` | Expand animation (bars grow from baseline) |
| `Draw` | Draw animation (lines/paths draw progressively) |

### AvatarSize Enum

**Used by:** Avatar

| Value | Description |
|-------|-------------|
| `Small` | Small avatar (32px). <remarks> Suitable for compact layouts, user lists, or inline mentions. </remarks> |
| `Default` | Default avatar size (40px). <remarks> Standard size for most use cases including navigation bars and comment sections. </remarks> |
| `Large` | Large avatar (48px). <remarks> Used for prominent user displays or profile headers. </remarks> |
| `ExtraLarge` | Extra large avatar (64px). <remarks> Reserved for profile pages or settings where user identity is focal. </remarks> |

### AxisScale Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Auto` | Automatic scale detection. |
| `Category` | Category scale for discrete values. |
| `Value` | Value scale for numeric data. |
| `Time` | Time scale for temporal data. |
| `Log` | Logarithmic scale. |

### BadgeVariant Enum

**Used by:** Badge

| Value | Description |
|-------|-------------|
| `Default` | Default primary badge style with solid background. Uses --primary and --primary-foreground CSS variables. Suitable for highlighting important items or new content. |
| `Secondary` | Secondary badge style with muted background. Uses --secondary and --secondary-foreground CSS variables. For alternative or less prominent labels. |
| `Destructive` | Destructive badge style for warnings or errors. Uses --destructive and --destructive-foreground CSS variables. Indicates critical status or requires user attention. |
| `Outline` | Outlined badge style with transparent background and border. Uses --foreground CSS variable for text. Minimal style for subtle categorization or tags. |

### BarLayout Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Vertical` | Vertical bars (default). |
| `Horizontal` | Horizontal bars. |

### ButtonSize Enum

**Used by:** Button

| Value | Description |
|-------|-------------|
| `Small` | Small button size (sm). Height: ~36px, Padding: 0.5rem horizontal. Suitable for compact UIs, toolbars, or inline actions. |
| `Default` | Default/medium button size (md). Height: ~40px, Padding: 1rem horizontal. Recommended default size for most use cases. |
| `Large` | Large button size (lg). Height: ~44px, Padding: 1.5rem horizontal. Best for primary CTAs or prominent actions. |
| `Icon` | Icon-only button size (icon). Square dimensions: 40x40px (h-10 w-10). Designed for buttons containing only an icon without text. Maintains accessibility with proper aria-label required. |
| `IconSmall` | Small icon-only button size (icon-sm). Square dimensions: 36x36px (h-9 w-9). Compact icon button for toolbars or tight layouts. Requires aria-label for accessibility. |
| `IconLarge` | Large icon-only button size (icon-lg). Square dimensions: 44x44px (h-11 w-11). Prominent icon button for primary icon actions. Requires aria-label for accessibility. |

### ButtonType Enum

**Used by:** Button

| Value | Description |
|-------|-------------|
| `Submit` | Submit button (type="submit"). Submits the form data to the server when clicked. This is the default behavior for buttons inside forms. |
| `Reset` | Reset button (type="reset"). Resets all form controls to their initial values when clicked. Use sparingly as this can be confusing to users. |
| `Button` | Regular button (type="button"). Does not submit or reset the form. Default for buttons outside forms or with custom click handlers. Prevents accidental form submission. |

### ButtonVariant Enum

**Used by:** Button

| Value | Description |
|-------|-------------|
| `Default` | Default primary button style with solid background. Uses --primary and --primary-foreground CSS variables. Recommended for primary actions (submit, save, confirm). |
| `Destructive` | Destructive action button style (delete, remove, cancel). Uses --destructive and --destructive-foreground CSS variables. Indicates potentially dangerous or irreversible actions. |
| `Outline` | Outlined button style with transparent background and border. Uses --border and --input CSS variables. Suitable for secondary actions or form controls. |
| `Secondary` | Secondary button style with muted background. Uses --secondary and --secondary-foreground CSS variables. For alternative actions or less prominent CTAs. |
| `Ghost` | Ghost button style with no background or border (minimal). Only shows background on hover using --accent CSS variable. Ideal for tertiary actions, toolbars, or navigation. |
| `Link` | Link-styled button that appears as underlined text. Uses --primary CSS variable for text color. Suitable for inline actions or navigation that should look like links. |

### CalendarCaptionLayout Enum

**Used by:** Calendar

| Value | Description |
|-------|-------------|
| `Label` | Displays the month and year as a static label. |
| `Dropdown` | Displays dropdown selects for quick month and year selection. |

### CarouselOrientation Enum

**Used by:** Carousel

| Value | Description |
|-------|-------------|
| `Horizontal` | Horizontal carousel (left to right). |
| `Vertical` | Vertical carousel (top to bottom). |

### ChartEngine Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `ChartJs` | Chart.js engine - Canvas-based rendering with excellent performance. Best for interactive dashboards and large datasets. |
| `ECharts` | ECharts engine - SVG-based rendering with rich features. Best for print-quality charts and complex visualizations. |

### ChartType Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Line` | Line chart for showing trends over time or continuous data. |
| `Bar` | Bar chart for comparing discrete categories. |
| `Pie` | Pie chart for showing composition of a whole. |
| `Donut` | Donut chart, a variant of pie chart with a hole in the center. |
| `Radar` | Radar chart for multivariate data. |
| `Scatter` | Scatter chart for showing correlation between two variables. |
| `Bubble` | Bubble chart for showing three dimensions of data. |
| `Area` | Area chart, a filled line chart. |

### DataTableSelectionMode Enum

**Used by:** DataTable

| Value | Description |
|-------|-------------|
| `None` | No row selection is allowed. |
| `Single` | Only a single row can be selected at a time. Selecting a new row deselects the previously selected row. |
| `Multiple` | Multiple rows can be selected simultaneously using checkboxes. Includes a select-all checkbox in the header. |

### Focus Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `None` | No focus effect (emphasis disabled). |
| `Self` | Focus on self only. |

### GradientDirection Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Vertical` | Top to bottom gradient. |
| `Horizontal` | Left to right gradient. |

### GridColumnPinPosition Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `None` | Not pinned. |
| `Left` | Pinned to left side. |
| `Right` | Pinned to right side. |

### GridDensity Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `Comfortable` | Standard padding (default). |
| `Compact` | Reduced padding for more rows. |
| `Spacious` | Increased padding for readability. |

### GridFilterOperator Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `Equals` | Equals comparison. |
| `NotEquals` | Not equals comparison. |
| `Contains` | Contains substring. |
| `NotContains` | Does not contain substring. |
| `StartsWith` | Starts with substring. |
| `EndsWith` | Ends with substring. |
| `LessThan` | Less than comparison. |
| `LessThanOrEqual` | Less than or equal comparison. |
| `GreaterThan` | Greater than comparison. |
| `GreaterThanOrEqual` | Greater than or equal comparison. |
| `IsEmpty` | Value is empty or null. |
| `IsNotEmpty` | Value is not empty or null. |

### GridPagingMode Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `None` | All data displayed, no pagination. |
| `Client` | All data loaded, paginated client-side. |
| `Server` | Data fetched page-by-page from server. |
| `InfiniteScroll` | Incremental loading as user scrolls. |

### GridRowModelType Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `ClientSide` | Client-side row model. All data is loaded into the grid at once. Best for datasets under 10,000 rows. |
| `ServerSide` | Server-side row model. Data is fetched on-demand from the server. Supports server-side sorting, filtering, and pagination. Best for large datasets (100,000+ rows) or when data must remain on the server. |
| `Infinite` | Infinite scroll row model. Data is fetched in blocks as the user scrolls. Best for medium-sized datasets (10,000-100,000 rows) with simple requirements. |

### GridSelectionMode Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `None` | No row selection allowed. |
| `Single` | Only one row can be selected at a time. |
| `Multiple` | Multiple rows can be selected simultaneously. |

### GridSortDirection Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `None` | No sorting applied. |
| `Ascending` | Sort in ascending order. |
| `Descending` | Sort in descending order. |

### GridStyle Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `Default` | Standard appearance with default borders and hover states. |
| `Striped` | Alternating row background colors for easier readability. |
| `Bordered` | Bordered cells with visible vertical dividers between columns. |
| `Minimal` | Minimal borders with subtle, clean styling. |

### GridTheme Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `Shadcn` | Shadcn theme integrated with shadcn/ui design tokens (default). Automatically adapts to your app's color scheme and supports dark mode. |
| `Alpine` | AG Grid's Alpine theme (clean, modern look). |
| `Balham` | AG Grid's Balham theme (professional business theme). |
| `Material` | AG Grid's Material theme (Google Material Design styled). |
| `Quartz` | AG Grid's Quartz theme (modern, polished appearance). |

### GridVirtualizationMode Enum

**Used by:** Grid

| Value | Description |
|-------|-------------|
| `Auto` | Renderer decides based on data size. |
| `None` | No virtualization. |
| `RowOnly` | Row virtualization only. |
| `RowAndColumn` | Both row and column virtualization. |

### IconPosition Enum

**Used by:** Button

| Value | Description |
|-------|-------------|
| `Start` | Icon appears before the text (left in LTR, right in RTL). Uses margin-end for RTL-aware spacing. |
| `End` | Icon appears after the text (right in LTR, left in RTL). Uses margin-start for RTL-aware spacing. |

### ImageFormat Enum

**Used by:** Chart

**Values:**
- `Png`
- `Svg`

### InputGroupAlign Enum

**Used by:** InputGroup

| Value | Description |
|-------|-------------|
| `InlineStart` | Position addon at the inline start (left in LTR, right in RTL). <remarks> Common use cases: search icons, currency symbols, URL protocols. </remarks> |
| `InlineEnd` | Position addon at the inline end (right in LTR, left in RTL). <remarks> Common use cases: action buttons, validation icons, clear buttons. </remarks> |
| `BlockStart` | Position addon above the input (block start). <remarks> Common use cases: labels, descriptions, field titles. </remarks> |
| `BlockEnd` | Position addon below the input (block end). <remarks> Common use cases: character counters, help text, validation messages. </remarks> |

### InputType Enum

**Used by:** Input

| Value | Description |
|-------|-------------|
| `Text` | Single-line text input (default). Accepts any text characters. |
| `Email` | Email address input. Provides email validation and @ key on mobile keyboards. |
| `Password` | Password input with obscured characters. Text is hidden for security (displayed as dots or asterisks). |
| `Number` | Numeric input. Shows numeric keyboard on mobile and allows spinner controls. |
| `Tel` | Telephone number input. Optimized for phone number entry with tel: keyboard layout. |
| `Url` | URL input. Provides URL validation and .com key on mobile keyboards. |
| `Search` | Search query input. Displays search icon and may show recent searches. |
| `Date` | Date picker input. Shows native date picker on supported browsers. |
| `Time` | Time picker input. Shows native time picker on supported browsers. |
| `File` | File upload input. Allows users to select files from their device. |

### InterpolationType Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Natural` | Natural/smooth curve interpolation (default). |
| `Linear` | Linear interpolation (straight lines). |
| `Step` | Step interpolation (middle). |
| `StepBefore` | Step interpolation before the point. |
| `StepAfter` | Step interpolation after the point. |
| `Monotone` | Monotone curve interpolation. |

### ItemMediaVariant Enum

**Used by:** Item

| Value | Description |
|-------|-------------|
| `Default` | Default variant with no specific styling. |
| `Icon` | Icon variant with square border container (8px). |
| `Image` | Image variant with rounded container (10px). |

### ItemSize Enum

**Used by:** Item

| Value | Description |
|-------|-------------|
| `Default` | Default size with standard padding. |
| `Sm` | Small size with reduced padding. |

### ItemVariant Enum

**Used by:** Item

| Value | Description |
|-------|-------------|
| `Default` | Default variant with no border or background. |
| `Outline` | Outlined variant with border. |
| `Muted` | Muted variant with subtle background color. |

### LabelPosition Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Top` | Position label at top. |
| `Bottom` | Position label at bottom. |
| `Left` | Position label at left. |
| `Right` | Position label at right. |
| `Inside` | Position label inside the data element. |
| `InsideTop` | Position label inside at top. |
| `InsideBottom` | Position label inside at bottom. |
| `InsideLeft` | Position label inside at left. |
| `InsideRight` | Position label inside at right. |
| `Center` | Position label at center. |
| `Outside` | Position label outside the data element. |

### LegendAlign Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Left` | Align to the left. |
| `Center` | Align to the center. |
| `Right` | Align to the right. |

### LegendIcon Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Default` |  |
| `Circle` | Circle icon. |
| `Rect` | Rectangle icon. |
| `RoundRect` | Rounded rectangle icon. |
| `Triangle` | Triangle icon. |
| `Diamond` | Diamond icon. |
| `Pin` | Pin icon (droplet shape). |
| `Arrow` | Arrow icon. |
| `None` | No icon (empty space). |

### LegendLayout Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Horizontal` | Horizontal layout. |
| `Vertical` | Vertical layout. |

### LegendVerticalAlign Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Top` | Align to the top. |
| `Middle` | Align to the middle. |
| `Bottom` | Align to the bottom. |

### LineStyleType Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Solid` | Solid line (default). |
| `Dashed` | Dashed line. |
| `Dotted` | Dotted line. |

### NavigationMenuOrientation Enum

**Used by:** NavigationMenu

| Value | Description |
|-------|-------------|
| `Horizontal` | Horizontal navigation menu. |
| `Vertical` | Vertical navigation menu. |

### Orientation Enum

**Used by:** ScrollArea

| Value | Description |
|-------|-------------|
| `Vertical` | Vertical scrollbar. |
| `Horizontal` | Horizontal scrollbar. |

### PolarGridType Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Circle` | Circular grid lines. |
| `Polygon` | Polygon grid lines. |

### RadarShape Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Polygon` | Polygon shape (angular). |
| `Circle` | Circle shape (smooth). |

### ResizableDirection Enum

**Used by:** Resizable

| Value | Description |
|-------|-------------|
| `Horizontal` | Panels are arranged horizontally (side by side). |
| `Vertical` | Panels are arranged vertically (stacked). |

### ScrollAreaType Enum

**Used by:** ScrollArea

| Value | Description |
|-------|-------------|
| `Auto` | Scrollbars are visible when content overflows. |
| `Always` | Scrollbars are always visible. |
| `Scroll` | Scrollbars appear only when scrolling. |
| `Hover` | Scrollbars appear only on hover. |

### SelectAllState Enum

**Used by:** MultiSelect

| Value | Description |
|-------|-------------|
| `None` | No items are selected. |
| `Indeterminate` | Some items are selected (indeterminate state). |
| `All` | All items are selected. |

### SelectionMode Enum

**Used by:** Table

| Value | Description |
|-------|-------------|
| `None` | No row selection allowed. |
| `Single` | Only one row can be selected at a time. |
| `Multiple` | Multiple rows can be selected simultaneously. |

### SeparatorOrientation Enum

**Used by:** Separator

| Value | Description |
|-------|-------------|
| `Horizontal` | Horizontal separator that spans the full width. Creates a horizontal line divider between vertically stacked content. CSS: h-[1px] w-full |
| `Vertical` | Vertical separator that spans the full height. Creates a vertical line divider between horizontally arranged content. CSS: h-full w-[1px] |

### SheetSide Enum

**Used by:** Sheet

| Value | Description |
|-------|-------------|
| `Top` | Sheet slides in from the top edge. |
| `Right` | Sheet slides in from the right edge (default). |
| `Bottom` | Sheet slides in from the bottom edge. |
| `Left` | Sheet slides in from the left edge. |

### SidebarGroupLabelElement Enum

**Used by:** Sidebar

| Value | Description |
|-------|-------------|
| `Div` | Render as a div element. |
| `Button` | Render as a button element (for collapsible groups). |

### SidebarMenuButtonElement Enum

**Used by:** Sidebar

| Value | Description |
|-------|-------------|
| `Button` | Render as a button element for actions and toggles. |
| `Anchor` | Render as an anchor element for navigation links. |

### SidebarMenuButtonSize Enum

**Used by:** Sidebar

| Value | Description |
|-------|-------------|
| `Small` | Small size with reduced padding and text (text-xs). |
| `Default` | Default size with standard padding and text. |
| `Large` | Large size with increased padding and text (text-base). |

### SidebarMenuButtonVariant Enum

**Used by:** Sidebar

| Value | Description |
|-------|-------------|
| `Default` | Default style with no outline or border. |
| `Outline` | Outlined style with a visible border. |

### SidebarMenuSubButtonSize Enum

**Used by:** Sidebar

| Value | Description |
|-------|-------------|
| `Small` | Small size variant. |
| `Medium` | Medium/default size variant. |

### SidebarSide Enum

**Used by:** Sidebar

| Value | Description |
|-------|-------------|
| `Left` | Sidebar appears on the left side. |
| `Right` | Sidebar appears on the right side. |

### SidebarVariant Enum

**Used by:** Sidebar

| Value | Description |
|-------|-------------|
| `Sidebar` | Default sidebar that pushes content. |
| `Floating` | Floating sidebar that overlays content. |
| `Inset` | Inset sidebar with padding. |

### SkeletonShape Enum

**Used by:** Skeleton

| Value | Description |
|-------|-------------|
| `Rectangular` | Rectangular skeleton with rounded corners (rounded-md). <remarks> Default shape suitable for most loading placeholders including: <list type="bullet"> <item>Text lines and paragraphs</item> <item>Images and thumbnails</item> <item>Cards and panels</item> <item>Buttons and form fields</item> </list> Uses Tailwind's <c>rounded-md</c> class for subtle rounded corners. </remarks> |
| `Circular` | Circular skeleton with fully rounded borders (rounded-full). <remarks> Ideal for avatar and icon placeholders including: <list type="bullet"> <item>User profile pictures</item> <item>Circular badges or indicators</item> <item>Icon placeholders</item> </list> Uses Tailwind's <c>rounded-full</c> class for perfectly circular shape. Ensure equal width and height (e.g., <c>h-12 w-12</c>) for proper circles. </remarks> |

### SortDirection Enum

**Used by:** Table

| Value | Description |
|-------|-------------|
| `None` | No sorting applied. |
| `Ascending` | Sort in ascending order (A to Z, 0 to 9, oldest to newest). |
| `Descending` | Sort in descending order (Z to A, 9 to 0, newest to oldest). |

### SpinnerSize Enum

**Used by:** Spinner

| Value | Description |
|-------|-------------|
| `Small` | Small spinner (16x16 pixels). |
| `Medium` | Medium spinner (32x32 pixels). Default size. |
| `Large` | Large spinner (48x48 pixels). |

### StackOffset Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `None` | No normalization (default stacking). |
| `Expand` | Expand/normalize to 100% per X bucket. |

### SymbolShape Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `EmptyCircle` | Circle marker (default). |
| `Circle` | Circle marker. |
| `Rect` | Rectangle/square marker. |
| `RoundRect` | Rounded rectangle marker. |
| `Triangle` | Triangle marker. |
| `Diamond` | Diamond marker. |
| `Pin` | Pin/droplet marker. |
| `Arrow` | Arrow marker. |
| `None` | No marker/symbol. |

### TabsActivationMode Enum

**Used by:** Tabs

| Value | Description |
|-------|-------------|
| `Automatic` | Tabs activate automatically when focused with arrow keys. |
| `Manual` | Tabs must be clicked or Enter/Space pressed to activate. |

### TabsOrientation Enum

**Used by:** Tabs

| Value | Description |
|-------|-------------|
| `Horizontal` | Tabs are arranged horizontally. |
| `Vertical` | Tabs are arranged vertically. |

### ToastPosition Enum

**Used by:** Toast

| Value | Description |
|-------|-------------|
| `TopLeft` | Top left corner. |
| `TopCenter` | Top center. |
| `TopRight` | Top right corner. |
| `BottomLeft` | Bottom left corner. |
| `BottomCenter` | Bottom center. |
| `BottomRight` | Bottom right corner. |

### ToastVariant Enum

**Used by:** Toast

| Value | Description |
|-------|-------------|
| `Default` | Default/neutral toast. |
| `Success` | Success toast (green). |
| `Warning` | Warning toast (yellow/orange). |
| `Destructive` | Error/destructive toast (red). |
| `Info` | Info toast (blue). |

### ToggleGroupType Enum

**Used by:** ToggleGroup

| Value | Description |
|-------|-------------|
| `Single` | Single selection mode (radio behavior). Only one item can be selected at a time. |
| `Multiple` | Multiple selection mode (checkbox behavior). Multiple items can be selected simultaneously. |

### ToggleSize Enum

**Used by:** Toggle

| Value | Description |
|-------|-------------|
| `Default` | Default size toggle. |
| `Small` | Small toggle. |
| `Large` | Large toggle. |

### ToggleVariant Enum

**Used by:** Toggle

| Value | Description |
|-------|-------------|
| `Default` | Default toggle style with transparent background. |
| `Outline` | Outlined toggle style with border. |

### TooltipCursor Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `None` | No cursor/pointer. |
| `Line` | Single line cursor. |
| `Cross` | Cross cursor (both horizontal and vertical lines). |
| `Shadow` | Shadow/highlight region. |

### TooltipMode Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Axis` | Tooltip triggered by axis (shows all series at that position). |
| `Item` | Tooltip triggered by individual data points. |

### TypographyVariant Enum

**Used by:** Typography

| Value | Description |
|-------|-------------|
| `H1` | Heading level 1 - largest heading. |
| `H2` | Heading level 2. |
| `H3` | Heading level 3. |
| `H4` | Heading level 4. |
| `P` | Paragraph text - default body text. |
| `Blockquote` | Blockquote - quoted text. |
| `InlineCode` | Inline code text. |
| `Lead` | Lead paragraph - larger emphasized text. |
| `Large` | Large text. |
| `Small` | Small text. |
| `Muted` | Muted text - de-emphasized. |

### YAxisPosition Enum

**Used by:** Chart

| Value | Description |
|-------|-------------|
| `Left` | Position axis on the left side (default). |
| `Right` | Position axis on the right side. |


---

## 🏗️ Architecture Overview

### Two-Tier System

**Primitives (NeoBlazorUI.Primitives):**
- Headless, unstyled components
- Complete accessibility implementation (ARIA attributes, keyboard navigation)
- Full control over styling
- Foundation for custom designs

**Components (NeoBlazorUI.Components):**
- Pre-styled with shadcn/ui design
- Built on top of primitives
- Ready to use out of the box
- Customizable through CSS variables

---

## 🎨 Theming

All components use CSS variables for theming, making them fully customizable.

**Core Variables:**
```css
--background: Page background
--foreground: Text color
--card: Card background
--card-foreground: Card text
--popover: Popover background
--popover-foreground: Popover text
--primary: Primary brand color
--primary-foreground: Text on primary
--secondary: Secondary brand color
--secondary-foreground: Text on secondary
--muted: Muted background
--muted-foreground: Muted text
--accent: Accent background
--accent-foreground: Accent text
--destructive: Destructive action color
--destructive-foreground: Text on destructive
--border: Border color
--input: Input border color
--ring: Focus ring color
--radius: Border radius
```

**Using Themes:**

NeoBlazorUI is compatible with shadcn/ui themes from:
- https://ui.shadcn.com/themes
- https://tweakcn.com

Simply copy the CSS variables and paste them into your `theme.css` file.

---

## 📦 Icon Libraries

### Lucide Icons
- **Package:** `NeoBlazorUI.Icons.Lucide`
- **Icon Count:** 1,640+
- **Style:** Stroke-based, 24x24 viewBox
- **Usage:** `<LucideIcon Name="LucideIconName.Home" />`

### Heroicons
- **Package:** `NeoBlazorUI.Icons.Heroicons`
- **Icon Count:** 1,288+
- **Variants:** Outline, Solid, Mini, Micro
- **Usage:** `<HeroIcon Name="HeroIconName.Home" Variant="HeroIconVariant.Outline" />`

### Feather Icons
- **Package:** `NeoBlazorUI.Icons.Feather`
- **Icon Count:** 286+
- **Style:** Minimalist, stroke-based
- **Usage:** `<FeatherIcon Name="FeatherIconName.Home" />`

---

## 🔍 Common Patterns

### AsChild Pattern

The `AsChild` pattern allows you to use a component's behavior without its default rendering:

```razor
<DialogTrigger AsChild>
    <Button>Open Dialog</Button>
</DialogTrigger>
```

When `AsChild="true"`, the component passes its functionality to its child element instead of rendering its own wrapper.

### Form Validation

Use the `Field` component with `EditForm` for integrated validation:

```razor
<EditForm Model="model" OnValidSubmit="HandleSubmit">
    <DataAnnotationsValidator />
    
    <Field>
        <FieldLabel>Username</FieldLabel>
        <FieldControl>
            <Input @bind-Value="model.Username" />
        </FieldControl>
        <FieldDescription>Choose a unique username</FieldDescription>
        <FieldMessage />
    </Field>
    
    <Button Type="ButtonType.Submit">Submit</Button>
</EditForm>
```

---

## 📖 Additional Resources

- **Live Demo:** https://blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net
- **GitHub:** https://github.com/blazorui-net/ui
- **NuGet:** https://www.nuget.org/packages?q=NeoBlazorUI

---

## 📚 Quick Reference

### Component Categories

**Layout & Structure:**
- AspectRatio - Responsive aspect ratio container
- Card - Content container with header, body, footer
- Grid - Responsive grid layout
- Resizable - Resizable panel layout
- ScrollArea - Custom styled scrollbar
- Separator - Visual divider

**Navigation:**
- Breadcrumb - Navigation breadcrumb trail
- NavigationMenu - Primary navigation menu
- Menubar - Desktop application-style menu
- Pagination - Page navigation controls
- Tabs - Tab-based content organization

**Forms & Inputs:**
- Button - Interactive button
- Checkbox - Checkbox input
- Input - Text input field
- InputOtp - One-time password input
- Label - Form label
- NativeSelect - Native HTML select
- RadioGroup - Radio button group
- Select - Custom select dropdown
- Slider - Range slider input
- Switch - Toggle switch
- Textarea - Multi-line text input
- DatePicker - Date selection
- TimePicker - Time selection
- Calendar - Calendar date picker
- MultiSelect - Multiple selection dropdown
- Combobox - Autocomplete combo box
- Field - Form field wrapper with validation

**Data Display:**
- Avatar - User avatar with fallback
- Badge - Status or label badge
- DataTable - Advanced data table
- Empty - Empty state placeholder
- Kbd - Keyboard shortcut display
- Progress - Progress indicator
- Skeleton - Loading skeleton
- Spinner - Loading spinner
- Toast - Toast notification
- Typography - Text styling

**Overlays & Popups:**
- Dialog - Modal dialog
- AlertDialog - Confirmation dialog
- Sheet - Side sheet/drawer
- Popover - Floating content popover
- HoverCard - Hover triggered card
- Tooltip - Informative tooltip
- ContextMenu - Right-click context menu
- DropdownMenu - Dropdown menu
- Command - Command palette

**Disclosure:**
- Accordion - Expandable accordion
- Collapsible - Collapsible content
- Toggle - Toggle button
- ToggleGroup - Toggle button group

**Rich Content:**
- MarkdownEditor - Markdown editor
- RichTextEditor - WYSIWYG editor
- Chart - Chart components

**Animation & Effects:**
- HeightAnimation - Animated height transitions
- Motion - Motion animations

**Sidebar:**
- Sidebar - Application sidebar

**Other:**
- Alert - Alert message
- ButtonGroup - Button group
- Carousel - Image/content carousel
- Item - Generic item component

---

## 🔍 Search Index

### By Use Case

**Need a button?** → Button, Toggle, ToggleGroup
**Need a form?** → Field, Input, Checkbox, Switch, RadioGroup, Select, Textarea, DatePicker, TimePicker
**Need validation?** → Field (with FieldMessage)
**Need a popup/modal?** → Dialog, AlertDialog, Sheet, Popover, Tooltip
**Need a menu?** → DropdownMenu, ContextMenu, NavigationMenu, Menubar
**Need navigation?** → Breadcrumb, Pagination, Tabs, NavigationMenu
**Need to display data?** → DataTable, Card, Badge, Avatar
**Need loading state?** → Spinner, Skeleton, Progress
**Need notifications?** → Toast, Alert
**Need expandable content?** → Accordion, Collapsible
**Need a layout?** → Grid, Card, Resizable, Separator
**Need date/time selection?** → DatePicker, TimePicker, Calendar
**Need rich text editing?** → RichTextEditor, MarkdownEditor
**Need icons?** → Lucide, Heroicons, Feather packages

### Alphabetical Index

A: Accordion, Alert, AlertDialog, AspectRatio, Avatar  
B: Badge, Breadcrumb, Button, ButtonGroup  
C: Calendar, Card, Carousel, Chart, Checkbox, Collapsible, Combobox, Command, ContextMenu  
D: DataTable, DatePicker, Dialog, DropdownMenu  
E: Empty  
F: Field  
G: Grid  
H: HeightAnimation, HoverCard  
I: Input, InputGroup, InputOtp, Item  
K: Kbd  
L: Label  
M: MarkdownEditor, Menubar, Motion, MultiSelect  
N: NativeSelect, NavigationMenu  
P: Pagination, Popover, Progress  
R: RadioGroup, Resizable, RichTextEditor  
S: ScrollArea, Select, Separator, Sheet, Sidebar, Skeleton, Slider, Spinner, Switch  
T: Table, Tabs, Textarea, TimePicker, Toast, Toggle, ToggleGroup, Tooltip, Typography

---

## 📄 License

MIT License - Copyright (c) 2025 BlazorUI Contributors

---

**Document Statistics:**
- Styled Components: 62
- Primitives: 21
- Total Enums: 75
- Total Sections: 158
- Generated: 2026-01-26
