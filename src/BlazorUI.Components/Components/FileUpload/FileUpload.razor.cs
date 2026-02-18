using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace BlazorUI.Components.FileUpload;

/// <summary>
/// A file upload component with drag-and-drop support following the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The FileUpload component provides a customizable, accessible file upload that supports
/// drag-and-drop, file validation, and image previews. It follows WCAG 2.1 AA standards
/// for accessibility and integrates with Blazor's data binding and validation system.
/// </para>
/// <para>
/// Features:
/// - Single and multiple file uploads
/// - Drag-and-drop support with visual feedback
/// - File validation (size, type, count)
/// - Image preview thumbnails
/// - File list with remove buttons
/// - Visual upload progress (simulated)
/// - File type icons for non-image files
/// - Full EditContext integration for form validation
/// - Custom validation messages
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;FileUpload @bind-Files="uploadedFiles" 
///             Multiple="true" 
///             Accept="image/*"
///             MaxFileSize="5242880" /&gt;
/// </code>
/// </example>
public partial class FileUpload : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Tracks the ID of the first invalid input to ensure only one tooltip is shown.
    /// </summary>
    private static string? _firstInvalidInputId = null;
    
    /// <summary>
    /// Maximum file size in bytes for generating previews (500KB).
    /// </summary>
    private const long MaxPreviewSize = 512000; // 500KB
    
    /// <summary>
    /// Progress increment percentage for simulated upload progress.
    /// </summary>
    private const int ProgressIncrement = 10;
    
    /// <summary>
    /// Delay in milliseconds between progress updates.
    /// </summary>
    private const int ProgressDelayMs = 50;
    
    /// <summary>
    /// JavaScript module reference for drag-drop functionality.
    /// </summary>
    private IJSObjectReference? _module;
    
    /// <summary>
    /// JavaScript module reference for validation display.
    /// </summary>
    private IJSObjectReference? _validationModule;
    
    /// <summary>
    /// Tracks the previous EditContext to manage event subscriptions.
    /// </summary>
    private EditContext? _previousEditContext;
    
    /// <summary>
    /// Field identifier for validation in EditContext.
    /// </summary>
    private FieldIdentifier _fieldIdentifier;
    
    /// <summary>
    /// Reference to the drop zone element.
    /// </summary>
    private ElementReference _dropZoneRef;
    
    /// <summary>
    /// Reference to the underlying InputFile component.
    /// </summary>
    private InputFile? _inputFileRef;
    
    /// <summary>
    /// Indicates whether a file is currently being dragged over the drop zone.
    /// </summary>
    private bool _isDragging = false;
    
    /// <summary>
    /// Stores client-side validation errors (file size, type, count).
    /// </summary>
    private string? _validationError;
    
    /// <summary>
    /// Stores the current validation error message from EditContext.
    /// </summary>
    private string? _currentErrorMessage;
    
    /// <summary>
    /// Tracks whether the validation tooltip has been shown to avoid duplicates.
    /// </summary>
    private bool _hasShownTooltip = false;
    
    /// <summary>
    /// Tracks simulated upload progress for each file by index.
    /// </summary>
    private Dictionary<int, int> _uploadProgress = new();
    
    /// <summary>
    /// Caches Base64 preview URLs for image files.
    /// </summary>
    private Dictionary<IBrowserFile, string> _previewUrls = new();

    /// <summary>
    /// Gets or sets the JavaScript runtime for interop operations.
    /// </summary>
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets the cascaded EditContext from an EditForm.
    /// </summary>
    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    /// <summary>
    /// Gets or sets the list of selected files.
    /// </summary>
    [Parameter]
    public List<IBrowserFile>? Files { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the file list changes.
    /// </summary>
    [Parameter]
    public EventCallback<List<IBrowserFile>> FilesChanged { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of files allowed.
    /// </summary>
    /// <remarks>
    /// Default is null (unlimited).
    /// </remarks>
    [Parameter]
    public int? MaxFiles { get; set; }

    /// <summary>
    /// Gets or sets the maximum file size in bytes.
    /// </summary>
    /// <remarks>
    /// Default is 10MB (10485760 bytes).
    /// </remarks>
    [Parameter]
    public long MaxFileSize { get; set; } = 10485760; // 10MB default

    /// <summary>
    /// Gets or sets the accepted file types.
    /// </summary>
    /// <remarks>
    /// Examples: "image/*", ".pdf,.docx", "image/png,image/jpeg"
    /// </remarks>
    [Parameter]
    public string? Accept { get; set; }

    /// <summary>
    /// Gets or sets whether multiple files can be selected.
    /// </summary>
    [Parameter]
    public bool Multiple { get; set; }

    /// <summary>
    /// Gets or sets whether to show image previews.
    /// </summary>
    [Parameter]
    public bool ShowPreview { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show upload progress.
    /// </summary>
    [Parameter]
    public bool ShowProgress { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the upload is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether file selection is required.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically show validation errors from EditContext.
    /// </summary>
    [Parameter]
    public bool ShowValidationError { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the wrapper.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the input for form submission.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element that describes the input.
    /// </summary>
    [Parameter]
    public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets the drop zone text.
    /// </summary>
    [Parameter]
    public string DropZoneText { get; set; } = "Drop files here or click to browse";

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<List<IBrowserFile>?>>? ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets additional attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the CSS classes for the wrapper element.
    /// </summary>
    private string WrapperClass => ClassNames.cn("w-full", Class);

    /// <summary>
    /// Gets the CSS classes for the drop zone based on drag state and disabled status.
    /// </summary>
    private string DropZoneCssClass => ClassNames.cn(
        "relative rounded-lg border-2 border-dashed transition-colors",
        _isDragging
            ? "border-primary bg-primary/5"
            : "border-border hover:border-muted-foreground/50",
        Disabled ? "opacity-50 cursor-not-allowed" : "cursor-pointer"
    );

    /// <summary>
    /// Gets the CSS classes for the label element.
    /// </summary>
    private string LabelCssClass => ClassNames.cn(
        "block cursor-pointer",
        Disabled ? "cursor-not-allowed" : ""
    );

    /// <summary>
    /// Gets the CSS classes for individual file items in the list.
    /// </summary>
    private string FileCssClass => ClassNames.cn(
        "flex items-center gap-3 p-3 rounded-lg border border-border bg-card hover:bg-accent/50 transition-colors"
    );

    /// <summary>
    /// Initializes the component with default values.
    /// </summary>
    protected override void OnInitialized()
    {
        Files ??= new List<IBrowserFile>();
        Id ??= $"file-upload-{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Updates the field identifier and validation subscriptions when parameters change.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ShowValidationError && ValueExpression != null)
        {
            _fieldIdentifier = FieldIdentifier.Create(ValueExpression);

            if (EditContext != _previousEditContext)
            {
                DetachValidationStateChangedListener();
                EditContext?.OnValidationStateChanged += OnValidationStateChanged;
                _previousEditContext = EditContext;
            }
        }
    }

    /// <summary>
    /// Imports JavaScript modules and initializes validation display after rendering.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                _module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/NeoBlazorUI.Components/js/file-upload.js");

                if (ShowValidationError)
                {
                    _validationModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./_content/NeoBlazorUI.Components/js/input-validation.js");
                }
            }
            catch (JSException)
            {
                // JS module not available
            }
        }

        if (ShowValidationError && _validationModule != null)
        {
            await UpdateValidationDisplayAsync();
        }
    }

    /// <summary>
    /// Handles file selection from the input or drop zone.
    /// </summary>
    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        if (Disabled) return;

        _validationError = null;
        var selectedFiles = e.GetMultipleFiles(MaxFiles ?? int.MaxValue).ToList();
        
        if (!await ValidateFiles(selectedFiles))
            return;

        if (Multiple)
        {
            Files ??= new List<IBrowserFile>();
            Files.AddRange(selectedFiles);
            
            if (MaxFiles.HasValue && Files.Count > MaxFiles.Value)
            {
                Files = Files.Take(MaxFiles.Value).ToList();
            }
        }
        else
        {
            Files = selectedFiles.Take(1).ToList();
        }

        await NotifyFilesChanged();
        await SimulateUploadProgress();
        await GeneratePreviews();
    }

    /// <summary>
    /// Handles the drop event when files are dropped on the zone.
    /// </summary>
    private async Task HandleDrop(DragEventArgs e)
    {
        if (Disabled) return;
        
        _isDragging = false;
        StateHasChanged();
    }

    /// <summary>
    /// Handles the drag over event to allow dropping.
    /// </summary>
    private void HandleDragOver(DragEventArgs e)
    {
        if (Disabled) return;
        // Prevent default to allow drop
    }

    /// <summary>
    /// Handles the drag enter event to show visual feedback.
    /// </summary>
    private void HandleDragEnter(DragEventArgs e)
    {
        if (Disabled) return;
        
        _isDragging = true;
        StateHasChanged();
    }

    /// <summary>
    /// Handles the drag leave event to remove visual feedback.
    /// </summary>
    private void HandleDragLeave(DragEventArgs e)
    {
        if (Disabled) return;
        
        _isDragging = false;
        StateHasChanged();
    }

    /// <summary>
    /// Removes a file from the upload list.
    /// </summary>
    private async Task RemoveFile(int index)
    {
        if (Disabled || Files == null || index < 0 || index >= Files.Count)
            return;

        var file = Files[index];
        
        // Clean up preview URL
        if (_previewUrls.ContainsKey(file))
        {
            _previewUrls.Remove(file);
        }

        Files.RemoveAt(index);
        _uploadProgress.Remove(index);
        
        // Reindex upload progress
        var newProgress = new Dictionary<int, int>();
        foreach (var kvp in _uploadProgress.Where(p => p.Key > index))
        {
            newProgress[kvp.Key - 1] = kvp.Value;
        }
        _uploadProgress = newProgress;

        await NotifyFilesChanged();
    }

    /// <summary>
    /// Validates files against size, type, and count constraints.
    /// </summary>
    private async Task<bool> ValidateFiles(List<IBrowserFile> filesToValidate)
    {
        // Validate file count
        if (MaxFiles.HasValue)
        {
            var totalCount = (Files?.Count ?? 0) + filesToValidate.Count;
            if (totalCount > MaxFiles.Value)
            {
                _validationError = $"Maximum {MaxFiles.Value} file(s) allowed. You're trying to add {filesToValidate.Count} more to {Files?.Count ?? 0} existing.";
                return false;
            }
        }

        // Validate file sizes
        foreach (var file in filesToValidate)
        {
            if (file.Size > MaxFileSize)
            {
                _validationError = $"File '{file.Name}' exceeds maximum size of {FormatFileSize(MaxFileSize)}.";
                return false;
            }
        }

        // Validate file types
        if (!string.IsNullOrEmpty(Accept))
        {
            var acceptedTypes = Accept.Split(',').Select(t => t.Trim()).ToList();
            
            foreach (var file in filesToValidate)
            {
                if (!IsFileTypeAccepted(file, acceptedTypes))
                {
                    _validationError = $"File '{file.Name}' type is not accepted. Accepted types: {Accept}";
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a file's type matches the accepted types list.
    /// </summary>
    private bool IsFileTypeAccepted(IBrowserFile file, List<string> acceptedTypes)
    {
        foreach (var type in acceptedTypes)
        {
            if (type.StartsWith("."))
            {
                // Extension match
                if (file.Name.EndsWith(type, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            else if (type.Contains("/*"))
            {
                // Wildcard match (e.g., "image/*")
                var prefix = type.Split('/')[0];
                if (file.ContentType.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            else if (file.ContentType.Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                // Exact MIME type match
                return true;
            }
        }
        
        return false;
    }

    /// <summary>
    /// Simulates upload progress animation for visual feedback.
    /// </summary>
    private async Task SimulateUploadProgress()
    {
        if (!ShowProgress || Files == null) return;

        for (int i = 0; i < Files.Count; i++)
        {
            _uploadProgress[i] = 0;
        }

        StateHasChanged();

        // Simulate progress
        for (int progress = 0; progress <= 100; progress += ProgressIncrement)
        {
            await Task.Delay(ProgressDelayMs);
            
            for (int i = 0; i < Files.Count; i++)
            {
                _uploadProgress[i] = progress;
            }
            
            StateHasChanged();
        }
    }

    /// <summary>
    /// Generates Base64 preview URLs for image files.
    /// </summary>
    private async Task GeneratePreviews()
    {
        if (!ShowPreview || Files == null) return;

        foreach (var file in Files.Where(IsImageFile))
        {
            if (!_previewUrls.ContainsKey(file))
            {
                try
                {
                    var buffer = new byte[Math.Min(file.Size, MaxPreviewSize)];
                    using var stream = file.OpenReadStream(maxAllowedSize: MaxPreviewSize);
                    var bytesRead = await stream.ReadAsync(buffer.AsMemory());
                    
                    var base64 = Convert.ToBase64String(buffer, 0, bytesRead);
                    _previewUrls[file] = $"data:{file.ContentType};base64,{base64}";
                }
                catch
                {
                    // Preview generation failed, skip
                }
            }
        }
    }

    /// <summary>
    /// Determines if a file is an image based on its content type.
    /// </summary>
    private bool IsImageFile(IBrowserFile file)
    {
        return file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the preview URL for a file from the cache.
    /// </summary>
    private string GetPreviewUrl(IBrowserFile file)
    {
        return _previewUrls.TryGetValue(file, out var url) ? url : string.Empty;
    }

    /// <summary>
    /// Determines the icon type to display for non-image files.
    /// </summary>
    private string GetFileIconType(IBrowserFile file)
    {
        var contentType = file.ContentType.ToLowerInvariant();
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();

        if (contentType.Contains("pdf") || extension == ".pdf")
            return "pdf";
        
        if (contentType.Contains("word") || extension == ".doc" || extension == ".docx")
            return "word";

        if (contentType.Contains("excel") || contentType.Contains("spreadsheet") || extension == ".xls" || extension == ".xlsx")
            return "excel";

        return "default";
    }

    /// <summary>
    /// Formats a file size in bytes to a human-readable string (B, KB, MB, GB, TB).
    /// </summary>
    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        
        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Notifies parent components and validation context of file list changes.
    /// </summary>
    private async Task NotifyFilesChanged()
    {
        if (FilesChanged.HasDelegate)
        {
            await FilesChanged.InvokeAsync(Files);
        }

        if (ShowValidationError && EditContext != null && ValueExpression != null)
        {
            EditContext.NotifyFieldChanged(_fieldIdentifier);
        }

        StateHasChanged();
    }

    /// <summary>
    /// Handles validation state changes from the EditContext.
    /// </summary>
    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        _firstInvalidInputId = null;
        _hasShownTooltip = false;

        InvokeAsync(async () =>
        {
            await UpdateValidationDisplayAsync();
            StateHasChanged();
        });
    }

    /// <summary>
    /// Updates the validation error display via JavaScript interop.
    /// </summary>
    private async Task UpdateValidationDisplayAsync()
    {
        if (EditContext == null || _validationModule == null || string.IsNullOrEmpty(Id))
            return;

        try
        {
            var messages = EditContext.GetValidationMessages(_fieldIdentifier).ToList();
            var errorMessage = messages.FirstOrDefault();

            if (errorMessage != _currentErrorMessage)
            {
                _currentErrorMessage = errorMessage;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    var isFirstInvalid = _firstInvalidInputId == null;
                    
                    if (isFirstInvalid)
                    {
                        _firstInvalidInputId = Id;
                    }

                    if (isFirstInvalid && !_hasShownTooltip)
                    {
                        await _validationModule.InvokeVoidAsync("showValidationError", Id, errorMessage);
                        _hasShownTooltip = true;
                    }
                }
                else
                {
                    await _validationModule.InvokeVoidAsync("clearValidationError", Id);
                    
                    if (_firstInvalidInputId == Id)
                    {
                        _firstInvalidInputId = null;
                    }
                }
            }
        }
        catch (JSException)
        {
            // Ignore JS errors
        }
    }

    /// <summary>
    /// Detaches the validation state changed event listener from the previous EditContext.
    /// </summary>
    private void DetachValidationStateChangedListener()
    {
        if (_previousEditContext != null)
        {
            _previousEditContext.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }

    /// <summary>
    /// Disposes JavaScript resources and unsubscribes from validation events.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        DetachValidationStateChangedListener();
        
        if (_module != null)
        {
            try
            {
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore
            }
        }

        if (_validationModule != null)
        {
            try
            {
                await _validationModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore
            }
        }
    }
}
