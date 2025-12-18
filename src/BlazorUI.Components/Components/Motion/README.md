# Motion Component

A **declarative, strongly-typed animation system** for Blazor that wraps the [Motion.dev](https://motion.dev) JavaScript library.

## Features

- 🎯 **Declarative API** - Define animations directly in Razor markup
- 💪 **Strongly Typed** - Full IntelliSense support for all animation properties
- 🎨 **Composable Presets** - Combine multiple animation effects
- ♿ **Accessible** - Respects user's reduced motion preferences
- 📱 **Responsive** - Works across all Blazor hosting models
- 🔧 **Flexible** - Use presets or define custom keyframes

## Installation

### 1. Add Motion.dev Library

Add the Motion.dev script to your `App.razor` or `_Host.cshtml`:

```html
<script src="https://cdn.jsdelivr.net/npm/motion@10.16.2/dist/motion.js"></script>
```

Or install via npm:

```bash
npm install motion
```

### 2. Import Namespace

Add to your `_Imports.razor`:

```razor
@using BlazorUI.Components.Motion
```

## Basic Usage

### Simple Fade In

```razor
<Motion Trigger="@MotionTrigger.OnAppear">
    <Presets>
        <FadeIn />
    </Presets>
    <div class="card">Hello World!</div>
</Motion>
```

### Combining Animations

```razor
<Motion Trigger="@MotionTrigger.OnAppear">
    <Presets>
        <FadeIn />
        <ScaleIn From="0.6" />
        <Spring Stiffness="200" Damping="22" />
    </Presets>
    <div class="card">Fade + Scale with spring physics</div>
</Motion>
```

## Triggers

### On Appear (Default)

Animates when the component first renders:

```razor
<Motion Trigger="@MotionTrigger.OnAppear">
    <Presets><FadeIn /></Presets>
    <div>Content</div>
</Motion>
```

### On In View

Animates when element scrolls into viewport:

```razor
<Motion Trigger="@MotionTrigger.OnInView" 
        InViewOptions="@(new InViewOptions { Threshold = 0.5, Once = true })">
    <Presets>
        <FadeInOnScroll />
        <SlideInOnScroll />
    </Presets>
    <div>Appears on scroll</div>
</Motion>
```

### Manual

Trigger animation programmatically:

```razor
<Motion @ref="_motion" Trigger="@MotionTrigger.Manual">
    <Presets><ShakeX /></Presets>
    <div>Click button to shake</div>
</Motion>

<Button OnClick="@(() => _motion.PlayAsync())">Shake It!</Button>

@code {
    private Motion _motion = default!;
}
```

## Presets

### Presence Animations

**Entry:**
- `<FadeIn />` - Fade from transparent to opaque
- `<ScaleIn />` - Scale from small to full size
- `<SlideInFromTop />` - Slide in from top
- `<SlideInFromBottom />` - Slide in from bottom
- `<SlideInFromLeft />` - Slide in from left
- `<SlideInFromRight />` - Slide in from right

**Exit:**
- `<FadeOut />` - Fade to transparent
- `<ScaleOut />` - Scale down

### Micro-Interactions

- `<Pulse />` - Pulsing scale animation
- `<BounceOnce />` - Single bounce effect
- `<ShakeX />` - Horizontal shake (great for errors)
- `<ShakeY />` - Vertical shake

### List & Grid Animations

- `<ListItemEnter />` - Polished list item entry
- `<ListItemExit />` - Polished list item exit
- `<GridItemEnter />` - Grid item entry with scale

### Scroll-Based Animations

- `<FadeInOnScroll />` - Fade in on viewport entry
- `<SlideInOnScroll />` - Slide up on viewport entry
- `<ExpandOnScroll />` - Scale up on viewport entry

### Physics

- `<Spring />` - Apply spring physics to animations

## Staggered Animations

Animate list items with delays:

```razor
<Motion Trigger="@MotionTrigger.OnAppear" StaggerChildren="0.05">
    @foreach (var item in Items)
    {
        <Motion>
            <Presets><ListItemEnter /></Presets>
            <div class="card">@item</div>
        </Motion>
    }
</Motion>
```

## Custom Keyframes

Define custom animations without presets:

```razor
<Motion Trigger="@MotionTrigger.OnAppear"
        Keyframes="@_customKeyframes"
        Options="@_options">
    <div>Custom animation</div>
</Motion>

@code {
    private List<MotionKeyframe> _customKeyframes = new()
    {
        new MotionKeyframe { Opacity = 0, Scale = 0.5, Y = "100px" },
        new MotionKeyframe { Opacity = 1, Scale = 1, Y = "0" }
    };

    private MotionOptions _options = new()
    {
        Duration = 0.6,
        Easing = MotionEasing.EaseOut
    };
}
```

## Spring Physics

Add natural, physics-based easing:

```razor
<Motion Trigger="@MotionTrigger.OnAppear">
    <Presets>
        <ScaleIn From="0.5" />
        <Spring Mass="1.0" Stiffness="250" Damping="20" />
    </Presets>
    <div>Bouncy animation</div>
</Motion>
```

### Spring Parameters

- **Mass** - Weight of the object (higher = slower)
- **Stiffness** - Spring tension (higher = snappier)
- **Damping** - Resistance (higher = less oscillation)
- **Bounce** - Alternative to stiffness/damping (0-1, higher = bouncier)

## Accessibility

### Reduced Motion

By default, Motion respects the user's `prefers-reduced-motion` setting:

```razor
<Motion RespectReducedMotion="true">
    <!-- Animation will be skipped if user prefers reduced motion -->
</Motion>
```

To force animations even with reduced motion preference:

```razor
<Motion RespectReducedMotion="false">
    <!-- Animation always plays -->
</Motion>
```

## Advanced Examples

### Dialog Entry Animation

```razor
<Dialog>
    <DialogTrigger AsChild>
        <Button>Open Dialog</Button>
    </DialogTrigger>
    
    <Motion Trigger="@MotionTrigger.OnAppear">
        <Presets>
            <FadeIn />
            <ScaleIn From="0.8" />
            <Spring Stiffness="200" Damping="25" />
        </Presets>
        <DialogContent>
            <DialogTitle>Animated Dialog</DialogTitle>
            <DialogDescription>
                This dialog slides in with spring physics!
            </DialogDescription>
        </DialogContent>
    </Motion>
</Dialog>
```

### Scroll-Triggered Cards

```razor
@foreach (var card in Cards)
{
    <Motion Trigger="@MotionTrigger.OnInView" 
            InViewOptions="@(new InViewOptions { Threshold = 0.2 })">
        <Presets>
            <FadeInOnScroll />
            <SlideInOnScroll From="100px" />
        </Presets>
        
        <Card class="mb-4">
            <CardHeader>
                <CardTitle>@card.Title</CardTitle>
            </CardHeader>
            <CardContent>
                @card.Content
            </CardContent>
        </Card>
    </Motion>
}
```

### Staggered Grid

```razor
<Motion Trigger="@MotionTrigger.OnAppear" StaggerChildren="0.1">
    <div class="grid grid-cols-3 gap-4">
        @foreach (var item in GridItems)
        {
            <Motion>
                <Presets>
                    <GridItemEnter Duration="0.5" />
                    <Spring Bounce="0.3" />
                </Presets>
                <Card>@item</Card>
            </Motion>
        }
    </div>
</Motion>
```

### Error Shake

```razor
<Motion @ref="_errorMotion" Trigger="@MotionTrigger.Manual">
    <Presets><ShakeX Intensity="10" /></Presets>
    
    <Input @bind-Value="@Username" 
           IsInvalid="@_isInvalid" />
</Motion>

@code {
    private Motion _errorMotion = default!;
    private string Username = "";
    private bool _isInvalid;

    private async Task ValidateUsername()
    {
        if (string.IsNullOrEmpty(Username))
        {
            _isInvalid = true;
            await _errorMotion.PlayAsync();
        }
    }
}
```

## API Reference

### Motion Component

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Trigger` | `MotionTrigger` | `OnAppear` | When animation triggers |
| `Presets` | `RenderFragment` | - | Preset animations to apply |
| `ChildContent` | `RenderFragment` | - | Content to animate |
| `Keyframes` | `List<MotionKeyframe>` | - | Custom keyframes |
| `Options` | `MotionOptions` | - | Animation options |
| `Spring` | `SpringOptions` | - | Spring physics |
| `InViewOptions` | `InViewOptions` | - | IntersectionObserver config |
| `StaggerChildren` | `double?` | - | Stagger delay in seconds |
| `RespectReducedMotion` | `bool` | `true` | Honor reduced motion preference |
| `Class` | `string` | - | CSS classes |
| `Style` | `string` | - | Inline styles |

### MotionKeyframe

All properties are optional:

- `Opacity` (double?) - 0 to 1
- `Scale` (double?) - Scale transformation
- `X`, `Y`, `Z` (string?) - Translation (px, %, etc.)
- `Rotate`, `RotateX`, `RotateY` (double?) - Rotation in degrees
- `ScaleX`, `ScaleY` (double?) - Axis-specific scale
- `SkewX`, `SkewY` (double?) - Skew in degrees
- `Filter` (string?) - CSS filter (e.g., "blur(5px)")
- `BackgroundColor`, `Color` (string?) - Colors
- `BorderRadius` (string?) - Border radius
- `Width`, `Height` (string?) - Dimensions

### MotionOptions

- `Duration` (double?) - Animation duration in seconds
- `Delay` (double?) - Delay before start in seconds
- `Easing` (MotionEasing?) - Easing function
- `Repeat` (double?) - Repeat count (-1 for infinite)
- `RepeatReverse` (bool?) - Reverse on repeat
- `RepeatDelay` (double?) - Delay between repeats
- `Fill` (string?) - Fill mode
- `Direction` (string?) - Animation direction

### SpringOptions

- `Mass` (double) - Default: 1.0
- `Stiffness` (double) - Default: 100
- `Damping` (double) - Default: 10
- `Velocity` (double) - Default: 0
- `Bounce` (double?) - Alternative to stiffness/damping
- `Duration` (double?) - Constrain spring to duration

### InViewOptions

- `Threshold` (double) - Default: 0.1 (0-1)
- `RootMargin` (string?) - IntersectionObserver margin
- `Once` (bool) - Default: true
- `Offset` (string?) - Viewport offset

## Browser Support

Motion.dev supports all modern browsers:
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Performance Tips

1. **Prefer transforms** - Use `scale`, `x`, `y`, `rotate` instead of width/height
2. **Use `Once: true`** - For scroll animations that should trigger once
3. **Limit simultaneous animations** - Don't animate too many elements at once
4. **Use springs sparingly** - Spring physics are more expensive than standard easing

## Troubleshooting

### Animations not working?

1. Ensure Motion.dev script is loaded:
   ```html
   <script src="https://cdn.jsdelivr.net/npm/motion@10.16.2/dist/motion.js"></script>
   ```

2. Check browser console for errors

3. Verify component has `@ref` if using Manual trigger

### Scroll animations not triggering?

1. Ensure `Trigger="@MotionTrigger.OnInView"`
2. Check `InViewOptions.Threshold` - lower values trigger earlier
3. Verify element is actually in/out of viewport

## License

This component is part of BlazorUI and is licensed under the MIT License.

Motion.dev is licensed separately - see [motion.dev](https://motion.dev) for details.
