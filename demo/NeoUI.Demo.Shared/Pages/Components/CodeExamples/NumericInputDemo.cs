namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class NumericInputDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _numericInputProps =
            [
                new("TValue",      "numeric type",    "-",      "The numeric type: int, decimal, double, float, long, etc."),
                new("Value",       "TValue",          "0",      "The current numeric value."),
                new("Min",         "TValue?",         null,     "Minimum allowed value."),
                new("Max",         "TValue?",         null,     "Maximum allowed value."),
                new("Step",        "TValue?",         null,     "The increment/decrement step value."),
                new("Placeholder", "string?",         null,     "Placeholder text shown when the input is empty."),
                new("Disabled",    "bool",            "false",  "When true, the input is non-interactive."),
                new("Readonly",    "bool",            "false",  "When true, the input is focusable but not editable."),
                new("Required",    "bool",            "false",  "When true, marks the field as required."),
                new("AriaInvalid", "bool",            "false",  "When true, applies aria-invalid for error state styling."),
                new("UpdateOn",    "UpdateOnMode",    "Change", "When to update bound value. Change = on blur, Input = on every keystroke."),
                new("Class",       "string?",         null,     "Additional CSS classes appended to the root element."),
            ];

        private const string _intCode =
                """
                <NumericInput TValue="int" @bind-Value="quantity" Placeholder="Enter quantity..." />
                """;

        private const string _decimalCode =
                """
                <NumericInput TValue="decimal" @bind-Value="price" Placeholder="0.00" Step="0.01" Min="0" Max="10000" />
                """;

        private const string _floatCode =
                """
                <NumericInput TValue="float" @bind-Value="rating" Placeholder="0.0" Step="0.1" Min="0" Max="5" />
                """;

        private const string _withLabelCode =
                """
                <label for="amount-input" class="text-sm font-medium leading-none">Amount</label>
                <NumericInput TValue="decimal" @bind-Value="amount" Id="amount-input" Placeholder="0.00" Step="0.01" AriaLabel="Amount" />
                """;

        private const string _doubleCode =
                """
                <NumericInput TValue="double" @bind-Value="percentage" Placeholder="0.0" Step="0.1" Min="0" Max="100" />
                """;

        private const string _constraintsCode =
                """
                <NumericInput TValue="int" @bind-Value="age" Min="18" Max="120" Step="1" Placeholder="Enter age" />
                """;

        private const string _disabledCode =
                """
                <NumericInput TValue="int" Value="42" Disabled="true" />
                """;

        private const string _requiredCode =
                """
                <NumericInput TValue="int" @bind-Value="quantity" Placeholder="Enter quantity" Required="true" Min="1" />
                """;

        private const string _errorCode =
                """
                <NumericInput TValue="int" @bind-Value="value" AriaInvalid="true" Placeholder="This field has an error" />
                """;

        private const string _readonlyCode =
                """
                <NumericInput TValue="decimal" Value="99.99m" Readonly="true" />
                """;
    }
}
