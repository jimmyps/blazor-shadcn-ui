namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class QuantityStepperDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _props =
        [
            new("Value",              "int",                  "1",         "Current quantity value."),
            new("ValueChanged",       "EventCallback&lt;int&gt;", null,   "Fires when the value changes. Use @bind-Value for two-way binding."),
            new("Min",                "int",                  "1",         "Minimum allowed value. Decrement button is disabled at Min."),
            new("Max",                "int?",                 null,        "Maximum allowed value. Increment button is disabled at Max. Null means no upper limit."),
            new("DestructiveAtMin",   "bool",                 "false",     "Replace the − button with a trash icon at Min, firing OnDestructiveClick instead."),
            new("OnDestructiveClick", "EventCallback",        null,        "Fires when the trash button is pressed (only when DestructiveAtMin is true and value is at Min)."),
            new("Size",               "QuantityStepperSize",  "Default",   "Button size: Small (28 px), Default (32 px), or Large (44 px)."),
            new("Disabled",           "bool",                 "false",     "Disables the entire stepper."),
            new("AriaLabel",          "string",               "\"Quantity\"", "Accessible label for the control group."),
            new("Class",              "string?",              null,        "Additional CSS classes on the wrapper."),
        ];

        private const string _basicCode =
            """
            <QuantityStepper @bind-Value="@_qty" />

            @code {
                private int _qty = 1;
            }
            """;

        private const string _minMaxCode =
            """
            <QuantityStepper @bind-Value="@_qty" Min="1" Max="10" />
            """;

        private const string _destructiveCode =
            """
            <QuantityStepper @bind-Value="@_qty"
                             DestructiveAtMin="true"
                             OnDestructiveClick="@RemoveItem"
                             Min="1" />

            @code {
                private int _qty = 1;
                private void RemoveItem() { /* remove item from cart */ }
            }
            """;

        private const string _sizesCode =
            """
            <QuantityStepper @bind-Value="@_qty" Size="QuantityStepperSize.Small" />
            <QuantityStepper @bind-Value="@_qty" />
            <QuantityStepper @bind-Value="@_qty" Size="QuantityStepperSize.Large" />
            """;

        private const string _disabledCode =
            """
            <QuantityStepper Value="3" Disabled="true" />
            """;
    }
}
