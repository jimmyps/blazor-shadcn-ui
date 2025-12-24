// AG Grid JavaScript interop module
// Note: This is a complete implementation structure for Milestone 4
// Full AG Grid integration requires the AG Grid Community library to be installed

// BlazorDomRenderer - Renders Blazor templates as HTML in AG Grid cells
class BlazorDomRenderer {
    init(params) {
        this.eGui = document.createElement('div');
        if (params.html) {
            this.eGui.innerHTML = params.html; // HTML from Blazor is sanitized
        } else {
            // Fallback: display the raw value
            this.eGui.textContent = params.value || '';
        }
    }

    getGui() {
        return this.eGui; // Return DOM element, not string
    }

    refresh(params) {
        if (params.html) {
            this.eGui.innerHTML = params.html;
            return true; // Refreshed successfully
        }
        return false;
    }

    destroy() {
        this.eGui = null;
    }
}

export function createGrid(element, config, dotNetRef) {
    // TODO: Import and initialize AG Grid when library is installed
    // import { createGrid } from 'ag-grid-community';
    
    console.log('AG Grid renderer: Initializing grid with config:', config);
    console.log('AG Grid Community library required for full functionality');
    
    // Placeholder implementation - returns mock grid instance
    const gridApi = {
        setRowData: (data) => {
            console.log('setRowData called with', data?.length, 'rows');
        },
        applyState: (state) => {
            console.log('applyState called with state:', state);
        },
        getState: () => {
            return {
                sortDescriptors: [],
                filterDescriptors: [],
                pageNumber: 1,
                pageSize: config.paginationPageSize || 25,
                columnStates: [],
                selectedRowIds: []
            };
        },
        destroy: () => {
            console.log('Grid destroyed');
        }
    };
    
    return gridApi;
    
    /* Full implementation when AG Grid is installed:
    
    const gridOptions = {
        ...config,
        components: {
            blazorDomRenderer: BlazorDomRenderer
        },
        onGridReady: (params) => {
            console.log('AG Grid ready');
        },
        onSortChanged: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onFilterChanged: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onSelectionChanged: async () => {
            const selectedRows = gridOptions.api.getSelectedRows();
            await dotNetRef.invokeMethodAsync('OnSelectionChanged', selectedRows);
            
            // Also update full state
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onColumnResized: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onColumnMoved: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onColumnVisible: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        onColumnPinned: async () => {
            const state = getState(gridOptions.api);
            await dotNetRef.invokeMethodAsync('OnGridStateChanged', state);
        },
        // Server-side row model datasource
        serverSideDatasource: config.rowModelType === 'serverSide' ? {
            getRows: async (params) => {
                const request = buildDataRequest(params);
                const response = await dotNetRef.invokeMethodAsync('OnDataRequested', request);
                params.success({
                    rowData: response.items,
                    rowCount: response.totalCount || response.filteredCount
                });
            }
        } : undefined
    };
    
    const grid = createGrid(element, gridOptions);
    
    return {
        setRowData: (data) => {
            gridOptions.api.setRowData(data);
        },
        applyState: (state) => {
            // Apply sort descriptors
            if (state.sortDescriptors && state.sortDescriptors.length > 0) {
                const sortModel = state.sortDescriptors.map(s => ({
                    colId: s.field,
                    sort: s.direction === 'Ascending' ? 'asc' : 'desc',
                    sortIndex: s.order
                }));
                gridOptions.api.applyColumnState({ 
                    state: sortModel,
                    defaultState: { sort: null }
                });
            }
            
            // Apply filter descriptors
            if (state.filterDescriptors && state.filterDescriptors.length > 0) {
                const filterModel = {};
                state.filterDescriptors.forEach(f => {
                    filterModel[f.field] = {
                        filterType: 'text',
                        type: mapFilterOperator(f.operator),
                        filter: f.value
                    };
                });
                gridOptions.api.setFilterModel(filterModel);
            }
            
            // Apply column states (visibility, width, pinning, order)
            if (state.columnStates && state.columnStates.length > 0) {
                const columnState = state.columnStates.map(cs => ({
                    colId: cs.field,
                    hide: !cs.visible,
                    width: parseInt(cs.width) || undefined,
                    pinned: cs.pinned === 'Left' ? 'left' : cs.pinned === 'Right' ? 'right' : null
                }));
                gridOptions.api.applyColumnState({ state: columnState });
            }
        },
        getState: () => {
            return getState(gridOptions.api);
        },
        destroy: () => {
            grid.destroy();
        }
    };
    */
}

function getState(api) {
    /* Full implementation:
    const sortModel = api.getColumnState().filter(c => c.sort);
    const filterModel = api.getFilterModel();
    const selectedRows = api.getSelectedRows();
    const columnState = api.getColumnState();
    
    return {
        sortDescriptors: sortModel.map(s => ({
            field: s.colId,
            direction: s.sort === 'asc' ? 'Ascending' : 'Descending',
            order: s.sortIndex || 0
        })),
        filterDescriptors: Object.keys(filterModel).map(field => ({
            field,
            operator: 'Contains',
            value: filterModel[field].filter,
            caseSensitive: false
        })),
        pageNumber: Math.floor((api.paginationGetCurrentPage() || 0)) + 1,
        pageSize: api.paginationGetPageSize() || 25,
        columnStates: columnState.map((col, index) => ({
            field: col.colId,
            visible: !col.hide,
            width: col.width ? `${col.width}px` : null,
            pinned: col.pinned === 'left' ? 'Left' : col.pinned === 'right' ? 'Right' : 'None',
            order: index
        })),
        selectedRowIds: selectedRows.map(row => row.id || row)
    };
    */
    return {
        sortDescriptors: [],
        filterDescriptors: [],
        pageNumber: 1,
        pageSize: 25,
        columnStates: [],
        selectedRowIds: []
    };
}

function buildDataRequest(params) {
    /* Full implementation:
    const pageSize = params.endRow - params.startRow;
    return {
        startIndex: params.startRow,
        count: pageSize,
        sortDescriptors: params.sortModel ? params.sortModel.map(s => ({
            field: s.colId,
            direction: s.sort === 'asc' ? 'Ascending' : 'Descending',
            order: 0
        })) : [],
        filterDescriptors: params.filterModel ? Object.keys(params.filterModel).map(field => ({
            field,
            operator: 'Contains',
            value: params.filterModel[field].filter,
            caseSensitive: false
        })) : [],
        pageNumber: Math.floor(params.startRow / pageSize) + 1,
        pageSize: pageSize,
        customParameters: {}
    };
    */
    return {
        startIndex: 0,
        count: 25,
        sortDescriptors: [],
        filterDescriptors: [],
        pageNumber: 1,
        pageSize: 25,
        customParameters: {}
    };
}

function mapFilterOperator(operator) {
    // Map GridFilterOperator to AG Grid filter types
    const mapping = {
        'Equals': 'equals',
        'NotEquals': 'notEqual',
        'Contains': 'contains',
        'NotContains': 'notContains',
        'StartsWith': 'startsWith',
        'EndsWith': 'endsWith',
        'LessThan': 'lessThan',
        'LessThanOrEqual': 'lessThanOrEqual',
        'GreaterThan': 'greaterThan',
        'GreaterThanOrEqual': 'greaterThanOrEqual',
        'IsEmpty': 'blank',
        'IsNotEmpty': 'notBlank'
    };
    return mapping[operator] || 'contains';
}
