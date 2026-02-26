namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class CollapsibleDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _collapsibleProps =
            [
                new("Open",         "bool",                  "false", "Controls whether the collapsible is expanded."),
                new("OpenChanged",  "EventCallback&lt;bool&gt;", "—", "Callback invoked when the open state changes."),
                new("Disabled",     "bool",                  "false", "Prevents toggling when true."),
                new("AsChild",      "bool",                  "false", "On CollapsibleTrigger: passes trigger behavior to the child element."),
                new("Class",        "string?",               "null",  "Additional CSS classes for the root element."),
                new("ChildContent", "RenderFragment?",       "null",  "Content inside the component."),
            ];

        private const string _asChildCode = """
                <!-- Without AsChild: CollapsibleTrigger renders its own button -->
                <Collapsible Class="border rounded-lg">
                    <div class="px-4 py-3">
                        <CollapsibleTrigger Class="flex w-full items-center justify-between ...">
                            <span>Show more</span>
                            <LucideIcon Name="chevron-down" Size="16" />
                        </CollapsibleTrigger>
                    </div>
                    <CollapsibleContent Class="px-4 pb-4">Hidden content</CollapsibleContent>
                </Collapsible>

                <!-- With AsChild: Button receives trigger behavior -->
                <Collapsible Class="border rounded-lg">
                    <CollapsibleTrigger AsChild>
                        <Button Variant="ButtonVariant.Ghost" Size="ButtonSize.Small" Class="w-full justify-between">
                            Toggle Section
                        </Button>
                    </CollapsibleTrigger>
                    <CollapsibleContent Class="px-4 pb-4">Content here</CollapsibleContent>
                </Collapsible>
                """;

        private const string _basicCode = """
                <Collapsible Class="border rounded-lg">
                    <div class="px-4 py-3">
                        <CollapsibleTrigger Class="flex w-full items-center justify-between rounded-md hover:bg-accent [&_svg]:transition-transform data-[state=open]:[&_svg]:rotate-180">
                            <span>Can I use this in my project?</span>
                            <LucideIcon Name="chevron-down" Size="16" />
                        </CollapsibleTrigger>
                    </div>
                    <CollapsibleContent Class="px-4 pb-4">
                        <p class="text-sm text-muted-foreground">Yes! This component is free to use in your projects.</p>
                    </CollapsibleContent>
                </Collapsible>
                """;

        private const string _controlledCode = """
                <Collapsible Open="@isOpen"
                             OpenChanged="@((bool open) => isOpen = open)"
                             Class="border rounded-lg">
                    <div class="px-4 py-3">
                        <CollapsibleTrigger Class="flex w-full items-center justify-between rounded-md hover:bg-accent">
                            <span>Controlled Content</span>
                            <LucideIcon Name="@(isOpen ? "chevron-up" : "chevron-down")" Size="16" />
                        </CollapsibleTrigger>
                    </div>
                    <CollapsibleContent Class="px-4 pb-4">
                        <p class="text-sm">The parent component controls this collapsible's state.</p>
                    </CollapsibleContent>
                </Collapsible>
                """;

        private const string _disabledCode = """
                <Collapsible Disabled Class="border rounded-lg opacity-50">
                    <div class="px-4 py-3">
                        <CollapsibleTrigger Class="flex w-full items-center justify-between rounded-md">
                            <span>Disabled Collapsible</span>
                            <LucideIcon Name="chevron-down" Size="16" />
                        </CollapsibleTrigger>
                    </div>
                    <CollapsibleContent Class="px-4 pb-4">
                        <p class="text-sm">This content cannot be toggled.</p>
                    </CollapsibleContent>
                </Collapsible>
                """;

        private const string _accordionCode = """
                @foreach (var item in faqItems)
                {
                    <Collapsible Class="border rounded-lg">
                        <div class="px-4 py-3">
                            <CollapsibleTrigger Class="flex w-full items-center justify-between text-left [&_svg]:transition-transform data-[state=open]:[&_svg]:rotate-180">
                                <span class="font-medium">@item.Question</span>
                                <LucideIcon Name="chevron-down" Size="16" />
                            </CollapsibleTrigger>
                        </div>
                        <CollapsibleContent Class="px-4 pb-4 border-t">
                            <p class="pt-4 text-sm text-muted-foreground">@item.Answer</p>
                        </CollapsibleContent>
                    </Collapsible>
                }
                """;

        private const string _iconsCode = """
                <Collapsible Class="border rounded-lg">
                    <div class="px-4 py-3">
                        <CollapsibleTrigger Class="flex w-full items-center justify-between [&>svg]:transition-transform data-[state=open]:[&>svg]:rotate-180">
                            <div class="flex items-center gap-2">
                                <LucideIcon Name="star" Size="16" />
                                <span class="font-medium">Premium Features</span>
                                <Badge Variant="BadgeVariant.Default">New</Badge>
                            </div>
                            <LucideIcon Name="chevron-down" Size="16" />
                        </CollapsibleTrigger>
                    </div>
                    <CollapsibleContent Class="px-4 pb-4">
                        <!-- feature list -->
                    </CollapsibleContent>
                </Collapsible>
                """;

        private const string _animationsCode = """
                <Collapsible Class="border rounded-lg overflow-hidden">
                    <div class="px-4 py-3">
                        <CollapsibleTrigger Class="flex w-full items-center justify-between rounded-md hover:bg-accent [&_svg]:transition-transform [&_svg]:duration-200 data-[state=open]:[&_svg]:rotate-180">
                            <span>Animated Content</span>
                            <LucideIcon Name="chevron-down" Size="16" />
                        </CollapsibleTrigger>
                    </div>
                    <CollapsibleContent Class="px-4 pb-4 transition-all duration-300 ease-in-out">
                        <p class="text-sm mb-2">This content animates smoothly when expanding and collapsing.</p>
                        <p class="text-sm text-muted-foreground">CSS transitions create a polished user experience.</p>
                    </CollapsibleContent>
                </Collapsible>
                """;

        private const string _nestedCode = """
                <Collapsible Class="border rounded-lg">
                    <div class="px-4 py-3">
                        <CollapsibleTrigger Class="flex w-full items-center justify-between [&_svg]:transition-transform data-[state=open]:[&_svg]:rotate-180">
                            <span class="font-medium">Parent Section</span>
                            <LucideIcon Name="chevron-down" Size="16" />
                        </CollapsibleTrigger>
                    </div>
                    <CollapsibleContent Class="px-4 pb-4">
                        <!-- Nested Collapsible -->
                        <Collapsible Class="border rounded-md">
                            <div class="px-3 py-2 bg-muted/50">
                                <CollapsibleTrigger Class="flex w-full items-center justify-between text-sm">
                                    <span>Child Section</span>
                                    <LucideIcon Name="chevron-down" Size="14" />
                                </CollapsibleTrigger>
                            </div>
                            <CollapsibleContent Class="px-3 py-2 text-sm text-muted-foreground">
                                Nested content here.
                            </CollapsibleContent>
                        </Collapsible>
                    </CollapsibleContent>
                </Collapsible>
                """;
    }
}
