namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class SelectPrimitiveDemo
    {
        private const string _basicCode =
        """
        <SelectPrimitive TValue="string" @bind-Value="basicValue">
            <SelectTriggerPrimitive TValue="string">
                <span>Selected: @GetFruitDisplayText(basicValue)</span>
            </SelectTriggerPrimitive>
            <SelectContentPrimitive TValue="string">
                <SelectItemPrimitive TValue="string" Value="@("apple")" TextValue="Apple">Apple</SelectItemPrimitive>
                <SelectItemPrimitive TValue="string" Value="@("banana")" TextValue="Banana">Banana</SelectItemPrimitive>
                <SelectItemPrimitive TValue="string" Value="@("orange")" TextValue="Orange">Orange</SelectItemPrimitive>
                <SelectItemPrimitive TValue="string" Value="@("grape")" TextValue="Grape">Grape</SelectItemPrimitive>
            </SelectContentPrimitive>
        </SelectPrimitive>
        """;

        private const string _controlledCode =
        """
        <SelectPrimitive TValue="string" @bind-Value="controlledValue">
            <SelectTriggerPrimitive TValue="string">Select a fruit</SelectTriggerPrimitive>
            <SelectContentPrimitive TValue="string">
                <SelectItemPrimitive TValue="string" Value="@("Strawberry")">🍓 Strawberry</SelectItemPrimitive>
                <SelectItemPrimitive TValue="string" Value="@("Watermelon")">🍉 Watermelon</SelectItemPrimitive>
                <SelectItemPrimitive TValue="string" Value="@("Pineapple")">🍍 Pineapple</SelectItemPrimitive>
                <SelectItemPrimitive TValue="string" Value="@("Mango")">🥭 Mango</SelectItemPrimitive>
            </SelectContentPrimitive>
        </SelectPrimitive>
        """;

        private const string _disabledCode =
        """
        <SelectPrimitive TValue="string" Disabled="true" DefaultValue="@("disabled")">
            <SelectTriggerPrimitive TValue="string">Disabled Select</SelectTriggerPrimitive>
            <SelectContentPrimitive TValue="string">
                <SelectItemPrimitive TValue="string" Value="@("disabled")">This won't open</SelectItemPrimitive>
            </SelectContentPrimitive>
        </SelectPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>↑</Kbd> <Kbd>↓</Kbd>
        <Kbd>Home</Kbd> <Kbd>End</Kbd>
        <Kbd>Enter</Kbd> <Kbd>Space</Kbd>
        <Kbd>Esc</Kbd> <Kbd>Tab</Kbd>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _selectProps =
    [
        new("Value", "TValue?", null, "Currently selected value (controlled)."),
        new("ValueChanged", "EventCallback&lt;TValue&gt;", null, "Callback when value changes."),
        new("DefaultValue", "TValue?", null, "Initial value (uncontrolled)."),
        new("Open", "bool?", null, "Whether dropdown is open (controlled)."),
        new("OpenChanged", "EventCallback&lt;bool&gt;", null, "Callback when open state changes."),
        new("Disabled", "bool", "false", "Whether the select is disabled."),
        new("Item.Value", "TValue", null, "Value represented by this item (required)."),
        new("Item.TextValue", "string?", null, "Optional display text override."),
    ];
    }
}
