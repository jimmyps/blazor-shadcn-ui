# NeoBlazorUI

A comprehensive UI component library for Blazor inspired by [shadcn/ui](https://ui.shadcn.com/).

## 🌟 Overview

NeoBlazorUI brings the beautiful design system of shadcn/ui to Blazor applications. This library provides **zero-config, plug-and-play UI components** with full shadcn/ui compatibility, featuring pre-built CSS, styled components, and headless primitives that work across all Blazor hosting models (Server, WebAssembly, and Hybrid).

**No Tailwind CSS setup required** - just install the NuGet package and start building!

**[🚀 Try the Live Demo](https://blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net)** - Explore all 85+ components with interactive examples

## 🚀 Getting Started

### 📦 Installation

Install NeoBlazorUI packages from NuGet:

```bash
# Headless primitives for custom styling
dotnet add package NeoBlazorUI.Primitives

# Styled components with shadcn/ui design
dotnet add package NeoBlazorUI.Components

# Icon libraries (choose one or more)
dotnet add package NeoBlazorUI.Icons.Lucide      # 1,640 icons - stroke-based, consistent
dotnet add package NeoBlazorUI.Icons.Heroicons   # 1,288 icons - 4 variants (outline, solid, mini, micro)
dotnet add package NeoBlazorUI.Icons.Feather     # 286 icons - minimalist, stroke-based
```

### Quick Start

1. **Add to your `_Imports.razor`:**

```razor
@using BlazorUI.Components
```

2. **Add PortalHost to your layout:**

   For overlay components (Dialog, Sheet, Popover, etc.) to work correctly, add portal hosts to your root layout:

```razor
@inherits LayoutComponentBase

<div class="min-h-screen bg-background">
    <!-- Your layout content -->
    @Body
</div>

@* RECOMMENDED: Use separate portal hosts for better performance *@
<ContainerPortalHost />
<OverlayPortalHost />

@* OR: Use legacy single host (backward compatible) *@
@* <PortalHost /> *@
```

3. **Add CSS to your `App.razor`:**

   NeoBlazorUI Components come with pre-built CSS - no Tailwind setup required!

```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <!-- Your theme CSS variables -->
    <link rel="stylesheet" href="styles/theme.css" />
    <!-- Pre-built NeoBlazorUI styles -->
    <link rel="stylesheet" href="_content/NeoBlazorUI.Components/blazorui.css" />
    <HeadOutlet @rendermode="InteractiveAuto" />
</head>
<body>
    <Routes @rendermode="InteractiveAuto" />
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

4. **Start using components:**

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
                Beautiful Blazor components inspired by shadcn/ui
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

**AsChild Pattern:** Use `AsChild` on trigger components to use your own styled elements (like Button) instead of the default button. This is the industry-standard pattern from Radix UI/shadcn/ui.

### Learn More

- **Contributing**: See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup and guidelines

## 🏗️ Architecture

NeoBlazorUI uses a **two-layer architecture** with modern .NET 10 features and Auto rendering mode:

### Project Structure

- **NeoBlazorUI.Primitives** - Headless components (runs on both Server and WebAssembly)
- **NeoBlazorUI.Components** - Pre-styled components (runs on both Server and WebAssembly)
- **BlazorUI.Demo** - Demo application (.NET 10, Auto rendering mode)
- **BlazorUI.Demo.Client** - WebAssembly-specific code (.NET 10, WASM)
- **BlazorUI.Demo.Shared** - Shared code between Server and WASM (.NET 10)

### Rendering Mode

All components support **.NET 10's Auto rendering mode**, which automatically switches between Server and WebAssembly based on availability:

```razor
<!-- App.razor -->
<HeadOutlet @rendermode="InteractiveAuto" />
<Routes @rendermode="InteractiveAuto" />
```

This provides:
- Fast initial load (Server-side rendering)
- Rich interactivity (WebAssembly after download)
- Seamless transition between modes
- Optimal performance for all scenarios

### Styled Components Layer (`NeoBlazorUI.Components`)
- Pre-styled components matching shadcn/ui design system
- **Pre-built CSS included** - no Tailwind configuration needed
- Built on top of primitives for consistency
- Ready to use out of the box
- Full theme support via CSS variables

### Primitives Layer (`NeoBlazorUI.Primitives`)
- Headless, unstyled components
- Complete accessibility implementation
- Keyboard navigation and ARIA support
- Maximum flexibility for custom styling

### Key Principles
- **Feature-based organization** - Each component in its own folder with all related files
- **Code-behind pattern** - Clean separation of markup (`.razor`) and logic (`.razor.cs`)
- **CSS Variables theming** - Runtime theme switching with light/dark mode support
- **Accessibility first** - WCAG 2.1 AA compliance with comprehensive keyboard navigation
- **Composition over inheritance** - Components designed to be composed together
- **Progressive enhancement** - Works without JavaScript where possible

## 🎨 Theming

NeoBlazorUI is **100% compatible with shadcn/ui themes**, making it easy to customize your application's appearance.

### Using Themes from shadcn/ui and tweakcn

You can use any theme from:
- **[shadcn/ui themes](https://ui.shadcn.com/themes)** - Official shadcn/ui theme gallery
- **[tweakcn.com](https://tweakcn.com)** - Advanced theme customization tool with live preview

Simply copy the CSS variables from these tools and paste them into your `wwwroot/styles/theme.css` file.

### Customizing Your Theme

1. **Create `wwwroot/styles/theme.css`** in your Blazor project

2. **Add your theme variables** inside the `:root` (light mode) and `.dark` (dark mode) selectors:

```css
@layer base {
  :root {
    --background: oklch(1 0 0);
    --foreground: oklch(0.1450 0 0);
    --primary: oklch(0.2050 0 0);
    --primary-foreground: oklch(0.9850 0 0);
    /* ... other variables */
  }

  .dark {
    --background: oklch(0.1450 0 0);
    --foreground: oklch(0.9850 0 0);
    --primary: oklch(0.9220 0 0);
    --primary-foreground: oklch(0.2050 0 0);
    /* ... other variables */
  }
}
```

3. **Reference it in your `App.razor`** before the NeoBlazorUI CSS:

```razor
<link rel="stylesheet" href="styles/theme.css" />
<link rel="stylesheet" href="_content/NeoBlazorUI.Components/blazorui.css" />
```

That's it! NeoBlazorUI will automatically use your theme variables.

### Available Theme Variables

NeoBlazorUI supports all standard shadcn/ui CSS variables:
- Colors: `--background`, `--foreground`, `--primary`, `--secondary`, `--accent`, `--destructive`, `--muted`, etc.
- Typography: `--font-sans`, `--font-serif`, `--font-mono`
- Layout: `--radius` (border radius), `--shadow-*` (shadows)
- Charts: `--chart-1` through `--chart-5`
- Sidebar: `--sidebar`, `--sidebar-primary`, `--sidebar-accent`, etc.

### Dark Mode

NeoBlazorUI automatically supports dark mode by applying the `.dark` class to the `<html>` element. All components will automatically switch to dark mode colors when this class is present.

## 💅 Styling

### NeoBlazorUI.Components (Pre-styled)

**No Tailwind CSS setup required!** NeoBlazorUI Components include pre-built, production-ready CSS that ships with the NuGet package.

Simply add two CSS files to your `App.razor`:

```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />

    <!-- 1. Your custom theme (defines CSS variables) -->
    <link rel="stylesheet" href="styles/theme.css" />

    <!-- 2. Pre-built NeoBlazorUI styles (included in NuGet package) -->
    <link rel="stylesheet" href="_content/NeoBlazorUI.Components/blazorui.css" />

    <HeadOutlet @rendermode="InteractiveAuto" />
</head>
<body>
    <Routes @rendermode="InteractiveAuto" />
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

**Important:** Load your theme CSS **before** `blazorui.css` so the CSS variables are defined when NeoBlazorUI references them.

**Note:** The pre-built CSS is already minified and optimized. You don't need to install Tailwind CSS, configure build processes, or set up any additional tooling.

### NeoBlazorUI.Primitives (Headless)

Primitives are completely **headless** - they provide behavior and accessibility without any styling. You have complete freedom to style them however you want:

**Option 1: Tailwind CSS** (requires your own Tailwind setup)
```razor
<NeoBlazorUI.Primitives.Accordion.Accordion class="space-y-4">
    <NeoBlazorUI.Primitives.Accordion.AccordionItem class="border rounded-lg">
        <!-- Your custom Tailwind classes -->
    </NeoBlazorUI.Primitives.Accordion.AccordionItem>
</NeoBlazorUI.Primitives.Accordion.Accordion>
```

**Option 2: CSS Modules / Vanilla CSS**
```razor
<NeoBlazorUI.Primitives.Accordion.Accordion class="my-accordion">
    <!-- Style with your own CSS -->
</NeoBlazorUI.Primitives.Accordion.Accordion>
```

**Option 3: Inline Styles**
```razor
<NeoBlazorUI.Primitives.Accordion.Accordion style="margin: 1rem;">
    <!-- Direct inline styling -->
</NeoBlazorUI.Primitives.Accordion.Accordion>
```

Primitives give you complete control over styling while handling all the complex behavior, accessibility, and keyboard navigation for you. Unlike `NeoBlazorUI.Components`, primitives don't include any CSS - you bring your own styling approach.

## 📚 Components

NeoBlazorUI includes **85+ styled components** with full shadcn/ui design compatibility:

### Form Components
- **Button** - Multiple variants (default, destructive, outline, secondary, ghost, link) with icon support
- **Button Group** - Visually grouped related buttons with connected styling
- **Checkbox** - Accessible checkbox with indeterminate state
- **Color Picker** - Color selection with hex, RGB, and HSL support
- **Combobox** - Searchable autocomplete dropdown
- **Currency Input** - Formatted currency input with locale support
- **Field** - Combine labels, controls, and help text for accessible forms
- **Input** - Text input with multiple types and validation support
- **Input Group** - Enhanced inputs with icons, buttons, and addons
- **Input OTP** - One-time password input with individual character slots
- **Label** - Accessible form labels
- **Link Button** - Semantic links styled as buttons for navigation
- **Masked Input** - Text input with customizable format masks (phone, date, etc.)
- **Multi Select** - Searchable multi-selection with tags and checkboxes
- **Native Select** - Styled native HTML select dropdown with chevron icon
- **Numeric Input** - Number input with increment/decrement buttons and formatting
- **Radio Group** - Radio button groups with keyboard navigation
- **Range Slider** - Dual-handle slider for selecting value ranges
- **Rating** - Star rating input with half-star precision and readonly mode
- **Select** - Dropdown select with search and keyboard navigation
- **Slider** - Range input for numeric value selection
- **Switch** - Toggle switch component
- **Textarea** - Multi-line text input with automatic content sizing

### Date & Time
- **Calendar** - Date selection grid with month navigation
- **Date Picker** - Date selection with calendar in popover
- **Date Range Picker** - Date range selection with presets and two-calendar view
- **Time Picker** - Time selection with 12/24-hour format support

### Layout & Navigation
- **Accordion** - Collapsible content sections
- **Aspect Ratio** - Display content within a desired width/height ratio
- **Breadcrumb** - Hierarchical navigation with customizable separators
- **Carousel** - Slideshow component with touch gestures and animations
- **Card** - Container for grouped content with header and footer
- **Collapsible** - Expandable/collapsible panels
- **Navigation Menu** - Horizontal navigation with dropdown panels
- **Pagination** - Page navigation with Previous/Next/Ellipsis support
- **Resizable** - Split layouts with draggable handles
- **Scroll Area** - Custom scrollbars for styled scroll regions
- **Item** - Flexible list items with media, content, and actions
- **Separator** - Visual dividers
- **Sidebar** - Responsive sidebar with collapsible icon mode, variants (default, floating, inset), and mobile sheet integration
- **Tabs** - Tabbed interfaces with controlled/uncontrolled modes

### Overlay Components
- **Alert Dialog** - Modal for critical confirmations
- **Context Menu** - Right-click menus with actions and shortcuts
- **Command** - Command palette with keyboard navigation
- **Dialog** - Modal dialogs
- **Dialog Service** - Programmatic dialogs with async/await API for alerts and confirmations
- **Drawer** - Slide-out panel with gesture controls and backdrop
- **Dropdown Menu** - Context menus with nested submenus
- **Hover Card** - Rich hover previews
- **Menubar** - Desktop application-style horizontal menu bar
- **Popover** - Floating content containers
- **Sheet** - Slide-out panels (top, right, bottom, left)
- **Toast** - Temporary notifications with variants and actions
- **Tooltip** - Contextual hover tooltips

### Data & Content
- **DataTable** - Powerful tables with sorting, filtering, pagination, and selection
- **MarkdownEditor** - Rich text editor with toolbar formatting and live preview
- **RichTextEditor** - WYSIWYG editor with formatting toolbar and HTML output

### Display Components
- **Alert** - Status messages and callouts
- **Avatar** - User avatars with fallback support
- **Badge** - Status badges and labels
- **Card** - Container for grouped content with header and footer
- **Command** - Command palette with keyboard navigation
- **Data Table** - Powerful tables with sorting, filtering, pagination, and selection
- **Empty** - Empty state displays
- **Item** - Flexible list items with media, content, and actions
- **Kbd** - Keyboard shortcut badges
- **Progress** - Progress bars with animations
- **Skeleton** - Loading placeholders
- **Spinner** - Loading indicators
- **Toggle** - Pressable toggle buttons
- **Toggle Group** - Single/multiple selection toggle groups
- **Typography** - Semantic text styling

### Data Visualization
- **Chart** - Beautiful data visualizations with multiple chart types:
  - **Area Chart** - Stacked and gradient area charts
  - **Bar Chart** - Vertical and horizontal bar charts (grouped/stacked)
  - **Composed Chart** - Mix multiple chart types (line + bar + area)
  - **Line Chart** - Single and multi-series line charts
  - **Pie Chart** - Pie and donut charts with labels
  - **Radar Chart** - Multi-axis radar/spider charts
  - **Radial Bar Chart** - Circular progress and gauge charts
  - **Scatter Chart** - X/Y coordinate plotting

### Animation
- **Motion** - Declarative animation system powered by Motion.dev with 20+ presets including fade, scale, slide, shake, bounce, pulse, spring physics, scroll-triggered animations, and staggered list/grid animations

### 🎭 Icons

NeoBlazorUI offers **three icon library packages** to suit different design preferences:

- **Lucide Icons** (`NeoBlazorUI.Icons.Lucide`) - 1,640 beautiful, consistent stroke-based icons
  - ISC licensed
  - 24x24 viewBox, 2px stroke width
  - Perfect for: Modern, clean interfaces

- **Heroicons** (`NeoBlazorUI.Icons.Heroicons`) - 1,288 icons across 4 variants
  - MIT licensed by Tailwind Labs
  - Variants: Outline (24x24), Solid (24x24), Mini (20x20), Micro (16x16)
  - Perfect for: Tailwind-based designs, flexible sizing needs

- **Feather Icons** (`NeoBlazorUI.Icons.Feather`) - 286 minimalist stroke-based icons
  - MIT licensed
  - 24x24 viewBox, 2px stroke width
  - Perfect for: Simple, lightweight projects

## 🔧 Primitives

NeoBlazorUI also includes **15 headless primitive components** for building custom UI:

- Accordion Primitive
- Checkbox Primitive
- Collapsible Primitive
- Dialog Primitive
- Dropdown Menu Primitive
- Hover Card Primitive
- Label Primitive
- Popover Primitive
- Radio Group Primitive
- Select Primitive
- Sheet Primitive
- Switch Primitive
- Table Primitive
- Tabs Primitive
- Tooltip Primitive

All primitives are fully accessible, keyboard-navigable, and provide complete control over styling.

## ✨ Features

- **Full shadcn/ui Compatibility** - Drop-in Blazor equivalents of shadcn/ui components
- **Zero Configuration** - Pre-built CSS included, no Tailwind setup required
- **🎯 .NET 10 Ready** - Built for the latest .NET platform with Auto rendering mode
- **Auto Rendering Mode** - Seamless transition between Server and WebAssembly rendering
- **🌙 Dark Mode Support** - Built-in light/dark theme switching with CSS variables
- **📱 Responsive Design** - Mobile-first components that adapt to all screen sizes
- **♿ Accessibility First** - WCAG 2.1 AA compliant with keyboard navigation and ARIA attributes
- **⌨️ Keyboard Shortcuts** - Native keyboard navigation support (e.g., Ctrl/Cmd+B for sidebar toggle)
- **🔄 State Persistence** - Cookie-based state management for user preferences
- **TypeScript-Inspired API** - Familiar API design for developers coming from React/shadcn/ui
- **Pure Blazor** - No JavaScript dependencies, no Node.js required
- **🎨 Icon Library Options** - 3 separate icon packages (Lucide, Heroicons, Feather) with 3,200+ total icons
- **Form Validation Ready** - Works seamlessly with Blazor's form validation

## 📄 License

NeoBlazorUI is open source software licensed under the [MIT License](LICENSE).

## 🙏 Acknowledgments

NeoBlazorUI is inspired by [shadcn/ui](https://ui.shadcn.com/) and based on the design principles of [Radix UI](https://www.radix-ui.com/).

While NeoBlazorUI is a complete reimplementation for Blazor/C# and contains no code from these projects, we are grateful for their excellent work which inspired this library.

- shadcn/ui: MIT License - Copyright (c) 2023 shadcn
- Radix UI: MIT License - Copyright (c) 2022-present WorkOS

NeoBlazorUI is an independent project and is not affiliated with or endorsed by shadcn or Radix UI.

**Additional Acknowledgments:**
- Initial Blazor components inspiration by [Mathew Taylor](https://github.com/blazorui-net/ui)
- [Tailwind CSS](https://tailwindcss.com/) - Utility-first CSS framework
- [Lucide Icons](https://lucide.dev/) - Beautiful stroke-based icon library (ISC License)
- [Heroicons](https://heroicons.com/) - Icon library by Tailwind Labs (MIT License)
- [Feather Icons](https://feathericons.com/) - Minimalist icon library (MIT License)
- Built with ❤️ for the Blazor community

## 📊 Version Information

- **Current Version**: 1.0.15
- **Target Framework**: .NET 10
- **Package IDs**: 
  - `NeoBlazorUI.Components`
  - `NeoBlazorUI.Primitives`
  - `NeoBlazorUI.Icons.Lucide`
  - `NeoBlazorUI.Icons.Heroicons`
  - `NeoBlazorUI.Icons.Feather`

---

**Note**: These packages were formerly known as `BlazorUI.*`. As of version 1.0.7, the assembly names have been updated to `NeoBlazorUI.*` to match the NuGet package IDs, ensuring consistent asset paths when consumed from NuGet.

