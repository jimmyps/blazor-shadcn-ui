# Motion Component

A **declarative, strongly-typed animation system** for Blazor that wraps the [Motion.dev](https://motion.dev) JavaScript library.

## Features

- 🎯 **Declarative API** - Define animations directly in Razor markup
- 💪 **Strongly Typed** - Full IntelliSense support for all animation properties
- 🎨 **Composable Presets** - Combine multiple animation effects
- 🎭 **27 Easing Functions** - Comprehensive easing options from linear to spring physics
- ⚡ **Auto-Loading** - Motion.dev library loads automatically from CDN (no manual setup required)
- 🔧 **Spring Physics** - Natural, physics-based animations
- ♿ **Accessible** - Respects user's reduced motion preferences
- 📱 **Responsive** - Works across all Blazor hosting models
- 🔧 **Flexible** - Use presets or define custom keyframes

## Installation

### Import Namespace

Add to your `_Imports.razor`:

```razor
@using BlazorUI.Components.Motion
```

That's it! The Motion.dev library loads automatically on first use.

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

### Custom Easing

All presets support custom easing functions:

```razor
<Motion Trigger="@MotionTrigger.OnAppear">
    <Presets>
        <SlideInFromLeft Easing="MotionEasing.BackOut" />
    </Presets>
    <div class="card">Slides in with overshoot effect!</div>
</Motion>
```

## Easing Functions

Choose from **27 built-in easing functions**:

**Basic:**
- `Linear` - Constant speed

**Quadratic:**
- `QuadraticIn`, `QuadraticOut`, `QuadraticInOut` - Gentle acceleration curves

**Cubic:**
- `CubicIn`, `CubicOut`, `CubicInOut` - Smooth acceleration

**Quartic:**
- `QuarticIn`, `QuarticOut`, `QuarticInOut` - Strong acceleration

**Quintic:**
- `QuinticIn`, `QuinticOut`, `QuinticInOut` - Very strong acceleration

**Sinusoidal:**
- `SinusoidalIn`, `SinusoidalOut`, `SinusoidalInOut` - Smooth sine wave motion

**Exponential:**
- `ExponentialIn`, `ExponentialOut`, `ExponentialInOut` - Dramatic acceleration

**Circular:**
- `CircularIn`, `CircularOut`, `CircularInOut` - Smooth arc motion

**Back (Overshoot):**
- `BackIn`, `BackOut`, `BackInOut` - ⭐ **Popular!** Overshoots target then settles

**Elastic:**
- `ElasticIn`, `ElasticOut`, `ElasticInOut` - Spring-like oscillation

**Bounce:**
- `BounceIn`, `BounceOut`, `BounceInOut` - Bouncing ball effect

**Legacy Aliases:**
- `EaseIn` (QuadraticIn), `EaseOut` (QuadraticOut), `EaseInOut` (QuadraticInOut)

**Custom:**
- `Custom` - Define your own cubic-bezier curve

### Example: Overshoot Effect

```razor
<Motion Trigger="@MotionTrigger.Manual" @ref="_motion">
    <Presets>
        <ScaleIn From="0.8" Easing="MotionEasing.BackOut" />
    </Presets>
    <Button OnClick="@(() => _motion?.PlayAsync())">
        Click me - I overshoot!
    </Button>
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

<Button OnClick="@HandleShake">Shake It!</Button>

@code {
    private Motion? _motion;

    private async Task HandleShake()
    {
        if (_motion != null)
        {
            await _motion.PlayAsync();
        }
    }
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

All entry presets support custom easing via `Easing` parameter!

### Micro-Interactions

- `<Pulse />` - Pulsing scale animation
- `<BounceOnce />` - Single bounce effect
- `<ShakeX />` - Horizontal shake (great for errors)

### List & Grid Animations

- `<ListItemEnter />` - Polished list item entry
- `<GridItemEnter />` - Grid item entry with scale

### Scroll-Based Animations

- `<FadeInOnScroll />` - Fade in on viewport entry
- `<SlideInOnScroll />` - Slide up on viewport entry

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
        Easing = MotionEasing.BackOut  // Try the overshoot effect!
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

### Dialog Entry Animation with Overshoot

```razor
<Dialog>
    <DialogTrigger AsChild>
        <Button>Open Dialog</Button>
    </DialogTrigger>
    
    <Motion Trigger="@MotionTrigger.OnAppear">
        <Presets>
            <FadeIn />
            <ScaleIn From="0.8" Easing="MotionEasing.BackOut" />
        </Presets>
        <DialogContent>
            <DialogTitle>Animated Dialog</DialogTitle>
            <DialogDescription>
                This dialog bounces in with an overshoot effect!
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
            <SlideInOnScroll From="100px" Easing="MotionEasing.CircularOut" />
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

### Staggered Grid with Different Easings

```razor
<Motion Trigger="@MotionTrigger.OnAppear" StaggerChildren="0.1">
    <div class="grid grid-cols-3 gap-4">
        @foreach (var item in GridItems)
        {
            <Motion>
                <Presets>
                    <GridItemEnter Duration="0.5" Easing="MotionEasing.BackOut" />
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

### Preset Parameters

All animation presets support the following parameters:

| Parameter | Type | Description |
|-----------|------|-------------|
| `Easing` | `MotionEasing?` | Easing function (overrides default) |
| `CustomEasing` | `double[]?` | Custom cubic-bezier [x1, y1, x2, y2] |
| `Duration` | `double` | Animation duration in seconds |

Individual presets may have additional parameters like `From`, `To`, `Intensity`, etc.

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
- `Easing` (MotionEasing?) - Easing function (27 options!)
- `CustomEasing` (double[]?) - Custom cubic-bezier curve
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

The Motion.dev library is automatically loaded from CDN on first use.

## Performance Tips

1. **Prefer transforms** - Use `scale`, `x`, `y`, `rotate` instead of width/height
2. **Use `Once: true`** - For scroll animations that should trigger once
3. **Limit simultaneous animations** - Don't animate too many elements at once
4. **Use springs sparingly** - Spring physics are more expensive than standard easing
5. **Try BackOut for UI elements** - Creates a satisfying "bounce-in" effect with minimal performance cost

## Easing Guide

### Most Popular Easings

- **`BackOut`** - ⭐ Best for buttons, dialogs, modals (overshoots slightly)
- **`CircularOut`** - Smooth deceleration, natural feeling
- **`CubicOut`** - All-purpose smooth easing
- **`ExponentialOut`** - Dramatic slow-down effect

### When to Use Each

- **Linear** - Progress bars, loading indicators
- **QuadraticOut / CubicOut** - General UI animations
- **BackOut** - Attention-grabbing elements (buttons, dialogs)
- **CircularOut** - Smooth, natural transitions
- **ExponentialOut** - Dramatic reveals
- **Spring** - Natural physics-based motion

## Troubleshooting

### Animations not working?

1. **Check browser console** - The Motion.dev library loads automatically, check for network errors
2. **Verify component has `@ref`** - Required when using `Trigger="@MotionTrigger.Manual"`
3. **Test in a modern browser** - Motion.dev requires Chrome 90+, Firefox 88+, Safari 14+, or Edge 90+

### Scroll animations not triggering?

1. **Ensure `Trigger="@MotionTrigger.OnInView"`**
2. **Lower the threshold** - Try `Threshold = 0.1` for earlier triggering
3. **Verify element scrolls into view** - Must actually enter the viewport
4. **Check `Once` setting** - Default is `true` (triggers only once)

### Easing doesn't look right?

Motion.dev maps our 27 easing functions to its built-in easing names. Some complex easings (like Elastic and Bounce) are approximated. For the most precise control, use the `Spring` preset with custom physics parameters.

## What's New

### Latest Features

✨ **Auto-Loading** - Motion.dev library now loads automatically from CDN
✨ **27 Easing Functions** - Comprehensive easing options including popular BackOut
✨ **Easing Parameters** - All presets support `Easing` and `CustomEasing` parameters
✨ **Smart Mapping** - Easing functions automatically mapped to Motion.dev's API

## License

This component is part of BlazorUI and is licensed under the MIT License.

Motion.dev is licensed separately - see [motion.dev](https://motion.dev) for details.
