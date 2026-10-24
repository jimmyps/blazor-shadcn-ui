# Interaction Mode expansion: P1 and P2 — v4.5.0

**Release target: Components and Primitives 4.5.0 (unreleased).** See [Upgrading to v4.5.0](upgrading-to-4.5.md).

This follow-up implements P1 before P2. P3 (modal Dialog/Sheet/AlertDialog and a complete rich-text editor conversion) is excluded. Existing application defaults and explicit Blazor exceptions remain the adoption policy.

## P1

- Harden accepted-value acknowledgements independently of child rendering. Cover MultiSelect, Command, ordinary binding, table/view search, filter values and dynamic forms. Preserve normalization, rejection and explicit revisions.
- Add Textarea, InputGroupInput and InputGroupTextarea to the draft/IME/debounce/reset/form-flush protocol.
- Add HoverCard and Tooltip browser timers, hover/focus persistence, ARIA, Escape dismissal, controlled commands, reconnect and disposal coverage.
- Add Accordion and Collapsible browser expansion, keyboard/ARIA state and revisioned resets at the primitive layer.

## P2

- Tabs, with the consumer review below.
- Numeric/Currency/Masked inputs: preserve incomplete editable text separately from parsed values; keep caret, culture, masks, normalization and commit-time validation correct.
- TagInput, InputOtp and segmented DateInput/TimeInput: local editing/navigation/paste and versioned application commits.
- Slider/RangeSlider and boolean/toggle families: local preview with separate committed values, rejection and reset semantics.
- MarkdownEditor: local typing/shortcuts and a form synchronization adapter. RichTextEditor requires a focused stale-echo/selection/undo audit; its full conversion stays outside this sweep's P3 scope.

## Tabs: consumer review is required

Before letting Tabs inherit an application Browser default, evaluate every tab's content. Browser-local switching is appropriate for small, already-mounted panels. It requires inactive panels to remain available in the DOM so switching can occur without asking Blazor to construct content.

Keep tabs in explicit Blazor mode when opening a tab intentionally triggers lazy loading, expensive initialization, a large grid/chart/editor, substantial subscriptions or data requests, or mount/unmount lifecycle work. Hidden mounted components can still execute lifecycle methods, subscribe, load data and consume memory; CSS hiding is not lazy loading.

Server-side content remains live, and revealing a panel does not mean its data request has completed. Exclude heavy/lazy tab sets from the application-wide Browser policy rather than assuming local switching makes their content cheap. Verify retained form/editor state, focus behavior and initial DOM cost before opting them in. Use `<Tabs InteractionMode="InteractionMode.Blazor">...</Tabs>` for that explicit exception.

## Validation gates

Each family requires a baseline regression and focused Server/WASM checks before it is advertised as supported. Cover application/scoped/explicit precedence, disabled/read-only states, delayed and rejected updates, rapid edits, stale acknowledgements, controlled resets, keyboard/ARIA, retained-circuit reconnect and removal while initialization is pending. Text controls must participate in the existing form flush/validation/duplicate-submit contract.

## Validation checkpoint

All scoped P1/P2 families are implemented. Full Server regression: 118 passed with one WASM-only skip. Scoped policy sweep: 27 passed. WASM sweep: 21 passed with three Server-only skips, followed by four focused redo/disclosure/picker checks. Final compatibility follow-ups passed 22 Edge checks, 14 WebKit checks and seven WASM checks, including raw mask boundaries, WebKit deletion, read-only snapshots and segmented FocusAsync. The actual gallery passed 12 checks (including mixed-control Save and desktop/mobile layout), plus both application-default spotlight/render-backpressure checks. Server/WASM/gallery builds, deterministic interaction-state checks and JavaScript/whitespace checks passed.

RichTextEditor was audited and remains outside the browser draft/form protocol. Full rich-editor conversion and modal families remain excluded as P3. See browser-interactions.md for canonical manual-composition getters, independent menu/Select commit queues, custom TagTemplate exceptions and Tabs adoption guidance.


## Final correctness and family-policy follow-up

Application policy now supports optional nullable Floating, Menus, Selection, Inputs and
Disclosures defaults. Explicit component settings win, then the nearest applicable scope,
application family setting and global DefaultMode fallback. Providers may limit a scope to
one Family; pill navigation uses a menu-only scope internally to preserve embedded field
policy. Selection controls explicitly forward their resolved mode to their floating and
search composition. Existing configurations without family settings preserve composed
Blazor exceptions. The gallery documents staged adoption and includes a standalone radio
example alongside the mixed form.

