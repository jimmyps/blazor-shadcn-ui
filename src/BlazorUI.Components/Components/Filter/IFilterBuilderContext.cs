namespace BlazorUI.Components.Filter;

/// <summary>
/// Interface for FilterBuilder context
/// </summary>
public interface IFilterBuilderContext
{
    void RegisterField(FilterFieldDefinition field);
    void RegisterPreset(FilterPresetDefinition preset);
}
