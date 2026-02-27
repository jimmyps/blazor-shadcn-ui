namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class MaskedInputDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _maskedInputProps =
            [
                new("Mask",               "string",   "—",      "Mask pattern. 0=digit, A=letter, *=any character. Other chars are literals."),
                new("Value",              "string?",  "null",   "Current value (raw digits/letters only). Use @bind-Value for two-way binding."),
                new("ValueChanged",       "EventCallback&lt;string?&gt;", "—", "Callback invoked when the value changes."),
                new("ShowMask",           "bool",     "true",   "Whether to show the mask template as placeholder text."),
                new("Placeholder",        "string?",  "null",   "Placeholder text when ShowMask is false."),
                new("Disabled",           "bool",     "false",  "Disables the input when true."),
                new("Readonly",           "bool",     "false",  "Makes the input read-only when true."),
                new("Required",           "bool",     "false",  "Marks the input as required for native form validation."),
                new("AriaInvalid",        "bool",     "false",  "Marks the input as invalid for screen readers."),
                new("ShowValidationError","bool",     "false",  "Shows browser native validation tooltip on invalid state."),
                new("Class",              "string?",  "null",   "Additional CSS classes appended to the input element."),
            ];

        private const string _phoneCode = """
                <MaskedInput Mask="@MaskedInput.Masks.PhoneUS" @bind-Value="phoneValue" />
                """;

        private const string _intlPhoneCode = """
                <MaskedInput Mask="+1 (000) 000-0000" @bind-Value="usPhoneValue" />
                <MaskedInput Mask="+44 0000 000000" @bind-Value="ukPhoneValue" />
                """;

        private const string _ssnCode = """
                <MaskedInput Mask="@MaskedInput.Masks.SSN" @bind-Value="ssnValue" />
                """;

        private const string _creditCardCode = """
                <MaskedInput Mask="@MaskedInput.Masks.CreditCard" @bind-Value="cardValue" />
                <MaskedInput Mask="00/00" @bind-Value="cardExpiry" Placeholder="MM/YY" ShowMask="false" />
                <MaskedInput Mask="000" @bind-Value="cardCvv" Placeholder="CVV" ShowMask="false" />
                """;

        private const string _dateCode = """
                <!-- US format -->
                <MaskedInput Mask="@MaskedInput.Masks.Date" @bind-Value="dateValue" />

                <!-- ISO format -->
                <MaskedInput Mask="0000-00-00" @bind-Value="isoDateValue" />
                """;

        private const string _timeCode = """
                <MaskedInput Mask="@MaskedInput.Masks.Time" @bind-Value="timeValue" />
                <MaskedInput Mask="00:00:00" @bind-Value="timeWithSecondsValue" />
                """;

        private const string _zipCode = """
                <MaskedInput Mask="@MaskedInput.Masks.ZIP" @bind-Value="zipValue" Placeholder="Enter ZIP" ShowMask="false" />
                """;

        private const string _zip4Code = """
                <MaskedInput Mask="@MaskedInput.Masks.ZIP4" @bind-Value="zipPlusFourValue" ShowMask="true" />
                """;

        private const string _productCodeMaskCode = """
                <MaskedInput Mask="AA-0000" @bind-Value="productCodeValue" Placeholder="Enter product code" ShowMask="true" />
                """;

        private const string _licensePlateMaskCode = """
                <MaskedInput Mask="000-AAA" @bind-Value="licensePlateValue" ShowMask="true" />
                """;

        private const string _disabledCode = """
                <MaskedInput Mask="@MaskedInput.Masks.PhoneUS" Value="5555551234" Disabled="true" ShowMask="true" />
                """;

        private const string _readonlyCode = """
                <MaskedInput Mask="@MaskedInput.Masks.SSN" Value="123456789" Readonly="true" ShowMask="true" />
                """;

        private const string _withoutMaskCode = """
                <MaskedInput Mask="@MaskedInput.Masks.PhoneUS"
                    @bind-Value="phoneValueNoMask"
                    Placeholder="(000) 000-0000"
                    ShowMask="false" />
                """;

        private const string _customCode = """
                <!-- Letters only (A = letter) -->
                <MaskedInput Mask="AAA-000" @bind-Value="productCode" />

                <!-- License plate -->
                <MaskedInput Mask="AAA-0000" @bind-Value="licensePlate" />
                """;

        private const string _indonesianIdCode = """
                <MaskedInput Mask="00.00.00.000000.0000"
                    @bind-Value="nikValue"
                    Placeholder="00.00.00.DDMMYY.0000"
                    ShowMask="true" />
                """;

        private const string _errorStateCode = """
                <MaskedInput Mask="@MaskedInput.Masks.PhoneUS"
                    @bind-Value="phoneValue"
                    AriaInvalid="true"
                    ShowMask="true" />
                <p class="text-sm text-destructive">This phone number is invalid.</p>
                """;

        private const string _requiredStateCode = """
                <MaskedInput Mask="@MaskedInput.Masks.PhoneUS"
                    @bind-Value="phoneValue"
                    Required="true"
                    ShowMask="true"
                    ShowValidationError="true" />
                """;

        private const string _statesCode = """
                <MaskedInput Mask="@MaskedInput.Masks.PhoneUS" Value="5555551234" Disabled="true" />
                <MaskedInput Mask="@MaskedInput.Masks.PhoneUS" Value="5555551234" Readonly="true" />
                """;

        private const string _validationCode = """
                <EditForm Model="@model" OnValidSubmit="HandleSubmit">
                    <DataAnnotationsValidator />
                    <MaskedInput Mask="@MaskedInput.Masks.PhoneUS" @bind-Value="model.Phone"
                        ShowMask="false" Placeholder="(555) 000-0000" Required="true" />
                    <ValidationMessage For="@(() => model.Phone)" class="text-sm text-destructive" />
                </EditForm>
                """;
    }
}
