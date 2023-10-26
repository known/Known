namespace Known.Demo.Pages.Samples;

class DevSample : PageComponent
{
    private readonly List<KMenuItem> items = new()
    {
        new KMenuItem("首页示例", "fa fa-home", typeof(DemoHome)),
        new KMenuItem("列表示例", "fa fa-list", typeof(DemoDataList)),
        new KMenuItem("表单示例", "fa fa-th-list", typeof(DemoForm)),
        new KMenuItem("图表示例", "fa fa-bar-chart", typeof(DemoChart)),
        new KMenuItem("其他示例", "fa fa-file-o", typeof(DemoOther))
    };

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<KTabs>()
               .Set(c => c.Style, "box demo")
               .Set(c => c.Position, PositionType.Left)
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}