# NeoUI.Blazor

Over 100+ production-ready Blazor components with shadcn/ui design and Tailwind CSS. Beautiful defaults that you can customize to match your brand.

## ✨ Features

- **🎨 Zero Configuration**: Pre-built CSS included - no Tailwind setup required
- **🌙 Dark Mode Built-in**: Automatic dark mode support using CSS variables
- **🎯 .NET 10 Ready**: Built for the latest .NET platform with full WebAssembly and Server support
- **♿ Accessible First**: WCAG 2.1 AA compliant with proper ARIA attributes
- **🧩 Composable**: Flexible component composition patterns
- **🎭 shadcn/ui Design**: Modern design language inspired by shadcn/ui
- **🎨 Theme Compatible**: Use any theme from shadcn/ui or tweakcn.com
- **📦 Pre-Styled**: Production-ready components with beautiful defaults
- **🔧 Fully Customizable**: Override styles with custom CSS or Tailwind classes
- **🔒 Type-Safe**: Full C# type safety with IntelliSense support
- **📱 Responsive**: Mobile-first design with touch gesture support

## 📦 Installation

```bash
dotnet add package NeoUI.Blazor
```

This package automatically includes:
- `NeoUI.Blazor.Primitives` - Headless primitives providing behavior and accessibility
- `NeoUI.Icons.Lucide` - 1,640+ beautiful icons
- `NeoUI.Icons.Heroicons` - 1,288 icons across 4 variants
- `NeoUI.Icons.Feather` - 286 minimalist icons
- Pre-built CSS - No Tailwind setup required!

## 🚀 Quick Start

### 1. Add to your `_Imports.razor`:

```razor
@using NeoUI.Blazor
```

All components and their enums (e.g. `ButtonVariant`, `InputType`) live in the single `NeoUI.Blazor` namespace. If you use chart components, also add `@using NeoUI.Blazor.Charts`.

**Optional icon packages** — add whichever you need:

```razor
@using NeoUI.Icons.Lucide      @* 1,640+ icons *@
@using NeoUI.Icons.Heroicons   @* 1,288 icons across 4 variants *@
@using NeoUI.Icons.Feather     @* 286 minimalist icons *@
```

### 2. Add CSS and scripts to your `App.razor`:

NeoUI.Blazor Components come with pre-built CSS and a theme script — no Tailwind setup required!

```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />

    <!-- Pre-built NeoUI.Blazor styles -->
    <link href="@Assets["_content/NeoUI.Blazor/components.css"]" rel="stylesheet" />

    <!-- Theme script: reads localStorage and applies classes before Blazor loads (prevents FOUC) -->
    <script src="@Assets["_content/NeoUI.Blazor/js/theme.js"]"></script>

    <ImportMap />
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="@Assets["_framework/blazor.web.js"]"></script>
</body>
</html>
```

### 3. Start using components:

```razor
<Button Variant="ButtonVariant.Default">Click me</Button>

<Dialog>
    <DialogTrigger AsChild>
        <Button>Open Dialog</Button>
    </DialogTrigger>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Welcome to NeoUI</DialogTitle>
            <DialogDescription>
                Beautiful Blazor components with zero configuration
            </DialogDescription>
        </DialogHeader>
        <DialogFooter>
            <DialogClose AsChild>
                <Button Variant="ButtonVariant.Outline">Close</Button>
            </DialogClose>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

That's it! No Tailwind installation, no build configuration needed.

> 💡 **Pre-built themes**: NeoUI ships with pre-built themes built on shadcn/ui defaults — ready to use out of the box with no extra setup. See the [Theming](#-theming) section for details on applying and customizing themes.

## Localization

NeoUI ships a built-in `ILocalizer` abstraction — a thin interface that decouples your app from any specific i18n framework.

**Default behaviour** — `DefaultLocalizer` is registered automatically by `AddNeoUIComponents()`. It returns the built-in English strings for every UI key (button labels, ARIA attributes, empty-state messages, etc.). No configuration required.

**Customizing strings** — implement `ILocalizer` and register it *before* calling `AddNeoUIComponents()`:

```csharp
// Option A — custom DefaultLocalizer keys (simplest)
builder.Services.AddSingleton<ILocalizer>(sp =>
    new DefaultLocalizer(keys => {
        keys["Dialog.Close"] = "Schließen";
    }));
