/**
 * ECharts Renderer Module for BlazorUI Charts
 * Provides integration between Blazor components and ECharts library
 */

/**
 * ThemeWatcher class that monitors theme changes and triggers callbacks
 */
class ThemeWatcher {
    constructor(callback) {
        this.callback = callback;
        this.currentTheme = this.detectTheme();
        
        // Watch for class/data-theme changes on <html>
        this.observer = new MutationObserver(() => {
            const newTheme = this.detectTheme();
            if (newTheme !== this.currentTheme) {
                console.log('[ThemeWatcher] Theme changed:', this.currentTheme, '->', newTheme);
                this.currentTheme = newTheme;
                this.callback(newTheme);
            }
        });
        
        this.observer.observe(document.documentElement, {
            attributes: true,
            attributeFilter: ['class', 'data-theme']
        });
        
        // Watch system preference changes
        this.darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
        this.systemThemeHandler = () => {
            const newTheme = this.detectTheme();
            if (newTheme !== this.currentTheme) {
                this.currentTheme = newTheme;
                this.callback(newTheme);
            }
        };
        this.darkModeQuery.addEventListener('change', this.systemThemeHandler);
    }
    
    detectTheme() {
        if (document.documentElement.classList.contains('dark')) return 'dark';
        const dataTheme = document.documentElement.getAttribute('data-theme');
        if (dataTheme) return dataTheme;
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    
    destroy() {
        this.observer.disconnect();
        this.darkModeQuery.removeEventListener('change', this.systemThemeHandler);
    }
}

// Store chart instances by ID
const chartInstances = new Map();

// Global theme watcher instance
let globalThemeWatcher = null;

// Global promise to track ECharts library loading (single-flight pattern)
let echartsLoadingPromise = null;

/**
 * Ensure theme watcher is initialized and running
 */
function ensureThemeWatcher() {
    if (!globalThemeWatcher) {
        globalThemeWatcher = new ThemeWatcher((newTheme) => {
            console.log('[ECharts] Refreshing all charts for theme:', newTheme);
            chartInstances.forEach((info, id) => {
                try {
                    if (info.chart && !info.chart.isDisposed() && info.cachedOption) {
                        // Re-resolve CSS variables with new theme values
                        const resolvedOptions = resolveCssVariables(info.cachedOption);
                        const processedOptions = convertFunctionStrings(resolvedOptions);
                        
                        // Apply the refreshed options to the chart
                        info.chart.setOption(processedOptions, { notMerge: false });
                        
                        console.log('[ECharts] Chart', id, 'refreshed for theme:', newTheme);
                    }
                } catch (err) {
                    console.error('[ECharts] Refresh error for chart', id, ':', err);
                }
            });
        });
    }
}

/**
 * Create a new ECharts instance
 * @param {HTMLElement} element - Container element to render the chart
 * @param {object} config - Chart configuration object
 * @returns {string} - Unique ID for the chart instance
 */
export async function createChart(element, config) {
    if (!element) {
        console.error('[ECharts] Chart element is required');
        throw new Error('Chart element is required');
    }
    
    console.log('[ECharts] createChart called with config:', config);
    
    // Ensure theme watcher is running
    ensureThemeWatcher();
    
    // Single-flight ECharts loading: only first call downloads, others await same promise
    if (!window.echarts) {
        if (!echartsLoadingPromise) {
            console.log('[ECharts] Loading ECharts library from CDN (v6.0.0)...');
            echartsLoadingPromise = import('https://cdn.jsdelivr.net/npm/echarts@6.0.0/dist/echarts.min.js');
        } else {
            console.log('[ECharts] ECharts library already loading, awaiting...');
        }
        
        await echartsLoadingPromise;
        echartsLoadingPromise = null; // Clear promise after successful load
        console.log('[ECharts] ECharts library loaded successfully');
    } else {
        console.log('[ECharts] ECharts library already loaded');
    }
    
    const chartId = generateId();
    console.log('[ECharts] Generated chart ID:', chartId);
    
    try {
        // Initialize ECharts instance with SVG renderer for better quality
        const chart = echarts.init(element, null, { renderer: 'svg' });
        console.log('[ECharts] ECharts instance initialized');
        
        // Check if config.Data is already an ECharts option object
        let option;
        let cachedOption = null;
        if (config.data && typeof config.data === 'object') {
            // config.data contains the EChartsOption object directly
            console.log('[ECharts] Using direct ECharts option from config.data');
            
            // Cache a deep copy of the original option with unresolved CSS variables for theme refresh
            cachedOption = JSON.parse(JSON.stringify(config.data));
            
            const resolvedOptions = resolveCssVariables(config.data);
            option = convertFunctionStrings(resolvedOptions);
        } else {
          console.log('[ECharts] Provided ECharts option is not valid');
          return;
        }
        
        console.log('[ECharts] Final option object:', option);
        chart.setOption(option);
        console.log('[ECharts] Chart option set successfully');
        
        // Make chart responsive - listen to window resize
        const resizeListener = () => chart.resize();
        window.addEventListener('resize', resizeListener);
        
        // Store instance with cached option for theme refresh
        chartInstances.set(chartId, {
            chart: chart,
            element: element,
            resizeListener: resizeListener,
            resizeObserver: null,
            cachedOption: cachedOption
        });
        
        // Delay ResizeObserver setup until after animation completes
        // This prevents the observer from disrupting the initial animation
        const animationEnabled = option.animation !== false;
        const animationDuration = option.animationDuration || 1000; // Default 1000ms if not specified
        const delay = animationEnabled ? animationDuration + 100 : 100; // Add 100ms buffer
        
        setTimeout(() => {
            const instance = chartInstances.get(chartId);
            if (!instance) return; // Chart was destroyed before timeout
            
            // Now set up ResizeObserver to watch for container size changes
            const resizeObserver = new ResizeObserver((entries) => {
                for (let entry of entries) {
                    if (entry.target === element) {
                        chart.resize();
                    }
                }
            });
            resizeObserver.observe(element);
            
            // Update instance with ResizeObserver
            instance.resizeObserver = resizeObserver;
            console.log('[ECharts] ResizeObserver activated after animation');
        }, delay);
        
        console.log('[ECharts] Chart instance stored in map');
        return chartId;
    } catch (error) {
        console.error('[ECharts] Failed to create ECharts instance:', error);
        console.error('[ECharts] Error stack:', error.stack);
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
    
    // Handle symbolSizeFunction in series
    if (options.series && Array.isArray(options.series)) {
        result.series = options.series.map(series => {
            const convertedSeries = convertFunctionStrings(series);
            if (convertedSeries.symbolSizeFunction && typeof convertedSeries.symbolSizeFunction === 'string') {
                try {
                    convertedSeries.symbolSize = eval(`(${convertedSeries.symbolSizeFunction})`);
                    delete convertedSeries.symbolSizeFunction;
                } catch (e) {
                    console.error('[ECharts] Failed to parse symbolSize function:', e);
                }
            }
            return convertedSeries;
        });
    }
    
    return result;
}

/**
 * Update chart options
 * @param {string} chartId - Chart instance ID
 * @param {object} newOptions - New options for the chart
 */
export function updateOptions(chartId, newOptions) {
    console.log('[ECharts] updateOptions called for chart:', chartId);
    console.log('[ECharts] New options:', newOptions);
    
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`[ECharts] Chart ${chartId} not found in instances map`);
        console.warn(`[ECharts] Available chart IDs:`, Array.from(chartInstances.keys()));
        return;
    }
    
