using NeoUI.Blazor.Primitives;

namespace NeoUI.Blazor;

internal static class ComposedInteraction
{
    internal static bool IsBrowser(InteractionMode? explicitMode, InteractionPolicy? policy,
        InteractionOptions defaults, PopoverContext? popover = null, InteractionFamily family = InteractionFamily.Inputs)
    {
        // Preserve existing composed exceptions when applications have not configured a family.
        // An Inputs override deliberately separates ordinary fields from their floating shell.
        var applicableScope = policy;
        while (applicableScope?.Family is not null && applicableScope.Family != family)
            applicableScope = applicableScope.Parent;
        if (defaults.GetFamilyMode(family) is null && applicableScope?.Family != family && popover is not null)
            return (explicitMode ?? popover.InteractionMode) == InteractionMode.Browser;
        return defaults.Resolve(explicitMode, policy, family, popover?.InteractionMode) == InteractionMode.Browser;
    }
}
