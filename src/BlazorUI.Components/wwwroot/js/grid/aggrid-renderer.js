/**
 * AG Grid Renderer for BlazorUI
 * Uses AG Grid ES modules with Theming API (no manual CSS loading needed)
 */


import { createGrid as agCreateGrid } from 'https://cdn.jsdelivr.net/npm/ag-grid-community@32.3.3/dist/package/main.esm.mjs';
import { themeAlpine, themeBalham, themeQuartz } from 'https://cdn.jsdelivr.net/npm/ag-grid-community@32.3.3/dist/package/main.esm.mjs';

import { createShadcnTheme } from './theme-shadcn.js';

/**
 * Gets the base AG Grid theme object
 */
function getBaseTheme(themeName) {
    switch (themeName) {
        case 'Shadcn':
        case 'Quartz':
            return themeQuartz;
        case 'Alpine':
            return themeAlpine;
        case 'Balham':
            return themeBalham;
        default:
            return themeQuartz;
    }
}

class BlazorTemplateCellRenderer {
    init(params) {
        this.eGui = document.createElement('div');
        this.eGui.className = 'blazor-cell-template';
        this.params = params;
        
        // Create a unique ID for this cell
        this.cellId = `cell-${params.column.getColId()}-${params.node.id || params.rowIndex}`;
        this.eGui.id = this.cellId;
        
        // Request template rendering from Blazor
        const templateId = params.colDef.cellRendererParams?.templateId;
        if (templateId && params.dotNetRef) {
            // Get row data - params.data should always be available for all columns
            const rowData = params.data || params.node?.data;
            console.log('[AG Grid] Cell renderer init for template:', templateId);
            console.log('[AG Grid] Has params.data:', !!params.data);
            console.log('[AG Grid] Has params.node.data:', !!params.node?.data);
            console.log('[AG Grid] Row data:', rowData);
            
            if (rowData) {
                this.renderTemplate(templateId, rowData);
            } else {
                console.error('[AG Grid] No row data available for template:', templateId);
                this.eGui.textContent = '[No data]';
            }
        }
    }
    
    async renderTemplate(templateId, data) {
        try {
            console.log('[AG Grid] Rendering template:', templateId, 'with data:', data);
            
            // Request HTML from Blazor
            const html = await this.params.dotNetRef.invokeMethodAsync(
                'RenderCellTemplate', 
                templateId, 
                data
            );
            
            if (html) {
                console.log('[AG Grid] Template rendered successfully, HTML length:', html.length);
                this.eGui.innerHTML = html;
                
                // Add event delegation for data-action attributes
                this.setupActionHandlers(data);
            } else {
                console.warn('[AG Grid] Template renderer returned empty HTML for:', templateId);
                // Fallback to field value
                const field = this.params.colDef.field;
                this.eGui.textContent = field ? this.params.data[field] : '';
            }
        } catch (error) {
            console.error('[AG Grid] Template rendering failed:', error);
            console.error('[AG Grid] Error details:', error.message, error.stack);
            // Fallback to field value
            const field = this.params.colDef.field;
            this.eGui.textContent = field ? this.params.data[field] : '[Error]';
        }
    }
    
    setupActionHandlers(rowData) {
        // Remove existing listeners to prevent duplicates
        if (this.actionClickHandler) {
            this.eGui.removeEventListener('click', this.actionClickHandler);
        }
        
        // Create new click handler
        this.actionClickHandler = async (e) => {
            // Find closest element with data-action attribute
            const actionElement = e.target.closest('[data-action]');
            if (!actionElement) return;
            
            // Prevent default behavior and stop propagation
            e.preventDefault();
            e.stopPropagation();
            
            const action = actionElement.dataset.action;
            console.log('[AG Grid] Cell action triggered:', action);
            
            try {
                await this.params.dotNetRef.invokeMethodAsync(
                    'HandleCellAction',
                    action,
                    rowData
                );
            } catch (error) {
                console.error('[AG Grid] Failed to invoke cell action:', error);
            }
        };
        
        // Add the click listener
        this.eGui.addEventListener('click', this.actionClickHandler);
    }
    
    getGui() {
        return this.eGui;
    }
    
