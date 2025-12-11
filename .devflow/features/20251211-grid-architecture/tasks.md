# Implementation Tasks: Blazor-native Data Grid Architecture

**Feature:** 20251211-grid-architecture
**Status:** Planning  
**Created:** 2025-12-11

This document breaks down the grid architecture implementation into atomic, executable tasks organized by milestone.

---

## Milestone 1: Core Object Model and DTOs

### Task 1.1: Create Grid Enums
**Description:** Create all grid-related enums with JSON serialization support  
**Estimated Effort:** 2 hours  
**Dependencies:** None  
**Files:**
- GridSelectionMode.cs
- GridPagingMode.cs
- GridVirtualizationMode.cs
- GridColumnPinPosition.cs
- GridSortDirection.cs
- GridFilterOperator.cs
- GridTheme.cs
- GridDensity.cs

**Acceptance:**
- [ ] All enums use `[JsonConverter(typeof(JsonStringEnumConverter))]`
- [ ] XML documentation on all enum types and values
- [ ] Enums match spec.md definitions

### Task 1.2: Create State DTOs
**Description:** Create grid state data transfer objects  
**Estimated Effort:** 3 hours  
**Dependencies:** Task 1.1  
**Files:**
- GridSortDescriptor.cs
- GridFilterDescriptor.cs
- GridColumnState.cs
- GridState.cs

**Acceptance:**
- [ ] All DTOs use `[JsonPropertyName]` attributes for camelCase serialization
- [ ] DTOs are JSON serializable/deserializable without data loss
- [ ] Unit tests verify serialization round-trip
- [ ] XML documentation on all properties

### Task 1.3: Create Data Request/Response DTOs
**Description:** Create DTOs for server-side data loading pipeline  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 1.1, 1.2  
**Files:**
- GridDataRequest.cs
- GridDataResponse.cs

**Acceptance:**
- [ ] GridDataRequest includes pagination, sorting, filtering parameters
- [ ] GridDataResponse includes items, totalCount, filteredCount
- [ ] Generic type parameter `<TItem>` on GridDataRequest
- [ ] JSON serialization works correctly

### Task 1.4: Create Internal Object Model
**Description:** Create internal grid and column definition classes  
**Estimated Effort:** 3 hours  
**Dependencies:** Task 1.1  
**Files:**
- GridColumnDefinition.cs
- GridDefinition.cs

**Acceptance:**
- [ ] GridColumnDefinition holds all column configuration
- [ ] GridColumnDefinition includes template references
- [ ] GridDefinition holds all grid configuration
- [ ] GridDefinition includes event callbacks
- [ ] Both classes are internal (not exposed in public API)

---

## Milestone 2: Grid Components

### Task 2.1: Create GridColumn Component
**Description:** Implement GridColumn component that registers with parent Grid  
**Estimated Effort:** 4 hours  
**Dependencies:** Task 1.4  
**Files:**
- GridColumn.razor
- GridColumn.razor.cs

**Acceptance:**
- [ ] Component accepts all parameters from spec.md
- [ ] Component uses CascadingParameter to get parent Grid
- [ ] Component registers itself with parent Grid on initialization
- [ ] Component throws clear error if not inside a Grid
- [ ] ToDefinition() method converts to GridColumnDefinition
- [ ] XML documentation on all public parameters
- [ ] Component has no render output

### Task 2.2: Create Grid Component Shell
**Description:** Create basic Grid component structure without renderer integration  
**Estimated Effort:** 5 hours  
**Dependencies:** Task 2.1  
**Files:**
- Grid.razor
- Grid.razor.cs

**Acceptance:**
- [ ] Component accepts all parameters from spec.md
- [ ] Component uses CascadingValue to expose itself to child columns
- [ ] RegisterColumn() method collects child GridColumn components
- [ ] BuildGridDefinition() method creates GridDefinition from columns
- [ ] Loading template support
- [ ] CSS class calculation (theme, density)
- [ ] XML documentation on all public parameters

### Task 2.3: Add CSS for Grid Themes
**Description:** Add CSS classes for grid themes and densities  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 2.2  
**Files:**
- wwwroot/css/blazorui-input.css

