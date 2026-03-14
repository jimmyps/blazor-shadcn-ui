namespace NeoUI.Demo.Shared.Pages.Components.Charts;

partial class RadialBarChartExamples
{
    public const string BasicCode =
        """
        <ChartContainer Height="320" Class="w-full">
            <RadialBarChart Data="@_skillData">
                <PolarGrid Stroke="var(--border)" />
                <XAxis DataKey="skill" />
                <ChartTooltip />
                <RadialBar DataKey="score" Name="Score"
                           Color="var(--chart-1)"
                           ShowBackground="true"
                           CornerRadius="4" />
            </RadialBarChart>
        </ChartContainer>

        @code {
            private record SkillScore(string skill, double score);
            private static readonly SkillScore[] _skillData =
            [
                new("Blazor",     92),
                new("TypeScript", 78),
                new("SQL",        85),
                new("Docker",     63),
                new("UI/UX",      71),
            ];
        }
        """;

    public const string MultiSeriesCode =
        """
        <ChartContainer Height="320" Class="w-full">
            <RadialBarChart Data="@_quarterData">
                <PolarGrid Stroke="var(--border)" />
                <XAxis DataKey="region" />
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <RadialBar DataKey="q1" Name="Q1" Color="var(--chart-1)" ShowBackground="true" CornerRadius="4" />
                <RadialBar DataKey="q2" Name="Q2" Color="var(--chart-2)" CornerRadius="4" />
            </RadialBarChart>
        </ChartContainer>

        @code {
            private record QuarterRevenue(string region, double q1, double q2);
            private static readonly QuarterRevenue[] _quarterData =
            [
                new("North", 82, 91),
                new("South", 67, 74),
                new("East",  55, 68),
                new("West",  90, 88),
            ];
        }
        """;

    public const string StackedCode =
        """
        <ChartContainer Height="320" Class="w-full">
            <RadialBarChart Data="@_budgetData">
                <PolarGrid Stroke="var(--border)" />
                <XAxis DataKey="category" />
                <ChartTooltip />
                <Legend TextColor="var(--foreground)" />
                <RadialBar DataKey="spent"     Name="Spent"     Color="var(--chart-1)" StackId="budget" CornerRadius="0" />
                <RadialBar DataKey="remaining" Name="Remaining" Color="var(--chart-3)" StackId="budget" CornerRadius="4" />
            </RadialBarChart>
        </ChartContainer>

        @code {
            private record BudgetAllocation(string category, double spent, double remaining);
            private static readonly BudgetAllocation[] _budgetData =
            [
                new("Engineering", 74, 26),
                new("Marketing",   58, 42),
                new("Operations",  83, 17),
                new("Research",    41, 59),
            ];
        }
        """;
}
