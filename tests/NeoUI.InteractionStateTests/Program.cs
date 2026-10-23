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

var options = new InteractionOptions();
foreach (var family in Enum.GetValues<InteractionFamily>())
{
    Require(options.Resolve(null, null, family) == InteractionMode.Blazor, $"{family}: library default must remain Blazor.");
    options.DefaultMode = InteractionMode.Browser;
    Require(options.Resolve(null, null, family) == InteractionMode.Browser, $"{family}: omitted override inherits global.");
    options.DefaultMode = InteractionMode.Blazor;
}
options.Floating = options.Menus = options.Disclosures = InteractionMode.Browser;
options.Inputs = options.Selection = InteractionMode.Blazor;
foreach (var family in Enum.GetValues<InteractionFamily>())
{
    var expected = family is InteractionFamily.Inputs or InteractionFamily.Selection ? InteractionMode.Blazor : InteractionMode.Browser;
    Require(options.Resolve(null, null, family) == expected, $"{family}: independent family default.");
    Require(options.Resolve(null, new(InteractionMode.Browser), family) == InteractionMode.Browser, $"{family}: scoped override wins.");
    Require(options.Resolve(InteractionMode.Blazor, new(InteractionMode.Browser), family) == InteractionMode.Blazor, $"{family}: explicit Blazor wins.");
    Require(options.Resolve(InteractionMode.Browser, new(InteractionMode.Blazor), family) == InteractionMode.Browser, $"{family}: explicit Browser wins.");
}
options.DefaultMode = InteractionMode.Browser;
Require(options.Resolve(null, null, InteractionFamily.Inputs, InteractionMode.Browser) == InteractionMode.Blazor, "Input family must not be bypassed by a Browser popover.");
options.Inputs = null;
Require(options.Resolve(null, null, InteractionFamily.Inputs, InteractionMode.Blazor) == InteractionMode.Blazor, "Without a family setting retain composed Blazor exception compatibility.");
Require(options.Resolve(null, null, InteractionFamily.Inputs) == InteractionMode.Browser, "Clearing a family override restores the global default.");
Console.WriteLine("Passed: family isolation, global fallback, explicit and scoped precedence, composed input exceptions.");

var outerScope = new InteractionPolicy(InteractionMode.Blazor);
var menuScope = new InteractionPolicy(InteractionMode.Browser, InteractionFamily.Menus, outerScope);
Require(options.Resolve(null, menuScope, InteractionFamily.Menus) == InteractionMode.Browser, "Nearest family scope overrides enclosing general scope.");
Require(options.Resolve(null, menuScope, InteractionFamily.Inputs) == InteractionMode.Blazor, "Unrelated scope families fall through to the enclosing scope.");
Require(options.Resolve(null, new(InteractionMode.Browser, InteractionFamily.Menus), InteractionFamily.Inputs) == options.DefaultMode, "A menu-only scope must not opt unrelated fields in.");
Console.WriteLine("Passed: family-scoped policies preserve unrelated fields and nested scope fallback.");

var rejectedOpen = new BrowserFloatingState();
rejectedOpen.Sync(InteractionMode.Browser, false, false, 0);
rejectedOpen.OpenChanged = _ => Task.CompletedTask;
await rejectedOpen.ReportOpenAsync(true);
rejectedOpen.Sync(InteractionMode.Browser, false, false, 0);
var beforeLaterOpen = rejectedOpen.OpenVersion;
rejectedOpen.Sync(InteractionMode.Browser, true, false, 0);
Require(rejectedOpen.Open && rejectedOpen.OpenVersion == beforeLaterOpen + 1, "Unaccepted observations must not swallow later application open commands.");
Console.WriteLine("Passed: rejected observations retire their pending echoes.");

var renderedReject = new BrowserFloatingState();
renderedReject.Sync(InteractionMode.Browser, false, false, 0);
renderedReject.OpenChanged = _ => { renderedReject.Sync(InteractionMode.Browser, false, false, 0); return Task.CompletedTask; };
await renderedReject.ReportOpenAsync(true);
await renderedReject.ReportOpenAsync(false);
var rejectedVersion = renderedReject.OpenVersion;
renderedReject.Sync(InteractionMode.Browser, true, false, 0);
Require(renderedReject.Open && renderedReject.OpenVersion == rejectedVersion + 1, "Parent renders during rejected observations must not retain obsolete echo tokens.");
Console.WriteLine("Passed: callback-time rejection renders do not mask later commands.");
