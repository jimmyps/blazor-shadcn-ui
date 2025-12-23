// AG Grid JavaScript interop module
// Note: This is a placeholder implementation for Milestone 4
// Full AG Grid integration requires the AG Grid Community library to be installed

export function createGrid(element, config, dotNetRef) {
    // TODO: Import and initialize AG Grid when library is installed
    // import { createGrid } from 'ag-grid-community';
    
    console.warn('AG Grid renderer: AG Grid Community library not yet installed.');
    console.log('Grid configuration:', config);
    
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
                    rowCount: response.totalCount
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
            // Apply sort, filter, column state
            if (state.sortDescriptors) {
                const sortModel = state.sortDescriptors.map(s => ({
                    colId: s.field,
                    sort: s.direction === 'Ascending' ? 'asc' : 'desc'
                }));
                gridOptions.api.applyColumnState({ state: sortModel });
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
    
    return {
        sortDescriptors: sortModel.map(s => ({
            field: s.colId,
            direction: s.sort === 'asc' ? 'Ascending' : 'Descending',
            order: s.sortIndex || 0
        })),
        filterDescriptors: Object.keys(filterModel).map(field => ({
            field,
            operator: 'Contains',
            value: filterModel[field].filter
        })),
        pageNumber: 1,
        pageSize: 25,
        columnStates: [],
        selectedRowIds: []
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
            direction: s.sort === 'asc' ? 'Ascending' : 'Descending'
        })) : [],
        filterDescriptors: [],
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
