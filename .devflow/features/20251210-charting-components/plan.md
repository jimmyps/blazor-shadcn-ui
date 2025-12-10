# Technical Implementation Plan: shadcn-Style Charting Components

**Feature:** 20251210-charting-components
**Status:** Planning
**Architect:** GitHub Copilot
**Created:** 2025-12-10

## 1. Architecture Overview

### Chart System Architecture

The implementation follows a three-layer architecture that separates presentation, rendering abstraction, and theme integration:

```
┌────────────────────────────────────────────────────────┐
│              STYLED COMPONENTS LAYER                     │
│         Components/Charts/ (Chart Components)           │
│  - LineChart, BarChart, PieChart, AreaChart, etc.      │
│  - ChartPanel, ChartLegend, ChartTooltip               │
│  - Applies shadcn styling & theme tokens              │
│  - Compositional API (like shadcn/Recharts)           │
└────────────────────────────────────────────────────────┘
                         ▲
                  Uses Renderer
                         │
┌────────────────────────────────────────────────────────┐
│           RENDERING ABSTRACTION LAYER                   │
│            Services/IChartRenderer                      │
│  - ChartJsRenderer (canvas-based, fast)               │
│  - EChartsRenderer (SVG-based, vector export)         │
│  - Pluggable architecture via interface               │
│  - Engine-specific optimizations                      │
└────────────────────────────────────────────────────────┘
                         ▲
                   Integrates
                         │
┌────────────────────────────────────────────────────────┐
│            THEME INTEGRATION LAYER                      │
│          Services/ChartThemeService                     │
│  - Syncs CSS variables to chart configs               │
│  - MutationObserver for dynamic theme switching        │
│  - OKLCH to RGB color conversion                       │
│  - Light/dark mode support                             │
└────────────────────────────────────────────────────────┘
```

### Alignment with BlazorUI Architecture

This approach maintains consistency with existing BlazorUI patterns:
- **Styled components** following Button, Dialog, Card patterns
- **Service-based architecture** like DropdownManager, PortalService
- **Theme integration** via CSS variables matching existing theming
- **No primitives layer** - charts are opinionated styled components

## 2. Technical Approach

### Phase 1: Foundation Infrastructure (Weeks 1-2)

#### IChartRenderer Interface

```csharp
namespace BlazorUI.Components.Services.Charts;

/// <summary>
/// Abstraction for chart rendering engines.
/// Provides pluggable architecture supporting Chart.js and ECharts.
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
    Line, Bar, Pie, Donut, Radar, Scatter, Bubble, Area
}

public enum ImageFormat
{
    Png, Svg
}
```

#### Chart.js Renderer Implementation

```csharp
namespace BlazorUI.Components.Services.Charts;

/// <summary>
/// Chart.js renderer (canvas-based, ~200KB).
/// Fast rendering, excellent performance, PNG export only.
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

#### ECharts Renderer Implementation

```csharp
namespace BlazorUI.Components.Services.Charts;

