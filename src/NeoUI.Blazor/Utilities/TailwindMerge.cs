using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace BlazorUI.Components.Utilities;

/// <summary>
/// Provides Tailwind CSS class conflict resolution logic.
/// Intelligently merges Tailwind utility classes by identifying conflicts
/// and keeping only the last occurrence of conflicting utilities.
/// </summary>
public static class TailwindMerge
{
    private static readonly Dictionary<string, string> TailwindGroups = new()
    {
        // Padding
        ["p"] = "padding",
        ["px"] = "padding-x",
        ["py"] = "padding-y",
        ["pt"] = "padding-top",
        ["pr"] = "padding-right",
        ["pb"] = "padding-bottom",
        ["pl"] = "padding-left",

        // Margin
        ["m"] = "margin",
        ["mx"] = "margin-x",
        ["my"] = "margin-y",
        ["mt"] = "margin-top",
        ["mr"] = "margin-right",
        ["mb"] = "margin-bottom",
        ["ml"] = "margin-left",

        // Width
        ["w"] = "width",
        ["min-w"] = "min-width",
        ["max-w"] = "max-width",

        // Height
        ["h"] = "height",
        ["min-h"] = "min-height",
        ["max-h"] = "max-height",

        // Font Size
        ["text-xs"] = "font-size",
        ["text-sm"] = "font-size",
        ["text-base"] = "font-size",
        ["text-lg"] = "font-size",
        ["text-xl"] = "font-size",
        ["text-2xl"] = "font-size",
        ["text-3xl"] = "font-size",
        ["text-4xl"] = "font-size",
        ["text-5xl"] = "font-size",
        ["text-6xl"] = "font-size",
        ["text-7xl"] = "font-size",
        ["text-8xl"] = "font-size",
        ["text-9xl"] = "font-size",

        // Font Weight
        ["font-thin"] = "font-weight",
        ["font-extralight"] = "font-weight",
        ["font-light"] = "font-weight",
        ["font-normal"] = "font-weight",
        ["font-medium"] = "font-weight",
        ["font-semibold"] = "font-weight",
        ["font-bold"] = "font-weight",
        ["font-extrabold"] = "font-weight",
        ["font-black"] = "font-weight",

        // Display
        ["block"] = "display",
        ["inline-block"] = "display",
        ["inline"] = "display",
        ["flex"] = "display",
        ["inline-flex"] = "display",
        ["grid"] = "display",
        ["inline-grid"] = "display",
        ["hidden"] = "display",

        // Position
        ["static"] = "position",
        ["fixed"] = "position",
        ["absolute"] = "position",
        ["relative"] = "position",
        ["sticky"] = "position",

        // Gap
        ["gap"] = "gap",
        ["gap-x"] = "gap-x",
        ["gap-y"] = "gap-y",

        // Flex Direction
        ["flex-row"] = "flex-direction",
        ["flex-row-reverse"] = "flex-direction",
        ["flex-col"] = "flex-direction",
        ["flex-col-reverse"] = "flex-direction",

        // Justify Content
        ["justify-start"] = "justify-content",
        ["justify-end"] = "justify-content",
        ["justify-center"] = "justify-content",
        ["justify-between"] = "justify-content",
        ["justify-around"] = "justify-content",
        ["justify-evenly"] = "justify-content",

        // Align Items
        ["items-start"] = "align-items",
        ["items-end"] = "align-items",
        ["items-center"] = "align-items",
        ["items-baseline"] = "align-items",
        ["items-stretch"] = "align-items",

        // Border Radius
        ["rounded-none"] = "border-radius",
        ["rounded-sm"] = "border-radius",
        ["rounded"] = "border-radius",
        ["rounded-md"] = "border-radius",
        ["rounded-lg"] = "border-radius",
        ["rounded-xl"] = "border-radius",
        ["rounded-2xl"] = "border-radius",
        ["rounded-3xl"] = "border-radius",
        ["rounded-full"] = "border-radius",

        // Opacity
        ["opacity"] = "opacity",

        // Z-Index
        ["z"] = "z-index",
    };

