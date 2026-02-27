namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class TypographyDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _typographyProps =
            [
                new("Variant",      "TypographyVariant", "P",   "Visual style. Options: H1, H2, H3, H4, P, Lead, Large, Small, Muted, Blockquote, InlineCode."),
                new("ChildContent", "RenderFragment?",   null,  "The text content to display."),
            ];

        private const string _headingsCode =
                """
                <Typography Variant="TypographyVariant.H1">Heading 1</Typography>
                <Typography Variant="TypographyVariant.H2">Heading 2</Typography>
                <Typography Variant="TypographyVariant.H3">Heading 3</Typography>
                <Typography Variant="TypographyVariant.H4">Heading 4</Typography>
                """;

        private const string _bodyCode =
                """
                <Typography Variant="TypographyVariant.P">Paragraph text.</Typography>
                <Typography Variant="TypographyVariant.Lead">Lead text.</Typography>
                <Typography Variant="TypographyVariant.Large">Large text.</Typography>
                <Typography Variant="TypographyVariant.Small">Small text.</Typography>
                <Typography Variant="TypographyVariant.Muted">Muted text.</Typography>
                """;

        private const string _specialCode =
                """
                <Typography Variant="TypographyVariant.Blockquote">
                    "The only way to do great work is to love what you do." - Steve Jobs
                </Typography>

                <Typography Variant="TypographyVariant.P">
                    Inline code example: <Typography Variant="TypographyVariant.InlineCode">npm install neoui</Typography>
                </Typography>
                """;
    }
}
