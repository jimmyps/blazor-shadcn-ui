namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class CarouselDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _carouselProps =
            [
                new("ShowIndicators",   "bool",                 "false",      "Show dot indicators below the carousel."),
                new("ShowNavigation",   "bool",                 "true",       "Show previous/next arrow buttons."),
                new("AutoPlay",         "bool",                 "false",      "Automatically advance slides."),
                new("AutoPlayInterval", "int",                  "3000",       "Interval in ms between auto-play advances."),
                new("Loop",             "bool",                 "false",      "Whether to loop back to the first slide after the last."),
                new("SlidesPerView",    "int",                  "1",          "Number of slides visible at once."),
                new("Gap",              "int",                  "0",          "Gap in pixels between slides."),
                new("Orientation",      "CarouselOrientation",  "Horizontal", "Scroll direction. Options: Horizontal, Vertical."),
                new("EnableDrag",       "bool",                 "false",      "Allow click-drag to navigate slides."),
                new("OnSlideChange",    "EventCallback<int>",   null,         "Fired when the active slide changes. Receives the zero-based index."),
                new("ContentClass",     "string?",              null,         "Additional CSS classes for the slides container."),
            ];

        private const string _defaultCode =
                """
                <Carousel>
                    <CarouselItem>
                        <Card>
                            <CardContent Class="flex h-[150px] items-center justify-center p-6">
                                <span class="text-4xl font-semibold">1</span>
                            </CardContent>
                        </Card>
                    </CarouselItem>
                    <!-- more items... -->
                </Carousel>
                """;

        private const string _withIndicatorsCode =
                """
                <Carousel ShowIndicators="true">
                    <!-- CarouselItems... -->
                </Carousel>
                """;

        private const string _autoPlayCode =
                """
                <Carousel AutoPlay="true" AutoPlayInterval="3000" Loop="true" ShowIndicators="true">
                    <!-- CarouselItems... -->
                </Carousel>
                """;

        private const string _loopCode =
                """
                <Carousel Loop="true">
                    <!-- CarouselItems... -->
                </Carousel>
                """;

        private const string _multipleCode =
                """
                <Carousel SlidesPerView="3" Gap="2">
                    <CarouselItem Class="p-1 basis-[33%]">
                        <Card>...</Card>
                    </CarouselItem>
                    <!-- more items... -->
                </Carousel>
                """;

        private const string _verticalCode =
                """
                <Carousel Orientation="CarouselOrientation.Vertical" ShowIndicators="true">
                    <!-- CarouselItems... -->
                </Carousel>
                """;

        private const string _galleryCode =
                """
                <Carousel ShowIndicators="true" OnSlideChange="HandleSlideChange">
                    @foreach (var image in galleryImages)
                    {
                        <CarouselItem>
                            <Card>
                                <CardContent Class="flex flex-col items-center justify-center p-6">
                                    <span class="text-6xl">@image.Icon</span>
                                    <h3 class="text-xl font-semibold">@image.Title</h3>
                                </CardContent>
                            </Card>
                        </CarouselItem>
                    }
                </Carousel>
                """;

        private const string _noNavCode =
                """
                <Carousel ShowNavigation="false" ShowIndicators="true" EnableDrag="true">
                    <!-- CarouselItems... -->
                </Carousel>
                """;
    }
}