The correctness review also fixed canonical null ToggleGroup rejection, initial and
between-segment keyboard focus, minute-step/shortcut parity, NavigationMenu timer callback
dispatch/disposal and WebKit floating ResizeObserver delivery errors. Input module import
failure retains its earlier log-only policy; independent controls survive while Browser Save
refuses an unsynchronized draft.

Validation before the family-policy follow-up: full Server regression 137 passed with one
renderer-only skip; focused input/form/two-mode follow-up 77 passed with one skip. Final
family policy checks passed eight cases across conservative and reverse policies, and the
final Server compatibility matrix passed 19 cases (three family-configuration-only skips).
Deterministic interaction/lifetime/family-policy checks and Server/WASM/gallery builds
passed. The updated gallery passed its mixed-control Save, radio selection and desktop/mobile
layout check locally and through the restored review tunnel.

WASM follow-up covered 34 unique passing checks with one Server-only skip across the field,
formatted, Markdown, special-input, tag and both-mode matrix suites. Seven initial matrix
checks stopped before hydration at the new harness's seven-second readiness limit; aligning
that limit with the existing WASM suites (30 seconds) produced a clean 17/17 matrix rerun.
These were startup timeouts, not failed interaction assertions. Existing WebKit coverage for
the shared positioning/segmented fixes was retained; physical-device coverage is not claimed.


## Reviewer regression gates

The interaction-regressions CI workflow covers both Server defaults, family policies,
WASM and real gallery/WebKit checks. New reviewer cases cover no-Browser serialization,
legacy Input module loading/disposal, partial segmented flush under novalidate, observable
numeric failures, MultiSelect trigger actions, provider remount, cancelled range gestures,
known-unsent reconnect delivery, shared connection signals and prerendered initial content.
A JSON-reflection-disabled executable renders cyclic TValue RadioGroup and disabled hosts.
Scroll positioning remains immediate; only element ResizeObserver writes are deferred.
Unknown in-flight callbacks retain the no-replay safety contract.

Reviewer-fix validation passed the full Server suite locally and in CI under both application
defaults: 155 passing checks and four configuration/renderer skips per run, plus eight
conservative/reverse family-policy checks per CI job. The default compatibility executable
also passes with JSON reflection disabled. Tooltip callback ordering now uses an explicit
application completion gate, avoiding a wall-clock assumption on slower Debug WASM runners.
The gallery discovery check allows retained portals to initialize after enhanced navigation;
search/interaction assertion deadlines remain unchanged. Consult the PR's latest workflow
run for the current WASM/gallery/WebKit result rather than treating these counts as a blanket
compatibility guarantee.

The small-item follow-up adds focused-trigger Escape inside a Dialog and nested popovers,
horizontal RTL range pointer/fill/keys with unchanged vertical stepping, Browser and Blazor
TagInput suggestions inside a Browser popover, and controlled tag reset/independent server
actions. The nested case also guards against portal-host refresh feedback during rendering.
Input lifetime coverage disposes a still-connected element during Browser module import and
then verifies one fresh controller; asset-alias tests verify one connection module URL at
both root and path-prefixed application bases. The expanded reviewer suite passes 21 Server
checks locally; the workflow retains its full both-defaults, WASM and gallery gates.


### Nested portal render ordering review

The reviewer suite exercises a Dialog with a portaled Popover and a further nested Popover.
A single event updates all three content levels; the test waits for their new text before
sending another event. A final registered overlay fragment also schedules a descendant update
after the existing overlay fragments are evaluated, covering a late-in-batch update. A further descendant-only event suppresses automatic parent rendering, preventing an unrelated
parent refresh from rescuing a missed update. Five ordinary updates, three simultaneous late
updates, and the descendant-only update run with omitted interaction mode, explicit Blazor,
and explicit Browser settings. The explicit Blazor case asserts that Browser controllers
were not mounted, including when the application default is Browser. The same fixture is
linked into the WASM host and runs in the existing CI reviewer suite.

The requested scenario did not reproduce stale content with the current render guard; no
production guard change was made on that evidence. These cases cover the tested ordering,
not every possible asynchronous descendant render. The mixed-portal ownership contract and
`data-neo-floating-owner` example are documented in the Browser interaction guide. The
TagInput reset assertion waits for the asynchronous authoritative badge render before checking
its text; reopening the shell locally does not imply the reset has already reached the browser.


### Styled Select content ownership

