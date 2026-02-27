namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ResizableDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _resizableProps =
            [
                new("Direction",       "ResizableDirection", "Horizontal", "Layout direction. Options: Horizontal, Vertical."),
                new("DefaultSize",     "int",                null,         "Initial size of the panel as a percentage (0–100)."),
                new("MinSize",         "int?",               null,         "Minimum allowed size of the panel as a percentage."),
                new("MaxSize",         "int?",               null,         "Maximum allowed size of the panel as a percentage."),
                new("Index",           "int",                null,         "Zero-based index of the ResizableHandle within its group."),
                new("WithHandle",      "bool",               "false",      "When true, renders a visible grip icon on the drag handle."),
                new("OnLayoutChange",  "EventCallback<int[]>", null,       "Fired when the panel layout changes. Receives current sizes array."),
                new("Class",           "string?",            null,         "Additional CSS classes for the component root element."),
            ];

        private const string _horizontalCode =
                """
                <ResizablePanelGroup Direction="ResizableDirection.Horizontal" Class="min-h-[200px] rounded-lg border">
                    <ResizablePanel DefaultSize="50">
                        <div class="flex h-full items-center justify-center p-6">Panel 1</div>
                    </ResizablePanel>
                    <ResizableHandle Index="0" />
                    <ResizablePanel DefaultSize="50">
                        <div class="flex h-full items-center justify-center p-6">Panel 2</div>
                    </ResizablePanel>
                </ResizablePanelGroup>
                """;

        private const string _verticalCode =
                """
                <ResizablePanelGroup Direction="ResizableDirection.Vertical" Class="min-h-[300px] rounded-lg border">
                    <ResizablePanel DefaultSize="30">
                        <div class="flex h-full items-center justify-center p-6">Header</div>
                    </ResizablePanel>
                    <ResizableHandle Index="0" />
                    <ResizablePanel DefaultSize="70">
                        <div class="flex h-full items-center justify-center p-6">Content</div>
                    </ResizablePanel>
                </ResizablePanelGroup>
                """;

        private const string _withHandleCode =
                """
                <ResizablePanelGroup Direction="ResizableDirection.Horizontal" Class="min-h-[200px] rounded-lg border">
                    <ResizablePanel DefaultSize="50">
                        <div class="flex h-full items-center justify-center p-6">Left Panel</div>
                    </ResizablePanel>
                    <ResizableHandle Index="0" WithHandle="true" />
                    <ResizablePanel DefaultSize="50">
                        <div class="flex h-full items-center justify-center p-6">Right Panel</div>
                    </ResizablePanel>
                </ResizablePanelGroup>
                """;

        private const string _threePanelCode =
                """
                <ResizablePanelGroup Direction="ResizableDirection.Horizontal" Class="min-h-[250px] rounded-lg border">
                    <ResizablePanel DefaultSize="20" MinSize="15">
                        <div class="flex h-full items-center justify-center p-4 bg-muted/20">Sidebar</div>
                    </ResizablePanel>
                    <ResizableHandle Index="0" />
                    <ResizablePanel DefaultSize="60">
                        <div class="flex h-full items-center justify-center p-4">Main Content</div>
                    </ResizablePanel>
                    <ResizableHandle Index="1" />
                    <ResizablePanel DefaultSize="20" MinSize="15">
                        <div class="flex h-full items-center justify-center p-4 bg-muted/20">Details</div>
                    </ResizablePanel>
                </ResizablePanelGroup>
                """;

        private const string _nestedCode =
                """
                <ResizablePanelGroup Direction="ResizableDirection.Horizontal" Class="min-h-[350px] rounded-lg border">
                    <ResizablePanel DefaultSize="25">
                        <div class="flex h-full items-center justify-center p-4 bg-muted/20">Navigation</div>
                    </ResizablePanel>
                    <ResizableHandle Index="0" />
                    <ResizablePanel DefaultSize="75">
                        <ResizablePanelGroup Direction="ResizableDirection.Vertical">
                            <ResizablePanel DefaultSize="60">
                                <div class="flex h-full items-center justify-center p-4">Editor</div>
                            </ResizablePanel>
                            <ResizableHandle Index="0" />
                            <ResizablePanel DefaultSize="40">
                                <div class="flex h-full items-center justify-center p-4 bg-muted/10">Terminal</div>
                            </ResizablePanel>
                        </ResizablePanelGroup>
                    </ResizablePanel>
                </ResizablePanelGroup>
                """;

        private const string _emailClientCode =
                """
                <ResizablePanelGroup Direction="ResizableDirection.Horizontal" Class="min-h-[400px] rounded-lg border">
                    <ResizablePanel DefaultSize="20" MinSize="10">
                        <!-- Folders sidebar -->
                    </ResizablePanel>
                    <ResizableHandle Index="0" WithHandle="true" />
                    <ResizablePanel DefaultSize="30" MinSize="20">
                        <!-- Message list -->
                    </ResizablePanel>
                    <ResizableHandle Index="1" WithHandle="true" />
                    <ResizablePanel DefaultSize="50">
                        <!-- Message detail -->
                    </ResizablePanel>
                </ResizablePanelGroup>
                """;
    }
}