    refresh(params) {
        // Update on data change
        const templateId = params.colDef.cellRendererParams?.templateId;
        if (templateId && params.dotNetRef) {
            this.params = params;
            const rowData = params.data || params.node?.data;
            if (rowData) {
                this.renderTemplate(templateId, rowData);
                return true;
            }
        }
        return false;
    }
    
    destroy() {
        // Cleanup event listeners
        if (this.actionClickHandler) {
            this.eGui.removeEventListener('click', this.actionClickHandler);
            this.actionClickHandler = null;
        }
    }
}

/**
 * Custom header renderer for Blazor templates
 */
class BlazorTemplateHeaderRenderer {
    init(params) {
        this.eGui = document.createElement('div');
        this.eGui.className = 'blazor-header-template';
        this.params = params;
        
        // Request template rendering from Blazor
        const templateId = params.templateId;
        if (templateId && params.dotNetRef) {
            this.renderTemplate(templateId);
        }
    }
    
    async renderTemplate(templateId) {
        try {
            console.log('[AG Grid] Rendering header template:', templateId);
            
            // Request HTML from Blazor
            const html = await this.params.dotNetRef.invokeMethodAsync(
                'RenderHeaderTemplate', 
                templateId
            );
            
            if (html) {
                console.log('[AG Grid] Header template rendered successfully');
                this.eGui.innerHTML = html;
            } else {
                console.warn('[AG Grid] Header template renderer returned empty HTML for:', templateId);
                // Fallback to headerName
                this.eGui.textContent = this.params.displayName || '';
            }
        } catch (error) {
            console.error('[AG Grid] Header template rendering failed:', error);
            // Fallback to headerName
            this.eGui.textContent = this.params.displayName || '[Error]';
        }
    }
    
    getGui() {
        return this.eGui;
    }
    
    refresh(params) {
        return false; // Header doesn't need refresh
    }
    
    destroy() {
        // Cleanup if needed
    }
}

/**
 * Creates and initializes an AG Grid instance
 * @param {HTMLElement|Object} elementOrRef - DOM element or Blazor ElementReference
 * @param {Object} config - Grid configuration
 * @param {Object} dotNetRef - .NET object reference for callbacks
 */
