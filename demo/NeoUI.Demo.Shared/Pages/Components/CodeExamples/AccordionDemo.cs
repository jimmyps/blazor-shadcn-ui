namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class AccordionDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _accordionProps =
            [
                new("Type",        "AccordionType", "Single",   "Controls whether one or multiple items can be open. Options: Single, Multiple."),
                new("Collapsible", "bool",          "false",    "When true (Single mode), allows closing an open item by clicking its trigger again."),
                new("Value",       "string?",       null,       "Controlled value of the open item (Single mode)."),
                new("Values",      "IEnumerable<string>?", null, "Controlled values of open items (Multiple mode)."),
                new("Class",       "string?",       null,       "Additional CSS classes appended to the root element."),
            ];

        private const string _defaultCode =
                """
                <Accordion Type="AccordionType.Single" Collapsible="true" Class="w-full">
                    <AccordionItem Value="item-1">
                        <AccordionTrigger>Is it accessible?</AccordionTrigger>
                        <AccordionContent>Yes. It adheres to the WAI-ARIA design pattern.</AccordionContent>
                    </AccordionItem>
                    <AccordionItem Value="item-2">
                        <AccordionTrigger>Is it styled?</AccordionTrigger>
                        <AccordionContent>Yes. It comes with default styles that matches the other components' aesthetic.</AccordionContent>
                    </AccordionItem>
                    <AccordionItem Value="item-3">
                        <AccordionTrigger>Is it animated?</AccordionTrigger>
                        <AccordionContent>Yes. It's animated by default, but you can disable it if you prefer.</AccordionContent>
                    </AccordionItem>
                </Accordion>
                """;

        private const string _faqCode =
                """
                <Accordion Type="AccordionType.Single" Collapsible="true" Class="w-full">
                    <AccordionItem Value="faq-1">
                        <AccordionTrigger>What payment methods do you accept?</AccordionTrigger>
                        <AccordionContent>We accept all major credit cards, PayPal, and bank transfers.</AccordionContent>
                    </AccordionItem>
                    <AccordionItem Value="faq-2">
                        <AccordionTrigger>How long does shipping take?</AccordionTrigger>
                        <AccordionContent>Standard shipping typically takes 5-7 business days.</AccordionContent>
                    </AccordionItem>
                </Accordion>
                """;

        private const string _multipleCode =
                """
                <Accordion Type="AccordionType.Multiple" Class="w-full">
                    <AccordionItem Value="features-1">
                        <AccordionTrigger>Performance</AccordionTrigger>
                        <AccordionContent>Blazing fast rendering with optimized algorithms.</AccordionContent>
                    </AccordionItem>
                    <AccordionItem Value="features-2">
                        <AccordionTrigger>Customization</AccordionTrigger>
                        <AccordionContent>Fully customizable to match your brand.</AccordionContent>
                    </AccordionItem>
                </Accordion>
                """;

        private const string _richCode =
                """
                <Accordion Type="AccordionType.Single" Collapsible="true" Class="w-full">
                    <AccordionItem Value="rich-1">
                        <AccordionTrigger>Step 1: Installation</AccordionTrigger>
                        <AccordionContent>
                            <div class="rounded-md bg-muted p-4">
                                <code class="text-sm">dotnet add package NeoUI.Blazor</code>
                            </div>
                        </AccordionContent>
                    </AccordionItem>
                </Accordion>
                """;
    }
}
