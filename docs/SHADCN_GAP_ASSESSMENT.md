# shadcn/ui → NeoUI Gap Assessment

**Date:** April 2, 2026  
**Scope:** Theming system, visual styles, font presets, border radius, and the new Luma style

---

## Executive Summary

shadcn/ui has introduced several significant customization features in the Dec 2025 – Mar 2026 window. NeoUI's current theming system (two-axis color presets + single `--radius` + font variables) maps well to the old shadcn model, but **misses four distinct feature areas** that users will increasingly expect.

---

## Gap 1: Border Radius Scale (Expanded)

### What shadcn/ui has
As of their updated theming system, `--radius` drives a full derived scale via CSS multipliers:

```css
--radius: 0.625rem;     /* base — also changed from old 0.5rem */
--radius-sm:  calc(var(--radius) * 0.6);
--radius-md:  calc(var(--radius) * 0.8);
--radius-lg:  var(--radius);
--radius-xl:  calc(var(--radius) * 1.4);
--radius-2xl: calc(var(--radius) * 1.8);
--radius-3xl: calc(var(--radius) * 2.2);
--radius-4xl: calc(var(--radius) * 2.6);
```

### What NeoUI has
```css
--radius: 0.5rem;   /* default — still old value */
/* Tailwind config derives: */
borderRadius: {
  lg: "var(--radius)",
  md: "calc(var(--radius) - 2px)",
  sm: "calc(var(--radius) - 4px)",
}
```

### Gap Analysis
| Aspect | shadcn/ui | NeoUI | Delta |
|---|---|---|---|
| Base default | `0.625rem` | `0.5rem` | Off by 0.125rem |
| Derivation approach | Multipliers (scale-proportional) | Pixel subtraction (breaks at small radii) | Different model |
| Scale steps | 7 named sizes (sm → 4xl) | 3 named sizes (sm → lg) | Missing xl, 2xl, 3xl, 4xl |
| Consumed via | CSS custom properties directly | Tailwind `rounded-lg/md/sm` utilities | Works but limited |

**Impact:** Components that need extra-large radius (dialogs, cards with Luma/Maia styles) have no tokens to reference. The pixel-subtraction model also becomes negative if `--radius` is set below `0.25rem`.

**Recommendation:** Add `--radius-sm` through `--radius-4xl` CSS variables using the multiplier approach. Update `tailwind.config.js` to map the new tokens. Update `--radius` default to `0.625rem` to stay in sync.

---

## Gap 2: Visual Styles / Component Themes (Vega, Nova, Lyra, Mira, Maia, Luma)

### What shadcn/ui has
Since December 2025, shadcn/ui ships **6 named visual styles** that change the character of components — not just colors but spacing, geometry, and font pairing:

| Style | Character | Spacing | Shape |
|---|---|---|---|
| **Vega** | Classic shadcn look | Default | Rounded |
| **Nova** | Compact | Reduced padding/margins | Default |
| **Lyra** | Technical/boxy | Default | Sharp, pairs with mono fonts |
| **Mira** | Ultra-compact | Dense | Default |
| **Maia** | Soft & airy | Generous | Very rounded |
| **Luma** *(new, Mar 31 2026)* | macOS Tahoe-inspired | Breathable | Rounded geometry + soft elevation |

These styles rewrite component structure at install time — they affect padding, border-radius usage, font choices, and elevation. Selecting a style is part of `npx shadcn create` / `--preset`.

### What NeoUI has
No concept of named component styles. All components use a single implicit style (equivalent to Vega/classic). `--radius` can be changed but there is no packaged "soft/compact/sharp" mode.

### Gap Analysis
NeoUI has zero equivalent. The `ThemeSwitcher` only exposes base color + primary color, not style variant.

**Impact:** High. Users migrating from shadcn/ui or building design systems expect to be able to pick a component character (compact vs. airy, sharp vs. rounded). Luma in particular is getting significant attention as the newest style.

**Recommendation (phased):**
1. **Short-term:** Introduce a `--spacing-scale` CSS variable (e.g. `0.75` for compact, `1.0` for default, `1.25` for airy) that component padding can reference, without requiring different component builds.
2. **Medium-term:** Add a `StyleVariant` enum (`Classic`, `Compact`, `Sharp`, `Soft`) to `ThemeService` and map it to CSS variable changes.
3. **Long-term / Luma:** Implement Luma as a style preset — larger radius tokens (`--radius-xl`/`--radius-2xl`), softer shadow elevation, breathable padding scale.

---

## Gap 3: Font Presets

### What shadcn/ui has
Fonts are now a **first-class registry type** (`registry:font`). The CLI installs and wires fonts the same way it installs components:

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

The shadcn/create theme builder lets you pick a font as part of your preset. Standard pairings include Inter (Vega), Geist (Nova), monospace (Lyra), etc. A `selector` field applies the font to a specific element (e.g. headings) rather than globally.

