namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class AspectRatioDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _aspectRatioProps =
            [
                new("Ratio",        "double",          "1",   "Width/height ratio as a decimal (e.g. 16.0/9.0)."),
                new("Class",        "string?",         null,  "Additional CSS classes applied to the container."),
                new("ChildContent", "RenderFragment?", null,  "Content to display inside the container."),
            ];

        private const string _videoCode =
                """
                <AspectRatio Ratio="16.0/9.0" Class="bg-muted rounded-md overflow-hidden">
                    <img src="image.jpg" alt="..." class="w-full h-full object-cover" />
                </AspectRatio>
                """;

        private const string _squareCode =
                """
                <AspectRatio Ratio="1" Class="bg-muted rounded-full overflow-hidden">
                    <img src="avatar.jpg" alt="Avatar" class="w-full h-full object-cover" />
                </AspectRatio>
                """;

        private const string _classicCode =
                """
                <AspectRatio Ratio="4.0/3.0" Class="bg-muted rounded-md overflow-hidden">
                    <img src="photo.jpg" alt="Photo" class="w-full h-full object-cover" />
                </AspectRatio>
                """;

        private const string _ultraWideCode =
                """
                <AspectRatio Ratio="21.0/9.0" Class="bg-muted rounded-lg overflow-hidden">
                    <div class="absolute inset-0 flex items-center justify-center bg-gradient-to-r from-primary/20 to-primary/40">
                        <span class="text-2xl font-bold">21:9 Ultra Wide Banner</span>
                    </div>
                </AspectRatio>
                """;

        private const string _placeholderCode =
                """
                <AspectRatio Ratio="1" Class="bg-muted rounded-md">
                    <div class="absolute inset-0 flex items-center justify-center">
                        <span class="text-sm text-muted-foreground">1:1</span>
                    </div>
                </AspectRatio>
                """;
    }
}
