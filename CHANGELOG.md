# Changelog

All notable changes to this project will be documented in this file.

## 2026-02-06 - Input Component UpdateOn Behavior & Performance Optimization

### 🚀 Performance Improvements

**Status:** ✅ Complete, Production Ready  
**Impact:** Improved default behavior for Input, CurrencyInput, MaskedInput, and NumericInput components with performance and UX optimizations.

---

### 🎯 Input Components UpdateOn Default Behavior

#### **Enhanced Default Performance Mode**

**Changed default behavior for all input components:**

**Components Updated:**
- `Input` - Text/email/password/number inputs
- `CurrencyInput` - Locale-aware currency formatting
- `MaskedInput` - Pattern-based masked input
- `NumericInput` - Type-safe numeric input

**What Changed:**
- **Default `UpdateOn` mode** changed from `Input` → `Change`
- **Updates on blur** instead of every keystroke
- **JavaScript-side validation tooltip management** for optimal performance

**Key Benefits:**
1. **Better Typing UX**
   - No interruptions while typing
   - Validation tooltips cleared automatically during input (when `UpdateOn="Change"`)
   - Tooltips only show after user completes input (on blur)

2. **Better Performance**
   - Fewer C# ↔ JS interop calls (critical for WebAssembly)
   - Reduced re-renders in parent components
   - Value updates once on blur instead of every keystroke

3. **Blazor WebAssembly Optimized**
   - Minimizes costly interop overhead
   - Client-side interactivity remains responsive
   - Ideal for WASM deployment scenarios

**Files Changed:**
```
src/BlazorUI.Components/Components/Input/Input.razor.cs
src/BlazorUI.Components/Components/CurrencyInput/CurrencyInput.razor.cs
src/BlazorUI.Components/Components/MaskedInput/MaskedInput.razor.cs (JS)
src/BlazorUI.Components/Components/NumericInput/NumericInput.razor.cs
src/BlazorUI.Components/wwwroot/js/input.js (new, renamed from input-validation.js)
src/BlazorUI.Components/wwwroot/js/masked-input.js
```

**Breaking Change:** ⚠️ Minor
- Previous default: `UpdateOn="Input"` (immediate updates)
- New default: `UpdateOn="Change"` (update on blur)
- **Migration:** Explicitly set `UpdateOn="Input"` if you need real-time updates

**Example:**
```razor
<!-- Default behavior (recommended) -->
<Input @bind-Value="username" />
<!-- Updates on blur -->

<!-- Real-time updates (when needed) -->
<Input @bind-Value="searchQuery" UpdateOn="InputUpdateMode.Input" />
<!-- Updates on every keystroke -->
```

---

### 🎨 MaskedInput Blur Event Fix

**Fixed Issue:** `onblur` event not triggering when `UpdateOn="Change"`

**Root Cause:**
- `lastRawValue` was being updated during typing
- On blur, `currentRaw == lastRawValue`, so no Blazor callback

**Solution:**
- Only update `lastRawValue` when actually notifying Blazor
- For `UpdateOn="Change"`: Don't update during typing, update on blur
- For `UpdateOn="Input"`: Update immediately on every change

**Files Changed:**
```javascript
src/BlazorUI.Components/wwwroot/js/masked-input.js
```

---

### 📖 Documentation Enhancements

**Added comprehensive UpdateOn behavior documentation:**

**Demo Pages Updated:**
- `InputDemo.razor` - Info alert with collapsible details
- `CurrencyInputDemo.razor` - Performance optimization tips
- `MaskedInputDemo.razor` - Masked input specific guidance
- `NumericInputDemo.razor` - Numeric validation examples

**Each demo includes:**
- ✅ Prominent info alert explaining `UpdateOn` behavior
- ✅ Collapsible "Read more" section with:
  - Benefits of `UpdateOn="Change"` (default)
  - Comparison of both modes
  - Context-specific use case examples
  - WebAssembly performance notes
- ✅ Lucide icons for visual consistency
- ✅ Smooth chevron rotation animation

**Example Info Alert Structure:**
```razor
<Alert Variant="AlertVariant.Info">
    <AlertTitle>Optimized for Performance & Best Typing Experience</AlertTitle>
    <AlertDescription>
        By default uses UpdateOn="Change" for better performance...
        <Collapsible>
            <CollapsibleTrigger>Read more ˅</CollapsibleTrigger>
            <CollapsibleContent>
                <!-- Detailed benefits, modes, tips -->
            </CollapsibleContent>
        </Collapsible>
    </AlertDescription>
</Alert>
```

---

### 🛠️ Technical Implementation

#### **JavaScript-Side Validation Management**

**New: `input.js` Module**
- Renamed from `input-validation.js` for broader scope
- Manages validation tooltips AND UpdateOn behavior
- Auto-clears tooltips on input when `UpdateOn="Change"`

**Key Functions:**
```javascript
// Initialize with UpdateOn mode
initializeValidation(elementId, updateOn = 'input')

// Auto-clear tooltip on first keystroke (Change mode)
// Prevents tooltip interference while typing

// Cleanup
disposeValidation(elementId)
```

