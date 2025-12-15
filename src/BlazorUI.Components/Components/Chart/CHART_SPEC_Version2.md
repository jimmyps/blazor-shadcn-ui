# BlazorUI Charts — New Declarative Chart API Spec (Recharts-inspired, ECharts-backed)

## Purpose (read this first)
This document is the **single source of truth** for Copilot to implement the new chart API in one pass. It includes:
- background and rationale
- final decisions and non-ambiguous defaults
- folder + namespace strategy
- “keep/reuse” guidance for the existing JS/ECharts foundation
- required public surface (roots, primitives, series, fill model)
- demo scenarios for **all chart types**
- explicit mapping guidance to ECharts concepts (where relevant)
- an ambiguity audit to prevent Copilot from guessing

If something is not specified here, treat it as **out of scope** for the initial implementation.

---

## 1) Background / Current State / Problems

### 1.1 Current state (legacy chart API)
The existing charts contain:
- engine-shaped config types (options models) that are closer to ECharts than to user intent,
- inconsistent configuration patterns across chart types,
- a specialized `MultiSeriesLineChart` with a different programming model,
- useful internal foundations that we want to keep (JS integration, CSS variable resolution, script loading, refresh plumbing).

### 1.2 Problems / misalignments
1. **Inconsistent DX**
   - Different chart types (and multi-series) require different configuration approaches.
2. **Hard to document in a shadcn-style**
   - shadcn/Recharts demos are compositional; current API is less discoverable / harder to mirror.
3. **Leaky engine concepts**
   - Public API “feels like ECharts options”, creating cognitive load and harder onboarding.
4. **We still want to keep what already works well**
   - JS/ECharts instance lifecycle, single-flight loading, and runtime CSS variable resolution are solid and should be reused.

### 1.3 Rationale for the new spec
We are implementing a **Recharts-inspired declarative component model** because it yields:
- consistent patterns across charts,
- a highly intuitive and compositional DX,
- documentation examples that match common industry patterns,
- clean internal mapping to ECharts without exposing ECharts option trees in the public API.

---

## 2) Final Decisions (Do not reinterpret)

### 2.1 Namespace & folder strategy (IMPORTANT)
We will **not** use a `.v2` namespace. The new API becomes canonical.

- **New charts (this spec)**
  - Folder: `src/BlazorUI.Components/Components/Chart`
  - Namespace: `BlazorUI.Components.Chart`

- **Legacy/unused charts**
  - Folder: `src/BlazorUI.Components/Components/Old/Chart`
  - Namespace: `BlazorUI.Components.Old.Chart`

### 2.2 Multi-series model
- No `MultiSeriesLineChart` in the new API.
- Multi-series is expressed via multiple `<Line />` children inside `<LineChart />`.

### 2.3 Mixed series policy
- Standard roots are strict (no mixed series types).
- Use `ComposedChart` for mixed series.

### 2.4 Plot area margins must be exposed at the root
- ECharts `grid` controls plot area margins plus label containment.
- We expose this as `Padding` on cartesian/composed roots.

### 2.5 Gridlines are controlled by `<Grid />`
- `<Grid />` is for gridlines only (not plot padding).

### 2.6 Legend API
- Keep Recharts-style trio: `Layout`, `Align`, `VerticalAlign`
- Add `MarginTop` (applies only when `VerticalAlign == Top`)

### 2.7 Tooltip axis pointer
- Must support crosshair via `Tooltip Cursor="Cross"` (when `Mode="Axis"`).

### 2.8 Emphasis / focus
- Emphasis is **disabled by default** for all series.
- Enable per-series via `Emphasis` + `Focus`.

### 2.9 Gradient fills
- Use a per-series `Fill` container with nested `LinearGradient` and `Stop`.

### 2.10 Implicit primitives (Blazor-friendly defaults)
Shared primitives are **created internally with defaults** even if the user does not declare them in markup. Markup is primarily for overriding defaults.

In MVP we support **exactly one** each of:
- `Grid`
- `XAxis`
- `YAxis`
- `Tooltip`
- `Legend`

Secondary axes are deferred.

---

## 3) Keep / Reuse (implementation foundation — explicit)

