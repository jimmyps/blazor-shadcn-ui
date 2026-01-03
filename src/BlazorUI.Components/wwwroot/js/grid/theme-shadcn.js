// Import AG Grid theming API
import { themeQuartz } from 'ag-grid-community';

/**
 * Creates the Shadcn theme by extending AG Grid's Quartz theme
 * and mapping to shadcn/ui design tokens.
 * @param {Object} customParams - Custom parameters to override defaults
 * @returns AG Grid theme with shadcn token integration
 */
export function createShadcnTheme(customParams = {}) {
  // Read shadcn CSS variables from document root
  const root = getComputedStyle(document.documentElement);
  
  // Helper to get CSS variable value
  const getCssVar = (name, fallback) => {
    const value = root.getPropertyValue(name).trim();
    if (!value) return fallback;
    
    // If value already contains a color function (hsl, rgb, rgba, oklch, etc.), use it as is
    if (value.includes('(')) {
      return value;
    }
    
    // Otherwise, assume it's shadcn-style space-separated HSL values (e.g., "222.2 84% 4.9%")
    // and wrap with hsl()
    return `hsl(${value})`;
  };
  
  const defaultParams = {
    // ===== COLORS (Map to shadcn tokens) =====
    accentColor: getCssVar('--primary', '#2563eb'),
    backgroundColor: getCssVar('--background', '#ffffff'),
    foregroundColor: getCssVar('--foreground', '#000000'),
    borderColor: getCssVar('--border', '#e5e7eb'),
    headerBackgroundColor: getCssVar('--muted', '#f9fafb'),
    headerForegroundColor: getCssVar('--foreground', '#000000'),
    invalidColor: getCssVar('--destructive', '#dc2626'),
    tooltipBackgroundColor: getCssVar('--popover', '#ffffff'),
    tooltipTextColor: getCssVar('--popover-foreground', '#000000'),
    
    // ===== TYPOGRAPHY =====
    fontFamily: 'var(--font-sans)',
    
    // ===== BORDERS =====
    borderRadius: 4, // Will be overridden by calc(var(--radius) - 2px) via CSS if needed
    borders: true,
    
    // Note: Spacing, rowHeight, headerHeight, fontSize are set by density presets
    // and should not have hardcoded defaults here
  };
  
  // Merge with custom params (custom params take precedence)
  const params = { ...defaultParams, ...customParams };
  
  return themeQuartz.withParams(params);
}
