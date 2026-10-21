namespace NeoUI.Blazor.Primitives;

// One independent transaction stream per checkbox or radio group.
internal sealed class BrowserMenuSelection
{
    internal string Id { get; } = $"menu-selection-{Guid.NewGuid():N}";
    internal string? Value { get; set; }
    internal long Version { get; private set; }
    internal bool Pending { get; private set; }
    internal Func<string?, Task<string?>>? Commit { get; set; }
    private bool initialized;
    private string? parameter;
    private long rootRevision, itemRevision;

    internal void Sync(string? value, long root, long item)
    {
        var reset = root != rootRevision || item != itemRevision;
        if (!initialized || reset || (!Pending && value != parameter))
        {
            Value = value;
            Version++;
        }
        parameter = value;
        rootRevision = root;
        itemRevision = item;
        initialized = true;
    }

    internal async Task<string?> CommitAsync(string? proposed)
    {
        var version = Version;
        Pending = true;
        try
        {
            var accepted = Commit == null ? Value : await Commit(proposed);
            if (Version == version) Value = accepted;
            return Value;
        }
        finally { Pending = false; }
    }
}
