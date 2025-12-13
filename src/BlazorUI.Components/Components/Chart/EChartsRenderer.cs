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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public EChartsRenderer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<string> InitializeAsync(ElementReference element, ChartConfig config)
    {
        _jsModule ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorUI.Components/js/echarts-renderer.js");
        
        // Convert universal ChartConfig to ECharts v6 format
        var echartsConfig = ConvertToEChartsFormat(config);
        
        // Serialize with camelCase for JavaScript
        var json = JsonSerializer.Serialize(echartsConfig, JsonOptions);
        var normalizedConfig = JsonSerializer.Deserialize<object>(json, JsonOptions);
        
        var chartId = await _jsModule.InvokeAsync<string>("createChart", element, normalizedConfig);
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
    
    private object ConvertLineChart(ChartData data, ChartOptions options)
    {
        // Use strongly-typed properties instead of dynamic access
        var labels = data.Labels;
        var datasets = data.Datasets;
        
        var series = new List<object>();
        for (int i = 0; i < datasets.Length; i++)
        {
            var ds = datasets[i];
            
            // Auto-assign color based on series index (1-based for CSS variables)
            var seriesIndex = i + 1;
            var defaultColor = $"hsl(var(--chart-{seriesIndex}))";
            
            // Build series item with proper ECharts v6 structure
            var seriesItem = new Dictionary<string, object>
            {
                ["type"] = "line",
                ["name"] = ds.Label ?? $"Series {seriesIndex}",
                ["data"] = ds.Data
            };
            
            // Handle tension → smooth (use actual value for smooth curves)
            seriesItem["smooth"] = ds.Tension ?? 0;
            
            // Handle pointRadius → symbolSize and showSymbol
            var pointRadius = ds.PointRadius ?? 4;
            seriesItem["showSymbol"] = pointRadius > 0;
            seriesItem["symbolSize"] = pointRadius;
            
            // Handle pointHoverRadius → emphasis with focus:'series'
            var hoverRadius = ds.PointHoverRadius ?? (pointRadius + 1);
            
            seriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["focus"] = "series", // Highlight series on hover
                ["symbolSize"] = hoverRadius
            };
            
            // Line style: borderColor → lineStyle.color, borderWidth → lineStyle.width
            var lineStyle = new Dictionary<string, object>
            {
                ["color"] = ds.BorderColor ?? defaultColor,
                ["width"] = ds.BorderWidth ?? 2
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
        
        // Build ECharts v6 option object (NO top-level 'type', NO responsive/maintainAspectRatio)
        return new
        {
            animationDuration = options.Animation?.Duration ?? 750,
            animationEasing = MapEasingToECharts(options.Animation?.Easing ?? "easeInOutQuart"),
            tooltip = new { show = options.Plugins.Tooltip.Enabled, trigger = "axis" },
            legend = new { 
                show = options.Plugins.Legend.Display, 
                top = "top", 
                left = "center",
                orient = "horizontal"
            },
            xAxis = new
            {
                type = "category",
                data = labels,
                show = options.Scales?.X?.Display ?? true,
                boundaryGap = false, // Lines start at axis edge
                axisLine = new { show = options.Scales?.X?.Display ?? true },
                splitLine = new { show = options.Scales?.X?.Grid?.Display ?? true }
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
            var defaultColor = $"hsl(var(--chart-{seriesIndex}))";
            
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
            
            // Add emphasis for hover
            seriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["focus"] = "series"
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
        
        // Build ECharts v6 option object (NO top-level 'type', NO responsive/maintainAspectRatio)
        return new
        {
            animationDuration = options.Animation?.Duration ?? 750,
            animationEasing = MapEasingToECharts(options.Animation?.Easing ?? "easeInOutQuart"),
            tooltip = new { show = options.Plugins.Tooltip.Enabled, trigger = "axis" },
            legend = new { 
                show = options.Plugins.Legend.Display, 
                top = "top", 
                left = "center",
                orient = "horizontal"
            },
            xAxis = new 
            { 
                type = "category", 
                data = labels,
                show = options.Scales?.X?.Display ?? true,
                axisLine = new { show = options.Scales?.X?.Display ?? true },
                splitLine = new { show = options.Scales?.X?.Grid?.Display ?? true }
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
    
    private object ConvertPieChart(dynamic data, dynamic options)
    {
        // Extract labels and datasets from universal format
        var labels = GetProperty<string[]>(data, "labels") ?? Array.Empty<string>();
        var datasets = GetProperty<object[]>(data, "datasets") ?? Array.Empty<object>();
        
        var pieData = new List<object>();
        if (datasets.Length > 0)
        {
            var dataset = datasets[0] as Dictionary<string, object>;
            var values = dataset?.ContainsKey("data") == true ? 
                (dataset["data"] as IEnumerable<object>)?.ToArray() ?? Array.Empty<object>() : 
                Array.Empty<object>();
            
            // Map each label with its value and auto-assigned color
            for (int i = 0; i < labels.Length && i < values.Length; i++)
            {
                var itemColor = $"hsl(var(--chart-{i + 1}))";
                pieData.Add(new 
                { 
                    name = labels[i], 
                    value = values[i],
                    itemStyle = new { color = itemColor }
                });
            }
        }
        
        // Build ECharts v6 option object (NO top-level 'type')
        return new
        {
            animationDuration = GetAnimationDuration(options),
            animationEasing = GetAnimationEasing(options),
            tooltip = new { show = GetTooltipEnabled(options), trigger = "item" },
            legend = new { show = GetLegendDisplay(options), orient = "vertical", left = "right", top = "middle" },
            series = new[]
            {
                new
                {
                    type = "pie",
                    radius = "70%",
                    data = pieData.ToArray(),
                    emphasis = new
                    {
                        focus = "self",
                        itemStyle = new
                        {
                            shadowBlur = 10,
                            shadowOffsetX = 0,
                            shadowColor = "rgba(0, 0, 0, 0.5)"
                        }
                    }
                }
            }
        };
    }
    
    private object ConvertRadarChart(dynamic data, dynamic options)
    {
        // Extract labels for radar indicators
        var labels = GetProperty<string[]>(data, "labels") ?? Array.Empty<string>();
        var datasets = GetProperty<object[]>(data, "datasets") ?? Array.Empty<object>();
        
        var indicators = new List<object>();
        foreach (var label in labels)
        {
            indicators.Add(new { name = label, max = 100 });
        }
        
        var series = new List<object>();
        for (int i = 0; i < datasets.Length; i++)
        {
            var dataset = datasets[i];
            var ds = dataset as Dictionary<string, object> ?? new Dictionary<string, object>();
            
            // Auto-assign color based on series index
            var seriesIndex = i + 1;
            var defaultColor = $"hsl(var(--chart-{seriesIndex}))";
            
            var radarSeriesItem = new Dictionary<string, object>
            {
                ["type"] = "radar",
                ["name"] = ds.ContainsKey("label") ? ds["label"] : $"Series {seriesIndex}",
                ["data"] = new[]
                {
                    new
                    {
                        value = ds.ContainsKey("data") ? ds["data"] : Array.Empty<double>(),
                        name = ds.ContainsKey("label") ? ds["label"] : $"Series {seriesIndex}"
                    }
                }
            };
            
            // Add line and area styling with auto-assigned color
            var color = ds.ContainsKey("borderColor") ? ds["borderColor"] : defaultColor;
            radarSeriesItem["lineStyle"] = new Dictionary<string, object>
            {
                ["color"] = color
            };
            radarSeriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = color
            };
            radarSeriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["focus"] = "series"
            };
            
            if (ds.ContainsKey("fill") && Convert.ToBoolean(ds["fill"]))
            {
                radarSeriesItem["areaStyle"] = new Dictionary<string, object>
                {
                    ["opacity"] = 0.3
                };
            }
            
            series.Add(radarSeriesItem);
        }
        
        // Build ECharts v6 option object (NO top-level 'type')
        return new
        {
            animationDuration = GetAnimationDuration(options),
            animationEasing = GetAnimationEasing(options),
            tooltip = new { show = GetTooltipEnabled(options), trigger = "item" },
            legend = new { show = GetLegendDisplay(options), top = "top", left = "center" },
            radar = new { indicator = indicators.ToArray() },
            series = series.ToArray()
        };
    }
    
    private object ConvertScatterChart(dynamic data, dynamic options)
    {
        // Extract datasets from universal format
        var datasets = GetProperty<object[]>(data, "datasets") ?? Array.Empty<object>();
        
        var series = new List<object>();
        for (int i = 0; i < datasets.Length; i++)
        {
            var dataset = datasets[i];
            var ds = dataset as Dictionary<string, object> ?? new Dictionary<string, object>();
            
            // Auto-assign color based on series index
            var seriesIndex = i + 1;
            var defaultColor = $"hsl(var(--chart-{seriesIndex}))";
            
            var scatterSeriesItem = new Dictionary<string, object>
            {
                ["type"] = "scatter",
                ["name"] = ds.ContainsKey("label") ? ds["label"] : $"Series {seriesIndex}",
                ["data"] = ds.ContainsKey("data") ? ds["data"] : Array.Empty<object>()
            };
            
            // Symbol size for scatter points
            if (ds.ContainsKey("pointRadius"))
            {
                scatterSeriesItem["symbolSize"] = ds["pointRadius"];
            }
            else
            {
                scatterSeriesItem["symbolSize"] = 8;
            }
            
            // Item style for point color with auto-assigned color
            scatterSeriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = ds.ContainsKey("borderColor") ? ds["borderColor"] : defaultColor
            };
            
            scatterSeriesItem["emphasis"] = new Dictionary<string, object>
            {
                ["focus"] = "series"
            };
            
            series.Add(scatterSeriesItem);
        }
        
        // Build ECharts v6 option object (NO top-level 'type')
        return new
        {
            animationDuration = GetAnimationDuration(options),
            animationEasing = GetAnimationEasing(options),
            tooltip = new { show = GetTooltipEnabled(options), trigger = "item" },
            legend = new { show = GetLegendDisplay(options), top = "top", left = "center" },
            xAxis = new 
            { 
                type = "value",
                show = GetXAxisDisplay(options),
                splitLine = new { show = GetGridDisplay(options) }
            },
            yAxis = new 
            { 
                type = "value",
                show = GetYAxisDisplay(options),
                splitLine = new { show = GetGridDisplay(options) }
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
        catch { }
        return "quarticInOut";
    }
    
    private string MapEasingToECharts(string easing)
    {
        return easing switch
        {
            "Linear" => "linear",
            "EaseInQuad" => "quadraticIn",
            "EaseOutQuad" => "quadraticOut",
            "EaseInOutQuad" => "quadraticInOut",
            "EaseInCubic" => "cubicIn",
            "EaseOutCubic" => "cubicOut",
            "EaseInOutCubic" => "cubicInOut",
            "EaseInQuart" => "quarticIn",
            "EaseOutQuart" => "quarticOut",
            "EaseInOutQuart" => "quarticInOut",
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
        if (_jsModule != null)
        {
            // Serialize with camelCase to match ECharts expectations
            var json = JsonSerializer.Serialize(data, JsonOptions);
            var normalizedData = JsonSerializer.Deserialize<object>(json, JsonOptions);
            
            await _jsModule.InvokeVoidAsync("updateData", chartId, normalizedData);
        }
    }
    
    public async Task UpdateOptionsAsync(string chartId, object options)
    {
        if (_jsModule != null)
        {
            // Serialize with camelCase to match ECharts expectations
            var json = JsonSerializer.Serialize(options, JsonOptions);
            var normalizedOptions = JsonSerializer.Deserialize<object>(json, JsonOptions);
            
            await _jsModule.InvokeVoidAsync("updateOptions", chartId, normalizedOptions);
        }
    }
    
    public async Task ApplyThemeAsync(string chartId, ChartTheme theme)
    {
        if (_jsModule != null)
        {
            var echartsTheme = MapToEChartsTheme(theme);
            await _jsModule.InvokeVoidAsync("applyTheme", chartId, echartsTheme);
        }
    }
    
    public async Task<string> ExportAsImageAsync(string chartId, ImageFormat format)
    {
        if (_jsModule != null)
        {
            var type = format == ImageFormat.Svg ? "svg" : "png";
            return await _jsModule.InvokeAsync<string>("exportImage", chartId, type);
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
        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("destroy", chartId);
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
    }
}
