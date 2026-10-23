# Browser interaction mode (preview)

`DropdownMenu`, `Popover`, `Select<TValue>`, `Input`, and the composed controls below accept an opt-in interaction mode:

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
application family setting → application global default → Blazor**. A nullable `InteractionMode` parameter distinguishes
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

Family settings are nullable and optional. Existing `DefaultMode` configurations retain their behavior.
For staged adoption, enable local floating UI without changing legacy fields or selection controls:

```csharp
builder.Services.ConfigureNeoUIInteractions(options =>
{
    options.DefaultMode = InteractionMode.Blazor;
    options.Floating = InteractionMode.Browser;
    options.Menus = InteractionMode.Browser;
    options.Disclosures = InteractionMode.Browser;
    options.Inputs = InteractionMode.Blazor;
    options.Selection = InteractionMode.Blazor;
});
```

Alternatively set `DefaultMode = Browser` and retain `Inputs = Blazor` and `Selection = Blazor`.
A null family setting falls back to `DefaultMode`. A scoped provider overrides every application
family setting; explicit component settings override the scope, including explicit Blazor.
Optionally limit a scope to one family with
`<InteractionModeProvider Family="InteractionFamily.Menus" Mode="InteractionMode.Browser">`.
Other families continue to inherit their nearest applicable outer scope or application policy.
Pill navigation uses this menu-only scope internally so its menu policy cannot opt embedded
form fields into Browser mode.

| Family | Supported controls |
| --- | --- |
| Floating | Popover, HoverCard, Tooltip; ordinary compositions such as ThemeSwitcher |
| Menus | DropdownMenu, Menubar, ContextMenu, NavigationMenu, SplitButton and SidebarPillNav |
| Selection | Select, Combobox, MultiSelect, DatePicker, DateRangePicker, TimePicker and ColorPicker shells |
| Inputs | Input, Textarea, InputGroup, formatted inputs, standalone Command, Checkbox, Switch, RadioGroup, Toggle/ToggleGroup, Slider/RangeSlider, InputOtp, DateInput/TimeInput, TagInput and MarkdownEditor |
| Disclosures | Accordion, Collapsible and Tabs |

A selection control forwards its resolved mode explicitly to its Popover and composed search UI.
`Floating = Browser` therefore does not enable Browser mode for selection controls. With
`Selection = Browser`, internal Combobox/MultiSelect searches follow that selection mode even
when standalone `Inputs = Blazor`; keep both Inputs and Selection in Blazor to preserve legacy
form behavior. Standalone inputs/Command inside an ordinary Popover honor an Inputs family
setting rather than silently inheriting its shell. Without an Inputs family setting in the application or an applicable family-specific scope they retain
the existing enclosing-Popover behavior, including explicit Blazor shell exceptions within a Browser scope. Checkbox/radio menu items follow Menus; form Checkbox/RadioGroup controls follow Inputs. A picker shell setting does not convert arbitrary
calendar/native/color editor contents into browser-owned data logic.

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
The floating shell, mounted-item hover/highlighting, and keyboard focus are browser-owned.
Filtering/search callbacks, value commits, validation, and bound value updates still use
the current Blazor renderer. Calendar focus/blur and day navigation within rendered content
are local; rendering a different month still requires the renderer.
They therefore still need a circuit in Interactive Server. Search focus and existing
keyboard handlers are preserved; MultiSelect's `CloseAsync()` clears search and restores
focus behavior on reopening. Application value resets remain declarative.

The Interaction Mode gallery includes live DatePicker, TimePicker, Combobox, and MultiSelect
examples under one Browser provider, with bound-value summaries and a shared reset.
`TimePicker`, `DateRangePicker` and `ColorPicker` accept nullable `InteractionMode` for their
Selection-family floating shells. They do not expose an `InteractionRevision` parameter. Its native time fields and Apply/Cancel actions remain Blazor handlers.

### Local item interaction audit

