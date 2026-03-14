namespace NeoUI.Blazor;

/// <summary>Represents the checked state of a tree node when <see cref="TreeView{TItem}.PropagateChecks"/> is enabled.</summary>
public enum CheckStateKind
{
    Unchecked,
    Checked,
    /// <summary>Some but not all descendants are checked.</summary>
    Indeterminate,
}
