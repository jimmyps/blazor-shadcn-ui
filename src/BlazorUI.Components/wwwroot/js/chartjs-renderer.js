/**
 * Chart.js Renderer Module for BlazorUI Charts
 * Provides integration between Blazor components and Chart.js library
 */

// Store chart instances by element ID
const chartInstances = new Map();

/**
 * Create a new chart instance
 * @param {HTMLElement} element - Canvas element to render the chart
 * @param {object} config - Chart.js configuration object
 * @returns {string} - Unique ID for the chart instance
 */
export async function createChart(element, config) {
    if (!element) {
        throw new Error('Chart element is required');
    }
    
    // Dynamically import Chart.js from CDN
    if (!window.Chart) {
        await import('https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js');
    }
    
    const chartId = generateId();
    
    try {
        // Resolve CSS variables in the config before creating the chart
        const resolvedConfig = resolveCssVariables(config);
        
        const chart = new Chart(element, resolvedConfig);
        chartInstances.set(chartId, {
            chart: chart,
            element: element
        });
        
        return chartId;
    } catch (error) {
        console.error('Failed to create chart:', error);
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
    
    const chart = instance.chart;
    // Resolve CSS variables in the new data
    chart.data = resolveCssVariables(newData);
    chart.update();
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
    
    const chart = instance.chart;
    chart.options = { ...chart.options, ...newOptions };
    chart.update();
}

/**
 * Apply theme colors to chart
 * @param {string} chartId - Chart instance ID
 * @param {object} theme - Theme configuration with colors
 */
export function applyTheme(chartId, theme) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return;
    }
    
    const chart = instance.chart;
    
    // Update chart colors based on theme
    if (theme.chartColors && theme.chartColors.length > 0) {
        chart.data.datasets.forEach((dataset, index) => {
            const colorIndex = index % theme.chartColors.length;
            dataset.borderColor = theme.chartColors[colorIndex];
            dataset.backgroundColor = theme.chartColors[colorIndex];
        });
    }
    
    // Update chart options for theme
    if (chart.options.scales) {
        Object.keys(chart.options.scales).forEach(scaleKey => {
            const scale = chart.options.scales[scaleKey];
            if (scale.grid) {
                scale.grid.color = theme.border || 'rgba(0, 0, 0, 0.1)';
            }
            if (scale.ticks) {
                scale.ticks.color = theme.mutedForeground || 'rgba(0, 0, 0, 0.5)';
            }
        });
    }
    
    chart.update();
}

/**
 * Export chart as base64 image
 * @param {string} chartId - Chart instance ID
 * @returns {string} - Base64 encoded PNG image
 */
export function toBase64Image(chartId) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return '';
    }
    
    return instance.chart.toBase64Image();
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
        instance.chart.destroy();
        chartInstances.delete(chartId);
    } catch (error) {
        console.error('Failed to destroy chart:', error);
    }
}

/**
 * Generate a unique ID for chart instances
 * @returns {string} - Unique identifier
 */
function generateId() {
    return `chart-${Date.now()}-${Math.random().toString(36).substring(2, 11)}`;
}

/**
 * Get computed CSS variable value
 * @param {string} varName - CSS variable name (e.g., '--chart-1')
 * @returns {string} - Computed color value
 */
export function getCssVariable(varName) {
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
 * Get all chart colors from CSS variables
 * @returns {string[]} - Array of color strings
 */
export function getChartColors() {
    return [
        getCssVariable('--chart-1'),
        getCssVariable('--chart-2'),
        getCssVariable('--chart-3'),
        getCssVariable('--chart-4'),
        getCssVariable('--chart-5')
    ];
}

/**
 * Get OHLC (candlestick) chart colors from CSS variables
 * @returns {object} - Object with up, down, and neutral colors
 */
export function getOHLCColors() {
    return {
        up: getCssVariable('--chart-up') || '0.646 0.222 41.116',
        down: getCssVariable('--chart-down') || '0.628 0.258 29.234',
        neutral: getCssVariable('--chart-neutral') || '0.5 0 0'
    };
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
            
            console.log(`Resolved ${obj} to ${result}`);
            return result;
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
