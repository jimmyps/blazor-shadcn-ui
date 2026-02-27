namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class RatingDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _ratingProps =
            [
                new("Value",       "double",              "0",      "The current rating value."),
                new("MaxRating",   "int",                 "5",      "Maximum number of rating icons displayed."),
                new("AllowHalf",   "bool",                "false",  "When true, half-value ratings are allowed."),
                new("AllowClear",  "bool",                "true",   "When true, clicking the current value clears the rating."),
                new("ReadOnly",    "bool",                "false",  "When true, the rating is displayed but not interactive."),
                new("Disabled",    "bool",                "false",  "When true, the rating is non-interactive and visually dimmed."),
                new("IconType",    "RatingIconType",      "Star",   "Icon shape. Options: Star, Heart, Circle."),
                new("Size",        "RatingSize",          "Medium", "Icon size. Options: Small, Medium, Large."),
                new("Class",       "string?",             null,     "Additional CSS classes appended to the root element."),
            ];

        private const string _basicCode =
                """
                <Rating Value="3" />
                """;

        private const string _halfCode =
                """
                <Rating Value="2.5" AllowHalf="true" />
                """;

        private const string _iconTypesCode =
                """
                <Rating Value="3" IconType="RatingIconType.Star" />
                <Rating Value="4" IconType="RatingIconType.Heart" />
                <Rating Value="2" IconType="RatingIconType.Circle" />
                """;

        private const string _sizesCode =
                """
                <Rating Value="3" Size="RatingSize.Small" />
                <Rating Value="3" Size="RatingSize.Medium" />
                <Rating Value="3" Size="RatingSize.Large" />
                """;

        private const string _maxCode =
                """
                <Rating Value="5" MaxRating="10" Size="RatingSize.Small" />
                """;

        private const string _disabledCode =
                """
                <Rating Value="3.5" Disabled="true" AllowHalf="true" />
                """;

        private const string _readOnlyCode =
                """
                <Rating Value="4" ReadOnly="true" />
                """;

        private const string _clearCode =
                """
                <Rating Value="3" AllowClear="true" />
                """;

        private const string _noClearCode =
                """
                <Rating Value="3" AllowClear="false" />
                """;
    }
}