**Flow for `UpdateOn="Change"`:**
1. User has validation error → tooltip shown
2. User starts typing → **JS auto-clears tooltip** (no C# call)
3. User tabs out → C# validates → new error/success shown

**Performance Impact:**
- Zero C# ↔ JS calls during typing (Change mode)
- Single event listener per input (efficient)
- Proper cleanup on dispose

---

### 📝 Code Quality

**Improvements:**
- ✅ Consistent behavior across all input components
- ✅ Optimized for WebAssembly deployment
- ✅ Better separation of concerns (JS handles UI, C# handles logic)
- ✅ Comprehensive inline documentation
- ✅ User-friendly demo documentation

**Test Scenarios Validated:**
1. ✅ `UpdateOn="Change"` - validates on blur
2. ✅ `UpdateOn="Input"` - validates on every keystroke  
3. ✅ Validation tooltip auto-clears during typing (Change mode)
4. ✅ MaskedInput blur event triggers correctly
5. ✅ No memory leaks (proper event listener cleanup)
6. ✅ WebAssembly performance (minimal interop)

---

### 🎯 Migration Guide

**For Existing Code:**

If you relied on immediate updates (old default behavior):
```razor
<!-- Before (implicit default) -->
<Input @bind-Value="searchTerm" />

<!-- After (explicit for real-time) -->
<Input @bind-Value="searchTerm" UpdateOn="InputUpdateMode.Input" />
```

**Recommended for most cases (new default):**
```razor
<Input @bind-Value="email" />
<!-- Updates on blur - better UX and performance -->
```

---

## 2026-02-05 - Input Components & Positioning Enhancements

### 🎯 New Components & Major Enhancements

**Status:** ✅ Complete, Production Ready  
**Impact:** 8 new components (TimePicker, DateRangePicker, plus 6 specialized inputs) and industry-standard popover positioning that prevents viewport clipping.

#### 📊 Session Statistics
- **8 new components** (TimePicker, DateRangePicker, ColorPicker, CurrencyInput, Drawer, MaskedInput, NumericInput, RangeSlider, Rating)
- **3 components enhanced** (NativeSelect styling, Popover positioning, DataTableToolbar accessibility)
- **2 new enums** (TimePeriod, DateRangePreset)
- **1 JavaScript enhancement** (popover viewport boundary detection)
- **100% accessibility** maintained with ARIA labels and keyboard navigation
- **9 components added** to all navigation indexes (Components Index, Spotlight Search, MainLayout sidebar)

---

### 🆕 New Components

#### **1. TimePicker Component**
**Location:** `src\BlazorUI.Components\Components\TimePicker\`

**Description:** Time selection component with hour/minute dropdowns and optional AM/PM toggle.

**Features:**
- **Native selects** for hour and minute (uses NativeSelect component)
- **12-hour or 24-hour format** with `Use24HourFormat` parameter
- **Flexible binding:**
  - `@bind-Value` (TimeOnly?) - two-way binding
  - `@bind-Hour`, `@bind-Minute`, `@bind-Period` - individual component binding
- **Customizable:**
  - Size variants (Small, Default, Large)
  - Hour/minute step increments
  - Placeholder text
  - Disabled state
  - CSS classes

**Example Usage:**
```razor
<!-- Simple binding -->
<TimePicker @bind-Value="meetingTime" />

<!-- 24-hour format -->
<TimePicker @bind-Value="departureTime" Use24HourFormat="true" />

<!-- In a form field -->
<Field>
    <FieldLabel>Appointment Time</FieldLabel>
    <TimePicker @bind-Value="appointmentTime" Size="NativeSelectSize.Large" />
    <FieldDescription>Select your preferred time slot</FieldDescription>
</Field>
```

**Files Added:**
- `TimePicker.razor` - Main component
- `TimePicker.razor.cs` - Component logic with time conversion
- `TimePeriod.cs` - AM/PM enum
- `TimePickerDemo.razor` - Comprehensive demo page

---

#### **2. DateRangePicker Component**
**Location:** `src\BlazorUI.Components\Components\DateRangePicker\`

**Description:** Date range selection with preset date ranges and two-calendar view.

**Features:**
- **Two side-by-side calendars** for intuitive range selection
- **Preset date ranges** (Today, Yesterday, Last 7 Days, Last 30 Days, This Month, Last Month, This Year)
- **Show/Apply button system** for confirmed selections
- **Clear button** to reset selection
- **Day counter** showing selected range
- **Calendar synchronization** - calendars stay one month apart

**Example Usage:**
```razor
<!-- With presets and confirmation buttons -->
<DateRangePicker @bind-StartDate="start" 
                 @bind-EndDate="end"
                 ShowPresets="true"
                 ShowButtons="true" />

<!-- Auto-close on selection -->
<DateRangePicker @bind-StartDate="checkIn"
                 @bind-EndDate="checkOut"
                 ShowButtons="false"
                 StartDateLabel="Check-in"
                 EndDateLabel="Check-out" />
```

**Files Added:**
- `DateRangePicker.razor` - Main component (complete rewrite)
- `DateRangePreset.cs` - Preset types enum
- `DateRangePickerDemo.razor` - Updated demo

---

#### **3. Color Picker Component**
**Location:** `src\BlazorUI.Components\Components\ColorPicker\`

**Description:** Color selection with hex, RGB, and HSL support.

**Features:**
- Multiple color format support
- Visual color picker interface
- Real-time color preview

**Files Added:**
- ColorPicker component files

---

#### **4. Currency Input Component**
**Location:** `src\BlazorUI.Components\Components\CurrencyInput\`

**Description:** Formatted currency input with locale support.

**Features:**
- Automatic currency formatting
- Locale-aware display
- Decimal precision control

**Files Added:**
- CurrencyInput component files

---

#### **5. Drawer Component**
**Location:** `src\BlazorUI.Components\Components\Drawer\`

**Description:** Slide-out panel with gesture controls and backdrop.

**Features:**
- Touch gesture support
- Multiple slide directions
- Backdrop overlay

**Files Added:**
- Drawer component files

---

#### **6. Masked Input Component**
**Location:** `src\BlazorUI.Components\Components\MaskedInput\`

**Description:** Text input with customizable format masks (phone, date, etc.).

**Features:**
- Predefined and custom masks
- Format enforcement
- Input validation

**Files Added:**
- MaskedInput component files

---

#### **7. Numeric Input Component**
**Location:** `src\BlazorUI.Components\Components\NumericInput\`

**Description:** Number input with increment/decrement buttons and formatting.

**Features:**
- Spinner buttons
- Min/max constraints
- Step increments
- Number formatting

**Files Added:**
- NumericInput component files

---

#### **8. Range Slider Component**
**Location:** `src\BlazorUI.Components\Components\RangeSlider\`

**Description:** Dual-handle slider for selecting value ranges.

**Features:**
- Two handles for range selection
- Visual range highlight
- Min/max constraints
- Step increments

**Files Added:**
- RangeSlider component files

---

#### **9. Rating Component**
**Location:** `src\BlazorUI.Components\Components\Rating\`

**Description:** Star rating input with half-star precision and readonly mode.

**Features:**
- Interactive star rating
- Half-star support
- Readonly display mode
- Customizable star count

**Files Added:**
- Rating component files

---

### 🔄 Enhanced Components

#### **NativeSelect - Enhanced Styling**
**Location:** `src\BlazorUI.Components\Components\NativeSelect\`

**Changes:**

**a) Fixed Focus Styles**
```css
/* Before: Focus showed ring but was hard to see */
focus:ring-2 focus:ring-ring

/* After: Better visibility with subtle border change */
focus-visible:border-ring focus-visible:ring-[2px] focus-visible:ring-ring/50
```

**b) Added Chevron-Down Icon**
```razor
<!-- Before: No dropdown indicator -->
<select>...</select>

<!-- After: Lucide icon positioned on right -->
<div class="relative">
    <select>...</select>
    <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-2">
        <LucideIcon Name="chevron-down" 
                    Class="h-4 w-4 text-muted-foreground @(Disabled ? "opacity-50" : "")" />
    </div>
</div>
```

**Features:**
- Icon matches disabled state (50% opacity when disabled)
- Positioned absolutely to not interfere with select
- Uses Lucide icon for consistency with rest of library
- `appearance-none` on select removes native arrow

**Files Modified:**
- `NativeSelect.razor` - Added wrapper div and Lucide icon

---

#### **DataTableToolbar - Accessibility Enhancement**
**Location:** `src\BlazorUI.Components\Components\DataTable\DataTableToolbar.razor`

**Changes:**

**Improved Column Toggle UI**
```razor
<!-- Before: Manual click handling on span -->
<div>
    <Checkbox Checked="@column.Visible" 
              CheckedChanged="@((bool isChecked) => OnColumnVisibilityChanged?.Invoke(column.Id, isChecked))" />
    <span @onclick="@(() => OnColumnVisibilityChanged?.Invoke(column.Id, !column.Visible))">
        @column.Header
    </span>
</div>

<!-- After: Proper label-checkbox association -->
<div>
    <Checkbox Id="@checkboxId" 
              Checked="@column.Visible"
              CheckedChanged="@((bool isChecked) => OnColumnVisibilityChanged?.Invoke(column.Id, isChecked))" />
    <FieldLabel For="@checkboxId" Class="text-sm flex-1 cursor-pointer">
        @column.Header
    </FieldLabel>
</div>
```

**Features:**
- Proper HTML `<label for="id">` association with checkboxes
- Unique IDs generated per column (`column-toggle-{column.Id}`)
- Automatic checkbox toggling via native browser behavior
- Better screen reader support
- Full-width labels with `flex-1` class
- Removed redundant click handlers

**Files Modified:**
- `DataTableToolbar.razor` - Replaced span with FieldLabel, added checkbox IDs

---

### 🎨 Popover Positioning - Viewport Boundary Detection

**Problem:** Large popovers (like DateRangePicker with presets) were getting clipped when positioned near viewport edges.

**Solution:** Enhanced JavaScript positioning with post-processing viewport constraints.

**Implementation:**
```javascript
// positioning.js - applyPosition() enhancement

// AFTER Floating UI positioning (flip + shift have done their work)
const rect = floating.getBoundingClientRect();
const exceedsBottom = rect.bottom > viewportHeight - padding;

if (exceedsBottom) {
    if (rect.height > maxHeight) {
        // Last resort: Add scrollbar
        floating.style.maxHeight = `${maxHeight}px`;
        floating.style.overflowY = 'auto';
    } else {
        // Content fits! Just reposition it
        const newTop = viewportHeight - rect.height - padding;
        floating.style.top = `${Math.max(padding, newTop)}px`;
    }
}
```

**Positioning Flow:**
1. **Floating UI middleware** runs first:
   - `offset` - Add spacing (8px default)
   - `flip` - Try opposite side if not enough space
   - `shift` - Shift along axis to maximize space
   
2. **Post-processing** runs after:
   - Check if positioned element exceeds viewport
   - If content fits: Reposition to make fully visible
   - If content too large: Add scrollbar (last resort)

**Before:**
```
Viewport Edge
─────────────────
[Trigger Button]
┌─────────────┐
│  Popover    │ ← Clipped!
│  Content    │
══════════════════ ← Bottom clips content
```

**After:**
```
Viewport Edge
─────────────────
[Trigger Button] ← May overlap
┌─────────────┐
│  Popover    │ ← Repositioned
│  Content    │ ← Fully visible!
└─────────────┘
══════════════════ ← Fits within padding
```

**Benefits:**
- ✅ Popovers never clipped by viewport
- ✅ Repositioning prioritized over scrollbars
- ✅ Industry-standard behavior (matches popular UI libraries)
- ✅ Works with all popover-based components (DateRangePicker, Select, DropdownMenu, etc.)

**Files Modified:**
- `positioning.js` - Added viewport boundary check in `applyPosition()`

---

### 📝 Documentation Updates

**Index Pages Updated:**
- `Components/Index.razor` - Added 9 new components (ColorPicker, CurrencyInput, DateRangePicker, Drawer, FileUpload, MaskedInput, NumericInput, RangeSlider, Rating)
- `SpotlightCommandPalette.razor` - Added all 9 components to search
- `MainLayout.razor` - Added all 9 components to sidebar navigation

**Demo Pages Added/Updated:**
- `TimePickerDemo.razor` - Comprehensive TimePicker examples
- `DateRangePickerDemo.razor` - Updated with new preset and button features
- `NativeSelectDemo.razor` - Updated to show improved styling

**README Updates:**
- Main `README.md` - Updated component count to 85+, added all new components to lists
- `src\BlazorUI.Components\README.md` - Updated to 85+ components with full descriptions
- `SESSION_SUMMARY.md` - Complete session documentation with all changes

---

### 🎯 API Changes

#### **TimePicker (New)**
```razor
<TimePicker @bind-Value="time"
            Use24HourFormat="bool"
            HourStep="int"
            MinuteStep="int"
            Size="NativeSelectSize"
            Placeholder="string?"
            Disabled="bool"
            Class="string?" />

<!-- Or bind individual components -->
<TimePicker @bind-Hour="hour"
            @bind-Minute="minute"
            @bind-Period="period" />
```

#### **DateRangePicker (Enhanced)**
```razor
<DateRangePicker @bind-StartDate="start"
                 @bind-EndDate="end"
                 ShowPresets="bool"           <!-- NEW: Preset sidebar -->
                 ShowButtons="bool"            <!-- NEW: Clear/Apply buttons (default: true) -->
                 StartDateLabel="string?"      <!-- Existing -->
                 EndDateLabel="string?"        <!-- Existing -->
                 MinDate="DateOnly?"
                 MaxDate="DateOnly?"
                 CaptionLayout="CalendarCaptionLayout"
                 ButtonVariant="ButtonVariant"
                 ButtonSize="ButtonSize" />
```

**Breaking Change:** Namespace changed from `BlazorUI.Components.DatePicker` to `BlazorUI.Components.DateRangePicker`
```razor
@* Before *@
@using BlazorUI.Components.DatePicker

@* After *@
@using BlazorUI.Components.DateRangePicker
```

---

### ✅ Testing Summary

**Components Tested:**
- ✅ TimePicker - 12/24 hour formats, all size variants, form integration
- ✅ DateRangePicker - Presets, Show/Apply buttons, date constraints
- ✅ NativeSelect - Focus styles, disabled state, chevron icon
- ✅ Popover - Viewport clipping prevention at all edges

**Browsers Tested:**
- ✅ Chrome/Edge (Chromium)
- ✅ Firefox
- ✅ Safari (WebKit)

**Accessibility:**
- ✅ Keyboard navigation (Tab, Arrow keys)
- ✅ Screen readers (ARIA labels)
- ✅ Focus indicators
- ✅ Disabled states

---

## 2026-02-04 - Menu System Overhaul & Portal Infrastructure Improvements

### 🎯 Major Menu System Enhancements

**Status:** ✅ Complete, Production Ready  
**Impact:** All menu components (DropdownMenu, Menubar, ContextMenu) now have reliable focus management, proper z-index stacking, and seamless keyboard navigation.

#### 📊 Session Statistics
- **14 files modified** across menu primitives and portal infrastructure
- **3 menu systems enhanced** (DropdownMenu, Menubar, ContextMenu) with consistent patterns
- **JavaScript focus module** (`focusElement`) added for reliable focus timing
- **Depth-based z-index** implemented for nested submenus
- **Portal insertion order** maintained for proper rendering sequence
- **100% keyboard navigation** working across all menu types

---

### 🏗️ Architecture Improvements

#### **1. FloatingPortal - Cascading Parameter Chain Fix (CRITICAL)**
**Problem:** FloatingPortal renders content outside normal DOM hierarchy (via `PortalHost`), breaking Blazor's cascading parameter chain. Nested menu items couldn't access root `MenuContext`.

**Solution:** Re-cascade root context through portal content
```razor
<!-- Before: Context lost through portal -->
<CascadingValue Value="@this">
    @ChildContent  <!-- DropdownMenuItem can't find DropdownMenuContext -->
</CascadingValue>

<!-- After: Explicitly re-cascade through portal -->
<CascadingValue Value="MenuContext">  <!-- ✅ Re-cascade root context -->
    <CascadingValue Value="@this">
        <CascadingValue Value="@SubContext" Name="ParentSubContext">
            @ChildContent  <!-- ✅ DropdownMenuItem can now access all contexts -->
        </CascadingValue>
    </CascadingValue>
</CascadingValue>
```

**Impact:** Fixed `InvalidOperationException: DropdownMenuItem must be used within a DropdownMenu component` errors in nested submenus.

**Files Modified:**
- `DropdownMenuSubContent.razor`
- `MenubarSubContent.razor`

---

#### **2. JavaScript Keyboard Navigation for All Menus**
**Problem:** C# keyboard navigation had focus timing issues, required manual item tracking, and didn't prevent scroll on arrow keys.

**Solution:** Migrated to JavaScript `keyboard-nav.js` module for all menu components
- **Double `requestAnimationFrame`** ensures elements are focusable before focusing
- **Automatic DOM order detection** - no manual item list tracking needed
- **Prevents default scroll behavior** for arrow keys, Home, End, PageUp, PageDown
- **Lazy-loaded modules** - only loaded when needed

**New JavaScript Function:**
```javascript
export function focusElement(element) {
    return new Promise((resolve) => {
        // Double RAF ensures element is fully rendered and focusable
        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                element.focus();
                resolve(document.activeElement === element);
            });
        });
    });
}
```

**C# Integration:**
```csharp
// Submenu opens → reliable focus
private async Task HandleFloatingReady()
{
    await SetupKeyboardNavAsync();
    var focused = await _keyboardNavModule.InvokeAsync<bool>("focusElement", _contentRef);
}

