namespace BlazorUI.Demo.Shared;

/// <summary>Tier of a registry entry: styled component or headless primitive.</summary>
public enum ComponentTier { Component, Primitive }

/// <summary>
/// Metadata for a single demo page in the component registry.
/// </summary>
public sealed record ComponentRegistryEntry(
    string Slug,
    string Title,
    string Description,
    string Icon,
    ComponentTier Tier,
    bool ExactNavMatch = false,
    bool IsSubPage = false)
{
    /// <summary>Relative route (no leading slash), e.g. "components/alert".</summary>
    public string Route => Tier == ComponentTier.Component
        ? $"components/{Slug}"
        : $"primitives/{Slug}";
}

/// <summary>A single row in a DemoPropsTable.</summary>
/// <param name="Name">Parameter name.</param>
/// <param name="Type">C# type name.</param>
/// <param name="Default">Default value string, or null.</param>
/// <param name="Description">
/// Description of the parameter. Supports inline HTML (e.g. &lt;code&gt; snippets).
/// <strong>Only pass developer-authored, trusted strings — never user-provided content.</strong>
/// </param>
public record DemoPropRow(string Name, string Type, string? Default, string Description);

/// <summary>
/// Central registry of all demo pages. Drives the sidebar nav, component index,
/// spotlight command palette, and prev/next pagination.
/// </summary>
public static class ComponentRegistry
{
    private static readonly IReadOnlyList<ComponentRegistryEntry> _all;

    static ComponentRegistry() => _all = BuildAll().AsReadOnly();

    /// <summary>All entries (components then primitives, each in alphabetical order).</summary>
    public static IReadOnlyList<ComponentRegistryEntry> All => _all;

    /// <summary>Only styled-component entries.</summary>
    public static IReadOnlyList<ComponentRegistryEntry> Components =>
        _all.Where(e => e.Tier == ComponentTier.Component).ToList().AsReadOnly();

    /// <summary>Only headless-primitive entries.</summary>
    public static IReadOnlyList<ComponentRegistryEntry> Primitives =>
        _all.Where(e => e.Tier == ComponentTier.Primitive).ToList().AsReadOnly();

