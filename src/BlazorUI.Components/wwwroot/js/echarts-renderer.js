/**
 * ECharts Renderer Module for BlazorUI Charts
 * Provides integration between Blazor components and ECharts library
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
    
    // Dynamically import ECharts from CDN
    if (!window.echarts) {
        await import('https://cdn.jsdelivr.net/npm/echarts@5.4.3/dist/echarts.min.js');
    }
    
    const chartId = generateId();
    
    try {
        // Initialize ECharts instance with SVG renderer for better quality
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
 * Convert function strings to actual functions in ECharts options
 * @param {object} options - ECharts options object
 * @returns {object} - Options with function strings converted to functions
 */
function convertFunctionStrings(options) {
    if (!options || typeof options !== 'object') {
        return options;
    }
    
    // Handle arrays
    if (Array.isArray(options)) {
        return options.map(item => convertFunctionStrings(item));
    }
    
    const result = {};
    for (const [key, value] of Object.entries(options)) {
        // Check if this is a formatter field with a function string
        if (key === 'formatter' && typeof value === 'string' && value.trim().startsWith('function')) {
            try {
                // Create function from string using Function constructor (safer than eval)
                // Extract function body and parameters
                const funcMatch = value.match(/function\s*\((.*?)\)\s*\{([\s\S]*)\}/);
                if (funcMatch) {
                    const params = funcMatch[1].trim();
                    const body = funcMatch[2].trim();
                    result[key] = new Function(params, body);
                } else {
                    // Keep as string if parsing fails
                    result[key] = value;
                }
            } catch (error) {
                console.warn(`Failed to parse formatter function: ${error.message}`);
                result[key] = value; // Keep as string template if function parsing fails
            }
        } else if (typeof value === 'object' && value !== null) {
            result[key] = convertFunctionStrings(value);
        } else {
            result[key] = value;
        }
    }
    
    return result;
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
    
    // Convert function strings to actual functions before passing to ECharts
    const processedOptions = convertFunctionStrings(newOptions);
    
    instance.chart.setOption(processedOptions, { notMerge: false });
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
    
    // Base ECharts option
    const echartsOption = {
        animation: options.animation !== false,
        animationDuration: options.animation?.duration || 750,
        animationEasing: mapEasing(options.animation?.easing || 'cubicOut')
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
            show: options.plugins?.legend?.display !== false
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
            show: options.plugins?.legend?.display !== false
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
            orient: 'vertical',
            left: 'right'
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
            show: options.plugins?.legend?.display !== false
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