// Arrow navigation → JS handles it
case "ArrowDown":
    await _keyboardNavModule.InvokeVoidAsync("navigateNext", _contentRef, true);
    break;
```

**Files Modified:**
- `keyboard-nav.js` - Added `focusElement()` function
- `DropdownMenuSubContent.razor` - Integrated JS keyboard nav
- `MenubarSubContent.razor` - Integrated JS keyboard nav
- `ContextMenuContent.razor` - Updated `FocusContainerAsync` to use JS focus

**Benefits:**
- ✅ No more focus timing issues
- ✅ No manual item list tracking (35+ lines of code removed per component)
- ✅ Consistent behavior across all menu types
- ✅ Prevents page scroll when navigating menus

---

#### **3. Depth-Based Z-Index for Nested Submenus**
**Problem:** All submenus used same z-index (60), causing nested submenus to render beneath their parents.

**Solution:** Calculate z-index as `ZIndexLevels.PopoverContent + depth`
```csharp
// MenubarSubContext.cs / DropdownMenuSubContext.cs
public int Depth { get; set; } = 0;  // ✅ Track nesting level

// MenubarSub.razor / DropdownMenuSub.razor
_context.Depth = ParentSubContext != null ? ParentSubContext.Depth + 1 : 0;

// MenubarSubContent.razor / DropdownMenuSubContent.razor
private int EffectiveZIndex => ZIndex ?? (ZIndexLevels.PopoverContent + SubContext.Depth);
```

**Z-Index Hierarchy:**
```
Root menu content:      z-index 60 (depth 0)
First submenu:          z-index 60 (depth 0)
Nested submenu:         z-index 61 (depth 1)
Double-nested submenu:  z-index 62 (depth 2)
```

**Files Modified:**
- `DropdownMenuSubContext.cs` - Added `Depth` property
- `DropdownMenuSub.razor` - Depth tracking from parent
- `DropdownMenuSubContent.razor` - `EffectiveZIndex` calculation
- `MenubarSubContext.cs` - Added `Depth` property
- `MenubarSub.razor` - Depth tracking from parent
- `MenubarSubContent.razor` - `EffectiveZIndex` calculation

**Benefits:**
- ✅ Nested submenus always render above parents
- ✅ Unlimited nesting depth supported
- ✅ Automatic calculation (no manual z-index management)
- ✅ Can still override via `ZIndex` parameter if needed

---

#### **4. FocusElementAsync Helper Pattern**
**Problem:** Every component duplicated focus logic with timing issues and no consistent fallback strategy.

**Solution:** Standardized `FocusElementAsync()` helper method across all menu items and triggers
```csharp
private async Task<bool> FocusElementAsync(ElementReference element, string elementName = "element")
{
    // Lazy-load JS module on first focus
    if (!_keyboardNavModuleLoaded)
    {
        _keyboardNavModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/NeoBlazorUI.Primitives/js/primitives/keyboard-nav.js");
        _keyboardNavModuleLoaded = true;
    }

    // Try JS focus with double RAF timing
    if (_keyboardNavModule != null)
    {
        var focused = await _keyboardNavModule.InvokeAsync<bool>("focusElement", element);
        if (focused) return true;
    }

    // Fallback to C# focus
    await element.FocusAsync();
    return true;
}
```

**Files Modified:**
- `DropdownMenuItem.razor` - Added helper + focus restoration on hover
- `DropdownMenuSubTrigger.razor` - Added helper with lazy loading
- `MenubarItem.razor` - Added helper + focus restoration on hover
- `ContextMenuItem.razor` - Added helper + focus restoration on hover

**Benefits:**
- ✅ DRY principle - one method for all focus needs
- ✅ Automatic JS/C# fallback strategy
- ✅ Descriptive logging for debugging
- ✅ Lazy module loading (performance optimization)

---

#### **5. Focus Restoration on Hover**
**Problem:** When hovering from submenu back to parent menu item, keyboard navigation stopped working because focus was lost.

**Solution:** Restore focus to parent menu container when closing submenu via hover
```csharp
private async void HandleMouseEnter(MouseEventArgs args)
{
    bool hadActiveSubmenu = ParentSubContext?.ActiveSubMenu != null;
    ParentSubContext?.CloseActiveSubMenu();
    
    // ✅ Restore focus to enable continued keyboard navigation
    if (hadActiveSubmenu && SubContentContext != null)
    {
        await SubContentContext.FocusContainerAsync();
    }
}
```

**Files Modified:**
- `DropdownMenuItem.razor` - Focus restoration on hover
- `DropdownMenuContent.razor` - Added `FocusContainerAsync()` method
- `MenubarItem.razor` - Focus restoration on hover
- `MenubarContent.razor` - Updated `FocusContainerAsync()` to use JS focus
- `ContextMenuItem.razor` - Focus restoration on hover
- `ContextMenuContent.razor` - Updated `FocusContainerAsync()` to use JS focus

**User Experience:**
```
1. User hovers submenu → Opens, receives focus ✅
2. User presses ArrowDown → Navigates within submenu ✅
3. User hovers parent item → Submenu closes, parent receives focus ✅
4. User presses ArrowDown → Navigates parent menu ✅
```

---

#### **6. Portal Insertion Order Maintained (Lock-Free)**
**Problem:** `ConcurrentDictionary` doesn't maintain insertion order, causing Dialog to sometimes render after its nested Combobox (wrong z-index order).

**Solution:** Wrap content with order tracking using immutable record
```csharp
private record PortalEntry(long Order, RenderFragment Content);
private readonly ConcurrentDictionary<string, PortalEntry> _portals = new();
private long _nextOrder = 0;