**Acceptance:**
- [ ] .grid-default, .grid-striped, .grid-bordered, .grid-minimal classes
- [ ] .grid-comfortable, .grid-compact, .grid-spacious classes
- [ ] Classes use shadcn CSS variables
- [ ] Dark mode support via .dark class
- [ ] Matches shadcn design system aesthetics

---

## Milestone 3: Renderer Abstraction

### Task 3.1: Create Renderer Interfaces
**Description:** Create IGridRenderer, IGridRendererCapabilities, IGridExportService  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 1.4  
**Files:**
- Services/Grid/IGridRenderer.cs
- Services/Grid/IGridRendererCapabilities.cs
- Services/Grid/IGridExportService.cs

**Acceptance:**
- [ ] IGridRenderer interface defined with all methods from plan.md
- [ ] IGridRendererCapabilities properties defined
- [ ] IGridExportService methods defined
- [ ] Interfaces are generic where needed
- [ ] XML documentation on all interface members

### Task 3.2: Create GridRendererFactory
**Description:** Create factory for creating grid renderer instances  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 3.1  
**Files:**
- Services/Grid/GridRendererFactory.cs

**Acceptance:**
- [ ] Factory creates renderer based on configuration
- [ ] Factory registered in DI container
- [ ] Factory handles renderer not found errors
- [ ] Unit tests verify factory behavior

### Task 3.3: Integrate Renderer into Grid Component
**Description:** Update Grid component to use IGridRenderer  
**Estimated Effort:** 3 hours  
**Dependencies:** Task 2.2, 3.1  
**Files:**
- Grid.razor.cs (update)

**Acceptance:**
- [ ] Grid injects IGridRenderer
- [ ] Grid calls InitializeAsync on AfterRenderAsync
- [ ] Grid calls UpdateDataAsync when Items parameter changes
- [ ] Grid implements IAsyncDisposable, calls renderer.DisposeAsync
- [ ] Error handling for renderer initialization failures

---

## Milestone 4: AG Grid Renderer

### Task 4.1: Create AG Grid Renderer Class
**Description:** Implement AgGridRenderer class  
**Estimated Effort:** 8 hours  
**Dependencies:** Task 3.1  
**Files:**
- Services/Grid/AgGridRenderer.cs

**Acceptance:**
- [ ] Class implements IGridRenderer
- [ ] Class implements IGridRendererCapabilities
- [ ] InitializeAsync loads JS module and creates grid
- [ ] UpdateDataAsync sets row data
- [ ] UpdateStateAsync applies sort/filter/column state
- [ ] GetStateAsync retrieves current state
- [ ] BuildAgGridConfig converts GridDefinition to AG Grid options
- [ ] DotNet reference for callbacks
- [ ] Proper disposal in DisposeAsync

### Task 4.2: Create AG Grid JavaScript Interop Module
**Description:** Create aggrid-renderer.js for AG Grid interop  
**Estimated Effort:** 10 hours  
**Dependencies:** Task 4.1  
**Files:**
- wwwroot/js/aggrid-renderer.js

**Acceptance:**
- [ ] createGrid function initializes AG Grid
- [ ] setRowData function updates grid data
- [ ] applyState function applies GridState to AG Grid
- [ ] getState function extracts GridState from AG Grid
- [ ] Event handlers for sort/filter/selection changes
- [ ] Callback to .NET OnGridStateChanged
- [ ] Server-side row model datasource integration
- [ ] Callback to .NET OnDataRequested
- [ ] destroy function cleans up AG Grid instance

### Task 4.3: Implement Column Definition Mapping
**Description:** Map GridColumnDefinition to AG Grid columnDefs  
**Estimated Effort:** 6 hours  
**Dependencies:** Task 4.1  
**Files:**
- AgGridRenderer.cs (update)

