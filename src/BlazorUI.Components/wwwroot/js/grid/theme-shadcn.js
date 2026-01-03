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
    return value ? `hsl(${value})` : fallback;
  };
  
  const defaultParams = {
    // ===== COLORS (Map to shadcn tokens) =====
    accentColor: getCssVar('--primary', '#2563eb'),
    backgroundColor: getCssVar('--background', '#ffffff'),
    foregroundColor: getCssVar('--foreground', '#000000'),
    borderColor: getCssVar('--border', '#e5e7eb'),
    headerBackgroundColor: getCssVar('--muted', '#f9fafb'),
    headerForegroundColor: getCssVar('--foreground', '#000000'),
    rowHoverColor: getCssVar('--accent', 'rgba(37, 99, 235, 0.1)').replace(')', ' / 0.1)'),
    selectedRowBackgroundColor: getCssVar('--primary', 'rgba(37, 99, 235, 0.1)').replace(')', ' / 0.1)'),
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
