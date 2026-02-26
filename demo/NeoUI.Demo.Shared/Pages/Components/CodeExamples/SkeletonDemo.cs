namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class SkeletonDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _skeletonProps =
            [
                new("Shape", "SkeletonShape", "Rectangular", "Shape variant. Options: Rectangular (rounded-md), Circular (rounded-full)."),
                new("Class", "string?",       null,          "Additional CSS classes. Use Tailwind utilities for width, height, and spacing."),
            ];

        private const string _basicCode =
                """
                <Skeleton Class="h-4 w-[250px]" />
                <Skeleton Class="h-4 w-[200px]" />
                <Skeleton Class="h-4 w-[150px]" />
                """;

        private const string _shapesCode =
                """
                <Skeleton Shape="SkeletonShape.Rectangular" Class="h-12 w-12" />
                <Skeleton Shape="SkeletonShape.Circular" Class="h-12 w-12" />
                """;

        private const string _cardCode =
                """
                <div class="border rounded-lg p-6 space-y-4">
                    <div class="flex items-center space-x-4">
                        <Skeleton Shape="SkeletonShape.Circular" Class="h-12 w-12" />
                        <div class="space-y-2 flex-1">
                            <Skeleton Class="h-4 w-[150px]" />
                            <Skeleton Class="h-4 w-[100px]" />
                        </div>
                    </div>
                    <div class="space-y-2">
                        <Skeleton Class="h-4 w-full" />
                        <Skeleton Class="h-4 w-full" />
                        <Skeleton Class="h-4 w-3/4" />
                    </div>
                </div>
                """;

        private const string _listCode =
                """
                @for (int i = 0; i < 4; i++)
                {
                    <div class="p-4 flex items-center space-x-4">
                        <Skeleton Shape="SkeletonShape.Circular" Class="h-10 w-10" />
                        <div class="space-y-2 flex-1">
                            <Skeleton Class="h-4 w-[200px]" />
                            <Skeleton Class="h-3 w-[150px]" />
                        </div>
                    </div>
                }
                """;

        private const string _avatarsCode =
                """
                <Skeleton Shape="SkeletonShape.Circular" Class="h-8 w-8" />
                <Skeleton Shape="SkeletonShape.Circular" Class="h-12 w-12" />
                <Skeleton Shape="SkeletonShape.Circular" Class="h-16 w-16" />
                <Skeleton Shape="SkeletonShape.Circular" Class="h-24 w-24" />
                """;

        private const string _profileCode =
                """
                <div class="border rounded-lg p-6 space-y-6">
                    <div class="flex items-center space-x-4">
                        <Skeleton Shape="SkeletonShape.Circular" Class="h-16 w-16" />
                        <div class="space-y-2">
                            <Skeleton Class="h-5 w-[120px]" />
                            <Skeleton Class="h-4 w-[100px]" />
                        </div>
                    </div>
                    <div class="space-y-2">
                        <Skeleton Class="h-4 w-full" />
                        <Skeleton Class="h-4 w-2/3" />
                    </div>
                </div>
                """;

        private const string _tableCode =
                """
                @for (int i = 0; i < 5; i++)
                {
                    <div class="px-6 py-4 flex gap-4">
                        <Skeleton Class="h-4 w-[150px]" />
                        <Skeleton Class="h-4 w-[100px]" />
                        <Skeleton Class="h-4 w-[120px]" />
                    </div>
                }
                """;

        private const string _formCode =
                """
                @for (int i = 0; i < 3; i++)
                {
                    <div class="space-y-2">
                        <Skeleton Class="h-4 w-[100px]" />
                        <Skeleton Class="h-10 w-full" />
                    </div>
                }
                <Skeleton Class="h-10 w-24" />
                """;

        private const string _imageGalleryCode =
                """
                <div class="grid grid-cols-4 gap-4">
                    @for (int i = 0; i < 8; i++)
                    {
                        <Skeleton Class="h-[200px] w-full" />
                    }
                </div>
                """;

        private const string _customStylingCode =
                """
                <Skeleton Class="h-32 w-full rounded-xl" />
                <Skeleton Class="h-4 w-full rounded-none" />
                <Skeleton Shape="SkeletonShape.Circular" Class="h-20 w-20" />
                """;
    }
}
