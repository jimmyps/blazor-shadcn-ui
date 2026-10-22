using NeoUI.Blazor.Primitives;

namespace NeoUI.Blazor;

internal static class ComposedInteraction
{
    internal static bool IsBrowser(InteractionMode? explicitMode, InteractionPolicy? policy,
        InteractionOptions defaults, PopoverContext? popover = null) =>
        (explicitMode ?? popover?.InteractionMode ?? policy?.Mode ?? defaults.DefaultMode) == InteractionMode.Browser;
}
