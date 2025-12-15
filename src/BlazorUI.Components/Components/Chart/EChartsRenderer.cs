using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.Chart;

/// <summary>
/// ECharts renderer implementation (SVG-based).
/// </summary>
public class EChartsRenderer : IChartRenderer
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _jsModule;
    
    // Static module reference and lock for single-flight loading across all instances
    private static IJSObjectReference? _sharedJsModule;
    private static readonly SemaphoreSlim _moduleLock = new(1, 1);
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public EChartsRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    /// <summary>
    /// Ensures the echarts-renderer.js module is loaded only once across all chart instances.
    /// </summary>
    private async Task<IJSObjectReference> EnsureModuleLoadedAsync()
    {
        // If instance already has module, return it
        if (_jsModule != null)
            return _jsModule;
        
        // Check if shared module is already loaded
        if (_sharedJsModule != null)
        {
            _jsModule = _sharedJsModule;
            return _jsModule;
        }
        
        // Single-flight loading: only one instance loads the module
        await _moduleLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_sharedJsModule != null)
            {
                _jsModule = _sharedJsModule;
                return _jsModule;
            }
            
            // Load the module once
            _sharedJsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/BlazorUI.Components/js/echarts-renderer.js");
            
            _jsModule = _sharedJsModule;
            return _jsModule;
        }
        finally
        {
            _moduleLock.Release();
        }
    }
    
    public async Task<string> InitializeAsync(ElementReference element, ChartConfig config)
    {
        var module = await EnsureModuleLoadedAsync();
        
        // Convert universal ChartConfig to ECharts v6 format
        var echartsConfig = ConvertToEChartsFormat(config);
        
        // Serialize with camelCase for JavaScript
        var json = JsonSerializer.Serialize(echartsConfig, JsonOptions);
        var normalizedConfig = JsonSerializer.Deserialize<object>(json, JsonOptions);
        
        var chartId = await module.InvokeAsync<string>("createChart", element, normalizedConfig);
        return chartId;
    }
    
    /// <summary>
    /// Converts universal ChartConfig to ECharts v6 option format.
    /// </summary>
    private object ConvertToEChartsFormat(ChartConfig config)
    {
        var chartType = config.Type.ToString().ToLowerInvariant();
        
        // Build ECharts configuration based on chart type (using strongly-typed data)
        return chartType switch
        {
            "line" => ConvertLineChart(config.Data, config.Options),
            "bar" => ConvertBarChart(config.Data, config.Options),
            "pie" or "donut" => ConvertPieChart(config.Data, config.Options),
            "radar" => ConvertRadarChart(config.Data, config.Options),
            "scatter" => ConvertScatterChart(config.Data, config.Options),
            _ => new { }
        };
    }
    
    private object MapLegendConfig(LegendConfig legend)
    {
        var position = legend.Position?.ToLowerInvariant();
        
        object? top = null;
        object? left = null;
        var orient = legend.Orient;
        
        switch (position)
        {
            case "top":
            case "bottom":
                top = position;
                left ??= "center";
                break;
            case "left":
            case "right":
                left = position;
                // 'middle' is the documented ECharts value for vertical centering
                top ??= "middle";
                // Side legends should stack vertically; keep existing vertical orientation if already set
                orient = orient == Orient.Horizontal ? Orient.Vertical : orient;
                break;
        }
        
        top ??= legend.Top;
        left ??= legend.Left;
        
        return new
        {
            show = legend.Display,
            top,
            left,
            orient = orient.ToString().ToLowerInvariant(),
            icon = legend.Icon.ToString().ToLowerInvariant()
        };
    }
    
    private object ConvertLineChart(ChartData data, ChartOptions options)
    {
        // Use strongly-typed properties instead of dynamic access
        var labels = data.Labels;
        var datasets = data.Datasets;
        
        var series = new List<object>();
        for (int i = 0; i < datasets.Length; i++)
        {
            var ds = datasets[i];
            
            // Auto-assign color based on series index (1-based for CSS variables per spec)
            var seriesIndex = i + 1;
            var defaultColor = $"var(--chart-{seriesIndex})";
            
            // Build series item with proper ECharts v6 structure
            var seriesItem = new Dictionary<string, object>
            {
                ["type"] = "line",
                ["name"] = ds.Label ?? $"Series {seriesIndex}",
                ["data"] = ds.Data
            };
            
            // Dashboard defaults for line charts (spec 6.2):
            // - smooth: true (unless explicitly set via tension)
            // - showSymbol: false (unless explicitly set via pointRadius > 0)
            // - lineStyle.width: 2
            
            var smooth = ds.Tension.HasValue ? (ds.Tension.Value > 0) : true; // Default true per spec
            seriesItem["smooth"] = smooth;
            
            var showSymbol = ds.PointRadius.HasValue ? (ds.PointRadius.Value > 0) : false; // Default false per spec
            seriesItem["showSymbol"] = showSymbol;
            seriesItem["symbolSize"] = ds.PointRadius ?? 4;
            
            // Line style with dashboard default width of 2
            var lineStyle = new Dictionary<string, object>
            {
                ["color"] = ds.BorderColor ?? defaultColor,
                ["width"] = ds.BorderWidth ?? 2 // Dashboard default per spec
            };
            
            // Handle borderDash → lineStyle.type (solid | dashed | dotted)
            if (ds.BorderDash != null && ds.BorderDash.Length > 0)
            {
                lineStyle["type"] = "dashed";
            }
            else
            {
                lineStyle["type"] = "solid";
            }
            
            seriesItem["lineStyle"] = lineStyle;
            
            // Item style for point markers (use same color as line)
            seriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = ds.BorderColor ?? defaultColor
            };
            
            // Emphasis OFF by default (spec 6.1 - emphasis disabled)
            seriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["disabled"] = true
            };
            
            // Area style: fill: true → add areaStyle
            if (ds.Fill == true)
            {
                if (ds.Gradient != null)
                {
                    var (x, y, x2, y2) = GetGradientCoordinates(ds.Gradient.Direction);
                    
                    seriesItem["areaStyle"] = new Dictionary<string, object>
                    {
                        ["color"] = new Dictionary<string, object>
                        {
                            ["type"] = "linear",
                            ["x"] = x,
                            ["y"] = y,
                            ["x2"] = x2,
                            ["y2"] = y2,
                            ["colorStops"] = ds.Gradient.ColorStops.Select(cs => new
                            {
                                offset = cs.Offset,
                                color = cs.Color
                            }).ToArray(),
                            ["global"] = false
                        }
                    };
                }
                else
                {
                    // Simple area fill with backgroundColor
                    var areaColor = (!string.IsNullOrEmpty(ds.BackgroundColor) && ds.BackgroundColor != "transparent")
                        ? ds.BackgroundColor 
                        : (ds.BorderColor ?? defaultColor);
                    
                    seriesItem["areaStyle"] = new Dictionary<string, object>
                    {
                        ["color"] = areaColor,
                        ["opacity"] = 0.3
                    };
                }
            }
            
            series.Add(seriesItem);
        }
        
        // Build ECharts v6 option object with dashboard defaults (spec 6.1)
        // Use model instances with baked defaults (user can override via options)
        var grid = options.Grid ?? new ChartGrid();
        
        // Use options.Legend if provided, otherwise create from Plugins.Legend with defaults
        var legendConfig = options.Legend ?? new LegendConfig 
        { 
            Display = options.Plugins.Legend.Display
            // Other properties use their defaults from LegendConfig
        };
        var legendOption = MapLegendConfig(legendConfig);
        
        // Use options.Tooltip if provided, otherwise create from Plugins.Tooltip with defaults
        var tooltip = options.Tooltip ?? new TooltipConfig 
        { 
            Enabled = options.Plugins.Tooltip.Enabled,
            Show = options.Plugins.Tooltip.Enabled,
            Trigger = TooltipTrigger.Axis,
            AxisPointer = new AxisPointerConfig { Type = AxisPointerType.None }
        };
        
        return new
        {
            // Animation
            animationDuration = options.Animation?.Duration ?? 750,
            animationEasing = MapEasingToECharts(options.Animation?.Easing ?? AnimationEasing.EaseInOutQuart),
            
            // Grid - Use model instance with defaults
            grid = new
            {
                left = grid.Left,
                right = grid.Right,
                top = grid.Top,
                bottom = grid.Bottom,
                containLabel = grid.ContainLabel
            },
            
            // Tooltip - Use model instance with defaults
            tooltip = new
            {
                show = tooltip.Show,
                trigger = tooltip.Trigger?.ToString().ToLowerInvariant() ?? "axis",
                axisPointer = new { type = tooltip.AxisPointer?.Type.ToString().ToLowerInvariant() ?? "none" },
                formatter = tooltip.Formatter
            },
            
            // Legend - Use model instance with defaults
            legend = legendOption,
            
            // X Axis
            xAxis = new
            {
                type = "category",
                data = labels,
                show = options.Scales?.X?.Display ?? true,
                boundaryGap = false, // Lines start at axis edge
                axisLine = new { show = options.Scales?.X?.Display ?? true },
                splitLine = new { show = options.Scales?.X?.Grid?.Display ?? true }
            },
            
            // Y Axis
            yAxis = new
            {
                type = "value",
                show = options.Scales?.Y?.Display ?? true,
                axisLine = new { show = options.Scales?.Y?.Display ?? true },
                splitLine = new { show = options.Scales?.Y?.Grid?.Display ?? true }
            },
            
            series = series.ToArray()
        };
    }
    
    private object ConvertBarChart(ChartData data, ChartOptions options)
    {
        // Use strongly-typed properties
        var labels = data.Labels;
        var datasets = data.Datasets;
        
        var series = new List<object>();
        for (int i = 0; i < datasets.Length; i++)
        {
            var ds = datasets[i];
            
            // Auto-assign color based on series index
            var seriesIndex = i + 1;
            var defaultColor = $"var(--chart-{seriesIndex})";
            
            var seriesItem = new Dictionary<string, object>
            {
                ["type"] = "bar",
                ["name"] = ds.Label ?? $"Series {seriesIndex}",
                ["data"] = ds.Data
            };
            
            // Item style: backgroundColor → itemStyle.color
            var itemColor = (!string.IsNullOrEmpty(ds.BackgroundColor) && ds.BackgroundColor != "transparent")
                ? ds.BackgroundColor
                : (ds.BorderColor ?? defaultColor);
            
            seriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = itemColor
            };
            
            // Emphasis OFF by default (spec 6.1 - emphasis disabled)
            seriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["disabled"] = true
            };
            
            // Support gradients for bars
            if (ds.Gradient != null)
            {
                var (x, y, x2, y2) = GetGradientCoordinates(ds.Gradient.Direction);
                
                seriesItem["itemStyle"] = new Dictionary<string, object>
                {
                    ["color"] = new Dictionary<string, object>
                    {
                        ["type"] = "linear",
                        ["x"] = x,
                        ["y"] = y,
                        ["x2"] = x2,
                        ["y2"] = y2,
                        ["colorStops"] = ds.Gradient.ColorStops.Select(cs => new
                        {
                            offset = cs.Offset,
                            color = cs.Color
                        }).ToArray(),
                        ["global"] = false
                    }
                };
            }
            
            series.Add(seriesItem);
        }
        
        // Build ECharts v6 option object with dashboard defaults (spec 6.1, 6.3)
        // Use model instances with baked defaults (user can override via options)
        var grid = options.Grid ?? new ChartGrid();
        
        // Use options.Legend if provided, otherwise create from Plugins.Legend with defaults
        var legendConfig = options.Legend ?? new LegendConfig 
        { 
            Display = options.Plugins.Legend.Display
            // Other properties use their defaults from LegendConfig
        };
        var legendOption = MapLegendConfig(legendConfig);
        
        // Use options.Tooltip if provided, otherwise create from Plugins.Tooltip with defaults
        var tooltip = options.Tooltip ?? new TooltipConfig 
        { 
            Enabled = options.Plugins.Tooltip.Enabled,
            Show = options.Plugins.Tooltip.Enabled,
            Trigger = TooltipTrigger.Axis,
            AxisPointer = new AxisPointerConfig { Type = AxisPointerType.None }
        };
        
        return new
        {
            // Animation
            animationDuration = options.Animation?.Duration ?? 750,
            animationEasing = MapEasingToECharts(options.Animation?.Easing ?? AnimationEasing.EaseInOutQuart),
            
            // Grid - Use model instance with defaults
            grid = new
            {
                left = grid.Left,
                right = grid.Right,
                top = grid.Top,
                bottom = grid.Bottom,
                containLabel = grid.ContainLabel
            },
            
            // Tooltip - Use model instance with defaults
            tooltip = new
            {
                show = tooltip.Show,
                trigger = tooltip.Trigger?.ToString().ToLowerInvariant() ?? "axis",
                axisPointer = new { type = tooltip.AxisPointer?.Type.ToString().ToLowerInvariant() ?? "none" },
                formatter = tooltip.Formatter
            },
            
            // Legend - Use model instance with defaults
            legend = legendOption,
            xAxis = new 
            { 
                type = "category", 
                data = labels,
                show = options.Scales?.X?.Display ?? true,
                boundaryGap = true,  // CRITICAL: Bars centered between ticks (Section 2.2)
                axisLine = new { show = options.Scales?.X?.Display ?? true },
                splitLine = new { show = false }  // No vertical grid lines per spec
            },
            yAxis = new 
            { 
                type = "value",
                show = options.Scales?.Y?.Display ?? true,
                axisLine = new { show = options.Scales?.Y?.Display ?? true },
                splitLine = new { show = options.Scales?.Y?.Grid?.Display ?? true }
            },
            series = series.ToArray()
        };
    }
    
    private object ConvertPieChart(ChartData data, ChartOptions options)
    {
        // Use strongly-typed properties (Section 2.4: Pie Charts)
        var labels = data.Labels;
        var datasets = data.Datasets;
        
        var pieData = new List<object>();
        if (datasets.Length > 0)
        {
            var ds = datasets[0];
            var values = ds.Data;
            
            // Map each label with its value and auto-assigned color
            // Pie data format: [{ name, value, itemStyle }, ...]
            for (int i = 0; i < labels.Length && i < values.Length; i++)
            {
                var itemColor = $"var(--chart-{i + 1})";  // CSS variable, no hsl() wrapper needed
                pieData.Add(new 
                { 
                    name = labels[i], 
                    value = values[i],
                    itemStyle = new { color = itemColor }
                });
            }
        }
        
        // Build ECharts v6 option with dashboard defaults (spec 6.1, 6.4)
        // CRITICAL: NO xAxis / NO yAxis for pie charts
        // Use model instances with baked defaults (user can override via options)
        
        // Use options.Legend if provided, otherwise create from Plugins.Legend with defaults
        var legendConfig = options.Legend ?? new LegendConfig 
        { 
            Display = options.Plugins.Legend.Display
            // Other properties use their defaults from LegendConfig
        };
        var legendOption = MapLegendConfig(legendConfig);
        
        // Use options.Tooltip if provided, otherwise create from Plugins.Tooltip with defaults
        var tooltip = options.Tooltip ?? new TooltipConfig 
        { 
            Enabled = options.Plugins.Tooltip.Enabled,
            Show = options.Plugins.Tooltip.Enabled,
            Trigger = TooltipTrigger.Item
        };
        
        return new
        {
            // Animation
            animationDuration = options.Animation?.Duration ?? 750,
            animationEasing = MapEasingToECharts(options.Animation?.Easing ?? AnimationEasing.EaseInOutQuart),
            
            // Tooltip - Use model instance with defaults
            tooltip = new
            {
                show = tooltip.Show,
                trigger = tooltip.Trigger?.ToString().ToLowerInvariant() ?? "item",
                formatter = tooltip.Formatter
            },
            
            // Legend - Use model instance with defaults
            legend = legendOption,
            
            series = new[]
            {
                new
                {
                    type = "pie",
                    radius = (object)(options.Cutout != null ? 
                        new[] { options.Cutout, "70%" } :  // Donut with inner radius
                        "70%"),  // Regular pie
                    data = pieData.ToArray(),
                    // Emphasis disabled by default (spec 6.4)
                    emphasis = new
                    {
                        disabled = true
                    }
                }
            }
        };
    }
    
    private object ConvertRadarChart(ChartData data, ChartOptions options)
    {
        // Use strongly-typed properties (Section 2.5: Radar Charts)
        var labels = data.Labels;
        var datasets = data.Datasets;
        
        // Build radar indicators from labels
        var indicators = new List<object>();
        foreach (var label in labels)
        {
            indicators.Add(new { name = label, max = 100 });
        }
        
        var series = new List<object>();
        for (int i = 0; i < datasets.Length; i++)
        {
            var ds = datasets[i];
            
            // Auto-assign color based on series index
            var seriesIndex = i + 1;
            var defaultColor = $"var(--chart-{seriesIndex})";  // CSS variable
            
            var radarSeriesItem = new Dictionary<string, object>
            {
                ["type"] = "radar",
                ["name"] = ds.Label ?? $"Series {seriesIndex}",
                ["data"] = new[]
                {
                    new
                    {
                        value = ds.Data,
                        name = ds.Label ?? $"Series {seriesIndex}"
                    }
                }
            };
            
            // Add line and area styling with auto-assigned color
            var color = ds.BorderColor ?? defaultColor;
            radarSeriesItem["lineStyle"] = new Dictionary<string, object>
            {
                ["color"] = color
            };
            radarSeriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = color
            };
            // Emphasis OFF by default (spec 6.1)
            radarSeriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["disabled"] = true
            };
            
            // Add area fill if specified (semi-transparent per spec)
            if (ds.Fill == true)
            {
                radarSeriesItem["areaStyle"] = new Dictionary<string, object>
                {
                    ["opacity"] = 0.3  // Section 2.5: opacity for radar areas
                };
            }
            
            series.Add(radarSeriesItem);
        }
        
        // Build ECharts v6 option with dashboard defaults (spec 6.1)
        // CRITICAL: NO xAxis / NO yAxis for radar charts
        // Use model instances with baked defaults (user can override via options)
        
        // Use options.Legend if provided, otherwise create from Plugins.Legend with defaults
        var legendConfig = options.Legend ?? new LegendConfig 
        { 
            Display = options.Plugins.Legend.Display
            // Other properties use their defaults from LegendConfig
        };
        var legendOption = MapLegendConfig(legendConfig);
        
        // Use options.Tooltip if provided, otherwise create from Plugins.Tooltip with defaults
        var tooltip = options.Tooltip ?? new TooltipConfig 
        { 
            Enabled = options.Plugins.Tooltip.Enabled,
            Show = options.Plugins.Tooltip.Enabled,
            Trigger = TooltipTrigger.Item
        };
        
        return new
        {
            // Animation
            animationDuration = options.Animation?.Duration ?? 750,
            animationEasing = MapEasingToECharts(options.Animation?.Easing ?? AnimationEasing.EaseInOutQuart),
            
            // Tooltip - Use model instance with defaults
            tooltip = new
            {
                show = tooltip.Show,
                trigger = tooltip.Trigger?.ToString().ToLowerInvariant() ?? "item",
                formatter = tooltip.Formatter
            },
            
            // Legend - Use model instance with defaults
            legend = legendOption,
            
            radar = new { indicator = indicators.ToArray() },  // Radar coordinate system
            series = series.ToArray()
        };
    }
    
    private object ConvertScatterChart(ChartData data, ChartOptions options)
    {
        // Use strongly-typed properties (Section 2.3: Scatter Charts)
        var datasets = data.Datasets;
        
        var series = new List<object>();
        for (int i = 0; i < datasets.Length; i++)
        {
            var ds = datasets[i];
            
            // Auto-assign color based on series index
            var seriesIndex = i + 1;
            var defaultColor = $"var(--chart-{seriesIndex})";  // CSS variable
            
            // Scatter data format: [[x, y], ...] or use ScatterData if available
            var scatterData = ds.ScatterData ?? new object[][] { };
            
            var scatterSeriesItem = new Dictionary<string, object>
            {
                ["type"] = "scatter",
                ["name"] = ds.Label ?? $"Series {seriesIndex}",
                ["data"] = scatterData
            };
            
            // Symbol size for scatter points
            var symbolSize = ds.PointRadius ?? 8;
            scatterSeriesItem["symbolSize"] = symbolSize;
            
            // Item style for point color with auto-assigned color
            var color = ds.BorderColor ?? defaultColor;
            scatterSeriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = color
            };
            
            // Emphasis OFF by default (spec 6.1)
            scatterSeriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["disabled"] = true
            };
            
            series.Add(scatterSeriesItem);
        }
        
        // Build ECharts v6 option with dashboard defaults (spec 6.1)
        // CRITICAL: xAxis/yAxis type = "value" (numeric, not category)
        // NO boundaryGap property (not applicable to value axes)
        // Use model instances with baked defaults (user can override via options)
        var grid = options.Grid ?? new ChartGrid();
        
        // Use options.Legend if provided, otherwise create from Plugins.Legend with defaults
        var legendConfig = options.Legend ?? new LegendConfig 
        { 
            Display = options.Plugins.Legend.Display
            // Other properties use their defaults from LegendConfig
        };
        var legendOption = MapLegendConfig(legendConfig);
        
        // Use options.Tooltip if provided, otherwise create from Plugins.Tooltip with defaults
        var tooltip = options.Tooltip ?? new TooltipConfig 
        { 
            Enabled = options.Plugins.Tooltip.Enabled,
            Show = options.Plugins.Tooltip.Enabled,
            Trigger = TooltipTrigger.Item
        };
        
        return new
        {
            // Animation
            animationDuration = options.Animation?.Duration ?? 750,
            animationEasing = MapEasingToECharts(options.Animation?.Easing ?? AnimationEasing.EaseInOutQuart),
            
            // Grid - Use model instance with defaults
            grid = new
            {
                left = grid.Left,
                right = grid.Right,
                top = grid.Top,
                bottom = grid.Bottom,
                containLabel = grid.ContainLabel
            },
            
            // Tooltip - Use model instance with defaults
            tooltip = new
            {
                show = tooltip.Show,
                trigger = tooltip.Trigger?.ToString().ToLowerInvariant() ?? "item",
                formatter = tooltip.Formatter
            },
            
            // Legend - Use model instance with defaults
            legend = legendOption,
            xAxis = new 
            { 
                type = "value",  // Numeric axis (Section 2.3)
                show = options.Scales?.X?.Display ?? true,
                splitLine = new { show = options.Scales?.X?.Grid?.Display ?? true }
            },
            yAxis = new 
            { 
                type = "value",  // Numeric axis (Section 2.3)
                show = options.Scales?.Y?.Display ?? true,
                splitLine = new { show = options.Scales?.Y?.Grid?.Display ?? true }
            },
            series = series.ToArray()
        };
    }
    
    // Helper methods to extract options
    private int GetAnimationDuration(dynamic options)
    {
        try
        {
            var animation = GetProperty<object>(options, "animation");
            if (animation != null)
            {
                return GetProperty<int>(animation, "duration", 750);
            }
        }
        catch { }
        return 750;
    }
    
    private string GetAnimationEasing(dynamic options)
    {
        try
        {
            var animation = GetProperty<object>(options, "animation");
            if (animation != null)
            {
                var easing = GetProperty<object>(animation, "easing");
                if (easing != null)
                {
                    return MapEasingToECharts(easing.ToString());
                }
            }
        }
        catch
        {
            // Fall back to default easing when parsing fails
        }
        return "quarticInOut";
    }
    
    private string MapEasingToECharts(string easing)
    {
        if (Enum.TryParse<AnimationEasing>(easing, true, out var parsed))
        {
            return MapEasingToECharts(parsed);
        }
        return "quarticInOut";
    }
    
    private string MapEasingToECharts(AnimationEasing easing)
    {
        return easing switch
        {
            AnimationEasing.Linear => "linear",
            AnimationEasing.EaseInQuad => "quadraticIn",
            AnimationEasing.EaseOutQuad => "quadraticOut",
            AnimationEasing.EaseInOutQuad => "quadraticInOut",
            AnimationEasing.EaseInCubic => "cubicIn",
            AnimationEasing.EaseOutCubic => "cubicOut",
            AnimationEasing.EaseInOutCubic => "cubicInOut",
            AnimationEasing.EaseInQuart => "quarticIn",
            AnimationEasing.EaseOutQuart => "quarticOut",
            AnimationEasing.EaseInOutQuart => "quarticInOut",
            AnimationEasing.EaseInQuint => "quinticIn",
            AnimationEasing.EaseOutQuint => "quinticOut",
            AnimationEasing.EaseInOutQuint => "quinticInOut",
            AnimationEasing.EaseInExpo => "exponentialIn",
            AnimationEasing.EaseOutExpo => "exponentialOut",
            AnimationEasing.EaseInOutExpo => "exponentialInOut",
            AnimationEasing.EaseInBack => "backIn",
            AnimationEasing.EaseOutBack => "backOut",
            AnimationEasing.EaseInOutBack => "backInOut",
            AnimationEasing.EaseInElastic => "elasticIn",
            AnimationEasing.EaseOutElastic => "elasticOut",
            AnimationEasing.EaseInOutElastic => "elasticInOut",
            AnimationEasing.EaseInBounce => "bounceIn",
            AnimationEasing.EaseOutBounce => "bounceOut",
            AnimationEasing.EaseInOutBounce => "bounceInOut",
            _ => "quarticInOut"
        };
    }
    
    private bool GetTooltipEnabled(dynamic options)
    {
        try
        {
            var plugins = GetProperty<object>(options, "plugins");
            if (plugins != null)
            {
                var tooltip = GetProperty<object>(plugins, "tooltip");
                if (tooltip != null)
                {
                    return GetProperty<bool>(tooltip, "enabled", true);
                }
            }
        }
        catch { }
        return true;
    }
    
    private bool GetLegendDisplay(dynamic options)
    {
        try
        {
            var plugins = GetProperty<object>(options, "plugins");
            if (plugins != null)
            {
                var legend = GetProperty<object>(plugins, "legend");
                if (legend != null)
                {
                    return GetProperty<bool>(legend, "display", true);
                }
            }
        }
        catch { }
        return true;
    }
    
    private bool GetXAxisDisplay(dynamic options)
    {
        try
        {
            var scales = GetProperty<object>(options, "scales");
            if (scales != null)
            {
                var xAxis = GetProperty<object>(scales, "x");
                if (xAxis != null)
                {
                    return GetProperty<bool>(xAxis, "display", true);
                }
            }
        }
        catch { }
        return true;
    }
    
    private bool GetYAxisDisplay(dynamic options)
    {
        try
        {
            var scales = GetProperty<object>(options, "scales");
            if (scales != null)
            {
                var yAxis = GetProperty<object>(scales, "y");
                if (yAxis != null)
                {
                    return GetProperty<bool>(yAxis, "display", true);
                }
            }
        }
        catch { }
        return true;
    }
    
    private bool GetGridDisplay(dynamic options)
    {
        try
        {
            var scales = GetProperty<object>(options, "scales");
            if (scales != null)
            {
                var xAxis = GetProperty<object>(scales, "x");
                if (xAxis != null)
                {
                    var grid = GetProperty<object>(xAxis, "grid");
                    if (grid != null)
                    {
                        return GetProperty<bool>(grid, "display", true);
                    }
                }
            }
        }
        catch { }
        return true;
    }
    
    private (double x, double y, double x2, double y2) GetGradientCoordinates(string direction)
    {
        return direction switch
        {
            "Vertical" => (0, 0, 0, 1),
            "Horizontal" => (0, 0, 1, 0),
            "TopLeftToBottomRight" => (0, 0, 1, 1),
            "TopRightToBottomLeft" => (1, 0, 0, 1),
            "BottomLeftToTopRight" => (0, 1, 1, 0),
            "BottomRightToTopLeft" => (1, 1, 0, 0),
            _ => (0, 0, 0, 1)
        };
    }
    
    private T? GetProperty<T>(object obj, string propertyName, T? defaultValue = default)
    {
        if (obj == null) return defaultValue;
        
        try
        {
            if (obj is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue(propertyName, out var value))
                {
                    if (value is T typedValue)
                        return typedValue;
                    if (value != null)
                        return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            else
            {
                var prop = obj.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    if (value is T typedValue)
                        return typedValue;
                    if (value != null)
                        return (T)Convert.ChangeType(value, typeof(T));
                }
            }
        }
        catch { }
        
        return defaultValue;
    }
    
    public async Task UpdateDataAsync(string chartId, object data)
    {
        var module = await EnsureModuleLoadedAsync();
        
        // Serialize with camelCase to match ECharts expectations
        var json = JsonSerializer.Serialize(data, JsonOptions);
        var normalizedData = JsonSerializer.Deserialize<object>(json, JsonOptions);
        
        await module.InvokeVoidAsync("updateData", chartId, normalizedData);
    }
    
    public async Task UpdateOptionsAsync(string chartId, object options)
    {
        var module = await EnsureModuleLoadedAsync();
        
        // Serialize with camelCase to match ECharts expectations
        var json = JsonSerializer.Serialize(options, JsonOptions);
        var normalizedOptions = JsonSerializer.Deserialize<object>(json, JsonOptions);
        
        await module.InvokeVoidAsync("updateOptions", chartId, normalizedOptions);
    }
    
    public async Task ApplyThemeAsync(string chartId, ChartTheme theme)
    {
        var module = await EnsureModuleLoadedAsync();
        
        var echartsTheme = MapToEChartsTheme(theme);
        await module.InvokeVoidAsync("applyTheme", chartId, echartsTheme);
    }
    
    public async Task<string> ExportAsImageAsync(string chartId, ImageFormat format)
    {
        var module = await EnsureModuleLoadedAsync();
        
        var type = format == ImageFormat.Svg ? "svg" : "png";
        return await module.InvokeAsync<string>("exportImage", chartId, type);
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
            grid = new
            {
                borderColor = theme.Border
            },
            categoryAxis = new
            {
                axisLine = new { lineStyle = new { color = theme.Border } },
                axisTick = new { lineStyle = new { color = theme.Border } },
                axisLabel = new { color = theme.MutedForeground },
                splitLine = new { lineStyle = new { color = theme.Muted } }
            },
            valueAxis = new
            {
                axisLine = new { lineStyle = new { color = theme.Border } },
                axisTick = new { lineStyle = new { color = theme.Border } },
                axisLabel = new { color = theme.MutedForeground },
                splitLine = new { lineStyle = new { color = theme.Muted } }
            }
        };
    }
    
    public async Task DestroyAsync(string chartId)
    {
        var module = await EnsureModuleLoadedAsync();
        await module.InvokeVoidAsync("destroy", chartId);
    }
    
    public async Task RefreshAsync(string chartId)
    {
        var module = await EnsureModuleLoadedAsync();
        await module.InvokeVoidAsync("refresh", chartId);
    }
    
    public async ValueTask DisposeAsync()
    {
        // Don't dispose the shared module - it's shared across all instances
        // Only set instance reference to null
        _jsModule = null;
    }
}