builder.Services.AddNeoUIComponents();

// Option B — full IStringLocalizer<T> integration
builder.Services.AddScoped<ILocalizer, StringLocalizerAdapter<MyResources>>();
builder.Services.AddNeoUIComponents();
```

Both options work for Blazor Server, WebAssembly, and Auto mode.

## ⚡ Project Template

The fastest way to start a new NeoUI app — scaffold a complete Blazor Web App pre-wired with a sidebar layout, theme switcher, dark mode toggle, Spotlight command palette, and Tailwind CSS v4 in seconds:

```bash
dotnet new install NeoUI.Blazor.Templates
dotnet new neoui -n MyApp
cd MyApp
dotnet run --project MyApp
```

Supports `Server`, `WebAssembly`, and `Auto` (default) interactivity modes. Tailwind CSS builds automatically on every `dotnet build` — Node.js just needs to be on your PATH.

> Or keep reading to add NeoUI to an existing project manually.

## 📚 Available Components (100+)

### Form & Input Components
- **Button** - Interactive buttons with 6 variants and multiple sizes
- **Button Group** - Visually group related buttons
- **Checkbox** - Binary selection with indeterminate state
- **Color Picker** - Color selection with hex, RGB, and HSL support
- **Combobox** - Autocomplete input with searchable dropdown
- **Currency Input** - Formatted currency input with locale support
- **Date Picker** - Date selection with calendar popover
- **Date Range Picker** - Date range selection with presets and two calendars
- **Field** - Form field wrapper with label, description, and errors
- **Input** - Text input fields with multiple types
- **Input Group** - Enhanced inputs with icons, buttons, and addons
- **Input OTP** - One-time password input with character slots
- **Label** - Accessible labels for form controls
- **Link Button** - Semantic links styled as buttons
- **Masked Input** - Text input with customizable format masks (phone, date, etc.)
- **Multi Select** - Searchable multi-selection with tags
- **Native Select** - Styled native HTML select with chevron icon
- **Numeric Input** - Number input with increment/decrement buttons and formatting
- **Radio Group** - Mutually exclusive options
- **Range Slider** - Dual-handle slider for selecting value ranges
- **Rating** - Star rating input with half-star precision and readonly mode
- **Select** - Dropdown selection with groups
- **Slider** - Range input for numeric values
- **Split Button** - Primary action button paired with a dropdown for secondary actions
- **Switch** - Toggle control for on/off states
- **Tag Input** - Chip/tag input with configurable triggers, async suggestions, and paste splitting
- **Textarea** - Multi-line text input
- **Time Picker** - Time selection with hour/minute/period controls
- **File Upload** - Drag-and-drop and click-to-browse file upload with validation

### Data Display Components
- **Avatar** - User profile images with fallbacks
- **Badge** - Labels for status and categories
- **Card** - Content container with header/footer
- **Data Table** - Advanced tables with sorting, filtering, pagination, server-side data, column pinning/resizing (`Resizable="true"`)/reordering (`Reorderable="true"`), tree/hierarchical rows (`ChildrenProperty`, `LoadChildrenAsync`), row context menu (`RowContextMenu`), virtualization (`Virtualize`, `ItemsProvider`), and striped rows (`Striped="true"`)
- **Data View** - Switchable list/grid layouts with search, sort, pagination, and selection
- **Dynamic Form** - Schema-driven form that renders any of 24 input types from a `FormSchema` definition
- **Empty** - Empty state displays
- **Filter** - Inline canvas filter builder with 8 field types, operator chips, LINQ extensions, and preset support
- **Grid** - Advanced data grid with state management
- **Item** - Flexible list items with media and actions
- **Kbd** - Keyboard shortcut badges
- **Progress** - Progress bars with ARIA support
- **Separator** - Visual dividers
- **Skeleton** - Loading placeholders
- **Spinner** - Loading indicators
- **Timeline** - Chronological event display with icons, connectors, status, and alternating alignment
- **Tree View** - Hierarchical data with expand/collapse, checkboxes, and single/multi-select
- **Typography** - Semantic text styling

### Navigation Components
- **Accordion** - Collapsible content sections
- **Breadcrumb** - Hierarchical navigation
- **Command** - Command palette for quick actions
- **Context Menu** - Right-click menus
- **Dropdown Menu** - Context menus with shortcuts
- **Menubar** - Desktop-style horizontal menu bar
- **Navigation Menu** - Horizontal navigation with dropdowns
- **Pagination** - Page navigation
- **Sidebar** - Responsive navigation sidebar
- **Tabs** - Tabbed interface

### Overlay Components
- **Alert Dialog** - Modal dialogs for confirmations
- **Dialog** - Modal dialogs with backdrop
- **Dialog Service** - Programmatic dialogs with async/await API
- **Drawer** - Slide-out panel with gesture controls and backdrop
- **Hover Card** - Rich preview cards on hover
- **Popover** - Floating panels for content
- **Sheet** - Side panels from viewport edges
- **Toast** - Temporary notifications
- **Tooltip** - Brief informational popups

### Feedback Components
- **Alert** - Status messages and callouts

### Layout & Display Components
- **Aspect Ratio** - Display content with desired ratio
- **Carousel** - Slideshow with gestures and animations
- **Collapsible** - Expandable content area
- **Resizable** - Split layouts with draggable handles
- **Scroll Area** - Custom scrollbars

### Advanced Components
- **Chart** - 12 chart types with beautiful defaults (Line, Bar, Area, Pie, Radar, Scatter, Radial Bar, Composed, Candlestick, Funnel, Gauge, Heatmap)
- **Markdown Editor** - Write/preview tabs with syntax support
- **Motion** - Declarative animation system with 20+ presets
- **Rich Text Editor** - WYSIWYG editor with formatting toolbar
- **Toggle** - Pressable toggle buttons
- **Toggle Group** - Single/multiple selection toggles

### Icon Libraries
- **Lucide Icons** - 1,640+ beautiful, consistent icons (included)
- **Heroicons** - 1,288 icons across 4 variants (separate package)
- **Feather Icons** - 286 minimalist icons (separate package)

## 🎯 Component Highlights

### Chart Component
Create beautiful, responsive charts with a declarative Recharts-inspired API:

```razor
<LineChart Data="@salesData">
    <XAxis DataKey="Month" />
    <YAxis />
    <CartesianGrid />
    <Tooltip />
    <Legend />
    <Line DataKey="Revenue" Fill="var(--chart-1)" />
    <Line DataKey="Target" Fill="var(--chart-2)" />
