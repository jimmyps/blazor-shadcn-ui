/**
 * ECharts Renderer Module for BlazorUI Charts
 * Provides integration between Blazor components and ECharts library
 * 
 * ECharts v6 Compatibility Notes:
 * - Default theme and visual styles have changed in v6
 * - Legend position default changed from top to bottom in v6
 * - Rich text labels now inherit plain label styles by default
 * - Grid anti-overflow and anti-overlap mechanisms are enabled by default
 * - SVG rendering mode is explicitly configured for best quality
 * 
 * We embrace v6's improved defaults while maintaining compatibility
 */

// Store chart instances by ID
const chartInstances = new Map();

/**
 * Create a new ECharts instance
 * @param {HTMLElement} element - Container element to render the chart
 * @param {object} config - Chart configuration object
 * @returns {string} - Unique ID for the chart instance
 */
export async function createChart(element, config) {
    if (!element) {
        throw new Error('Chart element is required');
    }
    
    // Dynamically import ECharts from CDN (latest v6)
    if (!window.echarts) {
        await import('https://cdn.jsdelivr.net/npm/echarts@6.0.0/dist/echarts.min.js');
    }
    
    const chartId = generateId();
    
    try {
        // Initialize ECharts instance with SVG renderer (default and required)
        // SVG mode provides: high-quality output, native OKLCH color support,
        // better export capabilities, and aligns with modern design systems
        const chart = echarts.init(element, null, { renderer: 'svg' });
        
        // Resolve CSS variables in the config before converting
        const resolvedConfig = resolveCssVariables(config);
        
        // Convert Chart.js-style config to ECharts option format
        const option = convertConfig(resolvedConfig);
        chart.setOption(option);
        
        // Make chart responsive - store listener reference for cleanup
        const resizeListener = () => chart.resize();
        window.addEventListener('resize', resizeListener);
        
        chartInstances.set(chartId, {
            chart: chart,
            element: element,
            resizeListener: resizeListener
        });
        
        return chartId;
    } catch (error) {
        console.error('Failed to create ECharts instance:', error);
        throw error;
    }
}

/**
 * Update chart data
 * @param {string} chartId - Chart instance ID
 * @param {object} newData - New data for the chart
 */
export function updateData(chartId, newData) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return;
    }
    
    // Resolve CSS variables before converting
    const resolvedData = resolveCssVariables(newData);
    const option = convertConfig({ data: resolvedData });
    instance.chart.setOption(option, { replaceMerge: ['series'] });
}

/**
 * Update chart options
 * @param {string} chartId - Chart instance ID
 * @param {object} newOptions - New options for the chart
 */
export function updateOptions(chartId, newOptions) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return;
    }
    
    instance.chart.setOption(newOptions, { notMerge: false });
}

/**
 * Apply theme to chart
 * @param {string} chartId - Chart instance ID
 * @param {object} theme - Theme configuration
 */
export function applyTheme(chartId, theme) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return;
    }
    
    // Store current option and remove old resize listener
    const currentOption = instance.chart.getOption();
    if (instance.resizeListener) {
        window.removeEventListener('resize', instance.resizeListener);
    }
    
    // Dispose old chart and create new one with theme
    // Always use SVG renderer for consistent high-quality output
    instance.chart.dispose();
    const chart = echarts.init(instance.element, theme, { renderer: 'svg' });
    chart.setOption(currentOption);
    
    // Add new resize listener
    const resizeListener = () => chart.resize();
    window.addEventListener('resize', resizeListener);
    
    // Update instance
    instance.chart = chart;
    instance.resizeListener = resizeListener;
}

/**
 * Export chart as image
 * @param {string} chartId - Chart instance ID
 * @param {string} type - Image type ('png' or 'svg')
 * @returns {string} - Base64 encoded image or SVG string
 */
export function exportImage(chartId, type) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return '';
    }
    
    if (type === 'svg') {
        return instance.chart.renderToSVGString();
    } else {
        return instance.chart.getDataURL({
            type: 'png',
            pixelRatio: 2,
            backgroundColor: '#fff'
        });
    }
}

/**
 * Destroy chart instance and clean up resources
 * @param {string} chartId - Chart instance ID
 */
export function destroy(chartId) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        return;
    }
    
    try {
        // Remove resize listener to prevent memory leak
        if (instance.resizeListener) {
            window.removeEventListener('resize', instance.resizeListener);
        }
        
        instance.chart.dispose();
        chartInstances.delete(chartId);
    } catch (error) {
        console.error('Failed to destroy chart:', error);
    }
}