### 3.1 Keep the existing JS/ECharts integration
Reuse the JS interop and chart lifecycle:
- single-flight ECharts script loading (load once; shared)
- init/create ECharts instances
- set/update options
- dispose on component disposal
- resize handling (responsive)

**Do not introduce** a parallel loader or a second interop system.

### 3.2 Keep JS-side CSS variable resolution (runtime)
CSS variables must be resolved in **JS at runtime** (computed styles), because themes and CSS values are runtime.

- C# is allowed to output tokens like `var(--chart-1)` in the option payload.
- JS must walk the option payload and resolve any `var(...)` strings to actual computed colors before calling `setOption`.
- This applies to:
  - auto series colors (when `Color == null`)
  - any `Color` explicitly containing `var(...)`
  - gradient `Stop.Color` values containing `var(...)`
  - grid strokes containing `var(...)`

### 3.3 Keep data extraction infrastructure for `DataKey`
Reuse existing reflection/cached accessor helpers for resolving `DataKey` values out of `TData`.
- `DataKey` stays a `string` in MVP.
- Do not implement lambda selectors in MVP.

### 3.4 Keep the renderer pipeline shape (re-target inputs)
Reuse the pattern:
- build a chart model snapshot in C#
- produce an ECharts `option` object (or JSON) in C#
- JS resolves CSS vars and calls `setOption`

But: the option inputs now come from the **new declarative primitives**, not old config objects.

### 3.5 Keep robustness/performance details
Retain any existing:
- throttled resize observation
- avoiding excessive JS calls on trivial re-renders
- caching resolved CSS vars if already implemented

---

## 4) Non-Ambiguous Defaults (Explicit numbers)

### 4.1 Root defaults (cartesian + composed roots)
- `Padding` default: `new Padding(top: 32, right: 16, bottom: 24, left: 16)`
- `ContainLabel` default: `true`

Applies to:
- `LineChart`, `AreaChart`, `BarChart`, `ScatterChart`, `ComposedChart`

### 4.2 Legend defaults
- `Show = true`
- `Layout = Horizontal`
- `Align = Center`
- `VerticalAlign = Top`
- `MarginTop = 4`

### 4.3 Grid defaults
- `Show = true`
- `Horizontal = true`
- `Vertical = true`
- `Stroke = null`

### 4.4 Tooltip defaults
- `Show = true`
- default `Mode`:
  - `Axis` for: `LineChart`, `AreaChart`, `BarChart`, `ScatterChart`, `ComposedChart`
  - `Item` for: `PieChart`
- `Cursor = None`

### 4.5 Axis defaults (single axes only)
- `Show = true`
- `DataKey = null` (until set by user)
- `Scale = Auto`
- `AxisLine = true`
- `TickLine = true`

### 4.6 Series defaults
- `Color = null` → auto palette `var(--chart-1)`, `var(--chart-2)`, ...
- `Emphasis = false`
- `Focus = None`

### 4.7 Line defaults
- `ShowDots = false`
- `LineWidth = 2`
- `Dashed = false`

### 4.8 Area defaults
- `ShowDots = false`
- `LineWidth = 2`
- `StackId = null`

---

## 5) Public Types & Enums (Required MVP Surface)

### 5.1 `Padding`
```csharp
public readonly record struct Padding(int Top, int Right, int Bottom, int Left);
```

### 5.2 Enums (names are fixed)
```csharp
public enum AxisScale { Auto, Category, Value, Time, Log }

public enum TooltipMode { Axis, Item }
public enum TooltipCursor { None, Line, Shadow, Cross }

public enum LegendLayout { Horizontal, Vertical }
public enum LegendAlign { Left, Center, Right }
public enum LegendVerticalAlign { Top, Middle, Bottom }

public enum Focus { None, Self }

public enum BarLayout { Vertical, Horizontal }

public enum GradientDirection { Vertical, Horizontal }
```

---

## 6) Components: wrappers (shadcn-like structure)

### 6.1 `ChartContainer`
**Parameters**
- `int Height` (required in demos)
- `bool Responsive = true`
- `string? Class = null`
- `RenderFragment? ChildContent`

### 6.2 `ChartHeader`, `ChartTitle`, `ChartDescription`
- Each takes `RenderFragment? ChildContent`.

---

## 7) Components: chart roots (Required MVP)

All roots are generic `TData` and accept `Data` as `IEnumerable<TData>`.

