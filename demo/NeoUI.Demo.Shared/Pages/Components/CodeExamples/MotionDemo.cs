namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class MotionDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _motionProps =
            [
                new("Trigger",            "MotionTrigger",   "OnAppear", "Animation trigger: OnAppear, OnInView, or Manual."),
                new("DisableOnPrerender", "bool",            "false",    "Prevents animation if content was server pre-rendered (SSR)."),
                new("StaggerChildren",    "double?",         "null",     "Delay in seconds between each child Motion's animation."),
                new("InViewOptions",      "InViewOptions?",  "null",     "Options for OnInView trigger (Threshold, RootMargin)."),
                new("Presets",            "RenderFragment?", "null",     "Preset components: FadeIn, ScaleIn, SlideInFromLeft, Spring, etc."),
                new("ChildContent",       "RenderFragment?", "null",     "The content to animate."),
            ];

        private const string _fadeCode = """
                <Motion Trigger="@MotionTrigger.OnAppear" DisableOnPrerender="true">
                    <Presets>
                        <FadeIn Duration="0.85" />
                    </Presets>
                    <ChildContent>
                        <Card>
                            <CardHeader><CardTitle>Animated Card</CardTitle></CardHeader>
                            <CardContent><p>Fades in on page load.</p></CardContent>
                        </Card>
                    </ChildContent>
                </Motion>
                """;

        private const string _scaleCode = """
                <Motion @ref="_scaleSpringMotion" Trigger="@MotionTrigger.Manual">
                    <Presets>
                        <ScaleIn From="0.5" />
                        <Spring Stiffness="200" Damping="15" />
                    </Presets>
                    <ChildContent>
                        <Card>...</Card>
                    </ChildContent>
                </Motion>
                """;

        private const string _slideCode = """
                <Motion @ref="_slideLeftMotion" Trigger="@MotionTrigger.Manual">
                    <Presets>
                        <SlideInFromLeft Duration="0.75" Easing="MotionEasing.CubicInOut" />
                    </Presets>
                    <ChildContent>
                        <Alert>...</Alert>
                    </ChildContent>
                </Motion>

                <Motion @ref="_slideBottomMotion" Trigger="@MotionTrigger.Manual">
                    <Presets>
                        <SlideInFromBottom Duration="0.75" Easing="MotionEasing.CubicOut" />
                    </Presets>
                    <ChildContent>
                        <Alert>...</Alert>
                    </ChildContent>
                </Motion>
                """;

        private const string _combinedCode = """
                <Motion @ref="_combinedMotion" Trigger="@MotionTrigger.Manual">
                    <Presets>
                        <FadeIn Duration="0.5" />
                        <ScaleIn From="0.6" Duration="0.5" />
                        <SlideInFromBottom From="30" Duration="0.5" />
                    </Presets>
                    <ChildContent>
                        <Card>...</Card>
                    </ChildContent>
                </Motion>
                """;

        private const string _microCode = """
                <Motion @ref="_shakeMotion" Trigger="@MotionTrigger.Manual">
                    <Presets><ShakeX /></Presets>
                    <ChildContent>
                        <Button OnClick="@(() => _shakeMotion?.PlayAsync())">Shake Me</Button>
                    </ChildContent>
                </Motion>

                <Motion @ref="_bounceMotion" Trigger="@MotionTrigger.Manual">
                    <Presets><BounceOnce /></Presets>
                    <ChildContent>
                        <Button OnClick="@(() => _bounceMotion?.PlayAsync())">Bounce Me</Button>
                    </ChildContent>
                </Motion>
                """;

        private const string _pulseCode = """
                <Motion @ref="_pulseMotion" Trigger="@MotionTrigger.Manual">
                    <Presets>
                        <Pulse Scale="1.05" Duration="1.0" />
                    </Presets>
                    <ChildContent>
                        <Badge Variant="BadgeVariant.Destructive">🔴 Live</Badge>
                    </ChildContent>
                </Motion>
                """;

        private const string _scrollCode = """
                <Motion Trigger="@MotionTrigger.OnInView"
                        InViewOptions="@(new InViewOptions { Threshold = 0.3 })">
                    <Presets>
                        <FadeInOnScroll />
                        <SlideInOnScroll From="50" />
                    </Presets>
                    <ChildContent>
                        <Card>
                            <CardHeader><CardTitle>Card</CardTitle></CardHeader>
                            <CardContent><p>Animates when scrolled into view.</p></CardContent>
                        </Card>
                    </ChildContent>
                </Motion>
                """;

        private const string _staggerListCode = """
                <Motion @ref="_staggerListMotion" Trigger="@MotionTrigger.Manual" StaggerChildren="0.1">
                    @foreach (var item in _listItems)
                    {
                        <Motion>
                            <Presets><ListItemEnter /></Presets>
                            <ChildContent>
                                <Card class="mb-3">
                                    <CardContent class="pt-6"><p>@item</p></CardContent>
                                </Card>
                            </ChildContent>
                        </Motion>
                    }
                </Motion>
                """;

        private const string _staggerGridCode = """
                <Motion @ref="_staggerGridMotion" Trigger="@MotionTrigger.Manual" StaggerChildren="0.15">
                    <div class="grid grid-cols-3 gap-4">
                        @for (int i = 1; i <= 9; i++)
                        {
                            var index = i;
                            <Motion>
                                <Presets><GridItemEnter Duration="0.4" /></Presets>
                                <ChildContent>
                                    <Card class="h-24 flex items-center justify-center">
                                        <CardContent class="pt-6">
                                            <span class="text-2xl font-bold">@index</span>
                                        </CardContent>
                                    </Card>
                                </ChildContent>
                            </Motion>
                        }
                    </div>
                </Motion>
                """;

        private const string _easingCode = """
                <Motion @ref="_cubicOutMotion" Trigger="@MotionTrigger.Manual">
                    <Presets>
                        <SlideInFromLeft Duration="0.6" Easing="MotionEasing.CubicOut" />
                    </Presets>
                    <ChildContent>
                        <Card><CardContent class="text-center">CubicOut</CardContent></Card>
                    </ChildContent>
                </Motion>
                """;
    }
}
