# Browser interaction mode (preview)

`DropdownMenu`, `Popover` and `Select<TValue>` now accept an opt-in interaction mode:

```razor
@using NeoUI.Blazor.Primitives

<Select TValue="string" InteractionMode="InteractionMode.Browser" @bind-Value="country">
    <SelectTrigger><SelectValue Placeholder="Country" /></SelectTrigger>
    <SelectContent>
        <SelectItem Value="@("id")" Text="Indonesia" />
        <SelectItem Value="@("th")" Text="Thailand" />
    </SelectContent>
</Select>
```

The default remains `InteractionMode.Blazor`. Browser is an interaction policy, not a new
Blazor render mode. It supports Interactive Server and WebAssembly. Initialization still
requires an interactive render; static SSR-only pages are not supported by this preview.
Auto and Hybrid hosting have not been separately runtime-verified.

## Application defaults and scoped exceptions

The precedence is **explicit component setting → nearest InteractionModeProvider →
application default → Blazor**. A nullable `InteractionMode` parameter distinguishes
omission (`null`, inherit) from explicit `InteractionMode.Blazor`.

```csharp
using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives;
using NeoUI.Blazor.Primitives.Extensions;

builder.Services.AddNeoUIPrimitives();
builder.Services.AddNeoUIComponents();
builder.Services.ConfigureNeoUIInteractions(options =>
    options.DefaultMode = InteractionMode.Browser);
```

```razor
<InteractionModeProvider Mode="InteractionMode.Blazor">
    <Select TValue="string">@* Inherits Blazor in this scope. *@</Select>
    <Popover InteractionMode="InteractionMode.Browser">@* Explicit override wins. *@</Popover>
    <InteractionModeProvider Mode="InteractionMode.Browser">
        @* Omitted settings inherit the nearest scope, including through floating portals. *@
    </InteractionModeProvider>
</InteractionModeProvider>
```

Configure the policy before mounting controls. Changing an effective mode after initialization
requires recreating that root/subtree with `@key`; existing controllers do not migrate in place.
The library default is unchanged for applications that do not opt in.

Select's BottomSheet presentation uses Drawer behavior and requires an explicit
`InteractionMode.Blazor` exception. Browser mode supports the floating presentation;
unsupported combinations fail clearly rather than leaving a trigger without a controller.

## Composed controls

`DatePicker`, `Combobox<TItem>`, and `MultiSelect<TItem>` accept the same nullable
`InteractionMode` and `InteractionRevision` parameters and forward them to their Popover.
They inherit application/scoped defaults and can opt out explicitly with Blazor.
Only the floating shell is browser-owned: calendar actions, filtering/search callbacks,
selection, validation, and bound value updates still use the current Blazor renderer.
They therefore still need a circuit in Interactive Server. Search focus and existing
keyboard handlers are preserved; MultiSelect's `CloseAsync()` clears search and restores
focus behavior on reopening. Application value resets remain declarative.

## Ownership

Razor owns composition, items, item labels, disabled configuration, and application bindings.
The existing PortalService owns each layer's DOM location. The controller never moves,
clones, removes, or renders Razor's child content. JS exclusively manages transient
visibility, positioning, data-state, focus, selection ARIA, and an empty selected-text span.
Razor omits browser-owned attributes where possible; the controller reconciles menu
checked-state presentation after declarative renders. Custom attributes must not override those interaction attributes.

Browser content is always mounted, even with `ForceMount=false`. This increases initial
DOM and component initialization costs. Data unavailable until a server fetch cannot be
made immediately available by this mode; supply loading content and update declaratively.

## Local interactions and server callbacks

Opening, closing, outside pointer dismissal, Escape, arrow/Home/End navigation, typeahead,
focus, submenu navigation, and resize/scroll positioning execute in JavaScript. Without
subscribed notifications they produce no .NET interaction messages. Initial setup and
explicit Razor updates still use JS interop in Interactive Server.

Plain actions, native links, nested submenus, checkbox items, and radio groups are
supported. Action dismissal is local; the action callback remains a Blazor event.
Checkboxes default to staying open, and radios default to closing. `CloseOnSelect=false`
supports multiple changes without dismissing the menu. Arrow/Home/End and typeahead move
focus; Space/Enter activate the focused item; disabled items cannot commit.

Checkbox/group callbacks are serialized per state unit, while local `aria-checked`,
`data-state`, and indicators update immediately. Different filters can commit independently.
Controlled `CheckedChanged` / `ValueChanged` parents accept by assigning the proposed value,
reject by retaining the existing value, or normalize to another value. Observation callbacks
`OnCheckedChange` / `OnValueChange` are awaited too. Failed commits restore the last
acknowledged state unless a newer local choice exists. Each item's/group's
`InteractionRevision`, or the root menu's revision, can force a controlled reset and
invalidate queued proposals and late acknowledgements. This does not cancel application
work: guard writes with your own revision/cancellation logic when resetting during a request.

