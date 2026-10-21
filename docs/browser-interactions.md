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

## Ownership

Razor owns composition, items, item labels, disabled configuration, and application bindings.
The existing PortalService owns each layer's DOM location. The controller never moves,
clones, removes, or renders Razor's child content. JS exclusively manages transient
visibility, positioning, data-state, focus, selection ARIA, and an empty selected-text span.
Razor omits those browser-owned attributes in Browser mode so later render diffs don't
compete with them. Custom attributes must not override those interaction attributes.

Browser content is always mounted, even with `ForceMount=false`. This increases initial
DOM and component initialization costs. Data unavailable until a server fetch cannot be
made immediately available by this mode; supply loading content and update declaratively.

## Local interactions and server callbacks

Opening, closing, outside pointer dismissal, Escape, arrow/Home/End navigation, typeahead,
focus, submenu navigation, and resize/scroll positioning execute in JavaScript. Without
subscribed notifications they produce no .NET interaction messages. Initial setup and
explicit Razor updates still use JS interop in Interactive Server.

Plain action and link menu items and nested submenus are supported. Native links retain
normal browser navigation. An action item's callback is still a Blazor event; dismissal
happens before that callback completes. `CloseOnSelect=false` is respected. Checkbox and
radio menu items explicitly reject Browser mode in this preview: retain Blazor mode for
those menus until optimistic checked-state reconciliation is implemented. Both the primitives and styled
packages must be released together; update the packing-time primitive package reference to
the newly released primitive version when preparing a release.

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
are serialized; binding echoes do not replay obsolete open/closed commands.

`Open` changes from application code issue explicit commands. Unchanged parameters on an
unrelated parent render preserve browser state. Increment `InteractionRevision` to reapply
`Open` and Select's authoritative value even when those parameters are unchanged (for
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
