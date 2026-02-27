namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class RichTextEditorDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _richTextEditorProps =
            [
                new("Value",       "string?",       null,       "HTML content. Use @bind-Value for two-way binding."),
                new("Toolbar",     "ToolbarPreset", "Standard", "Preset toolbar configuration: None, Simple, Standard, Full, Custom."),
                new("Placeholder", "string?",       null,       "Placeholder text shown when the editor is empty."),
                new("Id",          "string?",       null,       "HTML id for associating with a Label component."),
                new("MinHeight",   "string?",       null,       "Minimum height of the editor area."),
                new("MaxHeight",   "string?",       null,       "Maximum height; a scrollbar appears when content exceeds this."),
                new("Height",      "string?",       null,       "Fixed height for the editor area."),
                new("Disabled",    "bool",          "false",    "When true, prevents editing."),
                new("ReadOnly",    "bool",          "false",    "When true, content is read-only but still selectable."),
                new("Class",       "string?",       null,       "Additional CSS classes for the root element."),
            ];

        private const string _basicCode =
                """
                <RichTextEditor Placeholder="Start typing here..." />
                """;

        private const string _withLabelCode =
                """
                <div class="space-y-2">
                    <Label For="description-editor">Description</Label>
                    <RichTextEditor Id="description-editor" Placeholder="Enter a description..." />
                </div>
                """;

        private const string _toolbarPresetsCode =
                """
                <RichTextEditor Toolbar="ToolbarPreset.Simple" Placeholder="Basic formatting only..." />

                <RichTextEditor Toolbar="ToolbarPreset.Standard" Placeholder="Common formatting options..." />

                <RichTextEditor Toolbar="ToolbarPreset.Full" Placeholder="All formatting options..." />

                <RichTextEditor Toolbar="ToolbarPreset.None" Placeholder="Plain editor without toolbar..." />
                """;

        private const string _toolbarFeaturesCode =
                """
                <RichTextEditor Toolbar="ToolbarPreset.Full" Placeholder="Try out the toolbar features..." />
                """;

        private const string _prePopulatedCode =
                """
                <RichTextEditor Value="@initialHtml" />
                """;

        private const string _heightControlCode =
                """
                <RichTextEditor MaxHeight="200px" Placeholder="Type a lot of content to see the scrollbar..." />

                <RichTextEditor Height="150px" Placeholder="Fixed height editor..." />
                """;

        private const string _disabledCode =
                """
                <RichTextEditor Value="<p>This editor is <strong>disabled</strong> and cannot be edited.</p>"
                                Disabled="true" />
                """;

        private const string _formIntegrationCode =
                """
                <Card>
                    <CardHeader>
                        <CardTitle>Submit Feedback</CardTitle>
                        <CardDescription>Share your thoughts with us using rich text formatting.</CardDescription>
                    </CardHeader>
                    <CardContent Class="space-y-4">
                        <div class="space-y-2">
                            <Label For="feedback-editor">Your Feedback</Label>
                            <RichTextEditor @bind-Value="feedbackValue"
                                            Id="feedback-editor"
                                            Placeholder="What's on your mind?" />
                        </div>
                    </CardContent>
                    <CardFooter>
                        <Button OnClick="HandleSubmit" Disabled="@string.IsNullOrWhiteSpace(feedbackValue)">
                            Submit Feedback
                        </Button>
                    </CardFooter>
                </Card>
                """;
    }
}
