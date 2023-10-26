namespace WebSite.Docus.View.Timelines;

class Timeline2 : BaseComponent
{
    private readonly List<KTimelineItem> items = new()
    {
        new KTimelineItem { Title = "审核中", Type = StyleType.Info },
        new KTimelineItem { Title = "发布成功", Type = StyleType.Success },
        new KTimelineItem { Title = "审核失败", Type = StyleType.Danger }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTimeline>().Set(c => c.Items, items).Build();
    }
}