/// <summary>
/// ECharts renderer (SVG-based, ~900KB).
/// Vector graphics, PNG and SVG export, rich features.
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
        return new
        {
            color = theme.ChartColors,
            backgroundColor = theme.Background,
            textStyle = new
            {
                color = theme.Foreground,
                fontFamily = theme.FontFamily
            }
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

#### ChartThemeService Implementation

```csharp
namespace BlazorUI.Components.Services.Charts;

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

public interface IChartThemeService
{
    Task<ChartTheme> GetCurrentThemeAsync();
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

### Phase 2: Core Chart Components (Weeks 3-4)

#### Base Chart Component

```csharp
namespace BlazorUI.Components.Charts;

public abstract class ChartBase<TData> : ComponentBase, IAsyncDisposable
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] protected IChartThemeService ThemeService { get; set; } = null!;
    
    [Parameter] public IEnumerable<TData> Data { get; set; } = null!;
    [Parameter] public int Height { get; set; } = 350;
    [Parameter] public ChartEngine Engine { get; set; } = ChartEngine.ChartJs;
    [Parameter] public bool Responsive { get; set; } = true;
    [Parameter] public string? AspectRatio { get; set; }
    [Parameter] public ChartAnimation? Animation { get; set; }
    [Parameter] public bool DisableAnimations { get; set; }
    [Parameter] public bool AnimateOnUpdate { get; set; } = true;
    [Parameter] public int UpdateAnimationDuration { get; set; } = 300;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback<ChartClickEventArgs> OnClick { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public string? Description { get; set; }
    
    protected ElementReference ChartElement;
    protected IChartRenderer? Renderer;
    protected bool IsLoading = true;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Renderer = Engine == ChartEngine.ChartJs 
                ? new ChartJsRenderer(JSRuntime)
                : new EChartsRenderer(JSRuntime);
            
            await InitializeChartAsync();
            IsLoading = false;
            StateHasChanged();
        }
    }
    
    protected abstract Task InitializeChartAsync();
    
    public async ValueTask DisposeAsync()
    {
        if (Renderer != null)
        {
            await Renderer.DisposeAsync();
        }
    }
}

public enum ChartEngine
{
    ChartJs,
    ECharts
}
```

#### LineChart Component

```csharp
namespace BlazorUI.Components.Charts;

public partial class LineChart<TData> : ChartBase<TData>
{
    [Parameter] public string? XDataKey { get; set; }
    [Parameter] public List<LineSeriesConfig> Series { get; set; } = new();
    [Parameter] public bool Curved { get; set; } = false;
    [Parameter] public bool ShowDots { get; set; } = true;
    [Parameter] public bool Filled { get; set; } = false;
    
    protected override async Task InitializeChartAsync()
    {
        var options = new ChartOptions
        {
            Type = ChartType.Line,
            Responsive = Responsive,
            MaintainAspectRatio = !string.IsNullOrEmpty(AspectRatio)
        };
        
        await Renderer!.InitializeAsync(ChartElement, options);
        await Renderer.UpdateDataAsync(PrepareData());
        
        var theme = await ThemeService.GetCurrentThemeAsync();
        await Renderer.ApplyThemeAsync(theme);
    }
    
    private object PrepareData()
    {
        // Transform TData to chart.js format
        // Handle XDataKey, Series configurations
        return new { /* chart data */ };
    }
}

public class LineSeriesConfig
{
    public string DataKey { get; set; } = null!;
    public string? Name { get; set; }
    public string? Stroke { get; set; }
    public int StrokeWidth { get; set; } = 2;
    public bool Dashed { get; set; } = false;
}
```

### Phase 3: Animation System (Week 5)

#### Animation Classes

```csharp
namespace BlazorUI.Components.Charts;

public class ChartAnimation
{
    public bool Enabled { get; set; } = true;
    public int Duration { get; set; } = 750;
    public AnimationEasing Easing { get; set; } = AnimationEasing.EaseInOutQuart;
    public int Delay { get; set; } = 0;
    public AnimationType Type { get; set; } = AnimationType.Default;
}

public enum AnimationEasing
{
    Linear,
    EaseInQuad, EaseOutQuad, EaseInOutQuad,
    EaseInCubic, EaseOutCubic, EaseInOutCubic,
    EaseInQuart, EaseOutQuart, EaseInOutQuart,
    EaseInQuint, EaseOutQuint, EaseInOutQuint,
    EaseInExpo, EaseOutExpo, EaseInOutExpo,
    EaseInBack, EaseOutBack, EaseInOutBack,
    EaseInElastic, EaseOutElastic, EaseInOutElastic,
    EaseInBounce, EaseOutBounce, EaseInOutBounce
}

public enum AnimationType
{
    Default, FadeIn, ScaleIn,
    SlideInLeft, SlideInRight, SlideInTop, SlideInBottom,
    Wave, Expand, Draw
}
```

### JavaScript Integration

#### chart-theme.js

```javascript
let observer = null;
let dotNetRef = null;

