# NeoUI.io Marketing Website Specification

**Version:** V1 – Brand Identity Checkpoint  
**Date:** 2026-02-17  
**Status:** Locked for Implementation

---

## 1. Purpose

NeoUI.io exists to:
- Establish **NeoBlazorUI** as **the shadcn of Blazor**
- Define a modern design-system identity within the Blazor ecosystem
- Visually prove architectural quality through live interactive scenarios
- Position NeoBlazorUI as **open-source at the core**
- Provide a clear bridge to NeoBlazorUI Pro

**This is a brand and positioning site, not documentation.**

Detailed component breakdowns live on the dedicated demo site.

---

## 2. Core Positioning

### Primary Category Claim

**The shadcn of Blazor.**

### Positioning Summary

NeoBlazorUI brings modern design-system principles to Blazor—**composability, accessibility, thoughtful motion, and architectural depth**—built natively for .NET and optimized for real-world applications.

### Key Differentiators

- **Zero-Config CSS**: Pre-built CSS included, no Tailwind setup required
- **85+ Production-Ready Components**: Fully functional MIT-licensed core
- **Interactive Auto Support**: .NET 10 with seamless Server ↔ WASM rendering
- **85 Theme Combinations**: 5 base colors × 17 primary colors
- **Pure Blazor**: No JavaScript dependencies, no Node.js build tooling

---

## 3. Brand Direction

NeoUI.io must feel:
- **Bold**
- **Spacious**
- **Minimal**
- **Intentional**
- **Architecturally serious**
- **Design-system forward**

It must **not** feel like:
- Documentation
- A GitHub side project
- A generic SaaS landing page
- Marketing-heavy or hype-driven

---

## 4. Tone Guardrails

Copy must be:
- Calm and confident
- Design-aware
- Technically precise
- Free from hype language
- Free from negative comparisons

**Avoid:**
- "Revolutionary"
- "Ultimate"
- Enterprise marketing jargon
- Over-explaining

**Voice:** Modern, composed, authoritative.

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

A modern design system for Blazor—composable, accessible, and built natively for .NET.

**Zero Tailwind setup. Just install and build.**

### CTAs

**Primary:**  
Get Started

**Secondary:**  
View Demo

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
- Uses **real NeoBlazorUI components**
- Uses **real state and interaction**
- Uses **real theme switching**
- Uses **real portal layering**
- Uses **Interactive Auto**

**No titles. No code snippets. No playground. No explanation text per tile.**

It must feel like **cropped SaaS product screens**—but fully interactive.

### Grid Philosophy

Hybrid layout:
- 2–3 dominant tiles
- Several medium tiles
- Balanced spacing
- Clean rhythm

**Not chaotic. Not documentation-like.**

### SaaS-Oriented Scenarios (Component Mapping)

All tiles must use **real NeoBlazorUI components**:

| Showcase Scenario | NeoBlazorUI Components |
|-------------------|------------------------|
| **Analytics Dashboard** | `LineChart`, `BarChart`, `AreaChart` (with `--chart-1` through `--chart-5` CSS variables) |
| **Billing Plan Selector** | `ToggleGroup` (single selection) + `Card` + `Badge` |
| **Team Management** | Nested `Dialog` → `Dialog` (architectural proof via PortalHost) |
| **Data Table** | `DataTable` (sorting, filtering, pagination) |
| **Toast Notification Flow** | `Toast` with variants (default, destructive, success) |
| **Layered Popover** | `Popover` + `Dialog` (portal layering test) |
| **Settings Panel** | `Sheet` + `AlertDialog` (destructive confirmation) |

**Implementation Requirements:**
- ✅ Use **real data** (not placeholders)
- ✅ All interactions must function smoothly
- ✅ Use **InteractiveAuto** render mode
- ✅ Add **PortalHost components** (ContainerPortalHost + OverlayPortalHost) to layout
- ✅ Test nested overlays (Dialog → Dialog, Popover → Dialog)

### Theme Switching (Critical)

**Implementation:**

