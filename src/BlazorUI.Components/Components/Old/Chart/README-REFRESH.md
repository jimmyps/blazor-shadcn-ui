# Chart Refresh Registry - Usage Guide

## Overview

The `ChartRefreshRegistry` service provides a mechanism to refresh all charts globally when CSS variables change (e.g., during theme switches). Charts automatically register themselves on initialization and unregister on disposal.

## Service Registration

Add the following to your `Program.cs`:

```csharp
// Add chart refresh registry (singleton or scoped)
builder.Services.AddSingleton<IChartRefreshRegistry, ChartRefreshRegistry>();
```

**Note:** Use `AddSingleton` if you want a single registry across all users/circuits, or `AddScoped` if you want per-circuit/per-user registries.

## Automatic Registration

All chart components (LineChart, BarChart, PieChart, etc.) automatically:
- **Register** themselves with the registry when initialized
- **Unregister** themselves when disposed

No manual registration is required by consumers.

## Manual Refresh

### Individual Chart Refresh

Each chart component exposes a `RefreshAsync()` method:

```razor
@code {
    private LineChart<DataPoint>? chartRef;
    
    private async Task OnThemeChanged()
    {
        if (chartRef != null)
        {
            await chartRef.RefreshAsync();
        }
    }
}

<LineChart @ref="chartRef" Data="@data" ... />
```

### Global Refresh (All Charts)

Inject the registry and call `RefreshAllAsync()`:

```razor
@inject IChartRefreshRegistry ChartRefreshRegistry

@code {
    private async Task OnThemeChanged()
    {
        // Refresh all registered charts at once
        await ChartRefreshRegistry.RefreshAllAsync();
    }
}
```

## What Does Refresh Do?

When `RefreshAsync()` is called:
1. The stored chart configuration is retrieved
2. CSS variables (e.g., `var(--chart-1)`) are **re-resolved** to their current computed values
3. The chart is re-rendered with the new values using `setOption(..., { notMerge: true, lazyUpdate: false })`

This ensures charts reflect theme changes without requiring data updates or component re-initialization.

## Example: Theme Toggle Integration

```razor
@inject IChartRefreshRegistry ChartRefreshRegistry

<Button @onclick="ToggleTheme">Toggle Theme</Button>

<LineChart Data="@salesData" ... />
<BarChart Data="@revenueData" ... />

@code {
    private async Task ToggleTheme()
    {
        // Toggle theme (your implementation)
        await ThemeService.ToggleThemeAsync();
        
        // Refresh all charts to pick up new CSS variable values
        await ChartRefreshRegistry.RefreshAllAsync();
    }
}
```

## Implementation Details

- **Single-flight module loading**: The echarts-renderer.js module is loaded only once, even with multiple charts
- **CSS variable resolution**: The JS layer recursively resolves `var(--*)` patterns before applying to ECharts
- **Minimal JS responsibility**: JS only handles loading, CSS resolution, and applying options - no conversion logic
- **Thread-safe registry**: Uses `ConcurrentDictionary` for safe concurrent access

## Notes

- The registry only tracks charts that are currently mounted/initialized
- Disposed charts are automatically removed from the registry
- If `IChartRefreshRegistry` is not injected, charts still function normally but without global refresh capability
