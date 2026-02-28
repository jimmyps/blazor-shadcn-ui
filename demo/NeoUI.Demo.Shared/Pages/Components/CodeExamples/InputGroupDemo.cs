namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class InputGroupDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _inputGroupProps =
            [
                new("Align",        "InputGroupAlign", "—",    "On InputGroupAddon: position of the addon. Options: InlineStart, InlineEnd, BlockStart, BlockEnd."),
                new("Class",        "string?",         "null", "Additional CSS classes."),
                new("ChildContent", "RenderFragment?", "null", "Content of the component."),
                new("Variant",      "ButtonVariant",   "Default","On InputGroupButton: button visual variant."),
                new("Placeholder",  "string?",         "null", "On InputGroupInput / InputGroupTextarea: placeholder text."),
                new("Rows",         "int",             "3",    "On InputGroupTextarea: number of visible rows."),
            ];

        private const string _basicCode = """
                <!-- Icon at start -->
                <InputGroup>
                    <InputGroupAddon Align="InputGroupAlign.InlineStart">
                        <LucideIcon Name="search" Size="16" />
                    </InputGroupAddon>
                    <InputGroupInput Placeholder="Search..." />
                </InputGroup>

                <!-- Text suffix -->
                <InputGroup>
                    <InputGroupInput Value="johndoe123" Placeholder="Enter username" />
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd" Class="text-sm text-muted-foreground">
                        @company.com
                    </InputGroupAddon>
                </InputGroup>
                """;

        private const string _textAddonsCode = """
                <InputGroup>
                    <InputGroupAddon Align="InputGroupAlign.InlineStart">
                        <InputGroupText>$</InputGroupText>
                    </InputGroupAddon>
                    <InputGroupInput Type="InputType.Number" Placeholder="0.00" />
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                        <InputGroupText>USD</InputGroupText>
                    </InputGroupAddon>
                </InputGroup>
                """;

        private const string _actionButtonsCode = """
                <InputGroup>
                    <InputGroupInput Placeholder="Search products..." />
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                        <InputGroupButton Class="!px-2.5 !py-0.5 !rounded-sm">Search</InputGroupButton>
                    </InputGroupAddon>
                </InputGroup>

                <!-- Icon button -->
                <InputGroup>
                    <InputGroupInput Value="sk-1234567890abcdef" />
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                        <InputGroupButton Variant="ButtonVariant.Ghost" Class="!px-1.5 !py-1.5 !rounded-md">
                            <LucideIcon Name="copy" Size="16" />
                        </InputGroupButton>
                    </InputGroupAddon>
                </InputGroup>
                """;

        private const string _validationCode = """
                <!-- Success -->
                <InputGroup>
                    <InputGroupInput Value="johndoe" />
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                        <LucideIcon Name="circle-check" Size="16" Color="Green" />
                    </InputGroupAddon>
                </InputGroup>

                <!-- Error -->
                <InputGroup>
                    <InputGroupInput Type="InputType.Email" Value="invalid-email" AriaInvalid="true" />
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                        <LucideIcon Name="triangle-alert" Size="16" Class="text-destructive" />
                    </InputGroupAddon>
                </InputGroup>
                """;

        private const string _advancedCode = """
                <!-- Search with clear + submit -->
                <InputGroup>
                    <InputGroupAddon Align="InputGroupAlign.InlineStart">
                        <LucideIcon Name="search" Size="16" />
                    </InputGroupAddon>
                    <InputGroupInput Placeholder="Search files and folders..." />
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                        <InputGroupButton Variant="ButtonVariant.Ghost" Class="!px-1.5 !py-1.5 !rounded-md">
                            <LucideIcon Name="x" Size="16" />
                        </InputGroupButton>
                    </InputGroupAddon>
                    <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                        <InputGroupButton Class="!px-2.5 !py-0.5 !rounded-sm">Search</InputGroupButton>
                    </InputGroupAddon>
                </InputGroup>
                """;

        private const string _dropdownCode = """
                <DropdownMenu>
                    <InputGroup>
                        <InputGroupInput Placeholder="Enter file name" />
                        <InputGroupAddon Align="InputGroupAlign.InlineEnd">
                            <DropdownMenuTrigger AsChild="true">
                                <InputGroupButton Variant="ButtonVariant.Ghost" Class="!px-1.5 !py-1.5 !rounded-md">
                                    <LucideIcon Name="chevron-down" Size="16" />
                                </InputGroupButton>
                            </DropdownMenuTrigger>
                        </InputGroupAddon>
                    </InputGroup>
                    <DropdownMenuContent Align="@PopoverAlign.End">
                        <DropdownMenuItem>Settings</DropdownMenuItem>
                        <DropdownMenuItem>Copy path</DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
                """;

        private const string _codeEditorCode = """
                <InputGroup>
                    <InputGroupAddon Align="InputGroupAlign.BlockStart" Class="order-1 border-b !py-3">
                        <InputGroupText Class="font-mono font-medium gap-1.5">
                            <LucideIcon Name="file-code" Size="16" /> script.js
                        </InputGroupText>
                        <InputGroupButton Variant="ButtonVariant.Ghost" Class="ml-auto !px-2 !py-2 !rounded-md">
                            <LucideIcon Name="copy" Size="16" />
                        </InputGroupButton>
                    </InputGroupAddon>
                    <InputGroupTextarea Placeholder="console.log('Hello, world!');" Rows="8"
                        Class="min-h-[200px] font-mono text-sm order-2 !border-0" />
                    <InputGroupAddon Align="InputGroupAlign.BlockEnd" Class="order-3 border-t !py-3">
                        <InputGroupText>Line 1, Column 1</InputGroupText>
                        <InputGroupButton Class="ml-auto !px-3 !py-1.5 !rounded-md">
                            Run <LucideIcon Name="corner-down-left" Size="16" />
                        </InputGroupButton>
                    </InputGroupAddon>
                </InputGroup>
                """;
    }
}