/**
 * Convert Chart.js-style config to ECharts option format
 * @param {object} config - Chart.js style configuration
 * @returns {object} - ECharts option object
 */
function convertConfig(config) {
    // Support both PascalCase (from C#) and camelCase
    const type = config.type || config.Type;
    const data = config.data || config.Data || {};
    const options = config.options || config.Options || {};
    
    // Base ECharts option with v6-compatible settings
    const echartsOption = {
        animation: options.animation !== false,
        animationDuration: options.animation?.duration || 750,
        animationEasing: mapEasing(options.animation?.easing || 'cubicOut'),
        // v6: Opt-in to maintain v5 behavior if needed for compatibility
        // By default, we use v6 behavior for better defaults
        // richInheritPlainLabel: true (default in v6, rich text inherits plain label styles)
        // legacyViewCoordSysCenterBase: false (use v6 corrected percent base calculations)
    };
    
    // Convert based on chart type (handle both string and enum number values)
    const typeStr = typeof type === 'string' ? type.toLowerCase() : type;
    switch (typeStr) {
        case 'line':
        case 0: // ChartType.Line enum value
            return convertLineChart(data, options, echartsOption);
        case 'bar':
        case 1: // ChartType.Bar enum value
            return convertBarChart(data, options, echartsOption);
        case 'pie':
        case 'doughnut':
        case 'donut':
        case 2: // ChartType.Pie enum value
        case 3: // ChartType.Donut enum value
            return convertPieChart(data, options, echartsOption);
        case 'radar':
        case 4: // ChartType.Radar enum value
            return convertRadarChart(data, options, echartsOption);
        case 'scatter':
        case 5: // ChartType.Scatter enum value
            return convertScatterChart(data, options, echartsOption);
        case 'bubble':
        case 6: // ChartType.Bubble enum value
            return convertBubbleChart(data, options, echartsOption);
        case 'area':
        case 7: // ChartType.Area enum value
            return convertAreaChart(data, options, echartsOption);
        default:
            console.warn(`Unsupported chart type: ${type}`);
            return echartsOption;
    }
}

/**
 * Convert line chart configuration
 */
function convertLineChart(data, options, baseOption) {
    return {
        ...baseOption,
        xAxis: {
            type: 'category',
            data: data.labels || [],
            boundaryGap: false
        },
        yAxis: {
            type: 'value'
        },
        series: (data.datasets || []).map(dataset => ({
            type: 'line',
            name: dataset.label,
            data: dataset.data,
            smooth: dataset.tension > 0,
            showSymbol: dataset.pointRadius > 0,
            lineStyle: {
                width: dataset.borderWidth || 2,
                color: dataset.borderColor,
                type: dataset.borderDash ? 'dashed' : 'solid'
            },
            areaStyle: dataset.fill ? { opacity: 0.3 } : undefined,
            itemStyle: {
                color: dataset.borderColor
            }
        })),
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            show: options.plugins?.legend?.display !== false,
            // v6 changed default to bottom, but we prefer top for consistency with shadcn
            top: 'top',
            left: 'center'
        }
    };
}

/**
 * Convert bar chart configuration
 */
function convertBarChart(data, options, baseOption) {
    const isHorizontal = options.indexAxis === 'y';
    
    return {
        ...baseOption,
        xAxis: {
            type: isHorizontal ? 'value' : 'category',
            data: isHorizontal ? undefined : (data.labels || [])
        },
        yAxis: {
            type: isHorizontal ? 'category' : 'value',
            data: isHorizontal ? (data.labels || []) : undefined
        },
        series: (data.datasets || []).map(dataset => ({
            type: 'bar',
            name: dataset.label,
            data: dataset.data,
            itemStyle: {
                color: dataset.backgroundColor,
                borderRadius: dataset.borderRadius || 0
            },
            barMaxWidth: dataset.barThickness || undefined
        })),
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            show: options.plugins?.legend?.display !== false,
            // v6 changed default to bottom, but we prefer top for consistency with shadcn
            top: 'top',
            left: 'center'
        }
    };
}

/**
 * Convert pie/donut chart configuration
 */
