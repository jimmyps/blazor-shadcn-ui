# NeoBlazorUI.Components

Over 75+ production-ready Blazor components with shadcn/ui design and Tailwind CSS. Beautiful defaults that you can customize to match your brand.

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
dotnet add package NeoBlazorUI.Components
```

This package automatically includes:
- `NeoBlazorUI.Primitives` - Headless primitives providing behavior and accessibility
- `NeoBlazorUI.Icons.Lucide` - 1,640+ beautiful icons
- Pre-built CSS - No Tailwind setup required!

## 🚀 Quick Start

### 1. Add to your `_Imports.razor`:

```razor
@using BlazorUI.Components
@using BlazorUI.Components.Button
@using BlazorUI.Components.Input
@using BlazorUI.Components.Dialog
@using BlazorUI.Components.Sheet
@using BlazorUI.Components.Accordion
@using BlazorUI.Components.Tabs
@using BlazorUI.Components.Select
@using BlazorUI.Components.Avatar
@using BlazorUI.Components.Badge
@using BlazorUI.Components.Card
@using BlazorUI.Components.Chart
@using BlazorUI.Components.DataTable
@using BlazorUI.Icons.Lucide.Components
```

Add imports for each component namespace you use. This gives access to component-specific enums like `ButtonVariant`, `InputType`, etc.

### 2. Add CSS to your `App.razor` or `index.html`:

BlazorUI Components come with pre-built CSS - no Tailwind setup required!

```html
<!DOCTYPE html>
<html lang="en" data-theme="default">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />

    <!-- Optional: Your custom theme (defines CSS variables) -->
    <link rel="stylesheet" href="styles/theme.css" />

    <!-- Pre-built BlazorUI styles (included in NuGet package) -->
    <link rel="stylesheet" href="_content/NeoBlazorUI.Components/blazorui.css" />

    <HeadOutlet @rendermode="InteractiveAuto" />
</head>
<body>
    <Routes @rendermode="InteractiveAuto" />
    <script src="_framework/blazor.web.js"></script>
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
            <DialogTitle>Welcome to NeoBlazorUI</DialogTitle>
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

## 📚 Available Components (75+)

### Form & Input Components
- **Button** - Interactive buttons with 6 variants and multiple sizes
- **Button Group** - Visually group related buttons
- **Checkbox** - Binary selection with indeterminate state
- **Combobox** - Autocomplete input with searchable dropdown
- **Date Picker** - Date selection with calendar popover
- **Field** - Form field wrapper with label, description, and errors
- **Input** - Text input fields with multiple types
- **Input Group** - Enhanced inputs with icons, buttons, and addons
- **Input OTP** - One-time password input with character slots
- **Label** - Accessible labels for form controls
- **Multi Select** - Searchable multi-selection with tags
- **Native Select** - Styled native HTML select
- **Radio Group** - Mutually exclusive options
- **Select** - Dropdown selection with groups
- **Slider** - Range input for numeric values
- **Switch** - Toggle control for on/off states
- **Textarea** - Multi-line text input
- **Time Picker** - Time selection with hour/minute controls

### Data Display Components
- **Avatar** - User profile images with fallbacks
- **Badge** - Labels for status and categories
- **Card** - Content container with header/footer
- **Data Table** - Advanced tables with sorting, filtering, pagination
- **Empty** - Empty state displays
- **Grid** - Advanced data grid with state management
- **Item** - Flexible list items with media and actions
- **Kbd** - Keyboard shortcut badges
- **Progress** - Progress bars with ARIA support
- **Separator** - Visual dividers
- **Skeleton** - Loading placeholders
- **Spinner** - Loading indicators
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
- **Chart** - 8 chart types with beautiful defaults (Line, Bar, Area, Pie, Doughnut, Radar, Polar Area, Bubble)
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
    <Grid />
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
           PageSize="10" />
```

### Grid Component
Enterprise-grade data grid powered by AG Grid with Blazor template support and auto-discovery actions, designed with shadcn theme with real-time light/dark theme switching support:

```razor
<Grid Items="@orders" ActionHost="this">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" Width="100px" />
        <GridColumn Field="CustomerName" Header="Customer" Sortable="true" />
        <GridColumn Field="OrderDate" Header="Date" DataFormatString="d" />
        <GridColumn Field="Total" Header="Total" DataFormatString="C" />
        <GridColumn Field="Status" Header="Status">
            <CellTemplate Context="order">
                <Badge Variant="@GetStatusVariant(order.Status)">
                    @order.Status
                </Badge>
            </CellTemplate>
        </GridColumn>
        <GridColumn Field="Actions" Header="">
            <CellTemplate Context="order">
                <Button data-action="Edit" Variant="ButtonVariant.Ghost">
                    Edit
                </Button>
            </CellTemplate>
        </GridColumn>
    </Columns>
</Grid>

@code {
    [GridAction]
    private async Task Edit(Order order)
    {
        // Action auto-wired via [GridAction] attribute
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

NeoBlazorUI uses CSS variables for theming, compatible with shadcn/ui themes. You can use any theme from:
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

These components are built on top of `NeoBlazorUI.Primitives` - a library of headless, unstyled components that provide behavior and accessibility without any styling. If you need complete control over styling, you can use the primitives directly.

## 📦 Related Packages

- **NeoBlazorUI.Primitives** - Headless, unstyled components (included)
- **NeoBlazorUI.Icons.Lucide** - 1,640+ Lucide icons (included)
- **NeoBlazorUI.Icons.Heroicons** - 1,288 Heroicons across 4 variants
- **NeoBlazorUI.Icons.Feather** - 286 beautiful minimalist icons

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

- **Current Version**: 1.0.14
- **Target Framework**: .NET 10
- **Package ID**: NeoBlazorUI.Components
- **Assembly Name**: NeoBlazorUI.Components

---

**Note**: This package was formerly known as `BlazorUI.Components`. As of version 1.0.7, the assembly name has been updated to `NeoBlazorUI.Components` to match the NuGet package ID, ensuring consistent asset paths when consumed from NuGet.
