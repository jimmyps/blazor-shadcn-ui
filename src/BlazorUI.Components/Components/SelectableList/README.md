# SelectableList Component

An elegant, declarative component for creating styled selectable lists with built-in search and keyboard navigation. Leverages CommandList for accessibility and listbox semantics.

## Features

- **Two Usage Modes**: Automatic data-driven rendering or manual composition
- **Declarative API**: Clean, simple component-based approach
- **Keyboard Navigation**: Built-in arrow key navigation and selection
- **Optional Search**: Toggle search input for filtering items
- **Grouping Support**: Organize items into labeled groups
- **Fully Customizable**: Custom item templates and styling
- **Accessibility**: Proper listbox semantics and ARIA attributes
- **Type-Safe**: Generic component works with any data type

## Usage Modes

SelectableList supports two distinct usage patterns:

### Mode 1: Automatic Rendering (Data-Driven)

Best for: Simple lists where items come from a data source

```razor
<SelectableList Items="plans"
                ValueSelector="p => p.Id"
                @bind-SelectedValue="selectedPlan">
    <ItemTemplate Context="plan">
        <div>@plan.Name - @plan.Price</div>
    </ItemTemplate>
</SelectableList>
```

### Mode 2: Manual Composition (Direct Command Components)

Best for: Complex layouts, mixed content, or when you need full control

```razor
<SelectableList @bind-SelectedValue="selected">
    <CommandGroup Heading="Suggestions">
        <CommandItem Value="calendar">
            <LucideIcon Name="calendar" Size="16" Class="mr-2" />
            Calendar
        </CommandItem>
        <CommandItem Value="calculator">
            <LucideIcon Name="calculator" Size="16" Class="mr-2" />
            Calculator
        </CommandItem>
    </CommandGroup>
    <CommandSeparator />
    <CommandGroup Heading="Settings">
        <CommandItem Value="profile">Profile</CommandItem>
        <CommandItem Value="settings">Settings</CommandItem>
    </CommandGroup>
</SelectableList>
```

## Advantages Over RadioGroup Pattern

**Before (RadioGroup):**
```razor
<!-- Messy pattern with manual handlers -->
<RadioGroup @bind-Value="selected">
    <div @onclick="@(() => selected = "value")"
         class="...">
        <div @onclick:stopPropagation>
            <RadioGroupItem Value="value" />
        </div>
        <div>Content...</div>
    </div>
</RadioGroup>
```

**After (SelectableList):**
```razor
<!-- Clean, declarative pattern -->
<SelectableList Items="plans"
                ValueSelector="p => p.Id"
                @bind-SelectedValue="selectedPlan">
    <ItemTemplate Context="plan">
        <div class="flex items-center justify-between">
            <div>
                <div class="font-medium">@plan.Name</div>
                <div class="text-sm text-muted-foreground">@plan.Description</div>
            </div>
            <div class="font-bold">@plan.Price</div>
        </div>
    </ItemTemplate>
</SelectableList>
```

## Basic Usage

### Mode 1: Automatic Rendering

#### Simple List

```razor
@using BlazorUI.Components.SelectableList

<SelectableList Items="@plans"
                ValueSelector="@(p => p.Id)"
                @bind-SelectedValue="@selectedPlanId">
    <ItemTemplate Context="plan">
        <div class="flex flex-col">
            <span class="font-medium">@plan.Name</span>
            <span class="text-sm text-muted-foreground">@plan.Description</span>
        </div>
    </ItemTemplate>
</SelectableList>

@code {
    private string? selectedPlanId;
    
    private List<Plan> plans = new()
    {
        new Plan { Id = "free", Name = "Free", Description = "For personal use" },
        new Plan { Id = "pro", Name = "Pro", Description = "For professionals" },
        new Plan { Id = "team", Name = "Team", Description = "For teams" }
    };
}
```

#### With Search