`data-neo-pending` marks checkbox/radio items with outstanding proposals. A bubbling
`neoui:commit` event reports the proposed and accepted values after completion; a successful
callback can still report a rejected proposal (accepted differs from proposed). Local
`neoui:valuechange` is optimistic feedback, not evidence of a successful save.
Both packages must be released together; update the packing-time primitive package reference
to the newly released primitive version when preparing a release.

Select updates its displayed value and indicator and closes locally, then sends a stable,
opaque option ID to .NET. The server resolves that ID against the live item registry and
checks the select/item disabled state; generic values are never round-tripped as strings.
`ValueChanged` and `OnValueChange` are awaited in a serialized commit queue. A controlled
parent may accept, normalize, or reject a proposal. The latest acknowledgement updates the
browser; an older acknowledgement cannot overwrite a more recent local selection.
An application callback failure emits `neoui:error` and restores the last acknowledged
selection if no newer selection exists. A pending network call does not block local UI.

`OpenChanged`, `OnOpenChange`, outside-click, Escape, and content-ready callbacks are
optional observations after local behavior. They are not cancelable gates. Open callbacks
are serialized; binding echoes do not replay obsolete open/closed commands, including
parent renders scheduled after a callback finishes. When using controlled `Open` together
with observations, increment `InteractionRevision` for imperative commands that can overlap
those echoes; a bare boolean cannot distinguish such a command from an identical binding echo.

`Open` changes from application code issue explicit commands. Unchanged parameters on an
unrelated parent render preserve browser state. Increment `InteractionRevision` to reapply
`Open` and Select/menu authoritative values even when those parameters are unchanged (for
example, a reset to the original value). Explicit selection commands invalidate older
pending client acknowledgements; they do not cancel application work already on the server.
Changing `InteractionMode` after initialization requires recreating the root with `@key`.

Browser triggers handle click mechanics locally; custom trigger `OnClick` and
`CustomClickHandling` on primitive trigger components are rejected. Put business actions
on content items. NeoUI `Button` supports `AsChild` without focus/hover callbacks crossing
the circuit. A custom AsChild trigger must apply TriggerContext IDs and honor BrowserOwned and Disabled.

The controller also emits bubbling native events on the trigger: `neoui:openchange`,
`neoui:valuechange` (opaque option ID and client sequence), `neoui:outside`, `neoui:escape`,
`neoui:ready`, and `neoui:error`. Applications can use them for client integration. This
feature adds no HTTP endpoints or data transport; existing Blazor callbacks remain the
application integration. A fetch/Minimal API adapter can be layered on separately.

The existing Floating UI dependency is reused; applications should preload/bundle it as
usual to avoid an initial CDN fetch. Local interaction does not eliminate browser layout
or JavaScript loading cost. This preview uses immediate hide/show rather than preserving
the legacy exit-animation lifecycle.

## Circuit interruption and recovery

Immediate local feedback does **not** mean the server accepted or persisted a change.
Keep pending/error status visible for business-important filters and saves. NeoUI does not
add an AJAX endpoint, offline store, durable request queue, or an automatic retry policy.

Once initialized, opening/dismissal/focus/navigation can continue without a circuit. Blazor's
reconnection overlay may deliberately block pointer input; the application controls that UX.
Server-backed child controls cannot complete while disconnected. An interop request may
remain pending through a temporary outage; if the bridge reports failure, the latest local
proposal rolls back and emits `neoui:error`. A request already delivered to the server may
complete even if its acknowledgement is lost. Do not interpret an error as proof that no
write occurred, and do not blindly retry non-idempotent operations.

On a retained-circuit reconnect, existing controllers remain and outstanding interop responses
can settle. Versioned controller settings are carried in the portal's rendered attributes:
when a resumed render batch contains an `InteractionRevision` reset, the existing controller
reconciles its open/selection commands directly from that snapshot. This does not require a
separate post-render JS interop call, and unchanged command versions preserve local state.
No library replay of completed business callbacks is performed. Refresh authoritative
application state and increment `InteractionRevision` when you need to reconcile an uncertain
result. If Blazor cannot resume the circuit, reload/recreate the interactive tree; a full reload
loses transient browser state. Applications should use idempotency keys/version checks for
writes that must tolerate retries. Regression coverage drops a real SignalR WebSocket during
a pending commit, reconnects to the retained circuit, checks completion without duplication,
and then verifies an explicit reset closes the menu without an extra business callback, followed
by another interaction. This scenario runs three times by default (`NEOUI_RECONNECT_RUNS`
can increase the repetitions). A deterministic controller case also verifies render-only reset
delivery and initialization from the latest rendered snapshot. Another circuit case buffers
a server-side reset while disconnected and verifies reconciliation on resume.

## Persistent content: initial render and DOM cost

