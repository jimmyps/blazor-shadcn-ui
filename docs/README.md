# NeoUI Documentation

This directory contains technical documentation, architectural decisions, implementation guides, and best practices for the NeoUI component library.

## 📱 Mobile Responsiveness

**NEW:** Comprehensive mobile support documentation

- **[Mobile Responsive Analysis](MOBILE_RESPONSIVE_ANALYSIS.md)** - Complete analysis of mobile support across all 95+ components, including findings, recommendations, and testing results
- **[Mobile Best Practices](MOBILE_BEST_PRACTICES.md)** - Practical guide for building mobile-responsive applications with NeoUI components, with code examples and patterns
- **[Mobile Checklist](MOBILE_CHECKLIST.md)** - Development and testing checklist for ensuring mobile responsiveness

### Mobile Support Summary

The NeoUI library provides **production-ready mobile responsiveness** (80/100 rating) with:

✅ **Strengths:**
- Smart Sidebar ↔ Sheet navigation at 768px breakpoint
- Touch-friendly components (44px minimum targets)
- Excellent accessibility (ARIA, screen readers)
- All overlay components work perfectly on mobile
- Mobile-first Tailwind CSS approach

⚠️ **Minor Enhancements Needed:**
- DataTable mobile layouts (recommendations provided)
- Chart component mobile testing
- Touch-action CSS optimizations

**Primary Mobile Breakpoint:** 768px (`md:` in Tailwind)

## 🏗️ Architecture

### Portal System

- **[Portal Architecture](PORTAL_ARCHITECTURE.md)** - Complete architecture of the hierarchical portal system for overlay components
- **[Hierarchical Portals](HIERARCHICAL_PORTALS.md)** - Deep dive into the two-tier portal system (Container + Overlay)
- **[Portal Migration Complete](PORTAL_MIGRATION_COMPLETE.md)** - Summary of portal system migration

### Component Fixes & Improvements

- **[Menubar Cascading Fix](MENUBAR_CASCADING_FIX.md)** - Fixed cascading submenus in Menubar component
- **[Menubar Portal ID Fix](MENUBAR_PORTAL_ID_FIX.md)** - Resolved portal ID conflicts in Menubar
- **[Nested Submenu Fix](NESTED_SUBMENU_FIX.md)** - Fixed deeply nested submenu rendering
- **[Submenu Animation Fix](SUBMENU_ANIMATION_FIX.md)** - Improved submenu open/close animations
- **[Submenu Hover Race Condition Fix](SUBMENU_HOVER_RACE_CONDITION_FIX.md)** - Resolved hover state race conditions

## 📂 Directory Structure

```
docs/
├── README.md                           # This file
├── MOBILE_RESPONSIVE_ANALYSIS.md       # Mobile support analysis
├── MOBILE_BEST_PRACTICES.md            # Mobile development guide
├── MOBILE_CHECKLIST.md                 # Mobile testing checklist
├── PORTAL_ARCHITECTURE.md              # Portal system architecture
├── HIERARCHICAL_PORTALS.md             # Portal hierarchy details
├── PORTAL_MIGRATION_COMPLETE.md        # Migration summary
├── MENUBAR_CASCADING_FIX.md           # Menubar fix documentation
├── MENUBAR_PORTAL_ID_FIX.md           # Portal ID fix
├── NESTED_SUBMENU_FIX.md              # Submenu nesting fix
├── SUBMENU_ANIMATION_FIX.md           # Animation improvements
├── SUBMENU_HOVER_RACE_CONDITION_FIX.md # Hover state fixes
└── internal/                           # Internal development docs
```

## 🚀 Getting Started

### For Component Users

1. Start with [Mobile Best Practices](MOBILE_BEST_PRACTICES.md) for mobile-responsive layouts
2. Review [Mobile Checklist](MOBILE_CHECKLIST.md) during development
3. Check component-specific documentation in `/demo/NeoUI.Demo.Shared/Pages/Components`

### For Contributors

1. Read [Portal Architecture](PORTAL_ARCHITECTURE.md) to understand overlay components
2. Review existing fix documentation for patterns
3. Follow mobile development guidelines when adding new components
4. Use the checklist for testing

## 📊 Documentation Index

### By Topic

**Mobile Development:**
- Mobile Responsive Analysis
- Mobile Best Practices  
- Mobile Checklist

**Architecture:**
- Portal Architecture
- Hierarchical Portals
- Portal Migration Complete

**Bug Fixes:**
- Menubar Cascading Fix
- Menubar Portal ID Fix
- Nested Submenu Fix
- Submenu Animation Fix
- Submenu Hover Race Condition Fix

### By Audience

**Application Developers:**
→ Start with Mobile Best Practices

**Component Library Contributors:**
→ Read Portal Architecture and existing fixes

**QA/Testing:**
→ Use Mobile Checklist

**Technical Leadership:**
→ Review Mobile Responsive Analysis

## 🔍 Quick References

### Mobile Breakpoints

```
Mobile:  < 768px  (default, no prefix)
Tablet:  ≥ 768px  (md:)
Desktop: ≥ 1024px (lg:)
```

### Common Responsive Patterns

```razor
<!-- Hide on mobile, show on desktop -->
<div class="hidden md:block">Desktop Content</div>

<!-- Show on mobile, hide on desktop -->
<div class="md:hidden">Mobile Content</div>

<!-- Stack on mobile, row on desktop -->
<div class="flex flex-col md:flex-row gap-4">
    <div>Column 1</div>
    <div>Column 2</div>
</div>
```

### Touch Target Minimum

**WCAG 2.1 Level AA:** 44px × 44px

```razor
<Button Size="ButtonSize.Lg">Optimal Touch Target</Button>
```

## 📈 Testing

### Mobile Testing Viewports

- **320px** - Smallest common mobile
- **375px** - iPhone SE
- **390px** - iPhone 12/13  
- **430px** - iPhone 14 Pro Max
- **768px** - Tablet breakpoint

### Recommended Tools

- Chrome DevTools (Device Mode)
- Lighthouse Mobile Audit
- Real device testing (iOS Safari, Chrome Mobile)

## 🤝 Contributing

When contributing documentation:

1. **Format:** Use Markdown with clear headings
2. **Code Examples:** Include Razor code samples
3. **Completeness:** Include problem, solution, and testing
4. **Links:** Cross-reference related documents
5. **Updates:** Keep README.md index current

## 📝 Document Templates

### Fix Documentation Template

```markdown
# [Component] [Issue] Fix

## Problem
[Describe the issue]

## Root Cause
[Explain why it happened]

## Solution
[Describe the fix]

## Implementation
[Code changes]

## Testing
[How to verify the fix]

## Related
[Links to related docs/issues]
```

### Feature Documentation Template

```markdown
# [Feature Name]

## Overview
[What it does]

## Usage
[How to use it with examples]

## API Reference
[Parameters, events, etc.]

## Examples
[Common use cases]

## Best Practices
[Recommendations]

## Troubleshooting
[Common issues]
```

## 🔗 External Resources

- [BlazorUI Component Library](https://github.com/jimmyps/blazor-shadcn-ui)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [MDN Web Docs - Mobile](https://developer.mozilla.org/en-US/docs/Web/Guide/Mobile)

## 📞 Support

- **Issues:** [GitHub Issues](https://github.com/jimmyps/blazor-shadcn-ui/issues)
- **Discussions:** [GitHub Discussions](https://github.com/jimmyps/blazor-shadcn-ui/discussions)
- **Mobile Issues:** Use `mobile` label

---

**Last Updated:** February 16, 2026  
**Maintained By:** NeoUI Contributors
