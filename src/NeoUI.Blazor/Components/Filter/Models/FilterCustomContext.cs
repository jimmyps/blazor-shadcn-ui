namespace NeoUI.Blazor;

/// <summary>
/// Context passed to a custom filter control RenderFragment.
/// Provides the live <see cref="FilterCondition"/> to read/write, plus a
/// <see cref="NotifyChanged"/> callback to propagate value changes back up
/// through FilterValue → FilterChip → FilterBuilder.
/// </summary>
public sealed class FilterCustomContext
{
    /// <summary>The live filter condition. Set <c>Condition.Value</c> then call <see cref="NotifyChanged"/>.</summary>
    public FilterCondition Condition { get; }

    /// <summary>Call after mutating <see cref="Condition.Value"/> to trigger re-render and parent binding update.</summary>
    public Func<Task> NotifyChanged { get; }

    public FilterCustomContext(FilterCondition condition, Func<Task> notifyChanged)
    {
        Condition = condition;
        NotifyChanged = notifyChanged;
    }
}
