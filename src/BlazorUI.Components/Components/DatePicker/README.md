# DatePicker Component

Date selection components with Calendar in Popover pattern.

## Components

- **DatePicker**: Single date selection
- **DateRangePicker**: Date range selection (start and end dates)

## Features

- **Popover Integration**: Calendar appears in a popover overlay
- **Keyboard Accessible**: Full keyboard navigation support
- **Customizable**: Button variants, sizes, and styles
- **Date Constraints**: Min/max dates and custom disabled dates
- **Culture Support**: Localized date formatting
- **Form Friendly**: Two-way data binding support

## Usage

### Basic DatePicker

```razor
@using BlazorUI.Components.DatePicker

<DatePicker @bind-SelectedDate="selectedDate" />

<p>Selected: @selectedDate?.ToString("D")</p>

@code {
    private DateOnly? selectedDate;
}
```

### DatePicker with Constraints

```razor
<DatePicker 
    @bind-SelectedDate="selectedDate"
    MinDate="DateOnly.FromDateTime(DateTime.Today)"
    MaxDate="DateOnly.FromDateTime(DateTime.Today.AddMonths(3))"
    Placeholder="Select a date within 3 months" />

@code {
    private DateOnly? selectedDate;
}
```

### DatePicker with Custom Format

```razor
<DatePicker 
    @bind-SelectedDate="selectedDate"
    DateFormat="yyyy-MM-dd"
    Placeholder="YYYY-MM-DD" />

@code {
    private DateOnly? selectedDate;
}
```

### DatePicker with Disabled Dates

```razor
<DatePicker 
    @bind-SelectedDate="selectedDate"
    IsDateDisabled="IsWeekend"
    Placeholder="Select a weekday" />

@code {
    private DateOnly? selectedDate;

    private bool IsWeekend(DateOnly date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || 
               date.DayOfWeek == DayOfWeek.Sunday;
    }
}
```

### DateRangePicker

```razor
<DateRangePicker 
    @bind-StartDate="startDate"
    @bind-EndDate="endDate"
    Placeholder="Select date range" />

<p>From: @startDate?.ToString("D")</p>
<p>To: @endDate?.ToString("D")</p>

@code {
    private DateOnly? startDate;
    private DateOnly? endDate;
}
```

### DateRangePicker with Custom Labels

```razor
<DateRangePicker 
    @bind-StartDate="checkIn"
    @bind-EndDate="checkOut"
    StartDateLabel="Check-in"
    EndDateLabel="Check-out"
    Placeholder="Select travel dates" />

@code {
    private DateOnly? checkIn;
    private DateOnly? checkOut;
}
```

### DatePicker in a Form

```razor
@using BlazorUI.Components.Field

<Field>
    <FieldLabel>Date of Birth</FieldLabel>
    <DatePicker 
        @bind-SelectedDate="dateOfBirth"
        MaxDate="DateOnly.FromDateTime(DateTime.Today)"
        Placeholder="Select your birth date" />
    <FieldDescription>We use this to verify your age.</FieldDescription>
</Field>

@code {
    private DateOnly? dateOfBirth;
}
```

## API Reference

### DatePicker Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `SelectedDate` | `DateOnly?` | `null` | The selected date (two-way binding) |
| `MinDate` | `DateOnly?` | `null` | Minimum selectable date |
| `MaxDate` | `DateOnly?` | `null` | Maximum selectable date |
| `IsDateDisabled` | `Func<DateOnly, bool>?` | `null` | Function to disable specific dates |
| `Culture` | `CultureInfo?` | `CurrentCulture` | Culture for date formatting |
| `CaptionLayout` | `CalendarCaptionLayout` | `Buttons` | Calendar navigation style |
| `ButtonVariant` | `ButtonVariant` | `Outline` | Button style variant |
| `ButtonSize` | `ButtonSize` | `Default` | Button size |
| `ShowIcon` | `bool` | `true` | Whether to show calendar icon |
| `Placeholder` | `string` | `"Pick a date"` | Placeholder text |
| `DateFormat` | `string?` | `null` | Custom date format string |
| `Disabled` | `bool` | `false` | Whether disabled |
| `Align` | `PopoverAlign` | `Start` | Popover alignment |
| `Class` | `string?` | `null` | Additional button CSS classes |
| `CalendarClass` | `string?` | `null` | Additional calendar CSS classes |
| `AriaLabel` | `string?` | `null` | ARIA label for accessibility |

### DateRangePicker Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `StartDate` | `DateOnly?` | `null` | Start date of range (two-way binding) |
| `EndDate` | `DateOnly?` | `null` | End date of range (two-way binding) |
| `MinDate` | `DateOnly?` | `null` | Minimum selectable date |
| `MaxDate` | `DateOnly?` | `null` | Maximum selectable date |
| `IsDateDisabled` | `Func<DateOnly, bool>?` | `null` | Function to disable specific dates |
| `Culture` | `CultureInfo?` | `CurrentCulture` | Culture for date formatting |
| `CaptionLayout` | `CalendarCaptionLayout` | `Buttons` | Calendar navigation style |
| `ButtonVariant` | `ButtonVariant` | `Outline` | Button style variant |
| `ButtonSize` | `ButtonSize` | `Default` | Button size |
| `ShowIcon` | `bool` | `true` | Whether to show calendar icon |
| `Placeholder` | `string` | `"Pick a date range"` | Placeholder text |
| `DateFormat` | `string?` | `null` | Custom date format string |
| `Disabled` | `bool` | `false` | Whether disabled |
| `Align` | `PopoverAlign` | `Start` | Popover alignment |
| `StartDateLabel` | `string?` | `"From"` | Label for start date |
| `EndDateLabel` | `string?` | `"To"` | Label for end date |
| `Class` | `string?` | `null` | Additional button CSS classes |
| `CalendarClass` | `string?` | `null` | Additional calendar CSS classes |
| `AriaLabel` | `string?` | `null` | ARIA label for accessibility |

## Accessibility

- Full keyboard navigation support via Calendar component
- ARIA labels for screen readers
- Focus management within popover
- Disabled state properly communicated

## See Also

- [Calendar Component](../Calendar/README.md) - The underlying calendar used by DatePicker
- [Popover Component](../Popover/README.md) - The popover container
- [Button Component](../Button/README.md) - The trigger button
