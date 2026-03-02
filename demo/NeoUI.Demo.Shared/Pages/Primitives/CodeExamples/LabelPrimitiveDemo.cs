namespace NeoUI.Demo.Shared.Pages.Primitives
{
    partial class LabelPrimitiveDemo
    {
        private const string _basicCode =
        """
        <LabelPrimitive For="basic-input" class="text-sm font-medium">
            Basic label
        </LabelPrimitive>
        <input type="text" id="basic-input" placeholder="Click the label to focus" />
        """;

        private const string _formElementsCode =
        """
        <LabelPrimitive For="text-input">Text Input</LabelPrimitive>
        <input id="text-input" type="text" />

        <LabelPrimitive For="select-input">Select Dropdown</LabelPrimitive>
        <select id="select-input"><option>Option 1</option></select>

        <LabelPrimitive For="textarea-input">Textarea</LabelPrimitive>
        <textarea id="textarea-input"></textarea>
        """;

        private const string _keyboardCode =
        """
        <Kbd>Tab</Kbd>
        <Kbd>Click</Kbd>
        """;

        private const string _requiredCode =
        """
        <LabelPrimitive For="required-input">
            Username <span class="text-red-500">*</span>
        </LabelPrimitive>
        <input id="required-input" required />

        <LabelPrimitive For="optional-input">
            Nickname <span class="text-muted-foreground text-xs">(optional)</span>
        </LabelPrimitive>
        <input id="optional-input" />
        """;

        private static readonly IReadOnlyList<DemoPropRow> _labelProps =
    [
        new("For", "string?", null, "The ID of the form element this label is associated with. Maps to the HTML <code class=\"text-xs bg-muted px-1 rounded\">for</code> attribute."),
        new("ChildContent", "RenderFragment?", null, "Content rendered inside the label element."),
        new("AdditionalAttributes", "IReadOnlyDictionary&lt;string, object&gt;?", null, "Additional attributes (such as <code class=\"text-xs bg-muted px-1 rounded\">class</code> or <code class=\"text-xs bg-muted px-1 rounded\">style</code>) passed to the label element."),
    ];
    }
}
