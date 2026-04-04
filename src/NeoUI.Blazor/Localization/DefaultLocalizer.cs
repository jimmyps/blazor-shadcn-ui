using System.Globalization;

namespace NeoUI.Blazor;

/// <summary>
/// Default <see cref="ILocalizer"/> with English defaults for all NeoUI component strings.
/// Subclass to integrate with <c>IStringLocalizer&lt;T&gt;</c> for multi-language support.
/// </summary>
/// <remarks>
/// <para>
/// All string keys follow the <c>ComponentName.PropertyName</c> dot-notation convention.
/// Format strings use standard <c>{0}</c>, <c>{1}</c>, … placeholders compatible with
/// <see cref="string.Format(IFormatProvider, string, object[])"/>.
/// </para>
/// </remarks>
public class DefaultLocalizer : ILocalizer
{
    private readonly Dictionary<string, string> _strings = new(StringComparer.Ordinal)
    {
        // ── Alert ────────────────────────────────────────────────────────────
        ["Alert.Dismiss"] = "Dismiss",

        // ── Breadcrumb ───────────────────────────────────────────────────────
        ["Breadcrumb.Breadcrumb"] = "breadcrumb",
        ["Breadcrumb.More"] = "More",

        // ── Calendar ─────────────────────────────────────────────────────────
        ["Calendar.GoToPreviousMonth"] = "Go to previous month",
        ["Calendar.GoToNextMonth"] = "Go to next month",
        ["Calendar.AriaLabel"] = "Calendar",
        ["Calendar.SelectMonth"] = "Select month",
        ["Calendar.SelectYear"] = "Select year",

        // ── Carousel ─────────────────────────────────────────────────────────
        ["Carousel.NextSlide"] = "Next slide",
        ["Carousel.PreviousSlide"] = "Previous slide",
        ["Carousel.GoToSlide"] = "Go to slide {0}",

        // ── Combobox ─────────────────────────────────────────────────────────
        ["Combobox.Placeholder"] = "Select an option...",
        ["Combobox.SearchPlaceholder"] = "Search...",
        ["Combobox.EmptyMessage"] = "No results found.",
        ["Combobox.Selected"] = "Selected",

        // ── Command ──────────────────────────────────────────────────────────
        ["Command.ListAriaLabel"] = "Command list",
        ["Command.MenuAriaLabel"] = "Command menu",

        // ── DataGrid ─────────────────────────────────────────────────────────
        ["DataGrid.Loading"] = "Loading...",
        ["DataGrid.NoResultsFound"] = "No results found",
        ["DataGrid.InitializingGrid"] = "Initializing grid...",

        // ── DataTable ────────────────────────────────────────────────────────
        ["DataTable.Loading"] = "Loading...",
        ["DataTable.NoResultsFound"] = "No results found",
        ["DataTable.Search"] = "Search...",
        ["DataTable.Columns"] = "Columns",
        ["DataTable.ToggleColumns"] = "Toggle columns",
        ["DataTable.Filter"] = "Filter",
        ["DataTable.FilterColumns"] = "Filter columns",
        ["DataTable.SelectRowsAriaLabel"] = "Select rows - click to see options",
        ["DataTable.SelectAllOnPage"] = "Select all on this page ({0} items)",
        ["DataTable.SelectAllItems"] = "Select all {0} items",
        ["DataTable.ClearSelection"] = "Clear selection",
        ["DataTable.SelectAllRows"] = "Select all rows",
        ["DataTable.SelectThisRow"] = "Select this row",

        // ── DataView ─────────────────────────────────────────────────────────
        ["DataView.SearchPlaceholder"] = "Search…",
        ["DataView.NoResultsFound"] = "No items to display.",
        ["DataView.Loading"] = "Loading…",
        ["DataView.LoadingMore"] = "Loading more…",
        ["DataView.LoadMore"] = "Load more",
        ["DataView.ListView"] = "List view",
        ["DataView.GridView"] = "Grid view",
        ["DataView.Sort"] = "Sort",

        // ── DatePicker ───────────────────────────────────────────────────────
        ["DatePicker.Placeholder"] = "Pick a date",

        // ── DateRangePicker ──────────────────────────────────────────────────
        ["DateRangePicker.Placeholder"] = "Pick a date range",
        ["DateRangePicker.Apply"] = "Apply",
        ["DateRangePicker.Clear"] = "Clear",
        ["DateRangePicker.Today"] = "Today",
        ["DateRangePicker.Yesterday"] = "Yesterday",
        ["DateRangePicker.Last7Days"] = "Last 7 days",
        ["DateRangePicker.Last30Days"] = "Last 30 days",
        ["DateRangePicker.ThisMonth"] = "This month",
        ["DateRangePicker.LastMonth"] = "Last month",
        ["DateRangePicker.ThisYear"] = "This year",
        ["DateRangePicker.Custom"] = "Custom",
        ["DateRangePicker.DaysSelected"] = "{0} day(s) selected",
        ["DateRangePicker.SelectEndDate"] = "Select end date",
        ["DateRangePicker.QuickSelect"] = "Quick Select",

        // ── Dialog / AlertDialog / Sheet / Drawer ────────────────────────────
        ["Dialog.Close"] = "Close",

        // ── Toast ────────────────────────────────────────────────────────────
        ["Toast.Close"] = "Close",

        // ── FileUpload ───────────────────────────────────────────────────────
        ["FileUpload.Accepted"] = "Accepted: {0}",
        ["FileUpload.MaxSize"] = "Max size: {0}",
        ["FileUpload.MaxFiles"] = "Max files: {0}",
        ["FileUpload.FilesCount"] = "Files ({0})",
        ["FileUpload.RemoveFile"] = "Remove {0}",

        // ── Filter ───────────────────────────────────────────────────────────
        ["Filter.RemoveFilter"] = "Remove filter",
        ["Filter.Operator.Equals"] = "is",
        ["Filter.Operator.NotEquals"] = "is not",
        ["Filter.Operator.Contains"] = "contains",
        ["Filter.Operator.NotContains"] = "does not contain",
        ["Filter.Operator.StartsWith"] = "starts with",
        ["Filter.Operator.EndsWith"] = "ends with",
        ["Filter.Operator.IsEmpty"] = "is empty",
        ["Filter.Operator.IsNotEmpty"] = "is not empty",
        ["Filter.Operator.GreaterThan"] = "is greater than",
        ["Filter.Operator.LessThan"] = "is less than",
        ["Filter.Operator.GreaterThanOrEqual"] = "is ≥",
        ["Filter.Operator.LessThanOrEqual"] = "is ≤",
        ["Filter.Operator.Between"] = "is between",
        ["Filter.Operator.NotBetween"] = "is not between",
        ["Filter.Operator.IsAnyOf"] = "is any of",
        ["Filter.Operator.IsNoneOf"] = "is none of",
        ["Filter.Operator.IsAllOf"] = "is all of",
        ["Filter.Operator.IsTrue"] = "is true",
        ["Filter.Operator.IsFalse"] = "is false",

        // ── MultiSelect ──────────────────────────────────────────────────────
        ["MultiSelect.Placeholder"] = "Select items...",
        ["MultiSelect.SearchPlaceholder"] = "Search...",
        ["MultiSelect.EmptyMessage"] = "No results found.",
        ["MultiSelect.SelectAll"] = "Select All",
        ["MultiSelect.Clear"] = "Clear",
        ["MultiSelect.Close"] = "Close",
        ["MultiSelect.ClearAllAriaLabel"] = "Clear all",

        // ── NumericInput ─────────────────────────────────────────────────────
        ["NumericInput.IncreaseValue"] = "Increase value",
        ["NumericInput.DecreaseValue"] = "Decrease value",

        // ── Pagination ───────────────────────────────────────────────────────
        ["Pagination.Previous"] = "Previous",
        ["Pagination.Next"] = "Next",
        ["Pagination.RowsPerPage"] = "Rows per page",
        ["Pagination.ShowingFormat"] = "Showing {0}–{1} of {2}",
        ["Pagination.PageFormat"] = "Page {0} of {1}",
        ["Pagination.NoItems"] = "No items",
        ["Pagination.GoToFirstPage"] = "Go to first page",
        ["Pagination.GoToLastPage"] = "Go to last page",

        // ── RichTextEditor ───────────────────────────────────────────────────
        ["RichTextEditor.Normal"] = "Normal",
        ["RichTextEditor.Heading1"] = "Heading 1",
        ["RichTextEditor.Heading2"] = "Heading 2",
        ["RichTextEditor.Heading3"] = "Heading 3",
        ["RichTextEditor.BoldTooltip"] = "Bold (Ctrl+B)",
        ["RichTextEditor.ItalicTooltip"] = "Italic (Ctrl+I)",
        ["RichTextEditor.UnderlineTooltip"] = "Underline (Ctrl+U)",
        ["RichTextEditor.StrikethroughTooltip"] = "Strikethrough",
        ["RichTextEditor.BulletListTooltip"] = "Bullet List",
        ["RichTextEditor.NumberedListTooltip"] = "Numbered List",
        ["RichTextEditor.InsertLinkTooltip"] = "Insert Link",
        ["RichTextEditor.BlockquoteTooltip"] = "Blockquote",
        ["RichTextEditor.CodeBlockTooltip"] = "Code Block",
        ["RichTextEditor.EditLinkTitle"] = "Edit Link",
        ["RichTextEditor.InsertLinkTitle"] = "Insert Link",
        ["RichTextEditor.UpdateLinkDescription"] = "Update the URL or remove the link.",
        ["RichTextEditor.InsertLinkDescription"] = "Enter the URL for the selected text.",
        ["RichTextEditor.UrlLabel"] = "URL",
        ["RichTextEditor.UrlPlaceholder"] = "https://example.com",
        ["RichTextEditor.RemoveLink"] = "Remove Link",
        ["RichTextEditor.Cancel"] = "Cancel",
        ["RichTextEditor.Update"] = "Update",
        ["RichTextEditor.Insert"] = "Insert",

        // ── Rating ───────────────────────────────────────────────────────────
        ["Rating.Rating"] = "Rating",

        // ── ResponsiveNav ────────────────────────────────────────────────────
        ["ResponsiveNav.ToggleMenu"] = "Toggle Menu",

        // ── Sidebar ──────────────────────────────────────────────────────────
        ["Sidebar.ToggleSidebar"] = "Toggle Sidebar",
        ["Sidebar.Pill.AriaLabel"] = "Navigation",
        ["Sidebar.Pill.ExpandButton"] = "Open navigation",

        // ── Sortable ─────────────────────────────────────────────────────────
        ["Sortable.DragHandle"] = "Drag handle",

        // ── ThemeSwitcher ────────────────────────────────────────────────────
        ["ThemeSwitcher.ChangeTheme"] = "Change theme",
        ["ThemeSwitcher.Settings"] = "Theme Settings",
        ["ThemeSwitcher.BaseColor"] = "Base Color",
        ["ThemeSwitcher.ThemeColor"] = "Theme Color",
        ["ThemeSwitcher.Style"] = "Style",
        ["ThemeSwitcher.BorderRadius"] = "Border Radius",
        ["ThemeSwitcher.Font"] = "Font",

        // ── ThemeSwitcher — BaseColor names ──────────────────────────────────
        ["ThemeSwitcher.BaseColor.Mist"] = "Mist",
        ["ThemeSwitcher.BaseColor.Mauve"] = "Mauve",
        ["ThemeSwitcher.BaseColor.Taupe"] = "Taupe",
        ["ThemeSwitcher.BaseColor.Olive"] = "Olive",

        // ── ThemeSwitcher — StyleVariant names ───────────────────────────────
        ["ThemeSwitcher.Style.Default"] = "Default",
        ["ThemeSwitcher.Style.Vega"] = "Vega",
        ["ThemeSwitcher.Style.Nova"] = "Nova",
        ["ThemeSwitcher.Style.Maia"] = "Maia",
        ["ThemeSwitcher.Style.Lyra"] = "Lyra",
        ["ThemeSwitcher.Style.Mira"] = "Mira",
        ["ThemeSwitcher.Style.Luma"] = "Luma",

        // ── ThemeSwitcher — RadiusPreset names ───────────────────────────────
        ["ThemeSwitcher.Radius.Default"] = "Default",
        ["ThemeSwitcher.Radius.None"]    = "None",
        ["ThemeSwitcher.Radius.Small"]   = "Small",
        ["ThemeSwitcher.Radius.Medium"]  = "Medium",
        ["ThemeSwitcher.Radius.Large"]   = "Large",

        // ── ThemeSwitcher — FontPreset names ─────────────────────────────────
        ["ThemeSwitcher.Font.System"] = "System",
        ["ThemeSwitcher.Font.Inter"] = "Inter",
        ["ThemeSwitcher.Font.Geist"] = "Geist",
        ["ThemeSwitcher.Font.CalSans"] = "Cal Sans",
        ["ThemeSwitcher.Font.DmSans"] = "DM Sans",
        ["ThemeSwitcher.Font.PlusJakarta"] = "Plus Jakarta",

        // ── ThemeSwitcher — MenuAccent names ─────────────────────────────────
        ["ThemeSwitcher.MenuAccent"] = "Menu Accent",
        ["ThemeSwitcher.MenuAccent.Subtle"] = "Subtle",
        ["ThemeSwitcher.MenuAccent.Bold"] = "Bold",

        // ── ThemeSwitcher — MenuColor names ──────────────────────────────────
        ["ThemeSwitcher.MenuColor"] = "Menu Color",
        ["ThemeSwitcher.MenuColor.Default"] = "Default",
        ["ThemeSwitcher.MenuColor.Inverted"] = "Inverted",
        ["ThemeSwitcher.MenuColor.DefaultTranslucent"] = "Translucent",
        ["ThemeSwitcher.MenuColor.InvertedTranslucent"] = "Dark Glass",

        // ── Timeline ─────────────────────────────────────────────────────────
        ["Timeline.AriaLabel"] = "Timeline",
        ["Timeline.NoItems"] = "No timeline items.",

        // ── TagInput ─────────────────────────────────────────────────────────
        ["TagInput.Placeholder"] = "Add tag…",
        ["TagInput.RemoveTag"] = "Remove {0}",
        ["TagInput.ClearAllTags"] = "Clear all tags",
        ["TagInput.TagSuggestions"] = "Tag suggestions",
    };

    /// <inheritdoc />
    public virtual string this[string key] =>
        _strings.TryGetValue(key, out var value) ? value : key;

    /// <inheritdoc />
    public virtual string this[string key, params object[] arguments]
    {
        get
        {
            if (_strings.TryGetValue(key, out var value))
            {
                try
                {
                    return string.Format(CultureInfo.CurrentCulture, value, arguments);
                }
                catch (FormatException)
                {
                    return value;
                }
            }

            return key;
        }
    }

    /// <summary>
    /// Sets or overrides the localized string for the specified key.
    /// Call this at application startup to customize default strings without subclassing.
    /// </summary>
    /// <param name="key">The localization key in dot notation.</param>
    /// <param name="value">The replacement string value.</param>
    public void Set(string key, string value) => _strings[key] = value;
}