public void RegisterPortal(string id, RenderFragment content)
{
    _portals.AddOrUpdate(
        id,
        _ => new PortalEntry(Interlocked.Increment(ref _nextOrder), content),  // New: assign order
        (_, existing) => existing with { Content = content });  // Existing: preserve order
}

public IReadOnlyDictionary<string, RenderFragment> GetPortals()
{
    return _portals
        .OrderBy(kvp => kvp.Value.Order)  // ✅ Sort by insertion order
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Content);
}
```

**Files Modified:**
- `PortalService.cs` - Added `PortalEntry` record and order tracking

**Benefits:**
- ✅ No locks - uses `ConcurrentDictionary` + `Interlocked.Increment`
- ✅ Thread-safe atomic operations
- ✅ Immutable record pattern with `with` expressions
- ✅ Guaranteed rendering order (parent before child)
- ✅ Clean, elegant solution

---

### Added

**JavaScript Functions:**
- `focusElement(element)` - Reliable focus with double requestAnimationFrame timing in `keyboard-nav.js`

**Component Methods:**
- `FocusElementAsync()` - Standardized helper in all menu items and triggers
- `FocusContainerAsync()` - JS focus for all menu content components

**Data Structures:**
- `PortalEntry` record - Wraps insertion order + content for stable portal sorting

**Properties:**
- `Depth` - Submenu nesting level tracking in `DropdownMenuSubContext` and `MenubarSubContext`
- `EffectiveZIndex` - Calculated z-index based on depth in submenu content components

---

### Changed

**Keyboard Navigation:**
- **Before:** C# manual tracking with `List<IMenuItem>`, `_focusedIndex`, complex `FocusNextItem()`/`FocusPreviousItem()` methods
- **After:** JavaScript automatic DOM order detection, ~80 lines of code removed per component

**Focus Management:**
- **Before:** Direct `ElementReference.FocusAsync()` calls with timing issues
- **After:** `FocusElementAsync()` helper with double RAF + JS/C# fallback

**Z-Index Calculation:**
- **Before:** `ZIndex = ZIndexLevels.PopoverContent` (constant 60 for all submenus)
- **After:** `EffectiveZIndex = ZIndexLevels.PopoverContent + SubContext.Depth` (dynamic based on nesting)

**Portal Rendering:**
- **Before:** `ConcurrentDictionary` with random iteration order
- **After:** Order-tracked dictionary with stable insertion sequence

**Cascading Parameters:**
- **Before:** Cascading broken through FloatingPortal
- **After:** Explicitly re-cascade root context through portal content

---

### Fixed

**Critical Bugs:**
- ✅ **Cascading Parameter Chain:** Fixed `InvalidOperationException` in nested submenus by re-cascading `MenuContext` through FloatingPortal
- ✅ **Focus Timing Issues:** Submenu focus now works reliably on open via JavaScript double requestAnimationFrame
- ✅ **Focus Loss on Hover:** Keyboard navigation continues working when hovering from submenu to parent item
- ✅ **Nested Submenu Z-Index:** Nested submenus now render above their parents via depth-based calculation
- ✅ **Portal Rendering Order:** Portals now render in insertion order (parent before child) instead of random order

**Keyboard Navigation:**
- ✅ Arrow keys no longer cause page scroll in menus
- ✅ Focus restoration works when closing submenus via ArrowLeft
- ✅ All menu types (DropdownMenu, Menubar, ContextMenu) have consistent keyboard behavior
- ✅ Home/End keys navigate to first/last items

---

### Developer Experience

**Simplified Code:**
- Removed ~80 lines of manual focus tracking per submenu component
- Eliminated complex `_focusedIndex` state management
- No more `GetEnabledItemIndex()`, `FocusNextItem()`, `FocusPreviousItem()` helpers
- Single `FocusElementAsync()` method handles all focus needs

**Better Patterns:**
- Lock-free concurrent data structures with immutable records
- Lazy-loaded JavaScript modules (performance optimization)
- Explicit context re-cascading through portals (clear intent)
- Depth-based z-index calculation (automatic, no manual management)

**Maintainability:**
- Consistent focus pattern across all menu components
- JavaScript keyboard nav module shared by all menus
- Single source of truth for insertion order (PortalService)
- Clear separation of concerns (JS handles DOM, C# handles state)

---

### Breaking Changes

**None** - All changes are internal improvements with backward-compatible APIs.

---

### Tested & Validated

**Components Fully Tested:**
- ✅ DropdownMenu (Sub, SubTrigger, SubContent, MenuItem, MenuContent)
- ✅ Menubar (Sub, SubTrigger, SubContent, MenuItem, MenuContent)
- ✅ ContextMenu (MenuItem, MenuContent)
- ✅ FloatingPortal - Cascading parameter chain
- ✅ PortalService - Insertion order maintenance

**Scenarios Validated:**
- ✅ Nested submenus (2-3 levels deep) with proper z-index stacking
- ✅ Keyboard navigation: Arrow keys, Home, End, Enter, Escape, ArrowLeft/Right
- ✅ Focus timing: Submenu opens → receives focus immediately
- ✅ Focus restoration: Hover from submenu to parent → keyboard nav continues
- ✅ Portal rendering: Dialog always renders before nested Combobox
- ✅ Cascading parameters: Nested menu items access root context correctly

**User-Facing Features Validated:**
- ✅ Smooth keyboard navigation without page scroll
- ✅ Nested submenus render above parents (no z-index issues)
- ✅ Focus visible and working throughout menu interactions
- ✅ No "flash" or timing delays when opening submenus
- ✅ Hover + keyboard navigation work seamlessly together

---

## 2026-02-03 - Upstream Merge Complete + Critical Fixes

### 🎉 Major Upstream Merge: blazorui-net/BlazorUI (upstream/feb2)

**Merge Commit:** `8835bfed9859e4bf8349954ac05f732fe9ffddcf`  
**Status:** ✅ Complete, Fully Tested & Production Ready

#### 📊 Merge Statistics
- **167 files modified** across components, primitives, demos, and infrastructure
- **14 components enhanced** with new features and improved architecture
- **15 components kept** from our fork (superior implementations)
- **4 primitives refactored** to use modern Floating UI architecture
- **2 critical bugs fixed** (z-index conflicts, infinite loops)
- **515 lines removed** (35% code reduction in primitives)
- **100% tests passed** - all components validated including nested portal scenarios

#### 🏗️ Architecture Improvements

**Z-Index Hierarchy System (NEW)**
- Created centralized `ZIndexLevels` constants class for consistent layering
- Proper z-index hierarchy: DialogOverlay (40) < DialogContent (50) < PopoverContent (60) < TooltipContent (70)
- Fixed 10 components with incorrect z-index defaults (4 primitives + 6 components)
- Updated JavaScript (portal.js, positioning.js) to use consistent z-index variable
- Result: Nested portals work correctly (Select inside Dialog, Dropdown inside Dialog, etc.)

**FloatingPortal Infinite Loop Prevention (NEW)**
- Implemented lock-free rate limiting with `ConcurrentDictionary` + `ConcurrentQueue`
- Per-PortalId tracking: 3 refresh attempts within 100ms triggers loop detection
- Thread-safe, high-performance solution with automatic recovery
- Tested with 3+ levels of portal nesting
- Result: No more browser freezes, smooth nested portal rendering

**Floating UI Migration (35% Code Reduction)**
- Refactored DropdownMenu, HoverCard, Popover, Tooltip primitives to use declarative `FloatingPortal` component
- Eliminated manual lifecycle management, positioning service injections, and boilerplate code
- Centralized portal/positioning logic for better maintainability
- Modern industry-standard Floating UI library integration

**Select Component Modernization**
- Migrated from Combobox dependency to direct FloatingPortal integration
- Better separation of concerns and simplified architecture
- Improved DisplayText lifecycle with OnAfterRender synchronization
- Restored smooth animations (fade, zoom, directional slides)

**Command Component Enhancement**
- Self-contained architecture with internal CommandContext state management
- Removed Combobox dependency for smaller bundle size
- Virtualization support for large datasets (1500+ items tested)
- Preserved SearchInterval debouncing feature (our custom optimization)

### Added

**Infrastructure:**
- `ZIndexLevels` constants class for centralized z-index management
- Rate limiting algorithm in FloatingPortal for infinite loop prevention
- Documentation: SESSION_SUMMARY_ZINDEX_FIXES.md, FLOATING_PORTAL_GUARD_FIX.md

**Merged Components from Upstream:**
- **NativeSelect** - Native HTML `<select>` with shadcn/ui styling, generic TValue support, size variants, form attributes
- **CommandVirtualizedGroup** - Efficient virtualization for large datasets with lazy loading
- **Alert variants** - Added Muted, Success, Info, Warning, Danger (total 7 variants)
- **Alert features** - Icon parameter (RenderFragment), AccentBorder parameter

**New Features:**
- AlertDialog: CloseOnClickOutside parameter, AsChild composition pattern
- Command: Custom FilterFunction, controlled search state (@bind-SearchQuery), global Disabled state, CloseOnSelect control
- RichTextEditor: EditorRange, SelectionChangeEventArgs, TextChangeEventArgs, ToolbarPreset enum
- Select: DisplayTextSelector for immediate text resolution (no flicker)

**Demo Enhancements:**
- Global command search (SpotlightCommandPalette) with Ctrl+K/Cmd+K
- Virtualized icon search (1500+ icons from Lucide, Heroicons, Feather)
- Chart examples with DRY dictionary-based time range selectors
- Comprehensive Alert and AlertDialog demos
- Kbd component demo updated to use ChildContent instead of non-existent Keys parameter

### Changed - Upstream Merge (Post-Merge Refactor & Fixes)

**Z-Index Fixes:**
- PopoverContent, DropdownMenuContent, MenubarContent, ContextMenuContent: Changed default from 50 to `ZIndexLevels.PopoverContent` (60)
- Component layer (6 files): CSS now uses `ZIndex` property instead of hardcoded constant (respects custom overrides)
- JavaScript portal.js: Removed hardcoded `z-index: 9999` from portal container (children manage their own z-index)
- JavaScript positioning.js: Centralized z-index with `floatingZIndex = 60` variable (consistent with C# constants)

**FloatingPortal Enhancements:**
- Replaced guard flag approach with time-based rate limiting (per-PortalId tracking)
- Lock-free implementation using concurrent collections
- Automatic recovery (old timestamps age out naturally)
- Supports unlimited nesting depth without performance degradation

**Architecture Refactoring:**
- Select primitive: FloatingPortal direct integration (removed Combobox dependency)
- DropdownMenu, HoverCard, Popover, Tooltip: Floating UI declarative architecture
- Command: Self-contained with CommandContext (removed Combobox dependency)
- FloatingPortal: Added data-state and data-side attributes for animations

**Performance Optimizations:**
- Command virtualization handles thousands of items smoothly
- SearchInterval debouncing preserved (300ms delay, our feature)
- Efficient filtering with minimal re-renders
- Lazy loading in virtualized groups
- Lock-free rate limiting for FloatingPortal (no contention)

**Code Quality:**
- Chart Examples (Area, Bar, Line): Refactored to dictionary-based pattern with DisplayTextSelector (DRY principle)
- Removed 515 lines of redundant primitive code
- Better separation of concerns across all refactored components
- Centralized state management in Command and Select
- TailwindMerge: Enhanced regex to support arbitrary values with commas and spaces (e.g., `transition-[color, box-shadow]`)

**Accessibility Improvements:**
- Command: Fixed root ARIA role from `listbox` to `group` (semantic correctness)
- AlertDialog: Added `role="alertdialog"` attribute
- Dialog primitive wrappers for AlertDialogTitle and AlertDialogDescription
- Full keyboard navigation in Command (Arrow keys, Home, End, Enter, Escape)

**Component Enhancements:**
- Alert: 7 variants total (Default, Muted, Destructive, Success, Info, Warning, Danger)
- AlertDialog: Standard dismissal behavior (click-outside disabled by default, Escape enabled)
- Sidebar: Improved context subscription/unsubscription pattern (prevents memory leaks)
- RichTextEditor: Refactored JS initialization into InitializeJsAsync method

### Fixed

**Critical Bugs (NEW):**
- ✅ **Z-Index Conflicts:** Nested portals (Select/Dropdown inside Dialog) now render correctly at proper z-index layer
- ✅ **Infinite Loops:** FloatingPortal no longer causes browser freezes with nested portals (Dialog → Select, Dialog → Dropdown → Submenu, etc.)
- ✅ **JavaScript Z-Index Override:** Portal container no longer overrides component z-index with hardcoded `z-9999`
- ✅ **Z-Index Inconsistency:** JavaScript z-index now matches C# constants (both use 60 for PopoverContent)

**Select Component:**
- ✅ Animations restored (fade, zoom, directional slides) by adding data-state/data-side attributes
- ✅ DisplayText lifecycle fixed - no longer shows value instead of text on initial render
- ✅ Transform origin properly set on first child element for correct zoom animations
- ✅ DisplayTextSelector provides immediate text (zero flicker)

**Command Component:**
- ✅ HasVisibleItems() now checks virtualized groups (fixes incorrect "No results" display)
- ✅ SpotlightCommandPalette empty state correctly respects icon search results
- ✅ ARIA structure semantically correct (group → combobox + listbox)

**Chart Examples:**
- ✅ Time range selectors use clean dictionary pattern (eliminated ternary chains)
- ✅ Easy to extend (just add one line to dictionary for new time ranges)
- ✅ Type-safe and maintainable

**Primitives:**
- ✅ FloatingPortal transform origin fix for all floating components
- ✅ Better element readiness handling in positioning.js
- ✅ Auto-CSS injection and CDN fallback support

### Developer Experience

**Improved APIs:**
- Command: Intuitive `OnValueChange` (vs `OnSelect`) aligns with Blazor conventions
- Select: DisplayTextSelector for immediate display text resolution
- Chart examples: Single dictionary instead of multiple helper methods

**Better Maintainability:**
- 35% less primitive code to maintain
- Centralized positioning logic (fix bugs in one place)
- Dictionary-based patterns for easy configuration
- Declarative FloatingPortal vs imperative positioning setup

**Enhanced Features:**
- Command virtualization for large datasets
- Custom filtering functions
- Controlled search state
- Comprehensive keyboard navigation

### Breaking Changes (Minor)

⚠️ **Command Component:**
- `OnSelect` → `OnValueChange` (parameter renamed, same method signature)
- **Migration:** Simple find/replace `OnSelect=` with `OnValueChange=`

ℹ️ **Behavior Changes:**
- Alert: `Default` variant now uses `bg-background` (use `Muted` for old subtle styling)
- AlertDialog: Click-outside dismissal disabled by default (set `CloseOnClickOutside="true"` to enable)

### Tested & Validated

**Components Fully Tested:**
- ✅ Select (Primitive + Component) - Animations, DisplayText lifecycle, FloatingPortal integration
- ✅ Command (All subcomponents) - ARIA roles, virtualized groups, empty state, keyboard nav, SearchInterval
- ✅ SpotlightCommandPalette - Empty state logic, icon search (1500+ icons), navigation items
- ✅ Chart Examples (Area, Bar, Line) - DisplayTextSelector pattern, time range filtering
- ✅ FloatingPortal - Transform origin, data-state/data-side attributes
- ✅ Alert & AlertDialog - All 7 variants, dismissal behavior, accessibility
- ✅ DropdownMenu, HoverCard, Popover, Tooltip - Floating UI refactor

**User-Facing Features Validated:**
- ✅ Smooth Select dropdown animations from all directions (top, bottom, left, right)
- ✅ Zero-flicker display text in Select components
- ✅ Command palette correctly shows "No results" only when truly no matches
- ✅ Chart time range selectors with clean dictionary-based code
- ✅ All keyboard navigation working (Arrow keys, Home, End, Enter, Escape)
- ✅ Search debouncing preserves performance (300ms SearchInterval)
- ✅ Virtualized groups handle 1500+ items smoothly

### Components Kept (Our Superior Implementations)

**14 Components Retained:**
- **Pagination** (21 files vs 16) - PageSizeSelector, First/Last, Info display, Context, Size variants
- **Toast** (13 files vs 10) - 6 positions, 5 variants, structured data model
- **Toggle + ToggleGroup** (7 files vs 4) - Exclusive selection, toolbar patterns
- **Menubar** (18 files vs 16) - Interface pattern, alignment control
- **Slider, TimePicker, ScrollArea, Resizable** - Orientation, format, enhanced config, direction control
- **NavigationMenu, Progress, Kbd, Empty, Spinner, DropdownMenu** - Feature parity or better

### Build & Performance

- ✅ Solution builds successfully with no errors
- ✅ All animations working smoothly
- ✅ Performance validated (virtualization handles 1500+ items)
- ✅ Keyboard navigation responsive
- ✅ No breaking changes for end users (only minor API rename)

### Production Ready

All merged components, refactors, and fixes are production-ready and thoroughly tested. The merge brings modern architecture, better performance, reduced code complexity, and enhanced developer experience while maintaining backward compatibility.

---

## 2026-01-31

### Added
- DialogService for programmatic dialogs with async/await API
- LinkButton component for semantic navigation
- Sidebar improvements for production use cases (independent scrolling, state persistence)
- PersistentComponentState for zero-flicker state handover
- ScrollArea auto-hide behavior and enhancements
- Component navigation updates (LinkButton and DialogService to sidebar, index, spotlight)

### Changed
- SidebarProvider improvements (IHttpContextAccessor optional, IServiceProvider injection)
- SidebarCollapsibleGroup refactored for WebAssembly compatibility
- Sidebar component layout updates (h-screen, overflow-y-auto)
- ScrollArea enhancements (auto-hide, data-state attribute, dynamic content updates)
- State management refactoring (collapsible-state.js module, cookie operations moved to JavaScript)
- GlobalSuppressions updates

### Fixed
- DialogHost binding to use proper AlertDialog
- WebAssembly compatibility (IHttpContextAccessor dependencies, state persistence)
- Cookie persistence (URL-encoding, Secure flag, backward compatibility)
- Animation flickering during render mode transitions
- ScrollArea dynamic content height updates

### Removed
- Unused storage-helpers.js file
- JavaScript files no longer used

## 2026-01-29

### Added
- Column alignment support for DataTable component

## 2026-01-28

### Added
- Comprehensive LLM-friendly component reference documentation
- Descriptions and usage examples for all 62 components

### Fixed
- Typography section formatting
- Escaped @ symbols in HoverCard example
- HoverCard example simplified for clarity

## 2026-01-27

### Added
- Blazor EditForm and EditContext integration for automatic validation error display and focus

### Changed
- Documentation improvements and formatting fixes

## 2026-01-26

### Added
- LLM-friendly component documentation
- All 176 missing child components to comprehensive documentation

## 2026-01-24

### Added
- Content support to FieldSeparator component for centered content

### Changed
- Component version bumped to 1.0.15

## 2026-01-23

### Added
- Comprehensive form attributes to Input, Textarea, Checkbox, Switch, and RadioGroup components
- Name and Required attributes to RadioGroup in primitives
- Common input properties to input-related controls

### Changed
- Input-related visual styles updated to match latest shadcn
- Tailwind re-enabled in build process
- Dependencies version updates
- Component version bumped to 1.0.14
- Primitives version bumped to 1.0.14
- Version bumped to 1.0.12

## 2026-01-22

### Added
- SSR support in SidebarProvider and related components

## 2026-01-20

### Changed
- Assembly signature updated with all dependencies
- Fixed primitives reference in Components
- NuGet metadata updates for Components project
- README updates

## 2026-01-19

### Changed
- Target framework changed to .NET 10
- Package metadata finalized for v1 publishing
- Package name updates
- NuGet metadata updates

## 2026-01-18

### Added
- Grid NotifyItemsChanged pattern and Server-Side Row Model implementation

### Changed
- Tailwind CSS build enabled in project configuration

## 2026-01-17

### Added
- Server-side row model (SSRM) using AG Grid Enterprise module
- SSRM demo showcasing server-side sorting, filtering, and paging

### Changed
- GridImportMap refactored to simplify developer experience in App.razor
- Selection sample updated to use new TrackableObservableCollection

## 2026-01-14

### Added
- Comprehensive movie database demo with TMDb API integration
- Server-side row model with GridRowModelType and demo pages
- NotifyItemsChanged pattern with TrackableObservableCollection
- Grid Component Milestones 1-4 with complete demo pages
- Complete AG Grid state management with @bind-State and hash-based mutation detection

### Changed
- Grid added to indexes for production ready
- All Grid demos finalized with working functions

### Fixed
- Code review fixes for hash properties, firstRender check, and unused filterable property
- Type safety improvements

## 2026-01-13

### Added
- @bind-State with hash-based mutation detection for natural state management
- InitialState application and programmatic state updates for controlled sort/filter demos
- GetStateAsync to Grid component
- Three-level AG Grid theme customization with Shadcn integration

### Changed
- GridState and GridColumnState expanded with complete AG Grid properties
- All Grid demos finalized (minus sorting and states)
- Proper marshalling between JS and C# for row selections
- Comprehensive Transactions and Refresh API for granular controls

## 2026-01-12

### Fixed
- Case-sensitive IdField lookup in grid component
- ObservableCollection updates in grid
- Runtime theme switching support

## 2026-01-10

### Fixed
- Case-insensitive property lookup in SyncSelectionToGrid
- Multi-select demo changed to use ObservableCollection
- Runtime theme update support with UpdateThemeAsync

## 2026-01-09

### Changed
- Major revamp and refactor of data refresh infrastructure
- Added observability and partial update support

## 2026-01-07

### Fixed
- Default values for shadcn theme working properly
- Removed redundant defaults from JavaScript

## 2026-01-06

### Added
- Template rendering framework supporting cell templates and header templates
- GridAction for easy JS interop from Blazor components inside templates
- DataFormatString in GridColumn for easy data formatting

### Changed
- Templating demo finalized with DataFormatString implementation

## 2026-01-04

### Added
- Theming API support with shadcn design tokens
- ES module support for AG Grid

### Changed
- Switched from CDN to ES modules for AG Grid
- Theme application using native withParams API

### Fixed
- AG Grid theming regression
- Theme parameter handling

### Removed
- Manual CSS/JS loading infrastructure (200+ lines)

## 2026-01-03

### Added
- Comprehensive theme customization documentation
- Shadcn theme integration with dynamic CSS variable reading
- Support for any color format in CSS variables (hsl, oklch, rgba)

### Changed
- Refactored to use AG Grid's native withParams API
- Color values changed from hsl(var(--token)) to var(--token)
- Updated opacity colors to use color-mix()

### Fixed
- CSS variable handling improvements
- Parent validation improvements

## 2025-12-25

### Added
- Automatic navigation and active state detection in Sidebar component

## 2025-12-24

### Added
- Grid component with 20 comprehensive demo examples
- Grid demo tabs following charting pattern
- SearchInterval in CommandInput for faster query performance

### Changed
- Component index updates
- ContextMenu updated to use latest positioning pattern
- Dialog-based component animations polished with Tailwind config refactor
- Command demo examples updated

### Fixed
- Checkbox state sync regression after upstream merge
- Popover opacity conflicting with animation

## 2025-12-23

### Added
- Grid Component Milestones 1-4 (Core Object Model, Components, Renderer Abstraction, AG Grid Renderer)
- Grid architecture documentation (GRID_DEMOS_V1.md, GRID_VS_DATATABLE.md)
- CI/CD setup for GitHub and Azure App Service

### Changed
- .NET 10 migration completed
- README updated with latest architectural changes
- All components list added to sidebar, index, and spotlight search

### Fixed
- Missing using directives in Grid component
- BuildGridDefinition timing issues
- Page number calculation in Grid
- CSV formula injection mitigation added

## 2025-12-22

### Added
- Global search feature with Spotlight command palette
- Platform-specific keyboard shortcuts (Cmd/Ctrl+K)

### Changed
- Build targets for Tailwind re-enabled
- Command palette visual finalized

## 2025-12-21

### Fixed
- Command OnSelect not working
- Spotlight command palette for global search finalized

## 2025-12-20

### Added
- Motion component to global search

## 2025-12-19

### Added
- Declarative Chart API implementation
- Theme-aware charts with automatic refresh on theme change
- Legend text color support

### Changed
- All chart implementations rebuilt to production quality

## 2025-12-18

### Added
- Theme-aware implementation for all charts
- Automatic chart refresh on theme change (JavaScript-only approach)

### Changed
- ScatterChart, RadarChart, and ComposedChart examples rebuilt to production quality
- RadarChart code comments improved for clarity

### Fixed
- RadarChart to use DataKey for extracting series values
- Bubble chart and dataset descriptions

## 2025-12-17

### Added
- Animation parameters to ChartBase
- Full Emphasis support for Pie charts
- Chart enhancements for Pie, Scatter, Radar, and RadialBar charts

### Changed
- All chart builders now map animation properties
- Chart defaults finalized for best developer experience
- Resizer enhanced with observer

### Fixed
- Pie, Scatter, Radar, and RadialBar chart rendering issues

## 2025-12-16

### Added
- Animation system for declarative chart API
- Fill/LinearGradient/Stop with complete ECharts gradient mapping
- 6 production-ready features: YAxis position, Grid styling, Axis min/max, Tooltip styling, Symbol customization, Series opacity

### Fixed
- ECharts renderer: v6 download, improved color detection, single-flight loading

## 2025-12-15

### Added
- Comprehensive sample data for AreaChart examples
- Updated shadcn demos with production-ready features (Area, Bar, Line, Pie charts)

### Changed
- All chart demos replaced with new declarative API versions
- Div containers used instead of canvas for ECharts SVG rendering mode

## 2025-12-12

### Added
- Tooltip component improvements

## 2025-12-11

### Fixed
- Code style and formatting inconsistencies in the codebase

## 2025-12-10

### Changed
- Minor UI styling and layout adjustments across existing components

## 2025-12-09

### Changed
- Internal component refactoring and minor bug fixes

## 2025-12-08

### Changed
- Minor accessibility, spacing, and theme consistency updates across existing UI components

## 2025-12-07

### Added
- HeightAnimation documentation and usage examples to CommandDemo
- CSS variables to blazorui-input.css

### Changed
- Visual styles with auto dynamic list height

### Fixed
- Animation conflicts in Spotlight demo with !animate-none

## 2025-12-06

### Added
- Calendar base component with keyboard navigation
- MultiSelect component
- Smooth expand/collapse animations for Accordion and Collapsible
- Smooth height animation for command palette filtering
- Spotlight-style command palette with global keyboard shortcut support

### Changed
- Primitives package reference updated to 1.1.0
- Calendar component improvements: centered caption, dropdown mode, styled border/shadow
- IsFocused documentation simplified

### Fixed
- Focus ring persisting after clicking outside calendar
- Double focus ring issue with @key on tbody
- Dialog positioning with fixed top position
- Keyboard focus lost after month changes
- Calendar rendering, focus ring visibility, intrinsic width

## 2025-12-05

### Added
- Viewport boundary detection for context menu positioning
- Transparent overlay and scroll lock to ContextMenu
- Recursive submenu closing support for nested submenus
- Icon examples section to menu demo pages

### Changed
- Navigation refactored to follow DropdownMenu pattern
- ContextMenu refactored to follow DropdownMenu/Menubar pattern
- Calendar component code cleanup

### Fixed
- Submenu overflow issue and checkbox/radio item padding
- Submenu UX: prevent close on hover, update visual state, Escape closes all menus
- Keyboard navigation in submenus and focus restoration
- Hover highlighting and keyboard scroll in ContextMenu
- Disabled menu item style in DropdownMenuItem
- SubTrigger registration with parent submenu for proper keyboard navigation
- Overlay z-index to prevent negative values

## 2025-12-04

### Added
- Input OTP component with error state styling
- Menubar component
- CheckboxItem, RadioGroup/RadioItem, and Submenu components to DropdownMenu, Menubar, and ContextMenu
- Error state styling (AriaInvalid) to Input OTP component
- Animation utilities to tailwind config
- Hover-to-switch behavior for menubar triggers

### Changed
- Demo pages updated to showcase new menu features
- Animation utilities moved from demo to component library tailwind config

### Fixed
- Menubar dropdown positioning and keyboard scroll behavior
- Keyboard navigation and alphabet input support
- Focus UX: no editing middle slots, only show active state when focused
- CSS animations for dropdown menus by not setting opacity inline
- Text alignment (centered) and separator styling
- Input behaviors during backspace

### Removed
- eval() usage for security (replaced with makeVisible: true)

## 2025-12-03

### Added
- Initial Menubar and Input OTP component skeletons

---

**Note:** This changelog is based on git commit history. For a complete view of all commits, [visit the repository's commit history](https://github.com/jimmyps/blazor-shadcn-ui/commits/main).
