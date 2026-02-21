namespace BlazorUI.Demo.Shared;

/// <summary>Tier of a registry entry: styled component or headless primitive.</summary>
public enum ComponentTier { Component, Primitive }

/// <summary>
/// Metadata for a single component category.
/// Add new properties here (e.g. Icon, Description, SortOrder) to extend all
/// surfaces at once without touching <see cref="ComponentRegistryEntry"/> or any template.
/// </summary>
/// <param name="Id">Stable slug-style identifier — safe to use in code, URLs, and equality checks.</param>
/// <param name="DisplayName">Human-readable heading rendered in the UI.</param>
public sealed record ComponentCategoryInfo(string Id, string DisplayName);

/// <summary>
/// Central registry of all component categories in their intended display order.
/// Reference individual categories via the static fields (e.g. <c>ComponentCategories.Charts</c>)
/// rather than hard-coding strings, so every surface stays in sync automatically.
/// </summary>
public static class ComponentCategories
{
    /// <summary>Basic form controls: checkboxes, inputs, selects, etc.</summary>
    public static readonly ComponentCategoryInfo InputsForms    = new("inputs-forms",     "Inputs & Forms");
    /// <summary>Complex / specialised input components.</summary>
    public static readonly ComponentCategoryInfo AdvancedInputs = new("advanced-inputs",  "Advanced Inputs");
    /// <summary>Read-only display: badges, cards, tables, etc.</summary>
    public static readonly ComponentCategoryInfo DataDisplay    = new("data-display",     "Data Display");
    /// <summary>Navigation components: breadcrumbs, menus, pagination, etc.</summary>
    public static readonly ComponentCategoryInfo Navigation     = new("navigation",       "Navigation");
    /// <summary>Floating / layered UI: dialogs, popovers, drawers, etc.</summary>
    public static readonly ComponentCategoryInfo Overlays       = new("overlays",         "Overlays");
    /// <summary>Status and notification components: alerts, toasts, spinners.</summary>
    public static readonly ComponentCategoryInfo Feedback       = new("feedback",         "Feedback");
    /// <summary>Structural layout components: accordions, tabs, separators.</summary>
    public static readonly ComponentCategoryInfo Layout         = new("layout",           "Layout");
    /// <summary>Data visualisation components.</summary>
    public static readonly ComponentCategoryInfo Charts         = new("charts",           "Charts");
    /// <summary>Motion and animation components.</summary>
    public static readonly ComponentCategoryInfo Animation      = new("animation",        "Animation");

    /// <summary>All categories in intended display order.</summary>
    public static IReadOnlyList<ComponentCategoryInfo> All { get; } =
    [
        InputsForms, AdvancedInputs, DataDisplay, Navigation,
        Overlays, Feedback, Layout, Charts, Animation,
    ];
}

/// <summary>
/// Metadata for a single demo page in the component registry.
/// </summary>
public sealed record ComponentRegistryEntry(
    string Slug,
    string Title,
    string Description,
    string Icon,
    ComponentTier Tier,
    ComponentCategoryInfo Category,
    bool ExactNavMatch = false,
    bool IsSubPage = false)
{
    /// <summary>Relative route (no leading slash), e.g. "components/alert".</summary>
    public string Route => Tier == ComponentTier.Component
        ? $"components/{Slug}"
        : $"primitives/{Slug}";
}

