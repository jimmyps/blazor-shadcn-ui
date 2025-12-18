# Chart Demos (Blazor) — Comprehensive shadcn mirror plan + upstream code references

Upstream reference (pinned commit):
- Folder: https://github.com/shadcn-ui/ui/tree/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts
- Registry (provided by you): `_registry.ts` (copied into this spec)
- GitHub code search for the folder: https://github.com/shadcn-ui/ui/search?q=repo%3Ashadcn-ui%2Fui+path%3Aapps%2Fv4%2Fregistry%2Fnew-york-v4%2Fcharts%2F&type=code

## Page layout requirements (match shadcn)
- Main demo page includes:
  - light/dark mode
  - theme selector
- Tabs by chart type (Area / Bar / Line / Pie / Radar / Radial / Tooltip)
- Each tab:
  - **Hero = the interactive example** for that type
  - Hero includes a **date filter selector** (7d/30d/90d style). If the upstream interactive sample uses a month selector (like Pie), we still add a date selector in our hero wrapper to mirror the global hero convention.
  - Remaining demos in a responsive grid (max 3 columns desktop)

---

## Source registry (authoritative list)
(You provided this list; treat it as canonical.)

### Area (10)
- chart-area-axes → `charts/chart-area-axes.tsx`
- chart-area-default → `charts/chart-area-default.tsx`
- chart-area-gradient → `charts/chart-area-gradient.tsx`
- chart-area-icons → `charts/chart-area-icons.tsx`
- chart-area-interactive → `charts/chart-area-interactive.tsx` (HERO)
- chart-area-legend → `charts/chart-area-legend.tsx`
- chart-area-linear → `charts/chart-area-linear.tsx`
- chart-area-stacked-expand → `charts/chart-area-stacked-expand.tsx`
- chart-area-stacked → `charts/chart-area-stacked.tsx`
- chart-area-step → `charts/chart-area-step.tsx`

### Bar (10)
- chart-bar-active → `charts/chart-bar-active.tsx`
- chart-bar-default → `charts/chart-bar-default.tsx`
- chart-bar-horizontal → `charts/chart-bar-horizontal.tsx`
- chart-bar-interactive → `charts/chart-bar-interactive.tsx` (HERO)
- chart-bar-label-custom → `charts/chart-bar-label-custom.tsx`
- chart-bar-label → `charts/chart-bar-label.tsx`
- chart-bar-mixed → `charts/chart-bar-mixed.tsx`
- chart-bar-multiple → `charts/chart-bar-multiple.tsx`
- chart-bar-negative → `charts/chart-bar-negative.tsx`
- chart-bar-stacked → `charts/chart-bar-stacked.tsx`

### Line (10)
- chart-line-default → `charts/chart-line-default.tsx`
- chart-line-dots-colors → `charts/chart-line-dots-colors.tsx`
- chart-line-dots-custom → `charts/chart-line-dots-custom.tsx`
- chart-line-dots → `charts/chart-line-dots.tsx`
- chart-line-interactive → `charts/chart-line-interactive.tsx` (HERO)
- chart-line-label-custom → `charts/chart-line-label-custom.tsx`
- chart-line-label → `charts/chart-line-label.tsx`
- chart-line-linear → `charts/chart-line-linear.tsx`
- chart-line-multiple → `charts/chart-line-multiple.tsx`
- chart-line-step → `charts/chart-line-step.tsx`

### Pie (11)
- chart-pie-donut-active → `charts/chart-pie-donut-active.tsx`
- chart-pie-donut-text → `charts/chart-pie-donut-text.tsx`
- chart-pie-donut → `charts/chart-pie-donut.tsx`
- chart-pie-interactive → `charts/chart-pie-interactive.tsx` (HERO)
- chart-pie-label-custom → `charts/chart-pie-label-custom.tsx`
- chart-pie-label-list → `charts/chart-pie-label-list.tsx`
- chart-pie-label → `charts/chart-pie-label.tsx`
- chart-pie-legend → `charts/chart-pie-legend.tsx`
- chart-pie-separator-none → `charts/chart-pie-separator-none.tsx`
- chart-pie-simple → `charts/chart-pie-simple.tsx`
- chart-pie-stacked → `charts/chart-pie-stacked.tsx`

