# Feature Specification: shadcn-Style Charting Components

**Status:** Pending
**Created:** 2025-12-10T00:00:00Z
**Feature ID:** 20251210-charting-components

## Problem Statement

BlazorUI currently lacks charting capabilities to visualize data, which are essential for modern web applications displaying analytics, dashboards, and data-driven interfaces. Users coming from shadcn/ui expect similar charting components with Recharts-like compositional APIs. Without native charting support, developers must integrate third-party solutions manually, losing the benefit of theme integration, consistent styling, and the compositional patterns established in BlazorUI.

## Goals and Objectives

- Provide shadcn-style charting components with compositional, declarative API matching Recharts patterns
- Support 7 core chart types (Line, Bar, Area, Pie/Donut, Radar, Scatter, Bubble) with extensibility for advanced types
- Implement pluggable rendering engine architecture supporting Chart.js (canvas) and ECharts (SVG)
- Achieve seamless theme integration with automatic CSS variable synchronization for light/dark mode
- Deliver comprehensive animation system with 24 easing functions and 10 animation types
- Ensure WCAG 2.1 AA accessibility compliance with ARIA labels, keyboard navigation, and screen reader support
- Optimize performance with lazy loading, data throttling, and automatic engine selection
- Maintain security best practices with XSS prevention and data validation

## User Stories

1. **As a developer building a dashboard**, I want LineChart, BarChart, and AreaChart components, so that I can visualize time-series and categorical data with minimal configuration.

2. **As a developer**, I want charts that automatically adapt to my app's theme (light/dark mode), so that charts match my design system without manual color configuration.

3. **As a developer**, I want composable chart components like ChartPanel, ChartXAxis, ChartLegend, so that I can build complex visualizations by combining smaller components following Blazor patterns.

4. **As a developer**, I want customizable animations with easing functions, so that I can create engaging data visualizations with smooth transitions matching my brand.

5. **As an analytics developer**, I want PieChart and DonutChart with interactive legends and tooltips, so that I can display proportional data with rich interactivity.

6. **As a developer handling large datasets**, I want the framework to automatically choose the best rendering engine (canvas vs SVG), so that performance remains optimal without manual optimization.

7. **As an accessibility-focused developer**, I want charts with ARIA labels and keyboard navigation, so that my visualizations are accessible to all users including screen reader users.

8. **As a developer**, I want to export charts as PNG or SVG images, so that users can save and share visualizations.

## Acceptance Criteria

### Foundation Infrastructure
- [ ] IChartRenderer interface created with InitializeAsync, UpdateDataAsync, UpdateOptionsAsync, ApplyThemeAsync, ExportAsImageAsync, DestroyAsync methods
- [ ] ChartJsRenderer implementation created wrapping Chart.js with canvas rendering
- [ ] EChartsRenderer implementation created wrapping ECharts with SVG rendering
- [ ] ChartThemeService created to sync CSS variables to chart configurations
- [ ] JavaScript interop modules created (chartjs-renderer.js, echarts-renderer.js, chart-theme.js, chart-resize.js)
- [ ] CSS variables added for chart colors (--chart-1 through --chart-5) in light and dark themes
- [ ] ChartAnimation, AnimationEasing, and AnimationType classes created

### Core Chart Types
- [ ] LineChart component implemented with single/multi-series support
- [ ] LineChart supports curved and straight lines with data point markers
- [ ] LineChart supports filled area mode (AreaChart variant)
- [ ] BarChart component implemented with vertical and horizontal orientations
- [ ] BarChart supports grouped and stacked modes
- [ ] AreaChart component implemented with stacked and overlaid modes
- [ ] AreaChart supports gradient fills
- [ ] PieChart component implemented with labels and legends
- [ ] DonutChart variant created with center content slot and configurable inner radius
- [ ] RadarChart component implemented with multi-axis support
- [ ] ScatterChart component implemented with custom markers
- [ ] BubbleChart component implemented with multi-dimensional data support

### Compositional API Components
- [ ] ChartPanel wrapper component created with header, content, footer slots
- [ ] ChartPanelHeader, ChartPanelTitle, ChartPanelDescription components created
- [ ] ChartXAxis and ChartYAxis components created with grid, ticks, label support
- [ ] ChartLine, ChartBar, ChartArea series components created
- [ ] ChartTooltip component created with custom content support and theme styling
- [ ] ChartLegend component created with series toggle functionality and position options

