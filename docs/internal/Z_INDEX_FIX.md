# Z-Index Layering Fix for Nested Portals

## Problem

When having nested floating elements (e.g., a Combobox/Select inside a Dialog), all components were using `z-50`, causing stacking order conflicts. The stacking became unpredictable and depended on DOM order rather than logical layering.

**Example Issue:**
```razor
<Dialog>  <!-- z-50 -->
    <DialogContent>
        <Select>  <!-- z-50 -->
            <SelectContent /> <!-- Also z-50 - CONFLICT! -->
        </Select>
    </DialogContent>
</Dialog>
```

The Select dropdown would sometimes appear behind the dialog overlay/content because they were competing at the same z-index level.

## Solution

Implemented a **layered z-index hierarchy** using centralized constants and removed hardcoded JavaScript z-index:

### Z-Index Hierarchy

| Component Type | Z-Index | Usage |
|---------------|---------|-------|
| **Dialog Overlay** | `z-40` | Background backdrop/darkening |
| **Dialog Content** | `z-50` | Dialog box itself |
| **Popover/Select/Menu** | `z-60` | Dropdowns, menus, selects, popovers |
| **Tooltip** | `z-70` | Always on top |

### Implementation

1. **Created `ZIndexLevels` Constants** (`src/NeoUI.Blazor.Primitives/Constants/ZIndexLevels.cs`)
   ```csharp
   public static class ZIndexLevels
   {
       public const int DialogOverlay = 40;
       public const int DialogContent = 50;
       public const int PopoverContent = 60;
       public const int TooltipContent = 70;
   }
   ```

2. **Fixed JavaScript Portal Container** (`src/NeoUI.Blazor.Primitives/wwwroot/js/primitives/portal.js`)
   - **Removed:** Hardcoded `z-index: 9999` from portal container
   - **Why:** The portal container should NOT have a z-index - it's just a positioning wrapper
   - **Result:** Individual portal contents now control their own z-index via CSS classes

   ```javascript
   // BEFORE (WRONG):
   container.style.zIndex = '9999'; // ❌ Overrides all z-index values!
   
   // AFTER (CORRECT):
   // No z-index on container - children manage their own ✅
   ```

3. **Updated All Components** to use the constants:
   - ✅ `DialogContent.razor` → `z-40` (overlay), `z-50` (content)
   - ✅ `PopoverContent.razor` → `z-60`
   - ✅ `SelectContent.razor` → `z-60`
   - ✅ `TooltipContent.razor` → `z-70`
   - ✅ `DropdownMenuContent.razor` → `z-60` (uses `ZIndex` property)
   - ✅ `DropdownMenuSubContent.razor` → `z-60` (uses `ZIndex` property)
   - ✅ `ContextMenuContent.razor` → `z-60` (uses `ZIndex` property)
   - ✅ `ContextMenuSubContent.razor` → `z-60` (uses `ZIndex` property)
   - ✅ `MenubarContent.razor` → `z-60` (uses `ZIndex` property)
   - ✅ `MenubarSubContent.razor` → `z-60` (uses `ZIndex` property)
   - ✅ `FloatingPortal.razor` → `z-60` (default)

4. **Updated Default Parameters** 
   - Changed `ZIndex` parameter defaults from `50` to `ZIndexLevels.PopoverContent` (60)
   - Components with `ZIndex` parameters use the **property value** in CSS (not hardcoded constant)
   - Added XML documentation explaining the hierarchy

## Benefits

### ✅ **Proper Stacking Order**
Now when you have nested portals, they stack correctly:
```
┌─────────────────────────────────────┐
│ Tooltip (z-70) ← Always on top      │
├─────────────────────────────────────┤
│ Dropdown/Menu (z-60) ← Above dialog │
├─────────────────────────────────────┤
│ Dialog Content (z-50)                │
├─────────────────────────────────────┤
│ Dialog Overlay (z-40) ← Backdrop     │
└─────────────────────────────────────┘
```

### ✅ **Predictable Behavior**
- Select/Combobox dropdowns inside dialogs now correctly appear **above** the dialog content
- Tooltips always appear on top, even when hovering over menu items
- No more stacking order surprises

### ✅ **Centralized Management**
- All z-index values defined in one place (`ZIndexLevels.cs`)
- Easy to maintain and adjust if needed
- Type-safe and refactor-friendly

### ✅ **Backward Compatible**
- Components still accept custom `ZIndex` parameters
- Only changed defaults (from 50 to appropriate level)
- No breaking changes for existing code

## Example Use Case

**Before (Broken):**
```razor
<Dialog>
    <DialogContent>
        <!-- Select dropdown appears BEHIND dialog due to z-50 conflict -->
        <Select @bind-Value="selectedValue">
            <SelectTrigger>
                <SelectValue placeholder="Choose..." />
            </SelectTrigger>
            <SelectContent>  <!-- z-50 conflicts with Dialog! -->
                <SelectItem Value="1">Option 1</SelectItem>
            </SelectContent>
        </Select>
    </DialogContent>
</Dialog>
```

**After (Fixed):**
```razor
<Dialog>
    <DialogContent>  <!-- z-50 -->
        <!-- Select dropdown appears ABOVE dialog correctly! -->
        <Select @bind-Value="selectedValue">
            <SelectTrigger>
                <SelectValue placeholder="Choose..." />
            </SelectTrigger>
            <SelectContent>  <!-- z-60 → appears above dialog ✅ -->
                <SelectItem Value="1">Option 1</SelectItem>
            </SelectContent>
        </Select>
    </DialogContent>
</Dialog>
```

The Select dropdown now correctly appears above the dialog content and overlay.

## Files Modified

**New Files:**
- `src/NeoUI.Blazor.Primitives/Constants/ZIndexLevels.cs`

**Updated Components (14 files):**
- `src/NeoUI.Blazor/Components/Dialog/DialogContent.razor`
- `src/NeoUI.Blazor/Components/Popover/PopoverContent.razor`
- `src/NeoUI.Blazor/Components/Select/SelectContent.razor`
- `src/NeoUI.Blazor/Components/Tooltip/TooltipContent.razor`
- `src/NeoUI.Blazor/Components/DropdownMenu/DropdownMenuContent.razor`
- `src/NeoUI.Blazor/Components/DropdownMenu/DropdownMenuSubContent.razor`
- `src/NeoUI.Blazor/Components/ContextMenu/ContextMenuContent.razor`
- `src/NeoUI.Blazor/Components/ContextMenu/ContextMenuSubContent.razor`
- `src/NeoUI.Blazor/Components/Menubar/MenubarContent.razor`
- `src/NeoUI.Blazor/Components/Menubar/MenubarSubContent.razor`
- `src/NeoUI.Blazor.Primitives/Primitives/Floating/FloatingPortal.razor`

**Updated JavaScript (1 file):**
- `src/NeoUI.Blazor.Primitives/wwwroot/js/primitives/portal.js` - Removed hardcoded `z-index: 9999` from portal container

## Custom Z-Index Override

If you need a custom z-index for specific scenarios, you can still override:

```razor
<!-- Custom z-index for special cases -->
<PopoverContent ZIndex="100" Class="custom-popover">
    <!-- content -->
</PopoverContent>
```

But in most cases, the default hierarchy works perfectly! 🎯
