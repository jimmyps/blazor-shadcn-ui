namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class CurrencyInputDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _currencyProps =
            [
                new("TValue",             "type param",  "—",      "Value type. Options: decimal, decimal?, double, double?."),
                new("Value",              "TValue",       "—",      "Current value. Use @bind-Value for two-way binding."),
                new("Currency",           "string",       "USD",    "ISO 4217 currency code (e.g., USD, EUR, GBP, JPY)."),
                new("Culture",            "string?",      "null",   "Culture for number formatting (e.g., en-US, de-DE, fr-FR)."),
                new("ShowCurrencySymbol", "bool",         "true",   "Whether to show the currency symbol prefix."),
                new("Min",                "TValue?",      "null",   "Minimum allowed value."),
                new("Max",                "TValue?",      "null",   "Maximum allowed value."),
                new("Disabled",           "bool",         "false",  "Disables the input when true."),
                new("Readonly",           "bool",         "false",  "Makes the input read-only when true."),
                new("Required",           "bool",         "false",  "Marks the input as required for native form validation."),
                new("AriaInvalid",        "bool",         "false",  "Marks the input as invalid for screen readers."),
                new("ShowValidationError","bool",         "false",  "Shows browser native validation tooltip on invalid state."),
                new("Placeholder",        "string?",      "null",   "Placeholder text when the input is empty."),
                new("AriaLabel",          "string?",      "null",   "ARIA label for accessibility when no visible label exists."),
                new("AriaDescribedBy",    "string?",      "null",   "ID of the element that describes this input."),
                new("Class",              "string?",      "null",   "Additional CSS classes appended to the input element."),
            ];

        private const string _defaultCode = """
                <CurrencyInput TValue="decimal" @bind-Value="amount" Currency="USD" />

                @code {
                    private decimal amount = 0m;
                }
                """;

        private const string _currenciesCode = """
                <CurrencyInput TValue="decimal" @bind-Value="eurValue" Currency="EUR" />
                <CurrencyInput TValue="decimal" @bind-Value="gbpValue" Currency="GBP" />
                <CurrencyInput TValue="decimal" @bind-Value="jpyValue" Currency="JPY" />
                """;

        private const string _culturesCode = """
                <!-- 1,234.56 (en-US) -->
                <CurrencyInput TValue="decimal" @bind-Value="value" Currency="USD" Culture="en-US" />

                <!-- 1.234,56 (de-DE) -->
                <CurrencyInput TValue="decimal" @bind-Value="value" Currency="EUR" Culture="de-DE" />

                <!-- 1 234,56 (fr-FR) -->
                <CurrencyInput TValue="decimal" @bind-Value="value" Currency="EUR" Culture="fr-FR" />
                """;

        private const string _precisionCode = """
                <CurrencyInput TValue="decimal" @bind-Value="largeValue" Currency="USD" />
                """;

        private const string _noSymbolCode = """
                <CurrencyInput TValue="decimal" @bind-Value="value" Currency="USD" ShowCurrencySymbol="false" />
                """;

        private const string _doubleCode = """
                <CurrencyInput TValue="double" @bind-Value="doubleValue" Currency="USD" />
                """;

        private const string _minMaxCode = """
                <CurrencyInput TValue="decimal" @bind-Value="budget" Currency="USD" Min="0" Max="10000" />
                """;

        private const string _disabledCode = """
                <CurrencyInput TValue="decimal" Currency="USD" Placeholder="Disabled input" Disabled="true" Value="99.99m" />
                """;

        private const string _readonlyCode = """
                <CurrencyInput TValue="decimal" Currency="EUR" Value="249.99m" Readonly="true" />
                """;

        private const string _requiredCode = """
                <form class="space-y-4">
                    <div class="space-y-2">
                        <label class="text-sm font-medium">Amount (required)</label>
                        <CurrencyInput TValue="decimal?" @bind-Value="requiredValue" Currency="USD" Placeholder="Enter amount" Required="true" />
                    </div>
                    <button type="submit">Submit</button>
                </form>
                """;

        private const string _validationCode = """
                <EditForm Model="@model" OnValidSubmit="HandleSubmit">
                    <DataAnnotationsValidator />
                    <CurrencyInput TValue="decimal?" @bind-Value="model.Amount" Currency="USD" Required="true" />
                    <ValidationMessage For="@(() => model.Amount)" class="text-sm text-destructive" />
                </EditForm>
                """;

        private const string _ariaCode = """
                <CurrencyInput TValue="decimal"
                    @bind-Value="value"
                    Currency="USD"
                    AriaLabel="Total amount in US dollars"
                    AriaDescribedBy="amount-hint" />
                """;

        private const string _errorCode = """
                <CurrencyInput TValue="decimal" @bind-Value="errorValue" Currency="USD" AriaInvalid="true" />
                <p class="text-sm text-destructive">Please enter a valid amount.</p>
                """;

        private const string _internationalCode = """
                <CurrencyInput TValue="decimal" @bind-Value="cadValue" Currency="CAD" />
                <CurrencyInput TValue="decimal" @bind-Value="audValue" Currency="AUD" />
                <CurrencyInput TValue="decimal" @bind-Value="inrValue" Currency="INR" />
                <CurrencyInput TValue="decimal" @bind-Value="brlValue" Currency="BRL" />
                """;

        private const string _specialDecimalsCode = """
                <!-- 0 decimal places -->
                <CurrencyInput TValue="decimal" @bind-Value="jpyValue" Currency="JPY" />
                <CurrencyInput TValue="decimal" @bind-Value="krwValue" Currency="KRW" />

                <!-- 3 decimal places -->
                <CurrencyInput TValue="decimal" @bind-Value="kwdValue" Currency="KWD" />
                <CurrencyInput TValue="decimal" @bind-Value="bhdValue" Currency="BHD" />
                """;

        private const string _symbolPositionCode = """
                <!-- Symbol after amount -->
                <CurrencyInput TValue="decimal" @bind-Value="vndValue" Currency="VND" />

                <!-- Symbol before amount -->
                <CurrencyInput TValue="decimal" @bind-Value="usdValue" Currency="USD" />
                """;

        private const string _unicodeCode = """
                <CurrencyInput TValue="decimal" @bind-Value="thbValue" Currency="THB" />
                <CurrencyInput TValue="decimal" @bind-Value="phpValue" Currency="PHP" />
                <CurrencyInput TValue="decimal" @bind-Value="tryValue" Currency="TRY" />
                """;
    }
}
