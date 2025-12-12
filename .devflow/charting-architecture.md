# Charting Architecture for BlazorUI

**Domain:** Data Visualization
**Status:** Implemented (ECharts-only)
**Last Updated:** 2025-12-12
**Tags:** `charting-engine-echarts-2025`

---

## 1. Executive Summary

This document defines the architecture for shadcn-style charting components in BlazorUI, targeting UI/component parity with shadcn/ui's Recharts-based charting solution. The implementation uses ECharts as the sole rendering engine, chosen for its superior visual design, native OKLCH color support, and alignment with modern design systems.

**Key Principles:**
- ECharts as the primary and sole rendering engine (SVG-based)
- Native OKLCH color space support for modern theming
- Mimic shadcn/Recharts compositional developer experience
- Full theme integration with existing CSS variables
- Accessibility-first approach (WCAG 2.1 AA)

**Why ECharts Only:**
- Superior visual output with modern animations and polished themes
- Native support for CSS Level 4 colors (OKLCH), avoiding theming workarounds
- Advanced features (gradients, shadows) work natively without additional code
- SVG rendering aligns with modern frontend UI expectations
- Streamlined codebase with single, well-supported renderer
- Latest v6.0.0 brings improved performance and new features

---

## 2. Architecture Overview

### Two-Layer Architecture Pattern

Following BlazorUI's established architecture (primitives + styled components), charts are implemented as styled components only (no primitives layer needed):

```
┌──────────────────────────────────────────────────────┐
│            STYLED COMPONENTS LAYER                    │
│         Components/Charts/ (Styled Charts)            │
│  - LineChart, BarChart, PieChart, etc.               │
│  - ChartPanel, ChartLegend, ChartTooltip             │
│  - Applies shadcn styling & theme tokens             │
│  - Compositional API (like shadcn/Recharts)         │
└──────────────────────────────────────────────────────┘
                       ▲
                Uses Renderer
                       │
┌──────────────────────────────────────────────────────┐
│              RENDERING LAYER                          │
│            Services/IChartRenderer                    │
│  - ECharts renderer (SVG-based)                      │
│  - Theme synchronization with OKLCH support          │
│  - Export capabilities (PNG/SVG)                     │
└──────────────────────────────────────────────────────┘
                       ▲
                  Integrates
                       │
┌──────────────────────────────────────────────────────┐
│               THEMING INTEGRATION                     │
│          Services/ChartThemeService                   │
│  - Syncs CSS variables to ECharts configs            │
│  - Native OKLCH color support                        │
│  - Dynamic theme switching                           │
│  - Color palette mapping                             │
└──────────────────────────────────────────────────────┘
```

---

## 3. Chart Types & Features

### Core Chart Types (Phase 1)

**1. LineChart**
- Single and multi-series lines
- Filled area support (area chart variant)
- Curved and straight line interpolation
- Data point markers (customizable)
- Gradient fills
- Line styles (solid, dashed, dotted)

**2. BarChart**
- Vertical and horizontal orientations
- Grouped bars (side-by-side)
- Stacked bars (cumulative)
- Custom bar shapes and widths
- Negative value support

**3. AreaChart**
- Stacked areas
- Overlaid areas
- Gradient fills
- Data point markers
- Opacity controls

**4. PieChart & DonutChart**
- Customizable labels (inner, outer, callout)
- Legend integration
- Segment colors from theme
- Hover/click interactions
- Percentage and value displays
- Donut variant with center content slot

**5. RadarChart**
- Multi-axis radar/spider charts
- Customizable axis scales
- Multiple data series
- Filled or line-only modes

**6. ScatterChart**
- X/Y coordinate plotting
- Custom marker shapes and sizes
- Tooltips with data context
- Multiple series support

**7. BubbleChart**
- Multi-dimensional data (x, y, size)
- Color mapping for 4th dimension
- Tooltips with full context
- Size scaling controls

### Advanced Chart Types (Phase 2 - Optional)

- **HeatmapChart** - Color-coded grid for matrix data
- **GaugeChart** - Semi-circular or circular progress indicators
- **ComboChart** - Mixed chart types (line + bar)
- **CandlestickChart** - Financial data visualization
- **TreemapChart** - Hierarchical data rectangles

---

## 4. Component API Design

### Compositional API (shadcn/Recharts-inspired)

Following shadcn's compositional pattern, charts are built from smaller, composable components:

```razor
<ChartPanel Title="Sales Overview" Description="Monthly sales data for 2024">
    <ChartPanelHeader>
        <ChartPanelTitle>Sales Overview</ChartPanelTitle>
        <ChartPanelDescription>Monthly sales data for 2024</ChartPanelDescription>
    </ChartPanelHeader>
    
    <ChartPanelContent>
        <LineChart Data="@salesData" 
                   Height="350"
                   Engine="ChartEngine.ChartJs">
            <ChartXAxis DataKey="month" />
            <ChartYAxis />
            <ChartLine DataKey="sales" 
                      Stroke="var(--chart-1)" 
                      StrokeWidth="2"
                      Curved="true" />
            <ChartLine DataKey="target" 
                      Stroke="var(--chart-2)" 
                      StrokeWidth="2"
                      Dashed="true" />
            <ChartTooltip />
            <ChartLegend />
        </LineChart>
    </ChartPanelContent>
    
    <ChartPanelFooter>
        <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">
            Download PNG
        </Button>
    </ChartPanelFooter>
</ChartPanel>
```

### Core Components

**ChartPanel** - Container with header, content, footer slots
```csharp
[Parameter] public string? Title { get; set; }
[Parameter] public string? Description { get; set; }
[Parameter] public RenderFragment? ChartPanelHeader { get; set; }
[Parameter] public RenderFragment? ChartPanelContent { get; set; }
[Parameter] public RenderFragment? ChartPanelFooter { get; set; }
[Parameter] public string? Class { get; set; }
```

