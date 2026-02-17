// echarts-renderer.js
// Manages ECharts instance lifecycle: create, update, resize, dispose

import { resolveThemeColors, watchThemeChanges } from './chart-theme.js';

/** @type {Map<string, ChartState>} */
const instances = new Map();

/** @type {Promise|null} */
let echartsLoadPromise = null;

/** @type {any} */
let echartsLib = null;

/**
 * Lazily load the ECharts ESM library.
 * Uses a single-flight pattern to prevent duplicate loads.
 * @returns {Promise<any>}
 */
async function loadECharts() {
  if (echartsLib) return echartsLib;

  // Check for globally loaded ECharts first (e.g., via <script> tag)
  if (window.echarts) {
    echartsLib = window.echarts;
    return echartsLib;
  }

  if (!echartsLoadPromise) {
    echartsLoadPromise = (async () => {
      // Resolve relative to this module's own URL
      const libPath = new URL('../lib/echarts/echarts.min.js', import.meta.url).href;
      const mod = await import(libPath);
      return mod;
    })();
  }

  echartsLib = await echartsLoadPromise;
  return echartsLib;
}

/**
 * Initialize an ECharts instance on a DOM element.
 * @param {string} chartId - Unique chart identifier (matches element id)
 * @param {object} option - ECharts option object (JSON from C#)
 */
export async function initialize(chartId, option) {
  const element = document.getElementById(chartId);
  if (!element) return;

  // Dispose existing instance if re-initializing
  if (instances.has(chartId)) {
    dispose(chartId);
  }

  const echarts = await loadECharts();

  const chart = echarts.init(element, null, { renderer: 'svg' });

  // Resolve CSS variables and set options
  const resolvedOption = resolveThemeColors(option, element);
  applyRadarTooltipFormatter(resolvedOption);
  chart.setOption(resolvedOption);

  // ResizeObserver for responsive charts
  const resizeObserver = new ResizeObserver(() => {
    if (!chart.isDisposed()) {
      chart.resize();
    }
  });
  resizeObserver.observe(element);

  // Theme change watcher - re-resolve CSS variables on theme change
  const themeUnwatch = watchThemeChanges(() => {
    const state = instances.get(chartId);
    if (state && state.lastOption && !state.chart.isDisposed()) {
      const reresolved = resolveThemeColors(state.lastOption, state.element);
      applyRadarTooltipFormatter(reresolved);
      state.chart.setOption(reresolved, { notMerge: true });
    }
  });

  instances.set(chartId, {
    chart,
    element,
    resizeObserver,
    themeUnwatch,
    lastOption: option
  });
}

/**
 * Update chart options.
 * @param {string} chartId - Chart identifier
 * @param {object} option - New ECharts option
 * @param {boolean} notMerge - If true, replace entirely (default: true)
 */
export function update(chartId, option, notMerge) {
  const state = instances.get(chartId);
  if (!state || state.chart.isDisposed()) return;

  state.lastOption = option;
  const resolved = resolveThemeColors(option, state.element);
  applyRadarTooltipFormatter(resolved);
  state.chart.setOption(resolved, { notMerge: notMerge !== false });
}

/**
 * If the chart is a radar with rich-text indicator names (e.g. "{a|186/80}\n{b|January}"),
 * install a tooltip formatter that strips the rich-text tokens and renders clean HTML.
 * @param {object} option - Resolved ECharts option
 */
function applyRadarTooltipFormatter(option) {
  if (!option.radar?.indicator || !option.tooltip) return;

  const hasRichText = option.radar.indicator.some(
    i => i.name && /\{[a-z]\|/.test(i.name)
  );
  if (!hasRichText) return;

  // Extract clean display names by taking the last rich-text token (e.g. "{b|January}" → "January").
  // Falls back to stripping all tokens if no match.
  const cleanNames = option.radar.indicator.map(i => {
    if (!i.name) return '';
    const matches = [...i.name.matchAll(/\{[a-z]\|([^}]*)\}/g)];
    return matches.length > 0 ? matches[matches.length - 1][1] : i.name;
  });

  option.tooltip.formatter = (params) => {
    let html = `${params.marker} <strong>${params.seriesName}</strong>`;
    if (Array.isArray(params.value)) {
      params.value.forEach((val, idx) => {
        if (idx < cleanNames.length) {
          html += `<br/>${cleanNames[idx]}: ${val}`;
        }
      });
    }
    return html;
  };
}

/**
 * Dispose an ECharts instance and clean up all resources.
 * @param {string} chartId - Chart identifier
 */
export function dispose(chartId) {
  const state = instances.get(chartId);
  if (!state) return;

  state.resizeObserver.disconnect();
  if (state.themeUnwatch) {
    state.themeUnwatch();
  }
  if (!state.chart.isDisposed()) {
    state.chart.dispose();
  }
  instances.delete(chartId);
}
