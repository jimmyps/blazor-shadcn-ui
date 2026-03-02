namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class RadioGroupDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _radioGroupProps =
            [
                new("Value",     "TValue",  "—",     "The currently selected value. Use @bind-Value for two-way binding."),
                new("Disabled",  "bool",    "false",  "Whether the entire radio group is disabled."),
                new("AriaLabel", "string?", null,     "ARIA label for the group (for screen reader accessibility)."),
                new("Class",     "string?", null,     "Additional CSS classes for the root element."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _radioGroupItemProps =
            [
                new("Value",    "TValue",  "—",    "The value this item represents."),
                new("Id",       "string?", null,   "HTML id for label association."),
                new("Disabled", "bool",    "false", "Whether this individual item is disabled."),
            ];

        private const string _basicCode =
                """
                <RadioGroup @bind-Value="selectedOption" AriaLabel="Choose an option">
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="default" Id="r1" />
                        <label for="r1">Default</label>
                    </div>
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="comfortable" Id="r2" />
                        <label for="r2">Comfortable</label>
                    </div>
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="compact" Id="r3" />
                        <label for="r3">Compact</label>
                    </div>
                </RadioGroup>
                """;

        private const string _disabledCode =
                """
                <!-- Disabled group -->
                <RadioGroup Disabled="true" AriaLabel="Disabled group">
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="option1" Id="d1" />
                        <label for="d1">Option 1</label>
                    </div>
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="option2" Id="d2" />
                        <label for="d2">Option 2</label>
                    </div>
                </RadioGroup>

                <!-- Individual item disabled -->
                <RadioGroup AriaLabel="Individual items disabled">
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="enabled1" Id="id1" />
                        <label for="id1">Enabled Option 1</label>
                    </div>
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="disabled" Id="id2" Disabled="true" />
                        <label for="id2">Disabled Option</label>
                    </div>
                </RadioGroup>
                """;

        private const string _formValidationCode =
                """
                <EditForm Model="formModel" OnValidSubmit="HandleValidSubmit">
                    <DataAnnotationsValidator />
                    <RadioGroup @bind-Value="formModel.NotificationPreference" AriaLabel="Choose notification preference">
                        <div class="flex items-center gap-3">
                            <RadioGroupItem Value="email" Id="form-email" />
                            <label for="form-email">Email</label>
                        </div>
                        <div class="flex items-center gap-3">
                            <RadioGroupItem Value="sms" Id="form-sms" />
                            <label for="form-sms">SMS</label>
                        </div>
                        <div class="flex items-center gap-3">
                            <RadioGroupItem Value="push" Id="form-push" />
                            <label for="form-push">Push Notifications</label>
                        </div>
                    </RadioGroup>
                    <ValidationMessage For="@(() => formModel.NotificationPreference)" />
                    <Button Type="ButtonType.Submit">Submit</Button>
                </EditForm>
                """;

        private const string _valueTypesCode =
                """
                <!-- Integer values -->
                <RadioGroup @bind-Value="intValue" AriaLabel="Select a number">
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="1" Id="int1" />
                        <label for="int1">One</label>
                    </div>
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="2" Id="int2" />
                        <label for="int2">Two</label>
                    </div>
                </RadioGroup>

                <!-- Enum values -->
                <RadioGroup @bind-Value="sizeValue" AriaLabel="Select a size">
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="Size.Small" Id="size-small" />
                        <label for="size-small">Small</label>
                    </div>
                    <div class="flex items-center gap-3">
                        <RadioGroupItem Value="Size.Medium" Id="size-medium" />
                        <label for="size-medium">Medium</label>
                    </div>
                </RadioGroup>
                """;

        private const string _containerCode =
                """
                <RadioGroup @bind-Value="highlightedValue" AriaLabel="Select a plan">
                    <div @onclick="@(() => highlightedValue = "free")"
                         class="flex items-start space-x-3 rounded-lg border p-4 cursor-pointer transition-colors">
                        <div @onclick:stopPropagation class="mt-0.5">
                            <RadioGroupItem Value="free" Id="plan-free" />
                        </div>
                        <div class="flex-1">
                            <div class="text-sm font-medium">Free Plan</div>
                            <p class="text-sm text-muted-foreground mt-1.5">Perfect for getting started.</p>
                        </div>
                    </div>
                    <div @onclick="@(() => highlightedValue = "pro")"
                         class="flex items-start space-x-3 rounded-lg border p-4 cursor-pointer transition-colors">
                        <div @onclick:stopPropagation class="mt-0.5">
                            <RadioGroupItem Value="pro" Id="plan-pro" />
                        </div>
                        <div class="flex-1">
                            <div class="text-sm font-medium">Pro Plan</div>
                            <p class="text-sm text-muted-foreground mt-1.5">Unlimited features and priority support.</p>
                        </div>
                    </div>
                </RadioGroup>
                """;
    }
}
