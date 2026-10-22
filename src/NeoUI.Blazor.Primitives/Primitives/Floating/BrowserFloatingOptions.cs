namespace NeoUI.Blazor.Primitives;

/// <summary>Browser-owned behavior for a persistent <see cref="FloatingPortal"/>.</summary>
public sealed class BrowserFloatingOptions
{
    /// <summary>ID of the trigger element.</summary>
    public string TriggerId { get; set; } = "";
    /// <summary>ID of the floating content element.</summary>
    public string ContentId { get; set; } = "";
    /// <summary>Behavior: menu, select, popover, or submenu.</summary>
    public string Kind { get; set; } = "popover";
    /// <summary>Browser hover reveal delay in milliseconds.</summary>
    public int OpenDelay { get; set; }
    /// <summary>Browser hover dismissal delay in milliseconds.</summary>
    public int CloseDelay { get; set; }
    /// <summary>Context-menu viewport coordinates for programmatic opening.</summary>
    public double X { get; set; }
    /// <summary>Context-menu viewport coordinates for programmatic opening.</summary>
    public double Y { get; set; }
    /// <summary>Whether a context-menu shell locks body scrolling.</summary>
    public bool LockScroll { get; set; }
    /// <summary>Optional context-menu backdrop classes.</summary>
    public string? OverlayClass { get; set; }
    /// <summary>Context-menu viewport edge padding.</summary>
    public int ViewportPadding { get; set; } = 8;
    /// <summary>Root menu content ID for a submenu.</summary>
    public string? RootContentId { get; set; }
    /// <summary>Initial or explicitly requested open state.</summary>
    public bool Open { get; set; }
    /// <summary>Increment to issue a new open state command.</summary>
    public long OpenVersion { get; set; }
    /// <summary>Increment to issue an authoritative selection command.</summary>
    public long ValueVersion { get; set; }
    /// <summary>ID of the authoritative selected option.</summary>
    public string? SelectedItemId { get; set; }
    /// <summary>ID of the browser-owned selected text container.</summary>
    public string? ValueId { get; set; }
    /// <summary>Close on Escape.</summary>
    public bool CloseOnEscape { get; set; } = true;
    /// <summary>Close on an outside pointer press.</summary>
    public bool CloseOnClickOutside { get; set; } = true;
    /// <summary>Wrap keyboard navigation.</summary>
    public bool Loop { get; set; } = true;
    /// <summary>RTL keyboard navigation.</summary>
    public bool Rtl { get; set; }
    /// <summary>Send pointer coordinates when context-menu opening is observed.</summary>
    public bool NotifyContext { get; set; }
    /// <summary>Notify .NET after local open state changes.</summary>
    public bool NotifyOpen { get; set; }
    /// <summary>Notify .NET after Escape dismissal.</summary>
    public bool NotifyEscape { get; set; }
    /// <summary>Notify .NET after outside dismissal.</summary>
    public bool NotifyOutside { get; set; }
    /// <summary>Notify .NET when visible.</summary>
    public bool NotifyReady { get; set; }
}