| Control | Browser-owned mechanics | Application/content work |
| --- | --- | --- |
| DropdownMenu / Select | Hover, focus, keyboard/typeahead, submenus, dismissal, optimistic selection feedback | Existing asynchronous action/value callbacks |
| Combobox / Command | Active item, hover, Arrow/Home/End navigation, active descendant ARIA, immediate dismissal | Search/filter callbacks and action/value commits |
| CommandVirtualizedGroup | Highlight/navigation among mounted items; local scroll requests the next window | Blazor Virtualize materializes unmounted rows or fetches data |
| MultiSelect | Hover, active item, keyboard navigation, Space/Enter routing, immediate Escape/trigger/outside dismissal | Selected values, query callbacks, and load-more work |
| DatePicker / Calendar | Native hover, focus/blur, active-day ARIA and navigation of rendered days | Asynchronous date commits, disabled-date rules, and new month content |
| TimePicker / ordinary buttons | Native focus/hover; immediate popover dismissal on Apply/Cancel | Time fields and committed values remain Blazor content |

`CommandContent` also accepts nullable `InteractionMode` for standalone command lists.
With an Inputs family setting, an explicit mode wins, followed by scoped and Inputs-family policy.
Without an Inputs setting it retains the enclosing Popover’s actual resolved mode, preserving
existing explicit Blazor shell exceptions even inside a Browser scope. Combobox explicitly forwards its Selection mode.
Composed controls may observe open/close asynchronously to synchronize their bindings;
local mechanics do not wait for those observations. Date selection, Combobox item selection,
and time/range Apply actions dismiss before their application callbacks complete.
`Calendar.CloseOnSelect` expresses the enclosing browser popover close intent; DatePicker
sets it automatically, while range calendars can keep the layer open until completion.
Custom actions inside a browser popover can declare `data-neo-close="true"` to dismiss
before their existing click callback runs. Native footer controls keep their own Enter/Space
behavior; Escape still dismisses the layer. IME composition keys do not navigate or dismiss. Hover and mounted-item navigation do
not invoke .NET. Ordinary Button/LinkButton controls attach hover/focus handlers only when
an actual trigger behavior subscribes. Scroll thresholds are checked in JavaScript; only
an eligible load-more request calls the application. Virtualization and calendar month
materialization remain content boundaries, so disconnected controls cannot invent rows
or dates whose application rules have not been rendered.

### Browser-owned input drafts

`Input` inherits the same interaction policy, with explicit `InteractionMode` overrides.
Command/Combobox and MultiSelect search fields use it when their enclosing control is in
Browser mode. Razor renders a fixed initial `value`; subsequent render batches therefore
cannot overwrite a live draft. Native typing, selection, caret, paste and IME stay local,
including while blurred. `UpdateOn=Input` sends versioned edit snapshots out of band;
`UpdateOn=Change` submits the final value on change/blur, and also sends a snapshot
when typing crosses between empty and non-empty. Further typing within the same state
sends **no callbacks**. For example, `"" → "a" → "ab" → "abc" → ""` sends `"a"` and `""`
while focused; blur sends a final value only if it has changed. Empty means length zero;
whitespace counts as non-empty. Composition sends a boundary only after it completes.
In Browser mode this lets a bound
`Disabled="@string.IsNullOrEmpty(name)"` update without waiting for blur. It does not make
the complete current draft available to Save; use the submission barrier below.
Debounce is local and flushes on blur.

Callbacks are serialized in edit order. Every delivered snapshot still runs the application
callback; debounce can reduce that work. A returned acknowledgement can change the DOM only
if its edit ID and command revision still match the latest draft. The bound `Value` remains
the accepted application value. Keep it unchanged to reject an edit, or assign a normalized
value to acknowledge a correction. Field-change validation runs after the callback.

```razor
<Input InteractionMode="InteractionMode.Browser"
       Value="@acceptedText" ValueChanged="AcceptTextAsync"
       ValueRevision="@revision" UpdateOn="InputUpdateMode.Input" />
```

Increment `ValueRevision` together with `Value` for an authoritative replacement or reset,
**even when resetting to the same accepted value**. Ordinary parameter echoes do not replace
browser drafts. A changed revision clears queued edits and invalidates in-flight
acknowledgements. An application callback already running cannot be cancelled by this UI
protocol; applications must guard their own asynchronous writes against a reset as needed.
Use `SearchRevision` on standalone CommandContent, or `InteractionRevision` on composed
controls, to explicitly reset search drafts.

