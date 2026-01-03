/**
 * AG Grid Renderer for BlazorUI
 * Auto-loads AG Grid Community from CDN and provides grid rendering with theme support
 */

import { createShadcnTheme } from './theme-shadcn.js';

// AG Grid version and CDN URLs
const AG_GRID_VERSION = '32.3.3';
const AG_GRID_CDN = `https://cdn.jsdelivr.net/npm/ag-grid-community@${AG_GRID_VERSION}/dist/ag-grid-community.min.js`;
const AG_GRID_CSS = `https://cdn.jsdelivr.net/npm/ag-grid-community@${AG_GRID_VERSION}/styles/ag-grid.min.css`;
const AG_GRID_THEME_QUARTZ_CSS = `https://cdn.jsdelivr.net/npm/ag-grid-community@${AG_GRID_VERSION}/styles/ag-theme-quartz.min.css`;
const AG_GRID_THEME_ALPINE_CSS = `https://cdn.jsdelivr.net/npm/ag-grid-community@${AG_GRID_VERSION}/styles/ag-theme-alpine.min.css`;
const AG_GRID_THEME_BALHAM_CSS = `https://cdn.jsdelivr.net/npm/ag-grid-community@${AG_GRID_VERSION}/styles/ag-theme-balham.min.css`;
const AG_GRID_THEME_MATERIAL_CSS = `https://cdn.jsdelivr.net/npm/ag-grid-community@${AG_GRID_VERSION}/styles/ag-theme-material.min.css`;

let agGridLoaded = false;
let agGridLoadPromise = null;
let loadedThemes = new Set();

/**
 * Loads AG Grid library and theme CSS dynamically from CDN
 */
async function ensureAgGridLoaded(theme = 'Quartz') {
    if (agGridLoaded) {
        // Ensure the requested theme CSS is loaded
        await ensureThemeLoaded(theme);
        return;
    }
    
    if (agGridLoadPromise) {
        await agGridLoadPromise;
        await ensureThemeLoaded(theme);
        return;
    }
    
    agGridLoadPromise = (async () => {
        try {
            console.log('[AG Grid] Loading AG Grid from CDN...');
            
            // Load base CSS files
            await loadStylesheet(AG_GRID_CSS);
            
            // Load AG Grid JavaScript
            await loadScript(AG_GRID_CDN);
            
            // Wait for agGrid global to be available
            await waitForGlobal('agGrid', 5000);
            
            agGridLoaded = true;
            console.log('[AG Grid] AG Grid loaded successfully');
        } catch (error) {
            console.error('[AG Grid] Failed to load from CDN:', error);
            agGridLoadPromise = null; // Reset promise to allow retry
            throw new Error('AG Grid failed to load. Please check your internet connection and try again.');
        }
    })();
    
    await agGridLoadPromise;
    await ensureThemeLoaded(theme);
}

/**
 * Ensures a specific theme CSS is loaded
 */
async function ensureThemeLoaded(theme) {
    if (loadedThemes.has(theme)) {
        return;
    }
    
    const themeCssUrl = getThemeCssUrl(theme);
    if (themeCssUrl) {
        await loadStylesheet(themeCssUrl);
        loadedThemes.add(theme);
        console.log(`[AG Grid] Theme ${theme} CSS loaded`);
    }
}

/**
 * Gets the CSS URL for a given theme
 */
function getThemeCssUrl(theme) {
    switch (theme) {
        case 'Shadcn':
        case 'Quartz':
            return AG_GRID_THEME_QUARTZ_CSS;
        case 'Alpine':
            return AG_GRID_THEME_ALPINE_CSS;
        case 'Balham':
            return AG_GRID_THEME_BALHAM_CSS;
        case 'Material':
            return AG_GRID_THEME_MATERIAL_CSS;
        default:
            return AG_GRID_THEME_QUARTZ_CSS;
    }
}

/**
 * Dynamically loads a script from URL
 */
function loadScript(url) {
    return new Promise((resolve, reject) => {
        // Check if already loaded
        if (document.querySelector(`script[src="${url}"]`)) {
            resolve();
            return;
        }
        
        const script = document.createElement('script');
        script.src = url;
        script.onload = resolve;
        script.onerror = () => reject(new Error(`Failed to load script: ${url}`));
        document.head.appendChild(script);
    });
}

/**
 * Dynamically loads a stylesheet from URL
 */
function loadStylesheet(url) {
    return new Promise((resolve, reject) => {
        // Check if already loaded
        if (document.querySelector(`link[href="${url}"]`)) {
            resolve();
            return;
        }
        
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = url;
        link.onload = resolve;
        link.onerror = () => reject(new Error(`Failed to load stylesheet: ${url}`));
        document.head.appendChild(link);
    });
}

/**
 * Waits for a global variable to be available
 */