```razor
<SelectableList Items="@options"
                ValueSelector="@(o => o.Value)"
                @bind-SelectedValue="@selectedOption"
                ShowSearch="true"
                SearchPlaceholder="Search options...">
    <ItemTemplate Context="option">
        <div>@option.Label</div>
    </ItemTemplate>
</SelectableList>
```

#### With Grouping

```razor
<SelectableList Items="@settings"
                ValueSelector="@(s => s.Id)"
                GroupSelector="@(s => s.Category)"
                @bind-SelectedValue="@selectedSetting">
    <ItemTemplate Context="setting">
        <div class="flex items-center gap-3">
            <LucideIcon Name="@setting.Icon" Size="16" />
            <div>
                <div class="font-medium">@setting.Name</div>
                <div class="text-xs text-muted-foreground">@setting.Description</div>
            </div>
        </div>
    </ItemTemplate>
</SelectableList>

@code {
    private List<Setting> settings = new()
    {
        new Setting { Id = "profile", Category = "Account", Name = "Profile", Icon = "user" },
        new Setting { Id = "security", Category = "Account", Name = "Security", Icon = "shield" },
        new Setting { Id = "theme", Category = "Appearance", Name = "Theme", Icon = "palette" },
        new Setting { Id = "language", Category = "Appearance", Name = "Language", Icon = "globe" }
    };
}
```

#### Disabled Items

```razor
<SelectableList Items="@features"
                ValueSelector="@(f => f.Id)"
                IsDisabled="@(f => !f.Available)"
                @bind-SelectedValue="@selectedFeature">
    <ItemTemplate Context="feature">
        <div class="flex justify-between">
            <span>@feature.Name</span>
            @if (!feature.Available)
            {
                <span class="text-xs text-muted-foreground">Coming Soon</span>
            }
        </div>
    </ItemTemplate>
</SelectableList>
```

#### Custom Search Text

```razor
<SelectableList Items="@users"
                ValueSelector="@(u => u.Id)"
                SearchTextSelector="@(u => $"{u.Name} {u.Email}")"
                ShowSearch="true"
                @bind-SelectedValue="@selectedUserId">
    <ItemTemplate Context="user">
        <div>
            <div class="font-medium">@user.Name</div>
            <div class="text-sm text-muted-foreground">@user.Email</div>
        </div>
    </ItemTemplate>
</SelectableList>
```

### Mode 2: Manual Composition

#### With Command Components Directly

Use this mode when you need full control over the structure, want to mix different content types, or have complex requirements.

```razor
@using BlazorUI.Components.SelectableList
@using BlazorUI.Components.Command

<SelectableList @bind-SelectedValue="@selectedAction">
    <CommandGroup Heading="Actions">
        <CommandItem Value="new-file">
            <LucideIcon Name="file-plus" Size="16" Class="mr-2" />
            New File
        </CommandItem>
        <CommandItem Value="new-folder">
            <LucideIcon Name="folder-plus" Size="16" Class="mr-2" />
            New Folder
        </CommandItem>
    </CommandGroup>
    <CommandSeparator />
    <CommandGroup Heading="Edit">
        <CommandItem Value="copy">
            <LucideIcon Name="copy" Size="16" Class="mr-2" />
            Copy
        </CommandItem>
        <CommandItem Value="paste">
            <LucideIcon Name="clipboard" Size="16" Class="mr-2" />
            Paste
        </CommandItem>
    </CommandGroup>
</SelectableList>

@code {
    private string? selectedAction;
}
```

#### With Search in Manual Mode

```razor
<SelectableList @bind-SelectedValue="@selected" ShowSearch="true" SearchPlaceholder="Search commands...">
    <CommandGroup Heading="Suggestions">
        <CommandItem Value="calendar" SearchText="calendar schedule">
            <LucideIcon Name="calendar" Size="16" Class="mr-2" />
            Calendar
        </CommandItem>
        <CommandItem Value="calculator" SearchText="calculator math">
            <LucideIcon Name="calculator" Size="16" Class="mr-2" />
            Calculator
        </CommandItem>
    </CommandGroup>
    <CommandGroup Heading="Settings">
        <CommandItem Value="profile" SearchText="profile user account">
            <LucideIcon Name="user" Size="16" Class="mr-2" />
            Profile
        </CommandItem>
    </CommandGroup>
</SelectableList>
```

