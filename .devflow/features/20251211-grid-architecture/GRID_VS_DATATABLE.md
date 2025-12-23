# Grid vs DataTable Migration Guide

**Feature:** 20251211-grid-architecture  
**Status:** Planning  
**Created:** 2025-12-23

## Overview

This document provides guidance on when to use Grid versus DataTable, a detailed feature comparison, migration steps, and code examples to help developers transition from DataTable to the new Grid component.

---

## When to Use Grid vs DataTable

### Decision Matrix

| Criterion | Use DataTable | Use Grid |
|-----------|---------------|----------|
| **Dataset Size** | < 1,000 rows | 10,000+ rows |
| **Rendering Engine** | Pure Blazor (Table primitive) | AG Grid JavaScript (virtualization) |
| **Feature Requirements** | Basic sorting, filtering, pagination | Advanced features (pinning, virtualization, server-side data) |
| **Performance Needs** | Standard (client-side only) | High performance with large datasets |
| **Project Type** | Simple admin panels, CRUD forms | Enterprise applications, analytics dashboards |
| **Development Philosophy** | Pure Blazor, no JS dependencies | Willing to use JS library for performance |
| **Customization Needs** | Standard table styling | Advanced grid features (column reordering, resizing) |
| **State Management** | Basic (no persistence needed) | Advanced (state save/restore, export/import) |
| **Server Integration** | Client-side data only | Server-side paging, filtering, sorting |

### Recommendation Summary

**Use DataTable if:**
- You have fewer than 1,000 rows
- You prefer a pure Blazor solution with no JavaScript dependencies
- You're building a simple admin panel or CRUD interface
- You don't need advanced grid features (pinning, virtualization, state persistence)
- Your data fits entirely in memory on the client

**Use Grid if:**
- You have 10,000+ rows and need virtualization
- You need enterprise grid features (column pinning, resizing, reordering)
- You need server-side data loading for large datasets
- You need state persistence and serialization
- You want AG Grid-level performance and features
- You're willing to include AG Grid JavaScript library (~200KB)

---

## Feature Comparison Table

| Feature | DataTable | Grid | Notes |
|---------|-----------|------|-------|
| **Rendering** | Pure Blazor (Table primitive) | AG Grid JavaScript | Grid uses JS for performance |
| **Data Size** | < 1,000 rows optimal | 10,000+ rows with virtualization | Grid supports larger datasets efficiently |
| **Column API** | `Property` (lambda expression) | `Field` (string name) | Grid uses string fields for AG Grid JSON mapping |
| **Sorting** | ✅ Client-side only | ✅ Client-side + Server-side | Grid supports both modes |
| **Filtering** | ✅ Client-side only | ✅ Client-side + Server-side | Grid supports both modes |
| **Pagination** | ✅ Client-side only | ✅ Client + Server + Infinite scroll | Grid has more paging modes |
| **Virtualization** | ❌ Not supported | ✅ Row and column virtualization | Grid uses AG Grid's virtualization |
| **Column Pinning** | ❌ Not supported | ✅ Left and right pinning | Grid-only feature |
| **Column Resizing** | ❌ Not supported | ✅ Resizable columns | Grid-only feature |
| **Column Reordering** | ❌ Not supported | ✅ Drag-and-drop reordering | Grid-only feature |
| **Templates** | ✅ `RenderFragment<TItem>` | ✅ `RenderFragment<TItem>` | Both support custom templates |
| **Selection** | ✅ Single and multiple | ✅ Single and multiple | Similar APIs |
| **State Persistence** | ❌ Not supported | ✅ Save/restore, JSON export | Grid-only feature |
| **Export** | ❌ Not supported | ✅ CSV and Excel export | Grid-only feature |
| **Server-Side Data** | ❌ Not supported | ✅ `OnDataRequest` callback | Grid-only feature |
| **Performance (1,000 rows)** | Good | Excellent | Grid uses virtualization |
| **Performance (10,000 rows)** | Poor (not recommended) | Excellent | Grid designed for large datasets |
| **Bundle Size** | Small (~10KB) | Larger (~200KB AG Grid) | DataTable is lighter |
| **Accessibility** | ✅ WCAG 2.1 AA | ✅ WCAG 2.1 AA | Both are accessible |
| **Theme Integration** | ✅ shadcn CSS variables | ✅ shadcn CSS variables | Both integrate with theme |
| **Learning Curve** | Low (standard table) | Medium (more features) | Grid has more capabilities |

