namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class CheckboxDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _checkboxProps =
            [
                new("Checked",       "bool",                   "false", "Whether the checkbox is checked. Supports two-way binding with @bind-Checked."),
                new("Disabled",      "bool",                   "false", "When true, the checkbox cannot be interacted with and is removed from tab order."),
                new("Id",            "string?",                null,    "HTML id attribute, used to associate with a <label> element via its for attribute."),
                new("Class",         "string?",                null,    "Additional CSS classes for the checkbox element."),
                new("CheckedChanged","EventCallback<bool>",    null,    "Fired when the checked state changes."),
            ];

        private const string _basicCode =
                """
                <div class="flex items-center space-x-2">
                    <Checkbox @bind-Checked="isChecked" Id="my-checkbox" />
                    <label for="my-checkbox" class="text-sm font-medium leading-none">
                        Accept terms and conditions
                    </label>
                </div>
                """;

        private const string _multipleCode =
                """
                <Checkbox @bind-Checked="option1" Id="option1" />
                <label for="option1">Email notifications</label>

                <Checkbox @bind-Checked="option2" Id="option2" />
                <label for="option2">SMS notifications</label>
                """;

        private const string _disabledCode =
                """
                <Checkbox Disabled="true" Checked="false" Id="disabled-unchecked" />
                <Checkbox Disabled="true" Checked="true" Id="disabled-checked" />
                """;

        private const string _validationCode =
                """
                <EditForm Model="formModel" OnValidSubmit="HandleValidSubmit">
                    <DataAnnotationsValidator />
                    <div class="flex items-center space-x-2">
                        <Checkbox @bind-Checked="formModel.AcceptTerms" Id="terms" />
                        <label for="terms">I accept the terms and conditions</label>
                    </div>
                    <ValidationMessage For="@(() => formModel.AcceptTerms)" class="text-sm text-destructive" />
                    <Button Type="ButtonType.Submit">Submit</Button>
                </EditForm>
                """;

        private const string _interactiveCode =
                """
                <Checkbox @bind-Checked="featureEnabled" Id="feature-flag" />
                <label for="feature-flag">Toggle feature flag</label>

                @if (featureEnabled)
                {
                    <div class="p-4 bg-primary/10 rounded-lg border border-primary">
                        <p class="text-sm">✓ Feature is enabled!</p>
                    </div>
                }
                """;

        private const string _containerCode =
                """
                <div @onclick="@(() => isChecked = !isChecked)"
                     class="flex items-center space-x-3 rounded-lg border p-4 cursor-pointer">
                    <div @onclick:stopPropagation>
                        <Checkbox @bind-Checked="isChecked" Id="container-checkbox" />
                    </div>
                    <div>Accept terms and conditions</div>
                </div>
                """;

        private const string _customCode =
                """
                <Checkbox Class="border-2" Id="thick-border" />
                <Checkbox Class="shadow-md" Id="with-shadow" />
                """;
    }
}
