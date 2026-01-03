using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Grid;

/// <summary>
/// Provides fine-grained control over AG Grid theme parameters.
/// This component must be used as a child of a Grid component and allows you to override
/// specific theme settings using AG Grid's withParams API.
/// </summary>
/// <example>
/// <code>
/// &lt;Grid TItem="Order" Items="@orders"&gt;
///     &lt;GridThemeParameters
///         AccentColor="#ff6b6b"
///         BorderRadius="8"
///         RowHeight="48"
///     /&gt;
///     &lt;Columns&gt;
///         &lt;GridColumn Field="OrderId" Header="Order ID" /&gt;
///     &lt;/Columns&gt;
/// &lt;/Grid&gt;
/// </code>
/// </example>
public partial class GridThemeParameters : ComponentBase
{
    [CascadingParameter]
    private object? Parent { get; set; }

    #region Spacing & Sizing Parameters

    /// <summary>
    /// Gets or sets the base spacing unit in pixels. Controls padding and margins throughout the grid.
    /// Default varies by density: Compact=3, Comfortable=4, Spacious=6
    /// </summary>
    /// <example>4</example>
    [Parameter]
    public int? Spacing { get; set; }

    /// <summary>
    /// Gets or sets the row height in pixels.
    /// Default varies by density: Compact=28, Comfortable=42, Spacious=56
    /// </summary>
    /// <example>42</example>
    [Parameter]
    public int? RowHeight { get; set; }

    /// <summary>
    /// Gets or sets the header height in pixels.
    /// Default varies by density: Compact=32, Comfortable=48, Spacious=64
    /// </summary>
    /// <example>48</example>
    [Parameter]
    public int? HeaderHeight { get; set; }

    /// <summary>
    /// Gets or sets the size of icons in pixels.
    /// Default varies by density: Compact=14, Comfortable=16, Spacious=20
    /// </summary>
    /// <example>16</example>
    [Parameter]
    public int? IconSize { get; set; }

    /// <summary>
    /// Gets or sets the height of input elements (filters, editors) in pixels.
    /// Default varies by density: Compact=28, Comfortable=32, Spacious=40
    /// </summary>
    /// <example>32</example>
    [Parameter]
    public int? InputHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of toggle buttons in pixels.
    /// </summary>
    /// <example>28</example>
    [Parameter]
    public int? ToggleButtonWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of toggle buttons in pixels.
    /// </summary>
    /// <example>28</example>
    [Parameter]
    public int? ToggleButtonHeight { get; set; }

    #endregion

    #region Color Parameters

    /// <summary>
    /// Gets or sets the primary accent color used for active states, selections, and focus indicators.
    /// Accepts CSS color values (hex, rgb, hsl, oklch, etc.) or CSS variables.
    /// Default for Shadcn theme: var(--primary)
    /// </summary>
    /// <example>"#2563eb" or "var(--primary)"</example>
    [Parameter]
    public string? AccentColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for cells and the grid body.
    /// Default for Shadcn theme: var(--background)
    /// </summary>
    /// <example>"#ffffff" or "var(--background)"</example>
    [Parameter]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the default text color for grid content.
    /// Default for Shadcn theme: var(--foreground)
    /// </summary>
    /// <example>"#000000" or "var(--foreground)"</example>
    [Parameter]
    public string? ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the color of borders and dividing lines.
    /// Default for Shadcn theme: var(--border)
    /// </summary>
    /// <example>"#e5e7eb" or "var(--border)"</example>
    [Parameter]
    public string? BorderColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for column headers.
    /// Default for Shadcn theme: var(--muted)
    /// </summary>
    /// <example>"#f9fafb" or "var(--muted)"</example>
    [Parameter]
    public string? HeaderBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the text color for column headers.
    /// Default for Shadcn theme: var(--foreground)
    /// </summary>
    /// <example>"#000000" or "var(--foreground)"</example>
    [Parameter]
    public string? HeaderForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color when hovering over a row.
    /// Default for Shadcn theme: color-mix(in srgb, var(--accent) 10%, transparent)
    /// </summary>
    /// <example>"rgba(37, 99, 235, 0.1)" or "color-mix(in srgb, var(--accent) 10%, transparent)"</example>
    [Parameter]
    public string? RowHoverColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for odd rows (used with Striped style).
    /// Default for Shadcn theme with Striped style: color-mix(in srgb, var(--muted) 30%, transparent)
    /// </summary>
    /// <example>"#f9fafb" or "color-mix(in srgb, var(--muted) 30%, transparent)"</example>
    [Parameter]
    public string? OddRowBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for selected rows.
    /// Default for Shadcn theme: color-mix(in srgb, var(--primary) 10%, transparent)
    /// </summary>
    /// <example>"rgba(37, 99, 235, 0.1)" or "color-mix(in srgb, var(--primary) 10%, transparent)"</example>
    [Parameter]
    public string? SelectedRowBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the border color for range selections.
    /// </summary>
    /// <example>"#2563eb"</example>
    [Parameter]
    public string? RangeSelectionBorderColor { get; set; }

