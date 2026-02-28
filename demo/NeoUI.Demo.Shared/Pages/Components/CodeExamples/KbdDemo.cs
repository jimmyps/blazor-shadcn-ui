namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class KbdDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _kbdProps =
            [
                new("ChildContent", "RenderFragment?", null, "The key text or symbol to display."),
                new("Class",        "string?",         null, "Additional CSS classes."),
            ];

        private const string _basicCode =
                """
                <span>Press</span>
                <Kbd>Enter</Kbd>
                <span>to submit</span>
                """;

        private const string _combinationsCode =
                """
                <div class="flex gap-2 items-center">
                    <Kbd>Ctrl</Kbd>
                    <span>+</span>
                    <Kbd>C</Kbd>
                    <span class="text-sm text-muted-foreground">Copy</span>
                </div>

                <div class="flex gap-2 items-center">
                    <Kbd>Ctrl</Kbd>
                    <span>+</span>
                    <Kbd>V</Kbd>
                    <span class="text-sm text-muted-foreground">Paste</span>
                </div>
                """;

        private const string _symbolsCode =
                """
                <div class="flex gap-2 items-center">
                    <Kbd>⌘</Kbd>
                    <span>+</span>
                    <Kbd>K</Kbd>
                    <span class="text-sm text-muted-foreground">Open command palette</span>
                </div>
                """;

        private const string _usageCode =
                """
                <div class="flex justify-between items-center p-2 rounded hover:bg-accent">
                    <span class="text-sm">Save</span>
                    <div class="flex gap-1">
                        <Kbd>Ctrl</Kbd>
                        <span class="text-xs">+</span>
                        <Kbd>S</Kbd>
                    </div>
                </div>
                """;
    }
}