### Radar (13)
- chart-radar-default → `charts/chart-radar-default.tsx`
- chart-radar-dots → `charts/chart-radar-dots.tsx`
- chart-radar-grid-circle-fill → `charts/chart-radar-grid-circle-fill.tsx`
- chart-radar-grid-circle-no-lines → `charts/chart-radar-grid-circle-no-lines.tsx`
- chart-radar-grid-circle → `charts/chart-radar-grid-circle.tsx`
- chart-radar-grid-custom → `charts/chart-radar-grid-custom.tsx`
- chart-radar-grid-fill → `charts/chart-radar-grid-fill.tsx`
- chart-radar-grid-none → `charts/chart-radar-grid-none.tsx`
- chart-radar-icons → `charts/chart-radar-icons.tsx`
- chart-radar-label-custom → `charts/chart-radar-label-custom.tsx`
- chart-radar-legend → `charts/chart-radar-legend.tsx`
- chart-radar-lines-only → `charts/chart-radar-lines-only.tsx`
- chart-radar-multiple → `charts/chart-radar-multiple.tsx`
- chart-radar-radius → `charts/chart-radar-radius.tsx`

### Radial (6)
- chart-radial-grid → `charts/chart-radial-grid.tsx`
- chart-radial-label → `charts/chart-radial-label.tsx`
- chart-radial-shape → `charts/chart-radial-shape.tsx`
- chart-radial-simple → `charts/chart-radial-simple.tsx`
- chart-radial-stacked → `charts/chart-radial-stacked.tsx`
- chart-radial-text → `charts/chart-radial-text.tsx`

### Tooltip (8)
- chart-tooltip-default → `charts/chart-tooltip-default.tsx`
- chart-tooltip-indicator-line → `charts/chart-tooltip-indicator-line.tsx`
- chart-tooltip-indicator-none → `charts/chart-tooltip-indicator-none.tsx`
- chart-tooltip-label-none → `charts/chart-tooltip-label-none.tsx`
- chart-tooltip-label-custom → `charts/chart-tooltip-label-custom.tsx`
- chart-tooltip-label-formatter → `charts/chart-tooltip-label-formatter.tsx`
- chart-tooltip-formatter → `charts/chart-tooltip-formatter.tsx`
- chart-tooltip-icons → `charts/chart-tooltip-icons.tsx`
- chart-tooltip-advanced → `charts/chart-tooltip-advanced.tsx`

---

## Upstream code reference links (per example)

> Important: GitHub code search in-chat is limited; where an item below is not inlined, the permalink is still correct given the path.

### Area (Hero: chart-area-interactive)
- chart-area-interactive (HERO):  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-interactive.tsx
- chart-area-axes:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-axes.tsx
- chart-area-default:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-default.tsx
- chart-area-gradient:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-gradient.tsx
- chart-area-icons:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-icons.tsx
- chart-area-legend:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-legend.tsx
- chart-area-linear:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-linear.tsx
- chart-area-stacked-expand:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-stacked-expand.tsx
- chart-area-stacked:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-stacked.tsx
- chart-area-step:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-area-step.tsx

### Bar (Hero: chart-bar-interactive)
- chart-bar-interactive (HERO):  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-interactive.tsx
- chart-bar-active:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-active.tsx
- chart-bar-default:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-default.tsx
- chart-bar-horizontal:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-horizontal.tsx
- chart-bar-label-custom:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-label-custom.tsx
- chart-bar-label:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-label.tsx
- chart-bar-mixed:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-mixed.tsx
- chart-bar-multiple:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-multiple.tsx
- chart-bar-negative:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-negative.tsx
- chart-bar-stacked:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-bar-stacked.tsx

