# Mobile-First Roadmap for NeoUI

## Background & Motivation

NeoUI was originally designed as a desktop/responsive web component library mirroring
shadcn/ui. As Blazor adoption grows in mobile contexts — particularly **.NET MAUI Blazor
Hybrid** (via `BlazorWebView`) and **Progressive Web Apps** — there is a growing gap
between NeoUI's current component set and what mobile-first UIs require.

This roadmap makes NeoUI a **first-class citizen for mobile app development**, covering:

- Blazor mobile-first web apps
- .NET MAUI Blazor Hybrid projects
- PWAs targeting phone/tablet form factors

The components added here are designed to work in all three contexts with a single API
surface — no separate namespace or package required.

---

## Key Decisions

### 1. Same `NeoUI.Blazor` package — no separate namespace

Mobile layout components (`BottomNav`, `AppBar`) live alongside existing navigation
components (`Sidebar`, `ResponsiveNav`) in the main `NeoUI.Blazor` namespace.

**Rationale:**
- Components like `QuantityStepper` and `NotificationBadge` are not exclusively mobile —
  they appear in desktop e-commerce and dashboards too.
- A `NeoUI.Blazor.Mobile` namespace would force consumers to add a second `@using` in
  every razor file just to access `BottomNav`.
- `Sidebar` and `Drawer` already live in the main namespace and are considered
  "layout-specific" without needing a sub-namespace.
- If a future MAUI-native (non-web-rendered) component set is ever warranted, that would
  be a separate assembly (`NeoUI.Blazor.Maui`), not a namespace split within this one.

The **documentation** will use a "Mobile Layout" category badge to help developers
discover these components. The **code** stays in one namespace.

### 2. Complement, don't replace

`AppBar` ≠ `ResponsiveNav`. `QuantityStepper` ≠ `NumericInput`. `NotificationBadge` ≠ `Badge`.
These additions fill genuinely different UI slots; they do not deprecate existing components.

### 3. MAUI Hybrid compatibility from day one

All new components are authored with BlazorWebView constraints in mind:

- No `fixed` positioning assumptions (MAUI hosts Blazor inside a native container, not the
  full viewport); `BottomNav` exposes a `Fixed` parameter to opt into `position: fixed`.
- Safe-area insets via `env(safe-area-inset-bottom, 0px)` for iOS notch/home indicator.
- No JavaScript interop dependencies in the new components (pure Razor + CSS).
- Touch-target sizes are designed to align with WCAG 2.5.5 (44×44px minimum), but app layouts should verify and adjust touch targets as needed.

---

## New Components (this PR)

### `BottomNav` + `BottomNavItem`

**Purpose:** Mobile bottom tab bar — the primary navigation pattern for iOS, Android,
and .NET MAUI Shell `TabBar`.

**Why not `Sidebar`?** Sidebar is a collapsible desktop drawer. BottomNav is a persistent
fixed-height strip at the screen bottom, always visible, with 2–5 icon+label tabs.

```razor
<BottomNav @bind-ActiveTab="activeTab">
    <BottomNavItem Value="home"    Icon="house"           Label="Home" />
    <BottomNavItem Value="search"  Icon="search"          Label="Search" />
    <BottomNavItem Value="orders"  Icon="clipboard-list"  Label="Orders" BadgeCount="@pendingOrders" />
    <BottomNavItem Value="account" Icon="user"            Label="Account" />
</BottomNav>
```

Key parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ActiveTab` | `string?` | — | Two-way bindable active tab value |
| `Fixed` | `bool` | `true` | `position: fixed` vs. in-flow for MAUI Hybrid |
| `AriaLabel` | `string` | `"Main navigation"` | Nav landmark label |

`BottomNavItem` key parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | required | Matched against `BottomNav.ActiveTab` |
| `Icon` | `string?` | — | Lucide icon name shorthand |
| `IconContent` | `RenderFragment?` | — | Custom icon (takes precedence) |
| `Label` | `string?` | — | Text beneath icon |
| `BadgeCount` | `int` | `0` | Notification count overlay |
| `MaxBadgeCount` | `int` | `99` | Truncates to "N+" above this |
| `ShowZeroBadge` | `bool` | `false` | Show badge when count is 0 |

---

### `AppBar`

**Purpose:** Mobile-style top application bar — centered title, optional back chevron,
optional right-side action slot.

**Why not `ResponsiveNav`?** `ResponsiveNav` renders a full horizontal navigation bar with
brand, links, and a hamburger menu. `AppBar` is a simple, fixed-height header designed for
detail/sub-screens where only a back button and a title are needed.

```razor
<AppBar Title="Product Detail" OnBack="NavigateBack">
    <RightContent>
        <NotificationBadge Count="@cartCount">
            <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon">
                <LucideIcon Name="shopping-cart" Size="20" />
            </Button>
        </NotificationBadge>
    </RightContent>
</AppBar>

