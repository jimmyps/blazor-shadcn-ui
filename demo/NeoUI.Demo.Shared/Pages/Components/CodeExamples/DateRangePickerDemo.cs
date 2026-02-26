namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DateRangePickerDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _dateRangePickerProps =
            [
                new("StartDate",        "DateOnly?",              null,    "The selected start date."),
                new("EndDate",          "DateOnly?",              null,    "The selected end date."),
                new("MinDate",          "DateOnly?",              null,    "The minimum selectable date."),
                new("MaxDate",          "DateOnly?",              null,    "The maximum selectable date."),
                new("ShowPresets",      "bool",                   "false", "When true, displays quick preset date range buttons."),
                new("StartDateLabel",   "string?",                null,    "Label text for the start date calendar."),
                new("EndDateLabel",     "string?",                null,    "Label text for the end date calendar."),
                new("IsDateDisabled",   "Func<DateOnly, bool>?",  null,    "A function that returns true for dates that should be disabled."),
                new("CaptionLayout",    "CalendarCaptionLayout",  "Buttons", "Controls the month/year navigation style. Options: Buttons, Dropdown."),
            ];

        private const string _defaultCode =
                """
                <DateRangePicker @bind-StartDate="startDate" @bind-EndDate="endDate" />
                """;

        private const string _presetsCode =
                """
                <DateRangePicker @bind-StartDate="startDate" @bind-EndDate="endDate" ShowPresets="true" />
                """;

        private const string _constraintsCode =
                """
                <DateRangePicker @bind-StartDate="startDate"
                                 @bind-EndDate="endDate"
                                 MinDate="DateOnly.FromDateTime(DateTime.Today.AddDays(-30))"
                                 MaxDate="DateOnly.FromDateTime(DateTime.Today)" />
                """;

        private const string _customLabelsCode =
                """
                <DateRangePicker @bind-StartDate="checkIn"
                                 @bind-EndDate="checkOut"
                                 StartDateLabel="Check-in"
                                 EndDateLabel="Check-out"
                                 MinDate="DateOnly.FromDateTime(DateTime.Today)"
                                 ShowPresets="true" />
                """;

        private const string _disableWeekendsCode =
                """
                <DateRangePicker @bind-StartDate="startDate"
                                 @bind-EndDate="endDate"
                                 IsDateDisabled="IsWeekend" />

                @code {
                    private bool IsWeekend(DateOnly date) =>
                        date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                }
                """;

        private const string _dropdownCode =
                """
                <DateRangePicker @bind-StartDate="startDate"
                                 @bind-EndDate="endDate"
                                 CaptionLayout="CalendarCaptionLayout.Dropdown"
                                 ShowPresets="true" />
                """;

        private const string _inFormCode =
                """
                <Field>
                    <FieldLabel>Travel Dates</FieldLabel>
                    <DateRangePicker @bind-StartDate="tripStart"
                                     @bind-EndDate="tripEnd"
                                     StartDateLabel="Departure"
                                     EndDateLabel="Return"
                                     MinDate="DateOnly.FromDateTime(DateTime.Today)"
                                     ShowPresets="true" />
                    <FieldDescription>Select your travel dates.</FieldDescription>
                </Field>
                """;
    }
}