#### Disabled Items in Manual Mode

```razor
<SelectableList @bind-SelectedValue="@selected">
    <CommandGroup Heading="Available">
        <CommandItem Value="option1">Option 1</CommandItem>
        <CommandItem Value="option2">Option 2</CommandItem>
    </CommandGroup>
    <CommandGroup Heading="Coming Soon">
        <CommandItem Value="option3" Disabled="true">
            Option 3 (Coming Soon)
        </CommandItem>
        <CommandItem Value="option4" Disabled="true">
            Option 4 (Coming Soon)
        </CommandItem>
    </CommandGroup>
</SelectableList>
```

### With Custom Styling

```razor
<SelectableList Items="@plans"
                ValueSelector="@(p => p.Id)"
                ContainerClass="border-2"
                ItemClass="py-3 px-4"
                @bind-SelectedValue="@selectedPlan">
    <ItemTemplate Context="plan">
        <div class="flex items-center justify-between gap-4">
            <div class="flex-1">
                <div class="flex items-center gap-2">
                    <span class="font-semibold text-base">@plan.Name</span>
                    @if (plan.Popular)
                    {
                        <span class="px-2 py-0.5 text-xs font-medium bg-primary text-primary-foreground rounded">Popular</span>
                    }
                </div>
                <p class="text-sm text-muted-foreground mt-1">@plan.Description</p>
                <ul class="mt-2 space-y-1">
                    @foreach (var feature in plan.Features)
                    {
                        <li class="text-xs text-muted-foreground flex items-center gap-1.5">
                            <LucideIcon Name="check" Size="12" Class="text-primary" />
                            @feature
                        </li>
                    }
                </ul>
            </div>
            <div class="text-right">
                <div class="text-2xl font-bold">@plan.Price</div>
                <div class="text-xs text-muted-foreground">per month</div>
            </div>
        </div>
    </ItemTemplate>
</SelectableList>
```

## API Reference

### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | Conditional* | - | Collection of items to display (automatic mode) |
| `ValueSelector` | `Func<TItem, string>?` | Conditional* | - | Function to extract the value from an item (automatic mode) |
| `ItemTemplate` | `RenderFragment<TItem>?` | Conditional* | - | Template for rendering each item (automatic mode) |
| `ChildContent` | `RenderFragment?` | Conditional** | `null` | Manual composition content using CommandGroup/CommandItem |
| `SelectedValue` | `string?` | No | `null` | Currently selected value |
| `SelectedValueChanged` | `EventCallback<string>` | No | - | Callback invoked when selection changes |
| `SearchTextSelector` | `Func<TItem, string>?` | No | `null` | Function to extract search text (automatic mode). If not provided, uses `ValueSelector` |
| `GroupSelector` | `Func<TItem, string>?` | No | `null` | Function to group items (automatic mode). If provided, items are grouped with headings |
| `ShowSearch` | `bool` | No | `false` | Whether to show the search input |
| `SearchPlaceholder` | `string` | No | `"Search..."` | Placeholder text for the search input |
| `ContainerClass` | `string?` | No | `null` | Additional CSS classes for the container |
| `ItemClass` | `string?` | No | `null` | Additional CSS classes for each item (automatic mode only) |
| `IsDisabled` | `Func<TItem, bool>?` | No | `null` | Function that determines if an item is disabled (automatic mode only) |

\* Required when using automatic mode (not using `ChildContent`)  
\*\* Required when not using automatic mode parameters (`Items`, `ValueSelector`, `ItemTemplate`)

### Usage Modes

**Automatic Mode**: Provide `Items`, `ValueSelector`, and `ItemTemplate`. The component automatically renders CommandGroup and CommandItem.

**Manual Mode**: Provide `ChildContent` with CommandGroup and CommandItem directly. Gives full control over structure.

