namespace WebSite.Docus.Nav.Pagers;

class Pager1 : BaseComponent
{
    private PagingCriteria criteria = new(1);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KPager>()
               .Set(c => c.TotalCount, 120)
               .Set(c => c.PageIndex, criteria.PageIndex)
               .Set(c => c.PageSize, criteria.PageSize)
               .Set(c => c.OnPageChanged, OnPageChanged)
               .Build();
    }

    private Task OnPageChanged(PagingCriteria criteria)
    {
        this.criteria = criteria;
        return Task.CompletedTask;
    }
}