### 7.1 Common root parameters
- `IEnumerable<TData> Data` (required)
- `RenderFragment? ChildContent`
- `string? Id = null`
- `string? Class = null`

### 7.2 Cartesian/composed root parameters
- `Padding Padding = new Padding(32, 16, 24, 16)`
- `bool ContainLabel = true`

Applies to:
- `LineChart<TData>`
- `AreaChart<TData>`
- `BarChart<TData>`
- `ScatterChart<TData>`
- `ComposedChart<TData>`

### 7.3 Root list
- `LineChart<TData>`
- `AreaChart<TData>`
- `BarChart<TData>` with `BarLayout Layout = BarLayout.Vertical`
- `PieChart<TData>` (no `Padding` in MVP)
- `ScatterChart<TData>`
- `RadarChart<TData>` (no `Padding` in MVP)
- `ComposedChart<TData>`

### 7.4 Implicit primitives (internal creation)
Even if the user does not declare them, the root must behave as if these exist using defaults:
- `Grid`
- `XAxis`
- `YAxis`
- `Tooltip`
- `Legend`

If the user declares a primitive, it overrides the internal default instance.

**MVP supports only one of each** (no secondary axes).

### 7.5 Strict child acceptance rules
- `LineChart`: shared primitives + only `Line`
- `AreaChart`: shared primitives + only `Area`
- `BarChart`: shared primitives + only `Bar`
- `ScatterChart`: shared primitives + only `Scatter`
- `PieChart`: `Tooltip`, `Legend` + only `Pie`
- `RadarChart`: `Tooltip`, `Legend` + only `Radar`
- `ComposedChart`: shared primitives + mix of (`Line`, `Area`, `Bar`, `Scatter`)

---

## 8) Components: shared primitives (Required MVP)

### 8.1 `Grid` (gridlines only)
**Parameters**
- `bool Show = true`
- `bool Horizontal = true`
- `bool Vertical = true`
- `string? Stroke = null`

### 8.2 `XAxis`
**Parameters**
- `bool Show = true`
- `string? DataKey = null`
- `AxisScale Scale = AxisScale.Auto`
- `bool AxisLine = true`
- `bool TickLine = true`

### 8.3 `YAxis`
**Parameters**
- `bool Show = true`
- `string? DataKey = null`
- `AxisScale Scale = AxisScale.Auto`
- `bool AxisLine = true`
- `bool TickLine = true`

### 8.4 `Tooltip`
**Parameters**
- `bool Show = true`
- `TooltipMode Mode` (default depends on chart type; section 4.4)
- `TooltipCursor Cursor = TooltipCursor.None`

### 8.5 `Legend`
**Parameters**
- `bool Show = true`
- `LegendLayout Layout = LegendLayout.Horizontal`
- `LegendAlign Align = LegendAlign.Center`
- `LegendVerticalAlign VerticalAlign = LegendVerticalAlign.Top`
- `int MarginTop = 4` (applies only if `VerticalAlign == Top`)

---

## 9) Components: series primitives (Required MVP)

### 9.1 Common series parameters (`Line`, `Area`, `Bar`)
- `string DataKey` (required)
- `string? Name = null`
- `string? Color = null`
- `bool Emphasis = false`
- `Focus Focus = Focus.None`

### 9.2 `Line`
**Parameters**
- common series params
- `bool ShowDots = false`
- `int LineWidth = 2`
- `bool Dashed = false`

### 9.3 `Area`
**Parameters**
- common series params
- `bool ShowDots = false`
- `int LineWidth = 2`
- `string? StackId = null`
- supports `Fill` child

### 9.4 `Bar`
**Parameters**
- common series params
- `string? StackId = null`
- `int? Radius = null`

### 9.5 `Pie`
**Parameters**
- `string DataKey` (required)
- `string NameKey` (required)
- `string? InnerRadius = null` (supports `"60"` or `"60%"`)
- `string? Name = null`
- `string? Color = null`

### 9.6 `Scatter`
**Parameters**
- `string? Name = null`
- `string? Color = null`
- `bool Emphasis = false`
- `Focus Focus = Focus.None`

> Scatter uses `XAxis.DataKey` and `YAxis.DataKey` for X/Y. No `DataKey` on `Scatter` in MVP.

