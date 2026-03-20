# Filter Builder Component

A declarative, composable filter builder that renders as an **inline canvas toolbar** — no popover or modal. Conditions appear as interactive segmented chips that let users change the operator and value directly without leaving the page.

## Features

- **Inline Canvas UI** — chips displayed directly in the page flow, not in a popover
- **Instant Filtering** — changes apply on every operator/value edit, no Apply button needed
- **8 Field Types** — Text, Number, Date, DateRange, Boolean, Select, MultiSelect, Custom
- **EditorType Override** — use CurrencyInput for a decimal price field, MaskedInput for phone numbers, etc.
- **Filter Presets** — one-click preset filter sets via `FilterPresets` child content
- **Declarative API** — define fields with `FilterField`, presets with `FilterPreset`
- **LINQ Extensions** — `ApplyFilters()` on any `IEnumerable<T>` or `IQueryable<T>`
- **Two-Way Binding** — `@bind-Filters` + `OnFilterChange`
- **Field Icons** — Lucide icon names on each field, shown in chips and the field picker dropdown

## Quick Start

```razor
<FilterBuilder TData="Product"
               @bind-Filters="activeFilters"
               OnFilterChange="HandleFilterChange">
    <FilterFields>
        <FilterField Field="Name"     Label="Product Name" Icon="tag"
                     Type="FilterFieldType.Text" />
        <FilterField Field="Price"    Label="Price"        Icon="dollar-sign"
                     Type="FilterFieldType.Number"
                     EditorType="FilterEditorType.Currency" Min="0" />
        <FilterField Field="Category" Label="Category"     Icon="layers"
                     Type="FilterFieldType.Select"
                     Options="@categoryOptions" />
    </FilterFields>
</FilterBuilder>

@code {
    private FilterGroup activeFilters = new();

    private void HandleFilterChange(FilterGroup filters)
    {
        filteredProducts = allProducts.ApplyFilters(filters).ToList();
    }
}
```

## UI Layout

Each active condition is rendered on its **own row** as a segmented chip:

```
[≡ Filter]                                            [✕ Clear]
[icon Label] | [operator Select ▾] | [value input] | [×]
[icon Label] | [operator Select ▾] | [value input] | [×]
```

- **`[≡ Filter]` button** — opens a dropdown showing all registered fields. Shows "Filter" label when no chips are active.
- **Each chip** — segmented pill (one per row): field label (with optional icon) | operator Select | value input | ×
- **`[✕ Clear]` button** — pinned to the top-right; appears only when at least one condition is active
- **`Presets` dropdown / Tabs** — appears only when `FilterPresets` child content is provided; style controlled by `PresetsVariant`

## Component API

### FilterBuilder\<TData\>

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Filters` | `FilterGroup?` | `null` | Active filter state. Use `@bind-Filters`. |
| `FiltersChanged` | `EventCallback<FilterGroup>` | — | Two-way binding callback. |
| `OnFilterChange` | `EventCallback<FilterGroup>` | — | Fires immediately when any condition changes. |
| `FilterFields` | `RenderFragment?` | `null` | Slot for `FilterField` child declarations. |
| `FilterPresets` | `RenderFragment?` | `null` | Slot for `FilterPreset` children. Adds a Presets button or tab bar. |
| `ButtonText` | `string` | `"Filter"` | Label on the add-filter button (when no conditions are active). |
| `PresetsVariant` | `FilterPresetsVariant` | `Dropdown` | How presets are rendered: `Dropdown` (button + menu) or `Tabs` (horizontal tab bar with implicit "All" tab). |
| `ChipSize` | `FilterChipSize` | `Small` | Height of every chip: `Small` (h-7), `Medium` (h-8), `Large` (h-9). |
| `Class` | `string?` | `null` | Additional CSS classes for the wrapper. |

### FilterField

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Field` | `string` | ✓ | Property name on the data model. |
| `Label` | `string` | ✓ | Display label in chips and the field picker. |
| `Icon` | `string?` | | Lucide icon name. |
| `Type` | `FilterFieldType` | | Data type — controls the default operators list. Default: `Text`. |
| `EditorType` | `FilterEditorType` | | Value input widget. `Auto` infers from `Type`. |
| `Operators` | `IEnumerable<FilterOperator>?` | | Custom operator list (auto-populated from `Type` if null). |
| `DefaultOperator` | `FilterOperator?` | | Operator pre-selected when creating a new condition. |
| `Options` | `IEnumerable<SelectOption>?` | | Options for Select / MultiSelect fields. |
| `Placeholder` | `string?` | | Placeholder text for the value input. |
| `Min` / `Max` / `Step` | `object?` | | Constraints for Number fields. |
| `ChildContent` | `RenderFragment<FilterCustomContext>?` | | Custom value input for `FilterFieldType.Custom`. The context provides `Condition` (read/write) and `NotifyChanged()` to propagate updates. |