### Animation System
- [ ] ChartAnimation component supports all 24 easing functions (Linear, EaseInQuad through EaseInOutBounce)
- [ ] ChartAnimation supports all 10 animation types (Default, FadeIn, ScaleIn, SlideIn variants, Wave, Expand, Draw)
- [ ] Chart-specific animations implemented (LineChart draw, BarChart expand, PieChart rotate)
- [ ] DisableAnimations parameter implemented for opt-out
- [ ] Automatic prefers-reduced-motion detection implemented
- [ ] AnimateOnUpdate parameter implemented for data change animations
- [ ] Animation performance monitoring with OnAnimationComplete callback

### Theme Integration
- [ ] ChartThemeService syncs CSS variables on initialization
- [ ] MutationObserver watches for theme class changes on <html> element
- [ ] Theme changes trigger automatic chart re-theming without page reload
- [ ] OKLCH to RGB color conversion implemented for chart engines
- [ ] Charts work correctly in both light and dark themes

### Interactivity & Features
- [ ] Interactive tooltips with hover support and custom formatting
- [ ] Crosshair on hover for time-series charts
- [ ] Legend toggle for series visibility
- [ ] Active marker highlight on hover
- [ ] Click events implemented with ChartClickEventArgs
- [ ] Empty state handling with Empty component integration
- [ ] Loading state handling with Spinner component integration
- [ ] PNG export functionality implemented
- [ ] SVG export functionality implemented (ECharts only)

### Accessibility
- [ ] All charts have role="img" with aria-label
- [ ] Charts have aria-describedby with description text
- [ ] Hidden data tables provided for screen readers
- [ ] Keyboard navigation implemented (Tab, Arrow keys, Enter/Space, Esc)
- [ ] Focus indicators visible on chart containers
- [ ] ARIA attributes tested with screen readers (NVDA/JAWS)

### Performance
- [ ] Chart libraries loaded lazily on first use
- [ ] Data throttling implemented for real-time updates (100ms default)
- [ ] Automatic engine selection based on data point count (>1000 points → Chart.js)
- [ ] ResizeObserver used for responsive container sizing
- [ ] Disposal patterns implemented with IAsyncDisposable

### Testing & Quality
- [ ] Charts tested in Blazor Server
- [ ] Charts tested in Blazor WebAssembly
- [ ] Charts tested with 10, 100, 1000, 10000 data points for performance
- [ ] Theme switching tested without page reload
- [ ] Animation system tested with different easing and types
- [ ] Export functionality tested (PNG and SVG)
- [ ] Accessibility tested with keyboard-only navigation
- [ ] Cross-browser testing completed (Chrome, Firefox, Safari, Edge)

### Documentation
- [ ] Usage documentation created for each chart type with examples
- [ ] Theme customization guide created
- [ ] Animation system documentation with all easing/type combinations
- [ ] API reference documentation generated from XML comments
- [ ] Migration guide from Recharts created for React developers
- [ ] Performance optimization guide created
- [ ] Demo pages created with real-world data examples

## Technical Requirements

### Architecture
- **Two-layer pattern:** Styled components (LineChart, BarChart, etc.) → IChartRenderer abstraction → JavaScript engines
- **Rendering engines:** Chart.js (canvas, default) and ECharts (SVG, optional) with pluggable interface
- **Theme service:** ChartThemeService with CSS variable synchronization and MutationObserver
- **State management:** Parameter-driven with EventCallback for interactions
- **Composition:** RenderFragment parameters for flexible component composition (ChartLine, ChartBar inside charts)