    /// <summary>
    /// Gets or sets the text color for cell content.
    /// </summary>
    /// <example>"#000000"</example>
    [Parameter]
    public string? CellTextColor { get; set; }

    /// <summary>
    /// Gets or sets the color used to indicate validation errors.
    /// Default for Shadcn theme: var(--destructive)
    /// </summary>
    /// <example>"#dc2626" or "var(--destructive)"</example>
    [Parameter]
    public string? InvalidColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for modal overlays.
    /// </summary>
    /// <example>"rgba(0, 0, 0, 0.5)"</example>
    [Parameter]
    public string? ModalOverlayBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for UI chrome elements (panels, toolbars).
    /// </summary>
    /// <example>"#f9fafb"</example>
    [Parameter]
    public string? ChromeBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color for tooltips.
    /// Default for Shadcn theme: var(--popover)
    /// </summary>
    /// <example>"#ffffff" or "var(--popover)"</example>
    [Parameter]
    public string? TooltipBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the text color for tooltips.
    /// Default for Shadcn theme: var(--popover-foreground)
    /// </summary>
    /// <example>"#000000" or "var(--popover-foreground)"</example>
    [Parameter]
    public string? TooltipTextColor { get; set; }

    #endregion

    #region Typography Parameters

    /// <summary>
    /// Gets or sets the font family for the grid.
    /// Default for Shadcn theme: var(--font-sans)
    /// </summary>
    /// <example>"Inter, system-ui, sans-serif" or "var(--font-sans)"</example>
    [Parameter]
    public string? FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the base font size in pixels.
    /// Default varies by density: Compact=12, Comfortable=14, Spacious=16
    /// </summary>
    /// <example>14</example>
    [Parameter]
    public int? FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font size for column headers in pixels.
    /// </summary>
    /// <example>14</example>
    [Parameter]
    public int? HeaderFontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight for column headers.
    /// </summary>
    /// <example>600 or "bold"</example>
    [Parameter]
    public object? HeaderFontWeight { get; set; }

    #endregion

    #region Borders & Effects Parameters

    /// <summary>
    /// Gets or sets whether to show borders between cells and rows.
    /// Default varies by style: Default=true, Striped=true, Bordered=true, Minimal=false
    /// </summary>
    /// <example>true</example>
    [Parameter]
    public bool? Borders { get; set; }

    /// <summary>
    /// Gets or sets the border radius in pixels for UI elements.
    /// Default for Shadcn theme: 4
    /// </summary>
    /// <example>4</example>
    [Parameter]
    public int? BorderRadius { get; set; }

    /// <summary>
    /// Gets or sets whether to show a border around the entire grid wrapper.
    /// Default varies by style: Default=false, Striped=false, Bordered=true, Minimal=false
    /// </summary>
    /// <example>false</example>
    [Parameter]
    public bool? WrapperBorder { get; set; }

    /// <summary>
    /// Gets or sets the border radius in pixels for the grid wrapper.
    /// </summary>
    /// <example>8</example>
    [Parameter]
    public int? WrapperBorderRadius { get; set; }

    #endregion

    protected override void OnInitialized()
    {
        // Validate that this component is inside a Grid component
        if (Parent == null || !IsGridComponent(Parent))
        {
            throw new InvalidOperationException(
                "GridThemeParameters must be used as a child component of a Grid component.");
        }

        // Register with parent grid
        RegisterWithParent();
    }

