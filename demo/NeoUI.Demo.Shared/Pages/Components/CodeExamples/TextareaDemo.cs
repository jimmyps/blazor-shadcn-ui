namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class TextareaDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _textareaProps =
            [
                new("Value",           "string?",  null,    "The current value. Supports two-way binding with @bind-Value."),
                new("Placeholder",     "string?",  null,    "Placeholder text shown when the textarea is empty."),
                new("MaxLength",       "int?",     null,    "Maximum number of characters allowed."),
                new("Disabled",        "bool",     "false", "Whether the textarea is disabled."),
                new("Required",        "bool",     "false", "Whether the textarea is required for form validation."),
                new("AriaLabel",       "string?",  null,    "aria-label attribute for screen readers."),
                new("AriaDescribedBy", "string?",  null,    "aria-describedby attribute linking to a description element."),
                new("AriaInvalid",     "bool",     "false", "When true, applies destructive border and ring styling."),
                new("Class",           "string?",  null,    "Additional CSS classes to apply. Custom classes override default styles."),
            ];

        private const string _defaultCode =
                """
                <Textarea Placeholder="Type your message here." />
                """;

        private const string _disabledCode =
                """
                <Textarea Placeholder="Disabled textarea" Disabled="true" Value="Cannot edit this text." />
                """;

        private const string _requiredCode =
                """
                <Textarea Placeholder="Enter your message" Required="true" />
                """;

        private const string _withLabelCode =
                """
                <div class="space-y-2">
                    <label class="text-sm font-medium">Bio</label>
                    <Textarea Placeholder="Tell us about yourself..." AriaLabel="Biography" />
                    <p class="text-sm text-muted-foreground">Your bio will be displayed on your public profile.</p>
                </div>
                """;

        private const string _maxLengthCode =
                """
                <Textarea MaxLength="280" Placeholder="What's happening?" />
                """;

        private const string _ariaCode =
                """
                <Textarea AriaLabel="User feedback" Placeholder="Share your feedback..." />

                <Textarea AriaInvalid="true" Value="This text is invalid" Placeholder="Message" />
                """;

        private const string _bindingCode =
                """
                <Textarea @bind-Value="message" Placeholder="Start typing..." />
                <p>Character count: @message?.Length</p>
                """;

        private const string _customStylingCode =
                """
                <Textarea Placeholder="Blue border" Class="border-blue-500 focus-visible:ring-blue-500" />
                <Textarea Placeholder="Taller textarea" Class="min-h-32" />
                <Textarea Value="Valid message" Class="border-green-500 focus-visible:ring-green-500" />
                """;
    }
}