export async function createGrid(elementOrRef, config, dotNetRef) {
    try {
        console.log('[AG Grid] createGrid called');
        console.log('[AG Grid] elementOrRef type:', typeof elementOrRef);
        console.log('[AG Grid] elementOrRef:', elementOrRef);
        console.log('[AG Grid] elementOrRef constructor:', elementOrRef?.constructor?.name);
        
        // Handle Blazor ElementReference
        // Blazor passes ElementReference as a plain object with __internalId
        let element = elementOrRef;
        
        // If it's a Blazor ElementReference, we need to find the element by ID
        // But Blazor SHOULD pass the actual element when using ElementReference in JS interop
        // If we're getting __internalId, something is wrong with how we're passing it
        
        if (!element || typeof element !== 'object') {
            throw new Error('[AG Grid] Invalid element reference - not an object');
        }
        
        // Check if it's already a DOM element
        if (element instanceof HTMLElement) {
            console.log('[AG Grid] Received HTMLElement directly');
            console.log('[AG Grid] Element tag:', element.tagName);
            console.log('[AG Grid] Element id:', element.id);
            console.log('[AG Grid] Element classes:', element.className);
        } else if (element.__internalId !== undefined) {
            // This is a Blazor ElementReference that wasn't properly resolved
            console.error('[AG Grid] Received Blazor ElementReference with __internalId:', element.__internalId);
            console.error('[AG Grid] This means the DOM element is not ready or the ElementReference is invalid');
            throw new Error('[AG Grid] ElementReference not resolved to DOM element. The element may not be rendered yet.');
        } else {
            console.error('[AG Grid] Unknown element type');
            console.error('[AG Grid] Element keys:', Object.keys(element));
            throw new Error('[AG Grid] Invalid element type - not a DOM element and not a known ElementReference');
        }
        
        if (!(element instanceof Node)) {
            throw new Error(`[AG Grid] Element is not a valid DOM Node. Type: ${element?.constructor?.name}`);
        }
        
        console.log('[AG Grid] DOM element validated successfully');
        
        // Create theme with parameters
        let theme;
        let themeParams = config.themeParams || {};
        
        // For Shadcn theme, merge with dynamically read shadcn design tokens
        if (config.theme === 'Shadcn') {
            const shadcnDefaults = createShadcnTheme();
            themeParams = { ...shadcnDefaults, ...themeParams };
            theme = themeQuartz.withParams(themeParams);
        } else {
            const baseTheme = getBaseTheme(config.theme);
            if (baseTheme && Object.keys(themeParams).length > 0) {
                theme = baseTheme.withParams(themeParams);
            } else {
                theme = baseTheme;
            }
        }
        
        // Build grid options with event handlers that will receive the API
        const gridOptions = buildGridOptionsWithEvents(config, dotNetRef);
        gridOptions.theme = theme;
        
        // Create AG Grid instance - returns the API directly
        const gridApi = agCreateGrid(element, gridOptions);
        console.log('[AG Grid] Grid instance created successfully');
        console.log('[AG Grid] Grid API:', gridApi);
        
        // Return wrapper object with API methods
        return {
            setRowData: (data) => {
                console.log('[AG Grid] setRowData called');
                console.log('[AG Grid] Data type:', typeof data);
                console.log('[AG Grid] Data is array:', Array.isArray(data));
                console.log('[AG Grid] Data length:', data?.length || 0);
                if (data && data.length > 0) {
                    console.log('[AG Grid] First row:', data[0]);
                    console.log('[AG Grid] First row keys:', Object.keys(data[0]));
                }
                
                gridApi.setGridOption('rowData', data);
                console.log('[AG Grid] rowData set successfully');
            },
            
            applyTransaction: (transaction) => {
                console.log('[AG Grid] applyTransaction called');
                console.log('[AG Grid] Transaction:', {
                    add: transaction.add?.length || 0,
                    remove: transaction.remove?.length || 0,
                    update: transaction.update?.length || 0
                });
                
                // AG Grid's applyTransaction expects { add, remove, update }
                const result = gridApi.applyTransaction(transaction);
                
                console.log('[AG Grid] Transaction result:', {
                    added: result.add?.length || 0,
                    removed: result.remove?.length || 0,
                    updated: result.update?.length || 0
                });
            },
            
            applyState: (state) => {
                applyGridState(gridApi, state, gridOptions);
            },
            
            getState: () => {
                return getGridState(gridApi);
            },
            
            setGridOptions: (options) => {
                console.log('[AG Grid] setGridOptions called');
                console.log('[AG Grid] Options:', options);
                
                // Handle theme updates specially
                if (options.theme || options.themeParams) {
                    const themeName = options.theme || config.theme;
                    let themeParams = options.themeParams || {};
                    
                    console.log('[AG Grid] Updating theme:', themeName);
                    console.log('[AG Grid] Theme params:', themeParams);
                    
                    // For Shadcn theme, merge with dynamically read shadcn design tokens
                    if (themeName === 'Shadcn') {
                        const shadcnDefaults = createShadcnTheme();
                        themeParams = { ...shadcnDefaults, ...themeParams };
                    }
                    
                    // Create new theme with parameters
                    const baseTheme = getBaseTheme(themeName);
                    const newTheme = baseTheme.withParams(themeParams);
                    
                    // Apply the new theme using setGridOption
                    gridApi.setGridOption('theme', newTheme);
                    console.log('[AG Grid] Theme updated successfully');
                } else {
                    // For other options, apply them directly
                    Object.entries(options).forEach(([key, value]) => {
                        gridApi.setGridOption(key, value);
                    });
                }
            },
            
            destroy: () => {
                gridApi.destroy();
            }
        };
    } catch (error) {
        console.error('[AG Grid] Failed to create grid:', error);
        console.error('[AG Grid] Error message:', error.message);
        console.error('[AG Grid] Error stack:', error.stack);
        throw error;
    }
}

/**
 * Builds AG Grid options with event handlers
 * Event handlers receive the API as a parameter from AG Grid events
 */
