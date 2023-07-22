namespace Known.Test.Pages.Samples.Others;

/// <summary>
/// 导航类
/// </summary>
class DemoOther2 : BaseComponent
{
    //Breadcrumb、Pager、Steps、Tabs、Tree、Menu

    private readonly List<MenuItem> tabItems = new()
    {
        new MenuItem{Icon="fa fa-file-o",Name="Tab1"},
        new MenuItem{Icon="fa fa-file-o",Name="Tab2"}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildBreadcrumb(builder);
        BuildTabs(builder);
    }

    private void BuildBreadcrumb(RenderTreeBuilder builder)
    {
        builder.BuildDemo("面包屑", () =>
        {
            builder.Component<Breadcrumb>().Set(c => c.Items, new List<MenuItem>
            {
                new MenuItem("Test1", "测试1", "fa fa-home"),
                new MenuItem("Test2", "测试2", "fa fa-user"),
                new MenuItem("Test3", "测试3") {Action=()=>UI.Alert("Test")},
                new MenuItem("Test4", "测试4")
            }).Build();
        });
    }

    private void BuildTabs(RenderTreeBuilder builder)
    {
        builder.BuildDemo("选项卡", () =>
        {
            builder.Div("box", attr =>
            {
                attr.Style("height:150px;");
                builder.Component<Tabs>()
                       .Set(c => c.CurItem, tabItems[0])
                       .Set(c => c.Items, tabItems)
                       .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                       .Build();
            });
            builder.Div("box", attr =>
            {
                attr.Style("height:150px;");
                builder.Component<Tabs>()
                       .Set(c => c.CurItem, tabItems[0])
                       .Set(c => c.Items, tabItems)
                       .Set(c => c.Position, PositionType.Left)
                       .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                       .Build();
            });
            builder.Div("box", attr =>
            {
                attr.Style("height:150px;");
                builder.Component<Tabs>()
                       .Set(c => c.CurItem, tabItems[0])
                       .Set(c => c.Items, tabItems)
                       .Set(c => c.Position, PositionType.Right)
                       .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                       .Build();
            });
            builder.Div("box", attr =>
            {
                attr.Style("height:150px;");
                builder.Component<Tabs>()
                       .Set(c => c.CurItem, tabItems[0])
                       .Set(c => c.Items, tabItems)
                       .Set(c => c.Position, PositionType.Bottom)
                       .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                       .Build();
            });
        });
    }
}