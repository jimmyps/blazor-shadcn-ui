namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class CheckboxPrimitiveDemo
    {
        private const string _basicCode =
        """
        <CheckboxPrimitive @bind-Checked="basicChecked" Id="basic" class="h-5 w-5 border rounded">
            @if (basicChecked)
            {
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                    <polyline points="20 6 9 17 4 12"></polyline>
                </svg>
            }
        </CheckboxPrimitive>
        """;

        private const string _controlledCode =
        """
        <button @onclick="@(() => controlledChecked = true)">Check</button>
        <button @onclick="@(() => controlledChecked = false)">Uncheck</button>

        <CheckboxPrimitive @bind-Checked="controlledChecked" Id="controlled">
            @if (controlledChecked)
            {
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                    <polyline points="20 6 9 17 4 12"></polyline>
                </svg>
            }
        </CheckboxPrimitive>
        """;

        private const string _indeterminateCode =
        """
        <CheckboxPrimitive Checked="@allChecked"
                           Indeterminate="@indeterminate"
                           CheckedChanged="@HandleParentChanged"
                           Id="parent">
            @if (allChecked)
            {
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                    <polyline points="20 6 9 17 4 12"></polyline>
                </svg>
            }
            else if (indeterminate)
            {
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                    <line x1="5" y1="12" x2="19" y2="12"></line>
                </svg>
            }
        </CheckboxPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Tab</Kbd>
        <Kbd>Space</Kbd>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _checkboxProps =
    [
        new("Checked", "bool", "false", "Controls whether the checkbox is checked. Supports two-way binding via the <code class=\"text-xs bg-muted px-1 rounded\">Checked</code> parameter."),
        new("CheckedChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the checked state changes."),
        new("Indeterminate", "bool", "false", "Sets the checkbox to indeterminate state and applies <code class=\"text-xs bg-muted px-1 rounded\">aria-checked=\"mixed\"</code>."),
        new("IndeterminateChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the indeterminate state changes."),
        new("Disabled", "bool", "false", "Prevents toggling when true."),
        new("Id", "string?", null, "Unique identifier used for external label association."),
        new("ChildContent", "RenderFragment?", null, "Optional inner content, typically a check icon."),
    ];
    }
}
