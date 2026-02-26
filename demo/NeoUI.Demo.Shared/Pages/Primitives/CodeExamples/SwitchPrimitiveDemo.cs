namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class SwitchPrimitiveDemo
    {
        private const string _basicCode =
        """
        <SwitchPrimitive @bind-Checked="basicChecked"
                         Id="basic"
                         class="relative inline-flex h-6 w-11 shrink-0 cursor-pointer items-center rounded-full border-2 border-input transition-colors data-[state=checked]:bg-primary data-[state=unchecked]:bg-muted">
            <span class="pointer-events-none block h-5 w-5 rounded-full bg-background shadow-lg ring-0 transition-transform data-[state=checked]:translate-x-5 data-[state=unchecked]:translate-x-0"
                  data-state="@(basicChecked ? "checked" : "unchecked")"></span>
        </SwitchPrimitive>
        """;

        private const string _controlledCode =
        """
        <button @onclick="@(() => controlledChecked = true)">Turn On</button>
        <button @onclick="@(() => controlledChecked = false)">Turn Off</button>

        <SwitchPrimitive @bind-Checked="controlledChecked" Id="controlled">
            <span data-state="@(controlledChecked ? "checked" : "unchecked")"></span>
        </SwitchPrimitive>
        """;

        private const string _disabledCode =
        """
        <SwitchPrimitive Checked="true" Disabled="true" Id="disabled-on">
            <span data-state="checked"></span>
        </SwitchPrimitive>

        <SwitchPrimitive Checked="false" Disabled="true" Id="disabled-off">
            <span data-state="unchecked"></span>
        </SwitchPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Space</Kbd>
        <Kbd>Enter</Kbd>
        <Kbd>Tab</Kbd>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _switchProps =
    [
        new("Checked", "bool", "false", "Controls whether the switch is on. Supports two-way binding with <code class=\"text-xs bg-muted px-1 rounded\">@bind-Checked</code>."),
        new("CheckedChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the checked state changes."),
        new("Disabled", "bool", "false", "Prevents toggling when <code class=\"text-xs bg-muted px-1 rounded\">true</code>."),
        new("Id", "string?", null, "Identifier used for label association."),
        new("ChildContent", "RenderFragment?", null, "Optional custom thumb/content rendered inside the switch button."),
    ];
    }
}
