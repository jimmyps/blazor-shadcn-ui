using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Primitives.Popover;

/// <summary>
/// Primitive PopoverArrow component that automatically positions itself to point at the trigger element.
/// </summary>
public partial class PopoverArrow : IAsyncDisposable
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    /// <summary>
    /// Gets or sets the width of the arrow in pixels.
    /// Default is 16.
    /// </summary>
    [Parameter]
    public int Width { get; set; } = 16;

    /// <summary>
    /// Gets or sets the height of the arrow in pixels.
    /// Default is 8.
    /// </summary>
    [Parameter]
    public int Height { get; set; } = 8;

    /// <summary>
    /// Gets or sets the minimum distance from popover edges in pixels.
    /// Default is 8.
    /// </summary>
    [Parameter]
    public int EdgePadding { get; set; } = 8;

    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets additional attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the PopoverContext from cascade.
    /// </summary>
    [CascadingParameter]
    private PopoverContext Context { get; set; } = null!;

    protected ElementReference _arrowRef;
    protected string _arrowId = string.Empty;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<PopoverArrow>? _dotNetRef;
    private bool _isInitialized = false;
    protected string _positionStyle = "visibility: hidden;";

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Context == null)
        {
            throw new InvalidOperationException(
                "PopoverArrow must be used within a Popover component. " +
                "Ensure PopoverArrow is a child of PopoverContent.");
        }

        _arrowId = $"popover-arrow-{Guid.NewGuid():N}";
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Context.IsOpen && !_isInitialized)
        {
            _isInitialized = true;
            await InitializeArrowPositioningAsync();
        }
    }

    private async Task InitializeArrowPositioningAsync()
    {
        try
        {
            // Load JS module
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/NeoBlazorUI.Primitives/Components/Popover/popover-arrow.js");

            // Create .NET object reference for callbacks
            _dotNetRef = DotNetObjectReference.Create(this);

            // Calculate initial position
            await UpdateArrowPositionAsync();

            // Set up observers for dynamic updates
            if (Context.State.TriggerElement != null)
            {
                var side = ExtractSideFromPlacement();
                await _jsModule.InvokeVoidAsync("observeArrowPosition",
                    Context.ContentId,
                    Context.State.TriggerElement.Value,
                    _arrowId,
                    _dotNetRef,
                    new
                    {
                        width = Width,
                        height = Height,
                        edgePadding = EdgePadding,
                        side = side
                    });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize arrow positioning: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the arrow position. Called from JavaScript.
    /// </summary>
    [JSInvokable]
    public async Task UpdatePosition()
    {
        await UpdateArrowPositionAsync();
        StateHasChanged();
    }

    private async Task UpdateArrowPositionAsync()
    {
        if (_jsModule == null || Context.State.TriggerElement == null)
        {
            return;
        }

        try
        {
            // Get the side from the placement if available
            var side = ExtractSideFromPlacement();

            // Calculate position from JS
            var position = await _jsModule.InvokeAsync<ArrowPosition>("calculateArrowPosition",
                Context.ContentId,
                Context.State.TriggerElement.Value,
                _arrowId,
                new
                {
                    width = Width,
                    height = Height,
                    edgePadding = EdgePadding,
                    side = side
                });

            // Apply position styles
            ApplyArrowPosition(position);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update arrow position: {ex.Message}");
        }
    }

    private string? ExtractSideFromPlacement()
    {
        // Extract base side from placement (e.g., "top-start" -> "top")
        var placement = Context.State.Position?.Placement;
        if (string.IsNullOrEmpty(placement))
        {
            return null;
        }

        if (placement.StartsWith("top")) return "top";
        if (placement.StartsWith("bottom")) return "bottom";
        if (placement.StartsWith("left")) return "left";
        if (placement.StartsWith("right")) return "right";

        return null;
    }

    private void ApplyArrowPosition(ArrowPosition position)
    {
        var alignOffset = position.AlignOffset;

        // Calculate position and rotation based on side
        _positionStyle = position.Side switch
        {
            "top" => $"bottom: -{Height}px; left: {alignOffset}px; transform: translateX(-50%) rotate(180deg); visibility: visible;",
            "bottom" => $"top: -{Height}px; left: {alignOffset}px; transform: translateX(-50%); visibility: visible;",
            "left" => $"right: -{Height}px; top: {alignOffset}px; transform: translateY(-50%) rotate(-90deg); visibility: visible;",
            "right" => $"left: -{Height}px; top: {alignOffset}px; transform: translateY(-50%) rotate(90deg); visibility: visible;",
            _ => $"top: -{Height}px; left: {alignOffset}px; transform: translateX(-50%); visibility: visible;"
        };
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsModule != null && _isInitialized)
            {
                await _jsModule.InvokeVoidAsync("cleanup", _arrowId);
                await _jsModule.DisposeAsync();
            }

            _dotNetRef?.Dispose();
        }
        catch
        {
            // Ignore disposal errors
        }
    }

    private class ArrowPosition
    {
        public string Side { get; set; } = "bottom";
        public double AlignOffset { get; set; }
    }
}
