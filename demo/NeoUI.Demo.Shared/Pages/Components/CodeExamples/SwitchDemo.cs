namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SwitchDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _switchProps =
            [
                new("Checked",  "bool",        "false",  "Whether the switch is on. Use @bind-Checked for two-way binding."),
                new("Size",     "SwitchSize",  "Medium", "Size variant: Small, Medium, Large."),
                new("Disabled", "bool",        "false",  "Whether the switch cannot be interacted with."),
                new("Id",       "string?",     null,     "HTML id for label association."),
                new("Class",    "string?",     null,     "Additional CSS classes for the root element."),
            ];

        private const string _basicCode =
                """
                <Switch Checked="false" Id="basic-switch" />
                <label for="basic-switch" class="text-sm font-medium leading-none">
                    Airplane Mode
                </label>
                """;

        private const string _sizesCode =
                """
                <Switch Size="SwitchSize.Small" Id="small-switch" />
                <label for="small-switch">Small switch</label>

                <Switch Size="SwitchSize.Medium" Id="medium-switch" />
                <label for="medium-switch">Medium switch (default)</label>

                <Switch Size="SwitchSize.Large" Id="large-switch" />
                <label for="large-switch">Large switch</label>
                """;

        private const string _settingsCode =
                """
                <Switch Checked="true" Id="notifications" />
                <label for="notifications">Enable notifications</label>

                <Switch Checked="false" Id="auto-save" />
                <label for="auto-save">Auto-save changes</label>

                <Switch Checked="true" Id="analytics" />
                <label for="analytics">Send usage analytics</label>
                """;

        private const string _disabledCode =
                """
                <Switch Disabled="true" Checked="false" Id="disabled-off" />
                <label for="disabled-off">Disabled (off)</label>

                <Switch Disabled="true" Checked="true" Id="disabled-on" />
                <label for="disabled-on">Disabled (on)</label>
                """;

        private const string _formCode =
                """
                <EditForm Model="formModel" OnValidSubmit="HandleValidSubmit">
                    <DataAnnotationsValidator />
                    <Switch @bind-Checked="formModel.AcceptTerms" Id="terms" />
                    <label for="terms">I accept the terms and conditions</label>
                    <ValidationMessage For="@(() => formModel.AcceptTerms)" class="text-sm text-destructive" />

                    <Switch @bind-Checked="formModel.EnableMarketing" Id="marketing" />
                    <label for="marketing">Enable marketing emails (optional)</label>

                    <Button Type="ButtonType.Submit">Submit</Button>
                </EditForm>
                """;

        private const string _interactiveCode =
                """
                <Switch Checked="false" Id="interactive" />
                <label for="interactive">Enable dark mode</label>

                @if (interactiveChecked)
                {
                    <div class="p-4 bg-primary/10 rounded-lg border border-primary">
                        <p class="text-sm">✓ Dark mode enabled!</p>
                    </div>
                }
                else
                {
                    <div class="p-4 bg-muted rounded-lg">
                        <p class="text-sm text-muted-foreground">Dark mode disabled</p>
                    </div>
                }
                """;

        private const string _cardsCode =
                """
                <div class="flex items-center justify-between rounded-lg border p-4">
                    <div class="space-y-0.5">
                        <div class="text-sm font-medium">Email Notifications</div>
                        <div class="text-sm text-muted-foreground">Receive email about your account activity</div>
                    </div>
                    <Switch Checked="true" Id="card-notifications" />
                </div>
                """;

        private const string _customCode =
                """
                <Switch Class="shadow-lg" Id="styled1" />
                <label for="styled1">With shadow</label>

                <Switch Class="ring-2 ring-offset-2" Id="styled2" />
                <label for="styled2">With ring</label>
                """;
    }
}
