namespace NeoUI.Blazor.Primitives;

/// <summary>Application defaults for components with floating interaction modes.</summary>
public sealed class InteractionOptions
{
    /// <summary>Default interaction mode. Blazor preserves existing behavior.</summary>
    public InteractionMode DefaultMode { get; set; } = InteractionMode.Blazor;
}

/// <summary>A scoped interaction policy supplied by InteractionModeProvider.</summary>
public sealed record InteractionPolicy(InteractionMode Mode);
