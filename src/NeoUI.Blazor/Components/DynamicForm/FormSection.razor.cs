using Microsoft.AspNetCore.Components;

namespace NeoUI.Blazor;

/// <summary>Groups related form fields within a DynamicForm with an optional title, description, and collapsible toggle.</summary>
public partial class FormSection : ComponentBase
{
    /// <summary>Gets or sets the section heading.</summary>
    [Parameter] public string? Title { get; set; }
    /// <summary>Gets or sets a description shown below the heading.</summary>
    [Parameter] public string? Description { get; set; }
    /// <summary>Gets or sets whether the section content can be collapsed.</summary>
    [Parameter] public bool Collapsible { get; set; }
    /// <summary>Gets or sets whether a collapsible section starts expanded.</summary>
    [Parameter] public bool DefaultExpanded { get; set; } = true;
    /// <summary>Gets or sets the child content (fields).</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
