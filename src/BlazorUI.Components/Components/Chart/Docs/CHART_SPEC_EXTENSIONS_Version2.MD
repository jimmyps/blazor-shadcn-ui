# Chart Spec Extensions (MVP+) — Feature parity for shadcn demos (ECharts-backed, Recharts-style public API)

This document extends `CHART_SPEC.md` with additional **public API** properties needed to mirror shadcn chart examples more closely, while keeping:

- Public API: **Recharts-style declarative components** (Blazor)
- Internal rendering: **single DTO** that mirrors ECharts `option` JSON 1:1 (passthrough to JS)

Scope of this extension:
- (2) Label positioning / labels
- (3) Radial charts (true radial bar / radial grid / stacked radial)
- (5) Step interpolation for line/area
- (6) Percentage stacking (“expand” / 100% stacked)

Explicitly out of scope for this extension:
- (1) Icons in legend/tooltip content (UI templating concern, not ECharts)

---

## 0) Principles (public API)

1) **Recharts-style**: expose developer-friendly knobs similar to Recharts, not ECharts option names.
2) **MVP-friendly**: prefer a small number of high-leverage parameters.
3) **No DTO leakage**: public API must not require users to know ECharts schema.
4) **Composable**: prefer nested components (`<LabelList/>`, `<PolarGrid/>`) over large config objects.
5) **Deterministic defaults**: defaults must be stable and documented.

---

## 0.1 Formatter policy (UPDATED)

Formatter fields are `string` and are passed **1:1** into ECharts options.

A formatter string may be either:
- an ECharts template string (e.g. `"{b}: {c}"`), OR
- a JavaScript function string (e.g. `"function (params) { return params.value + '%'; }"`)

The JS layer is responsible for detecting function-strings and converting them to callable functions before `setOption` (already implemented).

---

# 2) Labels (Line/Area/Bar/Pie/Radar) — Public API

## 2.1 New Components

### 2.1.1 `LabelList` (child of Series components)
Enables per-point labels on a series (similar to Recharts `<LabelList />`).

**Usage (example)**
```razor
<Line DataKey="desktop">
  <LabelList Position="Top" Formatter="function (p) { return p.value; }" />
</Line>
```

**Parameters**
- `Position` : `LabelPosition` (default: `LabelPosition.Top`)
- `Formatter` : `string?` (default: `null`)
  - Template string OR JS function string (passed through to ECharts)
- `Color` : `string?` (default: `null`)
- `FontSize` : `double?` (default: `null`)
- `Offset` : `double?` (default: `null`)
  - High-level knob; mapped where possible.
- `Show` : `bool` (default: `true`)

**Enum: `LabelPosition`**
- `Top`
- `Bottom`
- `Left`
- `Right`
- `Inside`
- `InsideTop`
- `InsideBottom`
- `InsideLeft`
- `InsideRight`
- `Center`
- `Outside`

**Mapping note (internal)**
Internally, the builder maps these to ECharts `series.label.position` values as strings.

---

### 2.1.2 `LabelLine` (Pie only)
Controls the label connector line (for pie labels).

**Usage**
```razor
<Pie DataKey="visitors">
  <LabelList Position="Outside" />
  <LabelLine Show="false" />
</Pie>
```

**Parameters**
- `Show` : `bool` (default: `true`)
- `Length` : `double?` (default: `null`)
- `Length2` : `double?` (default: `null`)
- `Smooth` : `bool?` (default: `null`)

---

## 2.2 New/Extended Series Parameters

### 2.2.1 All cartesian series (`Line`, `Area`, `Bar`, `Scatter`)
- `ShowLabel` : `bool` (default: `false`)
  - Shortcut for enabling a basic label
- `LabelPosition` : `LabelPosition?` (default: `null`)
  - Used when `ShowLabel == true` and no `<LabelList/>` is provided.
- `LabelFormatter` : `string?` (default: `null`)
  - Template string OR JS function string (passed through)

### 2.2.2 Pie series (`Pie`)
- `ShowLabel` : `bool` (default: `false`)
- `LabelPosition` : `LabelPosition?` (default: `null`)
- `LabelFormatter` : `string?` (default: `null`)
- `ShowLabelLine` : `bool` (default: `true`)
  - Shortcut; overridden by `<LabelLine/>` child if present.