**LineChart** - Line chart component
```csharp
[Parameter] public IEnumerable<TData> Data { get; set; } = null!;
[Parameter] public int Height { get; set; } = 350;
[Parameter] public ChartEngine Engine { get; set; } = ChartEngine.ChartJs;
[Parameter] public bool Responsive { get; set; } = true;
[Parameter] public string? AspectRatio { get; set; } // e.g., "16:9"
[Parameter] public RenderFragment? ChildContent { get; set; }
[Parameter] public EventCallback<ChartClickEventArgs> OnClick { get; set; }
[Parameter] public string? Class { get; set; }
```

**BarChart** - Bar chart component
```csharp
[Parameter] public IEnumerable<TData> Data { get; set; } = null!;
[Parameter] public BarChartOrientation Orientation { get; set; } = BarChartOrientation.Vertical;
[Parameter] public BarChartMode Mode { get; set; } = BarChartMode.Grouped;
[Parameter] public int Height { get; set; } = 350;
[Parameter] public ChartEngine Engine { get; set; } = ChartEngine.ChartJs;
[Parameter] public RenderFragment? ChildContent { get; set; }
[Parameter] public string? Class { get; set; }
```

**PieChart** - Pie/donut chart component
```csharp
[Parameter] public IEnumerable<TData> Data { get; set; } = null!;
[Parameter] public bool IsDonut { get; set; } = false;
[Parameter] public double DonutInnerRadius { get; set; } = 0.6; // 60%
[Parameter] public int Height { get; set; } = 350;
[Parameter] public ChartEngine Engine { get; set; } = ChartEngine.ChartJs;
[Parameter] public RenderFragment? CenterContent { get; set; } // For donut center
[Parameter] public string? Class { get; set; }
```

**ChartXAxis / ChartYAxis** - Axis configuration
```csharp
[Parameter] public string DataKey { get; set; } = null!;
[Parameter] public string? Label { get; set; }
[Parameter] public bool ShowGrid { get; set; } = true;
[Parameter] public bool ShowTicks { get; set; } = true;
[Parameter] public string? TickFormat { get; set; } // e.g., "0.00", "$0,0"
```

**ChartLine / ChartBar / ChartArea** - Data series
```csharp
[Parameter] public string DataKey { get; set; } = null!;
[Parameter] public string? Name { get; set; } // For legend
[Parameter] public string? Stroke { get; set; } // Color
[Parameter] public int StrokeWidth { get; set; } = 2;
[Parameter] public bool Curved { get; set; } = false;
[Parameter] public bool Dashed { get; set; } = false;
[Parameter] public bool ShowDots { get; set; } = true;
```

**ChartTooltip** - Interactive tooltips
```csharp
[Parameter] public RenderFragment<ChartTooltipContext>? Content { get; set; }
[Parameter] public bool ShowCrosshair { get; set; } = false;
[Parameter] public string? Class { get; set; }
```

**ChartLegend** - Chart legend
```csharp
[Parameter] public LegendPosition Position { get; set; } = LegendPosition.Bottom;
[Parameter] public bool ToggleDataSeries { get; set; } = true;
[Parameter] public string? Class { get; set; }
```

### Enums & Types

```csharp
public enum ChartEngine
{
    ChartJs,  // Canvas-based, fast, simple
    ECharts   // SVG-based, rich features, themeable
}

public enum BarChartOrientation
{
    Vertical,
    Horizontal
}

public enum BarChartMode
{
    Grouped,
    Stacked
}

public enum LegendPosition
{
    Top,
    Right,
    Bottom,
    Left
}

public class ChartTooltipContext
{
    public object DataPoint { get; set; } = null!;
    public string Label { get; set; } = null!;
    public Dictionary<string, object> Values { get; set; } = new();
}

public class ChartClickEventArgs
{
    public object DataPoint { get; set; } = null!;
    public int Index { get; set; }
    public string DataKey { get; set; } = null!;
}
```

---

## 5. Rendering Engine Abstraction

### IChartRenderer Interface

```csharp
namespace BlazorUI.Components.Charts;

/// <summary>
/// Abstraction for chart rendering engines.
/// </summary>
public interface IChartRenderer : IAsyncDisposable
{
    /// <summary>
    /// Initializes the chart renderer with the target element.
    /// </summary>
    Task InitializeAsync(ElementReference element, ChartOptions options);
    
    /// <summary>
    /// Updates the chart with new data.
    /// </summary>
    Task UpdateDataAsync(object data);
    
    /// <summary>
    /// Updates chart configuration without recreating the chart.
    /// </summary>
    Task UpdateOptionsAsync(ChartOptions options);
    
    /// <summary>
    /// Applies theme colors to the chart.
    /// </summary>
    Task ApplyThemeAsync(ChartTheme theme);
    
    /// <summary>
    /// Exports the chart as an image (PNG/SVG).
    /// </summary>
    Task<string> ExportAsImageAsync(ImageFormat format);
    
    /// <summary>
    /// Destroys the chart instance and cleans up resources.
    /// </summary>
    Task DestroyAsync();
}

public class ChartOptions
{
    public ChartType Type { get; set; }
    public bool Responsive { get; set; } = true;
    public bool MaintainAspectRatio { get; set; } = true;
    public Dictionary<string, object> EngineSpecificOptions { get; set; } = new();
}

public enum ChartType
{
    Line,
    Bar,
    Pie,
    Donut,
    Radar,
    Scatter,
    Bubble,
    Area
}

public enum ImageFormat
{
    Png,
    Svg
}
```

### Chart.js Renderer Implementation

