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

### Added
- Blazor EditForm and EditContext integration for automatic validation error display and focus

### Changed
- Documentation improvements and formatting fixes

## 2026-01-26

### Added
- LLM-friendly component documentation
- All 176 missing child components to comprehensive documentation

## 2026-01-24

### Added
- Content support to FieldSeparator component for centered content

### Changed
- Component version bumped to 1.0.15

## 2026-01-23

### Added
- Comprehensive form attributes to Input, Textarea, Checkbox, Switch, and RadioGroup components
- Name and Required attributes to RadioGroup in primitives
- Common input properties to input-related controls

### Changed
- Input-related visual styles updated to match latest shadcn
- Tailwind re-enabled in build process
- Dependencies version updates
- Component version bumped to 1.0.14
- Primitives version bumped to 1.0.14
- Version bumped to 1.0.12

## 2026-01-22

### Added
- SSR support in SidebarProvider and related components

## 2026-01-20

### Changed
- Assembly signature updated with all dependencies
- Fixed primitives reference in Components
- NuGet metadata updates for Components project
- README updates

## 2026-01-19

### Changed
- Target framework changed to .NET 10
- Package metadata finalized for v1 publishing
- Package name updates
- NuGet metadata updates

## 2026-01-18

### Added
- Grid NotifyItemsChanged pattern and Server-Side Row Model implementation

### Changed
- Tailwind CSS build enabled in project configuration

## 2026-01-17

### Added
- Server-side row model (SSRM) using AG Grid Enterprise module
- SSRM demo showcasing server-side sorting, filtering, and paging

### Changed
- GridImportMap refactored to simplify DX in App.razor
- Selection sample updated to use new TrackableObservableCollection

## 2026-01-14

### Added
- Comprehensive movie database demo with TMDb API integration
- Server-side row model with GridRowModelType and demo pages
- NotifyItemsChanged pattern with TrackedObservableCollection
- Grid Component Milestones 1-4 with complete demo pages
- Complete AG Grid state management with @bind-State and hash-based mutation detection

### Changed
- Grid added to indexes for production ready
- All Grid demos finalized with working functions

### Fixed
- Code review fixes for hash properties, firstRender check, and unused filterable property
- Type safety improvements

## 2026-01-13

### Added
- @bind-State with hash-based mutation detection for natural state management
- InitialState application and programmatic state updates for controlled sort/filter demos
- GetStateAsync to Grid component
- Three-level AG Grid theme customization with Shadcn integration

### Changed
- GridState and GridColumnState expanded with complete AG Grid properties
- All Grid demos finalized (minus sorting and states)
- Proper marshalling between JS and C# for row selections
- Comprehensive Transactions and Refresh API for granular controls

## 2026-01-12

### Fixed
- Case-sensitive IdField lookup in grid component
- ObservableCollection updates in grid
- Runtime theme switching support

## 2026-01-10

### Fixed
- Case-insensitive property lookup in SyncSelectionToGrid
- Multi-select demo changed to use ObservableCollection
- Runtime theme update support with UpdateThemeAsync

## 2026-01-09

### Changed
- Major revamp and refactor of data refresh infrastructure
- Added observability and partial update support

## 2026-01-07

### Fixed
- Default values for shadcn theme working properly
- Removed redundant defaults from JavaScript

## 2026-01-06

### Added
- Template rendering framework supporting cell templates and header templates
- GridAction for easy JS interop from Blazor components inside templates
- DataFormatString in GridColumn for easy data formatting

### Changed
- Templating demo finalized with DataFormatString implementation

## 2026-01-04

### Added
- Theming API support with shadcn design tokens
- ES module support for AG Grid

### Changed
- Switched from CDN to ES modules for AG Grid
- Theme application using native withParams API

### Fixed
- AG Grid theming regression
- Theme parameter handling

### Removed
- Manual CSS/JS loading infrastructure (200+ lines)

## 2026-01-03

### Added
- Comprehensive theme customization documentation
- Shadcn theme integration with dynamic CSS variable reading
- Support for any color format in CSS variables (hsl, oklch, rgba)

### Changed
- Refactored to use AG Grid's native withParams API
- Color values changed from hsl(var(--token)) to var(--token)
- Updated opacity colors to use color-mix()

### Fixed
- CSS variable handling improvements
- Parent validation improvements

---

**Note:** This changelog is based on git commit history. For a complete view of all commits, [visit the repository's commit history](https://github.com/jimmyps/blazor-shadcn-ui/commits/main).
