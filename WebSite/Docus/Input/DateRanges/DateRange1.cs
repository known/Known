namespace WebSite.Docus.Input.DateRanges;

class DateRange1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<DateRange>("DateRange1").Build();
        builder.Field<DateRange>("DateRange2")
               .Set(f => f.Start, DateTime.Now)
               .Set(f => f.End, DateTime.Now.AddMonths(1))
               .Build();
    }
}