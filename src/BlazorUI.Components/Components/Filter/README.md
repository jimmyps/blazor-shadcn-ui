# Filter Builder Component

A flexible and declarative filter builder system for Blazor applications with support for multiple field types, custom controls, and filter presets.

## Features

- **8 Field Types**: Text, Number, Date, DateRange, Boolean, Select, MultiSelect, and Custom
- **Declarative API**: Define filter fields using child content with `FilterField` components
- **Filter Presets**: Create reusable filter configurations with icons and descriptions
- **LINQ Extensions**: Apply filters to IEnumerable collections with `ApplyFilters()` extension method
- **Two-Way Binding**: Use `@bind-Filters` for seamless state management
- **Mobile Responsive**: Popover-based UI that adapts to different screen sizes
- **Accessibility**: Full keyboard navigation and ARIA support
- **shadcn/ui Styling**: Consistent with the rest of the component library

## Installation

The Filter Builder component is part of the `BlazorUI.Components` package.

```bash
dotnet add package BlazorUI.Components
```

## Quick Start

```razor
@using BlazorUI.Components.Filter

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

### FilterBuilder<TData>

The main component that provides the filter builder UI.

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Filters` | `FilterGroup?` | `null` | The current filter state (supports two-way binding) |
| `FiltersChanged` | `EventCallback<FilterGroup>` | - | Event raised when filters change |
| `OnFilterChange` | `EventCallback<FilterGroup>` | - | Event raised when Apply is clicked |
| `FilterFields` | `RenderFragment?` | `null` | Child content for filter field definitions |
| `FilterPresets` | `RenderFragment?` | `null` | Child content for preset definitions |
| `ButtonText` | `string` | `"Filters"` | Text displayed on the trigger button |
| `ButtonVariant` | `ButtonVariant` | `Outline` | Variant of the trigger button |
| `ButtonSize` | `ButtonSize` | `Default` | Size of the trigger button |
| `ShowChips` | `bool` | `true` | Whether to show active filter chips |
| `Class` | `string?` | `null` | Additional CSS classes |

### FilterField

Metadata component that defines a filterable field. Does not render any UI.

#### Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Field` | `string` | ✓ | Property name from TData |
| `Label` | `string` | ✓ | Display label for the field |
| `Type` | `FilterFieldType` | | Field type (default: Text) |
| `Operators` | `List<FilterOperator>?` | | Custom operators (uses defaults if not specified) |
| `DefaultOperator` | `FilterOperator?` | | Default selected operator |
| `Options` | `List<SelectOption>?` | | Options for Select/MultiSelect |
| `CustomControl` | `RenderFragment<FilterCondition>?` | | Custom input control |
| `Placeholder` | `string?` | | Placeholder text for input |
| `Min` | `object?` | | Minimum value for Number fields |
| `Max` | `object?` | | Maximum value for Number fields |
| `Step` | `object?` | | Step value for Number fields |

### FilterPreset

Metadata component that defines a filter preset.

#### Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Name` | `string` | ✓ | Display name of the preset |
| `Icon` | `string?` | | Lucide icon name |
| `Filters` | `FilterGroup` | ✓ | Predefined filter configuration |
| `Description` | `string?` | | Optional description |

## Field Types

### 1. Text

For string-based filtering with text operations.

**Default Operators**: Contains, Equals, Starts With, Ends With, Is Empty, Is Not Empty

```razor
<FilterField Field="Name" 
             Label="Name" 
             Type="FilterFieldType.Text" />
```

### 2. Number

For numeric filtering with comparison operations.

**Default Operators**: Equals, Greater Than, Less Than, Between

```razor
<FilterField Field="Price" 
             Label="Price" 
             Type="FilterFieldType.Number"
             Min="0"
             Step="0.01" />
```

### 3. Date

For single date selection.

**Default Operators**: Equals, Between, Greater Than, Less Than

```razor
<FilterField Field="CreatedDate" 
             Label="Created Date" 
             Type="FilterFieldType.Date" />
```

### 4. DateRange

For date range selection (Between operator uses two date pickers).

```razor
<FilterField Field="OrderDate" 
             Label="Order Date" 
             Type="FilterFieldType.DateRange" />
```

### 5. Boolean

For true/false filtering.

**Default Operators**: Is True, Is False

```razor
<FilterField Field="IsActive" 
             Label="Active" 
             Type="FilterFieldType.Boolean" />
```

### 6. Select

Single selection from a list of options.

**Default Operators**: Is Any Of, Is None Of

```razor
<FilterField Field="Status" 
             Label="Status" 
             Type="FilterFieldType.Select"
             Options="@statusOptions" />
```

### 7. MultiSelect

Multiple selection from a list of options.

**Default Operators**: Is Any Of, Is None Of

```razor
<FilterField Field="Tags" 
             Label="Tags" 
             Type="FilterFieldType.MultiSelect"
             Options="@tagOptions" />
```

### 8. Custom

Use a custom RenderFragment for specialized input controls.

```razor
<FilterField Field="Rating" 
             Label="Rating" 
             Type="FilterFieldType.Custom">
    <CustomControl Context="condition">
        <Rating @bind-Value="condition.Value" />
    </CustomControl>
</FilterField>
```

## Filter Operators