Search callbacks are awaited through the input queue. Until the latest query finishes,
old items remain rendered but cannot be selected through the browser list controller;
`aria-busy` and `data-neo-search-pending` identify this state. The controller never renders
search results itself. Fire-and-forget application work is outside this guarantee: await
result assignment inside `SearchQueryChanged`, or implement request IDs/cancellation in
your data layer. A long queue still means old server requests take time; use debounce for
expensive searches. Set `SearchInterval="250"` on Combobox/CommandInput or on
Browser-mode MultiSelect to wait for a 250 ms typing pause before applying search.
These intervals default to 0; the Interaction Mode gallery opts into 250 ms for both composed searches.

`data-neo-input-pending` and the `neoui:input-state` event distinguish a draft from acceptance.
Failed delivery preserves the draft and marks `data-neo-input-uncertain`; queued callbacks
are discarded and are not automatically replayed on reconnect. A new user edit can retry after a failed delivery. An explicit
revision also releases new edits if an old acknowledgement never settles. Acknowledgement loss does not prove the server failed.
Rendered revision commands reconcile after reconnect without requiring a transient JS call.

The supported draft/field families and the RichTextEditor exception are listed in the expansion section below.

### Existing EditForm handlers: a one-tag opt-in

With the application default set to Browser, change only the form component tag from
`EditForm` to `BrowserEditForm` and optionally add synchronization feedback:

```csharp
builder.Services.ConfigureNeoUIInteractions(options =>
    options.DefaultMode = InteractionMode.Browser);
```

```razor
<BrowserEditForm EditContext="@editContext" OnValidSubmit="SaveAsync"
                 OnInvalidSubmit="ShowErrors" OnFlushFailed="ShowSynchronizationFeedback"
                 FormName="profile" class="profile-form" novalidate>
    <DataAnnotationsValidator />
    <ValidationSummary />
    <Input @bind-Value="model.Name" UpdateOn="InputUpdateMode.Input" />
    <Button Type="ButtonType.Submit">Save</Button>
</BrowserEditForm>
```

The wrapper renders a real EditForm and reuses `BrowserInputScope` to flush drafts before
validation. It **does not choose an interaction mode**: global/scoped defaults and explicit
input overrides continue to apply. Existing fields, validators, handlers and the explicit
EditContext are retained. `Model` is also supported, with a stable generated EditContext
until the model changes. `FormName`, `Enhance`, child-content context and form attributes
are forwarded; `Model`/`EditContext` and submit-handler combinations follow EditForm's rules.
This opt-in requires an interactive renderer; it is not a static SSR/enhanced POST handler.

| Existing EditForm behavior | BrowserEditForm behavior after successful draft synchronization |
| --- | --- |
| `OnValidSubmit` / `OnInvalidSubmit` | Calls `EditContext.Validate()` once, then invokes the matching existing callback |
| `OnSubmit` | Invokes the existing callback without implicit EditContext validation; the callback still owns validation |
| `OnSubmit` combined with valid/invalid handlers | Rejected, as with EditForm |
| `novalidate` / submitter `formnovalidate` | Retains the native-validation opt-out; EditContext validation behavior is unchanged |
| Native constraint failure | Reports native validity and suppresses submit callbacks; it does not become an `OnInvalidSubmit` or synchronization failure |

If a Server circuit is already known to be unavailable, the native submit is blocked
locally without locking the form or replaying the submit after reconnect. No server callback
can run during that outage: the form emits `neoui:form-flush-failed` with
`detail.status = "Uncertain"` and marks `data-neo-form-flush-status="Uncertain"` for client
feedback. A later deliberate submit can retry. A submission interrupted before .NET takes
the guard also releases on the documented connection-state hook. WebAssembly local
callbacks remain available offline.

Native constraint checks are deferred until after the final draft has been acknowledged,
so stale native/custom-validity messages cannot block synchronization. Automatic .NET
validation then sees the current model. A synchronization rejection, timeout, reset,
composition or uncertain delivery invokes `OnFlushFailed` **without** requesting validation
or invoking a submit handler. Failures are not automatically retried.

`IsSubmitting` and form `aria-busy` cover synchronization and the awaited handler. The
browser blocks duplicate click/Enter/`requestSubmit()` events; the .NET barrier also guards
programmatic submissions. Supported inputs remain locked through the awaited handler.
`BrowserEditForm.SubmitAsync()` uses the same path. Await all persistence in the existing
handler rather than starting fire-and-forget work. `FlushTimeoutMilliseconds` defaults to
30000. The barrier covers the supported text and shared data-field controllers listed below.
Select/Combobox/MultiSelect and menu selection queues remain independent, as do unsupported editors. Forms and scopes remain independent.