function buildGridOptionsWithEvents(config, dotNetRef) {
    console.log('[AG Grid] buildGridOptionsWithEvents called');
    console.log('[AG Grid] config:', config);
    
    // Flag to prevent selection event during programmatic sync
    let isSyncingSelection = false;
    
    // Register custom cell renderer and value formatter
    const components = {
        templateRenderer: BlazorTemplateCellRenderer,
        headerTemplateRenderer: BlazorTemplateHeaderRenderer
    };
    
    // Add value formatter function for DataFormatString support
    const valueFormatters = {
        // Simple formatter that uses pre-formatted values from C#
        // Looks for {field}_formatted property, falls back to raw value
        formattedValueFormatter: (params) => {
            const field = params.colDef.field;
            if (!field) return params.value;
            
            const formattedField = `${field}_formatted`;
            
            // If formatted value exists, use it; otherwise use raw value
            return params.data[formattedField] ?? params.value;
        }
    };
    
    // Enhance column definitions with dotNetRef for templates
    const enhancedColumnDefs = config.columnDefs.map(col => {
        console.log('[AG Grid] Column def:', col);
        const enhanced = {
            ...col,
            cellRendererParams: col.cellRenderer === 'templateRenderer' 
                ? { ...col.cellRendererParams, dotNetRef }
                : col.cellRendererParams
        };
        
        // Also enhance header component params with dotNetRef
        if (col.headerComponent === 'headerTemplateRenderer') {
            enhanced.headerComponentParams = {
                ...col.headerComponentParams,
                dotNetRef
            };
        }
        
        // Add value formatter function if specified
        if (col.valueFormatter === 'formattedValueFormatter') {
            enhanced.valueFormatter = valueFormatters.formattedValueFormatter;
        }
        
        return enhanced;
    });
    
    // ✅ AG Grid v32.2+ Migration: rowSelection as object instead of string
    // Convert legacy string rowSelection to new object format
    // "single" → { mode: 'singleRow' }
    // "multiple" → { mode: 'multiRow' }
    let rowSelectionConfig = config.rowSelection;
    if (typeof config.rowSelection === 'string') {
        // Map legacy string values to new object format
        rowSelectionConfig = {
            mode: config.rowSelection === 'multiple' ? 'multiRow' : 'singleRow',
            enableClickSelection: true  // ✅ Replaces deprecated suppressRowClickSelection: false
        };
    }
    
    // ✅ AG Grid v32.2+: Provide getRowId for stable row identification
    // This is CRITICAL for row selection persistence across data updates
    // Developer specifies the ID field via config.idField (e.g., "Id", "ProductId", "OrderId")
    const idField = config.idField || 'Id'; // Default to 'Id' if not specified
    
    const getRowIdFunc = (params) => {
        if (!params.data) {
            // Fallback to AG Grid's internal node ID if data is not available
            return params.node?.id || `row-${params.node?.rowIndex ?? 0}`;
        }
        
        // Try to get the ID from the specified field
        const idValue = params.data[idField];
        
        if (idValue !== undefined && idValue !== null) {
            // Convert to string to ensure consistent ID type
            return String(idValue);
        }
        
        // Fallback chain if specified field doesn't exist
        // Try common conventions: Id, id, _id
        const fallbackId = params.data.Id 
            || params.data.id 
            || params.data._id 
            || params.node?.id;
            
        if (fallbackId !== undefined && fallbackId !== null) {
            console.warn(`[AG Grid] IdField '${idField}' not found on row data, using fallback: ${fallbackId}`);
            return String(fallbackId);
        }
        
        // Last resort: use row index (not recommended for production)
        console.error(`[AG Grid] No ID field found on row data. Specify config.idField or ensure data has 'Id' property.`);
        return `row-${params.node?.rowIndex ?? 0}`;
    };
    
    const gridOptions = {
        columnDefs: enhancedColumnDefs,
        rowData: [],
        components: components,
        
        // ✅ AG Grid v32.2+: Use object-based rowSelection
        rowSelection: rowSelectionConfig,
        
        // ✅ AG Grid v32.2+: Stable row IDs for selection persistence
        getRowId: getRowIdFunc,
        
        // Pagination
        pagination: config.pagination,
        paginationPageSize: config.paginationPageSize,
        paginationPageSizeSelector: config.pagination ? [10, 25, 50, 100] : false,
        
        // Row model
        rowModelType: config.rowModelType,
        
        // Default column settings
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
            // ✅ Suppress header menus if configured (for controlled filtering)
            suppressHeaderMenuButton: config.suppressHeaderMenus || false,
        },
        
        // ✅ AG Grid v32.2+: Removed deprecated enableRangeSelection
        // Cell/range selection is now controlled via cellSelection property
        // Only add cellSelection if explicitly needed (we don't need it for basic functionality)
        
        // ✅ AG Grid v32.2+: Removed deprecated suppressRowClickSelection
        // Now controlled via rowSelection.enableClickSelection (set above)
        
        // Event handlers - these receive events with the API attached
        onSortChanged: (event) => notifyStateChanged(event.api, dotNetRef),
        onFilterChanged: (event) => notifyStateChanged(event.api, dotNetRef),
        onPaginationChanged: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnMoved: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnResized: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnPinned: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnVisible: (event) => notifyStateChanged(event.api, dotNetRef),
        onSelectionChanged: (event) => {
            // Skip event if we're currently syncing selection from parent
            if (isSyncingSelection) {
                console.log('[AG Grid] Skipping onSelectionChanged - currently syncing from parent');
                return;
            }
            
            const selectedRows = event.api.getSelectedRows();
            dotNetRef.invokeMethodAsync('OnSelectionChanged', selectedRows)
                .catch(err => console.error('[AG Grid] Selection callback failed:', err));
        },
        
        // Store the sync flag in grid options so applyGridState can access it
        __isSyncingSelection: () => isSyncingSelection,
        __setIsSyncingSelection: (value) => { isSyncingSelection = value; }
    };
    
    console.log('[AG Grid] Grid options built:', gridOptions);
    console.log('[AG Grid] ID field configured:', idField);
    
    // Server-side datasource
    if (config.rowModelType === 'serverSide') {
        gridOptions.serverSideDatasource = createServerSideDatasource(dotNetRef);
    }
    
    // Infinite scroll datasource
    if (config.rowModelType === 'infinite') {
        gridOptions.datasource = createInfiniteDatasource(dotNetRef);
    }
    
    return gridOptions;
}

