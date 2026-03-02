namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class LabelDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _labelProps =
            [
                new("For",          "string?",          null, "The ID of the form element this label is associated with (maps to htmlFor). Clicking the label will focus the associated control."),
                new("Class",        "string?",          null, "Additional CSS classes to apply to the label element."),
                new("ChildContent", "RenderFragment?",  null, "The content to display inside the label (text, icons, indicators, etc.)."),
            ];

        private const string _basicCode =
                """
                <Label For="name-input">Your Name</Label>
                <Input Id="name-input" Placeholder="Enter your name" />
                """;

        private const string _formFieldsCode =
                """
                <div class="space-y-2">
                    <Label For="email-field">Email Address</Label>
                    <Input Id="email-field" Type="InputType.Email" Placeholder="you@example.com" />
                </div>

                <div class="space-y-2">
                    <Label For="password-field">Password</Label>
                    <Input Id="password-field" Type="InputType.Password" Placeholder="••••••••" />
                </div>
                """;

        private const string _requiredCode =
                """
                <Label For="required-field">
                    Username <span class="text-destructive">*</span>
                </Label>
                <Input Id="required-field" Required Placeholder="Enter username" />
                """;

        private const string _checkboxCode =
                """
                <div class="flex items-center space-x-2">
                    <Checkbox Id="terms-checkbox" />
                    <Label For="terms-checkbox" Class="cursor-pointer">Accept terms and conditions</Label>
                </div>
                """;

        private const string _disabledCode =
                """
                <Input Id="disabled-input" Disabled Class="peer" />
                <Label For="disabled-input">Disabled Field Label</Label>
                """;

        private const string _customCode =
                """
                <Label For="input" Class="text-lg font-bold">Large Bold Label</Label>
                <Label For="input" Class="text-xs text-muted-foreground font-normal">Small Muted Label</Label>
                <Label For="input" Class="text-blue-600">Colored Label</Label>
                <Label For="input" Class="uppercase tracking-wide">Uppercase Label</Label>
                """;

        private const string _withDescriptionsCode =
                """
                <div class="space-y-2">
                    <Label For="display-name">Display Name</Label>
                    <Input Id="display-name" Placeholder="John Doe" />
                    <p class="text-xs text-muted-foreground">
                        This is your public display name.
                    </p>
                </div>
                """;

        private const string _completeFormCode =
                """
                <Label For="form-name">Full Name <span class="text-destructive">*</span></Label>
                <Input Id="form-name" Required Placeholder="John Doe" />

                <Label For="form-email">Email Address <span class="text-destructive">*</span></Label>
                <Input Id="form-email" Type="InputType.Email" Required Placeholder="john@example.com" />
                """;
    }
}
