# Mobile Responsiveness Checklist

Use this checklist when developing or reviewing mobile-responsive features in BlazorUI applications.

## Pre-Development Checklist

- [ ] Reviewed mobile best practices documentation
- [ ] Understand target mobile breakpoints (768px primary)
- [ ] Know which devices/viewports to support
- [ ] Have access to mobile device testing tools

## Component Development Checklist

### Layout & Structure
- [ ] No horizontal scroll at any supported viewport width (320px-768px)
- [ ] Content stacks properly on mobile (single column by default)
- [ ] Adequate padding/margin on mobile (minimum 16px)
- [ ] Safe area insets respected (for notched devices)
- [ ] Fixed/sticky elements don't obstruct content

### Typography
- [ ] Base font size ≥16px (prevents iOS zoom)
- [ ] Text remains readable without zooming
- [ ] Line height adequate for touch selection (1.5-1.75)
- [ ] Headings scale appropriately (`text-2xl md:text-4xl` pattern)
- [ ] No text overflow or wrapping issues

### Interactive Elements
- [ ] Touch targets ≥44px × 44px (WCAG 2.1 Level AA)
- [ ] Buttons have adequate padding
- [ ] Links are easy to tap (not too close together)
- [ ] Form inputs are large enough for mobile keyboards
- [ ] Spacing between clickable elements ≥8px

### Navigation
- [ ] Mobile navigation accessible (hamburger menu or similar)
- [ ] Navigation menu opens/closes smoothly
- [ ] All menu items accessible on mobile
- [ ] Close button visible and accessible
- [ ] Backdrop dismisses navigation when tapped

### Forms
- [ ] Form fields stack vertically on mobile
- [ ] Labels visible and associated with inputs
- [ ] Input font size ≥16px (prevents zoom)
- [ ] Submit buttons accessible (bottom of form)
- [ ] Error messages visible and readable
- [ ] Multi-step forms show progress on mobile

### Tables & Data Grids
- [ ] Tables handle narrow viewports (scroll or card layout)
- [ ] Important columns remain visible
- [ ] Horizontal scroll indicators present (if scrollable)
- [ ] Alternative mobile layout considered (cards)
- [ ] Row actions accessible on mobile

### Images & Media
- [ ] Images scale responsively (`w-full h-auto`)
- [ ] Images use `loading="lazy"` attribute
- [ ] Responsive images with `srcset` (if appropriate)
- [ ] No image overflow beyond viewport
- [ ] Alt text present for all images

### Modals & Overlays
- [ ] Dialogs/modals adapt to mobile (full-width or proper sizing)
- [ ] Close button visible and accessible (44px target)
- [ ] Content doesn't overflow dialog viewport
- [ ] Scrolling works within modal if content is long
- [ ] Backdrop prevents body scroll

### Charts & Visualizations
- [ ] Charts scale to mobile viewport width
- [ ] Legends readable on small screens
- [ ] Data labels don't overlap
- [ ] Touch interactions work (if applicable)
- [ ] Axis labels remain readable

## Accessibility Checklist

### Touch & Gestures
- [ ] All functionality accessible via touch
- [ ] No hover-only interactions
- [ ] Swipe gestures optional (not required)
- [ ] Long-press alternatives provided (if used)

### Screen Readers
- [ ] ARIA labels present where needed
- [ ] sr-only text for icon buttons
- [ ] Semantic HTML elements used
- [ ] Focus order makes sense on mobile
- [ ] Skip navigation links work

### Keyboard Support
- [ ] Tab navigation works (Bluetooth keyboards)
- [ ] Focus indicators visible
- [ ] No keyboard traps
- [ ] Logical tab order maintained

### Color & Contrast
- [ ] Color contrast meets WCAG AA (4.5:1 for text)
- [ ] UI doesn't rely on color alone
- [ ] Dark mode works on mobile
- [ ] Focus states visible in both themes

## Testing Checklist

### Browser DevTools Testing
- [ ] Tested at 320px width (smallest common)
- [ ] Tested at 375px width (iPhone SE)
- [ ] Tested at 390-430px (modern iPhones)
- [ ] Tested at 768px (tablet breakpoint)
- [ ] Tested in portrait orientation
- [ ] Tested in landscape orientation

### Real Device Testing (if available)
- [ ] Tested on iPhone (iOS Safari)
- [ ] Tested on Android (Chrome Mobile)
- [ ] Tested on tablet device
- [ ] Touch interactions work correctly
- [ ] Swipe gestures work (if applicable)
- [ ] Performance is acceptable