**Acceptance:**
- [ ] Field, Header, Sortable, Filterable mapped
- [ ] Width, MinWidth, MaxWidth converted to pixels
- [ ] Pinned position mapped (left/right)
- [ ] AllowResize, AllowReorder mapped
- [ ] CellTemplate handling (templateRenderer or HTML generation)
- [ ] HeaderTemplate handling
- [ ] FilterTemplate handling
- [ ] ValueSelector mapped to valueGetter

### Task 4.4: Implement Template Precompilation
**Description:** Render Blazor templates to HTML for AG Grid cells  
**Estimated Effort:** 8 hours  
**Dependencies:** Task 4.3  
**Files:**
- AgGridRenderer.cs (update)
- Services/Grid/TemplateRenderer.cs (new)

**Acceptance:**
- [ ] CellTemplate renders to HTML string
- [ ] HTML injected into AG Grid cells
- [ ] Template context includes TItem data
- [ ] Handles null/empty templates gracefully
- [ ] Performance acceptable for 100+ rows
- [ ] (Optional) Caching for repeated templates

### Task 4.5: Implement State Synchronization
**Description:** Sync GridState between Blazor and AG Grid  
**Estimated Effort:** 6 hours  
**Dependencies:** Task 4.2  
**Files:**
- AgGridRenderer.cs (update)
- aggrid-renderer.js (update)

**Acceptance:**
- [ ] Sort state synced bidirectionally
- [ ] Filter state synced bidirectionally
- [ ] Column visibility synced
- [ ] Column width synced
- [ ] Column pinning synced
- [ ] Column order synced
- [ ] Selection state synced
- [ ] OnStateChanged callback invoked on changes

### Task 4.6: Implement Server-Side Row Model
**Description:** Wire up OnDataRequest for server-side paging  
**Estimated Effort:** 5 hours  
**Dependencies:** Task 4.2  
**Files:**
- AgGridRenderer.cs (update)
- aggrid-renderer.js (update)

**Acceptance:**
- [ ] AG Grid server-side row model configured
- [ ] getRows callback invokes OnDataRequested in .NET
- [ ] GridDataRequest built from AG Grid params
- [ ] GridDataResponse items loaded into AG Grid
- [ ] Pagination controls work
- [ ] Sort/filter triggers new data request
- [ ] Loading indicators shown during fetch

### Task 4.7: Implement Selection
**Description:** Wire up row selection with AG Grid  
**Estimated Effort:** 4 hours  
**Dependencies:** Task 4.5  
**Files:**
- AgGridRenderer.cs (update)
- aggrid-renderer.js (update)

**Acceptance:**
- [ ] Single selection mode works
- [ ] Multiple selection mode works
- [ ] Selection state included in GridState
- [ ] OnSelectionChanged callback invoked
- [ ] SelectedItems binding works
- [ ] Selection persists across paging (if client-side)

---

## Milestone 5: Export Service

### Task 5.1: Create CSV Export Service
**Description:** Implement CsvExportService  
**Estimated Effort:** 4 hours  
**Dependencies:** Task 1.4  
**Files:**
- Services/Grid/CsvExportService.cs

**Acceptance:**
- [ ] ExportToCsvAsync generates valid CSV
- [ ] Headers from visible columns
- [ ] Data rows from Items
- [ ] CSV escaping (quotes, commas, newlines)
- [ ] ValueSelector used if available
- [ ] Returns byte array (UTF-8)
- [ ] Unit tests verify CSV format

### Task 5.2: Integrate Export with Grid Component
**Description:** Add export methods to Grid component  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 5.1  
**Files:**
- Grid.razor.cs (update)

**Acceptance:**
- [ ] ExportToCsvAsync public method
- [ ] Method uses IGridExportService
- [ ] Method returns byte array
- [ ] Example in documentation showing download trigger

---

## Milestone 6: Documentation and Examples

### Task 6.1: Write XML Documentation
**Description:** Comprehensive XML docs on all public types  
**Estimated Effort:** 4 hours  
**Dependencies:** All component tasks  
**Files:**
- All .cs files (update)

**Acceptance:**
- [ ] XML comments on all public classes
- [ ] XML comments on all public properties/parameters
- [ ] XML comments on all public methods
- [ ] `<summary>`, `<remarks>`, `<example>` tags used appropriately
- [ ] Code examples in `<example>` tags