/**
 * Creates a server-side datasource for AG Grid
 */
function createServerSideDatasource(dotNetRef) {
    return {
        getRows: async (params) => {
            const request = mapServerSideRequest(params.request);
            
            try {
                const response = await dotNetRef.invokeMethodAsync('OnDataRequested', request);
                params.success({
                    rowData: response.items,
                    rowCount: response.totalCount
                });
            } catch (error) {
                console.error('[AG Grid] Server-side data request failed:', error);
                params.fail();
            }
        }
    };
}

/**
 * Creates an infinite scroll datasource for AG Grid
 */
function createInfiniteDatasource(dotNetRef) {
    return {
        getRows: async (params) => {
            const request = {
                startIndex: params.startRow,
                count: params.endRow - params.startRow,
                sortDescriptors: mapSortModel(params.sortModel),
                filterDescriptors: mapFilterModel(params.filterModel),
                pageNumber: Math.floor(params.startRow / (params.endRow - params.startRow)) + 1,
                pageSize: params.endRow - params.startRow,
                customParameters: {}
            };
            
            try {
                const response = await dotNetRef.invokeMethodAsync('OnDataRequested', request);
                params.successCallback(response.items, response.totalCount);
            } catch (error) {
                console.error('[AG Grid] Infinite scroll data request failed:', error);
                params.failCallback();
            }
        }
    };
}

/**
 * Maps AG Grid server-side request to GridDataRequest
 */
function mapServerSideRequest(agRequest) {
    return {
        startIndex: agRequest.startRow || 0,
        count: (agRequest.endRow || 0) - (agRequest.startRow || 0),
        sortDescriptors: mapSortModel(agRequest.sortModel || []),
        filterDescriptors: mapFilterModel(agRequest.filterModel || {}),
        pageNumber: Math.floor((agRequest.startRow || 0) / ((agRequest.endRow || 0) - (agRequest.startRow || 0))) + 1,
        pageSize: (agRequest.endRow || 0) - (agRequest.startRow || 0),
        customParameters: {}
    };
}

/**
 * Maps AG Grid sort model to GridSortDescriptor[]
 */