</LineChart>

@code {
    private List<SalesData> salesData = new()
    {
        new() { Month = "Jan", Revenue = 4000, Target = 3500 },
        new() { Month = "Feb", Revenue = 3000, Target = 3200 },
        new() { Month = "Mar", Revenue = 5000, Target = 4500 },
        new() { Month = "Apr", Revenue = 4500, Target = 4000 },
        new() { Month = "May", Revenue = 6000, Target = 5500 }
    };

    public class SalesData
    {
        public string Month { get; set; } = "";
        public int Revenue { get; set; }
        public int Target { get; set; }
    }
}
```

### Data Table Component
Powerful data tables with built-in sorting, filtering, pagination, and selection:

```razor
<DataTable TItem="User"
           Items="@users"
           Columns="@columns"
           ShowPagination="true"
           InitialPageSize="10" />
```

### Grid Component
Enterprise-grade data grid powered by AG DataGrid with Blazor template support and auto-discovery actions, designed with shadcn theme with real-time light/dark theme switching support:

```razor
<DataGrid Items="@orders" ActionHost="this">
    <Columns>
        <DataGridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <DataGridColumn Field="CustomerName" Header="Customer" Sortable="true" />
        <DataGridColumn Field="OrderDate" Header="Date" DataFormatString="d" />
        <DataGridColumn Field="Total" Header="Total" DataFormatString="C" />
        <DataGridColumn Field="Status" Header="Status">
            <CellTemplate Context="order">
                <Badge Variant="@GetStatusVariant(order.Status)">
                    @order.Status
                </Badge>
            </CellTemplate>
        </DataGridColumn>
        <DataGridColumn Field="Actions" Header="">
            <CellTemplate Context="order">
                <Button data-action="Edit" Variant="ButtonVariant.Ghost">
                    Edit
                </Button>
            </CellTemplate>
        </DataGridColumn>
    </Columns>
