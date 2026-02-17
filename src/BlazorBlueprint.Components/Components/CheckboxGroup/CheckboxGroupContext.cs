namespace BlazorBlueprint.Components;

internal sealed class CheckboxGroupContext<TValue>
{
    public required IReadOnlyCollection<TValue> SelectedValues { get; init; }
    public required bool IsDisabled { get; init; }
    public required Func<TValue, bool, Task> ToggleItem { get; init; }
    public required Action<TValue> RegisterItem { get; init; }
    public required Action<TValue> UnregisterItem { get; init; }
}
