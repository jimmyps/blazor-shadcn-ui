using NeoUI.Blazor.Primitives;

static void Require(bool value, string message) { if (!value) throw new Exception(message); }
var state = new BrowserFloatingState();
state.Sync(InteractionMode.Browser, false, false, 0);
state.OpenChanged = _ => Task.CompletedTask;
var initial = state.OpenVersion;
await state.ReportOpenAsync(true);
await state.ReportOpenAsync(false);
// Simulate a parent whose render batches arrive AFTER both callbacks have returned.
state.Sync(InteractionMode.Browser, true, false, 0);
state.Sync(InteractionMode.Browser, false, false, 0);
Require(state.OpenVersion == initial, "Late binding echoes must not create open commands.");
state.Sync(InteractionMode.Browser, true, false, 1);
Require(state.Open && state.OpenVersion == initial + 1, "A versioned command must override echoes.");

var coalesced = new BrowserFloatingState();
coalesced.Sync(InteractionMode.Browser, false, false, 0);
coalesced.OpenChanged = _ => Task.CompletedTask;
await coalesced.ReportOpenAsync(true);
await coalesced.ReportOpenAsync(false);
coalesced.Sync(InteractionMode.Browser, false, false, 0);
await coalesced.ReportOpenAsync(true);
coalesced.Sync(InteractionMode.Browser, true, false, 0);
var beforeClose = coalesced.OpenVersion;
coalesced.Sync(InteractionMode.Browser, false, false, 0);
Require(coalesced.OpenVersion == beforeClose + 1, "Coalesced unchanged echoes must not swallow a later application close.");

var release = new TaskCompletionSource();
state.OpenChanged = _ => release.Task;
var report = state.ReportOpenAsync(false);
state.Sync(InteractionMode.Browser, false, false, 2);
Require(!state.Open && state.OpenVersion == initial + 2, "Reset commands must apply during a pending callback.");
release.SetResult(); await report;

var selection = new BrowserMenuSelection();
selection.Sync("false", 0, 0);
var commitRelease = new TaskCompletionSource<string?>();
selection.Commit = _ => commitRelease.Task;
var pending = selection.CommitAsync("true");
selection.Sync("false", 1, 0);
commitRelease.SetResult("true");
Require(await pending == "false" && selection.Value == "false", "Late menu acknowledgements must not overwrite a reset.");
selection.Sync("true", 1, 1);
Require(selection.Value == "true", "Item revisions must issue authoritative resets.");
using (var events = new BrowserFloatingEvents())
{
    Require(!events.IsDisposed && !BrowserFloatingEvents.IsReferenceDisposed(events.Reference), "A live owner bridge must remain available for initialization.");
    events.Dispose();
    Require(events.IsDisposed && BrowserFloatingEvents.IsReferenceDisposed(events.Reference), "A disposed owner must be recognized even though its reference Value now throws.");
    events.Dispose();
}
Require(!BrowserFloatingEvents.IsReferenceDisposed(null), "A portal without an owner bridge remains valid.");
using (var unrelated = Microsoft.JSInterop.DotNetObjectReference.Create(new object()))
{
    unrelated.Dispose();
    Require(!BrowserFloatingEvents.IsReferenceDisposed(unrelated), "An unrelated disposed object must not be mistaken for an ended floating-owner lifetime.");
}
Console.WriteLine("Passed: late binding echoes, explicit commands during callbacks, stale menu acknowledgements, item resets, owner bridge lifetime checks.");
