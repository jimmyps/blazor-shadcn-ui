# Mobile Development Best Practices for BlazorUI

This guide provides best practices and patterns for building mobile-responsive applications with BlazorUI components.

## Table of Contents

1. [Quick Start](#quick-start)
2. [Responsive Layout Patterns](#responsive-layout-patterns)
3. [Component-Specific Guidelines](#component-specific-guidelines)
4. [Touch Optimization](#touch-optimization)
5. [Performance Tips](#performance-tips)
6. [Testing Your Mobile UI](#testing-your-mobile-ui)

---

## Quick Start

### Essential Viewport Configuration

Always include the viewport meta tag in your `App.razor` or `index.html`:

```html
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
```

### Mobile Breakpoints

BlazorUI uses Tailwind's mobile-first breakpoints:

```
- Mobile:    0px - 767px    (default, no prefix)
- Tablet:    768px - 1023px (md:)
- Desktop:   1024px+        (lg:, xl:, 2xl:)
```

**Primary Mobile/Desktop Threshold:** `md:` at **768px**

---

## Responsive Layout Patterns

### 1. Hide/Show Based on Viewport

```razor
<!-- Visible only on mobile -->
<div class="md:hidden">
    <MobileNavigation />
</div>

<!-- Visible only on desktop -->
<div class="hidden md:block">
    <DesktopNavigation />
</div>
```

### 2. Stack on Mobile, Grid on Desktop

```razor
<!-- Single column on mobile, 3 columns on desktop -->
<div class="grid grid-cols-1 md:grid-cols-3 gap-4">
    <Card>Card 1</Card>
    <Card>Card 2</Card>
    <Card>Card 3</Card>
</div>

<!-- Responsive gap -->
<div class="grid grid-cols-1 md:grid-cols-2 gap-2 md:gap-4 lg:gap-6">
    <!-- Smaller gap on mobile, larger on desktop -->
</div>
```

### 3. Responsive Forms

```razor
<!-- Vertical on mobile, horizontal on desktop -->
<div class="flex flex-col md:flex-row gap-4">
    <Field Class="w-full md:w-1/2">
        <Label>First Name</Label>
        <Input @bind-Value="firstName" />
    </Field>
    
    <Field Class="w-full md:w-1/2">
        <Label>Last Name</Label>
        <Input @bind-Value="lastName" />
    </Field>
</div>
```

### 4. Responsive Padding and Spacing

```razor
<!-- Smaller padding on mobile, larger on desktop -->
<div class="p-4 md:p-6 lg:p-8">
    <h1 class="text-2xl md:text-4xl lg:text-5xl">
        Responsive Heading
    </h1>
    
    <p class="mt-2 md:mt-4 text-sm md:text-base">
        Responsive text and spacing
    </p>
</div>
```

### 5. Sidebar Pattern (Recommended)

Use the built-in Sidebar component which automatically transforms into a mobile Sheet:

```razor
<SidebarProvider>
    <Sidebar>
        <SidebarHeader>
            <!-- Your logo and branding -->
        </SidebarHeader>
        
        <SidebarContent>
            <SidebarMenu>
                <SidebarMenuItem>
                    <SidebarMenuButton Href="/">Home</SidebarMenuButton>
                </SidebarMenuItem>
                <!-- More menu items -->
            </SidebarMenu>
        </SidebarContent>
    </Sidebar>
    
    <main>
        <!-- Page content -->
        <SidebarTrigger Class="md:hidden" /> <!-- Mobile hamburger -->
        @Body
    </main>
</SidebarProvider>
```

**What it does:**
- Desktop (≥768px): Persistent sidebar
- Mobile (<768px): Hidden sidebar, opens as Sheet when triggered

---

## Component-Specific Guidelines

### Buttons

✅ **Recommended Touch Target Size:** 44px × 44px minimum

```razor
<!-- Good: Default button (40px) - acceptable -->
<Button>Click Me</Button>

<!-- Better: Large button (44px) - optimal for mobile -->
<Button Size="ButtonSize.Lg">Click Me</Button>

<!-- Icon buttons - ensure adequate size -->
<Button Size="ButtonSize.Icon" Class="min-w-[44px] min-h-[44px]">
    <Icon Name="menu" />
</Button>
```

**Button Groups on Mobile:**

```razor
<!-- Stack vertically on mobile -->
<div class="flex flex-col md:flex-row gap-2">
    <Button Variant="ButtonVariant.Default">Save</Button>
    <Button Variant="ButtonVariant.Outline">Cancel</Button>
</div>

<!-- Or use responsive button group -->
<ButtonGroup Class="flex-col md:flex-row">
    <Button>Option 1</Button>
    <Button>Option 2</Button>
    <Button>Option 3</Button>
</ButtonGroup>
```

### Data Tables

⚠️ **Challenge:** Wide tables don't fit on mobile screens

**Pattern 1: Horizontal Scroll with Indicators**

```razor
<div class="relative">
    <!-- Scroll container -->
    <div class="overflow-x-auto rounded-md border">
        <DataTable Data="@users" />
    </div>
    
    <!-- Optional: Scroll indicator -->
    <div class="md:hidden text-center text-sm text-muted-foreground mt-2">
        ← Swipe to see more →
    </div>
</div>
```

**Pattern 2: Card Layout for Mobile** (Recommended)

```razor
<!-- Desktop: Table -->
<div class="hidden md:block">
    <DataTable Data="@users" />
</div>

<!-- Mobile: Card Stack -->
<div class="md:hidden space-y-3">
    @foreach (var user in users)
    {
        <Card>
            <CardHeader>
                <CardTitle>@user.Name</CardTitle>
                <CardDescription>@user.Email</CardDescription>
            </CardHeader>
            <CardContent>
                <div class="space-y-1 text-sm">
                    <div><strong>Role:</strong> @user.Role</div>
                    <div><strong>Status:</strong> @user.Status</div>
                    <div><strong>Joined:</strong> @user.JoinDate.ToShortDateString()</div>
                </div>
            </CardContent>
            <CardFooter>
                <Button Size="ButtonSize.Sm" Variant="ButtonVariant.Outline" Class="w-full">
                    View Details
                </Button>
            </CardFooter>
        </Card>
    }
</div>
```

**Pattern 3: Column Hiding**

```razor
<DataTable Data="@users">
    <Columns>
        <DataTableColumn Field="Name" />
        <DataTableColumn Field="Email" />
        
        <!-- Hide on mobile -->
        <DataTableColumn Field="Phone" Class="hidden md:table-cell" />
        <DataTableColumn Field="Department" Class="hidden lg:table-cell" />
        
        <DataTableColumn Field="Actions" />
    </Columns>
</DataTable>
```

### Dialogs

✅ **Good news:** Dialogs automatically adapt to mobile!

```razor
<Dialog>
    <DialogTrigger AsChild>
        <Button>Open Dialog</Button>
    </DialogTrigger>
    
    <DialogContent Class="max-w-lg"> <!-- Becomes full-width on mobile -->
        <DialogHeader>
            <DialogTitle>Mobile-Friendly Dialog</DialogTitle>
            <DialogDescription>
                This dialog automatically adapts to mobile screens.
            </DialogDescription>
        </DialogHeader>
        
        <!-- Form content -->
        <div class="space-y-4">
            <Field>
                <Label>Name</Label>
                <Input />
            </Field>
        </div>
        
        <DialogFooter Class="flex-col md:flex-row gap-2">
            <DialogClose AsChild>
                <Button Variant="ButtonVariant.Outline" Class="w-full md:w-auto">
                    Cancel
                </Button>
            </DialogClose>
            <Button Class="w-full md:w-auto">Save</Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
```

**Tips:**
- Footer buttons stack vertically on mobile
- Use `w-full md:w-auto` for responsive button widths
- Keep form fields simple and single-column on mobile

### Tabs

```razor
<!-- Horizontal scroll on mobile when too many tabs -->
<Tabs DefaultValue="tab1" Class="w-full">
    <TabsList Class="w-full overflow-x-auto flex-nowrap md:flex-wrap">
        <TabsTrigger Value="tab1">Overview</TabsTrigger>
        <TabsTrigger Value="tab2">Analytics</TabsTrigger>
        <TabsTrigger Value="tab3">Reports</TabsTrigger>
        <TabsTrigger Value="tab4">Settings</TabsTrigger>
    </TabsList>
    
    <TabsContent Value="tab1">
        <!-- Content -->
    </TabsContent>
</Tabs>
```

**Alternative: Vertical Tabs on Mobile**

```razor
<Tabs DefaultValue="tab1" Orientation="@(isMobile ? Orientation.Vertical : Orientation.Horizontal)">
    <!-- Tabs -->
</Tabs>

@code {
    private bool isMobile = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            isMobile = await JSRuntime.InvokeAsync<bool>("matchMedia", "(max-width: 768px)");
        }
    }
}
```

### Cards

✅ Cards are naturally responsive!

```razor
<!-- Responsive grid of cards -->
<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
    @foreach (var item in items)
    {
        <Card>
            <CardHeader>
                <CardTitle>@item.Title</CardTitle>
            </CardHeader>
            <CardContent>
                <p>@item.Description</p>
            </CardContent>
            <CardFooter>
                <Button Size="ButtonSize.Sm" Class="w-full">Action</Button>
            </CardFooter>
        </Card>
    }
</div>
```

### Input Fields

**Font Size Consideration:** iOS Safari zooms in on inputs with font-size < 16px

```razor
<!-- ✅ Good: 16px or larger -->
<Input Class="text-base" /> <!-- Default is usually 16px -->

<!-- ❌ Avoid: Small text -->
<Input Class="text-sm" /> <!-- May trigger zoom on iOS -->
```

**Input Groups on Mobile:**

```razor
<!-- Stack addons on mobile -->
<InputGroup>
    <InputGroupAddon Class="hidden md:flex">$</InputGroupAddon>
    <Input Type="number" Placeholder="Amount" />
    <InputGroupAddon>.00</InputGroupAddon>
</InputGroup>

<!-- Or use mobile-friendly layout -->
<div class="space-y-2 md:space-y-0">
    <div class="flex items-center gap-2">
        <span class="md:hidden text-sm">$</span>
        <Input Type="number" />
    </div>
</div>
```

### Popovers and Dropdowns

✅ Automatically reposition on mobile!

```razor
<Popover>
    <PopoverTrigger AsChild>
        <Button>Show Options</Button>
    </PopoverTrigger>
    
    <PopoverContent Class="w-80 md:w-96">
        <!-- Content automatically repositions to stay in viewport -->
        <div class="space-y-2">
            <h4>Mobile-Friendly Popover</h4>
            <p>This popover adjusts its position on mobile screens.</p>
        </div>
    </PopoverContent>
</Popover>
```

**Tip:** Use `Sheet` instead of `Popover` for complex mobile interactions:

```razor
<!-- Desktop: Popover, Mobile: Sheet -->
@if (isMobile)
{
    <Sheet>
        <SheetTrigger AsChild>
            <Button>Options</Button>
        </SheetTrigger>
        <SheetContent>
            <SheetHeader>
                <SheetTitle>Options</SheetTitle>
            </SheetHeader>
            <!-- Full-screen mobile experience -->
        </SheetContent>
    </Sheet>
}
else
{
    <Popover>
        <PopoverTrigger AsChild>
            <Button>Options</Button>
        </PopoverTrigger>
        <PopoverContent>
            <!-- Desktop dropdown -->
        </PopoverContent>
    </Popover>
}
```

---

## Touch Optimization

### 1. Minimum Touch Target Sizes

**WCAG 2.1 Level AA Requirement:** 44px × 44px

```razor
<!-- ✅ Good touch targets -->
<Button Size="ButtonSize.Lg">Large Button (44px)</Button>
<Button Class="min-h-[44px] min-w-[44px]">Custom Size</Button>

<!-- ⚠️ May be too small -->
<Button Size="ButtonSize.Sm">Small (32px)</Button>

<!-- Fix: Add padding -->
<Button Size="ButtonSize.Sm" Class="min-h-[44px] px-4">
    Small with Padding
</Button>
```

### 2. Spacing Between Interactive Elements

```razor
<!-- ✅ Adequate spacing prevents mis-taps -->
<div class="flex flex-wrap gap-3">
    <Button>Button 1</Button>
    <Button>Button 2</Button>
    <Button>Button 3</Button>
</div>

<!-- ❌ Too cramped -->
<div class="flex gap-1">
    <Button>Button 1</Button>
    <Button>Button 2</Button>
</div>
```

### 3. Touch-Friendly Lists

```razor
<div class="space-y-1">
    @foreach (var item in items)
    {
        <!-- Adequate tap target with padding -->
        <button @onclick="() => SelectItem(item)" 
                class="w-full p-3 text-left rounded-md hover:bg-accent">
            <div class="font-medium">@item.Title</div>
            <div class="text-sm text-muted-foreground">@item.Description</div>
        </button>
    }
</div>
```

### 4. Swipe Gestures (Sheet Component)

The Sheet component supports swipe-to-close on mobile:

```razor
<Sheet>
    <SheetTrigger AsChild>
        <Button>Open Drawer</Button>
    </SheetTrigger>
    
    <SheetContent Side="SheetSide.Bottom">
        <!-- User can swipe down to close -->
        <SheetHeader>
            <SheetTitle>Swipe to Close</SheetTitle>
        </SheetHeader>
    </SheetContent>
</Sheet>
```

---

## Performance Tips

### 1. Lazy Load Images

```razor
<img src="@imageUrl" 
     loading="lazy" 
     alt="@imageAlt"
     class="w-full h-auto" />
```

### 2. Responsive Images

```razor
<img srcset="@($"{imageUrl}-small.jpg 400w, {imageUrl}-medium.jpg 800w, {imageUrl}-large.jpg 1200w")"
     sizes="(max-width: 768px) 100vw, (max-width: 1024px) 50vw, 33vw"
     src="@($"{imageUrl}-medium.jpg")"
     alt="@imageAlt" />
```

### 3. Avoid Heavy Animations on Mobile

```razor
<div class="@(isMobile ? "" : "transition-transform hover:scale-105")">
    <!-- Skip heavy animations on mobile -->
</div>
```

### 4. Respect Reduced Motion

```razor
<!-- Disable animations for users who prefer reduced motion -->
<div class="transition-opacity duration-300 motion-reduce:transition-none">
    Content
</div>
```

---

## Testing Your Mobile UI

### 1. Browser DevTools

**Chrome DevTools:**
1. Press `F12` or `Ctrl+Shift+I`
2. Click device toolbar icon (or `Ctrl+Shift+M`)
3. Test at these viewports:
   - 375×667 (iPhone SE)
   - 390×844 (iPhone 12/13)
   - 393×852 (iPhone 14 Pro)
   - 360×740 (Android - Galaxy S20)
   - 768×1024 (iPad)

### 2. Test Checklist

For each page/component:

- [ ] **Layout:** No horizontal scroll on mobile
- [ ] **Text:** Readable without zooming (≥16px)
- [ ] **Buttons:** Touch targets ≥44px × 44px
- [ ] **Forms:** Single column on mobile
- [ ] **Tables:** Scrollable or alternative layout
- [ ] **Navigation:** Accessible hamburger menu
- [ ] **Images:** Scale properly, no overflow
- [ ] **Spacing:** Adequate padding (≥16px)
- [ ] **Modals:** Full width or properly sized
- [ ] **Orientation:** Works in portrait and landscape

### 3. Real Device Testing

**Recommended Test Devices:**
- iPhone SE (smallest modern iPhone)
- iPhone 14/15 (current generation)
- Samsung Galaxy S21+ (Android flagship)
- iPad (tablet breakpoint)

### 4. Accessibility Testing

```razor
<!-- Use semantic HTML -->
<nav>
    <button aria-label="Open menu" class="md:hidden">
        <MenuIcon />
        <span class="sr-only">Toggle navigation</span>
    </button>
</nav>

<!-- Ensure keyboard navigation works -->
<button class="focus-visible:ring-2 focus-visible:ring-ring">
    Focusable Button
</button>
```

---

## Common Mobile Issues and Solutions

### Issue 1: Horizontal Scrolling

**Problem:** Content overflows viewport width

**Solution:**
```razor
<!-- Add to root layout -->
<body class="overflow-x-hidden">
    @Body
</body>

<!-- Or specific containers -->
<div class="max-w-full overflow-x-hidden">
    <!-- Content -->
</div>
```

### Issue 2: Text Too Small

**Problem:** Font sizes < 16px trigger zoom on iOS

**Solution:**
```razor
<!-- Use base size (16px) or larger -->
<Input Class="text-base" /> <!-- 16px -->
<Input Class="text-lg" />   <!-- 18px -->
```

### Issue 3: Buttons Too Close Together

**Problem:** Difficult to tap accurately

**Solution:**
```razor
<!-- Add spacing -->
<div class="flex flex-col gap-3 md:flex-row">
    <Button>Action 1</Button>
    <Button>Action 2</Button>
</div>
```

### Issue 4: Modal Too Tall for Viewport

**Problem:** Dialog content scrolls off screen

**Solution:**
```razor
<DialogContent Class="max-h-[90vh] overflow-y-auto">
    <!-- Long content scrolls within dialog -->
</DialogContent>
```

### Issue 5: Fixed Positioning Issues

**Problem:** Fixed elements (headers, footers) behave oddly on mobile

**Solution:**
```razor
<!-- Use sticky instead of fixed on mobile -->
<header class="sticky md:fixed top-0 z-50">
    <!-- Header content -->
</header>
```

---

## Additional Resources

### Documentation
- [Tailwind CSS Responsive Design](https://tailwindcss.com/docs/responsive-design)
- [MDN: Mobile Web Development](https://developer.mozilla.org/en-US/docs/Web/Guide/Mobile)
- [WCAG 2.1 Mobile Accessibility](https://www.w3.org/WAI/WCAG21/Understanding/)

### Tools
- [Responsive Design Checker](https://responsivedesignchecker.com/)
- [Google Mobile-Friendly Test](https://search.google.com/test/mobile-friendly)
- [Lighthouse Mobile Audit](https://developers.google.com/web/tools/lighthouse)

### BlazorUI Specific
- [Component Documentation](/docs)
- [Mobile Analysis Report](/docs/MOBILE_RESPONSIVE_ANALYSIS.md)
- [Example Implementations](/demo)

---

## Quick Reference: Responsive Class Patterns

```razor
<!-- Visibility -->
class="hidden md:block"          <!-- Hide mobile, show desktop -->
class="md:hidden"                 <!-- Show mobile, hide desktop -->

<!-- Layout -->
class="flex flex-col md:flex-row" <!-- Stack mobile, row desktop -->
class="grid grid-cols-1 md:grid-cols-3" <!-- 1 col mobile, 3 desktop -->

<!-- Sizing -->
class="w-full md:w-auto"         <!-- Full width mobile, auto desktop -->
class="w-full md:w-1/2 lg:w-1/3" <!-- Responsive widths -->

<!-- Spacing -->
class="p-4 md:p-6 lg:p-8"        <!-- Responsive padding -->
class="gap-2 md:gap-4"           <!-- Responsive gaps -->

<!-- Typography -->
class="text-sm md:text-base lg:text-lg" <!-- Responsive font size -->

<!-- Display -->
class="block md:flex"            <!-- Block mobile, flex desktop -->
class="relative md:absolute"     <!-- Responsive positioning -->
```

---

**Best Practice Summary:**

1. ✅ **Mobile-First:** Design for mobile, enhance for desktop
2. ✅ **Touch-Friendly:** 44px minimum touch targets
3. ✅ **Readable Text:** 16px minimum font size
4. ✅ **Flexible Layouts:** Use flexbox and grid
5. ✅ **Test Early:** Check mobile at every stage
6. ✅ **Accessibility:** ARIA labels, keyboard navigation
7. ✅ **Performance:** Lazy load, optimize images

---

**Questions or Issues?**

File an issue on [GitHub](https://github.com/jimmyps/blazor-shadcn-ui/issues) with the `mobile` label.
