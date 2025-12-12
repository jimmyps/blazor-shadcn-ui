using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// A description component for an empty state.
/// </summary>
public partial class EmptyDescription : ComponentBase
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "mb-4 mt-2 text-sm text-muted-foreground",
        Class
    );
}
