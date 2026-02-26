namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class NativeSelectDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _nativeSelectProps =
            [
                new("TValue",      "generic",          "-",       "The value type (string, int, etc.)."),
                new("Value",       "@bind-Value / TValue?", null, "The selected value."),
                new("Placeholder", "string?",           null,     "Placeholder text when no value is selected."),
                new("Size",        "NativeSelectSize",  "Default", "Small, Default, or Large."),
                new("Disabled",    "bool",              "false",   "Disables the select."),
            ];

        private const string _basicCode =
                """
                <NativeSelect TValue="string" Placeholder="Select a fruit">
                    <option value="apple">Apple</option>
                    <option value="banana">Banana</option>
                    <option value="orange">Orange</option>
                    <option value="grape">Grape</option>
                </NativeSelect>
                """;

        private const string _placeholderCode =
                """
                <NativeSelect TValue="string" Placeholder="Choose your country">
                    <option value="us">United States</option>
                    <option value="uk">United Kingdom</option>
                    <option value="ca">Canada</option>
                    <option value="au">Australia</option>
                    <option value="de">Germany</option>
                    <option value="fr">France</option>
                </NativeSelect>
                """;

        private const string _sizesCode =
                """
                <NativeSelect TValue="string" Size="NativeSelectSize.Small" Placeholder="Small">
                    <option value="1">Option 1</option>
                </NativeSelect>
                <NativeSelect TValue="string" Size="NativeSelectSize.Default" Placeholder="Default">
                    <option value="1">Option 1</option>
                </NativeSelect>
                <NativeSelect TValue="string" Size="NativeSelectSize.Large" Placeholder="Large">
                    <option value="1">Option 1</option>
                </NativeSelect>
                """;

        private const string _disabledCode =
                """
                <NativeSelect TValue="string" Disabled="true" Placeholder="Disabled select">
                    <option value="1">Option 1</option>
                </NativeSelect>
                """;

        private const string _groupsCode =
                """
                <NativeSelect TValue="string" Placeholder="Select a car">
                    <optgroup label="Swedish Cars">
                        <option value="volvo">Volvo</option>
                        <option value="saab">Saab</option>
                    </optgroup>
                    <optgroup label="German Cars">
                        <option value="mercedes">Mercedes</option>
                        <option value="audi">Audi</option>
                    </optgroup>
                </NativeSelect>
                """;

        private const string _numericCode =
                """
                <NativeSelect TValue="int" Placeholder="Select a number">
                    <option value="1">One</option>
                    <option value="2">Two</option>
                    <option value="3">Three</option>
                </NativeSelect>
                """;
    }
}
