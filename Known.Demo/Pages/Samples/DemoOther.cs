using Known.Demo.Pages.Samples.Others;

namespace Known.Demo.Pages.Samples;

class DemoOther : BaseComponent
{
    private readonly List<KMenuItem> items = new()
    {
        new KMenuItem{Icon="fa fa-circle-o",Name="输入类",ComType=typeof(DemoOther1)},
        new KMenuItem{Icon="fa fa-circle-o",Name="导航类",ComType=typeof(DemoOther2)},
        new KMenuItem{Icon="fa fa-circle-o",Name="展示类",ComType=typeof(DemoOther3)},
        new KMenuItem{Icon="fa fa-circle-o",Name="反馈类",ComType=typeof(DemoOther4)}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTabs>()
               .Set(c => c.Style, "other")
               .Set(c => c.Position, PositionType.Left)
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}