    /// <summary>
    /// Find an entry by its relative route (leading slash and fragment are stripped).
    /// Returns null when no entry matches.
    /// </summary>
    public static ComponentRegistryEntry? FindByRoute(string route)
    {
        var normalized = NormalizeRoute(route);
        return _all.FirstOrDefault(e =>
            e.Route.Equals(normalized, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get the previous and next navigable entry for the given route.
    /// Sub-page entries (e.g. anchor links) are excluded from navigation.
    /// Returns (null, null) when the route is not found or has no neighbours.
    /// </summary>
    public static (ComponentRegistryEntry? Prev, ComponentRegistryEntry? Next) GetNeighbors(string route)
    {
        var normalized = NormalizeRoute(route);
        var navigable = _all.Where(e => !e.IsSubPage).ToList();
        for (var i = 0; i < navigable.Count; i++)
        {
            if (!navigable[i].Route.Equals(normalized, StringComparison.OrdinalIgnoreCase))
                continue;
            var prev = i > 0 ? navigable[i - 1] : null;
            var next = i < navigable.Count - 1 ? navigable[i + 1] : null;
            return (prev, next);
        }
        return (null, null);
    }

    private static string NormalizeRoute(string route) =>
        route.TrimStart('/').Split('#')[0].ToLowerInvariant();

    // ──────────────────────────────────────────────────────────────────────
    private static List<ComponentRegistryEntry> BuildAll()
    {
        var components = new List<ComponentRegistryEntry>
        {
            new("accordion",        "Accordion",        "Collapsible content sections with smooth animations",                            "chevrons-down-up",      ComponentTier.Component),
            new("alert",            "Alert",            "Displays callouts for important information or feedback",                        "circle-alert",          ComponentTier.Component, ExactNavMatch: true),
            new("alert-dialog",     "Alert Dialog",     "Modal dialog for critical confirmations and warnings",                          "triangle-alert",        ComponentTier.Component),
            new("aspectratio",      "Aspect Ratio",     "Display content within a desired width/height ratio",                           "square",                ComponentTier.Component),
            new("avatar",           "Avatar",           "User profile images with fallback initials and icons",                          "user",                  ComponentTier.Component),
            new("badge",            "Badge",            "Labels for status, categories, and metadata",                                   "award",                 ComponentTier.Component),
            new("breadcrumb",       "Breadcrumb",       "Hierarchical navigation with customizable separators",                          "chevron-right",         ComponentTier.Component),
            new("button",           "Button",           "Interactive buttons with multiple variants and sizes",                          "pointer",               ComponentTier.Component, ExactNavMatch: true),
            new("button-group",     "Button Group",     "Visually group related buttons with connected styling",                         "boxes",                 ComponentTier.Component),
            new("button#linkbutton","Link Button",      "Semantic links styled as buttons for navigation",                              "link",                  ComponentTier.Component, IsSubPage: true),
            new("calendar",         "Calendar",         "Date selection grid with month navigation",                                     "calendar",              ComponentTier.Component),
            new("card",             "Card",             "Container for grouped content with header and footer",                          "credit-card",           ComponentTier.Component),
            new("carousel",         "Carousel",         "Slideshow component with touch gestures and animations",                        "images",                ComponentTier.Component),
            new("chart",            "Chart",            "Beautiful data visualizations with 8 chart types",                             "area-chart",            ComponentTier.Component),
            new("checkbox",         "Checkbox",         "Binary selection control with indeterminate state",                             "square-check",          ComponentTier.Component),
            new("collapsible",      "Collapsible",      "Expandable content area with trigger control",                                  "circle-chevron-down",   ComponentTier.Component),
            new("color-picker",     "Color Picker",     "Color selection with hex, RGB, and HSL support",                               "palette",               ComponentTier.Component),
            new("combobox",         "Combobox",         "Autocomplete input with searchable dropdown",                                   "square-chevron-down",   ComponentTier.Component),
            new("command",          "Command",          "Command palette for quick actions and navigation",                              "terminal",              ComponentTier.Component),
            new("context-menu",     "Context Menu",     "Right-click menus with actions and shortcuts",                                  "menu",                  ComponentTier.Component),
            new("currency-input",   "Currency Input",   "Formatted currency input with locale support",                                  "dollar-sign",           ComponentTier.Component),
            new("datatable",        "Data Table",       "Powerful tables with sorting, filtering, pagination, and selection",            "table",                 ComponentTier.Component),
            new("date-picker",      "Date Picker",      "Date selection with calendar in popover",                                       "calendar-days",         ComponentTier.Component),
            new("date-range-picker","Date Range Picker","Select date ranges with optional presets and two-calendar view",                "calendar-range",        ComponentTier.Component),
            new("dialog",           "Dialog",           "Modal dialogs with backdrop and focus management",                              "square",                ComponentTier.Component, ExactNavMatch: true),
            new("dialog-service",   "Dialog Service",   "Programmatic dialogs with async/await API for alerts and confirmations",        "square-check-big",      ComponentTier.Component),
            new("drawer",           "Drawer",           "Slide-out panel with gesture controls and backdrop",                            "panel-bottom",          ComponentTier.Component),
            new("dropdown-menu",    "Dropdown Menu",    "Context menus with items, separators, and shortcuts",                           "panel-top-open",        ComponentTier.Component),
            new("empty",            "Empty",            "Empty state displays with icon, title, and actions",                            "inbox",                 ComponentTier.Component),
            new("field",            "Field",            "Combine labels, controls, and help text for accessible forms",                  "text",                  ComponentTier.Component),
            new("fileupload",       "File Upload",      "File upload with drag-and-drop, validation, and previews",                      "upload",                ComponentTier.Component),
            new("grid",             "Grid",             "Advanced data grid with sorting, filtering, pagination, and state management",  "table-2",               ComponentTier.Component),
            new("hover-card",       "Hover Card",       "Rich preview cards on hover with delay control",                               "square-mouse-pointer",  ComponentTier.Component),
            new("input",            "Input",            "Text input fields with multiple types and sizes",                               "type",                  ComponentTier.Component, ExactNavMatch: true),
            new("input-group",      "Input Group",      "Enhanced inputs with icons, buttons, and addons",                               "blocks",                ComponentTier.Component),
            new("input-otp",        "Input OTP",        "One-time password input with individual character slots",                       "shield-check",          ComponentTier.Component),
            new("item",             "Item",             "Flexible list items with media, content, and actions",                          "circle",                ComponentTier.Component),
            new("kbd",              "Kbd",              "Keyboard shortcut badges with semantic markup",                                  "keyboard",              ComponentTier.Component),
            new("label",            "Label",            "Accessible labels for form controls",                                           "tag",                   ComponentTier.Component),
            new("markdown-editor",  "Markdown Editor",  "Write/preview tabs editor with markdown syntax support",                        "file-text",             ComponentTier.Component),
            new("masked-input",     "Masked Input",     "Text input with customizable format masks (phone, date, etc.)",                 "hash",                  ComponentTier.Component),
            new("menubar",          "Menubar",          "Desktop application-style horizontal menu bar",                                 "square-menu",           ComponentTier.Component),
            new("motion",           "Motion",           "Declarative animation system with 20+ presets",                                 "zap",                   ComponentTier.Component),
            new("multi-select",     "Multi Select",     "Searchable multi-selection with tags and checkboxes",                           "list-checks",           ComponentTier.Component),
            new("native-select",    "Native Select",    "Styled native HTML select dropdown",                                             "chevrons-up-down",      ComponentTier.Component),
            new("navigation-menu",  "Navigation Menu",  "Horizontal navigation with dropdown panels",                                    "compass",               ComponentTier.Component),
            new("numeric-input",    "Numeric Input",    "Number input with increment/decrement buttons and formatting",                  "hash",                  ComponentTier.Component),
            new("pagination",       "Pagination",       "Page navigation with Previous/Next/Ellipsis",                                   "chevrons-left-right",   ComponentTier.Component),
            new("popover",          "Popover",          "Floating panels for additional content and actions",                            "message-square",        ComponentTier.Component),
            new("progress",         "Progress",         "Progress bars with ARIA support and animations",                                "loader",                ComponentTier.Component),
            new("radio-group",      "Radio Group",      "Mutually exclusive options with keyboard navigation",                           "circle-dot",            ComponentTier.Component),
            new("range-slider",     "Range Slider",     "Dual-handle slider for selecting value ranges",                                  "sliders-horizontal",    ComponentTier.Component),
            new("rating",           "Rating",           "Star rating input with half-star precision and readonly mode",                  "star",                  ComponentTier.Component),
            new("resizable",        "Resizable",        "Split layouts with draggable handles",                                           "panel-left",            ComponentTier.Component),
            new("rich-text-editor", "Rich Text Editor", "WYSIWYG editor with toolbar formatting and live preview",                       "type",                  ComponentTier.Component),
            new("scroll-area",      "Scroll Area",      "Custom scrollbars for styled scroll regions",                                   "scroll-text",           ComponentTier.Component),
            new("select",           "Select",           "Dropdown selection with groups and labels",                                      "list",                  ComponentTier.Component),
            new("separator",        "Separator",        "Visual dividers for content sections",                                           "minus",                 ComponentTier.Component),
            new("sheet",            "Sheet",            "Side panels that slide in from viewport edges",                                  "panel-right",           ComponentTier.Component),
            new("sidebar",          "Sidebar",          "Responsive navigation sidebar with collapsible menus",                          "panel-left",            ComponentTier.Component),
            new("skeleton",         "Skeleton",         "Loading placeholders for content and images",                                   "box",                   ComponentTier.Component),
            new("slider",           "Slider",           "Range input for numeric value selection",                                        "sliders-horizontal",    ComponentTier.Component),
            new("spinner",          "Spinner",          "Loading indicators in multiple sizes",                                           "loader-circle",         ComponentTier.Component),
            new("switch",           "Switch",           "Toggle control for on/off states",                                               "toggle-left",           ComponentTier.Component),
            new("tabs",             "Tabs",             "Tabbed interface for organizing related content",                                "folder",                ComponentTier.Component),
            new("textarea",         "Textarea",         "Multi-line text input with automatic content sizing",                           "text-cursor-input",     ComponentTier.Component),
            new("time-picker",      "Time Picker",      "Time selection with hour and minute controls",                                   "clock",                 ComponentTier.Component),
            new("toast",            "Toast",            "Temporary notifications with variants and actions",                              "bell",                  ComponentTier.Component),
            new("toggle",           "Toggle",           "Pressable toggle buttons with variants",                                         "toggle-right",          ComponentTier.Component, ExactNavMatch: true),
            new("toggle-group",     "Toggle Group",     "Single/multiple selection toggle groups",                                        "boxes",                 ComponentTier.Component),
            new("tooltip",          "Tooltip",          "Brief informational popups on hover or focus",                                   "message-circle",        ComponentTier.Component),
            new("typography",       "Typography",       "Semantic text styling with consistent typography",                               "heading",               ComponentTier.Component),
        };

        var primitives = new List<ComponentRegistryEntry>
        {
            new("accordion",     "Accordion",     "Headless accordion with keyboard navigation",                       "chevrons-down-up",     ComponentTier.Primitive),
            new("checkbox",      "Checkbox",      "Unstyled checkbox with indeterminate state support",                "square-check",         ComponentTier.Primitive),
            new("collapsible",   "Collapsible",   "Unstyled collapsible content with trigger",                        "chevron-down",         ComponentTier.Primitive),
            new("dialog",        "Dialog",        "Headless modal dialog with focus trap and ARIA",                   "square",               ComponentTier.Primitive),
            new("dropdown-menu", "Dropdown Menu", "Unstyled floating menu with keyboard navigation",                  "chevron-down",         ComponentTier.Primitive),
            new("hover-card",    "Hover Card",    "Unstyled hover card with delay and pointer events",                "square-mouse-pointer", ComponentTier.Primitive),
            new("label",         "Label",         "Accessible label with associated control support",                 "tag",                  ComponentTier.Primitive),
            new("popover",       "Popover",       "Unstyled floating panel with positioning",                         "message-square",       ComponentTier.Primitive),
            new("radio-group",   "Radio Group",   "Headless radio group with keyboard navigation",                    "circle-dot",           ComponentTier.Primitive),
            new("select",        "Select",        "Headless select with search and keyboard navigation",              "list",                 ComponentTier.Primitive),
            new("sheet",         "Sheet",         "Unstyled side panel with animation support",                       "panel-right",          ComponentTier.Primitive),
            new("switch",        "Switch",        "Headless toggle switch with ARIA checked state",                   "toggle-left",          ComponentTier.Primitive),
            new("table",         "Table",         "Unstyled accessible data table",                                   "table",                ComponentTier.Primitive),
            new("tabs",          "Tabs",          "Headless tabs with keyboard navigation",                           "folder",               ComponentTier.Primitive),
            new("tooltip",       "Tooltip",       "Unstyled tooltip with delay and positioning",                      "message-circle",       ComponentTier.Primitive),
        };

        return components.Concat(primitives).ToList();
    }
}
