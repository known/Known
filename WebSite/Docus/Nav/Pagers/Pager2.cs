namespace WebSite.Docus.Nav.Pagers;

class Pager2 : BaseComponent
{
    private KNumber? total;
    private PagingCriteria criteria = new(1);
    private int totalCount = 120;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("control", attr =>
        {
            builder.Label("form-label", "条数：");
            builder.Field<KNumber>("Total").Value("200").Set(f => f.Width, 100).Build(value => total = value);
            builder.Button("设置", Callback(OnSetTotal), StyleType.Primary);
        });

        builder.Component<KPager>()
               .Set(c => c.TotalCount, totalCount)
               .Set(c => c.PageIndex, criteria.PageIndex)
               .Set(c => c.PageSize, criteria.PageSize)
               .Set(c => c.OnPageChanged, OnPageChanged)
               .Build();
    }

    private void OnSetTotal() => totalCount = total.ValueAs<int>();

    private Task OnPageChanged(PagingCriteria criteria)
    {
        this.criteria = criteria;
        return Task.CompletedTask;
    }
}