### 9.7 `Radar`
**Parameters**
- `string DataKey` (required)
- `string? Name = null`
- `string? Color = null`
- `bool Emphasis = false`
- `Focus Focus = Focus.None`
- `bool Fill = false` (required for the demo scenarios)

> Radar category axis key is explicitly deferred (phase 2). MVP may rely on implicit ordering or a fixed key in data; do not invent new public API for it.

---

## 10) Fill / paint components (Required MVP)

### 10.1 `Fill`
**Parameters**
- `string? Color = null`
- `double? Opacity = null`
- `RenderFragment? ChildContent`

### 10.2 `LinearGradient`
**Parameters**
- `GradientDirection Direction = GradientDirection.Vertical`
- `RenderFragment? ChildContent`

### 10.3 `Stop`
**Parameters**
- `double Offset` (0..1)
- `string Color` (required)
- `double? Opacity = null`

---

## 11) `RefreshAsync()` contract (Required MVP)
Every chart root must expose `Task RefreshAsync()`.

**Behavior (exact)**
- Rebuild the full ECharts option in C# from primitives and current `Data`.
- Invoke JS to resolve CSS variables in the option payload.
- Call ECharts `setOption` with the full option.
- Must work even if no parameters/data changed (forced refresh).

---

## 12) Demo & Documentation Scenarios (all chart types)
These examples are the canonical demos for docs and should compile with the MVP surface.

> (Same scenarios as previously reviewed — included inline here for implementation.)
> For brevity in this review copy, keep the “Demo & Documentation Scenarios” section content identical to the last agreed version:
> - Line (5)
> - Area (4)
> - Bar (5)
> - Pie (3)
> - Scatter (3)
> - Radar (2)
> - Composed (3)

(Implementation version of this file must include them fully — do not omit.)

---

## 13) ECharts Mapping Guidance (MVP)

### 13.1 Root plot padding (`Padding`, `ContainLabel`) -> ECharts `grid`
```js
option.grid = {
  top: 32,
  right: 16,
  bottom: 24,
  left: 16,
  containLabel: true
}
// Override with user-provided Padding/ContainLabel.
```

### 13.2 Gridlines (`Grid`) -> axis splitLine
```js
xAxis.splitLine.show = grid.vertical
yAxis.splitLine.show = grid.horizontal
if (grid.stroke) {
  xAxis.splitLine.lineStyle.color = grid.stroke
  yAxis.splitLine.lineStyle.color = grid.stroke
}
```

### 13.3 Legend trio -> ECharts legend placement
```js
legend.orient = layout === "Vertical" ? "vertical" : "horizontal"
legend.left = align === "Left" ? "left" : align === "Right" ? "right" : "center"
legend.top =
  verticalAlign === "Top" ? marginTop /* default 4 */ :
  verticalAlign === "Middle" ? "middle" :
  "bottom"
```

### 13.4 Tooltip mode + cursor -> ECharts tooltip/axisPointer
```js
tooltip.trigger = mode === "Axis" ? "axis" : "item"
tooltip.axisPointer.type =
  cursor === "Cross" ? "cross" :
  cursor === "Line" ? "line" :
  cursor === "Shadow" ? "shadow" :
  "none"
```

### 13.5 Series emphasis + focus -> ECharts series emphasis
```js
series.emphasis = {
  disabled: !emphasis,
  focus: focus === "Self" ? "self" : undefined
}
```

---

## 14) Ambiguity Audit (final)

1. **Implicit primitives**: Grid/XAxis/YAxis/Tooltip/Legend always exist internally with defaults; markup overrides them.
2. **Single axes only**: exactly one X and one Y in MVP; secondary axes deferred.
3. **Padding vs Grid**: Root `Padding` maps to ECharts `grid` margins; `<Grid />` controls gridlines only.
4. **Scatter keys**: X/Y keys come from axes; `Scatter` has no `DataKey` in MVP.
5. **CSS var resolution**: must be JS-side and applies to any string containing `var(`.
6. **RefreshAsync**: forces a full option rebuild+emit and re-resolves CSS vars.
7. **Radar axis key**: deferred; do not invent a new MVP public API for it.
8. **Pie per-slice styling**: deferred; no `Cell` in MVP.

---