function mapSortModel(sortModel) {
    if (!sortModel || sortModel.length === 0) return [];
    
    return sortModel.map((sort, index) => ({
        field: sort.colId,
        direction: sort.sort === 'asc' ? 1 : 2, // GridSortDirection: None=0, Ascending=1, Descending=2
        order: index
    }));
}

/**
 * Maps AG Grid filter model to GridFilterDescriptor[]
 */
function mapFilterModel(filterModel) {
    if (!filterModel) return [];
    
    const filters = [];
    for (const [field, filter] of Object.entries(filterModel)) {
        filters.push({
            field: field,
            operator: mapFilterOperator(filter.type),
            value: filter.filter,
            caseSensitive: false
        });
    }
    
    return filters;
}

/**
 * Maps AG Grid filter type to GridFilterOperator enum
 */
function mapFilterOperator(agType) {
    const mapping = {
        'equals': 0,            // Equals
        'notEqual': 1,          // NotEquals
        'contains': 2,          // Contains
        'notContains': 3,       // NotContains
        'startsWith': 4,        // StartsWith
        'endsWith': 5,          // EndsWith
        'lessThan': 6,          // LessThan
        'lessThanOrEqual': 7,   // LessThanOrEqual
        'greaterThan': 8,       // GreaterThan
        'greaterThanOrEqual': 9,// GreaterThanOrEqual
        'blank': 10,            // IsEmpty
        'notBlank': 11          // IsNotEmpty
    };
    
    return mapping[agType] || 0; // Default to Equals
}

/**
 * Notifies .NET of grid state changes
 */
function notifyStateChanged(api, dotNetRef) {
    if (!api) return;
    
    try {
        const state = getGridState(api);
        dotNetRef.invokeMethodAsync('OnGridStateChanged', state)
            .catch(err => console.error('[AG Grid] State change callback failed:', err));
    } catch (error) {
        console.error('[AG Grid] Failed to get grid state:', error);
    }
}

/**
 * Gets current grid state
 */
function getGridState(api) {
    const columnState = api.getColumnState();
    
    const sortModel = columnState
        .filter(col => col.sort)
        .map((col, index) => ({
            field: col.colId,
            direction: col.sort === 'asc' ? 1 : 2, // Ascending=1, Descending=2
            order: col.sortIndex || index
        }));
    
    const filterModel = api.getFilterModel();
    const filterDescriptors = mapFilterModel(filterModel);
    
    const paginationPageSize = api.paginationGetPageSize();
    const paginationCurrentPage = api.paginationGetCurrentPage();
    
    const selectedRows = api.getSelectedRows();
    
    // ✅ AG Grid v32.2+: Get selected row IDs using getRowId
    // AG Grid's getSelectedRows() returns the data objects with their IDs already resolved
    // We just need to extract them using the same logic as getRowId
    const selectedRowIds = selectedRows.map(row => {
        // Try to extract the ID that was used by getRowId
        // The grid should have already validated these IDs exist
        return String(row.Id || row.id || row._id || 'unknown');
    });
    
    return {
        sortDescriptors: sortModel,
        filterDescriptors: filterDescriptors,
        pageNumber: paginationCurrentPage + 1,
        pageSize: paginationPageSize,
        columnStates: columnState.map(col => ({
            field: col.colId,
            visible: !col.hide,
            width: col.width ? `${col.width}px` : null,
            pinned: col.pinned === 'left' ? 1 : col.pinned === 'right' ? 2 : 0, // None=0, Left=1, Right=2
            order: col.sortIndex || 0
        })),
        selectedRowIds: selectedRowIds
    };
}

/**
 * Applies state to the grid
 */
