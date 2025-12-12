using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Components.Empty;

/// <summary>
/// A title component for an empty state.
/// </summary>
public partial class EmptyTitle : ComponentBase
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string CssClass => ClassNames.cn(
        "mt-4 text-lg font-semibold",
        Class
    );
}
