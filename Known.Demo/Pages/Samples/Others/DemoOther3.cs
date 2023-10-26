using Known.Demo.Pages.Samples;

namespace Known.Demo.Pages.Samples.Others;

/// <summary>
/// 展示类
/// </summary>
class DemoOther3 : BaseComponent
{
    //Card、Carousel、Empty、Dropdown、GroupBox、ImageBox、Dialog、Chart、Form、QuickView、Badge、Icon、Timeline

    private readonly List<KTimelineItem> items1 = new()
    {
        new KTimelineItem{Title="第一标题",Description="第一节点内容"},
        new KTimelineItem{Title="第二标题",Description="第二节点内容"},
        new KTimelineItem{Title="第三标题",Description="第三节点内容"}
    };
    private readonly List<KTimelineItem> items2 = new()
    {
        new KTimelineItem{Title="审核中",Type=StyleType.Info},
        new KTimelineItem{Title="发布成功",Type=StyleType.Success},
        new KTimelineItem{Title="审核失败",Type=StyleType.Danger}
    };
    private readonly List<KTimelineItem> items3 = new()
    {
        new KTimelineItem{Title="第一标题",Description="第一节点内容"},
        new KTimelineItem{Template=BuildTimelineItem},
        new KTimelineItem{Title="第三标题",Description="第三节点内容"}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.BuildDemo("时间", () => builder.Component<KTimer>().Build());
        BuildCarousel(builder);
        BuildCard(builder);
        BuildBadge(builder);
        BuildTag(builder);
        BuildProgress(builder);
        BuildTimeline(builder);
    }

    private static void BuildCarousel(RenderTreeBuilder builder)
    {
        builder.BuildDemo("走马灯", () =>
        {
            builder.Div("box", attr =>
            {
                attr.Style("width:50%;height:100px;");
                builder.Component<KCarousel>().Build();
            });
        });
    }

    private static void BuildCard(RenderTreeBuilder builder)
    {
        builder.BuildDemo("卡片", () =>
        {
            builder.Div(attr =>
            {
                attr.Style("position:relative;height:100px;");
                builder.Component<KCard>().Set(c => c.Name, "Card1").Build();
            });
            builder.Div(attr =>
            {
                attr.Style("position:relative;height:100px;");
                builder.Component<KCard>().Set(c => c.Icon, "fa fa-list").Set(c => c.Name, "Card2").Build();
            });
        });
    }

    private static void BuildBadge(RenderTreeBuilder builder)
    {
        builder.BuildDemo("徽章", () =>
        {
            builder.Div(attr =>
            {
                BuildBadge(builder, StyleType.Default, "10");
                BuildBadge(builder, StyleType.Primary, "10");
                BuildBadge(builder, StyleType.Success, "10");
                BuildBadge(builder, StyleType.Info, "10");
                BuildBadge(builder, StyleType.Warning, "10");
                BuildBadge(builder, StyleType.Danger, "10");
            });
        });
    }

    private static void BuildBadge(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-badge", attr =>
        {
            builder.Text("消息中心");
            builder.Badge(style, text);
        });
    }

    private static void BuildTag(RenderTreeBuilder builder)
    {
        builder.BuildDemo("标签", () =>
        {
            builder.Div(attr =>
            {
                BuildTag(builder, StyleType.Default, "测试");
                BuildTag(builder, StyleType.Primary, "完成");
                BuildTag(builder, StyleType.Success, "通过");
                BuildTag(builder, StyleType.Info, "进行中");
                BuildTag(builder, StyleType.Warning, "警告");
                BuildTag(builder, StyleType.Danger, "失败");
                BuildTag(builder, StyleType.Success, b => b.IconName("fa fa-user", "模板"));
            });
        });
    }

    private static void BuildTag(RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, text));
    }

    private static void BuildTag(RenderTreeBuilder builder, StyleType style, Action<RenderTreeBuilder> content)
    {
        builder.Div("demo-tag", attr => builder.Tag(style, content));
    }

    private static void BuildProgress(RenderTreeBuilder builder)
    {
        builder.BuildDemo("进度条", () =>
        {
            builder.Div(attr =>
            {
                BuildProgress(builder, StyleType.Default, 0.5M);
                BuildProgress(builder, StyleType.Primary, 0.35M);
                BuildProgress(builder, StyleType.Success, 1);
                BuildProgress(builder, StyleType.Info, 0.6M);
                BuildProgress(builder, StyleType.Warning, 0.55M);
                BuildProgress(builder, StyleType.Danger, 0.8M);
            });
        });
    }

    private static void BuildProgress(RenderTreeBuilder builder, StyleType style, decimal value)
    {
        builder.Progress(style, value, 100);
    }

    private void BuildTimeline(RenderTreeBuilder builder)
    {
        builder.BuildDemo("时间轴", () =>
        {
            builder.Component<KTimeline>()
                    .Set(c => c.Items, items1)
                    .Build();
            builder.Component<KTimeline>()
                   .Set(c => c.Items, items2)
                   .Build();
            builder.Component<KTimeline>()
                   .Set(c => c.Items, items3)
                   .Build();
        });
    }

    private static void BuildTimelineItem(RenderTreeBuilder builder)
    {
        builder.Span("name", "自定义节点");
        builder.Span("time", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        builder.Span("text", "自定义模板内容");
        builder.Img("/img/login.jpg");
    }
}