Styled SelectContent captures additional attributes and forwards them to its primitive listbox,
or to the scroll container for the Blazor-only BottomSheet presentation. The guide now uses
styled components in its ownership example. Regression cases select and reselect items in a
Blazor Select, a Browser Select and a Blazor BottomSheet inside a Browser popover, verify the
application value and forwarded attributes, and then dismiss the outer shell by clicking outside.
The Browser case also guards against replacing physical hit containment with the logical anchor:
a marker supplements containment so it cannot close its own listbox before an item click arrives.
The shared ownership helper retains both physical and logical targets for composed controls.


### Disclosure motion regression

Animation tests cover styled Collapsible and Accordion in both modes. They pause actual grid
transitions to assert intermediate opening and closing heights, then finish the animations and
verify final presence. Browser cases drop outbound circuit traffic, check immediate inert/ARIA
state, render a child during exit, reverse an unfinished close, and verify zero-duration CSS
hides synchronously. These cases run in the existing Server/WASM P1 suites; the CI WASM job
also runs the four cases with WebKit.


### Existing JavaScript preservation audit — 2026-10-24

Audit baseline: `a2151714`. This is a findings report, **not a claim that the
items below have been fixed**. The review covered all 61 shipped JavaScript files
(40 Components, 21 Primitives), their C#/Razor callers, and Browser-mode early
returns and replacement controllers. Eleven files are Browser controllers/adapters;
one is the vendored motion runtime. The 49 other modules are inventoried below.

Browser mode should retain one editing/interaction engine per control. The existing
engine owns formatting, selection, caret, paste, undo and configured local behavior;
the versioned Browser protocol owns delivery, acceptance, resets and form flushing.
Do not enable both legacy server notifications and the Browser queue. A script that
only forwards keys to C# does not already own the editor: moving its C# behavior into
JavaScript still requires explicit configuration and behavior parity checks.

#### Confirmed preservation gaps

| Priority | Control | Finding and evidence | Required direction |
| --- | --- | --- | --- |
| P1 | MaskedInput | Browser mode skips `masked-input.js` and implements a second mask engine in `browser-input-formats.js`. Direct browser comparison: with mask `12000`, the existing script preserves `12345`, whereas the Browser formatter produces `12234`. The matching literal `2` is also consumed as an editable digit. Empty focus with `ShowMask=true` displays `___-___` in the existing engine and nothing in the Browser adapter. Clicking the separator in `123-456` advances the legacy caret to 4 but leaves the Browser caret at 3. The recent next-slot caret fix did not cover these behaviors. | Reuse the existing mask engine with a local edit notification adapter; keep exactly one formatter and add cross-mode configuration/selection/paste tests. Preserve the newer IME, mobile deletion, reset and form-flush guarantees while integrating it. |
| P1 | CurrencyInput | Browser focus strips formatting from the rounded display rather than exposing the full-precision model. In the gallery, enter `12.34567`, blur, then focus and blur without editing. Default Blazor retains model `12.34567`; Browser focuses as `12.35` and changes the model to `12.35` on the untouched blur. Legacy `HandleFocus()` deliberately uses `G29`; the Browser format payload has no equivalent editing value. | Keep display text and exact editable text separately. Reuse the existing currency definition/culture/precision rules and never reconstruct the editing value from a rounded display. Recheck zero-/multi-decimal currencies, symbol placement, hidden symbols, bounds and unchanged focus/blur. |
| P2 | TagInput | `tag-input.js` container click-to-focus is skipped without an equivalent in `browser-tags.js`. Live gallery probe: clicking the container does not focus the input. Its legacy delayed blur dismissal is also absent: Tab moves focus away while the suggestions remain visible and `aria-expanded=true`. | Reuse the existing event adapter where practical, with local callbacks. Restore container focus and blur dismissal, preserving pointer selection into portaled suggestions, IME, trigger flags, rejection and asynchronous search. |
| P2 | NavigationMenu | The Browser controller replaces the C# motion state and sets inactive panels to `display:none`, but never supplies `data-motion`. Live gallery comparison: default mode opens with `data-motion=""` and computed animation `enter`; Browser has no motion attribute and computed animation `none`. The styled content's existing Tailwind entry and directional-switch classes depend on those attributes. | Restore the same motion attributes and exit-presence contract in the local controller. Keep the existing Tailwind classes and verify opening, closing, sibling switching, reversal and reduced motion. |

The currency and mask cases are wrong-value findings and should be resolved before
broad adoption. A green existing regression matrix does not cover these newly
identified cases. The audit itself changes no runtime behavior.

