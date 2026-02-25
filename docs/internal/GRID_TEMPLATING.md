# Grid Templating with Interactive Components

## Overview

NeoUI DataGrid supports **rich templating with interactive Blazor components** through a unique hybrid rendering approach. Components are rendered to static HTML for performance, then enhanced with **automatic JavaScript event binding** for interactivity.

---

## Key Concepts

### 1. **Static HTML Rendering**

DataGrid cell templates use `HtmlRenderer` to convert Blazor components to static HTML strings:

```razor
<DataGridColumn Id="status" Header="Status">
    <CellTemplate Context="order">
        <Badge Variant="@GetStatusVariant(order.Status)">
            @order.Status
        </Badge>
    </CellTemplate>
</DataGridColumn>
```

**Renders to:**
```html
<span class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold bg-primary text-primary-foreground">
    Shipped
</span>
```

### 2. **Event Binding via Data Attributes**

For interactive components (buttons, links), use `data-action` attributes to trigger C# methods:

```razor
<DataGridColumn Id="actions" Header="Actions">
    <CellTemplate Context="order">
        <Button data-action="Edit">
            <LucideIcon Name="pencil" />
        </Button>
    </CellTemplate>
</DataGridColumn>
```

**Renders to:**
```html
<button type="button" class="..." data-action="Edit">
    <svg>...</svg>
</button>
```

### 3. **Automatic Action Discovery**

Methods marked with `[DataGridAction]` are automatically registered:

```csharp
[DataGridAction]
private void Edit(Order order)
{
    // This method is automatically invoked when data-action="Edit" is clicked
    NavigationManager.NavigateTo($"/orders/{order.Id}/edit");
}
```

---

## How It Works

### Architecture

```
┌─────────────────────────────────────────────────────┐
│  1. Developer writes Blazor components in template │
│     <Button data-action="Edit">Edit</Button>       │
└─────────────────────────────────────────────────────┘
                         ▼
┌─────────────────────────────────────────────────────┐
│  2. HtmlRenderer renders to static HTML            │
│     <button data-action="Edit">Edit</button>       │
└─────────────────────────────────────────────────────┘
                         ▼
┌─────────────────────────────────────────────────────┐
│  3. AG DataGrid displays HTML in cells                 │
│     (Fast virtualization, no Blazor overhead)      │
└─────────────────────────────────────────────────────┘
                         ▼
┌─────────────────────────────────────────────────────┐
│  4. JavaScript event delegation listens for clicks │
│     element.closest('[data-action]')               │
└─────────────────────────────────────────────────────┘
                         ▼
┌─────────────────────────────────────────────────────┐
│  5. JS Interop calls C# method                     │
│     dotNetRef.invokeMethodAsync('HandleCellAction')│
└─────────────────────────────────────────────────────┘
                         ▼
┌─────────────────────────────────────────────────────┐
│  6. Action registry invokes [DataGridAction] method    │
│     Edit(order) is called with typed item          │
└─────────────────────────────────────────────────────┘
```

---

## Features

### ✅ **What Works**

1. **Visual Rendering**
   - ✅ All Blazor components render correctly (Button, Badge, Icon, etc.)
   - ✅ CSS styling, classes, and themes applied
   - ✅ Conditional rendering and data binding
   - ✅ Complex component hierarchies

2. **Event Handling**
   - ✅ Click events via `data-action` attributes
   - ✅ Automatic C# method invocation
   - ✅ Type-safe row data passed to handlers
   - ✅ Async method support

3. **Developer Experience**
   - ✅ Natural Blazor syntax
   - ✅ Auto-discovery with `[DataGridAction]` attribute
   - ✅ IntelliSense and compile-time checking
   - ✅ No JavaScript required

### ❌ **Limitations**

1. **Interactive Events**
   - ❌ `@onclick` handlers (use `data-action` instead)
   - ❌ Two-way binding (`@bind`)
   - ❌ Component lifecycle methods (OnInitialized, etc.)
   - ❌ JavaScript interop from within templates

2. **Why?**
   - Templates are rendered **once** to static HTML
   - Blazor event handlers require component instances
   - DataGrid cells are managed by AG Grid, not Blazor renderer

