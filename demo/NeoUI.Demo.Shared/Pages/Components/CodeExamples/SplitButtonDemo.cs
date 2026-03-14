namespace NeoUI.Demo.Shared.Pages.Components;

partial class SplitButtonDemo
{
    private static readonly IReadOnlyList<DemoPropRow> _props =
    [
        new("ChildContent",    "RenderFragment?",               "null",    "Primary button label content."),
        new("DropdownContent", "RenderFragment?",               "null",    "Dropdown items (SplitButtonItem, SplitButtonSeparator)."),
        new("Icon",            "RenderFragment?",               "null",    "Icon rendered inside the primary button."),
        new("OnClick",         "EventCallback<MouseEventArgs>", "",        "Fires when the primary button is clicked."),
        new("Variant",         "ButtonVariant",                 "Default", "Visual style applied to both segments."),
        new("Size",            "ButtonSize",                    "Default", "Size applied to both segments."),
        new("Disabled",        "bool",                          "false",   "Disables both segments."),
        new("IconPosition",    "IconPosition",                  "Start",   "Whether the Icon renders before or after the label."),
    ];

    private static class SplitButtonDemoCode
    {
        public const string Basic =
            """
            <SplitButton OnClick="HandleSave">
                <ChildContent>Save</ChildContent>
                <DropdownContent>
                    <SplitButtonItem OnClick="HandleSaveAsDraft">Save as Draft</SplitButtonItem>
                    <SplitButtonItem OnClick="HandleSaveAndClose">Save & Close</SplitButtonItem>
                    <SplitButtonSeparator />
                    <SplitButtonItem OnClick="HandleDiscard">Discard Changes</SplitButtonItem>
                </DropdownContent>
            </SplitButton>
            """;

        public const string Variants =
            """
            <SplitButton>
                <ChildContent>Default</ChildContent>
                <DropdownContent><SplitButtonItem>Option A</SplitButtonItem></DropdownContent>
            </SplitButton>
            <SplitButton Variant="ButtonVariant.Secondary">
                <ChildContent>Secondary</ChildContent>
                <DropdownContent><SplitButtonItem>Option A</SplitButtonItem></DropdownContent>
            </SplitButton>
            <SplitButton Variant="ButtonVariant.Outline">
                <ChildContent>Outline</ChildContent>
                <DropdownContent><SplitButtonItem>Option A</SplitButtonItem></DropdownContent>
            </SplitButton>
            """;

        public const string Sizes =
            """
            <SplitButton Size="ButtonSize.Small">
                <ChildContent>Save</ChildContent>
                <DropdownContent>
                    <SplitButtonItem>Save as Draft</SplitButtonItem>
                    <SplitButtonItem>Save &amp; Close</SplitButtonItem>
                </DropdownContent>
            </SplitButton>
            <SplitButton Size="ButtonSize.Default">
                <ChildContent>Save</ChildContent>
                <DropdownContent>
                    <SplitButtonItem>Save as Draft</SplitButtonItem>
                    <SplitButtonItem>Save &amp; Close</SplitButtonItem>
                </DropdownContent>
            </SplitButton>
            <SplitButton Size="ButtonSize.Large">
                <ChildContent>Save</ChildContent>
                <DropdownContent>
                    <SplitButtonItem>Save as Draft</SplitButtonItem>
                    <SplitButtonItem>Save &amp; Close</SplitButtonItem>
                </DropdownContent>
            </SplitButton>
            """;

        public const string WithIcon =
            """
            <SplitButton OnClick="Deploy">
                <Icon><LucideIcon Name="rocket" Size="15" /></Icon>
                <ChildContent>Deploy to Production</ChildContent>
                <DropdownContent>
                    <SplitButtonItem OnClick="DeployToStaging">Deploy to Staging</SplitButtonItem>
                    <SplitButtonItem OnClick="DeployToPreview">Deploy to Preview</SplitButtonItem>
                    <SplitButtonSeparator />
                    <SplitButtonItem OnClick="Rollback">Rollback Last</SplitButtonItem>
                </DropdownContent>
            </SplitButton>
            """;

        public const string Disabled =
            """
            <SplitButton Disabled="true">
                <ChildContent>Unavailable</ChildContent>
                <DropdownContent><SplitButtonItem>Option A</SplitButtonItem></DropdownContent>
            </SplitButton>
            """;
    }
}
