namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class CalendarDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _calendarProps =
            [
                new("SelectedDate",    "DateOnly?",             null,      "The currently selected date."),
                new("MinDate",         "DateOnly?",             null,      "The minimum selectable date."),
                new("MaxDate",         "DateOnly?",             null,      "The maximum selectable date."),
                new("InitialMonth",    "DateOnly?",             null,      "The month displayed when the calendar first renders."),
                new("IsDateDisabled",  "Func<DateOnly, bool>?", null,      "A function that returns true for dates that should be disabled."),
                new("CaptionLayout",   "CalendarCaptionLayout", "Buttons", "Controls month/year navigation style. Options: Buttons, Dropdown."),
                new("Class",           "string?",               null,      "Additional CSS classes appended to the root element."),
            ];

        private const string _defaultCode =
                """
                <Calendar @bind-SelectedDate="selectedDate" />
                """;

        private const string _dropdownCode =
                """
                <Calendar @bind-SelectedDate="selectedDate" CaptionLayout="CalendarCaptionLayout.Dropdown" />
                """;

        private const string _constraintsCode =
                """
                <Calendar @bind-SelectedDate="selectedDate"
                          MinDate="DateOnly.FromDateTime(DateTime.Today)"
                          MaxDate="DateOnly.FromDateTime(DateTime.Today.AddMonths(3))" />
                """;

        private const string _disableWeekendsCode =
                """
                <Calendar @bind-SelectedDate="selectedDate" IsDateDisabled="IsWeekend" />

                @code {
                    private bool IsWeekend(DateOnly date) =>
                        date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                }
                """;

        private const string _initialMonthCode =
                """
                <Calendar @bind-SelectedDate="selectedDate" InitialMonth="new DateOnly(2025, 12, 1)" />
                """;

        private const string _sideBySideCode =
                """
                <div class="flex gap-4">
                    <Calendar @bind-SelectedDate="rangeStart" />
                    <Calendar @bind-SelectedDate="rangeEnd" />
                </div>
                """;
    }
}
