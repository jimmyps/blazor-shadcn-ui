namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ColorPickerDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _colorPickerProps =
            [
                new("Color",              "string?",          null,    "The current color value in the selected format."),
                new("Format",             "ColorFormat",      "Hex",   "Output color format. Options: Hex, RGB, HSL."),
                new("Size",               "ColorPickerSize",  "Compact","Picker size. Options: Compact, Large."),
                new("ShowInputs",         "bool",             "true",  "When false, hides the RGB/Hex text inputs."),
                new("ShowPresets",        "bool",             "true",  "When false, hides the color preset swatches."),
                new("ShowAlpha",          "bool",             "false", "When true, shows an opacity/alpha slider."),
                new("Disabled",           "bool",             "false", "When true, the picker is non-interactive."),
                new("Required",           "bool",             "false", "When true, marks the field as required for validation."),
                new("ShowValidationError","bool",             "false", "When true, shows validation error messages."),
                new("Class",              "string?",          null,    "Additional CSS classes appended to the root element."),
            ];

        private const string _defaultCode =
                """
                <ColorPicker @bind-Color="color" />
                """;

        private const string _largeCode =
                """
                <ColorPicker @bind-Color="color" Size="ColorPickerSize.Large" />
                """;

        private const string _minimalCode =
                """
                <ColorPicker @bind-Color="color" ShowInputs="false" ShowPresets="false" />
                """;

        private const string _rgbCode =
                """
                <ColorPicker @bind-Color="color" Format="ColorFormat.RGB" />
                """;

        private const string _hslCode =
                """
                <ColorPicker @bind-Color="color" Format="ColorFormat.HSL" />
                """;

        private const string _alphaCode =
                """
                <ColorPicker @bind-Color="color" ShowAlpha="true" />
                """;

        private const string _disabledCode =
                """
                <ColorPicker @bind-Color="color" Disabled="true" />
                """;

        private const string _presetsOnlyCode =
                """
                <ColorPicker @bind-Color="color" ShowInputs="false" />
                """;

        private const string _rgbaCode =
                """
                <ColorPicker @bind-Color="color" Format="ColorFormat.RGB" ShowAlpha="true" />
                """;

        private const string _comparisonCode =
                """
                <ColorPicker @bind-Color="hexColor" />
                <ColorPicker @bind-Color="rgbColor" Format="ColorFormat.RGB" />
                <ColorPicker @bind-Color="hslColor" Format="ColorFormat.HSL" />
                """;

        private const string _formCode =
                """
                <EditForm Model="@formModel" OnValidSubmit="HandleValidSubmit">
                    <DataAnnotationsValidator />
                    <Field>
                        <Label For="color-input">Favorite Color</Label>
                        <ColorPicker @bind-Color="formModel.FavoriteColor"
                                     Id="color-input"
                                     Required="true"
                                     ShowValidationError="true"
                                     ValueExpression="@(() => formModel.FavoriteColor)" />
                    </Field>
                    <button type="submit">Submit</button>
                </EditForm>
                """;
    }
}