---

## Usage Guide

### Basic Template

```razor
<DataGrid Items="@products" ActionHost="this">
    <Columns>
        <!-- Display column with custom formatting -->
        <DataGridColumn Field="Name" Header="Product">
            <CellTemplate Context="product">
                <div class="flex items-center gap-2">
                    <LucideIcon Name="package" Size="16" />
                    <span class="font-medium">@product.Name</span>
                </div>
            </CellTemplate>
        </DataGridColumn>
        
        <!-- Status badge -->
        <DataGridColumn Id="status" Header="Status">
            <CellTemplate Context="product">
                <Badge Variant="@(product.InStock ? BadgeVariant.Default : BadgeVariant.Destructive)">
                    @(product.InStock ? "In Stock" : "Out of Stock")
                </Badge>
            </CellTemplate>
        </DataGridColumn>
        
        <!-- Interactive actions -->
        <DataGridColumn Id="actions" Header="Actions">
            <CellTemplate Context="product">
                <div class="flex gap-1">
                    <Button Variant="ButtonVariant.Ghost" 
                            Size="ButtonSize.Icon"
                            data-action="Edit">
                        <LucideIcon Name="pencil" Size="16" />
                    </Button>
                    <Button Variant="ButtonVariant.Ghost" 
                            Size="ButtonSize.Icon"
                            data-action="Delete"
                            Class="text-destructive">
                        <LucideIcon Name="trash-2" Size="16" />
                    </Button>
                </div>
            </CellTemplate>
        </DataGridColumn>
    </Columns>
</DataGrid>

@code {
    private List<Product> products = new();
    
    [DataGridAction]
    private void Edit(Product product)
    {
        NavigationManager.NavigateTo($"/products/{product.Id}/edit");
    }
    
    [DataGridAction]
    private async Task Delete(Product product)
    {
        var confirmed = await DialogService.ConfirmAsync($"Delete {product.Name}?");
        if (confirmed)
        {
            await ProductService.DeleteAsync(product.Id);
            products.Remove(product);
        }
    }
}
```

### Action Registration Options

#### **Option 1: Auto-Discovery (Recommended)**

```csharp
<DataGrid Items="@orders" ActionHost="this">
    <!-- columns -->
</DataGrid>

@code {
    // Method name matches data-action value
    [DataGridAction]
    private void Edit(Order order) { }
    
    [DataGridAction]
    private void Delete(Order order) { }
}
```

#### **Option 2: Custom Action Names**

```csharp
// Use different method name than data-action
[DataGridAction(Name = "Edit")]
private void HandleOrderEdit(Order order) { }

[DataGridAction(Name = "Delete")]
private async Task HandleOrderDeletion(Order order) { }
```

#### **Option 3: Manual Registration**

```csharp
<DataGrid @ref="grid" Items="@orders">
    <!-- columns -->
</DataGrid>

@code {
    private DataGrid<Order> grid = default!;
    
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            grid.RegisterCellAction("CustomAction", HandleCustom);
        }
    }
    
    private void HandleCustom(Order order) { }
}
```

---

## Advanced Patterns

### Conditional Actions

```razor
<DataGridColumn Id="actions">
    <CellTemplate Context="order">
        @if (order.Status == OrderStatus.Pending)
        {
            <Button data-action="Approve">Approve</Button>
            <Button data-action="Reject">Reject</Button>
        }
        else if (order.Status == OrderStatus.Approved)
        {
            <Button data-action="Ship">Ship Order</Button>
        }
        else
        {
            <span class="text-muted-foreground">No actions</span>
        }
    </CellTemplate>
</DataGridColumn>

@code {
    [DataGridAction]
    private void Approve(Order order) { /* ... */ }
    
    [DataGridAction]
    private void Reject(Order order) { /* ... */ }
    
    [DataGridAction]
    private void Ship(Order order) { /* ... */ }
}
```

### Multiple Arguments via Data Attributes

