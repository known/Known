namespace WebSite.Docus.View.Timelines;

class Timeline1 : BaseComponent
{
    private readonly List<KTimelineItem> items = new()
    {
        new KTimelineItem { Title = "第一标题", Description = "第一节点内容" },
        new KTimelineItem { Title = "第二标题", Description = "第二节点内容" },
        new KTimelineItem { Title = "第三标题", Description = "第三节点内容" }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTimeline>().Set(c => c.Items, items).Build();
    }
}