## Examples

### Settings Panel

```razor
@page "/settings"
@using BlazorUI.Components.SelectableList

<div class="max-w-2xl mx-auto p-6">
    <h1 class="text-2xl font-bold mb-6">Settings</h1>
    
    <SelectableList Items="@settingOptions"
                    ValueSelector="@(s => s.Id)"
                    GroupSelector="@(s => s.Category)"
                    @bind-SelectedValue="@selectedSetting"
                    ContainerClass="border rounded-lg"
                    SelectedValueChanged="@OnSettingChanged">
        <ItemTemplate Context="setting">
            <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
                    <LucideIcon Name="@setting.Icon" Size="18" Class="text-primary" />
                </div>
                <div class="flex-1">
                    <div class="font-medium">@setting.Title</div>
                    <div class="text-sm text-muted-foreground">@setting.Description</div>
                </div>
                @if (selectedSetting == setting.Id)
                {
                    <LucideIcon Name="check" Size="20" Class="text-primary" />
                }
            </div>
        </ItemTemplate>
    </SelectableList>
    
    @if (!string.IsNullOrEmpty(selectedSetting))
    {
        <div class="mt-6 p-4 border rounded-lg bg-muted/50">
            <h2 class="font-medium mb-2">Selected Setting</h2>
            <p class="text-sm text-muted-foreground">
                You selected: @settingOptions.FirstOrDefault(s => s.Id == selectedSetting)?.Title
            </p>
        </div>
    }
</div>

@code {
    private string? selectedSetting = "profile";
    
    private void OnSettingChanged(string settingId)
    {
        // Handle setting change
        Console.WriteLine($"Setting changed to: {settingId}");
    }
}
```

### Pricing Plan Selection

```razor
<div class="max-w-4xl mx-auto">
    <div class="text-center mb-8">
        <h2 class="text-3xl font-bold">Choose Your Plan</h2>
        <p class="text-muted-foreground mt-2">Select the perfect plan for your needs</p>
    </div>
    
    <SelectableList Items="@pricingPlans"
                    ValueSelector="@(p => p.Id)"
                    @bind-SelectedValue="@selectedPlanId"
                    ContainerClass="border-2 rounded-xl shadow-sm">
        <ItemTemplate Context="plan">
            <div class="flex items-center gap-6 p-2">
                <div class="flex-shrink-0">
                    <div class="w-12 h-12 rounded-lg bg-gradient-to-br @plan.GradientClass flex items-center justify-center">
                        <LucideIcon Name="@plan.Icon" Size="24" Class="text-white" />
                    </div>
                </div>
                <div class="flex-1">
                    <div class="flex items-center gap-2 mb-1">
                        <h3 class="text-lg font-semibold">@plan.Name</h3>
                        @if (plan.Recommended)
                        {
                            <span class="px-2 py-0.5 text-xs font-medium bg-primary text-primary-foreground rounded-full">
                                Recommended
                            </span>
                        }
                    </div>
                    <p class="text-sm text-muted-foreground">@plan.Description</p>
                </div>
                <div class="flex-shrink-0 text-right">
                    <div class="text-3xl font-bold">@plan.Price</div>
                    <div class="text-sm text-muted-foreground">@plan.Billing</div>
                </div>
            </div>
        </ItemTemplate>
    </SelectableList>
</div>
```

## Keyboard Navigation

The SelectableList component supports full keyboard navigation:

- **↑/↓ Arrow Keys**: Navigate between items
- **Home**: Jump to first item
- **End**: Jump to last item
- **Enter**: Select focused item
- **Type to search**: Filter items (when `ShowSearch` is enabled)

## Accessibility

- Proper `role="listbox"` semantics
- ARIA attributes for screen readers
- Keyboard navigation support
- Focus management
- Disabled state handling

## Related Components

- [Command](/components/command) - Base command menu component
- [RadioGroup](/components/radiogroup) - Traditional radio group
- [Select](/components/select) - Dropdown select component