    private static readonly Regex SpacingRegex = new(@"^(p|px|py|pt|pr|pb|pl|m|mx|my|mt|mr|mb|ml)-(\d+\.?\d*|auto)$", RegexOptions.Compiled);
    private static readonly Regex SizingRegex = new(@"^(w|h|min-w|min-h|max-w|max-h)-(.+)$", RegexOptions.Compiled);
    private static readonly Regex GapRegex = new(@"^(gap|gap-x|gap-y)-(\d+\.?\d*)$", RegexOptions.Compiled);
    
    // Color regexes updated to support opacity modifiers (e.g., bg-blue-500/80, text-red-500/50)
    // and arbitrary values (e.g., bg-[#fff], text-[rgb(255,0,0)])
    private static readonly Regex TextColorRegex = new(@"^text-([a-z]+(?:-\d+)?|\[[^\]]+\])(?:/\d+)?$", RegexOptions.Compiled);
    private static readonly Regex BgColorRegex = new(@"^bg-([a-z]+(?:-\d+)?|\[[^\]]+\])(?:/\d+)?$", RegexOptions.Compiled);
    private static readonly Regex BorderColorRegex = new(@"^border-([a-z]+(?:-\d+)?|\[[^\]]+\])(?:/\d+)?$", RegexOptions.Compiled);
    
    private static readonly Regex BorderWidthRegex = new(@"^border(-\d+)?$", RegexOptions.Compiled);
    private static readonly Regex OpacityRegex = new(@"^opacity-(\d+)$", RegexOptions.Compiled);
    private static readonly Regex ZIndexRegex = new(@"^z-(\d+|auto)$", RegexOptions.Compiled);
    private static readonly Regex GridColsRegex = new(@"^grid-cols-(\d+|none)$", RegexOptions.Compiled);
    private static readonly Regex GridRowsRegex = new(@"^grid-rows-(\d+|none)$", RegexOptions.Compiled);
    private static readonly Regex DurationRegex = new(@"^duration-(\d+)$", RegexOptions.Compiled);
    private static readonly Regex DelayRegex = new(@"^delay-(\d+)$", RegexOptions.Compiled);
    private static readonly Regex EasingRegex = new(@"^ease-(.+)$", RegexOptions.Compiled);
    
    // Animation-specific utilities (more specific patterns)
    private static readonly Regex AnimateDurationRegex = new(@"^animate-duration-(\d+)$", RegexOptions.Compiled);
    private static readonly Regex AnimateEaseRegex = new(@"^animate-ease-(.+)$", RegexOptions.Compiled);
    private static readonly Regex AnimateNameRegex = new(@"^animate-([a-z]+)$", RegexOptions.Compiled); // Matches animate-{word} without additional hyphens
    
    private static readonly Regex TranslateRegex = new(@"^-?(translate-x|translate-y|translate)-(.+)$", RegexOptions.Compiled);
    private static readonly Regex PositionRegex = new(@"^-?(top|right|bottom|left|inset|inset-x|inset-y)-(.+)$", RegexOptions.Compiled);
    private static readonly Regex ShadowRegex = new(@"^shadow(-(.+))?$", RegexOptions.Compiled);

    // Cache for utility group lookups to avoid repeated regex evaluation
    private static readonly ConcurrentDictionary<string, string?> _utilityGroupCache = new();

    // Regex to validate CSS class names - allows alphanumeric, hyphens, underscores, colons, slashes, brackets, dots, percentages, commas, spaces, and CSS combinator characters
    // This covers Tailwind classes like "w-1/2", "hover:bg-blue-500", "data-[state=open]:block", "text-[14px]", "[&>svg]:absolute", "transition-[color, box-shadow]", "bg-[rgb(255, 0, 0)]"
    private static readonly Regex ValidClassNameRegex = new(@"^[a-zA-Z0-9_\-:/.[\]()%!@#&>+~=, ]+$", RegexOptions.Compiled);