---

# 3) Radial Charts (true radial bar) — Public API

We provide a Recharts-like surface that maps internally to ECharts **polar bar** and/or **ring/pie** patterns.

## 3.1 New Root: `RadialBarChart`
Recharts analog: `<RadialBarChart />`

**Usage**
```razor
<RadialBarChart Data="chartData" CategoryKey="browser">
  <PolarGrid GridType="Circle" />
  <RadialBar DataKey="visitors" />
  <Tooltip Formatter="function (p) { return p.name + ': ' + p.value; }" />
</RadialBarChart>
```

**Parameters**
- `Data` : `IEnumerable<TData>` (required)
- `CategoryKey` : `string` (required)
  - Equivalent to the dimension around the circle (angle axis categories)
- `StartAngle` : `double` (default: `-90`)
- `EndAngle` : `double` (default: `270`)
- `InnerRadius` : `string` (default: `"30%"`)
- `OuterRadius` : `string` (default: `"110%"`)
- `Stacked` : `bool` (default: `false`)
  - If true, multiple `<RadialBar/>` children stack

**Children allowed**
- `PolarGrid` (0..1)
- `Tooltip` (0..1)
- `Legend` (0..1)
- `RadialBar` (1..N)
- `CenterLabel` (0..1)

---

## 3.2 New Primitive: `PolarGrid`
Recharts analog: `<PolarGrid />`

**Parameters**
- `GridType` : `PolarGridType` (default: `PolarGridType.Circle`)
  - `Circle` | `Polygon`
- `RadialLines` : `bool` (default: `true`)
- `Stroke` : `string?` (default: `null`)
- `StrokeWidth` : `double?` (default: `null`)
- `Fill` : `string?` (default: `null`)
- `FillOpacity` : `double?` (default: `null`)
- `PolarRadius` : `double[]?` (default: `null`)
  - Allows “ring band” effects

---

## 3.3 Radial Series: `RadialBar`
Recharts analog: `<RadialBar />`

**Parameters**
- `DataKey` : `string` (required)
- `StackId` : `string?` (default: `null`)
- `CornerRadius` : `double?` (default: `null`)
- `Background` : `bool` (default: `false`)
- `Fill` : `string?` (default: `null`)
- `Max` : `double?` (default: `null`)

**Children allowed**
- `LabelList` (0..1)

---

## 3.4 `CenterLabel` (Radial / Pie)
For “radial text” / “donut with text” demos.

**Parameters**
- `Title` : `string?`
- `Value` : `string` (required)
- `ValueClass` : `string?`
- `TitleClass` : `string?`

**Behavior**
- Render as overlay HTML centered inside the chart container (not ECharts text).
- Keeps MVP simple and stable.

---

# 5) Step interpolation (line/area) — Public API

### On `Line` and `Area` series:
- `Interpolation` : `InterpolationType` (default: `InterpolationType.Natural`)

**Enum: `InterpolationType`**
- `Natural`
- `Linear`
- `Step`
- `StepBefore`
- `StepAfter`
- `Monotone`

**Behavior**
Maps to ECharts `series.step`:
- `Step` → `true` (or `"middle"`; pick one and document)
- `StepBefore` → `"start"`
- `StepAfter` → `"end"`

---

# 6) Percentage stacking (100% / expand) — Public API

This is primarily a **data transform** in the builder, then emitted as normal ECharts series data.

## 6.1 Chart-level parameter
### On `AreaChart` (and optionally `BarChart`):
- `StackOffset` : `StackOffset` (default: `StackOffset.None`)

**Enum: `StackOffset`**
- `None`
- `Expand` (normalize each X bucket to 100%)

**Behavior**
When `Expand`:
- For each X bucket, compute the sum of participating stack series
- Emit normalized values `(value / sum) * 100`
- Tooltip/axis can optionally show `%`:
  - via formatter string templates, OR
  - via formatter function strings passed through

## 6.2 Series-level requirement
Only series sharing a `StackId` participate in expand normalization.

---

# Compatibility with the Single DTO Approach
- `LabelList.Formatter` / `Tooltip.Formatter` / axis label formatters: **string passthrough**; can be template or function-string.
- JS layer converts function-string to function as needed before calling `setOption`.
