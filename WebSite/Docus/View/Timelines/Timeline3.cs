namespace WebSite.Docus.View.Timelines;

class Timeline3 : BaseComponent
{
    private readonly List<KTimelineItem> items = new()
    {
        new KTimelineItem { Title = "第一标题", Description = "第一节点内容"},
        new KTimelineItem { Template = BuildTimelineItem},
        new KTimelineItem { Title = "第三标题", Description = "第三节点内容"}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTimeline>().Set(c => c.Items, items).Build();
    }

    private static void BuildTimelineItem(RenderTreeBuilder builder)
    {
        builder.Span("name", "自定义节点");
        builder.Span("time", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        builder.Span("text", "自定义模板内容");
    }
}