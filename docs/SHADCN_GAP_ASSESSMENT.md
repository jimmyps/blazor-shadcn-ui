# shadcn/ui → NeoUI Gap Assessment

**Date:** April 2, 2026  
**Scope:** Theming system, visual styles, font presets, border radius, and the new Luma style

---

## Executive Summary

shadcn/ui has made significant moves in the Dec 2025 – Mar 2026 window, shifting from a "CSS variable theming" model to a full **design system configuration** model. NeoUI's two-axis color preset system maps well to the old shadcn approach but is missing five distinct feature dimensions that users will increasingly expect.

---

## Gap 1: Border Radius Scale (Expanded)

### What shadcn/ui has

A single `--radius` value now drives a **7-step proportional scale** via Tailwind v4's `@theme inline`:

```css
:root {
  --radius: 0.625rem;   /* default — also changed from old 0.5rem */
}

@theme inline {
  --radius-sm:  calc(var(--radius) * 0.6);
  --radius-md:  calc(var(--radius) * 0.8);
  --radius-lg:  var(--radius);
  --radius-xl:  calc(var(--radius) * 1.4);
  --radius-2xl: calc(var(--radius) * 1.8);
  --radius-3xl: calc(var(--radius) * 2.2);
  --radius-4xl: calc(var(--radius) * 2.6);
}
```

The `shadcn/create` preset picker exposes named radius options: **None** (`0rem`), **Small** (~`0.25rem`), **Medium** (~`0.5rem`), **Large** (~`0.75rem`), **Full** (`≥1.5rem`). The style system applies constraints — the Lyra style forces "None" to preserve its sharp aesthetic.

### What NeoUI has

```css
--radius: 0.5rem;   /* outdated default */
```

```js
// tailwind.config.js — derives via pixel subtraction:
borderRadius: {
  lg: "var(--radius)",
  md: "calc(var(--radius) - 2px)",
  sm: "calc(var(--radius) - 4px)",
}
```

### Gap Analysis

| Aspect | shadcn/ui | NeoUI | Delta |
|---|---|---|---|
| Default value | `0.625rem` | `0.5rem` | Off by `0.125rem` |
| Derivation approach | Multipliers (scale-proportional) | Pixel subtraction (breaks below `0.25rem`) | Different model |
| Scale steps | 7 named sizes (`sm` → `4xl`) | 3 named sizes (`sm` → `lg`) | Missing `xl`, `2xl`, `3xl`, `4xl` |
| Named preset options | None / Small / Medium / Large / Full | None | Missing picker |
| Consumed via | CSS variables directly on `:root` | Tailwind `borderRadius` extension | Different surface |

**Impact:** Luma and Maia styles require large-radius tokens (cards with `--radius-2xl`, dialogs with `--radius-xl`). The pixel-subtraction approach also becomes negative — `calc(0rem - 2px)` — when `--radius` is set to `None`.

**Recommendation:**
1. Add `--radius-sm` through `--radius-4xl` CSS variables using the multiplier approach in `components-input.css`.
2. Update `tailwind.config.js` `borderRadius` to reference the new CSS variables.
3. Update default `--radius` to `0.625rem` to stay in sync.
4. Add `RadiusPreset` enum to `ThemeService` (None, Small, Medium, Large, Full) that sets `--radius`.

---

## Gap 2: Visual Styles / Component Themes

### What shadcn/ui has

Since December 2025, shadcn/ui ships **6 named visual styles** that change the character of components — not just colors, but spacing, geometry, and font pairing. Critically, in the React model, styles **rewrite the copied component source code** to match the aesthetic.

| Style | Character | Spacing | Shape | Font Pairing |
|---|---|---|---|---|
| **Vega** | Classic shadcn look | Default | Rounded | Inter |
| **Nova** | Compact | Reduced padding/margins | Default | Geist |
| **Lyra** | Technical/boxy | Default | Sharp (forces radius = None) | Mono fonts only |
| **Mira** | Ultra-compact | Dense | Default | — |
| **Maia** | Soft & airy | Generous | Very rounded | Rounded serif/sans |
| **Luma** *(new, Mar 31 2026)* | macOS Tahoe-inspired | Breathable | Rounded geometry + soft elevation | — |