    // Update cached option for theme refresh (deep copy to prevent mutations)
    instance.cachedOption = JSON.parse(JSON.stringify(newOptions));
    
    // Resolve CSS variables and convert function strings
    const resolvedOptions = resolveCssVariables(newOptions);
    const processedOptions = convertFunctionStrings(resolvedOptions);
    
    console.log('[ECharts] Processed options:', processedOptions);
    instance.chart.setOption(processedOptions, { notMerge: false });
    console.log('[ECharts] Chart options updated successfully');
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
    
    // Store current option and remove old listeners/observers
    const currentOption = instance.chart.getOption();
    if (instance.resizeListener) {
        window.removeEventListener('resize', instance.resizeListener);
    }
    if (instance.resizeObserver) {
        instance.resizeObserver.disconnect();
    }
    
    // Dispose old chart and create new one with theme
    instance.chart.dispose();
    const chart = echarts.init(instance.element, theme, { renderer: 'svg' });
    chart.setOption(currentOption);
    
    // Add new resize listener
    const resizeListener = () => chart.resize();
    window.addEventListener('resize', resizeListener);
    
    // Update instance (ResizeObserver will be added after animation)
    instance.chart = chart;
    instance.resizeListener = resizeListener;
    instance.resizeObserver = null;
    
    // Delay ResizeObserver setup until after animation completes
    const animationEnabled = currentOption.animation !== false;
    const animationDuration = currentOption.animationDuration?.[0] || 1000;
    const delay = animationEnabled ? animationDuration + 100 : 100;
    