<!-- Transparent mode over hero image -->
<AppBar Transparent="true" OnBack="NavigateBack" />
```

Key parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | — | Centered title text |
| `TitleContent` | `RenderFragment?` | — | Custom title slot |
| `OnBack` | `EventCallback` | — | Back button shown when provided |
| `BackLabel` | `string` | `"Go back"` | Accessible back button label |
| `RightContent` | `RenderFragment?` | — | Right action slot |
| `Transparent` | `bool` | `false` | Transparent background (for heroes) |
| `ShowBorder` | `bool` | `true` | Bottom border (hidden when transparent) |

---

### `QuantityStepper`

**Purpose:** Circular +/− button stepper for adjusting a quantity — the standard
e-commerce mobile control. Different from `NumericInput` (a text box with spinner arrows).

Signature feature: `DestructiveAtMin` — when `Value == Min`, the `−` button becomes a
trash icon and fires `OnDestructiveClick`. This enables "remove from cart" semantics
without a separate delete button.

```razor
<!-- Product detail -->
<QuantityStepper @bind-Value="quantity" Min="1" Max="99" />

<!-- Cart item — turns into trash icon at qty=1 -->
<QuantityStepper @bind-Value="item.Quantity"
                 Min="1"
                 DestructiveAtMin="true"
                 OnDestructiveClick="() => RemoveFromCart(item)"
                 Size="QuantityStepperSize.Sm" />
```

Key parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `int` | `1` | Current quantity (two-way bindable) |
| `Min` | `int` | `1` | Minimum value |
| `Max` | `int?` | `null` | Maximum value (null = unlimited) |
| `DestructiveAtMin` | `bool` | `false` | Trash icon + `OnDestructiveClick` at min |
| `OnDestructiveClick` | `EventCallback` | — | Fires when trash button is pressed |
| `Size` | `QuantityStepperSize` | `Default` | `Sm` / `Default` / `Lg` |

---

### `NotificationBadge`

**Purpose:** Positions a count chip in the top-right corner of any wrapped element.
Different from `Badge` (an inline label); this is a positioning overlay.

```razor
<!-- Count badge on cart icon -->
<NotificationBadge Count="@cartCount">
    <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Icon">
        <LucideIcon Name="shopping-cart" Size="20" />
    </Button>
</NotificationBadge>

<!-- Dot-only (no number) on avatar -->
<NotificationBadge Count="1" Dot="true">
    <Avatar><AvatarFallback>JD</AvatarFallback></Avatar>
</NotificationBadge>
```

Key parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Count` | `int` | `0` | Number to display |
| `Max` | `int` | `99` | Truncates to "N+" above this |
| `ShowZero` | `bool` | `false` | Show badge when count is 0 |
| `Dot` | `bool` | `false` | Dot indicator instead of number |
| `Variant` | `NotificationBadgeVariant` | `Destructive` | Colour variant |

---

### `SectionHeader`

**Purpose:** Section title row with optional "view all" chevron button and separator line.
A recurring pattern in mobile UIs that is verbose to compose each time.

```razor
<SectionHeader Title="Today's Promo" OnViewAll="NavigateToPromo" />

<SectionHeader Title="Order Summary" ShowSeparator="false" />
```

Key parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Title` | `string?` | — | Section title text |
| `TitleContent` | `RenderFragment?` | — | Custom title slot |
| `OnViewAll` | `EventCallback` | — | Chevron shown when provided |
| `ViewAllText` | `string?` | `null` | Optional text beside chevron |
| `ShowSeparator` | `bool` | `true` | Separator line below title |

---

## Existing Component Enhancements (follow-up issues)

The following enhancements to existing components were identified during the same
mobile-first audit. Each deserves its own focused issue/PR.

| Component | Enhancement | Issue |
|---|---|---|
| `Carousel` | Add `ShowDots` + `ShowArrows` params for dots-only pagination mode (no prev/next buttons) | TBD |
| `ToggleGroup` | Add `Scrollable` param for horizontal-scroll overflow on mobile filter pills | TBD |
| `Badge` | Add `<BadgeIcon>` child render fragment for icon+text badges (e.g. coin icon + "17 pts") | TBD |
| `Separator` | Add `Style="SeparatorStyle.Dashed"` variant for ticket/voucher card dividers | TBD |

---

## What This PR Does NOT Include

- A separate `NeoUI.Blazor.Mobile` NuGet package or namespace split
- MAUI-native (non-web-rendered) wrappers — those would be a separate `NeoUI.Blazor.Maui` assembly
- Demo pages (to be added in a follow-up PR against the demo project)
- Unit tests (to be added in a follow-up PR)

---

## Reference: Audit Source

The component gaps were identified by building a full **Mama Roz** mobile app UI
(12 screens: Onboard, Auth, Home, Menu, Orders, Account, Product Detail, Cart, Points,
Voucher, Referral, Premium) in React + shadcn/ui, then systematically mapping every UI
pattern to its NeoUI equivalent — identifying what was missing or insufficient for a
1:1 port to Blazor.
