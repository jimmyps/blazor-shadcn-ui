# Chart Components

Blazor shadcn/ui Chart components provide beautiful, themeable data visualizations with a declarative Recharts-inspired API built on Apache ECharts.

## Overview

The chart library provides 8 chart types with comprehensive customization through declarative primitives, full MVP+ extensions, and shadcn-consistent styling.

**Key Features:**
- ✅ Declarative API (Recharts-inspired)
- ✅ 8 chart types (Line, Area, Bar, Pie, Scatter, Radar, RadialBar, Composed)
- ✅ Full MVP+ extensions (labels, animations, gradients, stacking)
- ✅ ECharts v6. 0. 0 (SVG rendering)
- ✅ CSS variable theming with OKLCH support
- ✅ Type-safe with generic `TData` support

---

## Architecture

### Declarative Component Model

Charts use a declarative composition pattern where you combine: 
1. **Chart Root** - Container that renders the chart (e.g., `LineChart`, `BarChart`)
2. **Primitives** - Configuration components (e.g., `XAxis`, `YAxis`, `Grid`, `Tooltip`, `Legend`)
3. **Series** - Data visualization components (e.g., `Line`, `Area`, `Bar`)
4. **Fill/Paint** - Gradient and styling components (e.g., `Fill`, `LinearGradient`, `Stop`)

**Example:**
```razor
<LineChart Data="@salesData">
    <XAxis DataKey="Month" />
    <YAxis />
    <Grid />
    <Tooltip />
    <Legend />
    <Line DataKey="Sales" Fill="var(--chart-1)" />
    <Line DataKey="Target" Fill="var(--chart-2)" />
</LineChart>
```

### Type Safety

All charts are generic on `TData`:
```razor
<LineChart Data="@List<SalesData>">
    <Line DataKey="Revenue" />  <!-- Type-safe property access -->
</LineChart>
```

---

## Chart Types

### LineChart

Display trends over time with customizable line styles, interpolation, and data labels.

**Features:**
- Step interpolation (Linear, Natural, Step, StepBefore, StepAfter, Monotone)
- Custom symbols (Circle, Rect, Triangle, Diamond, Pin, Arrow, None)
- Data point labels with positioning
- Gradient fills
- Stacking support

**Basic Usage:**
```razor
<LineChart Data="@data">
    <XAxis DataKey="date" />
    <YAxis />
    <Line DataKey="desktop" Fill="var(--chart-1)" />
    <Line DataKey="mobile" Fill="var(--chart-2)" />
</LineChart>
```

**Advanced Usage:**
```razor
<LineChart Data="@data">
    <XAxis DataKey="date">
        <AxisLabel Rotate="-45" FontSize="12" HideOverlap="true" />
    </XAxis>
    <YAxis Min="0" Max="1000" />
    <Grid StrokeWidth="1" StrokeType="LineStyleType.Dashed" Opacity="0.3" />
    <Tooltip Formatter="function(params) { return params.value + ' units'; }" />
    <Line DataKey="sales" 
          Interpolation="InterpolationType.Step"
          DotShape="SymbolShape.Diamond"
          DotSize="8"
          Opacity="0.8"
          ShowLabel="true"
          LabelPosition="LabelPosition.Top" />
</LineChart>
```

---

### AreaChart

Similar to LineChart with filled area below the line.  Supports stacking and percentage stacking.

**Features:**
- Linear gradients
- Stacked areas
- Percentage stacking (100% normalized)
- Step interpolation
- Data labels

**Basic Usage:**
```razor
<AreaChart Data="@data">
    <XAxis DataKey="month" />
    <YAxis />
    <Area DataKey="revenue" Fill="var(--chart-1)" />
</AreaChart>
```

**Stacked with Gradient:**
```razor
<AreaChart Data="@data" StackOffset="StackOffset. Expand">
    <XAxis DataKey="month" />
    <YAxis />
    <Area DataKey="desktop" StackId="1">
        <Fill>
            <LinearGradient Direction="GradientDirection. Vertical">
                <Stop Offset="0" Color="var(--chart-1)" Opacity="0.8" />
                <Stop Offset="1" Color="var(--chart-1)" Opacity="0.1" />
            </LinearGradient>
        </Fill>
    </Area>
    <Area DataKey="mobile" StackId="1">
        <Fill>
            <LinearGradient Direction="GradientDirection.Vertical">
                <Stop Offset="0" Color="var(--chart-2)" Opacity="0.8" />
                <Stop Offset="1" Color="var(--chart-2)" Opacity="0.1" />
            </LinearGradient>
        </Fill>
    </Area>
</AreaChart>
```

---

### BarChart