```csharp
namespace BlazorUI.Components.Charts;

/// <summary>
/// Chart.js renderer implementation (canvas-based).
/// </summary>
public class ChartJsRenderer : IChartRenderer
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _chartInstance;
    
    public ChartJsRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync(ElementReference element, ChartOptions options)
    {
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorUI.Components/js/chartjs-renderer.js");
        
        _chartInstance = await _jsModule.InvokeAsync<IJSObjectReference>(
            "createChart", element, options);
    }
    
    public async Task UpdateDataAsync(object data)
    {
        if (_chartInstance != null)
        {
            await _chartInstance.InvokeVoidAsync("updateData", data);
        }
    }
    
    public async Task UpdateOptionsAsync(ChartOptions options)
    {
        if (_chartInstance != null)
        {
            await _chartInstance.InvokeVoidAsync("updateOptions", options);
        }
    }
    
    public async Task ApplyThemeAsync(ChartTheme theme)
    {
        if (_chartInstance != null)
        {
            await _chartInstance.InvokeVoidAsync("applyTheme", theme);
        }
    }
    
    public async Task<string> ExportAsImageAsync(ImageFormat format)
    {
        if (format != ImageFormat.Png)
        {
            throw new NotSupportedException("Chart.js only supports PNG export");
        }
        
        if (_chartInstance != null)
        {
            return await _chartInstance.InvokeAsync<string>("toBase64Image");
        }
        
        return string.Empty;
    }
    
    public async Task DestroyAsync()
    {
        if (_chartInstance != null)
        {
            await _chartInstance.InvokeVoidAsync("destroy");
            await _chartInstance.DisposeAsync();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await DestroyAsync();
        
        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
    }
}
```

### ECharts Renderer Implementation

```csharp
namespace BlazorUI.Components.Charts;

/// <summary>
/// ECharts renderer implementation (SVG-based).
/// </summary>
public class EChartsRenderer : IChartRenderer
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _chartInstance;
    
    public EChartsRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync(ElementReference element, ChartOptions options)
    {
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorUI.Components/js/echarts-renderer.js");
        
        _chartInstance = await _jsModule.InvokeAsync<IJSObjectReference>(
            "createChart", element, options);
    }
    
    public async Task UpdateDataAsync(object data)
    {
        if (_chartInstance != null)
        {
            await _chartInstance.InvokeVoidAsync("setOption", data);
        }
    }
    
    public async Task UpdateOptionsAsync(ChartOptions options)
    {
        if (_chartInstance != null)
        {
            await _chartInstance.InvokeVoidAsync("setOption", new { option = options });
        }
    }
    
    public async Task ApplyThemeAsync(ChartTheme theme)
    {
        if (_chartInstance != null)
        {
            // ECharts uses JSON theme configuration
            var echartsTheme = MapToEChartsTheme(theme);
            await _chartInstance.InvokeVoidAsync("setTheme", echartsTheme);
        }
    }
    
    public async Task<string> ExportAsImageAsync(ImageFormat format)
    {
        if (_chartInstance != null)
        {
            var type = format == ImageFormat.Svg ? "svg" : "png";
            return await _chartInstance.InvokeAsync<string>("getDataURL", type);
        }
        
        return string.Empty;
    }
    
    private object MapToEChartsTheme(ChartTheme theme)
    {
        // Map BlazorUI theme to ECharts theme JSON format
        return new
        {
            color = theme.ChartColors,
            backgroundColor = theme.Background,
            textStyle = new
            {
                color = theme.Foreground,
                fontFamily = theme.FontFamily
            },
            // ... additional ECharts theme mappings
        };
    }
    
    public async Task DestroyAsync()
    {
        if (_chartInstance != null)
        {
            await _chartInstance.InvokeVoidAsync("dispose");
            await _chartInstance.DisposeAsync();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await DestroyAsync();
        
        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
    }
}
```

---

## 6. Theme Integration

### ChartTheme Model

```csharp
namespace BlazorUI.Components.Charts;

/// <summary>
/// Represents theme configuration for charts.
/// </summary>
public class ChartTheme
{
    public string Background { get; set; } = null!;
    public string Foreground { get; set; } = null!;
    public string Border { get; set; } = null!;
    public string Muted { get; set; } = null!;
    public string MutedForeground { get; set; } = null!;
    public string FontFamily { get; set; } = null!;
    public string[] ChartColors { get; set; } = Array.Empty<string>();
}
```

### ChartThemeService

```csharp
namespace BlazorUI.Components.Charts;

/// <summary>
/// Service that synchronizes CSS variables to chart theme configuration.
/// </summary>
public interface IChartThemeService
{
    /// <summary>
    /// Resolves the current theme from CSS variables.
    /// </summary>
    Task<ChartTheme> GetCurrentThemeAsync();
    
    /// <summary>
    /// Subscribes to theme changes.
    /// </summary>
    event Action<ChartTheme>? OnThemeChanged;
}

public class ChartThemeService : IChartThemeService, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<ChartThemeService>? _dotNetRef;
    
    public event Action<ChartTheme>? OnThemeChanged;
    
    public ChartThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task InitializeAsync()
    {
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorUI.Components/js/chart-theme.js");
        
        _dotNetRef = DotNetObjectReference.Create(this);
        
        // Subscribe to CSS variable changes via MutationObserver
        await _jsModule.InvokeVoidAsync("watchThemeChanges", _dotNetRef);
    }
    
    public async Task<ChartTheme> GetCurrentThemeAsync()
    {
        if (_jsModule == null)
        {
            await InitializeAsync();
        }
        
        return await _jsModule!.InvokeAsync<ChartTheme>("getCurrentTheme");
    }
    
    [JSInvokable]
    public void NotifyThemeChanged(ChartTheme theme)
    {
        OnThemeChanged?.Invoke(theme);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("unwatchThemeChanges");
            await _jsModule.DisposeAsync();
        }
        
        _dotNetRef?.Dispose();
    }
}
```

### JavaScript Theme Bridge

