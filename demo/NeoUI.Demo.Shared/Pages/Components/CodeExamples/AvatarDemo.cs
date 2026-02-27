namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class AvatarDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _avatarProps =
            [
                new("Size",  "AvatarSize", "Default", "Controls dimensions: Small (32px), Default (40px), Large (48px), ExtraLarge (64px)."),
                new("Class", "string?",    null,      "Additional CSS classes for the avatar container."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _avatarImageProps =
            [
                new("Source", "string?", null, "URL of the image. Falls back to AvatarFallback on load error."),
                new("Alt",    "string?", null, "Alternative text for the image (required for accessibility)."),
                new("Class",  "string?", null, "Additional CSS classes for the image element."),
            ];

        private static readonly IReadOnlyList<DemoPropRow> _avatarFallbackProps =
            [
                new("Class", "string?", null, "Additional CSS classes. Useful for custom background colors."),
            ];

        private const string _basicCode =
                """
                <Avatar>
                    <AvatarImage Source="_content/NeoUI.Demo.Shared/images/avatar-icon.png" Alt="James" />
                    <AvatarFallback>SH</AvatarFallback>
                </Avatar>

                <Avatar>
                    <AvatarImage Source="https://unavailable-image.jpg" Alt="User" />
                    <AvatarFallback>JD</AvatarFallback>
                </Avatar>
                """;

        private const string _sizesCode =
                """
                <Avatar Size="AvatarSize.Small">
                    <AvatarFallback>XS</AvatarFallback>
                </Avatar>

                <Avatar Size="AvatarSize.Default">
                    <AvatarFallback>MD</AvatarFallback>
                </Avatar>

                <Avatar Size="AvatarSize.Large">
                    <AvatarFallback>LG</AvatarFallback>
                </Avatar>

                <Avatar Size="AvatarSize.ExtraLarge">
                    <AvatarFallback>XL</AvatarFallback>
                </Avatar>
                """;

        private const string _imagesCode =
                """
                <Avatar Size="AvatarSize.Large">
                    <AvatarImage Source="https://github.com/vercel.png" Alt="Vercel" />
                    <AvatarFallback>VC</AvatarFallback>
                </Avatar>

                <Avatar Size="AvatarSize.Large">
                    <AvatarImage Source="https://github.com/microsoft.png" Alt="Microsoft" />
                    <AvatarFallback>MS</AvatarFallback>
                </Avatar>
                """;

        private const string _iconsCode =
                """
                <Avatar>
                    <AvatarFallback>
                        <LucideIcon Name="user" Size="20" />
                    </AvatarFallback>
                </Avatar>

                <Avatar Size="AvatarSize.Large">
                    <AvatarFallback>
                        <LucideIcon Name="circle-user" Size="24" />
                    </AvatarFallback>
                </Avatar>
                """;

        private const string _userListCode =
                """
                <div class="border rounded-lg divide-y">
                    <div class="flex items-center gap-4 p-4">
                        <Avatar>
                            <AvatarFallback>JD</AvatarFallback>
                        </Avatar>
                        <div>
                            <p class="font-medium">John Doe</p>
                            <p class="text-sm text-muted-foreground">john.doe@example.com</p>
                        </div>
                    </div>
                </div>
                """;

        private const string _groupCode =
                """
                <div class="flex -space-x-4">
                    <Avatar Class="border-2 border-background">
                        <AvatarFallback>AB</AvatarFallback>
                    </Avatar>
                    <Avatar Class="border-2 border-background">
                        <AvatarFallback>CD</AvatarFallback>
                    </Avatar>
                    <Avatar Class="border-2 border-background">
                        <AvatarFallback Class="bg-primary text-primary-foreground">+5</AvatarFallback>
                    </Avatar>
                </div>
                """;

        private const string _customCode =
                """
                <Avatar Size="AvatarSize.Large">
                    <AvatarFallback Class="bg-gradient-to-br from-purple-500 to-pink-500 text-white">AB</AvatarFallback>
                </Avatar>

                <Avatar Size="AvatarSize.Large">
                    <AvatarFallback Class="bg-gradient-to-br from-blue-500 to-cyan-500 text-white">CD</AvatarFallback>
                </Avatar>
                """;

        private const string _statusCode =
                """
                <div class="relative inline-block">
                    <Avatar Size="AvatarSize.Large">
                        <AvatarFallback>JD</AvatarFallback>
                    </Avatar>
                    <span class="absolute bottom-0 right-0 block h-3 w-3 rounded-full bg-green-500 ring-2 ring-background"></span>
                </div>
                """;
    }
}
