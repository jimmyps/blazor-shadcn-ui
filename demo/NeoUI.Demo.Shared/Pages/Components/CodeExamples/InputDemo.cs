namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class InputDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _inputProps =
            [
                new("Type",               "InputType",   "Text",   "HTML input type. Options: Text, Email, Password, Number, Date, File, Search, Tel, Url, Time."),
                new("Value",              "string?",     "null",   "Current value. Use @bind-Value for two-way binding."),
                new("Placeholder",        "string?",     "null",   "Placeholder text shown when input is empty."),
                new("Disabled",           "bool",        "false",  "Disables the input when true."),
                new("Required",           "bool",        "false",  "Marks the input as required for native form validation."),
                new("UpdateOn",           "InputUpdateMode", "Input", "When to update the bound value. Options: Input (every keystroke), Change (on blur)."),
                new("ShowValidationError","bool",        "false",  "Shows browser native validation tooltip on invalid state."),
                new("AriaLabel",          "string?",     "null",   "ARIA label for accessibility when no visible label exists."),
                new("AriaDescribedBy",    "string?",     "null",   "ID of element that describes this input."),
                new("AriaInvalid",        "bool",        "false",  "Marks the input as invalid for screen readers."),
                new("Class",              "string?",     "null",   "Additional CSS classes appended to the input element."),
            ];

        private const string _defaultCode = """
                <Input Type="InputType.Text" Placeholder="Email" />
                """;

        private const string _updateOnCode = """
                <!-- Updates on every keystroke -->
                <Input Type="InputType.Text" @bind-Value="value" UpdateOn="InputUpdateMode.Input" />

                <!-- Updates only when field loses focus -->
                <Input Type="InputType.Text" @bind-Value="value" UpdateOn="InputUpdateMode.Change" />
                """;

        private const string _typesCode = """
                <Input Type="InputType.Text" Placeholder="Enter text" />
                <Input Type="InputType.Email" Placeholder="name@example.com" />
                <Input Type="InputType.Password" Placeholder="Enter password" />
                <Input Type="InputType.Number" Placeholder="0" />
                <Input Type="InputType.Date" />
                <Input Type="InputType.File" />
                """;

        private const string _fileCode = """
                <Input Type="InputType.File" />
                <Input Type="InputType.File" multiple />
                """;

        private const string _disabledCode = """
                <Input Type="InputType.Text" Placeholder="Disabled input" Disabled="true" />
                """;

        private const string _requiredCode = """
                <Input Type="InputType.Email" Placeholder="name@example.com" Required="true" ShowValidationError="true" />
                """;

        private const string _withLabelCode = """
                <div class="grid w-full max-w-sm items-center gap-1.5">
                    <Label For="email">Email</Label>
                    <Input Type="InputType.Email" Id="email" Placeholder="Email" />
                </div>
                """;

        private const string _ariaCode = """
                <Input Type="InputType.Search"
                    AriaLabel="Search the website"
                    AriaDescribedBy="search-hint" />

                <Input Type="InputType.Text"
                    AriaInvalid="true"
                    AriaDescribedBy="email-error" />
                """;

        private const string _bindingCode = """
                <Input Type="InputType.Text" @bind-Value="name" Placeholder="Type your name" />
                <p>Value: @name</p>

                @code {
                    private string? name;
                }
                """;

        private const string _customStylingCode = """
                <Input Type="InputType.Text" Placeholder="Rounded full" Class="rounded-full px-4" />
                <Input Type="InputType.Text" Placeholder="Large text" Class="text-lg h-14" />
                """;

        private const string _validationCode = """
                <EditForm Model="@validationModel" OnValidSubmit="HandleValidSubmit">
                    <DataAnnotationsValidator />
                    <div class="space-y-4">
                        <Field>
                            <FieldLabel For="val-username">Username</FieldLabel>
                            <FieldContent>
                                <NeoUI.Blazor.Input Id="val-username" Type="InputType.Text"
                                       @bind-Value="validationModel.Username"
                                       ShowValidationError="true" Placeholder="Enter username" />
                                <FieldDescription>At least 3 characters, letters, numbers, and underscores only</FieldDescription>
                                <ValidationMessage For="@(() => validationModel.Username)" class="text-destructive text-sm" />
                            </FieldContent>
                        </Field>
                        <Field>
                            <FieldLabel For="val-email">Email</FieldLabel>
                            <FieldContent>
                                <NeoUI.Blazor.Input Id="val-email" Type="InputType.Email"
                                       @bind-Value="validationModel.Email"
                                       ShowValidationError="true" Placeholder="your@email.com" />
                                <ValidationMessage For="@(() => validationModel.Email)" class="text-destructive text-sm" />
                            </FieldContent>
                        </Field>
                        <Field>
                            <FieldLabel For="val-age">Age</FieldLabel>
                            <FieldContent>
                                <NeoUI.Blazor.Input Id="val-age" Type="InputType.Number"
                                       @bind-Value="validationModel.Age"
                                       ShowValidationError="true" />
                                <FieldDescription>Must be between 18 and 120</FieldDescription>
                                <ValidationMessage For="@(() => validationModel.Age)" class="text-destructive text-sm" />
                            </FieldContent>
                        </Field>
                        <div class="flex gap-2">
                            <Button Type="ButtonType.Submit">Validate &amp; Submit</Button>
                            <Button Variant="ButtonVariant.Outline" OnClick="ResetForm">Reset</Button>
                        </div>
                    </div>
                </EditForm>
                """;
    }
}