| Operator | Description | Applicable Types |
|----------|-------------|------------------|
| `Equals` | Exact match | Text, Number, Date |
| `NotEquals` | Not equal to | Text, Number, Date |
| `Contains` | Contains substring | Text |
| `NotContains` | Does not contain substring | Text |
| `StartsWith` | Starts with | Text |
| `EndsWith` | Ends with | Text |
| `IsEmpty` | Is null or empty | Text |
| `IsNotEmpty` | Is not null or empty | Text |
| `GreaterThan` | Greater than | Number, Date |
| `LessThan` | Less than | Number, Date |
| `GreaterThanOrEqual` | Greater than or equal | Number, Date |
| `LessThanOrEqual` | Less than or equal | Number, Date |
| `Between` | Between two values | Number, Date, DateRange |
| `NotBetween` | Not between two values | Number, Date |
| `IsAnyOf` | Matches any option | Select, MultiSelect |
| `IsNoneOf` | Matches none of the options | Select, MultiSelect |
| `IsAllOf` | Matches all options | MultiSelect |
| `IsTrue` | Boolean true | Boolean |
| `IsFalse` | Boolean false | Boolean |

## LINQ Extension Methods

Apply filters to any `IEnumerable<T>` collection:

```csharp
using BlazorUI.Components.Filter;

var filtered = allProducts.ApplyFilters(activeFilters).ToList();
```

The `ApplyFilters` extension method evaluates the filter conditions against each item in the collection.

## Filter Presets

Create reusable filter configurations for common scenarios:

```razor
<FilterBuilder TData="Order" @bind-Filters="activeFilters">
    <FilterFields>
        <FilterField Field="Status" Label="Status" Type="FilterFieldType.Select" Options="@statusOptions" />
        <FilterField Field="Priority" Label="Priority" Type="FilterFieldType.Select" Options="@priorityOptions" />
        <FilterField Field="Amount" Label="Amount" Type="FilterFieldType.Number" />
    </FilterFields>
    
    <FilterPresets>
        <FilterPreset Name="High Priority Orders" 
                      Icon="alert-circle"
                      Filters="@GetHighPriorityFilters()" />
        
        <FilterPreset Name="Large Orders" 
                      Icon="trending-up"
                      Filters="@GetLargeOrdersFilters()" />
    </FilterPresets>
</FilterBuilder>

@code {
    private FilterGroup GetHighPriorityFilters()
    {
        return new FilterGroup
        {
            Logic = LogicalOperator.Or,
            Conditions = new List<FilterCondition>
            {
                new FilterCondition { Field = "Priority", Operator = FilterOperator.Equals, Value = "High" },
                new FilterCondition { Field = "Priority", Operator = FilterOperator.Equals, Value = "Urgent" }
            }
        };
    }
}
```

## Advanced Usage

### Custom Operators

Override default operators for a field:

```razor
<FilterField Field="Name" 
             Label="Name" 
             Type="FilterFieldType.Text"
             Operators="@customOperators" />

@code {
    private List<FilterOperator> customOperators = new()
    {
        FilterOperator.Equals,
        FilterOperator.StartsWith,
        FilterOperator.IsEmpty
    };
}
```

### Nested Filter Groups

Build complex filter logic with nested groups:

```csharp
// (Status = "Pending" OR Status = "Processing") AND Amount > 1000
var filters = new FilterGroup
{
    Logic = LogicalOperator.And,
    NestedGroups = new List<FilterGroup>
    {
        new FilterGroup
        {
            Logic = LogicalOperator.Or,
            Conditions = new List<FilterCondition>
            {
                new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Pending" },
                new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Processing" }
            }
        }
    },
    Conditions = new List<FilterCondition>
    {
        new FilterCondition { Field = "Amount", Operator = FilterOperator.GreaterThan, Value = 1000 }
    }
};
```

### State Persistence

Persist filters using localStorage:

```csharp
@inject IJSRuntime JS

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var json = await JS.InvokeAsync<string>("localStorage.getItem", "filters");
            if (!string.IsNullOrEmpty(json))
            {
                activeFilters = JsonSerializer.Deserialize<FilterGroup>(json);
            }
        }
    }
    
    private async Task HandleFilterChange(FilterGroup filters)
    {
        var json = JsonSerializer.Serialize(filters);
        await JS.InvokeVoidAsync("localStorage.setItem", "filters", json);
        
        filteredData = allData.ApplyFilters(filters).ToList();
    }
}
```

## Accessibility

The Filter Builder component follows WAI-ARIA best practices:

- **Keyboard Navigation**: Full keyboard support with Tab, Enter, Escape, and Arrow keys
- **ARIA Attributes**: Proper labels, roles, and states for screen readers
- **Focus Management**: Logical focus order and visible focus indicators
- **Screen Reader Support**: Descriptive labels and announcements for state changes

### Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Tab` / `Shift+Tab` | Navigate between fields |
| `Enter` | Apply filters |
| `Escape` | Close popover |
| `Arrow Keys` | Navigate select options |

## Styling

The component uses shadcn/ui design tokens and Tailwind CSS:

```css
/* CSS Variables used */
--primary
--secondary
--muted
--border
--ring
--background
--foreground
```

### Customization

Add custom classes to the wrapper:

```razor
<FilterBuilder Class="my-custom-class" ... />
```

## Examples

See the demo pages for comprehensive examples:

- [Basic Example](/components/filter/basic) - Simple product filtering
- [All Field Types](/components/filter/all-types) - Showcase all 8 field types
- [Custom Controls](/components/filter/custom) - Custom input controls
- [Filter Presets](/components/filter/presets) - Reusable filter configurations
- [State Persistence](/components/filter/persistence) - localStorage integration
- [Nested Groups](/components/filter/nested) - Complex AND/OR logic

## Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers (iOS Safari, Chrome Android)

## License

MIT License - See LICENSE file for details
