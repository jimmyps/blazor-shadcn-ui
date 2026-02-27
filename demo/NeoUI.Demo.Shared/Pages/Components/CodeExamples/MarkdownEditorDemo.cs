namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class MarkdownEditorDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _markdownEditorProps =
            [
                new("Value",       "string?",  null,    "The current markdown content."),
                new("Placeholder", "string?",  null,    "Placeholder text shown when the editor is empty."),
                new("Disabled",    "bool",     "false", "When true, the editor is non-interactive."),
                new("Id",          "string?",  null,    "HTML id attribute, used to associate with a Label."),
                new("Class",       "string?",  null,    "Additional CSS classes appended to the root element."),
            ];

        private const string _basicCode =
                """
                <MarkdownEditor @bind-Value="value" Placeholder="Write something amazing..." />

                @code {
                    private string? value;
                }
                """;

        private const string _labelCode =
                """
                <Label For="editor">Description</Label>
                <MarkdownEditor @bind-Value="value" Id="editor" Placeholder="Enter a description..." />
                """;

        private const string _toolbarCode =
                """
                <MarkdownEditor @bind-Value="toolbarDemoValue" Placeholder="Try out the toolbar features..." />
                """;

        private const string _prePopulatedCode =
                """
                <MarkdownEditor @bind-Value="value" />

                @code {
                    private string value = "# Hello\n\nThis is **bold** text.";
                }
                """;

        private const string _disabledCode =
                """
                <MarkdownEditor Value="This editor is disabled." Disabled="true" />
                """;

        private const string _formCode =
                """
                <MarkdownEditor @bind-Value="feedbackValue" Placeholder="What's on your mind?" />
                <Button Disabled="@string.IsNullOrWhiteSpace(feedbackValue)">Submit</Button>
                """;
    }
}
