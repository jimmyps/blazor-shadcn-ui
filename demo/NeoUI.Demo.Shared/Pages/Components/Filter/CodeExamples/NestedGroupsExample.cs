namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class NestedGroupsExample
{
    private static readonly string _nestedCode = """
        // (Status = "Pending" OR Status = "Processing") AND Amount > 1000
        var filters = new FilterGroup
        {
            Logic = LogicalOperator.And,
            Conditions = new List<FilterCondition>
            {
                new FilterCondition { Field = "Amount", Operator = FilterOperator.GreaterThan, Value = 1000 }
            },
            NestedGroups = new List<FilterGroup>
            {
                new FilterGroup
                {
                    Logic = LogicalOperator.Or,
                    Conditions = new List<FilterCondition>
                    {
                        new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Pending" },
                        new FilterCondition { Field = "Status", Operator = FilterOperator.Equals, Value = "Processing" }
                    }
                }
            }
        };
        """;

    private static readonly string _linqCode = """
        // Apply the nested filter group to any IEnumerable<T> or IQueryable<T>
        var results = allOrders.ApplyFilters(filters).ToList();
        """;
}
