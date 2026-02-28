namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class EmptyDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _emptyProps =
            [
                new("Class", "string?", null, "Additional CSS classes applied to the container."),
            ];

        private const string _basicCode =
                """
                <Empty>
                    <EmptyIcon>
                        <LucideIcon Name="inbox" Class="h-10 w-10" />
                    </EmptyIcon>
                    <EmptyTitle>No messages</EmptyTitle>
                    <EmptyDescription>
                        You don't have any messages yet.
                    </EmptyDescription>
                </Empty>
                """;

        private const string _withActionsCode =
                """
                <Empty>
                    <EmptyIcon>
                        <LucideIcon Name="file-text" Class="h-10 w-10" />
                    </EmptyIcon>
                    <EmptyTitle>No documents</EmptyTitle>
                    <EmptyDescription>
                        Get started by creating your first document.
                    </EmptyDescription>
                    <EmptyActions>
                        <Button>Create Document</Button>
                    </EmptyActions>
                </Empty>
                """;

        private const string _usageCode =
                """
                <!-- No search results -->
                <Empty>
                    <EmptyIcon><LucideIcon Name="search" Class="h-10 w-10" /></EmptyIcon>
                    <EmptyTitle>No results found</EmptyTitle>
                    <EmptyDescription>Try using different keywords.</EmptyDescription>
                </Empty>

                <!-- Empty cart with action -->
                <Empty>
                    <EmptyIcon><LucideIcon Name="shopping-cart" Class="h-10 w-10" /></EmptyIcon>
                    <EmptyTitle>Your cart is empty</EmptyTitle>
                    <EmptyDescription>Add items to your cart to get started.</EmptyDescription>
                    <EmptyActions><Button>Continue Shopping</Button></EmptyActions>
                </Empty>
                """;
    }
}