---

## Migration Steps

### Step 1: Update Component Reference

**Before (DataTable):**
```razor
@using BlazorUI.Components.DataTable

<DataTable TItem="Order" Items="@orders">
    <!-- columns -->
</DataTable>
```

**After (Grid):**
```razor
@using BlazorUI.Components.Grid

<Grid TItem="Order" Items="@orders">
    <Columns>
        <!-- columns -->
    </Columns>
</Grid>
```

**Changes:**
- Import namespace changes from `BlazorUI.Components.DataTable` to `BlazorUI.Components.Grid`
- Component name changes from `DataTable` to `Grid`
- Wrap columns in `<Columns>` parameter (Grid requires explicit `RenderFragment`)

---

### Step 2: Update Column Definitions

**Before (DataTable):**
```razor
<DataTableColumn TItem="Order" TValue="int" 
                 Property="@(o => o.Id)" 
                 Header="Order ID" 
                 Sortable="true" />

<DataTableColumn TItem="Order" TValue="string" 
                 Property="@(o => o.Customer)" 
                 Header="Customer" 
                 Sortable="true" />

<DataTableColumn TItem="Order" TValue="decimal" 
                 Property="@(o => o.Amount)" 
                 Header="Amount" 
                 Sortable="true" />
```

**After (Grid):**
```razor
<GridColumn Field="Id" 
            Header="Order ID" 
            Sortable="true" />

<GridColumn Field="Customer" 
            Header="Customer" 
            Sortable="true" />

<GridColumn Field="Amount" 
            Header="Amount" 
            Sortable="true" />
```

**Changes:**
- `DataTableColumn<TItem, TValue>` → `GridColumn<TItem>` (TItem is inferred from parent Grid)
- `Property="@(o => o.Id)"` → `Field="Id"` (lambda expression → string field name)
- Remove `TValue` type parameter (Grid uses Field strings, not typed lambdas)
- If you need custom value extraction, use `ValueSelector` parameter:
  ```razor
  <GridColumn Field="FullName" 
              Header="Full Name" 
              ValueSelector="@(o => $"{o.FirstName} {o.LastName}")" />
  ```

---

### Step 3: Update Custom Cell Templates

**Before (DataTable):**
```razor
<DataTableColumn TItem="Order" TValue="OrderStatus" Property="@(o => o.Status)" Header="Status">
    <CellTemplate Context="order">
        <Badge Variant="@GetStatusVariant(order.Status)">
            @order.Status
        </Badge>
    </CellTemplate>
</DataTableColumn>
```

**After (Grid):**
```razor
<GridColumn Field="Status" Header="Status">
    <CellTemplate Context="order">
        <Badge Variant="@GetStatusVariant(order.Status)">
            @order.Status
        </Badge>
    </CellTemplate>
</GridColumn>
```

**Changes:**
- Template syntax is identical (both use `RenderFragment<TItem>`)
- Remove `TValue` type parameter
- Change `Property` to `Field`

---

### Step 4: Update Selection Mode

**Before (DataTable):**
```razor
<DataTable TItem="Order" 
           Items="@orders" 
           SelectionMode="DataTableSelectionMode.Multiple"
           @bind-SelectedItems="selectedOrders">
    <!-- columns -->
</DataTable>
```

**After (Grid):**
```razor
<Grid TItem="Order" 
      Items="@orders" 
      SelectionMode="GridSelectionMode.Multiple"
      @bind-SelectedItems="selectedOrders">
    <Columns>
        <!-- columns -->
    </Columns>
</Grid>
```

**Changes:**
- `DataTableSelectionMode` → `GridSelectionMode`
- Enum values are the same: `None`, `Single`, `Multiple`
- Binding syntax is identical

---

### Step 5: Update Pagination (if applicable)