export function watchThemeChanges(dotNetReference) {
    dotNetRef = dotNetReference;
    
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
    return oklchToRgb(value); // Convert OKLCH to RGB
}

export function unwatchThemeChanges() {
    if (observer) {
        observer.disconnect();
        observer = null;
    }
}
```

## 3. Implementation Sequence

### Week 1-2: Foundation
1. Create IChartRenderer interface and supporting types
2. Implement ChartJsRenderer with Chart.js integration
3. Implement EChartsRenderer with ECharts integration
4. Implement ChartThemeService with MutationObserver
5. Create JavaScript interop modules (chartjs-renderer.js, echarts-renderer.js, chart-theme.js)
6. Add CSS variables for chart colors to blazorui-input.css
7. Create ChartBase<TData> abstract component
8. Implement ChartPanel wrapper component

### Week 3-4: Core Charts
9. Implement LineChart with multi-series support
10. Implement BarChart with grouped/stacked modes
11. Implement AreaChart with gradient fills
12. Implement PieChart and DonutChart
13. Implement ChartXAxis and ChartYAxis components
14. Implement ChartLine, ChartBar, ChartArea series components
15. Implement ChartTooltip with custom content support
16. Implement ChartLegend with series toggle

### Week 5: Interactivity & Animation
17. Implement animation system (ChartAnimation, AnimationEasing, AnimationType)
18. Add chart-specific animations (line draw, bar expand, pie rotate)
19. Implement prefers-reduced-motion detection
20. Add click/hover event handling
21. Implement crosshair and active marker highlight
22. Add empty state and loading state handling

### Week 6: Advanced Charts
23. Implement RadarChart
24. Implement ScatterChart
25. Implement BubbleChart
26. Add zoom/pan controls for time-series

### Week 7: ECharts Enhancement
27. Test all chart types with ECharts renderer
28. Implement SVG export functionality
29. Add engine selection logic based on data size
30. Performance optimization and testing

### Week 8: Polish & Documentation
31. Comprehensive accessibility testing
32. Create demo pages with real data
33. Write usage documentation for each chart type
34. Create migration guide from Recharts
35. Performance benchmarking and optimization
36. Cross-browser testing

## 4. Testing Strategy

### Component Testing
- Chart initialization with different data sizes
- Theme switching without page reload
- Animation system with all easing/type combinations
- Export functionality (PNG and SVG)
- Event handling (click, hover)

### Accessibility Testing
- Keyboard navigation (Tab, Arrow keys, Enter/Space, Esc)
- Screen reader compatibility (NVDA, JAWS)
- ARIA attributes validation
- Color contrast verification

### Performance Testing
- Chart rendering with 10, 100, 1000, 10000 data points
- Theme switching speed
- Animation performance
- Memory leak detection

### Cross-Browser Testing
- Chrome, Firefox, Safari, Edge (last 2 versions each)
- Responsive behavior on mobile viewports

## 5. Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Bundle size too large | Lazy load engines, tree shake, modular imports |
| Performance degradation | Data throttling, automatic engine selection, virtualization |
| Theme sync complexity | Comprehensive MutationObserver testing, fallback strategies |
| Accessibility gaps | Follow WCAG 2.1 AA strictly, screen reader testing |
| Cross-browser issues | Playwright automated testing, manual verification |

## 6. Success Criteria

- All 7 core chart types working with both engines
- Animation system with 24 easing functions operational
- Theme integration working in light/dark modes
- Accessibility tests passing (WCAG 2.1 AA)
- Performance benchmarks met (<500ms init, <300KB bundle with Chart.js)
- 5+ demo pages with real-world examples
- Documentation complete with API reference

## 7. Dependencies

- Chart.js v4.x (~200KB)
- ECharts v5.x (~900KB, optional)
- Existing BlazorUI infrastructure (ClassNames, theming, PortalHost)

## 8. References

- Architecture document: `.devflow/charting-architecture.md`
- Feature spec: `.devflow/features/20251210-charting-components/spec.md`
- Chart.js: https://www.chartjs.org/
- ECharts: https://echarts.apache.org/