#### Deliberate differences and support boundaries

- **MarkdownEditor:** already follows the desired reuse pattern. Both modes use
  `markdown-editor.js`; Browser mode changes notification routing and adds local
  toolbar/shortcut handling while retaining formatting, list continuation and undo.
  Preview rendering still needs accepted content and server work.
- **DateInput/TimeInput:** the previous `date-time-input.js` is a scroll-prevention
  and focus helper, not the segment editor. Segment editing previously lived in C#;
  `browser-segments.js` is a local implementation of that behavior. Culture segment
  descriptors, year expansion, 12/24-hour mode, seconds, minute steps, placeholder,
  bounds and required/partial validation were traced. Existing parity coverage is
  meaningful, but this is still duplicated C#/JS behavior, not reuse of a rich JS
  editor. Initial current-date/time seeding uses the browser clock instead of the
  server clock; applications with different client/server timezones must account
  for that distinction.
- **NumericInput:** the previous element is native `type=number` with shared input
  notifications and a cursor helper. Browser mode uses a text draft plus explicit
  number parsing/arrow stepping to retain incomplete values. Bounds, step, required,
  maxlength and final normalization are forwarded, but native steppers, native step
  validity and browser-specific number behavior are not identical. Treat the change
  of editing surface as an explicit compatibility decision, not script preservation.
- **MultiSelect:** `multiselect.js` is intentionally replaced by `browser-list.js`.
  The old script handles Space in a focused search input as selection when an item
  is active. The new controller always treats Space in an editable search field as
  text; selection shortcuts apply to option/root targets. Live ArrowDown→Space
  comparison confirmed the difference. It is already documented in the changelog;
  decide whether to retain this exception or offer compatible routing. Hover,
  Enter/Escape, selection callbacks, scroll/load-more and templates remain wired.
- **TagTemplate:** custom templates are not silently discarded: TagInput throws an
  explicit unsupported-combination error in Browser mode. This is documented and
  requires `InteractionMode.Blazor`. A global Inputs=Browser policy therefore needs
  exceptions for existing custom-template instances. Preserving arbitrary Razor
  templates requires server-rendered content or a scoped fallback, not JS-generated
  replacement badges.
- **ColorPicker:** existing `color-picker.js` is retained, but its initialization
  still waits for server `_isOpen`. With outbound circuit traffic suppressed, a
  cold Browser popover opens and its canvas is visible but has no initialized
  `_updateGradient`. This is the documented shell-only support boundary, not a
  bypassed script or a fully local editor. Hue/alpha/application updates remain
  server work. Initialize the existing canvas on local visibility if full local
  editing is introduced; preserve the canvas engine and its configuration.
- **RichTextEditor:** Quill and `quill-interop.js` are retained. This control has not
  joined the Browser draft/form protocol. Its 150 ms text-change delivery and
  external `setHtml` behavior still need a separate revision/flush/selection/history
  adapter before claiming safe immediate-submit or stale-echo protection. Full
  conversion remains outside the earlier P1/P2 sweep scope.
- **RangeSlider:** `range-slider.js` mouse/touch-to-C# forwarding is replaced by local
  pointer handling. Orientation, RTL, step, endpoint constraints, keyboard movement
  and cancellation have equivalents. Pointer previews are local and notify on
  release; legacy drag handlers could notify during movement. Browser tooltip text
  also currently uses the raw number rather than legacy `0.##`. These are parity
  differences to retain deliberately or align; do not conflate them with a removed
  rich editor engine.

#### Existing-module inventory

“Retained” means the relevant import/initialization remains reachable; it does not
mean every configuration was exercised on every browser or device.