**Before (DataTable):**
```razor
<DataTable TItem="Order" 
           Items="@orders" 
           PageSize="25">
    <!-- DataTable has implicit client-side pagination -->
</DataTable>
```

**After (Grid):**
```razor
<Grid TItem="Order" 
      Items="@orders" 
      PagingMode="GridPagingMode.Client"
      PageSize="25">
    <Columns>
        <!-- columns -->
    </Columns>
</Grid>
```

**Changes:**
- Explicitly set `PagingMode="GridPagingMode.Client"` (DataTable had this implicitly)
- For server-side paging (Grid-only feature):
  ```razor
  <Grid PagingMode="GridPagingMode.Server"
        OnDataRequest="LoadDataAsync">
  ```

---

### Step 6: Register Grid Services (if not already done)

**In `Program.cs` or `Startup.cs`:**

```csharp
// Add Grid renderer and export services
builder.Services.AddScoped<IGridRenderer, AgGridRenderer>();
builder.Services.AddScoped<IGridExportService, CsvExportService>();
```

DataTable didn't require service registration, but Grid does for its renderer architecture.

---

## Breaking Changes

### 1. Column Property Binding

**Breaking Change:**  
DataTable uses `Property` (lambda expression) while Grid uses `Field` (string).

**Why:**  
Grid needs string field names for AG Grid JSON columnDefs. DataTable was pure Blazor and could use typed lambdas.

**Migration:**
- Replace `Property="@(o => o.FieldName)"` with `Field="FieldName"`
- For complex expressions, use `ValueSelector`:
  ```razor
  <GridColumn Field="FullName" 
              ValueSelector="@(o => $"{o.FirstName} {o.LastName}")" />
  ```

---

### 2. Column Type Parameter

**Breaking Change:**  
DataTable required `TValue` type parameter on columns. Grid does not.

**Why:**  
Grid uses string fields, so type inference is not needed per column.

**Migration:**
- Remove `TValue` from `DataTableColumn<TItem, TValue>` → `GridColumn<TItem>`
- TItem is inferred from parent Grid automatically

---

### 3. Pagination Mode

**Breaking Change:**  
DataTable had implicit client-side pagination. Grid requires explicit `PagingMode`.

**Why:**  
Grid supports multiple paging modes (Client, Server, InfiniteScroll). Explicit mode prevents ambiguity.

**Migration:**
- Add `PagingMode="GridPagingMode.Client"` to Grid if you were using DataTable pagination

---

### 4. Service Registration

**Breaking Change:**  
Grid requires DI service registration (`IGridRenderer`). DataTable did not.

**Why:**  
Grid uses pluggable renderer architecture (AG Grid, future native Blazor).

**Migration:**
- Add service registration to `Program.cs`:
  ```csharp
  builder.Services.AddScoped<IGridRenderer, AgGridRenderer>();
  ```

---

### 5. Column Wrapping

**Breaking Change:**  
Grid requires columns to be wrapped in `<Columns>` parameter. DataTable did not.

**Why:**  
Grid's internal architecture requires explicit RenderFragment for columns.

**Migration:**
- Wrap all `<GridColumn>` components in `<Columns>` parameter:
  ```razor
  <Grid Items="@orders">
      <Columns>
          <GridColumn Field="Id" Header="ID" />
      </Columns>
  </Grid>
  ```

---

## Side-by-Side Code Examples

### Example 1: Basic Table with Sorting

**DataTable:**
```razor
<DataTable TItem="Order" Items="@orders">
    <DataTableColumn TItem="Order" TValue="int" 
                     Property="@(o => o.Id)" 
                     Header="Order ID" 
                     Sortable="true" 
                     Width="100px" />
    <DataTableColumn TItem="Order" TValue="string" 
                     Property="@(o => o.Customer)" 
                     Header="Customer" 
                     Sortable="true" />
    <DataTableColumn TItem="Order" TValue="decimal" 
                     Property="@(o => o.Amount)" 
                     Header="Amount" 
                     Sortable="true" 
                     Width="120px" />
</DataTable>
```