Browser mode requires the content to exist before the first click. `ForceMount=false` is
therefore overridden: large lists, nested menus, and child components initialize while hidden.
Closing keeps component state, subscriptions, DOM nodes, and controller/listener allocations
alive until disposal. Server child renders can still update hidden contents. Compared with a
Blazor control configured to mount on demand, this increases initial render batches,
server component memory, browser DOM, and setup work. In these fixtures portals mount after
interactive initialization, so initial HTML bytes alone do not capture that extra content;
the test also measures incoming SignalR frame bytes. It is not virtualization.

The regression fixture at `/?cost=browser` uses a 1,000-option Select plus 25 row menus with
10 items each. It verifies all 1,250 entries are mounted while closed; `/?cost=blazor` with
`ForceMount=false` mounts none initially. The test prints element counts, initial HTML bytes, incoming server frame bytes,
and setup duration. Timings are diagnostic, not portable benchmark thresholds. Run these
fixtures with representative merchant data and row counts before opting in broadly.

One local Server run produced 65 elements with mount-on-demand versus 5,421 with Browser,
and approximately 69 KB versus 1.97 MB of incoming SignalR frames during setup. These are
fixture observations, not a general size or timing guarantee.

For large lists or repeated table-row menus, use explicit Blazor exceptions with
`ForceMount=false`, page/filter options, virtualize table rows, or render a shared contextual
menu for the active row. A globally enabled Browser policy is convenient but should not force
thousands of hidden floating trees into a grid. Conditionally creating content after a server
open callback necessarily gives up immediate access to that content; show a small loading
shell if that trade-off is appropriate.

## Regression hosts

Start the Interactive Server fixture:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project tests/NeoUI.BrowserTests.Host -p:CI=true --urls http://localhost:5198
```

In another terminal:

```powershell
cd tests/browser
npm ci
npm test
```

Tests default to installed Edge. Set `NEOUI_BROWSER_CHANNEL=chrome` to use Chrome. The
suite preloads locally installed Floating UI, avoiding CDN access in regression runs.
To exercise the shared fixture under WebAssembly:

```powershell
dotnet run --project tests/NeoUI.BrowserTests.Wasm -p:CI=true --urls http://localhost:5199
# In the test terminal:
$env:NEOUI_TEST_URL = 'http://localhost:5199'
$env:NEOUI_TEST_RENDERER = 'wasm'
npm test
```

The suite checks zero circuit traffic for unobserved UI mechanics, delayed/rapid commits,
rejection, resets, declarative item changes, parent rerenders, nested focus, legacy behavior,
blocked outbound circuit traffic, and positioning/disposal races. Full cross-browser,
screen-reader, Auto and Hybrid verification remains release work.

## Interactive demo and named tunnel

The shared demo gallery includes `/components/browser-interactions`. It compares browser
and standard Blazor menus, demonstrates delayed/rejected Select commits, and displays
native browser events without adding .NET observation callbacks. The demo self-hosts its
Floating UI preload and uses the shared gallery layout, theme controls, and Preview/Code examples.
Find it under **Interaction Mode** (immediately after Components) in the sidebar and component index.

Build the Interactive Server gallery after restoring its dependencies:

```powershell
dotnet build demo/NeoUI.Demo.Server -p:CI=true
```

The NeoUI preview uses its own named tunnel, `neoui-dev`, with anonymous access on port
5184. The launcher requires the tunnel and port to have been provisioned first; it does
not change tunnel access policy. Start or reuse its server and tunnel in hidden processes:

```powershell
./scripts/start-browser-demo.ps1
# Optional: -TunnelId <another-provisioned-tunnel> -Port <matching-local-port>
```

The launcher prints the public demo URL, process IDs, and log location under
`$env:TEMP/neoui-browser-demo`. The server and tunnel remain running after the launcher
returns. Other existing tunnels are unaffected.

To verify the page (with the server running):

```powershell
cd tests/browser
node --test demo.spec.mjs
# Set NEOUI_DEMO_URL to the tunnel's full /components/browser-interactions URL
# to run the same smoke test through the public tunnel.
```

To verify the application Browser default on Server, run a second fixture:

```powershell
dotnet run --project tests/NeoUI.BrowserTests.Host -p:CI=true --urls http://localhost:5200 --BrowserDefault=true
# In another terminal, from the repository root:
cd tests/browser
$env:NEOUI_TEST_URL = 'http://localhost:5200'
$env:NEOUI_EXPECT_DEFAULT = 'Browser'
node --test adoption.spec.mjs
```

The normal Server fixture leaves the library Blazor default untouched. The WASM fixture
opts into the Browser application default; circuit reconnection is skipped there because
WebAssembly has no server circuit.

Deterministic state regressions (including delayed/coalesced Open binding echoes and resets during a callback):

```powershell
dotnet run --project tests/NeoUI.InteractionStateTests
```