    private bool IsGridComponent(object parent)
    {
        var parentType = parent.GetType();
        
        // Check if the parent type is exactly Grid<TItem> (closed generic type)
        // or if it inherits from a Grid<TItem> base class
        while (parentType != null)
        {
            if (parentType.IsGenericType)
            {
                var genericTypeDef = parentType.GetGenericTypeDefinition();
                // Check if the generic type definition's full name contains Grid
                // This is more specific than just checking the name
                if (genericTypeDef.FullName != null && 
                    genericTypeDef.FullName.Contains("BlazorUI.Components.Grid.Grid"))
                {
                    return true;
                }
            }
            parentType = parentType.BaseType;
        }
        return false;
    }

    private void RegisterWithParent()
    {
        // Use reflection to call RegisterThemeParameters on the parent Grid
        var parentType = Parent!.GetType();
        var method = parentType.GetMethod("RegisterThemeParameters", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (method != null)
        {
            method.Invoke(Parent, new object[] { this });
        }
    }

    /// <summary>
    /// Converts the parameters to a dictionary compatible with AG Grid's withParams API.
    /// Only includes parameters that have been explicitly set (non-null values).
    /// </summary>
    /// <returns>A dictionary with camelCase keys matching AG Grid's parameter names.</returns>
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();

        // Spacing & Sizing
        if (Spacing.HasValue) dict["spacing"] = Spacing.Value;
        if (RowHeight.HasValue) dict["rowHeight"] = RowHeight.Value;
        if (HeaderHeight.HasValue) dict["headerHeight"] = HeaderHeight.Value;
        if (IconSize.HasValue) dict["iconSize"] = IconSize.Value;
        if (InputHeight.HasValue) dict["inputHeight"] = InputHeight.Value;
        if (ToggleButtonWidth.HasValue) dict["toggleButtonWidth"] = ToggleButtonWidth.Value;
        if (ToggleButtonHeight.HasValue) dict["toggleButtonHeight"] = ToggleButtonHeight.Value;

        // Colors
        if (AccentColor != null) dict["accentColor"] = AccentColor;
        if (BackgroundColor != null) dict["backgroundColor"] = BackgroundColor;
        if (ForegroundColor != null) dict["foregroundColor"] = ForegroundColor;
        if (BorderColor != null) dict["borderColor"] = BorderColor;
        if (HeaderBackgroundColor != null) dict["headerBackgroundColor"] = HeaderBackgroundColor;
        if (HeaderForegroundColor != null) dict["headerForegroundColor"] = HeaderForegroundColor;
        if (RowHoverColor != null) dict["rowHoverColor"] = RowHoverColor;
        if (OddRowBackgroundColor != null) dict["oddRowBackgroundColor"] = OddRowBackgroundColor;
        if (SelectedRowBackgroundColor != null) dict["selectedRowBackgroundColor"] = SelectedRowBackgroundColor;
        if (RangeSelectionBorderColor != null) dict["rangeSelectionBorderColor"] = RangeSelectionBorderColor;
        if (CellTextColor != null) dict["cellTextColor"] = CellTextColor;
        if (InvalidColor != null) dict["invalidColor"] = InvalidColor;
        if (ModalOverlayBackgroundColor != null) dict["modalOverlayBackgroundColor"] = ModalOverlayBackgroundColor;
        if (ChromeBackgroundColor != null) dict["chromeBackgroundColor"] = ChromeBackgroundColor;
        if (TooltipBackgroundColor != null) dict["tooltipBackgroundColor"] = TooltipBackgroundColor;
        if (TooltipTextColor != null) dict["tooltipTextColor"] = TooltipTextColor;

        // Typography
        if (FontFamily != null) dict["fontFamily"] = FontFamily;
        if (FontSize.HasValue) dict["fontSize"] = FontSize.Value;
        if (HeaderFontSize.HasValue) dict["headerFontSize"] = HeaderFontSize.Value;
        if (HeaderFontWeight != null) dict["headerFontWeight"] = HeaderFontWeight;

        // Borders & Effects
        if (Borders.HasValue) dict["borders"] = Borders.Value;
        if (BorderRadius.HasValue) dict["borderRadius"] = BorderRadius.Value;
        if (WrapperBorder.HasValue) dict["wrapperBorder"] = WrapperBorder.Value;
        if (WrapperBorderRadius.HasValue) dict["wrapperBorderRadius"] = WrapperBorderRadius.Value;

        return dict;
    }
}
