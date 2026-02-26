namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class AccordionPrimitiveDemo
    {
        private static readonly IReadOnlyList<DemoPropRow> _accordionProps =
        [
            new("Type",                "AccordionType",          "Single",   "Whether one or multiple items can be open at once. Options: Single, Multiple."),
            new("CollapsiblePrimitive","bool",                   "false",    "When true in Single mode, allows closing the open item so all are closed."),
            new("DefaultValue",        "HashSet&lt;string&gt;",  "—",        "Initial open items (uncontrolled mode)."),
            new("Value",               "HashSet&lt;string&gt;",  "—",        "Controlled open items. Use with @bind-Value."),
            new("ValueChanged",        "EventCallback",          "—",        "Callback invoked when the open items change."),
            new("Value (Item)",        "string",                 "required", "On AccordionItemPrimitive: unique identifier for this item."),
            new("Disabled",            "bool",                   "false",    "On AccordionItemPrimitive: prevents interaction when true."),
            new("ChildContent",        "RenderFragment?",        "—",        "Content rendered inside trigger or content primitive."),
        ];

        private const string _basicCode =
        """
        <AccordionPrimitive DefaultValue="@(new HashSet<string> { "item-1" })" class="border rounded">
            <AccordionItemPrimitive Value="item-1" class="border-b last:border-0">
                <AccordionTriggerPrimitive class="w-full p-4 text-left cursor-pointer hover:bg-muted/50">
                    Is it accessible?
                </AccordionTriggerPrimitive>
                <AccordionContentPrimitive class="p-4 pt-0">
                    Yes. It adheres to the WAI-ARIA design pattern.
                </AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;

        private const string _controlledCode =
        """
        <AccordionPrimitive @bind-Value="controlledValue" Type="AccordionType.Single" CollapsiblePrimitive="true" class="border rounded">
            <AccordionItemPrimitive Value="controlled-1" class="border-b last:border-0">
                <AccordionTriggerPrimitive class="w-full p-4 text-left cursor-pointer hover:bg-muted/50">
                    Controlled Item 1
                </AccordionTriggerPrimitive>
                <AccordionContentPrimitive class="p-4 pt-0">
                    This item's state is controlled externally.
                </AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;

        private const string _singleCode =
        """
        <AccordionPrimitive Type="AccordionType.Single" CollapsiblePrimitive="true" class="border rounded">
            <AccordionItemPrimitive Value="item-1" class="border-b last:border-0">
                <AccordionTriggerPrimitive class="w-full p-4 text-left cursor-pointer hover:bg-muted/50">Item 1</AccordionTriggerPrimitive>
                <AccordionContentPrimitive class="p-4 pt-0">Content 1 - Click again to collapse.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;

        private const string _multipleCode =
        """
        <AccordionPrimitive Type="AccordionType.Multiple" class="border rounded">
            <AccordionItemPrimitive Value="multiple-1" class="border-b last:border-0">
                <AccordionTriggerPrimitive class="w-full p-4 text-left cursor-pointer hover:bg-muted/50">First Section</AccordionTriggerPrimitive>
                <AccordionContentPrimitive class="p-4 pt-0">This can be open alongside other sections.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
            <AccordionItemPrimitive Value="multiple-2" class="border-b last:border-0">
                <AccordionTriggerPrimitive class="w-full p-4 text-left cursor-pointer hover:bg-muted/50">Second Section</AccordionTriggerPrimitive>
                <AccordionContentPrimitive class="p-4 pt-0">Open both sections at once to see them together.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;
    }
}