/// <summary>A named group of registry entries sharing the same category.</summary>
public sealed record ComponentCategoryGroup(
    ComponentCategoryInfo Category,
    IReadOnlyList<ComponentRegistryEntry> Entries);

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

    /// <summary>All entries (components then primitives).</summary>
    public static IReadOnlyList<ComponentRegistryEntry> All => _all;

    /// <summary>All styled-component entries (includes sub-pages; useful for search/spotlight).</summary>
    public static IReadOnlyList<ComponentRegistryEntry> Components =>
        _all.Where(e => e.Tier == ComponentTier.Component).ToList().AsReadOnly();

    /// <summary>All headless-primitive entries.</summary>
    public static IReadOnlyList<ComponentRegistryEntry> Primitives =>
        _all.Where(e => e.Tier == ComponentTier.Primitive).ToList().AsReadOnly();

    /// <summary>
    /// Returns styled-component entries grouped by category in definition order.
    /// Sub-page entries (e.g. chart sub-pages, anchor-only links) are excluded.
    /// </summary>
    public static IReadOnlyList<ComponentCategoryGroup> GetGroupedComponents() =>
        _all
            .Where(e => e.Tier == ComponentTier.Component && !e.IsSubPage)
            .GroupBy(e => e.Category)
            .Select(g => new ComponentCategoryGroup(g.Key, g.ToList().AsReadOnly()))
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Returns headless-primitive entries grouped by category in definition order.
    /// Sub-page entries are excluded.
    /// </summary>
    public static IReadOnlyList<ComponentCategoryGroup> GetGroupedPrimitives() =>
        _all
            .Where(e => e.Tier == ComponentTier.Primitive && !e.IsSubPage)
            .GroupBy(e => e.Category)
            .Select(g => new ComponentCategoryGroup(g.Key, g.ToList().AsReadOnly()))
            .ToList()
            .AsReadOnly();

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
        // Aliases for brevity inside this method only.
        var C = ComponentTier.Component;
        var P = ComponentTier.Primitive;
        var IF = ComponentCategories.InputsForms;
        var AI = ComponentCategories.AdvancedInputs;
        var DD = ComponentCategories.DataDisplay;
        var NV = ComponentCategories.Navigation;
        var OV = ComponentCategories.Overlays;
        var FB = ComponentCategories.Feedback;
        var LY = ComponentCategories.Layout;
        var CH = ComponentCategories.Charts;
        var AN = ComponentCategories.Animation;

        var components = new List<ComponentRegistryEntry>
        {
            // ── Inputs & Forms ────────────────────────────────────────────
            new("checkbox",         "Checkbox",         "Binary selection control with indeterminate state",                             "square-check",          C, IF),
            new("field",            "Field",            "Combine labels, controls, and help text for accessible forms",                  "text",                  C, IF),
            new("input",            "Input",            "Text input fields with multiple types and sizes",                               "type",                  C, IF, ExactNavMatch: true),
            new("label",            "Label",            "Accessible labels for form controls",                                           "tag",                   C, IF),
            new("native-select",    "Native Select",    "Styled native HTML select dropdown",                                            "chevrons-up-down",      C, IF),
            new("numeric-input",    "Numeric Input",    "Number input with increment/decrement buttons and formatting",                  "hash",                  C, IF),
            new("radio-group",      "Radio Group",      "Mutually exclusive options with keyboard navigation",                           "circle-dot",            C, IF),
            new("rating",           "Rating",           "Star rating input with half-star precision and readonly mode",                  "star",                  C, IF),
            new("select",           "Select",           "Dropdown selection with groups and labels",                                     "list",                  C, IF),
            new("slider",           "Slider",           "Range input for numeric value selection",                                       "sliders-horizontal",    C, IF),
            new("switch",           "Switch",           "Toggle control for on/off states",                                              "toggle-left",           C, IF),
            new("textarea",         "Textarea",         "Multi-line text input with automatic content sizing",                           "text-cursor-input",     C, IF),
            new("toggle",           "Toggle",           "Pressable toggle buttons with variants",                                        "toggle-right",          C, IF, ExactNavMatch: true),
            new("toggle-group",     "Toggle Group",     "Single/multiple selection toggle groups",                                       "boxes",                 C, IF),

            // ── Advanced Inputs ───────────────────────────────────────────
            new("calendar",         "Calendar",         "Date selection grid with month navigation",                                     "calendar",              C, AI),
            new("color-picker",     "Color Picker",     "Color selection with hex, RGB, and HSL support",                               "palette",               C, AI),
            new("combobox",         "Combobox",         "Autocomplete input with searchable dropdown",                                   "square-chevron-down",   C, AI),
            new("currency-input",   "Currency Input",   "Formatted currency input with locale support",                                  "dollar-sign",           C, AI),
            new("date-picker",      "Date Picker",      "Date selection with calendar in popover",                                       "calendar-days",         C, AI),
            new("date-range-picker","Date Range Picker","Select date ranges with optional presets and two-calendar view",                "calendar-range",        C, AI),
            new("fileupload",       "File Upload",      "File upload with drag-and-drop, validation, and previews",                      "upload",                C, AI),
            new("input-group",      "Input Group",      "Enhanced inputs with icons, buttons, and addons",                               "blocks",                C, AI),
            new("input-otp",        "Input OTP",        "One-time password input with individual character slots",                       "shield-check",          C, AI),
            new("markdown-editor",  "Markdown Editor",  "Write/preview tabs editor with markdown syntax support",                        "file-text",             C, AI),
            new("masked-input",     "Masked Input",     "Text input with customizable format masks (phone, date, etc.)",                 "hash",                  C, AI),
            new("multi-select",     "Multi Select",     "Searchable multi-selection with tags and checkboxes",                           "list-checks",           C, AI),
            new("range-slider",     "Range Slider",     "Dual-handle slider for selecting value ranges",                                 "sliders-horizontal",    C, AI),
            new("rich-text-editor", "Rich Text Editor", "WYSIWYG editor with toolbar formatting and live preview",                       "type",                  C, AI),
            new("time-picker",      "Time Picker",      "Time selection with hour and minute controls",                                  "clock",                 C, AI),

            // ── Data Display ──────────────────────────────────────────────
            new("aspectratio",      "Aspect Ratio",     "Display content within a desired width/height ratio",                           "square",                C, DD),
            new("avatar",           "Avatar",           "User profile images with fallback initials and icons",                          "user",                  C, DD),
            new("badge",            "Badge",            "Labels for status, categories, and metadata",                                   "award",                 C, DD),
            new("card",             "Card",             "Container for grouped content with header and footer",                          "credit-card",           C, DD),
            new("datatable",        "Data Table",       "Powerful tables with sorting, filtering, pagination, and selection",            "table",                 C, DD),
            new("empty",            "Empty",            "Empty state displays with icon, title, and actions",                            "inbox",                 C, DD),
            new("grid",             "Grid",             "Advanced data grid with sorting, filtering, pagination, and state management",  "table-2",               C, DD),
            new("item",             "Item",             "Flexible list items with media, content, and actions",                          "circle",                C, DD),
            new("kbd",              "Kbd",              "Keyboard shortcut badges with semantic markup",                                  "keyboard",              C, DD),
            new("scroll-area",      "Scroll Area",      "Custom scrollbars for styled scroll regions",                                   "scroll-text",           C, DD),
            new("skeleton",         "Skeleton",         "Loading placeholders for content and images",                                   "box",                   C, DD),
            new("typography",       "Typography",       "Semantic text styling with consistent typography",                               "heading",               C, DD),

            // ── Navigation ────────────────────────────────────────────────
            new("breadcrumb",       "Breadcrumb",       "Hierarchical navigation with customizable separators",                          "chevron-right",         C, NV),
            new("command",          "Command",          "Command palette for quick actions and navigation",                              "terminal",              C, NV),
            new("menubar",          "Menubar",          "Desktop application-style horizontal menu bar",                                 "square-menu",           C, NV),
            new("navigation-menu",  "Navigation Menu",  "Horizontal navigation with dropdown panels",                                    "compass",               C, NV),
            new("pagination",       "Pagination",       "Page navigation with Previous/Next/Ellipsis",                                   "chevrons-left-right",   C, NV),
            new("sidebar",          "Sidebar",          "Responsive navigation sidebar with collapsible menus",                          "panel-left",            C, NV),

            // ── Overlays ──────────────────────────────────────────────────
            new("alert-dialog",     "Alert Dialog",     "Modal dialog for critical confirmations and warnings",                          "triangle-alert",        C, OV),
            new("context-menu",     "Context Menu",     "Right-click menus with actions and shortcuts",                                  "menu",                  C, OV),
            new("dialog",           "Dialog",           "Modal dialogs with backdrop and focus management",                              "square",                C, OV, ExactNavMatch: true),
            new("dialog-service",   "Dialog Service",   "Programmatic dialogs with async/await API for alerts and confirmations",        "square-check-big",      C, OV),
            new("drawer",           "Drawer",           "Slide-out panel with gesture controls and backdrop",                            "panel-bottom",          C, OV),
            new("dropdown-menu",    "Dropdown Menu",    "Context menus with items, separators, and shortcuts",                           "panel-top-open",        C, OV),
            new("hover-card",       "Hover Card",       "Rich preview cards on hover with delay control",                               "square-mouse-pointer",  C, OV),
            new("popover",          "Popover",          "Floating panels for additional content and actions",                            "message-square",        C, OV),
            new("sheet",            "Sheet",            "Side panels that slide in from viewport edges",                                  "panel-right",           C, OV),
            new("tooltip",          "Tooltip",          "Brief informational popups on hover or focus",                                   "message-circle",        C, OV),

            // ── Feedback ──────────────────────────────────────────────────
            new("alert",            "Alert",            "Displays callouts for important information or feedback",                        "circle-alert",          C, FB, ExactNavMatch: true),
            new("progress",         "Progress",         "Progress bars with ARIA support and animations",                                "loader",                C, FB),
            new("spinner",          "Spinner",          "Loading indicators in multiple sizes",                                           "loader-circle",         C, FB),
            new("toast",            "Toast",            "Temporary notifications with variants and actions",                              "bell",                  C, FB),

            // ── Layout ────────────────────────────────────────────────────
            new("accordion",        "Accordion",        "Collapsible content sections with smooth animations",                            "chevrons-down-up",      C, LY),
            new("button",           "Button",           "Interactive buttons with multiple variants and sizes",                          "pointer",               C, LY, ExactNavMatch: true),
            new("button-group",     "Button Group",     "Visually group related buttons with connected styling",                         "boxes",                 C, LY),
            new("button#linkbutton","Link Button",      "Semantic links styled as buttons for navigation",                              "link",                  C, LY, IsSubPage: true),
            new("collapsible",      "Collapsible",      "Expandable content area with trigger control",                                  "circle-chevron-down",   C, LY),
            new("resizable",        "Resizable",        "Split layouts with draggable handles",                                           "panel-left",            C, LY),
            new("separator",        "Separator",        "Visual dividers for content sections",                                           "minus",                 C, LY),
            new("tabs",             "Tabs",             "Tabbed interface for organizing related content",                                "folder",                C, LY),

            // ── Charts ────────────────────────────────────────────────────
            new("chart",            "Chart",            "Beautiful data visualizations with 8 chart types",                             "area-chart",            C, CH),
            // Chart sub-pages: searchable but excluded from main indexes and prev/next navigation
            new("chart/area",       "Area Chart",       "Smooth area charts with optional stacking and gradients",                       "chart-area",            C, CH, IsSubPage: true),
            new("chart/bar",        "Bar Chart",        "Vertical and horizontal bar charts with grouping and stacking",                 "chart-bar",             C, CH, IsSubPage: true),
            new("chart/line",       "Line Chart",       "Line charts with multiple series and configurable curves",                      "chart-line",            C, CH, IsSubPage: true),
            new("chart/pie",        "Pie Chart",        "Pie and donut charts with labels and legends",                                  "chart-pie",             C, CH, IsSubPage: true),
            new("chart/scatter",    "Scatter Chart",    "Scatter plots for correlation and distribution data",                           "scatter-chart",         C, CH, IsSubPage: true),
            new("chart/radar",      "Radar Chart",      "Spider/radar charts for multi-dimensional data comparison",                     "radar",                 C, CH, IsSubPage: true),
            new("chart/composed",   "Composed Chart",   "Combine multiple chart types in a single visualization",                        "layers",                C, CH, IsSubPage: true),

            // ── Animation ─────────────────────────────────────────────────
            new("carousel",         "Carousel",         "Slideshow component with touch gestures and animations",                        "images",                C, AN),
            new("motion",           "Motion",           "Declarative animation system with 20+ presets",                                 "zap",                   C, AN),
        };

        var primitives = new List<ComponentRegistryEntry>
        {
            // ── Inputs & Forms (Primitives) ───────────────────────────────
            new("checkbox",      "Checkbox",      "Unstyled checkbox with indeterminate state support",                "square-check",         P, IF),
            new("label",         "Label",         "Accessible label with associated control support",                 "tag",                  P, IF),
            new("radio-group",   "Radio Group",   "Headless radio group with keyboard navigation",                    "circle-dot",           P, IF),
            new("select",        "Select",        "Headless select with search and keyboard navigation",              "list",                 P, IF),
            new("switch",        "Switch",        "Headless toggle switch with ARIA checked state",                   "toggle-left",          P, IF),

            // ── Data Display (Primitives) ─────────────────────────────────
            new("table",         "Table",         "Unstyled accessible data table",                                   "table",                P, DD),

            // ── Navigation (Primitives) ───────────────────────────────────
            new("tabs",          "Tabs",          "Headless tabs with keyboard navigation",                           "folder",               P, NV),

            // ── Layout (Primitives) ───────────────────────────────────────
            new("accordion",     "Accordion",     "Headless accordion with keyboard navigation",                       "chevrons-down-up",     P, LY),
            new("collapsible",   "Collapsible",   "Unstyled collapsible content with trigger",                        "chevron-down",         P, LY),

            // ── Overlays (Primitives) ─────────────────────────────────────
            new("dialog",        "Dialog",        "Headless modal dialog with focus trap and ARIA",                   "square",               P, OV),
            new("dropdown-menu", "Dropdown Menu", "Unstyled floating menu with keyboard navigation",                  "chevron-down",         P, OV),
            new("hover-card",    "Hover Card",    "Unstyled hover card with delay and pointer events",                "square-mouse-pointer", P, OV),
            new("popover",       "Popover",       "Unstyled floating panel with positioning",                         "message-square",       P, OV),
            new("sheet",         "Sheet",         "Unstyled side panel with animation support",                       "panel-right",          P, OV),
            new("tooltip",       "Tooltip",       "Unstyled tooltip with delay and positioning",                      "message-circle",       P, OV),
        };

        return components.Concat(primitives).ToList();
    }
}