| Existing modules | Browser-mode disposition |
| --- | --- |
| `input.js` | Retained entry point, validation and lifecycle helpers. Browser mode switches notification ownership to the shared draft controller; legacy notifications are not also attached. |
| `masked-input.js` | Bypassed; duplicate Browser formatter. P1 findings above. |
| `cursor-position.js` | Numeric-only legacy helper bypassed; the Browser text-draft controller manages selection directly. |
| `markdown-editor.js` | Reused by both paths with notification adaptation. |
| `date-time-input.js` | Bypassed in segment Browser mode; local segment controller replaces the old C# editing/focus route. |
| `tag-input.js` | Bypassed; local tags controller has focus/blur omissions described above. |
| `multiselect.js` | Bypassed; local list controller replaces keyboard/focus behavior, including the documented Space difference. |
| `range-slider.js` | Bypassed; local field controller replaces pointer-to-server forwarding. |
| `select.js` | Replaced in Browser Select by the floating controller's local focus, selection, typeahead and scroll handling. Item rendering remains Razor-owned. |
| `menu-keyboard.js` | Replaced in Browser menu families by local floating/navigation controllers. Loop/RTL, disabled items, typeahead and submenu routing remain represented. |
| `click-outside.js` | Replaced for Browser floating layers by local outside/ownership handling; retained for legacy layers. |
| `match-trigger-width.js` | Replaced for Browser dropdowns by the existing positioning engine's reference-width matching. |
| `element-utils.js`, `virtualization-scroll.js` | Legacy list paths still use helpers. Browser active-item navigation scrolls locally; Virtualize still materializes content and load-more still invokes application work. |
| `positioning.js`, `portal.js` | Retained shared positioning/portal machinery. Browser lifecycle changes ownership and timing, not the layout engine. |
| `dialog.js`, `focus-trap.js` | Dialog/Sheet focus and keyboard setup remain active; Browser popovers replace their Escape adapter with local handling. Modal families were not converted. |
| `selection-indicator.js` | Retained; its MutationObserver consumes local data-state/ARIA changes. No initialization bypass found. |
| `color-picker.js` | Retained, with server-gated cold initialization and shell-only coverage as noted above. |
| `quill-interop.js` | Retained; no Browser draft conversion or form-flush integration. |
| `carousel.js`, `resizable.js`, `scroll-area.js`, `tree-view.js`, `sortable.js`, `table-row-nav.js` | Existing initialization remains reachable; no interaction-mode early return bypass found. ScrollArea gained a disconnected-element initialization guard, not a replacement engine. |
| `drawer.js`, `responsive-nav.js`, `sidebar.js`, `sidebar-static.js` | Existing machinery remains active. The Browser pill controller coordinates with sidebar DOM rather than suppressing its scripts. |
| `file-upload.js`, `datatable.js`, `grid/aggrid-renderer.js`, `grid/theme-shadcn.js` | Existing upload/grid behavior retained; no conversion or mode-based import suppression found. |
| `chart-theme.js`, `chartjs-renderer.js`, `echarts-renderer.js` | Existing chart rendering/theme machinery retained. Mounted hidden panels can still initialize expensive content. |
| `height-animation.js`, `motion.js`, `toast-animation.js` | Existing animation wrappers retained. Their presence does not establish that all Browser controllers still emit the CSS state attributes they need; NavigationMenu is an example of missing state. |
| `collapsible-state.js`, `keyed-state.js`, `keyboard-shortcuts.js` | Existing persistence/shortcut services retained. |
| `navigation-menu.js`, `slider.js`, `theme.js`, `context-menu-position.js`, `keyboard-nav.js` | No direct C#/Razor module-path caller found in this checkout. No new Browser guard was identified as removing an active initialization route; consumer/direct-JS use remains outside this caller scan. |

The new Browser modules were also traced: `browser-input.js`,
`browser-input-formats.js`, `browser-list.js`, `browser-calendar.js`,
`browser-connection.js`, `browser-field.js`, `browser-floating.js`,
`browser-navigation.js`, `browser-segments.js`, `browser-state.js` and
`browser-tags.js`. The vendored `motion-one.min.js` integration was inventoried;
its third-party implementation was not re-reviewed.

#### Verification and next implementation order

Source/caller tracing covered the full inventory. Runtime comparisons used the
rebuilt gallery at ports 5184 (Blazor default) and 5185 (Browser default), on
Chromium. Confirmed observations: currency precision on a second untouched blur,
mask digit-prefix/focus/separator behavior, TagInput keyboard blur/container focus,
NavigationMenu motion attributes/animation, MultiSelect Space routing, and
ColorPicker cold opening with circuit sends suppressed. The isolated mask probe
compared the two shipped formatting modules directly; it did not substitute a
mock formatter. No production fixes or new cross-browser guarantees are implied
by these audit probes, and the full regression matrix was not rerun for this
findings-only documentation change.

1. Unify MaskedInput around its existing engine, preserving the newer transport,
   reset, IME and mobile-edit guarantees with an adapter and cross-mode tests.
2. Restore exact currency editing values and protect unchanged focus/blur from
   writes. Include culture/currency/bounds and immediate-save tests.
3. Restore TagInput focus/blur behavior and NavigationMenu's Tailwind motion contract.
4. Record the remaining deliberate mode differences; do not advertise shell-only
   or out-of-scope widgets as fully adopted editors.
