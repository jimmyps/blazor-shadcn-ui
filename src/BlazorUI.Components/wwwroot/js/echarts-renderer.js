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
 * Responsibilities:
 * - Single-flight ECharts script loading
 * - Store last config per chart instance
 * - Recursive CSS variable resolution before setOption
 * - Refresh capability to re-resolve CSS variables
 */

// Store chart instances by ID with their last config
const chartInstances = new Map();

// Promise to track echarts loading state
let echartsLoadingPromise = null;

/**
 * Load ECharts library dynamically using script tag
 * UMD modules need to be loaded this way to properly populate window.echarts
 * @returns {Promise} - Resolves when the script is loaded
 */
function loadEChartsScript() {
    // If already loading, return existing promise
    if (echartsLoadingPromise) {
        return echartsLoadingPromise;
    }
    
    // If already loaded, return resolved promise
    if (window.echarts) {
        return Promise.resolve();
    }
    
    echartsLoadingPromise = new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = '/_content/BlazorUI.Components/js/echarts.min.js';
        script.onload = () => {
            console.log('[ECharts] Script tag loaded, checking window.echarts...');
            // Give the UMD module a moment to set window.echarts
            setTimeout(() => {
                if (window.echarts) {
                    console.log('[ECharts] window.echarts is available');
                    resolve();
                } else {
                    console.error('[ECharts] window.echarts is NOT available after script load');
                    reject(new Error('ECharts did not populate window.echarts'));
                }
            }, 50);
        };
        script.onerror = () => reject(new Error('Failed to load ECharts script'));
        document.head.appendChild(script);
    });
    
    return echartsLoadingPromise;
}

/**
 * Recursively resolve CSS variables in config object
 * @param {any} obj - Object to process
 * @returns {any} - Processed object with resolved CSS variables
 */
function resolveCssVariables(obj) {
    if (typeof obj === 'string') {
        // Match var(--variable-name) or var(--variable-name, fallback) pattern
        // Extract only the variable name (before any comma for fallback)
        const varMatch = obj.match(/var\(([^,)]+)/);
        if (varMatch) {
            const varName = varMatch[1].trim();
            const computedValue = getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
            return computedValue || obj; // Return original if not found
        }
        return obj;
    }
    
    if (Array.isArray(obj)) {
        return obj.map(item => resolveCssVariables(item));
    }
    
    if (obj !== null && typeof obj === 'object') {
        const resolved = {};
        for (const [key, value] of Object.entries(obj)) {
            resolved[key] = resolveCssVariables(value);
        }
        return resolved;
    }
    
    return obj;
}

/**
 * Create a new ECharts instance
 * @param {HTMLElement} element - Container element to render the chart
 * @param {object} config - ECharts v6 configuration object (NOT Chart.js format)
 * @returns {string} - Unique ID for the chart instance
 */
export async function createChart(element, config) {
    console.log('[ECharts] createChart called', { element, hasConfig: !!config });
    
    if (!element) {
        throw new Error('Chart element is required');
    }
    
    // Load ECharts from local file (v6.0.0) - bundled with component library
    // Using script tag approach since the UMD module needs to populate window.echarts
    if (!window.echarts) {
        console.log('[ECharts] Loading ECharts library...');
        try {
            await loadEChartsScript();
            console.log('[ECharts] ECharts library loaded successfully');
        } catch (error) {
            console.error('Failed to load ECharts library:', error);
            throw new Error('Failed to load ECharts library. Charts cannot be rendered.');
        }
    }
    
    // Verify ECharts loaded successfully
    if (!window.echarts) {
        console.error('ECharts library is not available after load attempt.');
        throw new Error('ECharts library not found. Please ensure the library is properly bundled.');
    }
    
    const chartId = generateId();
    
    try {
        console.log('[ECharts] Initializing chart instance...');
        // Initialize ECharts instance with SVG renderer (default and required)
        // SVG mode provides: high-quality output, native OKLCH color support,
        // better export capabilities, and aligns with modern design systems
        const chart = echarts.init(element, null, { renderer: 'svg' });
        console.log('[ECharts] Chart instance created successfully');
        
        // Store original config for refresh capability
        const originalConfig = config;
        
        // Resolve CSS variables recursively before applying
        const resolvedConfig = resolveCssVariables(config);
        chart.setOption(resolvedConfig);
        console.log('[ECharts] Chart option set successfully, chartId:', chartId);
        
        // Make chart responsive - store listener reference for cleanup
        const resizeListener = () => chart.resize();
        window.addEventListener('resize', resizeListener);
        
        chartInstances.set(chartId, {
            chart: chart,
            element: element,
            resizeListener: resizeListener,
            lastConfig: originalConfig  // Store original config with CSS variables
        });
        
        console.log('[ECharts] Chart initialization complete');
        return chartId;
    } catch (error) {
        console.error('Failed to create ECharts instance:', error);
        throw error;
    }
}

/**
 * Update chart data
 * @param {string} chartId - Chart instance ID
 * @param {object} newConfig - New ECharts configuration (already in ECharts format)
 */
export function updateData(chartId, newConfig) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return;
    }
    
    // Store original config and resolve CSS variables
    instance.lastConfig = newConfig;
    const resolvedConfig = resolveCssVariables(newConfig);
    instance.chart.setOption(resolvedConfig, { replaceMerge: ['series'] });
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
    
    // Store original config and resolve CSS variables
    // Safely merge with existing config
    instance.lastConfig = { ...(instance.lastConfig || {}), ...newOptions };
    const resolvedOptions = resolveCssVariables(newOptions);
    instance.chart.setOption(resolvedOptions, { notMerge: false });
}

/**
 * Refresh chart by re-resolving CSS variables and re-applying options
 * @param {string} chartId - Chart instance ID
 */
export function refresh(chartId) {
    const instance = chartInstances.get(chartId);
    if (!instance) {
        console.warn(`Chart ${chartId} not found`);
        return;
    }
    
    if (!instance.lastConfig) {
        console.warn(`No config stored for chart ${chartId}`);
        return;
    }
    
    // Re-resolve CSS variables from stored config
    const resolvedConfig = resolveCssVariables(instance.lastConfig);
    
    // Re-apply with notMerge: true to fully replace and lazyUpdate: false for immediate rendering
    instance.chart.setOption(resolvedConfig, { notMerge: true, lazyUpdate: false });
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
 * Generate a unique ID for chart instances
 * @returns {string} - Unique identifier
 */
function generateId() {
    return `echarts-${Date.now()}-${Math.random().toString(36).substring(2, 11)}`;
}
