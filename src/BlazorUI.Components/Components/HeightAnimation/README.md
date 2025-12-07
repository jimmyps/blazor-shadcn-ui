# HeightAnimation Component

A reusable Blazor component that provides smooth height transitions for dynamic content using ResizeObserver and MutationObserver.

## Features

- Smooth height animations using CSS transitions
- Observes content changes automatically (ResizeObserver + MutationObserver)
- Configurable via parameters
- CSS variables for easy customization
- Works with any component that has dynamic content

## Usage

### Basic Example

```razor
@using BlazorUI.Components.HeightAnimation

<HeightAnimation Config="@_config">
    <Command OnSelect="@HandleSelect">
        <CommandInput Placeholder="Search..." />
        <CommandList>
            <!-- Dynamic content here -->
        </CommandList>
    </Command>
</HeightAnimation>

@code {
    private HeightAnimationConfig _config = new()
    {
        ContentSelector = "[role=\"listbox\"]",
        InputSelector = ".command-input-wrapper",
        IncludeInputHeight = true
    };
}
```

### With Combobox

```razor
<HeightAnimation Config="@_comboboxConfig">
    <Combobox>
        <ComboboxTrigger>Select item...</ComboboxTrigger>
        <ComboboxContent>
            <!-- Dynamic items -->
        </ComboboxContent>
    </Combobox>
</HeightAnimation>

@code {
    private HeightAnimationConfig _comboboxConfig = new()
    {
        ContentSelector = "[role=\"listbox\"]",
        MaxHeight = 300 // Optional: cap the maximum height
    };
}
```

## Configuration

### HeightAnimationConfig Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `ContentSelector` | `string` | Yes | CSS selector for the content element to observe (e.g., `'[role="listbox"]'`, `'.content'`) |
| `InputSelector` | `string?` | No | CSS selector for a fixed-height header/input element |
| `MaxHeight` | `int?` | No | Maximum height in pixels. If null, uses initial content height as max |
| `IncludeInputHeight` | `bool` | No | Whether to include input/header height in total (default: `true`) |

### Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Config` | `HeightAnimationConfig?` | null | Configuration for animation behavior |
| `ChildContent` | `RenderFragment?` | null | Content to render inside the animated container |
| `Class` | `string?` | null | Additional CSS classes for the container |
| `Style` | `string?` | null | Additional inline styles for the container |
| `Enabled` | `bool` | `true` | Whether animation is enabled |

## CSS Variables

You can customize the animation behavior by overriding these CSS variables in your theme:

```css
:root {
    /* Animation duration */
    --height-animation-duration: 200ms;
    
    /* Animation timing function */
    --height-animation-timing: cubic-bezier(0.4, 0, 0.2, 1);
}

/* Different timing for specific components */
.my-custom-component {
    --height-animation-duration: 300ms;
    --height-animation-timing: ease-in-out;
}
```

### Example: Faster Animation

```css
.fast-animation {
    --height-animation-duration: 150ms;
}
```

```razor
<HeightAnimation Config="@_config" Class="fast-animation">
    <!-- Content -->
</HeightAnimation>
```

### Example: Custom Easing

```css
.bounce-animation {
    --height-animation-timing: cubic-bezier(0.68, -0.55, 0.265, 1.55);
}
```

## How It Works

1. **Container Setup**: The component wraps your content in a container with CSS transitions
2. **Element Selection**: Uses the configured selectors to find content and optional header elements
3. **Observer Setup**: Attaches ResizeObserver and MutationObserver to the content element
4. **Height Calculation**: Calculates total height based on content + header (if configured)
5. **Animation**: Sets the container height, letting CSS transitions handle the animation

## Best Practices

### Selector Specificity

Use specific selectors to avoid conflicts:

```csharp
// Good - specific selector
ContentSelector = "[role=\"listbox\"][data-command-list]"

// Less specific - may match multiple elements
ContentSelector = ".list"
```

### Performance

- The component uses `will-change: height` for better performance
- Observers are properly cleaned up on disposal
- Animation only runs when `Enabled = true`

### Overflow Handling

Add overflow styles to your content element:

```css
.height-animation-container [role="listbox"] {
    overflow-y: auto;
    overflow-x: hidden;
}
```

## Example: Spotlight Command Palette

See `demo/BlazorUI.Demo/Shared/SpotlightCommandPalette.razor` for a complete example of using HeightAnimation with a Command component.

## Troubleshooting

### Animation Not Working

1. Check that `ContentSelector` matches an element in the DOM
2. Verify the component renders after the dialog/popover is open
3. Ensure `Enabled = true` when you want the animation
4. Check browser console for JavaScript errors

### Height Jumps Instead of Animating

1. Verify CSS variables are loaded
2. Check that no other CSS is overriding the transition
3. Ensure the container isn't constrained by a parent's height

### Content Selector Not Found

The JavaScript will log a warning if the selector doesn't match. Check:
1. Selector syntax is correct
2. Element exists when `OnAfterRenderAsync` runs
3. Component structure matches your expectations