function applyGridState(api, state, gridOptions) {
    if (!api || !state) return;
    
    try {
        // Apply column states (visibility, width, pinning, order)
        if (state.columnStates && state.columnStates.length > 0) {
            const columnState = state.columnStates.map(cs => ({
                colId: cs.field,
                hide: !cs.visible,
                width: cs.width ? parseInt(cs.width) : undefined,
                pinned: cs.pinned === 1 ? 'left' : cs.pinned === 2 ? 'right' : null
            }));
            api.applyColumnState({ state: columnState });
        }
        
        // Apply sorting - ALWAYS apply if sortDescriptors exists (even if empty = clear)
        if (state.sortDescriptors !== undefined && state.sortDescriptors !== null) {
            if (state.sortDescriptors.length > 0) {
                const sortModel = state.sortDescriptors.map(sort => ({
                    colId: sort.field,
                    sort: sort.direction === 1 ? 'asc' : 'desc',
                    sortIndex: sort.order
                }));
                
                // ✅ Use applyColumnState with defaultState to clear sorts on other columns
                api.applyColumnState({ 
                    state: sortModel,
                    defaultState: { sort: null },  // Clear sorts on columns not in sortModel
                    applyOrder: true                // Apply sort order
                });
            } else {
                // Clear all sorting - empty array means remove all sorts
                api.applyColumnState({ defaultState: { sort: null } });
            }
        }
        
        // Apply filtering - ALWAYS apply if filterDescriptors exists (even if empty = clear)
        if (state.filterDescriptors !== undefined && state.filterDescriptors !== null) {
            if (state.filterDescriptors.length > 0) {
                const filterModel = {};
                state.filterDescriptors.forEach(filter => {
                    // ✅ No need to detect type - AG Grid already knows from column definition
                    // The column def specifies the filter type (agNumberColumnFilter, agTextColumnFilter, etc.)
                    filterModel[filter.field] = {
                        type: reverseMapFilterOperator(filter.operator),
                        filter: filter.value
                    };
                });
                console.log('[AG Grid] Applying filter model:', filterModel);
                api.setFilterModel(filterModel);
            } else {
                // Clear all filters - empty array means remove all filters
                console.log('[AG Grid] Clearing all filters');
                api.setFilterModel(null);
            }
        }
        
        // Apply pagination
        if (state.pageNumber) {
            api.paginationGoToPage(state.pageNumber - 1);
        }
        
        // ✅ AG Grid v32.2+: Apply row selection using row IDs
        // CRITICAL: Set sync flag to prevent onSelectionChanged from firing during programmatic sync
        if (state.selectedRowIds !== undefined && state.selectedRowIds !== null) {
            const setIsSyncingSelection = gridOptions?.__setIsSyncingSelection;
            
            // Set flag before applying selection
            if (setIsSyncingSelection) {
                setIsSyncingSelection(true);
                console.log('[AG Grid] Setting isSyncingSelection = true');
            }
            
            try {
                // Clear current selection first
                api.deselectAll();
                
                if (state.selectedRowIds.length > 0) {
                    // Convert to Set for O(1) lookup performance
                    const selectedRowIds = new Set(state.selectedRowIds.map(id => String(id)));
                    
                    console.log('[AG Grid] Selecting rows with IDs:', Array.from(selectedRowIds));
                    
                    // Iterate through all rows and select matching ones
                    // AG Grid's getRowId will be used internally to match row IDs
                    let selectedCount = 0;
                    api.forEachNode((node) => {
                        if (node.data && node.id && selectedRowIds.has(String(node.id))) {
                            node.setSelected(true);
                            selectedCount++;
                        }
                    });
                    
                    console.log('[AG Grid] Selected', selectedCount, 'rows');
                }
            } finally {
                // Always clear the flag after selection is applied
                // Use setTimeout to ensure the flag is cleared AFTER all selection events have been queued
                if (setIsSyncingSelection) {
                    setTimeout(() => {
                        setIsSyncingSelection(false);
                        console.log('[AG Grid] Setting isSyncingSelection = false');
                    }, 0);
                }
            }
        }
    } catch (error) {
        console.error('[AG Grid] Failed to apply state:', error);
    }
}

/**
 * Reverse maps GridFilterOperator to AG Grid filter type
 */
function reverseMapFilterOperator(enumValue) {
    const mapping = {
        0: 'equals',
        1: 'notEqual',
        2: 'contains',
        3: 'notContains',
        4: 'startsWith',
        5: 'endsWith',
        6: 'lessThan',
        7: 'lessThanOrEqual',
        8: 'greaterThan',
        9: 'greaterThanOrEqual',
        10: 'blank',
        11: 'notBlank'
    };
    
    return mapping[enumValue] || 'equals';
}