In `components.json` this is reflected as `"style": "radix-nova"` or `"style": "base-luma"`.

### What NeoUI has

No concept of named component styles. All components implicitly use "Vega/classic" character. `--radius` can be changed but there is no packaged "soft/compact/sharp" mode. The `ThemeSwitcher` only exposes base color + primary color.

### Gap Analysis

NeoUI has zero equivalent. Unlike the React model where styles rewrite source files, in Blazor the pre-built component library approach means style variants must be expressed purely through CSS — but the mechanism is achievable.

**Impact:** High. Users building design systems expect to be able to set a component character. Luma is the newest and most prominent style being promoted.

**Recommendation (phased):**
1. **Short-term:** Introduce a `--component-spacing` CSS variable (e.g. `1.0` = default, `0.8` = compact, `1.25` = airy) that component padding multiplies against. Add `--component-radius-scale` that overrides the radius step used per component class.
2. **Medium-term:** Add a `StyleVariant` enum (`Classic`, `Compact`, `Sharp`, `Soft`, `Luma`) to `ThemeService`, persisted to localStorage as `"style"`. Wire it to a set of CSS variable overrides applied on `<html>` as `style-*` classes.
3. **Luma specifically:** Larger default radius (`--radius: 1rem`), use `--radius-2xl` on cards/dialogs, softer shadows, add `--elevation` variable driving shadow opacity.

---

## Gap 3: Font Presets

### What shadcn/ui has

Fonts are a **first-class registry type** (`registry:font`) in CLI v4. The CLI installs and wires fonts identically to components. 17 built-in fonts are available, including Inter, Geist, JetBrains Mono, Lora, Merriweather, Roboto. Font selection is style-aware (Lyra biases toward mono; Maia/Luma toward readable sans).

```json
{
  "name": "font-inter",
  "type": "registry:font",
  "font": {
    "family": "'Inter Variable', sans-serif",
    "provider": "google",
    "import": "Inter",
    "variable": "--font-sans",
    "subsets": ["latin"]
  }
}
```

A `selector` field can target specific elements (e.g. heading fonts applied only to `h1–h4`).

### What NeoUI has

`--font-sans`, `--font-serif`, `--font-mono` CSS variables exist and are consumed by Tailwind. Consumers override manually via `:root`. No font presets, no font picker in `ThemeSwitcher`, font choice not persisted in `ThemeService`.

### Gap Analysis

| Feature | shadcn/ui | NeoUI | Delta |
|---|---|---|---|
| Font CSS variables | `--font-sans`, `--font-mono`, `--font-serif` | Same 3 variables | **Equivalent** |
| Font picker UI | Built into `shadcn/create` + presets | None | Missing |
| Font persistence | Via preset / localStorage | Not persisted | Missing |
| Per-style font pairing | Yes (Lyra = mono, Luma/Maia = sans) | No | Missing |
| Heading-specific font | Yes (`--font-heading`, via `selector`) | No `--font-heading` variable | Missing |
| 17 curated font options | Yes | No curated set | Missing |

**Recommendation:**
1. Add `--font-heading` CSS variable (used for h1–h4). This is used in Luma to create typographic hierarchy.
2. Add a `FontPreset` enum to `ThemeService` with curated options: `SystemUI`, `Inter`, `GeistSans`, `GeistMono`, `JetBrainsMono`, `Lora`.
3. Persist font choice to localStorage under `"font"` key.
4. Add font picker section to `ThemeSwitcher` popover.

---

## Gap 4: New Neutral Color Options

### What shadcn/ui has

The color palette expanded to 26 options including 4 new neutral tones that extend beyond the original 5 grays:

| New Color | Character |
|---|---|
| **Mauve** | Muted purple-gray |
| **Olive** | Warm yellow-gray |
| **Mist** | Cool blue-gray |
| **Taupe** | Warm beige-gray |

### What NeoUI has

5 base colors: Zinc, Slate, Gray, Neutral, Stone.

### Gap Analysis

