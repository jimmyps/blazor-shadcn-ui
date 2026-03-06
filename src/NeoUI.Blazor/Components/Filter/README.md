# Filter Builder Component

A flexible and declarative filter builder system for Blazor applications with support for multiple field types, custom controls, and filter presets.

## Features

- **8 Field Types**: Text, Number, Date, DateRange, Boolean, Select, MultiSelect, and Custom
- **Declarative API**: Define filter fields using child content with `FilterField` components
- **Filter Presets**: Create reusable filter configurations with icons and descriptions
- **LINQ Extensions**: Apply filters to IEnumerable collections with `ApplyFilters()` extension method
- **Two-Way Binding**: Use `@bind-Filters` for seamless state management
- **Active Chips**: Displays applied filters as dismissible chips above the trigger button
- **Accessibility**: Full keyboard navigation and ARIA support

## Quick Start

```razor
@using NeoUI.Blazor.Filter

<FilterBuilder TData="Product"
               @bind-Filters="activeFilters"
               OnFilterChange="HandleFilterChange">
    <FilterFields>
        <FilterField Field="Name"
                     Label="Product Name"
                     Type="FilterFieldType.Text" />
        <FilterField Field="Price"
                     Label="Price"
                     Type="FilterFieldType.Number" />
        <FilterField Field="Category"
                     Label="Category"
                     Type="FilterFieldType.Select"
                     Options="@categoryOptions" />
    </FilterFields>
</FilterBuilder>

@code {
    private FilterGroup activeFilters = new();
    private List<Product> filteredProducts = new();

    private void HandleFilterChange(FilterGroup filters)
    {
        filteredProducts = allProducts.ApplyFilters(filters).ToList();
    }
}
```

## Component API

### FilterBuilder\<TData\>

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Filters` | `FilterGroup?` | `null` | Current filter state. Use `@bind-Filters` for two-way binding. |
| `FiltersChanged` | `EventCallback<FilterGroup>` | - | Two-way binding callback. |
| `OnFilterChange` | `EventCallback<FilterGroup>` | - | Fires immediately when Apply is clicked. |
| `FilterFields` | `RenderFragment?` | `null` | `FilterField` child declarations. |
| `FilterPresets` | `RenderFragment?` | `null` | `FilterPreset` child declarations. |
| `ButtonText` | `string` | `"Filters"` | Label on the trigger button. |
| `ButtonVariant` | `ButtonVariant` | `Outline` | Visual variant of the trigger button. |
| `ButtonSize` | `ButtonSize` | `Default` | Size of the trigger button. |
| `ShowChips` | `bool` | `true` | Show active filter chips above the button. |
| `Class` | `string?` | `null` | Additional CSS classes for the wrapper. |

### FilterField

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Field` | `string` | ✓ | Property name on the data model. |
| `Label` | `string` | ✓ | Human-readable label. |
| `Type` | `FilterFieldType` | | Field type (default: Text). |
| `Operators` | `IEnumerable<FilterOperator>?` | | Custom operators (auto-populated from Type if null). |
| `DefaultOperator` | `FilterOperator?` | | Default selected operator. |
| `Options` | `IEnumerable<SelectOption>?` | | Options for Select/MultiSelect. |
| `Placeholder` | `string?` | | Placeholder text. |
| `Min` | `object?` | | Minimum value (Number fields). |
| `Max` | `object?` | | Maximum value (Number fields). |
| `Step` | `object?` | | Step increment (Number fields). |
| `ChildContent` | `RenderFragment<FilterCondition>?` | | Custom value input. |

### FilterPreset

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Name` | `string` | ✓ | Display name of the preset. |
| `Filters` | `FilterGroup` | ✓ | Pre-configured filter group. |
| `Icon` | `string?` | | Lucide icon name. |
| `Description` | `string?` | | Optional description. |

## LINQ Extensions

```csharp
// IEnumerable<T>
var results = allItems.ApplyFilters(activeFilters).ToList();

// IQueryable<T>
var results = dbContext.Items.ApplyFilters(activeFilters).ToList();
```

## Examples

- [Basic Example](/components/filter/basic) — Simple product filtering
- [All Field Types](/components/filter/all-types) — Showcase all 8 field types with DataTable
- [Filter Presets](/components/filter/presets) — Reusable filter configurations with DataTable
- [Custom Controls](/components/filter/custom) — Custom input controls
- [Nested Groups](/components/filter/nested) — Complex AND/OR logic
- [State Persistence](/components/filter/persistence) — localStorage integration