```javascript
// wwwroot/js/chart-theme.js

let observer = null;
let dotNetRef = null;

export function watchThemeChanges(dotNetReference) {
    dotNetRef = dotNetReference;
    
    // Watch for .dark class changes on <html> element
    observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.attributeName === 'class') {
                const theme = getCurrentTheme();
                dotNetRef.invokeMethodAsync('NotifyThemeChanged', theme);
            }
        });
    });
    
    observer.observe(document.documentElement, {
        attributes: true,
        attributeFilter: ['class']
    });
}

export function getCurrentTheme() {
    const styles = getComputedStyle(document.documentElement);
    
    return {
        background: getCssVar(styles, '--background'),
        foreground: getCssVar(styles, '--foreground'),
        border: getCssVar(styles, '--border'),
        muted: getCssVar(styles, '--muted'),
        mutedForeground: getCssVar(styles, '--muted-foreground'),
        fontFamily: getCssVar(styles, '--font-sans'),
        chartColors: [
            getCssVar(styles, '--chart-1'),
            getCssVar(styles, '--chart-2'),
            getCssVar(styles, '--chart-3'),
            getCssVar(styles, '--chart-4'),
            getCssVar(styles, '--chart-5')
        ]
    };
}

function getCssVar(styles, varName) {
    const value = styles.getPropertyValue(varName).trim();
    
    // Convert OKLCH to RGB for chart engines
    if (value.startsWith('oklch(')) {
        return oklchToRgb(value);
    }
    
    // Already RGB or hex
    return value;
}

function oklchToRgb(oklch) {
    // Implement OKLCH to RGB conversion
    // or use a library like culori
    // ...
    return `rgb(0, 0, 0)`; // Placeholder
}

export function unwatchThemeChanges() {
    if (observer) {
        observer.disconnect();
        observer = null;
    }
}
```

### CSS Variables for Charts

Add to `wwwroot/css/blazorui-input.css`:

```css
@layer base {
  :root {
    /* Chart color palette (5 colors for series) */
    --chart-1: oklch(0.70 0.19 255); /* Blue */
    --chart-2: oklch(0.65 0.18 145); /* Green */
    --chart-3: oklch(0.60 0.15 85);  /* Orange */
    --chart-4: oklch(0.68 0.16 320); /* Purple */
    --chart-5: oklch(0.75 0.12 25);  /* Red */
  }

  .dark {
    --chart-1: oklch(0.60 0.19 255);
    --chart-2: oklch(0.55 0.18 145);
    --chart-3: oklch(0.50 0.15 85);
    --chart-4: oklch(0.58 0.16 320);
    --chart-5: oklch(0.65 0.12 25);
  }
}
```

---

## 7. UI/UX Features

### ChartPanel Component

Wrapper component for all charts with consistent shadcn styling:

```razor
@namespace BlazorUI.Components.Charts

<div class="@CssClass">
    @if (ChartPanelHeader != null)
    {
        <div class="flex flex-col space-y-1.5 p-6">
            @ChartPanelHeader
        </div>
    }
    
    <div class="p-6 pt-0">
        @ChartPanelContent
    </div>
    
    @if (ChartPanelFooter != null)
    {
        <div class="flex items-center p-6 pt-0">
            @ChartPanelFooter
        </div>
    }
</div>
```

```csharp
public partial class ChartPanel : ComponentBase
{
    [Parameter] public RenderFragment? ChartPanelHeader { get; set; }
    [Parameter] public RenderFragment? ChartPanelContent { get; set; }
    [Parameter] public RenderFragment? ChartPanelFooter { get; set; }
    [Parameter] public string? Class { get; set; }
    
    private string CssClass => ClassNames.cn(
        "rounded-lg border bg-card text-card-foreground shadow-sm",
        Class
    );
}
```

### Responsive Containers

Charts automatically resize using ResizeObserver:

```javascript
// wwwroot/js/chart-resize.js

export function observeResize(element, chartInstance) {
    const observer = new ResizeObserver((entries) => {
        for (const entry of entries) {
            chartInstance.resize({
                width: entry.contentRect.width,
                height: entry.contentRect.height
            });
        }
    });
    
    observer.observe(element);
    
    return {
        disconnect: () => observer.disconnect()
    };
}
```

### Interactive Tooltips

Custom tooltip with shadcn styling:

```razor
@* ChartTooltip.razor *@
@if (IsVisible && Context != null)
{
    <div class="@TooltipCssClass" style="@TooltipStyle">
        @if (Content != null)
        {
            @Content(Context)
        }
        else
        {
            <div class="text-sm font-medium">@Context.Label</div>
            @foreach (var kvp in Context.Values)
            {
                <div class="flex items-center justify-between gap-4 text-xs">
                    <span class="text-muted-foreground">@kvp.Key:</span>
                    <span class="font-medium">@kvp.Value</span>
                </div>
            }
        }
    </div>
}
```

### Loading & Empty States

```razor
@if (IsLoading)
{
    <div class="flex items-center justify-center" style="height: @(Height)px">
        <Spinner />
    </div>
}
else if (Data == null || !Data.Any())
{
    <Empty Description="No data available" />
}
else
{
    @* Render chart *@
}
```

### Export Functionality

```csharp
private async Task ExportChartAsync()
{
    if (_renderer != null)
    {
        var base64Image = await _renderer.ExportAsImageAsync(ImageFormat.Png);
        
        // Trigger browser download
        await JSRuntime.InvokeVoidAsync(
            "blazorui.downloadFile",
            $"chart-{DateTime.Now:yyyyMMdd-HHmmss}.png",
            base64Image
        );
    }
}
```

---

## 8. Animation System

### Animation Configuration

Charts support rich animations on par with Recharts, with both generic and chart-specific animation types:

```csharp
/// <summary>
/// Animation configuration for charts.
/// </summary>
public class ChartAnimation
{
    /// <summary>
    /// Gets or sets whether animations are enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the animation duration in milliseconds.
    /// </summary>
    public int Duration { get; set; } = 750;
    
    /// <summary>
    /// Gets or sets the animation easing function.
    /// </summary>
    public AnimationEasing Easing { get; set; } = AnimationEasing.EaseInOutQuart;
    
    /// <summary>
    /// Gets or sets the delay before animation starts in milliseconds.
    /// </summary>
    public int Delay { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the animation type for initial render.
    /// </summary>
    public AnimationType Type { get; set; } = AnimationType.Default;
}

/// <summary>
/// Animation easing functions matching Recharts.
/// </summary>
public enum AnimationEasing
{
    /// <summary>Linear animation with constant speed</summary>
    Linear,
    
    /// <summary>Ease in using quadratic function</summary>
    EaseInQuad,
    
    /// <summary>Ease out using quadratic function</summary>
    EaseOutQuad,
    
    /// <summary>Ease in and out using quadratic function</summary>
    EaseInOutQuad,
    
    /// <summary>Ease in using cubic function</summary>
    EaseInCubic,
    
    /// <summary>Ease out using cubic function</summary>
    EaseOutCubic,
    
    /// <summary>Ease in and out using cubic function</summary>
    EaseInOutCubic,
    
    /// <summary>Ease in using quartic function</summary>
    EaseInQuart,
    
    /// <summary>Ease out using quartic function</summary>
    EaseOutQuart,
    
    /// <summary>Ease in and out using quartic function (default)</summary>
    EaseInOutQuart,
    
    /// <summary>Ease in using quintic function</summary>
    EaseInQuint,
    
    /// <summary>Ease out using quintic function</summary>
    EaseOutQuint,
    
    /// <summary>Ease in and out using quintic function</summary>
    EaseInOutQuint,
    
    /// <summary>Ease in using exponential function</summary>
    EaseInExpo,
    
    /// <summary>Ease out using exponential function</summary>
    EaseOutExpo,
    
    /// <summary>Ease in and out using exponential function</summary>
    EaseInOutExpo,
    
    /// <summary>Ease in using back function (overshoots)</summary>
    EaseInBack,
    
    /// <summary>Ease out using back function (overshoots)</summary>
    EaseOutBack,
    
    /// <summary>Ease in and out using back function (overshoots)</summary>
    EaseInOutBack,
    
    /// <summary>Elastic ease in (spring effect)</summary>
    EaseInElastic,
    
    /// <summary>Elastic ease out (spring effect)</summary>
    EaseOutElastic,
    
    /// <summary>Elastic ease in and out (spring effect)</summary>
    EaseInOutElastic,
    
    /// <summary>Bounce ease in</summary>
    EaseInBounce,
    
    /// <summary>Bounce ease out</summary>
    EaseOutBounce,
    
    /// <summary>Bounce ease in and out</summary>
    EaseInOutBounce
}

/// <summary>
/// Animation types for different chart behaviors.
/// </summary>
public enum AnimationType
{
    /// <summary>Default animation for the chart type</summary>
    Default,
    
    /// <summary>Fade in animation</summary>
    FadeIn,
    
    /// <summary>Scale in animation (grow from center)</summary>
    ScaleIn,
    
    /// <summary>Slide in from left animation</summary>
    SlideInLeft,
    
    /// <summary>Slide in from right animation</summary>
    SlideInRight,
    
    /// <summary>Slide in from top animation</summary>
    SlideInTop,
    
    /// <summary>Slide in from bottom animation</summary>
    SlideInBottom,
    
    /// <summary>Wave animation (sequential reveal)</summary>
    Wave,
    
    /// <summary>Expand animation (bars grow from baseline)</summary>
    Expand,
    
    /// <summary>Draw animation (lines/paths draw progressively)</summary>
    Draw
}
```

### Chart-Specific Animation Behaviors

**LineChart Animations:**
```razor
<LineChart Data="@data" Height="350">
    <ChartAnimation Type="AnimationType.Draw" 
                    Duration="1000" 
                    Easing="AnimationEasing.EaseOutCubic" />
    <ChartLine DataKey="sales" />
</LineChart>
```

- **Draw**: Lines draw progressively from left to right
- **FadeIn**: Lines fade in while drawing
- **Wave**: Data points appear sequentially with a wave effect

**BarChart Animations:**
```razor
<BarChart Data="@data" Height="350">
    <ChartAnimation Type="AnimationType.Expand" 
                    Duration="800" 
                    Easing="AnimationEasing.EaseOutQuart" />
    <ChartBar DataKey="value" />
</BarChart>
```

- **Expand**: Bars grow from baseline to their full height
- **SlideInBottom**: Bars slide in from the bottom
- **Wave**: Bars animate sequentially from left to right

**PieChart/DonutChart Animations:**
```razor
<PieChart Data="@data" IsDonut="true">
    <ChartAnimation Type="AnimationType.ScaleIn" 
                    Duration="600" 
                    Easing="AnimationEasing.EaseOutBack" />
</PieChart>
```

- **ScaleIn**: Segments scale from center with rotation
- **FadeIn**: Segments fade in with rotation
- **Wave**: Segments appear sequentially clockwise

**AreaChart Animations:**
```razor
<AreaChart Data="@data" Height="350">
    <ChartAnimation Type="AnimationType.Draw" 
                    Duration="1200" 
                    Easing="AnimationEasing.EaseInOutQuart" />
    <ChartArea DataKey="value" Fill="var(--chart-1)" />
</AreaChart>
```

- **Draw**: Area fills progressively from left to right
- **FadeIn**: Area fades in while filling
- **Expand**: Area expands from baseline

### Component API

```csharp
/// <summary>
/// Base chart component with animation support.
/// </summary>
public abstract class ChartBase<TData> : ComponentBase
{
    /// <summary>
    /// Gets or sets the animation configuration.
    /// </summary>
    [Parameter]
    public ChartAnimation? Animation { get; set; }
    
    /// <summary>
    /// Gets or sets whether to disable animations (respects prefers-reduced-motion).
    /// </summary>
    [Parameter]
    public bool DisableAnimations { get; set; }
    
    /// <summary>
    /// Gets or sets whether to animate on data updates.
    /// </summary>
    [Parameter]
    public bool AnimateOnUpdate { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the animation duration for data updates in milliseconds.
    /// </summary>
    [Parameter]
    public int UpdateAnimationDuration { get; set; } = 300;
}
```

### Usage Examples

**Basic animation with default settings:**
```razor
<LineChart Data="@data" Height="350">
    <ChartLine DataKey="sales" />
</LineChart>
```

