/**
 * AG Grid Renderer for BlazorUI
 * Uses AG Grid ES modules with Theming API (no manual CSS loading needed)
 */

import { createGrid as agCreateGrid } from 'https://cdn.jsdelivr.net/npm/ag-grid-community@32.3.3/dist/package/main.esm.mjs';
import { themeAlpine, themeBalham, themeMaterial, themeQuartz } from 'https://cdn.jsdelivr.net/npm/ag-grid-community@32.3.3/dist/package/main.esm.mjs';

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
        case 'Material':
            return themeMaterial;
        default:
            return themeQuartz;
    }
}

/**
 * Custom cell renderer for Blazor templates
 */
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
        console.log('[AG Grid] config.theme:', config.theme);
        console.log('[AG Grid] config.themeParams:', config.themeParams);
        
        // Handle Blazor ElementReference
        let element = elementOrRef;
        
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
        
        // Create theme with parameters using AG Grid's native API
        let theme;
        
        // Prepare theme parameters by merging defaults with config
        let themeParams = config.themeParams || {};
        
        // For Shadcn theme, merge with dynamically read shadcn design tokens
        if (config.theme === 'Shadcn') {
            const shadcnDefaults = createShadcnTheme();
            // Merge: shadcn defaults < config.themeParams (C# side takes precedence)
            themeParams = { ...shadcnDefaults, ...themeParams };
            console.log('[AG Grid] Creating Shadcn theme with parameters:', themeParams);
            theme = themeQuartz.withParams(themeParams);
        } else {
            // Use other AG Grid themes with parameters
            const baseTheme = getBaseTheme(config.theme);
            if (baseTheme && Object.keys(themeParams).length > 0) {
                console.log(`[AG Grid] Creating ${config.theme} theme with parameters:`, themeParams);
                theme = baseTheme.withParams(themeParams);
            } else {
                console.log(`[AG Grid] Using ${config.theme} theme without parameters`);
                theme = baseTheme;
            }
        }
        
        // Build grid options with event handlers and theme
        const gridOptions = buildGridOptionsWithEvents(config, dotNetRef);
        gridOptions.theme = theme;
        
        // Create AG Grid instance using the imported createGrid function
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
    
    // Map column definitions
    const columnDefs = config.columns?.map(col => ({
        field: col.field,
        headerName: col.headerName,
        width: col.width,
        minWidth: col.minWidth,
        maxWidth: col.maxWidth,
        flex: col.flex,
        resizable: col.resizable ?? true,
        sortable: col.sortable ?? false,
        filter: col.filter ?? false,
        editable: col.editable ?? false,
        hide: col.hide ?? false,
        pinned: col.pinned,
        cellRenderer: col.hasTemplate ? 'templateRenderer' : undefined,
        cellRendererParams: col.hasTemplate ? {
            templateId: col.templateId,
            dotNetRef: dotNetRef
        } : undefined,
        valueGetter: col.valueGetter ? (params) => {
            // For complex field paths like "Address.City"
            const parts = col.field.split('.');
            let value = params.data;
            for (const part of parts) {
                value = value?.[part];
            }
            return value;
        } : undefined
    })) || [];
    
    const gridOptions = {
        columnDefs,
        rowData: [],
        components,
        
        // Selection
        rowSelection: config.selectionMode === 'None' ? undefined : 
                     config.selectionMode === 'Single' ? 'single' : 'multiple',
        
        // Pagination
        pagination: config.pagingMode !== 'None',
        paginationPageSize: config.pageSize || 50,
        paginationPageSizeSelector: [10, 25, 50, 100],
        
        // Virtualization/Data model
        rowModelType: config.virtualizationMode === 'ServerSide' ? 'serverSide' :
                      config.virtualizationMode === 'Infinite' ? 'infinite' : 'clientSide',
        
        // Server-side datasource
        ...(config.virtualizationMode === 'ServerSide' && {
            serverSideDatasource: createServerSideDatasource(dotNetRef)
        }),
        
        // Infinite scroll datasource
        ...(config.virtualizationMode === 'Infinite' && {
            datasource: createInfiniteDatasource(dotNetRef)
        }),
        
        // Event handlers
        onSelectionChanged: (event) => {
            if (config.onSelectionChanged && dotNetRef) {
                const selectedRows = event.api.getSelectedRows();
                dotNetRef.invokeMethodAsync('OnSelectionChanged', selectedRows);
            }
        },
        
        onSortChanged: (event) => {
            notifyStateChanged(event.api, dotNetRef);
        },
        
        onFilterChanged: (event) => {
            notifyStateChanged(event.api, dotNetRef);
        },
        
        onColumnMoved: (event) => {
            notifyStateChanged(event.api, dotNetRef);
        },
        
        onColumnResized: (event) => {
            if (event.finished) {
                notifyStateChanged(event.api, dotNetRef);
            }
        },
        
        onColumnVisible: (event) => {
            notifyStateChanged(event.api, dotNetRef);
        },
        
        onColumnPinned: (event) => {
            notifyStateChanged(event.api, dotNetRef);
        },
        
        // Default column settings
        defaultColDef: {
            resizable: true,
            sortable: true,
            filter: false,
        }
    };
    
    return gridOptions;
}

/**
 * Creates a server-side datasource for AG Grid
 */
