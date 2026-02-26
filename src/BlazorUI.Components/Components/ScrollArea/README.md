# ScrollArea Component

A styled scrollable region with custom scrollbars, scroll shadows, and enhanced visual styling using shadcn/Radix patterns.

## Features

- **Custom Scrollbars**: Styled scrollbar track and thumb with hover/active states
- **Scroll Shadows**: Automatic shadows indicating more content is available (vertical and horizontal)
- **Orientation Support**: Vertical and horizontal scrolling
- **Configurable Behavior**: Auto, always visible, hover, or scroll-triggered
- **Tailwind Styled**: Uses theme tokens for consistent styling
- **JavaScript Interop**: Scroll position tracking for dynamic shadow rendering

## Usage

### Basic Vertical Scroll with Shadows

```razor
@using BlazorUI.Components.ScrollArea

<ScrollArea Class="h-72 w-48 rounded-md border">
    <div class="p-4">
        @for (int i = 1; i <= 50; i++)
        {
            <div class="text-sm">Item @i</div>
        }
    </div>
</ScrollArea>
```

### Horizontal Scroll with Shadows

```razor
<ScrollArea ShowVerticalScrollbar="false" ShowHorizontalScrollbar="true" Class="w-96 whitespace-nowrap rounded-md border">
    <div class="flex w-max space-x-4 p-4">
        @for (int i = 1; i <= 20; i++)
        {
            <div class="w-32 h-20 rounded-md bg-muted flex items-center justify-center">
                Card @i
            </div>
        }
    </div>
</ScrollArea>
```

### Disable Scroll Shadows

```razor
<ScrollArea EnableScrollShadows="false" Class="h-72 w-48 rounded-md border">
    <div class="p-4">
        @* Content without shadows *@
    </div>
</ScrollArea>
```

### Both Scrollbars

```razor
<ScrollArea ShowVerticalScrollbar="true" ShowHorizontalScrollbar="true" Class="h-72 w-72 rounded-md border">
    <div class="w-[600px] p-4">
        @* Wide content here *@
    </div>
</ScrollArea>
```

### Card with Long Content

```razor
<div class="w-80 rounded-lg border">
    <div class="p-4 border-b">
        <h3 class="font-semibold">Notifications</h3>
    </div>
    <ScrollArea Class="h-64">
        <div class="p-4">
            @foreach (var notification in Notifications)
            {
                <div class="mb-4 last:mb-0">
                    <p class="text-sm font-medium">@notification.Title</p>
                    <p class="text-sm text-muted-foreground">@notification.Description</p>
                </div>
            }
        </div>
    </ScrollArea>
</div>
```

## API Reference

### ScrollArea Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | The content to render within the scrollable area |
| `Type` | `ScrollAreaType` | `Auto` | The type of scrollbar behavior |
| `ScrollHideDelay` | `int` | `600` | Controls visibility delay of scrollbars in milliseconds |
| `ShowVerticalScrollbar` | `bool` | `true` | Whether to show the vertical scrollbar |
| `ShowHorizontalScrollbar` | `bool` | `false` | Whether to show the horizontal scrollbar |
| `EnableScrollShadows` | `bool` | `false` | Whether to enable scroll shadows indicating more content |
| `Class` | `string?` | `null` | Additional CSS classes to apply |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | Additional HTML attributes |

### ScrollAreaType Enum

| Value | Description |
|-------|-------------|
| `Auto` | Scrollbars visible when content overflows |
| `Always` | Scrollbars always visible |
| `Scroll` | Scrollbars appear only when scrolling |
| `Hover` | Scrollbars appear only on hover |

### ScrollBar Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Vertical` | The orientation of the scrollbar |
| `Class` | `string?` | `null` | Additional CSS classes to apply |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null` | Additional HTML attributes |

### Orientation Enum

| Value | Description |
|-------|-------------|
| `Vertical` | Vertical scrollbar |
| `Horizontal` | Horizontal scrollbar |

## Styling

The ScrollArea component uses CSS variables and Tailwind classes for theming:

- Scrollbar thumb uses `bg-border` with opacity variations for hover/active states
- Scroll shadows use `--background` CSS variable for seamless integration
- Shadows automatically fade in/out with 150ms transitions
- Enhanced visual appearance with subtle gradients and better contrast

## JavaScript Interop

The component uses JavaScript interop to:
- Track scroll position in real-time
- Update shadow visibility based on scroll state
- Provide smooth transitions for shadow appearance
- Handle resize events to update shadow state

No manual JavaScript configuration is needed - it's all handled automatically.
