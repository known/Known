using Known.Demo.Pages;
using Known.Demo.Pages.Samples.Homes;

namespace Known.Demo.Pages.Samples;

class DemoHome : BaseComponent
{
    private readonly List<KMenuItem> items = new()
    {
        new KMenuItem{Icon="fa fa-home",Name="首页",ComType=typeof(Home)},
        new KMenuItem{Icon="fa fa-home",Name="首页一",ComType=typeof(DemoHome1)},
        new KMenuItem{Icon="fa fa-home",Name="首页二",ComType=typeof(DemoHome2)},
        new KMenuItem{Icon="fa fa-home",Name="首页三",ComType=typeof(DemoHome3)},
        new KMenuItem{Icon="fa fa-home",Name="首页四",ComType=typeof(DemoHome4)},
        new KMenuItem{Icon="fa fa-home",Name="首页五",ComType=typeof(DemoHome5)}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTabs>()
               .Set(c => c.Position, PositionType.Left)
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}