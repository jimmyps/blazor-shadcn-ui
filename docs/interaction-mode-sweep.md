# Interaction Mode expansion: P1 and P2

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
