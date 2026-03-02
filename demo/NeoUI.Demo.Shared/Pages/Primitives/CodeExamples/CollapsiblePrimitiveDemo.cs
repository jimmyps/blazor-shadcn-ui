namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class CollapsiblePrimitiveDemo
    {
        private const string _asChildCode =
        """
        <CollapsiblePrimitive>
            <CollapsibleTriggerPrimitive AsChild>
                <Button Variant="ButtonVariant.Outline" Size="ButtonSize.Small">Toggle</Button>
            </CollapsibleTriggerPrimitive>
            <CollapsibleContentPrimitive>Content</CollapsibleContentPrimitive>
        </CollapsiblePrimitive>
        """;

        private const string _basicCode =
        """
        <CollapsiblePrimitive>
            <CollapsibleTriggerPrimitive>Click to toggle</CollapsibleTriggerPrimitive>
            <CollapsibleContentPrimitive>Collapsible content</CollapsibleContentPrimitive>
        </CollapsiblePrimitive>
        """;

        private const string _controlledCode =
        """
        <CollapsiblePrimitive @bind-Open="isPrimitiveOpen">
            <CollapsibleTriggerPrimitive>Controlled trigger</CollapsibleTriggerPrimitive>
            <CollapsibleContentPrimitive>Controlled content</CollapsibleContentPrimitive>
        </CollapsiblePrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Tab</Kbd>
        <Kbd>Space</Kbd>
        <Kbd>Enter</Kbd>
        """;

        private static readonly IReadOnlyList<DemoPropRow> _collapsibleProps =
    [
        new("Open", "bool", "false", "Controls whether the collapsible is expanded. Supports two-way binding with <code class=\"text-xs bg-muted px-1 rounded\">@bind-Open</code>."),
        new("OpenChanged", "EventCallback&lt;bool&gt;", null, "Callback invoked when the open state changes."),
        new("Disabled", "bool", "false", "Prevents toggling when true."),
        new("ChildContent", "RenderFragment?", null, "Content rendered inside trigger/content primitives."),
    ];
    }
}