### Folder Structure
```
src/BlazorUI.Components/
├── Components/
│   └── Charts/                           # NEW: Chart components
│       ├── LineChart.razor
│       ├── LineChart.razor.cs
│       ├── BarChart.razor
│       ├── BarChart.razor.cs
│       ├── AreaChart.razor
│       ├── PieChart.razor
│       ├── RadarChart.razor
│       ├── ScatterChart.razor
│       ├── BubbleChart.razor
│       ├── ChartPanel.razor              # Container component
│       ├── ChartXAxis.razor              # Axis components
│       ├── ChartYAxis.razor
│       ├── ChartLine.razor               # Series components
│       ├── ChartBar.razor
│       ├── ChartArea.razor
│       ├── ChartTooltip.razor            # Interactive components
│       ├── ChartLegend.razor
│       ├── ChartAnimation.razor          # Animation configuration
│       ├── ChartBase.cs                  # Base class
│       ├── ChartEngine.cs                # Engine enum
│       ├── ChartOptions.cs               # Configuration classes
│       ├── ChartAnimation.cs
│       ├── AnimationEasing.cs
│       ├── AnimationType.cs
│       └── ChartEventArgs.cs
│
├── Services/                             # NEW: Chart services
│   ├── Charts/
│   │   ├── IChartRenderer.cs
│   │   ├── ChartJsRenderer.cs
│   │   ├── EChartsRenderer.cs
│   │   ├── IChartThemeService.cs
│   │   └── ChartThemeService.cs
│
└── wwwroot/
    ├── js/
    │   ├── chartjs-renderer.js           # NEW: Chart.js wrapper
    │   ├── echarts-renderer.js           # NEW: ECharts wrapper
    │   ├── chart-theme.js                # NEW: Theme synchronization
    │   └── chart-resize.js               # NEW: Responsive sizing
    │
    └── css/
        └── blazorui-input.css            # MODIFIED: Add chart CSS variables
```

### JavaScript Dependencies
- **Chart.js v4.x** (~200KB) - Canvas-based rendering
- **ECharts v5.x** (~900KB, optional) - SVG-based rendering
- Both bundled with BlazorUI.Components package (not CDN)

### Hosting Model Compatibility
- **Blazor Server:** Full support (primary testing target)
- **Blazor WebAssembly:** Full support with lazy loading
- **Blazor Hybrid (MAUI):** Full support

### Browser Support
- Chrome (last 2 versions)
- Firefox (last 2 versions)
- Safari (last 2 versions)
- Edge (last 2 versions)

## Non-Functional Requirements

### Performance
- Chart initialization < 500ms for 100 data points
- Data update animation < 300ms
- Theme switch < 200ms
- Memory cleanup on component disposal to prevent leaks
- Bundle size impact < 300KB with Chart.js, < 1MB with both engines

### Security
- No MarkupString usage in tooltips or labels
- HTML encoding via @ syntax for all user data
- Data validation before JS interop calls
- Safe event handler patterns with try-catch

### Accessibility
- WCAG 2.1 AA compliance
- Screen reader tested
- Keyboard navigation complete
- prefers-reduced-motion support

## Success Metrics

- [ ] All 7 core chart types implemented and functional
- [ ] Animation system with 24 easing functions and 10 types working
- [ ] Theme integration working in light and dark modes
- [ ] Accessibility tests passing (keyboard, screen reader)
- [ ] Performance benchmarks met (500ms init, <300KB bundle)
- [ ] 5+ demo pages with real-world examples created
- [ ] Migration guide from Recharts completed

## Out of Scope (Future Enhancements)

- HeatmapChart, GaugeChart, ComboChart (Phase 2)
- Real-time data streaming with WebSocket integration
- Brush/zoom controls for time-series
- Data annotations and markers
- Chart comparison mode
- CLI command for scaffolding charts
- Chart builder UI component

## Dependencies

- Existing: ClassNames utility, theme CSS variables, PortalHost for overlays
- New: Chart.js, ECharts JavaScript libraries
- No additional NuGet packages required

## Risk Assessment

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Chart.js/ECharts API changes | High | Low | Pin to specific versions, abstract behind IChartRenderer |
| Performance with large datasets | High | Medium | Implement data throttling, automatic engine selection, virtualization |
| Theme synchronization complexity | Medium | Medium | Use MutationObserver, test thoroughly in both modes |
| Bundle size impact | Medium | High | Lazy load engines, tree shake unused features, modular imports |
| Accessibility gaps | High | Low | Follow WCAG 2.1 AA, test with screen readers, keyboard navigation |
| Cross-browser rendering differences | Medium | Medium | Test in all supported browsers, use Playwright for automation |

## References

- Architecture document: `.devflow/charting-architecture.md`
- shadcn/ui charts: https://ui.shadcn.com/docs/components/chart
- Recharts documentation: https://recharts.org/
- Chart.js documentation: https://www.chartjs.org/
- ECharts documentation: https://echarts.apache.org/
- WCAG 2.1 Guidelines: https://www.w3.org/WAI/WCAG21/quickref/
