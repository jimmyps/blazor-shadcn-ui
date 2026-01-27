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

**Description:**
Vertically stacked collapsible content sections with smooth animations. Supports single or multiple panel expansion with keyboard navigation.

**Components & Parameters:**

#### `Accordion`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the accordion. Should include AccordionItem components. |
| `Value` | `HashSet<string>?` |  | Controls which items are open (controlled mode). |
| `ValueChanged` | `EventCallback<HashSet<string>>` |  | Event callback invoked when the open items change. |
| `DefaultValue` | `HashSet<string>?` |  | Default open items when in uncontrolled mode. |
| `OnValueChange` | `EventCallback<HashSet<string>>` |  | Event callback invoked when the open items change. |
| `Type` | `AccordionType` | `AccordionType.Single` | Type of accordion (single or multiple). |
| `Collapsible` | `bool` | `false` | Whether items can be collapsed when in single mode. |
| `Class` | `string?` |  | Additional CSS classes to apply to the accordion. |

#### `AccordionContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ForceMount` | `bool` | `true` | Whether to force mount the content even when closed. When true (default), enables smooth CSS animations. When false, content unmounts when closed (no animation, lower memory). |
| `ChildContent` | `RenderFragment?` |  | The child content to render when the item is open. |
| `Class` | `string?` |  | Additional CSS classes to apply to the content. |

#### `AccordionItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Disabled` | `bool` |  | Whether this accordion item is disabled. |
| `ChildContent` | `RenderFragment?` |  | The child content to render within the accordion item. |
| `Class` | `string?` |  | Additional CSS classes to apply to the accordion item. |

#### `AccordionTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply to the trigger. |

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

**Description:**
Status messages and callouts with multiple variants (default, destructive). Used to display important information to users.

**Components & Parameters:**

#### `Alert`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `AlertVariant` | `AlertVariant.Default` | Gets or sets the visual style variant of the alert. Controls the color scheme and visual appearance using CSS custom properties. Default value is . |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the alert. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the alert. Typically contains AlertTitle, AlertDescription, and optionally an icon. For accessibility, ensure meaningful content is provided. |

#### `AlertDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the alert description. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the alert description. |

#### `AlertTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the alert title. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the alert title. |

**Basic Usage:**
```razor
@* Default alert *@
<Alert>
    <AlertTitle>Note</AlertTitle>
    <AlertDescription>This is an informational message.</AlertDescription>
</Alert>

@* Destructive alert *@
<Alert Variant="AlertVariant.Destructive">
    <AlertTitle>Error</AlertTitle>
    <AlertDescription>Something went wrong. Please try again.</AlertDescription>
</Alert>

@* With icon *@
<Alert>
    <LucideIcon Name="info" Size="16" />
    <AlertTitle>Information</AlertTitle>
    <AlertDescription>Check your email for confirmation.</AlertDescription>
</Alert>
```

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

**Description:**
Modal dialog for critical confirmations requiring user action. Prevents accidental destructive actions with explicit confirm/cancel buttons.

**Components & Parameters:**

