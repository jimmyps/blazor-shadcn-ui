# Feature Specification: Blazor-native Data Grid Architecture

**Status:** Planning
**Created:** 2025-12-11T11:00:00Z
**Feature ID:** 20251211-grid-architecture

## Problem Statement

BlazorUI currently has a basic DataTable component that wraps the Table primitive with sorting, filtering, and pagination. However, it lacks:
- Advanced grid features (column pinning, resizing, reordering, virtualization)
- Renderer abstraction for swapping grid implementations (AG Grid, native Blazor, etc.)
- Server-side data loading with a unified contract
- State persistence and serialization
- Export capabilities
- Row-level actions and bulk commands

Developers building enterprise applications need a production-ready data grid that combines Blazor-native DX with the power of advanced grid libraries like AG Grid, without being locked into a specific implementation.

## Goals and Objectives

- Design a **Blazor-native, renderer-agnostic data grid** component architecture with no JS/AG Grid concepts in public APIs
- Provide **idiomatic Blazor DX** with declarative `<Grid>` and `<GridColumn>` components using `RenderFragment<TItem>` templates
- Implement **pluggable renderer architecture** (`IGridRenderer`) supporting AG Grid JS first, pure Blazor later
- Support **core enterprise grid features**: sorting, filtering, selection, paging, virtualization, column management, state persistence
- Enable **server-side data loading** with unified `GridDataRequest`/`GridDataResponse` contract
- Integrate with **shadcn theming** (GridTheme, GridDensity) for consistent styling
- Provide **export capabilities** via renderer-agnostic `IGridExportService`
- Include **diagnostics and capability detection** (`IGridRendererCapabilities`) for unsupported features

## User Stories

1. **As a developer building a CRUD interface**, I want a Grid component with declarative column definitions, so that I can quickly build data tables without manual DOM manipulation.

2. **As a developer**, I want to use custom cell templates with `RenderFragment<TItem>`, so that I can render badges, buttons, and complex UI in grid cells using Blazor components.

3. **As a developer**, I want the grid to be renderer-agnostic, so that I can swap between AG Grid, native Blazor, or other implementations without changing my code.

4. **As a developer managing large datasets**, I want server-side paging and filtering with `OnDataRequest`, so that I can efficiently load data from APIs without loading everything into memory.

5. **As a developer**, I want to persist grid state (sort, filter, column visibility, width, pinning), so that users' preferences are remembered across sessions.

6. **As a developer**, I want column pinning, resizing, and reordering, so that users can customize their grid layout.

7. **As a developer building admin panels**, I want row selection with bulk actions, so that users can perform operations on multiple rows at once.

8. **As a developer**, I want the grid to integrate with shadcn theming, so that it looks consistent with other components in light and dark modes.

9. **As a developer**, I want to export grid data to CSV/Excel, so that users can download reports.

10. **As a developer**, I want diagnostics when a renderer doesn't support a feature, so that I know what works and can plan accordingly.

## Acceptance Criteria

### Core Architecture
- [ ] `Grid<TItem>` component created with generic type inference and cascading type parameter
- [ ] `GridColumn<TItem>` child component created with automatic TItem inference from parent Grid
- [ ] Internal `GridDefinition<TItem>` and `GridColumnDefinition<TItem>` object model created
- [ ] `IGridRenderer` interface defined with Initialize, UpdateData, UpdateState, GetState methods
- [ ] No AG Grid or JS-specific types exposed in public component APIs

### Enums and Types
- [ ] `GridSelectionMode` enum created (None, Single, Multiple)
- [ ] `GridPagingMode` enum created (None, Client, Server, InfiniteScroll)
- [ ] `GridVirtualizationMode` enum created (Auto, None, RowOnly, RowAndColumn)
- [ ] `GridTheme` enum created (Default, Striped, Bordered, Minimal)
- [ ] `GridDensity` enum created (Comfortable, Compact, Spacious)
- [ ] `GridColumnPinPosition` enum created (None, Left, Right)
- [ ] `GridSortDirection` enum created (None, Ascending, Descending)
- [ ] `GridFilterOperator` enum created (Equals, NotEquals, Contains, etc.)

### State Management
- [ ] `GridState` class created with sorting, filtering, paging, column state
- [ ] `GridColumnState` class created with visibility, width, pinning, order properties
- [ ] `GridSortDescriptor` class created with field, direction, order properties
- [ ] `GridFilterDescriptor` class created with field, operator, value, caseSensitive properties
- [ ] All state classes are JSON serializable with `[JsonPropertyName]` attributes
- [ ] `InitialState` parameter implemented on Grid component
- [ ] `OnStateChanged` event callback implemented for state persistence

