namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class FileUploadDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _fileUploadProps =
            [
                new("Files",               "IBrowserFile list",  "—",      "The selected files. Use @bind-Files for two-way binding."),
                new("Multiple",            "bool",               "false",  "Whether to allow multiple file selection."),
                new("Accept",              "string?",            "null",   "Accepted file types (MIME types or extensions)."),
                new("MaxFileSize",         "long",               "512000", "Maximum file size in bytes."),
                new("MaxFiles",            "int",                "10",     "Maximum number of files allowed."),
                new("ShowPreview",         "bool",               "false",  "Whether to show image previews for image files."),
                new("ShowProgress",        "bool",               "true",   "Whether to show upload progress indicator."),
                new("Disabled",            "bool",               "false",  "Whether the upload zone is disabled."),
                new("DropZoneText",        "string?",            "null",   "Custom text for the drop zone."),
                new("ShowValidationError", "bool",               "false",  "Whether to show validation errors."),
                new("Class",               "string?",            "null",   "Additional CSS classes for the root element."),
            ];

        private const string _singleFileCode =
                """
                <FileUpload Multiple="false"
                            DropZoneText="Drop a file here or click to browse" />
                """;

        private const string _multipleFilesCode =
                """
                <FileUpload Multiple="true"
                            DropZoneText="Drop files here or click to browse" />
                """;

        private const string _imageUploadCode =
                """
                <FileUpload Multiple="true"
                            Accept="image/*"
                            ShowPreview="true"
                            DropZoneText="Drop images here or click to browse" />
                """;

        private const string _documentUploadCode =
                """
                <FileUpload Multiple="true"
                            Accept=".pdf,.doc,.docx,application/pdf,application/msword"
                            ShowPreview="false"
                            DropZoneText="Drop PDF or Word documents here" />
                """;

        private const string _fileSizeLimitCode =
                """
                <FileUpload Multiple="true"
                            MaxFileSize="2097152"
                            DropZoneText="Drop files here (max 2MB each)" />
                """;

        private const string _fileCountLimitCode =
                """
                <FileUpload Multiple="true"
                            MaxFiles="3"
                            DropZoneText="Drop up to 3 files here" />
                """;

        private const string _noProgressCode =
                """
                <FileUpload Multiple="true"
                            ShowProgress="false"
                            DropZoneText="Drop files here" />
                """;

        private const string _disabledCode =
                """
                <FileUpload Disabled="true"
                            DropZoneText="Upload disabled" />
                """;

        private const string _formIntegrationCode =
                """
                <EditForm Model="@uploadForm" OnValidSubmit="HandleValidSubmit">
                    <DataAnnotationsValidator />

                    <div class="space-y-4">
                        <div class="space-y-2">
                            <label class="text-sm font-medium">Profile Picture</label>
                            <FileUpload Multiple="false"
                                        Accept="image/*"
                                        MaxFileSize="5242880"
                                        ShowValidationError="true"
                                        DropZoneText="Upload your profile picture" />
                            <ValidationMessage For="@(() => uploadForm.ProfilePicture)" />
                        </div>

                        <div class="space-y-2">
                            <label class="text-sm font-medium">Documents (Required)</label>
                            <FileUpload Multiple="true"
                                        Accept=".pdf,.docx"
                                        MaxFiles="5"
                                        ShowValidationError="true"
                                        DropZoneText="Upload required documents" />
                            <ValidationMessage For="@(() => uploadForm.Documents)" />
                        </div>

                        <div class="flex gap-2">
                            <Button Type="ButtonType.Submit">Submit Form</Button>
                            <Button Type="ButtonType.Button" Variant="ButtonVariant.Outline">Reset</Button>
                        </div>
                    </div>
                </EditForm>
                """;

        private const string _customStylingCode =
                """
                <FileUpload Multiple="true"
                            Class="border-primary/50"
                            DropZoneText="Custom styled upload zone" />
                """;

        private const string _allFeaturesCode =
                """
                <FileUpload Multiple="true"
                            Accept="image/jpeg,image/png,image/gif,image/webp"
                            MaxFileSize="5242880"
                            MaxFiles="5"
                            ShowPreview="true"
                            ShowProgress="true"
                            DropZoneText="Drop up to 5 images (max 5MB each)" />
                """;
    }
}
