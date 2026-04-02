# NeoUI Theming Guide

NeoUI uses CSS custom properties (variables) for theming, following the [shadcn/ui](https://ui.shadcn.com) design system. This allows complete customization of colors, typography, and sizing without modifying component code.

> [!NOTE]
> For pre-built themes shipped with NeoUI and the latest updates, visit **[neoui.io/docs/theming](https://neoui.io/docs/theming)**.

## Quick Start

1. Create a theme CSS file (e.g., `wwwroot/styles/themes/my-theme.css`)
2. Define CSS variables for `:root` (light mode) and `.dark` (dark mode)
3. Load your theme **before** `components.css`:

```html
<link href="styles/themes/my-theme.css" rel="stylesheet" />
<link href="_content/NeoUI.Blazor/components.css" rel="stylesheet" />
```

## Color Format

NeoUI uses the [OKLCH color space](https://developer.mozilla.org/en-US/docs/Web/CSS/color_value/oklch) for perceptually uniform colors:

```css
--primary: oklch(0.205 0 0);
/*              │     │ │
                │     │ └── Hue (0-360)
                │     └──── Chroma (0-0.4, saturation)
                └────────── Lightness (0-1) */
```

You can also use any valid CSS color format (hex, rgb, hsl).

---

## Theme Variables Reference

### Core Colors

These are the foundational colors used throughout the library.

| Variable | Description | Used By |
|----------|-------------|---------|
| `--background` | Page/app background | Body, containers |
| `--foreground` | Primary text color | Body text, headings |

```css
:root {
  --background: oklch(1 0 0);           /* White */
  --foreground: oklch(0.145 0 0);       /* Near black */
}

.dark {
  --background: oklch(0.145 0 0);       /* Near black */
  --foreground: oklch(0.985 0 0);       /* Near white */
}
```

---

### Semantic Colors

Colors with specific meaning, each with a foreground variant for text/icons.

| Variable | Description | Used By |
|----------|-------------|---------|
| `--primary` | Primary brand color | Primary buttons, links, focus rings |
| `--primary-foreground` | Text on primary backgrounds | Button text |
| `--secondary` | Secondary/subtle actions | Secondary buttons, badges |
| `--secondary-foreground` | Text on secondary backgrounds | |
| `--destructive` | Dangerous/delete actions | Delete buttons, error states |
| `--destructive-foreground` | Text on destructive backgrounds | |
| `--muted` | Subdued backgrounds | Disabled states, placeholders |
| `--muted-foreground` | Subdued text | Helper text, captions |
| `--accent` | Hover/focus highlights | Menu item hover, keyboard focus |
| `--accent-foreground` | Text on accent backgrounds | |

```css
:root {
  --primary: oklch(0.205 0 0);
  --primary-foreground: oklch(0.985 0 0);

  --secondary: oklch(0.97 0 0);
  --secondary-foreground: oklch(0.205 0 0);

  --destructive: oklch(0.577 0.245 27.325);
  --destructive-foreground: oklch(1 0 0);

  --muted: oklch(0.97 0 0);
  --muted-foreground: oklch(0.556 0 0);

  --accent: oklch(0.97 0 0);
  --accent-foreground: oklch(0.205 0 0);
}
```

---

### UI Element Colors

Colors for specific UI elements.

| Variable | Description | Used By |
|----------|-------------|---------|
| `--border` | Default border color | Cards, inputs, dividers |
| `--input` | Input field borders | Input, Select, Textarea |
| `--ring` | Focus ring color | Focus outlines, keyboard navigation |

```css
:root {
  --border: oklch(0.922 0 0);
  --input: oklch(0.922 0 0);
  --ring: oklch(0.708 0 0);
}
```

---

### Component Colors

Colors for specific component types.

#### Card

| Variable | Description |
|----------|-------------|
| `--card` | Card background |
| `--card-foreground` | Card text color |

```css
:root {
  --card: oklch(1 0 0);
  --card-foreground: oklch(0.145 0 0);
}
```

#### Popover

Used by Popover, DropdownMenu, Tooltip, Select, Combobox, and other floating elements.

| Variable | Description |
|----------|-------------|
| `--popover` | Popover background |
| `--popover-foreground` | Popover text color |

```css
:root {
  --popover: oklch(1 0 0);
  --popover-foreground: oklch(0.145 0 0);
}
```

---

### Alert Colors

Colors for the Alert component variants. **Library provides defaults** - override only if needed.

| Variable | Description |
|----------|-------------|
| `--alert-success` | Success icon/border color |
| `--alert-success-foreground` | Success text color |
| `--alert-success-bg` | Success background |
| `--alert-info` | Info icon/border color |
| `--alert-info-foreground` | Info text color |
| `--alert-info-bg` | Info background |
| `--alert-warning` | Warning icon/border color |
| `--alert-warning-foreground` | Warning text color |
| `--alert-warning-bg` | Warning background |
| `--alert-danger` | Danger icon/border color |
| `--alert-danger-foreground` | Danger text color |
| `--alert-danger-bg` | Danger background |

```css
:root {
  /* Success - Green */
  --alert-success: oklch(0.55 0.20 142);
  --alert-success-foreground: oklch(0.30 0.09 142);
  --alert-success-bg: oklch(0.993 0.003 142);

  /* Info - Blue */
  --alert-info: oklch(0.50 0.20 255);
  --alert-info-foreground: oklch(0.30 0.10 255);
  --alert-info-bg: oklch(0.993 0.003 255);

  /* Warning - Amber */
  --alert-warning: oklch(0.68 0.18 55);
  --alert-warning-foreground: oklch(0.35 0.10 55);
  --alert-warning-bg: oklch(0.995 0.003 55);

  /* Danger - Red */
  --alert-danger: oklch(0.55 0.22 27);
  --alert-danger-foreground: oklch(0.30 0.12 27);
  --alert-danger-bg: oklch(0.993 0.003 27);
}
```

---

### Sidebar Colors

Colors specifically for the Sidebar component.

| Variable | Description |
|----------|-------------|
| `--sidebar-background` | Sidebar background |
| `--sidebar-foreground` | Sidebar text |
| `--sidebar-primary` | Active/selected item |
| `--sidebar-primary-foreground` | Active item text |
| `--sidebar-accent` | Hover state |
| `--sidebar-accent-foreground` | Hover state text |
| `--sidebar-border` | Sidebar borders |
| `--sidebar-ring` | Focus ring in sidebar |

```css
:root {
  --sidebar-background: oklch(0.985 0 0);
  --sidebar-foreground: oklch(0.145 0 0);
  --sidebar-primary: oklch(0.205 0 0);
  --sidebar-primary-foreground: oklch(0.985 0 0);
  --sidebar-accent: oklch(0.97 0 0);
  --sidebar-accent-foreground: oklch(0.205 0 0);
  --sidebar-border: oklch(0.922 0 0);
  --sidebar-ring: oklch(0.708 0 0);
}
```

---

### Chart Colors

Colors for data visualization components.

| Variable | Description |
|----------|-------------|
| `--chart-1` | First chart color |
| `--chart-2` | Second chart color |
| `--chart-3` | Third chart color |
| `--chart-4` | Fourth chart color |
| `--chart-5` | Fifth chart color |

```css
:root {
  --chart-1: oklch(0.81 0.1 252);
  --chart-2: oklch(0.62 0.19 260);
  --chart-3: oklch(0.55 0.22 263);
  --chart-4: oklch(0.49 0.22 264);
  --chart-5: oklch(0.42 0.18 266);
}
```

---

### Sizing & Layout

#### Border Radius

NeoUI v2 uses a **7-step proportional scale** driven by a single `--radius` base variable. All steps scale to zero when `--radius: 0rem`, so the Sharp/Lyra style works correctly.

| Variable | Multiplier | Example at default (0.625rem) |
|---|---|---|
| `--radius-xs` | `× 0.5` | `0.3125rem` |
| `--radius-sm` | `× 0.75` | `0.469rem` |
| `--radius-md` | `× 1` (base) | `0.625rem` |
| `--radius-lg` | `× 1.5` | `0.9375rem` |
| `--radius-xl` | `× 2` | `1.25rem` |
| `--radius-2xl` | `× 3` | `1.875rem` |
| `--radius-4xl` | `9999px` | full pill |

The base `--radius` is set to `0.625rem` by default (aligning with shadcn/ui).

Override for the whole app:

```css
:root {
  --radius: 0.375rem;   /* Subtle — "Nova" look */
  --radius: 1rem;       /* Generous — "Maia" look */
  --radius: 0rem;       /* None — "Lyra" look */
}
```

Or use a **named radius preset** (see [Radius Presets](#radius-presets) below).

#### Sidebar Dimensions

**Library provides defaults** - override only if needed.

| Variable | Description | Default |
|---|---|---|
| `--sidebar-width` | Expanded sidebar width | `16rem` |
| `--sidebar-width-mobile` | Mobile sidebar width | `18rem` |
| `--sidebar-width-icon` | Collapsed (icon-only) width | `3rem` |

```css
:root {
  --sidebar-width: 16rem;
  --sidebar-width-mobile: 18rem;
  --sidebar-width-icon: 3rem;
}
```

---

### Typography

| Variable | Description | Default |
|---|---|---|
| `--font-sans` | Body sans-serif font stack | system UI stack |
| `--font-serif` | Serif font stack | system UI stack |
| `--font-mono` | Monospace font stack | system UI stack |
| `--font-heading` | Heading font (h1–h4) | falls back to `--font-sans` |

```css
:root {
  --font-sans:    'Inter', ui-sans-serif, system-ui, sans-serif;
  --font-heading: 'Cal Sans', 'Inter', ui-sans-serif, sans-serif; /* distinct heading */
  --font-mono:    'JetBrains Mono', ui-monospace, monospace;
}
```

Or use a **named font preset** (see [Font Presets](#font-presets) below).

---

## Theme v2 — Dimensions & Presets

NeoUI v2 adds five new theme dimensions on top of the existing base color, primary color, and dark mode. Every dimension follows the same pattern end-to-end:

```
CSS file(s) → enum → ThemeService → JS apply → localStorage → ThemeSwitcher
```

---

### Base Colors (v2 additions)

Five new chromatic neutral base colors added in v2:

| Name | Character | `BaseColor` enum value |
|---|---|---|
| `Luma` | Vibrant blue-indigo tinted neutral — flagship modern SaaS look | `BaseColor.Luma` |
| `Mist` | Cool blue-gray | `BaseColor.Mist` |
| `Mauve` | Warm purple-gray | `BaseColor.Mauve` |
| `Taupe` | Warm brownish-gray | `BaseColor.Taupe` |
| `Olive` | Muted green-gray | `BaseColor.Olive` |

Load the CSS file for any base color you want available:

```html
<!-- In production, include only the base colors you use -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/luma.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/mist.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/mauve.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/taupe.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/olive.css" />
```

Apply in C#:

```csharp
await ThemeService.SetBaseColorAsync(BaseColor.Luma);
```

---

### Style Variants

Six named visual styles control `--radius` and `--spacing-scale` together, giving each a distinct character:

| Style | `--radius` | `--spacing-scale` | Character |
|---|---|---|---|
| `Default` | *(unchanged)* | `1` | Standard NeoUI |
| `Vega` | `0.625rem` | `1` | Professional, balanced |
| `Nova` | `0.375rem` | `0.85` | Compact, dashboard/admin |
| `Maia` | `1rem` | `1.15` | Spacious, consumer-friendly |
| `Lyra` | `0rem` | `1` | Sharp/boxy, developer tooling |
| `Mira` | `0.25rem` | `0.7` | Ultra-dense, data-heavy |

Load the style CSS files you want available:

```html
<!-- In production, include only the styles you use -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/vega.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/nova.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/maia.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/lyra.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/mira.css" />
```

Apply in C#:

```csharp
await ThemeService.SetStyleVariantAsync(StyleVariant.Nova);
```

---

### Radius Presets

Independent named radius overrides. When both a Style Variant and a Radius Preset are active, the Radius Preset wins (it is loaded after styles in the cascade).

| Preset | `--radius` | `RadiusPreset` value |
|---|---|---|
| `None` | `0rem` | `RadiusPreset.None` |
| `Small` | `0.25rem` | `RadiusPreset.Small` |
| `Medium` | `0.625rem` (default — no file) | `RadiusPreset.Medium` |
| `Large` | `1rem` | `RadiusPreset.Large` |
| `Full` | `9999px` (pill) | `RadiusPreset.Full` |

```html
<!-- In production, include only the radius presets you use -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/none.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/small.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/large.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/full.css" />
```

Apply in C#:

```csharp
await ThemeService.SetRadiusPresetAsync(RadiusPreset.Full);
```

---

### Font Presets

Curated font pairings that set both `--font-sans` and `--font-heading`. The `System` preset is the default (no CSS file needed).

| Preset | Body | Heading | `FontPreset` value |
|---|---|---|---|
| `System` | System UI stack | Same as body | `FontPreset.System` |
| `Inter` | Inter | Inter | `FontPreset.Inter` |
| `Geist` | Geist | Geist | `FontPreset.Geist` |
| `CalSans` | Inter | Cal Sans | `FontPreset.CalSans` |
| `DmSans` | DM Sans | DM Sans | `FontPreset.DmSans` |
| `PlusJakarta` | Plus Jakarta Sans | Plus Jakarta Sans | `FontPreset.PlusJakarta` |

> **Note:** Font preset CSS files set `--font-sans` and `--font-heading` only. You must load the actual font faces separately (via Google Fonts, Bunny Fonts, or self-hosted).

```html
<!-- 1. Load the font face (example using Google Fonts) -->
<link rel="preconnect" href="https://fonts.googleapis.com" />
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&display=swap" rel="stylesheet" />

<!-- 2. Load the NeoUI font preset CSS -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/fonts/inter.css" />
```

Apply in C#:

```csharp
await ThemeService.SetFontPresetAsync(FontPreset.Inter);
```

---

### ThemePreset — Named Presets

`ThemePreset` is a portable C# record that bundles all six theme dimensions into a single named unit. Apply it with `ApplyPresetAsync`.

**Built-in presets:**

| Preset | Base | Style | Radius | Font |
|---|---|---|---|---|
| `ThemePreset.Default` | Zinc | Default | Medium | System |
| `ThemePreset.Luma` | Luma | Vega | Medium | Inter |
| `ThemePreset.Nova` | Zinc | Nova | Small | Inter |
| `ThemePreset.Maia` | Mauve | Maia | Large | Plus Jakarta |
| `ThemePreset.Lyra` | Slate | Lyra | None | System |

Usage:

```csharp
// Apply a built-in preset
await ThemeService.ApplyPresetAsync(ThemePreset.Luma);

// Build a custom preset
var corporate = new ThemePreset(
    Name:         "Corporate",
    BaseColor:    BaseColor.Slate,
    PrimaryColor: PrimaryColor.Blue,
    StyleVariant: StyleVariant.Nova,
    RadiusPreset: RadiusPreset.Small,
    FontPreset:   FontPreset.Inter,
    IsDarkMode:   false);

await ThemeService.ApplyPresetAsync(corporate);
```

---

### CSS Load Order

Theme CSS files must be loaded in the following order so the cascade overrides work correctly:

```html
<!-- 1. Core component styles -->
<link href="_content/NeoUI.Blazor/components.css" rel="stylesheet" />

<!-- 2. Base color themes -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/base/zinc.css" />
<!-- ... other base colors ... -->

<!-- 3. Primary color themes -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/blue.css" />
<!-- ... other primary colors ... -->

<!-- 4. Visual style variants (sets --radius + --spacing-scale) -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/nova.css" />

<!-- 5. Radius presets (overrides style variant's --radius if present) -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/radius/small.css" />

<!-- 6. Font presets -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/fonts/inter.css" />

<!-- 7. Theme JS — must come before Blazor boots to prevent FOUC -->
<script src="_content/NeoUI.Blazor/js/theme.js"></script>
```

---

## Complete Theme Example

Here's a complete theme file with all variables:

```css
/* my-theme.css */

:root {
  /* Core */
  --background: oklch(1 0 0);
  --foreground: oklch(0.145 0 0);

  /* Semantic */
  --primary: oklch(0.205 0 0);
  --primary-foreground: oklch(0.985 0 0);
  --secondary: oklch(0.97 0 0);
  --secondary-foreground: oklch(0.205 0 0);
  --destructive: oklch(0.577 0.245 27.325);
  --destructive-foreground: oklch(1 0 0);
  --muted: oklch(0.97 0 0);
  --muted-foreground: oklch(0.556 0 0);
  --accent: oklch(0.97 0 0);
  --accent-foreground: oklch(0.205 0 0);

  /* UI Elements */
  --border: oklch(0.922 0 0);
  --input: oklch(0.922 0 0);
  --ring: oklch(0.708 0 0);

  /* Components */
  --card: oklch(1 0 0);
  --card-foreground: oklch(0.145 0 0);
  --popover: oklch(1 0 0);
  --popover-foreground: oklch(0.145 0 0);

  /* Sizing */
  --radius: 0.625rem;
}

.dark {
  /* Core */
  --background: oklch(0.145 0 0);
  --foreground: oklch(0.985 0 0);

  /* Semantic */
  --primary: oklch(0.985 0 0);
  --primary-foreground: oklch(0.205 0 0);
  --secondary: oklch(0.269 0 0);
  --secondary-foreground: oklch(0.985 0 0);
  --destructive: oklch(0.396 0.141 25.723);
  --destructive-foreground: oklch(0.985 0 0);
  --muted: oklch(0.269 0 0);
  --muted-foreground: oklch(0.708 0 0);
  --accent: oklch(0.269 0 0);
  --accent-foreground: oklch(0.985 0 0);

  /* UI Elements */
  --border: oklch(0.269 0 0);
  --input: oklch(0.269 0 0);
  --ring: oklch(0.556 0 0);

  /* Components */
  --card: oklch(0.145 0 0);
  --card-foreground: oklch(0.985 0 0);
  --popover: oklch(0.145 0 0);
  --popover-foreground: oklch(0.985 0 0);
}
```

---

## Dark Mode

NeoUI uses class-based dark mode. Add the `dark` class to the `<html>` element to enable dark mode:

```html
<html class="dark">
```

Toggle with JavaScript:
```javascript
document.documentElement.classList.toggle('dark');
```

Or with Blazor:
```csharp
await JS.InvokeVoidAsync("eval", "document.documentElement.classList.toggle('dark')");
```

---

## CSS Layer Priority

Theme variables defined outside `@layer` always override library defaults:

```
Priority (highest to lowest):
1. Your theme file (no @layer)     ← Your customizations win
2. @layer utilities
3. @layer components
4. @layer base                      ← Library defaults
```

This means you only need to define the variables you want to customize.

---

## Migrating from HSL

If you have an existing shadcn/ui theme using HSL format:

```css
/* Old HSL format */
--primary: 222.2 47.4% 11.2%;

/* New OKLCH format */
--primary: oklch(0.205 0.02 250);
```

Use a color converter tool or keep using HSL - both formats work:

```css
--primary: hsl(222.2 47.4% 11.2%);
```