A standard EditForm cannot accept an asynchronous pre-validation extension: its validation
request is synchronous and its public `OnSubmit` is the validation-owning replacement.
Intercepting DOM submission and replaying it would also lose a reliable completion boundary
for the original awaited handler. BrowserEditForm is a thin adapter around those public APIs;
it does not patch framework internals or mutate an existing EditForm's parameters.
For the framework's callback distinction, see the [.NET EditForm implementation](https://github.com/dotnet/aspnetcore/blob/v10.0.0/src/Components/Web/src/Forms/EditForm.cs).

If keeping the literal `<EditForm>` tag is essential, the manual `BrowserInputScope` /
`OnSubmit` integration below remains supported. It requires the existing handler to call
the scope; putting a flush in `OnValidSubmit` is too late.

### Flush before validating or saving a form

Blur sends a final edit but does not wait for application callbacks. An ordinary Save
handler can therefore still read an earlier model value. Use `BrowserInputForm` for
forms containing browser-owned inputs:

```razor
<BrowserInputForm Model="@model" OnValidSubmit="SaveAsync"
                  OnFlushFailed="ShowSynchronizationFeedback">
    <DataAnnotationsValidator />
    <ValidationSummary />
    <Input @bind-Value="model.Name" UpdateOn="InputUpdateMode.Input" />
    <Input @bind-Value="model.Note" UpdateOn="InputUpdateMode.Input" DebounceDelay="500" />
    <Button Type="ButtonType.Submit">Save</Button>
</BrowserInputForm>
```

The wrapper opts its Input descendants into Browser mode (explicit overrides still win).
It captures and locks the scoped browser fields in one browser task, bypasses debounce,
awaits acknowledgements for that snapshot, checks native constraints and validates the updated EditContext, then invokes
`OnValidSubmit`. The lock lasts through the awaited Save callback. Save clicks, Enter,
`requestSubmit()` and `BrowserInputForm.SubmitAsync()` use the same path; concurrent
submits are ignored. `OnInvalidSubmit` follows the usual EditForm validation behavior.
A form reference exposes `IsSubmitting` throughout synchronization, validation and awaited
persistence. Inside the form's child content, bind the submit button's `Disabled` state
and an inline spinner to `form?.IsSubmitting == true`; the form rerenders that content
when submission starts and finishes. Close a containing dialog from the successful
`OnValidSubmit` handler after awaiting persistence. Invalid or uncertain submissions
leave the dialog available for correction.
`Model` and `EditContext` are mutually exclusive, as in EditForm. Include validators as
usual. Native pre-submit validation is deferred with `novalidate`, so an old validation
message cannot prevent the final draft from flushing. Native constraints are checked after
acknowledgement with the captured fields temporarily eligible for validation. This component requires an interactive renderer; it is not a static POST handler.

