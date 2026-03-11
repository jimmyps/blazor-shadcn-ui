namespace NeoUI.Blazor;

/// <summary>Interface for FilterBuilder context — used by FilterField and FilterPreset to self-register.</summary>
public interface IFilterBuilderContext
{
    void RegisterField(FilterFieldDefinition field);
    void RegisterPreset(FilterPresetDefinition preset);
}