### FilterPreset

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `Name` | `string` | ✓ | Display name in the Presets dropdown. |
| `Filters` | `FilterGroup` | ✓ | Pre-configured filter group. |
| `Icon` | `string?` | | Lucide icon name. |
| `Description` | `string?` | | Optional description. |

## EditorType Values

| Value | Component | Best for |
|-------|-----------|----------|
| `Auto` | Inferred from `Type` | Default — picks the right component automatically |
| `Input` | `Input` | Plain text |
| `Numeric` | `NumericInput<decimal?>` | Decimal/integer fields |
| `Currency` | `CurrencyInput<decimal?>` | Monetary/price fields |
| `Masked` | `MaskedInput` | Phone numbers, IBANs, postal codes |
| `Date` | `DatePicker` | Single date |
| `DateRange` | `DateRangePicker` | From/to date range |
| `Boolean` | `Switch` | True/false |
| `Select` | `Select<string>` (borderless) | Single value from a list |
| `MultiSelect` | `MultiSelect<SelectOption>` (borderless) | Multiple values |
| `Custom` | `RenderFragment<FilterCustomContext>` | Any custom control |

## LINQ Extensions

```csharp
// IEnumerable<T>
var results = allProducts.ApplyFilters(activeFilters).ToList();

// IQueryable<T>
var results = dbContext.Products.ApplyFilters(activeFilters).ToList();
```

## Filter Presets

```razor
<FilterBuilder TData="Order" @bind-Filters="activeFilters">
    <FilterFields>
        <FilterField Field="Status" Label="Status" Type="FilterFieldType.Select" Options="@statusOptions" />
    </FilterFields>
    <FilterPresets>
        <FilterPreset Name="Pending Orders" Icon="clock"       Filters="@GetPendingFilters()" />
        <FilterPreset Name="High Priority"  Icon="alert-circle" Filters="@GetHighPriorityFilters()" />
    </FilterPresets>
</FilterBuilder>
```

## Custom Value Control

Use `ChildContent` on a `FilterFieldType.Custom` field to provide any Blazor control.
The context is a `FilterCustomContext` with a `Condition` property and a `NotifyChanged()` callback:

```razor
<FilterField Field="Rating" Label="Rating" Icon="star" Type="FilterFieldType.Custom">
    <ChildContent Context="ctx">
        <Rating Value="@GetRating(ctx.Condition)"
                ValueChanged="@(async v => { ctx.Condition.Value = (double)v; await ctx.NotifyChanged(); })"
                MaxRating="5"
                AllowClear="true"
                Size="RatingSize.Small" />
    </ChildContent>
</FilterField>
```

## Examples

- [Basic Example](/components/filter/basic) — Product catalog with Text, Select, Currency, Boolean
- [All Field Types](/components/filter/all-types) — Employee directory with all 8 types + EditorType overrides
- [Presets](/components/filter/presets) — Order management with preset filters + DataTable
- [Custom Controls](/components/filter/custom) — Star-rating custom control
- [Nested Groups](/components/filter/nested) — Complex AND/OR FilterGroup composition
- [State Persistence](/components/filter/persistence) — Persist filters to localStorage

---

> **Localization**: Filter uses `ILocalizer` for the remove-chip aria-label and all 20 operator labels (Equals, Contains, StartsWith, etc.). Override any key under `Filter.*` or `Filter.Operator.*` in your `ILocalizer` implementation.