function convertPieChart(data, options, baseOption) {
    const isDonut = options.cutout != null;
    
    return {
        ...baseOption,
        series: [{
            type: 'pie',
            radius: isDonut ? ['50%', '70%'] : '70%',
            data: (data.labels || []).map((label, index) => ({
                name: label,
                value: data.datasets[0]?.data[index] || 0
            })),
            label: {
                show: true
            },
            emphasis: {
                itemStyle: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }],
        tooltip: {
            trigger: 'item',
            formatter: '{b}: {c} ({d}%)'
        },
        legend: {
            show: options.plugins?.legend?.display !== false,
            // v6 default changed, for pie charts we keep vertical orientation on right
            orient: 'vertical',
            left: 'right',
            top: 'middle'
        }
    };
}

/**
 * Convert radar chart configuration
 */
function convertRadarChart(data, options, baseOption) {
    return {
        ...baseOption,
        radar: {
            indicator: (data.labels || []).map(label => ({ name: label, max: 100 }))
        },
        series: [{
            type: 'radar',
            data: (data.datasets || []).map(dataset => ({
                name: dataset.label,
                value: dataset.data,
                lineStyle: {
                    color: dataset.borderColor,
                    width: dataset.borderWidth || 2
                },
                areaStyle: {
                    opacity: 0.2
                },
                itemStyle: {
                    color: dataset.pointBackgroundColor
                }
            }))
        }],
        tooltip: {
            trigger: 'item'
        },
        legend: {
            show: options.plugins?.legend?.display !== false,
            // v6 changed default to bottom, but we prefer top for radar charts
            top: 'top',
            left: 'center'
        }
    };
}

/**
 * Convert scatter chart configuration
 */
function convertScatterChart(data, options, baseOption) {
    return {
        ...baseOption,
        xAxis: {
            type: 'value',
            name: options.scales?.x?.title || ''
        },
        yAxis: {
            type: 'value',
            name: options.scales?.y?.title || ''
        },
        series: (data.datasets || []).map(dataset => ({
            type: 'scatter',
            name: dataset.label,
            data: dataset.data,
            symbolSize: dataset.pointRadius || 10,
            itemStyle: {
                color: dataset.backgroundColor
            }
        })),
        tooltip: {
            trigger: 'item',
            formatter: params => `${params.seriesName}<br/>(${params.value[0]}, ${params.value[1]})`
        },
        legend: {
            show: options.plugins?.legend?.display !== false,
            top: 'top',
            left: 'center'
        }
    };
}

/**
 * Convert bubble chart configuration
 */
function convertBubbleChart(data, options, baseOption) {
    return {
        ...baseOption,
        xAxis: {
            type: 'value',
            name: options.scales?.x?.title || ''
        },
        yAxis: {
            type: 'value',
            name: options.scales?.y?.title || ''
        },
        series: (data.datasets || []).map(dataset => ({
            type: 'scatter',
            name: dataset.label,
            data: dataset.data,
            // Bubble size is the third element in data array [x, y, size]
            symbolSize: value => Array.isArray(value) && value.length > 2 ? value[2] / 2 : 10,
            itemStyle: {
                color: dataset.backgroundColor
            }
        })),
        tooltip: {
            trigger: 'item',
            formatter: params => {
                const [x, y, size] = params.value;
                return `${params.seriesName}<br/>X: ${x}<br/>Y: ${y}<br/>Size: ${size}`;
            }
        },
        legend: {
            show: options.plugins?.legend?.display !== false,
            top: 'top',
            left: 'center'
        }
    };
}

/**
 * Convert area chart configuration (line chart with filled area)
 */
function convertAreaChart(data, options, baseOption) {
    return {
        ...baseOption,
        xAxis: {
            type: 'category',
            data: data.labels || [],
            boundaryGap: false
        },
        yAxis: {
            type: 'value'
        },
        series: (data.datasets || []).map(dataset => ({
            type: 'line',
            name: dataset.label,
            data: dataset.data,
            smooth: dataset.tension > 0,
            showSymbol: dataset.pointRadius > 0,
            lineStyle: {
                width: dataset.borderWidth || 2,
                color: dataset.borderColor
            },
            areaStyle: {
                opacity: 0.3,
                color: dataset.backgroundColor
            },
            itemStyle: {
                color: dataset.borderColor
            },
            stack: dataset.stack || undefined
        })),
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            show: options.plugins?.legend?.display !== false,
            top: 'top',
            left: 'center'
        }
    };
}

/**
 * Map Chart.js easing to ECharts easing
 */
