namespace NeoUI.Demo.Shared.Pages.Components.Filter;

partial class CustomControlsExample
{
    private static readonly string _customCode = """
        <FilterField Field="Rating" Label="Rating" Icon="star" Type="FilterFieldType.Custom">
            <ChildContent Context="ctx">
                <span class="px-2 flex items-center self-stretch">
                    <Rating Value="@GetRatingValue(ctx.Condition)"
                            ValueChanged="@(v => SetRating(ctx, (int)v))"
                            MaxRating="5"
                            AllowClear="true"
                            Size="RatingSize.Small" />
                </span>
            </ChildContent>
        </FilterField>
        """;

    private static readonly string _usageCode = """
        <FilterField Field="MyField"
                     Label="My Custom Field"
                     Type="FilterFieldType.Custom">
            <ChildContent Context="ctx">
                @* 'ctx.Condition' is the live FilterCondition.
                   Set ctx.Condition.Value, then call ctx.NotifyChanged() to propagate. *@
                <MyCustomInput Value="@ctx.Condition.Value"
                               ValueChanged="@(v => { ctx.Condition.Value = v; _ = ctx.NotifyChanged(); })" />
            </ChildContent>
        </FilterField>
        """;
}