    /// <summary>
    /// Validates that a CSS class name contains only safe characters.
    /// Rejects classes that could be used for CSS injection attacks.
    /// </summary>
    private static bool IsValidClassName(string className)
    {
        if (string.IsNullOrWhiteSpace(className) || className.Length > 200)
            return false;

        // Check for potentially dangerous patterns
        if (className.Contains("expression", StringComparison.OrdinalIgnoreCase) ||
            className.Contains("javascript", StringComparison.OrdinalIgnoreCase) ||
            className.Contains("url(", StringComparison.OrdinalIgnoreCase) ||
            className.Contains("import", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return ValidClassNameRegex.IsMatch(className);
    }

    /// <summary>
    /// Merges an array of CSS class strings, resolving Tailwind utility conflicts.
    /// Later classes in the array take precedence over earlier ones when conflicts occur.
    /// /// </summary>
    /// <param name="classes">Array of class strings to merge</param>
    /// <returns>Merged class string with conflicts resolved</returns>
    public static string Merge(string[] classes)
    {
        if (classes == null || classes.Length == 0)
            return string.Empty;

        // Dictionary to track the last occurrence of each utility group
        var groupedClasses = new Dictionary<string, (string className, int index)>();
        var unGroupedClasses = new List<(string className, int index)>();

        for (int i = 0; i < classes.Length; i++)
        {
            var className = classes[i];
            if (string.IsNullOrWhiteSpace(className))
                continue;

            // Skip potentially dangerous class names
            if (!IsValidClassName(className))
                continue;

            var group = GetUtilityGroup(className);
            if (!string.IsNullOrEmpty(group))
            {
                // This class belongs to a known utility group
                // Store it with its index, replacing any previous occurrence from the same group
                groupedClasses[group] = (className, i);
            }
            else
            {
                // Unknown utility or custom class - preserve it
                unGroupedClasses.Add((className, i));
            }
        }

        // Combine grouped and ungrouped classes, maintaining relative order
        var allClasses = groupedClasses.Values
            .Concat(unGroupedClasses)
            .OrderBy(x => x.index)
            .Select(x => x.className);

        return string.Join(" ", allClasses);
    }

    /// <summary>
    /// Identifies which utility group a class belongs to.
    /// Returns null if the class doesn't match any known Tailwind utility pattern.
    /// Results are cached for performance.
    /// </summary>
    private static string? GetUtilityGroup(string className)
    {
        return _utilityGroupCache.GetOrAdd(className, ComputeUtilityGroup);
    }

    /// <summary>
    /// Ref struct to hold modifier extraction results without allocations.
    /// </summary>
    private ref struct ModifierExtraction
    {
        public ReadOnlySpan<char> Modifiers;
        public ReadOnlySpan<char> BaseClass;
    }

    /// <summary>
    /// Extracts modifiers and base class from a Tailwind class name.
    /// Uses ReadOnlySpan to minimize memory allocations.
    /// Supports multiple modifiers (e.g., "dark:hover:bg-blue-500").
    /// </summary>
    /// <param name="className">The class name as a span</param>
    /// <returns>ModifierExtraction struct containing modifiers and base class spans</returns>
    private static ModifierExtraction ExtractModifiers(ReadOnlySpan<char> className)
    {
        var lastColonIndex = className.LastIndexOf(':');
        
        if (lastColonIndex == -1)
            return new ModifierExtraction 
            { 
                Modifiers = ReadOnlySpan<char>.Empty, 
                BaseClass = className 
            };
        
        // Everything before last colon = modifiers (e.g., "dark:hover")
        // Everything after last colon = base class (e.g., "bg-blue-500")
        return new ModifierExtraction
        {
            Modifiers = className[..lastColonIndex],
            BaseClass = className[(lastColonIndex + 1)..]
        };
    }

    /// <summary>
    /// Combines modifiers with a base group, only allocating if modifiers exist.
    /// </summary>
    private static string CombineModifiersWithGroup(ReadOnlySpan<char> modifiers, string baseGroup)
    {
        return modifiers.IsEmpty ? baseGroup : $"{modifiers}:{baseGroup}";
    }

    /// <summary>
    /// Computes the utility group for a class name.
    /// This is the uncached implementation called by GetUtilityGroup.
    /// Supports modifier prefixes like hover:, dark:, focus:, etc.
    /// </summary>
    private static string? ComputeUtilityGroup(string className)
    {
        // Extract modifiers using spans (no allocations until we need the final string)
        var extraction = ExtractModifiers(className.AsSpan());
        var modifiers = extraction.Modifiers;
        var baseClassSpan = extraction.BaseClass;
        
        // Convert base class span to string for lookups
        var baseClass = baseClassSpan.ToString();
        
        // Check exact matches first (display, position, etc.)
        if (TailwindGroups.TryGetValue(baseClass, out var group))
            return CombineModifiersWithGroup(modifiers, group);

        // Check spacing utilities (padding, margin) - use Match directly to avoid double evaluation
        var spacingMatch = SpacingRegex.Match(baseClass);
        if (spacingMatch.Success)
        {
            var prefix = spacingMatch.Groups[1].Value;
            if (TailwindGroups.TryGetValue(prefix, out var spacingGroup))
                return CombineModifiersWithGroup(modifiers, spacingGroup);
            return null;
        }

        // Check sizing utilities (width, height, min/max)
        var sizingMatch = SizingRegex.Match(baseClass);
        if (sizingMatch.Success)
        {
            var prefix = sizingMatch.Groups[1].Value;
            if (TailwindGroups.TryGetValue(prefix, out var sizingGroup))
                return CombineModifiersWithGroup(modifiers, sizingGroup);
            return null;
        }

        // Check gap utilities
        var gapMatch = GapRegex.Match(baseClass);
        if (gapMatch.Success)
        {
            var prefix = gapMatch.Groups[1].Value;
            if (TailwindGroups.TryGetValue(prefix, out var gapGroup))
                return CombineModifiersWithGroup(modifiers, gapGroup);
            return null;
        }

        // Check text colors
        if (TextColorRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "text-color");

        // Check background colors
        if (BgColorRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "background-color");

        // Check border colors
        if (BorderColorRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "border-color");

        // Check border width
        if (BorderWidthRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "border-width");

        // Check opacity
        if (OpacityRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "opacity");

        // Check z-index
        if (ZIndexRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "z-index");

        // Check grid columns
        if (GridColsRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "grid-cols");

        // Check grid rows
        if (GridRowsRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "grid-rows");

        // Check animation duration
        if (DurationRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "animation-duration");

        // Check animation delay
        if (DelayRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "animation-delay");

        // Check animation duration (both duration-* and animate-duration-*)
        if (AnimateDurationRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "animate-duration");

        // Check animation easing (animate-ease-*)
        if (AnimateEaseRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "animate-ease");

        // Check animation timing function (ease-*)
        if (EasingRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "animation-timing-function");

        // Check named animations (animate-in, animate-out, animate-spin, etc.)
        // This must come AFTER the more specific animate-duration and animate-ease checks
        if (AnimateNameRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "animation-name");

        // Check translate utilities (translate-x, translate-y, translate)
        if (TranslateRegex.IsMatch(baseClass))
        {
            var match = TranslateRegex.Match(baseClass);
            var prefix = match.Groups[1].Value;
            return CombineModifiersWithGroup(modifiers, prefix); // Returns "translate-x", "translate-y", or "translate"
        }

        // Check position utilities (top, right, bottom, left, inset, inset-x, inset-y)
        if (PositionRegex.IsMatch(baseClass))
        {
            var match = PositionRegex.Match(baseClass);
            var prefix = match.Groups[1].Value;
            return CombineModifiersWithGroup(modifiers, prefix); // Returns "top", "right", "bottom", "left", "inset", "inset-x", or "inset-y"
        }

        // Check shadow utilities (shadow, shadow-sm, shadow-md, shadow-lg, shadow-xl, shadow-2xl, shadow-inner, shadow-none)
        if (ShadowRegex.IsMatch(baseClass))
            return CombineModifiersWithGroup(modifiers, "box-shadow");

        // Unknown utility
        return null;
    }
}
