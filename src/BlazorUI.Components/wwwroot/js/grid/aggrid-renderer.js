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
            this.renderTemplate(templateId, params.data);
        }
    }
    
    async renderTemplate(templateId, data) {
        try {
            // Request HTML from Blazor
            const html = await this.params.dotNetRef.invokeMethodAsync(
                'RenderCellTemplate', 
                templateId, 
                data
            );
            
            if (html) {
                this.eGui.innerHTML = html;
            } else {
                // Fallback to field value
                const field = this.params.colDef.field;
                this.eGui.textContent = field ? this.params.data[field] : '';
            }
        } catch (error) {
            console.error('[AG Grid] Template rendering failed:', error);
            // Fallback to field value
            const field = this.params.colDef.field;
            this.eGui.textContent = field ? this.params.data[field] : '';
        }
    }
    
    getGui() {
        return this.eGui;
    }
    
    refresh(params) {
        // Update on data change
        const templateId = params.colDef.cellRendererParams?.templateId;
        if (templateId && params.dotNetRef) {
            this.params = params;
            this.renderTemplate(templateId, params.data);
            return true;
        }
        return false;
    }
    
    destroy() {
        // Cleanup
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
            
            applyState: (state) => {
                applyGridState(gridApi, state);
            },
            
            getState: () => {
                return getGridState(gridApi);
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
    
    // Register custom cell renderer
    const components = {
        templateRenderer: BlazorTemplateCellRenderer
    };
    
    // Enhance column definitions with dotNetRef for templates
    const enhancedColumnDefs = config.columnDefs.map(col => {
        console.log('[AG Grid] Column def:', col);
        return {
            ...col,
            cellRendererParams: col.cellRenderer === 'templateRenderer' 
                ? { ...col.cellRendererParams, dotNetRef }
                : col.cellRendererParams
        };
    });
    
    const gridOptions = {
        columnDefs: enhancedColumnDefs,
        rowData: [],
        components: components,
        
        // Selection
        rowSelection: config.rowSelection,
        
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
        },
        
        // Enable features
        enableRangeSelection: false,
        suppressRowClickSelection: false,
        
        // Event handlers - these receive events with the API attached
        onSortChanged: (event) => notifyStateChanged(event.api, dotNetRef),
        onFilterChanged: (event) => notifyStateChanged(event.api, dotNetRef),
        onPaginationChanged: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnMoved: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnResized: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnPinned: (event) => notifyStateChanged(event.api, dotNetRef),
        onColumnVisible: (event) => notifyStateChanged(event.api, dotNetRef),
        onSelectionChanged: (event) => {
            const selectedRows = event.api.getSelectedRows();
            dotNetRef.invokeMethodAsync('OnSelectionChanged', selectedRows)
                .catch(err => console.error('[AG Grid] Selection callback failed:', err));
        },
    };
    
    console.log('[AG Grid] Grid options built:', gridOptions);
    
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
        selectedRowIds: selectedRows.map((row, index) => row.id || index)
    };
}

/**
 * Applies state to the grid
 */
function applyGridState(api, state) {
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
        
        // Apply sorting
        if (state.sortDescriptors && state.sortDescriptors.length > 0) {
            const sortModel = state.sortDescriptors.map(sort => ({
                colId: sort.field,
                sort: sort.direction === 1 ? 'asc' : 'desc',
                sortIndex: sort.order
            }));
            api.applyColumnState({ state: sortModel });
        }
        
        // Apply filtering
        if (state.filterDescriptors && state.filterDescriptors.length > 0) {
            const filterModel = {};
            state.filterDescriptors.forEach(filter => {
                filterModel[filter.field] = {
                    type: reverseMapFilterOperator(filter.operator),
                    filter: filter.value
                };
            });
            api.setFilterModel(filterModel);
        }
        
        // Apply pagination
        if (state.pageNumber) {
            api.paginationGoToPage(state.pageNumber - 1);
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
