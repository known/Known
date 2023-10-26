namespace WebSite.Docus.Inputs.DateRanges;

class DateRange1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KDateRange>("日期1：", "DateRange1").Build();
        builder.Field<KDateRange>("日期2：", "DateRange2").Value("2023-01-01~2023-01-31").Build();
    }
}