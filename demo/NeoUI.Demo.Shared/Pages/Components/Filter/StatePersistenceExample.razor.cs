using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NeoUI.Blazor;

namespace NeoUI.Demo.Shared.Pages.Components.Filter;

public partial class StatePersistenceExample
{
    internal const string StorageKey = "neoui.demo.filter.state";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private static readonly IReadOnlyDictionary<string, PersistedFieldShape> FieldShapes =
        new Dictionary<string, PersistedFieldShape>(StringComparer.OrdinalIgnoreCase)
        {
            ["OrderNumber"] = new(FilterFieldType.Text, FilterEditorType.Input),
            ["Customer"] = new(FilterFieldType.Select, FilterEditorType.Combobox),
            ["Status"] = new(FilterFieldType.Select, FilterEditorType.Select),
            ["Priority"] = new(FilterFieldType.Select, FilterEditorType.Select),
            ["Amount"] = new(FilterFieldType.Number, FilterEditorType.Currency)
        };

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private FilterBuilder<Order>? _filterBuilder;
    private FilterGroup _activeFilters = new();
    private List<Order> _allOrders = new();
    private List<Order> _filteredOrders = new();
    private List<SelectOption> _customerOptions = new();
    private string? _savedStateJson;
    private string _statusMessage = "Changes save automatically after the first client render.";
    private bool _hasSavedState;
    private bool _restorePresetSelectionPending;
    private int _builderRenderKey;

    private readonly List<SelectOption> _statusOptions =
    [
        new("Pending", "Pending"),
        new("Processing", "Processing"),
        new("Shipped", "Shipped"),
        new("Delivered", "Delivered"),
        new("Cancelled", "Cancelled")
    ];

    private readonly List<SelectOption> _priorityOptions =
    [
        new("Low", "Low"),
        new("Medium", "Medium"),
        new("High", "High")
    ];

    protected override void OnInitialized()
    {
        _allOrders =
        [
            new() { Id = 1, OrderNumber = "ORD-001", Customer = "Alice Johnson", Status = "Pending", Priority = "High", Amount = 1250.00m },
            new() { Id = 2, OrderNumber = "ORD-002", Customer = "Bob Smith", Status = "Processing", Priority = "Medium", Amount = 450.50m },
            new() { Id = 3, OrderNumber = "ORD-003", Customer = "Carol White", Status = "Shipped", Priority = "Low", Amount = 89.99m },
            new() { Id = 4, OrderNumber = "ORD-004", Customer = "David Brown", Status = "Delivered", Priority = "High", Amount = 3200.00m },
            new() { Id = 5, OrderNumber = "ORD-005", Customer = "Eve Davis", Status = "Pending", Priority = "Medium", Amount = 175.25m },
            new() { Id = 6, OrderNumber = "ORD-006", Customer = "Frank Miller", Status = "Cancelled", Priority = "Low", Amount = 299.99m },
            new() { Id = 7, OrderNumber = "ORD-007", Customer = "Grace Lee", Status = "Processing", Priority = "High", Amount = 5500.00m },
            new() { Id = 8, OrderNumber = "ORD-008", Customer = "Henry Wilson", Status = "Pending", Priority = "Medium", Amount = 650.00m },
            new() { Id = 9, OrderNumber = "ORD-009", Customer = "Iris Martinez", Status = "Shipped", Priority = "High", Amount = 2100.75m },
            new() { Id = 10, OrderNumber = "ORD-010", Customer = "Jack Anderson", Status = "Delivered", Priority = "Low", Amount = 55.00m }
        ];

        _filteredOrders = _allOrders.ToList();
        _customerOptions = _allOrders
            .Select(o => o.Customer)
            .Distinct()
            .OrderBy(customer => customer)
            .Select(customer => new SelectOption(customer, customer))
            .ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RestoreSavedStateAsync(showEmptyStatus: false, triggerRender: true);
            return;
        }

