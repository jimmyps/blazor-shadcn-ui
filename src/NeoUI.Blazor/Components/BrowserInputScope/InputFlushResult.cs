using System.Text.Json.Serialization;

namespace NeoUI.Blazor;

/// <summary>The outcome of synchronizing browser-owned input drafts.</summary>
public sealed record InputFlushResult
{
    /// <summary>The synchronization or submission outcome.</summary>
    public InputFlushStatus Status { get; init; }
    /// <summary>The input that prevented synchronization, when available.</summary>
    public string? FieldId { get; init; }
    /// <summary>Whether the captured drafts were acknowledged without correction.</summary>
    public bool Success => Status == InputFlushStatus.Success;
}

/// <summary>Flush failures never authorize persisting a stale model.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<InputFlushStatus>))]
public enum InputFlushStatus
{
    Success, NotReady, Composing, ValueAdjusted, Changed, Reset, Uncertain,
    TimedOut, Disposed, Busy, Invalid, NotSupported
}
