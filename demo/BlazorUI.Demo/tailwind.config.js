/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: ["class"],
  content: [
    './Pages/**/*.{razor,html,cs}',
    './Shared/**/*.{razor,html,cs}',
    './Components/**/*.{razor,html,cs}',
    '../../src/BlazorUI.Components/**/*.{razor,html,cs}',
    '../../src/BlazorUI.Primitives/**/*.{razor,html,cs}',
    '../../src/BlazorUI.Icons/**/*.{razor,html,cs}',
  ],
  theme: {
    extend: {
      colors: {
        border: "var(--border)",
        input: "var(--input)",
        ring: "var(--ring)",
        background: "var(--background)",
        foreground: "var(--foreground)",
        primary: {
          DEFAULT: "var(--primary)",
          foreground: "var(--primary-foreground)",
        },
        secondary: {
          DEFAULT: "var(--secondary)",
          foreground: "var(--secondary-foreground)",
        },
        destructive: {
          DEFAULT: "var(--destructive)",
          foreground: "var(--destructive-foreground)",
        },
        muted: {
          DEFAULT: "var(--muted)",
          foreground: "var(--muted-foreground)",
        },
        accent: {
          DEFAULT: "var(--accent)",
          foreground: "var(--accent-foreground)",
        },
        popover: {
          DEFAULT: "var(--popover)",
          foreground: "var(--popover-foreground)",
        },
        card: {
          DEFAULT: "var(--card)",
          foreground: "var(--card-foreground)",
        },
        sidebar: {
          DEFAULT: "var(--sidebar)",
          foreground: "var(--sidebar-foreground)",
          primary: "var(--sidebar-primary)",
          "primary-foreground": "var(--sidebar-primary-foreground)",
          accent: "var(--sidebar-accent)",
          "accent-foreground": "var(--sidebar-accent-foreground)",
          border: "var(--sidebar-border)",
          ring: "var(--sidebar-ring)",
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
      width: {
        "sidebar": "var(--sidebar-width)",
        "sidebar-mobile": "var(--sidebar-width-mobile)",
        "sidebar-icon": "var(--sidebar-width-icon)",
      },
      keyframes: {
        "accordion-down": {
          from: { height: "0" },
          to: { height: "var(--radix-accordion-content-height, auto)" },
        },
        "accordion-up": {
          from: { height: "var(--radix-accordion-content-height, auto)" },
          to: { height: "0" },
        },
        "collapsible-down": {
          from: { height: "0" },
          to: { height: "var(--radix-collapsible-content-height, auto)" },
        },
        "collapsible-up": {
          from: { height: "var(--radix-collapsible-content-height, auto)" },
          to: { height: "0" },
        },
        "dialog-panel-in": {
          from: {
            opacity: "0",
            transform: "translateY(var(--dialog-panel-translate-start, -8px)) scale(var(--dialog-panel-scale-start, 0.96))",
          },
          to: {
            opacity: "1",
            transform: "translateY(0) scale(1)",
          },
        },
        "dialog-panel-out": {
          from: {
            opacity: "1",
            transform: "translateY(0) scale(1)",
          },
          to: {
            opacity: "0",
            transform: "translateY(var(--dialog-panel-translate-exit, 8px)) scale(var(--dialog-panel-scale-exit, 0.98))",
          },
        },
        "dialog-item-in": {
          from: {
            opacity: "0",
            transform: "translateY(var(--dialog-item-translate-start, -4px))",
          },
          to: {
            opacity: "1",
            transform: "translateY(0)",
          },
        },
        "dialog-item-out": {
          from: {
            opacity: "1",
            transform: "translateY(0)",
          },
          to: {
            opacity: "0",
            transform: "translateY(var(--dialog-item-translate-exit, 4px))",
          },
        },
        "dialog-overlay-in": {
          from: { opacity: "0" },
          to: { opacity: "1" },
        },
        "dialog-overlay-out": {
          from: { opacity: "1" },
          to: { opacity: "0" },
        },
      },
      animation: {
        "accordion-down": "accordion-down 0.2s ease-out",
        "accordion-up": "accordion-up 0.2s ease-out",
        "collapsible-down": "collapsible-down 0.2s ease-out",
        "collapsible-up": "collapsible-up 0.2s ease-out",
        "dialog-panel-in": "dialog-panel-in var(--dialog-panel-duration, 200ms) var(--dialog-panel-easing, cubic-bezier(0.4, 0, 0.2, 1))",
        "dialog-panel-out": "dialog-panel-out var(--dialog-panel-duration, 200ms) var(--dialog-panel-easing, cubic-bezier(0.4, 0, 0.2, 1))",
        "dialog-item-in": "dialog-item-in var(--dialog-item-duration, 150ms) var(--dialog-item-easing, cubic-bezier(0.4, 0, 0.2, 1))",
        "dialog-item-out": "dialog-item-out var(--dialog-item-duration, 150ms) var(--dialog-item-easing, cubic-bezier(0.4, 0, 0.2, 1))",
        "dialog-overlay-in": "dialog-overlay-in var(--dialog-overlay-duration, 150ms) ease-out",
        "dialog-overlay-out": "dialog-overlay-out var(--dialog-overlay-duration, 150ms) ease-in",
      },
      transitionProperty: {
        height: "height",
      },
    },
  },
  plugins: [],
}