Compare values across categories with vertical or horizontal bars.  Supports grouping and stacking.

**Features:**
- Vertical/horizontal layouts
- Grouped or stacked bars
- Rounded corners
- Percentage stacking
- Data labels

**Basic Usage:**
```razor
<BarChart Data="@data" Layout="BarLayout.Vertical">
    <XAxis DataKey="category" />
    <YAxis />
    <Bar DataKey="value" Fill="var(--chart-1)" />
</BarChart>
```

**Stacked with Labels:**
```razor
<BarChart Data="@data" Layout="BarLayout.Horizontal">
    <XAxis />
    <YAxis DataKey="product" />
    <Bar DataKey="sales" StackId="1" Radius="4">
        <LabelList Position="LabelPosition.Inside" Formatter="{c}" />
    </Bar>
    <Bar DataKey="returns" StackId="1" Radius="4" />
</BarChart>
```

---

### PieChart

Show composition and proportions.  Supports donut charts with center content.

**Features:**
- Pie and donut variants
- Label lines
- Custom label formatting
- Inner/outer radius control

**Pie Chart:**
```razor
<PieChart Data="@data">
    <Pie DataKey="value" NameKey="category" />
</PieChart>
```

**Donut Chart with Labels:**
```razor
<PieChart Data="@data">
    <Pie DataKey="amount" 
         NameKey="product"
         InnerRadius="60%"
         OuterRadius="80%"
         ShowLabelLine="true">
        <LabelLine Length="10" Length2="20" />
        <LabelList Position="LabelPosition.Outside" 
                   Formatter="{b}:  {c} ({d}%)" />
    </Pie>
</PieChart>
```

---

### ScatterChart

Display correlations between two numerical variables. 

**Features:**
- Custom symbol shapes and sizes
- Trend analysis
- Bubble chart support (via symbol size)

**Usage:**
```razor
<ScatterChart Data="@data">
    <XAxis DataKey="height" />
    <YAxis DataKey="weight" />
    <Scatter DataKey="weight" 
             XAxisDataKey="height"
             SymbolSize="10"
             Symbol="SymbolShape.Circle" />
</ScatterChart>
```

---

### RadarChart

Visualize multivariate data across multiple dimensions. 

**Features:**
- Multiple series comparison
- Polygon or circle grid
- Customizable indicators
- Fill opacity control

**Usage:**
```razor
<RadarChart Data="@skillsData">
    <XAxis DataKey="skill" />
    <Radar DataKey="developer1" Fill="var(--chart-1)" />
    <Radar DataKey="developer2" Fill="var(--chart-2)" />
</RadarChart>
```

---

### RadialBarChart (Gauge Charts)

Circular progress indicators and gauges using polar coordinates.

**Features:**
- Circular/gauge layouts
- Stacked radial bars
- Center labels
- Custom radius ranges

**Usage:**
```razor
<RadialBarChart Data="@progressData">
    <PolarGrid Type="PolarGridType.Circle" />
    <XAxis DataKey="category" />
    <RadialBar DataKey="value" 
               InnerRadius="60%" 
               OuterRadius="80%"
               CornerRadius="10" />
    <CenterLabel Text="75%" FontSize="24" Color="var(--foreground)" />
</RadialBarChart>
```

---

### ComposedChart

Mix multiple chart types (Line, Area, Bar, Scatter) in a single chart.

**Usage:**
```razor
<ComposedChart Data="@data">
    <XAxis DataKey="month" />
    <YAxis />
    <Bar DataKey="sales" Fill="var(--chart-1)" />
    <Line DataKey="target" Stroke="var(--chart-2)" />
    <Area DataKey="profit" Fill="var(--chart-3)" Opacity="0.3" />
</ComposedChart>
```

---

## Primitives

### Grid

Configure chart grid lines. 

**Parameters:**
- `Show` (bool) - Show/hide grid
- `Horizontal` (bool) - Horizontal grid lines
- `Vertical` (bool) - Vertical grid lines
- `Stroke` (string?) - Line color
- `StrokeWidth` (int?) - Line width
- `StrokeType` (LineStyleType) - Solid, Dashed, Dotted
- `Opacity` (double?) - Line opacity

**Usage:**
```razor
<Grid StrokeWidth="2" 
      StrokeType="LineStyleType.Dashed" 
      Stroke="var(--border)" 
      Opacity="0.3" />
```

---

### XAxis / YAxis

Configure chart axes with full customization.

**Parameters:**
- `DataKey` (string) - Property name for axis data
- `Show` (bool) - Show/hide axis
- `Color` (string?) - Axis line color
- `Width` (int?) - Axis line width
- `Min` (double?) - Minimum value
- `Max` (double?) - Maximum value
- `Interval` (int?) - Tick interval
- `Position` (YAxisPosition) - Left/Right (YAxis only)

