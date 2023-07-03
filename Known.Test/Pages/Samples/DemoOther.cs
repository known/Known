namespace Known.Test.Pages.Samples;

class DemoOther : BaseComponent
{
    private readonly List<MenuItem> tabItems = new List<MenuItem>
    {
        new MenuItem{Icon="fa fa-file-o",Name="Tab1"},
        new MenuItem{Icon="fa fa-file-o",Name="Tab2"}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildBreadcrumb(builder);
        builder.Div("row", attr =>
        {
            BuildDemo(builder, "时间", () => builder.Component<Razor.Components.Timer>().Build());
            BuildDemo(builder, "搜索框", () => builder.Component<SearchBox>().Build());
            BuildDemo(builder, "验证码", () => builder.Component<Captcha>().Build());
        });
        BuildCarousel(builder);
        BuildCard(builder);
        BuildTabs(builder);
    }

    private void BuildBreadcrumb(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "面包屑", () =>
        {
            builder.Component<Breadcrumb>().Set(c => c.Items, new List<MenuItem>
            {
                new MenuItem("Test1", "测试1", "fa fa-user"),
                new MenuItem("Test2", "测试2"),
                new MenuItem("Test3", "测试3") {Action=()=>UI.Alert("Test")},
                new MenuItem("Test4", "测试4")
            }).Build();
        });
    }

    private static void BuildCarousel(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "走马灯", () =>
        {
            builder.Div("demo-box", attr =>
            {
                attr.Style("width:50%;height:300px;");
                builder.Component<Carousel>().Build();
            });
        });
    }

    private static void BuildCard(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "卡片", () =>
        {
            builder.Div("demo-box", attr =>
            {
                attr.Style("width:50%;");
                builder.Component<Card>().Set(c => c.Name, "Card1").Build();
            });
            builder.Div("demo-box", attr =>
            {
                attr.Style("width:50%;");
                builder.Component<Card>().Set(c => c.Icon, "fa fa-list").Set(c => c.Name, "Card2").Build();
            });
        });
    }

    private void BuildTabs(RenderTreeBuilder builder)
    {
        BuildDemo(builder, "选项卡", () =>
        {
            builder.Div("demo-box", attr =>
            {
                attr.Style("width:50%;");
                builder.Component<Tabs>()
                       .Set(c => c.CurItem, tabItems[0])
                       .Set(c => c.Items, tabItems)
                       .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                       .Build();
            });
            builder.Div("demo-box", attr =>
            {
                attr.Style("width:50%;");
                builder.Component<Tabs>()
                       .Set(c => c.CurItem, tabItems[0])
                       .Set(c => c.Items, tabItems)
                       .Set(c => c.Position, PositionType.Left)
                       .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
                       .Build();
            });
        });
    }

    private static void BuildDemo(RenderTreeBuilder builder, string text, Action action)
    {
        builder.Div(attr =>
        {
            builder.Div("demo-caption", text);
            action();
        });
    }
}