### Task 6.2: Create Basic Usage Example
**Description:** Simple grid with columns  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 6.1  
**Files:**
- demo/Examples/GridBasicExample.razor

**Acceptance:**
- [ ] Example shows Grid with 3-4 columns
- [ ] Sortable columns
- [ ] Filterable columns
- [ ] Runs without errors
- [ ] Visually matches shadcn design

### Task 6.3: Create Custom Template Example
**Description:** Grid with cell templates  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 6.1  
**Files:**
- demo/Examples/GridTemplateExample.razor

**Acceptance:**
- [ ] Example shows CellTemplate with Badge component
- [ ] Example shows CellTemplate with Button component
- [ ] Example shows custom header template
- [ ] Runs without errors

### Task 6.4: Create Server-Side Paging Example
**Description:** Grid with OnDataRequest callback  
**Estimated Effort:** 3 hours  
**Dependencies:** Task 4.6  
**Files:**
- demo/Examples/GridServerPagingExample.razor

**Acceptance:**
- [ ] Example implements OnDataRequest callback
- [ ] Simulates API call with Task.Delay
- [ ] Returns GridDataResponse with paged data
- [ ] Shows loading state
- [ ] Pagination controls work

### Task 6.5: Create State Persistence Example
**Description:** Grid with state saved to localStorage  
**Estimated Effort:** 3 hours  
**Dependencies:** Task 4.5  
**Files:**
- demo/Examples/GridStatePersistenceExample.razor

**Acceptance:**
- [ ] Example saves state to localStorage on change
- [ ] Example loads state on initialization
- [ ] State includes sort, filter, column visibility
- [ ] State persists across page refreshes

### Task 6.6: Create Export Example
**Description:** Grid with CSV export button  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 5.2  
**Files:**
- demo/Examples/GridExportExample.razor

**Acceptance:**
- [ ] Example shows export button
- [ ] Button triggers CSV download
- [ ] Downloaded file is valid CSV
- [ ] Example includes file download JS helper

### Task 6.7: Write Migration Guide
**Description:** Guide for migrating from DataTable to Grid  
**Estimated Effort:** 2 hours  
**Dependencies:** All tasks  
**Files:**
- docs/MigrationGuideDataTableToGrid.md

**Acceptance:**
- [ ] Document lists parameter mappings
- [ ] Document lists component name changes
- [ ] Document shows side-by-side code examples
- [ ] Document notes breaking changes
- [ ] Document notes new features only in Grid

---

## Milestone 7: Testing

### Task 7.1: Write Unit Tests for DTOs
**Description:** Test serialization of all DTOs  
**Estimated Effort:** 3 hours  
**Dependencies:** Task 1.2, 1.3  
**Files:**
- tests/GridStateTests.cs
- tests/GridDataRequestTests.cs

**Acceptance:**
- [ ] GridState serialization round-trip tests
- [ ] GridDataRequest serialization tests
- [ ] GridDataResponse serialization tests
- [ ] All tests pass

### Task 7.2: Write Component Tests for GridColumn
**Description:** bUnit tests for GridColumn registration  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 2.1  
**Files:**
- tests/GridColumnTests.cs

**Acceptance:**
- [ ] Test GridColumn registers with parent Grid
- [ ] Test GridColumn throws error without parent Grid
- [ ] Test ToDefinition() creates correct definition
- [ ] All tests pass

### Task 7.3: Write Component Tests for Grid
**Description:** bUnit tests for Grid component  
**Estimated Effort:** 4 hours  
**Dependencies:** Task 2.2, 3.3  
**Files:**
- tests/GridTests.cs

**Acceptance:**
- [ ] Test Grid renders with columns
- [ ] Test Grid builds GridDefinition correctly
- [ ] Test Grid calls renderer methods
- [ ] Test Grid handles loading state
- [ ] All tests pass