### Data Loading Pipeline
- [ ] `GridDataRequest<TItem>` class created with startIndex, count, sort/filter descriptors, pagination
- [ ] `GridDataResponse<TItem>` class created with items, totalCount, filteredCount
- [ ] `OnDataRequest` callback parameter implemented on Grid component
- [ ] Client-side paging mode implemented (all data loaded once)
- [ ] Server-side paging mode implemented (data fetched page-by-page via OnDataRequest)
- [ ] Infinite scroll paging mode implemented (incremental loading)

### Column Features
- [ ] Sortable columns implemented with multi-column sorting support
- [ ] Filterable columns implemented with operator-based filtering
- [ ] Column pinning implemented (left/right) with GridColumnPinPosition
- [ ] Column resizing implemented with AllowResize parameter
- [ ] Column reordering implemented with AllowReorder parameter
- [ ] Column visibility toggle implemented with IsVisible parameter
- [ ] Column width configuration (Width, MinWidth, MaxWidth parameters)

### Selection
- [ ] SelectionMode parameter implemented (None, Single, Multiple)
- [ ] SelectedItems binding implemented with `@bind-SelectedItems`
- [ ] OnSelectionChanged event callback implemented
- [ ] Selection state persisted in GridState

### Templating
- [ ] CellTemplate parameter implemented as `RenderFragment<TItem>`
- [ ] HeaderTemplate parameter implemented as `RenderFragment`
- [ ] FilterTemplate parameter implemented as `RenderFragment`
- [ ] CellEditTemplate parameter implemented as `RenderFragment<TItem>` (optional for v1)
- [ ] Templates work with all renderer implementations

### Theming Integration
- [ ] GridTheme parameter implemented with CSS class mapping
- [ ] GridDensity parameter implemented with padding/spacing variations
- [ ] Integration with shadcn CSS variables (--border, --muted, etc.)
- [ ] Automatic dark mode support via .dark class

### Export Service
- [ ] `IGridExportService` interface created with ExportToCsvAsync, ExportToExcelAsync methods
- [ ] CSV export implementation created (renderer-agnostic)
- [ ] Excel export implementation created (optional, separate package)

### Renderer Capabilities
- [ ] `IGridRendererCapabilities` interface created
- [ ] Capability properties defined (SupportsVirtualization, SupportsColumnPinning, etc.)
- [ ] UnsupportedFeatures array for diagnostics
- [ ] Warnings logged when using unsupported features

### AG Grid Renderer (v1)
- [ ] `AgGridRenderer` class created implementing `IGridRenderer`
- [ ] AG Grid JavaScript interop module created (aggrid-renderer.js)
- [ ] GridDefinition to AG Grid columnDefs mapping implemented
- [ ] Template precompilation/HTML generation for AG Grid cells
- [ ] State synchronization (sort, filter, selection, paging) implemented
- [ ] Column pinning, resizing, reordering wired to AG Grid
- [ ] Server-side row model integration for OnDataRequest
- [ ] Infinite scroll row model integration

### Row Actions and Commands
- [ ] Action column pattern documented with cell template examples
- [ ] Bulk command pattern documented using selection + custom UI
- [ ] (Optional) GridCommands component for structured command surface

### Performance
- [ ] Grid supports 10,000+ rows with virtualization enabled
- [ ] Render time < 500ms for 100-row grids
- [ ] Smooth scrolling at 60 FPS with virtualization
- [ ] Template precompilation optimized for JS-backed grids

### Accessibility
- [ ] WCAG 2.1 AA compliance verified
- [ ] Keyboard navigation implemented (arrow keys, tab, enter, space, escape)
- [ ] Screen reader support with ARIA attributes
- [ ] Focus management for interactive cells

### Documentation
- [ ] XML documentation on all public types and parameters
- [ ] Basic usage examples (simple grid with columns)
- [ ] Custom cell template examples
- [ ] Server-side data loading example
- [ ] State persistence example
- [ ] Theming and styling guide
- [ ] Export functionality example
- [ ] Migration guide from DataTable component

## Technical Constraints

- Must support both Blazor WebAssembly and Blazor Server hosting models
- AG Grid Community Edition (MIT license) - no enterprise features in v1
- Bundle size: Core grid components < 50KB, AG Grid renderer adds ~200KB
- Browser support: Chrome, Firefox, Safari, Edge (last 2 versions)
- .NET 8+ required

## Out of Scope (Future Milestones)

- Pivot tables and advanced grouping
- Full AG Grid Enterprise API exposure
- Advanced row/column virtualization tuning
- End-user charting integration
- Plugin system or renderer plugins
- Master-detail grids
- Tree data support

