namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class PopoverDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _popoverProps =
            [
                new("Open",           "@bind-Open / bool?",   null,  "Controlled open state."),
                new("DefaultOpen",    "bool",                  "false", "Initial open state."),
                new("OnOpenChange",   "EventCallback&lt;bool&gt;", "-", "Callback when open state changes."),
            ];

        private const string _basicCode =
                """
                <Popover>
                    <PopoverTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">Open Popover</Button>
                    </PopoverTrigger>
                    <PopoverContent>
                        <p class="text-sm">Popover content here.</p>
                    </PopoverContent>
                </Popover>
                """;

        private const string _asChildCode =
                """
                <!-- Without AsChild: PopoverTrigger renders its own button -->
                <Popover>
                    <PopoverTrigger class="...">Open Popover</PopoverTrigger>
                    <PopoverContent><p class="text-sm">Basic popover content</p></PopoverContent>
                </Popover>

                <!-- With AsChild: Button receives trigger behavior -->
                <Popover>
                    <PopoverTrigger AsChild>
                        <Button Variant="ButtonVariant.Outline">
                            <LucideIcon Name="settings" Size="16" /> Settings
                        </Button>
                    </PopoverTrigger>
                    <PopoverContent>...</PopoverContent>
                </Popover>
                """;

        private const string _positioningCode =
                """
                <Popover>
                    <PopoverTrigger AsChild><Button>Top</Button></PopoverTrigger>
                    <PopoverContent Side="@PopoverSide.Top">Positioned on top</PopoverContent>
                </Popover>

                <Popover>
                    <PopoverTrigger AsChild><Button>Right</Button></PopoverTrigger>
                    <PopoverContent Side="@PopoverSide.Right">Positioned on right</PopoverContent>
                </Popover>
                """;

        private const string _controlledCode =
                """
                <Popover @bind-Open="isOpen">
                    <PopoverTrigger AsChild>
                        <Button>Toggle Controlled</Button>
                    </PopoverTrigger>
                    <PopoverContent>
                        <p class="text-sm">This popover is controlled externally.</p>
                    </PopoverContent>
                </Popover>
                <Button Variant="ButtonVariant.Outline" OnClick="() => isOpen = !isOpen">
                    Toggle from Outside
                </Button>
                """;
    }
}
