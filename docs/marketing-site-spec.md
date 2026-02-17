# NeoUI.io Marketing Website Specification

**Version:** V1 – Brand Identity Checkpoint (Enhanced with Project Context)  
**Last Updated:** 2026-02-17 16:15:29  
**Project:** [NeoBlazorUI](https://github.com/jimmyps/blazor-shadcn-ui)

---

## 1. Purpose

NeoUI.io exists to:

- **Establish NeoBlazorUI as the shadcn of Blazor**
- **Define a modern design-system identity** within the Blazor ecosystem
- **Visually prove architectural quality** through live interactive scenarios
- **Position NeoBlazorUI as open-source at the core**
- **Provide a clear bridge to NeoBlazorUI Pro**

This is a **brand and positioning site**, not documentation.

Detailed component breakdowns live on the **[dedicated demo site](https://blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net)**.

---

## 2. Core Positioning

### Primary Category Claim

**The shadcn of Blazor.**

---

### Positioning Summary

NeoBlazorUI brings modern design-system principles to Blazor — composability, accessibility, thoughtful motion, and architectural depth — built natively for .NET and optimized for real-world applications.

**85+ production-ready components. Zero Tailwind setup. Pure Blazor.**

---

## 3. Brand Direction

NeoUI.io must feel:

- ✅ **Bold**
- ✅ **Spacious**
- ✅ **Minimal**
- ✅ **Intentional**
- ✅ **Architecturally serious**
- ✅ **Design-system forward**

It must **not** feel like:

- ❌ Documentation
- ❌ A GitHub side project
- ❌ A generic SaaS landing page
- ❌ Marketing-heavy or hype-driven

---

## 4. Tone Guardrails

Copy must be:

- ✅ **Calm and confident**
- ✅ **Design-aware**
- ✅ **Technically precise**
- ✅ **Free from hype language**
- ✅ **Free from negative comparisons**

### Avoid:

- ❌ "Revolutionary"
- ❌ "Ultimate"
- ❌ Enterprise marketing jargon
- ❌ Over-explaining

**Voice = Modern, composed, authoritative.**

---

## 5. Site Structure (Locked)

1. **Hero**
2. **Vision**
3. **Interactive Showcase** (Hybrid Bento)
4. **Why NeoBlazorUI**
5. **Key Capabilities**
6. **Open Source First**
7. **Newsletter**
8. **Footer**

**No component listing page on this site.**  
`/components` redirects to the dedicated demo site.

---

## 6. Hero Section

### Headline

**The shadcn of Blazor.**

### Subheadline

A modern design system for Blazor — composable, accessible, and built natively for .NET.

**Zero-config CSS. No Tailwind setup required.**

### CTAs

**Primary:**  
Get Started

**Secondary:**  
Docs

### Layout Rules

- `text-6xl` → `text-7xl` on desktop
- Generous whitespace (`py-36+`)
- Center aligned
- No gradients
- No decorative elements
- Strong typography focus

**Bold through restraint.**

---

## 7. Vision Section

### Title

**Reimagining Modern UI for Blazor**

### Body

Blazor is a powerful developer platform—but modern UI design patterns have been largely absent.

NeoBlazorUI changes that. It brings the shadcn/ui philosophy—**composability, accessibility, thoughtful motion, and design-system architecture**—natively to .NET.

**85+ production-ready components. Zero Tailwind setup. Pure Blazor.**

Applications should be beautiful inside and out.

---

## 8. Interactive Showcase (Hybrid Bento Grid)

### Purpose

This section **proves credibility visually**.

It:

- ✅ Uses **real NeoBlazorUI components**
- ✅ Uses **real state and interaction**
- ✅ Uses **real theme switching**
- ✅ Uses **real portal layering**
- ✅ Uses **Interactive Auto** rendering mode

**No titles. No code snippets. No playground. No explanation text per tile.**

It must feel like **cropped SaaS product screens** — but fully interactive.

---

### Grid Philosophy

Hybrid layout:

- 2–3 **dominant tiles**
- Several **medium tiles**
- Balanced spacing
- Clean rhythm

**Not chaotic. Not documentation-like.**

---

### SaaS-Oriented Scenarios → NeoBlazorUI Components

Map each showcase tile to **real components** from the library:

| Showcase Scenario | NeoBlazorUI Component(s) | Key Features to Demonstrate |
|-------------------|--------------------------|---------------------------|
| **Analytics Dashboard** | `LineChart`, `BarChart`, `AreaChart` | Live data, `--chart-1` through `--chart-5` CSS variables |
| **Billing Plan Selector** | `ToggleGroup` (single selection) + `Card` + `Badge` | Monthly/yearly toggle, pricing tiers |
| **Team Management** | `Dialog` with nested `Dialog` | Nested modal proof (PortalHost architecture) |
| **Data Table** | `DataTable` | Sorting, filtering, pagination |
| **Toast Notifications** | `Toast` | Variants (default, destructive, success), programmatic API |
| **Layered Popover** | `Popover` + `Dialog` | Portal layering (overlay over modal) |
| **Settings Panel** | `Sheet` + `AlertDialog` | Side panel with destructive confirmation |
| **Form Validation** | `Input` + `Select` + `Switch` + `Button` | Real-time validation, field composition |

### Component Requirements

All showcase tiles must:

- ✅ Use **real NeoBlazorUI components** (no placeholder HTML)
- ✅ Use **real data** (not hardcoded "Lorem ipsum")
- ✅ Use **Interactive Auto** render mode
- ✅ Demonstrate **PortalHost** for overlays (ContainerPortalHost + OverlayPortalHost)
- ✅ Work seamlessly with **theme switching**

**Reference:** [Component List in README](https://github.com/jimmyps/blazor-shadcn-ui#-components)

---

### Theme Switching (Critical)

**Implementation:**

Use the **built-in ThemeSwitcher component** from NeoBlazorUI (PR #124):

- **Primary Switcher:** Popover with 17 color swatches  
  (Red, Rose, Orange, Amber, Yellow, Lime, Green, Emerald, Teal, Cyan, Sky, Blue, Indigo, Violet, Purple, Fuchsia, Pink)
- **Base Switcher:** Dropdown for base colors (Zinc, Slate, Gray, Neutral, Stone)
- **Dark/Light Toggle:** DarkModeToggle component with sun/moon icons

### Technical Requirements

- ✅ Uses **ThemeService** for programmatic control
- ✅ **LocalStorage persistence** for user preferences
- ✅ **CSP-compliant** (named JavaScript functions)
- ✅ Theme changes update **all showcase tiles instantly**
- ✅ No hydration mismatch when switching Server ↔ WASM

### Architectural Proof

This demonstrates:

- CSS variable architecture working at scale
- State management across render modes
- Real-time theme application without page reload

**This is not just theming—it's proof of architectural correctness.**

**Reference:** [Theme System Documentation](https://github.com/jimmyps/blazor-shadcn-ui#-dynamic-theme-customization-new)  
**Implementation:** [PR #124](https://github.com/jimmyps/blazor-shadcn-ui/pull/124)

---

## 9. Why NeoBlazorUI

### Focus

**Craft × Composability × Architecture × DX**

---

### Thoughtfully Crafted Design

Refined spacing, meaningful motion, balanced UI decisions.

### Composable by Design

Small, focused components that build naturally into complex systems.

### Advanced Patterns Done Right

Nested dialogs, layered portals, scalable chart abstractions — engineered correctly.

### Developer Experience First

Clean APIs, sensible defaults, minimal ceremony.

**Zero-config CSS. No Tailwind setup. No Node.js tooling.**

---

## 10. Key Capabilities

✅ **85+ Production-Ready Components**  
✅ **Composable Primitives** (15 headless components)  
✅ **Design Token Driven Architecture**  
✅ **Performance-Considered Motion** (Motion.dev presets)  
✅ **Dark Mode First-Class**  
✅ **Interactive Auto Compatible** (.NET 10)  
✅ **Minimal JavaScript Surface** (pure Blazor)

### 🆕 Unique to NeoBlazorUI:

- **Zero-Config CSS** (pre-built, ships with NuGet)
- **85 Theme Combinations** (5 base × 17 primary colors)
- **Three Icon Libraries** (Lucide 1,640 icons, Heroicons 1,288 icons, Feather 286 icons)
- **Dual-Layer Architecture** (primitives + styled components)
- **PortalHost System** (nested overlays done right)
- **AsChild Pattern** (Radix UI composition)

---

## 11. Open Source First

### Title

**Built in the Open.**

### What's Included in NeoBlazorUI OSS

✅ **85+ Production-Ready Components**  
✅ **Pre-Built CSS** (no Tailwind setup)  
✅ **Interactive Auto** support (.NET 10)  
✅ **Three Icon Libraries** (Lucide, Heroicons, Feather)  
✅ **15 Headless Primitives**  
✅ **Dynamic Theme System** (85 combinations)  
✅ **Dark Mode First-Class**  
✅ **Accessibility (WCAG 2.1 AA)**

**MIT Licensed. Forever Free.**

NeoBlazorUI is open-source and community-driven. The core design system is free and continuously evolving.

---

### CTAs:

**Explore on GitHub**  
**Explore NeoBlazorUI Pro**

Pro mention is subtle and non-aggressive.

---

## 12. Pro Bridge (Informational Only)

NeoBlazorUI Pro extends the open core with:

- 🚀 **150+ Premium Blocks**
- 🏢 **SaaS Starter Kit**
- 🤖 **AI-Powered Components**
- 🏢 **Enterprise Components**
- ⚡ **Priority Support** (24-hour target)
- ♾️ **Lifetime Updates**
- 📜 **Commercial License**

**No pricing displayed on NeoUI.io.**

Pro site handles monetization.

**OSS library is fully functional** — not a limited "community edition."

---

## 13. Newsletter

### Title

**Stay in the Loop.**

### Body

Follow the evolution of modern Blazor UI.

**Get notified when new components ship. Join the Blazor design system community.**

### Purpose

- 📦 **Component updates** (new releases, bug fixes)
- 🌐 **Blazor ecosystem news** (framework updates, community projects)
- 🎨 **Design system insights** (theming, accessibility, performance)
- 🎁 **Pro subscriber perks** (early access, exclusive content)

**No spam. No sales pitches. Just quality updates.**

Simple email field. No popups. No aggressive prompts.

---

## 14. Visual Identity

### Typography

- Large, bold hero
- Generous spacing
- Clean sans-serif (inherits from `--font-sans`)
- Strong hierarchy

### Layout

- Max width 1200px
- Large section padding
- Clear rhythm

### Motion

- Subtle fade/translate (Motion.dev presets)
- Under 200ms
- No dramatic animation

### Color System

Uses **shadcn/ui CSS variables** for consistency:

- **Base colors:** Zinc, Slate, Gray, Neutral, Stone
- **Primary colors:** 17 options (Red → Pink)
- **Chart colors:** `--chart-1` through `--chart-5`
- **Sidebar colors:** `--sidebar`, `--sidebar-primary`, `--sidebar-accent`

Compatible with:
- [shadcn/ui themes](https://ui.shadcn.com/themes)
- [tweakcn.com](https://tweakcn.com)

### Aesthetic References

Aligned with:

- ✅ **shadcn/ui** (design language)
- ✅ **Radix UI** (component behavior)
- ✅ **Vercel** (developer aesthetic)
- ✅ **reui** (layout structure)
- ✅ **HeroUI** (cleanliness)
- ✅ **Magic UI** (polish)

**NOT Stripe-style gradients.**

---

## 15. Technical Constraints

### Platform

- **Built with .NET 10 Blazor Web App** (not .NET 8)
- **Interactive Auto rendering mode:**
  - Fast initial load (Server-side rendering)
  - Rich interactivity (WebAssembly after download)
  - Seamless transition between modes

### Architecture

- **Dual-layer architecture:**
  - `NeoBlazorUI.Primitives` (headless, runs on Server + WASM)
  - `NeoBlazorUI.Components` (styled, runs on Server + WASM)
- **PortalHost architecture** for overlays:
  - `ContainerPortalHost` (for contained overlays)
  - `OverlayPortalHost` (for full-screen overlays)
  - Reference: [README Setup Guide](https://github.com/jimmyps/blazor-shadcn-ui#quick-start)

### Zero-Config Approach

- ✅ **No JavaScript dependencies** (pure Blazor)
- ✅ **No Node.js build tooling** (pre-built CSS ships with NuGet)
- ✅ **No Tailwind setup required** (pre-built CSS included)

### Hydration & Rendering Strategy

- ✅ Use **InteractiveAuto render mode** (as documented in project)
- ✅ Add **PortalHost components** (ContainerPortalHost + OverlayPortalHost) to layout
- ✅ Avoid static rendering of interactive showcase—all tiles must be fully interactive on load
- ✅ **Theme switching** must work seamlessly across Server → WASM transition
- ✅ Test **nested dialogs, layered popovers** to prove architectural correctness

**The marketing site must itself prove the system.**

Real NeoBlazorUI components only. No mocked UI. No static screenshots.

---

## 16. What Must Not Happen

### Generic Risks

- ❌ No generic SaaS copy
- ❌ No documentation clutter
- ❌ No marketing fluff
- ❌ No code playground duplication
- ❌ No broken hydration or flicker
- ❌ No layout jank
- ❌ No inconsistency between light and dark mode

### NeoBlazorUI-Specific Risks

- ❌ **No confusion about Tailwind:** Site must clearly state "Pre-built CSS included, no Tailwind setup required"
- ❌ **No "lite version" perception:** OSS library is fully functional, not a teaser for Pro
- ❌ **No shadcn/ui clone accusations:** Emphasize "reimplementation for Blazor/C#" and Radix UI principles
- ❌ **No .NET 8 references:** All code examples must use .NET 10 syntax
- ❌ **No broken PortalHost examples:** Overlay components must show ContainerPortalHost + OverlayPortalHost setup
- ❌ **No theme switcher duplication:** Use built-in ThemeSwitcher from NeoBlazorUI (PR #124)

---

## 17. Implementation Checklist

### Phase 1: Foundation

- [ ] Hero Section
- [ ] Vision Section
- [ ] Footer with social links

### Phase 2: Interactive Showcase

- [ ] Analytics Dashboard tile (LineChart/BarChart)
- [ ] Billing Selector tile (ToggleGroup + Card)
- [ ] Team Management tile (nested Dialog)
- [ ] Data Table tile (sorting, pagination)
- [ ] Toast Notifications tile
- [ ] Layered Popover tile
- [ ] Settings Panel tile (Sheet + AlertDialog)

### Phase 3: Theme System

- [ ] Integrate ThemeSwitcher component (PR #124)
- [ ] Add DarkModeToggle to header
- [ ] Test theme persistence (LocalStorage)
- [ ] Verify all showcase tiles update on theme change

### Phase 4: Content

- [ ] "Why NeoBlazorUI" section
- [ ] "Key Capabilities" section
- [ ] "Open Source First" section
- [ ] Newsletter form integration

### Phase 5: Polish

- [ ] Motion.dev animations
- [ ] Responsive design (mobile-first)
- [ ] Performance audit (lighthouse)
- [ ] Accessibility audit (WCAG 2.1 AA)

---

## 18. Reference Links

- **Project Repository:** [github.com/jimmyps/blazor-shadcn-ui](https://github.com/jimmyps/blazor-shadcn-ui)
- **Live Demo:** [blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net](https://blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net)
- **README:** [Component List](https://github.com/jimmyps/blazor-shadcn-ui#-components)
- **Theme System:** [Dynamic Theme Customization](https://github.com/jimmyps/blazor-shadcn-ui#-dynamic-theme-customization-new)
- **Theme Implementation:** [PR #124](https://github.com/jimmyps/blazor-shadcn-ui/pull/124)

---

## ✅ Checkpoint Status

**Positioning:** ✅ Locked  
**Brand direction:** ✅ Locked  
**Demo philosophy:** ✅ Locked  
**OSS/Pro separation:** ✅ Locked  
**Visual tone:** ✅ Locked  
**Newsletter strategy:** ✅ Locked  
**Technical constraints:** ✅ Locked  
**Theme system:** ✅ Locked (uses built-in ThemeSwitcher)

**NeoUI.io V1 is defined and aligned with project architecture.**

---

**Last Updated:** 2026-02-17 16:15:29  
**Version:** V1 – Brand Identity Checkpoint (Enhanced)