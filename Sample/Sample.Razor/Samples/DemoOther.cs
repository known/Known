using Sample.Razor.Samples.Others;

namespace Sample.Razor.Samples;

class DemoOther : BaseComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem{Icon="fa fa-circle-o",Name="输入类",ComType=typeof(DemoOther1)},
        new MenuItem{Icon="fa fa-circle-o",Name="导航类",ComType=typeof(DemoOther2)},
        new MenuItem{Icon="fa fa-circle-o",Name="展示类",ComType=typeof(DemoOther3)},
        new MenuItem{Icon="fa fa-circle-o",Name="反馈类",ComType=typeof(DemoOther4)}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>()
               .Set(c => c.Style, "other")
               .Set(c => c.Position, PositionType.Left)
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}