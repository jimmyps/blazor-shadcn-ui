namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class ScrollAreaDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _scrollAreaProps =
            [
                new("Type",                   "ScrollAreaType",  "Auto",   "Scrollbar visibility. Options: Auto, Always, Scroll, Hover."),
                new("ShowVerticalScrollbar",  "bool",            "true",   "Whether to show the vertical scrollbar."),
                new("ShowHorizontalScrollbar","bool",            "false",  "Whether to show the horizontal scrollbar."),
                new("EnableScrollShadows",    "bool",            "false",  "When true, scroll shadows appear at edges to indicate overflow content."),
                new("Class",                  "string?",         null,     "Additional CSS classes for the scroll area root."),
                new("ChildContent",           "RenderFragment?", null,     "The scrollable content."),
            ];

        private const string _scrollShadowsCode =
                """
                <ScrollArea Class="h-60 w-full rounded-md border" EnableScrollShadows="true">
                    <div class="p-4">
                        <!-- content -->
                    </div>
                </ScrollArea>
                """;

        private const string _defaultCode =
                """
                <ScrollArea Class="h-72 w-48 rounded-md border">
                    <div class="p-4">
                        <h4 class="mb-4 text-sm font-medium leading-none">Tags</h4>
                        @foreach (var tag in tags)
                        {
                            <div class="text-sm">@tag</div>
                            <Separator Class="my-2" />
                        }
                    </div>
                </ScrollArea>
                """;

        private const string _horizontalCode =
                """
                <ScrollArea Class="w-96 whitespace-nowrap rounded-md border"
                            ShowHorizontalScrollbar="true"
                            ShowVerticalScrollbar="false">
                    <div class="flex w-max space-x-4 p-4">
                        <!-- content -->
                    </div>
                </ScrollArea>
                """;

        private const string _bothScrollbarsCode =
                """
                <ScrollArea Class="h-72 w-72 rounded-md border"
                            ShowHorizontalScrollbar="true"
                            ShowVerticalScrollbar="true">
                    <div class="p-4 w-[500px]">
                        <!-- wide content -->
                    </div>
                </ScrollArea>
                """;

        private const string _cardListCode =
                """
                <ScrollArea Class="h-80 w-full max-w-md rounded-md border">
                    <div class="p-4 space-y-4">
                        @for (int i = 1; i <= 10; i++)
                        {
                            <div class="rounded-lg border p-4">Item @i</div>
                        }
                    </div>
                </ScrollArea>
                """;

        private const string _chatCode =
                """
                <ScrollArea Class="h-64">
                    <div class="p-4 space-y-4">
                        @foreach (var msg in messages)
                        {
                            <div class="@(msg.IsMe ? "flex justify-end" : "flex")">
                                <div class="rounded-lg px-3 py-2">
                                    <p class="text-sm">@msg.Text</p>
                                </div>
                            </div>
                        }
                    </div>
                </ScrollArea>
                """;

        private const string _scrollTypesCode =
                """
                <ScrollArea Class="h-32 rounded-md border" Type="ScrollAreaType.Auto">
                    <!-- content -->
                </ScrollArea>

                <ScrollArea Class="h-32 rounded-md border" Type="ScrollAreaType.Always">
                    <!-- content -->
                </ScrollArea>
                """;
    }
}