Use the **built-in ThemeSwitcher component** from NeoBlazorUI (see PR #124):

- **Primary Switcher**: Popover with 17 color swatches (Red, Rose, Orange, Amber, Yellow, Lime, Green, Emerald, Teal, Cyan, Sky, Blue, Indigo, Violet, Purple, Fuchsia, Pink)
- **Base Switcher**: Dropdown for base colors (Zinc, Slate, Gray, Neutral, Stone)
- **Dark/Light Toggle**: DarkModeToggle component with sun/moon icons

**Technical Requirements:**

- Uses **ThemeService** for programmatic control
- **LocalStorage persistence** for user preferences
- **CSP-compliant** (named JavaScript functions)
- Theme changes must update **all showcase tiles instantly**
- No hydration mismatch when switching Server ↔ WASM

**Architectural Proof:**

This demonstrates:
- CSS variable architecture working at scale
- State management across render modes
- Real-time theme application without page reload

**This is not just theming—it's proof of architectural correctness.**

---

## 9. Why NeoBlazorUI

### Focus

**Craft × Composability × Architecture × DX**

### Thoughtfully Crafted Design

Refined spacing, meaningful motion, balanced UI decisions.

### Composable by Design

Small, focused components that build naturally into complex systems.

### Advanced Patterns Done Right

Nested dialogs, layered portals, scalable chart abstractions—engineered correctly.

### Developer Experience First

Clean APIs, sensible defaults, minimal ceremony.

**Zero-config CSS. No Tailwind setup. Pure Blazor.**

---

## 10. Key Capabilities

✅ **85+ Production-Ready Components**  
✅ **Composable Primitives** (15 headless components)  
✅ **Design Token Driven Architecture** (shadcn/ui CSS variables)  
✅ **Performance-Considered Motion** (Motion.dev presets)  
✅ **Dark Mode First-Class** (CSS variable theming)  
✅ **Interactive Auto Compatible** (.NET 10)  
✅ **Minimal JavaScript Surface** (pure Blazor)

### 🆕 Unique to NeoBlazorUI

- **Zero-Config CSS**: Pre-built, no Tailwind setup required
- **85 Theme Combinations**: 5 base × 17 primary colors with live preview
- **Three Icon Libraries**: Lucide (1,640), Heroicons (1,288), Feather (286)
- **Dual-Layer Architecture**: Primitives (headless) + Components (styled)
- **PortalHost System**: Nested overlays done right
- **AsChild Pattern**: Radix UI-style composition

---

## 11. Open Source First

### Title

**Built in the Open.**

### Body

NeoBlazorUI is **open-source and community-driven**.  
The core design system is **free, fully functional, and continuously evolving**.

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

### CTAs

**Primary:**  
Explore on GitHub

**Secondary:**  
Explore NeoBlazorUI Pro

**Note:** Pro mention is subtle and non-aggressive.

---

## 12. Pro Bridge (Informational Only)

### NeoBlazorUI Pro

NeoBlazorUI Pro extends the open core with:

🚀 **150+ Premium Blocks**  
🤖 **AI-Powered Components**  
🏢 **Enterprise Components**  
⚡ **Priority Support** (24-hour target)  
♾️ **Lifetime Updates**  
📜 **Commercial License**

**No pricing displayed on NeoUI.io.**  
Pro site handles monetization.

---

## 13. Newsletter

### Title

**Stay in the Loop.**

### Body

Follow the evolution of modern Blazor UI.

**Get notified when new components ship. Join 1,000+ Blazor developers.**

### Purpose

- **Component updates** (new releases, bug fixes)
- **Blazor ecosystem news** (framework updates, community projects)
- **Design system insights** (theming, accessibility, performance)
- **Pro subscriber perks** (early access, exclusive content)

**No spam. No sales pitches. Just quality updates.**

### Implementation

- Simple email field
- No popups
- No aggressive prompts

---

## 14. Visual Identity

### Typography & Layout

- Uses **shadcn/ui CSS variables** for consistency
- Compatible with **tweakcn.com** themes
- Large, bold hero typography
- Generous spacing (large section padding)
- Clean sans-serif (inherits from `--font-sans`)
- Strong hierarchy
- Max width 1200px
- Clear rhythm

### Color System

- **Base colors**: Zinc, Slate, Gray, Neutral, Stone
- **Primary colors**: 17 options (Red → Pink)
- **Chart colors**: `--chart-1` through `--chart-5`
- **Sidebar colors**: `--sidebar`, `--sidebar-primary`, `--sidebar-accent`

### Motion

- Uses **Motion.dev presets** (fade, scale, slide)
- Subtle fade/translate
- Under 200ms (performance-first)
- No dramatic animation
- Subtle, purposeful (not showy)

### Aesthetic References

Aligned with:
- **shadcn/ui** (design language, clarity)
- **Radix UI** (component behavior)
- **reui** (layout structure)
- **HeroUI** (cleanliness)
- **Magic UI** (polish)
- **Vercel** (developer aesthetic)

**NOT Stripe-style gradients. NOT generic SaaS marketing.**

---

## 15. Technical Constraints

### Architecture

- **Built with .NET 10 Blazor Web App** (not .NET 8)
- **Interactive Auto rendering mode**:
  - Fast initial load (Server-side rendering)
  - Rich interactivity (WebAssembly after download)
  - Seamless transition between modes
- **Dual-layer architecture**:
  - `NeoBlazorUI.Primitives` (headless, runs on Server + WASM)
  - `NeoBlazorUI.Components` (styled, runs on Server + WASM)

### Component Requirements

- **Real NeoBlazorUI components only**
- **No mocked UI**
- **No static screenshots**
- Uses **PortalHost architecture** for overlays:
  - `ContainerPortalHost` for inline portals
  - `OverlayPortalHost` for backdrop overlays
- **No JavaScript dependencies** (pure Blazor)
- **No Node.js build tooling** (pre-built CSS ships with NuGet)

### Hydration & Rendering Strategy

- **Use InteractiveAuto render mode** (as documented in project)
- **Add PortalHost components** (ContainerPortalHost + OverlayPortalHost) to layout for overlay components
- **Avoid static rendering** of interactive showcase—all tiles must be fully interactive on load
- **Theme switching** must work seamlessly across Server → WASM transition
- **Test nested dialogs, layered popovers** to prove architectural correctness

**The marketing site must itself prove the system.**

---

## 16. What Must Not Happen

### Generic Risks

❌ **No generic SaaS copy**  
❌ **No documentation clutter**  
❌ **No marketing fluff**  
❌ **No code playground duplication**  
❌ **No broken hydration or flicker**  
❌ **No layout jank**  
❌ **No inconsistency between light and dark mode**

### NeoBlazorUI-Specific Risks

❌ **No confusion about Tailwind**: Site must clearly state "Pre-built CSS included, no Tailwind setup required"  
❌ **No "lite version" perception**: OSS library is fully functional, not a teaser for Pro  
❌ **No shadcn/ui clone accusations**: Emphasize "reimplementation for Blazor/C#" and Radix UI principles  
❌ **No .NET 8 references**: All code examples must use .NET 10 syntax  
❌ **No broken PortalHost examples**: Overlay components must show ContainerPortalHost + OverlayPortalHost setup  
❌ **No duplicate theme switcher**: Use the built-in ThemeSwitcher from NeoBlazorUI (PR #124)

---

## 17. Checkpoint Status

✅ **Positioning:** Locked  
✅ **Brand direction:** Locked  
✅ **Demo philosophy:** Locked  
✅ **OSS/Pro separation:** Locked  
✅ **Visual tone:** Locked  
✅ **Newsletter strategy:** Locked  
✅ **Technical constraints:** Locked  
✅ **Theme system:** Locked (uses built-in ThemeSwitcher)

**NeoUI.io V1 is defined.**

---

## 18. Implementation Checklist

### Phase 1: Foundation
- [ ] Project setup (.NET 10 Blazor Web App, InteractiveAuto)
- [ ] Add NeoBlazorUI.Components NuGet package
- [ ] Configure PortalHost architecture (ContainerPortalHost + OverlayPortalHost)
- [ ] Integrate ThemeSwitcher component from NeoBlazorUI
- [ ] Set up base layout and routing

### Phase 2: Core Sections
- [ ] Hero section (headline, subheadline, CTAs)
- [ ] Vision section (body copy)
- [ ] Why NeoBlazorUI (4-point breakdown)
- [ ] Key Capabilities (feature grid)
- [ ] Open Source First (OSS details)
- [ ] Newsletter form (email capture)
- [ ] Footer (links, social, legal)

### Phase 3: Interactive Showcase
- [ ] Analytics Dashboard tile (LineChart/BarChart)
- [ ] Billing Plan Selector tile (ToggleGroup + Card)
- [ ] Team Management tile (nested Dialog)
- [ ] Data Table tile (sorting, filtering, pagination)
- [ ] Toast Notification tile (programmatic API)
- [ ] Layered Popover tile (Popover + Dialog)
- [ ] Settings Panel tile (Sheet + AlertDialog)

### Phase 4: Polish
- [ ] Theme switching tested across all tiles
- [ ] Hydration validated (Server → WASM transition)
- [ ] Accessibility audit (WCAG 2.1 AA)
- [ ] Performance optimization (Lighthouse score)
- [ ] Mobile responsiveness
- [ ] SEO metadata

### Phase 5: Launch
- [ ] Domain setup (NeoUI.io)
- [ ] Analytics integration
- [ ] Newsletter backend
- [ ] Redirect `/components` to demo site
- [ ] Pro site integration (CTA links)

---

## 19. Reference Links

- **Main README**: [/README.md](../README.md)
- **NeoBlazorUI.Components README**: [/src/BlazorUI.Components/README.md](../src/BlazorUI.Components/README.md)
- **Live Demo**: https://blazoruidemo20251223130817-bch0fhddfkh2bthv.indonesiacentral-01.azurewebsites.net
- **Theme System PR**: [#124](https://github.com/jimmyps/blazor-shadcn-ui/pull/124)
- **shadcn/ui Themes**: https://ui.shadcn.com/themes
- **tweakcn.com**: https://tweakcn.com

---

**End of Specification**