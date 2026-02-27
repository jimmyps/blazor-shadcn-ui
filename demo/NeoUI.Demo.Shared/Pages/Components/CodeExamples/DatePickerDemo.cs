namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class DatePickerDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _datePickerProps =
            [
                new("SelectedDate",    "DateOnly?",                null,    "The selected date. Supports two-way binding with @bind-SelectedDate."),
                new("Placeholder",     "string?",                  null,    "Text shown when no date is selected."),
                new("MinDate",         "DateOnly?",                null,    "The earliest selectable date."),
                new("MaxDate",         "DateOnly?",                null,    "The latest selectable date."),
                new("IsDateDisabled",  "Func<DateOnly, bool>?",    null,    "Function to determine whether a specific date should be disabled."),
                new("DateFormat",      "string?",                  null,    "Format string for displaying the selected date."),
                new("ShowIcon",        "bool",                     "true",  "Show calendar icon on the left of the trigger button."),
                new("ShowDropdownIcon","bool",                     "false", "Show chevron-down icon on the right of the trigger button."),
                new("CaptionLayout",   "CalendarCaptionLayout",    "Label", "Calendar header style. Options: Label, Dropdown."),
                new("Disabled",        "bool",                     "false", "When true, the picker cannot be opened."),
            ];

        private const string _defaultCode =
                """
                <DatePicker @bind-SelectedDate="selectedDate" />
                """;

        private const string _dropdownIconCode =
                """
                <DatePicker @bind-SelectedDate="selectedDate"
                           ShowIcon="false"
                           ShowDropdownIcon="true" />
                """;

        private const string _dateRangeCode =
                """
                <DateRangePicker @bind-StartDate="startDate"
                                @bind-EndDate="endDate"
                                Placeholder="Select date range" />
                """;

        private const string _constraintsCode =
                """
                <DatePicker @bind-SelectedDate="selectedDate"
                           MinDate="DateOnly.FromDateTime(DateTime.Today)"
                           MaxDate="DateOnly.FromDateTime(DateTime.Today.AddMonths(3))"
                           Placeholder="Pick a date (next 3 months)" />
                """;

        private const string _disableWeekendsCode =
                """
                <DatePicker @bind-SelectedDate="selectedDate"
                           IsDateDisabled="IsWeekend"
                           Placeholder="Pick a weekday" />

                @code {
                    private bool IsWeekend(DateOnly date) =>
                        date.DayOfWeek == DayOfWeek.Saturday ||
                        date.DayOfWeek == DayOfWeek.Sunday;
                }
                """;

        private const string _customFormatCode =
                """
                <DatePicker @bind-SelectedDate="selectedDate"
                           DateFormat="yyyy-MM-dd"
                           Placeholder="YYYY-MM-DD" />
                """;

        private const string _dropdownModeCode =
                """
                <DatePicker @bind-SelectedDate="selectedDate"
                           CaptionLayout="CalendarCaptionLayout.Dropdown"
                           Placeholder="Pick a date" />
                """;

        private const string _formCode =
                """
                <Field>
                    <FieldLabel>Date of Birth</FieldLabel>
                    <DatePicker @bind-SelectedDate="dateOfBirth"
                               MaxDate="DateOnly.FromDateTime(DateTime.Today)"
                               Placeholder="Select your birth date" />
                    <FieldDescription>We use this to verify your age.</FieldDescription>
                </Field>
                """;

        private const string _travelDatesCode =
                """
                <DateRangePicker @bind-StartDate="checkIn"
                                @bind-EndDate="checkOut"
                                StartDateLabel="Check-in"
                                EndDateLabel="Check-out"
                                MinDate="DateOnly.FromDateTime(DateTime.Today)"
                                Placeholder="Select travel dates" />
                """;

        private const string _dateTimeCode =
                """
                <div class="flex gap-2">
                    <DatePicker @bind-SelectedDate="appointmentDate"
                               ShowIcon="false"
                               ShowDropdownIcon="true"
                               Placeholder="Select date" />
                    <TimePicker @bind-SelectedTime="appointmentTime"
                               MinuteStep="15"
                               Placeholder="Select time"
                               ShowIcon="false" />
                </div>
                """;

        private const string _disabledCode =
                """
                <DatePicker Disabled="true" Placeholder="Disabled picker" />
                """;
    }
}
