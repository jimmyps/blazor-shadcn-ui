namespace NeoUI.Blazor.Primitives;

// Identifies primitive adapters that share the dropdown browser protocol.
internal sealed class BrowserMenuFamily
{
    internal Action<double, double>? ContextPosition { get; set; }
    internal long Revision { get; set; }
    internal bool Enabled { get; set; }
}
