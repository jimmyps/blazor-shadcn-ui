namespace BlazorUI.Components.Sidebar;

/// <summary>
/// Represents the variant style of the sidebar.
/// </summary>
public enum SidebarVariant
{
    /// <summary>
    /// Default sidebar that pushes content.
    /// </summary>
    Sidebar,

    /// <summary>
    /// Floating sidebar that overlays content.
    /// </summary>
    Floating,

    /// <summary>
    /// Inset sidebar with padding.
    /// </summary>
    Inset
}

/// <summary>
/// Represents which side the sidebar appears on.
/// </summary>
public enum SidebarSide
{
    /// <summary>
    /// Sidebar appears on the left side.
    /// </summary>
    Left,

    /// <summary>
    /// Sidebar appears on the right side.
    /// </summary>
    Right
}

/// <summary>
/// State container for the sidebar component.
/// </summary>
public class SidebarState
{
    /// <summary>
    /// Whether the sidebar is open on desktop.
    /// </summary>
    public bool Open { get; set; } = true;

    /// <summary>
    /// Whether the sidebar is open on mobile.
    /// </summary>
    public bool OpenMobile { get; set; }

    /// <summary>
    /// Whether the current viewport is mobile.
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// The variant/style of the sidebar.
    /// </summary>
    public SidebarVariant Variant { get; set; } = SidebarVariant.Sidebar;

    /// <summary>
    /// Which side the sidebar appears on.
    /// </summary>
    public SidebarSide Side { get; set; } = SidebarSide.Left;

    /// <summary>
    /// Whether menu items should automatically detect their active state based on current URL.
    /// </summary>
    public bool AutoDetectActive { get; set; } = false;

    /// <summary>
    /// The current navigation path (absolute path from NavigationManager.Uri).
    /// Updated when AutoDetectActive is enabled.
    /// </summary>
    public string CurrentPath { get; set; } = "/";
}

/// <summary>
/// Context for managing sidebar state across all sidebar components.
/// Provides state management, responsive behavior, and keyboard shortcuts.
/// </summary>
public class SidebarContext
{
    private SidebarState _state = new();

    /// <summary>
    /// Gets the current sidebar state.
    /// </summary>
    public SidebarState State => _state;

    /// <summary>
    /// Gets whether the sidebar is currently open (desktop or mobile based on viewport).
    /// </summary>
    public bool IsOpen => _state.IsMobile ? _state.OpenMobile : _state.Open;

    /// <summary>
    /// Gets whether the sidebar is open on desktop.
    /// </summary>
    public bool Open => _state.Open;

    /// <summary>
    /// Gets whether the sidebar is open on mobile.
    /// </summary>
    public bool OpenMobile => _state.OpenMobile;

    /// <summary>
    /// Gets whether the current viewport is mobile.
    /// </summary>
    public bool IsMobile => _state.IsMobile;

    /// <summary>
    /// Gets the sidebar variant.
    /// </summary>
    public SidebarVariant Variant => _state.Variant;

    /// <summary>
    /// Gets which side the sidebar appears on.
    /// </summary>
    public SidebarSide Side => _state.Side;

    /// <summary>
    /// Gets whether menu items should automatically detect their active state based on current URL.
    /// </summary>
    public bool AutoDetectActive => _state.AutoDetectActive;

    /// <summary>
    /// Gets the current navigation path.
    /// </summary>
    public string CurrentPath => _state.CurrentPath;

    /// <summary>
    /// Gets whether static rendering mode is enabled.
    /// When true, click events are handled via JS interop instead of direct C# handlers.
    /// </summary>
    public bool StaticRendering { get; private set; }

    /// <summary>
    /// Event raised when the sidebar state changes.
    /// </summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// Toggles the sidebar open/closed state.
    /// On mobile, toggles OpenMobile. On desktop, toggles Open.
    /// </summary>
    public void ToggleSidebar()
    {
        if (_state.IsMobile)
        {
            SetOpenMobile(!_state.OpenMobile);
        }
        else
        {
            SetOpen(!_state.Open);
        }
    }

    /// <summary>
    /// Sets the desktop open state.
    /// </summary>
    public void SetOpen(bool open)
    {
        if (_state.Open != open)
        {
            _state.Open = open;
            OnStateChanged();
        }
    }

    /// <summary>
    /// Sets the mobile open state.
    /// </summary>
    public void SetOpenMobile(bool open)
    {
        if (_state.OpenMobile != open)
        {
            _state.OpenMobile = open;
            OnStateChanged();
        }
    }

    /// <summary>
    /// Sets whether the viewport is mobile.
    /// Called by SidebarProvider via JS interop.
    /// </summary>
    public void SetIsMobile(bool isMobile)
    {
        if (_state.IsMobile != isMobile)
        {
            _state.IsMobile = isMobile;
            OnStateChanged();
        }
    }

    /// <summary>
    /// Sets the sidebar variant.
    /// </summary>
    public void SetVariant(SidebarVariant variant)
    {
        if (_state.Variant != variant)
        {
            _state.Variant = variant;
            OnStateChanged();
        }
    }

    /// <summary>
    /// Sets which side the sidebar appears on.
    /// </summary>
    public void SetSide(SidebarSide side)
    {
        if (_state.Side != side)
        {
            _state.Side = side;
            OnStateChanged();
        }
    }

    /// <summary>
    /// Sets whether menu items should automatically detect their active state.
    /// </summary>
    public void SetAutoDetectActive(bool autoDetectActive)
    {
        if (_state.AutoDetectActive != autoDetectActive)
        {
            _state.AutoDetectActive = autoDetectActive;
            OnStateChanged();
        }
    }

    /// <summary>
    /// Sets the current navigation path.
    /// Called by Sidebar component when AutoDetectActive is enabled.
    /// </summary>
    public void SetCurrentPath(string path)
    {
        if (_state.CurrentPath != path)
        {
            _state.CurrentPath = path;
            OnStateChanged();
        }
    }

    /// <summary>
    /// Initializes the state from values (typically from cookies or defaults).
    /// </summary>
    public void Initialize(bool? open = null, SidebarVariant? variant = null, SidebarSide? side = null, bool? autoDetectActive = null, string? currentPath = null, bool? staticRendering = null)
    {
        bool changed = false;

        if (open.HasValue && _state.Open != open.Value)
        {
            _state.Open = open.Value;
            changed = true;
        }

        if (variant.HasValue && _state.Variant != variant.Value)
        {
            _state.Variant = variant.Value;
            changed = true;
        }

        if (side.HasValue && _state.Side != side.Value)
        {
            _state.Side = side.Value;
            changed = true;
        }

        if (autoDetectActive.HasValue && _state.AutoDetectActive != autoDetectActive.Value)
        {
            _state.AutoDetectActive = autoDetectActive.Value;
            changed = true;
        }

        if (currentPath != null && _state.CurrentPath != currentPath)
        {
            _state.CurrentPath = currentPath;
            changed = true;
        }

        if (staticRendering.HasValue && StaticRendering != staticRendering.Value)
        {
            StaticRendering = staticRendering.Value;
            changed = true;
        }

        if (changed)
        {
            OnStateChanged();
        }
    }

    private void OnStateChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
