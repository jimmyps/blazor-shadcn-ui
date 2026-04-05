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

> **Note — Two things named "Luma":** `BaseColor.Luma` is a chromatic neutral palette (blue-indigo tint). `StyleVariant.Luma` is a separate glassmorphism visual style. They are independent and can be mixed freely. The built-in `ThemePreset.Luma` combines both.

---

### Primary Colors

17 named primary accent colors replace `--primary` / `--primary-foreground` system-wide:

`Red` · `Rose` · `Orange` · `Amber` · `Yellow` · `Lime` · `Green` · `Emerald` · `Teal` · `Cyan` · `Sky` · `Blue` · `Indigo` · `Violet` · `Purple` · `Fuchsia` · `Pink`

The `Default` primary color (matching each base color's neutral) requires no CSS file.

```html
<!-- In production, include only the primary colors you use -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/blue.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/primary/violet.css" />
```

```csharp
await ThemeService.SetPrimaryColorAsync(PrimaryColor.Blue);
```

---

### Style Variants

Seven named visual styles control `--radius`, `--spacing-scale`, and shadow values together, giving each a distinct character:

| Style | `--radius` | `--spacing-scale` | Character |
|---|---|---|---|
| `Default` | *(unchanged)* | `1` | **Backward-compatible** — preserves pre-v2 radius ratios |
| `Vega` | `0.625rem` | `1` | Professional, balanced |
| `Nova` | `0.375rem` | `0.85` | Compact, dashboard/admin |
| `Maia` | `1rem` | `1.15` | Spacious, consumer-friendly |
| `Lyra` | `0rem` | `1` | Sharp/boxy, developer tooling |
| `Mira` | `0.25rem` | `0.7` | Ultra-dense, data-heavy |
| `Luma` | `0.75rem` | `1` | Glassmorphism, modern SaaS |

The `Default` style uses the pre-v2 pixel-subtraction radius scale (`calc(var(--radius) - 4px)`, etc.) rather than the new proportional scale. Apps that don't set a style variant remain on `Default` automatically — **no visual change after upgrading**.

Each non-default style also ships with **custom shadow values tuned to its persona**:

| Style | Shadow character |
|---|---|
| `Default` | Tailwind defaults (unchanged) |
| `Vega` | Tailwind defaults — balanced for professional use |
| `Nova` | Crisp, tight shadows — suits compact admin interfaces |
| `Maia` | Elevated, deeper shadows — adds depth to the spacious rounded aesthetic |
| `Lyra` | All shadows `none` — flat, shadowless for sharp tooling UIs |
| `Mira` | Minimal single-layer shadows — ultra-subtle for dense data views |
| `Luma` | Soft, diffuse double-layer shadows — glassmorphism aesthetic |

The `--spacing-scale` variable multiplies internal component padding, gaps, and margins. You can override it directly in your theme CSS if you want a custom density independent of any preset:

```css
:root { --spacing-scale: 0.9; }  /* slightly tighter than default */
```

Load the style CSS files you want available:

```html
<!-- In production, include only the styles you use -->
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/vega.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/nova.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/maia.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/lyra.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/mira.css" />
<link rel="stylesheet" href="_content/NeoUI.Blazor/css/themes/styles/luma.css" />
```

Apply in C#:

```csharp
await ThemeService.SetStyleVariantAsync(StyleVariant.Nova);
```

#### Luma Style — Extra Overrides

The `Luma` style goes beyond `--radius` + `--spacing-scale`. It adds three additional glassmorphism touches:

**1. Soft diffuse shadow scale** — replaces Tailwind's default sharp shadows with multi-layer, low-opacity OKLCH shadows suited for glass surfaces:

| Variable | Value |
|---|---|
| `--shadow-sm` | `0 2px 8px oklch(0 0 0/0.06), 0 1px 2px oklch(0 0 0/0.04)` |
| `--shadow-md` | `0 4px 16px oklch(0 0 0/0.08), 0 1px 4px oklch(0 0 0/0.04)` |
| `--shadow-lg` | `0 8px 24px oklch(0 0 0/0.10), 0 2px 8px oklch(0 0 0/0.05)` |
| `--shadow-xl` | `0 16px 40px oklch(0 0 0/0.12), 0 4px 12px oklch(0 0 0/0.06)` |

**2. Semi-transparent form inputs** — softens input/textarea borders and fills:
```css
input, textarea {
  background-color: color-mix(in oklch, var(--background) 60%, transparent);
  border-color:     color-mix(in oklch, var(--border) 70%, transparent);
}
```

**3. Stronger overlay blur** — increases modal/sheet backdrop blur from the default thin tint to 4px:
```css
[data-slot="overlay"] { --blur-sm: 4px; }
```

Applies to Dialog, AlertDialog, Sheet, and Drawer overlays.

---

### Radius Presets

Independent named radius overrides. When both a Style Variant and a Radius Preset are active, the Radius Preset wins (it is loaded after styles in the cascade).

| Preset | `--radius` | `RadiusPreset` value |
|---|---|---|
| `None` | `0rem` | `RadiusPreset.None` |
| `Small` | `0.45rem` | `RadiusPreset.Small` |
| `Medium` | `0.625rem` (default — no file) | `RadiusPreset.Medium` |
| `Large` | `0.875rem` | `RadiusPreset.Large` |
| `Full` | `calc(infinity * 1px)` (pill) | `RadiusPreset.Full` |

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

### Menu Accent

Controls the intensity of hover and active states on menu items (DropdownMenu, Select, Combobox, Popover).

| Value | `--accent` remapped to | Effect |
|---|---|---|
| `Subtle` | `--accent` (default) | Soft, low-contrast hover |
| `Bold` | `--primary` | High-contrast hover — primary brand color |

```csharp
await ThemeService.SetMenuAccentAsync(MenuAccent.Bold);
```

`Bold` works by remapping `--accent → --primary` and `--accent-foreground → --primary-foreground` at the root level. All components that use `bg-accent` / `text-accent-foreground` for hover states respond automatically. This is a good pairing with a vibrant primary color (e.g. `PrimaryColor.Blue`).

---

### Menu Color

Controls the background treatment of every floating surface — Popover, DropdownMenu, Select, and Combobox content panels.

| Value | Background | Blur | Dark mode |
|---|---|---|---|
| `Default` | Solid `--popover` | None | ✅ |
| `Inverted` | Dark surface — `oklch(0.145 0 0)` | None | Light mode only (no-op in dark) |
| `DefaultTranslucent` | `--popover` at 50% opacity | `blur(18px) saturate(150%)` | ✅ |
| `InvertedTranslucent` | Dark surface at 70% opacity | `blur(18px) saturate(150%)` | Light mode only |

```csharp
await ThemeService.SetMenuColorAsync(MenuColor.DefaultTranslucent);
```

`Inverted` and `InvertedTranslucent` are light-mode-only — they are scoped to `:root:not(.dark)` and have no effect in dark mode (dark surfaces are already inverted by definition).

The translucent modes use `backdrop-filter` directly on the menu element (not a pseudo-element). The filter value is stored in a CSS custom property to work around a Tailwind v4 minifier issue that strips spaces between filter functions, and to ensure correct `-webkit-` vendoring.

Pairs well with a transparent or blurred page background for a full glassmorphism effect.

---

`ThemePreset` is a portable C# record that bundles all eight theme dimensions into a single named unit. Apply it with `ApplyPresetAsync`.

**Built-in presets:**

| Preset | Base | Style | Radius | Font | Menu Accent | Menu Color |
|---|---|---|---|---|---|---|
| `ThemePreset.Default` | Zinc | Default | Medium | System | Subtle | Default |
| `ThemePreset.Luma` | Zinc | Luma | Medium | Inter | Subtle | Default |
| `ThemePreset.Nova` | Zinc | Nova | Small | Geist | Subtle | Default |
| `ThemePreset.Maia` | Mauve | Maia | Large | Plus Jakarta | Subtle | Default |
| `ThemePreset.Lyra` | Slate | Lyra | None | Geist | Subtle | Default |

Usage:

```csharp
// Apply a built-in preset
await ThemeService.ApplyPresetAsync(ThemePreset.Luma);

// Build a custom preset — all eight dimensions in one place
var glassDash = new ThemePreset(
    Name:         "Glass Dashboard",
    BaseColor:    BaseColor.Luma,
    PrimaryColor: PrimaryColor.Blue,
    StyleVariant: StyleVariant.Luma,
    RadiusPreset: RadiusPreset.Medium,
    FontPreset:   FontPreset.Inter,
    MenuAccent:   MenuAccent.Bold,
    MenuColor:    MenuColor.DefaultTranslucent,
    IsDarkMode:   false);

await ThemeService.ApplyPresetAsync(glassDash);
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
1. Your theme file (no @layer)         ← Your customizations win
2. Menu color / accent rules (unlayered) ← Menu surface overrides
3. @layer utilities
4. @layer components
5. @layer base                          ← Library defaults
```

Menu color and menu accent rules are intentionally placed outside any `@layer` so they can override `@layer base` base-color variable definitions at the same specificity.

This means you only need to define the variables you want to customize.

---

## ThemeService API Reference

Complete list of `ThemeService` members:

### Properties

| Property | Type | Description |
|---|---|---|
| `IsDarkMode` | `bool` | Whether dark mode is currently active |
| `CurrentBaseColor` | `BaseColor` | Active base color |
| `CurrentPrimaryColor` | `PrimaryColor` | Active primary color |
| `CurrentStyleVariant` | `StyleVariant` | Active visual style variant |
| `CurrentRadiusPreset` | `RadiusPreset` | Active radius preset |
| `CurrentFontPreset` | `FontPreset` | Active font preset |
| `CurrentMenuAccent` | `MenuAccent` | Active menu accent intensity |
| `CurrentMenuColor` | `MenuColor` | Active menu color/surface mode |
| `OnThemeChanged` | `event Action?` | Raised after any theme change |

### Methods

| Method | Description |
|---|---|
| `InitializeAsync()` | Restore persisted theme from `localStorage`. Call on app startup. |
| `ToggleThemeAsync()` | Toggle dark/light mode and persist. |
| `SetThemeAsync(bool isDark)` | Set dark mode to a specific state and persist. |
| `SetBaseColorAsync(BaseColor)` | Set base neutral palette. |
| `SetPrimaryColorAsync(PrimaryColor)` | Set primary accent color. |
| `SetStyleVariantAsync(StyleVariant)` | Set visual style variant. |
| `SetRadiusPresetAsync(RadiusPreset)` | Set radius override. |
| `SetFontPresetAsync(FontPreset)` | Set font pairing. |
| `SetMenuAccentAsync(MenuAccent)` | Set menu item hover intensity. |
| `SetMenuColorAsync(MenuColor)` | Set menu/popover surface mode. |
| `ApplyPresetAsync(ThemePreset)` | Apply all dimensions atomically from a preset record. |

### localStorage Keys

`theme.js` reads and writes these keys to persist and restore the full theme state before Blazor boots (preventing FOUC):

| Key | Values | Dimension |
|---|---|---|
| `theme` | `"dark"` / `"light"` | Dark mode |
| `baseColor` | e.g. `"Zinc"`, `"Luma"` | Base color |
| `primaryColor` | e.g. `"Default"`, `"Blue"` | Primary color |
| `styleVariant` | e.g. `"Nova"`, `"Luma"` | Style variant |
| `radiusPreset` | e.g. `"Default"`, `"Full"` | Radius preset |
| `fontPreset` | e.g. `"System"`, `"Inter"` | Font preset |
| `menuAccent` | `"Subtle"` / `"Bold"` | Menu accent |
| `menuColor` | e.g. `"Default"`, `"DefaultTranslucent"` | Menu color |

`theme.js` must load before Blazor to apply CSS classes synchronously and prevent a flash of unstyled content on page load.

---

## ThemeSwitcher Component

The built-in `ThemeSwitcher` component renders a popover picker for all theme dimensions. Drop it anywhere in your layout:

```razor
<ThemeSwitcher />
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Strategy` | `PositioningStrategy` | `Fixed` | `Fixed` escapes stacking contexts; use `Absolute` if fixed positioning causes issues |
| `ZIndex` | `int` | `60` | CSS z-index for the popover panel |
| `TriggerClass` | `string?` | `null` | Additional CSS classes for the trigger button |
| `PopoverContentClass` | `string?` | `null` | Additional CSS classes for the popover panel |
| `Align` | `PopoverAlign` | `End` | Popover alignment: `Start`, `Center`, or `End` |

---

NeoUI does not require Tailwind — `components.css` is a self-contained pre-built stylesheet. However, if your app uses Tailwind v4, you must add `@theme inline` to your Tailwind build input to prevent generated utilities from hardcoding computed color values that would override NeoUI's runtime CSS variable changes:

```css
/* your-app/styles/app.css  (Tailwind build input) */
@import "tailwindcss";
@theme inline;   /* ← ensures utilities use var(--color-*) not hardcoded oklch() */
```

Without `@theme inline`, Tailwind v4 generates utilities with hardcoded values (e.g. `color: oklch(0.145 0 0)`) that are written after `components.css` in the final stylesheet and permanently override the runtime variable changes made by `ThemeService`.



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