**AxisLabel Parameters:**
- `Rotate` (double?) - Label rotation in degrees
- `FontSize` (int?) - Font size
- `HideOverlap` (bool) - Hide overlapping labels
- `Formatter` (string?) - Template string or JS function
- `Inside` (bool) - Render inside chart area
- `Margin` (int?) - Distance from axis line
- `Align` (string?) - Horizontal alignment
- `VerticalAlign` (string?) - Vertical alignment

**Usage:**
```razor
<YAxis Position="YAxisPosition.Right" 
       Min="0" Max="100" Interval="10"
       Color="var(--chart-1)" Width="2">
    <AxisLabel Rotate="-45" FontSize="12" HideOverlap="true" />
</YAxis>

<XAxis DataKey="date" Interval="5">
    <AxisLabel FontSize="10" />
</XAxis>
```

---

### Tooltip

Configure tooltip appearance and behavior.

**Parameters:**
- `Mode` (TooltipMode) - Item, Axis
- `Cursor` (TooltipCursor?) - None, Line, Cross, Shadow
- `Formatter` (string?) - Custom formatter (template or JS function)
- `BackgroundColor` (string?) - Background color
- `BorderColor` (string?) - Border color
- `BorderWidth` (int?) - Border width
- `TextColor` (string?) - Text color

**Usage:**
```razor
<Tooltip Mode="TooltipMode.Axis"
         Cursor="TooltipCursor.Cross"
         BackgroundColor="rgba(0,0,0,0.9)"
         BorderColor="var(--chart-1)"
         BorderWidth="2"
         TextColor="#ffffff"
         Formatter="function(params) { return params.value + '%'; }" />
```

---

### Legend

Configure chart legend. 

**Parameters:**
- `Show` (bool) - Show/hide legend
- `Layout` (LegendLayout) - Horizontal, Vertical
- `Align` (LegendAlign) - Left, Center, Right
- `VerticalAlign` (LegendVerticalAlign) - Top, Middle, Bottom
- `Icon` (LegendIcon) - Circle, Rect, RoundRect, Triangle, Diamond, Pin, Arrow, None

**Usage:**
```razor
<Legend Layout="LegendLayout.Horizontal"
        Align="LegendAlign.Center"
        VerticalAlign="LegendVerticalAlign.Bottom"
        Icon="LegendIcon.Circle" />
```

---

## MVP+ Extensions

### Labels (LabelList / LabelLine)

Add data labels to series.

**LabelList (for Line, Area, Bar, Scatter, Radar):**
```razor
<Line DataKey="sales">
    <LabelList Position="LabelPosition.Top"
               Formatter="{c}"
               Color="var(--foreground)"
               FontSize="12"
               Offset="[0, -10]" />
</Line>
```

**LabelLine (for Pie charts):**
```razor
<Pie DataKey="value">
    <LabelLine Show="true" Length="10" Length2="20" Smooth="true" />
</Pie>
```

**LabelPosition Values:**
`Top`, `Bottom`, `Left`, `Right`, `Inside`, `InsideTop`, `InsideBottom`, `InsideLeft`, `InsideRight`, `Center`, `Outside`

---

### Step Interpolation

Control line/area interpolation.

**InterpolationType Values:**
- `Natural` - Smooth curves
- `Linear` - Straight lines
- `Step` - Step at middle
- `StepBefore` - Step before point
- `StepAfter` - Step after point
- `Monotone` - Monotone cubic interpolation

**Usage:**
```razor
<Line DataKey="data" Interpolation="InterpolationType.Step" />
```

---

### Percentage Stacking

Normalize stacked areas/bars to 100%.

**Usage:**
```razor
<AreaChart Data="@data" StackOffset="StackOffset.Expand">
    <Area DataKey="desktop" StackId="1" />
    <Area DataKey="mobile" StackId="1" />
</AreaChart>
```

---

### Gradients

Apply linear gradients to fills.

**Usage:**
```razor
<Area DataKey="revenue">
    <Fill>
        <LinearGradient Direction="GradientDirection.Vertical">
            <Stop Offset="0" Color="var(--chart-1)" Opacity="0.8" />
            <Stop Offset="1" Color="var(--chart-1)" Opacity="0.1" />
        </LinearGradient>
    </Fill>
</Area>
```

**GradientDirection:** `Horizontal`, `Vertical`

---

### Symbol Customization

Customize data point markers.

**SymbolShape Values:**
`Circle`, `Rect`, `RoundRect`, `Triangle`, `Diamond`, `Pin`, `Arrow`, `None`