</DataGrid>

@code {
    [DataGridAction]
    private async Task Edit(Order order)
    {
        // Action auto-wired via [DataGridAction] attribute
        await ShowEditDialog(order);
    }
}
```

### Motion Component
Add smooth animations with 20+ presets:

```razor
<Motion Preset="MotionPreset.FadeIn" Duration="500">
    <div>Animated content</div>
</Motion>
```

## 🎨 Theming

NeoUI uses CSS variables for theming, compatible with shadcn/ui themes. You can use any theme from:
- [shadcn/ui themes](https://ui.shadcn.com/themes)
- [tweakcn.com](https://tweakcn.com)

Or create your own by defining CSS variables:

```css
:root {
  --background: 0 0% 100%;
  --foreground: 222.2 84% 4.9%;
  --primary: 222.2 47.4% 11.2%;
  --primary-foreground: 210 40% 98%;
  /* ... and more */
}

.dark {
  --background: 222.2 84% 4.9%;
  --foreground: 210 40% 98%;
  /* ... dark mode colors */
}
```

## 🔧 Customization

All components accept standard HTML attributes and CSS classes:

```razor
<Button Class="my-custom-class" id="my-button" data-test="submit">
    Custom Button
</Button>
```

Override default styles with Tailwind classes:

```razor
<Card Class="bg-blue-500 border-blue-700">
    Custom colored card
</Card>
```

## ♿ Accessibility

All components are built with accessibility in mind:
- WCAG 2.1 AA compliant
- Proper ARIA attributes
- Keyboard navigation support
- Screen reader friendly
- Focus management
- High contrast mode support

## 📖 Documentation & Examples

Visit our interactive documentation site for:
- Live component demos
- API reference for all components
- Code examples and recipes
- Theming guides
- Best practices

[View Full Documentation](https://github.com/jimmyps/blazor-shadcn-ui)

## 🏗️ Built On Primitives

These components are built on top of `NeoUI.Blazor.Primitives` - a library of headless, unstyled components that provide behavior and accessibility without any styling. If you need complete control over styling, you can use the primitives directly.

## 📦 Related Packages

- **NeoUI.Blazor.Primitives** - Headless, unstyled components (included)
- **NeoUI.Icons.Lucide** - 1,640+ Lucide icons (included)
- **NeoUI.Icons.Heroicons** - 1,288 Heroicons across 4 variants
- **NeoUI.Icons.Feather** - 286 beautiful minimalist icons
- **NeoUI.Blazor.Templates** - Official project template for scaffolding full Blazor apps with NeoUI

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

MIT License - see LICENSE file for details

## 🙏 Acknowledgments

- Initial Blazor components inspiration by [Mathew Taylor](https://github.com/blazorui-net/ui)
- Design inspired by [shadcn/ui](https://ui.shadcn.com)
- Icons from [Lucide](https://lucide.dev), [Heroicons](https://heroicons.com), and [Feather](https://feathericons.com)
- Built with ❤️ for the Blazor community

## 📊 Version Information

- **Current Version**: 3.6.4
- **Target Framework**: .NET 10
- **Package ID**: NeoUI.Blazor
- **Assembly Name**: NeoUI.Blazor

---

**Note**: This package was formerly known as `BlazorUI.Components`. As of version 1.0.7, the assembly name has been updated to `NeoUI.Blazor` to match the NuGet package ID, ensuring consistent asset paths when consumed from NuGet.
