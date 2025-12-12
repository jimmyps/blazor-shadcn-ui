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
        
        // Extract data from universal format
        var dataObj = config.Data as dynamic ?? new { };
        var optionsObj = config.Options as dynamic ?? new { };
        
        // Build ECharts configuration based on chart type
        return chartType switch
        {
            "line" => ConvertLineChart(dataObj, optionsObj),
            "bar" => ConvertBarChart(dataObj, optionsObj),
            "pie" or "donut" => ConvertPieChart(dataObj, optionsObj),
            "radar" => ConvertRadarChart(dataObj, optionsObj),
            "scatter" => ConvertScatterChart(dataObj, optionsObj),
            _ => new { }
        };
    }
    
    private object ConvertLineChart(dynamic data, dynamic options)
    {
        // Extract labels and datasets from universal format
        var labels = GetProperty<string[]>(data, "labels") ?? Array.Empty<string>();
        var datasets = GetProperty<object[]>(data, "datasets") ?? Array.Empty<object>();
        
        var series = new List<object>();
        foreach (var dataset in datasets)
        {
            var ds = dataset as Dictionary<string, object> ?? new Dictionary<string, object>();
            
            // Build series item with proper ECharts v6 structure
            var seriesItem = new Dictionary<string, object>
            {
                ["type"] = "line",
                ["name"] = ds.ContainsKey("label") ? ds["label"] : "Data",
                ["data"] = ds.ContainsKey("data") ? ds["data"] : Array.Empty<double>()
            };
            
            // Handle tension → smooth (can be boolean or 0-1 value)
            if (ds.ContainsKey("tension"))
            {
                var tension = Convert.ToDouble(ds["tension"]);
                seriesItem["smooth"] = tension > 0 ? tension : false;
            }
            
            // Handle pointRadius → symbolSize and showSymbol
            if (ds.ContainsKey("pointRadius"))
            {
                var pointRadius = Convert.ToInt32(ds["pointRadius"]);
                seriesItem["showSymbol"] = pointRadius > 0;
                seriesItem["symbolSize"] = pointRadius > 0 ? pointRadius : 0;
            }
            else
            {
                seriesItem["showSymbol"] = true;
                seriesItem["symbolSize"] = 4;
            }
            
            // Handle pointHoverRadius → emphasis.symbolSize
            if (ds.ContainsKey("pointHoverRadius"))
            {
                var hoverRadius = Convert.ToInt32(ds["pointHoverRadius"]);
                seriesItem["emphasis"] = new Dictionary<string, object>
                {
                    ["symbolSize"] = hoverRadius
                };
            }
            
            // Line style: borderColor → lineStyle.color, borderWidth → lineStyle.width
            var lineStyle = new Dictionary<string, object>
            {
                ["color"] = ds.ContainsKey("borderColor") ? ds["borderColor"] : "hsl(var(--chart-1))",
                ["width"] = ds.ContainsKey("borderWidth") ? ds["borderWidth"] : 2
            };
            
            // Handle borderDash → lineStyle.type (solid | dashed | dotted)
            if (ds.ContainsKey("borderDash"))
            {
                var borderDash = ds["borderDash"];
                if (borderDash is Array arr && arr.Length > 0)
                {
                    lineStyle["type"] = "dashed";
                }
                else
                {
                    lineStyle["type"] = "solid";
                }
            }
            else
            {
                lineStyle["type"] = "solid";
            }
            
            seriesItem["lineStyle"] = lineStyle;
            
            // Item style for point markers
            seriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = ds.ContainsKey("borderColor") ? ds["borderColor"] : "hsl(var(--chart-1))"
            };
            
            // Area style: fill: true → add areaStyle
            if (ds.ContainsKey("fill") && Convert.ToBoolean(ds["fill"]))
            {
                if (ds.ContainsKey("gradient"))
                {
                    var gradient = ds["gradient"] as Dictionary<string, object>;
                    if (gradient != null)
                    {
                        var direction = gradient.ContainsKey("direction") ? gradient["direction"]?.ToString() : "Vertical";
                        var (x, y, x2, y2) = GetGradientCoordinates(direction);
                        
                        seriesItem["areaStyle"] = new Dictionary<string, object>
                        {
                            ["color"] = new Dictionary<string, object>
                            {
                                ["type"] = "linear",
                                ["x"] = x,
                                ["y"] = y,
                                ["x2"] = x2,
                                ["y2"] = y2,
                                ["colorStops"] = gradient.ContainsKey("colorStops") ? gradient["colorStops"] : Array.Empty<object>(),
                                ["global"] = false
                            }
                        };
                    }
                }
                else
                {
                    // Simple area fill with backgroundColor
                    var areaColor = ds.ContainsKey("backgroundColor") && ds["backgroundColor"]?.ToString() != "transparent" 
                        ? ds["backgroundColor"] 
                        : ds.ContainsKey("borderColor") ? ds["borderColor"] : "hsl(var(--chart-1))";
                    
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
            animationDuration = GetAnimationDuration(options),
            animationEasing = GetAnimationEasing(options),
            tooltip = new { show = GetTooltipEnabled(options), trigger = "axis" },
            legend = new { show = GetLegendDisplay(options), top = "top", left = "center" },
            xAxis = new
            {
                type = "category",
                data = labels,
                show = GetXAxisDisplay(options),
                boundaryGap = false,
                axisLine = new { show = GetXAxisDisplay(options) },
                splitLine = new { show = GetGridDisplay(options) }
            },
            yAxis = new
            {
                type = "value",
                show = GetYAxisDisplay(options),
                axisLine = new { show = GetYAxisDisplay(options) },
                splitLine = new { show = GetGridDisplay(options) }
            },
            series = series.ToArray()
        };
    }
    
    private object ConvertBarChart(dynamic data, dynamic options)
    {
        // Extract labels and datasets from universal format
        var labels = GetProperty<string[]>(data, "labels") ?? Array.Empty<string>();
        var datasets = GetProperty<object[]>(data, "datasets") ?? Array.Empty<object>();
        
        var series = new List<object>();
        foreach (var dataset in datasets)
        {
            var ds = dataset as Dictionary<string, object> ?? new Dictionary<string, object>();
            
            var seriesItem = new Dictionary<string, object>
            {
                ["type"] = "bar",
                ["name"] = ds.ContainsKey("label") ? ds["label"] : "Data",
                ["data"] = ds.ContainsKey("data") ? ds["data"] : Array.Empty<double>()
            };
            
            // Item style: backgroundColor → itemStyle.color
            var itemColor = ds.ContainsKey("backgroundColor") && ds["backgroundColor"]?.ToString() != "transparent"
                ? ds["backgroundColor"]
                : ds.ContainsKey("borderColor") ? ds["borderColor"] : "hsl(var(--chart-1))";
            
            seriesItem["itemStyle"] = new Dictionary<string, object>
            {
                ["color"] = itemColor
            };
            
            // Support gradients for bars (horizontal gradient)
            if (ds.ContainsKey("gradient"))
            {
                var gradient = ds["gradient"] as Dictionary<string, object>;
                if (gradient != null)
                {
                    var direction = gradient.ContainsKey("direction") ? gradient["direction"]?.ToString() : "Horizontal";
                    var (x, y, x2, y2) = GetGradientCoordinates(direction);
                    
                    seriesItem["itemStyle"] = new Dictionary<string, object>
                    {
                        ["color"] = new Dictionary<string, object>
                        {
                            ["type"] = "linear",
                            ["x"] = x,
                            ["y"] = y,
                            ["x2"] = x2,
                            ["y2"] = y2,
                            ["colorStops"] = gradient.ContainsKey("colorStops") ? gradient["colorStops"] : Array.Empty<object>(),
                            ["global"] = false
                        }
                    };
                }
            }
            
            series.Add(seriesItem);
        }
        
        // Build ECharts v6 option object (NO top-level 'type', NO responsive/maintainAspectRatio)
        return new
        {
            animationDuration = GetAnimationDuration(options),
            animationEasing = GetAnimationEasing(options),
            tooltip = new { show = GetTooltipEnabled(options), trigger = "axis" },
            legend = new { show = GetLegendDisplay(options), top = "top", left = "center" },
            xAxis = new 
            { 
                type = "category", 
                data = labels,
                show = GetXAxisDisplay(options),
                axisLine = new { show = GetXAxisDisplay(options) },
                splitLine = new { show = GetGridDisplay(options) }
            },
            yAxis = new 
            { 
                type = "value",
                show = GetYAxisDisplay(options),
                axisLine = new { show = GetYAxisDisplay(options) },
                splitLine = new { show = GetGridDisplay(options) }
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
            
            // Map each label with its value
            for (int i = 0; i < labels.Length && i < values.Length; i++)
            {
                pieData.Add(new { name = labels[i], value = values[i] });
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
        foreach (var dataset in datasets)
        {
            var ds = dataset as Dictionary<string, object> ?? new Dictionary<string, object>();
            
            var radarSeriesItem = new Dictionary<string, object>
            {
                ["type"] = "radar",
                ["name"] = ds.ContainsKey("label") ? ds["label"] : "Data",
                ["data"] = new[]
                {
                    new
                    {
                        value = ds.ContainsKey("data") ? ds["data"] : Array.Empty<double>(),
                        name = ds.ContainsKey("label") ? ds["label"] : "Data"
                    }
                }
            };
            
            // Add line and area styling if available
            if (ds.ContainsKey("borderColor"))
            {
                radarSeriesItem["lineStyle"] = new Dictionary<string, object>
                {
                    ["color"] = ds["borderColor"]
                };
                radarSeriesItem["itemStyle"] = new Dictionary<string, object>
                {
                    ["color"] = ds["borderColor"]
                };
            }
            
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
        foreach (var dataset in datasets)
        {
            var ds = dataset as Dictionary<string, object> ?? new Dictionary<string, object>();
            
            var scatterSeriesItem = new Dictionary<string, object>
            {
                ["type"] = "scatter",
                ["name"] = ds.ContainsKey("label") ? ds["label"] : "Data",
                ["data"] = ds.ContainsKey("data") ? ds["data"] : Array.Empty<object>()
            };
            
            // Symbol size for scatter points
            if (ds.ContainsKey("pointRadius"))
            {
                scatterSeriesItem["symbolSize"] = ds["pointRadius"];
            }
            
            // Item style for point color
            if (ds.ContainsKey("borderColor"))
            {
                scatterSeriesItem["itemStyle"] = new Dictionary<string, object>
                {
                    ["color"] = ds["borderColor"]
                };
            }
            
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
