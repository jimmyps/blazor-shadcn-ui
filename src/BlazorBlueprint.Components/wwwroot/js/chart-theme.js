// chart-theme.js
// CSS variable resolution and theme change detection for ECharts

/**
 * Recursively walk an option object and resolve any string values
 * that contain CSS var() references to computed color values.
 * @param {object} option - ECharts option object
 * @param {HTMLElement} contextElement - Element for getComputedStyle
 * @returns {object} - New option object with resolved colors
 */
export function resolveThemeColors(option, contextElement) {
  if (!option || !contextElement) return option;
  return deepResolve(structuredClone(option), contextElement);
}

/**
 * @param {any} obj
 * @param {HTMLElement} el
 * @returns {any}
 */
function deepResolve(obj, el) {
  if (obj === null || obj === undefined) return obj;

  if (typeof obj === 'string') {
    return resolveValue(obj, el);
  }

  if (Array.isArray(obj)) {
    return obj.map(item => deepResolve(item, el));
  }

  if (typeof obj === 'object') {
    for (const key of Object.keys(obj)) {
      obj[key] = deepResolve(obj[key], el);
    }
  }

  return obj;
}

/**
 * Resolve a single string value. If it contains var(--x), compute it.
 * Handles OKLCH values by converting to hex.
 * @param {string} value
 * @param {HTMLElement} el
 * @returns {string}
 */
function resolveValue(value, el) {
  if (typeof value !== 'string') return value;

  const varRegex = /var\(\s*(--[a-zA-Z0-9-]+)\s*(?:,\s*([^)]+))?\s*\)/g;
  if (!varRegex.test(value)) return value;

  // Reset regex lastIndex after test
  varRegex.lastIndex = 0;

  const resolved = value.replace(varRegex, (_match, varName, fallback) => {
    const computed = getComputedStyle(el).getPropertyValue(varName).trim();
    if (computed) {
      return convertToUsableColor(computed);
    }
    return fallback ? fallback.trim() : _match;
  });

  // If the result is still a complex CSS expression (e.g., color-mix()),
  // resolve it to a hex color through the canvas context
  if (resolved !== value && /color-mix\s*\(/i.test(resolved)) {
    return convertToUsableColor(resolved);
  }

  return resolved;
}

/**
 * Convert OKLCH or other CSS color formats to hex for ECharts.
 * Uses an offscreen canvas 2D context for color resolution.
 * @param {string} colorValue
 * @returns {string}
 */
function convertToUsableColor(colorValue) {
  // If already hex, return as-is
  if (/^#[0-9a-fA-F]{3,8}$/.test(colorValue)) return colorValue;

  // If it looks like a bare OKLCH value (without the oklch() wrapper),
  // try wrapping it
  if (/^\d/.test(colorValue) && colorValue.includes(' ')) {
    colorValue = `oklch(${colorValue})`;
  }

  // Use canvas 2D context for color conversion
  const ctx = getCanvasContext();
  ctx.fillStyle = '#000000'; // Reset
  ctx.fillStyle = colorValue;
  return ctx.fillStyle; // Returns as #rrggbb
}

/** @type {CanvasRenderingContext2D|null} */
let canvasCtx = null;

/**
 * @returns {CanvasRenderingContext2D}
 */
function getCanvasContext() {
  if (!canvasCtx) {
    const canvas = document.createElement('canvas');
    canvas.width = 1;
    canvas.height = 1;
    canvasCtx = canvas.getContext('2d');
  }
  return canvasCtx;
}

/**
 * Watch for theme changes (dark/light mode toggle) via MutationObserver
 * and custom 'bb-theme-changed' events.
 *
 * Observes the <html> element for class or data-theme attribute changes.
 * Also listens for 'bb-theme-changed' custom events on <html>, allowing
 * developers to trigger chart color updates after programmatically changing
 * CSS variables (e.g., dynamic theming, theme builders).
 *
 * Usage:
 *   document.documentElement.dispatchEvent(new CustomEvent('bb-theme-changed'));
 *
 * @param {Function} callback - Called when theme changes
 * @returns {Function} - Cleanup function to stop watching
 */
export function watchThemeChanges(callback) {
  const target = document.documentElement;

  const observer = new MutationObserver((mutations) => {
    for (const mutation of mutations) {
      if (mutation.type === 'attributes' &&
        (mutation.attributeName === 'class' || mutation.attributeName === 'data-theme')) {
        // Delay to let CSS variables update
        requestAnimationFrame(() => callback());
        break;
      }
    }
  });

  observer.observe(target, {
    attributes: true,
    attributeFilter: ['class', 'data-theme']
  });

  // Listen for custom theme change events (e.g., from dynamic theme editors)
  const onThemeChanged = () => requestAnimationFrame(() => callback());
  target.addEventListener('bb-theme-changed', onThemeChanged);

  return () => {
    observer.disconnect();
    target.removeEventListener('bb-theme-changed', onThemeChanged);
  };
}