**Grid:**
```razor
<Grid TItem="Order" Items="@orders">
    <Columns>
        <GridColumn Field="Id" 
                    Header="Order ID" 
                    Sortable="true" 
                    Width="100px" />
        <GridColumn Field="Customer" 
                    Header="Customer" 
                    Sortable="true" />
        <GridColumn Field="Amount" 
                    Header="Amount" 
                    Sortable="true" 
                    Width="120px" />
    </Columns>
</Grid>
```

---

### Example 2: Custom Cell Templates

**DataTable:**
```razor
<DataTable TItem="Order" Items="@orders">
    <DataTableColumn TItem="Order" TValue="int" Property="@(o => o.Id)" Header="Order ID" />
    <DataTableColumn TItem="Order" TValue="string" Property="@(o => o.Customer)" Header="Customer" />
    <DataTableColumn TItem="Order" TValue="OrderStatus" Property="@(o => o.Status)" Header="Status">
        <CellTemplate Context="order">
            <Badge Variant="@GetStatusVariant(order.Status)">
                @order.Status
            </Badge>
        </CellTemplate>
    </DataTableColumn>
    <DataTableColumn TItem="Order" TValue="decimal" Property="@(o => o.Amount)" Header="Amount">
        <CellTemplate Context="order">
            <span class="font-mono">@order.Amount.ToString("C")</span>
        </CellTemplate>
    </DataTableColumn>
</DataTable>

@code {
    private BadgeVariant GetStatusVariant(OrderStatus status) => status switch
    {
        OrderStatus.Delivered => BadgeVariant.Default,
        OrderStatus.Cancelled => BadgeVariant.Destructive,
        _ => BadgeVariant.Secondary
    };
}
```

**Grid:**
```razor
<Grid TItem="Order" Items="@orders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Status" Header="Status">
            <CellTemplate Context="order">
                <Badge Variant="@GetStatusVariant(order.Status)">
                    @order.Status
                </Badge>
            </CellTemplate>
        </GridColumn>
        <GridColumn Field="Amount" Header="Amount">
            <CellTemplate Context="order">
                <span class="font-mono">@order.Amount.ToString("C")</span>
            </CellTemplate>
        </GridColumn>
    </Columns>
</Grid>

@code {
    private BadgeVariant GetStatusVariant(OrderStatus status) => status switch
    {
        OrderStatus.Delivered => BadgeVariant.Default,
        OrderStatus.Cancelled => BadgeVariant.Destructive,
        _ => BadgeVariant.Secondary
    };
}
```

---

### Example 3: Multiple Selection with Actions

**DataTable:**
```razor
<DataTable TItem="Order" 
           Items="@orders" 
           SelectionMode="DataTableSelectionMode.Multiple"
           @bind-SelectedItems="selectedOrders">
    <DataTableColumn TItem="Order" TValue="int" Property="@(o => o.Id)" Header="Order ID" />
    <DataTableColumn TItem="Order" TValue="string" Property="@(o => o.Customer)" Header="Customer" />
    <DataTableColumn TItem="Order" TValue="decimal" Property="@(o => o.Amount)" Header="Amount" />
</DataTable>

@if (selectedOrders.Any())
{
    <div class="mt-4">
        <Button OnClick="DeleteSelected" Variant="ButtonVariant.Destructive">
            Delete @selectedOrders.Count Selected
        </Button>
    </div>
}

@code {
    private IReadOnlyCollection<Order> selectedOrders = Array.Empty<Order>();
    
    private void DeleteSelected()
    {
        // Delete logic
    }
}
```

**Grid:**
```razor
<Grid TItem="Order" 
      Items="@orders" 
      SelectionMode="GridSelectionMode.Multiple"
      @bind-SelectedItems="selectedOrders">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Amount" Header="Amount" />
    </Columns>
</Grid>

@if (selectedOrders.Any())
{
    <div class="mt-4">
        <Button OnClick="DeleteSelected" Variant="ButtonVariant.Destructive">
            Delete @selectedOrders.Count Selected
        </Button>
    </div>
}

@code {
    private IReadOnlyCollection<Order> selectedOrders = Array.Empty<Order>();
    
    private void DeleteSelected()
    {
        // Delete logic
    }
}
```

---

### Example 4: Pagination