function mapEasing(easing) {
    const easingMap = {
        'linear': 'linear',
        'easeInQuad': 'quadraticIn',
        'easeOutQuad': 'quadraticOut',
        'easeInOutQuad': 'quadraticInOut',
        'easeInCubic': 'cubicIn',
        'easeOutCubic': 'cubicOut',
        'easeInOutCubic': 'cubicInOut',
        'easeInQuart': 'quarticIn',
        'easeOutQuart': 'quarticOut',
        'easeInOutQuart': 'quarticInOut'
    };
    
    return easingMap[easing] || 'cubicOut';
}

/**
 * Generate a unique ID for chart instances
 * @returns {string} - Unique identifier
 */
function generateId() {
    return `echarts-${Date.now()}-${Math.random().toString(36).substring(2, 11)}`;
}

/**
 * Get computed CSS variable value
 * @param {string} varName - CSS variable name (e.g., '--chart-1')
 * @returns {string} - Computed color value
 */
function getCssVariable(varName) {
    const styles = getComputedStyle(document.documentElement);
    const value = styles.getPropertyValue(varName).trim();
    
    if (!value) {
        console.warn(`CSS variable ${varName} not found or is empty`);
        return '';
    }
    
    // Check if this is HSL format (contains %) or OKLCH format (no %)
    if (value.includes('%')) {
        // HSL format: "212.7 26.8% 83.9%" → "212.7, 26.8%, 83.9%"
        const parts = value.split(' ');
        if (parts.length === 3) {
            return `${parts[0]}, ${parts[1]}, ${parts[2]}`;
        }
    } else {
        // OKLCH format: "0.646 0.222 41.116" → "0.646 0.222 41.116"
        // OKLCH uses spaces, not commas
        return value;
    }
    
    return value;
}

/**
 * Recursively resolve CSS variables in configuration object
 * @param {any} obj - Object to process
 * @returns {any} - Object with resolved CSS variables
 */
function resolveCssVariables(obj) {
    if (obj === null || obj === undefined) {
        return obj;
    }
    
    if (typeof obj === 'string') {
        // Match patterns like "hsl(var(--chart-1))", "oklch(var(--chart-up))" or "var(--chart-2)"
        const cssVarMatch = obj.match(/var\(([^)]+)\)/);
        if (cssVarMatch) {
            const varName = cssVarMatch[1];
            const resolvedValue = getCssVariable(varName);
            if (!resolvedValue) {
                console.warn(`Could not resolve CSS variable in: ${obj}`);
                return obj; // Return original if can't resolve
            }
            
            // Check if the resolved value is already a complete color function (e.g., "hsl(...)" or "oklch(...)")
            // If so, recursively resolve any nested var() within it, then use it directly
            if (resolvedValue.startsWith('hsl(') || resolvedValue.startsWith('oklch(') || 
                resolvedValue.startsWith('rgb(') || resolvedValue.startsWith('rgba(')) {
                // Recursively resolve in case the resolved value contains nested var()
                const fullyResolved = resolveCssVariables(resolvedValue);
                console.log(`Resolved ${obj} to ${fullyResolved} (already complete, recursively resolved)`);
                return fullyResolved;
            }
            
            // Check if var() is inside hsl() or oklch() - if so, just replace var() with values
            // Otherwise, wrap the resolved value appropriately
            let result;
            if (obj.startsWith('hsl(var(')) {
                // Already has hsl() wrapper, just replace var(...) with the values
                result = obj.replace(cssVarMatch[0], resolvedValue);
            } else if (obj.startsWith('oklch(var(')) {
                // Already has oklch() wrapper, just replace var(...) with the values
                result = obj.replace(cssVarMatch[0], resolvedValue);
            } else if (obj.startsWith('var(')) {
                // No wrapper, detect format and add appropriate wrapper
                // HSL has %, OKLCH doesn't
                if (resolvedValue.includes('%') || resolvedValue.includes(',')) {
                    result = `hsl(${resolvedValue})`;
                } else {
                    result = `oklch(${resolvedValue})`;
                }
            } else {
                // Replace var() with resolved value as-is
                result = obj.replace(cssVarMatch[0], resolvedValue);
            }
            
            // Recursively resolve in case there are more nested var() expressions
            const fullyResolved = resolveCssVariables(result);
            console.log(`Resolved ${obj} to ${fullyResolved}`);
            return fullyResolved;
        }
        return obj;
    }
    
    if (Array.isArray(obj)) {
        return obj.map(item => resolveCssVariables(item));
    }
    
    if (typeof obj === 'object') {
        const resolved = {};
        for (const key in obj) {
            if (obj.hasOwnProperty(key)) {
                resolved[key] = resolveCssVariables(obj[key]);
            }
        }
        return resolved;
    }
    
    return obj;
}