        if (_restorePresetSelectionPending && _filterBuilder != null)
        {
            _restorePresetSelectionPending = false;
            _filterBuilder.RestoreMatchingPresetSelection(_activeFilters);
        }
    }

    private async Task HandleFilterChange(FilterGroup filters)
    {
        _activeFilters = CloneGroup(filters);
        ApplyCurrentFilters();
        await PersistCurrentFiltersAsync("Saved filter state to localStorage.");
    }

    private Task RestoreSavedStateAsync()
        => RestoreSavedStateAsync(showEmptyStatus: true, triggerRender: false);

    private async Task RestoreSavedStateAsync(bool showEmptyStatus, bool triggerRender)
    {
        try
        {
            var json = await JS.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            _hasSavedState = !string.IsNullOrWhiteSpace(json);

            if (!_hasSavedState)
            {
                _savedStateJson = null;
                if (showEmptyStatus)
                {
                    _statusMessage = "No saved filter state was found in localStorage.";
                }
            }
            else
            {
                _savedStateJson = FormatJson(json!);
                var restored = JsonSerializer.Deserialize<FilterGroup>(json!, JsonOptions);

                if (restored == null)
                {
                    _activeFilters = new FilterGroup();
                    _statusMessage = "Saved filter state was empty, so the builder was reset.";
                }
                else
                {
                    _activeFilters = NormalizeGroup(restored);
                    _builderRenderKey++;
                    _restorePresetSelectionPending = true;
                    ApplyCurrentFilters();
                    _statusMessage = "Restored filter state from localStorage.";
                }
            }
        }
        catch (JSException)
        {
            _hasSavedState = false;
            _savedStateJson = null;
            _statusMessage = "localStorage is not available in this environment.";
        }
        catch (InvalidOperationException)
        {
            _hasSavedState = false;
            _savedStateJson = null;
            _statusMessage = "JavaScript interop is not available yet.";
        }
        catch (JsonException)
        {
            _statusMessage = "Saved filter JSON could not be deserialized.";
        }

        if (triggerRender)
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private void ResetBuilder()
    {
        _activeFilters = new FilterGroup();
        _builderRenderKey++;
        _restorePresetSelectionPending = false;
        ApplyCurrentFilters();
        _statusMessage = "Reset the builder. The saved localStorage entry was left unchanged.";
    }

    private async Task ClearSavedStateAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("localStorage.removeItem", StorageKey);
            _hasSavedState = false;
            _savedStateJson = null;
            _restorePresetSelectionPending = false;
            _statusMessage = "Cleared the saved filter state from localStorage.";
        }
        catch (JSException)
        {
            _statusMessage = "localStorage is not available in this environment.";
        }
        catch (InvalidOperationException)
        {
            _statusMessage = "JavaScript interop is not available yet.";
        }
    }

    private async Task PersistCurrentFiltersAsync(string successMessage)
    {
        var json = JsonSerializer.Serialize(_activeFilters, JsonOptions);

        try
        {
            await JS.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            _hasSavedState = true;
            _savedStateJson = json;
            _statusMessage = successMessage;
        }
        catch (JSException)
        {
            _statusMessage = "localStorage is not available in this environment.";
        }
        catch (InvalidOperationException)
        {
            _statusMessage = "JavaScript interop is not available yet.";
        }
    }

    private void ApplyCurrentFilters()
    {
        _filteredOrders = _allOrders.ApplyFilters(_activeFilters).ToList();
    }

    private static FilterGroup NormalizeGroup(FilterGroup group)
    {
        var normalized = new FilterGroup
        {
            Id = string.IsNullOrWhiteSpace(group.Id) ? Guid.NewGuid().ToString() : group.Id,
            Logic = group.Logic
        };

        normalized.Conditions = group.Conditions
            .Select(NormalizeCondition)
            .ToList();

        normalized.NestedGroups = group.NestedGroups
            .Select(NormalizeGroup)
            .ToList();

        return normalized;
    }

    private static FilterCondition NormalizeCondition(FilterCondition condition)
    {
        FieldShapes.TryGetValue(condition.Field, out var shape);

        return new FilterCondition
        {
            Id = string.IsNullOrWhiteSpace(condition.Id) ? Guid.NewGuid().ToString() : condition.Id,
            Field = condition.Field,
            Operator = condition.Operator,
            Value = NormalizeValue(condition.Value, shape),
            SecondaryValue = NormalizeValue(condition.SecondaryValue, shape)
        };
    }

    private static object? NormalizeValue(object? value, PersistedFieldShape shape)
    {
        if (value is not JsonElement element)
        {
            return value;
        }

        if (element.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return null;
        }

        if (shape.IsNumeric)
        {
            return TryReadDecimal(element, out var decimalValue)
                ? decimalValue
                : element.ToString();
        }

        if (shape.IsBoolean)
        {
            return TryReadBoolean(element, out var boolValue)
                ? boolValue
                : element.ToString();
        }

        if (shape.IsMultiSelect)
        {
            return element.ValueKind == JsonValueKind.Array
                ? element.EnumerateArray().Select(item => item.ToString()).ToList()
                : new List<string> { element.ToString() };
        }

        return element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : element.ToString();
    }

    private static bool TryReadDecimal(JsonElement element, out decimal value)
    {
        if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out value))
        {
            return true;
        }

        return decimal.TryParse(element.ToString(), out value);
    }

    private static bool TryReadBoolean(JsonElement element, out bool value)
    {
        if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            value = element.GetBoolean();
            return true;
        }

        return bool.TryParse(element.ToString(), out value);
    }

    private static string FormatJson(string json)
    {
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document.RootElement, JsonOptions);
    }

    private static FilterGroup CloneGroup(FilterGroup source)
    {
        return new FilterGroup
        {
            Id = source.Id,
            Logic = source.Logic,
            Conditions = source.Conditions.Select(CloneCondition).ToList(),
            NestedGroups = source.NestedGroups.Select(CloneGroup).ToList()
        };
    }

    private static FilterCondition CloneCondition(FilterCondition source)
    {
        return new FilterCondition
        {
            Id = source.Id,
            Field = source.Field,
            Operator = source.Operator,
            Value = source.Value,
            SecondaryValue = source.SecondaryValue
        };
    }

    private static FilterGroup GetPendingFilters() => new()
    {
        Conditions = [new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Pending" }]
    };

    private static FilterGroup GetHighValueFilters() => new()
    {
        Conditions = [new FilterCondition { Field = "Amount", Operator = FilterOperator.GreaterThan, Value = 1000m }]
    };

    private static BadgeVariant GetStatusVariant(string status) => status switch
    {
        "Delivered" => BadgeVariant.Default,
        "Shipped" => BadgeVariant.Secondary,
        "Processing" => BadgeVariant.Secondary,
        "Cancelled" => BadgeVariant.Destructive,
        _ => BadgeVariant.Outline
    };

    private static BadgeVariant GetPriorityVariant(string priority) => priority switch
    {
        "High" => BadgeVariant.Destructive,
        "Medium" => BadgeVariant.Secondary,
        _ => BadgeVariant.Outline
    };

    private readonly record struct PersistedFieldShape(FilterFieldType Type, FilterEditorType EditorType)
    {
        public bool IsBoolean => EffectiveEditorType == FilterEditorType.Boolean || Type == FilterFieldType.Boolean;
        public bool IsMultiSelect => EffectiveEditorType == FilterEditorType.MultiSelect || Type == FilterFieldType.MultiSelect;
        public bool IsNumeric => EffectiveEditorType is FilterEditorType.Numeric or FilterEditorType.Currency || Type == FilterFieldType.Number;

        private FilterEditorType EffectiveEditorType => EditorType == FilterEditorType.Auto
            ? Type switch
            {
                FilterFieldType.Number => FilterEditorType.Numeric,
                FilterFieldType.Boolean => FilterEditorType.Boolean,
                FilterFieldType.MultiSelect => FilterEditorType.MultiSelect,
                _ => FilterEditorType.Input
            }
            : EditorType;
    }

    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
