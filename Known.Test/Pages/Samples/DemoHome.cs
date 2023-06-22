using Known.Test.Pages.Samples.Homes;

namespace Known.Test.Pages.Samples;

class DemoHome : BaseComponent
{
    private readonly TabItem[] items = new TabItem[]
    {
        new TabItem{Icon="fa fa-home",Title="首页",ChildContent=b => b.Component<Home>().Build()},
        new TabItem{Icon="fa fa-home",Title="首页一",ChildContent=b => b.Component<DemoHome1>().Build()},
        new TabItem{Icon="fa fa-home",Title="首页二",ChildContent=b => b.Component<DemoHome2>().Build()},
        new TabItem{Icon="fa fa-home",Title="首页三",ChildContent=b => b.Component<DemoHome3>().Build()},
        new TabItem{Icon="fa fa-home",Title="首页四",ChildContent=b => b.Component<DemoHome4>().Build()},
        new TabItem{Icon="fa fa-home",Title="首页五",ChildContent=b => b.Component<DemoHome5>().Build()}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>().Set(c => c.Items, items).Build();
    }
}