namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class AccordionPrimitiveDemo
    {
        private const string _basicCode =
        """
        <AccordionPrimitive DefaultValue="@(new HashSet<string> { "item-1" })">
            <AccordionItemPrimitive Value="item-1">
                <AccordionTriggerPrimitive>Is it accessible?</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>Yes.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;

        private const string _controlledCode =
        """
        <AccordionPrimitive @bind-Value="controlledValue" Type="AccordionType.Single" CollapsiblePrimitive="true">
            <AccordionItemPrimitive Value="controlled-1">
                <AccordionTriggerPrimitive>Controlled Item 1</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>This item's state is controlled externally.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
            <AccordionItemPrimitive Value="controlled-2">
                <AccordionTriggerPrimitive>Controlled Item 2</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>Use the buttons above to control which item is open.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Tab</Kbd>
        <Kbd>Enter</Kbd>
        <Kbd>Space</Kbd>
        <Kbd>↑</Kbd>
        <Kbd>↓</Kbd>
        """;

        private const string _multipleCode =
        """
        <AccordionPrimitive Type="AccordionType.Multiple" DefaultValue="@(new HashSet<string> { \"multiple-1\" })">
            <AccordionItemPrimitive Value="multiple-1">
                <AccordionTriggerPrimitive>First Section</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>This can be open alongside other sections.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
            <AccordionItemPrimitive Value="multiple-2">
                <AccordionTriggerPrimitive>Second Section</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>Open both sections at once to see them together.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
            <AccordionItemPrimitive Value="multiple-3">
                <AccordionTriggerPrimitive>Third Section</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>All three can be open simultaneously in multiple mode.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;

        private const string _singleModeCode =
        """
        <AccordionPrimitive Type="AccordionType.Single" CollapsiblePrimitive="true">
            <AccordionItemPrimitive Value="item-1">
                <AccordionTriggerPrimitive>Item 1</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>Content 1 - Click again to collapse.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
            <AccordionItemPrimitive Value="item-2">
                <AccordionTriggerPrimitive>Item 2</AccordionTriggerPrimitive>
                <AccordionContentPrimitive>Content 2 - Click again to collapse.</AccordionContentPrimitive>
            </AccordionItemPrimitive>
        </AccordionPrimitive>
        """;
    }
}