    setTimeout(() => {
        const currentInstance = chartInstances.get(chartId);
        if (!currentInstance) return; // Chart was destroyed
        
        // Set up ResizeObserver for container size changes
        const resizeObserver = new ResizeObserver((entries) => {
            for (let entry of entries) {
                if (entry.target === instance.element) {
                    chart.resize();
                }
            }
        });
        resizeObserver.observe(instance.element);
        
        // Update instance with ResizeObserver
        currentInstance.resizeObserver = resizeObserver;
    }, delay);
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
        // Remove window resize listener to prevent memory leak
        if (instance.resizeListener) {
            window.removeEventListener('resize', instance.resizeListener);
        }
        
        // Disconnect ResizeObserver to prevent memory leak
        if (instance.resizeObserver) {
            instance.resizeObserver.disconnect();
        }
        
        instance.chart.dispose();
        chartInstances.delete(chartId);
        
        // Cleanup theme watcher if no charts remain
        if (chartInstances.size === 0 && globalThemeWatcher) {
            globalThemeWatcher.destroy();
            globalThemeWatcher = null;
            console.log('[ECharts] Theme watcher destroyed - no charts remain');
        }
    } catch (error) {
        console.error('Failed to destroy chart:', error);
    }
}

/**
 * Generate a unique ID for chart instances
 * @returns {string} - Unique identifier
 */
function generateId() {
    return `echarts-${Date.now()}-${Math.random().toString(36).substring(2, 11)}`;
}

/**
 * Convert OKLCH color to hex format
 * @param {number} l - Lightness (0-1)
 * @param {number} c - Chroma (0-0.4)
 * @param {number} h - Hue (0-360)
 * @returns {string} - Hex color string
 */
