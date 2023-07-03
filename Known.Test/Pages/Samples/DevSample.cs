namespace Known.Test.Pages.Samples;

class DevSample : PageComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem("首页示例", "fa fa-home", typeof(DemoHome)),
        new MenuItem("列表示例", "fa fa-list", typeof(DemoDataList)),
        new MenuItem("表单示例", "fa fa-th-list", typeof(DemoForm)),
        new MenuItem("图表示例", "fa fa-bar-chart", typeof(DemoChart)),
        new MenuItem("其他示例", "fa fa-file-o", typeof(DemoOther))
    };

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>()
               .Set(c => c.Style, "box demo")
               .Set(c => c.Position, PositionType.Left)
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}