function waitForGlobal(globalName, timeout = 5000) {
    return new Promise((resolve, reject) => {
        if (window[globalName]) {
            resolve();
            return;
        }
        
        const startTime = Date.now();
        const checkInterval = setInterval(() => {
            if (window[globalName]) {
                clearInterval(checkInterval);
                resolve();
            } else if (Date.now() - startTime > timeout) {
                clearInterval(checkInterval);
                reject(new Error(`Timeout waiting for ${globalName} to load`));
            }
        }, 50);
    });
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
        console.log('[AG Grid] elementOrRef:', elementOrRef);
        console.log('[AG Grid] elementOrRef constructor:', elementOrRef?.constructor?.name);
        console.log('[AG Grid] config.theme:', config.theme);
        console.log('[AG Grid] config.themeParams:', config.themeParams);
        
        // Ensure AG Grid is loaded before proceeding with the appropriate theme CSS
        await ensureAgGridLoaded(config.theme || 'Quartz');
        
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
        
        // Build grid options with event handlers and theme parameters
        const gridOptions = buildGridOptionsWithEvents(config, dotNetRef);
        
        // Prepare theme parameters
        let themeParams = config.themeParams || {};
        
        // For Shadcn theme, merge with dynamically read shadcn design tokens
        if (config.theme === 'Shadcn') {
            const shadcnDefaults = createShadcnTheme();
            // Merge: shadcn defaults < config.themeParams (C# side takes precedence)
            themeParams = { ...shadcnDefaults, ...themeParams };
            console.log('[AG Grid] Applied Shadcn theme with design tokens');
        }
        
        // Apply theme parameters as CSS custom properties
        if (themeParams && Object.keys(themeParams).length > 0) {
            applyThemeParameters(element, themeParams);
        }
        
        // Create AG Grid instance - returns the API directly
        const gridApi = window.agGrid.createGrid(element, gridOptions);
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
 * Applies theme parameters as CSS custom properties on the grid element
 * @param {HTMLElement} element - The grid container element
 * @param {Object} params - Theme parameters to apply
 */
function applyThemeParameters(element, params) {
    console.log('[AG Grid] Applying theme parameters:', params);
    
    // Map parameter names to AG Grid CSS variable names
    const parameterMap = {
        // Spacing & Sizing
        'spacing': '--ag-grid-size',
        'rowHeight': '--ag-row-height',
        'headerHeight': '--ag-header-height',
        'fontSize': '--ag-font-size',
        'iconSize': '--ag-icon-size',
        'inputHeight': '--ag-list-item-height',
        
        // Colors
        'accentColor': '--ag-accent-color',
        'backgroundColor': '--ag-background-color',
        'foregroundColor': '--ag-foreground-color',
        'borderColor': '--ag-border-color',
        'headerBackgroundColor': '--ag-header-background-color',
        'headerForegroundColor': '--ag-header-foreground-color',
        'rowHoverColor': '--ag-row-hover-color',
        'oddRowBackgroundColor': '--ag-odd-row-background-color',
        'selectedRowBackgroundColor': '--ag-selected-row-background-color',
        'rangeSelectionBorderColor': '--ag-range-selection-border-color',
        'cellTextColor': '--ag-data-color',
        'invalidColor': '--ag-invalid-color',
        'modalOverlayBackgroundColor': '--ag-modal-overlay-background-color',
        'chromeBackgroundColor': '--ag-chrome-background-color',
        'tooltipBackgroundColor': '--ag-tooltip-background-color',
        'tooltipTextColor': '--ag-tooltip-text-color',
        
        // Typography
        'fontFamily': '--ag-font-family',
        'headerFontSize': '--ag-header-column-resize-handle-height',
        'headerFontWeight': '--ag-header-font-weight',
        
        // Borders
        'borderRadius': '--ag-border-radius',
        'wrapperBorderRadius': '--ag-wrapper-border-radius',
    };
    
    for (const [key, value] of Object.entries(params)) {
        const cssVar = parameterMap[key];
        if (cssVar) {
            let cssValue = value;
            
            // Convert numeric values to pixels for size/spacing parameters
            if (typeof value === 'number') {
                if (key.includes('Height') || key.includes('Size') || key === 'spacing' || key.includes('Width')) {
                    cssValue = `${value}px`;
                } else if (key === 'borderRadius' || key === 'wrapperBorderRadius') {
                    cssValue = `${value}px`;
                }
            }
            
            element.style.setProperty(cssVar, cssValue);
            console.log(`[AG Grid] Set ${cssVar} = ${cssValue}`);
        }
    }
    
    // Handle special boolean parameters
    if (params.borders === false) {
        element.classList.add('ag-no-borders');
    }
    if (params.wrapperBorder === true) {
        element.classList.add('ag-wrapper-border');
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
