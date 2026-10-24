using Microsoft.AspNetCore.Components;
using NeoUI.Blazor.Primitives.Services;

namespace NeoUI.Blazor.Primitives;

public partial class CheckboxPrimitive
{
    [Inject] private CheckboxKeyboardService Keyboard { get; set; } = default!;

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender) =>
        firstRender ? Keyboard.EnsureInitializedAsync() : Task.CompletedTask;
}