**Custom animation configuration:**
```razor
<LineChart Data="@data" Height="350">
    <ChartAnimation 
        Type="AnimationType.Draw" 
        Duration="1500" 
        Easing="AnimationEasing.EaseOutCubic" 
        Delay="200" />
    <ChartLine DataKey="sales" />
</LineChart>
```

**Disable animations:**
```razor
<LineChart Data="@data" Height="350" DisableAnimations="true">
    <ChartLine DataKey="sales" />
</LineChart>
```

**Different animations for multiple series:**
```razor
<LineChart Data="@data" Height="350">
    <ChartAnimation Type="AnimationType.Wave" Duration="1000" />
    <ChartLine DataKey="sales" AnimationDelay="0" />
    <ChartLine DataKey="profit" AnimationDelay="200" />
    <ChartLine DataKey="costs" AnimationDelay="400" />
</LineChart>
```

### Accessibility: Reduced Motion

Respect user's motion preferences:

```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // Check for prefers-reduced-motion
        var prefersReducedMotion = await JSRuntime.InvokeAsync<bool>(
            "window.matchMedia", 
            "(prefers-reduced-motion: reduce)"
        );
        
        if (prefersReducedMotion || DisableAnimations)
        {
            // Disable or shorten animations
            if (Animation != null)
            {
                Animation.Duration = 0;
            }
        }
    }
}
```

CSS media query fallback:

```css
@media (prefers-reduced-motion: reduce) {
    .chart-animation {
        animation-duration: 0ms !important;
        transition-duration: 0ms !important;
    }
}
```

### Animation Implementation Notes

**Chart.js Integration:**
```javascript
// chartjs-renderer.js
export function createChart(element, options) {
    const config = {
        type: options.type,
        data: options.data,
        options: {
            animation: {
                duration: options.animation?.duration || 750,
                easing: mapEasingFunction(options.animation?.easing),
                delay: options.animation?.delay || 0,
                // Chart.js specific animation callbacks
                onProgress: options.animation?.onProgress,
                onComplete: options.animation?.onComplete
            }
        }
    };
    
    return new Chart(element, config);
}

function mapEasingFunction(easing) {
    const easingMap = {
        'Linear': 'linear',
        'EaseInOutQuart': 'easeInOutQuart',
        'EaseOutCubic': 'easeOutCubic',
        // ... map all easing functions
    };
    
    return easingMap[easing] || 'easeInOutQuart';
}
```

**ECharts Integration:**
```javascript
// echarts-renderer.js
export function createChart(element, options) {
    const chart = echarts.init(element);
    
    const option = {
        animation: options.animation?.enabled !== false,
        animationDuration: options.animation?.duration || 750,
        animationEasing: mapEasingFunction(options.animation?.easing),
        animationDelay: options.animation?.delay || 0,
        animationDurationUpdate: options.updateAnimationDuration || 300
    };
    
    chart.setOption(option);
    return chart;
}
```

### Animation Performance

**Optimization strategies:**
1. **Use CSS transforms** where possible (GPU accelerated)
2. **Limit simultaneous animations** to prevent jank
3. **Throttle animations** for large datasets (>500 points)
4. **Progressive rendering** for complex charts

**Performance monitoring:**
```csharp
[Parameter]
public EventCallback<AnimationPerformance> OnAnimationComplete { get; set; }

public class AnimationPerformance
{
    public TimeSpan Duration { get; set; }
    public int FrameCount { get; set; }
    public double AverageFps { get; set; }
}
```

---

## 9. Accessibility

### ARIA Attributes

All charts include proper ARIA labels:

```razor
<div role="img" 
     aria-label="@AriaLabel" 
     aria-describedby="@DescriptionId">
    <canvas @ref="_canvasRef"></canvas>
</div>

@if (!string.IsNullOrEmpty(Description))
{
    <div id="@DescriptionId" class="sr-only">@Description</div>
}
```

### Keyboard Navigation

- **Tab**: Focus the chart container
- **Arrow Keys**: Navigate data points (when focused)
- **Enter/Space**: Trigger click events on focused data point
- **Esc**: Clear selection/focus

### Screen Reader Support

Provide data table alternative:

```razor
<ChartPanel>
    <ChartPanelContent>
        <LineChart Data="@data" AriaLabel="Sales chart">
            @* Chart rendering *@
        </LineChart>
        
        @* Hidden data table for screen readers *@
        <table class="sr-only" aria-label="Sales data">
            <thead>
                <tr>
                    <th>Month</th>
                    <th>Sales</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var item in data)
                {
                    <tr>
                        <td>@item.Month</td>
                        <td>@item.Sales</td>
                    </tr>
                }
            </tbody>
        </table>
    </ChartPanelContent>
</ChartPanel>
```

---

## 10. Implementation Roadmap

### Phase 1: Foundation (Weeks 1-2)

**Infrastructure:**
- [ ] Create `IChartRenderer` interface
- [ ] Implement `ChartJsRenderer` with Chart.js integration
- [ ] Implement `ChartThemeService` with CSS variable syncing
- [ ] Add chart-specific CSS variables to theme
- [ ] Create JavaScript interop modules

**Core Components:**
- [ ] Implement `ChartPanel` wrapper component
- [ ] Implement `ChartPanelHeader`, `ChartPanelTitle`, `ChartPanelDescription`
- [ ] Implement `ChartPanelContent`, `ChartPanelFooter`

### Phase 2: Basic Chart Types (Weeks 3-4)

- [ ] Implement `LineChart` with multi-series support
- [ ] Implement `BarChart` (vertical/horizontal, grouped/stacked)
- [ ] Implement `AreaChart` with gradient fills
- [ ] Implement `PieChart` and `DonutChart`
- [ ] Implement `ChartXAxis` and `ChartYAxis` components
- [ ] Implement `ChartLine`, `ChartBar`, `ChartArea` series components

### Phase 3: Interactivity (Week 5)