function createServerSideDatasource(dotNetRef) {
    return {
        getRows: async (params) => {
            try {
                console.log('[AG Grid] Server-side getRows called');
                console.log('[AG Grid] Request:', params.request);
                
                const request = mapServerSideRequest(params.request);
                const response = await dotNetRef.invokeMethodAsync('OnDataRequest', request);
                
                console.log('[AG Grid] Server response:', response);
                params.success({
                    rowData: response.rows,
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
            try {
                console.log('[AG Grid] Infinite scroll getRows called');
                console.log('[AG Grid] Start:', params.startRow, 'End:', params.endRow);
                
                const request = {
                    startRow: params.startRow,
                    endRow: params.endRow,
                    sortModel: mapSortModel(params.sortModel),
                    filterModel: mapFilterModel(params.filterModel)
                };
                
                const response = await dotNetRef.invokeMethodAsync('OnDataRequest', request);
                
                let lastRow = -1;
                if (response.rows.length < (params.endRow - params.startRow)) {
                    lastRow = params.startRow + response.rows.length;
                }
                
                params.successCallback(response.rows, lastRow);
            } catch (error) {
                console.error('[AG Grid] Infinite scroll data request failed:', error);
                params.failCallback();
            }
        }
    };
}

/**
 * Maps AG Grid server-side request to our format
 */
function mapServerSideRequest(agRequest) {
    return {
        startRow: agRequest.startRow,
        endRow: agRequest.endRow,
        sortModel: mapSortModel(agRequest.sortModel),
        filterModel: mapFilterModel(agRequest.filterModel),
        groupKeys: agRequest.groupKeys,
        pivotCols: agRequest.pivotCols,
        pivotMode: agRequest.pivotMode,
        rowGroupCols: agRequest.rowGroupCols,
        valueCols: agRequest.valueCols
    };
}

/**
 * Maps AG Grid sort model to our format
 */
function mapSortModel(sortModel) {
    if (!sortModel || sortModel.length === 0) return [];
    
    return sortModel.map(sort => ({
        field: sort.colId,
        direction: sort.sort === 'asc' ? 'Ascending' : 'Descending'
    }));
}

/**
 * Maps AG Grid filter model to our format
 */
function mapFilterModel(filterModel) {
    if (!filterModel) return [];
    
    const filters = [];
    for (const [field, filter] of Object.entries(filterModel)) {
        filters.push({
            field,
            operator: mapFilterOperator(filter.type),
            value: filter.filter,
            filterType: filter.filterType
        });
    }
    return filters;
}

/**
 * Maps AG Grid filter type to our operator enum
 */
function mapFilterOperator(agType) {
    switch (agType) {
        case 'equals': return 'Equals';
        case 'notEqual': return 'NotEquals';
        case 'lessThan': return 'LessThan';
        case 'lessThanOrEqual': return 'LessThanOrEqual';
        case 'greaterThan': return 'GreaterThan';
        case 'greaterThanOrEqual': return 'GreaterThanOrEqual';
        case 'contains': return 'Contains';
        case 'notContains': return 'NotContains';
        case 'startsWith': return 'StartsWith';
        case 'endsWith': return 'EndsWith';
        case 'blank': return 'IsNull';
        case 'notBlank': return 'IsNotNull';
        default: return 'Equals';
    }
}

/**
 * Notifies Blazor when grid state changes
 */
function notifyStateChanged(api, dotNetRef) {
    if (dotNetRef) {
        const state = getGridState(api);
        dotNetRef.invokeMethodAsync('OnStateChanged', state);
    }
}

/**
 * Gets the current grid state
 */
function getGridState(api) {
    const state = {
        sortModel: [],
        filterModel: {},
        columnState: []
    };
    
    // Get sort model
    const sortModel = api.getColumnState().filter(col => col.sort);
    state.sortModel = sortModel.map(col => ({
        field: col.colId,
        direction: col.sort === 'asc' ? 'Ascending' : 'Descending'
    }));
    
    // Get filter model
    const filterModel = api.getFilterModel();
    if (filterModel) {
        for (const [field, filter] of Object.entries(filterModel)) {
            state.filterModel[field] = {
                operator: mapFilterOperator(filter.type),
                value: filter.filter
            };
        }
    }
    
    // Get column state
    state.columnState = api.getColumnState().map(col => ({
        field: col.colId,
        width: col.width,
        hide: col.hide,
        pinned: col.pinned,
        sort: col.sort,
        sortIndex: col.sortIndex
    }));
    
    return state;
}

/**
 * Applies a saved state to the grid
 */
function applyGridState(api, state) {
    if (!state) return;
    
    // Apply column state
    if (state.columnState) {
        api.applyColumnState({
            state: state.columnState.map(col => ({
                colId: col.field,
                width: col.width,
                hide: col.hide,
                pinned: col.pinned,
                sort: col.sort,
                sortIndex: col.sortIndex
            })),
            applyOrder: true
        });
    }
    
    // Apply filter model
    if (state.filterModel) {
        const agFilterModel = {};
        for (const [field, filter] of Object.entries(state.filterModel)) {
            agFilterModel[field] = {
                type: reverseMapFilterOperator(filter.operator),
                filter: filter.value,
                filterType: 'text'
            };
        }
        api.setFilterModel(agFilterModel);
    }
}

/**
 * Reverse maps our operator enum to AG Grid filter type
 */
function reverseMapFilterOperator(enumValue) {
    switch (enumValue) {
        case 'Equals': return 'equals';
        case 'NotEquals': return 'notEqual';
        case 'LessThan': return 'lessThan';
        case 'LessThanOrEqual': return 'lessThanOrEqual';
        case 'GreaterThan': return 'greaterThan';
        case 'GreaterThanOrEqual': return 'greaterThanOrEqual';
        case 'Contains': return 'contains';
        case 'NotContains': return 'notContains';
        case 'StartsWith': return 'startsWith';
        case 'EndsWith': return 'endsWith';
        case 'IsNull': return 'blank';
        case 'IsNotNull': return 'notBlank';
        default: return 'equals';
    }
}