function oklchToHex(l, c, h) {
    // Accept both CSS-style (0–100) and normalized (0–1) lightness.
    // If L looks like a percentage value, normalize it.
    if (l > 1) {
        l = l / 100;
    }

    // Convert hue from degrees to radians
    const hRad = (h * Math.PI) / 180;
    const a = c * Math.cos(hRad);
    const b = c * Math.sin(hRad);

    // Convert OKLab to LMS (non-linear)
    const l_ = l + 0.3963377774 * a + 0.2158037573 * b;
    const m_ = l - 0.1055613458 * a - 0.0638541728 * b;
    const s_ = l - 0.0894841775 * a - 1.2914855480 * b;

    // Cubic to get linear LMS
    const l3 = l_ * l_ * l_;
    const m3 = m_ * m_ * m_;
    const s3 = s_ * s_ * s_;

    // LMS (linear) to linear sRGB
    let r = +4.0767416621 * l3 - 3.3077115913 * m3 + 0.2309699292 * s3;
    let g = -1.2684380046 * l3 + 2.6097574011 * m3 - 0.3413193965 * s3;
    let bl = -0.0041960863 * l3 - 0.7034186147 * m3 + 1.7076147010 * s3;

    // Clamp to [0,1] before gamma correction
    const clamp01 = (v) => Math.min(1, Math.max(0, v));
    r = clamp01(r);
    g = clamp01(g);
    bl = clamp01(bl);

    // sRGB gamma correction
    const gammaCorrect = (val) =>
        val <= 0.0031308
            ? 12.92 * val
            : 1.055 * Math.pow(val, 1 / 2.4) - 0.055;

    r = gammaCorrect(r);
    g = gammaCorrect(g);
    bl = gammaCorrect(bl);

    const toHex = (val) => {
        const int = Math.round(clamp01(val) * 255);
        return int.toString(16).padStart(2, '0');
    };

    return `#${toHex(r)}${toHex(g)}${toHex(bl)}`;
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
    
    // Check if value has 3 numeric parts - could be HSL or OKLCH
    const parts = value
        .replace(/(oklch|hsl)\(/, '') // remove optional function prefix
        .replace(')', '')            // remove trailing parenthesis
        .trim()
        .split(/\s+/);               // split on any whitespace

    if (parts.length === 3) {
        // HSL format: "212.7 26.8% 83.9%" (often used as hsl(var(--...)))
        // We treat anything with % as HSL-like and return "h, s%, l%"
        if (value.startsWith('hsl(')) {
            return `${parts[0]}, ${parts[1]}, ${parts[2]}`;
        }

        // OKLCH format: "0.646 0.222 41.116" or "oklch(0.646 0.222 41.116)"
        // Detect explicitly by function name or by lack of % when oklch is present
        if (value.startsWith('oklch(') || value.includes('oklch')) {
            const l = parseFloat(parts[0]);
            const c = parseFloat(parts[1]);
            const h = parseFloat(parts[2]);

            if (!Number.isNaN(l) && !Number.isNaN(c) && !Number.isNaN(h)) {
                const hex = oklchToHex(l, c, h);
                console.log(`Converted OKLCH(${l}, ${c}, ${h}) to ${hex}`);
                return hex;
            }
        }
    }
    
    // Return as-is if not HSL or OKLCH 3-part format
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
        // Check for CSS variable with opacity suffix: "var(--chart-1)|0.8"
        const cssVarWithOpacityMatch = obj.match(/^(var\([^)]+\))\|([\d.]+)$/);
        if (cssVarWithOpacityMatch) {
            const cssVar = cssVarWithOpacityMatch[1];
            const opacity = parseFloat(cssVarWithOpacityMatch[2]);
            
            // Resolve the CSS variable first
            const resolvedColor = resolveCssVariables(cssVar);
            
            // Apply opacity to resolved color
            return applyOpacityToColor(resolvedColor, opacity);
        }
        
        // Check for hex8 format with alpha channel (#RRGGBBAA)
        // Convert to rgba() for better ECharts compatibility
        const hex8Match = obj.match(/^#([0-9A-Fa-f]{8})$/);
        if (hex8Match) {
            const hex = hex8Match[1];
            const r = parseInt(hex.substring(0, 2), 16);
            const g = parseInt(hex.substring(2, 4), 16);
            const b = parseInt(hex.substring(4, 6), 16);
            const a = parseInt(hex.substring(6, 8), 16) / 255; // Convert alpha to 0-1 range
            const rgba = `rgba(${r}, ${g}, ${b}, ${a.toFixed(3)})`;
            console.log(`Converted hex8 ${obj} to ${rgba}`);
            return rgba;
        }
        
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
            
            // Check wrapper function in original string to determine format
            let result;
            if (obj.startsWith('hsl(')) {
                // HSL wrapper detected: hsl(var(--chart-1))
                // Replace var(...) with resolved values inside hsl()
                result = obj.replace(cssVarMatch[0], resolvedValue);
            } else if (obj.startsWith('oklch(')) {
                // OKLCH wrapper detected: oklch(var(--chart-1))
                // Resolved value should be hex, use it directly (strip wrapper)
                if (resolvedValue.startsWith('#')) {
                    result = resolvedValue;
                } else {
                    // Fallback: replace var() if not hex
                    result = obj.replace(cssVarMatch[0], resolvedValue);
                }
            } else if (obj.startsWith('rgb(')) {
                // RGB wrapper detected: rgb(var(--chart-1))
                result = obj.replace(cssVarMatch[0], resolvedValue);
            } else if (obj.startsWith('var(')) {
                // No wrapper: var(--chart-1)
                // Use resolved value directly (already formatted by getCssVariable)
                if (resolvedValue.startsWith('#')) {
                    // Hex value, use as-is
                    result = resolvedValue;
                } else if (resolvedValue.includes(',')) {
                    // HSL format with commas, wrap it
                    result = `hsl(${resolvedValue})`;
                } else {
                    // Unknown format, use as-is
                    result = resolvedValue;
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

/**
 * Apply opacity to a color string
 * @param {string} color - Color in any format (hex, rgb, hsl, oklch, etc.)
 * @param {number} opacity - Opacity value between 0 and 1
 * @returns {string} - Color with applied opacity
 */
function applyOpacityToColor(color, opacity) {
    // Clamp opacity
    opacity = Math.max(0, Math.min(1, opacity));
    
    // Handle OKLCH colors - convert to hex first, then apply opacity
    const oklchMatch = color.match(/^oklch\(([\d.]+)\s+([\d.]+)\s+([\d.]+)\)$/);
    if (oklchMatch) {
        const l = parseFloat(oklchMatch[1]);
        const c = parseFloat(oklchMatch[2]);
        const h = parseFloat(oklchMatch[3]);
        
        if (!Number.isNaN(l) && !Number.isNaN(c) && !Number.isNaN(h)) {
            const hex = oklchToHex(l, c, h);
            console.log(`Converted OKLCH(${l}, ${c}, ${h}) to ${hex} before applying opacity`);
            // Recursively call with hex color
            return applyOpacityToColor(hex, opacity);
        }
    }
    
    // Handle hex colors (#RGB or #RRGGBB)
    const hexMatch = color.match(/^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$/);
    if (hexMatch) {
        let hex = hexMatch[1];
        
        // Convert 3-digit to 6-digit
        if (hex.length === 3) {
            hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
        }
        
        const r = parseInt(hex.substring(0, 2), 16);
        const g = parseInt(hex.substring(2, 4), 16);
        const b = parseInt(hex.substring(4, 6), 16);
        
        const result = `rgba(${r}, ${g}, ${b}, ${opacity.toFixed(3)})`;
        console.log(`Applied opacity ${opacity} to ${color} -> ${result}`);
        return result;
    }
    
    // Handle rgb/rgba colors
    const rgbMatch = color.match(/^rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*[\d.]+)?\)$/);
    if (rgbMatch) {
        const result = `rgba(${rgbMatch[1]}, ${rgbMatch[2]}, ${rgbMatch[3]}, ${opacity.toFixed(3)})`;
        console.log(`Applied opacity ${opacity} to ${color} -> ${result}`);
        return result;
    }
    
    // Handle hsl/hsla colors
    const hslMatch = color.match(/^hsla?\(([^)]+)\)$/);
    if (hslMatch) {
        const parts = hslMatch[1].split(',').map(s => s.trim());
        if (parts.length >= 3) {
            const result = `hsla(${parts[0]}, ${parts[1]}, ${parts[2]}, ${opacity})`;
            console.log(`Applied opacity ${opacity} to ${color} -> ${result}`);
            return result;
        }
    }
    
    // Unknown format - log warning and return as-is
    console.warn(`Unable to apply opacity to color: ${color}`);
    return color;
}