```razor
<Button data-action="UpdateStatus" 
        data-status="@OrderStatus.Shipped"
        data-notify="true">
    Ship Order
</Button>

@code {
    [DataGridAction]
    private void UpdateStatus(Order order)
    {
        // Access additional data from JavaScript:
        // Custom implementation would extract data-status and data-notify
        // and pass as method parameters
    }
}
```

---

## Best Practices

### ✅ **Do**

1. **Use Blazor components for styling**
   ```razor
   <Badge Variant="BadgeVariant.Default">Status</Badge>
   ```

2. **Use data-action for interactivity**
   ```razor
   <Button data-action="Edit">Edit</Button>
   ```

3. **Mark methods with [DataGridAction]**
   ```csharp
   [DataGridAction]
   private void Edit(Order order) { }
   ```

4. **Set ActionHost parameter**
   ```razor
   <DataGrid Items="@orders" ActionHost="this">
   ```

### ❌ **Don't**

1. **Don't use @onclick**
   ```razor
   ❌ <Button @onclick="() => Edit(order)">Edit</Button>
   ✅ <Button data-action="Edit">Edit</Button>
   ```

2. **Don't use @bind**
   ```razor
   ❌ <Input @bind-Value="order.Name" />
   ✅ Use edit dialog/form instead
   ```

3. **Don't rely on component state**
   ```razor
   ❌ Component fields/properties won't persist
   ✅ Use server state or parent component state
   ```

---

## Performance Considerations

### Why Static HTML?

1. **Virtualization** - AG DataGrid can render 1000s of rows efficiently
2. **Memory** - No Blazor component instances per cell
3. **Rendering** - Single HTML string per cell vs component tree
4. **Scrolling** - Smooth virtualization without Blazor overhead

### Trade-offs

| Aspect | Static HTML | Blazor Components |
|--------|-------------|-------------------|
| Initial render | ⚡ Fast | 🐌 Slower |
| Memory usage | 💚 Low | 🔴 High (1 instance per cell) |
| Event handling | ⚡ Fast (delegation) | 🐌 Slower (per-instance) |
| Interactivity | ⚠️ Limited (data-action) | ✅ Full Blazor events |
| Use case | ✅ Large grids (100+ rows) | ✅ Small grids (<50 rows) |

---

## Troubleshooting

### Actions Not Working?

**Check:**
1. ✅ `ActionHost="this"` on Grid
2. ✅ Method has `[DataGridAction]` attribute
3. ✅ Method signature: `void MethodName(TItem item)` or `Task MethodName(TItem item)`
4. ✅ `data-action` value matches method name (or `[DataGridAction(Name = "...")]`)
5. ✅ Console for `[Grid] Auto-registered N actions` log

### Template Not Rendering?

**Check:**
1. ✅ `IDataGridDataGridTemplateRenderer` registered in DI: `services.AddNeoUIComponents()`
2. ✅ DataGrid renderer is **Transient**: `services.AddTransient(typeof(IDataGridRenderer<>), ...)`
3. ✅ Template has `Context` parameter: `<CellTemplate Context="item">`
4. ✅ Console for `[AgDataGridRenderer] Template rendered successfully` log

---

## Comparison with Other Approaches

| Approach | Pros | Cons |
|----------|------|------|
| **Hybrid (Current)** | ✅ Blazor syntax<br>✅ Performance<br>✅ Auto event binding | ⚠️ Limited events |
| **Pure Blazor (QuickGrid)** | ✅ Full interactivity<br>✅ Two-way binding | ❌ Poor virtualization<br>❌ Memory intensive |
| **Pure JS (AG Grid)** | ✅ Maximum performance<br>✅ Full AG DataGrid features | ❌ No Blazor components<br>❌ Manual JS |

---

## Future Enhancements

**Planned:**
- ✅ Full event support via component hydration
- ✅ Inline editing with two-way binding
- ✅ Real-time updates via SignalR
- ✅ Custom cell renderers with Blazor lifecycle

**Status:** Under consideration

---

## Related Documentation

- [Grid Overview](./README.md)
- [Grid Columns](./COLUMNS.md)
- [Grid Actions API](./API.md#actions)
- [Template Renderer](../../Services/README.md#template-renderer)

---

**Last Updated:** 2025-01-11  
**Version:** 1.0
