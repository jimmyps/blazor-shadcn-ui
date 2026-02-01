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
- GridImportMap refactored to simplify developer experience in App.razor
- Selection sample updated to use new TrackableObservableCollection

## 2026-01-14

### Added
- Comprehensive movie database demo with TMDb API integration
- Server-side row model with GridRowModelType and demo pages
- NotifyItemsChanged pattern with TrackableObservableCollection
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

## 2025-12-25

### Added
- Automatic navigation and active state detection in Sidebar component

## 2025-12-24

### Added
- Grid component with 20 comprehensive demo examples
- Grid demo tabs following charting pattern
- SearchInterval in CommandInput for faster query performance

### Changed
- Component index updates
- ContextMenu updated to use latest positioning pattern
- Dialog-based component animations polished with Tailwind config refactor
- Command demo examples updated

### Fixed
- Checkbox state sync regression after upstream merge
- Popover opacity conflicting with animation

## 2025-12-23

### Added
- Grid Component Milestones 1-4 (Core Object Model, Components, Renderer Abstraction, AG Grid Renderer)
- Grid architecture documentation (GRID_DEMOS_V1.md, GRID_VS_DATATABLE.md)
- CI/CD setup for GitHub and Azure App Service

### Changed
- .NET 10 migration completed
- README updated with latest architectural changes
- All components list added to sidebar, index, and spotlight search

### Fixed
- Missing using directives in Grid component
- BuildGridDefinition timing issues
- Page number calculation in Grid
- CSV formula injection mitigation added

## 2025-12-22

### Added
- Global search feature with Spotlight command palette
- Platform-specific keyboard shortcuts (Cmd/Ctrl+K)

### Changed
- Build targets for Tailwind re-enabled
- Command palette visual finalized

## 2025-12-21

### Fixed
- Command OnSelect not working
- Spotlight command palette for global search finalized

## 2025-12-20

### Added
- Motion component to global search

## 2025-12-19

### Added
- Declarative Chart API implementation
- Theme-aware charts with automatic refresh on theme change
- Legend text color support

### Changed
- All chart implementations rebuilt to production quality

## 2025-12-18

### Added
- Theme-aware implementation for all charts
- Automatic chart refresh on theme change (JavaScript-only approach)

### Changed
- ScatterChart, RadarChart, and ComposedChart examples rebuilt to production quality
- RadarChart code comments improved for clarity

### Fixed
- RadarChart to use DataKey for extracting series values
- Bubble chart and dataset descriptions

## 2025-12-17

### Added
- Animation parameters to ChartBase
- Full Emphasis support for Pie charts
- Chart enhancements for Pie, Scatter, Radar, and RadialBar charts

### Changed
- All chart builders now map animation properties
- Chart defaults finalized for best developer experience
- Resizer enhanced with observer

### Fixed
- Pie, Scatter, Radar, and RadialBar chart rendering issues

## 2025-12-16

### Added
- Animation system for declarative chart API
- Fill/LinearGradient/Stop with complete ECharts gradient mapping
- 6 production-ready features: YAxis position, Grid styling, Axis min/max, Tooltip styling, Symbol customization, Series opacity

### Fixed
- ECharts renderer: v6 download, improved color detection, single-flight loading

## 2025-12-15

### Added
- Comprehensive sample data for AreaChart examples
- Updated shadcn demos with production-ready features (Area, Bar, Line, Pie charts)

### Changed
- All chart demos replaced with new declarative API versions
- Div containers used instead of canvas for ECharts SVG rendering mode

## 2025-12-12

### Added
- Tooltip component improvements

## 2025-12-11

### Fixed
- Code style and formatting inconsistencies in the codebase

## 2025-12-10

### Changed
- Minor UI styling and layout adjustments across existing components

## 2025-12-09

### Changed
- Internal component refactoring and minor bug fixes

## 2025-12-08

### Changed
- Minor accessibility, spacing, and theme consistency updates across existing UI components

## 2025-12-07

### Added
- HeightAnimation documentation and usage examples to CommandDemo
- CSS variables to blazorui-input.css

### Changed
- Visual styles with auto dynamic list height

### Fixed
- Animation conflicts in Spotlight demo with !animate-none

## 2025-12-06

### Added
- Calendar base component with keyboard navigation
- MultiSelect component
- Smooth expand/collapse animations for Accordion and Collapsible
- Smooth height animation for command palette filtering
- Spotlight-style command palette with global keyboard shortcut support

### Changed
- Primitives package reference updated to 1.1.0
- Calendar component improvements: centered caption, dropdown mode, styled border/shadow
- IsFocused documentation simplified

### Fixed
- Focus ring persisting after clicking outside calendar
- Double focus ring issue with @key on tbody
- Dialog positioning with fixed top position
- Keyboard focus lost after month changes
- Calendar rendering, focus ring visibility, intrinsic width

## 2025-12-05

### Added
- Viewport boundary detection for context menu positioning
- Transparent overlay and scroll lock to ContextMenu
- Recursive submenu closing support for nested submenus
- Icon examples section to menu demo pages

### Changed
- Navigation refactored to follow DropdownMenu pattern
- ContextMenu refactored to follow DropdownMenu/Menubar pattern
- Calendar component code cleanup

### Fixed
- Submenu overflow issue and checkbox/radio item padding
- Submenu UX: prevent close on hover, update visual state, Escape closes all menus
- Keyboard navigation in submenus and focus restoration
- Hover highlighting and keyboard scroll in ContextMenu
- Disabled menu item style in DropdownMenuItem
- SubTrigger registration with parent submenu for proper keyboard navigation
- Overlay z-index to prevent negative values

## 2025-12-04

### Added
- Input OTP component with error state styling
- Menubar component
- CheckboxItem, RadioGroup/RadioItem, and Submenu components to DropdownMenu, Menubar, and ContextMenu
- Error state styling (AriaInvalid) to Input OTP component
- Animation utilities to tailwind config
- Hover-to-switch behavior for menubar triggers

### Changed
- Demo pages updated to showcase new menu features
- Animation utilities moved from demo to component library tailwind config

### Fixed
- Menubar dropdown positioning and keyboard scroll behavior
- Keyboard navigation and alphabet input support
- Focus UX: no editing middle slots, only show active state when focused
- CSS animations for dropdown menus by not setting opacity inline
- Text alignment (centered) and separator styling
- Input behaviors during backspace

### Removed
- eval() usage for security (replaced with makeVisible: true)

## 2025-12-03

### Added
- Initial Menubar and Input OTP component skeletons

---

**Note:** This changelog is based on git commit history. For a complete view of all commits, [visit the repository's commit history](https://github.com/jimmyps/blazor-shadcn-ui/commits/main).
