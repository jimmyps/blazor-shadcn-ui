namespace NeoUI.Blazor.Primitives;

/// <summary>Supported control families for application interaction defaults.</summary>
public enum InteractionFamily
{
    /// <summary>Popover, HoverCard and Tooltip shells.</summary>
    Floating,
    /// <summary>Menus, navigation menus, split buttons and pill navigation.</summary>
    Menus,
    /// <summary>Select, Combobox, MultiSelect and date/time pickers, including their internal search UI.</summary>
    Selection,
    /// <summary>Text and structured inputs, checkboxes, radio groups, switches, toggles, sliders and editors.</summary>
    Inputs,
    /// <summary>Accordion, Collapsible and Tabs.</summary>
    Disclosures
}

/// <summary>Application defaults for supported controls. Configure before mounting components.</summary>
public sealed class InteractionOptions
{
    /// <summary>Fallback for families without an override. Blazor preserves existing behavior.</summary>
    public InteractionMode DefaultMode { get; set; } = InteractionMode.Blazor;
    /// <summary>Optional default for floating shells.</summary>
    public InteractionMode? Floating { get; set; }
    /// <summary>Optional default for menus and navigation.</summary>
    public InteractionMode? Menus { get; set; }
    /// <summary>Optional default for selection controls and their composed interaction UI.</summary>
    public InteractionMode? Selection { get; set; }
    /// <summary>Optional default for input fields and editors.</summary>
    public InteractionMode? Inputs { get; set; }
    /// <summary>Optional default for disclosures and tabs.</summary>
    public InteractionMode? Disclosures { get; set; }

    /// <summary>Gets the configured family override, or null to inherit the global default.</summary>
    public InteractionMode? GetFamilyMode(InteractionFamily family) => family switch
    {
        InteractionFamily.Floating => Floating,
        InteractionFamily.Menus => Menus,
        InteractionFamily.Selection => Selection,
        InteractionFamily.Inputs => Inputs,
        InteractionFamily.Disclosures => Disclosures,
        _ => throw new ArgumentOutOfRangeException(nameof(family), family, null)
    };

    /// <summary>Resolves explicit, scoped, family and global policy. An enclosing composed mode
    /// is a compatibility fallback when neither scoped nor family policy supplies a mode.</summary>
    public InteractionMode Resolve(InteractionMode? explicitMode, InteractionPolicy? policy,
        InteractionFamily family, InteractionMode? composedMode = null) =>
        explicitMode ?? policy?.GetMode(family) ?? GetFamilyMode(family) ?? composedMode ?? DefaultMode;
}

/// <summary>A scoped interaction policy supplied by InteractionModeProvider.</summary>
public sealed record InteractionPolicy(InteractionMode Mode, InteractionFamily? Family = null, InteractionPolicy? Parent = null)
{
    /// <summary>Gets the nearest scoped setting applicable to a family.</summary>
    public InteractionMode? GetMode(InteractionFamily family) =>
        Family is null || Family == family ? Mode : Parent?.GetMode(family);
}
