namespace WebSite.Docus.Inputs.DateRanges;

class DateRange1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<DateRange>("DateRange1").Build();
        builder.Field<DateRange>("DateRange2").Value("2023-01-01~2023-01-31").Build();
    }
}