namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class RadioGroupPrimitiveDemo
    {
        private const string _basicCode =
        """
        <RadioGroupPrimitive @bind-Value="basicValue" AriaLabel="Basic radio group">
            <RadioGroupItemPrimitive Value="Option 1" />
            <RadioGroupItemPrimitive Value="Option 2" />
            <RadioGroupItemPrimitive Value="Option 3" />
        </RadioGroupPrimitive>
        """;

        private const string _controlledCode =
        """
        <button @onclick='@(() => controlledValue = "Small")'>Select Small</button>
        <button @onclick='@(() => controlledValue = "Medium")'>Select Medium</button>
        <button @onclick='@(() => controlledValue = "Large")'>Select Large</button>

        <RadioGroupPrimitive @bind-Value="controlledValue" AriaLabel="Size selection">
            <RadioGroupItemPrimitive Value="Small" />
            <RadioGroupItemPrimitive Value="Medium" />
            <RadioGroupItemPrimitive Value="Large" />
        </RadioGroupPrimitive>
        """;

        private const string _disabledCode =
        """
        <RadioGroupPrimitive Value="Option A" Disabled="true" AriaLabel="Disabled group">
            <RadioGroupItemPrimitive Value="Option A" />
            <RadioGroupItemPrimitive Value="Option B" />
        </RadioGroupPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Arrow Keys</Kbd>
        <Kbd>Space / Enter</Kbd>
        <Kbd>Tab</Kbd>
        """;

        private const string _navigationCode =
        """
        <RadioGroupPrimitive @bind-Value="navValue" AriaLabel="Navigation demo">
            <RadioGroupItemPrimitive Value="North" />
            <RadioGroupItemPrimitive Value="East" />
            <RadioGroupItemPrimitive Value="South" />
            <RadioGroupItemPrimitive Value="West" />
        </RadioGroupPrimitive>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _radioGroupProps =
    [
        new("Value", "TValue?", null, "The currently selected value. Supports two-way binding."),
        new("ValueChanged", "EventCallback&lt;TValue&gt;", null, "Callback invoked when the selected value changes."),
        new("Disabled", "bool", "false", "Disables the entire radio group when true."),
        new("AriaLabel", "string?", null, "Accessible label for screen readers."),
        new("Item.Value", "TValue", null, "The value of each radio item (required)."),
        new("Item.AdditionalAttributes", "IReadOnlyDictionary&lt;string, object&gt;?", null, "Additional attributes passed to the item button element."),
    ];
    }
}