- [ ] Implement `ChartTooltip` with custom content support
- [ ] Implement `ChartLegend` with series toggle
- [ ] Add click/hover event handling
- [ ] Implement crosshair on hover
- [ ] Add active marker highlight

### Phase 4: Advanced Charts (Week 6)

- [ ] Implement `RadarChart`
- [ ] Implement `ScatterChart`
- [ ] Implement `BubbleChart`
- [ ] Add zoom/pan controls for time-series

### Phase 5: ECharts Renderer (Week 7)

- [ ] Implement `EChartsRenderer` with SVG support
- [ ] Add engine selection via component parameter
- [ ] Test all chart types with ECharts
- [ ] Implement SVG export functionality

### Phase 6: Polish & Documentation (Week 8)

- [ ] Add loading states and skeletons
- [ ] Implement empty state handling
- [ ] Add export functionality (PNG/SVG download)
- [ ] Comprehensive accessibility testing
- [ ] Create usage documentation and examples
- [ ] Build demo pages with real data
- [ ] Performance optimization and testing

---

## 11. Engine Comparison & Trade-offs

### Chart.js (Default Engine)

**Pros:**
- ✅ Fast rendering with canvas
- ✅ Simple API and configuration
- ✅ Small bundle size (~200KB)
- ✅ Excellent performance with large datasets
- ✅ Wide browser support
- ✅ Active community and ecosystem

**Cons:**
- ❌ Canvas-only (no vector graphics)
- ❌ PNG export only (no SVG)
- ❌ Less themeable than SVG
- ❌ Pixel-perfect rendering challenges at different DPIs

**Best For:**
- Interactive dashboards
- Real-time data visualization
- Large datasets (1000+ points)
- Simple charts with standard requirements

### ECharts (Alternative Engine)

**Pros:**
- ✅ Elegant SVG rendering (vector graphics)
- ✅ Highly themeable with JSON configurations
- ✅ Rich feature set (animations, interactions)
- ✅ SVG and PNG export
- ✅ Beautiful default designs
- ✅ Supports most chart types out-of-the-box

**Cons:**
- ❌ Larger bundle size (~900KB)
- ❌ More complex API
- ❌ SVG performance degrades with huge datasets
- ❌ Theme mapping complexity (CSS vars → JSON)
- ❌ Canvas fallback needed for dense data

**Best For:**
- Print-quality charts (SVG export)
- Complex visualizations (radar, gauge, heatmap)
- Fewer data points (<1000)
- Advanced theming requirements

### Decision Matrix

| Feature                  | Chart.js | ECharts |
|--------------------------|----------|---------|
| Bundle Size              | 200KB    | 900KB   |
| Rendering                | Canvas   | SVG/Canvas |
| Export Formats           | PNG      | PNG/SVG |
| Theme Integration        | ⭐⭐⭐   | ⭐⭐    |
| Performance (1k+ points) | ⭐⭐⭐   | ⭐⭐    |
| Visual Quality           | ⭐⭐     | ⭐⭐⭐  |
| Advanced Features        | ⭐⭐     | ⭐⭐⭐  |
| Learning Curve           | Easy     | Moderate |

**Recommendation:** Start with Chart.js as the default, add ECharts as an opt-in engine for users who need SVG export or advanced features.

---

## 12. Security Considerations

### XSS Prevention

- Always use `@` syntax for rendering user data in tooltips
- Validate and sanitize data before passing to JS engines
- Avoid `MarkupString` for chart labels

### Data Validation

```csharp
private void ValidateData()
{
    if (Data == null)
        throw new ArgumentNullException(nameof(Data));
    
    if (!Data.Any())
        throw new ArgumentException("Chart data cannot be empty", nameof(Data));
    
    // Validate data point structure
    foreach (var item in Data)
    {
        if (item == null)
            throw new ArgumentException("Data points cannot be null", nameof(Data));
    }
}
```

### Safe Event Handlers

```csharp
[JSInvokable]
public async Task HandleChartClick(JsonElement dataPoint)
{
    try
    {
        // Safely deserialize
        var index = dataPoint.GetProperty("index").GetInt32();
        var value = dataPoint.GetProperty("value").GetDouble();
        
        // Invoke user callback
        await OnClick.InvokeAsync(new ChartClickEventArgs
        {
            Index = index,
            DataPoint = value
        });
    }
    catch (Exception ex)
    {
        // Log error, don't expose to user
        Console.Error.WriteLine($"Chart click error: {ex.Message}");
    }
}
```

---

## 13. Performance Optimizations

### Lazy Loading

Load chart libraries only when needed:

```razor
@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Load chart library dynamically
            await JSRuntime.InvokeVoidAsync(
                "import", 
                "./_content/BlazorUI.Components/js/chartjs-bundle.js"
            );
        }
    }
}
```

### Data Throttling

For real-time charts, throttle updates:

```csharp
private DateTime _lastUpdate = DateTime.MinValue;
private const int UpdateIntervalMs = 100;

private async Task UpdateChartData(IEnumerable<DataPoint> newData)
{
    var now = DateTime.UtcNow;
    if ((now - _lastUpdate).TotalMilliseconds < UpdateIntervalMs)
    {
        return; // Throttle updates
    }
    
    _lastUpdate = now;
    await _renderer.UpdateDataAsync(newData);
}
```

### Virtual Scrolling for Large Legends

For charts with many series, use virtual scrolling in legends.

### Canvas vs. SVG Selection

Automatically choose renderer based on data size:

```csharp
private ChartEngine DetermineOptimalEngine()
{
    var dataPointCount = Data.Sum(series => series.Count());
    
    // Use canvas for large datasets
    if (dataPointCount > 1000)
        return ChartEngine.ChartJs;
    
    // Use SVG for smaller datasets (better quality)
    return ChartEngine.ECharts;
}
```

---

## 14. Testing Strategy

### Component Tests

Using bUnit for component testing:

```csharp
[Fact]
public void LineChart_RendersWithData()
{
    // Arrange
    using var ctx = new TestContext();
    var data = new[]
    {
        new { month = "Jan", sales = 100 },
        new { month = "Feb", sales = 150 }
    };
    
    // Act
    var cut = ctx.RenderComponent<LineChart<object>>(parameters => parameters
        .Add(p => p.Data, data)
        .Add(p => p.Height, 350));
    
    // Assert
    cut.Find("canvas").Should().NotBeNull();
}
```

### Visual Regression Tests

Use Playwright for screenshot testing:

```csharp
[Test]
public async Task LineChart_MatchesSnapshot()
{
    await Page.GotoAsync("/charts/line");
    
    var screenshot = await Page.ScreenshotAsync(new()
    {
        FullPage = true
    });
    
    await Verify(screenshot);
}
```

### Accessibility Tests

Automated accessibility testing:

```csharp
[Fact]
public async Task LineChart_MeetsAccessibilityStandards()
{
    using var ctx = new TestContext();
    var cut = ctx.RenderComponent<LineChart<object>>();
    
    // Check ARIA attributes
    cut.Find("[role='img']").Should().NotBeNull();
    cut.Find("[aria-label]").Should().NotBeNull();
}
```

---

## 15. Dependencies

### NuGet Packages

- `Microsoft.JSInterop` (already included in Blazor)
- No additional C# dependencies needed

### JavaScript Libraries

**Chart.js Stack:**
- `chart.js` v4.x (~200KB)
- `chartjs-adapter-date-fns` (for time-series)

**ECharts Stack:**
- `echarts` v5.x (~900KB)
- Consider using modular builds for smaller bundle

### CDN vs. Bundled

**Decision:** Bundle libraries with BlazorUI.Components package
- ✅ Better version control
- ✅ Offline support
- ✅ Consistent behavior
- ❌ Larger initial download

---

## 16. Documentation Requirements

### Usage Examples

For each chart type, provide:
1. Basic example with minimal configuration
2. Advanced example with all features
3. Theme customization example
4. Real-world scenario (dashboard, analytics)

### API Reference

Auto-generated from XML documentation comments:
- Component parameters
- Event callbacks
- Enums and types
- JavaScript interop methods

### Migration Guides

For users familiar with:
- shadcn/ui + Recharts
- Chart.js
- ECharts

Provide mapping guides showing equivalent APIs.

---

## 17. Open Questions & Decisions Needed

### 1. Chart.js vs. ECharts Priority

**Question:** Which engine should be implemented first?

**Recommendation:** Start with Chart.js
- Simpler API for faster MVP
- Better performance for common use cases
- Smaller bundle size
- Add ECharts as Phase 2 for users needing SVG export

### 2. Recharts API Parity

**Question:** How closely should we match Recharts API?

**Recommendation:** Inspired by, not identical
- Use similar compositional pattern
- Adapt naming to C# conventions (PascalCase)
- Add Blazor-specific features (EventCallback, etc.)
- Don't force exact 1:1 mapping if it hurts DX

### 3. Animation Support

**Question:** Should charts have animations by default?

**Decision:** Yes, with opt-out and comprehensive animation system
- ✅ Animations improve UX and data comprehension
- ✅ 24 easing functions matching Recharts (linear, quadratic, cubic, back, elastic, bounce)
- ✅ 10 animation types (fade, scale, slide, wave, expand, draw)
- ✅ Chart-specific animation behaviors (line draw, bar expand, pie rotate)
- ✅ Configurable duration, delay, and easing per chart
- ✅ `DisableAnimations` parameter for opt-out
- ✅ Automatic respect for `prefers-reduced-motion` media query
- ✅ Subtle, fast default animations (750ms with easeInOutQuart)
- ✅ Update animations for data changes (300ms default)
- See Section 8 for complete animation system specification

### 4. Data Binding Strategy

**Question:** Support two-way binding for interactive charts?

**Recommendation:** One-way binding with events
- Use `@bind-SelectedDataPoint` for selected items
- Emit events for user interactions
- Keep data flow unidirectional for predictability

---

## 18. Success Criteria

### Functional

- ✅ All 7 core chart types implemented and functional
- ✅ Full theme integration with CSS variables
- ✅ Dynamic theme switching without page reload
- ✅ Chart.js renderer complete and tested
- ✅ Export to PNG functional

### Non-Functional

- ✅ Bundle size impact < 300KB (with Chart.js)
- ✅ Render time < 500ms for 100-point chart
- ✅ WCAG 2.1 AA accessibility compliance
- ✅ Works in all Blazor hosting models
- ✅ Browser support: Chrome, Firefox, Safari, Edge (last 2 versions)

### Documentation

- ✅ Usage guide for each chart type
- ✅ Theme customization guide
- ✅ API reference documentation
- ✅ 5+ real-world examples
- ✅ Migration guide from Recharts

---

## 19. Future Enhancements

### Phase 3+ (Post-MVP)

**Advanced Chart Types:**
- Heatmap charts
- Gauge/meter charts
- Candlestick charts (financial)
- Treemap charts
- Sankey diagrams
- Network graphs

**Features:**
- Real-time data streaming
- Brush/zoom controls for time-series
- Data annotations and markers
- Chart comparison mode (overlay multiple charts)
- Built-in data transformations (aggregations, moving averages)
- Chart templates/presets

**Developer Experience:**
- CLI command for scaffolding charts
- Design-time preview in IDEs
- Chart builder UI component
- TypeScript definitions for JS interop

---

## 20. References

- [shadcn/ui Charts Documentation](https://ui.shadcn.com/docs/components/chart)
- [Recharts Documentation](https://recharts.org/)
- [Chart.js Documentation](https://www.chartjs.org/)
- [ECharts Documentation](https://echarts.apache.org/)
- [WCAG 2.1 Guidelines for Data Visualization](https://www.w3.org/WAI/WCAG21/quickref/)

---

**Document Version:** 1.0
**Last Review:** 2025-12-09
**Next Review:** After Phase 1 completion
