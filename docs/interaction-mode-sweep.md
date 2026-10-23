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