### Task 7.4: Write Integration Tests for AG Grid Renderer
**Description:** Test AG Grid renderer with mock JS runtime  
**Estimated Effort:** 5 hours  
**Dependencies:** Task 4.1, 4.2  
**Files:**
- tests/AgGridRendererTests.cs

**Acceptance:**
- [ ] Test renderer initialization
- [ ] Test UpdateDataAsync
- [ ] Test UpdateStateAsync
- [ ] Test GetStateAsync
- [ ] Test disposal
- [ ] All tests pass

### Task 7.5: Write Integration Tests for Export
**Description:** Test CSV export service  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 5.1  
**Files:**
- tests/CsvExportServiceTests.cs

**Acceptance:**
- [ ] Test CSV generation with various data
- [ ] Test CSV escaping (quotes, commas, newlines)
- [ ] Test empty data
- [ ] All tests pass

---

## Milestone 8: Polish and Deployment

### Task 8.1: Accessibility Audit
**Description:** Ensure grid meets WCAG 2.1 AA  
**Estimated Effort:** 4 hours  
**Dependencies:** All component tasks  
**Files:**
- Various (fixes)

**Acceptance:**
- [ ] Keyboard navigation works (arrow keys, tab, enter)
- [ ] Screen reader support with ARIA attributes
- [ ] Focus management for interactive cells
- [ ] Color contrast meets AA standard
- [ ] No accessibility violations in automated tests

### Task 8.2: Performance Testing
**Description:** Benchmark grid with large datasets  
**Estimated Effort:** 3 hours  
**Dependencies:** Task 4.6  
**Files:**
- tests/PerformanceTests.cs

**Acceptance:**
- [ ] Grid renders 100 rows in <500ms
- [ ] Grid supports 10,000+ rows with virtualization
- [ ] Scrolling at 60 FPS with virtualization
- [ ] Memory usage reasonable
- [ ] Performance benchmarks documented

### Task 8.3: Bundle Size Analysis
**Description:** Verify bundle size targets  
**Estimated Effort:** 2 hours  
**Dependencies:** All tasks  
**Files:**
- None (analysis only)

**Acceptance:**
- [ ] Core grid components <50KB
- [ ] AG Grid renderer adds ~200KB (separate chunk)
- [ ] No unexpected dependencies
- [ ] Bundle analysis documented

### Task 8.4: Final Code Review
**Description:** Review all code for quality and consistency  
**Estimated Effort:** 4 hours  
**Dependencies:** All tasks  
**Files:**
- All files

**Acceptance:**
- [ ] Code follows BlazorUI conventions
- [ ] No AG Grid types in public APIs
- [ ] All XML documentation complete
- [ ] No TODOs or FIXMEs
- [ ] Consistent naming conventions
- [ ] All warnings resolved

### Task 8.5: Update Architecture Documentation
**Description:** Update .devflow/architecture.md with grid components  
**Estimated Effort:** 2 hours  
**Dependencies:** Task 8.4  
**Files:**
- .devflow/architecture.md

**Acceptance:**
- [ ] Grid components section added
- [ ] Renderer abstraction pattern documented
- [ ] Relationship to DataTable documented
- [ ] Diagrams updated

---

## Summary

**Total Estimated Effort:** ~145 hours (~4 weeks at 36 hours/week)

**Critical Path:**
1. Milestone 1 (Object Model) → Milestone 2 (Components) → Milestone 3 (Abstraction) → Milestone 4 (AG Grid) → Milestone 6 (Examples)

**Parallel Work Opportunities:**
- Milestone 5 (Export) can be done in parallel with Milestone 4
- Milestone 6 (Documentation) can start as soon as components are functional
- Milestone 7 (Testing) can be incremental throughout

**Risk Mitigation:**
- AG Grid integration (Milestone 4) is the highest risk - allocate extra buffer time
- Template precompilation (Task 4.4) may need optimization - benchmark early
- State sync (Task 4.5) complexity may require iteration

**Dependencies:**
- AG Grid Community library (MIT license, ~200KB)
- No other external dependencies

---

## Approval

This task breakdown requires approval before beginning implementation.

**Approved by:** _Pending_  
**Date:** _Pending_
