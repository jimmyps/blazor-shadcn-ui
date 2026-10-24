# Upgrading to NeoUI v4.5.0

**Release target: 4.5.0 for both `NeoUI.Blazor` (Components) and
`NeoUI.Blazor.Primitives` (unreleased).** This is a substantial feature upgrade; existing
applications keep Blazor interaction behavior unless they opt in. .NET 10 remains the target.

## Package alignment

After publication, upgrade both packages together if your application references both:

```sh
dotnet add package NeoUI.Blazor --version 4.5.0
dotnet add package NeoUI.Blazor.Primitives --version 4.5.0
```

Components already brings Primitives as a dependency, so a direct Primitives reference is
optional. Update any direct version pins or central package management entries to 4.5.0;
Components requires Primitives 4.5.0. Icon packages retain their independent version numbers.

## Adopt Browser mode by family

```csharp
using NeoUI.Blazor.Primitives;
using NeoUI.Blazor.Primitives.Extensions;

builder.Services.ConfigureNeoUIInteractions(options =>
{
    options.DefaultMode = InteractionMode.Blazor;
    options.Floating = InteractionMode.Browser;
    options.Menus = InteractionMode.Browser;
    // Preserve legacy form fields and selection controls during staged adoption.
    options.Inputs = InteractionMode.Blazor;
    options.Selection = InteractionMode.Blazor;
});
```

Family settings are nullable; omitted settings inherit `DefaultMode`. The precedence is
explicit component → nearest applicable scoped provider → application family setting →
application global default → library Blazor default. A provider can optionally restrict its
scope with `Family="InteractionFamily.Menus"`. Existing configurations remain valid.

Selection covers Select, Combobox, MultiSelect and picker shells; internal search follows
that selection control's resolved mode. Floating alone does not opt these fields in. Inputs
covers standalone Command, text/structured fields, boolean/toggle fields, sliders and supported
editors. DateInput/TimeInput are Inputs; DatePicker/TimePicker shells are Selection.
Menu checkbox/radio items follow Menus. Disclosures covers Accordion, Collapsible and Tabs.

Configure policy before mounting. Changing an `InteractionModeProvider` mode or applicable
family scope automatically keys and recreates its own subtree. For a direct component mode
change outside that provider mechanism, recreate the affected root/subtree with `@key`;
controllers do not migrate in place. Flush/persist drafts first because recreated children
restart their lifetimes and local state. Explicit Blazor exceptions remain available.

## Forms and application acceptance

Browser mode separates editable drafts and local feedback from server acceptance. A successful
local interaction does not mean persistence succeeded. Await application work and use the
supported canonical binding/accessor and revision/reset mechanisms.

When enabling Browser input fields, opt forms into synchronization before validation. The
thin `BrowserEditForm` adapter preserves Model/EditContext, validators, OnSubmit versus
OnValidSubmit/OnInvalidSubmit semantics and awaited submit handlers. It guards duplicate
submits and blocks stale submission on rejection, incomplete input, missing initialization,
timeout or uncertain delivery. `BrowserInputForm` and manual `BrowserInputScope.SubmitAsync`
remain available. An ordinary EditForm alone does not flush browser drafts before validation.
Menu and selection commit queues remain independent of the field synchronization barrier.

## Review retained content and hosting

Browser Tabs retain inactive panels. Keep heavy or intentionally lazy tab sets explicitly in
Blazor mode; hidden components can initialize, load data and retain subscriptions/memory.
Large Select lists and menus repeated across table rows also incur initial rendering/DOM cost.
Select BottomSheet and custom TagInput Razor templates require explicit Blazor exceptions.
RichTextEditor and modal families have not been converted by this work.

Floating content stays live Blazor UI; the browser owns its shell. Supported Browser mechanics
were verified on Interactive Server and WebAssembly. Initialization requires an interactive
render; static SSR-only pages are not supported. Auto/Hybrid, Native AOT and physical devices
are not established by these checks. Reconnect reconciles rendered controller state without
replaying business callbacks; uncertain application delivery is not automatically retried.

See the [complete interaction and form adoption guide](browser-interactions.md),
[validation report](interaction-mode-sweep.md), [changelog](../CHANGELOG.md), and
[Interaction Mode demo](https://demos.neoui.io/components/browser-interactions).

## Release coordination

The release tag names will be `primitives/v4.5.0` and `components/v4.5.0`, using the existing prefixes.
When publishing, release Primitives first and wait for its package to become available before
publishing Components, whose release build consumes the NuGet dependency. These notes do
not create tags or publish packages; this branch remains unreleased until that release runs.


## Compatibility details to audit

- Register NeoUI services in custom/bUnit renderers for IOptions-based interaction policy.
- Update manual CommandContent disposal to await DisposeAsync.
- Use typed Checkbox/Switch Name and Required parameters; native disabled on dropdown triggers changes focus/event behavior.
- Add explicit Blazor exceptions for trigger OnClick/custom-click compositions when enabling Browser globally.
- Selection roots explicitly forward their resolved mode to internal shells/search. An Inputs family setting is independent of a Popover shell exception; use an Inputs-specific scope/component override when needed.
- Provider mode changes recreate the subtree automatically; synchronize drafts before changing policy.
- Invalid formatted or partial segmented drafts block synchronization even under novalidate and are surfaced through OnFlushFailed. Standard native constraint and EditContext handler semantics are preserved.
- Known unsent drafts can flush after reconnect; uncertain business callback delivery is not automatically replayed. In OnFlushFailed, reconcile the pending operation and authoritative value, then increment ValueRevision (InteractionRevision for primitive fields/TagInput), even for an unchanged value. Offer visible recovery; do not silently discard the draft or automatically replay Save. See [the recovery example](browser-interactions.md#recovering-from-a-timeout-or-uncertain-delivery).

See [the detailed compatibility notes and keyboard shortcuts](browser-interactions.md#reviewer-follow-up-precedence-validation-and-compatibility).

### Additional default-mode compatibility notes

- MultiSelect check markers are presentation-only `<span>` elements in both modes. Options retain their selection semantics; there is no nested `role="checkbox"`. Update application CSS and tests that targeted those former checkbox elements; prefer the option's `aria-selected` state.
- MaskedInput now honors `ShowMask=false` while typing in Blazor mode too. The existing mask still formats entered characters, but no longer fills the unused template when that option is false.
- Checkbox keyboard activation is Space-only in both modes. A local Enter guard preserves the ARIA checkbox pattern while native Space clicks avoid double toggles.