**DataTable:**
```razor
<DataTable TItem="Order" 
           Items="@orders" 
           PageSize="25">
    <DataTableColumn TItem="Order" TValue="int" Property="@(o => o.Id)" Header="Order ID" />
    <DataTableColumn TItem="Order" TValue="string" Property="@(o => o.Customer)" Header="Customer" />
    <DataTableColumn TItem="Order" TValue="decimal" Property="@(o => o.Amount)" Header="Amount" />
</DataTable>
```

**Grid:**
```razor
<Grid TItem="Order" 
      Items="@orders" 
      PagingMode="GridPagingMode.Client"
      PageSize="25">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" />
        <GridColumn Field="Customer" Header="Customer" />
        <GridColumn Field="Amount" Header="Amount" />
    </Columns>
</Grid>
```

---

### Example 5: Server-Side Data (Grid Only)

**Grid with Server-Side Paging:**
```razor
<Grid TItem="Order" 
      PagingMode="GridPagingMode.Server"
      PageSize="25"
      OnDataRequest="LoadOrdersAsync">
    <Columns>
        <GridColumn Field="Id" Header="Order ID" Sortable="true" />
        <GridColumn Field="Customer" Header="Customer" Sortable="true" Filterable="true" />
        <GridColumn Field="Amount" Header="Amount" Sortable="true" />
        <GridColumn Field="OrderDate" Header="Order Date" Sortable="true" />
    </Columns>
</Grid>

@code {
    private async Task<GridDataResponse<Order>> LoadOrdersAsync(GridDataRequest<Order> request)
    {
        // Call API with request parameters (sort, filter, page)
        var result = await OrderService.GetOrdersAsync(
            page: request.PageNumber,
            pageSize: request.PageSize,
            sortBy: request.SortDescriptors.FirstOrDefault()?.Field,
            sortDirection: request.SortDescriptors.FirstOrDefault()?.Direction.ToString(),
            filters: request.FilterDescriptors
        );
        
        return new GridDataResponse<Order>
        {
            Items = result.Items,
            TotalCount = result.TotalCount,
            FilteredCount = result.FilteredCount
        };
    }
}
```

**Note:** DataTable does not support server-side data loading. This is a Grid-only feature.

---

## Summary

### What Stays the Same
- Template syntax (`RenderFragment<TItem>`)
- Selection mode concepts (None, Single, Multiple)
- Selection binding (`@bind-SelectedItems`)
- Theme integration (both use shadcn CSS variables)
- Accessibility features (both are WCAG 2.1 AA compliant)

### What Changes
- Component name: `DataTable` → `Grid`
- Column component: `DataTableColumn` → `GridColumn`
- Column binding: `Property` (lambda) → `Field` (string)
- Type parameter: `TValue` removed from columns
- Pagination: Implicit → Explicit `PagingMode`
- Column wrapping: None → `<Columns>` parameter required
- Service registration: None → `IGridRenderer` DI required

### New Capabilities (Grid Only)
- Virtualization for large datasets (10,000+ rows)
- Server-side paging, sorting, filtering
- Column pinning (left/right)
- Column resizing and reordering
- State persistence (save/restore to localStorage)
- State serialization (export/import JSON)
- Export to CSV and Excel
- Infinite scroll paging mode

---

## Migration Checklist

- [ ] Update component imports (`BlazorUI.Components.Grid`)
- [ ] Rename `DataTable` to `Grid`
- [ ] Wrap columns in `<Columns>` parameter
- [ ] Update column definitions (`Property` → `Field`)
- [ ] Remove `TValue` type parameters from columns
- [ ] Update selection mode enum (`DataTableSelectionMode` → `GridSelectionMode`)
- [ ] Add explicit `PagingMode` if using pagination
- [ ] Register Grid services in DI (`IGridRenderer`)
- [ ] Test all functionality (sorting, filtering, selection, templates)
- [ ] Consider using new Grid-only features (virtualization, server-side data, state persistence)

---

## Additional Resources

- [Grid Component Specification](spec.md)
- [Grid Technical Plan](plan.md)
- [Grid Demo Examples](GRID_DEMOS_V1.md)
- [DataTable Component Documentation](../20251111-data-table-component/spec.md)
