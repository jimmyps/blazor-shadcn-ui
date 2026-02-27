namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class TimePickerDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _timePickerProps =
            [
                new("SelectedTime", "TimeOnly?",  null,    "The currently selected time."),
                new("Use12Hour",    "bool",       "false", "When true, uses 12-hour format with AM/PM selector."),
                new("ShowSeconds",  "bool",       "false", "When true, shows a seconds selector."),
                new("MinuteStep",   "int",        "1",     "The interval between selectable minute values."),
                new("MinTime",      "TimeOnly?",  null,    "The minimum selectable time."),
                new("MaxTime",      "TimeOnly?",  null,    "The maximum selectable time."),
                new("Placeholder",  "string?",   null,    "Placeholder text shown when no time is selected."),
                new("Disabled",     "bool",      "false", "When true, the picker is non-interactive."),
                new("Class",        "string?",   null,    "Additional CSS classes appended to the root element."),
            ];

        private const string _defaultCode =
                """
                <TimePicker @bind-SelectedTime="selectedTime" />
                """;

        private const string _12HourCode =
                """
                <TimePicker @bind-SelectedTime="selectedTime" Use12Hour="true" />
                """;

        private const string _secondsCode =
                """
                <TimePicker @bind-SelectedTime="selectedTime" ShowSeconds="true" />
                """;

        private const string _15minCode =
                """
                <TimePicker @bind-SelectedTime="selectedTime" MinuteStep="15" />
                """;

        private const string _30minCode =
                """
                <TimePicker @bind-SelectedTime="selectedTime" MinuteStep="30" Use12Hour="true" />
                """;

        private const string _constraintsCode =
                """
                <TimePicker @bind-SelectedTime="selectedTime"
                            MinTime="@(new TimeOnly(9, 0, 0))"
                            MaxTime="@(new TimeOnly(17, 0, 0))"
                            Use12Hour="true"
                            Placeholder="Business hours only" />
                """;

        private const string _formCode =
                """
                <Field>
                    <FieldLabel>Appointment Time</FieldLabel>
                    <TimePicker @bind-SelectedTime="appointmentTime"
                                MinuteStep="15"
                                Use12Hour="true"
                                Placeholder="Select appointment time" />
                    <FieldDescription>Select your preferred appointment time.</FieldDescription>
                </Field>
                """;

        private const string _meetingCode =
                """
                <Field>
                    <FieldLabel>Meeting Start</FieldLabel>
                    <TimePicker @bind-SelectedTime="meetingStart" MinuteStep="15" Placeholder="Start time" />
                </Field>
                <Field>
                    <FieldLabel>Meeting End</FieldLabel>
                    <TimePicker @bind-SelectedTime="meetingEnd" MinuteStep="15" Placeholder="End time" />
                </Field>
                """;

        private const string _disabledCode =
                """
                <TimePicker Disabled="true" Placeholder="Disabled picker" />
                """;
    }
}
