# Menubar Portal ID Mismatch Fix

## Issue
**Error:** `Parent portal 'menubar-menu-menubar-38-menu-14-trigger' is not registered`

## Root Cause

MenubarSubContent was using the **wrong portal ID** to find its parent:

```csharp
// ❌ WRONG - MenubarSubContent was trying to append to:
return $"menubar-menu-{MenubarMenuContext.TriggerId}";
// Result: "menubar-menu-menubar-38-menu-14-trigger"
```

But MenubarContent actually registers with a **different ID**:

```csharp
// MenubarContent.razor line 10:
<FloatingPortal PortalId="@MenuContext.ContentId"
                ...>
// Result: "menubar-38-menu-14" (no prefix!)
```

The IDs didn't match, causing the "portal not registered" error.

## Solution

Updated MenubarSubContent to use the **exact same ID** as MenubarContent:

```csharp
// ✅ CORRECT - Use MenuContext.ContentId directly
return MenubarMenuContext != null ? MenubarMenuContext.ContentId : null;
// Result: "menubar-38-menu-14" (matches MenubarContent!)
```

## Portal ID Patterns Across Components

After fixing all three components, here are the correct patterns:

| Component | Root Portal ID | Submenu Parent Portal ID |
|-----------|----------------|-------------------------|
| **DropdownMenu** | `$"dropdown-portal-{MenuContext.ContentId}"` | `$"dropdown-portal-{MenuContext.ContentId}"` |
| **Menubar** | `MenuContext.ContentId` | `MenuContext.ContentId` |
| **ContextMenu** | `$"contextmenu-portal-{MenuContext.ContentId}"` | `$"contextmenu-portal-{MenuContext.ContentId}"` |

**Key Insight:** Menubar is unique in that it **doesn't use a prefix** for its portal ID, while DropdownMenu and ContextMenu do.

## Files Changed

**src/BlazorUI.Primitives/Primitives/Menubar/MenubarSubContent.razor**
- Changed portal ID from: `$"menubar-menu-{MenubarMenuContext.TriggerId}"`
- Changed to: `MenubarMenuContext.ContentId`
- **Added critical cascading:** Must re-cascade both `MenuContext` AND `MenubarMenuContext`

```razor
<CascadingValue Value="MenuContext" IsFixed="false">
    @* CRITICAL: Also cascade as MenubarMenuContext for nested submenus *@
    <CascadingValue Value="MenubarMenuContext" IsFixed="false">
        <CascadingValue Value="@this" IsFixed="false">
            <CascadingValue Value="@SubContext" Name="ParentSubContext" IsFixed="false">
                @ChildContent
            </CascadingValue>
        </CascadingValue>
    </CascadingValue>
</CascadingValue>
```

**Why Both Are Needed:**
- `MenuContext` - The MenubarMenuContext instance (used by MenubarContent)
- `MenubarMenuContext` - Explicit parameter name that nested submenus expect
- Without both, 3rd level submenus can't receive `MenubarMenuContext` and infinite loops occur

## Documentation Updated

1. **docs/NESTED_SUBMENU_FIX.md** - Added portal ID patterns table and pitfalls section
2. **docs/HIERARCHICAL_PORTALS.md** - Updated examples with correct IDs

## Verification Steps

To verify the fix:

1. Open a Menubar menu → ✅ Works
2. Open first submenu → ✅ Works
3. Open second submenu (nested) → ✅ Works
4. Open third submenu (deeply nested) → ✅ Works

## Lesson Learned

**Always verify the root content component's portal ID before implementing the submenu's `GetParentPortalId()`.**

Don't assume the portal ID pattern - check the actual `PortalId` parameter in the root component's FloatingPortal:

```razor
<!-- Step 1: Find root content component (e.g., MenubarContent.razor) -->
<FloatingPortal PortalId="@MenuContext.ContentId"  <!-- This is what you need to match! -->
                ...>
```

```csharp
// Step 2: Use the exact same value in submenu's GetParentPortalId()
private string? GetParentPortalId()
{
    return MenubarMenuContext != null ? MenubarMenuContext.ContentId : null;
}
```

## Status

✅ **Fixed** - MenubarSubContent now correctly resolves parent portal ID
✅ **Tested** - All nesting levels work
✅ **Documented** - Portal ID patterns documented for future reference
