# Changelog

All notable changes to this project will be documented in this file.

## 2026-01-31

### Added
- DialogService for programmatic dialogs with async/await API
- LinkButton component for semantic navigation
- Sidebar improvements for production use cases (independent scrolling, state persistence)
- PersistentComponentState for zero-flicker state handover
- ScrollArea auto-hide behavior and enhancements
- Component navigation updates (LinkButton and DialogService to sidebar, index, spotlight)

### Changed
- SidebarProvider improvements (IHttpContextAccessor optional, IServiceProvider injection)
- SidebarCollapsibleGroup refactored for WebAssembly compatibility
- Sidebar component layout updates (h-screen, overflow-y-auto)
- ScrollArea enhancements (auto-hide, data-state attribute, dynamic content updates)
- State management refactoring (collapsible-state.js module, cookie operations moved to JavaScript)
- GlobalSuppressions updates

### Fixed
- DialogHost binding to use proper AlertDialog
- WebAssembly compatibility (IHttpContextAccessor dependencies, state persistence)
- Cookie persistence (URL-encoding, Secure flag, backward compatibility)
- Animation flickering during render mode transitions
- ScrollArea dynamic content height updates

### Removed
- Unused storage-helpers.js file
- JavaScript files no longer used

## 2026-01-29

### Added
- Column alignment support for DataTable component

## 2026-01-28

### Added
- Comprehensive LLM-friendly component reference documentation
- Descriptions and usage examples for all 62 components

### Fixed
- Typography section formatting
- Escaped @ symbols in HoverCard example
- HoverCard example simplified for clarity

## 2026-01-27

### Changed
- Documentation improvements and formatting fixes

---

**Note:** This changelog is based on git commit history. For a complete view of all commits, [visit the repository's commit history](https://github.com/jimmyps/blazor-shadcn-ui/commits/main).
