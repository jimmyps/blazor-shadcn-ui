using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using NeoUI.Blazor.Primitives;

namespace NeoUI.Blazor;

/// <summary>
/// A styled data table component that wraps the Table Primitive with automatic sorting,
/// filtering, pagination, and row selection capabilities.
/// </summary>
/// <typeparam name="TData">The type of data items in the table.</typeparam>
/// <remarks>
/// <para>
/// DataTable provides a complete table solution with declarative column definitions,
/// automatic data processing, and shadcn styling. It handles common table features
/// out-of-the-box while maintaining flexibility through templates and callbacks.
/// </para>
/// <para>
/// Features:
/// - Declarative column API via DataTableColumn child components
/// - Automatic sorting, filtering, and pagination (hybrid mode with overrides)
/// - Row selection (single/multiple) with checkboxes
/// - Optional toolbar with global search and column visibility toggle
/// - Empty and loading state templates
/// - Full shadcn styling with hover states and transitions
/// - Accessibility support (ARIA attributes, keyboard navigation)
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;DataTable TData="Person" Data="@people" SelectionMode="DataTableSelectionMode.Multiple"&gt;
///     &lt;Columns&gt;
///         &lt;DataTableColumn Property="@(p => p.Name)" Header="Name" Sortable Filterable /&gt;
///         &lt;DataTableColumn Property="@(p => p.Age)" Header="Age" Sortable /&gt;
///     &lt;/Columns&gt;
/// &lt;/DataTable&gt;
/// </code>
/// </example>
public partial class DataTable<TData> : ComponentBase, IAsyncDisposable where TData : class
{
    /// <summary>
    /// Public class for storing column data without component parameters.
    /// This avoids BL0005 warnings when creating column instances programmatically.
    /// </summary>
    public class ColumnData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the column.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the header text displayed for this column.
        /// </summary>
        public string Header { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the property accessor function that retrieves the column value from a data item.
        /// </summary>
        public Func<TData, object> Property { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether the column can be sorted.
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// Gets or sets whether the column is included in global search filtering.
        /// </summary>
        public bool Filterable { get; set; }

        /// <summary>
        /// Gets or sets whether the column is currently visible.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the width CSS value for the column (e.g., "200px", "20%").
        /// </summary>
        public string? Width { get; set; }

        /// <summary>
        /// Gets or sets the minimum width CSS value for the column.
        /// </summary>
        public string? MinWidth { get; set; }

        /// <summary>
        /// Gets or sets the maximum width CSS value for the column.
        /// </summary>
        public string? MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the custom render template for cells in this column.
        /// </summary>
        public RenderFragment<TData>? CellTemplate { get; set; }

        /// <summary>
        /// Gets or sets additional CSS classes to apply to cells in this column.
        /// </summary>
        public string? CellClass { get; set; }

        /// <summary>
        /// Gets or sets additional CSS classes to apply to the column header.
        /// </summary>
        public string? HeaderClass { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment for the column content.
        /// </summary>
        public ColumnAlignment Alignment { get; set; } = ColumnAlignment.Left;

        /// <summary>
        /// Gets or sets which edge this column is pinned to during horizontal scrolling.
        /// </summary>
        public ColumnPinnedSide Pinned { get; set; } = ColumnPinnedSide.None;

        /// <summary>
        /// Gets or sets whether the column exposes a drag-to-resize handle on its right edge.
        /// Resolved from <see cref="DataTableColumn{TData,TValue}.Resizable"/> falling back to the table-level <c>Resizable</c> parameter.
        /// </summary>
        public bool ResizeEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the column can be reordered by dragging its header.
        /// Resolved from <see cref="DataTableColumn{TData,TValue}.Reorderable"/> falling back to the table-level <c>Reorderable</c> parameter.
        /// </summary>
        public bool ReorderEnabled { get; set; }
    }

    /// <summary>
    /// Stores the list of registered column definitions.
    /// </summary>
    private List<ColumnData> _columns = new();

    /// <summary>
    /// Width of the multi-select checkbox column in pixels.
    /// Derived from the Tailwind class "w-12" (3 rem = 48 px at the default 16 px root font size)
    /// used for the selection cell in both header and body rows.
    /// If the class ever changes, update this constant to match.
    /// </summary>
    private const int SelectionColumnWidthPx = 48;

    /// <summary>
    /// Maintains the table state including sorting, filtering, pagination, and selection.
    /// </summary>
    private TableState<TData> _tableState = new();

    /// <summary>
    /// Contains the final processed data after filtering, sorting, and pagination.
    /// </summary>
    private IEnumerable<TData> _processedData = Array.Empty<TData>();

    /// <summary>
    /// Contains the data after filtering but before pagination.
    /// </summary>
    private IEnumerable<TData> _filteredData = Array.Empty<TData>();

    /// <summary>
    /// Stores the current global search filter value.
    /// </summary>
    private string _globalSearchValue = string.Empty;

    /// <summary>
    /// Version counter for column visibility changes to track render dependencies.
    /// </summary>
    private int _columnsVersion = 0;

    /// <summary>
    /// Tracks whether the select-all dropdown is currently open.
    /// </summary>
    private bool _selectAllDropdownOpen = false;

    /// <summary>Blazor Virtualize component reference used to trigger RefreshDataAsync.</summary>
    private Virtualize<TData>? _virtualizeRef;

    /// <summary>Pre-sorted/filtered item list supplied to the client-side Virtualize component.</summary>
    private List<TData> _virtualizeItems = new();

    /// <summary>True when the component operates in virtualised server-side mode.</summary>
    private bool IsVirtualizedServerMode => ItemsProvider is not null;

    /// <summary>True when any form of virtualised rendering is active.</summary>
    private bool IsVirtualized => IsVirtualizedServerMode || Virtualize;

    /// <summary>
    /// Cached reference to the ServerData delegate for ShouldRender optimization.
    /// </summary>
    private Func<DataTableRequest, Task<DataTableResult<TData>>>? _lastServerData;

    /// <summary>
    /// Cached reference to the last data collection for ShouldRender optimization.
    /// </summary>
    private IEnumerable<TData>? _lastData;

    /// <summary>
    /// Cached selection mode for ShouldRender optimization.
    /// </summary>
    private DataTableSelectionMode _lastSelectionMode;

    /// <summary>
    /// Cached loading state for ShouldRender optimization.
    /// </summary>
    private bool _lastIsLoading;

    /// <summary>
    /// Cached columns version for ShouldRender optimization.
    /// </summary>
    private int _lastColumnsVersion;

    /// <summary>
    /// Cached global search value for ShouldRender optimization.
    /// </summary>
    private string _lastGlobalSearchValue = string.Empty;

    /// <summary>
    /// Version counter for selection changes to track render dependencies.
    /// </summary>
    private int _selectionVersion = 0;

    /// <summary>
    /// Cached selection version for ShouldRender optimization.
    /// </summary>
    private int _lastSelectionVersion = 0;

    /// <summary>
    /// Cached selected items collection for parameter change detection.
    /// </summary>
    private IReadOnlyCollection<TData>? _lastSelectedItems;

    /// <summary>
    /// Version counter for pagination changes to track render dependencies.
    /// </summary>
    private int _paginationVersion = 0;

    /// <summary>
    /// Cached pagination version for ShouldRender optimization.
    /// </summary>
    private int _lastPaginationVersion = 0;

    /// <summary>
    /// Incremented each time ProcessDataAsync completes a server-side fetch so ShouldRender
    /// can detect that _processedData has changed even when no external parameters changed.
    /// </summary>
    private int _serverResultVersion = 0;

    /// <summary>
    /// Cached server result version for ShouldRender optimization.
    /// </summary>
    private int _lastServerResultVersion = -1;

    /// <summary>
    /// Cached Dense value for ShouldRender optimization.
    /// </summary>
    private bool _lastDense;

    /// <summary>
    /// Cached HeaderBackground value for ShouldRender optimization.
    /// </summary>
    private bool _lastHeaderBackground;

    /// <summary>
    /// Cached HeaderBorder value for ShouldRender optimization.
    /// </summary>
    private bool _lastHeaderBorder;

    /// <summary>
    /// Cached HeaderClass value for ShouldRender optimization.
    /// </summary>
    private string? _lastHeaderClass;

    /// <summary>
    /// Cached HeaderRowClass value for ShouldRender optimization.
    /// </summary>
    private string? _lastHeaderRowClass;

    /// <summary>
    /// Cached BodyRowClass value for ShouldRender optimization.
    /// </summary>
    private string? _lastBodyRowClass;

    /// <summary>
    /// Cached Striped value for ShouldRender optimization.
    /// </summary>
    private bool _lastStriped;

    /// <summary>
    /// Cached StripeClass value for ShouldRender optimization.
    /// </summary>
    private string? _lastStripeClass;

    /// <summary>
    /// Cached CellBorder value for ShouldRender optimization.
    /// </summary>
    private bool _lastCellBorder;

    /// <summary>
    /// Cached ColumnsVisibility value for ShouldRender optimization.
    /// </summary>
    private bool _lastColumnsVisibility;

    /// <summary>Cached Virtualize value for ShouldRender optimization.</summary>
    private bool _lastVirtualize;

    /// <summary>Cached ItemsProvider reference for ShouldRender optimization.</summary>
    private DataTableVirtualProvider<TData>? _lastItemsProvider;

    /// <summary>Cached Height value for ShouldRender optimization.</summary>
    private string _lastHeight = string.Empty;

    /// <summary>Cached ItemHeight value for ShouldRender optimization.</summary>
    private float _lastItemHeight;

    /// <summary>Cached VirtualizeOverscanCount value for ShouldRender optimization.</summary>
    private int _lastVirtualizeOverscanCount;

    // ── Tree-mode state ──────────────────────────────────────────────────────

    /// <summary>Represents a flattened row in the tree, carrying depth and expand state.</summary>
    private record TreeRow(TData Item, int Depth, bool HasChildren, bool IsLoading, bool IsExpanded);

    /// <summary>Flattened ordered list of visible tree rows.</summary>
    private List<TreeRow> _treeRows = new();

    /// <summary>Working set of expanded item keys.</summary>
    private HashSet<string> _expandedRowsInternal = new();

    /// <summary>Last known reference to ExpandedValues for change detection in OnParametersSetAsync.</summary>
    private HashSet<string>? _lastExpandedValues;

    /// <summary>Cache of lazily-fetched children keyed by parent item key.</summary>
    private Dictionary<string, List<TData>> _fetchedChildren = new();

    /// <summary>Set of item keys currently being loaded (shows spinner).</summary>
    private HashSet<string> _loadingNodes = new();

    /// <summary>Incremented whenever the tree expand/collapse state changes.</summary>
    private int _treeVersion;

    /// <summary>Cached tree version for ShouldRender optimization.</summary>
    private int _lastTreeVersion;

    // ── JS interop state ─────────────────────────────────────────────────────

    /// <summary>Reference to the table scroll container, passed to JS for resize/reorder init.</summary>
    private ElementReference _tableContainerRef;

    /// <summary>Cached reference to the imported datatable.js ES module.</summary>
    private IJSObjectReference? _jsModule;

    /// <summary>Cleanup handle returned by <c>initColumnResize</c> in datatable.js.</summary>
    private IJSObjectReference? _resizeCleanup;
    /// <summary>Guards against double-init of resize JS when multiple renders overlap the await.</summary>
    private bool _resizeInitializing;

    /// <summary>Cleanup handle returned by <c>initColumnReorder</c> in datatable.js.</summary>
    private IJSObjectReference? _reorderCleanup;
    /// <summary>Guards against double-init of reorder JS when multiple renders overlap the await.</summary>
    private bool _reorderInitializing;

    /// <summary>.NET object reference passed to JS so it can invoke JSInvokable callbacks.</summary>
    private DotNetObjectReference<DataTable<TData>>? _dotNetRef;

    // ── Row context menu state ──────────────────────────────────────────────────

    /// <summary>Component reference used to programmatically open the row context menu.</summary>
    private ContextMenu? _rowContextMenuRef;

    /// <summary>The row item that was right-clicked.</summary>
    private TData? _contextMenuItem;

    /// <summary>
    /// Gets or sets the client-side data source for the table.
    /// When <see cref="ServerData"/> is provided this can be omitted (defaults to an empty collection).
    /// </summary>
    [Parameter]
    public IEnumerable<TData> Data { get; set; } = Array.Empty<TData>();

    /// <summary>
    /// Gets or sets a server-side data callback.  When set the component bypasses the
    /// local filter / sort / pagination pipeline and calls this function instead, passing
    /// a <see cref="DataTableRequest"/> that contains the current page, page size, sort
    /// column, sort direction, and search text.  The callback must return a
    /// <see cref="DataTableResult{TData}"/> with the page of items and the total count.
    /// </summary>
    [Parameter]
    public Func<DataTableRequest, Task<DataTableResult<TData>>>? ServerData { get; set; }

    /// <summary>True when the component is operating in server-side paged data mode.</summary>
    private bool IsServerMode => ServerData is not null;

    /// <summary>True when tree mode is active (ChildrenProperty or LoadChildrenAsync is set, and not in VirtualizedServer mode).</summary>
    private bool IsTreeMode => !IsVirtualizedServerMode && (ChildrenProperty is not null || LoadChildrenAsync is not null);

    /// <summary>
    /// Server-side data provider for virtualised infinite-scroll rendering.
    /// When set, <see cref="Data"/> and <see cref="ServerData"/> are ignored, pagination is
    /// hidden, and rows are rendered via <c>&lt;Virtualize&gt;</c>.
    /// The delegate receives a <see cref="DataTableVirtualRequest"/> with the current window
    /// offset, sort descriptors, and search text, and must return an
    /// <see cref="Microsoft.AspNetCore.Components.Web.Virtualization.ItemsProviderResult{TData}"/>
    /// containing the slice and the total matching-item count.
    /// </summary>
    [Parameter] public DataTableVirtualProvider<TData>? ItemsProvider { get; set; }

    /// <summary>
    /// When <c>true</c> and <see cref="Data"/> is set, renders rows via
    /// <c>&lt;Virtualize Items&gt;</c> instead of a plain loop.
    /// All filtering and sorting still run client-side; only DOM nodes outside the
    /// viewport are removed. Pagination is hidden in this mode.
    /// Has no effect when <see cref="ItemsProvider"/> is also set.
    /// </summary>
    [Parameter] public bool Virtualize { get; set; }

    /// <summary>
    /// Approximate height of a single row in pixels used by the virtualizer as
    /// <c>ItemSize</c>. Defaults to 40 px (dense row height). Adjust to match your
    /// actual row height when <see cref="Virtualize"/> is <c>true</c> or
    /// <see cref="ItemsProvider"/> is set.
    /// </summary>
    [Parameter] public float ItemHeight { get; set; } = 40f;

    /// <summary>
    /// CSS height of the virtualised scroll container, e.g. <c>"400px"</c> or
    /// <c>"calc(100vh - 200px)"</c>. Required when <see cref="Virtualize"/> is
    /// <c>true</c> or <see cref="ItemsProvider"/> is set. Defaults to <c>"400px"</c>.
    /// </summary>
    [Parameter] public string Height { get; set; } = "400px";

    /// <summary>
    /// Number of extra rows rendered beyond the visible viewport to reduce blank
    /// flicker during fast scrolling. Defaults to 3.
    /// </summary>
    [Parameter] public int VirtualizeOverscanCount { get; set; } = 3;

    /// <summary>
    /// Provides child items for in-memory tree rendering.
    /// Tree mode is active when this or <see cref="LoadChildrenAsync"/> is set.
    /// </summary>
    [Parameter] public Func<TData, IEnumerable<TData>?>? ChildrenProperty { get; set; }

    /// <summary>
    /// Provides children lazily per node via an async fetch.
    /// Mutually exclusive benefit with <see cref="ChildrenProperty"/>; only one should be set.
    /// </summary>
    [Parameter] public Func<TData, Task<IEnumerable<TData>>>? LoadChildrenAsync { get; set; }

    /// <summary>
    /// Optional hint telling the table whether a node has children (shows expander without pre-loading).
    /// </summary>
    [Parameter] public Func<TData, bool>? HasChildrenField { get; set; }

    /// <summary>
    /// Required in tree mode: returns a stable unique string key for each item, used to track expand state.
    /// Falls back to <see cref="RuntimeHelpers.GetHashCode"/> when not provided.
    /// </summary>
    [Parameter] public Func<TData, string>? ValueField { get; set; }

    /// <summary>
    /// Gets or sets the set of currently expanded item keys for two-way binding.
    /// </summary>
    [Parameter] public HashSet<string>? ExpandedValues { get; set; }

    /// <summary>
    /// Event callback invoked when the expanded item keys change.
    /// </summary>
    [Parameter] public EventCallback<HashSet<string>> ExpandedValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets the column definitions as child content.
    /// Use DataTableColumn components to define columns declaratively.
    /// </summary>
    [Parameter]
    public RenderFragment? Columns { get; set; }

    /// <summary>
    /// Gets or sets the row selection mode.
    /// Default is None (no selection).
    /// </summary>
    [Parameter]
    public DataTableSelectionMode SelectionMode { get; set; } = DataTableSelectionMode.None;

    /// <summary>
    /// Gets or sets whether to show the toolbar with global search and column visibility.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show pagination controls.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool ShowPagination { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the table is in a loading state.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Gets or sets whether keyboard navigation is enabled for table rows.
    /// When true, rows can be navigated with arrow keys and selected with Enter/Space.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool EnableKeyboardNavigation { get; set; } = true;

    /// <summary>
    /// Gets or sets the available page size options.
    /// Default is [5, 10, 20, 50, 100].
    /// </summary>
    [Parameter]
    public int[] PageSizes { get; set; } = { 5, 10, 20, 50, 100 };

    /// <summary>
    /// Gets or sets the initial page size.
    /// Default is 5.
    /// </summary>
    [Parameter]
    public int InitialPageSize { get; set; } = 5;

    /// <summary>
    /// Gets or sets custom toolbar actions (buttons, etc.).
    /// </summary>
    [Parameter]
    public RenderFragment? ToolbarActions { get; set; }

    /// <summary>
    /// Gets or sets a custom template for the empty state.
    /// If null, displays default "No results found" message.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets a custom template for the loading state.
    /// If null, displays default "Loading..." message.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the container div.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// When true, the table's total width is kept in sync with the sum of all column widths during
    /// and after resize. Useful when the table should shrink below its container width rather than
    /// leaving empty space. Requires <see cref="TableContainerClass"/> <c>border-0</c> (or similar)
    /// for the best visual result. Default is <c>false</c>.
    /// </summary>
    [Parameter]
    public bool SyncWidthOnResize { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the inner table container (the div with the border and overflow).
    /// Use e.g. <c>border-0</c> to remove the border.
    /// </summary>
    [Parameter]
    public string? TableContainerClass { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the table.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the selected items.
    /// Use @bind-SelectedItems for two-way binding.
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<TData> SelectedItems { get; set; } = Array.Empty<TData>();

    /// <summary>
    /// Event callback invoked when the selected items change due to user interaction.
    /// Used for two-way binding with @bind-SelectedItems.
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<TData>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// Event callback invoked when the user changes the sort column or direction.
    /// Provides the column ID and sort direction for implementing custom sorting logic.
    /// </summary>
    [Parameter]
    public EventCallback<(string ColumnId, SortDirection Direction)> OnSort { get; set; }

    /// <summary>
    /// Event callback invoked when the user changes the global search filter value.
    /// Provides the search text for implementing custom filtering logic.
    /// </summary>
    [Parameter]
    public EventCallback<string?> OnFilter { get; set; }

    /// <summary>
    /// When <c>true</c>, drag-to-resize handles appear on column headers, letting users adjust
    /// column widths at runtime. Per-column <c>Resizable</c> on <see cref="DataTableColumn{TData,TValue}"/>
    /// overrides this table-level default.
    /// Activates <c>table-layout: fixed</c> automatically so widths are strictly honoured.
    /// Default is <c>false</c>.
    /// </summary>
    [Parameter]
    public bool Resizable { get; set; }

    /// <summary>
    /// Minimum column width in pixels enforced during drag-to-resize. Default is 80.
    /// </summary>
    [Parameter]
    public int MinColumnWidth { get; set; } = 80;

    /// <summary>
    /// Event callback invoked when the user finishes resizing a column.
    /// Provides the column ID and the new CSS width string (e.g. <c>"240px"</c>).
    /// </summary>
    [Parameter]
    public EventCallback<(string ColumnId, string Width)> OnColumnResize { get; set; }

    /// <summary>
    /// When <c>true</c>, column headers can be dragged to reorder columns at runtime.
    /// Per-column <c>Reorderable</c> on <see cref="DataTableColumn{TData,TValue}"/> overrides this default.
    /// Pinned columns and the selection checkbox column are always excluded from reordering.
    /// Default is <c>false</c>.
    /// </summary>
    [Parameter]
    public bool Reorderable { get; set; }

    /// <summary>
    /// Event callback invoked when the user drops a column into a new position.
    /// Provides the column ID and the new zero-based display index.
    /// </summary>
    [Parameter]
    public EventCallback<(string ColumnId, int NewIndex)> OnColumnReorder { get; set; }

    /// <summary>
    /// Template for the context menu shown when a data row is right-clicked.
    /// Receives a <see cref="DataTableRowMenuContext{TData}"/> with the row item, the current
    /// selection, and the IDs of all visible columns.
    /// Use <c>ContextMenuItem</c>, <c>ContextMenuSeparator</c>, etc. as child content.
    /// </summary>
    [Parameter]
    public RenderFragment<DataTableRowMenuContext<TData>>? RowContextMenu { get; set; }

    /// <summary>
    /// Gets or sets a custom function to preprocess data before filtering, sorting, and pagination.
    /// Use this to transform data, apply server-side operations, or fetch additional details.
    /// </summary>
    [Parameter]
    public Func<IEnumerable<TData>, Task<IEnumerable<TData>>>? PreprocessData { get; set; }

    /// <summary>
    /// Gets or sets whether to use dense (compact) row padding.
    /// Dense mode reduces cell padding for a more information-dense layout.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool Dense { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show a muted background on the header row.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool HeaderBackground { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show vertical borders between header cells.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool HeaderBorder { get; set; } = false;

    /// <summary>
    /// Gets or sets additional CSS classes applied to the &lt;thead&gt; element.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes applied to the header &lt;tr&gt; element.
    /// </summary>
    [Parameter]
    public string? HeaderRowClass { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes applied to each body &lt;tr&gt; element.
    /// </summary>
    [Parameter]
    public string? BodyRowClass { get; set; }

    /// <summary>
    /// When true, applies alternating row background colours (zebra striping).
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool Striped { get; set; }

    /// <summary>
    /// Tailwind class applied to alternating rows when <see cref="Striped"/> is true.
    /// Defaults to <c>even:bg-muted/50</c>. Override to change colour or swap odd/even,
    /// e.g. <c>odd:bg-muted/30</c> or <c>even:bg-blue-50 dark:even:bg-blue-950/20</c>.
    /// </summary>
    [Parameter]
    public string? StripeClass { get; set; }

    /// <summary>
    /// Gets or sets whether to show vertical borders between body cells.
    /// Default is false.
    /// </summary>
    [Parameter]
    public bool CellBorder { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to show the column visibility toggle button in the toolbar.
    /// Default is true.
    /// </summary>
    [Parameter]
    public bool ColumnsVisibility { get; set; } = true;

    /// <summary>
    /// Controls the column-sizing algorithm used by the table.
    /// <list type="bullet">
    ///   <item><see cref="TableColumnSizing.Auto"/> (default) — <c>table-layout: auto</c>;
    ///   column <c>Width</c> values are hints the browser may override.</item>
    ///   <item><see cref="TableColumnSizing.Fixed"/> — <c>table-layout: fixed</c>;
    ///   column widths are strictly honoured via a rendered <c>&lt;colgroup&gt;</c>.
    ///   Use this when you need guaranteed fixed widths without column pinning, or when
    ///   combining fixed widths with future column-resizing support.</item>
    /// </list>
    /// Tables with pinned columns always use fixed sizing regardless of this setting.
    /// </summary>
    [Parameter]
    public TableColumnSizing ColumnSizing { get; set; } = TableColumnSizing.Auto;

    /// <summary>Total number of visible columns including the selection checkbox column. Always at least 1 to avoid invalid colspan="0".</summary>
    private int VisibleColumnCount =>
        Math.Max(1, _columns.Count(c => c.Visible) +
        (SelectionMode == DataTableSelectionMode.Multiple ? 1 : 0));

    private string ContainerCssClass => ClassNames.cn(
        "w-full space-y-4",
        Class
    );

    /// <summary>
    /// Gets the computed CSS classes for the table container element.
    /// overflow-x-auto is always present so sticky columns work from the first render
    /// without waiting for a re-render to detect HasPinnedColumns.
    /// </summary>
    private string TableContainerCssClass => ClassNames.cn(
        "rounded-md border overflow-x-auto",
        TableContainerClass
    );

    /// <summary>
    /// Gets the inline style for the table container, combining height/scroll for virtualization.
    /// </summary>
    private string? TableContainerStyle => IsVirtualized ? $"height:{Height};overflow-y:auto" : null;

    /// <summary>True when at least one visible column is pinned left or right.</summary>
    private bool HasPinnedColumns => _columns.Any(c => c.Pinned != ColumnPinnedSide.None);

    /// <summary>True when at least one visible column has resize enabled.</summary>
    private bool HasResizableColumns => _columns.Any(c => c.Visible && c.ResizeEnabled);

    /// <summary>True when at least one visible column has reorder enabled.</summary>
    private bool HasReorderableColumns => _columns.Any(c => c.Visible && c.ReorderEnabled);
  
    /// <summary>True when at least one visible column is pinned to the left.</summary>
    private bool HasLeftPinnedColumns => _columns.Any(c => c.Pinned == ColumnPinnedSide.Left);

    /// <summary>
    /// Returns true when the table should use <c>table-layout: fixed</c>.
    /// Fixed layout is required whenever any column is pinned (sticky positioning needs
    /// stable widths), when <see cref="ColumnSizing"/> is explicitly set to
    /// <see cref="TableColumnSizing.Fixed"/>, or when column resizing is enabled (resize
    /// requires strict widths so columns don't snap back to auto-sized values mid-drag).
    /// </summary>
    private bool HasTableFixed =>
        ColumnSizing == TableColumnSizing.Fixed || HasPinnedColumns || Resizable;

    /// <summary>
    /// Gets the computed CSS classes for the table element.
    /// Adds <c>table-fixed</c> (table-layout: fixed) when pinned columns are present so that
    /// column widths defined via &lt;colgroup&gt; are strictly honoured and the table can
    /// overflow its container horizontally.
    /// </summary>
    private string TableCssClass => ClassNames.cn(
        "w-full caption-bottom text-sm",
        HasTableFixed ? "table-fixed" : null
    );

    /// <summary>
    /// Inline style applied to the &lt;table&gt; element when pinned columns are present.
    /// TanStack Table's sticky-pinning example documents this as required: sticky columns
    /// with borders or box-shadows only work correctly under <c>border-collapse: separate</c>
    /// because <c>border-collapse: collapse</c> shares borders between adjacent cells and the
    /// browser's resolution algorithm can suppress them, especially when a <c>backdrop-filter</c>
    /// or <c>z-index</c> stacking context is involved.
    /// <c>border-spacing: 0</c> keeps the visual layout identical to the collapsed model.
    /// </summary>
    private string? TableBorderStyle =>
        HasPinnedColumns ? "border-collapse: separate; border-spacing: 0" : null;

    /// <summary>
    /// Gets the inline width style for a single &lt;col&gt; element inside &lt;colgroup&gt;.
    /// Only emitted when the table is in fixed-layout mode.
    /// </summary>
    private string? GetColStyle(ColumnData column) =>
        !string.IsNullOrWhiteSpace(column.Width) ? $"width: {column.Width}" : null;

    /// <summary>
    /// Gets the padding classes for header cells based on the Dense setting.
    /// </summary>
    private string HeaderCellPaddingClass => Dense ? "h-9 px-4" : "h-12 px-4";

    /// <summary>
    /// Gets the padding classes for body cells based on the Dense setting.
    /// </summary>
    private string BodyCellPaddingClass => Dense ? "py-2 px-4" : "p-4";

    /// <summary>
    /// Gets the computed CSS classes for the header row, driven by HeaderBackground,
    /// HeaderBorder, and the optional HeaderRowClass override.
    /// </summary>
    private string ComputedHeaderRowClass => ClassNames.cn(
        "border-b transition-colors",
        HeaderBackground ? "bg-muted/50" : null,
        HeaderBorder ? "divide-x divide-border" : null,
        HeaderRowClass
    );

    /// <summary>
    /// Builds the CSS class string for a body row, merging selection state and
    /// the optional BodyRowClass override.
    /// </summary>
    private string GetBodyRowClass(bool isSelected) => ClassNames.cn(
        HasPinnedColumns
            ? "group/row bg-background transition-colors"
            : "group border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted",
        Striped ? (StripeClass ?? "even:bg-muted/50 even:hover:bg-muted/70") : null,
        isSelected ? "bg-muted" : null,
        CellBorder ? "divide-x divide-border" : null,
        BodyRowClass
    );

    /// <summary>
    /// Initializes the component and sets up default pagination and selection state.
    /// </summary>
    protected override void OnInitialized()
    {
        _tableState.Pagination.PageSize = InitialPageSize;
        _tableState.Pagination.CurrentPage = 1;
        // Set selection mode on the state so Select/Deselect methods work correctly
        _tableState.Selection.Mode = GetPrimitiveSelectionMode();
    }

    /// <summary>
    /// Synchronizes parameters with internal state and processes the data.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        // Sync externally-provided ExpandedValues into internal state.
        // Use SetEquals so that in-place mutations of the same HashSet instance are also detected.
        if (ExpandedValues is not null &&
            (!ReferenceEquals(ExpandedValues, _lastExpandedValues) ||
             !_expandedRowsInternal.SetEquals(ExpandedValues)))
        {
            _lastExpandedValues = ExpandedValues;
            _expandedRowsInternal = new HashSet<string>(ExpandedValues);
        }

        // Keep selection mode in sync with parameter
        _tableState.Selection.Mode = GetPrimitiveSelectionMode();

        // Sync SelectedItems parameter to internal state if changed externally
        // Skip if SelectedItems is the same reference as our internal collection (shouldn't happen with the copy we make, but defensive)
        if (!ReferenceEquals(SelectedItems, _lastSelectedItems) &&
            !ReferenceEquals(SelectedItems, _tableState.Selection.SelectedItems))
        {
            _tableState.Selection.Clear();
            foreach (var item in SelectedItems)
            {
                _tableState.Selection.Select(item);
            }
            _lastSelectedItems = SelectedItems;
            _selectionVersion++;
        }

        await ProcessDataAsync();
    }

    /// <summary>
    /// Registers a column with the data table during component initialization.
    /// Called internally by DataTableColumn components when they are initialized.
    /// </summary>
    /// <typeparam name="TValue">The type of value returned by the column's property accessor.</typeparam>
    /// <param name="column">The column component to register.</param>
    internal void RegisterColumn<TValue>(DataTableColumn<TData, TValue> column) where TValue : notnull
    {
        // Create internal column data structure (avoids BL0005 component parameter warnings)
        var columnData = new ColumnData
        {
            Id = column.Id ?? column.Header.ToLowerInvariant().Replace(" ", "-"),
            Header = column.Header,
            Property = item =>
            {
                var value = column.Property(item);
                return value ?? throw new InvalidOperationException($"Column '{column.Header}' returned null for a non-nullable type.");
            },
            Sortable = column.Sortable,
            Filterable = column.Filterable,
            Visible = column.Visible,
            Width = column.Width,
            MinWidth = column.MinWidth,
            MaxWidth = column.MaxWidth,
            CellTemplate = column.CellTemplate,
            CellClass = column.CellClass,
            HeaderClass = column.HeaderClass,
            Alignment = column.Alignment,
            Pinned = column.Pinned,
            ResizeEnabled = column.Resizable ?? Resizable,
            ReorderEnabled = (column.Reorderable ?? Reorderable) && column.Pinned == ColumnPinnedSide.None,
        };

        _columns.Add(columnData);
        _columnsVersion++;
        // Schedule a re-render so properties that depend on _columns (HasTableFixed, HasPinnedColumns,
        // column offsets, colgroup, etc.) are evaluated with the fully-populated column list.
        // Calling StateHasChanged here is safe — Blazor batches re-renders triggered during a
        // render cycle and applies them after the current batch completes.
        StateHasChanged();
    }

    /// <summary>
    /// Processes the data through the complete pipeline: preprocessing, filtering, sorting, and pagination.
    /// In server mode, delegates entirely to the <see cref="ServerData"/> callback.
    /// </summary>
    private async Task ProcessDataAsync()
    {
        // Virtualised server mode: the Virtualize component drives fetching via
        // the ItemsProvider delegate — nothing to compute here.
        if (IsVirtualizedServerMode) return;

        // Tree mode: build flat tree from root items (server or local).
        if (IsTreeMode)
        {
            if (IsServerMode)
            {
                var request = new DataTableRequest
                {
                    Page            = _tableState.Pagination.CurrentPage,
                    PageSize        = _tableState.Pagination.PageSize,
                    SortDescriptors = BuildSortDescriptors(),
                    SearchText      = string.IsNullOrWhiteSpace(_globalSearchValue) ? null : _globalSearchValue
                };
                var result = await ServerData!(request);
                _processedData = result.Items.ToList();
                _filteredData  = _processedData;
                _tableState.Pagination.TotalItems = result.TotalCount;
                _serverResultVersion++;
            }
            else
            {
                var treeData = Data ?? Array.Empty<TData>();
                if (PreprocessData != null) treeData = await PreprocessData(treeData);
                _filteredData  = ApplyTreeFiltering(treeData).ToList();
                var sorted     = ApplySorting(_filteredData);
                _tableState.Pagination.TotalItems = sorted.Count();
                _processedData = sorted.ToList();
            }
            BuildTreeRows();
            return;
        }

        // Client-side virtualise: filter + sort the full dataset but skip the
        // pagination slice; the Virtualize component handles windowing.
        // _processedData and _filteredData are kept in sync so selection state
        // (IsAllSelected, IsSomeSelected, "select all N items") works correctly.
        if (Virtualize && !IsTreeMode)
        {
            var allData = Data ?? Array.Empty<TData>();
            if (PreprocessData != null) allData = await PreprocessData(allData);
            var filtered = ApplyFiltering(allData);
            _filteredData = filtered;
            _virtualizeItems = ApplySorting(filtered).ToList();
            _processedData = _virtualizeItems;
            return;
        }

        if (IsServerMode)
        {
            var request = new DataTableRequest
            {
                Page             = _tableState.Pagination.CurrentPage,
                PageSize         = _tableState.Pagination.PageSize,
                SortDescriptors  = BuildSortDescriptors(),
                SearchText       = string.IsNullOrWhiteSpace(_globalSearchValue) ? null : _globalSearchValue
            };

            var result = await ServerData!(request);
            _processedData = result.Items.ToList();
            _filteredData  = _processedData;
            _tableState.Pagination.TotalItems = result.TotalCount;
            _serverResultVersion++;
            return;
        }

        var data = Data ?? Array.Empty<TData>();

        // 1. Preprocess (if custom function provided)
        if (PreprocessData != null)
        {
            data = await PreprocessData(data);
        }

        // 2. Apply filtering (column filters + global search)
        _filteredData = ApplyFiltering(data);

        // 3. Apply sorting
        var sortedData = ApplySorting(_filteredData);

        // 4. Update pagination total items BEFORE pagination
        _tableState.Pagination.TotalItems = sortedData.Count();

        // 5. Apply pagination
        _processedData = sortedData
            .Skip(_tableState.Pagination.StartIndex)
            .Take(_tableState.Pagination.PageSize)
            .ToList();
    }

    /// <summary>
    /// Builds the current sort descriptor list from the active single-column sort state.
    /// Returns an empty list when no sort is active. The list shape is ready to carry
    /// multiple descriptors once multi-column sorting is added.
    /// </summary>
    private IReadOnlyList<SortDescriptor> BuildSortDescriptors() =>
        _tableState.Sorting.Direction == SortDirection.None
            ? []
            : [new SortDescriptor(_tableState.Sorting.SortedColumn, _tableState.Sorting.Direction)];

    /// <summary>
    /// Builds a <see cref="DataTableVirtualRequest"/> from the current table state and
    /// the supplied virtualizer request. Called by the Virtualize ItemsProvider wrapper.
    /// </summary>
    private DataTableVirtualRequest BuildVirtualRequest(ItemsProviderRequest req) =>
        new(req.StartIndex, req.Count, BuildSortDescriptors(),
            string.IsNullOrWhiteSpace(_globalSearchValue) ? null : _globalSearchValue,
            req.CancellationToken);

    /// <summary>
    /// Adapts <see cref="ItemsProvider"/> into the <see cref="ItemsProviderDelegate{TData}"/>
    /// shape expected by Blazor's Virtualize component.
    /// </summary>
    private async ValueTask<ItemsProviderResult<TData>> VirtualizeProviderAdapter(
        ItemsProviderRequest req)
    {
        var result = await ItemsProvider!(BuildVirtualRequest(req));
        return result;
    }

    /// <summary>
    /// Applies global search filtering across all filterable columns.
    /// Returns the filtered data collection.
    /// </summary>
    /// <param name="data">The data to filter.</param>
    /// <returns>The filtered data matching the global search criteria.</returns>
    private IEnumerable<TData> ApplyFiltering(IEnumerable<TData> data)
    {
        if (string.IsNullOrWhiteSpace(_globalSearchValue))
        {
            return data;
        }

        // Cache search value to avoid repeated property access in closure
        var searchValue = _globalSearchValue;

        // Pre-filter to only filterable columns to reduce iterations
        var filterableColumns = _columns.Where(c => c.Filterable).ToList();
        if (filterableColumns.Count == 0)
        {
            filterableColumns = _columns; // Fall back to all columns if none marked filterable
        }

        return data.Where(item => MatchesSearch(item, searchValue, filterableColumns));
    }

    /// <summary>
    /// Checks if a data item matches the search value in any of the specified columns.
    /// Uses case-insensitive string comparison.
    /// </summary>
    /// <param name="item">The data item to check.</param>
    /// <param name="searchValue">The search text to match against.</param>
    /// <param name="columns">The columns to search within.</param>
    /// <returns>True if the item matches the search value in any column; otherwise, false.</returns>
    private static bool MatchesSearch(TData item, string searchValue, List<ColumnData> columns)
    {
        foreach (var column in columns)
        {
            try
            {
                var value = column.Property(item);
                if (value == null) continue;

                var stringValue = value.ToString();
                if (!string.IsNullOrEmpty(stringValue) &&
                    stringValue.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
                // Skip columns that cause errors during property access
            }
        }
        return false;
    }

    /// <summary>
    /// Applies sorting to the data based on the current sort column and direction.
    /// Returns the data unchanged if no sorting is active.
    /// </summary>
    /// <param name="data">The data to sort.</param>
    /// <returns>The sorted data collection.</returns>
    private IEnumerable<TData> ApplySorting(IEnumerable<TData> data)
    {
        if (_tableState.Sorting.Direction == SortDirection.None)
            return data;

        var column = _columns.FirstOrDefault(c => c.Id == _tableState.Sorting.SortedColumn);
        if (column == null)
            return data;

        var sorted = _tableState.Sorting.Direction == SortDirection.Ascending
            ? data.OrderBy(item => column.Property(item))
            : data.OrderByDescending(item => column.Property(item));

        return sorted;
    }

    /// <summary>
    /// Handles sort change events triggered by column header clicks.
    /// Invokes the OnSort callback and reprocesses the data.
    /// </summary>
    /// <param name="sortInfo">The column ID and sort direction.</param>
    private async Task HandleSortChange((string ColumnId, SortDirection Direction) sortInfo)
    {
        if (OnSort.HasDelegate)
            await OnSort.InvokeAsync(sortInfo);

        if (IsVirtualizedServerMode && _virtualizeRef is not null)
        {
            await _virtualizeRef.RefreshDataAsync();
            StateHasChanged();
            return;
        }

        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Handles changes to the global search input.
    /// Resets pagination to the first page and reprocesses the data.
    /// </summary>
    /// <param name="value">The new search value.</param>
    private async Task HandleGlobalSearchChanged(string value)
    {
        _globalSearchValue = value;

        if (OnFilter.HasDelegate)
            await OnFilter.InvokeAsync(_globalSearchValue);

        if (IsVirtualizedServerMode && _virtualizeRef is not null)
        {
            await _virtualizeRef.RefreshDataAsync();
            _serverResultVersion++;
            return;
        }

        // Reset to first page when filtering
        _tableState.Pagination.CurrentPage = 1;

        await ProcessDataAsync();
    }

    /// <summary>
    /// Handles column visibility toggle events from the column visibility menu.
    /// Updates the column's visibility state and triggers a re-render.
    /// </summary>
    /// <param name="columnId">The ID of the column to toggle.</param>
    /// <param name="visible">The new visibility state.</param>
    private void HandleColumnVisibilityChanged(string columnId, bool visible)
    {
        var column = _columns.FirstOrDefault(c => c.Id == columnId);
        if (column != null)
        {
            column.Visible = visible;

            // Increment version to signal change without list recreation
            _columnsVersion++;

            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles selection changes from the underlying table primitive.
    /// Invokes the SelectedItemsChanged callback with a defensive copy of the selection.
    /// </summary>
    /// <param name="selectedItems">The collection of currently selected items.</param>
    private async Task HandleSelectionChange(IReadOnlyCollection<TData> selectedItems)
    {
        if (SelectedItemsChanged.HasDelegate)
        {
            // Pass a copy to avoid reference aliasing - if parent stores the reference
            // and we later call Clear(), it would clear the parent's collection too
            await SelectedItemsChanged.InvokeAsync(selectedItems.ToList().AsReadOnly());
        }
    }

    /// <summary>
    /// Determines whether to show the select-all dropdown menu.
    /// Returns true when there are more items in the dataset than currently visible on the page.
    /// </summary>
    /// <returns>True if the select-all dropdown should be shown; otherwise, false.</returns>
    private bool ShouldShowSelectAllPrompt()
    {
        return _tableState.Pagination.TotalItems > _processedData.Count();
    }

    /// <summary>
    /// Gets the total count of items after filtering but before pagination.
    /// This represents all items matching the current filter criteria.
    /// </summary>
    /// <returns>The total filtered item count.</returns>
    private int GetTotalFilteredItemCount()
    {
        return _filteredData.Count();
    }

    /// <summary>
    /// Opens the select-all dropdown menu that allows choosing between selecting current page or all items.
    /// </summary>
    private void OpenSelectAllDropdown()
    {
        _selectAllDropdownOpen = true;
        StateHasChanged();
    }

    /// <summary>
    /// Handles the select-all checkbox state change.
    /// Opens the dropdown menu if multiple pages exist, otherwise selects all items on the current page.
    /// </summary>
    /// <param name="isChecked">True if the checkbox was checked; false if unchecked.</param>
    private async Task HandleSelectAllChanged(bool isChecked)
    {
        if (!isChecked)
        {
            await HandleClearSelection();
            return;
        }

        if (ShouldShowSelectAllPrompt())
        {
            _selectAllDropdownOpen = true;
            StateHasChanged();
            return;
        }

        await HandleSelectAllOnCurrentPage();
    }

    /// <summary>
    /// Selects all items visible on the current page and closes the select-all dropdown.
    /// </summary>
    private async Task HandleSelectAllOnCurrentPage()
    {
        foreach (var item in _processedData)
        {
            _tableState.Selection.Select(item);
        }
        _selectAllDropdownOpen = false;
        _selectionVersion++;
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    /// <summary>
    /// Selects all items in the entire filtered dataset across all pages and closes the select-all dropdown.
    /// In server mode this is not supported and falls back to selecting the current page only.
    /// </summary>
    private async Task HandleSelectAllItems()
    {
        if (IsServerMode)
        {
            // Cannot enumerate all server-side items; select current page instead.
            await HandleSelectAllOnCurrentPage();
            return;
        }

        foreach (var item in _filteredData)
        {
            _tableState.Selection.Select(item);
        }
        _selectAllDropdownOpen = false;
        _selectionVersion++;
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    /// <summary>
    /// Clears all selected items and closes the select-all dropdown.
    /// </summary>
    private async Task HandleClearSelection()
    {
        _tableState.Selection.Clear();
        _selectAllDropdownOpen = false;
        _selectionVersion++;
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    /// <summary>
    /// Handles individual row checkbox state changes.
    /// Selects or deselects the item based on the checkbox state.
    /// </summary>
    /// <param name="item">The data item associated with the row.</param>
    /// <param name="isChecked">True if the checkbox was checked; false if unchecked.</param>
    private async Task HandleRowSelectionChanged(TData item, bool isChecked)
    {
        if (isChecked)
        {
            _tableState.Selection.Select(item);
        }
        else
        {
            _tableState.Selection.Deselect(item);
        }

        _selectionVersion++;  // Track selection change for ShouldRender
        await HandleSelectionChange(_tableState.Selection.SelectedItems);
        StateHasChanged();
    }

    private void HandleRowContextMenu(TData item, MouseEventArgs e)
    {
        _contextMenuItem = item;
        // Open directly on the ContextMenu ref — bypasses @bind-Open timing issues
        // so every right-click reliably repositions and shows the menu.
        _rowContextMenuRef?.OpenAt(e.ClientX, e.ClientY);
    }

    /// <summary>
    /// Builds the <see cref="DataTableRowMenuContext{TData}"/> passed to the RowContextMenu render fragment.
    /// </summary>
    private DataTableRowMenuContext<TData> BuildRowMenuContext(TData item) =>
        new(
            item,
            _tableState.Selection.SelectedItems.ToList().AsReadOnly(),
            _columns.Where(c => c.Visible).Select(c => c.Id ?? c.Header ?? "").ToList().AsReadOnly()
        );

    /// <summary>
    /// Determines if all items on the current page are selected.
    /// Returns false if there are no items on the page.
    /// </summary>
    /// <returns>True if all current page items are selected; otherwise, false.</returns>
    private bool IsAllSelected()
    {
        if (!_processedData.Any())
            return false;

        return _processedData.All(item => _tableState.Selection.IsSelected(item));
    }

    /// <summary>
    /// Determines if some (but not all) items on the current page are selected.
    /// Used to set the indeterminate state of the select-all checkbox.
    /// </summary>
    /// <returns>True if some but not all items are selected; otherwise, false.</returns>
    private bool IsSomeSelected()
    {
        if (!_processedData.Any())
            return false;

        var selectedCount = _processedData.Count(item => _tableState.Selection.IsSelected(item));
        return selectedCount > 0 && selectedCount < _processedData.Count();
    }

    /// <summary>
    /// Builds the complete inline style for a column, covering width constraints and sticky pinning.
    /// </summary>
    private string? GetColumnStyle(ColumnData column)
    {
        var styles = new List<string>();

        if (!string.IsNullOrWhiteSpace(column.Width))
            styles.Add($"width: {column.Width}");

        if (!string.IsNullOrWhiteSpace(column.MinWidth))
            styles.Add($"min-width: {column.MinWidth}");

        if (!string.IsNullOrWhiteSpace(column.MaxWidth))
            styles.Add($"max-width: {column.MaxWidth}");

        if (column.Pinned != ColumnPinnedSide.None)
        {
            styles.Add("position: sticky");
            styles.Add(column.Pinned == ColumnPinnedSide.Left
                ? $"left: {GetPinnedOffset(column)}px"
                : $"right: {GetPinnedOffset(column)}px");

            // Separator between the pinned group and scrollable columns.
            // Must be an inline style — TailwindMerge treats border-r/border-l and
            // border-b as the same conflict group, so border-r added via a class gets
            // dropped when border-b appears later in the cn() call.
            if (IsLastPinnedLeft(column))
                styles.Add("border-right: 1px solid var(--border)");
            else if (IsFirstPinnedRight(column))
                styles.Add("border-left: 1px solid var(--border)");
        }

        return styles.Count > 0 ? string.Join("; ", styles) : null;
    }

    /// <summary>Returns the inline style for a column's header cell, extending
    /// <see cref="GetColumnStyle"/> with header-only properties such as the
    /// grab cursor for reorderable columns.</summary>
    private string? GetHeaderCellStyle(ColumnData column)
    {
        var base_ = GetColumnStyle(column);
        if (!column.ReorderEnabled) return base_;
        return base_ is null ? "cursor: grab" : base_ + "; cursor: grab";
    }

    /// <summary>
    /// Returns extra CSS classes for a pinned column cell.
    /// Background, hover, selected, and border-b are all handled by
    /// .group/row CSS rules in components-input.css — cells only need
    /// sticky z-index classes here. The data-pinned attribute triggers
    /// the CSS background-color: inherit rule for opaque sticky coverage.
    /// </summary>
    private string GetPinnedCellClass(ColumnData column, bool isHeader = false)
    {
        if (column.Pinned == ColumnPinnedSide.None) return string.Empty;

        return isHeader ? "z-20 bg-background/40 backdrop-blur-sm" : "z-[1] bg-background/60 backdrop-blur-sm";
    }

    /// <summary>
    /// Returns the inline style to pin the selection checkbox column when left-pinned data
    /// columns are present.  The column is always at <c>left: 0</c> — it is the leftmost
    /// sticky cell and has no predecessors to account for.
    /// </summary>
    private string? GetSelectionColumnStyle() =>
        HasLeftPinnedColumns ? "position: sticky; left: 0px" : null;

    /// <summary>
    /// Returns the sticky CSS classes for the selection checkbox column when left-pinned
    /// data columns are present.  Uses the same z-index / backdrop tiers as data columns.
    /// </summary>
    private string GetSelectionColumnClass(bool isHeader = false) =>
        HasLeftPinnedColumns
            ? (isHeader ? "z-20 bg-background/40 backdrop-blur-sm" : "z-[1] bg-background/60 backdrop-blur-sm")
            : string.Empty;

    /// <summary>
    /// Computes the sticky offset (px) for a pinned column based on the combined widths
    /// of all preceding pinned columns on the same side.
    /// When <see cref="SelectionMode"/> is <see cref="DataTableSelectionMode.Multiple"/> the
    /// sticky selection checkbox column precedes all data columns and its width is added to the
    /// offset of every left-pinned column so they don't overlap it.
    /// </summary>
    private int GetPinnedOffset(ColumnData column)
    {
        var offset = 0;
        if (column.Pinned == ColumnPinnedSide.Left)
        {
            // Reserve space for the sticky selection checkbox column when present.
            if (SelectionMode == DataTableSelectionMode.Multiple)
                offset += SelectionColumnWidthPx;

            foreach (var col in _columns.Where(c => c.Visible && c.Pinned == ColumnPinnedSide.Left))
            {
                if (ReferenceEquals(col, column)) break;
                offset += ParsePxWidth(col.Width);
            }
        }
        else
        {
            var rightPinned = _columns.Where(c => c.Visible && c.Pinned == ColumnPinnedSide.Right).ToList();
            var idx = rightPinned.FindIndex(c => ReferenceEquals(c, column));
            for (var i = idx + 1; i < rightPinned.Count; i++)
                offset += ParsePxWidth(rightPinned[i].Width);
        }
        return offset;
    }

    private bool IsLastPinnedLeft(ColumnData column)
    {
        var last = _columns.LastOrDefault(c => c.Visible && c.Pinned == ColumnPinnedSide.Left);
        return last is not null && ReferenceEquals(last, column);
    }

    private bool IsFirstPinnedRight(ColumnData column)
    {
        var first = _columns.FirstOrDefault(c => c.Visible && c.Pinned == ColumnPinnedSide.Right);
        return first is not null && ReferenceEquals(first, column);
    }

    /// <summary>
    /// Parses an integer pixel value from a CSS width string such as <c>"200px"</c>.
    /// </summary>
    /// <returns>
    /// The integer pixel value when the string ends with <c>px</c> and contains a valid integer;
    /// otherwise 0. Pinned columns <b>must</b> use a pixel width (e.g. <c>"200px"</c>) so that
    /// <see cref="GetPinnedOffset"/> can compute correct sticky <c>left</c>/<c>right</c> values.
    /// Other CSS length units (%, rem, em, …) are not supported and will result in an offset of 0,
    /// which can cause adjacent pinned columns to overlap.
    /// </returns>
    private static int ParsePxWidth(string? width)
    {
        if (!string.IsNullOrWhiteSpace(width) && width.TrimEnd().EndsWith("px") &&
            int.TryParse(width.TrimEnd()[..^2].Trim(), out var px))
            return px;
        return 0;
    }

    // ── Tree-mode helper methods ─────────────────────────────────────────────

    /// <summary>Returns a stable string key for the given item.</summary>
    private string GetItemKey(TData item) =>
        ValueField?.Invoke(item) ?? RuntimeHelpers.GetHashCode(item).ToString();

    /// <summary>Filters root items for tree mode, auto-expanding ancestors with matching descendants.</summary>
    private IEnumerable<TData> ApplyTreeFiltering(IEnumerable<TData> data)
    {
        if (string.IsNullOrWhiteSpace(_globalSearchValue))
            return data;

        // Lazy-load mode: only filter roots (cannot walk children without fetching).
        if (ChildrenProperty is null)
            return ApplyFiltering(data);

        var searchValue = _globalSearchValue;
        var filterableColumns = _columns.Where(c => c.Filterable).ToList();
        if (filterableColumns.Count == 0) filterableColumns = _columns;

        var result = data.Where(item =>
            MatchesSearch(item, searchValue, filterableColumns) ||
            HasMatchingDescendant(item, searchValue, filterableColumns))
            .ToList();

        foreach (var item in result)
            AutoExpandForSearch(item, searchValue, filterableColumns);

        return result;
    }

    private bool HasMatchingDescendant(TData item, string searchValue, List<ColumnData> filterableColumns)
    {
        if (ChildrenProperty is null) return false;
        var children = ChildrenProperty(item);
        if (children is null) return false;
        foreach (var child in children)
        {
            if (MatchesSearch(child, searchValue, filterableColumns)) return true;
            if (HasMatchingDescendant(child, searchValue, filterableColumns)) return true;
        }
        return false;
    }

    private void AutoExpandForSearch(TData item, string searchValue, List<ColumnData> filterableColumns)
    {
        if (ChildrenProperty is null) return;
        var children = ChildrenProperty(item);
        if (children is null) return;
        bool anyChildMatches = false;
        foreach (var child in children)
        {
            if (MatchesSearch(child, searchValue, filterableColumns) ||
                HasMatchingDescendant(child, searchValue, filterableColumns))
            {
                anyChildMatches = true;
                AutoExpandForSearch(child, searchValue, filterableColumns);
            }
        }
        if (anyChildMatches)
            _expandedRowsInternal.Add(GetItemKey(item));
    }

    /// <summary>Rebuilds <see cref="_treeRows"/> from <see cref="_processedData"/>.</summary>
    private void BuildTreeRows()
    {
        _treeRows = new List<TreeRow>();
        FlattenTree(_processedData, 0, _treeRows);
    }

    private void FlattenTree(IEnumerable<TData> items, int depth, List<TreeRow> result)
    {
        foreach (var item in items)
        {
            var key = GetItemKey(item);
            bool hasChildren;
            if (ChildrenProperty is not null)
            {
                var children = ChildrenProperty(item);
                hasChildren = HasChildrenField?.Invoke(item) ?? (children?.Any() == true);
            }
            else
            {
                hasChildren = HasChildrenField?.Invoke(item) ??
                    (_fetchedChildren.TryGetValue(key, out var fc) ? fc.Count > 0 : true);
            }

            var isExpanded = _expandedRowsInternal.Contains(key);
            var isLoading  = _loadingNodes.Contains(key);
            result.Add(new TreeRow(item, depth, hasChildren, isLoading, isExpanded));

            if (isExpanded && !isLoading)
            {
                if (ChildrenProperty is not null)
                {
                    var children = ChildrenProperty(item) ?? Enumerable.Empty<TData>();
                    FlattenTree(children, depth + 1, result);
                }
                else if (_fetchedChildren.TryGetValue(key, out var fetched))
                {
                    FlattenTree(fetched, depth + 1, result);
                }
            }
        }
    }

    /// <summary>Toggles the expanded state of the given tree node, fetching children lazily if needed.</summary>
    private async Task ToggleExpand(TData item)
    {
        var key = GetItemKey(item);
        if (_expandedRowsInternal.Contains(key))
        {
            _expandedRowsInternal.Remove(key);
        }
        else
        {
            if (LoadChildrenAsync is not null && !_fetchedChildren.ContainsKey(key))
            {
                _loadingNodes.Add(key);
                BuildTreeRows();
                _treeVersion++;
                StateHasChanged();
                try
                {
                    var children = await LoadChildrenAsync(item);
                    _fetchedChildren[key] = children.ToList();
                }
                catch
                {
                    _loadingNodes.Remove(key);
                    BuildTreeRows();
                    _treeVersion++;
                    StateHasChanged();
                    return;
                }
                _loadingNodes.Remove(key);

                // Don't expand if the load returned no children — the expander would
                // disappear (hasChildren becomes false) with no way to collapse.
                if (_fetchedChildren[key].Count == 0)
                {
                    BuildTreeRows();
                    _treeVersion++;
                    StateHasChanged();
                    return;
                }
            }
            _expandedRowsInternal.Add(key);
        }

        if (ExpandedValuesChanged.HasDelegate)
            await ExpandedValuesChanged.InvokeAsync(new HashSet<string>(_expandedRowsInternal));

        BuildTreeRows();
        _treeVersion++;
        StateHasChanged();
    }

    /// <summary>
    /// Converts the DataTable's selection mode to the underlying table primitive's selection mode enum.
    /// </summary>
    /// <returns>The corresponding primitive selection mode.</returns>
    private NeoUI.Blazor.Primitives.SelectionMode GetPrimitiveSelectionMode()
    {
        return SelectionMode switch
        {
            DataTableSelectionMode.None => NeoUI.Blazor.Primitives.SelectionMode.None,
            DataTableSelectionMode.Single => NeoUI.Blazor.Primitives.SelectionMode.Single,
            DataTableSelectionMode.Multiple => NeoUI.Blazor.Primitives.SelectionMode.Multiple,
            _ => NeoUI.Blazor.Primitives.SelectionMode.None
        };
    }

    /// <summary>
    /// Handles page navigation events from the pagination component.
    /// Reprocesses the data to display the new page.
    /// </summary>
    /// <param name="newPage">The new page number (1-based index).</param>
    private async Task HandlePageChanged(int newPage)
    {
        _paginationVersion++;  // Track pagination change for ShouldRender
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Handles page size selection events from the pagination component.
    /// Reprocesses the data with the new page size.
    /// </summary>
    /// <param name="newPageSize">The new page size.</param>
    private async Task HandlePageSizeChanged(int newPageSize)
    {
        _paginationVersion++;  // Track pagination change for ShouldRender
        await ProcessDataAsync();
        StateHasChanged();
    }

    /// <summary>
    /// Optimizes rendering by detecting which state has changed.
    /// Prevents unnecessary re-renders when no relevant state has been modified.
    /// </summary>
    /// <returns>True if the component should re-render; otherwise, false.</returns>
    protected override bool ShouldRender()
    {
        var dataChanged = !ReferenceEquals(_lastData, Data)
            || !ReferenceEquals(_lastServerData, ServerData)
            || !ReferenceEquals(_lastItemsProvider, ItemsProvider);
        var selectionModeChanged = _lastSelectionMode != SelectionMode;
        var loadingChanged = _lastIsLoading != IsLoading;
        var columnsChanged = _lastColumnsVersion != _columnsVersion;
        var searchChanged = _lastGlobalSearchValue != _globalSearchValue;
        var selectionChanged = _lastSelectionVersion != _selectionVersion;
        var paginationChanged = _lastPaginationVersion != _paginationVersion;
        var serverResultChanged = _lastServerResultVersion != _serverResultVersion;
        var styleChanged = _lastDense != Dense
            || _lastHeaderBackground != HeaderBackground
            || _lastHeaderBorder != HeaderBorder
            || _lastHeaderClass != HeaderClass
            || _lastHeaderRowClass != HeaderRowClass
            || _lastBodyRowClass != BodyRowClass
            || _lastStriped != Striped
            || _lastStripeClass != StripeClass
            || _lastCellBorder != CellBorder
            || _lastColumnsVisibility != ColumnsVisibility;
        var virtualizeChanged = _lastVirtualize != Virtualize
            || _lastHeight != Height
            || _lastItemHeight != ItemHeight
            || _lastVirtualizeOverscanCount != VirtualizeOverscanCount;
        var treeVersionChanged = _lastTreeVersion != _treeVersion;

        if (dataChanged || selectionModeChanged || loadingChanged || columnsChanged || searchChanged || selectionChanged || paginationChanged || serverResultChanged || styleChanged || virtualizeChanged || treeVersionChanged)
        {
            _lastData = Data;
            _lastServerData = ServerData;
            _lastItemsProvider = ItemsProvider;
            _lastSelectionMode = SelectionMode;
            _lastIsLoading = IsLoading;
            _lastColumnsVersion = _columnsVersion;
            _lastGlobalSearchValue = _globalSearchValue;
            _lastSelectionVersion = _selectionVersion;
            _lastPaginationVersion = _paginationVersion;
            _lastServerResultVersion = _serverResultVersion;
            _lastDense = Dense;
            _lastHeaderBackground = HeaderBackground;
            _lastHeaderBorder = HeaderBorder;
            _lastHeaderClass = HeaderClass;
            _lastHeaderRowClass = HeaderRowClass;
            _lastBodyRowClass = BodyRowClass;
            _lastStriped = Striped;
            _lastStripeClass = StripeClass;
            _lastCellBorder = CellBorder;
            _lastColumnsVisibility = ColumnsVisibility;
            _lastVirtualize = Virtualize;
            _lastHeight = Height;
            _lastItemHeight = ItemHeight;
            _lastVirtualizeOverscanCount = VirtualizeOverscanCount;
            _lastTreeVersion = _treeVersion;
            return true;
        }

        return false;
    }

    // ── JS Lifecycle ─────────────────────────────────────────────────────────

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            _dotNetRef ??= DotNetObjectReference.Create(this);
            _jsModule ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NeoUI.Blazor/js/datatable.js");

            if (HasResizableColumns && _resizeCleanup is null && !_resizeInitializing)
            {
                _resizeInitializing = true;
                _resizeCleanup = await _jsModule.InvokeAsync<IJSObjectReference>(
                    "initColumnResize", _tableContainerRef, _dotNetRef, MinColumnWidth, SyncWidthOnResize);
            }

            if (HasReorderableColumns && _reorderCleanup is null && !_reorderInitializing)
            {
                _reorderInitializing = true;
                var reorderableIds = _columns
                    .Where(c => c.Visible && c.ReorderEnabled)
                    .Select(c => c.Id)
                    .ToArray();
                _reorderCleanup = await _jsModule.InvokeAsync<IJSObjectReference>(
                    "initColumnReorder", _tableContainerRef, _dotNetRef, (object)reorderableIds);
            }
        }
        catch (JSDisconnectedException)
        {
            _resizeInitializing = false;
            _reorderInitializing = false;
        }
        catch (InvalidOperationException)
        {
            _resizeInitializing = false;
            _reorderInitializing = false;
        }
    }

    /// <summary>
    /// Invoked by JS when the user finishes dragging a resize handle.
    /// Updates the column's runtime width and fires <see cref="OnColumnResize"/>.
    /// </summary>
    [JSInvokable]
    public async Task OnResizeCompleted(string columnId, double widthPx)
    {
        var col = _columns.FirstOrDefault(c => c.Id == columnId);
        if (col is null) return;

        col.Width = $"{Math.Round(widthPx)}px";
        _columnsVersion++;
        StateHasChanged();

        if (OnColumnResize.HasDelegate)
            await OnColumnResize.InvokeAsync((columnId, col.Width));
    }

    /// <summary>
    /// Invoked by JS when the user drops a column header into a new position.
    /// Reorders <see cref="_columns"/> and fires <see cref="OnColumnReorder"/>.
    /// </summary>
    [JSInvokable]
    public async Task OnColumnReordered(string columnId, int newIndex)
    {
        var col = _columns.FirstOrDefault(c => c.Id == columnId);
        if (col is null) return;

        // JS clears drag transforms but does NOT commit DOM reorder — Blazor must
        // be the sole owner of DOM element ordering.  Update _columns and call
        // StateHasChanged() so Blazor diffs old-vdom → new-vdom against an actual
        // DOM that still matches old-vdom (original positions).  The diff produces
        // correct insertBefore ops that move elements to the reordered positions.
        _columns.Remove(col);
        var clamped = Math.Clamp(newIndex, 0, _columns.Count);
        _columns.Insert(clamped, col);
        _columnsVersion++;
        StateHasChanged();

        if (OnColumnReorder.HasDelegate)
            await OnColumnReorder.InvokeAsync((columnId, clamped));
    }

    public async ValueTask DisposeAsync()
    {
        if (_resizeCleanup is not null)
        {
            try { await _resizeCleanup.InvokeVoidAsync("dispose"); } catch { }
            await _resizeCleanup.DisposeAsync();
        }
        if (_reorderCleanup is not null)
        {
            try { await _reorderCleanup.InvokeVoidAsync("dispose"); } catch { }
            await _reorderCleanup.DisposeAsync();
        }
        if (_jsModule is not null)
            await _jsModule.DisposeAsync();
        _dotNetRef?.Dispose();
    }
}
