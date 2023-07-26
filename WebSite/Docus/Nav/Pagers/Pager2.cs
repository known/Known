namespace WebSite.Docus.Nav.Pagers;

class Pager2 : BaseComponent
{
    private Number? total;
    private Pager? pager;
    private PagingCriteria criteria = new(1);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("control", attr =>
        {
            builder.Label("form-label", "条数：");
            builder.Field<Number>("Total").Value("200").Set(f => f.Width, 100).Build(value => total = value);
            builder.Button("设置", Callback(OnSetTotal), StyleType.Primary);
        });

        builder.Component<Pager>()
               .Set(c => c.TotalCount, 120)
               .Set(c => c.PageIndex, criteria.PageIndex)
               .Set(c => c.PageSize, criteria.PageSize)
               .Set(c => c.OnPageChanged, OnPageChanged)
               .Build(value => pager = value);
    }

    private void OnSetTotal() => pager?.SetTotalCount(total.ValueAs<int>());

    private Task OnPageChanged(PagingCriteria criteria)
    {
        this.criteria = criteria;
        return Task.CompletedTask;
    }
}