### What NeoUI has
`--font-sans`, `--font-serif`, `--font-mono` CSS variables exist and are consumed by Tailwind. No font presets, no font picker in `ThemeSwitcher`, no font persisted to localStorage, no curated pairings per style.

### Gap Analysis
| Feature | shadcn/ui | NeoUI | Delta |
|---|---|---|---|
| Font CSS variables | `--font-sans`, `--font-mono`, `--font-serif` | Same 3 variables | Equivalent |
| Font picker UI | Built into `shadcn/create` + presets | None | Missing |
| Font persistence | Via preset code | Not persisted | Missing |
| Per-style font pairing | Yes (Lyra = mono, Maia = rounded serif) | No | Missing |
| Heading-specific font | Yes (via `selector` field) | No `--font-heading` variable | Missing |

**Recommendation:**
1. Add `--font-heading` CSS variable (used for h1–h4) — a common pattern in the new shadcn ecosystem.
2. Add a `FontPreset` enum to `ThemeService` with curated options (e.g. `SystemUI`, `Inter`, `GeistSans`, `GeistMono`, `Playfair`).
3. Add font picker to `ThemeSwitcher`, persisted to localStorage.

---

## Gap 4: Preset System

### What shadcn/ui has
The March 2026 CLI v4 introduced a **preset** that encodes an entire design system config as a short opaque code string:

```
pnpm dlx shadcn@latest init --preset a1Dg5eFl
```

A preset bundles: base color, style variant, icon library, font choice, and radius — shared across teams and AI coding agents. Built via `ui.shadcn.com/create`.

### What NeoUI has
NeoUI ships a pre-built NuGet package — it doesn't use a copy-paste/install CLI model. The concept of a "preset string" doesn't apply 1:1. However, `ThemeService` already persists `baseColor + primaryColor + isDark` to localStorage.

### Gap Analysis
The preset system is CLI-centric (React/Next.js paradigm) and doesn't directly translate to the Blazor NuGet model. The closest analogy is the combination of `BaseColor` + `PrimaryColor` + `dark` that NeoUI already persists.

**What's actually missing is surface area:** NeoUI's `ThemeService` needs to also persist the new dimensions (style variant + font) to fully cover the equivalent of a shadcn preset.

**Recommendation:** Extend `ThemeService` with `CurrentStyle` (StyleVariant enum) and `CurrentFont` (FontPreset enum) properties, persisted to localStorage alongside the existing color/dark keys.

---

## Gap 5: New Neutral Color Options in the Palette

### What shadcn/ui has
The shadcn/ui colors page now lists 26 color palettes, including 4 new neutral tone options not present before:
- **Mauve** — muted purple-gray
- **Olive** — warm yellow-gray
- **Mist** — cool blue-gray
- **Taupe** — warm beige-gray

These extend beyond the original 5 neutrals (Zinc, Slate, Gray, Neutral, Stone).

### What NeoUI has
5 base colors: Zinc, Slate, Gray, Neutral, Stone.

**Recommendation:** Add `Mauve`, `Olive`, `Mist`, `Taupe` to the `BaseColor` enum and create corresponding CSS files under `wwwroot/css/themes/base/`. Update `theme.js` whitelist and `ThemeSwitcher`.

---

## Summary Table

| Feature | shadcn/ui Status | NeoUI Status | Priority |
|---|---|---|---|
| Radius scale (xl → 4xl) | 7-step multiplier scale | 3-step pixel-subtraction | **High** |
| Default `--radius` value | `0.625rem` | `0.5rem` | Medium |
| Visual styles (Vega/Nova/Lyra/Mira/Maia) | 5 styles since Dec 2025 | None | **High** |
| Luma style | Introduced Mar 31, 2026 | None | **High** |
| Font presets (registry:font) | First-class, CLI-managed | Manual CSS variable override | Medium |
| `--font-heading` variable | Supported | Not defined | Medium |
| Font picker in ThemeService | Via preset/create | None | Medium |
| New neutral colors (Mauve, Olive, Mist, Taupe) | Added to palette | Not included | Low–Medium |
| Preset string (CLI) | Yes (`--preset <code>`) | N/A (NuGet model) | Low (different paradigm) |

---

## References

- [shadcn/ui Theming Docs](https://ui.shadcn.com/docs/theming)
- [December 2025 Changelog — npx shadcn create + Styles](https://ui.shadcn.com/docs/changelog/2025-12-shadcn-create)
- [March 2026 Changelog — CLI v4, registry:font, Presets](https://ui.shadcn.com/docs/changelog/2026-03-cli-v4)
- [shadcn/ui Colors](https://ui.shadcn.com/colors)
- [shadcn Luma announcement (X)](https://x.com/shadcn/status/2039049334369447952)
- [Shadcn Studio — registry:base and registry:font deep dive](https://shadcnstudio.com/blog/shadcn-cli-v4-registry-base-and-registry-font)
