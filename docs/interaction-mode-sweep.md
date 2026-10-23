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
