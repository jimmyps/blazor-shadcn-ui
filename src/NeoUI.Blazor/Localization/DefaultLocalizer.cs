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

        // ── Carousel ─────────────────────────────────────────────────────────
        ["Carousel.NextSlide"] = "Next slide",
        ["Carousel.PreviousSlide"] = "Previous slide",

        // ── Combobox ─────────────────────────────────────────────────────────
        ["Combobox.Placeholder"] = "Select an option...",
        ["Combobox.SearchPlaceholder"] = "Search...",
        ["Combobox.EmptyMessage"] = "No results found.",

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
        ["DataView.SearchPlaceholder"] = "Search...",
        ["DataView.NoResultsFound"] = "No results found",
        ["DataView.Loading"] = "Loading...",
        ["DataView.LoadingMore"] = "Loading more...",
        ["DataView.LoadMore"] = "Load more",
        ["DataView.ListView"] = "List view",
        ["DataView.GridView"] = "Grid view",
        ["DataView.Sort"] = "Sort",

        // ── DatePicker ───────────────────────────────────────────────────────
        ["DatePicker.Placeholder"] = "Pick a date",

        // ── DateRangePicker ──────────────────────────────────────────────────
        ["DateRangePicker.Placeholder"] = "Select date range",
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

        // ── MultiSelect ──────────────────────────────────────────────────────
        ["MultiSelect.Placeholder"] = "Select items...",
        ["MultiSelect.SearchPlaceholder"] = "Search...",
        ["MultiSelect.EmptyMessage"] = "No results found.",
        ["MultiSelect.SelectAll"] = "Select All",
        ["MultiSelect.Clear"] = "Clear",
        ["MultiSelect.Close"] = "Close",

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

        // ── Rating ───────────────────────────────────────────────────────────
        ["Rating.Rating"] = "Rating",

        // ── ResponsiveNav ────────────────────────────────────────────────────
        ["ResponsiveNav.ToggleMenu"] = "Toggle Menu",

        // ── Sidebar ──────────────────────────────────────────────────────────
        ["Sidebar.ToggleSidebar"] = "Toggle Sidebar",

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