**Impact:** Medium. The new neutrals are particularly useful for Luma (which favors `Mist` and `Mauve` aesthetics) and Maia. Users checking the shadcn/create picker will notice these options are absent.

**Recommendation:** Add `Mauve`, `Olive`, `Mist`, `Taupe` to the `BaseColor` enum. Create CSS files under `wwwroot/css/themes/base/`. Update `theme.js` whitelist and `ThemeSwitcher` color grid.

---

## Gap 5: Preset System

### What shadcn/ui has

The March 2026 CLI v4 introduced a **compact preset code** (7-char Base62 encoding 40 bits) that bundles 8 design parameters:

| Parameter | Options |
|---|---|
| Style | nova, vega, maia, lyra, mira, luma |
| Base Color | neutral, zinc, mauve, olive, mist, taupe, stone |
| Accent/Primary Color | blue, purple, neutral, etc. |
| Font | inter, geist, jetbrains-mono, lora, merriweather |
| Icon Library | lucide, hugeicons, tabler |
| Border Radius | none, small, medium, large, full |
| Menu Accent | subtle, bold |
| Menu Color | default, inverted |

```bash
pnpm dlx shadcn@latest init --preset aIkeymG
```

A **lock system** lets you freeze specific parameters while randomizing others. Presets are shareable in URLs and AI agent prompts.

### What NeoUI has

NuGet-based model (not CLI copy-paste). `ThemeService` persists `baseColor` + `primaryColor` + `isDark`. No concept of style, font, radius preset, menu accent, or menu color dimensions.

### Gap Analysis

The preset system's CLI mechanics don't apply to NeoUI. However the **state surface** is the gap: NeoUI's `ThemeService` only tracks 3 of the ~8 equivalent parameters. Missing: style variant, font, radius preset, menu style.

**Recommendation:** Extend `ThemeService` with additional dimensions as they're implemented: `CurrentStyle`, `CurrentFont`, `CurrentRadiusPreset`. This is mostly additive work alongside Gaps 1–3 above.

---

## Consolidated Priority Matrix

| Gap | Feature | Priority | Effort |
|---|---|---|---|
| 1 | Radius scale (`--radius-sm` → `--radius-4xl`) | **High** | Low |
| 1 | Fix `--radius` derivation to use multipliers | **High** | Low |
| 1 | Update default `--radius` to `0.625rem` | Medium | Trivial |
| 2 | Visual style system (`StyleVariant` enum + CSS classes) | **High** | High |
| 2 | Luma style implementation | **High** | Medium |
| 2 | ThemeSwitcher: style picker UI | Medium | Medium |
| 3 | `--font-heading` CSS variable | Medium | Low |
| 3 | `FontPreset` enum + ThemeService persistence | Medium | Medium |
| 3 | ThemeSwitcher: font picker UI | Medium | Medium |
| 4 | New neutral colors: Mauve, Olive, Mist, Taupe | Medium | Low |
| 5 | ThemeService: track style + font + radius dimensions | Low | Low |

---

## References

- [shadcn/ui Theming Docs](https://ui.shadcn.com/docs/theming)
- [December 2025 Changelog — npx shadcn create + 5 Styles](https://ui.shadcn.com/docs/changelog/2025-12-shadcn-create)
- [March 2026 Changelog — CLI v4, registry:font, Presets](https://ui.shadcn.com/docs/changelog/2026-03-cli-v4)
- [shadcn/ui Colors](https://ui.shadcn.com/colors)
- [shadcn Luma announcement (X)](https://x.com/shadcn/status/2039049334369447952)
- [Shadcn Studio — registry:base and registry:font deep dive](https://shadcnstudio.com/blog/shadcn-cli-v4-registry-base-and-registry-font)
- [How shadcn/ui Presets Work (7-char encoding)](https://shadcnstudio.com/blog/how-shadcn-ui-presets-work)
- [CSS Variable and Theme Management — DeepWiki](https://deepwiki.com/shadcn-ui/ui/4.6-css-variable-and-theme-management)
- [Font Management — DeepWiki](https://deepwiki.com/shadcn-ui/ui/7.3-font-management)
