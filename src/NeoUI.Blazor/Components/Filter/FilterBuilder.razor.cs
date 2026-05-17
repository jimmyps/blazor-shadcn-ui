using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace NeoUI.Blazor;

/// <summary>
/// A declarative, composable filter builder that renders as an inline canvas toolbar.
/// Conditions are applied immediately — no Apply/Cancel steps needed.
/// </summary>
/// <typeparam name="TData">The data model type being filtered.</typeparam>
public partial class FilterBuilder<TData> : ComponentBase, IFilterBuilderContext where TData : class
{
    private readonly List<FilterFieldDefinition> _fields = new();
    private readonly List<FilterPresetDefinition> _presets = new();
    private FilterGroup _rootGroup = new();
    private HashSet<string> _lastEmittedIds = new();
    private string? _activePresetName;

    // ── Parameters ──────────────────────────────────────────────────────────

    /// <summary>Current filter state. Use @bind-Filters for two-way binding.</summary>
    [Parameter] public FilterGroup? Filters { get; set; }

    /// <summary>Callback for two-way binding of the filter state.</summary>
    [Parameter] public EventCallback<FilterGroup> FiltersChanged { get; set; }

    /// <summary>Fires immediately whenever the active conditions change.</summary>
    [Parameter] public EventCallback<FilterGroup> OnFilterChange { get; set; }

    /// <summary>Child content slot for <see cref="FilterField"/> declarations.</summary>
    [Parameter] public RenderFragment? FilterFields { get; set; }

    /// <summary>Child content slot for <see cref="FilterPreset"/> declarations.</summary>
    [Parameter] public RenderFragment? FilterPresets { get; set; }

    /// <summary>Label shown on the add-filter button when no conditions are active.</summary>
    [Parameter] public string ButtonText { get; set; } = "Filter";

    /// <summary>
    /// How presets are rendered. <see cref="FilterPresetsVariant.Dropdown"/> (default) shows a Presets button.
    /// <see cref="FilterPresetsVariant.Tabs"/> shows horizontal tab buttons below the filter bar.
    /// </summary>
    [Parameter] public FilterPresetsVariant PresetsVariant { get; set; } = FilterPresetsVariant.Dropdown;

    /// <summary>Additional CSS classes for the wrapper element.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Maximum number of preset tabs to show inline when <see cref="PresetsVariant"/> is
    /// <see cref="FilterPresetsVariant.Tabs"/>. Presets beyond this count overflow into a
    /// "More ▾" dropdown appended as the last tab slot. The slot label updates dynamically
    /// to reflect the currently selected overflow preset.
    /// Has no effect when <see cref="PresetsVariant"/> is <see cref="FilterPresetsVariant.Dropdown"/>
    /// or when <see langword="null"/> (default — all presets shown as tabs).
    /// </summary>
    [Parameter] public int? MaxTabs { get; set; }

    [CascadingParameter(Name = "StyleVariant")]
    private StyleVariant _styleVariant { get; set; } = StyleVariant.Default;

    /// <summary>Size applied to every <see cref="FilterChip"/> rendered by this builder.</summary>
    [Parameter] public FilterChipSize ChipSize { get; set; } = FilterChipSize.Small;

    /// <summary>
    /// When true, the UI renders the root logic toggle (ALL of / ANY of)
    /// and exposes the [+ Add group] button, enabling predicate-tree / nested-group editing.
    /// Defaults to false to preserve the legacy flat-chip behaviour.
    /// </summary>
    [Parameter] public bool AllowGroups { get; set; } = false;

    // ── Computed ─────────────────────────────────────────────────────────────

    private string WrapperCssClass => ClassNames.cn(Class);

    /// <summary>
    /// Show the "Filter" text label only when: no conditions are active AND the tabs variant
    /// is not in use. When tabs are showing the text is always hidden so the button stays
    /// icon-only and the toolbar width never shifts as the user switches presets.
    /// </summary>
    private bool ShowButtonText => !_rootGroup.Conditions.Any() && !_rootGroup.NestedGroups.Any() && !(PresetsVariant == FilterPresetsVariant.Tabs && _presets.Any());