### Line (Hero: chart-line-interactive)
- chart-line-interactive (HERO):  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-interactive.tsx
- chart-line-default:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-default.tsx
- chart-line-dots-colors:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-dots-colors.tsx
- chart-line-dots-custom:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-dots-custom.tsx
- chart-line-dots:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-dots.tsx
- chart-line-label-custom:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-label-custom.tsx
- chart-line-label:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-label.tsx
- chart-line-linear:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-linear.tsx
- chart-line-multiple:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-multiple.tsx
- chart-line-step:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-line-step.tsx

### Pie (Hero: chart-pie-interactive)
- chart-pie-interactive (HERO):  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-interactive.tsx
- chart-pie-donut-active:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-donut-active.tsx
- chart-pie-donut-text:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-donut-text.tsx
- chart-pie-donut:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-donut.tsx
- chart-pie-label-custom:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-label-custom.tsx
- chart-pie-label-list:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-label-list.tsx
- chart-pie-label:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-label.tsx
- chart-pie-legend:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-legend.tsx
- chart-pie-separator-none:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-separator-none.tsx
- chart-pie-simple:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-simple.tsx
- chart-pie-stacked:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-pie-stacked.tsx

### Radar (Hero: none in upstream list; choose chart-radar-radius as best featured)
Upstream has no `chart-radar-interactive`. We still follow the hero rule (“interactive + date selector”) by:
- rendering **chart-radar-radius** as hero content
- adding the same date selector UI in the hero card header (even if it does not affect radar data in v1)

Code references shown/retrieved:
- chart-radar-default:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-default.tsx
- chart-radar-dots:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-dots.tsx
- chart-radar-grid-circle-fill:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-grid-circle-fill.tsx
- chart-radar-grid-circle-no-lines:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-grid-circle-no-lines.tsx
- chart-radar-grid-circle:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-grid-circle.tsx
- chart-radar-grid-custom:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-grid-custom.tsx
- chart-radar-grid-fill:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-grid-fill.tsx
- chart-radar-grid-none:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-grid-none.tsx
- chart-radar-icons:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-icons.tsx
- chart-radar-label-custom:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-label-custom.tsx
- chart-radar-legend:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-legend.tsx
- chart-radar-lines-only:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-lines-only.tsx
- chart-radar-multiple:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-multiple.tsx
- chart-radar-radius:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radar-radius.tsx

### Radial (Hero: none in upstream list; prefer chart-radial-text or chart-radial-stacked as featured)
Code references shown/retrieved:
- chart-radial-grid:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radial-grid.tsx
- chart-radial-label:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radial-label.tsx
- chart-radial-shape:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radial-shape.tsx
- chart-radial-simple:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radial-simple.tsx
- chart-radial-stacked:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radial-stacked.tsx
- chart-radial-text:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-radial-text.tsx

### Tooltip (Hero not required; keep as its own tab)
Code references shown/retrieved:
- chart-tooltip-default:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-default.tsx
- chart-tooltip-indicator-line:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-indicator-line.tsx
- chart-tooltip-indicator-none:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-indicator-none.tsx
- chart-tooltip-label-none:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-label-none.tsx
- chart-tooltip-label-custom:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-label-custom.tsx
- chart-tooltip-label-formatter:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-label-formatter.tsx
- chart-tooltip-formatter:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-formatter.tsx
- chart-tooltip-icons:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-icons.tsx
- chart-tooltip-advanced:  
  https://github.com/shadcn-ui/ui/blob/55f5d1c7ccc140eb66d56885dcdb167f8d1c53fb/apps/v4/registry/new-york-v4/charts/chart-tooltip-advanced.tsx

---

## Notes for our Blazor chart API parity
Some shadcn examples use Recharts-only primitives we may not ship in MVP (e.g., custom dot renderers, cells, polar grids, label lists). For our mirror demos:
- Keep the *layout and scenario* identical (title, description, card layout, selector UX).
- Approximate the visualization using our MVP primitives (and defer unsupported micro-features).
- Always preserve CSS var usage (`var(--chart-1)`… etc.) and demonstrate theme switching.