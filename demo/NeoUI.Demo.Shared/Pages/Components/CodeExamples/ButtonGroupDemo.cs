namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ButtonGroupDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _buttonGroupProps =
            [
                new("Orientation",   "ButtonGroupOrientation", "Horizontal", "Group orientation. Options: Horizontal, Vertical."),
                new("Class",         "string?",                "null",       "Additional CSS classes appended to the root element."),
                new("ChildContent",  "RenderFragment?",        "null",       "Button and other group elements."),
            ];

        private const string _basicCode = """
                <ButtonGroup>
                    <Button Variant="ButtonVariant.Outline">Archive</Button>
                    <Button Variant="ButtonVariant.Outline">Report</Button>
                    <Button Variant="ButtonVariant.Outline">Delete</Button>
                </ButtonGroup>

                <!-- With icons -->
                <ButtonGroup>
                    <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Icon" AriaLabel="Bold">
                        <LucideIcon Name="bold" Size="16" />
                    </Button>
                    <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Icon" AriaLabel="Italic">
                        <LucideIcon Name="italic" Size="16" />
                    </Button>
                </ButtonGroup>
                """;

        private const string _verticalCode = """
                <ButtonGroup Orientation="ButtonGroupOrientation.Vertical">
                    <Button Variant="ButtonVariant.Outline" Class="justify-start">
                        <LucideIcon Name="house" Size="16" /> Home
                    </Button>
                    <Button Variant="ButtonVariant.Outline" Class="justify-start">
                        <LucideIcon Name="user" Size="16" /> Profile
                    </Button>
                    <Button Variant="ButtonVariant.Outline" Class="justify-start">
                        <LucideIcon Name="settings" Size="16" /> Settings
                    </Button>
                </ButtonGroup>
                """;

        private const string _separatorCode = """
                <ButtonGroup>
                    <Button Variant="ButtonVariant.Outline">Copy</Button>
                    <Button Variant="ButtonVariant.Outline">Paste</Button>
                    <ButtonGroupSeparator />
                    <Button Variant="ButtonVariant.Outline">Undo</Button>
                    <Button Variant="ButtonVariant.Outline">Redo</Button>
                </ButtonGroup>
                """;

        private const string _textLabelCode = """
                <ButtonGroup>
                    <ButtonGroupText>
                        <LucideIcon Name="file-text" Size="16" />
                        Document Actions:
                    </ButtonGroupText>
                    <Button Variant="ButtonVariant.Outline">View</Button>
                    <Button Variant="ButtonVariant.Outline">Edit</Button>
                    <Button Variant="ButtonVariant.Outline">Delete</Button>
                </ButtonGroup>
                """;

        private const string _nestedCode = """
                <ButtonGroup>
                    <ButtonGroup>
                        <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Icon" AriaLabel="Go Back">
                            <LucideIcon Name="arrow-left" Size="16" />
                        </Button>
                    </ButtonGroup>
                    <ButtonGroup>
                        <Button Variant="ButtonVariant.Outline">Archive</Button>
                        <Button Variant="ButtonVariant.Outline">Report</Button>
                    </ButtonGroup>
                    <ButtonGroup>
                        <Button Variant="ButtonVariant.Outline">Snooze</Button>
                    </ButtonGroup>
                </ButtonGroup>
                """;

        private const string _complexCode = """
                <ButtonGroup>
                    <ButtonGroup>
                        <Button Variant="ButtonVariant.Outline">Snooze</Button>
                        <DropdownMenu>
                            <DropdownMenuTrigger class="... !rounded-l-none ...">
                                <LucideIcon Name="ellipsis" Size="16" />
                            </DropdownMenuTrigger>
                            <DropdownMenuContent Align="@PopoverAlign.End">
                                <DropdownMenuItem>Mark as Read</DropdownMenuItem>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem>Trash</DropdownMenuItem>
                            </DropdownMenuContent>
                        </DropdownMenu>
                    </ButtonGroup>
                </ButtonGroup>
                """;

        private const string _popoverCode = """
                <ButtonGroup>
                    <Button Variant="ButtonVariant.Outline">
                        <LucideIcon Name="bot" Size="16" /> Copilot
                    </Button>
                    <Popover>
                        <PopoverTrigger>
                            <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Icon" Class="!rounded-l-none" AriaLabel="Open Popover">
                                <LucideIcon Name="chevron-down" Size="16" />
                            </Button>
                        </PopoverTrigger>
                        <PopoverContent Align="@PopoverAlign.End" Class="w-80">
                            <!-- popover content -->
                        </PopoverContent>
                    </Popover>
                </ButtonGroup>
                """;

        private const string _selectCode = """
                <ButtonGroup>
                    <Select @bind-Value="selectedCurrency" TValue="string">
                        <SelectTrigger Class="w-[70px] !rounded-r-none">
                            <SelectValue Placeholder="$" />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem Value="@("$")" TValue="string">$ US Dollar</SelectItem>
                            <SelectItem Value="@("€")" TValue="string">€ Euro</SelectItem>
                        </SelectContent>
                    </Select>
                    <Input Type="InputType.Text" Placeholder="10.00" Class="!rounded-l-none !border-l-0" />
                </ButtonGroup>
                """;
    }
}