#### `AlertDialog`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the alert dialog. |
| `Open` | `bool?` |  | Controls whether the alert dialog is open (controlled mode). When null, the dialog manages its own state (uncontrolled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. Use with @bind-Open for two-way binding. |
| `DefaultOpen` | `bool` | `false` | Default open state when in uncontrolled mode. |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the dialog open state changes. |

#### `AlertDialogAction`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render as the action button. |

#### `AlertDialogCancel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render as the cancel button. |

#### `AlertDialogContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the alert dialog. |
| `Class` | `string?` |  | Additional CSS classes to apply to the alert dialog content. |
| `CloseOnEscape` | `bool` | `true` | Whether the alert dialog can be closed with the Escape key. Default is true for accessibility. |

#### `AlertDialogDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `AlertDialogFooter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `AlertDialogHeader`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `AlertDialogTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `AlertDialogTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render as the trigger. |

**Basic Usage:**
```razor
<AlertDialog>
    <AlertDialogTrigger AsChild>
        <Button Variant="ButtonVariant.Destructive">Delete Account</Button>
    </AlertDialogTrigger>
    <AlertDialogContent>
        <AlertDialogHeader>
            <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
            <AlertDialogDescription>
                This action cannot be undone. This will permanently delete your account.
            </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
            <AlertDialogCancel AsChild>
                <Button Variant="ButtonVariant.Outline">Cancel</Button>
            </AlertDialogCancel>
            <AlertDialogAction AsChild>
                <Button Variant="ButtonVariant.Destructive">Delete</Button>
            </AlertDialogAction>
        </AlertDialogFooter>
    </AlertDialogContent>
</AlertDialog>
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

**Description:**
Display content within a desired width/height ratio. Maintains consistent proportions across different screen sizes.

**Components & Parameters:**

#### `AspectRatio`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render within the aspect ratio container. |
| `Ratio` | `double` | `1.0` | The desired aspect ratio (width / height). Default is 1 (square). Common values: 16/9 = 1.778, 4/3 = 1.333, 1/1 = 1. |
| `Class` | `string?` |  | Additional CSS classes to apply to the container. |

**Basic Usage:**
```razor
@* 16:9 video container *@
<AspectRatio Ratio="16.0 / 9.0">
    <img src="/images/placeholder.jpg" alt="Photo" class="rounded-md object-cover" />
</AspectRatio>

@* Square 1:1 *@
<AspectRatio Ratio="1">
    <img src="/images/avatar.jpg" alt="Avatar" class="rounded-md object-cover" />
</AspectRatio>

@* 4:3 classic *@
<AspectRatio Ratio="4.0 / 3.0">
    <iframe src="https://example.com" class="w-full h-full"></iframe>
</AspectRatio>
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

**Description:**
User avatars with image fallback support. Displays user profile pictures with automatic fallback to initials or placeholder.

**Components & Parameters:**

#### `Avatar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to render inside the avatar. Typically contains AvatarImage and AvatarFallback components. The first successfully loaded content will be displayed. |
| `Size` | `AvatarSize` | `AvatarSize.Default` | Gets or sets the size variant of the avatar. Controls the dimensions and font-size of the avatar. Default value is . |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the avatar container. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |

#### `AvatarFallback`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to render as fallback. Typically contains: - User initials (e.g., "JD" for John Doe) - An icon component (e.g., LucideIcon with "user") - Custom markup or components Content is automatically centered within the avatar. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the fallback container. Custom classes are appended after the component's base classes, allowing for style overrides such as custom background colors. |

#### `AvatarImage`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Source` | `string?` |  | Gets or sets the URL of the image to display. Should be a valid image URL. If the image fails to load, the component will hide and defer to AvatarFallback. |
| `Alt` | `string?` |  | Gets or sets the alternative text for the image. Essential for accessibility. Screen readers use this to describe the image to visually impaired users. Should describe who the avatar represents (e.g., "John Doe", "User avatar"). |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the image. Custom classes are appended after the component's base classes. |

**Basic Usage:**
```razor
@* Avatar with image *@
<Avatar>
    <AvatarImage Src="/images/avatar.jpg" Alt="User Name" />
    <AvatarFallback>UN</AvatarFallback>
</Avatar>

@* Avatar with fallback initials *@
<Avatar>
    <AvatarFallback>JD</AvatarFallback>
</Avatar>

@* Avatar group *@
<div class="flex -space-x-4">
    <Avatar>
        <AvatarImage Src="/images/user1.jpg" Alt="User 1" />
        <AvatarFallback>U1</AvatarFallback>
    </Avatar>
    <Avatar>
        <AvatarImage Src="/images/user2.jpg" Alt="User 2" />
        <AvatarFallback>U2</AvatarFallback>
    </Avatar>
</div>
```

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

**Description:**
Status badges and labels with multiple variants. Used to highlight status, categories, or counts.

**Components & Parameters:**

#### `Badge`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `BadgeVariant` | `BadgeVariant.Default` | Gets or sets the visual style variant of the badge. Controls the color scheme and visual appearance using CSS custom properties. Default value is . |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the badge. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the badge. Typically contains short text (1-2 words) or a small number. For accessibility, ensure the content is meaningful. |

**Basic Usage:**
```razor
@* Default badge *@
<Badge>Default</Badge>

@* Variant badges *@
<Badge Variant="BadgeVariant.Secondary">Secondary</Badge>
<Badge Variant="BadgeVariant.Destructive">Destructive</Badge>
<Badge Variant="BadgeVariant.Outline">Outline</Badge>

@* With icon *@
<Badge>
    <LucideIcon Name="check" Size="12" />
    Verified
</Badge>
```

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

**Description:**
Hierarchical navigation with customizable separators. Shows current location within site hierarchy.

**Components & Parameters:**

#### `Breadcrumb`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `AriaLabel` | `string` | `"breadcrumb"` | Gets or sets the aria-label for the breadcrumb navigation. Provides accessible label for screen readers. Default value is "breadcrumb". |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the breadcrumb. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the breadcrumb. |

#### `BreadcrumbEllipsis`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the breadcrumb ellipsis. |

#### `BreadcrumbItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the breadcrumb item. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the breadcrumb item. |

#### `BreadcrumbLink`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Href` | `string?` |  | Gets or sets the href attribute for the link. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the breadcrumb link. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the breadcrumb link. |

#### `BreadcrumbList`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the breadcrumb list. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the breadcrumb list. |

#### `BreadcrumbPage`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the breadcrumb page. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the breadcrumb page. |

#### `BreadcrumbSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the breadcrumb separator. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets custom content for the separator. If not provided, a default chevron icon is used. |

**Basic Usage:**
```razor
<Breadcrumb>
    <BreadcrumbList>
        <BreadcrumbItem>
            <BreadcrumbLink Href="/">Home</BreadcrumbLink>
        </BreadcrumbItem>
        <BreadcrumbSeparator />
        <BreadcrumbItem>
            <BreadcrumbLink Href="/products">Products</BreadcrumbLink>
        </BreadcrumbItem>
        <BreadcrumbSeparator />
        <BreadcrumbItem>
            <BreadcrumbPage>Product Details</BreadcrumbPage>
        </BreadcrumbItem>
    </BreadcrumbList>
</Breadcrumb>
```

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

**Description:**
Interactive button component with multiple variants (default, destructive, outline, secondary, ghost, link) and sizes. Supports icons, disabled states, and full keyboard navigation.

**Components & Parameters:**

#### `Button`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `ButtonVariant` | `ButtonVariant.Default` | Gets or sets the visual style variant of the button. Controls the color scheme and visual appearance using CSS custom properties. Default value is . |
| `Size` | `ButtonSize` | `ButtonSize.Default` | Gets or sets the size of the button. Controls padding, font size, and overall dimensions. Default value is . All sizes maintain minimum touch target sizes (44x44px) for accessibility. |
| `Type` | `ButtonType` | `ButtonType.Button` | Gets or sets the HTML button type attribute. Controls form submission behavior when button is inside a form. Default value is  to prevent accidental form submissions. |
| `Disabled` | `bool` |  | Gets or sets whether the button is disabled. When disabled: - Button cannot be clicked or focused - Opacity is reduced (via disabled:opacity-50 Tailwind class) - Pointer events are disabled (via disabled:pointer-events-none) - aria-disabled attribute is set to true |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the button. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Gets or sets the callback invoked when the button is clicked. The event handler receives a  parameter with click details. If the button is disabled, this callback will not be invoked. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the button. Can contain text, icons, or any other Blazor markup. For icon-only buttons, use  and provide an aria-label. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the button. Required for icon-only buttons to provide accessible text for screen readers. Optional for buttons with text content. |
| `Icon` | `RenderFragment?` |  | Gets or sets the icon to display in the button. Can be any RenderFragment (SVG, icon font, image). Position is controlled by . Automatically adds RTL-aware spacing between icon and text. |
| `IconPosition` | `IconPosition` | `IconPosition.Start` | Gets or sets the position of the icon relative to the button text. Default value is  (before text in LTR). Automatically adapts to RTL layouts using Tailwind directional utilities. |

**Basic Usage:**
```razor
@* Simple button *@
<Button>Click me</Button>

@* With variant and size *@
<Button Variant="ButtonVariant.Destructive" Size="ButtonSize.Large">
    Delete
</Button>

@* With icon *@
<Button>
    <LucideIcon Name="download" Size="16" />
    Download
</Button>

@* Icon-only button *@
<Button Size="ButtonSize.Icon" AriaLabel="Settings">
    <LucideIcon Name="settings" Size="20" />
</Button>
```

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

**Description:**
Visually grouped related buttons with connected styling. Groups multiple buttons as a cohesive unit with shared borders.

**Components & Parameters:**

#### `ButtonGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `ButtonGroupOrientation` | `ButtonGroupOrientation.Horizontal` | Gets or sets the orientation of the button group. Controls whether buttons are arranged horizontally (default) or vertically. Default value is . |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the button group. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the button group. Provides an accessible name for the group when role="group" is used. Important for screen reader users to understand the group's purpose. Recommended for button groups that perform a specific function (e.g., "Text formatting", "Email actions"). |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the button group. Typically contains Button components, but can also contain nested ButtonGroup components for creating complex layouts. |

#### `ButtonGroupSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `SeparatorOrientation` | `SeparatorOrientation.Vertical` | Gets or sets the orientation of the separator. Should match the parent ButtonGroup's orientation. Default value is  (for horizontal button groups). |
| `Decorative` | `bool` | `true` | Gets or sets whether the separator is purely decorative. When true (default), the separator is treated as decorative and hidden from assistive technologies. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the separator. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |

#### `ButtonGroupText`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the text container. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the text container. Can contain text, icons, or any other markup. Icons will automatically be sized to match the button group's icon size. |

**Basic Usage:**
```razor
<ButtonGroup>
    <Button Variant="ButtonVariant.Outline">Left</Button>
    <Button Variant="ButtonVariant.Outline">Center</Button>
    <Button Variant="ButtonVariant.Outline">Right</Button>
</ButtonGroup>

@* With active state *@
<ButtonGroup>
    <Button Variant="ButtonVariant.Default">Bold</Button>
    <Button Variant="ButtonVariant.Outline">Italic</Button>
    <Button Variant="ButtonVariant.Outline">Underline</Button>
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

**Description:**
Date selection grid with month navigation. Interactive calendar for selecting single dates or date ranges.

**Components & Parameters:**

#### `Calendar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `SelectedDate` | `DateOnly?` |  | The currently selected date. |
| `SelectedDateChanged` | `EventCallback<DateOnly?>` |  | Event callback invoked when the selected date changes. |
| `MinDate` | `DateOnly?` |  | The minimum selectable date. |
| `MaxDate` | `DateOnly?` |  | The maximum selectable date. |
| `Culture` | `CultureInfo` | `CultureInfo.CurrentCulture` | The culture to use for formatting. Default is the current culture. |
| `InitialMonth` | `DateOnly?` |  | The initial month to display. Defaults to the selected date's month or current month. |
| `IsDateDisabled` | `Func<DateOnly, bool>?` |  | Function to determine if a specific date is disabled. |
| `Class` | `string?` |  | Additional CSS classes to apply to the calendar. |
| `RowClass` | `string?` |  | Additional CSS classes to apply to each week row in the day grid. |
| `CaptionLayout` | `CalendarCaptionLayout` | `CalendarCaptionLayout.Label` | The caption layout mode for month/year display. Default is Label (static text). Dropdown allows quick month/year selection. |
| `YearRange` | `int` | `20` | The number of years to show before and after the current year in dropdown mode. Default is 20 years in each direction. |
| `RangeStart` | `DateOnly?` |  | The start date of a range selection (for visual styling). |
| `RangeEnd` | `DateOnly?` |  | The end date of a range selection (for visual styling). |
| `DisplayedMonthChanged` | `EventCallback<DateOnly>` |  | Event callback invoked when the displayed month changes. This fires whenever the user navigates to a different month via buttons, keyboard, or dropdown. |

**Basic Usage:**
```razor
@* Simple calendar *@
<Calendar @bind-Value="selectedDate" />

@* Date range calendar *@
<Calendar @bind-Value="dateRange" Mode="CalendarMode.Range" />

@* With disabled dates *@
<Calendar @bind-Value="selectedDate" 
          DisabledDates="@disabledDates" />

@code {
    private DateTime? selectedDate = DateTime.Today;
    private DateRange? dateRange;
    private HashSet<DateTime> disabledDates = new() { DateTime.Today.AddDays(1) };
}
```

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

**Description:**
Container for grouped content with header, body, and footer sections. Flexible component for displaying information, forms, or actions.

**Components & Parameters:**

#### `Card`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the card. Typically contains CardHeader, CardContent, and CardFooter components. Can contain any Blazor markup. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the card. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |

#### `CardAction`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered in the card action area. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the card action area. |

#### `CardContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the card content area. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the card content. |

#### `CardDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered as the card description. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the card description. |

#### `CardFooter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the card footer. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the card footer. |

#### `CardHeader`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the card header. Typically contains CardTitle, CardDescription, and optionally CardAction. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the card header. |

#### `CardTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered as the card title. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the card title. |

**Basic Usage:**
```razor
<Card>
    <CardHeader>
        <CardTitle>Card Title</CardTitle>
        <CardDescription>Card description goes here</CardDescription>
    </CardHeader>
    <CardContent>
        <p>Main content of the card.</p>
    </CardContent>
    <CardFooter Class="flex justify-between">
        <Button Variant="ButtonVariant.Outline">Cancel</Button>
        <Button>Save</Button>
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

**Description:**
Slideshow component with touch gestures and animations. Displays content in a rotating carousel with navigation controls.

**Components & Parameters:**

#### `Carousel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content containing CarouselItem components. |
| `Orientation` | `CarouselOrientation` | `CarouselOrientation.Horizontal` | Orientation of the carousel. Default is Horizontal. |
| `ShowNavigation` | `bool` | `true` | Whether to show navigation arrows. Default is true. |
| `ShowIndicators` | `bool` | `false` | Whether to show dot indicators. Default is false. |
| `AutoPlay` | `bool` | `false` | Whether to enable auto-play. Default is false. |
| `AutoPlayInterval` | `int` | `3000` | Auto-play interval in milliseconds. Default is 3000. |
| `Loop` | `bool` | `false` | Whether to enable loop (infinite scroll). Default is false. |
| `SlidesPerView` | `int` | `1` | Number of slides to show at once. Default is 1. |
| `Gap` | `int` | `0` | Gap between slides in pixels. Default is 0. |
| `EnableDrag` | `bool` | `false` | Whether to enable drag gestures. Default is false. |
| `Class` | `string?` |  | Additional CSS classes for the carousel container. |
| `ContentClass` | `string?` |  | Additional CSS classes for the inner content container that holds the slides. |
| `OnSlideChange` | `EventCallback<int>` |  | Event callback invoked when the current slide index changes. |

#### `CarouselItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content of the carousel item. |
| `Class` | `string?` |  | Additional CSS classes for the item. |

**Basic Usage:**
```razor
<Carousel>
    <CarouselContent>
        <CarouselItem>
            <Card>
                <CardContent Class="p-6">
                    <h3>Slide 1</h3>
                </CardContent>
            </Card>
        </CarouselItem>
        <CarouselItem>
            <Card>
                <CardContent Class="p-6">
                    <h3>Slide 2</h3>
                </CardContent>
            </Card>
        </CarouselItem>
    </CarouselContent>
    <CarouselPrevious />
    <CarouselNext />
</Carousel>
```

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

**Description:**
Beautiful data visualizations with multiple chart types including Area, Bar, Composed, Line, Pie, Radar, Radial Bar, and Scatter charts.

**Components & Parameters:**

#### `Area`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `string?` |  | Gets or sets the area color (legacy, use Fill instead). |
| `Emphasis` | `bool` | `false` | Gets or sets whether emphasis is enabled. |
| `Focus` | `Focus` | `Focus.None` | Gets or sets the focus behavior. |
| `ShowDots` | `bool` | `false` | Gets or sets whether to show dots at data points. |
| `LineWidth` | `int` | `1` | Gets or sets the line width in pixels. |
| `Opacity` | `double?` |  | Gets or sets the opacity for the area fill (0.0 to 1.0). Maps to series.areaStyle.opacity. If Fill.Opacity is also set, Fill.Opacity takes precedence. |
| `StackId` | `string?` |  | Gets or sets the stack group ID for stacked areas. |
| `Interpolation` | `InterpolationType` | `InterpolationType.Natural` | Gets or sets the interpolation type for the area. |

#### `AreaChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `StackOffset` | `StackOffset` | `StackOffset.None` | Gets or sets the stack offset mode for percentage stacking. |

#### `AxisLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Show` | `bool?` |  | Gets or sets whether to show axis labels. |
| `Rotate` | `int?` |  | Gets or sets the rotation angle in degrees (clockwise). Common value: -45. |
| `Formatter` | `string?` |  | Gets or sets the formatter string. Can be an ECharts template string or JavaScript function. Examples: "{value}", "function(value) { return value + '%'; }" |
| `Interval` | `int?` |  | Gets or sets the interval for showing labels. Shows every Nth label. |
| `Inside` | `bool?` |  | Gets or sets whether to render labels inside the chart area. |
| `Margin` | `int?` |  | Gets or sets the distance to the axis line in pixels. |
| `HideOverlap` | `bool?` |  | Gets or sets whether to hide overlapping labels. |
| `Color` | `string?` |  | Gets or sets the label color. CSS variable or hex color. |
| `FontSize` | `int?` |  | Gets or sets the font size in pixels. |
| `FontFamily` | `string?` |  | Gets or sets the font family. |
| `FontWeight` | `string?` |  | Gets or sets the font weight. E.g., "normal", "bold", "600". |
| `LineHeight` | `int?` |  | Gets or sets the line height in pixels. |
| `Align` | `string?` |  | Gets or sets the horizontal text alignment. E.g., "left", "center", "right". |
| `VerticalAlign` | `string?` |  | Gets or sets the vertical text alignment. E.g., "top", "middle", "bottom". |
| `Overflow` | `string?` |  | Gets or sets the overflow behavior. E.g., "truncate", "break", "breakAll". |
| `Width` | `int?` |  | Gets or sets the maximum width for label text. |
| `Ellipsis` | `string?` |  | Gets or sets the ellipsis character for truncated text. |

#### `Bar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `string?` |  | Gets or sets the bar color (legacy, use Fill instead). |
| `Emphasis` | `bool` | `false` | Gets or sets whether emphasis is enabled. |
| `Focus` | `Focus` | `Focus.None` | Gets or sets the focus behavior. |
| `StackId` | `string?` |  | Gets or sets the stack group ID for stacked bars. |
| `Radius` | `int?` |  | Gets or sets the border radius for bar corners. |
| `Opacity` | `double?` |  | Gets or sets the opacity for the bars (0.0 to 1.0). Maps to series.itemStyle.opacity. |

#### `BarChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Layout` | `BarLayout` | `BarLayout.Vertical` | Gets or sets the bar chart layout (vertical or horizontal). |
| `StackOffset` | `StackOffset` | `StackOffset.None` | Gets or sets the stack offset mode for percentage stacking. |

#### `CenterLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Text` | `string?` |  | Gets or sets the text to display in the center. |
| `FontSize` | `double?` |  | Gets or sets the font size. |
| `Color` | `string?` |  | Gets or sets the text color. |
| `FontWeight` | `string?` |  | Gets or sets the font weight. |

#### `ChartContainer`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content. |

#### `ChartDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content. |

#### `ChartEmptyState`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Height` | `int` | `350` | Gets or sets the height of the empty state in pixels. |
| `Title` | `string` | `"No data available"` | Gets or sets the title text to display. |
| `Description` | `string?` |  | Gets or sets the description text to display. |
| `ShowIcon` | `bool` | `true` | Gets or sets whether to show the chart icon. |
| `Content` | `RenderFragment?` |  | Gets or sets custom content to display instead of the default message. |
| `Class` | `string?` |  | Gets or sets additional CSS classes. |

#### `ChartHeader`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content. |

#### `ChartSkeleton`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Height` | `int` | `350` | Gets or sets the height of the skeleton in pixels. |
| `BarCount` | `int` | `8` | Gets or sets the number of skeleton bars to display. |
| `Class` | `string?` |  | Gets or sets additional CSS classes. |

#### `ChartTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content. |

#### `ComposedChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| *(No parameters)* | | | |

#### `Fill`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `string?` |  | Gets or sets the fill color. |
| `Opacity` | `double?` |  | Gets or sets the fill opacity. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content (for LinearGradient). |

#### `Grid`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Show` | `bool` | `true` | Gets or sets whether to show grid lines. |
| `Horizontal` | `bool` | `true` | Gets or sets whether to show horizontal grid lines. |
| `Vertical` | `bool` | `true` | Gets or sets whether to show vertical grid lines. |
| `Stroke` | `string?` |  | Gets or sets the stroke color for grid lines. |
| `StrokeWidth` | `int?` |  | Gets or sets the stroke width for grid lines in pixels. Maps to grid line lineStyle.width. |
| `StrokeType` | `LineStyleType` | `LineStyleType.Solid` | Gets or sets the stroke/line type for grid lines. Maps to grid line lineStyle.type. |
| `Opacity` | `double?` |  | Gets or sets the opacity for grid lines (0.0 to 1.0). Maps to grid line lineStyle.opacity. |

#### `LabelLine`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Show` | `bool` | `true` | Whether to show the label line. |
| `Length` | `double?` |  | Length of the first segment (in pixels). |
| `Length2` | `double?` |  | Length of the second segment (in pixels). |
| `Smooth` | `bool?` |  | Whether to use smooth curved lines. When true, label lines will be smoothly curved. When false or null, label lines will be straight. |

#### `LabelList`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Position` | `LabelPosition` | `LabelPosition.Top` | Label position. |
| `Formatter` | `string?` |  | Label formatter (template string or JS function string). |
| `Color` | `string?` |  | Label text color. |
| `FontSize` | `double?` |  | Label font size. |
| `Offset` | `double?` |  | Label offset from data point. |
| `Show` | `bool` | `true` | Whether to show labels. |

#### `Legend`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Show` | `bool` | `true` | Gets or sets whether to show the legend. |
| `Layout` | `LegendLayout` | `LegendLayout.Horizontal` | Gets or sets the legend layout orientation. |
| `Align` | `LegendAlign` | `LegendAlign.Center` | Gets or sets the horizontal alignment of the legend. |
| `VerticalAlign` | `LegendVerticalAlign` | `LegendVerticalAlign.Top` | Gets or sets the vertical alignment of the legend. |
| `MarginTop` | `int` | `0` | Gets or sets the top margin in pixels (applies only when VerticalAlign is Top). |
| `Icon` | `LegendIcon` | `LegendIcon.Circle` | Gets or sets the icon type for legend items. Maps to ECharts legend.icon property. |
| `TextColor` | `string?` |  | Gets or sets the text color for legend labels. Supports CSS variables like 'var(--foreground)'. |

#### `Line`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `string?` |  | Gets or sets the line color (legacy, use Fill instead). |
| `Emphasis` | `bool` | `false` | Gets or sets whether emphasis is enabled. |
| `Focus` | `Focus` | `Focus.None` | Gets or sets the focus behavior. |
| `ShowDots` | `bool` | `false` | Gets or sets whether to show dots at data points. |
| `DotSize` | `int` | `4` | Gets or sets the size of dots/symbols at data points in pixels. Maps to series.symbolSize. |
| `DotShape` | `SymbolShape` | `SymbolShape.EmptyCircle` | Gets or sets the shape of dots/symbols at data points. Maps to series.symbol. |
| `LineWidth` | `int` | `1` | Gets or sets the line width in pixels. |
| `Opacity` | `double?` |  | Gets or sets the opacity for the line (0.0 to 1.0). Maps to series.lineStyle.opacity or itemStyle.opacity. |
| `Dashed` | `bool` | `false` | Gets or sets whether the line should be dashed. |
| `Interpolation` | `InterpolationType` | `InterpolationType.Natural` | Gets or sets the interpolation type for the line. |

#### `LineChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| *(No parameters)* | | | |

#### `LinearGradient`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Direction` | `GradientDirection` | `GradientDirection.Vertical` | Gets or sets the gradient direction. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content (for Stop components). |

#### `Pie`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `InnerRadius` | `string?` |  | Gets or sets the inner radius for donut charts (e.g., "60" or "60%"). |
| `OuterRadius` | `string` | `"75%"` | Gets or sets the outer radius (e.g., "75%", "80%", "200"). Controls the overall size of the pie chart. |
| `StartAngle` | `int` | `90` | Gets or sets the start angle in degrees (0-360). 90 = top, 0 = right, 180 = bottom, 270 = left. |
| `EndAngle` | `int` | `450` | Gets or sets the end angle in degrees. For full circle: StartAngle + 360. For semi-circle gauge: StartAngle + 180. |
| `RoseType` | `string?` |  | Gets or sets the rose chart type. "radius" - radius represents value (Nightingale rose chart). "area" - area represents value. null - normal pie chart. |
| `PadAngle` | `int` | `0` | Gets or sets the padding angle between slices in degrees. |
| `Center` | `string[]?` |  | Gets or sets the center position as [x, y] (e.g., ["50%", "50%"]). |
| `SelectedMode` | `string?` |  | Gets or sets the selection mode. "single" - single slice selection. "multiple" - multiple slice selection. null/false - no selection. |
| `SelectedOffset` | `int` | `10` | Gets or sets the offset distance when slice is selected (pixels). |
| `MinAngle` | `int` | `0` | Gets or sets the minimum angle for a slice to be visible (degrees). Small value slices below this threshold will be hidden. |
| `Color` | `string?` |  | Gets or sets the default color (for auto palette override, legacy). |
| `ShowLabelLine` | `bool` | `true` | Gets or sets whether to show label lines. |
| `EmphasisScale` | `bool?` |  | Gets or sets whether to enable emphasis (hover) scaling. |
| `EmphasisScaleSize` | `int?` |  | Gets or sets the emphasis scale size in pixels (default 5-10). |
| `EmphasisFocus` | `string?` |  | Gets or sets the emphasis focus mode ("self", "series", "none"). |
| `EmphasisShowLabel` | `bool?` |  | Gets or sets whether to show labels on hover (emphasis). |
| `EmphasisLabelFormatter` | `string?` |  | Gets or sets the label formatter for emphasis state. |

#### `PieChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| *(No parameters)* | | | |

#### `PolarGrid`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Type` | `PolarGridType` | `PolarGridType.Circle` | Gets or sets the grid type (Circle or Polygon). |
| `Show` | `bool` | `true` | Gets or sets whether to show the grid. |
| `Stroke` | `string?` |  | Gets or sets the grid line color. |
| `StrokeWidth` | `int?` |  | Gets or sets the grid line width. |
| `SplitNumber` | `int` | `5` | Gets or sets the number of split levels (concentric rings). |
| `ShowAxisLine` | `bool` | `true` | Gets or sets whether to show the angle axis line. |
| `ShowSplitLine` | `bool` | `true` | Gets or sets whether to show split lines. |

#### `Radar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `string?` |  | Gets or sets the series color (legacy, use Fill instead). |
| `Emphasis` | `bool` | `false` | Gets or sets whether emphasis is enabled. |
| `Focus` | `Focus` | `Focus.None` | Gets or sets the focus behavior. |
| `FillArea` | `bool` | `false` | Gets or sets whether to fill the radar area. |
| `AreaOpacity` | `double?` |  | Gets or sets the area fill opacity (0.0 to 1.0). Only applies when FillArea is true. |
| `LineWidth` | `int` | `2` | Gets or sets the line width in pixels. |

#### `RadarChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| *(No parameters)* | | | |

#### `RadarGrid`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Shape` | `RadarShape` | `RadarShape.Polygon` | Gets or sets the radar grid shape. |
| `SplitNumber` | `int` | `5` | Gets or sets the number of split levels (concentric grid lines). |
| `Radius` | `string` | `"75%"` | Gets or sets the radar radius relative to container (e.g., "75%", "200"). |
| `Center` | `string[]?` |  | Gets or sets the center position [x, y] (e.g., ["50%", "50%"]). |
| `ShowAxisLine` | `bool` | `true` | Gets or sets whether to show axis lines (spokes). |
| `ShowSplitLine` | `bool` | `true` | Gets or sets whether to show split lines (rings). |
| `AxisLineColor` | `string?` |  | Gets or sets the axis line color. |
| `AxisLineWidth` | `int?` |  | Gets or sets the axis line width. |
| `SplitLineColor` | `string?` |  | Gets or sets the split line color. |
| `SplitLineWidth` | `int?` |  | Gets or sets the split line width. |
| `ShowIndicatorLabels` | `bool` | `true` | Gets or sets whether to show radar indicator labels (axis names like "Coding", "Design", etc.). |
| `IndicatorColor` | `string?` |  | Gets or sets the text color for radar indicator labels. Supports CSS variables like 'var(--foreground)'. |
| `IndicatorFontSize` | `int?` |  | Gets or sets the font size for radar indicator labels. |
| `IndicatorFontWeight` | `string?` |  | Gets or sets the font weight for radar indicator labels (e.g., "normal", "bold"). |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content. |

#### `RadialBar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `string?` |  | Gets or sets the series color (legacy, use Fill instead). |
| `InnerRadius` | `string?` |  | Gets or sets the inner radius (e.g., "60%" or "60"). |
| `OuterRadius` | `string?` |  | Gets or sets the outer radius (e.g., "80%" or "80"). |
| `CornerRadius` | `int?` |  | Gets or sets the corner radius for rounded bars. |
| `StackId` | `string?` |  | Gets or sets the stack group ID for stacked radial bars. |
| `ShowBackground` | `bool` | `false` | Gets or sets whether to show a background track. |
| `BackgroundColor` | `string?` |  | Gets or sets the background track color. |
| `BackgroundOpacity` | `double?` | `0.1` | Gets or sets the background track opacity (0.0 to 1.0). |

#### `RadialBarChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `StartAngle` | `int` | `90` | Gets or sets the start angle in degrees (0-360). 90 = top, 0 = right, 180 = bottom, 270 = left. |
| `EndAngle` | `int` | `450` | Gets or sets the end angle in degrees. For full circle: 450 (90 + 360). For semi-circle gauge: 270 (90 + 180). |
| `Clockwise` | `bool` | `true` | Gets or sets whether angle axis runs clockwise. |
| `RadiusMin` | `double?` | `0` | Gets or sets the minimum value for the radius axis. |
| `RadiusMax` | `double?` |  | Gets or sets the maximum value for the radius axis. |

#### `Scatter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Color` | `string?` |  | Gets or sets the marker color (legacy, use Fill instead). Note: Scatter uses XAxis.DataKey and YAxis.DataKey for X/Y coordinates. |
| `SymbolSize` | `int` | `10` | Gets or sets the size of scatter symbols in pixels. Maps to series.symbolSize. |
| `Symbol` | `SymbolShape` | `SymbolShape.Circle` | Gets or sets the shape of scatter symbols. Maps to series.symbol. |
| `Emphasis` | `bool` | `false` | Gets or sets whether emphasis is enabled. |
| `Focus` | `Focus` | `Focus.None` | Gets or sets the focus behavior. |
| `SymbolSizeFunction` | `string?` |  | Gets or sets a JavaScript function string to dynamically calculate symbol size. Example: "function(data) { return data[2] / 100; }" for bubble charts. If set, overrides SymbolSize parameter. |
| `SymbolRotate` | `int?` |  | Gets or sets the symbol rotation in degrees. |
| `BorderColor` | `string?` |  | Gets or sets the border color for scatter symbols. |
| `BorderWidth` | `int?` |  | Gets or sets the border width for scatter symbols in pixels. |
| `Large` | `bool` | `false` | Gets or sets whether to enable large dataset optimization (10K+ points). |
| `LargeThreshold` | `int?` | `2000` | Gets or sets the threshold for enabling large mode automatically. |
| `Progressive` | `int?` |  | Gets or sets the progressive rendering chunk size for huge datasets. |
| `Clip` | `bool` | `true` | Gets or sets whether to clip points outside axis range. |

#### `ScatterChart`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| *(No parameters)* | | | |

#### `Stop`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Opacity` | `double?` |  | Gets or sets the opacity at this stop. |

#### `Tooltip`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Show` | `bool` | `true` | Gets or sets whether to show the tooltip. |
| `Mode` | `TooltipMode?` |  | Gets or sets the tooltip trigger mode. Default depends on chart type (see spec section 4.4). |
| `Cursor` | `TooltipCursor` | `TooltipCursor.None` | Gets or sets the cursor/axis pointer type. |
| `Formatter` | `string?` |  | Gets or sets the formatter string. Can be either an ECharts template string or a JavaScript function as a string. Examples: - Template: "{b}: {c}" - Function: "function(params) { return params.name + ': ' + params.value; }" |
| `BackgroundColor` | `string?` |  | Gets or sets the background color for the tooltip. Maps to tooltip.backgroundColor. |
| `BorderColor` | `string?` |  | Gets or sets the border color for the tooltip. Maps to tooltip.borderColor. |
| `BorderWidth` | `int?` |  | Gets or sets the border width for the tooltip in pixels. Maps to tooltip.borderWidth. |
| `TextColor` | `string?` |  | Gets or sets the text color for the tooltip. Maps to tooltip.textStyle.color. |

#### `XAxis`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Show` | `bool` | `true` | Gets or sets whether to show the X axis. |
| `DataKey` | `string?` |  | Gets or sets the data key for X axis values. |
| `Scale` | `AxisScale` | `AxisScale.Auto` | Gets or sets the scale type for the axis. |
| `Min` | `double?` |  | Gets or sets the minimum value for the axis. Maps to xAxis.min. |
| `Max` | `double?` |  | Gets or sets the maximum value for the axis. Maps to xAxis.max. |
| `Interval` | `double?` |  | Gets or sets the interval of axis ticks. Maps to xAxis.interval. |
| `AxisLine` | `bool` | `true` | Gets or sets whether to show the axis line. |
| `TickLine` | `bool` | `true` | Gets or sets whether to show tick lines. |
| `Color` | `string?` |  | Gets or sets the color of the axis line. Maps to axisLine.lineStyle.color. |
| `Width` | `int?` |  | Gets or sets the width of the axis line in pixels. Maps to axisLine.lineStyle.width. |
| `TickColor` | `string?` |  | Gets or sets the color of the tick lines. Maps to axisTick.lineStyle.color. |
| `TickWidth` | `int?` |  | Gets or sets the width of the tick lines in pixels. Maps to axisTick.lineStyle.width. |
| `BoundaryGap` | `bool?` |  | Gets or sets whether there is a gap on both sides of the axis. Default is null (auto). For bar charts, this leaves space on both sides. For line charts, this is typically set to false to have data points at the edges. Maps to xAxis.boundaryGap. |
| `IndicatorMin` | `double?` |  | Gets or sets the minimum value for this radar indicator. Only applies to RadarChart. |
| `IndicatorMax` | `double?` |  | Gets or sets the maximum value for this radar indicator. Only applies to RadarChart. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content for nested components (e.g., AxisLabel). |

#### `YAxis`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Show` | `bool` | `true` | Gets or sets whether to show the Y axis. |
| `DataKey` | `string?` |  | Gets or sets the data key for Y axis values. |
| `Scale` | `AxisScale` | `AxisScale.Auto` | Gets or sets the scale type for the axis. |
| `Position` | `YAxisPosition` | `YAxisPosition.Left` | Gets or sets the position of the Y axis (left or right). Maps to yAxis.position. |
| `Min` | `double?` |  | Gets or sets the minimum value for the axis. Maps to yAxis.min. |
| `Max` | `double?` |  | Gets or sets the maximum value for the axis. Maps to yAxis.max. |
| `Interval` | `double?` |  | Gets or sets the interval of axis ticks. Maps to yAxis.interval. |
| `AxisLine` | `bool` | `true` | Gets or sets whether to show the axis line. |
| `TickLine` | `bool` | `true` | Gets or sets whether to show tick lines. |
| `Color` | `string?` |  | Gets or sets the color of the axis line. Maps to axisLine.lineStyle.color. |
| `Width` | `int?` |  | Gets or sets the width of the axis line in pixels. Maps to axisLine.lineStyle.width. |
| `TickColor` | `string?` |  | Gets or sets the color of the tick lines. Maps to axisTick.lineStyle.color. |
| `TickWidth` | `int?` |  | Gets or sets the width of the tick lines in pixels. Maps to axisTick.lineStyle.width. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content for nested components (e.g., AxisLabel). |

**Basic Usage:**
```razor
@* Line Chart *@
<ChartContainer Config="chartConfig">
    <LineChart Data="chartData">
        <CartesianGrid StrokeDasharray="3 3" />
        <XAxis DataKey="name" />
        <YAxis />
        <ChartTooltip />
        <Line DataKey="value" Stroke="#8884d8" />
    </LineChart>
</ChartContainer>

@* Bar Chart *@
<ChartContainer Config="chartConfig">
    <BarChart Data="chartData">
        <CartesianGrid StrokeDasharray="3 3" />
        <XAxis DataKey="name" />
        <YAxis />
        <ChartTooltip />
        <Bar DataKey="value" Fill="#8884d8" />
    </BarChart>
</ChartContainer>

@code {
    private List<ChartDataItem> chartData = new()
    {
        new() { Name = "Jan", Value = 100 },
        new() { Name = "Feb", Value = 200 }
    };
}
```

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

**Description:**
Accessible checkbox with indeterminate state support. Binary selection control with support for partial selections.

**Components & Parameters:**

#### `Checkbox`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Checked` | `bool` |  | Gets or sets whether the checkbox is checked. This property supports two-way binding using the @bind-Checked directive. Changes to this property trigger the CheckedChanged event callback. |
| `CheckedChanged` | `EventCallback<bool>` |  | Gets or sets the callback invoked when the checked state changes. This event callback enables two-way binding with @bind-Checked. It is invoked whenever the user toggles the checkbox state. Also notifies EditContext for form validation. |
| `Indeterminate` | `bool` |  | Gets or sets whether the checkbox is in an indeterminate state. The indeterminate state is typically used for "select all" checkboxes when only some child items are selected. When indeterminate is true, a dash icon is displayed instead of a checkmark. |
| `IndeterminateChanged` | `EventCallback<bool>` |  | Gets or sets the callback invoked when the indeterminate state changes. |
| `Disabled` | `bool` |  | Gets or sets whether the checkbox is disabled. When disabled: - Checkbox cannot be clicked or focused - Opacity is reduced - Pointer events are disabled - aria-disabled attribute is set to true |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the checkbox. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the checkbox. Provides accessible text for screen readers when the checkbox doesn't have associated label text. |
| `Id` | `string?` |  | Gets or sets the ID attribute for the checkbox element. Used for associating the checkbox with label elements via htmlFor attribute. |
| `Name` | `string?` |  | Gets or sets the name of the checkbox for form submission. This is critical for form submission. The name/value pair is submitted to the server. If not specified, falls back to the Id value. |
| `Required` | `bool` |  | Gets or sets whether the checkbox is required. When true, the checkbox must be checked for form submission. Works with form validation. |
| `CheckedExpression` | `Expression<Func<bool>>?` |  | Gets or sets an expression that identifies the bound value. Used for form validation integration. When provided, the checkbox registers with the EditContext and participates in form validation. |

**Basic Usage:**
```razor
@* Simple checkbox *@
<Checkbox @bind-Checked="isAccepted" />

@* With label *@
<div class="flex items-center space-x-2">
    <Checkbox Id="terms" @bind-Checked="acceptTerms" />
    <Label For="terms">I accept the terms and conditions</Label>
</div>

@* Indeterminate state *@
<Checkbox Checked="false" Indeterminate="true" />

@code {
    private bool isAccepted = false;
    private bool acceptTerms = false;
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

**Description:**
Expandable/collapsible content panels. Shows or hides content with smooth animations.

**Components & Parameters:**

#### `Collapsible`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Open` | `bool` |  | Gets or sets a value indicating whether the collapsible is currently expanded. true if the collapsible is open (content visible); otherwise, false. Default is false. Supports two-way binding via @bind-Open syntax. |
| `OpenChanged` | `EventCallback<bool>` |  | Gets or sets the callback invoked when the open state changes. An  that receives the new open state, or null if no callback is provided. |
| `Disabled` | `bool` |  | Gets or sets a value indicating whether the collapsible is disabled. true if the collapsible is disabled and cannot be toggled; otherwise, false. Default is false. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the container element. A string containing one or more CSS class names, or null. Use this parameter to customize the container's appearance and layout. Common Tailwind utilities include: Borders: border rounded-lg Padding: p-4, px-6 Background: bg-muted, bg-card Spacing: space-y-2, mb-4 |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content to be rendered inside the collapsible container. A  containing the child components, or null. Typically contains:  - The button/trigger element  - The expandable content area |

#### `CollapsibleContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the content container element. A string containing one or more CSS class names, or null. Use this parameter to style the content area and add animations. Common Tailwind utilities include: Padding: p-4, px-6 py-4 Borders: border-t, border-x Background: bg-muted, bg-card Transitions: transition-all duration-300 Animation: animate-in slide-in-from-top Overflow: overflow-hidden (for smooth height transitions) |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered when the collapsible is expanded. A  containing the collapsible content, or null. |

#### `CollapsibleTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the trigger button element. A string containing one or more CSS class names, or null. Common Tailwind utilities for styling triggers: Flex layout: flex items-center gap-2 Padding: px-4 py-2 Hover states: hover:bg-accent Transitions: transition-all duration-200 |
| `AsChild` | `bool` | `false` | When true, the trigger does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to act as the trigger. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the trigger. A  containing the trigger content, or null. |

**Basic Usage:**
```razor
<Collapsible>
    <CollapsibleTrigger AsChild>
        <Button Variant="ButtonVariant.Ghost">
            <LucideIcon Name="chevron-down" Size="16" />
            Toggle Content
        </Button>
    </CollapsibleTrigger>
    <CollapsibleContent>
        <p>This content can be shown or hidden.</p>
    </CollapsibleContent>
</Collapsible>

@* Controlled mode *@
<Collapsible @bind-Open="isOpen">
    <CollapsibleTrigger AsChild>
        <Button>@(isOpen ? "Hide" : "Show") Details</Button>
    </CollapsibleTrigger>
    <CollapsibleContent>
        <p>Detailed information here.</p>
    </CollapsibleContent>
</Collapsible>

@code {
    private bool isOpen = false;
}
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

**Description:**
Searchable autocomplete dropdown with keyboard navigation. Combines text input with dropdown selection.

**Components & Parameters:**

#### `Combobox`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string?` |  | Gets or sets the currently selected value. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback that is invoked when the selected value changes. |
| `Placeholder` | `string` | `"Select an option..."` | Gets or sets the placeholder text shown in the button when no item is selected. |
| `SearchPlaceholder` | `string` | `"Search..."` | Gets or sets the placeholder text shown in the search input. |
| `EmptyMessage` | `string` | `"No results found."` | Gets or sets the message displayed when no items match the search. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the combobox container. |
| `Disabled` | `bool` |  | Gets or sets whether the combobox is disabled. |
| `PopoverWidth` | `string` | `"w-[200px]"` | Gets or sets the width of the popover content. Defaults to "w-[200px]". Can be overridden with Tailwind classes. |

**Basic Usage:**
```razor
<Combobox @bind-Value="selectedValue" 
          Placeholder="Select item...">
    <ComboboxItem Value="item1">Item 1</ComboboxItem>
    <ComboboxItem Value="item2">Item 2</ComboboxItem>
    <ComboboxItem Value="item3">Item 3</ComboboxItem>
</Combobox>

@code {
    private string? selectedValue;
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

**Description:**
Command palette with keyboard navigation and search. Quick access to actions and navigation via keyboard shortcuts.

**Components & Parameters:**

#### `Command`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the command container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the command. |
| `OnSelect` | `EventCallback<string>` |  | Event callback invoked when a command item is selected. Receives the selected value as a parameter. |

#### `CommandEmpty`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the empty state container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered when there are no results. |

#### `CommandGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the group container. |
| `Heading` | `string?` |  | Gets or sets the heading text for this group (optional). |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the group. |

#### `CommandInput`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the input container. |
| `Placeholder` | `string` | `"Type a command or search..."` | Gets or sets the placeholder text for the input. |
| `Disabled` | `bool` |  | Gets or sets whether the input is disabled. |
| `AutoFocus` | `bool` |  | Gets or sets whether the input should auto-focus when rendered. |
| `SearchInterval` | `int` | `0` | Gets or sets the debounce interval in milliseconds before triggering search. Default is 0 (no debouncing). Set to 150-300ms for better performance with large datasets. |

#### `CommandItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the item. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the item. |
| `Value` | `string?` |  | Gets or sets the value associated with this item. |
| `Disabled` | `bool` |  | Gets or sets whether this item is disabled. |
| `OnSelect` | `EventCallback` |  | Gets or sets the callback invoked when this item is selected. |
| `SearchText` | `string?` |  | Gets or sets the text used for search filtering. If not provided, uses Value. |

#### `CommandList`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the list container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the list. |

#### `CommandSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the separator. |

#### `CommandShortcut`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the shortcut. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the shortcut (e.g., "⌘K", "Ctrl+S"). |

**Basic Usage:**
```razor
<Command>
    <CommandInput Placeholder="Type a command..." />
    <CommandList>
        <CommandEmpty>No results found.</CommandEmpty>
        <CommandGroup Heading="Suggestions">
            <CommandItem>Calendar</CommandItem>
            <CommandItem>Search Emoji</CommandItem>
            <CommandItem>Calculator</CommandItem>
        </CommandGroup>
        <CommandSeparator />
        <CommandGroup Heading="Settings">
            <CommandItem>Profile</CommandItem>
            <CommandItem>Settings</CommandItem>
        </CommandGroup>
    </CommandList>
</Command>
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

**Description:**
Right-click context menus with actions and keyboard shortcuts. Displays contextual actions on right-click.

**Components & Parameters:**

#### `ContextMenu`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render. |
| `Open` | `bool?` |  | Controls whether the context menu is open. |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. |
| `OnOpen` | `EventCallback<(double X, double Y)>` |  | Event callback invoked when the context menu opens. |
| `OnClose` | `EventCallback` |  | Event callback invoked when the context menu closes. |

#### `ContextMenuCheckboxItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the checkbox item. |
| `Disabled` | `bool` | `false` | Whether the checkbox item is disabled. |
| `Checked` | `bool` | `false` | Whether the checkbox is checked. |
| `CheckedChanged` | `EventCallback<bool>` |  | Event callback invoked when the checked state changes (for two-way binding). |
| `OnCheckedChange` | `EventCallback<bool>` |  | Event callback invoked when the checkbox state changes. |
| `CloseOnSelect` | `bool` | `false` | Whether to close the context menu when this item is selected. Default is false for checkbox items. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the menu. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the menu. |
| `CloseOnClickOutside` | `bool` | `true` | Whether clicking outside should close the menu. |
| `ZIndex` | `int` | `50` | Z-index value for the menu content. |
| `LockScroll` | `bool` | `true` | Whether to lock body scroll when menu is open. Default is true. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the group. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the group. |

#### `ContextMenuItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the item. |
| `Disabled` | `bool` | `false` | Whether the item is disabled. |
| `OnClick` | `EventCallback` |  | Event callback invoked when the item is clicked. |
| `Inset` | `bool` | `false` | Whether to show the item with inset padding. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The label content. |
| `Inset` | `bool` | `false` | Whether to show the label with inset padding. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuRadioGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the radio group. Typically contains ContextMenuRadioItem components. |
| `Class` | `string?` |  | Additional CSS classes to apply to the radio group. |
| `Value` | `TValue?` |  | The currently selected value. |
| `ValueChanged` | `EventCallback<TValue?>` |  | Event callback invoked when the selected value changes (for two-way binding). |
| `OnValueChange` | `EventCallback<TValue?>` |  | Event callback invoked when the selection changes. |

#### `ContextMenuRadioItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the radio item. |
| `Disabled` | `bool` | `false` | Whether the radio item is disabled. |
| `Value` | `TValue?` |  | The value associated with this radio item. |
| `CloseOnSelect` | `bool` | `true` | Whether to close the context menu when this item is selected. Default is true for radio items. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuShortcut`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The shortcut text (e.g., "⌘C" or "Ctrl+C"). |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuSub`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the submenu. Typically contains ContextMenuSubTrigger and ContextMenuSubContent. |
| `Open` | `bool?` |  | Controls whether the submenu is open (controlled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes (for two-way binding). |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the submenu open state changes. |

#### `ContextMenuSubContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the submenu. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the submenu. |
| `Offset` | `int` | `-4` | Offset distance from the trigger in pixels. |
| `ZIndex` | `int` | `50` | Z-index for the submenu content. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuSubTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger. |
| `Disabled` | `bool` | `false` | Whether the trigger is disabled. |
| `Inset` | `bool` | `false` | Whether to show the item with inset padding. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ContextMenuTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content that triggers the context menu. |
| `Disabled` | `bool` | `false` | Whether the trigger is disabled. |

**Basic Usage:**
```razor
<ContextMenu>
    <ContextMenuTrigger>
        <div class="border rounded p-4">
            Right-click me
        </div>
    </ContextMenuTrigger>
    <ContextMenuContent>
        <ContextMenuItem>Edit</ContextMenuItem>
        <ContextMenuItem>Copy</ContextMenuItem>
        <ContextMenuSeparator />
        <ContextMenuItem>Delete</ContextMenuItem>
    </ContextMenuContent>
</ContextMenu>
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

**Description:**
Powerful tables with sorting, filtering, pagination, and row selection. Enterprise-grade data table component.

**Components & Parameters:**

#### `DataTable`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Columns` | `RenderFragment?` |  | Gets or sets the column definitions as child content. Use DataTableColumn components to define columns declaratively. |
| `SelectionMode` | `DataTableSelectionMode` | `DataTableSelectionMode.None` | Gets or sets the row selection mode. Default is None (no selection). |
| `ShowToolbar` | `bool` | `true` | Gets or sets whether to show the toolbar with global search and column visibility. Default is true. |
| `ShowPagination` | `bool` | `true` | Gets or sets whether to show pagination controls. Default is true. |
| `IsLoading` | `bool` |  | Gets or sets whether the table is in a loading state. Default is false. |
| `PageSizes` | `int[]` | `{ 10, 20, 50, 100 }` | Gets or sets the available page size options. Default is [10, 20, 50, 100]. |
| `InitialPageSize` | `int` | `10` | Gets or sets the initial page size. Default is 10. |
| `ToolbarActions` | `RenderFragment?` |  | Gets or sets custom toolbar actions (buttons, etc.). |
| `EmptyTemplate` | `RenderFragment?` |  | Gets or sets a custom template for the empty state. If null, displays default "No results found" message. |
| `LoadingTemplate` | `RenderFragment?` |  | Gets or sets a custom template for the loading state. If null, displays default "Loading..." message. |
| `Class` | `string?` |  | Gets or sets additional CSS classes for the container div. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the table. |
| `SelectedItems` | `IReadOnlyCollection<TData>` | `Array.Empty<TData>()` | Gets or sets the selected items. Use @bind-SelectedItems for two-way binding. |
| `SelectedItemsChanged` | `EventCallback<IReadOnlyCollection<TData>>` |  | Event callback invoked when the selected items change. |
| `OnSort` | `EventCallback<(string ColumnId, SortDirection Direction)>` |  | Event callback invoked when sorting changes. Use for custom sorting logic (hybrid mode). |
| `OnFilter` | `EventCallback<(string? GlobalSearch, Dictionary<string, string> ColumnFilters)>` |  | Event callback invoked when filtering changes. Use for custom filtering logic (hybrid mode). |
| `PreprocessData` | `Func<IEnumerable<TData>, Task<IEnumerable<TData>>>?` |  | Gets or sets a function to preprocess data before automatic processing. Use for custom transformations or server-side data fetching. |

#### `DataTableColumn`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string?` |  | Gets or sets the unique identifier for this column. If not provided, it will be auto-generated from the Header. |
| `Sortable` | `bool` |  | Gets or sets whether this column can be sorted. Default is false. |
| `Filterable` | `bool` |  | Gets or sets whether this column can be filtered. Default is false. |
| `Visible` | `bool` | `true` | Gets or sets whether this column is currently visible. Default is true. |
| `Width` | `string?` |  | Gets or sets the width of the column (e.g., "200px", "20%", "auto"). Null means the column will size automatically. |
| `MinWidth` | `string?` |  | Gets or sets the minimum width of the column (e.g., "100px"). Useful for responsive layouts. |
| `MaxWidth` | `string?` |  | Gets or sets the maximum width of the column (e.g., "400px"). Useful for preventing excessively wide columns. |
| `CellTemplate` | `RenderFragment<TData>?` |  | Gets or sets a custom template for rendering cell values. If null, the value is rendered using ToString(). The context parameter provides the data item (TData) for the row. &lt;DataTableColumn Property="@(p => p.Status)" Header="Status"&gt; &lt;CellTemplate Context="person"&gt; &lt;Badge Variant="@(person.Status == "Active" ? BadgeVariant.Default : BadgeVariant.Destructive)"&gt; @person.Status &lt;/Badge&gt; &lt;/CellTemplate&gt; &lt;/DataTableColumn&gt; |
| `CellClass` | `string?` |  | Gets or sets additional CSS classes to apply to cells in this column. |
| `HeaderClass` | `string?` |  | Gets or sets additional CSS classes to apply to the header cell. |

#### `DataTablePagination`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `PageSizes` | `int[]` | `{ 10, 20, 50, 100 }` |  |
| `OnPageChanged` | `EventCallback<int>` |  |  |
| `OnPageSizeChanged` | `EventCallback<int>` |  |  |

#### `DataTableToolbar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `GlobalSearchValue` | `string` | `string.Empty` |  |
| `OnGlobalSearchChanged` | `EventCallback<string>` |  |  |
| `Columns` | `List<DataTable<TData>.ColumnData>` | `new()` |  |
| `OnColumnVisibilityChanged` | `Action<string, bool>` | `null!` |  |
| `ChildContent` | `RenderFragment?` |  |  |

**Basic Usage:**
```razor
<DataTable Items="users" TItem="User">
    <Columns>
        <DataTableColumn TItem="User" Property="u => u.Name" Title="Name" />
        <DataTableColumn TItem="User" Property="u => u.Email" Title="Email" />
        <DataTableColumn TItem="User" Property="u => u.Role" Title="Role" />
    </Columns>
</DataTable>

@code {
    private List<User> users = new()
    {
        new User { Name = "John", Email = "john@example.com", Role = "Admin" },
        new User { Name = "Jane", Email = "jane@example.com", Role = "User" }
    };
    
    public class User
    {
        [Required]
        public string Name { get; set; } = "";
        
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        
        [Required]
        public string Role { get; set; } = "";
    }
}
```

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

**Description:**
Date selection with calendar in popover. Includes support for single dates and date ranges.

**Components & Parameters:**

#### `DatePicker`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `SelectedDateChanged` | `EventCallback<DateOnly?>` |  | Event callback invoked when the selected date changes. Use with @bind-SelectedDate for two-way binding. |
| `MinDate` | `DateOnly?` |  | Minimum selectable date. |
| `MaxDate` | `DateOnly?` |  | Maximum selectable date. |
| `IsDateDisabled` | `Func<DateOnly, bool>?` |  | Function to determine if a specific date should be disabled. |
| `Culture` | `System.Globalization.CultureInfo?` |  | Culture for date formatting. |
| `CaptionLayout` | `CalendarCaptionLayout` | `CalendarCaptionLayout.Label` | Calendar caption layout (label or dropdown). |
| `ButtonVariant` | `ButtonVariant` | `ButtonVariant.Outline` | Button variant for the trigger button. |
| `ButtonSize` | `ButtonSize` | `ButtonSize.Default` | Button size for the trigger button. |
| `ShowIcon` | `bool` | `true` | Whether to show the calendar icon in the button. |
| `ShowDropdownIcon` | `bool` | `false` | Whether to show the chevron-down icon on the right side of the button. |
| `Placeholder` | `string` | `"Pick a date"` | Placeholder text when no date is selected. |
| `DateFormat` | `string?` |  | Date format string for displaying the selected date. |
| `Disabled` | `bool` |  | Whether the date picker is disabled. |
| `Align` | `PopoverAlign` | `PopoverAlign.Start` | Alignment of the popover content. |
| `Class` | `string?` |  | Additional CSS classes for the button. |
| `CalendarClass` | `string?` |  | Additional CSS classes for the calendar. |
| `AriaLabel` | `string?` |  | ARIA label for the button. |

#### `DateRangePicker`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `StartDateChanged` | `EventCallback<DateOnly?>` |  | Event callback invoked when the start date changes. |
| `EndDateChanged` | `EventCallback<DateOnly?>` |  | Event callback invoked when the end date changes. |
| `MinDate` | `DateOnly?` |  | Minimum selectable date. |
| `MaxDate` | `DateOnly?` |  | Maximum selectable date. |
| `IsDateDisabled` | `Func<DateOnly, bool>?` |  | Function to determine if a specific date should be disabled. |
| `Culture` | `System.Globalization.CultureInfo?` |  | Culture for date formatting. |
| `CaptionLayout` | `CalendarCaptionLayout` | `CalendarCaptionLayout.Label` | Calendar caption layout (label or dropdown). |
| `ButtonVariant` | `ButtonVariant` | `ButtonVariant.Outline` | Button variant for the trigger button. |
| `ButtonSize` | `ButtonSize` | `ButtonSize.Default` | Button size for the trigger button. |
| `ShowIcon` | `bool` | `true` | Whether to show the calendar icon in the button. |
| `ShowDropdownIcon` | `bool` | `false` | Whether to show the chevron-down icon on the right side of the button. |
| `Placeholder` | `string` | `"Pick a date range"` | Placeholder text when no date range is selected. |
| `DateFormat` | `string?` |  | Date format string for displaying dates. |
| `Disabled` | `bool` |  | Whether the date range picker is disabled. |
| `Align` | `PopoverAlign` | `PopoverAlign.Start` | Alignment of the popover content. |
| `StartDateLabel` | `string?` |  | Label for the start date calendar. |
| `EndDateLabel` | `string?` |  | Label for the end date calendar. |
| `Class` | `string?` |  | Additional CSS classes for the button. |
| `CalendarClass` | `string?` |  | Additional CSS classes for the calendars. |
| `AriaLabel` | `string?` |  | ARIA label for the button. |

**Basic Usage:**
```razor
@* Simple date picker *@
<DatePicker @bind-Value="selectedDate" />

@* Date range picker *@
<DateRangePicker @bind-Value="dateRange" />

@* With placeholder *@
<DatePicker @bind-Value="selectedDate" 
            Placeholder="Pick a date" />

@code {
    private DateTime? selectedDate;
    private DateRange? dateRange;
}
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

**Description:**
Modal dialogs for focused interactions. Displays content in an overlay that requires user interaction.

**Components & Parameters:**

#### `Dialog`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the dialog. |
| `Open` | `bool?` |  | Controls whether the dialog is open (controlled mode). When null, the dialog manages its own state (uncontrolled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. Use with @bind-Open for two-way binding. |
| `DefaultOpen` | `bool` | `false` | Default open state when in uncontrolled mode. |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the dialog open state changes. |
| `Modal` | `bool` | `true` | Whether the dialog can be dismissed by clicking outside or pressing Escape. Default is true. |

#### `DialogClose`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the close button. |
| `AsChild` | `bool` | `false` | When true, the close does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to act as the close button. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Custom click handler. Called after the dialog is closed. |
| `PreventClose` | `bool` | `false` | Whether to prevent the default close behavior. When true, only the OnClick handler is invoked. Default is false (dialog closes on click). |

#### `DialogContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the dialog. |
| `Class` | `string?` |  | Additional CSS classes to apply to the dialog content. |
| `OverlayClass` | `string?` |  | Additional CSS classes to apply to the dialog overlay/backdrop. Merged with built-in defaults using ClassNames.cn. |
| `UseCustomAnimations` | `bool` | `false` | When true, do not include default animation-related classes so consumers can provide completely custom animations. |
| `ShowClose` | `bool` | `true` | Whether to show the close button (X icon). Default is true. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the dialog. Default is true. |
| `TrapFocus` | `bool` | `true` | Whether to trap focus within the dialog. Default is true for modal dialogs. |
| `LockScroll` | `bool` | `true` | Whether to lock body scroll when dialog is open. Default is true for modal dialogs. |
| `OnEscapeKeyDown` | `EventCallback<KeyboardEventArgs>` |  | Event callback invoked when Escape key is pressed. |

#### `DialogDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The description content to display. |
| `Class` | `string?` |  | Additional CSS classes to apply to the description. |

#### `DialogFooter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display in the dialog footer (typically action buttons). |
| `Class` | `string?` |  | Additional CSS classes to apply to the footer container. |

#### `DialogHeader`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display in the dialog header (typically DialogTitle and DialogDescription). |
| `Class` | `string?` |  | Additional CSS classes to apply to the header container. |

#### `DialogTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The title content to display. |
| `Class` | `string?` |  | Additional CSS classes to apply to the title. |

#### `DialogTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger button. |
| `AsChild` | `bool` | `false` | When true, the trigger does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to act as the trigger. |

**Basic Usage:**
```razor
<Dialog>
    <DialogTrigger AsChild>
        <Button>Open Dialog</Button>
    </DialogTrigger>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Edit Profile</DialogTitle>
            <DialogDescription>
                Make changes to your profile here.
            </DialogDescription>
        </DialogHeader>
        <div class="space-y-4">
            <Field>
                <FieldLabel>Name</FieldLabel>
                <Input @bind-Value="name" />
            </Field>
        </div>
        <DialogFooter>
            <Button>Save Changes</Button>
        </DialogFooter>
    </DialogContent>
</Dialog>

@code {
    private string name = "";
}
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

**Description:**
Context menus with nested submenus and keyboard navigation. Displays a menu of actions triggered by a button.

**Components & Parameters:**

#### `DropdownMenu`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the dropdown menu. Typically includes DropdownMenuTrigger and DropdownMenuContent. |
| `Open` | `bool?` |  | Controls whether the dropdown menu is open (controlled mode). When null, the dropdown menu manages its own state (uncontrolled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. Use with @bind-Open for two-way binding. |
| `DefaultOpen` | `bool` | `false` | Default open state when in uncontrolled mode. |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the dropdown menu open state changes. |
| `Modal` | `bool` | `true` | Whether the dropdown menu can be dismissed by clicking outside or pressing Escape. Default is true. |
| `Dir` | `string` | `"ltr"` | Direction for the menu content layout ("ltr" or "rtl"). Default is "ltr". |

#### `DropdownMenuCheckboxItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the checkbox item. |
| `Class` | `string?` |  | Additional CSS classes to apply to the checkbox item. |
| `Disabled` | `bool` | `false` | Whether the checkbox item is disabled. |
| `Checked` | `bool` | `false` | Whether the checkbox is checked. |
| `CheckedChanged` | `EventCallback<bool>` |  | Event callback invoked when the checked state changes (for two-way binding). |
| `OnCheckedChange` | `EventCallback<bool>` |  | Event callback invoked when the checkbox state changes. |
| `CloseOnSelect` | `bool` | `false` | Whether to close the dropdown menu when this item is selected. Default is false for checkbox items. |

#### `DropdownMenuContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the dropdown menu. |
| `Class` | `string?` |  | Additional CSS classes to apply to the content container. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the dropdown menu. |
| `CloseOnClickOutside` | `bool` | `true` | Whether clicking outside should close the dropdown menu. |
| `Side` | `PopoverSide` | `PopoverSide.Bottom` | Preferred side for positioning. |
| `Align` | `PopoverAlign` | `PopoverAlign.Start` | Alignment relative to trigger. |
| `Offset` | `int` | `4` | Offset distance from the trigger in pixels. |
| `OnEscapeKeyDown` | `EventCallback<KeyboardEventArgs>` |  | Event callback invoked when Escape key is pressed. |
| `OnClickOutside` | `EventCallback` |  | Event callback invoked when clicking outside. |
| `Loop` | `bool` | `true` | Whether to enable keyboard loop navigation. |
| `Strategy` | `PositioningStrategy` | `PositioningStrategy.Absolute` | Positioning strategy: "absolute" or "fixed". Use "fixed" when dropdown needs to escape stacking contexts (e.g., in sidebars). Default is "absolute". |
| `MatchTriggerWidth` | `bool` | `false` | Whether to match the trigger element's width. Default is false. |
| `ZIndex` | `int` | `50` | Z-index for the dropdown content. Default is 50. |
| `Style` | `string?` |  | Inline styles to apply to the dropdown content. |

#### `DropdownMenuGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the group. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the group. |

#### `DropdownMenuItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the menu item. |
| `Class` | `string?` |  | Additional CSS classes to apply to the menu item. |
| `Disabled` | `bool` | `false` | Whether the menu item is disabled. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Custom click handler. |
| `CloseOnSelect` | `bool` | `true` | Whether to close the dropdown menu when this item is selected. |

#### `DropdownMenuLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the label. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the label. |

#### `DropdownMenuRadioGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the radio group. Typically contains DropdownMenuRadioItem components. |
| `Class` | `string?` |  | Additional CSS classes to apply to the radio group. |
| `Value` | `TValue?` |  | The currently selected value. |
| `ValueChanged` | `EventCallback<TValue?>` |  | Event callback invoked when the selected value changes (for two-way binding). |
| `OnValueChange` | `EventCallback<TValue?>` |  | Event callback invoked when the selection changes. |

#### `DropdownMenuRadioItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the radio item. |
| `Class` | `string?` |  | Additional CSS classes to apply to the radio item. |
| `Disabled` | `bool` | `false` | Whether the radio item is disabled. |
| `Value` | `TValue?` |  | The value associated with this radio item. |
| `CloseOnSelect` | `bool` | `true` | Whether to close the dropdown menu when this item is selected. Default is true for radio items. |

#### `DropdownMenuSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the separator. |

#### `DropdownMenuShortcut`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the shortcut. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the shortcut (e.g., "⌘K", "Ctrl+S"). |

#### `DropdownMenuSub`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the submenu. Typically contains DropdownMenuSubTrigger and DropdownMenuSubContent. |
| `Open` | `bool?` |  | Controls whether the submenu is open (controlled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes (for two-way binding). |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the submenu open state changes. |

#### `DropdownMenuSubContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the submenu. |
| `Class` | `string?` |  | Additional CSS classes to apply to the content container. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the submenu. |
| `Offset` | `int` | `-4` | Offset distance from the trigger in pixels. |
| `ZIndex` | `int` | `50` | Z-index for the submenu content. |

#### `DropdownMenuSubTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply to the trigger. |
| `Disabled` | `bool` | `false` | Whether the trigger is disabled. |
| `Inset` | `bool` | `false` | Whether this trigger is an inset item (has additional left padding). |

#### `DropdownMenuTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger button. |
| `Class` | `string?` |  | Additional CSS classes to apply to the wrapper. |
| `AsChild` | `bool` | `false` | When true, the trigger does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to act as the trigger. |
| `Disabled` | `bool` | `false` | Whether the trigger is disabled. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Custom click handler. |
| `CustomClickHandling` | `bool` | `false` | Whether to use custom click handling only. |

**Basic Usage:**
```razor
<DropdownMenu>
    <DropdownMenuTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">
            Options
            <LucideIcon Name="chevron-down" Size="16" />
        </Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent>
        <DropdownMenuLabel>My Account</DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem>Profile</DropdownMenuItem>
        <DropdownMenuItem>Settings</DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem>Logout</DropdownMenuItem>
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

**Description:**
Empty state displays with customizable icons and messages. Shows when there's no data or content to display.

**Components & Parameters:**

#### `Empty`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the empty state. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the empty state. |

#### `EmptyActions`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `EmptyDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `EmptyIcon`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `EmptyTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

**Basic Usage:**
```razor
@* Simple empty state *@
<Empty />

@* Custom empty state *@
<Empty>
    <EmptyIcon>
        <LucideIcon Name="inbox" Size="48" />
    </EmptyIcon>
    <EmptyTitle>No messages</EmptyTitle>
    <EmptyDescription>You don't have any messages yet.</EmptyDescription>
    <EmptyAction>
        <Button>Create Message</Button>
    </EmptyAction>
</Empty>
```

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

**Description:**
Combine labels, controls, help text, and validation messages for accessible forms. Complete form field wrapper with proper ARIA attributes.

**Components & Parameters:**

#### `Field`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `FieldOrientation` | `FieldOrientation.Vertical` | Gets or sets the orientation of the field layout. Controls the layout direction and behavior: - Vertical: Stacks label above control (default, mobile-first) - Horizontal: Places label beside control with aligned items - Responsive: Automatically switches from vertical to horizontal at medium breakpoint Default value is . |
| `IsInvalid` | `bool` |  | Gets or sets whether the field is in an invalid/error state. When true, applies error styling via the data-invalid attribute. This enables conditional styling for validation errors. Default value is false. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the field. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the field. Typically contains FieldLabel, FieldContent, FieldDescription, and FieldError components for a complete form field. |

#### `FieldContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the field content container. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the field content container. Typically contains an input control, followed by optional FieldDescription and FieldError components. |

#### `FieldDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string?` |  | Gets or sets the ID for aria-describedby association. When set, this ID should be referenced by the associated input's aria-describedby attribute for proper accessibility. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the description. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the description. Typically contains helpful text explaining the purpose or format of the associated form field. |

#### `FieldError`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Errors` | `IEnumerable<string>?` |  | Gets or sets the array of error messages to display. When provided, each error is rendered as a bulleted list item. If both Errors and ChildContent are provided, Errors takes precedence. Component only renders when errors are present or ChildContent is not null. |
| `Id` | `string?` |  | Gets or sets the ID for aria-describedby association. When set, this ID should be referenced by the associated input's aria-describedby attribute for proper accessibility. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the error container. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets custom error content to be rendered. Used when you need custom error rendering beyond simple text messages. If both Errors and ChildContent are provided, Errors takes precedence. |

#### `FieldGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `FieldGroupOrientation` | `FieldGroupOrientation.Vertical` | Gets or sets the orientation of the field group layout. Controls the layout direction: - Vertical: Stacks fields vertically (default) - Horizontal: Places fields horizontally with wrap support - Responsive: Uses container queries to adapt layout based on available space Default value is . |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the field group. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the field group. Typically contains multiple Field components. |

#### `FieldLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `For` | `string?` |  | Gets or sets the ID of the form control this label is associated with. Should match the ID of the input element for proper accessibility. Enables clicking the label to focus the associated input. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the label. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the label. Typically contains text describing the associated form control. Can also include required indicators, tooltips, etc. |

#### `FieldLegend`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `FieldLegendVariant` | `FieldLegendVariant.Legend` | Gets or sets the variant of the legend element. Controls the HTML element used: - Legend: Renders a semantic legend element (use with FieldSet) - Label: Renders a div with role="group" (use with FieldGroup) Default value is . |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label when using Label variant. Provides an accessible name for the group when variant is Label. Only applicable when Variant is FieldLegendVariant.Label. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the legend. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the legend. Typically contains text describing the grouped form fields. |

#### `FieldSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the separator. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to display in the center of the separator. When provided, the content appears centered on top of the separator line with background color to create a visual break in the line. Typically used for text like "Or" or "Or continue with". |

#### `FieldSet`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the fieldset. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the fieldset. Typically contains a FieldLegend followed by multiple Field components for grouped form controls. |

#### `FieldTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the title. Custom classes are merged with the component's base classes, allowing for style overrides and extensions. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the title. Typically contains text describing the field or group of controls. |

**Basic Usage:**
```razor
<Field>
    <FieldLabel>Email</FieldLabel>
    <FieldContent>
        <Input Type="InputType.Email" @bind-Value="email" />
    </FieldContent>
    <FieldHelpText>We'll never share your email.</FieldHelpText>
</Field>

@* With validation *@
<EditForm Model="model">
    <DataAnnotationsValidator />
    <Field>
        <FieldLabel>Username</FieldLabel>
        <FieldContent>
            <Input @bind-Value="model.Username" />
        </FieldContent>
        <FieldValidationMessage For="() => model.Username" />
    </Field>
</EditForm>

@code {
    private string email = "";
    private FormModel model = new();
    
    public class FormModel 
    {
        [Required]
        public string Username { get; set; } = "";
    }
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

**Description:**
Responsive grid layout system with customizable columns and gaps. CSS Grid-based layout component.

**Components & Parameters:**

#### `Grid`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ActionHost` | `object?` |  | Gets or sets the host component that contains grid action methods. Set this to 'this' to enable auto-discovery of methods marked with [GridAction]. |
| `Items` | `IEnumerable<TItem>` | `Array.Empty<TItem>()` | Gets or sets the collection of items to display in the grid. |
| `SelectionMode` | `GridSelectionMode` | `GridSelectionMode.None` | Gets or sets the selection mode for the grid. |
| `PagingMode` | `GridPagingMode` | `GridPagingMode.None` | Gets or sets the paging mode for the grid. |
| `PageSize` | `int` | `25` | Gets or sets the number of items per page. Must be greater than 0. Default is 25. |
| `IdField` | `string` | `"Id"` | Gets or sets the property name to use as the unique row identifier. This is critical for row selection persistence across data updates and pagination. Examples: "Id" (C# convention), "ProductId", "OrderId", "id" (JavaScript convention), "_id" (MongoDB). Default is "Id". |
| `VirtualizationMode` | `GridVirtualizationMode` | `GridVirtualizationMode.Auto` | Gets or sets the virtualization mode for the grid. |
| `Theme` | `GridTheme` | `GridTheme.Shadcn` | Gets or sets the AG Grid theme to use (Shadcn, Alpine, Balham, Material, Quartz). Default is Shadcn. |
| `VisualStyle` | `GridStyle` | `GridStyle.Default` | Gets or sets the visual style modifiers for the grid (Default, Striped, Bordered, Minimal). These modifiers work with any AG Grid theme. |
| `Density` | `GridDensity` | `GridDensity.Comfortable` | Gets or sets the spacing density for the grid. |
| `SuppressHeaderMenus` | `bool` | `false` | Gets or sets whether to suppress the header menus (filter/column menu). When true, columns will not show the menu icon even if filterable/sortable. This is useful for controlled filtering scenarios where you provide external filter UI. Default is false. |
| `State` | `GridState?` |  | Gets or sets the current state of the grid. Supports two-way binding via @bind-State for automatic state synchronization. |
| `StateChanged` | `EventCallback<GridState>` |  | Gets or sets the callback invoked when the grid state changes. Used for two-way binding support (@bind-State). |
| `IsLoading` | `bool` |  | Gets or sets whether the grid is in a loading state. |
| `Columns` | `RenderFragment?` |  | Gets or sets the child content containing GridColumn definitions. |
| `LoadingTemplate` | `RenderFragment?` |  | Gets or sets the template to display while the grid is loading. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the grid container. |
| `Height` | `string?` |  | Gets or sets the height of the grid. Can be a fixed value (e.g., "500px") or a percentage (e.g., "100%"). If not specified, defaults to "300px". |
| `Width` | `string?` |  | Gets or sets the width of the grid. Can be a fixed value (e.g., "800px") or a percentage (e.g., "100%"). Defaults to "100%". |
| `InlineStyle` | `string?` |  | Gets or sets inline styles to apply to the grid container. |
| `LocalizationKeyPrefix` | `string?` |  | Gets or sets the localization key prefix for grid text resources. |
| `OnStateChanged` | `EventCallback<GridState>` |  | Gets or sets the callback invoked when the grid state changes. |
| `RowModelType` | `GridRowModelType` | `GridRowModelType.ClientSide` | Gets or sets the row model type for the grid. Default is ClientSide. Use ServerSide for server-side data fetching with sorting/filtering/pagination. |
| `OnServerDataRequest` | `Func<GridDataRequest<TItem>, Task<GridDataResponse<TItem>>>?` |  | Gets or sets the callback invoked when server-side data is requested. Required when RowModelType is ServerSide or Infinite. This callback receives a GridDataRequest and should return a GridDataResponse. |
| `OnDataRequest` | `EventCallback<GridDataRequest<TItem>>` |  | Gets or sets the callback invoked when server-side data is requested (legacy EventCallback version). For new code, use OnServerDataRequest (Func) instead. |
| `OnSelectionChanged` | `EventCallback<IReadOnlyCollection<TItem>>` |  | Gets or sets the callback invoked when the selection changes. |
| `SelectedItems` | `IReadOnlyCollection<TItem>` | `Array.Empty<TItem>()` | Gets or sets the selected items in the grid. |
| `SelectedItemsChanged` | `EventCallback<IReadOnlyCollection<TItem>>` |  | Gets or sets the callback invoked when the selected items change (for two-way binding). |

#### `GridColumn`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string?` |  | Gets or sets the unique identifier for this column. If not specified, it will be generated from Field or Header. |
| `Field` | `string?` |  | Gets or sets the field name for data binding (e.g., "CustomerName"). This is a string field name, NOT a lambda expression. |
| `Sortable` | `bool` |  | Gets or sets whether the column is sortable. |
| `Filterable` | `bool` |  | Gets or sets whether the column is filterable. |
| `Width` | `string?` |  | Gets or sets the column width (e.g., "100px", "20%"). |
| `MinWidth` | `string?` |  | Gets or sets the minimum column width. |
| `MaxWidth` | `string?` |  | Gets or sets the maximum column width. |
| `Pinned` | `GridColumnPinPosition` | `GridColumnPinPosition.None` | Gets or sets the column pinning position. |
| `AllowResize` | `bool` | `true` | Gets or sets whether the column can be resized. |
| `AllowReorder` | `bool` | `true` | Gets or sets whether the column can be reordered. |
| `IsVisible` | `bool` | `true` | Gets or sets whether the column is visible. |
| `CellTemplate` | `RenderFragment<TItem>?` |  | Gets or sets the cell template for rendering cell content. |
| `HeaderTemplate` | `RenderFragment?` |  | Gets or sets the header template for custom header rendering. |
| `FilterTemplate` | `RenderFragment?` |  | Gets or sets the filter template for custom filter UI. |
| `CellEditTemplate` | `RenderFragment<TItem>?` |  | Gets or sets the cell edit template for inline editing. |
| `ValueSelector` | `Func<TItem, object?>?` |  | Gets or sets the value selector function for extracting cell values. |
| `CellClass` | `string?` |  | Gets or sets the CSS class to apply to cells in this column. |
| `HeaderClass` | `string?` |  | Gets or sets the CSS class to apply to the column header. |
| `DataFormatString` | `string?` |  | Gets or sets the format string for displaying cell values. Supports standard .NET format strings (e.g., "C" for currency, "N2" for numbers with 2 decimals, "d" for dates). Can also use composite format strings (e.g., "{0:C}", "{0:N2}"). This is a simpler alternative to CellTemplate for basic formatting scenarios. &lt;GridColumn Field="Price" Header="Price" DataFormatString="C" /&gt;  // $1,234.56 &lt;GridColumn Field="Quantity" Header="Quantity" DataFormatString="N0" /&gt;  // 1,234 &lt;GridColumn Field="Date" Header="Date" DataFormatString="d" /&gt;  // 12/31/2024 &lt;GridColumn Field="Percentage" Header="%" DataFormatString="P2" /&gt;  // 45.67% |

#### `GridImportMap`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| *(No parameters)* | | | |

#### `GridThemeParameters`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Spacing` | `int?` |  | Gets or sets the base spacing unit in pixels. Controls padding and margins throughout the grid. Default varies by density: Compact=3, Comfortable=4, Spacious=6 4 |
| `RowHeight` | `int?` |  | Gets or sets the row height in pixels. Default varies by density: Compact=28, Comfortable=42, Spacious=56 42 |
| `HeaderHeight` | `int?` |  | Gets or sets the header height in pixels. Default varies by density: Compact=32, Comfortable=48, Spacious=64 48 |
| `IconSize` | `int?` |  | Gets or sets the size of icons in pixels. Default varies by density: Compact=14, Comfortable=16, Spacious=20 16 |
| `InputHeight` | `int?` |  | Gets or sets the height of input elements (filters, editors) in pixels. Default varies by density: Compact=28, Comfortable=32, Spacious=40 32 |
| `ToggleButtonWidth` | `int?` |  | Gets or sets the width of toggle buttons in pixels. 28 |
| `ToggleButtonHeight` | `int?` |  | Gets or sets the height of toggle buttons in pixels. 28 |
| `AccentColor` | `string?` |  | Gets or sets the primary accent color used for active states, selections, and focus indicators. Accepts CSS color values (hex, rgb, hsl, oklch, etc.) or CSS variables. Default for Shadcn theme: var(--primary) "#2563eb" or "var(--primary)" |
| `BackgroundColor` | `string?` |  | Gets or sets the background color for cells and the grid body. Default for Shadcn theme: var(--background) "#ffffff" or "var(--background)" |
| `ForegroundColor` | `string?` |  | Gets or sets the default text color for grid content. Default for Shadcn theme: var(--foreground) "#000000" or "var(--foreground)" |
| `BorderColor` | `string?` |  | Gets or sets the color of borders and dividing lines. Default for Shadcn theme: var(--border) "#e5e7eb" or "var(--border)" |
| `HeaderBackgroundColor` | `string?` |  | Gets or sets the background color for column headers. Default for Shadcn theme: var(--muted) "#f9fafb" or "var(--muted)" |
| `HeaderForegroundColor` | `string?` |  | Gets or sets the text color for column headers. Default for Shadcn theme: var(--foreground) "#000000" or "var(--foreground)" |
| `RowHoverColor` | `string?` |  | Gets or sets the background color when hovering over a row. Default for Shadcn theme: color-mix(in srgb, var(--accent) 10%, transparent) "rgba(37, 99, 235, 0.1)" or "color-mix(in srgb, var(--accent) 10%, transparent)" |
| `OddRowBackgroundColor` | `string?` |  | Gets or sets the background color for odd rows (used with Striped style). Default for Shadcn theme with Striped style: color-mix(in srgb, var(--muted) 30%, transparent) "#f9fafb" or "color-mix(in srgb, var(--muted) 30%, transparent)" |
| `SelectedRowBackgroundColor` | `string?` |  | Gets or sets the background color for selected rows. Default for Shadcn theme: color-mix(in srgb, var(--primary) 20%, transparent) "rgba(37, 99, 235, 0.2)" or "color-mix(in srgb, var(--primary) 20%, transparent)" |
| `RangeSelectionBorderColor` | `string?` |  | Gets or sets the border color for range selections. "#2563eb" |
| `CellTextColor` | `string?` |  | Gets or sets the text color for cell content. "#000000" |
| `InvalidColor` | `string?` |  | Gets or sets the color used to indicate validation errors. Default for Shadcn theme: var(--destructive) "#dc2626" or "var(--destructive)" |
| `ModalOverlayBackgroundColor` | `string?` |  | Gets or sets the background color for modal overlays. "rgba(0, 0, 0, 0.5)" |
| `ChromeBackgroundColor` | `string?` |  | Gets or sets the background color for UI chrome elements (panels, toolbars). "#f9fafb" |
| `TooltipBackgroundColor` | `string?` |  | Gets or sets the background color for tooltips. Default for Shadcn theme: var(--popover) "#ffffff" or "var(--popover)" |
| `TooltipTextColor` | `string?` |  | Gets or sets the text color for tooltips. Default for Shadcn theme: var(--popover-foreground) "#000000" or "var(--popover-foreground)" |
| `FontFamily` | `string?` |  | Gets or sets the font family for the grid. Default for Shadcn theme: var(--font-sans) "Inter, system-ui, sans-serif" or "var(--font-sans)" |
| `FontSize` | `int?` |  | Gets or sets the base font size in pixels. Default varies by density: Compact=12, Comfortable=14, Spacious=16 14 |
| `HeaderFontSize` | `int?` |  | Gets or sets the font size for column headers in pixels. 14 |
| `HeaderFontWeight` | `object?` |  | Gets or sets the font weight for column headers. 600 or "bold" |
| `Borders` | `bool?` |  | Gets or sets whether to show borders between cells and rows. Default varies by style: Default=true, Striped=true, Bordered=true, Minimal=false true |
| `BorderRadius` | `int?` |  | Gets or sets the border radius in pixels for UI elements. Default for Shadcn theme: 4 4 |
| `WrapperBorder` | `bool?` |  | Gets or sets whether to show a border around the entire grid wrapper. Default varies by style: Default=false, Striped=false, Bordered=true, Minimal=false false |
| `WrapperBorderRadius` | `int?` |  | Gets or sets the border radius in pixels for the grid wrapper. 8 |

**Basic Usage:**
```razor
@* 2-column grid *@
<Grid Columns="2" Gap="4">
    <div>Column 1</div>
    <div>Column 2</div>
</Grid>

@* Responsive grid *@
<Grid Columns="1" Sm="2" Md="3" Lg="4" Gap="4">
    <Card>Item 1</Card>
    <Card>Item 2</Card>
    <Card>Item 3</Card>
    <Card>Item 4</Card>
</Grid>
```

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

**Description:**
Smooth height transition animation component. Animates content height changes for expand/collapse effects.

**Components & Parameters:**

#### `HeightAnimation`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to be rendered inside the animated container. |
| `Config` | `HeightAnimationConfig?` |  | Configuration for the height animation behavior. |
| `Class` | `string?` |  | Additional CSS classes to apply to the container element. |
| `Style` | `string?` |  | Additional inline styles to apply to the container element. |
| `Enabled` | `bool` | `true` | Whether the animation is currently enabled. When false, no animation setup will occur. |

**Basic Usage:**
```razor
<HeightAnimation>
    @if (isExpanded)
    {
        <div>
            <p>This content animates when shown/hidden.</p>
            <p>Height transitions smoothly.</p>
        </div>
    }
</HeightAnimation>

<Button OnClick="() => isExpanded = !isExpanded">
    Toggle
</Button>

@code {
    private bool isExpanded = false;
}
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

**Description:**
Rich hover previews with delay support. Displays detailed information on hover with smooth transitions.

**Components & Parameters:**

#### `HoverCard`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the hover card. Should include HoverCardTrigger and HoverCardContent components. |
| `Open` | `bool?` |  | Controls whether the hover card is open (controlled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. |
| `DefaultOpen` | `bool` | `false` | Default open state when in uncontrolled mode. |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the hover card open state changes. |
| `OpenDelay` | `int` | `700` | Delay in milliseconds before opening the hover card. Default is 700ms. |
| `CloseDelay` | `int` | `300` | Delay in milliseconds before closing the hover card. Default is 300ms. |

#### `HoverCardContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render inside the hover card. |
| `Side` | `PopoverSide` | `PopoverSide.Bottom` | Preferred side for positioning. |
| `Align` | `PopoverAlign` | `PopoverAlign.Center` | Alignment relative to trigger. |
| `Offset` | `int` | `4` | Offset distance from trigger in pixels. |
| `Class` | `string?` |  | Additional CSS classes to apply to the content. |

#### `HoverCardTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the trigger. |
| `AsChild` | `bool` | `false` | When true, the trigger does not render its own div element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component to act as the trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply to the trigger. |

**Basic Usage:**
```razor
<HoverCard>
    <HoverCardTrigger AsChild>
        <Button Variant="ButtonVariant.Link">johndoe</Button>
    </HoverCardTrigger>
    <HoverCardContent>
        <div class="space-y-2">
            <h4 class="font-semibold">John Doe</h4>
            <p class="text-sm">Full stack developer</p>
            <div class="flex gap-2 text-sm text-muted-foreground">
                <span>Joined March 2024</span>
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

**Description:**
Text input with multiple types (text, email, password, number, etc.) and validation support. Full-featured form input component.

**Components & Parameters:**

#### `Input`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Type` | `InputType` | `InputType.Text` | Gets or sets the type of input. Determines the HTML input type attribute. Default value is . |
| `Value` | `string?` |  | Gets or sets the current value of the input. Supports two-way binding via @bind-Value syntax. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback invoked when the input value changes. This event is fired on every keystroke (oninput event). Use with Value parameter for two-way binding. |
| `Placeholder` | `string?` |  | Gets or sets the placeholder text displayed when the input is empty. Provides a hint to the user about what to enter. Should not be used as a replacement for a label. |
| `Disabled` | `bool` |  | Gets or sets whether the input is disabled. When disabled: - Input cannot be focused or edited - Cursor is set to not-allowed - Opacity is reduced for visual feedback |
| `Required` | `bool` |  | Gets or sets whether the input is required. When true, the HTML5 required attribute is set. Works with form validation and :invalid CSS pseudo-class. |
| `Name` | `string?` |  | Gets or sets the name of the input for form submission. This is critical for form submission. The name/value pair is submitted to the server. Should be unique within the form. |
| `Autocomplete` | `string?` |  | Gets or sets the autocomplete hint for the browser. Examples: "email", "username", "current-password", "new-password", "name", "tel", "off". Helps browsers provide appropriate autofill suggestions. |
| `Readonly` | `bool` |  | Gets or sets whether the input is read-only. When true, the user cannot modify the value, but it's still focusable and submitted with forms. Different from Disabled - readonly inputs are still submitted with forms. |
| `MaxLength` | `int?` |  | Gets or sets the maximum number of characters allowed. When set, the browser will prevent users from entering more characters. Applies to text, email, password, tel, url, and search types. |
| `MinLength` | `int?` |  | Gets or sets the minimum number of characters required. Works with form validation. Applies to text, email, password, tel, url, and search types. |
| `Min` | `string?` |  | Gets or sets the minimum value for number, date, or time inputs. Applies to number, date, time inputs. Works with form validation and :invalid pseudo-class. |
| `Max` | `string?` |  | Gets or sets the maximum value for number, date, or time inputs. Applies to number, date, time inputs. Works with form validation and :invalid pseudo-class. |
| `Step` | `string?` |  | Gets or sets the step interval for number inputs. Defines the granularity of values (e.g., "0.01" for currency, "1" for integers). Applies to number, date, time inputs. |
| `Pattern` | `string?` |  | Gets or sets the regex pattern for validation. Validates input against the specified regular expression. Works with form validation and :invalid pseudo-class. |
| `InputMode` | `string?` |  | Gets or sets the input mode hint for mobile keyboards. Examples: "none", "text", "decimal", "numeric", "tel", "search", "email", "url". Helps mobile devices show the appropriate keyboard. |
| `Autofocus` | `bool` |  | Gets or sets whether the input should be auto-focused when the page loads. Only one element per page should have autofocus. Improves accessibility when used appropriately. |
| `Spellcheck` | `bool?` |  | Gets or sets whether spell checking is enabled. Can be true, false, or null (browser default). Useful for controlling spell checking on email addresses, usernames, etc. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the input. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `Id` | `string?` |  | Gets or sets the HTML id attribute for the input element. Used to associate the input with a label element via the label's 'for' attribute. This is essential for accessibility and allows clicking the label to focus the input. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the input. Provides an accessible name for screen readers. Use when there is no visible label element. |
| `AriaDescribedBy` | `string?` |  | Gets or sets the ID of the element that describes the input. References the id of an element containing help text or error messages. Improves screen reader experience by associating descriptive text. |
| `AriaInvalid` | `bool?` |  | Gets or sets whether the input value is invalid. When true, aria-invalid="true" is set. Should be set based on validation state. |

**Basic Usage:**
```razor
@* Text input *@
<Input @bind-Value="name" Placeholder="Enter name" />

@* Email input *@
<Input Type="InputType.Email" @bind-Value="email" />

@* Password input *@
<Input Type="InputType.Password" @bind-Value="password" />

@* Disabled input *@
<Input Value="Disabled" Disabled="true" />

@code {
    private string name = "";
    private string email = "";
    private string password = "";
}
```

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

**Description:**
Enhanced inputs with icons, buttons, and addons. Combines input with prefix/suffix elements.

**Components & Parameters:**

#### `InputGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content to be rendered inside the input group. Typically contains InputGroupInput/InputGroupTextarea and InputGroupAddon components. The order of child components affects the visual layout and keyboard navigation. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the input group container. Custom classes are merged with the component's base classes using TailwindMerge, allowing for intelligent conflict resolution. |

#### `InputGroupAddon`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Align` | `InputGroupAlign` | `InputGroupAlign.InlineStart` | Gets or sets the alignment position of the addon. Determines where the addon content appears relative to the input: - InlineStart: Left side (or right in RTL) - InlineEnd: Right side (or left in RTL) - BlockStart: Above the input - BlockEnd: Below the input |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content to be rendered inside the addon. Can contain icons, buttons, text, or any other content. The component automatically adjusts padding based on content type. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the addon container. |

#### `InputGroupButton`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Type` | `ButtonType` | `ButtonType.Button` | Gets or sets the button type (submit, button, reset). |
| `Variant` | `ButtonVariant` | `ButtonVariant.Default` | Gets or sets the button visual variant. |
| `Size` | `ButtonSize` | `ButtonSize.Small` | Gets or sets the button size. Default is Small for better proportions within input groups. |
| `Disabled` | `bool` |  | Gets or sets whether the button is disabled. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for accessibility. |
| `OnClick` | `EventCallback` |  | Gets or sets the click event handler. |
| `Icon` | `RenderFragment?` |  | Gets or sets the icon to display in the button. |
| `IconPosition` | `IconPosition` | `IconPosition.Start` | Gets or sets the position of the icon relative to button text. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content (button text). |
| `Class` | `string?` |  | Gets or sets additional CSS classes. |

#### `InputGroupInput`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Type` | `InputType` | `InputType.Text` | Gets or sets the type of input. |
| `Value` | `string?` |  | Gets or sets the current value of the input. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback invoked when the input value changes. |
| `Placeholder` | `string?` |  | Gets or sets the placeholder text. |
| `Disabled` | `bool` |  | Gets or sets whether the input is disabled. |
| `Required` | `bool` |  | Gets or sets whether the input is required. |
| `Class` | `string?` |  | Gets or sets additional CSS classes. |
| `Id` | `string?` |  | Gets or sets the HTML id attribute. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label. |
| `AriaDescribedBy` | `string?` |  | Gets or sets the ARIA described-by attribute. |
| `AriaInvalid` | `bool?` |  | Gets or sets whether the input value is invalid. |

#### `InputGroupText`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content (text or icons). |
| `Class` | `string?` |  | Gets or sets additional CSS classes. |

#### `InputGroupTextarea`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string?` |  | Gets or sets the current value of the textarea. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback invoked when the textarea value changes. |
| `Rows` | `int` | `3` | Gets or sets the number of visible text rows. Default is 3 rows. The textarea can grow beyond this if resize is enabled. |
| `Placeholder` | `string?` |  | Gets or sets the placeholder text. |
| `Disabled` | `bool` |  | Gets or sets whether the textarea is disabled. |
| `Required` | `bool` |  | Gets or sets whether the textarea is required. |
| `Class` | `string?` |  | Gets or sets additional CSS classes. |
| `Id` | `string?` |  | Gets or sets the HTML id attribute. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label. |
| `AriaDescribedBy` | `string?` |  | Gets or sets the ARIA described-by attribute. |
| `AriaInvalid` | `bool?` |  | Gets or sets whether the textarea value is invalid. |

**Basic Usage:**
```razor
@* With prefix icon *@
<InputGroup>
    <InputGroupPrefix>
        <LucideIcon Name="search" Size="16" />
    </InputGroupPrefix>
    <Input @bind-Value="searchTerm" Placeholder="Search..." />
</InputGroup>

@* With suffix button *@
<InputGroup>
    <Input @bind-Value="email" Type="InputType.Email" />
    <InputGroupSuffix>
        <Button Size="ButtonSize.Small">Subscribe</Button>
    </InputGroupSuffix>
</InputGroup>

@code {
    private string searchTerm = "";
    private string email = "";
}
```

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

**Description:**
One-time password input with individual character slots. Specialized input for OTP/PIN entry with auto-focus.

**Components & Parameters:**

#### `InputOtp`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the OTP input. Typically includes InputOtpSlot components or InputOtpGroup with slots. |
| `Class` | `string?` |  | Additional CSS classes to apply to the container. |
| `Length` | `int` | `6` | The number of OTP slots. Default is 6. |
| `Value` | `string?` |  | Controls the OTP value (controlled mode). |
| `ValueChanged` | `EventCallback<string>` |  | Event callback invoked when the OTP value changes. Use with @bind-Value for two-way binding. |
| `DefaultValue` | `string` | `""` | Default value when in uncontrolled mode. |
| `OnValueChange` | `EventCallback<string>` |  | Event callback invoked when the OTP value changes. |
| `OnComplete` | `EventCallback<string>` |  | Event callback invoked when the OTP is complete. |
| `Pattern` | `string` | `"[0-9]"` | Pattern for input validation (e.g., "[0-9]" for digits only). Default is digits only. |
| `Disabled` | `bool` | `false` | Whether the OTP input is disabled. |
| `AriaLabel` | `string` | `"One-time password"` | ARIA label for the OTP input group. |
| `AriaInvalid` | `bool` | `false` | Whether the OTP input is in an invalid/error state. When true, aria-invalid="true" is set on the slots, applying error styling. |

#### `InputOtpGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content, typically InputOtpSlot components. |
| `Class` | `string?` |  | Additional CSS classes to apply to the group. |

#### `InputOtpSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Optional custom content for the separator. If not provided, displays a minus/dash icon. |
| `Class` | `string?` |  | Additional CSS classes to apply to the separator. |

#### `InputOtpSlot`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Index` | `int` |  | The index of this slot (0-based). |
| `Class` | `string?` |  | Additional CSS classes to apply to the slot. |

**Basic Usage:**
```razor
<InputOtp @bind-Value="otpCode" Length="6" />

@* With custom pattern *@
<InputOtp @bind-Value="otpCode" 
          Length="6" 
          Pattern="[0-9]*" />

@code {
    private string otpCode = "";
}
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

**Description:**
Flexible list items with media, content, and action slots. Reusable component for building lists and menus.

**Components & Parameters:**

#### `Item`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `ItemVariant` | `ItemVariant.Default` | Gets or sets the visual style variant of the item. |
| `Size` | `ItemSize` | `ItemSize.Default` | Gets or sets the size of the item. |
| `AsChild` | `string?` |  | Gets or sets the element type to render as (e.g., "a", "button"). When set, the component renders as that element instead of a div. |
| `Href` | `string?` |  | Gets or sets the href attribute when rendering as an anchor. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the item. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the item. |
| `DataSlot` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |
| `DataSlot` | `string?` |  |  |
| `href` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |
| `DataSlot` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `ItemActions`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the actions container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the actions container. |

#### `ItemContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the content container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the container. |

#### `ItemDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the description. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered as the description. |

#### `ItemFooter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the footer. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered in the footer. |

#### `ItemGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the group. |

#### `ItemHeader`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the header. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered in the header. |

#### `ItemMedia`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `ItemMediaVariant` | `ItemMediaVariant.Default` | Gets or sets the visual style variant of the media. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the media container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the media container. |

#### `ItemSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the separator. |

#### `ItemTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the title. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered as the title. |

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
            <LucideIcon Name="more-vertical" Size="16" />
        </Button>
    </ItemAction>
</Item>
```

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

**Description:**
Keyboard shortcut badges for displaying key combinations. Shows keyboard shortcuts in styled format.

**Components & Parameters:**

#### `Kbd`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the kbd element. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the kbd element. Typically contains a single key name (e.g., "Ctrl", "Shift", "Enter") or a key symbol (e.g., "⌘", "⌥", "⇧"). |

**Basic Usage:**
```razor
@* Single key *@
<Kbd>Ctrl</Kbd>

@* Key combination *@
<div class="flex gap-1">
    <Kbd>Ctrl</Kbd>
    <span>+</span>
    <Kbd>C</Kbd>
</div>

@* In text *@
<p>Press <Kbd>Enter</Kbd> to submit</p>
```

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

**Description:**
Accessible form labels with proper for/id association. Connects labels to form controls for accessibility.

**Components & Parameters:**

#### `Label`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `For` | `string?` |  | Gets or sets the ID of the form element this label is associated with. A string containing the ID of the target form control, or null. This parameter maps to the HTML for attribute (htmlFor in JSX). When set, clicking the label will focus or activate the associated form control. Best practices: Always provide a For value for explicit label-control association Ensure the For value matches the Id of the target form control Use meaningful IDs that describe the field's purpose |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the label element. A string containing one or more CSS class names, or null. Use this parameter to customize the label's appearance beyond default styling. Common Tailwind utilities include: Text size: text-lg, text-sm Font weight: font-bold, font-normal Color: text-muted-foreground, text-destructive Spacing: mb-2, mr-2 |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the label element. A  containing the label's content, or null. Typically contains the label text, but can include additional elements such as: Required field indicators (asterisks, badges) Help text or tooltips Icons or visual indicators Nested spans for styling portions of text |

**Basic Usage:**
```razor
<Label For="email">Email Address</Label>
<Input Id="email" Type="InputType.Email" @bind-Value="email" />

@code {
    private string email = "";
}
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

**Description:**
Rich text editor with toolbar formatting and live preview. Full-featured markdown editing with syntax highlighting.

**Components & Parameters:**

#### `MarkdownEditor`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string?` |  | Gets or sets the markdown content value. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback invoked when the value changes. |
| `Placeholder` | `string?` |  | Gets or sets the placeholder text displayed when the editor is empty. |
| `Disabled` | `bool` |  | Gets or sets whether the editor is disabled. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the editor container. |
| `Id` | `string?` |  | Gets or sets the HTML id attribute for the textarea element. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the textarea. |
| `AriaDescribedBy` | `string?` |  | Gets or sets the ID of the element that describes the textarea. |
| `AriaInvalid` | `bool?` |  | Gets or sets whether the textarea value is invalid. |

**Basic Usage:**
```razor
<MarkdownEditor @bind-Value="markdownContent" 
                Placeholder="Write your markdown here..." />

@* With preview *@
<MarkdownEditor @bind-Value="markdownContent" 
                ShowPreview="true" />

@code {
    private string markdownContent = "# Hello World";
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

**Description:**
Desktop application-style horizontal menu bar with dropdown menus. Top-level navigation with nested submenus.

**Components & Parameters:**

#### `Menubar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the menubar. Typically includes MenubarMenu components. |
| `Class` | `string?` |  | Additional CSS classes to apply to the menubar. |
| `ActiveIndex` | `int?` |  | Controls which menu is active/open (controlled mode). When null, the menubar manages its own state. |
| `ActiveIndexChanged` | `EventCallback<int>` |  | Event callback invoked when the active index changes. Use with @bind-ActiveIndex for two-way binding. |
| `DefaultActiveIndex` | `int` | `-1` | Default active index when in uncontrolled mode. |
| `OnActiveIndexChange` | `EventCallback<int>` |  | Event callback invoked when the active menu changes. |
| `Loop` | `bool` | `true` | Whether keyboard loop navigation is enabled. Default is true. |

#### `MenubarCheckboxItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the checkbox item. |
| `Class` | `string?` |  | Additional CSS classes to apply to the checkbox item. |
| `Disabled` | `bool` | `false` | Whether the checkbox item is disabled. |
| `Checked` | `bool` | `false` | Whether the checkbox is checked. |
| `CheckedChanged` | `EventCallback<bool>` |  | Event callback invoked when the checked state changes (for two-way binding). |
| `OnCheckedChange` | `EventCallback<bool>` |  | Event callback invoked when the checkbox state changes. |
| `CloseOnSelect` | `bool` | `false` | Whether to close the menu when this item is selected. Default is false for checkbox items. |

#### `MenubarContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the menu. |
| `Class` | `string?` |  | Additional CSS classes to apply to the content container. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the menu. |
| `CloseOnClickOutside` | `bool` | `true` | Whether clicking outside should close the menu. |
| `Side` | `PopoverSide` | `PopoverSide.Bottom` | Preferred side for positioning. |
| `Align` | `PopoverAlign` | `PopoverAlign.Start` | Alignment relative to trigger. |
| `Offset` | `int` | `4` | Offset distance from the trigger in pixels. |
| `Loop` | `bool` | `true` | Whether to enable keyboard loop navigation. |
| `ZIndex` | `int` | `50` | Z-index for the menu content. |

#### `MenubarGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the group. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the group. |

#### `MenubarItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the menu item. |
| `Class` | `string?` |  | Additional CSS classes to apply to the menu item. |
| `Disabled` | `bool` | `false` | Whether the menu item is disabled. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Custom click handler. |
| `CloseOnSelect` | `bool` | `true` | Whether to close the menu when this item is selected. |
| `Inset` | `bool` | `false` | Whether this menu item represents an inset item (has additional left padding). |

#### `MenubarLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the label. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the label. |
| `Inset` | `bool` | `false` | Whether to show the label with inset padding to align with items that have icons. |

#### `MenubarMenu`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content, typically MenubarTrigger and MenubarContent. |

#### `MenubarRadioGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the radio group. Typically contains MenubarRadioItem components. |
| `Class` | `string?` |  | Additional CSS classes to apply to the radio group. |
| `Value` | `TValue?` |  | The currently selected value. |
| `ValueChanged` | `EventCallback<TValue?>` |  | Event callback invoked when the selected value changes (for two-way binding). |
| `OnValueChange` | `EventCallback<TValue?>` |  | Event callback invoked when the selection changes. |

#### `MenubarRadioItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the radio item. |
| `Class` | `string?` |  | Additional CSS classes to apply to the radio item. |
| `Disabled` | `bool` | `false` | Whether the radio item is disabled. |
| `Value` | `TValue?` |  | The value associated with this radio item. |
| `CloseOnSelect` | `bool` | `true` | Whether to close the menu when this item is selected. Default is true for radio items. |

#### `MenubarSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Additional CSS classes to apply to the separator. |

#### `MenubarShortcut`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Additional CSS classes to apply to the shortcut. |
| `ChildContent` | `RenderFragment?` |  | The content to be rendered inside the shortcut (e.g., "⌘K", "Ctrl+S"). |

#### `MenubarSub`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the submenu. Typically contains MenubarSubTrigger and MenubarSubContent. |
| `Open` | `bool?` |  | Controls whether the submenu is open (controlled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes (for two-way binding). |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the submenu open state changes. |

#### `MenubarSubContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the submenu. |
| `Class` | `string?` |  | Additional CSS classes to apply to the content container. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the submenu. |
| `Offset` | `int` | `-4` | Offset distance from the trigger in pixels. |
| `ZIndex` | `int` | `50` | Z-index for the submenu content. |

#### `MenubarSubTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply to the trigger. |
| `Disabled` | `bool` | `false` | Whether the trigger is disabled. |
| `Inset` | `bool` | `false` | Whether this trigger is an inset item (has additional left padding). |

#### `MenubarTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger button. |
| `Class` | `string?` |  | Additional CSS classes to apply to the trigger. |
| `Disabled` | `bool` | `false` | Whether the trigger is disabled. |

**Basic Usage:**
```razor
<Menubar>
    <MenubarMenu>
        <MenubarTrigger>File</MenubarTrigger>
        <MenubarContent>
            <MenubarItem>New File</MenubarItem>
            <MenubarItem>Open</MenubarItem>
            <MenubarSeparator />
            <MenubarItem>Save</MenubarItem>
        </MenubarContent>
    </MenubarMenu>
    
    <MenubarMenu>
        <MenubarTrigger>Edit</MenubarTrigger>
        <MenubarContent>
            <MenubarItem>Cut</MenubarItem>
            <MenubarItem>Copy</MenubarItem>
            <MenubarItem>Paste</MenubarItem>
        </MenubarContent>
    </MenubarMenu>
</Menubar>
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

**Description:**
Declarative animation system powered by Motion.dev with 20+ presets including fade, scale, slide, shake, bounce, pulse, spring physics, scroll-triggered animations, and staggered list/grid animations.

**Components & Parameters:**

#### `BounceOnce`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Height` | `string` | `"-30px"` | Bounce height (pixels or percentage). Default: "-30px" |
| `Duration` | `double` | `0.6` | Duration in seconds. Default: 0.6 |

#### `ExpandOnScroll`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `0.8` | Starting scale (0-1). Default: 0.8 |
| `To` | `double` | `1` | Ending scale (0-1+). Default: 1 |
| `Duration` | `double` | `0.6` | Duration in seconds. Default: 0.6 |

#### `FadeIn`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `0` | Starting opacity (0-1). Default: 0 |
| `To` | `double` | `1` | Ending opacity (0-1). Default: 1 |
| `Duration` | `double` | `0.3` | Duration in seconds. Default: 0.3 |

#### `FadeInOnScroll`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `0` | Starting opacity (0-1). Default: 0 |
| `To` | `double` | `1` | Ending opacity (0-1). Default: 1 |
| `Duration` | `double` | `0.6` | Duration in seconds. Default: 0.6 |

#### `FadeOut`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `1` | Starting opacity (0-1). Default: 1 |
| `To` | `double` | `0` | Ending opacity (0-1). Default: 0 |
| `Duration` | `double` | `0.2` | Duration in seconds. Default: 0.2 |

#### `GridItemEnter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Duration` | `double` | `0.4` | Duration in seconds. Default: 0.4 |

#### `ListItemEnter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Duration` | `double` | `0.3` | Duration in seconds. Default: 0.3 |

#### `ListItemExit`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Duration` | `double` | `0.2` | Duration in seconds. Default: 0.2 |

#### `Motion`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render and animate. |
| `Presets` | `RenderFragment?` |  | Preset animations to apply (FadeIn, ScaleIn, Spring, etc.). |
| `Trigger` | `MotionTrigger` | `MotionTrigger.OnAppear` | When the animation should trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply. |
| `Style` | `string?` |  | Additional inline styles to apply. |
| `InViewOptions` | `InViewOptions?` |  | Options for IntersectionObserver when Trigger is OnInView. |
| `StaggerChildren` | `double?` |  | Stagger delay for child animations in seconds. |
| `RespectReducedMotion` | `bool` | `true` | Whether to respect user's reduced motion preference. Default: true |
| `Keyframes` | `List<MotionKeyframe>?` |  | Custom keyframes to animate (alternative to using presets). |
| `Options` | `MotionOptions?` |  | Animation options (duration, delay, easing, etc.). |
| `Spring` | `SpringOptions?` |  | Spring physics options (overrides standard easing). |

#### `Presets`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Preset components to apply (FadeIn, ScaleIn, Spring, etc.). |

#### `Pulse`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Scale` | `double` | `1.05` | Scale amplitude. Default: 1.05 |
| `Duration` | `double` | `0.6` | Duration in seconds. Default: 0.6 |
| `Repeat` | `double` | `-1` | Number of pulses. Default: infinite |

#### `ScaleIn`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `0.8` | Starting scale (0-1+). Default: 0.8 |
| `To` | `double` | `1` | Ending scale (0-1+). Default: 1 |
| `Duration` | `double` | `0.3` | Duration in seconds. Default: 0.3 |

#### `ScaleOut`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `1` | Starting scale (0-1+). Default: 1 |
| `To` | `double` | `0.8` | Ending scale (0-1+). Default: 0.8 |
| `Duration` | `double` | `0.2` | Duration in seconds. Default: 0.2 |

#### `ShakeX`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Intensity` | `int` | `10` | Shake intensity (pixels). Default: 10 |
| `Duration` | `double` | `0.4` | Duration in seconds. Default: 0.4 |

#### `ShakeY`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Intensity` | `int` | `10` | Shake intensity (pixels). Default: 10 |
| `Duration` | `double` | `0.4` | Duration in seconds. Default: 0.4 |

#### `SlideInFromBottom`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `100` | Distance to slide from in pixels. Default: 100 |
| `Duration` | `double` | `0.3` | Duration in seconds. Default: 0.3 |

#### `SlideInFromLeft`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `-100` | Distance to slide from in pixels (negative value). Default: -100 |
| `Duration` | `double` | `0.3` | Duration in seconds. Default: 0.3 |

#### `SlideInFromRight`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `100` | Distance to slide from in pixels. Default: 100 |
| `Duration` | `double` | `0.3` | Duration in seconds. Default: 0.3 |

#### `SlideInFromTop`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `-100` | Distance to slide from in pixels (negative value). Default: -100 |
| `Duration` | `double` | `0.3` | Duration in seconds. Default: 0.3 |

#### `SlideInOnScroll`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `From` | `double` | `50` | Starting Y position in pixels. Default: 50 |
| `Duration` | `double` | `0.6` | Duration in seconds. Default: 0.6 |

#### `Spring`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Mass` | `double` | `1.0` | Mass of the spring. Higher = slower. Default: 1.0 |
| `Stiffness` | `double` | `100` | Stiffness of the spring. Higher = snappier. Default: 100 |
| `Damping` | `double` | `10` | Damping ratio. Higher = less oscillation. Default: 10 |
| `Velocity` | `double` | `0` | Initial velocity. Default: 0 |
| `Bounce` | `double?` |  | Bounce amount (0-1). Alternative to stiffness/damping. |

**Basic Usage:**
```razor
@* Fade in animation *@
<Motion Preset="MotionPreset.Fade">
    <div>This content fades in</div>
</Motion>

@* Slide up animation *@
<Motion Preset="MotionPreset.SlideUp" Duration="0.5">
    <Card>Slides up on mount</Card>
</Motion>

@* Stagger children *@
<Motion Preset="MotionPreset.StaggerList">
    <div>Item 1</div>
    <div>Item 2</div>
    <div>Item 3</div>
</Motion>
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

**Description:**
Searchable multi-selection with tags and checkboxes. Select multiple options from a dropdown list.

**Components & Parameters:**

#### `MultiSelect`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Values` | `IEnumerable<string>?` |  | Gets or sets the currently selected values. |
| `ValuesChanged` | `EventCallback<IEnumerable<string>?>` |  | Gets or sets the callback that is invoked when the selected values change. |
| `Placeholder` | `string` | `"Select items..."` | Gets or sets the placeholder text shown when no items are selected. |
| `SearchPlaceholder` | `string` | `"Search..."` | Gets or sets the placeholder text shown in the search input. |
| `EmptyMessage` | `string` | `"No results found."` | Gets or sets the message displayed when no items match the search. |
| `SelectAllLabel` | `string` | `"Select All"` | Gets or sets the label for the Select All option. |
| `ShowSelectAll` | `bool` | `true` | Gets or sets whether to show the Select All option. |
| `ClearLabel` | `string` | `"Clear"` | Gets or sets the label for the Clear button. |
| `CloseLabel` | `string` | `"Close"` | Gets or sets the label for the Close button. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the multiselect container. |
| `Disabled` | `bool` |  | Gets or sets whether the multiselect is disabled. |
| `MaxDisplayTags` | `int` | `3` | Gets or sets the maximum number of tags to display before showing "+N more". |
| `PopoverWidth` | `string` | `"w-[300px]"` | Gets or sets the width of the popover content. |
| `ValuesExpression` | `Expression<Func<IEnumerable<string>?>>?` |  | Gets or sets an expression that identifies the bound values. Used for form validation integration. |

**Basic Usage:**
```razor
<MultiSelect @bind-Values="selectedItems" 
             Placeholder="Select items...">
    <MultiSelectItem Value="option1">Option 1</MultiSelectItem>
    <MultiSelectItem Value="option2">Option 2</MultiSelectItem>
    <MultiSelectItem Value="option3">Option 3</MultiSelectItem>
</MultiSelect>

@code {
    private HashSet<string> selectedItems = new();
}
```

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

**Description:**
Styled native HTML select dropdown. Enhanced styling for standard select element.

**Components & Parameters:**

#### `NativeSelect`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string?` |  | Gets or sets the selected value. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback that is invoked when the value changes. |
| `Id` | `string?` |  | Gets or sets the id attribute for the select element. |
| `Name` | `string?` |  | Gets or sets the name attribute for the select element. |
| `Placeholder` | `string?` |  | Gets or sets the placeholder text when no option is selected. |
| `Disabled` | `bool` |  | Gets or sets whether the select is disabled. |
| `Required` | `bool` |  | Gets or sets whether the select is required. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the select. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the select (options). |

#### `NativeSelectOption`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string?` |  |  |
| `Disabled` | `bool` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

**Basic Usage:**
```razor
<NativeSelect @bind-Value="selectedValue">
    <option value="">Select an option</option>
    <option value="option1">Option 1</option>
    <option value="option2">Option 2</option>
    <option value="option3">Option 3</option>
</NativeSelect>

@code {
    private string selectedValue = "";
}
```

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

**Description:**
Horizontal navigation with dropdown panels. Main site navigation with mega menu support.

**Components & Parameters:**

#### `NavigationMenu`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the navigation menu. |
| `Value` | `string?` |  | Controls which item is active/open. |
| `ValueChanged` | `EventCallback<string>` |  | Event callback invoked when the active item changes. |
| `Orientation` | `NavigationMenuOrientation` | `NavigationMenuOrientation.Horizontal` | The orientation of the navigation menu. |
| `AriaLabel` | `string` | `"Main"` | The accessible label for the navigation. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `NavigationMenuContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the content panel. |
| `ForceMount` | `bool` | `true` | When true, the content is always mounted in the DOM (for animations). Default is true for proper animation support. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `NavigationMenuIndicator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `NavigationMenuItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | `string.Empty` | The unique value for this item. |
| `ChildContent` | `RenderFragment?` |  | The child content to render within the item. |

#### `NavigationMenuLink`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Href` | `string` | `"#"` | The href for the link. |
| `Active` | `bool?` |  | Whether the link is currently active. If not set, active state will be auto-detected based on the current URL when AutoActive is true. |
| `AutoActive` | `bool` | `false` | Whether to automatically detect active state based on the current URL. Similar to NavLink behavior. Default is false. |
| `Match` | `NavLinkMatch` | `NavLinkMatch.Prefix` | How to match the URL when AutoActive is true. NavLinkMatch.All requires an exact match, NavLinkMatch.Prefix requires the URL to start with Href. Default is NavLinkMatch.Prefix. |
| `ChildContent` | `RenderFragment?` |  | The child content to render within the link. |
| `OnClick` | `EventCallback` |  | Event callback invoked when the link is clicked. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `NavigationMenuList`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the list. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `NavigationMenuTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `NavigationMenuViewport`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the viewport. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

**Basic Usage:**
```razor
<NavigationMenu>
    <NavigationMenuList>
        <NavigationMenuItem>
            <NavigationMenuTrigger>Products</NavigationMenuTrigger>
            <NavigationMenuContent>
                <NavigationMenuLink Href="/products/all">All Products</NavigationMenuLink>
                <NavigationMenuLink Href="/products/new">New Arrivals</NavigationMenuLink>
            </NavigationMenuContent>
        </NavigationMenuItem>
        
        <NavigationMenuItem>
            <NavigationMenuLink Href="/about">About</NavigationMenuLink>
        </NavigationMenuItem>
    </NavigationMenuList>
</NavigationMenu>
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

**Description:**
Page navigation with Previous/Next/Ellipsis support. Navigate through paginated content.

**Components & Parameters:**

#### `Pagination`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `AriaLabel` | `string` | `"pagination"` | Gets or sets the aria-label for the pagination navigation. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the pagination. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the pagination. |

#### `PaginationContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `PaginationEllipsis`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |

#### `PaginationItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `PaginationLink`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Href` | `string?` |  |  |
| `IsActive` | `bool` |  |  |
| `Class` | `string?` |  |  |
| `ChildContent` | `RenderFragment?` |  |  |

#### `PaginationNext`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Href` | `string?` |  |  |
| `Class` | `string?` |  |  |

#### `PaginationPrevious`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Href` | `string?` |  |  |
| `Class` | `string?` |  |  |

**Basic Usage:**
```razor
<Pagination CurrentPage="currentPage" 
            TotalPages="10" 
            OnPageChanged="HandlePageChange">
    <PaginationContent>
        <PaginationPrevious />
        <PaginationItem PageNumber="1" />
        <PaginationEllipsis />
        <PaginationItem PageNumber="currentPage" />
        <PaginationEllipsis />
        <PaginationItem PageNumber="10" />
        <PaginationNext />
    </PaginationContent>
</Pagination>

@code {
    private int currentPage = 1;
    
    private void HandlePageChange(int page)
    {
        currentPage = page;
    }
}
```

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

**Description:**
Floating content containers triggered by user interaction. Displays rich content in an overlay.

**Components & Parameters:**

#### `Popover`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the popover. Should include PopoverTrigger and PopoverContent. |
| `Open` | `bool?` |  | Controls whether the popover is open (controlled mode). When null, the popover manages its own state (uncontrolled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. Use with @bind-Open for two-way binding. |
| `DefaultOpen` | `bool` | `false` | Default open state when in uncontrolled mode. |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the popover open state changes. |
| `Modal` | `bool` | `true` | Whether the popover can be dismissed by clicking outside or pressing Escape. Default is true. |

#### `PopoverContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  |  |
| `CloseOnEscape` | `bool` | `true` |  |
| `CloseOnClickOutside` | `bool` | `true` |  |
| `Side` | `PopoverSide` | `PopoverSide.Bottom` |  |
| `Align` | `PopoverAlign` | `PopoverAlign.Center` |  |
| `Offset` | `int` | `4` |  |
| `OnEscapeKeyDown` | `EventCallback<KeyboardEventArgs>` |  |  |
| `OnClickOutside` | `EventCallback` |  |  |
| `Class` | `string?` |  |  |

#### `PopoverTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  |  |
| `AsChild` | `bool` | `false` | When true, the trigger does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to act as the trigger. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  |  |
| `CustomClickHandling` | `bool` | `false` |  |

**Basic Usage:**
```razor
<Popover>
    <PopoverTrigger AsChild>
        <Button Variant="ButtonVariant.Outline">Open Popover</Button>
    </PopoverTrigger>
    <PopoverContent>
        <div class="space-y-2">
            <h4 class="font-medium">Dimensions</h4>
            <p class="text-sm">Set the dimensions for the layer.</p>
        </div>
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

**Description:**
Progress bars with animations and indeterminate state. Visual indicator for task completion.

**Components & Parameters:**

#### `Progress`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` |  | Gets or sets the current progress value. |
| `Max` | `double` | `100` | Gets or sets the maximum value for the progress bar. Default value is 100. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the progress bar. |

**Basic Usage:**
```razor
@* Determinate progress *@
<Progress Value="progressValue" />

@* Indeterminate progress *@
<Progress Indeterminate="true" />

@* With custom color *@
<Progress Value="75" Class="[&>div]:bg-green-500" />

@code {
    private int progressValue = 60;
}
```

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

**Description:**
Radio button groups with keyboard navigation. Mutually exclusive selection from multiple options.

**Components & Parameters:**

#### `RadioGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `TValue` |  | Gets or sets the currently selected value. This property supports two-way binding using the @bind-Value directive. Changes to this property trigger the ValueChanged event callback. |
| `ValueChanged` | `EventCallback<TValue>` |  | Gets or sets the callback invoked when the selected value changes. This event callback enables two-way binding with @bind-Value. It is invoked whenever a radio button is selected. |
| `Disabled` | `bool` |  | Gets or sets whether the entire radio group is disabled. When disabled, all radio items in the group cannot be selected. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the radio group container. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the radio group. Provides accessible text for screen readers to describe the purpose of the radio group. |
| `Name` | `string?` |  | Gets or sets the name for all radio items in this group. This name is shared by all radio items in the group, making them mutually exclusive. Critical for form submission - the selected value will be submitted with this name. |
| `Required` | `bool` |  | Gets or sets whether a selection is required in this radio group. When true, the user must select one of the radio items for form submission. Works with form validation. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the radio group. Should contain RadioGroupItem components. |
| `ValueExpression` | `Expression<Func<TValue>>?` |  | Gets or sets an expression that identifies the bound value. Used for form validation integration. When provided, the radio group registers with the EditContext and participates in form validation. |

#### `RadioGroupItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Disabled` | `bool` |  | Gets or sets whether this individual radio item is disabled. When disabled, the item cannot be selected and appears with reduced opacity. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the radio item. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the radio item. Provides accessible text for screen readers when the radio item doesn't have an associated label element. |
| `Id` | `string?` |  | Gets or sets the ID attribute for the radio item element. Used for associating the radio item with label elements via htmlFor attribute. |

**Basic Usage:**
```razor
<RadioGroup @bind-Value="selectedOption">
    <div class="flex items-center space-x-2">
        <RadioGroupItem Value="option1" Id="option1" />
        <Label For="option1">Option 1</Label>
    </div>
    <div class="flex items-center space-x-2">
        <RadioGroupItem Value="option2" Id="option2" />
        <Label For="option2">Option 2</Label>
    </div>
    <div class="flex items-center space-x-2">
        <RadioGroupItem Value="option3" Id="option3" />
        <Label For="option3">Option 3</Label>
    </div>
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

**Description:**
Split layouts with draggable handles. Create resizable split panes.

**Components & Parameters:**

#### `ResizableHandle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Index` | `int` |  | The index of the handle (zero-based, between panels). |
| `WithHandle` | `bool` | `false` | Whether to show a visual grip handle. Default is false. |
| `Class` | `string?` |  | Additional CSS classes to apply to the handle. |

#### `ResizablePanel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render within the panel. |
| `DefaultSize` | `double?` |  | The default size of the panel as a percentage. |
| `MinSize` | `double?` |  | The minimum size of the panel as a percentage. |
| `MaxSize` | `double?` |  | The maximum size of the panel as a percentage. |
| `Collapsible` | `bool` | `false` | Whether the panel is collapsible. |
| `Class` | `string?` |  | Additional CSS classes to apply to the panel. |

#### `ResizablePanelGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render within the panel group. Should contain ResizablePanel and ResizableHandle components. |
| `Direction` | `ResizableDirection` | `ResizableDirection.Horizontal` | The direction of the panel layout. Default is Horizontal. |
| `DefaultSizes` | `double[]?` |  | The initial sizes of the panels as percentages (must sum to 100). If not provided, panels will be sized equally. |
| `OnLayoutChange` | `EventCallback<double[]>` |  | Event callback invoked when panel sizes change. |
| `Class` | `string?` |  | Additional CSS classes to apply to the container. |

**Basic Usage:**
```razor
<ResizablePanelGroup Direction="ResizableDirection.Horizontal">
    <ResizablePanel DefaultSize="50">
        <div class="p-4">Left Panel</div>
    </ResizablePanel>
    <ResizableHandle />
    <ResizablePanel DefaultSize="50">
        <div class="p-4">Right Panel</div>
    </ResizablePanel>
</ResizablePanelGroup>
```

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

**Description:**
WYSIWYG editor with formatting toolbar and HTML output. Full-featured rich text editing.

**Components & Parameters:**

#### `RichTextEditor`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string?` |  | Gets or sets the HTML content value. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback invoked when the value changes. |
| `Placeholder` | `string?` |  | Gets or sets the placeholder text displayed when the editor is empty. |
| `Disabled` | `bool` |  | Gets or sets whether the editor is disabled. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the editor container. |
| `Id` | `string?` |  | Gets or sets the HTML id attribute for the contenteditable element. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the editor. |
| `AriaDescribedBy` | `string?` |  | Gets or sets the ID of the element that describes the editor. |
| `AriaInvalid` | `bool?` |  | Gets or sets whether the editor value is invalid. |
| `MinHeight` | `string` | `"150px"` | Gets or sets the minimum height of the editor content area. |
| `MaxHeight` | `string?` |  | Gets or sets the maximum height of the editor content area. When content exceeds this height, a scrollbar appears. |
| `Height` | `string?` |  | Gets or sets a fixed height for the editor content area. When set, the editor will not auto-expand and will show scrollbar when content overflows. Takes precedence over MinHeight/MaxHeight when set. |

**Basic Usage:**
```razor
<RichTextEditor @bind-Value="htmlContent" 
                Placeholder="Start typing..." />

@* With custom toolbar *@
<RichTextEditor @bind-Value="htmlContent">
    <ToolbarContent>
        <ToolbarButton Command="bold">Bold</ToolbarButton>
        <ToolbarButton Command="italic">Italic</ToolbarButton>
    </ToolbarContent>
</RichTextEditor>

@code {
    private string htmlContent = "<p>Hello World</p>";
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

**Description:**
Custom scrollbars for styled scroll regions. Enhanced scrolling with custom styled scrollbars.

**Components & Parameters:**

#### `ScrollArea`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render within the scrollable area. |
| `Type` | `ScrollAreaType` | `ScrollAreaType.Auto` | The type of scrollbar to show. Default is Auto (scrollbars visible when content overflows). |
| `ScrollHideDelay` | `int` | `600` | Controls the visibility delay of scrollbars in milliseconds. Only applies when Type is Scroll or Hover. Default is 600ms. |
| `ShowVerticalScrollbar` | `bool` | `true` | Whether to show the vertical scrollbar. Default is true. |
| `ShowHorizontalScrollbar` | `bool` | `false` | Whether to show the horizontal scrollbar. Default is false. |
| `EnableScrollShadows` | `bool` | `false` | Whether to enable scroll shadows that indicate more content is available. Default is false. |
| `Class` | `string?` |  | Additional CSS classes to apply to the root element. |

#### `ScrollBar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Orientation.Vertical` | The orientation of the scrollbar. Default is Vertical. |
| `Class` | `string?` |  | Additional CSS classes to apply to the scrollbar. |

**Basic Usage:**
```razor
<ScrollArea Class="h-72 w-48">
    <div class="p-4">
        <ul class="space-y-4">
            @for (int i = 1; i <= 50; i++)
            {
                <li>Item @i</li>
            }
        </ul>
    </div>
</ScrollArea>

@* Horizontal scroll *@
<ScrollArea Orientation="ScrollOrientation.Horizontal">
    <ul class="flex gap-4 p-4">
        @for (int i = 1; i <= 20; i++)
        {
            <li class="w-48">
                <Card>Item @i</Card>
            </li>
        }
    </ul>
</ScrollArea>
```

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

**Description:**
Dropdown select with search and keyboard navigation. Enhanced select component with filtering.

**Components & Parameters:**

#### `Select`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `TValue?` |  | Gets or sets the currently selected value. |
| `ValueChanged` | `EventCallback<TValue?>` |  | Gets or sets the callback invoked when the selected value changes. |
| `Disabled` | `bool` |  | Gets or sets whether the select is disabled. |
| `Open` | `bool?` |  | Gets or sets whether the dropdown is open (controlled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Gets or sets the callback invoked when the open state changes. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the select container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the select component. Should contain SelectTrigger and SelectContent components. |

#### `SelectContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the content container. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the dropdown. |

#### `SelectGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the group. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the group. |

#### `SelectItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Text` | `string?` |  | Gets or sets the display text for this item. If not provided, will use ChildContent text or Value.ToString(). |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the item. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered as the item's display text. |
| `Disabled` | `bool` |  | Gets or sets whether this item is disabled. |

#### `SelectLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the label. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the label. |

#### `SelectTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the trigger button. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the trigger button. |

#### `SelectValue`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Placeholder` | `string?` |  | Gets or sets the placeholder text to display when no value is selected. |

**Basic Usage:**
```razor
<Select @bind-Value="selectedValue" Placeholder="Select an option">
    <SelectTrigger>
        <SelectValue />
    </SelectTrigger>
    <SelectContent>
        <SelectItem Value="option1">Option 1</SelectItem>
        <SelectItem Value="option2">Option 2</SelectItem>
        <SelectItem Value="option3">Option 3</SelectItem>
    </SelectContent>
</Select>

@code {
    private string? selectedValue;
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

**Description:**
Visual dividers for separating content. Horizontal or vertical line separator.

**Components & Parameters:**

#### `Separator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `SeparatorOrientation` | `SeparatorOrientation.Horizontal` | Gets or sets the orientation of the separator. Determines whether the separator is displayed horizontally or vertically. Default value is . |
| `Decorative` | `bool` | `true` | Gets or sets whether the separator is purely decorative. When true (default), the separator is treated as decorative (role="none") and hidden from assistive technologies. When false, the separator is semantic (role="separator") and will be announced to screen readers, with the orientation specified via aria-orientation. Set to false when the separator provides meaningful structural information about content hierarchy. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the separator. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |

**Basic Usage:**
```razor
@* Horizontal separator *@
<div>Content above</div>
<Separator />
<div>Content below</div>

@* Vertical separator *@
<div class="flex gap-4 items-center">
    <span>Left</span>
    <Separator Orientation="SeparatorOrientation.Vertical" Class="h-4" />
    <span>Right</span>
</div>
```

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

**Description:**
Slide-out panels from any edge (top, right, bottom, left). Drawer component for mobile and desktop.

**Components & Parameters:**

#### `Sheet`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the sheet. |
| `Open` | `bool?` |  | Controls whether the sheet is open (controlled mode). When null, the sheet manages its own state (uncontrolled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. Use with @bind-Open for two-way binding. |
| `DefaultOpen` | `bool` | `false` | Default open state when in uncontrolled mode. |
| `OnOpenChange` | `EventCallback<bool>` |  | Event callback invoked when the sheet open state changes. |
| `Side` | `SheetSide` | `SheetSide.Right` | The side from which the sheet slides in. Default is Right. |
| `Modal` | `bool` | `true` | Whether the sheet can be dismissed by clicking the overlay or pressing Escape. Default is true. |

#### `SheetClose`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the close button. |
| `AsChild` | `bool` | `false` | When true, the close does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to act as the close button. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Custom click handler. Called after the sheet is closed. |
| `PreventClose` | `bool` | `false` | Whether to prevent the default close behavior. When true, only the OnClick handler is invoked. Default is false (sheet closes on click). |

#### `SheetContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the sheet. |
| `Class` | `string?` |  | Additional CSS classes to apply to the sheet content. |
| `Side` | `SheetSide?` |  | The side from which the sheet slides in. When null, uses the side from the parent Sheet component. |
| `ShowClose` | `bool` | `true` | Whether to show the close button (X icon). Default is true. |
| `CloseOnEscape` | `bool` | `true` | Whether pressing Escape should close the sheet. Default is true. |
| `TrapFocus` | `bool` | `true` | Whether to trap focus within the sheet. Default is true for modal sheets. |
| `LockScroll` | `bool` | `true` | Whether to lock body scroll when sheet is open. Default is true for modal sheets. |
| `OnEscapeKeyDown` | `EventCallback<KeyboardEventArgs>` |  | Event callback invoked when Escape key is pressed. |

#### `SheetDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The description text or content. |
| `Class` | `string?` |  | Additional CSS classes to apply to the description. |

#### `SheetFooter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display in the sheet footer (typically action buttons). |
| `Class` | `string?` |  | Additional CSS classes to apply to the footer container. |

#### `SheetHeader`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display in the sheet header (typically SheetTitle and SheetDescription). |
| `Class` | `string?` |  | Additional CSS classes to apply to the header container. |

#### `SheetTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The title text or content. |
| `Class` | `string?` |  | Additional CSS classes to apply to the title. |

#### `SheetTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger button. |
| `AsChild` | `bool` | `false` | When true, the trigger does not render its own button element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component (like Button) to act as the trigger. |

**Basic Usage:**
```razor
<Sheet>
    <SheetTrigger AsChild>
        <Button>Open Sheet</Button>
    </SheetTrigger>
    <SheetContent>
        <SheetHeader>
            <SheetTitle>Sheet Title</SheetTitle>
            <SheetDescription>Sheet description goes here.</SheetDescription>
        </SheetHeader>
        <div class="py-4">
            <p>Sheet content</p>
        </div>
        <SheetFooter>
            <Button>Save</Button>
        </SheetFooter>
    </SheetContent>
</Sheet>

@* From different sides *@
<Sheet Side="SheetSide.Left">...</Sheet>
<Sheet Side="SheetSide.Top">...</Sheet>
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

**Description:**
Responsive sidebar with collapsible icon mode, multiple variants (default, floating, inset), and mobile sheet integration.

**Components & Parameters:**

#### `Sidebar`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the sidebar. |
| `Class` | `string?` |  | Additional CSS classes to apply to the sidebar. |
| `Collapsible` | `bool` | `true` | Collapsible behavior: icon-only when collapsed, full width when expanded. Default is true. |
| `AutoDetectActive` | `bool` | `false` | Whether menu items should automatically detect their active state based on current URL. When enabled, all SidebarMenuButton and SidebarMenuSubButton components will automatically highlight based on whether their Href matches the current route. Default is false. |

#### `SidebarContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarFooter`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render in the footer. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The group content (typically SidebarGroupLabel, SidebarGroupAction, SidebarGroupContent). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarGroupAction`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The button content (typically an icon). |
| `Tooltip` | `string?` |  | Tooltip text for the action. |
| `AsChild` | `SidebarMenuButtonElement` | `SidebarMenuButtonElement.Button` | The element tag to render (button or a). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarGroupContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The group content. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarGroupLabel`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The label content. |
| `AsChild` | `SidebarGroupLabelElement` | `SidebarGroupLabelElement.Div` | The element tag to render (div or button for collapsible). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarHeader`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render in the header. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarHeaderContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render (typically SidebarHeaderIcon and SidebarHeaderInfo). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarHeaderInfo`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The text content to render (typically title and subtitle spans). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarInset`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The main content to render. |
| `Class` | `string?` |  | Additional CSS classes to apply to the inset. |

#### `SidebarMenu`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The menu items to render. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuAction`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The button content (typically an icon). |
| `Tooltip` | `string?` |  | Tooltip text for the action. |
| `ShowOnHover` | `bool` |  | Whether to show the action only on hover. |
| `AsChild` | `SidebarMenuButtonElement` | `SidebarMenuButtonElement.Button` | The element tag to render (button or a). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuBadge`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The badge content (typically a number or text). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuButton`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The button content. |
| `Tooltip` | `string?` |  | Tooltip text to show when sidebar is collapsed (icon-only mode). |
| `Size` | `SidebarMenuButtonSize` | `SidebarMenuButtonSize.Default` | Size variant for the button. |
| `Variant` | `SidebarMenuButtonVariant` | `SidebarMenuButtonVariant.Default` | Style variant for the button. |
| `IsActive` | `bool?` |  | Whether this menu item is active/selected. If not set, active state will be auto-detected based on Href and Match if the Sidebar has AutoDetectActive enabled. |
| `AsChild` | `SidebarMenuButtonElement` | `SidebarMenuButtonElement.Button` | The element type to render. Defaults to Button, but automatically switches to Anchor if Href is provided. |
| `Href` | `string?` |  | The URL to navigate to. When provided, the button automatically renders as a NavLink (unless AsChild is explicitly set to Button). |
| `Match` | `NavLinkMatch` | `NavLinkMatch.Prefix` | How the link should match the current URL. Default is NavLinkMatch.Prefix. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuChevron`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The chevron icon content (typically a LucideIcon). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The menu item content (typically SidebarMenuButton). |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuSkeleton`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowIcon` | `bool` |  | Whether to show an icon skeleton. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuSub`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The submenu items to render. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuSubButton`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The button content. |
| `AsChild` | `SidebarMenuButtonElement` | `SidebarMenuButtonElement.Button` | The element tag to render. Defaults to Button, but automatically switches to Anchor if Href is provided. |
| `Href` | `string?` |  | The URL to navigate to. When provided, the button automatically renders as a NavLink (unless AsChild is explicitly set to Button). |
| `Match` | `NavLinkMatch` | `NavLinkMatch.Prefix` | How the link should match the current URL. Default is NavLinkMatch.Prefix. |
| `IsActive` | `bool?` |  | Whether this submenu item is active/selected. If not set, active state will be auto-detected based on Href and Match if the Sidebar has AutoDetectActive enabled. |
| `Size` | `SidebarMenuSubButtonSize` | `SidebarMenuSubButtonSize.Medium` | Button size variant. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarMenuSubItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The submenu item content (typically SidebarMenuSubButton). |

#### `SidebarProvider`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the sidebar provider. Should typically contain SidebarLayout with Sidebar and SidebarInset. |
| `DefaultOpen` | `bool` | `true` | Default open state for the sidebar on desktop. |
| `Variant` | `SidebarVariant` | `SidebarVariant.Sidebar` | The sidebar variant/style. |
| `Side` | `SidebarSide` | `SidebarSide.Left` | Which side the sidebar appears on. |
| `CookieKey` | `string?` | `"sidebar:state"` | Cookie key for persisting sidebar state. Set to null to disable persistence. |
| `HeightClass` | `string` | `"min-h-screen"` | CSS class for controlling the container height. Defaults to "min-h-screen" to fill viewport and grow with content. Can be set to "h-screen" for fixed viewport height or "h-full" for contained layouts. |
| `StaticRendering` | `bool` | `false` | Enable static rendering mode for SSR/prerendering scenarios. When true, JS will set up click delegation to call C# toggle via interop. Set to true when using rendermode="InteractiveServer" or "InteractiveAuto" in MainLayout. |

#### `SidebarRail`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Click handler for custom behavior. |
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarSeparator`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` |  | Additional CSS classes. |

#### `SidebarTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | Custom content for the trigger button. If not provided, defaults to a panel-left icon. |
| `OnClick` | `EventCallback<MouseEventArgs>` |  | Click handler for custom behavior.4 |
| `Class` | `string?` |  | Additional CSS classes. |

**Basic Usage:**
```razor
<SidebarProvider>
    <Sidebar>
        <SidebarHeader>
            <h2 class="text-lg font-semibold">App Name</h2>
        </SidebarHeader>
        <SidebarContent>
            <SidebarGroup>
                <SidebarGroupLabel>Navigation</SidebarGroupLabel>
                <SidebarGroupContent>
                    <SidebarMenuItem>
                        <SidebarMenuButton Href="/">
                            <LucideIcon Name="home" Size="16" />
                            Home
                        </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                        <SidebarMenuButton Href="/settings">
                            <LucideIcon Name="settings" Size="16" />
                            Settings
                        </SidebarMenuButton>
                    </SidebarMenuItem>
                </SidebarGroupContent>
            </SidebarGroup>
        </SidebarContent>
        <SidebarFooter>
            <SidebarMenuItem>User Menu</SidebarMenuItem>
        </SidebarFooter>
    </Sidebar>
    <SidebarInset>
        <main>@Body</main>
    </SidebarInset>
</SidebarProvider>
```

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

**Description:**
Loading placeholders with shimmer animation. Shows content structure while loading.

**Components & Parameters:**

#### `Skeleton`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Shape` | `SkeletonShape` | `SkeletonShape.Rectangular` | Gets or sets the shape variant of the skeleton. A  value. Default is . : Default rectangular shape with rounded corners : Circular shape, ideal for avatar placeholders |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the skeleton element. A string containing one or more CSS class names, or null. Use this parameter to customize the skeleton's dimensions and spacing. Common Tailwind utilities include: Height: h-4, h-12, h-[200px] Width: w-full, w-[250px], w-1/2 Margin: mb-2, mt-4 |

**Basic Usage:**
```razor
@* Simple skeleton *@
<Skeleton Class="h-12 w-12 rounded-full" />

@* Card skeleton *@
<Card>
    <CardHeader>
        <Skeleton Class="h-4 w-1/2" />
        <Skeleton Class="h-3 w-3/4 mt-2" />
    </CardHeader>
    <CardContent>
        <Skeleton Class="h-32 w-full" />
    </CardContent>
</Card>
```

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

**Description:**
Range input for numeric value selection with single or multiple handles. Interactive slider for selecting values.

**Components & Parameters:**

#### `Slider`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` |  | Gets or sets the current value of the slider. |
| `ValueChanged` | `EventCallback<double>` |  | Gets or sets the callback that is invoked when the value changes. |
| `Min` | `double` | `0` | Gets or sets the minimum value. |
| `Max` | `double` | `100` | Gets or sets the maximum value. |
| `Step` | `double` | `1` | Gets or sets the step increment. |
| `Disabled` | `bool` |  | Gets or sets whether the slider is disabled. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the slider. |

**Basic Usage:**
```razor
@* Single value slider *@
<Slider @bind-Value="sliderValue" Min="0" Max="100" Step="1" />
<p>Value: @sliderValue</p>

@* Range slider *@
<Slider @bind-Value="rangeValues" Min="0" Max="100" />

@code {
    private double sliderValue = 50;
    private double[] rangeValues = new[] { 25.0, 75.0 };
}
```

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

**Description:**
Loading indicators with multiple sizes. Animated spinner for loading states.

**Components & Parameters:**

#### `Spinner`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Size` | `SpinnerSize` | `SpinnerSize.Medium` | Gets or sets the size of the spinner. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the spinner. |

**Basic Usage:**
```razor
@* Default spinner *@
<Spinner />

@* Different sizes *@
<Spinner Size="SpinnerSize.Small" />
<Spinner Size="SpinnerSize.Large" />

@* With text *@
<div class="flex items-center gap-2">
    <Spinner />
    <span>Loading...</span>
</div>
```

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

**Description:**
Toggle switch component for boolean settings. Alternative to checkbox with toggle UI.

**Components & Parameters:**

#### `Switch`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Checked` | `bool` |  | Gets or sets whether the switch is checked (on). This property supports two-way binding using the @bind-Checked directive. Changes to this property trigger the CheckedChanged event callback. |
| `CheckedChanged` | `EventCallback<bool>` |  | Gets or sets the callback invoked when the checked state changes. This event callback enables two-way binding with @bind-Checked. It is invoked whenever the user toggles the switch state. |
| `Disabled` | `bool` |  | Gets or sets whether the switch is disabled. When disabled: - Switch cannot be clicked or focused - Opacity is reduced - Pointer events are disabled - aria-disabled attribute is set to true |
| `Size` | `SwitchSize` | `SwitchSize.Medium` | Gets or sets the size variant of the switch. Available sizes: - Small: Compact switch for dense layouts - Medium: Default size (recommended) - Large: Prominent switch for primary actions |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the switch. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the switch. Provides accessible text for screen readers when the switch doesn't have associated label text. |
| `Id` | `string?` |  | Gets or sets the ID attribute for the switch element. Used for associating the switch with label elements via htmlFor attribute. |
| `Name` | `string?` |  | Gets or sets the name of the switch for form submission. This is critical for form submission. The name/value pair is submitted to the server. If not specified, falls back to the Id value. |
| `Required` | `bool` |  | Gets or sets whether the switch is required. When true, the switch must be checked for form submission. Works with form validation. |
| `CheckedExpression` | `Expression<Func<bool>>?` |  | Gets or sets an expression that identifies the bound value. Used for form validation integration. When provided, the switch registers with the EditContext and participates in form validation. |

**Basic Usage:**
```razor
@* Simple switch *@
<Switch @bind-Checked="isEnabled" />

@* With label *@
<div class="flex items-center space-x-2">
    <Switch Id="airplane-mode" @bind-Checked="airplaneMode" />
    <Label For="airplane-mode">Airplane Mode</Label>
</div>

@* Disabled *@
<Switch Disabled="true" />

@code {
    private bool isEnabled = false;
    private bool airplaneMode = false;
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

**Description:**
Tabbed interfaces with controlled/uncontrolled modes. Organize content into switchable tabs.

**Components & Parameters:**

#### `Tabs`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the tabs. Should include TabsList and TabsContent components. |
| `Value` | `string?` |  | Controls which tab is active (controlled mode). When null, the tabs manage their own state (uncontrolled mode). |
| `ValueChanged` | `EventCallback<string?>` |  | Event callback invoked when the active tab changes. Use with @bind-Value for two-way binding. |
| `DefaultValue` | `string?` |  | Default active tab value when in uncontrolled mode. |
| `OnValueChange` | `EventCallback<string?>` |  | Event callback invoked when the active tab changes. |
| `Orientation` | `TabsOrientation` | `TabsOrientation.Horizontal` | Orientation of the tabs (horizontal or vertical). Default is horizontal. |
| `ActivationMode` | `TabsActivationMode` | `TabsActivationMode.Automatic` | Activation mode for tabs (automatic on focus or manual on click). Default is automatic. |

#### `TabsContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ForceMount` | `bool` | `false` | Whether to force mount the content even when inactive. |
| `ChildContent` | `RenderFragment?` |  | The child content to render when this tab is active. |
| `Class` | `string?` |  | Additional CSS classes to apply to the tab content. |

#### `TabsList`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the tabs list. Should contain TabsTrigger components. |
| `Class` | `string?` |  | Additional CSS classes to apply to the tabs list. |

#### `TabsTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Disabled` | `bool` |  | Whether this tab trigger is disabled. |
| `ChildContent` | `RenderFragment?` |  | The child content to render within the tab trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply to the tab trigger. |

**Basic Usage:**
```razor
<Tabs DefaultValue="tab1">
    <TabsList>
        <TabsTrigger Value="tab1">Account</TabsTrigger>
        <TabsTrigger Value="tab2">Password</TabsTrigger>
    </TabsList>
    <TabsContent Value="tab1">
        <Card>
            <CardHeader>
                <CardTitle>Account</CardTitle>
            </CardHeader>
            <CardContent>Account settings content</CardContent>
        </Card>
    </TabsContent>
    <TabsContent Value="tab2">
        <Card>
            <CardHeader>
                <CardTitle>Password</CardTitle>
            </CardHeader>
            <CardContent>Password settings content</CardContent>
        </Card>
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

**Description:**
Multi-line text input with automatic content sizing. Resizable textarea for long text input.

**Components & Parameters:**

#### `Textarea`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string?` |  | Gets or sets the current value of the textarea. Supports two-way binding via @bind-Value syntax. |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback invoked when the textarea value changes. This event is fired on every keystroke (oninput event). Use with Value parameter for two-way binding. |
| `Placeholder` | `string?` |  | Gets or sets the placeholder text displayed when the textarea is empty. Provides a hint to the user about what to enter. Should not be used as a replacement for a label. |
| `Disabled` | `bool` |  | Gets or sets whether the textarea is disabled. When disabled: - Textarea cannot be focused or edited - Cursor is set to not-allowed - Opacity is reduced for visual feedback |
| `Required` | `bool` |  | Gets or sets whether the textarea is required. When true, the HTML5 required attribute is set. Works with form validation and :invalid CSS pseudo-class. |
| `MaxLength` | `int?` |  | Gets or sets the maximum number of characters allowed in the textarea. When set, the HTML5 maxlength attribute is applied. Browser will prevent users from entering more than this many characters. |
| `MinLength` | `int?` |  | Gets or sets the minimum number of characters required. Works with form validation. Browser validates that at least this many characters are present. |
| `Name` | `string?` |  | Gets or sets the name of the textarea for form submission. This is critical for form submission. The name/value pair is submitted to the server. Should be unique within the form. |
| `Autocomplete` | `string?` |  | Gets or sets the autocomplete hint for the browser. Examples: "on", "off", "name", "street-address". Helps browsers provide appropriate autofill suggestions. |
| `Readonly` | `bool` |  | Gets or sets whether the textarea is read-only. When true, the user cannot modify the value, but it's still focusable and submitted with forms. Different from Disabled - readonly textareas are still submitted with forms. |
| `Rows` | `int?` |  | Gets or sets the visible number of text rows. Specifies the height of the textarea in rows of text. If not specified, the component uses field-sizing-content for automatic sizing. |
| `Cols` | `int?` |  | Gets or sets the visible width in characters. Specifies the width of the textarea in average character widths. Usually controlled by CSS width instead. |
| `Wrap` | `string?` |  | Gets or sets how text wraps when submitted in a form. Values: "soft" (default - newlines not submitted), "hard" (newlines submitted), "off" (no wrapping). When "hard", the cols attribute must be specified. |
| `InputMode` | `string?` |  | Gets or sets the input mode hint for mobile keyboards. Examples: "none", "text", "decimal", "numeric", "tel", "search", "email", "url". Helps mobile devices show the appropriate keyboard. |
| `Autofocus` | `bool` |  | Gets or sets whether the textarea should be auto-focused when the page loads. Only one element per page should have autofocus. Improves accessibility when used appropriately. |
| `Spellcheck` | `bool?` |  | Gets or sets whether spell checking is enabled. Can be true, false, or null (browser default). Useful for controlling spell checking on technical content, code, etc. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the textarea. Custom classes are appended after the component's base classes, allowing for style overrides and extensions. |
| `Id` | `string?` |  | Gets or sets the HTML id attribute for the textarea element. Used to associate the textarea with a label element via the label's 'for' attribute. This is essential for accessibility and allows clicking the label to focus the textarea. |
| `AriaLabel` | `string?` |  | Gets or sets the ARIA label for the textarea. Provides an accessible name for screen readers. Use when there is no visible label element. |
| `AriaDescribedBy` | `string?` |  | Gets or sets the ID of the element that describes the textarea. References the id of an element containing help text or error messages. Improves screen reader experience by associating descriptive text. |
| `AriaInvalid` | `bool?` |  | Gets or sets whether the textarea value is invalid. When true, aria-invalid="true" is set. Should be set based on validation state. Triggers destructive color styling for error states. |

**Basic Usage:**
```razor
@* Simple textarea *@
<Textarea @bind-Value="description" 
          Placeholder="Enter description..." />

@* With rows *@
<Textarea @bind-Value="comments" 
          Rows="5" 
          Placeholder="Your comments..." />

@* Disabled *@
<Textarea Value="Read only text" Disabled="true" />

@code {
    private string description = "";
    private string comments = "";
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

**Description:**
Time selection with 12/24-hour format support. Interactive time input with dropdown selection.

**Components & Parameters:**

#### `TimePicker`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `SelectedTime` | `TimeOnly?` |  | The selected time. |
| `SelectedTimeChanged` | `EventCallback<TimeOnly?>` |  | Event callback invoked when the selected time changes. Use with @bind-SelectedTime for two-way binding. |
| `Use24HourFormat` | `bool` | `false` | Whether to use 24-hour format. Default is false (12-hour with AM/PM). |
| `MinuteStep` | `int` | `1` | Minute step interval. Default is 1. |
| `ButtonVariant` | `ButtonVariant` | `ButtonVariant.Outline` | Button variant for the trigger button. |
| `ButtonSize` | `ButtonSize` | `ButtonSize.Default` | Button size for the trigger button. |
| `ShowIcon` | `bool` | `true` | Whether to show the clock icon in the button. |
| `Placeholder` | `string` | `"Pick a time"` | Placeholder text when no time is selected. |
| `TimeFormat` | `string?` |  | Time format string for displaying the selected time. If not specified, uses "hh:mm tt" for 12-hour or "HH:mm" for 24-hour. |
| `Disabled` | `bool` |  | Whether the time picker is disabled. |
| `Align` | `PopoverAlign` | `PopoverAlign.Start` | Alignment of the popover content. |
| `Class` | `string?` |  | Additional CSS classes for the button. |
| `AriaLabel` | `string?` |  | ARIA label for the button. |

**Basic Usage:**
```razor
@* 12-hour format *@
<TimePicker @bind-Value="selectedTime" />

@* 24-hour format *@
<TimePicker @bind-Value="selectedTime" 
            Format="TimeFormat.Hour24" />

@code {
    private TimeSpan? selectedTime;
}
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

**Description:**
Temporary notifications with variants and actions. Shows brief messages and alerts.

**Components & Parameters:**

#### `Toast`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string?` |  | The unique identifier for this toast. |
| `Title` | `string?` |  | The title of the toast. |
| `Description` | `string?` |  | The description of the toast. |
| `Variant` | `ToastVariant` | `ToastVariant.Default` | The variant/style of the toast. |
| `ActionLabel` | `string?` |  | The label for the action button. |
| `OnAction` | `Action?` |  | Callback when action button is clicked. |
| `OnDismiss` | `Action?` |  | Callback when the toast is dismissed. |
| `Class` | `string?` |  | Additional CSS classes to apply to the toast. |

#### `ToastAction`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Label` | `string?` |  | The button label. |
| `OnClick` | `EventCallback` |  | Callback when the button is clicked. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ToastClose`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `OnClick` | `EventCallback` |  | Callback when the button is clicked. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ToastDescription`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The description content. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ToastProvider`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render within the provider. |
| `Position` | `ToastPosition` | `ToastPosition.BottomRight` | The position of the toast viewport. Default is BottomRight. |
| `MaxToasts` | `int` | `5` | Maximum number of toasts to display at once. Default is 5. |

#### `ToastTitle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The title content. |
| `Class` | `string?` |  | Additional CSS classes to apply. |

#### `ToastViewport`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Position` | `ToastPosition` | `ToastPosition.BottomRight` | The position of the viewport. Default is BottomRight. |
| `MaxToasts` | `int` | `5` | Maximum number of toasts to display at once. Default is 5. |
| `Class` | `string?` |  | Additional CSS classes to apply to the viewport. |

**Basic Usage:**
```razor
@inject IToastService ToastService

<Button OnClick="ShowToast">Show Toast</Button>

@code {
    private void ShowToast()
    {
        ToastService.Show("Event created successfully!");
    }
    
    private void ShowDestructiveToast()
    {
        ToastService.Show(
            "Error occurred",
            "There was a problem with your request.",
            ToastVariant.Destructive
        );
    }
}
```

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

**Description:**
Pressable toggle buttons for boolean states. Button that toggles between pressed and unpressed.

**Components & Parameters:**

#### `Toggle`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Pressed` | `bool` |  | Gets or sets whether the toggle is pressed. |
| `PressedChanged` | `EventCallback<bool>` |  | Gets or sets the callback that is invoked when the pressed state changes. |
| `Variant` | `ToggleVariant` | `ToggleVariant.Default` | Gets or sets the visual variant of the toggle. |
| `Size` | `ToggleSize` | `ToggleSize.Default` | Gets or sets the size of the toggle. |
| `Disabled` | `bool` |  | Gets or sets whether the toggle is disabled. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the toggle. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the toggle. |

**Basic Usage:**
```razor
@* Simple toggle *@
<Toggle @bind-Pressed="isPressed">
    <LucideIcon Name="bold" Size="16" />
</Toggle>

@* Toggle group *@
<div class="flex gap-1">
    <Toggle @bind-Pressed="isBold">
        <LucideIcon Name="bold" Size="16" />
    </Toggle>
    <Toggle @bind-Pressed="isItalic">
        <LucideIcon Name="italic" Size="16" />
    </Toggle>
</div>

@code {
    private bool isPressed = false;
    private bool isBold = false;
    private bool isItalic = false;
}
```

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

**Description:**
Single or multiple selection toggle groups. Group of toggle buttons with exclusive or multi-select.

**Components & Parameters:**

#### `ToggleGroup`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Type` | `ToggleGroupType` | `ToggleGroupType.Single` | Gets or sets the type of toggle group (single or multiple selection). |
| `Value` | `string?` |  | Gets or sets the currently selected value (for single selection mode). |
| `ValueChanged` | `EventCallback<string?>` |  | Gets or sets the callback that is invoked when the value changes. |
| `Values` | `List<string>?` |  | Gets or sets the currently selected values (for multiple selection mode). |
| `ValuesChanged` | `EventCallback<List<string>?>` |  | Gets or sets the callback that is invoked when the values change. |
| `Disabled` | `bool` |  | Gets or sets whether the toggle group is disabled. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the toggle group. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the toggle group. |

#### `ToggleGroupItem`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | `string.Empty` | Gets or sets the value of this toggle item. |
| `Disabled` | `bool` |  | Gets or sets whether this item is disabled. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the toggle item. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the toggle item. |

**Basic Usage:**
```razor
@* Single selection *@
<ToggleGroup @bind-Value="selectedValue" Type="ToggleGroupType.Single">
    <ToggleGroupItem Value="left">
        <LucideIcon Name="align-left" Size="16" />
    </ToggleGroupItem>
    <ToggleGroupItem Value="center">
        <LucideIcon Name="align-center" Size="16" />
    </ToggleGroupItem>
    <ToggleGroupItem Value="right">
        <LucideIcon Name="align-right" Size="16" />
    </ToggleGroupItem>
</ToggleGroup>

@* Multiple selection *@
<ToggleGroup @bind-Values="selectedValues" Type="ToggleGroupType.Multiple">
    <ToggleGroupItem Value="bold">B</ToggleGroupItem>
    <ToggleGroupItem Value="italic">I</ToggleGroupItem>
    <ToggleGroupItem Value="underline">U</ToggleGroupItem>
</ToggleGroup>

@code {
    private string? selectedValue;
    private HashSet<string> selectedValues = new();
}
```

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

**Description:**
Contextual hover tooltips with delay and positioning. Shows helpful text on hover.

**Components & Parameters:**

#### `Tooltip`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The child content to render within the tooltip. Typically includes TooltipTrigger and TooltipContent. |
| `Open` | `bool?` |  | Controls whether the tooltip is open (controlled mode). When null, the tooltip manages its own state (uncontrolled mode). |
| `OpenChanged` | `EventCallback<bool>` |  | Event callback invoked when the open state changes. Use with @bind-Open for two-way binding. |
| `DefaultOpen` | `bool` | `false` | Default open state when in uncontrolled mode. |
| `DelayDuration` | `int` | `300` | The delay in milliseconds before showing the tooltip. Default is 300ms. |
| `HideDelay` | `int` | `0` | The delay in milliseconds before hiding the tooltip after mouse leaves. Default is 0ms (immediate). |
| `Placement` | `PopoverPlacement` | `PopoverPlacement.Top` | The placement position for the tooltip. Default is PopoverPlacement.Top. |

#### `TooltipContent`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to render inside the tooltip. |
| `Class` | `string?` |  | Additional CSS classes to apply to the content container. |
| `Side` | `PopoverSide` | `PopoverSide.Top` | Preferred side for positioning: PopoverSide.Top, PopoverSide.Bottom, PopoverSide.Left, PopoverSide.Right. Default is PopoverSide.Top. |
| `Align` | `PopoverAlign` | `PopoverAlign.Center` | Alignment relative to trigger: "@PopoverAlign.Start", "@PopoverAlign.Center", "@PopoverAlign.End". Default is "@PopoverAlign.Center". |
| `Offset` | `int` | `8` | Offset distance from the trigger in pixels. Default is 8. |

#### `TooltipProvider`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `DelayDuration` | `int` | `700` | Gets or sets the duration in milliseconds to wait before showing a tooltip. The delay duration in milliseconds. Default is 700ms. This delay applies to all child  components. When a user hovers over or focuses a tooltip trigger, the tooltip will wait this duration before appearing. Recommended values: 0-200ms: Very fast (toolbars, frequently accessed UI) 300-500ms: Fast (interactive elements) 600-800ms: Standard (balanced, default) 900-1200ms: Slow (detailed information) Individual tooltips cannot override this value - they inherit it from their nearest TooltipProvider ancestor. |
| `SkipDelayDuration` | `int` | `300` | Gets or sets the duration in milliseconds during which the delay is skipped for subsequent tooltips. The skip delay duration in milliseconds. Default is 300ms. After a user sees one tooltip, subsequent tooltips within this time window appear immediately without delay. This creates a fluid "tooltip discovery mode" experience. Recommended values: 0ms: Disable skip behavior 200-400ms: Short window (closely grouped elements) 500-1000ms: Standard window (comfortable exploration) 1500-3000ms: Long window (very forgiving) Set to 0 to disable the skip delay feature entirely. All tooltips will always wait the full . |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the child content to be rendered within the provider. A  containing child components, or null. All child  components will receive the tooltip configuration from this provider via Blazor's CascadingValue mechanism. |

#### `TooltipTrigger`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` |  | The content to display inside the trigger. |
| `Class` | `string?` |  | Additional CSS classes to apply to the wrapper. |
| `AsChild` | `bool` | `false` | When true, the trigger does not render its own span element. Instead, it passes trigger behavior via TriggerContext to child components. Use this when you want a custom component to act as the trigger. |
| `Focusable` | `bool` | `true` | Whether the trigger should be keyboard focusable. Default is true. Set to false if child content is already focusable. |
| `OnMouseEnter` | `EventCallback<MouseEventArgs>` |  | Custom mouse enter handler. |
| `OnMouseLeave` | `EventCallback<MouseEventArgs>` |  | Custom mouse leave handler. |

**Basic Usage:**
```razor
<Tooltip>
    <TooltipTrigger AsChild>
        <Button Size="ButtonSize.Icon" Variant="ButtonVariant.Outline">
            <LucideIcon Name="info" Size="16" />
        </Button>
    </TooltipTrigger>
    <TooltipContent>
        <p>Additional information</p>
    </TooltipContent>
</Tooltip>

@* With delay *@
<Tooltip DelayDuration="500">
    <TooltipTrigger AsChild>
        <span>Hover me</span>
    </TooltipTrigger>
    <TooltipContent>Tooltip text</TooltipContent>
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

**Description:**
Semantic text styling components for headings, paragraphs, lists, and more. Consistent text styles across the application.

**Components & Parameters:**

#### `Typography`

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `TypographyVariant` | `TypographyVariant.P` | Gets or sets the typography variant. |
| `Class` | `string?` |  | Gets or sets additional CSS classes to apply to the typography element. |
| `ChildContent` | `RenderFragment?` |  | Gets or sets the content to be rendered inside the typography element. |

---

**Basic Usage:**
```razor
@* Headings *@
<TypographyH1>Heading 1</TypographyH1>
<TypographyH2>Heading 2</TypographyH2>
<TypographyH3>Heading 3</TypographyH3>

@* Paragraph *@
<TypographyP>
    This is a paragraph with proper styling and spacing.
</TypographyP>

@* Other elements *@
<TypographyBlockquote>
    A meaningful quote
</TypographyBlockquote>

<TypographyList>
    <li>List item 1</li>
    <li>List item 2</li>
</TypographyList>

<TypographyInlineCode>const code = true;</TypographyInlineCode>

<TypographyLead>
    Lead text that stands out
</TypographyLead>
```