If a draft is adjusted (including rejection or normalization), changed, reset, composing,
uninitialized, disposed, uncertain, or timed out, persistence is not attempted. The form
calls `OnFlushFailed` with `InputFlushResult.Status` and the relevant `FieldId` when available.
An adjusted value is shown for review; a subsequent submit may save it. Delivery failures
are not retried automatically. `FlushTimeoutMilliseconds` defaults to 30000. Locks release
on completion/failure, removal, and rendered scope reconciliation after reconnect.
Server callbacks avoid known offline/reconnecting states using the documented .NET 10
[reconnect event and modal states](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/signalr?view=aspnetcore-10.0#reflect-the-server-side-connection-state-in-the-ui).
An application replacing that UI/handler should call `setConnectionAvailable(false/true)`
from `browser-input.js` in its down/up handlers. This guard is not applied to WebAssembly's
local .NET callbacks. A lost completion acknowledgement still blocks Save at the deadline.

For a custom flow that keeps the literal EditForm tag, use the reusable scope and the **OnSubmit** event:

```razor
<BrowserInputScope @ref="inputs">
    <EditForm EditContext="@editContext" OnSubmit="SubmitAsync" novalidate>
        <DataAnnotationsValidator />
        <Input InteractionMode="InteractionMode.Browser" @bind-Value="model.Name"
               UpdateOn="InputUpdateMode.Input" />
        <Button Type="ButtonType.Submit">Save</Button>
    </EditForm>
</BrowserInputScope>

@code {
    private BrowserInputScope? inputs;
    private async Task SubmitAsync(EditContext context)
    {
        var result = await inputs!.SubmitAsync(context, SaveAsync);
        if (!result.Success) ShowSynchronizationFeedback(result);
    }
}
```

Do not put a flush inside an existing `OnValidSubmit`: validation would already have run
against the earlier model. `BrowserInputScope.FlushAsync()` and `Input.FlushAsync()` are
available for custom flows, but only `SubmitAsync` holds the scope lock through Save.
A standalone Input flush requires Browser mode. For unsupported editors and independent
Select/Combobox/MultiSelect or menu selection queues, await application commits before saving. Bind ordinary form model values as drafts and validate on submit so a partial
first-character boundary update can enable actions without being rejected as a complete
field value. Nested scopes are independent and fields
explicitly using Blazor or unsupported editors retain their existing synchronization.
Neither a flush nor validation cancels business writes already in progress; application
callbacks must still respect revision/reset guards. Save callbacks must await persistence.
A native `form.submit()` bypasses submit events and this barrier; use `requestSubmit()` or
the component's `SubmitAsync()` instead.

### Removal during asynchronous floating initialization

FloatingPortal captures its owner's event bridge for each initialization attempt and checks
both portal and owner lifetimes after awaiting portal rendering, module import and controller
initialization. Removing an owner during a delayed import aborts initialization before the
disposed bridge reaches interop. A late controller result is disposed. Disposal exceptions
are ignored only when the portal or that owner bridge has ended; unrelated errors are not
masked. This also applies under the application-wide Browser default. The lifecycle browser
fixture delays the actual module request, removes the owner, then verifies an independent
callback and a freshly mounted control, including submenu actions.

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
The menu-family suite also supports WebKit with real touch taps:

```powershell
npx playwright install webkit
$env:NEOUI_BROWSER_ENGINE = 'webkit'
node --test menus.spec.mjs
Remove-Item Env:NEOUI_BROWSER_ENGINE
```

Touch taps own menu switching and dismissal; their synthetic hover events do not open
or close menus. Mouse hover and keyboard navigation retain their existing behavior.

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
node --test adoption.spec.mjs input.spec.mjs
```

To exercise the real gallery spotlight under an application Browser default:

```powershell
dotnet run --project demo/NeoUI.Demo.Server -p:CI=true --urls http://localhost:5185 --BrowserDefault=true
# In another terminal, from tests/browser:
$env:NEOUI_DEMO_URL = 'http://localhost:5185/components/browser-interactions'
$env:NEOUI_EXPECT_DEFAULT = 'Browser'
node --test --test-name-pattern='spotlight (preserves|acknowledges)' demo.spec.mjs
# Repeat with NEOUI_BROWSER_ENGINE=webkit for the WebKit engine.
```

The delayed-render regression withholds render completions and then releases them,
verifying that the accepted context query is acknowledged without relying on the child
Input receiving a cascading render first. Business callbacks are not replayed.

Neither check retries a failing query by refilling it. Coverage includes draft retention,
caret edits, repeated backspacing to empty, spaces during pending searches and result
selection with delayed circuit traffic. The gallery default remains Blazor unless the
review host opts in. In an affected application, inspect the open CommandInput's
`data-neo-input-config` attribute: Browser mode emits JSON configuration, while Blazor
mode omits it. Confirm the interactive host's configuration and explicit overrides
when the runtime mode differs from the intended policy.

The normal Server fixture leaves the library Blazor default untouched. The WASM fixture
opts into the Browser application default; circuit reconnection is skipped there because
WebAssembly has no server circuit.

Deterministic state regressions (including delayed/coalesced Open binding echoes and resets during a callback):

```powershell
dotnet run --project tests/NeoUI.InteractionStateTests
```


## Menu families and pill navigation

`Menubar`, `ContextMenu`, `NavigationMenu`, `SplitButton`, and `SidebarPillNav` accept nullable `InteractionMode` and `InteractionRevision`. Omitted settings follow the scoped/application policy; an explicit `InteractionMode.Blazor` remains an exception to a global Browser default.

The implementation is shared at the **primitive layer**: Menubar and ContextMenu adapt to the same DropdownMenu browser controller and selection queues. Styled components forward policy and render persistent selection indicators. Menubar switching/roving focus and NavigationMenu hover timers, panels, keyboard state, ARIA, outside dismissal and link dismissal use a browser coordinator. Application callbacks run after local feedback. Hovering a closed menubar does not open it; hovering a sibling while open switches locally. Disabled triggers are skipped. Native navigation still follows the application's router.

Checkbox/radio items in Menubar and ContextMenu support the same delayed/rejected selection acknowledgements and reset generations as DropdownMenu. Their checkbox items default to staying open. Increment the root `InteractionRevision` to close/reset a family or the checkbox/radio group's `InteractionRevision` to reset only a selection. No business callback is replayed when a rendered command is reconciled.

ContextMenu opens at the pointer position or with the ContextMenu key / Shift+F10. `OpenAt(x, y)` issues a versioned programmatic command. Its optional backdrop and scroll lock are local and released on dismissal/disposal. Coordinates are sent with the asynchronous `OnOpen` notification. The trigger remains available for ordinary left-click actions.

Pill button active feedback is optimistic and local. `OnClick` continues to run application logic asynchronously; use `InteractionRevision` for controlled resets/rejection. Route-based pill items retain NavLink's router-owned active state. `AsChild` content retains its own interactions; dropdown and SplitButton children inherit the pill's resolved policy, and individual controls can explicitly override it. CSS hover needs no server callback. The already-mounted **desktop** sidebar can be revealed immediately by the pill expand action; mobile expansion still uses the existing server-controlled Sheet. This change does not convert the entire Sidebar/Sheet system to Browser mode.

SplitButton's secondary menu inherits/overrides the policy. Both segments reflect its locally updated `aria-expanded` through CSS; the primary action still invokes application code.

Browser menu content and navigation panels remain mounted. Account for the initial rendering/DOM cost when repeating menus across many rows. The form input flush barrier does not await these menu selection queues.

Try the [menu-family gallery](../demo/NeoUI.Demo.Shared/Pages/Components/BrowserMenuFamilies.razor) in the Interaction Mode demo (`#menu-families`).


## Canonical values in composed text inputs

A browser acknowledgement must read the accepted model value rather than infer it from whether a child render has completed. Standard `@bind-Value` supplies `ValueExpression`; Browser Input reads that expression after the awaited callback. Manually composed inputs can supply `AcceptedValueAccessor="@(() => model.Name)"` alongside `Value`/`ValueChanged`. The accessor is read after application work, so it must return the actual canonical value, including rejection or normalization. Returning the submitted draft unconditionally would bypass rejection.

Command, MultiSelect search, DataView search and DynamicForm text fields supply their live context/model accessors. DataTable and FilterValue standard bindings use their binding expressions. Revision checks still invalidate acknowledgements from before a reset. This does not turn server acceptance into persistence confirmation.

The P1/P2 scope and consumer review for lazy/heavy tab panels are tracked in [the expansion notes](interaction-mode-sweep.md). The supported families and exceptions are detailed below.


## Expanded input and disclosure families

Textarea, InputGroupInput and InputGroupTextarea reuse Input's browser draft controller, including IME, debounce, emptiness boundaries, explicit ValueRevision resets and form flushing. NumericInput, CurrencyInput and MaskedInput use the same queue with typed canonical acceptance and a final blur/Save commit. Incomplete or unrepresentable numeric values remain editable but block Save; they cannot silently submit an older parsed value. Locale formatting that preserves the typed value does not require a second Save. Clamping, normalization or rejection that changes the value returns ValueAdjusted so the user can review it.

Checkbox, Switch, RadioGroup, Toggle, ToggleGroup, Slider, RangeSlider, InputOtp, DateInput, TimeInput and TagInput use the primitive shared field host. Checked/pressed state, mounted indicators, segment editing, OTP paste/focus and range preview run locally. Their asynchronous accepted values join BrowserInputScope and BrowserEditForm, including fields in floating portals. Use InteractionRevision for authoritative resets. Ordinary binding supplies canonical expressions automatically; manual asynchronous callbacks should supply the corresponding AcceptedValueAccessor (or AcceptedValuesAccessor / AcceptedTagsAccessor / AcceptedMinValueAccessor and AcceptedMaxValueAccessor).

TagInput filters static suggestions and handles tokenization, paste, hover and removal locally. OnSearchSuggestions remains application work, with the configured SuggestionDebounceMs (300 ms by default); stale search results cannot replace a newer query. Server Validate and TagsChanged can reject optimistic tags. Save tokenizes a remaining draft before taking the snapshot. A Razor TagTemplate requires an explicit Blazor exception because the browser cannot construct arbitrary Razor components:

```razor
<TagInput InteractionMode="InteractionMode.Blazor" TagTemplate="@customTemplate" />
```

MarkdownEditor uses the textarea draft/form adapter. Formatting shortcuts, toolbar actions, list continuation and undo/redo stay local; an explicit ValueRevision starts a new document and clears prior history. Markdown preview rendering and HTML sanitization remain server work. Revealing Preview does not mean that synchronization or rendering has finished.

HoverCard and Tooltip keep their pointer/focus timers, persistence and Escape dismissal local, while action-button callbacks and inner Blazor content remain live. Accordion and Collapsible share the primitive disclosure controller. Tabs share its keyboard/ARIA and controlled reset protocol, with manual/automatic activation and horizontal/vertical navigation.

### Review Tabs before inheriting a Browser default

Browser Tabs retain inactive panels. Hidden components still initialize, load data and retain subscriptions and memory. Evaluate lazy data loads, heavy grids/charts/editors and mount/unmount side effects before opting a tab set in. Keep such sets in Blazor mode when their lifecycle is intentional:

```razor
<Tabs InteractionMode="InteractionMode.Blazor">
    @* Heavy or intentionally lazy panels. *@
</Tabs>
```

### RichTextEditor audit boundary

RichTextEditor already types in Quill locally, but it does not join the new browser draft/form protocol in this sweep. Its 150 ms notification debounce is not an acceptance revision. The source audit found that delayed parent values can still reach OnParametersSetAsync and setHtml, which calls Quill setContents with API source. Callback suppression prevents loops; it does not prove that the value is current or preserve the user's latest selection/undo history. Do not treat global Browser mode or BrowserEditForm as fixing this editor. A separate versioned Delta/HTML and selection/undo integration is P3 and remains excluded from this change.


## Expansion regression commands

The default `npm test` in `tests/browser` now includes the expansion fixtures. Set `NEOUI_TEST_URL` to the Server test host and `NEOUI_EXPECT_DEFAULT=Browser` for its application-default configuration; omit the latter for scoped-policy coverage. Use `NEOUI_TEST_RENDERER=wasm` against the WASM test host and `NEOUI_BROWSER_ENGINE=webkit` for the expansion's WebKit cases. `npm run test:demo` uses `NEOUI_DEMO_URL` to verify the actual gallery, including mixed-control Save and desktop/mobile layout.

Focused regressions include blocked circuit traffic, canonical acknowledgements under render backpressure, empty-mask change boundaries, WebKit deletion across literals, unrepresentable numeric values, read-only snapshots, Markdown redo, public segmented FocusAsync, delayed/rejected changes, explicit resets, retained-circuit reconciliation, and removal during controller import. A debug WASM build and browser emulation do not establish Native AOT, Hybrid or physical-device coverage.


### Family-policy regression fixtures

The Server host supports `--FamilyPolicy=conservative` (global Browser with Inputs/Selection
Blazor) and `--FamilyPolicy=reverse` (global Blazor with Inputs/Selection Browser).
Run a host with either option, then run `family-policy.spec.mjs` with `NEOUI_TEST_URL`
pointing at that host and `NEOUI_FAMILY_POLICY` set to the matching policy name. The suite
checks fields, standalone Command, menus, disclosures, Select/Combobox/MultiSelect, picker
shells, ordinary nested inputs, explicit overrides, general scopes and nested family scopes.
With `NEOUI_FAMILY_POLICY` omitted it tests compatibility of existing composed Blazor
exceptions and explicit Inputs scopes against an ordinary host. Mode changes after mounting
still require recreation; family defaults are startup policy, not a live migration mechanism.