### Cross-Browser Testing
- [ ] Chrome Mobile
- [ ] Safari iOS
- [ ] Firefox Mobile
- [ ] Samsung Internet (if targeting Android)

### Performance Testing
- [ ] Page loads within 3 seconds on 3G
- [ ] No layout shift during load
- [ ] Animations smooth (60fps)
- [ ] Images optimized for mobile
- [ ] Lazy loading implemented where appropriate

### Accessibility Testing
- [ ] Lighthouse mobile audit passed (≥90 score)
- [ ] VoiceOver tested (iOS)
- [ ] TalkBack tested (Android)
- [ ] Color contrast verified
- [ ] Focus management verified

## Responsive Patterns Checklist

### Using Correct Patterns
- [ ] Mobile-first approach (default is mobile, enhance for desktop)
- [ ] Tailwind breakpoints used correctly (`md:`, `lg:`, etc.)
- [ ] No `!important` used to override mobile styles
- [ ] CSS Grid/Flexbox used for layouts (not floats)
- [ ] Viewport units used appropriately

### Component Composition
- [ ] Sidebar uses SidebarProvider pattern
- [ ] Dialogs use DialogContent with responsive classes
- [ ] Sheets used for mobile-specific drawers
- [ ] Cards used for mobile data display (instead of tables)
- [ ] Proper spacing utilities applied

## Common Issues Checklist

### Check For These Common Problems
- [ ] ❌ No horizontal scrolling
- [ ] ❌ No text requiring zoom to read
- [ ] ❌ No tiny buttons (<44px)
- [ ] ❌ No overlapping clickable elements
- [ ] ❌ No fixed elements covering content
- [ ] ❌ No unscrollable overflow content
- [ ] ❌ No font sizes below 16px for inputs
- [ ] ❌ No hover-only critical functionality
- [ ] ❌ No viewport-breaking fixed widths
- [ ] ❌ No missing viewport meta tag

## Pre-Deployment Checklist

### Final Review
- [ ] All pages tested on mobile
- [ ] No console errors on mobile browsers
- [ ] Navigation works on all pages
- [ ] Forms submit successfully
- [ ] Links navigate correctly
- [ ] Images load properly
- [ ] No performance issues

### Documentation
- [ ] Mobile-specific features documented
- [ ] Known mobile limitations noted
- [ ] Browser support clearly stated
- [ ] Testing instructions provided

### Monitoring
- [ ] Analytics set up for mobile users
- [ ] Error tracking configured
- [ ] Performance monitoring enabled
- [ ] User feedback mechanism in place

## Post-Deployment Checklist

### User Testing
- [ ] Gather feedback from mobile users
- [ ] Monitor analytics for mobile usage patterns
- [ ] Track mobile-specific errors
- [ ] Review performance metrics

### Continuous Improvement
- [ ] Address mobile user feedback
- [ ] Update for new device sizes
- [ ] Optimize based on performance data
- [ ] Stay current with mobile web standards

---

## Quick Viewport Size Reference

```
Mobile Phones (Portrait):
- iPhone SE:         375 × 667
- iPhone 12/13:      390 × 844
- iPhone 14 Pro:     393 × 852
- iPhone 14 Pro Max: 430 × 932
- Samsung Galaxy S21: 360 × 800
- Pixel 5:           393 × 851

Tablets (Portrait):
- iPad Mini:         768 × 1024
- iPad Air:          820 × 1180
- iPad Pro 11":      834 × 1194
- iPad Pro 12.9":    1024 × 1366

Common Breakpoints:
- Mobile:  < 768px
- Tablet:  768px - 1024px
- Desktop: > 1024px
```

---

## Minimum Requirements Summary

| Aspect | Minimum Requirement |
|--------|-------------------|
| **Touch Target** | 44px × 44px |
| **Font Size (Input)** | 16px |
| **Font Size (Body)** | 16px |
| **Spacing (Touch Elements)** | 8px |
| **Contrast Ratio** | 4.5:1 (text) |
| **Viewport Padding** | 16px |
| **Load Time (3G)** | < 3 seconds |

---

## Resources

- [Mobile Best Practices](/docs/MOBILE_BEST_PRACTICES.md)
- [Mobile Analysis Report](/docs/MOBILE_RESPONSIVE_ANALYSIS.md)
- [BlazorUI Component Docs](/docs)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [Google Mobile SEO](https://developers.google.com/search/mobile-sites)

---

## Usage

Print this checklist or keep it handy during development. Check off items as you complete them to ensure comprehensive mobile support.

**Pro Tip:** Create a GitHub issue template based on this checklist for mobile-related feature requests or bug reports.
