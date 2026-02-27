namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class PaginationDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _paginationProps =
            [
                new("ChildContent", "RenderFragment?", null, "PaginationContent and items."),
            ];

        private const string _basicCode =
                """
                <Pagination>
                    <PaginationContent>
                        <PaginationItem><PaginationPrevious Href="#" /></PaginationItem>
                        <PaginationItem><PaginationLink Href="#">1</PaginationLink></PaginationItem>
                        <PaginationItem><PaginationLink Href="#" IsActive="true">2</PaginationLink></PaginationItem>
                        <PaginationItem><PaginationLink Href="#">3</PaginationLink></PaginationItem>
                        <PaginationItem><PaginationNext Href="#" /></PaginationItem>
                    </PaginationContent>
                </Pagination>
                """;

        private const string _ellipsisCode =
                """
                <Pagination>
                    <PaginationContent>
                        <PaginationItem><PaginationPrevious Href="#" /></PaginationItem>
                        <PaginationItem><PaginationLink Href="#">1</PaginationLink></PaginationItem>
                        <PaginationItem><PaginationLink Href="#" IsActive="true">2</PaginationLink></PaginationItem>
                        <PaginationItem><PaginationLink Href="#">3</PaginationLink></PaginationItem>
                        <PaginationItem><PaginationEllipsis /></PaginationItem>
                        <PaginationItem><PaginationLink Href="#">10</PaginationLink></PaginationItem>
                        <PaginationItem><PaginationNext Href="#" /></PaginationItem>
                    </PaginationContent>
                </Pagination>
                """;

        private const string _interactiveCode =
                """
                <Pagination>
                    <PaginationContent>
                        <PaginationItem>
                            <button @onclick="GoToPrevious" disabled="@(currentPage == 1)">Previous</button>
                        </PaginationItem>
                        @for (int page = 1; page <= totalPages; page++)
                        {
                            var pageNum = page;
                            <PaginationItem>
                                <button @onclick="() => GoToPage(pageNum)">@pageNum</button>
                            </PaginationItem>
                        }
                        <PaginationItem>
                            <button @onclick="GoToNext" disabled="@(currentPage == totalPages)">Next</button>
                        </PaginationItem>
                    </PaginationContent>
                </Pagination>
                """;
    }
}
