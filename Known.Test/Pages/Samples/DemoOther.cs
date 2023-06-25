namespace Known.Test.Pages.Samples;

class DemoOther : BaseComponent
{
    private readonly TabItem[] tabItems = new TabItem[]
    {
        new TabItem{Icon="fa fa-file-o",Title="Tab1",ChildContent=b => b.Span("Tab1 Content")},
        new TabItem{Icon="fa fa-file-o",Title="Tab2",ChildContent=b => b.Span("Tab2 Content")}
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
                builder.Component<Tabs>().Set(c => c.Items, tabItems).Build();
            });
            builder.Div("demo-box", attr =>
            {
                attr.Style("width:50%;");
                builder.Component<Tabs>().Set(c => c.Items, tabItems).Set(c => c.Position, "left").Build();
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