    // ── Lifecycle ────────────────────────────────────────────────────────────

    protected override void OnInitialized()
    {
        if (Filters != null)
        {
            _rootGroup = CloneGroup(Filters);
            _lastEmittedIds = _rootGroup.Conditions.Select(c => c.Id).ToHashSet();
        }
    }

    protected override void OnParametersSet()
    {
        if (Filters == null) return;
        var externalIds = Filters.Conditions.Select(c => c.Id).ToHashSet();
        if (!externalIds.SetEquals(_lastEmittedIds) || !AreEquivalentGroups(Filters, _rootGroup))
        {
            _rootGroup = CloneGroup(Filters);
            _lastEmittedIds = _rootGroup.Conditions.Select(c => c.Id).ToHashSet();
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        // Child FilterField/FilterPreset components call RegisterField/RegisterPreset during their
        // OnInitialized, which runs after the parent's first BuildRenderTree pass. A second render
        // is required so the field picker, preset tabs, and dropdown reflect the registered items.
        if (firstRender)
            StateHasChanged();
    }

    // ── IFilterBuilderContext ────────────────────────────────────────────────

    public void RegisterField(FilterFieldDefinition field)
    {
        if (!_fields.Any(f => f.Field == field.Field))
            _fields.Add(field);
    }

    public void RegisterPreset(FilterPresetDefinition preset)
    {
        if (!_presets.Any(p => p.Name == preset.Name))
        {
            _presets.Add(preset);
        }
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    private void AddCondition(FilterFieldDefinition field)
    {
        _activePresetName = null;
        var newCond = new FilterCondition
        {
            Field = field.Field,
            Operator = field.DefaultOperator ?? field.Operators.FirstOrDefault()
        };
        _rootGroup.Conditions.Add(newCond);
        _ = NotifyFiltersChanged();
        StateHasChanged();
    }

    /// <summary>
    /// Adds a condition with a pre-selected option value — called when the user clicks an
    /// option directly from the submenu shortcut in the field picker.
    /// </summary>
    private void AddConditionWithValue(FilterFieldDefinition field, string optionValue)
    {
        _activePresetName = null;
        object value = field.Type == FilterFieldType.MultiSelect
            ? new List<string> { optionValue }
            : optionValue;
        _rootGroup.Conditions.Add(new FilterCondition
        {
            Field = field.Field,
            Operator = field.DefaultOperator ?? field.Operators.FirstOrDefault(),
            Value = value
        });
        _ = NotifyFiltersChanged();
        StateHasChanged();
    }

    private void RemoveCondition(FilterCondition condition)
    {
        _activePresetName = null;
        _rootGroup.Conditions.Remove(condition);
        _ = NotifyFiltersChanged();
        StateHasChanged();
    }

    private async Task ClearAll()
    {
        _activePresetName = null;
        _rootGroup.Conditions.Clear();
        _rootGroup.NestedGroups.Clear();
        await NotifyFiltersChanged();
        StateHasChanged();
    }

    private void ApplyPreset(FilterPresetDefinition preset)
    {
        _activePresetName = preset.Name;
        _rootGroup.Logic = preset.Filters.Logic;
        _rootGroup.Conditions = preset.Filters.Conditions.Select(CloneCondition).ToList();
        _rootGroup.NestedGroups = preset.Filters.NestedGroups.Select(CloneGroup).ToList();
        _ = NotifyFiltersChanged();
        StateHasChanged();
    }

    private Task HandleConditionChanged(FilterCondition condition)
    {
        _ = NotifyFiltersChanged();
        return Task.CompletedTask;
    }

    private Task HandleGroupChanged()
    {
        _activePresetName = null;
        _ = NotifyFiltersChanged();
        return Task.CompletedTask;
    }

    private async Task NotifyFiltersChanged()
    {
        var updated = CloneGroup(_rootGroup);
        updated.Id = Filters?.Id ?? _rootGroup.Id;
        _lastEmittedIds = updated.Conditions.Select(c => c.Id).ToHashSet();

        if (FiltersChanged.HasDelegate)
            await FiltersChanged.InvokeAsync(updated);
        if (OnFilterChange.HasDelegate)
            await OnFilterChange.InvokeAsync(updated);
    }

    // ── Tab helpers ────────────────────────────────────────────────────────────

    /// <summary>Presets shown as tabs. When <see cref="MaxTabs"/> is set, limited to the first N presets.</summary>
    private IReadOnlyList<FilterPresetDefinition> VisiblePresets =>
        MaxTabs.HasValue ? _presets.Take(Math.Max(1, MaxTabs.Value)).ToList() : _presets;

    /// <summary>Presets that overflow into the More dropdown. Empty when <see cref="MaxTabs"/> is null.</summary>
    private IReadOnlyList<FilterPresetDefinition> OverflowPresets =>
        MaxTabs.HasValue ? _presets.Skip(Math.Max(1, MaxTabs.Value)).ToList() : Array.Empty<FilterPresetDefinition>();

    /// <summary>The overflow preset that is currently active, derived from <see cref="_activePresetName"/>.</summary>
    private FilterPresetDefinition? CurrentOverflowPreset =>
        _activePresetName != null ? OverflowPresets.FirstOrDefault(p => p.Name == _activePresetName) : null;

    /// <summary>CSS classes for the preset tab bar container, sized to match <see cref="ChipSize"/>.</summary>
    private string TabsContainerClass => ClassNames.cn(
        "inline-flex items-center rounded-md bg-muted p-0.5",
        _styleVariant.GetClasses("Tabs.List"),
        ChipSize switch
        {
            FilterChipSize.Small  => "h-8",
            FilterChipSize.Medium => "h-9",
            FilterChipSize.Large  => "h-10",
            _                     => "h-8",
        });

    /// <summary>Returns CSS classes for a preset tab button. Pass null for the "All" tab.</summary>
    private string GetTabClass(string? presetName) => BuildTabClass(presetName == _activePresetName);

    /// <summary>Returns CSS classes for the "More" overflow tab button.</summary>
    private string GetMoreTabClass() => BuildTabClass(CurrentOverflowPreset != null);

    /// <summary>Shared tab-button CSS builder to ensure visual consistency across all tab slots.</summary>
    private string BuildTabClass(bool isActive)
    {
        var itemHeight = ChipSize switch
        {
            FilterChipSize.Small  => "h-7",
            FilterChipSize.Medium => "h-8",
            FilterChipSize.Large  => "h-9",
            _                     => "h-7",
        };
        return ClassNames.cn(
            "inline-flex items-center gap-1 whitespace-nowrap rounded-sm px-2.5 text-sm font-medium transition-all",
            "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2",
            _styleVariant.GetClasses("Tabs.Trigger"),
            itemHeight,
            isActive
                ? "bg-background text-foreground shadow-sm"
                : "text-muted-foreground hover:text-foreground"
        );
    }

    // ── Cloning ───────────────────────────────────────────────────────────────

    private static FilterCondition CloneCondition(FilterCondition c) => new()
    {
        Id = c.Id,
        Field = c.Field,
        Operator = c.Operator,
        Value = c.Value,
        SecondaryValue = c.SecondaryValue
    };

    private static FilterGroup CloneGroup(FilterGroup group) => new()
    {
        Id = group.Id,
        Logic = group.Logic,
        Conditions = group.Conditions.Select(CloneCondition).ToList(),
        NestedGroups = group.NestedGroups.Select(CloneGroup).ToList()
    };

    public void RestoreMatchingPresetSelection(FilterGroup? filters = null)
    {
        if (PresetsVariant != FilterPresetsVariant.Tabs)
        {
            return;
        }

        _activePresetName = FindMatchingPresetName(filters ?? Filters);
        _ = InvokeAsync(StateHasChanged);
    }

    private string? FindMatchingPresetName(FilterGroup? filters)
    {
        if (filters == null || !_presets.Any())
        {
            return null;
        }

        return _presets.FirstOrDefault(preset => AreEquivalentGroups(preset.Filters, filters))?.Name;
    }

    private static bool AreEquivalentGroups(FilterGroup? left, FilterGroup? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left == null || right == null)
        {
            return false;
        }

        if (left.Logic != right.Logic)
        {
            return false;
        }

        if (left.Conditions.Count != right.Conditions.Count || left.NestedGroups.Count != right.NestedGroups.Count)
        {
            return false;
        }

        return BuildConditionSignatures(left.Conditions).SequenceEqual(BuildConditionSignatures(right.Conditions), StringComparer.Ordinal)
            && BuildGroupSignatures(left.NestedGroups).SequenceEqual(BuildGroupSignatures(right.NestedGroups), StringComparer.Ordinal);
    }

    private static IReadOnlyList<string> BuildConditionSignatures(IEnumerable<FilterCondition> conditions)
        => conditions.Select(BuildConditionSignature)
            .OrderBy(signature => signature, StringComparer.Ordinal)
            .ToList();

    private static IReadOnlyList<string> BuildGroupSignatures(IEnumerable<FilterGroup> groups)
        => groups.Select(BuildGroupSignature)
            .OrderBy(signature => signature, StringComparer.Ordinal)
            .ToList();

    private static string BuildGroupSignature(FilterGroup group)
    {
        var conditionSignatures = BuildConditionSignatures(group.Conditions)
            .OrderBy(signature => signature, StringComparer.Ordinal);

        var nestedGroupSignatures = BuildGroupSignatures(group.NestedGroups)
            .OrderBy(signature => signature, StringComparer.Ordinal);

        return $"{group.Logic}|[{string.Join(";", conditionSignatures)}]|[{string.Join(";", nestedGroupSignatures)}]";
    }

    private static string BuildConditionSignature(FilterCondition condition)
        => string.Join("|",
            condition.Field,
            condition.Operator,
            NormalizeValueSignature(condition.Value),
            NormalizeValueSignature(condition.SecondaryValue));

    private static string NormalizeValueSignature(object? value)
    {
        if (value == null)
        {
            return "<null>";
        }

        if (value is not string && value is IEnumerable<string> values)
        {
            return $"[{string.Join(",", values.OrderBy(item => item, StringComparer.OrdinalIgnoreCase).Select(item => item ?? string.Empty))}]";
        }

        if (TryConvertToDecimal(value, out var decimalValue))
        {
            return decimalValue.ToString(CultureInfo.InvariantCulture);
        }

        if (value is bool boolValue)
        {
            return boolValue ? "true" : "false";
        }

        if (value is DateOnly dateOnly)
        {
            return dateOnly.ToString("O", CultureInfo.InvariantCulture);
        }

        if (value is DateTime dateTime)
        {
            return dateTime.ToString("O", CultureInfo.InvariantCulture);
        }

        return value.ToString() ?? string.Empty;
    }

    private static bool TryConvertToDecimal(object value, out decimal result)
    {
        result = default;

        return value switch
        {
            byte byteValue => SetDecimal(byteValue, out result),
            sbyte sbyteValue => SetDecimal(sbyteValue, out result),
            short shortValue => SetDecimal(shortValue, out result),
            ushort ushortValue => SetDecimal(ushortValue, out result),
            int intValue => SetDecimal(intValue, out result),
            uint uintValue => SetDecimal(uintValue, out result),
            long longValue => SetDecimal(longValue, out result),
            ulong ulongValue => SetDecimal(ulongValue, out result),
            float floatValue => SetDecimal((decimal)floatValue, out result),
            double doubleValue => SetDecimal((decimal)doubleValue, out result),
            decimal decimalValue => SetDecimal(decimalValue, out result),
            string stringValue => decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                || decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.CurrentCulture, out result),
            _ => false
        };
    }

    private static bool SetDecimal(decimal value, out decimal result)
    {
        result = value;
        return true;
    }
}