**Usage:**
```razor
<Line DataKey="data" 
      DotShape="SymbolShape. Diamond" 
      DotSize="8" 
      ShowDots="true" />

<Scatter DataKey="data" 
         Symbol="SymbolShape.Circle" 
         SymbolSize="10" />
```

---

## Wrapper Components

### ChartContainer

Layout wrapper with responsive sizing and CSS variable support.

**Usage:**
```razor
<ChartContainer Config="@chartConfig" Class="h-[400px]">
    <LineChart Data="@data">
        <!-- chart content -->
    </LineChart>
</ChartContainer>
```

---

### ChartHeader / ChartTitle / ChartDescription

Consistent chart headers. 

**Usage:**
```razor
<Card>
    <CardHeader>
        <ChartTitle>Sales Overview</ChartTitle>
        <ChartDescription>Monthly revenue trends</ChartDescription>
    </CardHeader>
    <CardContent>
        <ChartContainer Config="@chartConfig" Class="h-[350px]">
            <LineChart Data="@data">
                <!-- chart content -->
            </LineChart>
        </ChartContainer>
    </CardContent>
</Card>
```

---

## Theming

Charts use CSS variables for consistent theming:

```css
:root {
  --chart-1: oklch(0.646 0.222 41.116);   /* Primary series color */
  --chart-2: oklch(0.708 0.156 195.361);  /* Secondary series color */
  --chart-3: oklch(0.567 0.196 142.515);  /* Tertiary series color */
  --chart-4: oklch(0.833 0.116 93.912);   /* Quaternary series color */
  --chart-5: oklch(0.705 0.184 31.085);   /* Quinary series color */
}
```

**OKLCH Color Support:**
Charts automatically convert OKLCH colors to hex for ECharts compatibility.

**Usage:**
```razor
<Line DataKey="sales" Fill="var(--chart-1)" />
<Area DataKey="profit" Fill="var(--chart-3)" Opacity="0.6" />
```

---

## Animation

*Coming soon - Animation support is planned for the next release.*

Expected API:
```razor
<LineChart Data="@data" 
           EnableAnimation="true"
           AnimationDuration="1000"
           AnimationEasing="AnimationEasing.ElasticOut">
    <Line DataKey="sales" 
          AnimationDuration="2000"  <!-- Series-level override -->
          AnimationEasing="AnimationEasing.BounceOut" />
</LineChart>
```

---

## Common Parameters

All chart roots support: 

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Data` | `IEnumerable<TData>` | Required | Data to display |
| `Class` | `string?` | null | Additional CSS classes |
| `ChildContent` | `RenderFragment?` | null | Declarative chart content |

---

## Technical Details

### Rendering Engine

- **ECharts v6. 0.0** - SVG rendering with high-quality vector output
- Loaded dynamically from CDN (single-flight loading prevents duplicates)
- Runtime CSS variable resolution with OKLCH→hex conversion

### Data Extraction

Charts use reflection-based `DataKey` property access with caching for performance.

**Example:**
```csharp
public class SalesData
{
    public string Month { get; set; }  // Used by XAxis DataKey="Month"
    public double Sales { get; set; }   // Used by Line DataKey="Sales"
}
```

### Resource Management

All charts implement `IAsyncDisposable` for proper cleanup of ECharts instances.

### RefreshAsync()

Programmatically refresh chart rendering: 
```csharp
@ref LineChart<SalesData> chartRef;

await chartRef.RefreshAsync();
```

---

## Examples

See the demo pages for 68 comprehensive examples:
- `/components/charts/area` - 11 area chart variations
- `/components/charts/bar` - 10 bar chart variations
- `/components/charts/line` - 10 line chart variations
- `/components/charts/pie` - 11 pie chart variations
- `/components/charts/scatter` - Scatter plot examples
- `/components/charts/radar` - Radar chart examples
- `/components/charts/composed` - Mixed chart examples

---

## Migration from Legacy API

Legacy charts have been moved to `BlazorUI.Components.Old. Chart` namespace. The new declarative API is the recommended approach for all new development.

**Key Differences:**
- Declarative composition vs configuration objects
- Generic `TData` type safety
- Primitive-based customization
- ECharts-only (no Chart. js)
- MVP+ extensions built-in

---

## Specification Documents

For detailed technical specifications, see:
- `docs/CHART_SPEC_Version2.md` - Public API specification
- `docs/CHART_SPEC_Internal.md` - Internal ECharts DTO mapping
- `docs/CHART_SPEC_EXTENSIONS_Version2.MD` - MVP+ extensions spec
- `docs/CHART_DEMOS_Version3.md` - Demo page specifications