## Dependencies

### External
- AG Grid Community (MIT, ~200KB) - for AG Grid renderer
- (Optional) ClosedXML or EPPlus - for Excel export

### Internal
- BlazorUI.Components.Utilities (ClassNames.cn)
- Microsoft.JSInterop
- System.Text.Json (serialization)

## Success Metrics

- Grid component API has zero AG Grid-specific types
- All templates work with any renderer implementation
- State serialization round-trips without data loss
- Server-side paging works with 100,000+ row datasets
- Export generates valid CSV/Excel files
- Grid integrates visually with shadcn design system
- All acceptance criteria checked off
- Documentation complete with working examples

## Risks and Mitigations

### Risk 1: AG Grid Integration Complexity
**Severity:** High  
**Mitigation:** Start with minimal AG Grid features. Abstract all AG-specific logic behind IGridRenderer. Test early and often.

### Risk 2: Template Precompilation for JS
**Severity:** Medium  
**Mitigation:** Use server-side rendering for templates initially. Optimize with client-side HTML generation if needed.

### Risk 3: Performance with Large Datasets
**Severity:** Medium  
**Mitigation:** Mandatory virtualization for >1,000 rows. Server-side paging recommended for >10,000 rows. Benchmark regularly.

### Risk 4: State Sync Complexity
**Severity:** Medium  
**Mitigation:** Well-defined state DTOs. Clear ownership: Blazor owns state, renderer reflects it. Use diffing for optimization.

### Risk 5: Renderer Portability
**Severity:** Low  
**Mitigation:** IGridRenderer interface is well-defined. Future renderers (native Blazor) will reuse same object model.

## Open Questions

1. **Should we support inline editing in v1?**
   - **Decision:** Yes, via `CellEditTemplate`, but keep it simple (no form validation in grid).

2. **How to handle custom toolbar/actions?**
   - **Decision:** Provide `<GridToolbar>` slot in future. For v1, document patterns using cell templates.

3. **Should GridColumn infer TItem from parent Grid?**
   - **Decision:** Yes, using `CascadingTypeParameter` attribute.

4. **Excel export library choice?**
   - **Decision:** Start with CSV only (trivial). Add Excel as optional package later (ClosedXML or EPPlus).

5. **Localization strategy?**
   - **Decision:** Use `LocalizationKeyPrefix` parameter, integrate with existing i18n infrastructure (future).

## Ambiguity Resolution

### A. `Field` vs `Property`
**Decision:** GridColumn uses `Field` (string) to match AG Grid JSON mapping.  
**Rationale:** DataTable uses `Property` (Func<TData, TValue> lambda) because it's client-side C# only. Grid needs string field names for AG Grid columnDefs.

### B. Template Precompilation Strategy
**Decision (v1):** Use AG Grid's class-based `cellRenderer` with DOM element creation.  
**Implementation:** 
- Pre-render Blazor `RenderFragment<TItem>` to HTML using HtmlRenderer
- Pass HTML to custom `BlazorDomRenderer` JavaScript class via `cellRendererParams`
- Renderer creates DOM element with `innerHTML` (safe, Blazor-sanitized)
- **NOT** HTML string return (deprecated in modern AG Grid)

**Limitation:** v1 does not support interactive Blazor components in cells (buttons with @onclick won't work)  
**Future (v2):** Lazy rendering with JSInterop callbacks for interactivity

### C. Theming Integration
**Decision:** Use `ag-theme-quartz` as base, override with shadcn CSS variables.  
**Implementation:** Map `--primary`, `--background`, `--border` etc. to AG Grid theme variables.

### D. Error Handling for Invalid Fields
**Decision:** Validate column field names early, throw descriptive exceptions.  
**Rationale:** DataTable fails silently when property doesn't exist. Grid should not - throw `InvalidOperationException` with clear message during initialization.

### E. Selection State Persistence
**Decision (v1):** Selection state lost across pagination in client-side mode.  
**Rationale:** AG Grid client-side model uses row references, not IDs.  
**Future (v2):** Track selection by row ID for persistence across pages.

### F. GridColumn Type Inference
**Decision:** Yes, use `[CascadingTypeParameter]` attribute.  
**Implementation:** GridColumn<TItem> infers TItem from parent Grid<TItem> automatically.

## Related Features

- **20251210-charting-components** - Similar renderer architecture pattern
- **20251111-data-table-component** - Migration source, will be superseded by Grid
- **20251110-data-table-primitive** - Table primitive used internally

## Approval

This specification requires approval before proceeding to technical plan and implementation.

**Approved by:** _Pending_  
**Date:** _Pending_
